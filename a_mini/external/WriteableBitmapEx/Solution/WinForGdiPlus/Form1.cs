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
                wb.DrawLine(0, 0, 100, 100, System.Windows.Media.Imaging.Color.FromArgb(255, 255, 0, 0)); //red
                wb.DrawLine(0, 100, 100, 0, System.Windows.Media.Imaging.Color.FromArgb(255, 0, 0, 255)); //blue

                wb.DrawLineAa(100, 0, 200, 100, System.Windows.Media.Imaging.Color.FromArgb(255, 255, 0, 0));
                wb.DrawLineAa(100, 100, 200, 0, System.Windows.Media.Imaging.Color.FromArgb(255, 0, 0, 255)); //blue 
                wb.DrawEllipse(200, 0, 300, 100, System.Windows.Media.Imaging.Color.FromArgb(255, 255, 0, 0));

                //
                bmplock.WriteAndUnlock();

                bmp1.Save("d:\\WImageTest\\a0002.png");
            }

        }
    }
}
