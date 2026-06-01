namespace math_combat
{
    partial class HomePage
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomePage));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.title = new System.Windows.Forms.Label();
            this.enter_game = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.game_rule = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.input_room_number = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.game_rule.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.input_room_number.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.BackColor = System.Drawing.Color.Silver;
            this.pictureBox1.Location = new System.Drawing.Point(21, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Padding = new System.Windows.Forms.Padding(10);
            this.pictureBox1.Size = new System.Drawing.Size(900, 450);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // title
            // 
            this.title.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.title.AutoSize = true;
            this.title.BackColor = System.Drawing.Color.Silver;
            this.title.Font = new System.Drawing.Font("jf open 粉圓 2.1", 49.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.title.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.title.Location = new System.Drawing.Point(207, 70);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(544, 95);
            this.title.TabIndex = 1;
            this.title.Text = "數字運算對戰";
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // enter_game
            // 
            this.enter_game.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.enter_game.AutoSize = true;
            this.enter_game.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.enter_game.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.enter_game.FlatAppearance.BorderSize = 0;
            this.enter_game.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.enter_game.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.enter_game.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.enter_game.Font = new System.Drawing.Font("jf open 粉圓 2.1", 30F, System.Drawing.FontStyle.Bold);
            this.enter_game.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.enter_game.Location = new System.Drawing.Point(318, 187);
            this.enter_game.Name = "enter_game";
            this.enter_game.Size = new System.Drawing.Size(317, 94);
            this.enter_game.TabIndex = 1;
            this.enter_game.Text = "私人對戰";
            this.enter_game.UseVisualStyleBackColor = false;
            this.enter_game.Click += new System.EventHandler(this.button1_Click);
            this.enter_game.Paint += new System.Windows.Forms.PaintEventHandler(this.button1_Paint);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Silver;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Maroon;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("jf open 粉圓 2.1", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button2.Location = new System.Drawing.Point(390, 300);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(180, 60);
            this.button2.TabIndex = 3;
            this.button2.Text = ">  遊戲規則";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button2.MouseCaptureChanged += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Silver;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Maroon;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("jf open 粉圓 2.1", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button3.Location = new System.Drawing.Point(390, 366);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(180, 60);
            this.button3.TabIndex = 4;
            this.button3.Text = ">  音效設定";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Silver;
            this.button4.FlatAppearance.BorderColor = System.Drawing.Color.Maroon;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("jf open 粉圓 2.1", 30F);
            this.button4.ForeColor = System.Drawing.Color.Maroon;
            this.button4.Location = new System.Drawing.Point(842, 395);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(60, 60);
            this.button4.TabIndex = 5;
            this.button4.Text = "X";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            this.button4.Paint += new System.Windows.Forms.PaintEventHandler(this.button4_Paint);
            this.button4.MouseClick += new System.Windows.Forms.MouseEventHandler(this.button4_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Silver;
            this.label1.Font = new System.Drawing.Font("jf open 粉圓 2.1", 14F);
            this.label1.Location = new System.Drawing.Point(27, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 28);
            this.label1.TabIndex = 6;
            this.label1.Text = "製作名單：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Silver;
            this.label2.Font = new System.Drawing.Font("jf open 粉圓 2.1", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(28, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "葉辰宇";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Silver;
            this.label3.Font = new System.Drawing.Font("jf open 粉圓 2.1", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(28, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 23);
            this.label3.TabIndex = 8;
            this.label3.Text = "林湘庭";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Silver;
            this.label4.Font = new System.Drawing.Font("jf open 粉圓 2.1", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(28, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 23);
            this.label4.TabIndex = 9;
            this.label4.Text = "徐巧芸";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Silver;
            this.label5.Font = new System.Drawing.Font("jf open 粉圓 2.1", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(28, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 23);
            this.label5.TabIndex = 10;
            this.label5.Text = "夏允謙";
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.White;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.ForeColor = System.Drawing.Color.Maroon;
            this.button5.Location = new System.Drawing.Point(171, 274);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(47, 46);
            this.button5.TabIndex = 0;
            this.button5.Text = "X";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Font = new System.Drawing.Font("jf open 粉圓 2.1", 15F, System.Drawing.FontStyle.Underline);
            this.label6.Location = new System.Drawing.Point(100, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(163, 29);
            this.label6.TabIndex = 1;
            this.label6.Text = "遊戲規則介紹";
            // 
            // game_rule
            // 
            this.game_rule.BackColor = System.Drawing.Color.Maroon;
            this.game_rule.Controls.Add(this.label8);
            this.game_rule.Controls.Add(this.label6);
            this.game_rule.Controls.Add(this.button5);
            this.game_rule.Controls.Add(this.pictureBox2);
            this.game_rule.Font = new System.Drawing.Font("jf open 粉圓 2.1", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.game_rule.Location = new System.Drawing.Point(286, 70);
            this.game_rule.Name = "game_rule";
            this.game_rule.Size = new System.Drawing.Size(380, 345);
            this.game_rule.TabIndex = 11;
            this.game_rule.Visible = false;
            this.game_rule.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(28, 73);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(314, 138);
            this.label8.TabIndex = 3;
            this.label8.Text = "總共有0、1、2、3、4、5、6、7\r\n、8、9，及加減乘除十四張卡牌，\r\n每一回合開始玩家會抽出其中五張\r\n卡牌，以這五張卡牌組合運算式，\r\n以運算式的結果來跟" +
    "對手對戰，結\r\n果比較大的一方獲勝。";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.Location = new System.Drawing.Point(15, 15);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(351, 315);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // input_room_number
            // 
            this.input_room_number.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.input_room_number.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(72)))), ((int)(((byte)(110)))));
            this.input_room_number.CausesValidation = false;
            this.input_room_number.Controls.Add(this.label7);
            this.input_room_number.Controls.Add(this.textBox1);
            this.input_room_number.Location = new System.Drawing.Point(223, 187);
            this.input_room_number.Name = "input_room_number";
            this.input_room_number.Size = new System.Drawing.Size(528, 151);
            this.input_room_number.TabIndex = 12;
            this.input_room_number.UseWaitCursor = true;
            this.input_room_number.Visible = false;
            this.input_room_number.Paint += new System.Windows.Forms.PaintEventHandler(this.input_room_number_Paint);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("jf open 粉圓 2.1", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox1.Location = new System.Drawing.Point(44, 87);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(444, 36);
            this.textBox1.TabIndex = 0;
            this.textBox1.UseWaitCursor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("jf open 粉圓 2.1", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label7.Location = new System.Drawing.Point(37, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(289, 39);
            this.label7.TabIndex = 1;
            this.label7.Text = "請輸入房間密碼：";
            this.label7.UseWaitCursor = true;
            // 
            // HomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.ClientSize = new System.Drawing.Size(942, 493);
            this.Controls.Add(this.input_room_number);
            this.Controls.Add(this.game_rule);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.enter_game);
            this.Controls.Add(this.title);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(960, 540);
            this.MinimumSize = new System.Drawing.Size(960, 540);
            this.Name = "HomePage";
            this.Text = "math_combat";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.game_rule.ResumeLayout(false);
            this.game_rule.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.input_room_number.ResumeLayout(false);
            this.input_room_number.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Button enter_game;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel game_rule;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel input_room_number;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox1;
    }
}

