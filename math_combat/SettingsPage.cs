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
    public partial class SettingsPage : Form
    {
        private HomePage homePage;

        public SettingsPage(HomePage homePage)
        {
            InitializeComponent();
            this.homePage = homePage;

        }

        private void SettingsPage_Load(object sender, EventArgs e)
        {

        }

        private void SettingsPage_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (homePage != null)
            {
                this.Hide();
                homePage.Show();
                homePage.Location = this.Location; // 保持位置一致
            }
        }   

        private void bgm_control_Click(object sender, EventArgs e)
        {

        }
        private void bgm_sub_Click(object sender, EventArgs e)
        {
            if (int.Parse(bgm_control.Text) <= 100 && int.Parse(bgm_control.Text) > 0)
            {
                bgm_control.Text = (int.Parse(bgm_control.Text) - 1).ToString();
            }
        }

        private void bgm_plus_Click(object sender, EventArgs e)
        {
            if (int.Parse(bgm_control.Text) < 100 && int.Parse(bgm_control.Text) >= 0)
            {
                bgm_control.Text = (int.Parse(bgm_control.Text) + 1).ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            homePage.Location = this.Location; // 保持位置一致
            homePage.Show();
        }

        private void sfx_sub_Click(object sender, EventArgs e)
        {
            if(int.Parse(sfx_control.Text) <= 100 && int.Parse(sfx_control.Text) > 0)
            {
                sfx_control.Text = (int.Parse(sfx_control.Text) - 1).ToString();
            }
        }

        private void sfx_control_Click(object sender, EventArgs e)
        {

        }

        private void sfx_plus_Click(object sender, EventArgs e)
        {
            if(int.Parse(sfx_control.Text) < 100 && int.Parse(sfx_control.Text) >= 0)
            {
                sfx_control.Text = (int.Parse(sfx_control.Text) + 1).ToString();
            }
        }
    }
}
