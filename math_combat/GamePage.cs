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
        private HomePage homePage;
        private RoomPage roomPage => GameUnits.roomPage;
        private PictureBox[] _boardSlots = new PictureBox[5];

        public GamePage(HomePage homePage)
        {
            InitializeComponent();
            this.homePage = GameUnits.homePage;
        }

        private void GamePage_Load(object sender, EventArgs e)
        {
            DealCards();
            UpdateActionButtons();
           
        }
        

        private void DealCards()
        {
            // ── 手牌 ──
            var hand = GameUnits.CreateGameHand();
            for (int i = 0; i < hand.Count; i++)
            {
                var card = hand[i];
                var pb = new PictureBox
                {
                    Image = card.CardImage,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill,
                    Tag = card,
                    Cursor = Cursors.Hand,
                };
                pb.Click += HandCard_Click;
                tableLayout_HandCards.Controls.Add(pb, i, 0);
            }

            // ── Board 槽（5 個，對應 5 個 column）──
            tableLayout_BoardCards.SuspendLayout();
            tableLayout_BoardCards.Controls.Clear();
            tableLayout_BoardCards.RowCount = 1;
            tableLayout_BoardCards.ColumnCount = 5;
            tableLayout_BoardCards.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayout_BoardCards.BringToFront();

            for (int i = 0; i < 5; i++)
            {
                var slot = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill,
                    Tag = null,
                    BackColor = Color.WhiteSmoke,
                    Cursor = Cursors.Hand,
                };
                _boardSlots[i] = slot;
                tableLayout_BoardCards.Controls.Add(slot, i, 0);
            }
            tableLayout_BoardCards.Visible = true;
            tableLayout_BoardCards.ResumeLayout();
        }

        // ── 點手牌直接依序填入 Board ──
        private void HandCard_Click(object sender, EventArgs e)
        {
            var pb = (PictureBox)sender;
            if (pb.Tag == null || !pb.Enabled) return;

            var card = (GameUnits.Card)pb.Tag;

            // 找下一個空的 Board 槽
            PictureBox emptySlot = null;
            for (int i = 0; i < _boardSlots.Length; i++)
            {
                if (_boardSlots[i] != null && _boardSlots[i].Tag == null)
                {
                    emptySlot = _boardSlots[i];
                    break;
                }
            }

            if (emptySlot == null) return; // Board 已滿

            // 放到 Board
            emptySlot.Image = card.CardImage;
            emptySlot.Tag = card;
            emptySlot.BackColor = Color.Transparent;

            // 手牌灰掉禁用
            pb.Image = Properties.Resources.card_background; // 顯示背面圖
            pb.Tag = null;
            pb.BackColor = Color.FromArgb(50, 0, 0, 0);
            pb.Enabled = false;

            UpdateActionButtons();
        }

        // ── 退回手牌 ──
        private void ReturnCardToHand(GameUnits.Card card)
        {
            foreach (Control ctrl in tableLayout_HandCards.Controls)
            {
                if (ctrl is PictureBox pb && pb.Tag == null)
                {
                    pb.Image = card.CardImage;
                    pb.Tag = card;
                    pb.BackColor = Color.Transparent;
                    pb.Enabled = true;
                    pb.Click -= HandCard_Click;
                    pb.Click += HandCard_Click;
                    break;
                }
            }
        }

        // ── 按鈕顯示控制 ──
        private void UpdateActionButtons()
        {
            bool hasCard = _boardSlots.Any(s => s != null && s.Tag is GameUnits.Card);
            check_card.Visible = hasCard;
            cancel_card.Visible = hasCard;
        }

        // ── 確認出牌 ──
        private void check_card_Click(object sender, EventArgs e)
        {
            var played = _boardSlots
                .Where(s => s != null && s.Tag is GameUnits.Card)
                .Select(s => (GameUnits.Card)s.Tag)
                .ToList();

            if (played.Count == 0) return;

            // ── 單張數字牌直接當結果 ──
            if (played.Count == 1)
            {
                if (played[0].Type == GameUnits.CardType.Number)
                {
                    MessageBox.Show($"結果：{played[0].Value}", "出牌結果");
                    return;
                }
                else
                {
                    MessageBox.Show("單張牌必須是數字牌", "出牌錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // ── 檢查算式結構：必須是「數字 運算子 數字 運算子 數字...」交錯 ──
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
                    MessageBox.Show($"第 {i + 1} 張應該是運算子牌（+  -  ×  ÷）", "算式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // 最後一張不能是運算子
            if (played.Last().Type == GameUnits.CardType.Operator)
            {
                MessageBox.Show("算式不能以運算子結尾", "算式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── 計算 ──
            string expression = string.Join(" ", played.Select(c => c.Value));
            try
            {
                var rawResult = new DataTable().Compute(expression, null);
                double result = Convert.ToDouble(rawResult);

                // 除法檢查（DataTable 會回傳 Infinity 而不是例外）
                if (double.IsInfinity(result) || double.IsNaN(result))
                {
                    MessageBox.Show("不能除以零", "計算錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 結果是整數就不顯示小數點
                string resultStr = (result == Math.Floor(result))
                    ? ((int)result).ToString()
                    : result.ToString("0.##");

                MessageBox.Show($"{expression} = {resultStr}", "出牌結果");
            }
            catch
            {
                MessageBox.Show($"無法計算算式：{expression}", "計算錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── 取消出牌，全部退回手牌 ──
        private void cancel_card_Click(object sender, EventArgs e)
        {
            foreach (var slot in _boardSlots)
            {
                if (slot == null) continue;
                if (slot.Tag is GameUnits.Card card)
                {
                    ReturnCardToHand(card);
                    slot.Image = null;
                    slot.Tag = null;
                    slot.BackColor = Color.FromArgb(30, 255, 255, 255);
                }
            }
            UpdateActionButtons();
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void tableLayoutPanel2_Resize(object sender, EventArgs e) { }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
        private void BoardCard_Click(object sender, MouseEventArgs e) { }
        private void pictureBox10_Click(object sender, EventArgs e) { }

        private void GamePage_FormClosed(object sender, FormClosedEventArgs e)
        {
            DialogResult result = MessageBox.Show("確定要離開對戰嗎？", "離開對戰",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                GameUnits.SwitchToForm(this, roomPage);
        }
    }
}