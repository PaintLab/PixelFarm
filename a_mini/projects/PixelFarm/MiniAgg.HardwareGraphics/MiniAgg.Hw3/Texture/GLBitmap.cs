//MIT 2014-2016, WinterDev

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    public abstract class LazyBitmapBufferProvider
    {
        public abstract IntPtr GetRawBufferHead();
        public abstract void ReleaseBufferHead();
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract bool IsInvert { get; }
    }


    public class GLImage : PixelFarm.Drawing.Image
    {
        GLBitmap glBitmap;
        public GLImage(GLBitmap glBitmap)
        {
            this.glBitmap = glBitmap;
        }
        public override int Height
        {
            get
            {
                return glBitmap.Height;
            }
        }

        public override IDisposable InnerImage
        {
            get
            {
                return this.glBitmap;
            }

            set
            {
                this.glBitmap = (GLBitmap)value;
            }
        }

        public override bool IsReferenceImage
        {
            get
            {
                return false;
            }
        }

        public override int ReferenceX
        {
            get
            {
                return 0;
            }
        }

        public override int ReferenceY
        {
            get
            {
                return 0;
            }
        }

        public override int Width
        {
            get
            {
                return this.glBitmap.Width;
            }
        }

        public override void Dispose()
        {
            //nothing
        }
    }

    public class GLBitmap : IDisposable
    {
        int textureId;
        int width;
        int height;
        byte[] rawBuffer;
        LazyBitmapBufferProvider lazyProvider;
        bool isInvertImage = false;
        public GLBitmap(int w, int h, byte[] rawBuffer, bool isInvertImage)
        {
            this.width = w;
            this.height = h;
            this.rawBuffer = rawBuffer;
            this.isInvertImage = isInvertImage;
        }
        public GLBitmap(LazyBitmapBufferProvider lazyProvider)
        {
            this.width = lazyProvider.Width;
            this.height = lazyProvider.Height;
            this.lazyProvider = lazyProvider;
            this.isInvertImage = lazyProvider.IsInvert;
        }

        public GLBitmap(int textureId, int w, int h)
        {
            this.textureId = textureId;
            this.width = w;
            this.height = h;
        }
        public bool DontSwapRedBlueChannel
        {
            get; set;
        }
        public bool IsInvert
        {
            get { return this.isInvertImage; }
        }


        public int TextureId { get { return textureId; } }
        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
        }


        //---------------------------------
        //only after gl context is created
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
                            PixelFormat.Rgba,
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
                           PixelFormat.Rgba,
                           PixelType.UnsignedByte, (IntPtr)bmpScan0);
                    this.lazyProvider.ReleaseBufferHead();
                }
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }

            return this.textureId;
        }
        //public void Bind()
        //{
        //    GL.BindTexture(TextureTarget.Texture2D, this.textureId);
        //}
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