using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ImageTransformation;

namespace Mini
{
    public partial class FormRasterImage : Form
    {

        public FormRasterImage()
        {
            InitializeComponent();

            this.Load += new EventHandler(FormRasterImage_Load);
        }

        void FormRasterImage_Load(object sender, EventArgs e)
        {

        }

        private void cmdLinearEq_Click(object sender, EventArgs e)
        {
            //from wikipedia: Linear_algebra
            //Gaussian elimination 
            //3 points
            //eg. 2x+y-z  =   8 // L1
            //   -3x-y+2z = -11 // L2
            //   -2x+y+2z =  -3 // L3

            //todo : find x,y,z

            //solve steps:
            //step 1: eliminate x from L2,L3
            //step 2: eliminate y from L3 -> found z
            //resubstitue back


            //test1:
            double x, y, z;
            LinearEq3.Resolve(
                new LinearEq3(2, 1, -1, 8),
                new LinearEq3(-3, -1, 2, -11),
                new LinearEq3(-2, 1, 2, -3),
                out x,   //x=2
                out y,  //y=3
                out z); //z=-1


            //test2
            double c1, c2, c3, c4;
            LinearEq4.Resolve(
                new LinearEq4(2, 1, -1, 0, 8),
                new LinearEq4(-3, -1, 2, 0, -11),
                new LinearEq4(-2, 1, 2, 0, -3),
                new LinearEq4(-2, 1, 2, 0, -3),
                out c1,   //x=2
                out c2,  //y=3
                out c3, //-1
                out c4); //0 


        }
        private void button1_Click(object sender, EventArgs e)
        {



            //create control point 
            // Create array
            //eg sample pixel
            double[][] arr = { new double[]{ 1, 3, 3, 4 },
                              new double[] { 7, 2, 3, 4 }, 
                              new double[] { 1, 6, 3, 6 },
                              new double[] { 2, 5, 7, 2 } };
            BicubicInterpolator interpolator = new BicubicInterpolator();
            var value = interpolator.getValue(arr, 1, 0);
            var value2 = interpolator.getValue(arr, 1, 0);
            var value3 = interpolator.getValue(arr, 1, 0);
            var value4 = interpolator.getValue(arr, 1, 0);
            var value5 = interpolator.getValue(arr, 1, 0);
        }

        static int RoundPixel(double d)
        {

            return d > 0 ? (int)(d + 0.5) : (int)(d - 0.5);

        }
        static int RoundPixel(float f)
        {
            //double floor = Math.Floor(d);
            //double ceiling = Math.Ceiling(d);
            //double diff = floor - ceiling;
            //return (ceiling - floor) > 0.5 ? (int)ceiling : (int)floor; 
            return f > 0 ? (int)(f + 0.5f) : (int)(f - 0.5f);

        }
        static BicubicInterpolator2 myInterpolator = new BicubicInterpolator2();

        static MyColor GetApproximateColor(BufferReader4 reader, double cx, double cy)
        {
            return reader.ReadOnePixel();
        }
        static MyColor GetApproximateColor3(BufferReader4 reader, double cx, double cy)
        {
            //nearest neighbor
            if (reader.CurrentX > 2 && reader.CurrentY > 2 &&
             reader.CurrentX < reader.Width - 2 && reader.CurrentY < reader.Height - 2)
            {
                //read 4 point sample
                MyColor[] colors = new MyColor[16];

                reader.Read16(colors);
                int r_sum = 0, g_sum = 0, b_sum = 0, a_sum = 0;

                for (int i = 0; i < 16; ++i)
                {
                    //4*4
                    MyColor c = colors[i];
                    r_sum += c.r;
                    g_sum += c.g;
                    b_sum += c.b;
                    a_sum += c.a;

                }


                //poor man average all values 
                return new MyColor(
                    (byte)RoundPixel((float)r_sum / 16),
                    (byte)RoundPixel((float)g_sum / 16),
                    (byte)RoundPixel((float)b_sum / 16),
                    (byte)RoundPixel((float)a_sum / 16));

            }
            else
            {
                return reader.ReadOnePixel();
            }
        }
        static MyColor GetApproximateColor4(BufferReader4 reader, double cx, double cy)
        {
            //nearest neighbor
            if (reader.CurrentX > 2 && reader.CurrentY > 2 &&
             reader.CurrentX < reader.Width - 2 && reader.CurrentY < reader.Height - 2)
            {
                //read 4 point sample
                MyColor[] colors = new MyColor[16];

                reader.Read16(colors);
                int r_sum = 0, g_sum = 0, b_sum = 0, a_sum = 0;

                int totalWeight = 12 + (4 * 5);
                //core 0---------------------
                {
                    int w = 5;
                    MyColor c = colors[5];
                    r_sum += c.r * w;
                    g_sum += c.g * w;
                    b_sum += c.b * w;
                    a_sum += c.a * w;
                    c = colors[6];
                    r_sum += c.r * w;
                    g_sum += c.g * w;
                    b_sum += c.b * w;
                    a_sum += c.a * w;
                    c = colors[9];
                    r_sum += c.r * w;
                    g_sum += c.g * w;
                    b_sum += c.b * w;
                    a_sum += c.a * w;
                    c = colors[10];
                    r_sum += c.r * w;
                    g_sum += c.g * w;
                    b_sum += c.b * w;
                    a_sum += c.a * w;
                }
                //------------------------------
                //core 2
                int[] others = new int[] { 0, 1, 2, 3, 4, 7, 8, 11, 12, 13, 14, 15, };
                int len = others.Length;
                for (int i = 0; i < len; ++i)
                {
                    MyColor c = colors[others[i]];
                    r_sum += c.r;
                    g_sum += c.g;
                    b_sum += c.b;
                    a_sum += c.a;
                }


                //------------------------------

                //poor man average all values 
                return new MyColor(
                    (byte)RoundPixel((float)r_sum / totalWeight),
                    (byte)RoundPixel((float)g_sum / totalWeight),
                    (byte)RoundPixel((float)b_sum / totalWeight),
                    (byte)RoundPixel((float)a_sum / totalWeight));

            }
            else
            {
                return reader.ReadOnePixel();
            }
        }

        static MyColor GetApproximateColor_Bicubic(BufferReader4 reader, double cx, double cy)
        {
            byte[] rBuffer = new byte[16];
            byte[] gBuffer = new byte[16];
            byte[] bBuffer = new byte[16];
            byte[] aBuffer = new byte[16];

            //nearest neighbor
            if (reader.CurrentX > 2 && reader.CurrentY > 2 &&
                reader.CurrentX < reader.Width - 2 && reader.CurrentY < reader.Height - 2)
            {
                //read 4 point sample
                MyColor[] colors = new MyColor[16];
                reader.SetStartPixel((int)cx, (int)cy);
                reader.Read16(colors);

                double x0 = (int)cx;
                double x1 = (int)(cx + 1);
                double xdiff = cx - x0;

                double y0 = (int)cy;
                double y1 = (int)(cy + 1);
                double ydiff = cy - y0;


                SeparateByChannel(colors, rBuffer, gBuffer, bBuffer, aBuffer);

                double result_B = myInterpolator.getValueBytes(bBuffer, xdiff, ydiff);
                double result_G = myInterpolator.getValueBytes(gBuffer, xdiff, ydiff);
                double result_R = myInterpolator.getValueBytes(rBuffer, xdiff, ydiff);
                double result_A = myInterpolator.getValueBytes(aBuffer, xdiff, ydiff);

                //clamp
                if (result_B > 255)
                {
                    result_B = 255;
                }
                else if (result_B < 0)
                {
                    result_B = 0;
                }
                if (result_G > 255)
                {
                    result_G = 255;
                }
                else if (result_G < 0)
                {
                    result_G = 0;
                }
                if (result_R > 255)
                {
                    result_R = 255;
                }
                else if (result_R < 0)
                {
                    result_R = 0;
                }
                if (result_A > 255)
                {
                    result_A = 255;
                }
                else if (result_A < 0)
                {
                    result_A = 0;
                }

                return new MyColor((byte)result_R, (byte)result_G, (byte)result_B, (byte)result_A);


            }
            else
            {
                return reader.ReadOnePixel();
            }
        }
        static MyColor GetApproximateColor8(BufferReader4 reader, double cx, double cy)
        {
            //incorrect invert interpolate x,y
            byte[] rBuffer = new byte[16];
            byte[] gBuffer = new byte[16];
            byte[] bBuffer = new byte[16];
            byte[] aBuffer = new byte[16];

            //nearest neighbor
            if (reader.CurrentX > 2 && reader.CurrentY > 2 &&
                reader.CurrentX < reader.Width - 2 && reader.CurrentY < reader.Height - 2)
            {
                //read 4 point sample
                MyColor[] colors = new MyColor[16];
                reader.SetStartPixel((int)cx, (int)cy);
                reader.Read16(colors);

                double x0 = (int)cx;
                double x1 = (int)(cx + 1);
                double xdiff = cx - x0;

                double y0 = (int)cy;
                double y1 = (int)(cy + 1);
                double ydiff = cy - y0;


                SeparateByChannel(colors, rBuffer, gBuffer, bBuffer, aBuffer);

                double result_B = myInterpolator.getValueBytes(bBuffer, xdiff, ydiff);
                double result_G = myInterpolator.getValueBytes(gBuffer, xdiff, ydiff);
                double result_R = myInterpolator.getValueBytes(rBuffer, xdiff, ydiff);
                double result_A = myInterpolator.getValueBytes(aBuffer, xdiff, ydiff);

                if (result_B > 255)
                {
                    result_B = 255;
                }
                else if (result_B < 0)
                {
                    result_B = 0;
                }
                if (result_G > 255)
                {
                    result_G = 255;
                }
                else if (result_G < 0)
                {
                    result_G = 0;
                }
                if (result_R > 255)
                {
                    result_R = 255;
                }
                else if (result_R < 0)
                {
                    result_R = 0;
                }
                if (result_A > 255)
                {
                    result_A = 255;
                }
                else if (result_A < 0)
                {
                    result_A = 0;
                }

                return new MyColor((byte)result_R, (byte)result_G, (byte)result_B, (byte)result_A);


            }
            else
            {
                return reader.ReadOnePixel();
            }
        }
        static MyColor GetApproximateColor5(BufferReader4 reader, double cx, double cy)
        {
            //nearest neighbor
            if (reader.CurrentX > 2 && reader.CurrentY > 2 &&
             reader.CurrentX < reader.Width - 2 && reader.CurrentY < reader.Height - 2)
            {


                MyColor[] colors = new MyColor[16];
                reader.Read16(colors);
                int r_sum = 0, g_sum = 0, b_sum = 0, a_sum = 0;
                int totalWeight = 4;

                //4 points average***
                //core 0---------------------
                {
                    int w = 1;
                    MyColor c = colors[5];
                    r_sum += c.r * w;
                    g_sum += c.g * w;
                    b_sum += c.b * w;
                    a_sum += c.a * w;
                    c = colors[6];
                    r_sum += c.r * w;
                    g_sum += c.g * w;
                    b_sum += c.b * w;
                    a_sum += c.a * w;
                    c = colors[9];
                    r_sum += c.r * w;
                    g_sum += c.g * w;
                    b_sum += c.b * w;
                    a_sum += c.a * w;
                    c = colors[10];
                    r_sum += c.r * w;
                    g_sum += c.g * w;
                    b_sum += c.b * w;
                    a_sum += c.a * w;
                }


                //------------------------------

                //poor man average all values 
                return new MyColor(
                    (byte)RoundPixel((float)r_sum / totalWeight),
                    (byte)RoundPixel((float)g_sum / totalWeight),
                    (byte)RoundPixel((float)b_sum / totalWeight),
                    (byte)RoundPixel((float)a_sum / totalWeight));

            }
            else
            {
                return reader.ReadOnePixel();
            }
        }

        /// <summary>
        /// bilinear interpolation
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <returns></returns>
        static MyColor GetApproximateColor_Bilinear(BufferReader4 reader, float cx, float cy)
        {
            //four point bilinear
            if (reader.CurrentX > 2 && reader.CurrentY > 2 &&
                reader.CurrentX < reader.Width - 2 && reader.CurrentY < reader.Height - 2)
            {

                //double r_sum = 0, g_sum = 0, b_sum = 0, a_sum = 0;
                //double totalWeight = 0; 
                //---------------------------------
                float cx1 = (int)(cx);
                float cx2 = (int)(cx + 1);

                float cy1 = (int)(cy);
                float cy2 = (int)(cy + 1);

                MyColor[] colors = new MyColor[4];
                reader.SetStartPixel((int)cx1, (int)cy1);
                reader.Read4(colors);


                MyColor qA = colors[0];
                MyColor qB = colors[1];
                MyColor qC = colors[2];
                MyColor qD = colors[3];

                double r_, g_, b_, a_;

                double cx2_cx = cx2 - cx;
                double cx_cx1 = cx - cx1;
                double cy2_cy = cy2 - cy;
                double cy_cy1 = cy - cy1;


                r_ = (qA.r * (cx2_cx) * (cy2_cy)) +
                     (qB.r * (cx_cx1) * (cy2_cy)) +
                     (qC.r * (cx2_cx) * (cy_cy1)) +
                     (qD.r * (cx_cx1) * (cy_cy1));

                b_ = (qA.b * (cx2_cx) * (cy2_cy)) +
                     (qB.b * (cx_cx1) * (cy2_cy)) +
                     (qC.b * (cx2_cx) * (cy_cy1)) +
                     (qD.b * (cx_cx1) * (cy_cy1));

                g_ = (qA.g * (cx2_cx) * (cy2_cy)) +
                     (qB.g * (cx_cx1) * (cy2_cy)) +
                     (qC.g * (cx2_cx) * (cy_cy1)) +
                     (qD.g * (cx_cx1) * (cy_cy1));

                a_ = (qA.a * (cx2_cx) * (cy2_cy)) +
                     (qB.a * (cx_cx1) * (cy2_cy)) +
                     (qC.a * (cx2_cx) * (cy_cy1)) +
                     (qD.a * (cx_cx1) * (cy_cy1));

                return new MyColor(
                    (byte)(r_),
                    (byte)(g_),
                    (byte)(b_),
                    (byte)(a_));
            }
            else
            {
                return reader.ReadOnePixel();
            }
        }

        static void SeparateByChannel(MyColor[] myColors, byte[] rBuffer, byte[] gBuffer, byte[] bBuffer, byte[] aBuffer)
        {
            for (int i = 0; i < 16; ++i)
            {
                MyColor m = myColors[i];
                rBuffer[i] = m.r;
                gBuffer[i] = m.g;
                bBuffer[i] = m.b;
                aBuffer[i] = m.a;
            }
        }

        private void cmdBilinearInterpolation_Click(object sender, EventArgs e)
        {

            Bitmap bmp = new Bitmap("d:\\WImageTest\\n_lion.png");
            int bmpHeight = bmp.Height;
            int bmpWidth = bmp.Width;
            var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmpWidth, bmpHeight), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            int stride = bmpdata.Stride;
            byte[] buffer = new byte[stride * bmpHeight];

            System.Runtime.InteropServices.Marshal.Copy(
                bmpdata.Scan0, buffer,
                0, buffer.Length);
            bmp.UnlockBits(bmpdata);
            bmpdata = null;
            bmp = null;
            //-----------------------------------------------


            int heightLim = bmpHeight - 3;
            int widthLim = bmpWidth - 3;


            BufferReader4 reader = new BufferReader4(buffer, stride, bmpWidth, bmpHeight);
            MyColor[] pixelBuffer = new MyColor[16];


            //crate small half image and interpolate
            double x_factor = 3;
            double y_factor = 3;

            //double x_factor = 0.35;
            // double y_factor = 0.35;
            double newWidth = (double)bmpWidth * x_factor;
            double newHeight = (double)bmpHeight * y_factor;
            int nWidth = (int)(bmpWidth * x_factor);
            int nHeight = (int)(bmpHeight * y_factor);

            Bitmap outputbmp = new Bitmap(nWidth, nHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //-----------------------------------------------
            var bmpdata2 = outputbmp.LockBits(new Rectangle(0, 0, nWidth, nHeight),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, outputbmp.PixelFormat);
            //-----------------------------------------
            int stride2 = bmpdata2.Stride;
            byte[] outputBuffer = new byte[stride2 * outputbmp.Height];

            int targetPixelIndex = 0;
            int startLine = 0;

            for (int ey = 0; ey < nHeight; ++ey)
            {
                for (int bx = 0; bx < nWidth; ++bx)
                {

                    float ideal_originalX = (float)(bx / x_factor);
                    float ideal_originalY = (float)(ey / y_factor);
                    //---
                    //round
                    int originalPosX = RoundPixel(ideal_originalX); ;//(int)(bx / x_factor);
                    int originalPosY = RoundPixel(ideal_originalY);//(int)(ey / y_factor);
                    //---
                    if (originalPosX >= bmpWidth)
                    {
                        originalPosX = bmpWidth - 1;
                    }
                    if (originalPosY >= bmpHeight)
                    {
                        originalPosY = bmpHeight - 1;
                    }
                    //---
                    reader.SetStartPixel(originalPosX, originalPosY);
                    //find src pixel and approximate  
                    MyColor color = GetApproximateColor_Bilinear(reader,
                       ideal_originalX,
                       ideal_originalY);

                    outputBuffer[targetPixelIndex] = (byte)color.b;
                    outputBuffer[targetPixelIndex + 1] = (byte)color.g;
                    outputBuffer[targetPixelIndex + 2] = (byte)color.r;
                    outputBuffer[targetPixelIndex + 3] = (byte)color.a;
                    targetPixelIndex += 4;
                }
                //newline
                startLine += stride2;
                targetPixelIndex = startLine;
            }


            System.Runtime.InteropServices.Marshal.Copy(
               outputBuffer, 0,
               bmpdata2.Scan0, outputBuffer.Length);
            outputbmp.UnlockBits(bmpdata2);
            outputbmp.Save("d:\\WImageTest\\n_lion_bilinear.png");
        }

        private void cmdBicubicInterpolation_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap("d:\\WImageTest\\n_lion.png");
            int bmpHeight = bmp.Height;
            int bmpWidth = bmp.Width;
            var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmpWidth, bmpHeight), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            int stride = bmpdata.Stride;
            byte[] buffer = new byte[stride * bmpHeight];

            System.Runtime.InteropServices.Marshal.Copy(
                bmpdata.Scan0, buffer,
                0, buffer.Length);
            bmp.UnlockBits(bmpdata);
            bmpdata = null;
            bmp = null;
            //-----------------------------------------------


            int heightLim = bmpHeight - 3;
            int widthLim = bmpWidth - 3;


            BufferReader4 reader = new BufferReader4(buffer, stride, bmpWidth, bmpHeight);
            MyColor[] pixelBuffer = new MyColor[16];


            //crate small half image and interpolate
            double x_factor = 3;
            double y_factor = 3;

            //double x_factor = 0.35;
            // double y_factor = 0.35;
            double newWidth = (double)bmpWidth * x_factor;
            double newHeight = (double)bmpHeight * y_factor;
            int nWidth = (int)(bmpWidth * x_factor);
            int nHeight = (int)(bmpHeight * y_factor);

            Bitmap outputbmp = new Bitmap(nWidth, nHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //-----------------------------------------------
            var bmpdata2 = outputbmp.LockBits(new Rectangle(0, 0, nWidth, nHeight),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, outputbmp.PixelFormat);
            //-----------------------------------------
            int stride2 = bmpdata2.Stride;
            byte[] outputBuffer = new byte[stride2 * outputbmp.Height];

            int targetPixelIndex = 0;
            int startLine = 0;

            for (int ey = 0; ey < nHeight; ++ey)
            {
                for (int bx = 0; bx < nWidth; ++bx)
                {

                    float ideal_originalX = (float)(bx / x_factor);
                    float ideal_originalY = (float)(ey / y_factor);
                    //---
                    //round
                    int originalPosX = RoundPixel(ideal_originalX); ;//(int)(bx / x_factor);
                    int originalPosY = RoundPixel(ideal_originalY);//(int)(ey / y_factor);
                    //---
                    if (originalPosX >= bmpWidth)
                    {
                        originalPosX = bmpWidth - 1;
                    }
                    if (originalPosY >= bmpHeight)
                    {
                        originalPosY = bmpHeight - 1;
                    }
                    //---
                    reader.SetStartPixel(originalPosX, originalPosY);
                    //find src pixel and approximate  
                    MyColor color = GetApproximateColor_Bicubic(reader,
                       ideal_originalX,
                       ideal_originalY);

                    outputBuffer[targetPixelIndex] = (byte)color.b;
                    outputBuffer[targetPixelIndex + 1] = (byte)color.g;
                    outputBuffer[targetPixelIndex + 2] = (byte)color.r;
                    outputBuffer[targetPixelIndex + 3] = (byte)color.a;
                    targetPixelIndex += 4;
                }
                //newline
                startLine += stride2;
                targetPixelIndex = startLine;
            }


            System.Runtime.InteropServices.Marshal.Copy(
               outputBuffer, 0,
               bmpdata2.Scan0, outputBuffer.Length);
            outputbmp.UnlockBits(bmpdata2);
            outputbmp.Save("d:\\WImageTest\\n_lion_bicubic.png");
        }

        private void cmdRotate30Bilinear_Click(object sender, EventArgs e)
        {
            //reverse map *** 
            Bitmap bmp = new Bitmap("d:\\WImageTest\\n_lion.png");
            int bmpHeight = bmp.Height;
            int bmpWidth = bmp.Width;
            var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmpWidth, bmpHeight), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            int stride = bmpdata.Stride;
            byte[] buffer = new byte[stride * bmpHeight];

            System.Runtime.InteropServices.Marshal.Copy(
                bmpdata.Scan0, buffer,
                0, buffer.Length);
            bmp.UnlockBits(bmpdata);
            bmpdata = null;
            bmp = null;
            //-----------------------------------------------


            int heightLim = bmpHeight - 3;
            int widthLim = bmpWidth - 3;


            BufferReader4 reader = new BufferReader4(buffer, stride, bmpWidth, bmpHeight);
            MyColor[] pixelBuffer = new MyColor[16];


            //crate small half image and interpolate
            double x_factor = 1;
            double y_factor = 1;

            //double x_factor = 0.35;
            //double y_factor = 0.35;
            //double newWidth = (double)bmpWidth * x_factor;
            //double newHeight = (double)bmpHeight * y_factor;
            int nWidth = (int)(bmpWidth * x_factor);
            int nHeight = (int)(bmpHeight * y_factor);

            Bitmap outputbmp = new Bitmap(nWidth, nHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //-----------------------------------------------
            var bmpdata2 = outputbmp.LockBits(new Rectangle(0, 0, nWidth, nHeight),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, outputbmp.PixelFormat);
            //-----------------------------------------
            int stride2 = bmpdata2.Stride;
            byte[] outputBuffer = new byte[stride2 * outputbmp.Height];

            int targetPixelIndex = 0;
            int startLine = 0;


            //------------------------------------------------------------------
            double degreeAngle = -30;
            double radianAngle = degreeAngle * Math.PI / 180;

            //find approppriate pixel 
            for (int ey = 0; ey < nHeight; ++ey)
            {
                for (int bx = 0; bx < nWidth; ++bx)
                {
                    //float u = x*cos(-Q) - y*sin(-Q);
                    //float v = x*sin(-Q) + y*cos(-Q);
                    double ideal_originalX = bx * Math.Cos(-radianAngle) - (ey * Math.Sin(-radianAngle)); //(float)(bx / x_factor);
                    double ideal_originalY = bx * Math.Sin(-radianAngle) + (ey * Math.Cos(-radianAngle));//(float)(ey / y_factor);
                    //---
                    //round
                    int originalPosX = RoundPixel(ideal_originalX); ;//(int)(bx / x_factor);
                    int originalPosY = RoundPixel(ideal_originalY);//(int)(ey / y_factor);
                    //---
                    if (originalPosX >= bmpWidth)
                    {
                        originalPosX = bmpWidth - 1;
                    }
                    if (originalPosY >= bmpHeight)
                    {
                        originalPosY = bmpHeight - 1;
                    }
                    //---
                    if (originalPosX < 0 || originalPosY < 0)
                    {
                        //skip
                        targetPixelIndex += 4;
                    }
                    else
                    {
                        reader.SetStartPixel(originalPosX, originalPosY);
                        //find src pixel and approximate  
                        MyColor color = GetApproximateColor_Bilinear(reader,
                           (float)ideal_originalX,
                           (float)ideal_originalY);

                        outputBuffer[targetPixelIndex] = (byte)color.b;
                        outputBuffer[targetPixelIndex + 1] = (byte)color.g;
                        outputBuffer[targetPixelIndex + 2] = (byte)color.r;
                        outputBuffer[targetPixelIndex + 3] = (byte)color.a;
                        targetPixelIndex += 4;
                    }
                }
                //newline
                startLine += stride2;
                targetPixelIndex = startLine;
            }


            System.Runtime.InteropServices.Marshal.Copy(
               outputBuffer, 0,
               bmpdata2.Scan0, outputBuffer.Length);
            outputbmp.UnlockBits(bmpdata2);
            outputbmp.Save("d:\\WImageTest\\n_lion_rotate30_bilinear.png");
        }

        private void cmdRotate30Bicubic_Click(object sender, EventArgs e)
        {
            //reverse map *** 
            Bitmap bmp = new Bitmap("d:\\WImageTest\\n_lion.png");
            int bmpHeight = bmp.Height;
            int bmpWidth = bmp.Width;
            var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmpWidth, bmpHeight), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            int stride = bmpdata.Stride;
            byte[] buffer = new byte[stride * bmpHeight];

            System.Runtime.InteropServices.Marshal.Copy(
                bmpdata.Scan0, buffer,
                0, buffer.Length);
            bmp.UnlockBits(bmpdata);
            bmpdata = null;
            bmp = null;
            //-----------------------------------------------


            int heightLim = bmpHeight - 3;
            int widthLim = bmpWidth - 3;


            BufferReader4 reader = new BufferReader4(buffer, stride, bmpWidth, bmpHeight);
            MyColor[] pixelBuffer = new MyColor[16];


            //crate small half image and interpolate
            double x_factor = 1;
            double y_factor = 1;

            //double x_factor = 0.35;
            //double y_factor = 0.35;
            //double newWidth = (double)bmpWidth * x_factor;
            //double newHeight = (double)bmpHeight * y_factor;
            int nWidth = (int)(bmpWidth * x_factor);
            int nHeight = (int)(bmpHeight * y_factor);

            Bitmap outputbmp = new Bitmap(nWidth, nHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //-----------------------------------------------
            var bmpdata2 = outputbmp.LockBits(new Rectangle(0, 0, nWidth, nHeight),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, outputbmp.PixelFormat);
            //-----------------------------------------
            int stride2 = bmpdata2.Stride;
            byte[] outputBuffer = new byte[stride2 * outputbmp.Height];

            int targetPixelIndex = 0;
            int startLine = 0;


            //------------------------------------------------------------------
            double degreeAngle = -30;
            double radianAngle = degreeAngle * Math.PI / 180;

            //find approppriate pixel 
            for (int ey = 0; ey < nHeight; ++ey)
            {
                for (int bx = 0; bx < nWidth; ++bx)
                {
                    //float u = x*cos(-Q) - y*sin(-Q);
                    //float v = x*sin(-Q) + y*cos(-Q);
                    double ideal_originalX = bx * Math.Cos(-radianAngle) - (ey * Math.Sin(-radianAngle)); //(float)(bx / x_factor);
                    double ideal_originalY = bx * Math.Sin(-radianAngle) + (ey * Math.Cos(-radianAngle));//(float)(ey / y_factor);
                    //---
                    //round
                    int originalPosX = RoundPixel(ideal_originalX); ;//(int)(bx / x_factor);
                    int originalPosY = RoundPixel(ideal_originalY);//(int)(ey / y_factor);
                    //---
                    if (originalPosX >= bmpWidth)
                    {
                        originalPosX = bmpWidth - 1;
                    }
                    if (originalPosY >= bmpHeight)
                    {
                        originalPosY = bmpHeight - 1;
                    }
                    //---
                    if (originalPosX < 0 || originalPosY < 0)
                    {
                        //skip
                        targetPixelIndex += 4;
                    }
                    else
                    {
                        reader.SetStartPixel(originalPosX, originalPosY);
                        //find src pixel and approximate  
                        MyColor color = GetApproximateColor_Bicubic(reader,
                           (float)ideal_originalX,
                           (float)ideal_originalY);

                        outputBuffer[targetPixelIndex] = (byte)color.b;
                        outputBuffer[targetPixelIndex + 1] = (byte)color.g;
                        outputBuffer[targetPixelIndex + 2] = (byte)color.r;
                        outputBuffer[targetPixelIndex + 3] = (byte)color.a;
                        targetPixelIndex += 4;
                    }
                }
                //newline
                startLine += stride2;
                targetPixelIndex = startLine;
            }


            System.Runtime.InteropServices.Marshal.Copy(
               outputBuffer, 0,
               bmpdata2.Scan0, outputBuffer.Length);
            outputbmp.UnlockBits(bmpdata2);
            outputbmp.Save("d:\\WImageTest\\n_lion_rotate30_bicubic.png");
        }

        private void button2_Click(object sender, EventArgs e)
        {

            var txBilinear1 = MatterHackers.Agg.Transform.Bilinear.RectToQuad(
                 0, 0, 5, 5,
                        new double[]{ 5,5, 
                              10,10,
                              5,15,
                              0,10});

            double x = 0;
            double y = 5;
            txBilinear1.Transform(ref x, ref y);


            var txBilinear2 = MatterHackers.Agg.Transform.Bilinear.QuadToRect(
                     new double[]{ 5,5, 
                              10,10,
                              5,15,
                              0,10}, 0, 0, 5, 5);

            x = 5;
            y = 5;
            txBilinear1.Transform(ref x, ref y);
        }



        //buffer = new byte[]{
        //    1,2,3,4      , 5,6,7,8,      9,10,11,12,    13,14,15,16, /*|*/ 101,102,103,104,  105,106,107,108,  109,110,111,112,  113,114,115,116,
        //    17,18,19,20  , 21,22,23,24,  25,26,27,28,   29,30,31,32, /*|*/ 117,118,119,120,  121,122,123,124,  125,126,127,128,  129,130,131,132,
        //    33,34,35,36,   37,38,39,40,  41,42,43,44,   45,46,47,48,/*|*/  133,134,135,136,  137,138,139,140,  141,142,143,144,  145,146,147,148,
        //    49,50,51,52 ,  53,54,55,56,  57,58,59,60,   61,62,63,64,/*|*/  149,150,151,152,  153,154,155,156,  157,158,159,160,  161,162,163,164,
        //    //-----------------------------------------------------------------------------------------------------------------------------------
        //    1,2,3,4      , 5,6,7,8,      9,10,11,12,    13,14,15,16, /*|*/ 101,102,103,104,  105,106,107,108,  109,110,111,112,  113,114,115,116,
        //    17,18,19,20  , 21,22,23,24,  25,26,27,28,   29,30,31,32, /*|*/ 117,118,119,120,  121,122,123,124,  125,126,127,128,  129,130,131,132,
        //    33,34,35,36,   37,38,39,40,  41,42,43,44,   45,46,47,48,/*|*/  133,134,135,136,  137,138,139,140,  141,142,143,144,  145,146,147,148,
        //    49,50,51,52 ,  53,54,55,56,  57,58,59,60,   61,62,63,64,/*|*/  149,150,151,152,  153,154,155,156,  157,158,159,160,  161,162,163,164,
        //    //-----------------------------------------------------------------------------------------------------------------------------------
        //    1,2,3,4      , 5,6,7,8,      9,10,11,12,    13,14,15,16, /*|*/ 101,102,103,104,  105,106,107,108,  109,110,111,112,  113,114,115,116,
        //    17,18,19,20  , 21,22,23,24,  25,26,27,28,   29,30,31,32, /*|*/ 117,118,119,120,  121,122,123,124,  125,126,127,128,  129,130,131,132,
        //    33,34,35,36,   37,38,39,40,  41,42,43,44,   45,46,47,48,/*|*/  133,134,135,136,  137,138,139,140,  141,142,143,144,  145,146,147,148,
        //    49,50,51,52 ,  53,54,55,56,  57,58,59,60,   61,62,63,64,/*|*/  149,150,151,152,  153,154,155,156,  157,158,159,160,  161,162,163,164,
        //    //-----------------------------------------------------------------------------------------------------------------------------------
        //};
        //stride = 32; //bytes
        //bmpWidth = 8;
        //bmpHeight = 8;
    }


}
