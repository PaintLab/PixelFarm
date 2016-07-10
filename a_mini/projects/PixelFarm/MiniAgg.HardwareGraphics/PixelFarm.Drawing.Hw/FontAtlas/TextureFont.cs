//MIT,2016, WinterDev
//----------------------------------- 
using System;
using System.Xml;
namespace PixelFarm.Drawing.Fonts
{

    public static class TextureFontBuilder
    {
        public static TextureFont CreateFont(string xmlFontInfo, string imgAtlas)
        {
            string fontfilename = "d:\\WImageTest\\a_total.xml";

            SimpleFontAtlasBuilder atlasBuilder = new SimpleFontAtlasBuilder();

            SimpleFontAtlas fontAtlas = atlasBuilder.LoadFontInfo(fontfilename);
            //2. load glyph image


            //GlyphImage glyImage = null;
            // MyFtLib.st
            //totalImg = new System.Drawing.Bitmap("d:\\WImageTest\\a_total.png");
            //{
            //    var bmpdata = totalImg.LockBits(new System.Drawing.Rectangle(0, 0, totalImg.Width, totalImg.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, totalImg.PixelFormat);
            //    var buffer = new int[totalImg.Width * totalImg.Height];
            //    System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
            //    totalImg.UnlockBits(bmpdata);
            //    glyImage = new GlyphImage(totalImg.Width, totalImg.Height);
            //    glyImage.SetImageBuffer(buffer, false);
            //}
            //fontAtlas.SetImage(glyImage);
            return null;
        }
        public static TextureFont CreateFont(XmlDocument xmlFontInfo, int[] imgBuffer)
        {

            return null;
        }
    }
    public class TextureFont : Font
    {
        internal TextureFont()
        {

        }
        public override double AscentInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override double CapHeightInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override double DescentInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override float EmSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int EmSizeInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override FontFace FontFace
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override FontInfo FontInfo
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Height
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override object InnerFont
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override FontStyle Style
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override double XHeightInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int GetAdvanceForCharacter(char c)
        {
            throw new NotImplementedException();
        }

        public override int GetAdvanceForCharacter(char c, char next_c)
        {
            throw new NotImplementedException();
        }

        public override FontGlyph GetGlyph(char c)
        {
            throw new NotImplementedException();
        }

        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            throw new NotImplementedException();
        }

        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            throw new NotImplementedException();
        }

        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}