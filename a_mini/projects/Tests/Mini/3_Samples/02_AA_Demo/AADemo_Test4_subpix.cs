//MIT, 2017, WinterDev 
using System;
using Mini;
namespace PixelFarm.Agg.Sample_AADemoTest4
{

    public enum Sample
    {
        A,
        B,
        C
    }

    [Info(OrderCode = "02")]
    [Info("SubPixelRendering_4")]
    public class AADemoTest4_subpix : DemoBase
    {
        public AADemoTest4_subpix()
        {
        }
        static byte[] CreateGreyScaleBuffer(ActualImage img)
        {
            //assume img is 32 rgba img
            int imgW = img.Width;
            int height = img.Height;
            //56 level grey scale buffer

            byte[] srcImgBuffer = ActualImage.GetBuffer(img);
            int greyScaleBufferLen = imgW * height;
            byte[] greyScaleBuffer = new byte[greyScaleBufferLen];

            //for (int i = greyScaleBufferLen - 1; i >= 0; --i)
            //{
            //    greyScaleBuffer[i] = 64;
            //}


            int destIndex = 0;
            int srcImgIndex = 0;
            int srcImgStride = img.Stride;

            for (int y = 0; y < height; ++y)
            {
                srcImgIndex = srcImgStride * y;
                destIndex = imgW * y;
                for (int x = 0; x < imgW; ++x)
                {
                    byte r = srcImgBuffer[srcImgIndex];
                    byte g = srcImgBuffer[srcImgIndex + 1];
                    byte b = srcImgBuffer[srcImgIndex + 2];
                    byte a = srcImgBuffer[srcImgIndex + 3];
                    if (r != 0 || g != 0 || b != 0)
                    {
                    }
                    if (a != 255)
                    {

                    }
                    //skip alpha
                    //byte greyScaleValue =
                    //    (byte)((0.333f * (float)r) + (0.5f * (float)g) + (0.1666f * (float)b));

                    greyScaleBuffer[destIndex] = (byte)(((a + 1) / 256f) * 64f);

                    destIndex++;
                    srcImgIndex += 4;
                }
            }
            return greyScaleBuffer;
        }
        void BlendWithLcdSpans(ActualImage destImg, byte[] greyBuff, int greyBufferWidth, int greyBufferHeight)
        {
            PixelFarm.Drawing.Color color = PixelFarm.Drawing.Color.Black;
            for (int y = 0; y < greyBufferHeight; ++y)
            {
                BlendLcdSpan(destImg, greyBuff, color, 0, y, greyBufferWidth);
            }
            //SwapRB(destImg);
        }
        void BlendLcdSpan(ActualImage destImg, byte[] expandGreyBuffer,
         PixelFarm.Drawing.Color color, int x, int y, int width)
        {
            byte[] rgb = new byte[3]{
                color.R,
                color.G,
                color.B
            };
            //-------------------------
            //destination
            byte[] destImgBuffer = ActualImage.GetBuffer(destImg);
            //start pixel
            int destImgIndex = (x * 4) + (destImg.Stride * y);
            //start img src
            int srcImgIndex = x + (width * y);
            int spanIndex = srcImgIndex;
            int i = x % 3;

            int round = 0;

            do
            {
                int a0 = expandGreyBuffer[spanIndex] * color.alpha;
                byte existingColor = destImgBuffer[destImgIndex];
                byte newValue = (byte)((((rgb[i] - existingColor) * a0) + (existingColor << 16)) >> 16);
                destImgBuffer[destImgIndex] = newValue;
                //move to next dest
                destImgIndex++;


                i++;
                if (i > 2)
                {
                    i = 0;//reset
                }
                round++;
                if (round > 2)
                {
                    //this is alpha chanel
                    //so we skip alpha byte to next

                    //and swap rgb of latest write pixel
                    //--------------------------
                    //in-place swap
                    byte r = destImgBuffer[destImgIndex - 1];
                    byte b = destImgBuffer[destImgIndex - 3];
                    destImgBuffer[destImgIndex - 3] = r;
                    destImgBuffer[destImgIndex - 1] = b;
                    //--------------------------

                    destImgIndex++;
                    round = 0;
                }
                spanIndex++;
                srcImgIndex++;


            } while (--width > 0);
        }
        static void SwapRB(ActualImage destImg)
        {
            byte[] destImgBuffer = ActualImage.GetBuffer(destImg);
            int width = destImg.Width;
            int height = destImg.Height;
            int destIndex = 0;
            for (int y = 0; y < height; ++y)
            {
                destIndex = (y * destImg.Stride);
                for (int x = 0; x < width; ++x)
                {
                    byte r = destImgBuffer[destIndex];
                    byte g = destImgBuffer[destIndex + 1];
                    byte b = destImgBuffer[destIndex + 2];
                    byte a = destImgBuffer[destIndex + 3];
                    //swap
                    destImgBuffer[destIndex + 2] = r;
                    destImgBuffer[destIndex] = b;
                    destIndex += 4;
                }
            }
        }
        // Swap Blue and Red, that is convert RGB->BGR or BGR->RGB
        ////---------------------------------
        //void swap_rb(unsigned char* buf, unsigned width, unsigned height, unsigned stride)
        //{
        //    unsigned x, y;
        //    for(y = 0; y < height; ++y)
        //    {
        //        unsigned char* p = buf + stride * y;
        //        for(x = 0; x < width; ++x)
        //        {
        //            unsigned char v = p[0];
        //            p[0] = p[2];
        //            p[2] = v;
        //            p += 3;
        //        }
        //    }
        //}
        //// Blend one span into the R-G-B 24 bit frame buffer
        //// For the B-G-R byte order or for 32-bit buffers modify
        //// this function accordingly. The general idea is 'span' 
        //// contains alpha values for individual color channels in the 
        //// R-G-B order, so, for the B-G-R order you will have to 
        //// choose values from the 'span' array differently
        ////---------------------------------
        //void blend_lcd_span(int x, 
        //                    int y, 
        //                    const unsigned char* span, 
        //                    int width, 
        //                    const rgba& color, 
        //                    unsigned char* rgb24_buf, 
        //                    unsigned rgb24_stride)
        //{
        //    unsigned char* p = rgb24_buf + rgb24_stride * y + x;
        //    unsigned char rgb[3] = { color.r, color.g, color.b };
        //    int i = x % 3;
        //    do
        //    {
        //        int a0 = int(*span++) * color.a;
        //        *p++ = (unsigned char)((((rgb[i++] - *p) * a0) + (*p << 16)) >> 16);
        //        if(i > 2) i = 0;
        //    }
        //    while(--width);
        //}


        [DemoConfig]
        public Sample Sample
        {
            get;
            set;
        }
        void RunSampleA(CanvasPainter p)
        {

            //1. create simple vertical line to test agg's lcd rendernig technique
            //create gray-scale actual image
            ActualImage glyphImg = new ActualImage(100, 100, PixelFormat.ARGB32);
            ImageGraphics2D glyph2d = new ImageGraphics2D(glyphImg);
            AggCanvasPainter painter = new AggCanvasPainter(glyph2d);

            painter.StrokeColor = PixelFarm.Drawing.Color.Black;
            painter.StrokeWidth = 2.0f * 3;
            painter.Line(2 * 3, 0, 3 * 3, 15); //scale horizontal 3 times, 

            //painter.Line(2, 0, 2, 15);
            //painter.Line(2, 0, 20, 20);
            //painter.Line(2, 0, 30, 15);
            //painter.Line(2, 0, 30, 5);
            //clear surface bg
            p.Clear(PixelFarm.Drawing.Color.White);
            //draw img into that bg
            //--------------- 
            //convert glyphImg from RGBA to grey Scale buffer
            //---------------
            //lcd process ...
            byte[] glyphGreyScale = CreateGreyScaleBuffer(glyphImg);
            //
            int newGreyBuffWidth;
            byte[] expanedGreyScaleBuffer = CreateNewExpandedLcdGrayScale(glyphGreyScale, glyphImg.Width, glyphImg.Height, out newGreyBuffWidth);
            //blend lcd 
            var aggPainer = (PixelFarm.Agg.AggCanvasPainter)p;
            BlendWithLcdSpans(aggPainer.Graphics.DestActualImage, expanedGreyScaleBuffer, newGreyBuffWidth, glyphImg.Height);
            //--------------- 
            p.DrawImage(glyphImg, 0, 50);
        }

        void RunSampleB(CanvasPainter p)
        {
            //version 2:
            //1. create simple vertical line to test agg's lcd rendernig technique
            //create gray-scale actual image
            ActualImage glyphImg = new ActualImage(100, 100, PixelFormat.ARGB32);
            ImageGraphics2D glyph2d = new ImageGraphics2D(glyphImg);
            AggCanvasPainter painter = new AggCanvasPainter(glyph2d);

            painter.StrokeColor = PixelFarm.Drawing.Color.Black;
            painter.StrokeWidth = 2.0f;
            painter.Line(2, 0, 3, 15);//not need to scale3

            //clear surface bg
            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            var aggPainer = (PixelFarm.Agg.AggCanvasPainter)p;
            BlendWithLcdTechnique(aggPainer.Graphics.DestActualImage, glyphImg, PixelFarm.Drawing.Color.Black);
            //--------------- 
            p.DrawImage(glyphImg, 0, 50);
            //--------------- 
        }
        void RunSampleC(CanvasPainter p)
        {
            //version 3:  
            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 1.0f;
            p.UseSubPixelRendering = true;
            p.Line(0, 0, 15, 20);
            //p.UseSubPixelRendering = false;
            //p.Line(30, 0, 45, 20);
            //--------------------------


        }
        public override void Draw(CanvasPainter p)
        {
            //specific for agg

            if (!(p is PixelFarm.Agg.AggCanvasPainter))
            {
                return;
            }
            switch (Sample)
            {
                default: throw new NotSupportedException();
                case Sample.A:
                    RunSampleA(p);
                    break;
                case Sample.B:
                    RunSampleB(p);
                    break;
                case Sample.C:
                    RunSampleC(p);
                    break;
            }

        }

        static LcdDistributionLut g8_1_2lcd = new LcdDistributionLut(LcdDistributionLut.GrayLevels.Gray8, 0.5, 0.25, 0.125);
        void BlendWithLcdTechnique(ActualImage destImg, ActualImage glyphImg, PixelFarm.Drawing.Color color)
        {
            var g8Lut = g8_1_2lcd;
            var forwardBuffer = new ScanlineSubPixelRasterizer.ForwardTemporaryBuffer();
            int glyphH = glyphImg.Height;
            int glyphW = glyphImg.Width;
            byte[] glyphBuffer = ActualImage.GetBuffer(glyphImg);
            int srcIndex = 0;
            int srcStride = glyphImg.Stride;
            byte[] destImgBuffer = ActualImage.GetBuffer(destImg);
            //start pixel
            int destImgIndex = 0;
            int destX = 0;
            byte[] rgb = new byte[]{
                color.R,
                color.G,
                color.B
            };

            byte color_a = color.alpha;

            for (int y = 0; y < glyphH; ++y)
            {
                srcIndex = srcStride * y;
                destImgIndex = (destImg.Stride * y) + (destX * 4); //4 color component
                int i = 0;
                int round = 0;
                forwardBuffer.Reset();
                byte e0 = 0;
                for (int x = 0; x < glyphW; ++x)
                {
                    //1.
                    //read 1 pixel (4 bytes, 4 color components)
                    byte r = glyphBuffer[srcIndex];
                    byte g = glyphBuffer[srcIndex + 1];
                    byte b = glyphBuffer[srcIndex + 2];
                    byte a = glyphBuffer[srcIndex + 3];
                    //2.
                    //convert to grey scale and convert to 65 level grey scale value 
                    byte greyScaleValue = g8Lut.Convert255ToLevel(a);
                    //3.
                    //from single grey scale value it is expanded into 5 color component
                    for (int n = 0; n < 3; ++n)
                    {
                        forwardBuffer.WriteAccum(
                            g8Lut.Tertiary(greyScaleValue),
                            g8Lut.Secondary(greyScaleValue),
                            g8Lut.Primary(greyScaleValue));
                        //4. read accumulate 'energy' back 
                        forwardBuffer.ReadNext(out e0);
                        //5. blend this pixel to dest image (expand to 5 (sub)pixel) 
                        //------------------------------------------------------------
                        ScanlineSubPixelRasterizer.BlendSpan(e0 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);

                    }
                    srcIndex += 4;
                }
                //---------
                //when finish each line
                //we must draw extened 4 pixels
                //---------
                {
                    byte e1, e2, e3, e4;
                    forwardBuffer.ReadRemaining4(out e1, out e2, out e3, out e4);
                    int remainingEnergy = Math.Min(srcStride, 4);
                    switch (remainingEnergy)
                    {
                        default: throw new NotSupportedException();
                        case 4:
                            ScanlineSubPixelRasterizer.BlendSpan(e1 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
                            ScanlineSubPixelRasterizer.BlendSpan(e2 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
                            ScanlineSubPixelRasterizer.BlendSpan(e3 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
                            ScanlineSubPixelRasterizer.BlendSpan(e4 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
                            break;
                        case 3:
                            ScanlineSubPixelRasterizer.BlendSpan(e1 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
                            ScanlineSubPixelRasterizer.BlendSpan(e2 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
                            ScanlineSubPixelRasterizer.BlendSpan(e3 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
                            break;
                        case 2:
                            ScanlineSubPixelRasterizer.BlendSpan(e1 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
                            ScanlineSubPixelRasterizer.BlendSpan(e2 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
                            break;
                        case 1:
                            ScanlineSubPixelRasterizer.BlendSpan(e1 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
                            break;
                        case 0:
                            //nothing
                            break;
                    }
                }
            }
        }



        /// <summary>
        /// convert from original grey scale to expand lcd-ready grey scale ***
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcW"></param>
        /// <param name="srcH"></param>
        /// <param name="newImageStride"></param>
        /// <returns></returns>
        static byte[] CreateNewExpandedLcdGrayScale(byte[] src, int srcW, int srcH, out int newImageStride)
        {
            //version 1:
            //agg lcd test
            //lcd_distribution_lut<ggo_gray8> lut(1.0/3.0, 2.0/9.0, 1.0/9.0);
            //lcd_distribution_lut<ggo_gray8> lut(0.5, 0.25, 0.125);
            LcdDistributionLut lut = g8_1_2lcd;
            int destImgStride = srcW + 4; //expand the original gray scale 
            newImageStride = destImgStride;

            byte[] destBuffer = new byte[destImgStride * srcH];


            int destImgIndex = 0;
            int srcImgIndex = 0;
            for (int y = 0; y < srcH; ++y)
            {

                //find destination img
                srcImgIndex = y * srcW;
                destImgIndex = y * destImgStride; //start at new line  
                for (int x = 0; x < srcW; ++x)
                {
                    //convert to grey scale  
                    int v = src[srcImgIndex];// (int)((greyScaleValue / 255f) * 65f);
                    //----------------------------------
                    destBuffer[destImgIndex] += lut.Tertiary(v);
                    destBuffer[destImgIndex + 1] += lut.Secondary(v);
                    destBuffer[destImgIndex + 2] += lut.Primary(v);
                    destBuffer[destImgIndex + 3] += lut.Secondary(v);
                    destBuffer[destImgIndex + 4] += lut.Tertiary(v);
                    destImgIndex++;
                    srcImgIndex++;
                }
            }
            return destBuffer;
        }
    }
}



