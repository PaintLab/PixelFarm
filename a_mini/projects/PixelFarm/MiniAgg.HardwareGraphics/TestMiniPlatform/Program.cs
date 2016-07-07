using System;
using System.Runtime.InteropServices;

namespace TestGlfw
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //dbugTestStbImages();
            GLFWProgram.Start();
        }

#if DEBUG 
        static void dbugTestStbImages()
        {

            int ver = PixelFarm.Drawing.MyFtImageLib.MyFtLibGetVersion();
            //--------------------------------------------------------------
            int w;
            int h;
            int comp;

            IntPtr loadImg1 = PixelFarm.Drawing.MyFtImageLib.stbi_load("d:\\WImageTest\\a00123.png", out w, out h, out comp, 0);
            PixelFarm.Drawing.MyFtImageLib.DeleteUnmanagedObj(loadImg1);

            //System.Drawing.Imaging.PixelFormat selectedFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            //if (comp == 3)
            //{
            //    selectedFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            //}
            //byte[] buffer = new byte[w * h * comp];
            //System.Runtime.InteropServices.Marshal.Copy(loadImg1, buffer, 0, buffer.Length);
            //DeleteUnmanagedObj(loadImg1);
            ////--------------------------------------------------------------

            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h, selectedFormat);
            //var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, selectedFormat);
            //System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
            //bmp.UnlockBits(bmpdata);
            //bmp.Save("d:\\WImageTest\\a00123_x.png");

        }
#endif
    }




}
