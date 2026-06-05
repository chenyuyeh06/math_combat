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
        }

        /// <summary>收到 GAME_OVER：顯示最終結果</summary>
        public void OnGameOver(GameOverMsg m)
        {
            StopRoundTimer();

            string finalResult = m.result == "WIN"  ? "🏆 你贏了！" :
                                 m.result == "LOSE" ? "💀 你輸了！" : "🤝 平局！";

            string msg = $"{finalResult}\n\n最終比數　{GetP1Name()} {m.wins1} : {m.wins2} {GetP2Name()}";

            var dlg = MessageBox.Show(msg + "\n\n回到大廳？", "遊戲結束",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dlg == DialogResult.Yes)
            {
                GameUnits.SendReturnRoom();
                GameUnits.SwitchToForm(this, roomPage);
            }
            else
            {
                GameUnits.SendLeaveAfterGame();
                GameUnits.SwitchToForm(this, homePage);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  發牌
        // ══════════════════════════════════════════════════════════════════════

        private void DealCardsFromServer(List<string> cardValues)
        {
            tableLayout_HandCards.SuspendLayout();
            tableLayout_HandCards.Controls.Clear();

            var hand = GameUnits.BuildHandFromCards(cardValues);
            for (int i = 0; i < hand.Count; i++)
            {
                var card = hand[i];
                var pb = new PictureBox
                {
                    Image    = card.CardImage,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock     = DockStyle.Fill,
                    Tag      = card,
                    Cursor   = Cursors.Hand,
                };
                pb.Click += HandCard_Click;
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
                    Cursor    = Cursors.Hand,
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
            bool hasCard = _boardSlots.Any(s => s != null && s.Tag is GameUnits.Card);
            check_card.Visible  = hasCard && !_scoreSubmitted;
            cancel_card.Visible = hasCard && !_scoreSubmitted;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  確認出牌 → 計算 → 送分
        // ══════════════════════════════════════════════════════════════════════

        private void check_card_Click(object sender, EventArgs e)
        {
            if (_scoreSubmitted) return;

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
            double result;
            try
            {
                var raw = new DataTable().Compute(expression, null);
                result = Convert.ToDouble(raw);
                if (double.IsInfinity(result) || double.IsNaN(result))
                {
                    MessageBox.Show("不能除以零", "計算錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch
            {
                MessageBox.Show($"無法計算算式：{expression}", "計算錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string resultStr = (result == Math.Floor(result))
                ? ((int)result).ToString()
                : result.ToString("0.##");

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
                    if (!_scoreSubmitted)
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

        private string GetP1Name() => GameUnits.isHost ? GameUnits.player_name : GameUnits.hostName;
        private string GetP2Name() => GameUnits.isHost ? GameUnits.player2Name : GameUnits.player_name;

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