using System;

using SkiaSharp;
using PixelFarm;
using PixelFarm.OpenType;

using System.Collections.Generic;
using System.IO;

using NOpenType;
using NOpenType.Extensions;

using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;

namespace SkiaSharpSample.Samples
{
    [Preserve(AllMembers = true)]
    public class ATest01 : SampleBase
    {
        ActualImage destImg;
        ImageGraphics2D imgGfx2d;
        AggCanvasPainter p;
        bool buildVxs;

        [Preserve]
        public ATest01()
        {
             
        }

        public override string Title => "ATest01";

        public override SampleCategories Category => SampleCategories.BitmapDecoding;

        protected override void OnDrawSample(SKCanvas canvas, int width, int height)
        {
            if (!buildVxs)
            {
                destImg = new ActualImage(400, 300, PixelFarm.Agg.PixelFormat.ARGB32);
                imgGfx2d = new ImageGraphics2D(destImg); //no platform
                p = new AggCanvasPainter(imgGfx2d);

                buildVxs = true;
                ReadAndRender("tahoma.ttf", "A");
            }

            canvas.Clear(SKColors.White);

            using (SKBitmap bitmap = new SKBitmap(destImg.Width, destImg.Height, false))
            {
                //IntPtr pixelHeader = bitmap.GetPixels();
                //byte[] srcBuffer = ActualImage.GetBuffer(destImg);
                bitmap.LockPixels();
                //System.Runtime.InteropServices.Marshal.Copy(
                //    srcBuffer, 0,
                //    pixelHeader, srcBuffer.Length);
                //bitmap.UnlockPixels();
                PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(
                    destImg, destImg.Width, destImg.Height,
                    destImg.Stride, bitmap.GetPixels());
                bitmap.UnlockPixels();
                canvas.DrawBitmap(bitmap, 10, 10);

            }

        }

        void ReadAndRender(string fontfile, string text)
        {

            char testChar = text[0];//only 1 char 
            int resolution = 96;
            var reader = new OpenTypeReader();
            using (var fs = new FileStream(fontfile, FileMode.Open))
            {
                //1. read typeface from font file
                Typeface typeFace = reader.Read(fs);

#if DEBUG
                //-----
                //about typeface 
                short ascender = typeFace.Ascender;
                short descender = typeFace.Descender;
                short lineGap = typeFace.LineGap;

                //NOpenType.Tables.UnicodeLangBits test = NOpenType.Tables.UnicodeLangBits.Thai;
                //NOpenType.Tables.UnicodeRangeInfo rangeInfo = test.ToUnicodeRangeInfo();
                //bool doseSupport = typeFace.DoseSupportUnicode(test); 

                List<int> outputGlyphIndice = new List<int>();
                typeFace.Lookup(text.ToCharArray(), outputGlyphIndice);
#endif

                float fontSizeInPoint = 96;
                RenderWithMiniAgg(typeFace, testChar, fontSizeInPoint);

            }
        }

        void RenderWithMiniAgg(Typeface typeface, char testChar, float sizeInPoint)
        {
            //2. glyph-to-vxs builder
            var builder = new GlyphPathBuilderVxs(typeface);
            builder.Build(testChar, sizeInPoint);
            VertexStore vxs = builder.GetVxs();

            //5. use PixelFarm's Agg to render to bitmap...
            //5.1 clear background
            p.Clear(PixelFarm.Drawing.Color.White);

            //if (chkFillBackground.Checked)
            //{
            //5.2 
            p.FillColor = PixelFarm.Drawing.Color.Black;
            //5.3
            p.Fill(vxs);
            //}
            //if (chkBorder.Checked)
            //{
            //5.4 
            p.StrokeColor = PixelFarm.Drawing.Color.Green;
            //user can specific border width here...
            //p.StrokeWidth = 2;
            //5.5 
            p.Draw(vxs);
            //}
            //6. use this util to copy image from Agg actual image to System.Drawing.Bitmap
            //BitmapHelper.CopyToWindowsBitmap(destImg, winBmp, new RectInt(0, 0, 300, 300));
            ////--------------- 
            ////7. just render our bitmap
            //g.Clear(Color.White);
            //g.DrawImage(winBmp, new Point(10, 0));
        }
    }
}
