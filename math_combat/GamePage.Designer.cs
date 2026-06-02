namespace math_combat
{
    partial class GamePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GamePage));
            this.panel1 = new System.Windows.Forms.Panel();
            this.player1 = new System.Windows.Forms.Label();
            this.player2 = new System.Windows.Forms.Label();
            this.round = new System.Windows.Forms.Label();
            this.seconds = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.check_card = new System.Windows.Forms.Button();
            this.cancel_card = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.cancel_card);
            this.panel1.Controls.Add(this.check_card);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.seconds);
            this.panel1.Controls.Add(this.round);
            this.panel1.Controls.Add(this.player2);
            this.panel1.Controls.Add(this.player1);
            this.panel1.Location = new System.Drawing.Point(21, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 311);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // player1
            // 
            this.player1.AutoSize = true;
            this.player1.Font = new System.Drawing.Font("jf open 粉圓 2.1", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.player1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.player1.Location = new System.Drawing.Point(21, 20);
            this.player1.Name = "player1";
            this.player1.Size = new System.Drawing.Size(78, 23);
            this.player1.TabIndex = 0;
            this.player1.Text = "player1";
            // 
            // player2
            // 
            this.player2.AutoSize = true;
            this.player2.Font = new System.Drawing.Font("jf open 粉圓 2.1", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.player2.Location = new System.Drawing.Point(792, 20);
            this.player2.Name = "player2";
            this.player2.Size = new System.Drawing.Size(78, 23);
            this.player2.TabIndex = 1;
            this.player2.Text = "player2";
            // 
            // round
            // 
            this.round.AutoSize = true;
            this.round.Font = new System.Drawing.Font("jf open 粉圓 2.1", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.round.Location = new System.Drawing.Point(312, 20);
            this.round.Name = "round";
            this.round.Size = new System.Drawing.Size(103, 23);
            this.round.TabIndex = 2;
            this.round.Text = "回合：1/3";
            // 
            // seconds
            // 
            this.seconds.AutoSize = true;
            this.seconds.Font = new System.Drawing.Font("jf open 粉圓 2.1", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.seconds.Location = new System.Drawing.Point(495, 20);
            this.seconds.Name = "seconds";
            this.seconds.Size = new System.Drawing.Size(91, 23);
            this.seconds.TabIndex = 3;
            this.seconds.Text = "時間：5s";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Teal;
            this.pictureBox1.Location = new System.Drawing.Point(25, 48);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(845, 10);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // check_card
            // 
            this.check_card.FlatAppearance.BorderSize = 0;
            this.check_card.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.check_card.Image = global::math_combat.Properties.Resources.Check_Mark;
            this.check_card.Location = new System.Drawing.Point(748, 78);
            this.check_card.Name = "check_card";
            this.check_card.Size = new System.Drawing.Size(116, 107);
            this.check_card.TabIndex = 5;
            this.check_card.UseVisualStyleBackColor = true;
            this.check_card.Click += new System.EventHandler(this.check_card_Click);
            // 
            // cancel_card
            // 
            this.cancel_card.FlatAppearance.BorderSize = 0;
            this.cancel_card.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancel_card.Image = global::math_combat.Properties.Resources.Cancel;
            this.cancel_card.Location = new System.Drawing.Point(748, 191);
            this.cancel_card.Name = "cancel_card";
            this.cancel_card.Size = new System.Drawing.Size(116, 104);
            this.cancel_card.TabIndex = 6;
            this.cancel_card.UseVisualStyleBackColor = true;
            this.cancel_card.Click += new System.EventHandler(this.cancel_card_Click);
            // 
            // GamePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.ClientSize = new System.Drawing.Size(942, 493);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GamePage";
            this.Text = "GamePage";
            this.Load += new System.EventHandler(this.GamePage_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label seconds;
        private System.Windows.Forms.Label round;
        private System.Windows.Forms.Label player2;
        private System.Windows.Forms.Label player1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button cancel_card;
        private System.Windows.Forms.Button check_card;
    }
}