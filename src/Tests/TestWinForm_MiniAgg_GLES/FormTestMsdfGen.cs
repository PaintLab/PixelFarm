//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Typography.OpenFont;
using Typography.Rendering;
using Typography.Contours;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;

namespace Mini
{
    public partial class FormTestMsdfGen : Form
    {
        public FormTestMsdfGen()
        {
            InitializeComponent();
        }



        void GetExampleVxs(VertexStore outputVxs)
        {
            //counter-clockwise 
            //a triangle
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddCloseFigure();

            //a quad
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddLineTo(50, 10);
            //outputVxs.AddCloseFigure();



            ////curve4
            //outputVxs.AddMoveTo(5, 5);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddCurve4To(70, 20, 50, 10, 10, 5);
            //outputVxs.AddCloseFigure();

            //curve3
            //outputVxs.AddMoveTo(5, 5);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddCurve3To(70, 20, 10, 5);
            //outputVxs.AddCloseFigure();


            //a quad with hole
            outputVxs.AddMoveTo(10, 20);
            outputVxs.AddLineTo(50, 60);
            outputVxs.AddLineTo(70, 20);
            outputVxs.AddLineTo(50, 10);
            outputVxs.AddCloseFigure();

            outputVxs.AddMoveTo(30, 30);
            outputVxs.AddLineTo(40, 30);
            outputVxs.AddLineTo(40, 35);
            outputVxs.AddLineTo(30, 35);
            outputVxs.AddCloseFigure();
        }

        /// <summary>
        /// find (perpendicular) distance from point(x0,y0) to 
        /// a line that pass through point (x1,y1) and (x2,y2)
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        static double FindDistance(double x0, double y0,
            double x1, double y1,
            double x2, double y2)
        {
            //https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line

            double upperEq = Math.Abs((((y2 - y1) * x0) - ((x2 - x1) * y0) + (x2 * y1) - (y2 * x1)));
            double lowerEq = Math.Sqrt(((y2 - y1) * (y2 - y1)) + ((x2 - x1) * (x2 - x1)));
            return upperEq / lowerEq;
        }


        private void cmdSignedDistance_Click(object sender, EventArgs e)
        {
            double d1 = FindDistance(7, 10, 0, 0, 5, 5);
            double d2 = FindDistance(1, 1, 0, 0, 5, 5);
            double d3 = FindDistance(3, 1, 0, 0, 5, 5);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //test fake msdf (this is not real msdf gen)
            //--------------------  
            using (VxsTemp.Borrow(out var v1))
            {
                //--------
                GetExampleVxs(v1);
                //--------

                ExtMsdfGen.MsdfGen3 gen3 = new ExtMsdfGen.MsdfGen3();
#if DEBUG
                gen3.dbugWriteMsdfTexture = true;
#endif
                gen3.GenerateMsdfTexture(v1);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

     
        static float median(float r, float g, float b)
        {
            //return max(min(r, g), min(max(r, g), b));
            return Math.Max(Math.Min(r, g), Math.Min(Math.Max(r, g), b));
        }
        float[] ConvertToFloatBmp(int[] input)
        {
            float[] floatBmp = new float[input.Length * 3];
            int index = 0;
            for (int i = 0; i < input.Length; ++i)
            {
                int pixel = input[i];
                //rgb
                float r = (pixel & 0xff) / 255f;
                float g = ((pixel >> 8) & 0xff) / 255f;
                float b = ((pixel >> 16) & 0xff) / 255f;

                floatBmp[index] = r;
                floatBmp[index + 1] = g;
                floatBmp[index + 2] = b;

                index += 3;

            }
            return floatBmp;
        }

        private void FormTestMsdfGen_Load(object sender, EventArgs e)
        {
            string[] msdfGlyphs = Directory.GetFiles("Data", "*.png");
            foreach (string s in msdfGlyphs)
            {
                listBox1.Items.Add(s);
            }

            comboBox1.Items.Add(5);
            comboBox1.Items.Add(9);

            comboBox1.SelectedIndex = 0;
            listBox1.SelectedIndexChanged += (s1, e1) => GenerateMsdfOutput();
            chkOnlySignDist.CheckedChanged += (s1, e1) => GenerateMsdfOutput();
            comboBox1.SelectedIndexChanged += (s1, e1) => GenerateMsdfOutput();
        }

        void GenerateMsdfOutput1()
        {
            //generate msdf output 
            //from msdf fragment shader
            //#ifdef GL_OES_standard_derivatives
            //               #extension GL_OES_standard_derivatives : enable
            //           #endif  
            //           precision mediump float; 
            //           varying vec2 v_texCoord;                
            //           uniform sampler2D s_texture; //msdf texture 
            //           uniform vec4 u_color;

            //           float median(float r, float g, float b) {
            //               return max(min(r, g), min(max(r, g), b));
            //           }
            //           void main() {
            //               vec4 sample = texture2D(s_texture, v_texCoord);
            //               float sigDist = median(sample[0], sample[1], sample[2]) - 0.5;
            //               float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);  
            //               gl_FragColor= vec4(u_color[0],u_color[1],u_color[2],opacity * u_color[3]);
            //           }

            string msdfImg = listBox1.SelectedItem as string;

            Bitmap bmp = new Bitmap(msdfImg);
            this.pictureBox1.Image = bmp;
            //
            var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int[] buffer = new int[bmp.Width * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, bmp.Width * bmp.Height);
            bmp.UnlockBits(bmpdata);

            //------------
            //software-base msdf renderer (for study)
            float[] floatBmp = ConvertToFloatBmp(buffer);
            int[] output = new int[bmp.Width * bmp.Height];
            //------------
            int whiteBG = (255 << 24) | (255 << 16) | (255 << 8) | (255);
            for (int m = 0; m < output.Length; ++m)
            {
                //fill with white bg
                output[m] = whiteBG;
            }

            int px_index = 0;

            int px_height = bmp.Height;
            int px_width = bmp.Width;

            int line_head = 0;
            int nextline_head = 0;

            bool onlySignDist = chkOnlySignDist.Checked;
            for (int y = 0; y < px_height - 1; ++y)
            {
                line_head = y * px_width * 3;
                nextline_head = (y + 1) * px_width * 3;
                px_index = y * px_width;

                int i = 0;
                for (int x = 0; x < px_width - 1; ++x)
                {
                    //each pixel 
                    float r = floatBmp[line_head + i];
                    float g = floatBmp[line_head + i + 1];
                    float b = floatBmp[line_head + i + 2];
                    float sigDist = median(r, g, b) - 0.5f;

                    float toClamp = sigDist;

                    //get right px
                    float r1 = floatBmp[line_head + i + 3];
                    float g1 = floatBmp[line_head + i + 4];
                    float b1 = floatBmp[line_head + i + 5];

                    float d_r1 = r1 - r;
                    float d_g1 = g1 - g;
                    float d_b1 = b1 - b;

                    //get bottom px
                    float r_y = floatBmp[nextline_head + i];
                    float g_y = floatBmp[nextline_head + i + 1];
                    float b_y = floatBmp[nextline_head + i + 2];

                    float d_ry = r_y - r;
                    float d_gy = g_y - g;
                    float d_by = b_y - b;

                    //for test only
                    //fake gles  fwidth
                    float fwidth = Math.Abs(((d_r1 + d_g1 + d_b1) / 3f)) + Math.Abs(((d_ry + d_gy + d_by) / 3f));


                    toClamp = onlySignDist ? sigDist : sigDist / (fwidth);

                    toClamp = sigDist / (fwidth);

                    //float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);  

                    float opacity = (float)Math.Max(0, Math.Min(toClamp + 0.5, 1));

                    byte o_r = 0;
                    byte o_g = 0;
                    byte o_b = 0;
                    byte o_a = (byte)(255 * opacity);

                    output[px_index] = (o_a << 24) | (o_b << 16) | (o_g << 8) | o_r;
                    px_index++;
                    i += 3;
                }
            }


            Bitmap output2 = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpdata2 = output2.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, output2.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(output, 0, bmpdata2.Scan0, output.Length);
            output2.UnlockBits(bmpdata2);

            pictureBox2.Image = output2;
        }



        struct Pixel3f
        {
            public float r;
            public float g;
            public float b;
            public Pixel3f(float r, float g, float b)
            {
                this.r = r;
                this.g = g;
                this.b = b;
            }
        }

        class Pixel3fSampling9
        {
            public Pixel3f middle;

            public Pixel3f left;
            public Pixel3f right;
            public Pixel3f top;
            public Pixel3f bottom;

            public Pixel3f left_top;
            public Pixel3f right_top;

            public Pixel3f left_bottom;
            public Pixel3f right_bottom;

            public int N = 1;

            public Pixel3f WeightAvg()
            {
                switch (N)
                {
                    default: throw new NotSupportedException();
                    case 1: return middle;
                    case 5:
                        {
                            float r = (middle.r * 5 + left.r + right.r + top.r + bottom.r) / 9;
                            float g = (middle.g * 5 + left.g + right.g + top.g + bottom.g) / 9;
                            float b = (middle.b * 5 + left.b + right.b + top.b + bottom.b) / 9;
                            return new Pixel3f(r, g, b);
                        }
                    case 9:
                        {
                            float r = (middle.r * 5 + left.r + right.r + top.r + bottom.r + ((left_top.r + right_top.r + left_bottom.r + right_bottom.r) / 2)) / (9 + 2);
                            float g = (middle.g * 5 + left.g + right.g + top.g + bottom.g + ((left_top.g + right_top.g + left_bottom.g + right_bottom.g) / 2)) / 11;
                            float b = (middle.b * 5 + left.b + right.b + top.b + bottom.b + ((left_top.b + right_top.b + left_bottom.b + right_bottom.b) / 2)) / 11;
                            return new Pixel3f(r, g, b);


                        }
                }

            }
        }
        class Pixel3fBitmap
        {
            public Pixel3f[] pixelBuffer;
            public readonly int Width;
            public readonly int Height;
            public Pixel3fBitmap(int width, int height)
            {
                Width = width;
                Height = height;
            }
            public void Sampling5(int x, int y, Pixel3fSampling9 samplingOutput)
            {
                samplingOutput.N = 5;
                if (x > 0 && x < Width - 1 && y > 0 && y < Height - 1)
                {
                    int rowHead = Width * y;
                    int upperRowHead = rowHead - Width;
                    int lowerRowHead = rowHead + Width;

                    samplingOutput.middle = pixelBuffer[rowHead + x];
                    samplingOutput.left = pixelBuffer[rowHead + x - 1];
                    samplingOutput.right = pixelBuffer[rowHead + x + 1];
                    samplingOutput.top = pixelBuffer[upperRowHead + x];
                    samplingOutput.bottom = pixelBuffer[lowerRowHead + x];
                }
            }
            public void Sampling9(int x, int y, Pixel3fSampling9 samplingOutput)
            {
                samplingOutput.N = 9;
                if (x > 0 && x < Width - 1 && y > 0 && y < Height - 1)
                {

                    int rowHead = Width * y;
                    int upperRowHead = rowHead - Width;
                    int lowerRowHead = rowHead + Width;

                    samplingOutput.middle = pixelBuffer[rowHead + x];
                    samplingOutput.left = pixelBuffer[rowHead + x - 1];
                    samplingOutput.right = pixelBuffer[rowHead + x + 1];

                    samplingOutput.top = pixelBuffer[upperRowHead + x];
                    samplingOutput.left_top = pixelBuffer[upperRowHead + x - 1];
                    samplingOutput.right_top = pixelBuffer[upperRowHead + x + 1];

                    samplingOutput.bottom = pixelBuffer[lowerRowHead + x];
                    samplingOutput.left_bottom = pixelBuffer[lowerRowHead + x - 1];
                    samplingOutput.right_bottom = pixelBuffer[lowerRowHead + x + 1];
                }
            }
        }

        static Pixel3fBitmap CreatePixel3Bitmap(Bitmap bmp)
        {


            var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int[] buffer = new int[bmp.Width * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, bmp.Width * bmp.Height);
            bmp.UnlockBits(bmpdata);

            Pixel3fBitmap pix3fbmp = new Pixel3fBitmap(bmp.Width, bmp.Height);
            Pixel3f[] buffer3f = new Pixel3f[buffer.Length];
            pix3fbmp.pixelBuffer = buffer3f;


            for (int i = 0; i < buffer.Length; ++i)
            {
                int pixel = buffer[i];
                //rgb
                float r = (pixel & 0xff) / 255f;
                float g = ((pixel >> 8) & 0xff) / 255f;
                float b = ((pixel >> 16) & 0xff) / 255f;

                buffer3f[i] = new Pixel3f(r, g, b);
            }

            return pix3fbmp;
        }
        void GenerateMsdfOutput2()
        {
            //generate msdf output 
            //from msdf fragment shader
            //#ifdef GL_OES_standard_derivatives
            //               #extension GL_OES_standard_derivatives : enable
            //           #endif  
            //           precision mediump float; 
            //           varying vec2 v_texCoord;                
            //           uniform sampler2D s_texture; //msdf texture 
            //           uniform vec4 u_color;

            //           float median(float r, float g, float b) {
            //               return max(min(r, g), min(max(r, g), b));
            //           }
            //           void main() {
            //               vec4 sample = texture2D(s_texture, v_texCoord);
            //               float sigDist = median(sample[0], sample[1], sample[2]) - 0.5;
            //               float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);  
            //               gl_FragColor= vec4(u_color[0],u_color[1],u_color[2],opacity * u_color[3]);
            //           }

            string msdfImg = listBox1.SelectedItem as string;

            Bitmap bmp = new Bitmap(msdfImg);
            this.pictureBox1.Image = bmp;

            Pixel3fBitmap pixel3fBmp = CreatePixel3Bitmap(bmp);
            int px_index = 0;

            int px_height = bmp.Height;
            int px_width = bmp.Width;

            int line_head = 0;
            int nextline_head = 0;

            Pixel3f[] pixel3fBuffer = pixel3fBmp.pixelBuffer;
            int[] output = new int[px_width * px_height];

            bool onlySignDist = chkOnlySignDist.Checked;

            for (int y = 0; y < px_height - 1; ++y)
            {
                line_head = y * px_width;
                nextline_head = (y + 1) * px_width;
                px_index = y * px_width;

                int i = 0;
                for (int x = 0; x < px_width - 1; ++x)
                {
                    //each pixel 
                    Pixel3f rgb = pixel3fBuffer[line_head + i];

                    float r = rgb.r;
                    float g = rgb.g;
                    float b = rgb.b;
                    float sigDist = median(r, g, b) - 0.5f;

                    float toClamp = sigDist;

                    //get right px
                    Pixel3f next_right = pixel3fBuffer[line_head + i + 1];

                    float d_r1 = next_right.r - r;
                    float d_g1 = next_right.g - g;
                    float d_b1 = next_right.b - b;

                    //get bottom px
                    Pixel3f bottom = pixel3fBuffer[nextline_head + i];

                    float d_ry = bottom.r - r;
                    float d_gy = bottom.g - g;
                    float d_by = bottom.b - b;

                    //for test only
                    //fake gles  fwidth
                    float fwidth = Math.Abs(((d_r1 + d_g1 + d_b1) / 3f)) + Math.Abs(((d_ry + d_gy + d_by) / 3f));

                    toClamp = onlySignDist ? sigDist : sigDist / (fwidth);

                    //float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);  

                    float opacity = (float)Math.Max(0, Math.Min(toClamp + 0.5, 1));

                    byte o_r = 0;
                    byte o_g = 0;
                    byte o_b = 0;
                    byte o_a = (byte)(255 * opacity);

                    output[px_index] = (o_a << 24) | (o_b << 16) | (o_g << 8) | o_r;
                    px_index++;
                    i++;
                }
            }


            Bitmap output2 = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpdata2 = output2.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, output2.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(output, 0, bmpdata2.Scan0, output.Length);
            output2.UnlockBits(bmpdata2);

            pictureBox2.Image = output2;
        }

        void GenerateMsdfOutput3()
        {
            //generate msdf output 
            //from msdf fragment shader
            //#ifdef GL_OES_standard_derivatives
            //               #extension GL_OES_standard_derivatives : enable
            //           #endif  
            //           precision mediump float; 
            //           varying vec2 v_texCoord;                
            //           uniform sampler2D s_texture; //msdf texture 
            //           uniform vec4 u_color;

            //           float median(float r, float g, float b) {
            //               return max(min(r, g), min(max(r, g), b));
            //           }
            //           void main() {
            //               vec4 sample = texture2D(s_texture, v_texCoord);
            //               float sigDist = median(sample[0], sample[1], sample[2]) - 0.5;
            //               float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);  
            //               gl_FragColor= vec4(u_color[0],u_color[1],u_color[2],opacity * u_color[3]);
            //           }

            string msdfImg = listBox1.SelectedItem as string;

            Bitmap bmp = new Bitmap(msdfImg);
            this.pictureBox1.Image = bmp;

            Pixel3fBitmap pixel3fBmp = CreatePixel3Bitmap(bmp);
            int px_index = 0;

            int px_height = bmp.Height;
            int px_width = bmp.Width;



            int[] output = new int[px_width * px_height];

            bool onlySignDist = chkOnlySignDist.Checked;


            Pixel3fSampling9 current = new Pixel3fSampling9();
            Pixel3fSampling9 next_right_sm = new Pixel3fSampling9();
            Pixel3fSampling9 next_bottom_sm = new Pixel3fSampling9();

            int sampling = (int)comboBox1.SelectedItem;


            for (int y = 1; y < px_height - 1; ++y)
            {
                int x = 1;
                px_index = x + (px_width * y);

                for (; x < px_width - 1; ++x)
                {
                    //each pixel 
                    if (sampling == 9)
                    {
                        pixel3fBmp.Sampling9(x, y, current);
                        pixel3fBmp.Sampling9(x + 1, y, next_right_sm);
                        pixel3fBmp.Sampling9(x, y + 1, next_bottom_sm);
                    }
                    else
                    {
                        pixel3fBmp.Sampling5(x, y, current);
                        pixel3fBmp.Sampling5(x + 1, y, next_right_sm);
                        pixel3fBmp.Sampling5(x, y + 1, next_bottom_sm);
                    }


                    Pixel3f rgb = current.WeightAvg();
                    float r = rgb.r;
                    float g = rgb.g;
                    float b = rgb.b;
                    float sigDist = median(r, g, b) - 0.5f;

                    float toClamp = sigDist;

                    Pixel3f next_right = next_right_sm.WeightAvg();

                    float d_r1 = next_right.r - r;
                    float d_g1 = next_right.g - g;
                    float d_b1 = next_right.b - b;

                    //get bottom px
                    Pixel3f bottom = next_bottom_sm.WeightAvg();

                    float d_ry = bottom.r - r;
                    float d_gy = bottom.g - g;
                    float d_by = bottom.b - b;

                    //for test only
                    //fake gles  fwidth
                    float fwidth = Math.Abs(((d_r1 + d_g1 + d_b1) / 3f)) + Math.Abs(((d_ry + d_gy + d_by) / 3f));

                    toClamp = onlySignDist ? sigDist : sigDist / (fwidth);

                    //float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);  

                    float opacity = (float)Math.Max(0, Math.Min(toClamp + 0.5, 1));

                    byte o_r = 0;
                    byte o_g = 0;
                    byte o_b = 0;
                    byte o_a = (byte)(255 * opacity);

                    output[px_index] = (o_a << 24) | (o_b << 16) | (o_g << 8) | o_r;
                    px_index++;

                }
            }


            Bitmap output2 = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpdata2 = output2.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, output2.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(output, 0, bmpdata2.Scan0, output.Length);
            output2.UnlockBits(bmpdata2);

            pictureBox2.Image = output2;
        }
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void GenerateMsdfOutput()
        {
            //GenerateMsdfOutput1();
            //GenerateMsdfOutput2();
            GenerateMsdfOutput3();
        }

    }
}
