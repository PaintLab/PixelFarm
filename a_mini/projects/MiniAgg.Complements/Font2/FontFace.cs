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

        IntPtr hb_font;

      

        /// <summary>
        /// glyph
        /// </summary>
        Dictionary<char, FontGlyph> dicGlyphs = new Dictionary<char, FontGlyph>();
        Dictionary<uint, FontGlyph> dicGlyphs2 = new Dictionary<uint, FontGlyph>();

        internal FontFace(IntPtr unmanagedMem, IntPtr ftFaceHandle)
        {
            //store font file in unmanaged memory side
            this.unmanagedMem = unmanagedMem;
            this.ftFaceHandle = ftFaceHandle;
        }
        ~FontFace()
        {
            Dispose();
        }
        public IntPtr Handle
        {
            get { return this.ftFaceHandle; }
        }
        public FontGlyph GetGlyph(char c)
        {
            FontGlyph found;
            if (!dicGlyphs.TryGetValue(c, out found))
            {
                found = FontStore.GetGlyph(ftFaceHandle, c);
                this.dicGlyphs.Add(c, found);
            }
            return found;
        }
        public FontGlyph GetGlyphByCodePoint(uint codePoint)
        {
            FontGlyph found;
            if (!dicGlyphs2.TryGetValue(codePoint, out found))
            {
                found = FontStore.GetGlyphByGlyphIndex(ftFaceHandle, codePoint);
                this.dicGlyphs2.Add(codePoint, found);
            }
            return found;
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
            dicGlyphs.Clear();
            dicGlyphs = null;

        }
        public bool HasKerning { get; set; }

        internal IntPtr HBFont
        {
            get { return this.hb_font; }
            set { this.hb_font = value; }
        }
         
        public void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {

            unsafe
            {
                //byte[] unicodeBuffer = System.Text.Encoding.Unicode.GetBytes(buffer); 
                fixed (ProperGlyph* propGlyphH = &properGlyphs[0])
                fixed (char* head = &buffer[0])
                {
                    NativeMyFontsLib.MyFtShaping(
                        this.hb_font, 
                        head,
                        buffer.Length,
                        propGlyphH);
                }
            }
        }
    }

}