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

        private RoomPage   roomPage   => GameUnits.roomPage;
        private GamePage   gamePage   => GameUnits.gamePage;
        private ResultPage resultPage => GameUnits.resultPage;

        // true = 建立房間；false = 加入房間
        private bool _createMode = false;

        public HomePage()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //
            this.StartPosition = FormStartPosition.CenterScreen;

            //創建新視窗

            enter_game.FlatStyle = FlatStyle.Flat;
            button2.FlatStyle = FlatStyle.Flat;
            volumnSettingsButton.FlatStyle = FlatStyle.Flat;
            button4.FlatStyle = FlatStyle.Flat;
            button5.FlatStyle = FlatStyle.Flat;

            // 圓角
            GameUnits.MakeRoundedControl(game_rule, 20);
            GameUnits.MakeRoundedControl(input_room_number, 20);
            GameUnits.MakeRoundedControl(input_player_name, 20);
            GameUnits.MakeRoundedControl(pictureBox1, 15);
            GameUnits.MakeRoundedControl(pictureBox2, 15);
            GameUnits.MakeRoundedControl(pictureBox3, 15);
            GameUnits.MakeRoundedControl(volumn_settings, 20);
            

            // 傳入：(物件, 圓角半徑, 平時字體顏色, 滑鼠滑過字體顏色)
            GameUnits.MakeFancyControl(enter_game, 10, Color.WhiteSmoke, Color.White);
            GameUnits.MakeFancyControl(button2, 1, Color.Black, Color.WhiteSmoke);
            GameUnits.MakeFancyControl(volumnSettingsButton, 1, Color.Black, Color.WhiteSmoke);
            GameUnits.MakeFancyControl(button4, 1, Color.Maroon, Color.FromArgb(25, 156, 124));
            GameUnits.MakeFancyControl(button5, 10, Color.Maroon, Color.FromArgb(25, 156, 124));
        }

        //載入字體
        public void LoadFontFromResource()
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
        // enter_game 按鈕（唯一入口）
        private void button1_Click(object sender, EventArgs e)
        {
            _createMode = false; // 預設為加入模式
            input_player_name.Visible = true;
            input_room_number.Visible = false;
            input_player_name.BringToFront();
            textBox2.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            game_rule.BringToFront();
            game_rule.Visible = true;
            input_room_number.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            volumn_settings.BringToFront();
            volumn_settings.Visible = true;
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

        private void input_room_number_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            input_room_number.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // 向左移動
            labelMarquee.Location = new Point(labelMarquee.Location.X - 8, labelMarquee.Location.Y);

            // 判斷文字是不是已經「完全走出左邊邊界」了
            // labelMarquee.Width 是文字本身的寬度，變成負數代表整串字都隱形了
            if (labelMarquee.Location.X + labelMarquee.Width < 0)
            {
                //讓文字重回最右邊（this.ClientSize.Width 是主表單的總寬度）
                labelMarquee.Location = new Point(this.ClientSize.Width + 10, labelMarquee.Location.Y);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panelMarquee_Paint(object sender, PaintEventArgs e)
        {

        }

        private void input_player_name_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string roomId = textBox1.Text.Trim();
                if (roomId.Length > 0)
                {
                    textBox1.Enabled = false;
                    // StartNetwork 連到 Server 後送 CREATE_ROOM 或 JOIN_ROOM
                    // UI 切換由 GameUnits.ProcessMessage 在收到 ROOM_CREATED / ROOM_JOINED 後觸發
                    bool success = await GameUnits.StartNetwork(roomId, _createMode);
                    textBox1.Enabled = true;

                    if (!success)
                    {
                        // 連線失敗，保持在 HomePage
                        input_room_number.Visible = false;
                    }
                    // 成功時 GameUnits 會自動切換到 RoomPage
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (textBox2.Text.Length > 0)
                {
                    GameUnits.player_name = textBox2.Text;
                    input_room_number.Visible = true;
                    input_player_name.Visible = false;
                    input_room_number.BringToFront();
                    textBox1.Focus();
                }
            }
        }

        private void volumn_settings_Paint(object sender, PaintEventArgs e)
        {

        }

        public void sfx_sub_Click(object sender, EventArgs e)
        {
            if (int.Parse(sfx_control.Text) <= 100 && int.Parse(sfx_control.Text) > 0)
            {
                sfx_control.Text = (int.Parse(sfx_control.Text) - 1).ToString();
                GameUnits.vulume_sfx = int.Parse(sfx_control.Text);
                
            }
        }

        private void bgm_sub_Click(object sender, EventArgs e)
        {
            if (int.Parse(bgm_control.Text) <= 100 && int.Parse(bgm_control.Text) > 0)
            {
                bgm_control.Text = (int.Parse(bgm_control.Text) - 1).ToString();
                GameUnits.volume_bgm = int.Parse(bgm_control.Text);
            }
        }

        private void bgm_plus_Click(object sender, EventArgs e)
        {
            if (int.Parse(bgm_control.Text) < 100 && int.Parse(bgm_control.Text) >= 0)
            {
                bgm_control.Text = (int.Parse(bgm_control.Text) + 1).ToString();
                GameUnits.volume_bgm = int.Parse(bgm_control.Text);

            }
        }

        private void sfx_plus_Click(object sender, EventArgs e)
        {
            if (int.Parse(sfx_control.Text) < 100 && int.Parse(sfx_control.Text) >= 0)
            {
                sfx_control.Text = (int.Parse(sfx_control.Text) + 1).ToString();
                GameUnits.vulume_sfx = int.Parse(sfx_control.Text);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            volumn_settings.Visible = false;
        }

        private void HomePage_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 如果是程式自己呼叫 Application.Exit() 就不攔截
            if (e.CloseReason == CloseReason.ApplicationExitCall) return;

            e.Cancel = true; // 先阻止關閉

            DialogResult result = MessageBox.Show("確定要結束遊戲嗎？", "離開遊戲",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                Application.Exit();
        }
    }
}
