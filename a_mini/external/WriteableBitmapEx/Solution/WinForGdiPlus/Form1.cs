using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Windows.Media.Imaging;
namespace WinFormGdiPlus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(400, 500);
            var bmpdata = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bufferLenInBytes = bmpdata.Stride * bmpdata.Height;
            int[] buffer = new int[bufferLenInBytes / 4];

            System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, bufferLenInBytes / 4);

            WriteableBitmap wb = new WriteableBitmap(bmp1.Width, bmp1.Height, buffer);

            //lines
            wb.DrawLine(0, 0, 100, 100, System.Windows.Media.Imaging.Color.FromArgb(255, 255, 0, 0)); //red
            wb.DrawLine(0, 100, 100, 0, System.Windows.Media.Imaging.Color.FromArgb(255, 0, 0, 255)); //blue

            wb.DrawLineAa(100, 0, 200, 100, System.Windows.Media.Imaging.Color.FromArgb(255, 255, 0, 0));
            wb.DrawLineAa(100, 100, 200, 0, System.Windows.Media.Imaging.Color.FromArgb(255, 0, 0, 255)); //blue

            //-------
            //write back
            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, bufferLenInBytes / 4);
            bmp1.UnlockBits(bmpdata);
            bmp1.Save("d:\\WImageTest\\a0002.png");

        }
    }
}
