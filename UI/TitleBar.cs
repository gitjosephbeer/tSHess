using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using tSHess.Font;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace tSHess.UI
{
    public class TitleBar : Control
    {
        public TitleBar()
        {
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.m_controlLeft1Hover = false;
            this.m_controlLeft2Hover = false;
            this.m_controlLeft3Hover = false;
            this.m_controlRight1Hover = false;
            this.m_controlRight2Hover = false;
            this.m_controlRight3Hover = false;

            Graphics g = this.CreateGraphics();
            Region test = null;

            if (this.m_controlLeft1 && this.m_controlLeft1Region != null)
            {
                test = this.m_controlLeft1Region.Clone();
                test.Intersect(this.RectangleToClient(new Rectangle(Control.MousePosition, new Size(1, 1))));
                if (!test.IsEmpty(g))
                {
                    this.m_controlLeft1Hover = true;
                }
            }

            if (this.m_controlLeft2 && this.m_controlLeft2Region != null)
            {
                test = this.m_controlLeft2Region.Clone();
                test.Intersect(this.RectangleToClient(new Rectangle(Control.MousePosition, new Size(1, 1))));
                if (!test.IsEmpty(g))
                {
                    this.m_controlLeft2Hover = true;
                }
            }

            if (this.m_controlLeft3 && this.m_controlLeft3Region != null)
            {
                test = this.m_controlLeft3Region.Clone();
                test.Intersect(this.RectangleToClient(new Rectangle(Control.MousePosition, new Size(1, 1))));
                if (!test.IsEmpty(g))
                {
                    this.m_controlLeft3Hover = true;
                }
            }

            if (this.m_controlRight1 && this.RectangleToScreen(this.m_controlRight1Rect).Contains(Control.MousePosition))
            {
                this.m_controlRight1Hover = true;
            }
            if (this.m_controlRight2 && this.RectangleToScreen(this.m_controlRight2Rect).Contains(Control.MousePosition))
            {
                this.m_controlRight2Hover = true;
            }
            if (this.m_controlRight3 && this.RectangleToScreen(this.m_controlRight3Rect).Contains(Control.MousePosition))
            {
                this.m_controlRight3Hover = true;
            }
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            //this.m_controlLeftXHover = false;

            this.m_controlLeft1Hover = false;
            this.m_controlLeft2Hover = false;
            this.m_controlLeft3Hover = false;
            this.m_controlRight1Hover = false;
            this.m_controlRight2Hover = false;
            this.m_controlRight3Hover = false;
            this.Invalidate();
        }

        private static StringFormat m_stringFormat;

        static TitleBar()
        {
            m_stringFormat = new StringFormat(StringFormat.GenericDefault);
            m_stringFormat.Alignment = StringAlignment.Center;
            m_stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoFontFallback | StringFormatFlags.FitBlackBox;
            m_stringFormat.LineAlignment = StringAlignment.Center;
            m_stringFormat.Trimming = StringTrimming.EllipsisCharacter;
        }

        private Color m_controlColorLeft1 = Color.OrangeRed;

        public Color ControlColorLeft1
        {
            get
            {
                return this.m_controlColorLeft1;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlColorLeft1)
                {
                    invalidateNeeded = true;
                }
                this.m_controlColorLeft1 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        private Color m_controlColorLeft2 = Color.Red;

        public Color ControlColorLeft2
        {
            get
            {
                return this.m_controlColorLeft2;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlColorLeft2)
                {
                    invalidateNeeded = true;
                }
                this.m_controlColorLeft2 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        private Color m_controlColorLeft3 = Color.Blue;

        public Color ControlColorLeft3
        {
            get
            {
                return this.m_controlColorLeft3;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlColorLeft3)
                {
                    invalidateNeeded = true;
                }
                this.m_controlColorLeft3 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        private Color m_controlColorRight1 = Color.MidnightBlue;

        public Color ControlColorRight1
        {
            get
            {
                return this.m_controlColorRight1;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlColorRight1)
                {
                    invalidateNeeded = true;
                }
                this.m_controlColorRight1 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        private Color m_controlColorRight2 = Color.Purple;

        public Color ControlColorRight2
        {
            get
            {
                return this.m_controlColorRight2;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlColorRight2)
                {
                    invalidateNeeded = true;
                }
                this.m_controlColorRight2 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        private Color m_controlColorRight3 = Color.SeaGreen;

        public Color ControlColorRight3
        {
            get
            {
                return this.m_controlColorRight3;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlColorRight3)
                {
                    invalidateNeeded = true;
                }
                this.m_controlColorRight3 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        //private Region m_controlLeftXRegion = null;
        //private bool m_controlLeftXHover = false;

        private Region m_controlLeft1Region = null;
        private Region m_controlLeft2Region = null;
        private Region m_controlLeft3Region = null;
        private Region m_controlRight1Region = null;
        private Region m_controlRight2Region = null;
        private Region m_controlRight3Region = null;

        //private Rectangle m_controlLeft1Rect = Rectangle.Empty;
        private Rectangle m_controlLeft2Rect = Rectangle.Empty;
        private Rectangle m_controlLeft3Rect = Rectangle.Empty;
        private Rectangle m_controlRight1Rect = Rectangle.Empty;
        private Rectangle m_controlRight2Rect = Rectangle.Empty;
        private Rectangle m_controlRight3Rect = Rectangle.Empty;

        private bool m_controlLeft1Hover = false;
        private bool m_controlLeft2Hover = false;
        private bool m_controlLeft3Hover = false;
        private bool m_controlRight1Hover = false;
        private bool m_controlRight2Hover = false;
        private bool m_controlRight3Hover = false;

        private bool m_controlLeft1 = true;

        public bool ControlLeft1
        {
            get
            {
                return this.m_controlLeft1;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlLeft1)
                {
                    invalidateNeeded = true;
                }
                this.m_controlLeft1 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        private bool m_controlLeft2 = true;

        public bool ControlLeft2
        {
            get
            {
                return this.m_controlLeft2;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlLeft2)
                {
                    invalidateNeeded = true;
                }
                this.m_controlLeft2 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        private bool m_controlLeft3 = true;

        public bool ControlLeft3
        {
            get
            {
                return this.m_controlLeft3;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlLeft3)
                {
                    invalidateNeeded = true;
                }
                this.m_controlLeft3 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        private bool m_controlRight1 = true;

        public bool ControlRight1
        {
            get
            {
                return this.m_controlRight1;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlRight1)
                {
                    invalidateNeeded = true;
                }
                this.m_controlRight1 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        private bool m_controlRight2 = true;

        public bool ControlRight2
        {
            get
            {
                return this.m_controlRight2;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlRight2)
                {
                    invalidateNeeded = true;
                }
                this.m_controlRight2 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        private bool m_controlRight3 = true;

        public bool ControlRight3
        {
            get
            {
                return this.m_controlRight3;
            }
            set
            {
                bool invalidateNeeded = false;
                if (value != this.m_controlRight3)
                {
                    invalidateNeeded = true;
                }
                this.m_controlRight3 = value;
                if (invalidateNeeded)
                {
                    this.Invalidate();
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (e.ClipRectangle.Equals(Rectangle.Empty))
            {
                return;
            }
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            Rectangle rect = e.ClipRectangle;
            Rectangle upperleft = rect;
            upperleft.Width -= 2;
            upperleft.Height -= 2;
            Rectangle lowerright = rect;
            lowerright.Width -= 2;
            lowerright.Height -= 2;
            lowerright.Offset(2, 2);
            Rectangle middle = rect;
            middle.Width -= 2;
            middle.Height -= 2;
            middle.Offset(1, 1);

            System.Drawing.Color c = this.BackColor;
            System.Drawing.Color cul = Extensions.Color.ColorFromAhsb(255, c.GetHue(), c.GetSaturation(), 0.9f);
            System.Drawing.Color clr = Extensions.Color.ColorFromAhsb(255, c.GetHue(), c.GetSaturation(), 0.25f);
            System.Drawing.Color cc = this.m_controlColorLeft1;
            System.Drawing.Color ccul = Extensions.Color.ColorFromAhsb(255, cc.GetHue(), cc.GetSaturation(), 0.9f);
            System.Drawing.Color cclr = Extensions.Color.ColorFromAhsb(255, cc.GetHue(), cc.GetSaturation(), 0.25f);
            System.Drawing.Color ccl1 = this.m_controlColorLeft1;
            System.Drawing.Color ccull1 = Extensions.Color.ColorFromAhsb(255, ccl1.GetHue(), ccl1.GetSaturation(), 0.9f);
            System.Drawing.Color cclrl1 = Extensions.Color.ColorFromAhsb(255, ccl1.GetHue(), ccl1.GetSaturation(), 0.25f);
            System.Drawing.Color ccl2 = this.m_controlColorLeft2;
            System.Drawing.Color ccull2 = Extensions.Color.ColorFromAhsb(255, ccl2.GetHue(), ccl2.GetSaturation(), 0.9f);
            System.Drawing.Color cclrl2 = Extensions.Color.ColorFromAhsb(255, ccl2.GetHue(), ccl2.GetSaturation(), 0.25f);
            System.Drawing.Color ccl3 = this.m_controlColorLeft3;
            System.Drawing.Color ccull3 = Extensions.Color.ColorFromAhsb(255, ccl3.GetHue(), ccl3.GetSaturation(), 0.9f);
            System.Drawing.Color cclrl3 = Extensions.Color.ColorFromAhsb(255, ccl3.GetHue(), ccl3.GetSaturation(), 0.25f);
            System.Drawing.Color ccr1 = this.m_controlColorRight1;
            System.Drawing.Color cculr1 = Extensions.Color.ColorFromAhsb(255, ccr1.GetHue(), ccr1.GetSaturation(), 0.9f);
            System.Drawing.Color cclrr1 = Extensions.Color.ColorFromAhsb(255, ccr1.GetHue(), ccr1.GetSaturation(), 0.25f);
            System.Drawing.Color ccr2 = this.m_controlColorRight2;
            System.Drawing.Color cculr2 = Extensions.Color.ColorFromAhsb(255, ccr2.GetHue(), ccr2.GetSaturation(), 0.9f);
            System.Drawing.Color cclrr2 = Extensions.Color.ColorFromAhsb(255, ccr2.GetHue(), ccr2.GetSaturation(), 0.25f);
            System.Drawing.Color ccr3 = this.m_controlColorRight3;
            System.Drawing.Color cculr3 = Extensions.Color.ColorFromAhsb(255, ccr3.GetHue(), ccr3.GetSaturation(), 0.9f);
            System.Drawing.Color cclrr3 = Extensions.Color.ColorFromAhsb(255, ccr3.GetHue(), ccr3.GetSaturation(), 0.25f);

            GraphicsPath path = null;
            path = RoundedRectangle.Create(upperleft, middle.Height / 2 - 1, RectangleCorners.All);
            using (SolidBrush brush = new SolidBrush(cul))
            {
                e.Graphics.FillPath(brush, path);
            }
            path = RoundedRectangle.Create(lowerright, middle.Height / 2 - 1, RectangleCorners.All);
            using (SolidBrush brush = new SolidBrush(clr))
            {
                e.Graphics.FillPath(brush, path);
            }
            path = RoundedRectangle.Create(middle, middle.Height / 2 - 1, RectangleCorners.All);
            using (LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, middle.Height * 6 / 5), this.BackColor, cul))
            {
                e.Graphics.FillPath(brush, path);
            }

            Rectangle aqua = middle;
            aqua.Inflate(-(middle.Height / 3), -(middle.Height / 5));
            aqua.Height = aqua.Height * 2 / 3;
            path = RoundedRectangle.Create(aqua, 3, RectangleCorners.All);
            using (LinearGradientBrush brush = new LinearGradientBrush(aqua, Color.FromArgb(220, cul), Color.FromArgb(0, cul), LinearGradientMode.Vertical))
            {
                e.Graphics.FillPath(brush, path); //draw top bubble
            }

            // Text
            using (SolidBrush brush = new SolidBrush(cul))
            {
                e.Graphics.DrawString(this.Text, this.Font, brush, new RectangleF(rect.X + 1, rect.Y + 1, rect.Width, rect.Height), m_stringFormat);
            }
            using (SolidBrush brush = new SolidBrush(clr))
            {
                e.Graphics.DrawString(this.Text, this.Font, brush, new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), m_stringFormat);
            }

            Rectangle btn = Rectangle.Empty;



            btn = middle;
            btn.X += btn.Height;
            btn.Inflate(0, -1);
            btn.Width = btn.Height + btn.Height / 5 * 2;
            using (LinearGradientBrush brush = new LinearGradientBrush(btn, cul, clr, LinearGradientMode.Vertical))
            {
                e.Graphics.FillEllipse(brush, btn);
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(btn, Color.FromArgb(10, cul), Color.FromArgb(180, cul), LinearGradientMode.Horizontal))
            {
                e.Graphics.FillEllipse(brush, btn);
            }
            using (Pen pen = new Pen(Color.FromArgb(35, clr)))
            {
                e.Graphics.DrawEllipse(pen, btn);
            }
            btn.Inflate(-(btn.Height / 5 + 1), -1);

            path = new GraphicsPath();
            path.AddEllipse(btn);
            this.m_controlLeft1Region = new Region(path);

            using (LinearGradientBrush brush = new LinearGradientBrush(btn, this.m_controlLeft1Hover ? cclrl1 : clr, this.m_controlLeft1Hover ? ccull1 : cul, LinearGradientMode.BackwardDiagonal))
            {
                e.Graphics.FillEllipse(brush, btn);
            }
            using (Pen pen = new Pen(this.m_controlLeft1Hover ? Color.FromArgb(75, cclrl1) : Color.FromArgb(75, clr)))
            {
                e.Graphics.DrawEllipse(pen, btn);
            }
            btn.Inflate(-1, -1);
            btn.X += 1;
            btn.Y += 1;
            using (LinearGradientBrush brush = new LinearGradientBrush(btn, this.m_controlLeft1Hover ? Color.FromArgb(0, ccull1) : Color.FromArgb(0, cul), this.m_controlLeft1Hover ? Color.FromArgb(100, ccull1) : Color.FromArgb(100, cul), LinearGradientMode.BackwardDiagonal))
            {
                e.Graphics.FillEllipse(brush, btn);
            }



            btn = middle;
            btn.X += btn.Height + btn.Height + btn.Height / 5 * 2;
            btn.Inflate(0, -1);
            btn.Width = btn.Height + btn.Height / 5 * 2;
            using (LinearGradientBrush brush = new LinearGradientBrush(btn, cul, clr, LinearGradientMode.Vertical))
            {
                e.Graphics.FillEllipse(brush, btn);
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(btn, Color.FromArgb(10, cul), Color.FromArgb(180, cul), LinearGradientMode.Horizontal))
            {
                e.Graphics.FillEllipse(brush, btn);
            }
            using (Pen pen = new Pen(Color.FromArgb(35, clr)))
            {
                e.Graphics.DrawEllipse(pen, btn);
            }
            btn.Inflate(-(btn.Height / 5 + 1), -1);

            path = new GraphicsPath();
            path.AddEllipse(btn);
            this.m_controlLeft2Region = new Region(path);

            using (LinearGradientBrush brush = new LinearGradientBrush(btn, this.m_controlLeft2Hover ? cclrl2 : clr, this.m_controlLeft2Hover ? ccull2 : cul, LinearGradientMode.BackwardDiagonal))
            {
                e.Graphics.FillEllipse(brush, btn);
            }
            using (Pen pen = new Pen(this.m_controlLeft2Hover ? Color.FromArgb(75, cclrl2) : Color.FromArgb(75, clr)))
            {
                e.Graphics.DrawEllipse(pen, btn);
            }
            btn.Inflate(-1, -1);
            btn.X += 1;
            btn.Y += 1;
            using (LinearGradientBrush brush = new LinearGradientBrush(btn, this.m_controlLeft2Hover ? Color.FromArgb(0, ccull2) : Color.FromArgb(0, cul), this.m_controlLeft2Hover ? Color.FromArgb(100, ccull2) : Color.FromArgb(100, cul), LinearGradientMode.BackwardDiagonal))
            {
                e.Graphics.FillEllipse(brush, btn);
            }



            btn = middle;
            btn.X += btn.Height + (btn.Height + btn.Height / 5 * 2) * 2;
            btn.Inflate(0, -1);
            btn.Width = btn.Height + btn.Height / 5 * 2;
            using (LinearGradientBrush brush = new LinearGradientBrush(btn, cul, clr, LinearGradientMode.Vertical))
            {
                e.Graphics.FillEllipse(brush, btn);
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(btn, Color.FromArgb(10, cul), Color.FromArgb(180, cul), LinearGradientMode.Horizontal))
            {
                e.Graphics.FillEllipse(brush, btn);
            }
            using (Pen pen = new Pen(Color.FromArgb(35, clr)))
            {
                e.Graphics.DrawEllipse(pen, btn);
            }
            btn.Inflate(-(btn.Height / 5 + 1), -1);

            path = new GraphicsPath();
            path.AddEllipse(btn);
            this.m_controlLeft3Region = new Region(path);

            using (LinearGradientBrush brush = new LinearGradientBrush(btn, this.m_controlLeft3Hover ? cclrl3 : clr, this.m_controlLeft3Hover ? ccull3 : cul, LinearGradientMode.BackwardDiagonal))
            {
                e.Graphics.FillEllipse(brush, btn);
            }
            using (Pen pen = new Pen(this.m_controlLeft3Hover ? Color.FromArgb(75, cclrl3) : Color.FromArgb(75, clr)))
            {
                e.Graphics.DrawEllipse(pen, btn);
            }
            btn.Inflate(-1, -1);
            btn.X += 1;
            btn.Y += 1;
            using (LinearGradientBrush brush = new LinearGradientBrush(btn, this.m_controlLeft3Hover ? Color.FromArgb(0, ccull3) : Color.FromArgb(0, cul), this.m_controlLeft3Hover ? Color.FromArgb(100, ccull3) : Color.FromArgb(100, cul), LinearGradientMode.BackwardDiagonal))
            {
                e.Graphics.FillEllipse(brush, btn);
            }


            //cupperleft = upperleft;
            //cupperleft.X += 15;
            //cupperleft.Width = 12;
            //clowerright = lowerright;
            //clowerright.X += 15;
            //clowerright.Width = 12;
            //cmiddle = middle;
            //cmiddle.X += 14;
            //cmiddle.Width = 13;
            //this.m_controlLeft2Rect = Rectangle.Union(cupperleft, clowerright);
            //if (this.m_controlLeft2 && this.m_controlLeft2Hover)
            //{
            //    using (SolidBrush brush = new SolidBrush(ccull2))
            //    {
            //        e.Graphics.FillRectangle(brush, cupperleft);
            //    }
            //    using (SolidBrush brush = new SolidBrush(cclrl2))
            //    {
            //        e.Graphics.FillRectangle(brush, clowerright);
            //    }
            //    using (LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, cmiddle.Height + 10), this.ControlColorLeft2, ccull2))
            //    {
            //        e.Graphics.FillRectangle(brush, cmiddle);
            //    }

            //    caqua = cmiddle;
            //    caqua.Inflate(0, -3);
            //    caqua.Height = caqua.Height * 2 / 3;
            //    path = RoundedRectangle.Create(caqua, 3, RectangleCorners.None);
            //    using (LinearGradientBrush brush = new LinearGradientBrush(caqua, Color.FromArgb(255, ccull2), Color.FromArgb(0, ccull2), LinearGradientMode.Vertical))
            //    {
            //        e.Graphics.FillPath(brush, path); //draw top bubble
            //    }
            //}
            //cupperleft = upperleft;
            //cupperleft.X += 29;
            //cupperleft.Width = 12;
            //clowerright = lowerright;
            //clowerright.X += 29;
            //clowerright.Width = 12;
            //cmiddle = middle;
            //cmiddle.X += 28;
            //cmiddle.Width = 13;
            //this.m_controlLeft3Rect = Rectangle.Union(cupperleft, clowerright);
            //if (this.m_controlLeft3 && this.m_controlLeft3Hover)
            //{
            //    using (SolidBrush brush = new SolidBrush(ccull3))
            //    {
            //        e.Graphics.FillRectangle(brush, cupperleft);
            //    }
            //    using (SolidBrush brush = new SolidBrush(cclrl3))
            //    {
            //        e.Graphics.FillRectangle(brush, clowerright);
            //    }
            //    using (LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, cmiddle.Height + 10), this.ControlColorLeft3, ccull3))
            //    {
            //        e.Graphics.FillRectangle(brush, cmiddle);
            //    }

            //    caqua = cmiddle;
            //    caqua.Inflate(0, -3);
            //    caqua.Height = caqua.Height * 2 / 3;
            //    path = RoundedRectangle.Create(caqua, 3, RectangleCorners.None);
            //    using (LinearGradientBrush brush = new LinearGradientBrush(caqua, Color.FromArgb(255, ccull3), Color.FromArgb(0, ccull3), LinearGradientMode.Vertical))
            //    {
            //        e.Graphics.FillPath(brush, path); //draw top bubble
            //    }
            //}





            // *****************************
            //try
            //{
            //    Rectangle sr = middle;
            //    sr.X += 45;
            //    sr.Inflate(0, -1);
            //    sr.Width = sr.Height + sr.Height / 5 * 2;
            //    using (LinearGradientBrush brush = new LinearGradientBrush(sr, cul, clr, LinearGradientMode.BackwardDiagonal))
            //    {
            //        e.Graphics.FillEllipse(brush, sr);
            //    }
            //    using (Pen pen = new Pen(Color.FromArgb(40, clr)))
            //    {
            //        e.Graphics.DrawEllipse(pen, sr);
            //    }
            //    sr.Inflate(-(sr.Height / 5 + 1), -1);

            //    path = new GraphicsPath();
            //    path.AddEllipse(sr);
            //    this.m_controlLeftXRegion = new Region(path);

            //    using (LinearGradientBrush brush = new LinearGradientBrush(sr, this.m_controlLeftXHover ? ccull3 : cul, this.m_controlLeftXHover ? cclrl3 : clr, LinearGradientMode.ForwardDiagonal))
            //    {
            //        e.Graphics.FillEllipse(brush, sr);
            //    }
            //    using (Pen pen = new Pen(Color.FromArgb(80, cclrl3)))
            //    {
            //        e.Graphics.DrawEllipse(pen, sr);
            //    }
            //    sr.Inflate(-1, -1);
            //    sr.X += 1;
            //    sr.Y += 1;
            //    using (LinearGradientBrush brush = new LinearGradientBrush(sr, Color.FromArgb(100, ccull3), Color.FromArgb(0, ccull3), LinearGradientMode.BackwardDiagonal))
            //    {
            //        e.Graphics.FillEllipse(brush, sr);
            //    }
            //}
            //catch { }





            //cupperleft = upperleft;
            //cupperleft.X += cupperleft.Width - 13;
            //cupperleft.Width = 13;
            //clowerright = lowerright;
            //clowerright.X += clowerright.Width - 13;
            //clowerright.Width = 13;
            //cmiddle = middle;
            //cmiddle.X += cmiddle.Width - 14;
            //cmiddle.Width = 14;
            //this.m_controlRight1Rect = Rectangle.Union(cupperleft, clowerright);
            //if (this.m_controlRight1 && this.m_controlRight1Hover)
            //{
            //    path = RoundedRectangle.Create(cupperleft, 4, RectangleCorners.TopRight | RectangleCorners.BottomRight);
            //    using (SolidBrush brush = new SolidBrush(cculr1))
            //    {
            //        e.Graphics.FillPath(brush, path);
            //    }
            //    path = RoundedRectangle.Create(clowerright, 4, RectangleCorners.TopRight | RectangleCorners.BottomRight);
            //    using (SolidBrush brush = new SolidBrush(cclrr1))
            //    {
            //        e.Graphics.FillPath(brush, path);
            //    }
            //    path = RoundedRectangle.Create(cmiddle, 4, RectangleCorners.TopRight | RectangleCorners.BottomRight);
            //    using (LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, cmiddle.Height + 10), this.ControlColorRight1, cculr1))
            //    {
            //        e.Graphics.FillPath(brush, path);
            //    }

            //    caqua = cmiddle;
            //    caqua.Inflate(0, -3);
            //    caqua.Width -= 4;
            //    caqua.Height = caqua.Height * 2 / 3;
            //    path = RoundedRectangle.Create(caqua, 3, RectangleCorners.TopRight | RectangleCorners.BottomRight);
            //    using (LinearGradientBrush brush = new LinearGradientBrush(caqua, Color.FromArgb(255, cculr1), Color.FromArgb(0, cculr1), LinearGradientMode.Vertical))
            //    {
            //        e.Graphics.FillPath(brush, path); //draw top bubble
            //    }

            //}
            //cupperleft = upperleft;
            //cupperleft.X += cupperleft.Width - 13 - 15;
            //cupperleft.Width = 12;
            //clowerright = lowerright;
            //clowerright.X += clowerright.Width - 13 - 15;
            //clowerright.Width = 12;
            //cmiddle = middle;
            //cmiddle.X += cmiddle.Width - 13 - 16;
            //cmiddle.Width = 13;
            //this.m_controlRight2Rect = Rectangle.Union(cupperleft, clowerright);
            //if (this.m_controlRight2 && this.m_controlRight2Hover)
            //{
            //    using (SolidBrush brush = new SolidBrush(cculr2))
            //    {
            //        e.Graphics.FillRectangle(brush, cupperleft);
            //    }
            //    using (SolidBrush brush = new SolidBrush(cclrr2))
            //    {
            //        e.Graphics.FillRectangle(brush, clowerright);
            //    }
            //    using (LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, cmiddle.Height + 10), this.ControlColorRight2, cculr2))
            //    {
            //        e.Graphics.FillRectangle(brush, cmiddle);
            //    }

            //    caqua = cmiddle;
            //    caqua.Inflate(0, -3);
            //    caqua.Height = caqua.Height * 2 / 3;
            //    path = RoundedRectangle.Create(caqua, 3, RectangleCorners.None);
            //    using (LinearGradientBrush brush = new LinearGradientBrush(caqua, Color.FromArgb(255, cculr2), Color.FromArgb(0, cculr2), LinearGradientMode.Vertical))
            //    {
            //        e.Graphics.FillPath(brush, path); //draw top bubble
            //    }
            //}
            //cupperleft = upperleft;
            //cupperleft.X += cupperleft.Width - 13 - 15 -15;
            //cupperleft.Width = 12;
            //clowerright = lowerright;
            //clowerright.X += clowerright.Width - 13 - 15 -15;
            //clowerright.Width = 12;
            //cmiddle = middle;
            //cmiddle.X += cmiddle.Width - 13 - 15 -16;
            //cmiddle.Width = 13;
            //this.m_controlRight3Rect = Rectangle.Union(cupperleft, clowerright);
            //if (this.m_controlRight3 && this.m_controlRight3Hover)
            //{
            //    using (SolidBrush brush = new SolidBrush(cculr3))
            //    {
            //        e.Graphics.FillRectangle(brush, cupperleft);
            //    }
            //    using (SolidBrush brush = new SolidBrush(cclrr3))
            //    {
            //        e.Graphics.FillRectangle(brush, clowerright);
            //    }
            //    using (LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, cmiddle.Height + 10), this.ControlColorRight3, cculr3))
            //    {
            //        e.Graphics.FillRectangle(brush, cmiddle);
            //    }

            //    caqua = cmiddle;
            //    caqua.Inflate(0, -3);
            //    caqua.Height = caqua.Height * 2 / 3;
            //    path = RoundedRectangle.Create(caqua, 3, RectangleCorners.None);
            //    using (LinearGradientBrush brush = new LinearGradientBrush(caqua, Color.FromArgb(255, cculr3), Color.FromArgb(0, cculr3), LinearGradientMode.Vertical))
            //    {
            //        e.Graphics.FillPath(brush, path); //draw top bubble
            //    }
            //}
        }
    }
}
