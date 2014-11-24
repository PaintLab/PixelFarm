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
            ProperGlyph[] properGlyphs = null;
            int j = buffer.Length;
            int buffsize = j * 2;
            //get kerning list
            if (properGlyphs == null)
            {
                properGlyphs = new ProperGlyph[buffsize];
                this.currentFontFace.GetGlyphPos(buffer, 0, buffsize, properGlyphs);
            }

            double xpos = x;
            for (int i = 0; i < buffsize; ++i)
            {
                uint codepoint = properGlyphs[i].codepoint;
                if (codepoint == 0)
                {
                    break;
                }
                //-------------------------------------------------------------
                FontGlyph glyph = this.currentFontFace.GetGlyphByCodePoint(codepoint); 
                var left = glyph.exportGlyph.img_horiBearingX;


                this.painter.DrawImage(glyph.glyphImage32,
                    (float)(xpos + (left >> 6)),
                    (float)(y + (glyph.exportGlyph.bboxYmin >> 6)));

                int w = (glyph.exportGlyph.advanceX) >> 6;
                xpos += (w);
                //-------------------------------------------------------------                
            }
        }
    }

}