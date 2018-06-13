//MIT, 2014-present, WinterDev

using System;
using System.Windows.Forms;

namespace Mini
{
    static class Program
    {


        static unsafe void LookAsIntArray(IntPtr array)
        {
            int* a = (int*)array;
            int data = *a;
            byte R = (byte)(data & 0xff);
            byte G = (byte)((data >> 8) & 0xff);
            byte B = (byte)((data >> 16) & 0xff);
            byte A = (byte)((data >> 24) & 0xff);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //PaintLab.Svg.SvgParser parser = new PaintLab.Svg.SvgParser();
            //string svgContent = System.IO.File.ReadAllText("Samples/arrow2.svg");
            //parser.ParseDocument(new LayoutFarm.WebLexer.TextSnapshot(svgContent));

            RootDemoPath.Path = @"..\Data";
            YourImplementation.TestBedStartup.Setup();

#if GL_ENABLE
            YourImplementation.BootStrapOpenGLES2.SetupDefaultValues();
#endif
            //you can use your font loader
            YourImplementation.BootStrapWinGdi.SetupDefaultValues();
            //default text breaker, this bridge between              
#if DEBUG
            //PixelFarm.Agg.ActualImage.InstallImageSaveToFileService((IntPtr imgBuffer, int stride, int width, int height, string filename) =>
            //{

            //    using (System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //    {
            //        PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(imgBuffer, newBmp);
            //        //save
            //        newBmp.Save(filename);
            //    }
            //});
#endif

            //---------------------------------------------------
            //register image loader
            Mini.DemoHelper.RegisterImageLoader(LoadImage);
            //----------------------------

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormDev());
        }
        static PixelFarm.Agg.ActualBitmap LoadImage(string filename)
        {


            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename);


            var bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                       System.Drawing.Imaging.PixelFormat.Format32bppArgb //lock and read as 32-argb
                                       );

            int[] imgBuffer = new int[bmpData.Width * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgBuffer, 0, imgBuffer.Length);
            bmp.UnlockBits(bmpData);

            //PixelFarm.Agg.PixelFormat selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32;
            //switch (bmp.PixelFormat)
            //{
            //    default:
            //        throw new NotSupportedException();
            //    //case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
            //    //    {
            //    //        bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            //    //             System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //    //             System.Drawing.Imaging.PixelFormat.Format32bppArgb //lock and read as 32-argb 
            //    //             );
            //    //        selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32; //lock and read as 32-argb
            //    //        imgBuffer = new byte[bmpData.Stride * bmp.Height];
            //    //        System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgBuffer, 0, imgBuffer.Length);
            //    //        bmp.UnlockBits(bmpData);
            //    //    }
            //    //    break;
            //    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
            //    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
            //        {
            //            selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32;
            //            bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            //                System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //                bmp.PixelFormat //lock and read as 32-argb
            //                );

            //            imgBuffer = new int[bmpData.Width * bmp.Height];
            //            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgBuffer, 0, imgBuffer.Length);
            //            bmp.UnlockBits(bmpData);
            //        }
            //        break;
            //    case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
            //        //grey scale
            //        //selectedFormat = PixelFarm.Agg.PixelFormat.GrayScale8;
            //        throw new NotSupportedException();
            //}

            PixelFarm.Agg.ActualBitmap actualImg = PixelFarm.Agg.ActualBitmap.CreateFromBuffer(
                bmp.Width,
                bmp.Height,
                imgBuffer
                );
            //gdi+ load as little endian             
            actualImg.IsBigEndian = false;
            bmp.Dispose();
            return actualImg;
        }

    }
}
