using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using tSHess.Font;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace tSHess.UI
{
    public partial class Board : Form
    {
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (Control.MouseButtons == MouseButtons.None)
            {
                if (!this.m_rolledUp)
                {
                    if (e.Delta > 0)
                    {
                        if (this.chessBoard1.FontSize < 60)
                        {
                            this.chessBoard1.Visible = false;
                            this.chessBoard1.FontSize++;
                            this.chessBoard1.Visible = true;
                        }
                    }
                    else
                    {
                        if (this.chessBoard1.FontSize > 14)
                        {
                            this.chessBoard1.Visible = false;
                            this.chessBoard1.FontSize--;
                            this.chessBoard1.Visible = true;
                        }
                    }
                }
            }
            else if (Control.MouseButtons == MouseButtons.Right)
            {
                if (e.Delta > 0)
                {
                    if (this.Opacity < 1.0d)
                    {
                        this.Opacity += 0.05d;
                    }
                }
                else
                {
                    if (this.Opacity > 0.1d)
                    {
                        this.Opacity -= 0.05d;
                    }
                }
            }
        }

        private bool m_rolledUp = true;
        private bool m_rolling = false;

        private void RollUp()
        {
            if (this.m_rolling || this.m_rolledUp)
            {
                return;
            }
            this.m_rolling = true;
            try
            {
                int counter = 0;
                int height = this.chessBoard1.OptimalBoardSize.Height;
                this.chessBoard1.Visible = false;
                this.chessBoard1.Location = new Point(0, this.titleBar.Height);
                this.chessBoard1.Height = height;
                this.chessBoard1.Width = this.chessBoard1.OptimalBoardSize.Width;
                this.chessBoard1.Visible = true;
                while (height > 0)
                {
                    counter += 200;
                    height = (int)((1d - Math.Log(counter, 10000)) * (double)this.chessBoard1.OptimalBoardSize.Height);
                    this.chessBoard1.Height = Math.Max(height, 0);
                    Rectangle rc = new Rectangle(this.chessBoard1.Location, this.chessBoard1.Size);
                    this.Invalidate(rc, true);
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(10);
                }
            }
            finally
            {
                this.m_rolledUp = true;
                this.m_rolling = false;
            }
        }

        private void RollDown()
        {
            if (this.m_rolling || !this.m_rolledUp)
            {
                return;
            }
            this.m_rolling = true;
            try
            {
                int counter = 0;
                int height = 0;
                this.chessBoard1.Visible = false;
                this.chessBoard1.Location = new Point(0, this.titleBar.Height);
                this.chessBoard1.Height = height;
                this.chessBoard1.Width = this.chessBoard1.OptimalBoardSize.Width;
                this.chessBoard1.Visible = true;
                while (height < this.chessBoard1.OptimalBoardSize.Height)
                {
                    counter += 200;
                    height = (int)(Math.Log(counter, 10000) * (double)this.chessBoard1.OptimalBoardSize.Height);
                    this.chessBoard1.Height = Math.Min(height, this.chessBoard1.OptimalBoardSize.Height);
                    Rectangle rc = new Rectangle(this.chessBoard1.Location, this.chessBoard1.Size);
                    this.Invalidate(rc, true);
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(10);
                }
            }
            finally
            {
                this.m_rolledUp = false;
                this.m_rolling = false;
            }
        }

        private void ToggleRoll()
        {
            if (this.m_rolledUp)
            {
                this.RollDown();
            }
            else
            {
                this.RollUp();
            }
        }

        public Board()
        {
            InitializeComponent();

            this.chessBoard1.OptimalBoardSizeChanged += delegate(object _sender, EventArgs _e)
            {
                this.ClientSize = new Size(this.chessBoard1.OptimalBoardSize.Width, this.chessBoard1.OptimalBoardSize.Height + titleBar.Height);
            };
            this.chessBoard1.FieldClicked += delegate(object _sender, FieldClickedEventArgs _e)
            {
                MessageBox.Show(_e.HCoordinate.ToString() + "-" + _e.VCoordinate.ToString() + ": " + _e.FieldIndex.ToString());
            };
            this.titleBar.DoubleClick += delegate(object _sender, EventArgs _e)
            {
                this.ToggleRoll();
            };
        }

        private void Board_Load(object sender, EventArgs e)
        {
            this.chessBoard1.Text = //ChessFonts.GetInitialSetupString(chessBoard1.ChessFont);
                ChessFonts.GetInitialSetupWithoutBorderString(chessBoard1.ChessFont);

            this.RollDown();
        }

        private void titleBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.m_start.HasValue)
            {
                int deltaX = e.X - this.m_start.Value.X;
                int deltaY = e.Y - this.m_start.Value.Y;
                this.m_start = null;
                this.Location = new Point(this.Location.X + deltaX, this.Location.Y + deltaY);
                this.Cursor = Cursors.Default;
            }
        }

        private Nullable<Point> m_start = null;

        private void titleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.m_start = e.Location;
            }
        }

        private void titleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.m_start.HasValue)
            {
                this.Cursor = Cursors.SizeAll;
            }
        }
    }
}
