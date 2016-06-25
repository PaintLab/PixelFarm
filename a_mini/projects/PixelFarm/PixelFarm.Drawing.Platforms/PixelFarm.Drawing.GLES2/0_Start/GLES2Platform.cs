//BSD, 2014-2016, WinterDev
using System;

namespace PixelFarm.Drawing.GLES2
{
    class GLES2Platform : GraphicsPlatform
    {
        public override IFonts SampleIFonts
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Canvas CreateCanvas(int left, int top, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override Canvas CreateCanvas(object platformCanvas, int left, int top, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override GraphicsPath CreateGraphicsPath()
        {
            throw new NotImplementedException();
        }

        public override Bitmap CreatePlatformBitmap(int w, int h, byte[] rawBuffer, bool isBottomUp)
        {
            throw new NotImplementedException();
        }

        public override FontInfo GetFont(string fontfaceName, float emsize, FontStyle st)
        {
            throw new NotImplementedException();
        }
    }
}