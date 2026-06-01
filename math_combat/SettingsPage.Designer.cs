namespace math_combat
{
    partial class SettingsPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.BGM = new System.Windows.Forms.Label();
            this.SFX = new System.Windows.Forms.Label();
            this.bgm_plus = new System.Windows.Forms.Button();
            this.bgm_sub = new System.Windows.Forms.Button();
            this.sfx_sub = new System.Windows.Forms.Button();
            this.sfx_plus = new System.Windows.Forms.Button();
            this.bgm_control = new System.Windows.Forms.Label();
            this.sfx_control = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Silver;
            this.pictureBox1.Location = new System.Drawing.Point(21, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(900, 450);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Silver;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("jf open 粉圓 2.1", 15F);
            this.button1.Location = new System.Drawing.Point(34, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(142, 67);
            this.button1.TabIndex = 1;
            this.button1.Text = "< 返回";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BGM
            // 
            this.BGM.AutoSize = true;
            this.BGM.BackColor = System.Drawing.Color.Silver;
            this.BGM.Font = new System.Drawing.Font("jf open 粉圓 2.1", 25F);
            this.BGM.Location = new System.Drawing.Point(248, 161);
            this.BGM.Name = "BGM";
            this.BGM.Size = new System.Drawing.Size(188, 48);
            this.BGM.TabIndex = 2;
            this.BGM.Text = "背景音樂";
            // 
            // SFX
            // 
            this.SFX.AutoSize = true;
            this.SFX.BackColor = System.Drawing.Color.Silver;
            this.SFX.Font = new System.Drawing.Font("jf open 粉圓 2.1", 25F);
            this.SFX.Location = new System.Drawing.Point(248, 253);
            this.SFX.Name = "SFX";
            this.SFX.Size = new System.Drawing.Size(188, 48);
            this.SFX.TabIndex = 3;
            this.SFX.Text = "遊戲音效";
            // 
            // bgm_plus
            // 
            this.bgm_plus.BackColor = System.Drawing.Color.Silver;
            this.bgm_plus.FlatAppearance.BorderSize = 0;
            this.bgm_plus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bgm_plus.Image = global::math_combat.Properties.Resources.Plus_Math;
            this.bgm_plus.Location = new System.Drawing.Point(659, 161);
            this.bgm_plus.Name = "bgm_plus";
            this.bgm_plus.Size = new System.Drawing.Size(54, 51);
            this.bgm_plus.TabIndex = 4;
            this.bgm_plus.UseVisualStyleBackColor = false;
            this.bgm_plus.Click += new System.EventHandler(this.bgm_plus_Click);
            // 
            // bgm_sub
            // 
            this.bgm_sub.BackColor = System.Drawing.Color.Silver;
            this.bgm_sub.FlatAppearance.BorderSize = 0;
            this.bgm_sub.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bgm_sub.Image = global::math_combat.Properties.Resources.Subtract;
            this.bgm_sub.Location = new System.Drawing.Point(479, 168);
            this.bgm_sub.Name = "bgm_sub";
            this.bgm_sub.Size = new System.Drawing.Size(62, 37);
            this.bgm_sub.TabIndex = 5;
            this.bgm_sub.UseVisualStyleBackColor = false;
            this.bgm_sub.Click += new System.EventHandler(this.bgm_sub_Click);
            // 
            // sfx_sub
            // 
            this.sfx_sub.BackColor = System.Drawing.Color.Silver;
            this.sfx_sub.FlatAppearance.BorderSize = 0;
            this.sfx_sub.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sfx_sub.Image = global::math_combat.Properties.Resources.Subtract;
            this.sfx_sub.Location = new System.Drawing.Point(479, 273);
            this.sfx_sub.Name = "sfx_sub";
            this.sfx_sub.Size = new System.Drawing.Size(62, 23);
            this.sfx_sub.TabIndex = 6;
            this.sfx_sub.UseVisualStyleBackColor = false;
            this.sfx_sub.Click += new System.EventHandler(this.sfx_sub_Click);
            // 
            // sfx_plus
            // 
            this.sfx_plus.BackColor = System.Drawing.Color.Silver;
            this.sfx_plus.Cursor = System.Windows.Forms.Cursors.Default;
            this.sfx_plus.FlatAppearance.BorderSize = 0;
            this.sfx_plus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sfx_plus.Image = global::math_combat.Properties.Resources.Plus_Math;
            this.sfx_plus.Location = new System.Drawing.Point(659, 261);
            this.sfx_plus.Name = "sfx_plus";
            this.sfx_plus.Size = new System.Drawing.Size(54, 47);
            this.sfx_plus.TabIndex = 7;
            this.sfx_plus.UseVisualStyleBackColor = false;
            this.sfx_plus.Click += new System.EventHandler(this.sfx_plus_Click);
            // 
            // bgm_control
            // 
            this.bgm_control.AutoSize = true;
            this.bgm_control.BackColor = System.Drawing.Color.Silver;
            this.bgm_control.Font = new System.Drawing.Font("jf open 粉圓 2.1", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.bgm_control.Location = new System.Drawing.Point(558, 166);
            this.bgm_control.Name = "bgm_control";
            this.bgm_control.Size = new System.Drawing.Size(77, 38);
            this.bgm_control.TabIndex = 8;
            this.bgm_control.Text = "100";
            this.bgm_control.Click += new System.EventHandler(this.bgm_control_Click);
            // 
            // sfx_control
            // 
            this.sfx_control.AutoSize = true;
            this.sfx_control.BackColor = System.Drawing.Color.Silver;
            this.sfx_control.Font = new System.Drawing.Font("jf open 粉圓 2.1", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sfx_control.Location = new System.Drawing.Point(558, 261);
            this.sfx_control.Name = "sfx_control";
            this.sfx_control.Size = new System.Drawing.Size(77, 38);
            this.sfx_control.TabIndex = 9;
            this.sfx_control.Text = "100";
            this.sfx_control.Click += new System.EventHandler(this.sfx_control_Click);
            // 
            // SettingsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.ClientSize = new System.Drawing.Size(942, 493);
            this.Controls.Add(this.sfx_control);
            this.Controls.Add(this.bgm_control);
            this.Controls.Add(this.sfx_plus);
            this.Controls.Add(this.sfx_sub);
            this.Controls.Add(this.bgm_sub);
            this.Controls.Add(this.bgm_plus);
            this.Controls.Add(this.SFX);
            this.Controls.Add(this.BGM);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "SettingsPage";
            this.Text = "SettingsPage";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.Load += new System.EventHandler(this.SettingsPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label BGM;
        private System.Windows.Forms.Label SFX;
        private System.Windows.Forms.Button bgm_plus;
        private System.Windows.Forms.Button bgm_sub;
        private System.Windows.Forms.Button sfx_sub;
        private System.Windows.Forms.Button sfx_plus;
        private System.Windows.Forms.Label bgm_control;
        private System.Windows.Forms.Label sfx_control;
    }
}