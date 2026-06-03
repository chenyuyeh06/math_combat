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
    public partial class ResultPage : Form
    {
        HomePage homePage => GameUnits.homePage;
        RoomPage roomPage => GameUnits.roomPage;
        GamePage gamePage => GameUnits.gamePage;

        public ResultPage(HomePage homePage)
        {
            InitializeComponent();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            GameUnits.MakeRoundedControl(pictureBox3, 15);
        }

        private void ResultPage_Load(object sender, EventArgs e)
        {
            GameUnits.MakeRoundedControl(pictureBox1, 15);
            GameUnits.MakeRoundedControl(pictureBox2, 15);
            GameUnits.MakeRoundedControl(pictureBox3, 15);
            GameUnits.MakeRoundedControl(back_to_room, 15);
            GameUnits.MakeRoundedControl(back_to_home_page, 15);
        }

        private void player1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void player2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void back_to_room_Click(object sender, EventArgs e)
        {
            GameUnits.SwitchToForm(this, roomPage);
        }

        private void back_to_home_page_Click(object sender, EventArgs e)
        {
            GameUnits.SwitchToForm(this, homePage);
        }
    }
}
