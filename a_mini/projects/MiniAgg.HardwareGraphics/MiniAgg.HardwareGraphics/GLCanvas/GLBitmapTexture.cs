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

        public GLBitmapTexture(Bitmap bitmap)
        {

            GL.GenTextures(1, out textureId);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            this.width = data.Width;
            this.height = data.Height;

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba, width, height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
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


    }
}