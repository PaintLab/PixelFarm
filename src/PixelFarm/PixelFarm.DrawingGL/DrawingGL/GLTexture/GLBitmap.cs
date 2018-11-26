//MIT, 2014-present, WinterDev

using System;
using OpenTK.Graphics.ES20;
using PixelFarm.Drawing;

namespace PixelFarm.DrawingGL
{

    public class GLBitmap : Image
    {
        int _textureId;
        int _width;
        int _height;



        IntPtr _nativeImgMem;
        LazyBitmapBufferProvider _lazyProvider;
        bool _isNativePtrOwner;


        public GLBitmap(int w, int h, IntPtr nativeImgMem)
        {
            this._width = w;
            this._height = h;
            this._nativeImgMem = nativeImgMem;
        }
        public GLBitmap(LazyBitmapBufferProvider lazyProvider)
        {
            this._width = lazyProvider.Width;
            this._height = lazyProvider.Height;
            this._lazyProvider = lazyProvider;
            this.IsYFlipped = lazyProvider.IsYFlipped;
            this.BitmapFormat = lazyProvider.BitmapFormat;

        }
        public GLBitmap(int textureId, int w, int h)
        {
            this._textureId = textureId;
            this._width = w;
            this._height = h;
        }

        public GLBitmap(PixelFarm.CpuBlit.MemBitmap srcBmp)
        {
            this._width = srcBmp.Width;
            this._height = srcBmp.Height;

            _isNativePtrOwner = true;

            PixelFarm.CpuBlit.Imaging.TempMemPtr tmp = PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(srcBmp);
            _nativeImgMem = System.Runtime.InteropServices.Marshal.AllocHGlobal(tmp.LengthInBytes);
            unsafe
            {
                PixelFarm.CpuBlit.MemMx.memcpy((byte*)_nativeImgMem, (byte*)tmp.Ptr, tmp.LengthInBytes);
            }

        }

        public BitmapBufferFormat BitmapFormat { get; set; }
        public bool IsBigEndianPixel { get; set; }
        /// <summary>
        /// is vertical flipped
        /// </summary>
        public bool IsYFlipped { get; set; }
        public int TextureId => _textureId;

        public override int Width => this._width;
        public override int Height => this._height;

        public override bool IsReferenceImage => false;
        public override int ReferenceX => 0;
        public override int ReferenceY => 0;

        //---------------------------------
        //only after gl context is created
        internal int GetServerTextureId()
        {
            if (_textureId == 0)
            {
                BuildTexture();
            }
            return _textureId;
        }
        void BuildTexture()
        {
            //server part
            //gen texture 
            GL.GenTextures(1, out this._textureId);
            //bind
            GL.BindTexture(TextureTarget.Texture2D, this._textureId);
            if (_nativeImgMem != IntPtr.Zero)
            {
                GL.TexImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                      (TextureComponentCount)PixelInternalFormat.Rgba, this._width, this._height, 0,
                      PixelFormat.Rgba, // 
                      PixelType.UnsignedByte, _nativeImgMem);
            }
            else
            {
                //use lazy provider
                IntPtr bmpScan0 = this._lazyProvider.GetRawBufferHead();
                GL.TexImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                       (TextureComponentCount)PixelInternalFormat.Rgba, this._width, this._height, 0,
                       PixelFormat.Rgba,
                       PixelType.UnsignedByte, (IntPtr)bmpScan0);
                this._lazyProvider.ReleaseBufferHead();
            }
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        /// <summary>
        /// update texture from the same 'client source'
        /// </summary>
        public void UpdateTexture(Rectangle updateArea)
        {

            if (_textureId == 0)
            {
                BuildTexture();
                return;
            }

#if DEBUG
            Rectangle backupRect = updateArea;
#endif


            //if (updateArea.X != 0 || updateArea.Y != 0 ||
            //    updateArea.Width != this.Width || updateArea.Height != Height)
            //{
            //}
            //updateArea = Rectangle.Intersect(updateArea, new Rectangle(0, 0, Width, Height));
            updateArea = new Rectangle(0, 0, this.Width, this.Height);
            if (updateArea.Width == 0 || updateArea.Height == 0)
            {
                return;
            }
            //----

            GL.BindTexture(TextureTarget.Texture2D, this._textureId);
            if (_nativeImgMem != IntPtr.Zero)
            {
                GL.TexSubImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                      updateArea.X, updateArea.Y, updateArea.Width, updateArea.Height,
                      PixelFormat.Rgba, // 
                      PixelType.UnsignedByte, _nativeImgMem);
            }
            else
            {
                //use lazy provider

                IntPtr bmpScan0 = this._lazyProvider.GetRawBufferHead();

                GL.TexSubImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                     updateArea.X, updateArea.Y, updateArea.Width, updateArea.Height,
                     PixelFormat.Rgba, // 
                     PixelType.UnsignedByte, (IntPtr)bmpScan0);


                this._lazyProvider.ReleaseBufferHead();

            }

        }
        public override void Dispose()
        {
            //after delete the textureId will set to 0 ?
            if (_textureId > 0)
            {
                GL.DeleteTextures(1, ref _textureId);
            }

            if (_isNativePtrOwner && _nativeImgMem != IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(_nativeImgMem);
                _nativeImgMem = IntPtr.Zero;
                _isNativePtrOwner = false;
            }

        }
        //public override void RequestInternalBuffer(ref ImgBufferRequestArgs buffRequest)
        //{
        //    if (_rawIntBuffer != null)
        //    {
        //        int[] newBuff = new int[_rawIntBuffer.Length];
        //        System.Buffer.BlockCopy(_rawIntBuffer, 0, _rawIntBuffer, 0, newBuff.Length);
        //        buffRequest.OutputBuffer32 = newBuff;
        //    }
        //    else
        //    {

        //    }
        //}
#if DEBUG

        public readonly int dbugId = dbugIdTotal++;
        static int dbugIdTotal;
#endif
    }
}