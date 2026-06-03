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

            GameUnits.SwitchToForm(this, gamePage);
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
            GameUnits.SwitchToForm(this, homePage);
        }

        private void RoomPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            GameUnits.SwitchToForm(this, homePage);
        }

        private void RoomPage_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
    }
}
