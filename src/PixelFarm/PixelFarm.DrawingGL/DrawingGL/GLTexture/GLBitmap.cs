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
        bool _isOwner;
        PixelFarm.CpuBlit.MemBitmap _memBitmap;
        LazyBitmapBufferProvider _lazyProvider;


        public GLBitmap(int textureId, int w, int h)
        {
#if DEBUG
            if (dbugId >= 22)
            {

            }
#endif
            _textureId = textureId;
            _width = w;
            _height = h;
        }
        public GLBitmap(LazyBitmapBufferProvider lazyProvider)
        {
#if DEBUG
            if (dbugId >= 22)
            {

            }
#endif
            _width = lazyProvider.Width;
            _height = lazyProvider.Height;
            _lazyProvider = lazyProvider;
            this.IsYFlipped = lazyProvider.IsYFlipped;
            this.BitmapFormat = lazyProvider.BitmapFormat;
        }

        public GLBitmap(PixelFarm.CpuBlit.MemBitmap srcBmp, bool isOwner = false)
        {
#if DEBUG
            if (dbugId >= 22)
            {

            }
#endif
            _width = srcBmp.Width;
            _height = srcBmp.Height;
            //
            _memBitmap = srcBmp;
            _isOwner = isOwner;

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
            if (_memBitmap != null)
            {

                GL.TexImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                      (TextureComponentCount)PixelInternalFormat.Rgba, this._width, this._height, 0,
                      PixelFormat.Rgba, // 
                      PixelType.UnsignedByte, PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(_memBitmap).Ptr);
            }
            else
            {
                //use lazy provider
                IntPtr bmpScan0 = _lazyProvider.GetRawBufferHead();
                GL.TexImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                       (TextureComponentCount)PixelInternalFormat.Rgba, this._width, this._height, 0,
                       PixelFormat.Rgba,
                       PixelType.UnsignedByte, (IntPtr)bmpScan0);
                _lazyProvider.ReleaseBufferHead();
                _lazyProvider.NotifyUsage();

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
            if (_memBitmap != null)
            {
                GL.TexSubImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                      updateArea.X, updateArea.Y, updateArea.Width, updateArea.Height,
                      PixelFormat.Rgba, // 
                      PixelType.UnsignedByte, PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(_memBitmap).Ptr);
            }
            else
            {
                //use lazy provider 
                IntPtr bmpScan0 = _lazyProvider.GetRawBufferHead();
                GL.TexSubImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                     updateArea.X, updateArea.Y, updateArea.Width, updateArea.Height,
                     PixelFormat.Rgba, // 
                     PixelType.UnsignedByte, (IntPtr)bmpScan0);
                _lazyProvider.ReleaseBufferHead();
                _lazyProvider.NotifyUsage();
            } 
        }
        public override void Dispose()
        {
            //after delete the textureId will set to 0 ?
            if (_textureId > 0)
            {
                GL.DeleteTextures(1, ref _textureId);
            }
            if (_memBitmap != null)
            {
                //notify unused here?
                if (_isOwner)
                {
                    _memBitmap.Dispose();
                }
                _memBitmap = null; //***
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