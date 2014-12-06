//MIT 2014, WinterDev
using System.Text;
using System;
using OpenTK.Graphics.OpenGL;
using Tesselate;
using System.Drawing;


namespace OpenTkEssTest
{
    public class GLBitmapTexture : IDisposable
    {
        int textureId;
        int width;
        int height;
        private GLBitmapTexture()
        {
        }
        public static GLBitmapTexture CreateBitmapTexture(Bitmap bitmap)
        {
            GLBitmapTexture bmpTexture = new GLBitmapTexture();
            GL.GenTextures(1, out bmpTexture.textureId);
            GL.BindTexture(TextureTarget.Texture2D, bmpTexture.textureId);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);

            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int w = bmpTexture.width = data.Width;
            int h = bmpTexture.height = data.Height;

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba, w, h, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            return bmpTexture;
        }
        public static GLBitmapTexture CreateBitmapTexture(PixelFarm.Agg.ActualImage image)
        {
            GLBitmapTexture bmpTexture = new GLBitmapTexture();
            GL.GenTextures(1, out bmpTexture.textureId);
            GL.BindTexture(TextureTarget.Texture2D, bmpTexture.textureId);
          
            int w = bmpTexture.width = image.Width;
            int h = bmpTexture.height = image.Height;

            byte[] buffer = image.GetBuffer();
            unsafe
            {
                fixed (byte* buffHead = &buffer[0])
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0,
                        PixelInternalFormat.Rgba, w, h, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, buffer);
                }
            }


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            return bmpTexture;
        }

        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
        }
        internal int TextureId
        {
            get { return this.textureId; }
        }
        public void Dispose()
        {
            if (this.dbugId == 0)
            {
            }

            GL.DeleteTextures(1, ref textureId);
        }
#if DEBUG

        public readonly int dbugId = dbugIdTotal++;
        static int dbugIdTotal;
#endif

    }
}