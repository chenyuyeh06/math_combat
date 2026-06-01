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

            // 按鈕圓角
            MakeRoundedControl(button1, 10);

            // 傳入：(物件, 圓角半徑, 平時字體顏色, 滑鼠滑過字體顏色)
            MakeFancyControl(button1, 10, Color.White, Color.Maroon);
            MakeFancyControl(button2, 10, Color.Black, Color.White);
            MakeFancyControl(button3, 10, Color.Black, Color.White);
            MakeFancyControl(button4, 10, Color.Maroon, Color.FromArgb(25, 156, 124));

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

        public static void MakeRoundedControl(Control ctrl, int radius)
        {
            // 自動幫傳進來的物件綁定 Paint 事件
            ctrl.Paint += (sender, e) =>
            {
                Control c = (Control)sender;
                int r = radius;

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.StartFigure();
                path.AddArc(0, 0, 2 * r, 2 * r, 180, 90);
                path.AddArc(c.Width - 2 * r, 0, 2 * r, 2 * r, 270, 90);
                path.AddArc(c.Width - 2 * r, c.Height - 2 * r, 2 * r, 2 * r, 0, 90);
                path.AddArc(0, c.Height - 2 * r, 2 * r, 2 * r, 90, 90);
                path.CloseFigure();

                c.Region = new Region(path);
            };
        }

        public static void MakeFancyControl(Control ctrl, int radius, Color normalColor, Color hoverColor)
        {

            // 滑鼠滑入時改字體顏色
            ctrl.MouseEnter += (sender, e) =>
            {
                ctrl.ForeColor = hoverColor;
            };

            //滑鼠離開時回復字體顏色
            ctrl.MouseLeave += (sender, e) =>
            {
                ctrl.ForeColor = normalColor;
            };
        }

        private void button4_MouseClick(object sender, MouseEventArgs e)
        {

        }


    }
}
