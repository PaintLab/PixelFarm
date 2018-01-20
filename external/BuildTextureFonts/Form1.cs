//MIT, 2016-2018, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PixelFarm.Drawing.Fonts;
using Typography.Rendering;
using Typography.Contours;

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
            var bmpdata = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, (int)size.Width, (int)size.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
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
                var outputBmpData = outputBmp.LockBits(new System.Drawing.Rectangle(0, 0, (int)size.Width, (int)size.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
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
            char[] fontChars = new char[] { (char)'X' };
            int j = fontChars.Length;
            for (int i = 0; i < j; ++i)
            {
                char c = fontChars[i];
                //msdf
                //string args = @"msdfgen msdf -font C:\Windows\Fonts\tahoma.ttf 'A' -o msdf.png -size 32 32 -pxrange 4 -autoframe -testrender render_msdf.png 1024 1024";
                //MsdfParameters pars = new MsdfParameters(@"C:\Windows\Fonts\tahoma.ttf", c);
                MsdfParameters pars = new MsdfParameters(@"d:\\WImageTest\\shardailes.ttf", c);
                pars.enableRenderTestFile = false;
                string[] splitStr = pars.GetArgs();
                MyFtLib.MyFtMSDFGEN(splitStr.Length, splitStr);
            }
            //for (int i = 0; i < j; ++i)
            //{
            //    char c = fontChars[i];
            //    MsdfParameters pars = new MsdfParameters(@"C:\Windows\Fonts\tahoma.ttf", c);
            //    pars.enableRenderTestFile = false;
            //    pars.useClassicSdf = true;

            //    string[] splitStr = pars.GetArgs();
            //    MyFtLib.MyFtMSDFGEN(splitStr.Length, pars.GetArgs());
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int version = MyFtLib.MyFtLibGetVersion();
            IntPtr shape = MyFtLib.CreateShape();
            IntPtr cnt = MyFtLib.ShapeAddBlankContour(shape);
            //MyFtLib.ContourAddLinearSegment(cnt, 10, 10, 25, 25);
            //MyFtLib.ContourAddLinearSegment(cnt, 25, 25, 15, 10);
            //MyFtLib.ContourAddLinearSegment(cnt, 15, 10, 10, 10);

            MyFtLib.ContourAddLinearSegment(cnt, 3.84f, 0, 1.64f, 0);
            MyFtLib.ContourAddLinearSegment(cnt, 1.64f, 0, 1.64f, 18.23f);
            MyFtLib.ContourAddLinearSegment(cnt, 1.64f, 18.23f, 3.84f, 18.23f);
            MyFtLib.ContourAddLinearSegment(cnt, 3.84f, 18.23f, 3.84f, 0);


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
                    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                    System.Runtime.InteropServices.Marshal.Copy(output, 0, bmpdata.Scan0, output.Length);
                    bmp.UnlockBits(bmpdata);
                    bmp.Save("d:\\WImageTest\\a001_x1.png");
                }
            }
            MyFtLib.DeleteUnmanagedObj(shape);
        }

        static ActualFont GetActualFont(string fontName, float size)
        {
            //in the case that we want to use FreeType
            FontFace face = FreeTypeFontLoader.LoadFont(fontName, "en", HBDirection.HB_DIRECTION_LTR);
            return face.GetFontAtPointSize(size);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            //1. load font
            string fontName = "tahoma";
            int fontSizeInPts = 28;
            ActualFont font = GetActualFont(fontName, fontSizeInPts);

            //2. get glyph 
            char[] fontChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            int j = fontChars.Length;


            var atlasBuilder = new Typography.Rendering.SimpleFontAtlasBuilder();
            atlasBuilder.SetAtlasInfo(TextureKind.Msdf, fontSizeInPts);

            for (int i = 0; i < j; ++i)
            {
                char c = fontChars[i];

                FontGlyph fontGlyph = font.GetGlyph(c);
                GlyphImage glyphImg = NativeMsdfGen.BuildMsdfFontImage(fontGlyph);

                int w = glyphImg.Width;
                int h = glyphImg.Height;
                int[] buffer = glyphImg.GetImageBuffer();
                NativeMsdfGen.SwapColorComponentFromBigEndianToWinGdi(buffer);
                glyphImg.SetImageBuffer(buffer, false);
                atlasBuilder.AddGlyph(0, glyphImg);

                using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var bmpdata = bmp.LockBits(
                        new System.Drawing.Rectangle(0, 0, w, h),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                    bmp.UnlockBits(bmpdata);
                    bmp.Save("d:\\WImageTest\\a001_x1_" + (int)c + ".png");
                }
            }
            //----------------------------------------------------
            GlyphImage totalImg = atlasBuilder.BuildSingleImage();
            using (Bitmap bmp = new Bitmap(totalImg.Width, totalImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {

                int[] buffer = totalImg.GetImageBuffer();
                if (totalImg.IsBigEndian)
                {
                    NativeMsdfGen.SwapColorComponentFromBigEndianToWinGdi(buffer);
                    totalImg.SetImageBuffer(buffer, false);
                }

                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, totalImg.Width, totalImg.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
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
            var atlasBuilder = new Typography.Rendering.SimpleFontAtlasBuilder();
            SimpleFontAtlas fontAtlas = atlasBuilder.LoadFontInfo(fontfilename);

            using (Bitmap totalImg = new Bitmap("d:\\WImageTest\\a_total.png"))
            {
                var bmpdata = totalImg.LockBits(new System.Drawing.Rectangle(0, 0, totalImg.Width, totalImg.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, totalImg.PixelFormat);
                var buffer = new int[totalImg.Width * totalImg.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
                totalImg.UnlockBits(bmpdata);
                var glyImage = new GlyphImage(totalImg.Width, totalImg.Height);
                glyImage.SetImageBuffer(buffer, false);
                fontAtlas.TotalGlyph = glyImage;
            }
        }


        static void BuildFontGlyphs(PixelFarm.Drawing.RequestFont font,
            Typography.Rendering.SimpleFontAtlasBuilder atlasBuilder,
            int startAt, int endAt)
        {
            //font glyph for specific font face
            ActualFont nativeFont = GetActualFont(font.Name, font.SizeInPoints);// nativeFontStore.GetResolvedNativeFont(font);
            for (int i = startAt; i <= endAt; ++i)
            {
                char c = (char)i;
                FontGlyph fontGlyph = nativeFont.GetGlyph(c);
                //-------------------
                GlyphImage glyphImg = NativeMsdfGen.BuildMsdfFontImage(fontGlyph);

                // Console.WriteLine(c.ToString() + " ox,oy" + glyphImg.OffsetX + "," + glyphImg.OffsetY);

                int w = glyphImg.Width;
                int h = glyphImg.Height;
                int[] buffer = glyphImg.GetImageBuffer();
                NativeMsdfGen.SwapColorComponentFromBigEndianToWinGdi(buffer);
                glyphImg.SetImageBuffer(buffer, false);
                // atlasBuilder.AddGlyph(fontGlyph.glyphMatrix.u c, fontGlyph, glyphImg);
                //using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //{
                //    var bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                //    bmp.UnlockBits(bmpdata);
                //    bmp.Save("d:\\WImageTest\\a001_x1_" + (int)c + ".png"); 
                //}
            }
        }
        static void BuildFontGlyphsByIndex(ActualFont nativefont, Typography.Rendering.SimpleFontAtlasBuilder atlasBuilder, int startAtGlyphIndex, int endAtGlyphIndex)
        {
            //font glyph for specific font face

            for (int i = startAtGlyphIndex; i <= endAtGlyphIndex; ++i)
            {

                FontGlyph fontGlyph = nativefont.GetGlyphByIndex((uint)i);
                GlyphImage glyphImg = NativeMsdfGen.BuildMsdfFontImage(fontGlyph);

                int w = glyphImg.Width;
                int h = glyphImg.Height;
                int[] buffer = glyphImg.GetImageBuffer();
                NativeMsdfGen.SwapColorComponentFromBigEndianToWinGdi(buffer);
                glyphImg.SetImageBuffer(buffer, false);
                atlasBuilder.AddGlyph(i, glyphImg);
                //using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //{
                //    var bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                //    bmp.UnlockBits(bmpdata);
                //    bmp.Save("d:\\WImageTest\\a001_x1_" + (int)c + ".png"); 
                //}
            }
        }

        static void BuildFontGlyph(ActualFont nativefont, Typography.Rendering.SimpleFontAtlasBuilder atlasBuilder, char c)
        {
            //font glyph for specific font face



            FontGlyph fontGlyph = nativefont.GetGlyph(c);
            GlyphImage glyphImg = NativeMsdfGen.BuildMsdfFontImage(fontGlyph);

            int w = glyphImg.Width;
            int h = glyphImg.Height;
            int[] buffer = glyphImg.GetImageBuffer();
            NativeMsdfGen.SwapColorComponentFromBigEndianToWinGdi(buffer);
            glyphImg.SetImageBuffer(buffer, false);
            atlasBuilder.AddGlyph(0, glyphImg);
            //using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //{
            //    var bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
            //    bmp.UnlockBits(bmpdata);
            //    bmp.Save("d:\\WImageTest\\a001_x1_" + (int)c + ".png"); 
            //}

        }
        private void button5_Click(object sender, EventArgs e)
        {

            //1. load font
            string fontName = "tahoma";
            string fontfile = "c:\\Windows\\Fonts\\tahoma.ttf";
            //string fontfile = @"D:\WImageTest\THSarabunNew\THSarabunNew.ttf";
            ActualFont font = GetActualFont(fontfile, 18);// nativeFontStore.LoadFont(fontName, fontfile, 28);
            //2. get glyph 
            var atlasBuilder = new Typography.Rendering.SimpleFontAtlasBuilder();
            //for (int i = 0; i < 256; ++i)
            //BuildFontGlyphsByIndex(font, atlasBuilder, 0, 255);
            //BuildFontGlyphs(font, atlasBuilder, 0x0e00, 0x0e5b);
            //BuildFontGlyphsByIndex(font, atlasBuilder, 0, 3417);
            //BuildFontGlyphsByIndex(font, atlasBuilder, 0, 509);
            //BuildFontGlyphsByIndex(font, atlasBuilder, 97, 97);
            BuildFontGlyph(font, atlasBuilder, 'B');
            //----------------------------------------------------
            //GlyphImage totalImg = atlasBuilder.BuildSingleImage();
            GlyphImage totalImg = atlasBuilder.BuildSingleImage();
            using (Bitmap bmp = new Bitmap(totalImg.Width, totalImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {

                int[] buffer = totalImg.GetImageBuffer();
                if (totalImg.IsBigEndian)
                {
                    NativeMsdfGen.SwapColorComponentFromBigEndianToWinGdi(buffer);
                    totalImg.SetImageBuffer(buffer, false);
                }

                var bmpdata = bmp.LockBits(
                    new System.Drawing.Rectangle(0, 0, totalImg.Width, totalImg.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                bmp.UnlockBits(bmpdata);
                bmp.Save("d:\\WImageTest\\a_total.png");
            }

            string fontfilename = "d:\\WImageTest\\a_total.xml";
            atlasBuilder.SaveFontInfo(fontfilename);
            //---------------------------------- 
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //1. load font 
            ActualFont font = GetActualFont("tahoma", 28);// nativeFontStore.LoadFont("tahoma", 28);
            //2. get glyph

            var g1 = font.GetGlyph('C');

            var plans = new List<Typography.TextLayout.UnscaledGlyphPlan>();
            PixelFarm.Drawing.Text.TextShapingService.GetGlyphPos(font, "ABC".ToCharArray(), 0, 3, plans);


            int[] glyphIndice = new int[] { 1076, 1127, 1164 };
            int j = glyphIndice.Length;

            var atlasBuilder = new Typography.Rendering.SimpleFontAtlasBuilder();

            for (int i = 0; i < j; ++i)
            {

                int codepoint = glyphIndice[i];
                FontGlyph fontGlyph = font.GetGlyphByIndex((uint)codepoint);

                GlyphImage glyphImg = NativeMsdfGen.BuildMsdfFontImage(fontGlyph);
                int w = glyphImg.Width;
                int h = glyphImg.Height;
                int[] buffer = glyphImg.GetImageBuffer();
                NativeMsdfGen.SwapColorComponentFromBigEndianToWinGdi(buffer);
                glyphImg.SetImageBuffer(buffer, false);
                atlasBuilder.AddGlyph(codepoint, glyphImg);

                using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                    bmp.UnlockBits(bmpdata);
                    bmp.Save("d:\\WImageTest\\a001_y1_" + codepoint + ".png");
                }
            }
            //----------------------------------------------------
            GlyphImage totalImg = atlasBuilder.BuildSingleImage();
            using (Bitmap bmp = new Bitmap(totalImg.Width, totalImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {

                int[] buffer = totalImg.GetImageBuffer();
                if (totalImg.IsBigEndian)
                {
                    NativeMsdfGen.SwapColorComponentFromBigEndianToWinGdi(buffer);
                    totalImg.SetImageBuffer(buffer, false);
                }

                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, totalImg.Width, totalImg.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                bmp.UnlockBits(bmpdata);
                bmp.Save("d:\\WImageTest\\a_total.png");
            }

            string fontfilename = "d:\\WImageTest\\a_total.xml";
            atlasBuilder.SaveFontInfo(fontfilename);
            //---------------------------------- 
        }
        Font ff = new Font("tahoma", 10);
        private void button7_Click(object sender, EventArgs e)
        {
            //from msdn: CreateCompatibleBitmap function
            //The color format of the bitmap created by the CreateCompatibleBitmap function matches the color format of the device identified by the hdc parameter.
            //This bitmap can be selected into any memory device context that is compatible with the original device.

            //Because memory device contexts allow both color and monochrome bitmaps, 
            //the format of the bitmap returned by the CreateCompatibleBitmap function differs when the specified device context is a memory device context.
            //However, a compatible bitmap that was created for a nonmemory device context always possesses the same color format
            //and uses the same color palette as the specified device context.

            //Note: When a memory device context is created, it initially has a 1 - by - 1 monochrome bitmap selected into it.
            //If this memory device context is used in CreateCompatibleBitmap, the bitmap that is created is a monochrome bitmap.
            //To create a color bitmap, use the HDC that was used to create the memory device context, as shown in the following code:



            IntPtr winHwnd = panel2.Handle;
            IntPtr hdc = Win32.MyWin32.GetDC(winHwnd);
            IntPtr hbmp = Win32.MyWin32.CreateCompatibleBitmap(hdc, 400, 50);

            //Bitmap bmp = new Bitmap(panel2.Width, panel2.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //IntPtr hbmp = bmp.GetHbitmap();

            Win32.MyWin32.SelectObject(hdc, hbmp);
            IntPtr hfont = ff.ToHfont();
            Win32.MyWin32.SelectObject(hdc, hfont);
            Win32.MyWin32.SetTextColor(hdc, 0);
            Win32.NativeTextWin32.TextOut(hdc, 0, 0, "OKOK\0", 4);


            Win32.BITMAP win32Bitmap = new Win32.BITMAP();
            unsafe
            {
                Win32.MyWin32.GetObject(hbmp,
                    System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32.BITMAP)),
                      &win32Bitmap);
            }
            Win32.MyWin32.ReleaseDC(winHwnd, hdc);
            Win32.MyWin32.DeleteObject(hbmp);
            //-------------------------------------------- 

        }

        private void button8_Click(object sender, EventArgs e)
        {
            IntPtr winHwnd = panel2.Handle;
            IntPtr hdc = Win32.MyWin32.GetDC(winHwnd);
            IntPtr dib;
            IntPtr ppvBits;
            int bmpWidth = 200;
            IntPtr memHdc = Win32.MyWin32.CreateMemoryHdc(hdc, bmpWidth, 50, out dib, out ppvBits);
            Win32.MyWin32.PatBlt(memHdc, 0, 0, bmpWidth, 50, Win32.MyWin32.WHITENESS);

            IntPtr hfont = ff.ToHfont();
            Win32.MyWin32.SelectObject(memHdc, hfont);
            Win32.MyWin32.SetTextColor(memHdc, 0);
            Win32.NativeTextWin32.TextOut(memHdc, 0, 0, "OKOK\0", 4);

            Win32.MyWin32.BitBlt(hdc, 0, 0, bmpWidth, 50, memHdc, 0, 0, Win32.MyWin32.SRCCOPY);
            //---------------
            int stride = 4 * ((bmpWidth * 32 + 31) / 32);

            Bitmap newBmp = new Bitmap(bmpWidth, 50, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpData = newBmp.LockBits(new System.Drawing.Rectangle(0, 0, bmpWidth, 50), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] tmp1 = new byte[stride * 50];

            System.Runtime.InteropServices.Marshal.Copy(ppvBits, tmp1, 0, tmp1.Length);

            //---------------
            int pos = 3;
            for (int r = 0; r < 50; ++r)
            {
                for (int c = 0; c < stride; ++c)
                {
                    tmp1[pos] = 255;
                    pos += 4;
                    c += 4;

                }
            }
            //---------------
            System.Runtime.InteropServices.Marshal.Copy(tmp1, 0, bmpData.Scan0, tmp1.Length);
            //---------------
            newBmp.UnlockBits(bmpData);
            newBmp.Save("d:\\WImageTest\\testBmp1.png");
            //---------------

            //Win32.MyWin32.DeleteObject(hbmp);
            Win32.MyWin32.DeleteObject(dib);
            Win32.MyWin32.DeleteDC(memHdc);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //#if DEBUG
            //            dbugTestMyFtLib.Test2();
            //#endif

        }

        private void button10_Click(object sender, EventArgs e)
        {
            //--------------------------------------------
            Msdfgen.Shape shape = new Msdfgen.Shape();
            Msdfgen.Contour contour = new Msdfgen.Contour();
            //contour.AddLine(10, 10, 25, 25);
            //contour.AddLine(25, 25, 15, 10);
            //contour.AddLine(15, 10, 10, 10);

            contour.AddLine(10, 10, 25, 25);
            contour.AddQuadraticSegment(25, 25, 15, 30, 10, 15);
            contour.AddLine(10, 15, 10, 10);

            shape.contours.Add(contour);
            //-+---------------------------

            double left, bottom, right, top;
            shape.findBounds(out left, out bottom, out right, out top);

            Msdfgen.FloatRGBBmp frgbBmp = new Msdfgen.FloatRGBBmp((int)Math.Ceiling((right - left)), (int)Math.Ceiling((top - bottom)));
            double edgeThreshold = 1.00000001;//use default
            double angleThreshold = 1;
            shape.InverseYAxis = true;

            Msdfgen.EdgeColoring.edgeColoringSimple(shape, 3);
            Msdfgen.MsdfGenerator.generateMSDF(frgbBmp, shape, 4, new Msdfgen.Vector2(1, 1), new Msdfgen.Vector2(), -1);
            int[] buffer = Msdfgen.MsdfGenerator.ConvertToIntBmp(frgbBmp);

            //MsdfGen.SwapColorComponentFromBigEndianToWinGdi(buffer);

            using (Bitmap bmp = new Bitmap(frgbBmp.Width, frgbBmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, frgbBmp.Width, frgbBmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                bmp.UnlockBits(bmpdata);

                bmp.Save("d:\\WImageTest\\a001_xn2_.png");
            }
        }

        internal unsafe static GlyphImage BuildMsdfFontImage()
        {
            IntPtr shape = MyFtLib.CreateShape();
            IntPtr cnt = MyFtLib.ShapeAddBlankContour(shape);
            //MyFtLib.ContourAddLinearSegment(cnt, 10, 10, 25, 25);
            //MyFtLib.ContourAddLinearSegment(cnt, 25, 25, 15, 10);
            //MyFtLib.ContourAddLinearSegment(cnt, 15, 10, 10, 10);
            // 

            MyFtLib.ContourAddLinearSegment(cnt, 10, 10, 25, 25);
            MyFtLib.ContourAddQuadraticSegment(cnt, 25, 25, 15, 30, 10, 15);
            MyFtLib.ContourAddLinearSegment(cnt, 10, 15, 10, 10);

            double s_left, s_bottom, s_right, s_top;
            MyFtLib.ShapeFindBounds(shape, out s_left, out s_bottom, out s_right, out s_top);
            var glyphBounds = new PixelFarm.Drawing.RectangleF((float)s_left, (float)s_top, (float)(s_right - s_left), (float)(s_top - s_bottom));
            //then create msdf texture
            if (!MyFtLib.ShapeValidate(shape))
            {
                throw new NotSupportedException();
            }
            MyFtLib.ShapeNormalize(shape);
            int borderXY = 0;
            int w = (int)Math.Ceiling(glyphBounds.Width) + (borderXY + borderXY);
            int h = (int)(Math.Ceiling(glyphBounds.Height)) + (borderXY + borderXY);
            if (w == 0)
            {
                w = 5;
                h = 5;
            }
            int[] outputBuffer = new int[w * h];
            GlyphImage glyphImage = new GlyphImage(w, h);
            glyphImage.BorderXY = borderXY;
            glyphImage.OriginalGlyphBounds = Typography.Contours.RectangleF.FromLTRB(
                glyphBounds.Left,
                glyphBounds.Top,
                glyphBounds.Right,
                glyphBounds.Bottom);
            unsafe
            {
                fixed (int* output_header = &outputBuffer[0])
                {
                    float dx = 0;
                    float dy = 0;
                    if (s_left < 0)
                    {
                        //make it positive
                        dx = (float)-s_left;
                    }
                    else if (s_left > 0)
                    {

                    }
                    if (s_bottom < 0)
                    {
                        //make it positive
                        dy = (float)-s_bottom;
                    }
                    else if (s_bottom > 0)
                    {

                    }
                    //this glyph image has border (for msdf to render correctly)
                    MyFtLib.MyFtGenerateMsdf(shape, w, h, 4, 1, dx + borderXY, dy + borderXY, -1, 3, output_header);
                    MyFtLib.DeleteUnmanagedObj(shape);
                }
                glyphImage.SetImageBuffer(outputBuffer, true);
            }
            return glyphImage;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            unsafe
            {

                GlyphImage g1 = BuildMsdfFontImage();

                int[] buffer = g1.GetImageBuffer();
                using (Bitmap bmp = new Bitmap(g1.Width, g1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, g1.Width, g1.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                    bmp.UnlockBits(bmpdata);
                    bmp.Save("d:\\WImageTest\\a001_xn1_.png");
                }
            }

        }

        private void cmdTestTess_Click(object sender, EventArgs e)
        {

        }
    }
}
