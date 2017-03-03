//MIT, 2014-2017, WinterDev
 
namespace Mini
{
    public delegate PixelFarm.Agg.ActualImage LoadImageDelegate(string filename);

    static class DemoHelper
    {
        static LoadImageDelegate s_LoadImgDel;

        public static void RegisterImageLoader(LoadImageDelegate loadImgDel)
        {
            s_LoadImgDel = loadImgDel;
        }
        public static PixelFarm.Agg.ActualImage LoadImage(string imgFileName)
        {
            return s_LoadImgDel(imgFileName);
        }

        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(string imgFileName)
        {
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(imgFileName))
            {
                return LoadTexture(bmp);
            }
        }
        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(PixelFarm.Agg.ActualImage actualImg)
        {
            return new PixelFarm.DrawingGL.GLBitmap(actualImg.Width,
                actualImg.Height,
                PixelFarm.Agg.ActualImage.GetBuffer(actualImg), false);
        }
        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(System.Drawing.Bitmap bmp)
        {
            var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int stride = bmpdata.Stride;
            byte[] buffer = new byte[stride * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
            bmp.UnlockBits(bmpdata);
            //---------------------------
            //if we are on Little-endian  machine,
            //
            //---------------------------
            return new PixelFarm.DrawingGL.GLBitmap(bmp.Width, bmp.Height, buffer, false);
        }
    } 

}