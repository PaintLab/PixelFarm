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
       
        static object syncObj = new object();
        static bool isInitLib = false; 

        public static int InitLib()
        {
            int initResult = 0;
            lock (syncObj)
            {
                if (!isInitLib)
                {
                    initResult = NativeMyFontsLib.MyFtInitLib();
                    isInitLib = true;
                }
            }
            return initResult;
        }



       
        internal static FontGlyph GetGlyph(IntPtr ftFaceHandle, char unicodeChar)
        {
            
            //--------------------------------------------------
            unsafe
            {   
                ExportTypeFace exportTypeFace = new ExportTypeFace(); 
                PixelFarm.Font2.NativeMyFontsLib.MyFtLoadChar(ftFaceHandle, unicodeChar, ref exportTypeFace);
                return FontGlyphBuilder.BuildGlyph(&exportTypeFace);                
            }
        }
        
        
        internal static void SetShapingEngine(IntPtr ftFaceHandle)
        {
            string lang = "en";
            PixelFarm.Font2.NativeMyFontsLib.MyFtSetupShapingEngine(ftFaceHandle,
                lang,
                lang.Length,
                HBDirection.HB_DIRECTION_LTR,
                HBScriptCode.HB_SCRIPT_LATIN);
        }
        public static void ShapeText(string data)
        {
            ShapeText(data.ToCharArray());
        }
        public static void ShapeText(char[] data)
        {
            byte[] unicodeBuffer = Encoding.Unicode.GetBytes(data);
            unsafe
            {
                fixed (byte* u = &unicodeBuffer[0])
                {
                    PixelFarm.Font2.NativeMyFontsLib.MyFtShaping(u, 2);
                }
            }
        }

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

                IntPtr faceHandle = PixelFarm.Font2.NativeMyFontsLib.MyFtNewMemoryFace(unmanagedMem, filelen, pixelSize);
                if (faceHandle != IntPtr.Zero)
                {
                    //ok pass
                    fontFace = new FontFace(unmanagedMem, faceHandle);
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
