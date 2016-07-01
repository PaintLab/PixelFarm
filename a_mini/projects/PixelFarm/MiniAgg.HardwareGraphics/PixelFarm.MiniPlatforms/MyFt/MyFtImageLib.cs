using System;
using System.Runtime.InteropServices;
namespace PixelFarm.Drawing
{
    public static class MyFtImageLib
    {
        const string myfontLib = @"myft.dll";
        //static void dbugTestStbImages()
        //{
        //    int ver = MyFtLibGetVersion();

        //    //--------------------------------------------------------------
        //    int w;
        //    int h;
        //    int comp;
        //    IntPtr loadImg1 = stbi_load("d:\\WImageTest\\a00123.png", out w, out h, out comp, 0);
        //    DeleteUnmanagedObj(loadImg1);

        //    //System.Drawing.Imaging.PixelFormat selectedFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
        //    //if (comp == 3)
        //    //{
        //    //    selectedFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
        //    //}
        //    //byte[] buffer = new byte[w * h * comp];
        //    //System.Runtime.InteropServices.Marshal.Copy(loadImg1, buffer, 0, buffer.Length);
        //    //DeleteUnmanagedObj(loadImg1);
        //    ////--------------------------------------------------------------

        //    //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h, selectedFormat);
        //    //var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, selectedFormat);
        //    //System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
        //    //bmp.UnlockBits(bmpdata);
        //    //bmp.Save("d:\\WImageTest\\a00123_x.png");

        //}

        [DllImport(myfontLib)]
        public static extern int MyFtLibGetVersion();
        [DllImport(myfontLib)]
        public static extern void DeleteUnmanagedObj(IntPtr unmanagedObject);
        [DllImport(myfontLib)]
        public static extern IntPtr stbi_load(string filename, out int w, out int h, out int comp, int requestOutputComponent);


    }
}