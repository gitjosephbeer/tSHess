using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tSHess.Extensions
{
    public static class Color
    {
        public static System.Drawing.Color ColorFromAhsb(int a, float h, float s, float b)
        {
            if (0 > a || 255 < a)
            {
                throw new ArgumentOutOfRangeException("a", a,
                  "Invalid alpha value");
            }
            if (0f > h || 360f < h)
            {
                throw new ArgumentOutOfRangeException("h", h,
                  "Invalid hue value");
            }
            if (0f > s || 1f < s)
            {
                throw new ArgumentOutOfRangeException("s", s,
                  "Invalid saturation value");
            }
            if (0f > b || 1f < b)
            {
                throw new ArgumentOutOfRangeException("b", b,
                  "Invalid brightness value");
            }

            if (0 == s)
            {
                return System.Drawing.Color.FromArgb(a, Convert.ToInt32(b * 255),
                  Convert.ToInt32(b * 255), Convert.ToInt32(b * 255));
            }

            float fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;

            if (0.5 < b)
            {
                fMax = b - (b * s) + s;
                fMin = b + (b * s) - s;
            }
            else
            {
                fMax = b + (b * s);
                fMin = b - (b * s);
            }

            iSextant = (int)Math.Floor(h / 60f);
            if (300f <= h)
            {
                h -= 360f;
            }
            h /= 60f;
            h -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
            if (0 == iSextant % 2)
            {
                fMid = h * (fMax - fMin) + fMin;
            }
            else
            {
                fMid = fMin - h * (fMax - fMin);
            }

            iMax = Convert.ToInt32(fMax * 255);
            iMid = Convert.ToInt32(fMid * 255);
            iMin = Convert.ToInt32(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return System.Drawing.Color.FromArgb(a, iMid, iMax, iMin);
                case 2:
                    return System.Drawing.Color.FromArgb(a, iMin, iMax, iMid);
                case 3:
                    return System.Drawing.Color.FromArgb(a, iMin, iMid, iMax);
                case 4:
                    return System.Drawing.Color.FromArgb(a, iMid, iMin, iMax);
                case 5:
                    return System.Drawing.Color.FromArgb(a, iMax, iMin, iMid);
                default:
                    return System.Drawing.Color.FromArgb(a, iMax, iMid, iMin);
            }
        }
    }
}
