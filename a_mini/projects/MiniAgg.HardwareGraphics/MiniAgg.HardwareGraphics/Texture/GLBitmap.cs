//MIT 2014, WinterDev
using System.Text;
using System;
using System.Runtime.InteropServices;

using Tesselate;
using OpenTK.Graphics.OpenGL;

namespace LayoutFarm.DrawingGL
{



    public abstract class LazyBitmapBufferProvider
    {
        public abstract IntPtr GetRawBufferHead();
        public abstract void ReleaseBufferHead();
        public abstract int Width { get; }
        public abstract int Height { get; }
    }

   
    public class GLBitmap : IDisposable
    {
        int textureId;
        int width;
        int height;
        byte[] rawBuffer;
        LazyBitmapBufferProvider lazyProvider;

        public GLBitmap(int w, int h, byte[] rawBuffer)
        {
            this.width = w;
            this.height = h;
            this.rawBuffer = rawBuffer;
        }
        public GLBitmap(LazyBitmapBufferProvider lazyProvider)
        {
            this.width = lazyProvider.Width;
            this.height = lazyProvider.Height;
            this.lazyProvider = lazyProvider;
        }

        internal int GetServerTextureId()
        {
            if (this.textureId == 0)
            {
                //server part
                //gen texture 
                GL.GenTextures(1, out this.textureId);
                //bind
                GL.BindTexture(TextureTarget.Texture2D, this.textureId);

                if (this.rawBuffer != null)
                {
                    unsafe
                    {
                        fixed (byte* bmpScan0 = &this.rawBuffer[0])
                        {
                            GL.TexImage2D(TextureTarget.Texture2D, 0,
                            PixelInternalFormat.Rgba, this.width, this.height, 0,
                            PixelFormat.Bgra,
                            PixelType.UnsignedByte, (IntPtr)bmpScan0);
                        }
                    }
                }
                else
                {
                    //use lazy provider
                    IntPtr bmpScan0 = this.lazyProvider.GetRawBufferHead();
                    GL.TexImage2D(TextureTarget.Texture2D, 0,
                           PixelInternalFormat.Rgba, this.width, this.height, 0,
                           PixelFormat.Bgra,
                           PixelType.UnsignedByte, (IntPtr)bmpScan0);
                    this.lazyProvider.ReleaseBufferHead();
                }
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }
            return this.textureId;
        }
        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
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