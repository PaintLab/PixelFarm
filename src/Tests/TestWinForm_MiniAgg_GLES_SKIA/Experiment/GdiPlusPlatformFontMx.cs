////BSD, 2014-2018, WinterDev
////ArthurHub  , Jose Manuel Menendez Poo

//// "Therefore those skilled at the unorthodox
//// are infinite as heaven and earth,
//// inexhaustible as the great rivers.
//// When they come to an end,
//// they begin again,
//// like the days and months;
//// they die and are reborn,
//// like the four seasons."
//// 
//// - Sun Tsu,
//// "The Art of War"

//using System;
//using System.Collections.Generic;
//using Win32;
//namespace PixelFarm.Drawing.WinGdi
//{
//    /// <summary>
//    /// for this platform font management
//    /// </summary>
//    class GdiPlusPlatformFontMx
//    {
//        //gdiplus platform can handle following font
//        //1. gdiplus font
//        //2. gdi font
//        //3. vector font
//        //4. opentype font


//        public GdiPlusPlatformFontMx()
//        {
//        }

//        public WinGdiFont ResolveForWinGdiPlusFont(RequestFont r)
//        {
//            WinGdiFont winGdiPlusFont = r.ActualFont as WinGdiFont;
//            if (winGdiPlusFont != null)
//            {
//                return winGdiPlusFont;
//            }
//            //check if 
//            throw new NotSupportedException();
//        }

//        //---------
//        static GdiPlusPlatformFontMx s_gdiPlusFontMx = new GdiPlusPlatformFontMx();
//        public static GdiPlusPlatformFontMx Default { get { return s_gdiPlusFontMx; } }
//    }

//}