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
    public class FontFace : IDisposable
    {
        /// <summary>
        /// store font file content in unmanaged memory
        /// </summary>
        IntPtr unmanagedMem;

        /// <summary>
        /// free type handle (unmanaged mem)
        /// </summary>
        IntPtr ftFaceHandle;

        int currentFacePixelSize = 0;

        /// <summary>
        /// store font glyph for each px size
        /// </summary>
        Dictionary<int, Font> fonts = new Dictionary<int, Font>();
        IntPtr hb_font;
        internal FontFace(IntPtr unmanagedMem, IntPtr ftFaceHandle)
        {

            this.unmanagedMem = unmanagedMem;
            this.ftFaceHandle = ftFaceHandle;
        }


        ~FontFace()
        {
            Dispose();
        }

        /// <summary>
        /// free typpe handler
        /// </summary>
        internal IntPtr Handle
        {
            get { return this.ftFaceHandle; }
        }


        public void Dispose()
        {

            if (this.ftFaceHandle != IntPtr.Zero)
            {
                NativeMyFontsLib.MyFtDoneFace(this.ftFaceHandle);
                ftFaceHandle = IntPtr.Zero;
            }

            if (unmanagedMem != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(unmanagedMem);
                unmanagedMem = IntPtr.Zero;
            }

            fonts.Clear();
            fonts = null;

        }
        public bool HasKerning { get; set; }

        //---------------------------
        //for font shaping engine
        //--------------------------- 
        internal IntPtr HBFont
        {
            get { return this.hb_font; }
            set { this.hb_font = value; }
        }

        internal Font GetFontAtPointSize(float fontPointSize)
        {
            //convert from point size to pixelsize *** 

            int pixelSize = FontStore.ConvertFromPointUnitToPixelUnit(fontPointSize);
            Font found;
            if (!fonts.TryGetValue(pixelSize, out found))
            {
                //----------------------------------
                //set current fontface size
                currentFacePixelSize = pixelSize;
                NativeMyFontsLib.MyFtSetPixelSizes(this.ftFaceHandle, pixelSize);

                //create font size
                Font f = new Font(this, fontPointSize);
                fonts.Add(pixelSize, f);

                //------------------------------------
                return f;
            }
            return found;
        }

        internal FontGlyph ReloadGlyphFromIndex(uint glyphIndex, int pixelSize)
        {
            if (pixelSize != currentFacePixelSize)
            {
                NativeMyFontsLib.MyFtSetPixelSizes(this.ftFaceHandle, pixelSize);
            }
            unsafe
            {
                ExportGlyph exportTypeFace = new ExportGlyph();
                PixelFarm.Font2.NativeMyFontsLib.MyFtLoadGlyph(ftFaceHandle, glyphIndex, ref exportTypeFace);
                return FontGlyphBuilder.BuildGlyph(&exportTypeFace);
            } 
        }
        internal FontGlyph ReloadGlyphFromChar(char unicodeChar, int pixelSize)
        {
            if (pixelSize != currentFacePixelSize)
            {
                NativeMyFontsLib.MyFtSetPixelSizes(this.ftFaceHandle, pixelSize);
            }
            //--------------------------------------------------
            unsafe
            {
                ExportGlyph exportTypeFace = new ExportGlyph();
                PixelFarm.Font2.NativeMyFontsLib.MyFtLoadChar(ftFaceHandle, unicodeChar, ref exportTypeFace);
                return FontGlyphBuilder.BuildGlyph(&exportTypeFace);
            }
             
        }
    }

}