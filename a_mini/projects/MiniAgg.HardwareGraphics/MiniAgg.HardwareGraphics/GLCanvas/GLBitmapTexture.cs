//MIT 2014, WinterDev
using System.Text;
using System;

using Tesselate;
using OpenTK.Graphics.OpenGL;

namespace LayoutFarm.DrawingGL
{
    public class GLBitmapTexture : IDisposable
    {
        int textureId;
        int width;
        int height;
        private GLBitmapTexture()
        {
        }

        public static GLBitmapTexture CreateBitmapTexture(int width, int height, IntPtr bmpScan0)
        {
            GLBitmapTexture bmpTexture = new GLBitmapTexture();
            GL.GenTextures(1, out bmpTexture.textureId);
            GL.BindTexture(TextureTarget.Texture2D, bmpTexture.textureId);


            int w = bmpTexture.width = width;
            int h = bmpTexture.height = height;

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba, w, h, 0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte, bmpScan0);

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
                        PixelFormat.Bgra,
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


            GL.DeleteTextures(1, ref textureId);
        }

#if DEBUG

        public readonly int dbugId = dbugIdTotal++;
        static int dbugIdTotal;
#endif

    }
}