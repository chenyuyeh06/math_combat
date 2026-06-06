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
            this.panel_game_board = new System.Windows.Forms.Panel();
            this.tableLayout_BoardCards = new System.Windows.Forms.TableLayoutPanel();
            this.cancel_card = new System.Windows.Forms.Button();
            this.check_card = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.seconds = new System.Windows.Forms.Label();
            this.round = new System.Windows.Forms.Label();
            this.player2 = new System.Windows.Forms.Label();
            this.player1 = new System.Windows.Forms.Label();
            this.tableLayout_HandCards = new System.Windows.Forms.TableLayoutPanel();
            this.panel_game_board.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_game_board
            // 
            this.panel_game_board.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel_game_board.Controls.Add(this.tableLayout_BoardCards);
            this.panel_game_board.Controls.Add(this.cancel_card);
            this.panel_game_board.Controls.Add(this.check_card);
            this.panel_game_board.Controls.Add(this.pictureBox1);
            this.panel_game_board.Controls.Add(this.seconds);
            this.panel_game_board.Controls.Add(this.round);
            this.panel_game_board.Controls.Add(this.player2);
            this.panel_game_board.Controls.Add(this.player1);
            this.panel_game_board.Location = new System.Drawing.Point(21, 12);
            this.panel_game_board.Name = "panel_game_board";
            this.panel_game_board.Size = new System.Drawing.Size(900, 311);
            this.panel_game_board.TabIndex = 0;
            this.panel_game_board.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // tableLayout_BoardCards
            // 
            this.tableLayout_BoardCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayout_BoardCards.ColumnCount = 5;
            this.tableLayout_BoardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayout_BoardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayout_BoardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayout_BoardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayout_BoardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayout_BoardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayout_BoardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayout_BoardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayout_BoardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayout_BoardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayout_BoardCards.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayout_BoardCards.Location = new System.Drawing.Point(25, 89);
            this.tableLayout_BoardCards.Name = "tableLayout_BoardCards";
            this.tableLayout_BoardCards.RowCount = 1;
            this.tableLayout_BoardCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout_BoardCards.Size = new System.Drawing.Size(704, 190);
            this.tableLayout_BoardCards.TabIndex = 7;
            this.tableLayout_BoardCards.Visible = false;
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
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Teal;
            this.pictureBox1.Location = new System.Drawing.Point(25, 48);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(845, 10);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
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
            // tableLayout_HandCards
            // 
            this.tableLayout_HandCards.ColumnCount = 5;
            this.tableLayout_HandCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayout_HandCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayout_HandCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayout_HandCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayout_HandCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayout_HandCards.Location = new System.Drawing.Point(21, 329);
            this.tableLayout_HandCards.Name = "tableLayout_HandCards";
            this.tableLayout_HandCards.RowCount = 2;
            this.tableLayout_HandCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95.50562F));
            this.tableLayout_HandCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.494382F));
            this.tableLayout_HandCards.Size = new System.Drawing.Size(886, 264);
            this.tableLayout_HandCards.TabIndex = 1;
            this.tableLayout_HandCards.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // GamePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.ClientSize = new System.Drawing.Size(942, 493);
            this.Controls.Add(this.tableLayout_HandCards);
            this.Controls.Add(this.panel_game_board);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GamePage";
            this.Text = "GamePage";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GamePage_FormClosed);
            this.Load += new System.EventHandler(this.GamePage_Load);
            this.panel_game_board.ResumeLayout(false);
            this.panel_game_board.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_game_board;
        private System.Windows.Forms.Label seconds;
        private System.Windows.Forms.Label round;
        private System.Windows.Forms.Label player2;
        private System.Windows.Forms.Label player1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button cancel_card;
        private System.Windows.Forms.Button check_card;
        private System.Windows.Forms.TableLayoutPanel tableLayout_HandCards;
        private System.Windows.Forms.TableLayoutPanel tableLayout_BoardCards;
    }
}