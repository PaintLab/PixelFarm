//MIT,2016, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PixelFarm.Drawing.Fonts;

namespace BuildTextureFonts
{
    public partial class Form1 : Form
    {
        //-----------------------
        //how far from edge of each pixel
        //-----------------------

        Font font;
        Graphics panel1Gfx;
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.BackColor = Color.White;
            font = new Font("Tahoma", 24, FontStyle.Regular);
            panel1Gfx = panel1.CreateGraphics();
        }
        private void cmdBuild_Click(object sender, EventArgs e)
        {

            SizeF size = panel1Gfx.MeasureString("A", font);
            Bitmap bmp1 = new Bitmap((int)size.Width, (int)size.Height);
            using (Graphics g = Graphics.FromImage(bmp1))
            {
                g.Clear(Color.Black);
                size = g.MeasureString("A", font);
                g.DrawString("A", font, Brushes.White, new PointF(0, 0));
                panel1Gfx.DrawImage(bmp1, new Point(0, 0));
            }
            //-----------------------------------------------------------
            //then analysis of that font
            //-----------------------------------------------------------
            //copy data from bmp1
            //and analyze each pixel : 
            bmp1.Save("d:\\WImageTest\\a001_m.png");
            //copy pixel data
            var bmpdata = bmp1.LockBits(new Rectangle(0, 0, (int)size.Width, (int)size.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp1.PixelFormat);
            int bmpHeight = bmpdata.Height;
            int bmpWidth = bmpdata.Width;
            //byte[] buffer = new byte[bmpdata.Height * bmpdata.Stride];
            int[] intBuffer = new int[bmpWidth * bmpHeight];//32 argb **
            System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, intBuffer, 0, intBuffer.Length);
            bmp1.UnlockBits(bmpdata);
            //--------------------------------------------------------------
            //process each scanline pixel***

            int[] distanceBuffer = new int[bmpWidth * bmpHeight];//distance count
            DepthAnalysisXAxis(intBuffer, bmpWidth, bmpHeight, distanceBuffer);   //1st pass horizontal scanline 
            //DepthAnalysisYAxis(intBuffer, bmpWidth, bmpHeight, distanceBuffer);

            {
                //test output
                var outputBmp = new Bitmap(bmpWidth, bmpHeight);
                var outputBmpData = outputBmp.LockBits(new Rectangle(0, 0, (int)size.Width, (int)size.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    bmp1.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(distanceBuffer, 0, outputBmpData.Scan0, distanceBuffer.Length);
                outputBmp.UnlockBits(outputBmpData);
                outputBmp.Save("d:\\WImageTest\\a001_x.png");
                //--------
            }
        }
        static void RotateLeft90(int[] intBuffer, int bmpWidth, int bmpHeight, int[] outputBuffer)
        {
            int targetPos = 0;
            for (int c = 0; c < bmpWidth; ++c)
            {
                int startSrcPos = bmpWidth - (c + 1);
                for (int row = 0; row < bmpHeight; ++row)
                {
                    outputBuffer[targetPos] = intBuffer[startSrcPos];
                    targetPos++;
                    startSrcPos += bmpWidth;
                }
            }
        }
        static void RotateRight90(int[] intBuffer, int bmpWidth, int bmpHeight, int[] outputBuffer)
        {
            //int targetPos = 0;
            //for (int row = bmpHeight - 1; row >= 0; --row)
            //{
            //    int startSrcPos = row * bmpWidth;
            //    for (int c = 0; c < bmpWidth; ++c)
            //    {
            //        outputBuffer[targetPos] = intBuffer[startSrcPos];
            //        targetPos++;
            //        startSrcPos++;
            //    }
            //}
            int targetPos = 0;
            for (int c = 0; c < bmpWidth; ++c)
            {
                for (int row = bmpHeight - 1; row >= 0; --row)
                {
                    int startSrcPos = (row * bmpWidth) + c;
                    outputBuffer[targetPos] = intBuffer[startSrcPos];
                    targetPos++;
                }
            }
        }
        static void DepthAnalysisYAxis(int[] intBuffer, int bmpWidth, int bmpHeight, int[] distanceBuffer)
        {
            //rotate left 90
            int r90W = bmpHeight;
            int r90H = bmpWidth;
            int[] rotateLeft90 = new int[r90W * r90H];
            int[] r90DistanceBuffer = new int[r90W * r90H];
            RotateLeft90(intBuffer, bmpWidth, bmpHeight, rotateLeft90);
            //{
            //    //test output
            //    var outputBmp = new Bitmap(bmpHeight, bmpWidth);
            //    var outputBmpData = outputBmp.LockBits(new Rectangle(0, 0, (int)bmpHeight, (int)bmpWidth), System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //       System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //    System.Runtime.InteropServices.Marshal.Copy(rotateLeft90, 0, outputBmpData.Scan0, distanceBuffer.Length);
            //    outputBmp.UnlockBits(outputBmpData);
            //    outputBmp.Save("d:\\WImageTest\\a001_x.png");
            //}

            DepthAnalysisXAxis(rotateLeft90, r90W, r90H, r90DistanceBuffer);
            //{
            //    var outputBmp = new Bitmap(r90W, r90H);
            //    var outputBmpData = outputBmp.LockBits(new Rectangle(0, 0, (int)r90W, (int)r90H), System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //       System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //    System.Runtime.InteropServices.Marshal.Copy(r90DistanceBuffer, 0, outputBmpData.Scan0, r90DistanceBuffer.Length);
            //    outputBmp.UnlockBits(outputBmpData);
            //    outputBmp.Save("d:\\WImageTest\\a001_x.png");
            //} 

            RotateRight90(r90DistanceBuffer, r90W, r90H, distanceBuffer);

        }
        static void DepthAnalysisXAxis(int[] intBuffer, int bmpWidth, int bmpHeight, int[] distanceBuffer)
        {
            int i = 0;
            int p = 0;

            List<int> pxCollection = new List<int>();
            bool glyphArea = false;
            for (int row = 0; row < bmpHeight; ++row)
            {
                glyphArea = false;
                pxCollection.Clear();
                //int prevLevel = 0;
                //int currentStripLen = 0;
                //row   
                for (int c = 0; c < bmpWidth; ++c)
                {
                    int pixel = intBuffer[i];
                    int a = (pixel >> 24) & 0xff;
                    int b = (pixel >> 16) & 0xff;
                    int g = (pixel >> 8) & 0xff;
                    int r = (pixel) & 0xff;
                    //convert to grey scale
                    double level = ((0.2126 * r) + (0.7152 * g) + (0.0722) * b);
                    //int luminosity method;
                    // R' = G' = B' = 0.2126R + 0.7152G + 0.0722B 

                    if (level > 0)
                    {
                        //this enter glyph area
                        if (!glyphArea)
                        {
                            //just  enter the glyph area
                            //clear existing data in collection strip
                            if (pxCollection.Count > 0)
                            {
                                FillDataXAxis(distanceBuffer, p, pxCollection, false);
                            }
                            pxCollection.Clear();
                            pxCollection.Add((int)level);
                            glyphArea = true;
                            p = i;
                        }
                        else
                        {
                            //we alreary in glyph area
                            //so collect strip len
                            pxCollection.Add((int)level);
                        }

                    }
                    else
                    {
                        //now we are not in glyph area
                        if (!glyphArea)
                        {
                            //already not in glyph area
                            pxCollection.Add((int)level);
                        }
                        else
                        {

                            //just exit glyph area
                            if (pxCollection.Count > 0)
                            {
                                FillDataXAxis(distanceBuffer, p, pxCollection, true);
                            }
                            pxCollection.Clear();
                            pxCollection.Add((int)level);
                            glyphArea = false;
                            p = i;
                        }
                    }
                    ++i;
                }
                //---------------------------
                //exit
                //fill remaining databack
                if (pxCollection.Count > 0)
                {
                    FillDataXAxis(distanceBuffer, p, pxCollection, glyphArea);
                    p = i;
                }
            }
        }
        //static void DepthAnalysisXAxis2(int[] intBuffer, int bmpWidth, int bmpHeight, int[] distanceBuffer)
        //{
        //    int i = 0;
        //    int p = 0;
        //    for (int row = 0; row < bmpHeight; ++row)
        //    {
        //        int prevLevel = 0;
        //        int currentStripLen = 0;
        //        //row 
        //        int cut = 0;
        //        for (int c = 0; c < bmpWidth; ++c)
        //        {
        //            int pixel = intBuffer[i];
        //            int a = (pixel >> 24) & 0xff;
        //            int b = (pixel >> 16) & 0xff;
        //            int g = (pixel >> 8) & 0xff;
        //            int r = (pixel >> 8) & 0xff;
        //            //convert to grey scale
        //            int level = (int)((0.2126 * r) + (0.7152 * g) + (0.0722) * b);
        //            //int luminosity method;
        //            // R' = G' = B' = 0.2126R + 0.7152G + 0.0722B 
        //            if (level > 0)
        //            {
        //                level = 255;
        //            }
        //            if (level != prevLevel)
        //            {
        //                if (currentStripLen > 0)
        //                {
        //                    //fill data
        //                    FillDataXAxis(distanceBuffer, p, currentStripLen, (cut % 2) != 0);
        //                    cut++;
        //                }
        //                else
        //                {

        //                }
        //                currentStripLen = 1;
        //                p = i;
        //                prevLevel = level;
        //            }
        //            else
        //            {
        //                //same level
        //                currentStripLen++;
        //            }
        //            ++i;
        //        }
        //        //---------------------------
        //        //exit
        //        //fill remaining databack
        //        if (currentStripLen > 0)
        //        {
        //            FillDataXAxis(distanceBuffer, p, currentStripLen, (cut % 2) != 0);
        //            p = i;
        //        }
        //    }
        //}
        const int SCALE = 20;
        const int MAX_LEVEL = 5;
        static void FillDataXAxis(int[] outputPixels, int startIndex, List<int> levelCollection, bool inside)
        {

            int count = levelCollection.Count;
            if (inside)
            {
                //inside polygon
                int n = count;
                int p = startIndex;
                if ((count % 2) != 0)
                {
                    //odd number
                    int eachSide = ((count - 1) / 2);
                    //start(left side)
                    FillStrip(levelCollection, 0, outputPixels, startIndex, eachSide, MAX_LEVEL, true, true);
                    //middle
                    //-----------------
                    startIndex += eachSide;
                    if (eachSide > MAX_LEVEL)
                    {
                        // outputPixels[startIndex] = (255 << 24) | (((MAX_LEVEL + 1) * SCALE) << Current_INSIDE_SHIFT);

                        outputPixels[startIndex] = (255 << 24) | levelCollection[eachSide];
                    }
                    else
                    {

                        //  outputPixels[startIndex] = (255 << 24) | (((eachSide + 1) * SCALE) << Current_INSIDE_SHIFT);
                        outputPixels[startIndex] = (255 << 24) | levelCollection[eachSide];
                    }
                    startIndex += 1;
                    //-----------------
                    //right side 
                    FillStrip(levelCollection, eachSide + 1, outputPixels, startIndex, eachSide, MAX_LEVEL, true, false);
                }
                else
                {
                    //even number
                    int eachSide = count / 2;
                    //start(left side)
                    FillStrip(levelCollection, 0, outputPixels, startIndex, eachSide, MAX_LEVEL, true, true);
                    startIndex += eachSide;
                    //right side
                    FillStrip(levelCollection, eachSide, outputPixels, startIndex, eachSide, MAX_LEVEL, true, false);
                }
            }
            else
            {
                //outside polygon
                int n = count;
                int p = startIndex;

                //if ((count % 2) != 0)
                //{
                //    //odd number
                //    int eachSide = ((count - 1) / 2);
                //    //start(left side)

                //    FillStrip(levelCollection, 0, outputPixels, startIndex, eachSide, MAX_LEVEL, false, true);
                //    //-----------------
                //    //middle
                //    startIndex += eachSide;
                //    if (eachSide > MAX_LEVEL)
                //    {
                //        outputPixels[startIndex] = (255 << 24) | (((MAX_LEVEL + 1) * SCALE) << Current_OUTSIDE_SHIFT);
                //    }
                //    else
                //    {
                //        outputPixels[startIndex] = (255 << 24) | (((eachSide + 1) * SCALE) << Current_OUTSIDE_SHIFT);
                //    }
                //    startIndex += 1;
                //    //-----------------
                //    //right side                    
                //    FillStrip(levelCollection, eachSide + 1, outputPixels, startIndex, eachSide, MAX_LEVEL, false, false);
                //}
                //else
                //{
                //    //even number
                //    int eachSide = count / 2;
                //    //start(left side)
                //    FillStrip(levelCollection, 0, outputPixels, startIndex, eachSide, MAX_LEVEL, false, true);
                //    startIndex += eachSide;
                //    //right side
                //    FillStrip(levelCollection, eachSide, outputPixels, startIndex, eachSide, MAX_LEVEL, false, false);
                //}
            }
        }


        //16 : red
        //8 : green
        //0: blue
        static int Current_INSIDE_SHIFT = 16;
        static int Current_OUTSIDE_SHIFT = 0;

        static void FillStrip(List<int> srcLevels, int srcLevelStart, int[] outputPixels, int startIndex, int count, int maxLevel, bool inside, bool uphill)
        {
            int compoShift = inside ? Current_INSIDE_SHIFT : Current_OUTSIDE_SHIFT;
            int ss = srcLevelStart;
            if (uphill)
            {
                //from dark to bright
                int n = count;
                int p = startIndex;
                if (count > maxLevel)
                {
                    int i = 0;
                    //gradient up 
                    int value = 0;
                    int c = 1;

                    for (; i < maxLevel; ++i)
                    {
                        // outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        outputPixels[p] = value = (255 << 24) | srcLevels[ss];
                        p++;
                        c++;
                        ss++;

                    }
                    //long distance                    
                    for (; i < count; ++i)
                    {
                        //outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        outputPixels[p] = value = (255 << 24) | srcLevels[ss];
                        p++;
                        ss++;
                    }

                }
                else
                {
                    //gradient up
                    int i = 0;
                    int value = 0;
                    int c = 1;
                    for (; i < count; ++i)
                    {
                        //outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        outputPixels[p] = value = (255 << 24) | srcLevels[ss];
                        p++;
                        c++;
                        ss++;
                    }
                }
            }
            else
            {
                //downhill
                int n = count;
                int p = startIndex;
                int value = 0;
                if (count > maxLevel)
                {
                    //long distance
                    int lim = count - maxLevel;
                    int i = 0;
                    int c = maxLevel + 1;
                    for (; i < lim; ++i)
                    {
                        //  outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        outputPixels[p] = value = (255 << 24) | srcLevels[ss];
                        p++;
                        ss++;
                    }

                    //gradient down
                    c = maxLevel;
                    for (; i < count; ++i)
                    {
                        //  outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        outputPixels[p] = value = (255 << 24) | srcLevels[ss];
                        p++;
                        c--;
                        ss++;
                    }
                }
                else
                {
                    //gradient down
                    int i = 0;
                    int c = count;
                    for (; i < count; ++i)
                    {
                        // outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        outputPixels[p] = value = (255 << 24) | srcLevels[ss];
                        p++;
                        c--;
                        ss++;
                    }
                }
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //msdfgen: see more https://github.com/Chlumsky/msdfgen

            //sdf – generates a conventional monochrome signed distance field.
            //psdf – generates a monochrome signed pseudo - distance field.
            //msdf(default) – generates a multi - channel signed distance field using my new method.

            //char[] fontChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            char[] fontChars = new char[] { (char)197 };
            int j = fontChars.Length;
            for (int i = 0; i < j; ++i)
            {
                char c = fontChars[i];
                //msdf
                //string args = @"msdfgen msdf -font C:\Windows\Fonts\tahoma.ttf 'A' -o msdf.png -size 32 32 -pxrange 4 -autoframe -testrender render_msdf.png 1024 1024";
                MsdfParameters pars = new MsdfParameters(@"C:\Windows\Fonts\tahoma.ttf", c);
                pars.enableRenderTestFile = false;
                string[] splitStr = pars.GetArgs();
                MyFtLib.MyFtMSDFGEN(splitStr.Length, splitStr);
            }
            for (int i = 0; i < j; ++i)
            {
                char c = fontChars[i];
                MsdfParameters pars = new MsdfParameters(@"C:\Windows\Fonts\tahoma.ttf", c);
                pars.enableRenderTestFile = false;
                pars.useClassicSdf = true;
                string[] splitStr = pars.GetArgs();
                MyFtLib.MyFtMSDFGEN(splitStr.Length, pars.GetArgs());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int version = MyFtLib.MyFtLibGetVersion();
            IntPtr shape = MyFtLib.CreateShape();
            IntPtr cnt = MyFtLib.ShapeAddBlankContour(shape);
            MyFtLib.ContourAddLinearSegment(cnt, 10, 10, 25, 25);
            MyFtLib.ContourAddLinearSegment(cnt, 25, 25, 15, 10);
            MyFtLib.ContourAddLinearSegment(cnt, 15, 10, 10, 10);

            double s_left, s_bottom, s_right, s_top;
            MyFtLib.ShapeFindBounds(shape, out s_left, out s_bottom, out s_right, out s_top);

            //then create msdf texture
            if (!MyFtLib.ShapeValidate(shape))
            {
                throw new NotSupportedException();
            }
            MyFtLib.ShapeNormalize(shape);

            unsafe
            {
                int w = 32, h = 32;
                int[] output = new int[w * h];
                fixed (int* output_h = &output[0])
                {
                    MyFtLib.MyFtGenerateMsdf(shape, w, h, 2, 1, 1, 1, -1, 3, output_h);
                }
                //save to bmp
                using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                    System.Runtime.InteropServices.Marshal.Copy(output, 0, bmpdata.Scan0, output.Length);
                    bmp.UnlockBits(bmpdata);
                    bmp.Save("d:\\WImageTest\\a001_x1.png");
                }
            }
            MyFtLib.DeleteUnmanagedObj(shape);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //1. load font
            string fontfile = "c:\\Windows\\Fonts\\tahoma.ttf";
            PixelFarm.Drawing.Font font = NativeFontStore.LoadFont(fontfile, 28);
            //2. get glyph

            char[] fontChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            int j = fontChars.Length;


            SimpleFontAtlasBuilder atlasBuilder = new SimpleFontAtlasBuilder();
            for (int i = 0; i < j; ++i)
            {
                char c = fontChars[i];
                FontGlyph fontGlyph = font.GetGlyph(c);

                GlyphImage glyphImg = NativeFontStore.BuildMsdfFontImage(fontGlyph);
                int w = glyphImg.Width;
                int h = glyphImg.Height;
                int[] buffer = glyphImg.GetImageBuffer();
                NativeFontStore.SwapColorComponentFromBigEndianToWinGdi(buffer);
                glyphImg.SetImageBuffer(buffer, false);
                atlasBuilder.AddGlyph(c, fontGlyph, glyphImg);

                //using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //{
                //    var bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                //    bmp.UnlockBits(bmpdata);
                //    bmp.Save("d:\\WImageTest\\a001_x1_" + (int)c + ".png");

                //}
            }
            //----------------------------------------------------
            GlyphImage totalImg = atlasBuilder.BuildSingleImage();
            using (Bitmap bmp = new Bitmap(totalImg.Width, totalImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {

                int[] buffer = totalImg.GetImageBuffer();
                if (totalImg.IsBigEndian)
                {
                    NativeFontStore.SwapColorComponentFromBigEndianToWinGdi(buffer);
                    totalImg.SetImageBuffer(buffer, false);
                }

                var bmpdata = bmp.LockBits(new Rectangle(0, 0, totalImg.Width, totalImg.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                bmp.UnlockBits(bmpdata);
                bmp.Save("d:\\WImageTest\\a_total.png");
            }

            string fontfilename = "d:\\WImageTest\\a_total.xml";
            atlasBuilder.SaveFontInfo(fontfilename);
            //---------------------------------- 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string fontfilename = "d:\\WImageTest\\a_total.xml";
            SimpleFontAtlasBuilder atlasBuilder = new SimpleFontAtlasBuilder();
            SimpleFontAtlas fontAtlas = atlasBuilder.LoadFontInfo(fontfilename);

            using (Bitmap totalImg = new Bitmap("d:\\WImageTest\\a_total.png"))
            {
                var bmpdata = totalImg.LockBits(new Rectangle(0, 0, totalImg.Width, totalImg.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, totalImg.PixelFormat);
                var buffer = new int[totalImg.Width * totalImg.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
                totalImg.UnlockBits(bmpdata);
                var glyImage = new GlyphImage(totalImg.Width, totalImg.Height);
                glyImage.SetImageBuffer(buffer, false);
                fontAtlas.TotalGlyph = glyImage;
            }
        }


        static void BuildFontGlyphs(PixelFarm.Drawing.Font font, SimpleFontAtlasBuilder atlasBuilder, int startAt, int endAt)
        {
            //font glyph for specific font face
            for (int i = startAt; i <= endAt; ++i)
            {
                char c = (char)i;
                FontGlyph fontGlyph = font.GetGlyph(c);
                GlyphImage glyphImg = NativeFontStore.BuildMsdfFontImage(fontGlyph);
                int w = glyphImg.Width;
                int h = glyphImg.Height;
                int[] buffer = glyphImg.GetImageBuffer();
                NativeFontStore.SwapColorComponentFromBigEndianToWinGdi(buffer);
                glyphImg.SetImageBuffer(buffer, false);
                atlasBuilder.AddGlyph(c, fontGlyph, glyphImg);
                //using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //{
                //    var bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                //    bmp.UnlockBits(bmpdata);
                //    bmp.Save("d:\\WImageTest\\a001_x1_" + (int)c + ".png"); 
                //}
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            //1. load font
            string fontfile = "c:\\Windows\\Fonts\\tahoma.ttf";
            PixelFarm.Drawing.Font font = NativeFontStore.LoadFont(fontfile, 28);
            //2. get glyph 
            SimpleFontAtlasBuilder atlasBuilder = new SimpleFontAtlasBuilder();
            //for (int i = 0; i < 256; ++i)
            BuildFontGlyphs(font, atlasBuilder, 0, 255);
            BuildFontGlyphs(font, atlasBuilder, 0x0e00, 0x0e5b);

            //for (int i = 0x0e00; i < 0x0e5b; ++i)
            //{
            //    char c = (char)i;
            //    FontGlyph fontGlyph = font.GetGlyph(c);
            //    GlyphImage glyphImg = NativeFontStore.BuildMsdfFontImage(fontGlyph);
            //    int w = glyphImg.Width;
            //    int h = glyphImg.Height;
            //    int[] buffer = glyphImg.GetImageBuffer();
            //    NativeFontStore.SwapColorComponentFromBigEndianToWinGdi(buffer);
            //    glyphImg.SetImageBuffer(buffer, false);
            //    atlasBuilder.AddGlyph(c, fontGlyph, glyphImg);
            //    //using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //    //{
            //    //    var bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            //    //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
            //    //    bmp.UnlockBits(bmpdata);
            //    //    bmp.Save("d:\\WImageTest\\a001_x1_" + (int)c + ".png"); 
            //    //}
            //}
            //----------------------------------------------------
            GlyphImage totalImg = atlasBuilder.BuildSingleImage();
            using (Bitmap bmp = new Bitmap(totalImg.Width, totalImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {

                int[] buffer = totalImg.GetImageBuffer();
                if (totalImg.IsBigEndian)
                {
                    NativeFontStore.SwapColorComponentFromBigEndianToWinGdi(buffer);
                    totalImg.SetImageBuffer(buffer, false);
                }

                var bmpdata = bmp.LockBits(new Rectangle(0, 0, totalImg.Width, totalImg.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                bmp.UnlockBits(bmpdata);
                bmp.Save("d:\\WImageTest\\a_total.png");
            }

            string fontfilename = "d:\\WImageTest\\a_total.xml";
            atlasBuilder.SaveFontInfo(fontfilename);
            //---------------------------------- 
        }
    }
}
