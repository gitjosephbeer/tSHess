using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using tSHess.Font;
using System.Diagnostics;
using tSHess.Engine;
using System.Drawing.Drawing2D;

namespace tSHess.UI
{
    public class ChessBoard : Panel
    {
        private string m_text = string.Empty;
        public override string Text
        {
            get
            {
                return this.m_text;
            }
            set
            {
                this.m_text = value;
                this.InvalidateEx();
            }
        }

        private ChessFont m_chessFont = ChessFont.Zurich;
        public ChessFont ChessFont
        {
            get
            {
                return this.m_chessFont;
            }
            set
            {
                this.m_chessFont = value;
                this.InvalidateEx();
            }
        }

        private System.Drawing.Color m_boardColor = System.Drawing.Color.GhostWhite;
        public System.Drawing.Color BoardColor
        {
            get
            {
                return this.m_boardColor;
            }
            set
            {
                this.m_boardColor = value;
                this.InvalidateEx();
            }
        }

        private float m_fontSize = 14;
        public float FontSize
        {
            get
            {
                return this.m_fontSize;
            }
            set
            {
                this.m_fontSize = value;
                this.InvalidateEx();
            }
        }

        private Size m_optimalBoardSize = Size.Empty;
        public Size OptimalBoardSize
        {
            get
            {
                if (this.m_optimalBoardSize.Equals(Size.Empty))
                {
                    this.CalculateOptimalBoardSize();
                }
                return this.m_optimalBoardSize;
            }
            protected set
            {
                this.m_optimalBoardSize = value;
                this.OnOptimalBoardSizeChanged(EventArgs.Empty);
            }
        }

        public event EventHandler OptimalBoardSizeChanged;

        protected virtual void OnOptimalBoardSizeChanged(EventArgs e)
        {
            EventHandler h = this.OptimalBoardSizeChanged;
            if (h != null)
            {
                h(this, e);
            }
        }

        private PointF m_boardOffset;
        private SizeF m_fieldSize;
        private SizeF m_allFieldsSize;

        public ChessBoard()
        {
            this.DoubleBuffered = true;
        }

        private static StringFormat m_stringFormat;

        static ChessBoard()
        {
            m_stringFormat = new StringFormat(StringFormat.GenericTypographic);
            m_stringFormat.Alignment = StringAlignment.Near;
            //m_stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoFontFallback | StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox;
            m_stringFormat.LineAlignment = StringAlignment.Near;
            m_stringFormat.Trimming = StringTrimming.None;
        }

        private void CalculateOptimalBoardSize()
        {
            try
            {
                Graphics g = this.CreateGraphics();

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                System.Drawing.Font f = ChessFonts.GetFont(this.m_chessFont, this.m_fontSize);

                SizeF fullSize = g.MeasureString(ChessFonts.GetInitialSetupString(this.m_chessFont), f, Int16.MaxValue, m_stringFormat);
                this.OptimalBoardSize = new Size((int)Math.Ceiling(fullSize.Width), (int)Math.Ceiling(fullSize.Height));

                //SizeF offsetSize = g.MeasureString(ChessFonts.GetUpperLeftCornerMappingCharacter(this.m_chessFont).ToString(), f, Int16.MaxValue, m_stringFormat);
                SizeF upperBorderSize = g.MeasureString(ChessFonts.GetUpperBorderString(this.m_chessFont), f, Int16.MaxValue, m_stringFormat);
                SizeF leftBoderSize = g.MeasureString(ChessFonts.GetLeftBorderString(this.m_chessFont), f, Int16.MaxValue, m_stringFormat);
                this.m_boardOffset = new PointF(leftBoderSize.Width, upperBorderSize.Height);

                this.m_allFieldsSize = g.MeasureString(ChessFonts.GetInitialSetupWithoutBorderString(this.m_chessFont), f, Int16.MaxValue, m_stringFormat); // using empty black field
                this.m_fieldSize = new SizeF(this.m_allFieldsSize.Width / 8, this.m_allFieldsSize.Height / 8); // g.MeasureString(ChessFonts.GetEmptyBlackFieldMappingCharacter(this.m_chessFont).ToString(), f, Int16.MaxValue, m_stringFormat); // using empty black field
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected void InvalidateEx()
        {
            if (this.IsHandleCreated)
            {
                try
                {
                    this.CalculateOptimalBoardSize();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            if (this.AutoSize)
            {
                this.Size = new Size(this.OptimalBoardSize.Width, this.OptimalBoardSize.Height);
            }
            if (Parent == null)
                return;
            Rectangle rc = new Rectangle(this.Location, this.Size);
            Parent.Invalidate(rc, true);
        }

        public event FieldClickedEventHandler FieldClicked;

        protected virtual void OnFieldClicked(FieldClickedEventArgs e)
        {
            FieldClickedEventHandler h = this.FieldClicked;
            if (h != null)
            {
                h(this, e);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                float x = (float)e.X - this.m_boardOffset.X;
                float y = 8 * this.m_fieldSize.Height - (float)e.Y + this.m_boardOffset.Y;

                if (x < 0 || y < 0 || x >= 8 * this.m_fieldSize.Width || y >= 8 * this.m_fieldSize.Height)
                {
                    return;
                }

                HorizontalCoordinateCode hc = (HorizontalCoordinateCode)(int)Math.Floor(x / this.m_fieldSize.Width);
                VerticalCoordinateCode vc = (VerticalCoordinateCode)(int)Math.Floor(y / this.m_fieldSize.Height);
                FieldClickedEventArgs args = new FieldClickedEventArgs(hc, vc);
                OnFieldClicked(args);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            RectangleF rect = e.ClipRectangle;
            rect.Location = this.m_boardOffset;
            rect.X = rect.X + this.Size.Width - this.OptimalBoardSize.Width;
            rect.Y = rect.Y + this.Size.Height - this.OptimalBoardSize.Height;
            rect.Width = this.m_allFieldsSize.Width; //this.m_fieldSize.Width * 8;
            rect.Height = this.m_allFieldsSize.Height; //this.m_fieldSize.Height * 8;
            using (System.Drawing.SolidBrush brush = new SolidBrush(this.m_boardColor))
            {
                g.FillRectangle(brush, rect);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.DesignMode)
            {
                try
                {
                    Graphics g = e.Graphics;

                    System.Drawing.Font f = ChessFonts.GetFont(this.m_chessFont, this.m_fontSize);

                    using (SolidBrush brush = new SolidBrush(this.ForeColor))
                    {
                        g.DrawString(ChessFonts.GetInitialSetupBorderOnlyString(this.m_chessFont), f, brush, this.Size.Width - this.OptimalBoardSize.Width, this.Size.Height - this.OptimalBoardSize.Height, m_stringFormat);

                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                        g.DrawString(this.m_text, f, brush, (float)this.Size.Width - (float)this.OptimalBoardSize.Width + this.m_boardOffset.X, (float)this.Size.Height - (float)this.OptimalBoardSize.Height + this.m_boardOffset.Y, m_stringFormat);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }
    }


    public delegate void FieldClickedEventHandler(object sender, FieldClickedEventArgs e);

    public class FieldClickedEventArgs : EventArgs
    {
        private HorizontalCoordinateCode m_hCoordinate;
        public HorizontalCoordinateCode HCoordinate
        {
            get
            {
                return this.m_hCoordinate;
            }
        }

        private VerticalCoordinateCode m_vCoordinate;
        public VerticalCoordinateCode VCoordinate
        {
            get
            {
                return this.m_vCoordinate;
            }
        }

        private int m_fieldIndex;
        public int FieldIndex
        {
            get
            {
                return this.m_fieldIndex;
            }
        }

        public FieldClickedEventArgs(HorizontalCoordinateCode hCoordinate, VerticalCoordinateCode vCoordinate)
        {
            this.m_hCoordinate = hCoordinate;
            this.m_vCoordinate = vCoordinate;
            this.m_fieldIndex = (int)hCoordinate * 8 + (int)vCoordinate;
        }
    }
}
