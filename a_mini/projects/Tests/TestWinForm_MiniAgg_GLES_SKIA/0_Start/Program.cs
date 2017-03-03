//MIT, 2014-2017, WinterDev

using System;
using System.Windows.Forms;

namespace Mini
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //---------------------------------------------------
            //register image loader
            Mini.DemoHelper.RegisterImageLoader(LoadImage);
            //----------------------------
            OpenTK.Toolkit.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RootDemoPath.Path = @"..\Data";
            //you can use your font loader
            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(YourImplementation.BootStrapWinGdi.myFontLoader);
            PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(YourImplementation.BootStrapOpenGLES2.myFontLoader);

            Application.Run(new FormDev());
        }
        static PixelFarm.Agg.ActualImage LoadImage(string filename)
        {


            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename);
            var bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                  System.Drawing.Imaging.ImageLockMode.ReadOnly,
                  bmp.PixelFormat);
            byte[] imgBuffer = new byte[bmpData.Stride * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgBuffer, 0, imgBuffer.Length);
            bmp.UnlockBits(bmpData);
           

            PixelFarm.Agg.PixelFormat selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32;
            switch (bmp.PixelFormat)
            {
                default:
                    {

                    }
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    selectedFormat = PixelFarm.Agg.PixelFormat.RGB24;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    //grey scale
                    selectedFormat = PixelFarm.Agg.PixelFormat.GrayScale8;
                    break;
            }

            PixelFarm.Agg.ActualImage actualImg = PixelFarm.Agg.ActualImage.CreateFromBuffer(
                bmp.Width,
                bmp.Height,
                selectedFormat,
                imgBuffer
                );
            //gdi+ load as little endian             
            actualImg.IsBigEndian = false;
            bmp.Dispose();
            return actualImg;
        }

    }
}
