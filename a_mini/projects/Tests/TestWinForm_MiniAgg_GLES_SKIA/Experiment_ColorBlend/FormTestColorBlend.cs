//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

using System.Windows.Forms;

namespace Mini
{
    public partial class FormTestColorBlend : Form
    {
        Graphics g;
        Bitmap bmp_src;
        Bitmap bmp_result;
        BrightnessAndContrastAdjustment brightnessAndContrastAdjustment;
        public FormTestColorBlend()
        {
            InitializeComponent(); 
        }

        private void FormTestColorBlend_Load(object sender, EventArgs e)
        {
            this.colorCompoBox1.SetColor(System.Drawing.Color.FromArgb(255, 125, 125, 125));
            brightnessAndContrastAdjustment = new BrightnessAndContrastAdjustment();
            brightnessAndContrastAdjustment.SetParameters(0,30);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //test img contrast

            UpdateOutput();
        }
        void UpdateOutput()
        {
            if (g == null)
            {
                g = panel1.CreateGraphics();
                bmp_src = new Bitmap("d:\\WimageTest\\subpix_29_1.png");
                bmp_result = new Bitmap(bmp_src.Width, bmp_src.Height, bmp_src.PixelFormat);
                //--------
                var bmp_src_data = bmp_src.LockBits(new Rectangle(0, 0, bmp_src.Width, bmp_src.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp_src.PixelFormat);
                var bmp_dest_data = bmp_result.LockBits(new Rectangle(0, 0, bmp_result.Width, bmp_result.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp_result.PixelFormat);
                byte[] srcBuffer = new byte[bmp_src_data.Stride * bmp_src_data.Height];
                byte[] destBuffer = new byte[bmp_dest_data.Stride * bmp_dest_data.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmp_src_data.Scan0, srcBuffer, 0, srcBuffer.Length);
                //--------

                brightnessAndContrastAdjustment.Apply(srcBuffer, destBuffer, bmp_src_data.Stride, bmp_src_data.Height);
                //--------
                System.Runtime.InteropServices.Marshal.Copy(destBuffer, 0, bmp_dest_data.Scan0, destBuffer.Length);
                //--------
                bmp_src.UnlockBits(bmp_src_data);
                bmp_result.UnlockBits(bmp_dest_data);
            }
            g.DrawImage(bmp_src, 0, 0);
            g.DrawImage(bmp_result, 0, bmp_src.Height);

        }
    }



}
