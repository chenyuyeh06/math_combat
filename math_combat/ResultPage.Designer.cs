namespace math_combat
{
    partial class ResultPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultPage));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.RoomNumber = new System.Windows.Forms.Label();
            this.player1 = new System.Windows.Forms.Label();
            this.back_to_room = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.back_to_home_page = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Teal;
            this.pictureBox1.Location = new System.Drawing.Point(343, 226);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(235, 91);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // RoomNumber
            // 
            this.RoomNumber.AutoSize = true;
            this.RoomNumber.BackColor = System.Drawing.Color.WhiteSmoke;
            this.RoomNumber.Font = new System.Drawing.Font("jf open 粉圓 2.1", 30F);
            this.RoomNumber.Location = new System.Drawing.Point(250, 54);
            this.RoomNumber.Name = "RoomNumber";
            this.RoomNumber.Size = new System.Drawing.Size(410, 57);
            this.RoomNumber.TabIndex = 2;
            this.RoomNumber.Text = "房間號碼：XXXX";
            // 
            // player1
            // 
            this.player1.AutoSize = true;
            this.player1.BackColor = System.Drawing.Color.Teal;
            this.player1.Font = new System.Drawing.Font("jf open 粉圓 2.1", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.player1.Location = new System.Drawing.Point(395, 252);
            this.player1.Name = "player1";
            this.player1.Size = new System.Drawing.Size(130, 38);
            this.player1.TabIndex = 3;
            this.player1.Text = "player1";
            this.player1.Click += new System.EventHandler(this.player1_Click);
            // 
            // back_to_room
            // 
            this.back_to_room.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(156)))), ((int)(((byte)(104)))));
            this.back_to_room.FlatAppearance.BorderSize = 0;
            this.back_to_room.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Maroon;
            this.back_to_room.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Teal;
            this.back_to_room.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.back_to_room.Font = new System.Drawing.Font("jf open 粉圓 2.1", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back_to_room.Location = new System.Drawing.Point(203, 350);
            this.back_to_room.Name = "back_to_room";
            this.back_to_room.Size = new System.Drawing.Size(235, 79);
            this.back_to_room.TabIndex = 5;
            this.back_to_room.Text = "回到房間";
            this.back_to_room.UseVisualStyleBackColor = false;
            this.back_to_room.Click += new System.EventHandler(this.back_to_room_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pictureBox3.Location = new System.Drawing.Point(21, 22);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(900, 450);
            this.pictureBox3.TabIndex = 6;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pictureBox2.Image = global::math_combat.Properties.Resources.Crown2;
            this.pictureBox2.Location = new System.Drawing.Point(421, 135);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(78, 70);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // back_to_home_page
            // 
            this.back_to_home_page.BackColor = System.Drawing.Color.Maroon;
            this.back_to_home_page.FlatAppearance.BorderSize = 0;
            this.back_to_home_page.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.back_to_home_page.Font = new System.Drawing.Font("jf open 粉圓 2.1", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back_to_home_page.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.back_to_home_page.Location = new System.Drawing.Point(480, 350);
            this.back_to_home_page.Name = "back_to_home_page";
            this.back_to_home_page.Size = new System.Drawing.Size(235, 79);
            this.back_to_home_page.TabIndex = 8;
            this.back_to_home_page.Text = "回到首頁";
            this.back_to_home_page.UseVisualStyleBackColor = false;
            this.back_to_home_page.Click += new System.EventHandler(this.back_to_home_page_Click);
            // 
            // ResultPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.ClientSize = new System.Drawing.Size(942, 493);
            this.Controls.Add(this.back_to_home_page);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.back_to_room);
            this.Controls.Add(this.player1);
            this.Controls.Add(this.RoomNumber);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ResultPage";
            this.Text = "ResultPage";
            this.Load += new System.EventHandler(this.ResultPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label RoomNumber;
        private System.Windows.Forms.Label player1;
        private System.Windows.Forms.Button back_to_room;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button back_to_home_page;
    }
}