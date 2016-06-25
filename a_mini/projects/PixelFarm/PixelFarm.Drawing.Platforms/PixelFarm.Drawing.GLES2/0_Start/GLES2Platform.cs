//BSD, 2014-2016, WinterDev

using System;
namespace PixelFarm.Drawing.GLES2
{
    class GLES2Platform : GraphicsPlatform
    {
        IFonts sampleIFonts;
        System.Drawing.Bitmap sampleBmp;
        public override IFonts SampleIFonts
        {
            get
            {
                if (sampleIFonts == null)
                {
                    if (sampleBmp == null)
                    {
                        sampleBmp = new System.Drawing.Bitmap(2, 2);
                    }
                    //System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(sampleBmp);
                    //TODO: review here, we need some platform-specific func
                    sampleIFonts = new PixelFarm.Drawing.WinGdi.MyScreenCanvas(this, 0, 0, 0, 0, 2, 2);
                }
                return this.sampleIFonts;
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