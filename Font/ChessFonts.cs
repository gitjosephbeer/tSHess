using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace tSHess.Font
{
    public static class ChessFonts
    {
        private static Dictionary<ChessFont, FontFamily> m_fontFamilies;
        private static Dictionary<ChessFont, Dictionary<ChessFontSymbol, char>> m_fontMappings;
        private static readonly int CHUNK_SIZE = 4096;
        private static readonly int DEFAULT_FONT_SIZE = 18;
        private static ChessFontSymbol[] m_initialBoardSetup;
        private static ChessFontSymbol[] m_initialBoardSetupWithoutBorder;
        private static ChessFontSymbol[] m_initialBoardSetupBorderOnly;
        private static ChessFontSymbol[] m_upperBorder;
        private static ChessFontSymbol[] m_leftBorder;
        private static Dictionary<ChessFont, string> m_initialBoardSetupStrings;
        private static Dictionary<ChessFont, string> m_initialBoardSetupWithoutBorderStrings;
        private static Dictionary<ChessFont, string> m_initialBoardSetupBorderOnlyStrings;
        private static Dictionary<ChessFont, string> m_upperBorderStrings;
        private static Dictionary<ChessFont, string> m_leftBorderStrings;

        static ChessFonts()
        {
            InitializeFonts();
            InitializeFontMappings();
        }

        //private static void InitializeStrings()
        //{
        //    m_initialBoardSetupStrings = new Dictionary<ChessFont, string>();



        //    StringBuilder sb = new StringBuilder();
        //    Dictionary<ChessFontSymbol, char> mappings = m_fontMappings[font];
        //    int i = 0;
        //    foreach (ChessFontSymbol symbol in m_initialBoardSetup)
        //    {
        //        sb.Append(mappings[symbol]);
        //        if (++i % 10 == 0)
        //        {
        //            sb.Append("\r\n");
        //        }
        //    }
        //    return sb.ToString();


        //}

        private static void InitializeFontMappings()
        {
            m_initialBoardSetup = new ChessFontSymbol[] {
                ChessFontSymbol.BorderUpperLeftCorner,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,                
                ChessFontSymbol.BorderUpperRightCorner,

                ChessFontSymbol.BorderLeftEdge8,
                ChessFontSymbol.BlackRookOnWhiteField,
                ChessFontSymbol.BlackKnightOnBlackField,
                ChessFontSymbol.BlackBishopOnWhiteField,
                ChessFontSymbol.BlackQueenOnBlackField,
                ChessFontSymbol.BlackKingOnWhiteField,
                ChessFontSymbol.BlackBishopOnBlackField,
                ChessFontSymbol.BlackKnightOnWhiteField,
                ChessFontSymbol.BlackRookOnBlackField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge7,
                ChessFontSymbol.BlackPawnOnBlackField,
                ChessFontSymbol.BlackPawnOnWhiteField,
                ChessFontSymbol.BlackPawnOnBlackField,
                ChessFontSymbol.BlackPawnOnWhiteField,
                ChessFontSymbol.BlackPawnOnBlackField,
                ChessFontSymbol.BlackPawnOnWhiteField,
                ChessFontSymbol.BlackPawnOnBlackField,
                ChessFontSymbol.BlackPawnOnWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge6,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge5,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge4,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge3,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge2,
                ChessFontSymbol.WhitePawnOnWhiteField,
                ChessFontSymbol.WhitePawnOnBlackField,
                ChessFontSymbol.WhitePawnOnWhiteField,
                ChessFontSymbol.WhitePawnOnBlackField,
                ChessFontSymbol.WhitePawnOnWhiteField,
                ChessFontSymbol.WhitePawnOnBlackField,
                ChessFontSymbol.WhitePawnOnWhiteField,
                ChessFontSymbol.WhitePawnOnBlackField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge1,
                ChessFontSymbol.WhiteRookOnBlackField,
                ChessFontSymbol.WhiteKnightOnWhiteField,
                ChessFontSymbol.WhiteBishopOnBlackField,
                ChessFontSymbol.WhiteQueenOnWhiteField,
                ChessFontSymbol.WhiteKingOnBlackField,
                ChessFontSymbol.WhiteBishopOnWhiteField,
                ChessFontSymbol.WhitePawnOnBlackField,
                ChessFontSymbol.WhiteRookOnWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLowerLeftCorner,
                ChessFontSymbol.BorderLowerEdgeA,
                ChessFontSymbol.BorderLowerEdgeB,
                ChessFontSymbol.BorderLowerEdgeC,
                ChessFontSymbol.BorderLowerEdgeD,
                ChessFontSymbol.BorderLowerEdgeE,
                ChessFontSymbol.BorderLowerEdgeF,
                ChessFontSymbol.BorderLowerEdgeG,
                ChessFontSymbol.BorderLowerEdgeH,
                ChessFontSymbol.BorderLowerRightCorner
            };

            m_initialBoardSetupWithoutBorder = new ChessFontSymbol[] {
                ChessFontSymbol.BlackRookOnWhiteField,
                ChessFontSymbol.BlackKnightOnBlackField,
                ChessFontSymbol.BlackBishopOnWhiteField,
                ChessFontSymbol.BlackQueenOnBlackField,
                ChessFontSymbol.BlackKingOnWhiteField,
                ChessFontSymbol.BlackBishopOnBlackField,
                ChessFontSymbol.BlackKnightOnWhiteField,
                ChessFontSymbol.BlackRookOnBlackField,

                ChessFontSymbol.BlackPawnOnBlackField,
                ChessFontSymbol.BlackPawnOnWhiteField,
                ChessFontSymbol.BlackPawnOnBlackField,
                ChessFontSymbol.BlackPawnOnWhiteField,
                ChessFontSymbol.BlackPawnOnBlackField,
                ChessFontSymbol.BlackPawnOnWhiteField,
                ChessFontSymbol.BlackPawnOnBlackField,
                ChessFontSymbol.BlackPawnOnWhiteField,

                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,

                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,

                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,

                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyBlackField,
                ChessFontSymbol.EmptyWhiteField,

                ChessFontSymbol.WhitePawnOnWhiteField,
                ChessFontSymbol.WhitePawnOnBlackField,
                ChessFontSymbol.WhitePawnOnWhiteField,
                ChessFontSymbol.WhitePawnOnBlackField,
                ChessFontSymbol.WhitePawnOnWhiteField,
                ChessFontSymbol.WhitePawnOnBlackField,
                ChessFontSymbol.WhitePawnOnWhiteField,
                ChessFontSymbol.WhitePawnOnBlackField,

                ChessFontSymbol.WhiteRookOnBlackField,
                ChessFontSymbol.WhiteKnightOnWhiteField,
                ChessFontSymbol.WhiteBishopOnBlackField,
                ChessFontSymbol.WhiteQueenOnWhiteField,
                ChessFontSymbol.WhiteKingOnBlackField,
                ChessFontSymbol.WhiteBishopOnWhiteField,
                ChessFontSymbol.WhitePawnOnBlackField,
                ChessFontSymbol.WhiteRookOnWhiteField
            };

            m_initialBoardSetupBorderOnly = new ChessFontSymbol[] {
                ChessFontSymbol.BorderUpperLeftCorner,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,                
                ChessFontSymbol.BorderUpperRightCorner,

                ChessFontSymbol.BorderLeftEdge8,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge7,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge6,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge5,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge4,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge3,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge2,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLeftEdge1,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.EmptyWhiteField,
                ChessFontSymbol.BorderRightEdge,

                ChessFontSymbol.BorderLowerLeftCorner,
                ChessFontSymbol.BorderLowerEdgeA,
                ChessFontSymbol.BorderLowerEdgeB,
                ChessFontSymbol.BorderLowerEdgeC,
                ChessFontSymbol.BorderLowerEdgeD,
                ChessFontSymbol.BorderLowerEdgeE,
                ChessFontSymbol.BorderLowerEdgeF,
                ChessFontSymbol.BorderLowerEdgeG,
                ChessFontSymbol.BorderLowerEdgeH,
                ChessFontSymbol.BorderLowerRightCorner
            };

            m_upperBorder = new ChessFontSymbol[] {
                ChessFontSymbol.BorderUpperLeftCorner,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,
                ChessFontSymbol.BorderUpperEdge,                
                ChessFontSymbol.BorderUpperRightCorner
            };

            m_leftBorder = new ChessFontSymbol[] {
                ChessFontSymbol.BorderUpperLeftCorner,
                ChessFontSymbol.BorderLeftEdge8,
                ChessFontSymbol.BorderLeftEdge7,
                ChessFontSymbol.BorderLeftEdge6,
                ChessFontSymbol.BorderLeftEdge5,
                ChessFontSymbol.BorderLeftEdge4,
                ChessFontSymbol.BorderLeftEdge3,
                ChessFontSymbol.BorderLeftEdge2,
                ChessFontSymbol.BorderLeftEdge1,
                ChessFontSymbol.BorderLowerLeftCorner
            };

            m_fontMappings = new Dictionary<ChessFont, Dictionary<ChessFontSymbol, char>>();

            m_initialBoardSetupStrings = new Dictionary<ChessFont, string>();
            m_initialBoardSetupWithoutBorderStrings = new Dictionary<ChessFont, string>();
            m_initialBoardSetupBorderOnlyStrings = new Dictionary<ChessFont, string>();
            m_upperBorderStrings = new Dictionary<ChessFont, string>();
            m_leftBorderStrings = new Dictionary<ChessFont, string>();

            foreach (ChessFont font in Enum.GetValues(typeof(ChessFont)))
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(typeof(ChessFont).GetField(font.ToString()), typeof(ChessFontMappingsAttribute));
                if (attrs == null || attrs.Length == 0)
                {
                    m_fontMappings[font] = null;
                    continue;
                }
                Dictionary<ChessFontSymbol, char> mappings = new Dictionary<ChessFontSymbol, char>();
                foreach (ChessFontMappingsAttribute attr in attrs)
                {
                    mappings[attr.Symbol] = attr.MappingCharacter;
                }
                m_fontMappings[font] = mappings;

                StringBuilder sb = null;
                int i = 0;
                    
                sb = new StringBuilder();
                i = 0;
                foreach (ChessFontSymbol symbol in m_initialBoardSetup)
                {
                    sb.Append(mappings[symbol]);
                    if (++i % 10 == 0)
                    {
                        sb.Append("\r\n");
                    }
                }
                m_initialBoardSetupStrings[font] = sb.ToString();

                sb = new StringBuilder();
                i = 0;
                foreach (ChessFontSymbol symbol in m_initialBoardSetupWithoutBorder)
                {
                    sb.Append(mappings[symbol]);
                    if (++i % 8 == 0)
                    {
                        sb.Append("\r\n");
                    }
                }
                m_initialBoardSetupWithoutBorderStrings[font] = sb.ToString();

                sb = new StringBuilder();
                i = 0;
                foreach (ChessFontSymbol symbol in m_initialBoardSetupBorderOnly)
                {
                    sb.Append(mappings[symbol]);
                    if (++i % 10 == 0)
                    {
                        sb.Append("\r\n");
                    }
                }
                m_initialBoardSetupBorderOnlyStrings[font] = sb.ToString();

                sb = new StringBuilder();
                i = 0;
                foreach (ChessFontSymbol symbol in m_upperBorder)
                {
                    sb.Append(mappings[symbol]);
                }
                m_upperBorderStrings[font] = sb.ToString();

                sb = new StringBuilder();
                i = 0;
                foreach (ChessFontSymbol symbol in m_leftBorder)
                {
                    sb.Append(mappings[symbol]);
                    sb.Append("\r\n");
                }
                m_leftBorderStrings[font] = sb.ToString();
            }
        }

        private static void InitializeFonts()
        {
            //Debug.WriteLine(string.Join("\r\n", typeof(ChessFonts).Assembly.GetManifestResourceNames()));

            m_fontFamilies = new Dictionary<ChessFont,FontFamily>();

            foreach (ChessFont font in Enum.GetValues(typeof(ChessFont)))
            {
                ChessFontResourceNameAttribute attr = Attribute.GetCustomAttribute(typeof(ChessFont).GetField(font.ToString()), typeof(ChessFontResourceNameAttribute)) as ChessFontResourceNameAttribute;
                if (attr == null)
                {
                    m_fontFamilies[font] = null;
                    continue;
                }
                using (Stream fontStream = typeof(ChessFonts).Assembly.GetManifestResourceStream(attr.Name))
                {
                    PrivateFontCollection privateFontCollection = new PrivateFontCollection();
                    //using (MemoryStream tempFontData = new MemoryStream())
                    //{
                    //    byte[] buffer = new byte[CHUNK_SIZE];
                    //    int bytesRead = 0;
                    //    while ((bytesRead = fontStream.Read(buffer, 0, CHUNK_SIZE)) > 0)
                    //    {
                    //        tempFontData.Write(buffer, 0, bytesRead);
                    //    }
                    //    tempFontData.Flush();
                    //    unsafe
                    //    {
                    //        fixed (byte* p = tempFontData.ToArray())
                    //        {
                    //            privateFontCollection.AddMemoryFont((IntPtr)p, (int)tempFontData.Length);
                    //        }
                    //    }
                    //}

                    string tempFontFileName;
                    using (FileStream tempFontFile = File.Create(tempFontFileName = font.ToString() + ".ttf", CHUNK_SIZE)) //Path.GetTempFileName()))
                    {
                        byte[] buffer = new byte[CHUNK_SIZE];
                        int bytesRead = 0;
                        while ((bytesRead = fontStream.Read(buffer, 0, CHUNK_SIZE)) > 0)
                        {
                            tempFontFile.Write(buffer, 0, bytesRead);
                        }
                        tempFontFile.Flush();
                    }
                    privateFontCollection.AddFontFile(tempFontFileName);


                    m_fontFamilies[font] = privateFontCollection.Families[0];
                }
            }
        }

        public static System.Drawing.Font Usual
        {
            get
            {
                return GetFont(ChessFont.Usual, DEFAULT_FONT_SIZE);
            }
        }

        public static System.Drawing.Font Merida
        {
            get
            {
                return GetFont(ChessFont.Merida, DEFAULT_FONT_SIZE);
            }
        }

        public static System.Drawing.Font Cases
        {
            get
            {
                return GetFont(ChessFont.Cases, DEFAULT_FONT_SIZE);
            }
        }

        public static System.Drawing.Font Zurich
        {
            get
            {
                return GetFont(ChessFont.Zurich, DEFAULT_FONT_SIZE);
            }
        }

        public static System.Drawing.Font GetFont(ChessFont font)
        {
            return GetFont(font, DEFAULT_FONT_SIZE);
        }

        public static System.Drawing.Font GetFont(ChessFont font, float size)
        {
            try
            {
                return new System.Drawing.Font(m_fontFamilies[font], size, FontStyle.Regular, GraphicsUnit.Point);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                try
                {
                    InitializeFonts();
                    return new System.Drawing.Font(m_fontFamilies[font], size, FontStyle.Regular, GraphicsUnit.Point);
                }
                catch { }
                throw;
            }
        }

        public static string GetInitialSetupString(ChessFont font)
        {
            return m_initialBoardSetupStrings[font];
        }

        public static string GetInitialSetupWithoutBorderString(ChessFont font)
        {
            return m_initialBoardSetupWithoutBorderStrings[font];
        }

        public static string GetInitialSetupBorderOnlyString(ChessFont font)
        {
            return m_initialBoardSetupBorderOnlyStrings[font];
        }

        public static string GetUpperBorderString(ChessFont font)
        {
            return m_upperBorderStrings[font];
        }

        public static string GetLeftBorderString(ChessFont font)
        {
            return m_leftBorderStrings[font];
        }

        public static char GetUpperLeftCornerMappingCharacter(ChessFont font)
        {
            Dictionary<ChessFontSymbol, char> mappings = m_fontMappings[font];
            return mappings[ChessFontSymbol.BorderUpperLeftCorner];
        }

        public static char GetEmptyBlackFieldMappingCharacter(ChessFont font)
        {
            Dictionary<ChessFontSymbol, char> mappings = m_fontMappings[font];
            return mappings[ChessFontSymbol.EmptyBlackField];
        }
    }
}
