using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace math_combat
{
    public partial class GamePage : Form
    {
        private HomePage homePage => GameUnits.homePage;
        private RoomPage roomPage => GameUnits.roomPage;

        private PictureBox[] _boardSlots = new PictureBox[5];

        // 本回合資訊
        private int    _currentRound   = 0;
        private int    _roundSeconds    = 30;
        private bool   _scoreSubmitted  = false;
        private System.Windows.Forms.Timer _roundTimer;

        public GamePage(HomePage homePage)
        {
            InitializeComponent();
            this.FormClosing += GamePage_FormClosing;
        }

        private void GamePage_Load(object sender, EventArgs e)
        {
            // 等 Server 送 ROUND_START 才真正發牌
            SetupBoardSlots();
            UpdateActionButtons();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Server 事件回呼（由 GameUnits 在 UI 執行緒上觸發）
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>收到 ROUND_START：發牌並啟動計時器</summary>
        public void OnRoundStart(int roundNum, int secCount, List<string> cardValues)
        {
            _currentRound   = roundNum;
            _roundSeconds   = secCount;
            _scoreSubmitted = false;

            // 更新回合顯示
            if (round != null)
                round.Text = $"回合 {roundNum} / {GameUnits.rounds}";

            // 顯示分數板
            UpdateScoreBoard();

            // 發牌（使用 Server 下發的牌）
            DealCardsFromServer(cardValues);
            UpdateActionButtons();

            // 啟動倒數計時
            StartRoundTimer(secCount);
        }

        /// <summary>收到 ROUND_RESULT：顯示結算</summary>
        public void OnRoundResult(RoundResultMsg m)
        {
            StopRoundTimer();

            string resultText = m.result == "WIN"  ? "🏆 本回合獲勝！" :
                                m.result == "LOSE" ? "💀 本回合落敗！" : "🤝 平手！";

            string msg = $"{resultText}\n\n你的得分：{m.yourScore}\n對手得分：{m.opponentScore}\n\n" +
                         $"目前比數　{GetP1Name()} {m.wins1} : {m.wins2} {GetP2Name()}";

            MessageBox.Show(msg, $"第 {m.round} 回合結果", MessageBoxButtons.OK, MessageBoxIcon.Information);

            GameUnits.wins1 = m.wins1;
            GameUnits.wins2 = m.wins2;
            UpdateScoreBoard();

            // 檢查是否已達到贏得對戰的勝場門檻 (過半勝出)
            int threshold = (GameUnits.rounds / 2) + 1;
            if (GameUnits.wins1 >= threshold || GameUnits.wins2 >= threshold)
            {
                // 提前結算！建立一個 GameOverMsg 物件來觸發 OnGameOver
                var gameOverMsg = new GameOverMsg
                {
                    wins1 = GameUnits.wins1,
                    wins2 = GameUnits.wins2,
                    result = (GameUnits.wins1 == GameUnits.wins2) ? "DRAW" :
                             ((GameUnits.isSpectator || GameUnits.isHost) 
                               ? (GameUnits.wins1 > GameUnits.wins2 ? "WIN" : "LOSE")
                               : (GameUnits.wins2 > GameUnits.wins1 ? "WIN" : "LOSE"))
                };
                OnGameOver(gameOverMsg);
            }
        }

        /// <summary>收到 GAME_OVER：顯示最終結果</summary>
        public void OnGameOver(GameOverMsg m)
        {
            if (GameUnits.currentPage == GameUnits.PageState.Result) return;

            StopRoundTimer();

            bool isDraw = (m.wins1 == m.wins2);
            string winnerName = "";
            if (!isDraw)
            {
                winnerName = (m.wins1 > m.wins2) ? GetP1Name() : GetP2Name();
            }

            // 使用專屬的結算頁面顯示最終結果
            var resultPage = GameUnits.resultPage;
            if (resultPage != null)
            {
                resultPage.SetupResult(GameUnits.room_number, winnerName, isDraw);
                GameUnits.SwitchToForm(this, resultPage);
            }
            else
            {
                // 後備防禦邏輯，避免沒有結算頁面時出錯
                GameUnits.SendReturnRoom();
                GameUnits.SwitchToForm(this, roomPage);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  發牌
        // ══════════════════════════════════════════════════════════════════════

        private void DealCardsFromServer(List<string> cardValues)
        {
            tableLayout_HandCards.SuspendLayout();
            tableLayout_HandCards.Controls.Clear();

            // 動態配置 TableLayoutPanel 以容納任意數量的卡牌並避免版面跑掉
            var hand = GameUnits.BuildHandFromCards(cardValues);
            tableLayout_HandCards.ColumnCount = Math.Max(1, hand.Count);
            tableLayout_HandCards.ColumnStyles.Clear();
            for (int i = 0; i < tableLayout_HandCards.ColumnCount; i++)
            {
                tableLayout_HandCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / tableLayout_HandCards.ColumnCount));
            }

            tableLayout_HandCards.RowCount = 1;
            tableLayout_HandCards.RowStyles.Clear();
            tableLayout_HandCards.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            for (int i = 0; i < hand.Count; i++)
            {
                var card = hand[i];
                var pb = new PictureBox
                {
                    Image    = card.CardImage,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock     = DockStyle.Fill,
                    Tag      = card,
                    Cursor   = GameUnits.isSpectator ? Cursors.Default : Cursors.Hand,
                };
                if (!GameUnits.isSpectator)
                {
                    pb.Click += HandCard_Click;
                }
                tableLayout_HandCards.Controls.Add(pb, i, 0);
            }
            tableLayout_HandCards.ResumeLayout();

            SetupBoardSlots();
        }

        private void SetupBoardSlots()
        {
            tableLayout_BoardCards.SuspendLayout();
            tableLayout_BoardCards.Controls.Clear();
            tableLayout_BoardCards.RowCount    = 1;
            tableLayout_BoardCards.ColumnCount = 5;
            tableLayout_BoardCards.GrowStyle   = TableLayoutPanelGrowStyle.FixedSize;

            for (int i = 0; i < 5; i++)
            {
                var slot = new PictureBox
                {
                    SizeMode  = PictureBoxSizeMode.Zoom,
                    Dock      = DockStyle.Fill,
                    Tag       = null,
                    BackColor = Color.WhiteSmoke,
                    Cursor    = GameUnits.isSpectator ? Cursors.Default : Cursors.Hand,
                };
                _boardSlots[i] = slot;
                tableLayout_BoardCards.Controls.Add(slot, i, 0);
            }
            tableLayout_BoardCards.Visible = true;
            tableLayout_BoardCards.ResumeLayout();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  卡牌操作
        // ══════════════════════════════════════════════════════════════════════

        private void HandCard_Click(object sender, EventArgs e)
        {
            if (_scoreSubmitted) return;

            var pb = (PictureBox)sender;
            if (pb.Tag == null || !pb.Enabled) return;

            var card = (GameUnits.Card)pb.Tag;

            PictureBox emptySlot = null;
            for (int i = 0; i < _boardSlots.Length; i++)
            {
                if (_boardSlots[i] != null && _boardSlots[i].Tag == null)
                {
                    emptySlot = _boardSlots[i];
                    break;
                }
            }
            if (emptySlot == null) return;

            emptySlot.Image     = card.CardImage;
            emptySlot.Tag       = card;
            emptySlot.BackColor = Color.Transparent;

            pb.Image    = Properties.Resources.card_background;
            pb.Tag      = null;
            pb.BackColor = Color.FromArgb(50, 0, 0, 0);
            pb.Enabled  = false;

            UpdateActionButtons();
        }

        private void ReturnCardToHand(GameUnits.Card card)
        {
            foreach (Control ctrl in tableLayout_HandCards.Controls)
            {
                if (ctrl is PictureBox pb && pb.Tag == null)
                {
                    pb.Image     = card.CardImage;
                    pb.Tag       = card;
                    pb.BackColor = Color.Transparent;
                    pb.Enabled   = true;
                    pb.Click    -= HandCard_Click;
                    pb.Click    += HandCard_Click;
                    break;
                }
            }
        }

        private void UpdateActionButtons()
        {
            if (GameUnits.isSpectator)
            {
                check_card.Visible  = false;
                cancel_card.Visible = false;
                return;
            }

            bool hasCard = _boardSlots.Any(s => s != null && s.Tag is GameUnits.Card);
            check_card.Visible  = hasCard && !_scoreSubmitted;
            cancel_card.Visible = hasCard && !_scoreSubmitted;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  確認出牌 → 計算 → 送分
        // ══════════════════════════════════════════════════════════════════════

        private void check_card_Click(object sender, EventArgs e)
        {
            if (_scoreSubmitted || GameUnits.isSpectator) return;

            var played = _boardSlots
                .Where(s => s != null && s.Tag is GameUnits.Card)
                .Select(s => (GameUnits.Card)s.Tag)
                .ToList();

            if (played.Count == 0) return;

            // 單張必須是數字牌
            if (played.Count == 1)
            {
                if (played[0].Type != GameUnits.CardType.Number)
                {
                    MessageBox.Show("單張牌必須是數字牌", "出牌錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                // 交錯檢查：數字 運算子 數字 ...
                for (int i = 0; i < played.Count; i++)
                {
                    bool shouldBeNumber = (i % 2 == 0);
                    bool isNumber = played[i].Type == GameUnits.CardType.Number;
                    if (shouldBeNumber && !isNumber)
                    {
                        MessageBox.Show($"第 {i + 1} 張應該是數字牌", "算式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (!shouldBeNumber && isNumber)
                    {
                        MessageBox.Show($"第 {i + 1} 張應該是運算子牌（+ - × ÷）", "算式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (played.Last().Type == GameUnits.CardType.Operator)
                {
                    MessageBox.Show("算式不能以運算子結尾", "算式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // 計算
            string expression = string.Join(" ", played.Select(c => c.Value));
            // 為了在 DataTable.Compute 中強制進行浮點數除法，將數字都加上 .0
            string evalExpression = string.Join(" ", played.Select(c => 
                c.Type == GameUnits.CardType.Number ? c.Value + ".0" : c.Value));

            double result;
            try
            {
                var raw = new DataTable().Compute(evalExpression, null);
                result = Convert.ToDouble(raw);
                if (double.IsInfinity(result) || double.IsNaN(result))
                {
                    result = -999;
                }
            }
            catch
            {
                result = -999;
            }

            string resultStr = (result == -999) ? "計算錯誤 (-999)" :
                               ((result == Math.Floor(result)) ? ((int)result).ToString() : result.ToString("0.##"));

            MessageBox.Show($"{expression} = {resultStr}", "出牌結果");

            // 送分給 Server
            _scoreSubmitted = true;
            StopRoundTimer();
            GameUnits.SendScore(_currentRound, result);
            UpdateActionButtons();
        }

        private void cancel_card_Click(object sender, EventArgs e)
        {
            if (_scoreSubmitted) return;
            foreach (var slot in _boardSlots)
            {
                if (slot == null) continue;
                if (slot.Tag is GameUnits.Card card)
                {
                    ReturnCardToHand(card);
                    slot.Image     = null;
                    slot.Tag       = null;
                    slot.BackColor = Color.FromArgb(30, 255, 255, 255);
                }
            }
            UpdateActionButtons();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  倒數計時器
        // ══════════════════════════════════════════════════════════════════════

        private int _timerSeconds;

        private void StartRoundTimer(int seconds)
        {
            StopRoundTimer();
            _timerSeconds = seconds;
            UpdateTimerLabel();

            _roundTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _roundTimer.Tick += (s, e) =>
            {
                _timerSeconds--;
                UpdateTimerLabel();
                if (_timerSeconds <= 0)
                {
                    StopRoundTimer();
                    if (!_scoreSubmitted && !GameUnits.isSpectator)
                    {
                        // 時間到，送 -999 表示放棄
                        _scoreSubmitted = true;
                        GameUnits.SendScore(_currentRound, -999);
                        UpdateActionButtons();
                    }
                }
            };
            _roundTimer.Start();
        }

        private void StopRoundTimer()
        {
            if (_roundTimer != null)
            {
                _roundTimer.Stop();
                _roundTimer.Dispose();
                _roundTimer = null;
            }
        }

        private void UpdateTimerLabel()
        {
            if (seconds != null)
                seconds.Text = $"剩餘：{_timerSeconds}s";
        }

        // ══════════════════════════════════════════════════════════════════════
        //  分數板
        // ══════════════════════════════════════════════════════════════════════

        private void UpdateScoreBoard()
        {
            if (player1 != null)
                player1.Text = $"{GetP1Name()} 勝 {GameUnits.wins1}";
            if (player2 != null)
                player2.Text = $"{GetP2Name()} 勝 {GameUnits.wins2}";
        }

        private string GetP1Name() => GameUnits.isSpectator ? GameUnits.hostName :
                                      (GameUnits.isHost ? GameUnits.player_name : GameUnits.hostName);

        private string GetP2Name() => GameUnits.isSpectator ? GameUnits.player2Name :
                                      (GameUnits.isHost ? GameUnits.player2Name : GameUnits.player_name);

        // ══════════════════════════════════════════════════════════════════════
        //  視窗關閉
        // ══════════════════════════════════════════════════════════════════════

        private void GamePage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.Visible) return;
            if (e.CloseReason == CloseReason.ApplicationExitCall) return;

            var result = MessageBox.Show("確定要離開對戰嗎？", "離開對戰",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                StopRoundTimer();
                GameUnits.SendLeaveAndCleanup();
                GameUnits.SwitchToForm(this, homePage);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void GamePage_FormClosed(object sender, FormClosedEventArgs e) { }

        // ── 保留原有空事件 ──────────────────────────────────────────────────────
        private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void tableLayoutPanel2_Resize(object sender, EventArgs e) { }
        private void tableLayoutPanel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void BoardCard_Click(object sender, System.Windows.Forms.MouseEventArgs e) { }
        private void pictureBox10_Click(object sender, EventArgs e) { }
    }
}