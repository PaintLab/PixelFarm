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
        Dictionary<char, FontGlyph> dicGlyphs = new Dictionary<char, FontGlyph>();

        public FontFace(IntPtr unmanagedMem)
        {
            //store font file in unmanaged memory side
            this.unmanagedMem = unmanagedMem;
        }
        public FontGlyph GetGlyph(char c)
        {

            FontGlyph found;
            if (!dicGlyphs.TryGetValue(c, out found))
            {
                found = FontStore.GetGlyph(c);
                this.dicGlyphs.Add(c, found);
            }
            return found;
        }
        public void Dispose()
        {
            if (unmanagedMem != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(unmanagedMem);
                unmanagedMem = IntPtr.Zero;
            }
            dicGlyphs.Clear();
            dicGlyphs = null;

        }
    }

}