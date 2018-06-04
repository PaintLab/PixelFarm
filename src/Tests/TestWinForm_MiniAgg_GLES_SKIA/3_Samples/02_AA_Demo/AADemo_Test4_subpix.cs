//MIT, 2017-2018, WinterDev 
using System;
using PixelFarm.Drawing;
using PixelFarm.Agg;
using Mini;
namespace PixelFarm.Agg.Sample_AADemoTest4
{

    public enum Sample
    {
        A,
        B,
        C,
        D,
        E,
        F,
    }


    [Info(OrderCode = "02")]
    [Info("SubPixelRendering_4")]
    public class AADemoTest4_subpix : DemoBase
    {
        public AADemoTest4_subpix()
        {
            this.EnableSubPix = false;
        }
        static byte[] CreateGreyScaleBuffer(ActualImage img)
        {
            //assume img is 32 rgba img
            int imgW = img.Width;
            int height = img.Height;
            //56 level grey scale buffer


            TempMemPtr srcMemPtr = ActualImage.GetBufferPtr(img);

            int greyScaleBufferLen = imgW * height;
            byte[] greyScaleBuffer = new byte[greyScaleBufferLen];

            int destIndex = 0;
            int srcImgIndex = 0;
            int srcImgStride = img.Stride;
            unsafe
            {
                byte* srcImgBuffer = (byte*)srcMemPtr.Ptr;
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
            }

            srcMemPtr.Release();
            return greyScaleBuffer;
        }
        void Blend(ActualImage destImg, byte[] greyBuff, int greyBufferWidth, int greyBufferHeight)
        {
            PixelFarm.Drawing.Color color = PixelFarm.Drawing.Color.Black;
            for (int y = 0; y < greyBufferHeight; ++y)
            {
                BlendScanline(destImg, greyBuff, color, 0, y, greyBufferWidth);
            }
            //SwapRB(destImg);
        }


        void BlendScanline(ActualImage destImg, byte[] expandGreyBuffer,
         PixelFarm.Drawing.Color color, int x, int y, int width)

        {
            byte[] rgb = new byte[3]{
                color.R,
                color.G,
                color.B
            };
            //-------------------------
            //destination

            TempMemPtr memPtr = ActualImage.GetBufferPtr(destImg);
            //start pixel
            int destImgIndex = (x * 4) + (destImg.Stride * y);
            //start img src
            int srcImgIndex = x + (width * y);
            int colorIndex = 0;
            int round = 0;
            byte color_a = color.alpha;
            unsafe
            {
                byte* destImgBuffer = (byte*)memPtr.Ptr;
                while (width > 3)
                {
                    int a0 = expandGreyBuffer[srcImgIndex] * color_a;
                    int a1 = expandGreyBuffer[srcImgIndex + 1] * color_a;
                    int a2 = expandGreyBuffer[srcImgIndex + 2] * color_a;

                    byte ec0 = destImgBuffer[destImgIndex];//existing color
                    byte ec1 = destImgBuffer[destImgIndex + 1];//existing color
                    byte ec2 = destImgBuffer[destImgIndex + 2];//existing color 
                                                               //------------------------------------------------------
                                                               //please note that we swap a2 and a0 on the fly****
                                                               //------------------------------------------------------
                    byte n0 = (byte)((((rgb[colorIndex] - ec0) * a2) + (ec0 << 16)) >> 16);
                    byte n1 = (byte)((((rgb[colorIndex + 1] - ec1) * a1) + (ec1 << 16)) >> 16);
                    byte n2 = (byte)((((rgb[colorIndex + 2] - ec2) * a0) + (ec2 << 16)) >> 16);

                    destImgBuffer[destImgIndex] = n0;
                    destImgBuffer[destImgIndex + 1] = n1;
                    destImgBuffer[destImgIndex + 2] = n2;

                    destImgIndex += 4;
                    round = 0;
                    colorIndex = 0;
                    srcImgIndex += 3;
                    width -= 3;
                }
                memPtr.Release();
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
        public bool EnableSubPix
        {
            get; set;
        }

        [DemoConfig]
        public Sample Sample
        {
            get;
            set;
        }

        void RunSampleA(PixelFarm.Drawing.Painter p)
        {

            //1. create simple vertical line to test agg's lcd rendernig technique
            //create gray-scale actual image
            ActualImage glyphImg = new ActualImage(100, 100);
            AggRenderSurface glyph2d = new AggRenderSurface(glyphImg);
            AggPainter painter = new AggPainter(glyph2d);

            painter.StrokeColor = PixelFarm.Drawing.Color.Black;
            painter.StrokeWidth = 2.0f * 3;
            int x = 10, y = 10;
            painter.DrawLine(x * 3, 0, y * 3, 20); //scale horizontal 3 times, 
            int lineLen = 4;


            //painter.Line(x * 3, 0, y * 3, 20); //scale horizontal 3 times, 
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
            //---------------

            //swap gray scale 
            int newGreyImgStride;
            byte[] expanedGreyScaleBuffer = CreateNewExpandedLcdGrayScale(glyphGreyScale, glyphImg.Width, glyphImg.Height, out newGreyImgStride);

            //blend lcd 
            var aggPainer = (PixelFarm.Agg.AggPainter)p;
            Blend(aggPainer.RenderSurface.DestActualImage, expanedGreyScaleBuffer, newGreyImgStride, glyphImg.Height);
            //--------------- 
            p.DrawImage(glyphImg, 0, 50);
        }

        void RunSampleB(PixelFarm.Drawing.Painter p)
        {
            //version 2:
            //1. create simple vertical line to test agg's lcd rendernig technique
            //create gray-scale actual image
            ActualImage glyphImg = new ActualImage(100, 100);
            AggRenderSurface glyph2d = new AggRenderSurface(glyphImg);
            AggPainter painter = new AggPainter(glyph2d);
            //
            painter.StrokeColor = PixelFarm.Drawing.Color.Black;
            painter.StrokeWidth = 2.0f;
            painter.DrawLine(2, 0, 3, 15);//not need to scale3                        
            //
            //clear surface bg
            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            var aggPainer = (PixelFarm.Agg.AggPainter)p;
            BlendWithLcdTechnique(aggPainer.RenderSurface.DestActualImage, glyphImg, PixelFarm.Drawing.Color.Black);
            //--------------- 
            p.DrawImage(glyphImg, 0, 50);
            //--------------- 
        }
        void RunSampleC(PixelFarm.Drawing.Painter p)
        {
            //version 3:  
            p.Clear(PixelFarm.Drawing.Color.White);
            //---------------------------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 1.0f;
            p.UseSubPixelLcdEffect = this.EnableSubPix;
            p.DrawLine(0, 1, 15, 20);
        }
        static double DegToRad(double degree)
        {
            return degree * (Math.PI / 180d);
        }
        static double RadToDeg(double degree)
        {
            return degree * (180d / Math.PI);
        }

        void RunSampleD(PixelFarm.Drawing.Painter p)
        {
            //version 4: 
            p.Clear(PixelFarm.Drawing.Color.White);
            p.UseSubPixelLcdEffect = this.EnableSubPix;
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 1.0f;
            //p.Line(2, 0, 10, 15);

            int lineLen = 10;
            int x = 30;
            int y = 30;
            for (int i = 0; i < 360; i += 15)
            {
                p.DrawLine(x, y, x + lineLen * Math.Cos(DegToRad(i)), y + lineLen * Math.Sin(DegToRad(i)));
                y += 5;
            }
            //y += 10;
            //for (int i = 0; i < 360; i += 360)
            //{
            //    p.Line(x, y, x + lineLen * Math.Cos(DegToRad(i)), y + lineLen * Math.Sin(DegToRad(i)));
            //}
        }
        void RunSampleE(PixelFarm.Drawing.Painter p)
        {
            //version 4: 
            p.Clear(PixelFarm.Drawing.Color.White);
            p.UseSubPixelLcdEffect = this.EnableSubPix;
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 2.0f;
            p.DrawLine(2, 0, 10, 15);

            int lineLen = 10;
            int x = 30;
            int y = 30;
            p.FillColor = PixelFarm.Drawing.Color.Black;
            p.FillRect(0, 0, 1, 1);

            for (int i = 0; i < 360; i += 30)
            {
                p.DrawLine(x, y, x + lineLen * Math.Cos(DegToRad(i)), y + lineLen * Math.Sin(DegToRad(i)));

            }
            y += 10;
            for (int i = 0; i < 360; i += 360)
            {
                p.DrawLine(x, y, x + lineLen * Math.Cos(DegToRad(i)), y + lineLen * Math.Sin(DegToRad(i)));
            }
        }
        void RunSampleF(PixelFarm.Drawing.Painter p)
        {
            //version 4: 
            p.Clear(PixelFarm.Drawing.Color.White);
            p.UseSubPixelLcdEffect = this.EnableSubPix;
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 2.0f;
            p.DrawLine(2, 0, 10, 15);

            int lineLen = 10;
            int x = 30;
            int y = 30;
            p.FillColor = PixelFarm.Drawing.Color.Black;

         
            using (System.IO.FileStream fs = new System.IO.FileStream("c:\\Windows\\Fonts\\tahoma.ttf", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                Typography.OpenFont.OpenFontReader reader = new Typography.OpenFont.OpenFontReader();
                Typography.OpenFont.Typeface typeface = reader.Read(fs);


                var builder = new Typography.Contours.GlyphPathBuilder(typeface);
                builder.BuildFromGlyphIndex((ushort)typeface.LookupIndex('C'), 16);
                PixelFarm.Drawing.Fonts.GlyphTranslatorToVxs tovxs = new Drawing.Fonts.GlyphTranslatorToVxs();
                builder.ReadShapes(tovxs);
                VertexStore vxs = new VertexStore();
                tovxs.WriteOutput(vxs);
                p.Fill(vxs);
            }
            p.FillRect(0, 0, 20, 20);

        }

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            //specific for agg

            if (!(p is PixelFarm.Agg.AggPainter))
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
                case Sample.D:
                    RunSampleD(p);
                    break;
                case Sample.E:
                    RunSampleE(p);
                    break;
                case Sample.F:
                    RunSampleF(p);
                    break;

            }

        }

        static LcdDistributionLut g8_4_2_1 = new LcdDistributionLut(64, 4 / 8f, 2 / 8f, 1 / 8f);
        void BlendWithLcdTechnique(ActualImage destImg, ActualImage glyphImg, PixelFarm.Drawing.Color color)
        {
            //var g8Lut = g8_4_2_1;
            //var forwardBuffer = new ScanlineSubPixelRasterizer.TempForwardAccumBuffer();
            //int glyphH = glyphImg.Height;
            //int glyphW = glyphImg.Width;
            //byte[] glyphBuffer = ActualImage.GetBuffer(glyphImg);
            //int srcIndex = 0;
            //int srcStride = glyphImg.Stride;
            //byte[] destImgBuffer = ActualImage.GetBuffer(destImg);
            ////start pixel
            //int destImgIndex = 0;
            //int destX = 0;
            //byte[] rgb = new byte[]{
            //    color.R,
            //    color.G,
            //    color.B
            //};

            //byte color_a = color.alpha;

            //for (int y = 0; y < glyphH; ++y)
            //{
            //    srcIndex = srcStride * y;
            //    destImgIndex = (destImg.Stride * y) + (destX * 4); //4 color component
            //    int i = 0;
            //    int round = 0;
            //    forwardBuffer.Reset();
            //    byte e0 = 0;
            //    int prev_a = 0;

            //    for (int x = 0; x < glyphW; ++x)
            //    {
            //        //1.
            //        //read 1 pixel (4 bytes, 4 color components)
            //        byte r = glyphBuffer[srcIndex];
            //        byte g = glyphBuffer[srcIndex + 1];
            //        byte b = glyphBuffer[srcIndex + 2];
            //        byte a = glyphBuffer[srcIndex + 3];


            //        //2.
            //        //convert to grey scale and convert to 65 level grey scale value 
            //        byte greyScaleValue = g8Lut.Convert255ToLevel(a);

            //        for (int n = 0; n < 3; ++n)
            //        {

            //            forwardBuffer.WriteAccumAndReadBack(
            //             g8Lut.TertiaryFromLevel(greyScaleValue),
            //             g8Lut.SecondaryFromLevel(greyScaleValue),
            //             g8Lut.PrimaryFromLevel(greyScaleValue), out e0);

            //            //5. blend this pixel to dest image (expand to 5 (sub)pixel)                          
            //            BlendPixel(e0 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //        }
            //        //------------------------------------------------------------
            //        prev_a = a;
            //        srcIndex += 4;
            //    }
            //    //---------
            //    //when finish each line
            //    //we must draw extened 4 pixels
            //    //---------

            //    {


            //        byte e1, e2, e3, e4;
            //        forwardBuffer.ReadRemaining4(out e1, out e2, out e3, out e4);
            //        int remainingEnergy = Math.Min(srcStride, 4);
            //        switch (remainingEnergy)
            //        {
            //            default: throw new NotSupportedException();
            //            case 4:
            //                BlendPixel(e1 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //                BlendPixel(e2 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //                BlendPixel(e3 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //                BlendPixel(e4 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //                break;
            //            case 3:
            //                BlendPixel(e1 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //                BlendPixel(e2 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //                BlendPixel(e3 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //                break;
            //            case 2:
            //                BlendPixel(e1 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //                BlendPixel(e2 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //                break;
            //            case 1:
            //                BlendPixel(e1 * color_a, rgb, ref i, destImgBuffer, ref destImgIndex, ref round);
            //                break;
            //            case 0:
            //                //nothing
            //                break;
            //        }
            //    }
            //}
        }

        static void BlendPixel(int a0, byte[] rgb, ref int color_index, byte[] destImgBuffer, ref int destImgIndex, ref int round)
        {
            //a0 = energy * color_alpha
            byte existingColor = destImgBuffer[destImgIndex];
            byte newValue = (byte)((((rgb[color_index] - existingColor) * a0) + (existingColor << 16)) >> 16);
            destImgBuffer[destImgIndex] = newValue;
            //move to next dest
            destImgIndex++;
            color_index++;
            if ((color_index) > 2)
            {
                color_index = 0;//reset
            }
            round++;
            if ((round) > 2)
            {
                //this is alpha chanel
                //so we skip alpha byte to next
                //and swap rgb of latest write pixel
                //-------------------------- 
                //TODO: review here, not correct
                //in-place swap
                byte r1 = destImgBuffer[destImgIndex - 1];
                byte b1 = destImgBuffer[destImgIndex - 3];
                destImgBuffer[destImgIndex - 3] = r1;
                destImgBuffer[destImgIndex - 1] = b1;
                //-------------------------- 
                destImgIndex++;//skip alpha chanel
                round = 0;
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
            LcdDistributionLut lut = g8_4_2_1;
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
                    destBuffer[destImgIndex] += lut.TertiaryFromLevel(v);
                    destBuffer[destImgIndex + 1] += lut.SecondaryFromLevel(v);
                    destBuffer[destImgIndex + 2] += lut.PrimaryFromLevel(v);
                    destBuffer[destImgIndex + 3] += lut.SecondaryFromLevel(v);
                    destBuffer[destImgIndex + 4] += lut.TertiaryFromLevel(v);
                    destImgIndex++;
                    srcImgIndex++;
                }
            }
            return destBuffer;
        }
    }
}



