//MIT,2016, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
            //                                                    //1st pass horizontal scanline

            int i = 0;
            int p = 0;
            for (int row = 0; row < bmpHeight; ++row)
            {
                int prevLevel = 0;
                int currentStripLen = 0;
                //row

                int cut = 0;
                for (int c = 0; c < bmpWidth; ++c)
                {
                    int pixel = intBuffer[i];
                    int a = (pixel >> 24) & 0xff;
                    int b = (pixel >> 16) & 0xff;
                    int g = (pixel >> 8) & 0xff;
                    int r = (pixel >> 8) & 0xff;
                    //convert to grey scale
                    int level = (int)((0.2126 * r) + (0.7152 * g) + (0.0722) * b);
                    //int luminosity method;
                    // R' = G' = B' = 0.2126R + 0.7152G + 0.0722B

                    if (level > 0)
                    {
                        level = 255;
                    }

                    if (level != prevLevel)
                    {
                        if (currentStripLen > 0)
                        {
                            //fill data
                            FillData(distanceBuffer, p, currentStripLen, (cut % 2) != 0);
                            cut++;
                        }
                        else
                        {

                        }
                        currentStripLen = 1;
                        p = i;
                        prevLevel = level;
                    }
                    else
                    {
                        //same level
                        currentStripLen++;
                    }
                    ++i;
                }
                //---------------------------
                //exit
                //fill remaining databack
                if (currentStripLen > 0)
                {
                    FillData(distanceBuffer, p, currentStripLen, (cut % 2) != 0);
                    p = i;
                }
            }
            //--------
            //test output
            var outputBmp = new Bitmap(bmpWidth, bmpHeight);
            var outputBmpData = outputBmp.LockBits(new Rectangle(0, 0, (int)size.Width, (int)size.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp1.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(distanceBuffer, 0, outputBmpData.Scan0, distanceBuffer.Length);
            outputBmp.UnlockBits(outputBmpData);
            outputBmp.Save("d:\\WImageTest\\a001_x.png");
            //--------
        }
        const int SCALE = 20;
        static void FillData(int[] outputPixels, int startIndex, int count, bool inside)
        {

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
                    FillStrip(outputPixels, startIndex, eachSide, 5, true, true);
                    //middle
                    //-----------------
                    startIndex += eachSide;
                    if (eachSide > 5)
                    {
                        outputPixels[startIndex] = (255 << 24) | ((6 * SCALE) << INSIDE_SHIFT);
                    }
                    else
                    {
                        outputPixels[startIndex] = (255 << 24) | (((eachSide + 1) * SCALE) << INSIDE_SHIFT);
                    }
                    startIndex += 1;
                    //-----------------
                    //right side 
                    FillStrip(outputPixels, startIndex, eachSide, 5, true, false);
                }
                else
                {
                    //even number
                    int eachSide = count / 2;
                    //start(left side)
                    FillStrip(outputPixels, startIndex, eachSide, 5, true, true);
                    startIndex += eachSide;
                    //right side
                    FillStrip(outputPixels, startIndex, eachSide, 5, true, false);
                }
            }
            else
            {
                //outside polygon
                int n = count;
                int p = startIndex;

                if ((count % 2) != 0)
                {
                    //odd number
                    int eachSide = ((count - 1) / 2);
                    //start(left side)
                    FillStrip(outputPixels, startIndex, eachSide, 5, false, true);
                    //-----------------
                    //middle
                    startIndex += eachSide;
                    if (eachSide > 5)
                    {
                        outputPixels[startIndex] = (255 << 24) | ((6 * SCALE) << OUTSIDE_SHIFT);
                    }
                    else
                    {
                        outputPixels[startIndex] = (255 << 24) | (((eachSide + 1) * SCALE) << OUTSIDE_SHIFT);
                    }
                    startIndex += 1;
                    //-----------------
                    //right side                    
                    FillStrip(outputPixels, startIndex, eachSide, 5, false, false);
                }
                else
                {
                    //even number
                    int eachSide = count / 2;
                    //start(left side)
                    FillStrip(outputPixels, startIndex, eachSide, 5, false, true);
                    startIndex += eachSide;
                    //right side
                    FillStrip(outputPixels, startIndex, eachSide, 5, false, false);
                }
            }
        }
        const int INSIDE_SHIFT = 16;
        const int OUTSIDE_SHIFT = 8;
        static void FillStrip(int[] outputPixels, int startIndex, int count, int maxLevel, bool inside, bool uphill)
        {
            int compoShift = inside ? INSIDE_SHIFT : OUTSIDE_SHIFT;

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
                        outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        p++;
                        c++;
                    }
                    //long distance                    
                    for (; i < count; ++i)
                    {
                        outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift); //red
                        p++;
                    }
                }
                else
                {  //gradient up
                    int i = 0;
                    int value = 0;
                    int c = 1;
                    for (; i < count; ++i)
                    {
                        outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        p++;
                        c++;
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
                        outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        p++;
                    }

                    //gradient down
                    c = maxLevel;
                    for (; i < count; ++i)
                    {
                        outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        p++;
                        c--;
                    }
                }
                else
                {
                    //gradient down
                    int i = 0;
                    int c = count;
                    for (; i < count; ++i)
                    {
                        outputPixels[p] = value = (255 << 24) | ((c * SCALE) << compoShift);
                        p++;
                        c--;
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

            char[] fontChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
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
    }
}
