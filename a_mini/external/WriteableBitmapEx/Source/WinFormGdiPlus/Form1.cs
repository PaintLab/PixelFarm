//MIT, 2017, WinterDev
//example and test for WritableBitmap (https://github.com/teichgraf/WriteableBitmapEx) on Gdi+
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
            using (Bitmap bmp1 = new Bitmap(400, 500))
            using (var bmplock = bmp1.Lock())
            {
                WriteableBitmap wb = bmplock.GetWritableBitmap();
                //lines

                int y = 0;

                wb.DrawLine(0, y, 100, y + 100, System.Windows.Media.Imaging.Color.FromArgb(255, 255, 0, 0)); //red
                wb.DrawLine(0, y + 100, 100, y + 0, System.Windows.Media.Imaging.Color.FromArgb(255, 0, 0, 255)); //blue

                wb.DrawLineAa(100, y, 200, y + 100, System.Windows.Media.Imaging.Color.FromArgb(255, 255, 0, 0));
                wb.DrawLineAa(100, y + 100, 200, y + 0, System.Windows.Media.Imaging.Color.FromArgb(255, 0, 0, 255)); //blue 


                //----------
                y += 150;
                wb.DrawLineDDA(0, y, 100, y + 100, System.Windows.Media.Imaging.Color.FromArgb(255, 255, 0, 0)); //red
                wb.DrawLineDDA(0, y + 100, 100, y + 0, System.Windows.Media.Imaging.Color.FromArgb(255, 0, 0, 255)); //blue


                wb.DrawEllipse(200, 0, 300, 100, System.Windows.Media.Imaging.Color.FromArgb(255, 255, 0, 0));

                //
                bmplock.WriteAndUnlock();

                bmp1.Save("d:\\WImageTest\\a0002.png");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (Bitmap bmp1 = new Bitmap(400, 500))
            using (var bmplock = bmp1.Lock())
            {
                WriteableBitmap wb = bmplock.GetWritableBitmap();

                int y = 0;
                wb.FillRectangle(5, 5, 20, 20, System.Windows.Media.Imaging.Color.FromArgb(255, 255, 0, 0));
                wb.FillTriangle(100, 0, 150, 150, 200, 0, System.Windows.Media.Imaging.Color.FromArgb(255, 0, 0, 255));
                bmplock.WriteAndUnlock();
                bmp1.Save("d:\\WImageTest\\a0003.png");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (Bitmap src = new Bitmap("d:\\WImageTest\\L01.png"))
            using (var srcLock = src.Lock())
            using (Bitmap dest = new Bitmap(400, 500))
            using (var dstLock = dest.Lock())
            {
                WriteableBitmap dstWb = dstLock.GetWritableBitmap();
                WriteableBitmap srcWb = srcLock.GetWritableBitmap();
                int y = 0;
                dstWb.Clear(System.Windows.Media.Imaging.Color.FromArgb(255, 255, 255, 255));

                dstWb.Blit(new Rect(10, 10, src.Width, src.Height),
                        srcWb,
                        new Rect(0, 0, src.Width, src.Height), WriteableBitmapExtensions.BlendMode.None
                        );
                dstWb.Blit(new Rect(100, 100, src.Width * 2, src.Height * 2),
                       srcWb,
                       new Rect(0, 0, src.Width, src.Height), WriteableBitmapExtensions.BlendMode.None
                       );

                dstLock.WriteAndUnlock();
                dest.Save("d:\\WImageTest\\a0004.png");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormShape formShape = new FormShape();
            formShape.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FormFill formFill = new FormFill();
            formFill.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FormBlit formBlit = new FormBlit();
            formBlit.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormPlantDemo formPlant = new FormPlantDemo();
            formPlant.Show();
        }
    }
}
