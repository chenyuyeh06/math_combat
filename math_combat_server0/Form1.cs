using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace math_combat_server
{
    public enum LogType { Info, Success, Warning, Error, Score }

    public partial class Form1 : Form
    {
        public static Form1 Instance;

        public Form1()
        {
            Instance = this;
            InitializeComponent();
            this.Text = "數字運算對戰 Server";
            this.BackColor = Color.FromArgb(18, 18, 28);
            this.MinimumSize = new Size(780, 500);
            this.Size = new Size(900, 620);
        }

        public void Log(string message, LogType type = LogType.Info)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action(() => Log(message, type)));
                return;
            }

            Color color;
            if (type == LogType.Success) color = Color.FromArgb(80, 220, 120);
            else if (type == LogType.Warning) color = Color.FromArgb(255, 210, 60);
            else if (type == LogType.Error) color = Color.FromArgb(255, 90, 90);
            else if (type == LogType.Score) color = Color.FromArgb(80, 200, 255);
            else color = Color.FromArgb(200, 200, 210);

            string prefix;
            if (type == LogType.Success) prefix = "✓";
            else if (type == LogType.Warning) prefix = "⚠";
            else if (type == LogType.Error) prefix = "✗";
            else if (type == LogType.Score) prefix = "★";
            else prefix = "·";

            string time = DateTime.Now.ToString("HH:mm:ss");

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;

            rtbLog.SelectionColor = Color.FromArgb(100, 100, 120);
            rtbLog.AppendText("[" + time + "] ");

            rtbLog.SelectionColor = color;
            rtbLog.AppendText(prefix + " " + message + Environment.NewLine);

            rtbLog.SelectionColor = rtbLog.ForeColor;
            rtbLog.ScrollToCaret();

            if (rtbLog.Lines.Length > 500)
            {
                int removeUntil = rtbLog.GetFirstCharIndexFromLine(100);
                rtbLog.Select(0, removeUntil);
                rtbLog.SelectedText = "";
            }
        }

        public void UpdateStatus()
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(UpdateStatus));
                return;
            }
            lblStatus.Text =
                "在線人數：" + GameServer.TotalConnections +
                "　房間數：" + GameServer.Rooms.Count +
                "　名字池：" + GameServer.UsedNames.Count;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Tag != null && btnStart.Tag.ToString() == "running")
            {
                GameServer.Stop();
                btnStart.Text = "啟動 Server";
                btnStart.Tag = null;
            }
            else
            {
                // 用 Task.Run 把 Start 丟到背景執行緒
                Task.Run(() => GameServer.Start(9000));
                btnStart.Text = "停止 Server";
                btnStart.Tag = "running";
                UpdateStatus();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
            Log("Log 已清除", LogType.Info);
        }
    }
}
