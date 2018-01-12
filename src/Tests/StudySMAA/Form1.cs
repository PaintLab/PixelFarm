using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StudySMAA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string searchArea = "iVBORw0KGgoAAAANSUhEUgAAAEIAAAAhCAAAAABIXyLAAAAAOElEQVRIx2NgGAWjYBSMglEwEICREYRgFBZBqDCSLA2MGPUIVQETE9iNUAqLR5gIeoQKRgwXjwAAGn4AtaFeYLEAAAAASUVORK5CYII=";
            char[] bufferStr = searchArea.ToCharArray();
            byte[] pngBuffer = Convert.FromBase64CharArray(bufferStr, 0, bufferStr.Length);
            System.IO.File.WriteAllBytes("d:\\WImageTest\\search02.png", pngBuffer);

            //using (System.IO.MemoryStream ms = new System.IO.MemoryStream(pngBuffer))
            //{
            //    using (System.Drawing.Image bmp = System.Drawing.Bitmap.FromStream(ms))
            //    {
            //        bmp.Save("d:\\WImageTest\\search01.png");

            //    }
            //} 
        }
        private void button2_Click(object sender, EventArgs e)
        {
            byte[] buffer = null;
            using (System.Drawing.Bitmap bmp = new Bitmap("d:\\WImageTest\\line.png"))
            {
                //read as 32 rgba 
                //and copy a buffer
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                buffer = new byte[bmpdata.Stride * bmp.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
                bmp.UnlockBits(bmpdata);
            }
            //analyze the buffer


        }
    }
}
