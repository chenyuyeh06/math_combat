using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace math_combat
{
    public partial class HomePage : Form
    {
        // 宣告一個私有的 PrivateFontCollection 來存放字體
        private PrivateFontCollection privateFonts = new PrivateFontCollection();
        private Font customFont;
        public HomePage()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCustomFont();

            // 套用字體給指定的控制項
            button1.Font = new Font(privateFonts.Families[0], 20, FontStyle.Regular);
            label1.Font = new Font(privateFonts.Families[0], 15, FontStyle.Regular);
        }

        //載入字體
        private void LoadCustomFont()
        {
            // 字體檔案放在輸出的資料夾路徑
            string fontPath = System.IO.Path.Combine(Application.StartupPath, "jf-openhuninn-2.1.ttf");

            try
            {
                // 將字體檔案載入到 privateFonts 集合中
                privateFonts.AddFontFile(fontPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("字體載入失敗: " + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button1.FlatStyle = FlatStyle.Flat;


        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;

            // 設定圓角半徑（數字越大越圓）
            int borderRadius = 5;

            // 啟用抗鋸齒，讓圓角邊緣看起來更平滑
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 建立一個圓角矩形的路徑
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, borderRadius, borderRadius, 180, 90); // 左上角
            path.AddArc(btn.Width - borderRadius, 0, borderRadius, borderRadius, 270, 90); // 右上角
            path.AddArc(btn.Width - borderRadius, btn.Height - borderRadius, borderRadius, borderRadius, 0, 90); // 右下角
            path.AddArc(0, btn.Height - borderRadius, borderRadius, borderRadius, 90, 90); // 左下角
            path.CloseFigure();

            // 關鍵：將按鈕的形狀裁剪成我們設定的圓角路徑
            btn.Region = new Region(path);

        }

    }
}
