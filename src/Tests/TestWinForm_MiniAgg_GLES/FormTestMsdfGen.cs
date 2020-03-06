//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using ExtMsdfGen;


namespace Mini
{
    public partial class FormTestMsdfGen : Form
    {
        public FormTestMsdfGen()
        {
            InitializeComponent();

            cmbCustomVxs.Items.Add(
                new CustomVxsExample("triangle", outputVxs =>
                {
                    //counter - clockwise
                    //a triangle
                    outputVxs.AddMoveTo(10, 20);
                    outputVxs.AddLineTo(50, 60);
                    outputVxs.AddLineTo(70, 20);
                    outputVxs.AddCloseFigure();
                }));
            cmbCustomVxs.Items.Add(
              new CustomVxsExample("curve4", outputVxs =>
              {
                  outputVxs.AddMoveTo(5, 5);
                  outputVxs.AddLineTo(50, 60);
                  outputVxs.AddCurve4To(70, 20, 50, 10, 10, 5);
                  outputVxs.AddCloseFigure();

              }));
            cmbCustomVxs.Items.Add(
             new CustomVxsExample("curve3", outputVxs =>
             {
                 //curve3
                 outputVxs.AddMoveTo(5, 5);
                 outputVxs.AddLineTo(50, 60);
                 outputVxs.AddCurve3To(70, 20, 10, 5);
                 outputVxs.AddCloseFigure();

             }));
            cmbCustomVxs.Items.Add(
              new CustomVxsExample("quad", outputVxs =>
              {
                  //counter - clockwise
                  //a triangle
                  outputVxs.AddMoveTo(10, 20);
                  outputVxs.AddLineTo(50, 60);
                  outputVxs.AddLineTo(70, 20);
                  outputVxs.AddLineTo(50, 10);
                  outputVxs.AddCloseFigure();
              }));
            cmbCustomVxs.Items.Add(
             new CustomVxsExample("a quad with  a hole", outputVxs =>
             {
                 //counter - clockwise
                 //a triangle
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
             }));
            cmbCustomVxs.Items.Add(new CustomVxsExample("ArrowHead", outputVxs =>
            {
                VertexStore vxs1 = PixelFarm.PolygonShopDemo.BuildArrow(true);
                vxs1.ReverseClockDirection(outputVxs);//?? 
            }));
            cmbCustomVxs.Items.Add(
             new CustomVxsExample("BuildRoundCornerPolygon", outputVxs =>
             {
                 VertexStore vxs1 = PixelFarm.PolygonShopDemo.BuildRoundCornerPolygon();
                 vxs1.ReverseClockDirection(outputVxs);//?? 
             }));
            cmbCustomVxs.Items.Add(
            new CustomVxsExample("BuildCatmullRomSpline1", outputVxs =>
            {
                VertexStore vxs1 = PixelFarm.PolygonShopDemo.BuildCatmullRomSpline1();
                vxs1.ReverseClockDirection(outputVxs);//?? 
            }));

            cmbCustomVxs.SelectedIndex = cmbCustomVxs.Items.Count - 1;

            //
            cmbScaleMsdfOutput.Items.Add(1);
            cmbScaleMsdfOutput.Items.Add(2);
            cmbScaleMsdfOutput.Items.Add(3);
            cmbScaleMsdfOutput.Items.Add(5);
            cmbScaleMsdfOutput.Items.Add(8);
            cmbScaleMsdfOutput.Items.Add(16);
            cmbScaleMsdfOutput.Items.Add(32);
            cmbScaleMsdfOutput.Items.Add(64);
            cmbScaleMsdfOutput.SelectedIndex = 0;


            picLut.Bounds = pictureBox1.Bounds;//set to the same location
            picIdeal.Bounds = pictureBox2.Bounds;
        }

        class CustomVxsExample
        {
            readonly Action<VertexStore> _genVxs;
            public CustomVxsExample(string name, Action<VertexStore> genVxs)
            {
                Name = name;
                _genVxs = genVxs;
            }
            public void GenExampleVxs(VertexStore output)
            {
                _genVxs(output);
            }
            public string Name { get; private set; }
            public override string ToString()
            {
                return Name;
            }
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

        static MemBitmap LoadImage(string filename)
        {
            //read sample image
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename))
            {
                //read to image buffer 
                int bmpW = bmp.Width;
                int bmpH = bmp.Height;
                MemBitmap img = new MemBitmap(bmpW, bmpH);
                PixelFarm.CpuBlit.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(bmp, img);
                return img;
            }
        }


        static void FillAndSave(VertexStore vxs, string filename)
        {
            using (MemBitmap bmp = new MemBitmap(300, 300)) //approximate
            using (VectorToolBox.Borrow(out CurveFlattener flattener))
            using (VxsTemp.Borrow(out var v1))
            using (AggPainterPool.Borrow(bmp, out AggPainter painter))
            {
                painter.Clear(PixelFarm.Drawing.Color.White);//bg
                painter.FillColor = PixelFarm.Drawing.Color.Black;
                flattener.MakeVxs(vxs, v1);
                painter.Fill(v1);

                bmp.SaveImage(filename);
            }
        }
        string _scaled_lutFilename;
        private void button2_Click(object sender, EventArgs e)
        {
            //test fake msdf (this is not real msdf gen)
            //--------------------
            _scaled_lutFilename = null;//reset
            picLut.Image = null;
            if (_prevLutBmp != null)
            {
                _prevLutBmp.Dispose();
                _prevLutBmp = null;
            }

            using (VxsTemp.Borrow(out var v1))
            {
                //--------
                if (!(cmbCustomVxs.SelectedItem is CustomVxsExample customVxsExample))
                {
                    return;
                }
                customVxsExample.GenExampleVxs(v1);
                //--------

                ExtMsdfGen.MsdfGen3 gen3 = new ExtMsdfGen.MsdfGen3();
#if DEBUG
                gen3.dbugWriteMsdfTexture = true;

                {
                    //create ideal final image with agg for debug
                    _scaled_idealImgFilename = "ideal_1.png";
                    FillAndSave(v1, _scaled_idealImgFilename);

                    pictureBox5.Image = new Bitmap(_scaled_idealImgFilename);

                    int scale = (int)cmbScaleMsdfOutput.SelectedItem;
                    if (scale > 1)
                    {
                        ScaleImgAndSave(_scaled_idealImgFilename, scale, PixelFarm.CpuBlit.Imaging.FreeTransform.InterpolationMode.Bilinear, _scaled_idealImgFilename + "_s.png");
                        _scaled_idealImgFilename += "_s.png";
                    }
                }

#endif
                gen3.GenerateMsdfTexture(v1);

#if DEBUG
                if (gen3.dbugWriteMsdfTexture)
                {
                    pictureBox3.Image = new Bitmap(gen3.dbug_msdf_shape_lutName);
                    pictureBox4.Image = new Bitmap(gen3.dbug_msdf_output);
                    //----------------
                    string msdf_filename = gen3.dbug_msdf_output;

                    int scale = (int)cmbScaleMsdfOutput.SelectedItem;
                    if (scale > 1)
                    {
                        _scaled_lutFilename = gen3.dbug_msdf_shape_lutName + "_s.png";
                        ScaleImgAndSave(gen3.dbug_msdf_shape_lutName, scale, PixelFarm.CpuBlit.Imaging.FreeTransform.InterpolationMode.None, _scaled_lutFilename);

                        ScaleImgAndSave(msdf_filename, scale, PixelFarm.CpuBlit.Imaging.FreeTransform.InterpolationMode.Bilinear, msdf_filename + "_s.png");
                        msdf_filename = msdf_filename + "_s.png";
                    }

                    GenerateMsdfOutput3(msdf_filename);
                }
#endif

            }
        }
        static void ScaleImgAndSave(string inputImgFilename, float scale, PixelFarm.CpuBlit.Imaging.FreeTransform.InterpolationMode interpolation, string outputImgFilename)
        {
            PixelFarm.CpuBlit.Imaging.FreeTransform freeTx = new PixelFarm.CpuBlit.Imaging.FreeTransform();
            MemBitmap bmp = LoadImage(inputImgFilename);
            //freeTx.Interpolation = PixelFarm.CpuBlit.Imaging.FreeTransform.InterpolationMode.Bicubic;// PixelFarm.Agg.Imaging.FreeTransform.InterpolationMode.Bilinear;
            freeTx.Interpolation = interpolation;// PixelFarm.CpuBlit.Imaging.FreeTransform.InterpolationMode.Bilinear;
            //freeTx.SetFourCorners(
            //    new PixelFarm.VectorMath.PointF(0, 0),
            //    new PixelFarm.VectorMath.PointF(bmp.Width / 5, 0),
            //    new PixelFarm.VectorMath.PointF(bmp.Width / 5, bmp.Height / 5),
            //    new PixelFarm.VectorMath.PointF(0, bmp.Height / 5)
            //);

            freeTx.SetFourCorners(
               new PixelFarm.VectorMath.PointF(0, 0),
               new PixelFarm.VectorMath.PointF(bmp.Width * scale, 0),
               new PixelFarm.VectorMath.PointF(bmp.Width * scale, bmp.Height * scale),
               new PixelFarm.VectorMath.PointF(0, bmp.Height * scale)
           );


            using (MemBitmap transferBmp = freeTx.GetTransformedBitmap(bmp))
            {
                SaveImage(transferBmp, outputImgFilename);
            }

        }

        static void SaveImage(MemBitmap bmp, string filename)
        {
            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);
            PixelFarm.CpuBlit.BitmapHelper.CopyToGdiPlusBitmapSameSize(bmp, newBmp, false);
            newBmp.Save(filename);
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

            cmdSamplingSize.Items.Add(5);
            cmdSamplingSize.Items.Add(9);

            cmdSamplingSize.SelectedIndex = 0;
            listBox1.SelectedIndexChanged += (s1, e1) => GenerateMsdfOutput();
            chkOnlySignDist.CheckedChanged += (s1, e1) => GenerateMsdfOutput();
            cmdSamplingSize.SelectedIndexChanged += (s1, e1) => GenerateMsdfOutput();
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


        class Pixel3fSampling9
        {
            public FloatRGB middle;

            public FloatRGB left;
            public FloatRGB right;
            public FloatRGB top;
            public FloatRGB bottom;

            public FloatRGB left_top;
            public FloatRGB right_top;

            public FloatRGB left_bottom;
            public FloatRGB right_bottom;

            public int N = 1;

            public FloatRGB WeightAvg()
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
                            return new FloatRGB(r, g, b);
                        }
                    case 9:
                        {
                            float r = (middle.r * 5 + left.r + right.r + top.r + bottom.r + ((left_top.r + right_top.r + left_bottom.r + right_bottom.r) / 2)) / (9 + 2);
                            float g = (middle.g * 5 + left.g + right.g + top.g + bottom.g + ((left_top.g + right_top.g + left_bottom.g + right_bottom.g) / 2)) / 11;
                            float b = (middle.b * 5 + left.b + right.b + top.b + bottom.b + ((left_top.b + right_top.b + left_bottom.b + right_bottom.b) / 2)) / 11;
                            return new FloatRGB(r, g, b);
                        }
                }

            }


            FloatRGBBmp _samplingSrc;
            int _sW;
            int _sH;
            public Pixel3fSampling9() { }
            public void SetSamplingSource(FloatRGBBmp bmp)
            {
                _samplingSrc = bmp;
                if (_samplingSrc != null)
                {
                    _sW = bmp.Width;
                    _sH = bmp.Height;
                }
            }
            public void Sampling(int x, int y)
            {
                switch (N)
                {
                    default: throw new NotSupportedException();
                    case 1:
                        {
                            int rowHead = _sW * y;
                            middle = _samplingSrc._buffer[rowHead + x];
                        }
                        break;
                    case 5:
                        {
                            if (x > 0 && x < _sW - 1 && y > 0 && y < _sH - 1)
                            {
                                FloatRGB[] pixelBuffer = _samplingSrc._buffer;
                                int rowHead = _sW * y;
                                int upperRowHead = rowHead - _sW;
                                int lowerRowHead = rowHead + _sW;

                                middle = pixelBuffer[rowHead + x];
                                left = pixelBuffer[rowHead + x - 1];
                                right = pixelBuffer[rowHead + x + 1];
                                top = pixelBuffer[upperRowHead + x];
                                bottom = pixelBuffer[lowerRowHead + x];
                            }
                        }
                        break;
                    case 9:
                        {
                            if (x > 0 && x < _sW - 1 && y > 0 && y < _sH - 1)
                            {
                                FloatRGB[] pixelBuffer = _samplingSrc._buffer;
                                int rowHead = _sW * y;
                                int upperRowHead = rowHead - _sW;
                                int lowerRowHead = rowHead + _sW;

                                middle = pixelBuffer[rowHead + x];
                                left = pixelBuffer[rowHead + x - 1];
                                right = pixelBuffer[rowHead + x + 1];

                                top = pixelBuffer[upperRowHead + x];
                                left_top = pixelBuffer[upperRowHead + x - 1];
                                right_top = pixelBuffer[upperRowHead + x + 1];

                                bottom = pixelBuffer[lowerRowHead + x];
                                left_bottom = pixelBuffer[lowerRowHead + x - 1];
                                right_bottom = pixelBuffer[lowerRowHead + x + 1];
                            }
                        }
                        break;
                }

            }
        }

        static FloatRGBBmp CreatePixel3Bitmap(Bitmap bmp)
        {
            var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int[] buffer = new int[bmp.Width * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, bmp.Width * bmp.Height);
            bmp.UnlockBits(bmpdata);

            FloatRGBBmp pix3fbmp = new FloatRGBBmp(bmp.Width, bmp.Height);
            FloatRGB[] buffer3f = pix3fbmp._buffer;
            for (int i = 0; i < buffer.Length; ++i)
            {
                int pixel = buffer[i];
                //rgb
                float r = (pixel & 0xff) / 255f;
                float g = ((pixel >> 8) & 0xff) / 255f;
                float b = ((pixel >> 16) & 0xff) / 255f;

                buffer3f[i] = new FloatRGB(r, g, b);
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

            FloatRGBBmp pixel3fBmp = CreatePixel3Bitmap(bmp);
            int px_index = 0;

            int px_height = bmp.Height;
            int px_width = bmp.Width;

            int line_head = 0;
            int nextline_head = 0;

            FloatRGB[] pixel3fBuffer = pixel3fBmp._buffer;
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
                    FloatRGB rgb = pixel3fBuffer[line_head + i];

                    float r = rgb.r;
                    float g = rgb.g;
                    float b = rgb.b;
                    float sigDist = median(r, g, b) - 0.5f;

                    float toClamp = sigDist;

                    //get right px
                    FloatRGB next_right = pixel3fBuffer[line_head + i + 1];

                    float d_r1 = next_right.r - r;
                    float d_g1 = next_right.g - g;
                    float d_b1 = next_right.b - b;

                    //get bottom px
                    FloatRGB bottom = pixel3fBuffer[nextline_head + i];

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

        Bitmap _pic1Bmp = null;
        Bitmap _pic2Bmp = null;


        void GenerateMsdfOutput3(string msdfImg)
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


            pictureBox1.Image = null;
            if (_pic1Bmp != null)
            {
                _pic1Bmp.Dispose();
                _pic1Bmp = null;
            }
            _pic1Bmp = new Bitmap(msdfImg);
            this.pictureBox1.Image = _pic1Bmp;

            FloatRGBBmp pixel3fBmp = CreatePixel3Bitmap(_pic1Bmp);
            int px_index = 0;

            int px_height = _pic1Bmp.Height;
            int px_width = _pic1Bmp.Width;



            int[] output = new int[px_width * px_height];

            bool onlySignDist = chkOnlySignDist.Checked;


            Pixel3fSampling9 sm = new Pixel3fSampling9();
            sm.SetSamplingSource(pixel3fBmp);

            if ((int)cmdSamplingSize.SelectedItem == 9)
            {
                sm.N = 9;
            }
            else
            {
                sm.N = 5;
            }

            for (int y = 1; y < px_height - 1; ++y)
            {
                int x = 1;
                px_index = x + (px_width * y);

                for (; x < px_width - 1; ++x)
                {
                    //each pixel 
                    sm.Sampling(x, y);


                    FloatRGB rgb = sm.WeightAvg();
                    float r = rgb.r;
                    float g = rgb.g;
                    float b = rgb.b;
                    float sigDist = median(r, g, b) - 0.5f;

                    float toClamp = sigDist;

                    sm.Sampling(x + 1, y);
                    FloatRGB next_right = sm.WeightAvg();

                    float d_r1 = next_right.r - r;
                    float d_g1 = next_right.g - g;
                    float d_b1 = next_right.b - b;

                    //get bottom px
                    sm.Sampling(x, y + 1);
                    FloatRGB bottom = sm.WeightAvg();

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


            Bitmap output2 = new Bitmap(_pic1Bmp.Width, _pic1Bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpdata2 = output2.LockBits(new System.Drawing.Rectangle(0, 0, _pic1Bmp.Width, _pic1Bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, output2.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(output, 0, bmpdata2.Scan0, output.Length);
            output2.UnlockBits(bmpdata2);


            pictureBox2.Image = null;
            if (_pic2Bmp != null)
            {
                _pic2Bmp.Dispose();
                _pic2Bmp = null;
            }
            pictureBox2.Image = _pic2Bmp = output2;
        }


        void GenerateMsdfOutput()
        {
            //GenerateMsdfOutput1();
            //GenerateMsdfOutput2();


            if (listBox1.SelectedItem is string filename)
            {
                if (cmbScaleMsdfOutput.SelectedIndex > 0)
                {
                    string only_filename = Path.GetFileName(filename);
                    int scale = (int)cmbScaleMsdfOutput.SelectedItem;
                    ScaleImgAndSave(filename, scale, PixelFarm.CpuBlit.Imaging.FreeTransform.InterpolationMode.Bilinear,
                        only_filename + "_s.png"
                        );
                    filename = only_filename + "_s.png";
                }


                GenerateMsdfOutput3(filename);
            }

        }

        Bitmap _prevLutBmp;
        private void chkShowLut_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowLut.Checked)
            {
                //show lut img for debug
                if (_scaled_lutFilename != null)
                {
                    if (_prevLutBmp == null)
                    {
                        _prevLutBmp = new Bitmap(_scaled_lutFilename);
                        picLut.Image = _prevLutBmp;
                    }
                    picLut.Visible = true;
                }
            }
            else
            {
                picLut.Visible = false;
            }
        }

        Bitmap _idealBmp;
        string _scaled_idealImgFilename;

        private void chkShowIdeal_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowIdeal.Checked)
            {
                //show lut img for debug
                if (_scaled_idealImgFilename != null)
                {
                    if (_idealBmp == null)
                    {
                        _idealBmp = new Bitmap(_scaled_idealImgFilename);
                        picIdeal.Image = _idealBmp;
                    }
                    picIdeal.Visible = true;
                }
            }
            else
            {
                picIdeal.Visible = false;
            }

        }
    }
}
