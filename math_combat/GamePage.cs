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
        public GamePage(HomePage homePage)
        {
            InitializeComponent();
            this.homePage = GameUnits.homePage;
        }

        private void GamePage_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
