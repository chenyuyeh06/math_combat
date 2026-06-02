using System;
using System.Collections.Generic;
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
    public static class GameUnits
    {
        public static HomePage homePage;
        public static RoomPage roomPage;
        public static GamePage gamePage;
        public static ResultPage resultPage;



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

        // 滑鼠懸停變色
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

        //字體載入


        //視窗切換
        public static void SwitchToForm(Form currentForm, Form targetForm)
        {
            if (currentForm == null || targetForm == null) return;

            // 設定新視窗的位置與大小同步
            targetForm.StartPosition = FormStartPosition.Manual;
            targetForm.Location = currentForm.Location;
            targetForm.Size = currentForm.Size;

            // 讓新視窗記住原本的owner是誰
            // 如果目前視窗本身就有owner就把owner傳承給新視窗
            if (currentForm.Owner != null)
            {
                targetForm.Owner = currentForm.Owner;
            }
            else
            {
                // 如果目前視窗沒有owner那自己就是新視窗的owner
                targetForm.Owner = currentForm;
            }

            targetForm.Show();
            currentForm.Hide();
        }
    }
}
