using System.Drawing;
using LayoutFarm.DrawingGL;
namespace LayoutFarm.DrawingGL
{
    public static class GLBitmapTextureHelper
    {

        public static GLBitmapTexture CreateBitmapTexture(System.Drawing.Bitmap bitmap)
        {

            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var bmpTexture = GLBitmapTexture.CreateBitmapTexture(bitmap.Width, bitmap.Height, data.Scan0);

            bitmap.UnlockBits(data);
            return bmpTexture;
        }
    }
}