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
    class TextPrinter
    {
        FontFace currentFontFace;
        CanvasPainter painter;
        public TextPrinter(CanvasPainter painter)
        {
            this.painter = painter;
        }
        public FontFace CurrentFontFace
        {
            get { return this.currentFontFace; }
            set { this.currentFontFace = value; }
        }
        public void Print(string t, double x, double y)
        {
            Print(t.ToCharArray(), x, y);
        }
        public void Print(char[] buffer, double x, double y)
        {
            int j = buffer.Length;
            double xpos = x;
            for (int i = 0; i < j; ++i)
            {
                char c = buffer[i];
                switch (c)
                {
                    case ' ':
                        {
                        } break;
                    case '\r':
                        {
                        } break;
                    case '\n':
                        {
                        } break;
                    default:
                        {
                            FontGlyph glyph = this.currentFontFace.GetGlyph(c);
                            this.painter.DrawImage(glyph.glyphImage32, xpos, y);
                            xpos += (glyph.advanceX / 64);
                        } break;
                }
            }
        }
    }

}