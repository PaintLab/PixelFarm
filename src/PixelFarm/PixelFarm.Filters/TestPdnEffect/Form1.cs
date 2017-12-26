using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

using PaintDotNet;
using PaintDotNet.Effects;

namespace TestPdnEffect
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //create surface from memory 
            //on 32 argb format


            Bitmap bmp = new Bitmap("d:\\WImageTest\\lion1.png");

            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int w = bmp.Width;
            int h = bmp.Height;
            int stride = bmpData.Stride;
            int bufferLen = w * h;
            int[] srcBmpBuffer = new int[bufferLen]; 
            int[] destBmpBuffer = new int[bufferLen];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, srcBmpBuffer, 0, srcBmpBuffer.Length);
            bmp.UnlockBits(bmpData);
            //
            unsafe
            {
                fixed (int* srcBmpH = &srcBmpBuffer[0])
                fixed (int* destBmpH = &destBmpBuffer[0])
                {
                    MemHolder srcMemHolder = new MemHolder((IntPtr)srcBmpH, bufferLen);
                    Surface srcSurface = new Surface(stride, w, h, srcMemHolder);

                    MemHolder destMemHolder = new MemHolder((IntPtr)destBmpH, bufferLen);
                    Surface destSurface = new Surface(stride, w, h, destMemHolder);

                    //
                    //apply some filter
                    //

                    EmbossRenderer emboss = new EmbossRenderer();
                    emboss.SetParameters(30);
                    emboss.Render(srcSurface, destSurface, new PixelFarm.Drawing.Rectangle[]{
                            new PixelFarm.Drawing.Rectangle(0,0,w,h)
                        }, 0, 1);

                    //SharpenRenderer sharpen = new SharpenRenderer();
                    //sharpen.Amount = 2;
                    //sharpen.Render(srcSurface, destSurface, new PixelFarm.Drawing.Rectangle[]{
                    //        new PixelFarm.Drawing.Rectangle(0,0,w,h)
                    //    }, 0, 0);

                }
            }

            //save to output
            Bitmap outputBmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpData2 = outputBmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(destBmpBuffer, 0, bmpData2.Scan0, destBmpBuffer.Length);
            outputBmp.UnlockBits(bmpData2);

            this.pictureBox2.Image = outputBmp;
            this.pictureBox1.Image = bmp;


            //process the image
            //then copy to bitmap 
            //


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
