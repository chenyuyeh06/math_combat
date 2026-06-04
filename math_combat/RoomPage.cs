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
        private HomePage homePage => GameUnits.homePage;
        private GamePage gamePage => GameUnits.gamePage;
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

            //hover color for fonts
            GameUnits.MakeFancyControl(start_game_button, 15, Color.Black, Color.WhiteSmoke);
            GameUnits.MakeFancyControl(round_sub, 15, Color.Black, Color.WhiteSmoke);
            GameUnits.MakeFancyControl(round_plus, 15, Color.Black, Color.WhiteSmoke);
            GameUnits.MakeFancyControl(sec_sub, 15, Color.Black, Color.WhiteSmoke);
            GameUnits.MakeFancyControl(sec_plus, 15, Color.Black, Color.WhiteSmoke);


        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void player1_name_Click(object sender, EventArgs e)
        {

        }

        private void start_game_button_Click(object sender, EventArgs e)
        {
            // TODO: 傳送rounds和secs的數值到GamePage, 檢查進入遊戲的條件
            
            if (GameUnits.isHost)
            {
                GameUnits.hostReady = !GameUnits.hostReady;
                GameUnits.CheckStartGame();
            }
            else
            {
                if (!GameUnits.isSpectator)
                {
                    GameUnits.clientReady = !GameUnits.clientReady;
                    byte[] bytes = Encoding.UTF8.GetBytes(GameUnits.clientReady ? "READY" : "UNREADY");
                    try
                    {
                        GameUnits.stream.Write(bytes, 0, bytes.Length);
                    }
                    catch {}
                    UpdateNames();
                }
            }
        }

        private void round_plus_Click(object sender, EventArgs e)
        {
            if (int.Parse(round_set.Text) < 5)
            {
                GameUnits.rounds = 5;
                round_set.Text = GameUnits.rounds.ToString();
            }
        }

        private void round_sub_Click(object sender, EventArgs e)
        {
            if (int.Parse(round_set.Text) > 3)
            {
                GameUnits.rounds = 3;
                round_set.Text = GameUnits.rounds.ToString();
            }
        }

        private void sec_plus_Click(object sender, EventArgs e)
        {
            if (int.Parse(sec_set.Text) < 10 && int.Parse(sec_set.Text) > 2)
            {
                GameUnits.secs = int.Parse(sec_set.Text) + 1;
                sec_set.Text = GameUnits.secs.ToString();
            }
        }

        private void sec_sub_Click(object sender, EventArgs e)
        {
            if (int.Parse(sec_set.Text) <= 10 && int.Parse(sec_set.Text) > 3)
            {
                GameUnits.secs = int.Parse(sec_set.Text) - 1;
                sec_set.Text = GameUnits.secs.ToString();
            }
        }

        private void back_to_home_page_Click(object sender, EventArgs e)
        {
            GameUnits.CleanupNetwork();
            GameUnits.SwitchToForm(this, homePage);
        }

        private void RoomPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            GameUnits.CleanupNetwork();
            GameUnits.SwitchToForm(this, homePage);
        }

        private void RoomPage_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void RoomPage_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                room_number.Text = "房間號碼：" + GameUnits.room_number;
                if (GameUnits.isHost)
                {
                    GameUnits.hostReady = false;
                    GameUnits.clientReady = false;
                    GameUnits.BroadcastRoomInfo();
                    GameUnits.BroadcastReadyState();
                }
                else
                {
                    GameUnits.clientReady = false;
                    UpdateNames();
                }
            }
        }

        public void UpdateNames()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(UpdateNames));
                return;
            }

            if (GameUnits.isHost)
            {
                player1_name.Text = GameUnits.player_name;
                player2_name.Text = (GameUnits.clientNames.Count > 0 && !string.IsNullOrEmpty(GameUnits.clientNames[0])) ? GameUnits.clientNames[0] : "等候玩家加入...";
            }
            else
            {
                player1_name.Text = string.IsNullOrEmpty(GameUnits.hostName) ? "房主" : GameUnits.hostName;
                player2_name.Text = string.IsNullOrEmpty(GameUnits.player2Name) ? GameUnits.player_name : GameUnits.player2Name;
            }

            UpdateButtonState();
        }

        public void UpdateNamesFromInfo(string p1, List<string> guests)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateNamesFromInfo(p1, guests)));
                return;
            }

            GameUnits.hostName = p1;
            if (guests.Count > 0)
            {
                GameUnits.player2Name = guests[0];
            }
            else
            {
                GameUnits.player2Name = "";
            }

            player1_name.Text = p1;
            player2_name.Text = !string.IsNullOrEmpty(GameUnits.player2Name) ? GameUnits.player2Name : "等候玩家加入...";

            // 更新觀戰名單
            Label[] spectatorLabels = new Label[] { label3, label4, label5, label6, label7, label8 };
            for (int i = 0; i < spectatorLabels.Length; i++)
            {
                if (i + 1 < guests.Count && !string.IsNullOrEmpty(guests[i + 1]))
                {
                    spectatorLabels[i].Text = guests[i + 1];
                    spectatorLabels[i].Visible = true;
                }
                else
                {
                    spectatorLabels[i].Text = "";
                    spectatorLabels[i].Visible = false;
                }
            }

            UpdateButtonState();
        }

        public void UpdateButtonState()
        {
            if (GameUnits.isHost)
            {
                bool hasClient = GameUnits.clientList.Count > 0;
                if (!hasClient)
                {
                    start_game_button.Text = "等候對手...";
                    start_game_button.Enabled = false;
                }
                else
                {
                    if (GameUnits.hostReady)
                    {
                        start_game_button.Text = "已準備(等待中)";
                        start_game_button.Enabled = true;
                    }
                    else
                    {
                        start_game_button.Text = "準備對戰";
                        start_game_button.Enabled = true;
                    }
                }
            }
            else
            {
                if (GameUnits.isSpectator)
                {
                    start_game_button.Text = "觀戰中...";
                    start_game_button.Enabled = false;
                }
                else
                {
                    if (GameUnits.clientReady)
                    {
                        start_game_button.Text = "已準備(等待中)";
                        start_game_button.Enabled = true;
                    }
                    else
                    {
                        start_game_button.Text = "準備對戰";
                        start_game_button.Enabled = true;
                    }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
