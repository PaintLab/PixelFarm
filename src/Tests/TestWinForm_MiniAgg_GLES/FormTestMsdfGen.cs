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
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
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
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GenerateMsdfOutput1();
        }
    }
}
