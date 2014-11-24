//MIT 2014,WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using PixelFarm.Agg;


namespace PixelFarm.Font2
{

    public static class FontStore
    {

        static Dictionary<string, FontFace> fonts = new Dictionary<string, FontFace>();
        internal static FontGlyph GetGlyph(IntPtr ftFaceHandle, char unicodeChar)
        {
            //--------------------------------------------------
            unsafe
            {
                ExportGlyph exportTypeFace = new ExportGlyph();
                PixelFarm.Font2.NativeMyFontsLib.MyFtLoadChar(ftFaceHandle, unicodeChar, ref exportTypeFace);
                return FontGlyphBuilder.BuildGlyph(&exportTypeFace);
            }
        }
        internal static FontGlyph GetGlyphByGlyphIndex(IntPtr ftFaceHandle, uint glyphIndex)
        {
            //--------------------------------------------------
            unsafe
            {
                ExportGlyph exportTypeFace = new ExportGlyph();
                PixelFarm.Font2.NativeMyFontsLib.MyFtLoadGlyph(ftFaceHandle, glyphIndex, ref exportTypeFace);
                return FontGlyphBuilder.BuildGlyph(&exportTypeFace);
            }
        }

        internal static void SetShapingEngine(FontFace fontFace, string lang, HBDirection hb_direction, int hb_scriptcode)
        {
            //string lang = "en";
            //PixelFarm.Font2.NativeMyFontsLib.MyFtSetupShapingEngine(ftFaceHandle,
            //    lang,
            //    lang.Length,
            //    HBDirection.HB_DIRECTION_LTR,
            //    HBScriptCode.HB_SCRIPT_LATIN); 
            ExportTypeFaceInfo exportTypeInfo = new ExportTypeFaceInfo();
            PixelFarm.Font2.NativeMyFontsLib.MyFtSetupShapingEngine(fontFace.Handle,
                lang,
                lang.Length,
                hb_direction,
                hb_scriptcode,
                ref exportTypeInfo);
            fontFace.HBFont = exportTypeInfo.hb_font; 


        }

        //public static void ShapeText(char[] data)
        //{
        //    byte[] unicodeBuffer = Encoding.Unicode.GetBytes(data);
        //    unsafe
        //    {
        //        fixed (byte* u = &unicodeBuffer[0])
        //        {
        //            PixelFarm.Font2.NativeMyFontsLib.MyFtShaping(u, 2);
        //        }
        //    }
        //}

        public static FontFace LoadFont(string filename, int pixelSize)
        {
            //load font from specific file 
            FontFace fontFace;
            if (!fonts.TryGetValue(filename, out fontFace))
            {

                //if not found
                //then load it
                byte[] fontFileContent = File.ReadAllBytes(filename);
                int filelen = fontFileContent.Length;
                IntPtr unmanagedMem = Marshal.AllocHGlobal(filelen);
                Marshal.Copy(fontFileContent, 0, unmanagedMem, filelen);

                IntPtr faceHandle = NativeMyFontsLib.MyFtNewMemoryFace(unmanagedMem, filelen, pixelSize);
                if (faceHandle != IntPtr.Zero)
                {
                    //ok pass
                    fontFace = new FontFace(unmanagedMem, faceHandle);

                    ExportTypeFaceInfo exportTypeInfo = new ExportTypeFaceInfo();
                    NativeMyFontsLib.MyFtGetFaceInfo(faceHandle, ref exportTypeInfo);
                    fontFace.HasKerning = exportTypeInfo.hasKerning;

                    SetShapingEngine(fontFace, "th", HBDirection.HB_DIRECTION_LTR, HBScriptCode.HB_SCRIPT_THAI);

                    fonts.Add(filename, fontFace);
                }
                else
                {

                    //load font error
                    Marshal.FreeHGlobal(unmanagedMem);
                }

            }
            return fontFace;

        }
    }

}
