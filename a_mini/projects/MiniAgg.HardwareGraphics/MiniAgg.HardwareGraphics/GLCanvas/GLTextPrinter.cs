//MIT 2014, WinterDev
using System.Text;
using System;
using OpenTK.Graphics.OpenGL;
using Tesselate;
using System.Drawing;
using PixelFarm.Font2;

namespace OpenTkEssTest
{
    class GLTextPrinter
    {
        FontFace currentFontFace;
        CanvasGL2d canvas2d;
        public GLTextPrinter(CanvasGL2d canvas2d)
        {
            this.canvas2d = canvas2d;
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
                            GLBitmapTexture bmp = new GLBitmapTexture(glyph.glyphImage32);
                            this.canvas2d.DrawImageInvert(bmp, (float)xpos, (float)y);
                            bmp.Dispose();
                            xpos += (glyph.advanceX / 64);
                        } break;
                }
            }
        }
    }
}