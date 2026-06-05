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
    public partial class RoomPage : Form
    {
        private HomePage   homePage   => GameUnits.homePage;
        private GamePage   gamePage   => GameUnits.gamePage;
        private ResultPage resultPage => GameUnits.resultPage;

        public RoomPage(HomePage homePage)
        {
            InitializeComponent();
            this.VisibleChanged += RoomPage_VisibleChanged;
        }

        private void RoomPage_Load(object sender, EventArgs e)
        {
            // 圓角
            GameUnits.MakeRoundedControl(pictureBox1, 15);
            GameUnits.MakeRoundedControl(start_game_button, 15);
            GameUnits.MakeRoundedControl(game_set_bg, 15);
            GameUnits.MakeRoundedControl(player1_bg, 15);
            GameUnits.MakeRoundedControl(player2_bg, 15);
            GameUnits.MakeRoundedControl(panel_guest_list, 15);

            // hover color for fonts
            GameUnits.MakeFancyControl(start_game_button, 15, Color.Black, Color.WhiteSmoke);
            GameUnits.MakeFancyControl(round_sub,  15, Color.Black, Color.WhiteSmoke);
            GameUnits.MakeFancyControl(round_plus, 15, Color.Black, Color.WhiteSmoke);
            GameUnits.MakeFancyControl(sec_sub,    15, Color.Black, Color.WhiteSmoke);
            GameUnits.MakeFancyControl(sec_plus,   15, Color.Black, Color.WhiteSmoke);
        }

        private void RoomPage_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible) return;

            room_number.Text = "房間號碼：" + GameUnits.room_number;

            // 重置準備狀態
            GameUnits.hostReady   = false;
            GameUnits.clientReady = false;

            RefreshRoomInfo();
        }

        // ── 準備 / 開始按鈕 ────────────────────────────────────────────────────

        private void start_game_button_Click(object sender, EventArgs e)
        {
            if (GameUnits.isSpectator) return;

            if (GameUnits.isHost)
            {
                // Player1（房主）送 START_GAME
                GameUnits.hostReady = true;
                GameUnits.SendStartGame();
                UpdateButtonState();
            }
            else
            {
                // Player2 切換準備狀態
                GameUnits.ToggleClientReady();
                UpdateButtonState();
            }
        }

        // ── 房間設定按鈕（只有房主可調整）─────────────────────────────────────

        private void round_plus_Click(object sender, EventArgs e)
        {
            if (!GameUnits.isHost) return;
            if (GameUnits.rounds < 5)
            {
                GameUnits.rounds = GameUnits.rounds + 1;
                round_set.Text = GameUnits.rounds.ToString();
                GameUnits.SendUpdateRoom();
            }
        }

        private void round_sub_Click(object sender, EventArgs e)
        {
            if (!GameUnits.isHost) return;
            if (GameUnits.rounds > 1)
            {
                GameUnits.rounds = GameUnits.rounds - 1;
                round_set.Text = GameUnits.rounds.ToString();
                GameUnits.SendUpdateRoom();
            }
        }

        private void sec_plus_Click(object sender, EventArgs e)
        {
            if (!GameUnits.isHost) return;
            if (GameUnits.secs < 60)
            {
                GameUnits.secs = GameUnits.secs + 5;
                sec_set.Text = GameUnits.secs.ToString();
                GameUnits.SendUpdateRoom();
            }
        }

        private void sec_sub_Click(object sender, EventArgs e)
        {
            if (!GameUnits.isHost) return;
            if (GameUnits.secs > 5)
            {
                GameUnits.secs = GameUnits.secs - 5;
                sec_set.Text = GameUnits.secs.ToString();
                GameUnits.SendUpdateRoom();
            }
        }

        // ── 返回首頁 ───────────────────────────────────────────────────────────

        private void back_to_home_page_Click(object sender, EventArgs e)
        {
            GameUnits.SendLeaveAndCleanup();
            GameUnits.SwitchToForm(this, homePage);
        }

        private void RoomPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            GameUnits.SendLeaveAndCleanup();
            GameUnits.SwitchToForm(this, homePage);
        }

        private void RoomPage_FormClosed(object sender, FormClosedEventArgs e) { }

        // ── 公開 UI 更新方法（由 GameUnits 觸發）────────────────────────────────

        /// <summary>GameUnits 收到 ROOM_INFO / READY_STATE / ROOM_UPDATED 後呼叫此方法</summary>
        public void RefreshRoomInfo()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(RefreshRoomInfo));
                return;
            }

            // ── 玩家名稱 ──
            if (GameUnits.isHost)
            {
                player1_name.Text = GameUnits.player_name;
                player2_name.Text = string.IsNullOrEmpty(GameUnits.player2Name)
                    ? "等候對手加入..." : GameUnits.player2Name;
            }
            else
            {
                player1_name.Text = string.IsNullOrEmpty(GameUnits.hostName)
                    ? "房主" : GameUnits.hostName;
                player2_name.Text = GameUnits.player_name;
            }

            // ── 房間設定顯示 ──
            round_set.Text = GameUnits.rounds.ToString();
            sec_set.Text   = GameUnits.secs.ToString();

            // ── 觀戰者列表 ──
            Label[] spectatorLabels = new Label[] { label3, label4, label5, label6, label7, label8 };
            var specs = GameUnits.spectatorNames;
            for (int i = 0; i < spectatorLabels.Length; i++)
            {
                if (i < specs.Count && !string.IsNullOrEmpty(specs[i]))
                {
                    spectatorLabels[i].Text    = specs[i];
                    spectatorLabels[i].Visible = true;
                }
                else
                {
                    spectatorLabels[i].Text    = "";
                    spectatorLabels[i].Visible = false;
                }
            }

            UpdateButtonState();
        }

        public void UpdateButtonState()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(UpdateButtonState));
                return;
            }

            if (GameUnits.isSpectator)
            {
                start_game_button.Text    = "觀戰中...";
                start_game_button.Enabled = false;
                return;
            }

            if (GameUnits.isHost)
            {
                bool hasPlayer2 = !string.IsNullOrEmpty(GameUnits.player2Name);
                if (!hasPlayer2)
                {
                    start_game_button.Text    = "等候對手...";
                    start_game_button.Enabled = false;
                }
                else if (GameUnits.hostReady)
                {
                    start_game_button.Text    = "已送出(等待對手)";
                    start_game_button.Enabled = false;
                }
                else
                {
                    start_game_button.Text    = "開始遊戲";
                    start_game_button.Enabled = true;
                }
            }
            else
            {
                // Player2
                if (GameUnits.clientReady)
                {
                    start_game_button.Text    = "已準備(等待開始)";
                    start_game_button.Enabled = true;
                }
                else
                {
                    start_game_button.Text    = "準備對戰";
                    start_game_button.Enabled = true;
                }
            }
        }

        // ── 舊有相容保留 ────────────────────────────────────────────────────────
        public void UpdateNames()         => RefreshRoomInfo();
        public void UpdateNamesFromInfo(string p1, List<string> guests) => RefreshRoomInfo();

        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void player1_name_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e)  { }
    }
}
