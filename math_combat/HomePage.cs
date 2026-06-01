using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace math_combat
{
    public partial class HomePage : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();
        public HomePage()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadFontFromResource();

            enter_game.FlatStyle = FlatStyle.Flat;
            button2.FlatStyle = FlatStyle.Flat;
            button3.FlatStyle = FlatStyle.Flat;
            button4.FlatStyle = FlatStyle.Flat;
            button5.FlatStyle = FlatStyle.Flat;

            // 圓角
            MakeRoundedControl(game_rule, 20);
            MakeRoundedControl(input_room_number, 20);
            MakeRoundedControl(pictureBox1, 15);
            MakeRoundedControl(pictureBox2, 15);

            // 傳入：(物件, 圓角半徑, 平時字體顏色, 滑鼠滑過字體顏色)
            MakeFancyControl(enter_game, 10, Color.White, Color.Maroon);
            MakeFancyControl(button2, 1, Color.Black, Color.White);
           MakeFancyControl(button3, 1, Color.Black, Color.White);
            MakeFancyControl(button4, 1, Color.Maroon, Color.FromArgb(25, 156, 124));
            MakeFancyControl(button5, 10, Color.Maroon, Color.FromArgb(25, 156, 124));

        }

        //載入字體
        private void LoadFontFromResource()
        {
            try
            {
                // 直接從資源檔撈出字體的 byte 陣列
                byte[] fontData = Properties.Resources.jf_openhuninn_2_1;

                // 在記憶體中配置空間並複製資料
                IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

                // 將記憶體中的字體資料註冊到私有字體庫中
                privateFonts.AddMemoryFont(fontPtr, fontData.Length);

                // 釋放記憶體指標
                Marshal.FreeCoTaskMem(fontPtr);
            }
            catch (Exception ex)
            {
                MessageBox.Show("從資源載入字體失敗: " + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            input_room_number.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            game_rule.BringToFront();
            game_rule.Visible = true;
            input_room_number.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {


        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("確定要結束遊戲嗎？", "離開遊戲", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // 如果玩家點擊了「是」
            if (result == DialogResult.Yes)
            {
                // 結束整個應用程式
                Application.Exit();
            }
        }

        private void button4_Paint(object sender, PaintEventArgs e)
        {

        }

     

        private void button4_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            game_rule.Visible = false;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        public static void MakeRoundedControl(Control ctrl, int radius)
        {
            ctrl.Paint += (sender, e) =>
            {
                Control c = (Control)sender;
                int r = radius;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                GraphicsPath path = new GraphicsPath();
                path.StartFigure();
                path.AddArc(0, 0, 2 * r, 2 * r, 180, 90);
                path.AddArc(c.Width - 2 * r, 0, 2 * r, 2 * r, 270, 90);
                path.AddArc(c.Width - 2 * r, c.Height - 2 * r, 2 * r, 2 * r, 0, 90);
                path.AddArc(0, c.Height - 2 * r, 2 * r, 2 * r, 90, 90);
                path.CloseFigure();

                c.Region = new Region(path);
            };
        }

        // 圓角 + 滑鼠懸停變色
        public static void MakeFancyControl(Control ctrl, int radius, Color normalColor, Color hoverColor)
        {
            // 先套用圓角效果
            MakeRoundedControl(ctrl, radius);

            // 設定預設字體顏色
            ctrl.ForeColor = normalColor;

            // 滑鼠滑入
            ctrl.MouseEnter += (sender, e) =>
            {
                ctrl.ForeColor = hoverColor;
            };

            // 滑鼠離開
            ctrl.MouseLeave += (sender, e) =>
            {
                ctrl.ForeColor = normalColor;
            };
        }

        private void input_room_number_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            input_room_number.Visible = false;
        }
    }
}
