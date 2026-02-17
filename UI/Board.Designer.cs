namespace tSHess.UI
{
    partial class Board
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
            this.chessBoard1 = new tSHess.UI.ChessBoard();
            this.titleBar = new tSHess.UI.TitleBar();
            this.SuspendLayout();
            // 
            // chessBoard1
            // 
            this.chessBoard1.AutoSize = true;
            this.chessBoard1.BackColor = System.Drawing.SystemColors.Control;
            this.chessBoard1.BoardColor = System.Drawing.Color.GhostWhite;
            this.chessBoard1.ChessFont = tSHess.Font.ChessFont.Zurich;
            this.chessBoard1.FontSize = 23F;
            this.chessBoard1.Location = new System.Drawing.Point(12, 19);
            this.chessBoard1.Name = "chessBoard1";
            this.chessBoard1.Size = new System.Drawing.Size(71, 38);
            this.chessBoard1.TabIndex = 1;
            this.chessBoard1.Visible = false;
            // 
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.DimGray;
            this.titleBar.ControlColorLeft1 = System.Drawing.Color.Green;
            this.titleBar.ControlColorLeft2 = System.Drawing.Color.Red;
            this.titleBar.ControlColorLeft3 = System.Drawing.Color.Blue;
            this.titleBar.ControlColorRight1 = System.Drawing.Color.MidnightBlue;
            this.titleBar.ControlColorRight2 = System.Drawing.Color.Purple;
            this.titleBar.ControlColorRight3 = System.Drawing.Color.SeaGreen;
            this.titleBar.ControlLeft1 = true;
            this.titleBar.ControlLeft2 = true;
            this.titleBar.ControlLeft3 = true;
            this.titleBar.ControlRight1 = true;
            this.titleBar.ControlRight2 = true;
            this.titleBar.ControlRight3 = true;
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.titleBar.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(112, 19);
            this.titleBar.TabIndex = 0;
            this.titleBar.Text = "C#ESS";
            this.titleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseMove);
            this.titleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseDown);
            this.titleBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.titleBar_MouseUp);
            // 
            // Board
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(112, 19);
            this.ControlBox = false;
            this.Controls.Add(this.chessBoard1);
            this.Controls.Add(this.titleBar);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Board";
            this.Opacity = 0.7;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "c#ess";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.Load += new System.EventHandler(this.Board_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private System.Windows.Forms.Label titleBar;
        private TitleBar titleBar;
        private ChessBoard chessBoard1;




    }
}

