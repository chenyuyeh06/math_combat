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
        private HomePage homePage;
        
        public RoomPage(HomePage homePage)
        {
            InitializeComponent();
            this.homePage = homePage;
        }

        private void RoomPage_Load(object sender, EventArgs e)
        {
            GameUnits.MakeRoundedControl(pictureBox1, 15);
            if (homePage != null)
            {
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void player1_name_Click(object sender, EventArgs e)
        {

        }

        private void start_game_button_Click(object sender, EventArgs e)
        {

        }

        private void round_plus_Click(object sender, EventArgs e)
        {
            if (int.Parse(round_set.Text) < 5)
            {
                round_set.Text = "5";
            }
        }

        private void round_sub_Click(object sender, EventArgs e)
        {
            if (int.Parse(round_set.Text) > 3)
            {
                round_set.Text = "3";
            }
        }
    }
}
