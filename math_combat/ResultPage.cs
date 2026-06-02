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
        HomePage homePage;
        public ResultPage(HomePage homePage)
        {
            InitializeComponent();
            this.homePage = homePage;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            GameUnits.MakeRoundedControl(pictureBox3, 15);
        }
    }
}
