//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using OpenTK.Graphics.ES20;
using PixelFarm.Drawing;

namespace PixelFarm.DrawingGL
{

    public sealed class GLBitmap : Image
    {
        int _textureId;
        bool _createFromBlank;

        int _width;
        int _height;
        bool _isOwner;
        PixelFarm.CpuBlit.MemBitmap _memBitmap;
        ImageBinder _bmpBufferProvider;//bmp binder  

        public GLBitmap(int w, int h, BitmapBufferFormat format = BitmapBufferFormat.RGBA)//native gl
        {
            //create blank glbitmap
            _width = w;
            _height = h;
            _createFromBlank = true;
            IsYFlipped = true;
            BitmapFormat = format;
        }
        public GLBitmap(ImageBinder bmpBuffProvider)
        {
            _width = bmpBuffProvider.Width;
            _height = bmpBuffProvider.Height;
            _bmpBufferProvider = bmpBuffProvider;

            this.IsYFlipped = bmpBuffProvider.IsYFlipped;
            this.BitmapFormat = bmpBuffProvider.BitmapFormat;
        }
        public GLBitmap(PixelFarm.CpuBlit.MemBitmap srcBmp, bool isMemBmpOwner = false)
        {
            _width = srcBmp.Width;
            _height = srcBmp.Height;
            //
            _memBitmap = srcBmp;
            _isOwner = isMemBmpOwner;
            this.BitmapFormat = srcBmp.BufferPixelFormat;
        }

#if DEBUG
        internal void dbugNotifyUsage()
        {
            //if (_bmpBufferProvider != null)
            //{
            //    _bmpBufferProvider.dbugNotifyUsage();
            //}
        }
#endif
        public BitmapBufferFormat BitmapFormat { get; private set; }

        /// <summary>
        /// is vertical flipped
        /// </summary>
        public bool IsYFlipped { get; set; }
        public int TextureId => _textureId;

        public override int Width => _width;
        public override int Height => _height;

        public override bool IsReferenceImage => false;
        public override int ReferenceX => 0;
        public override int ReferenceY => 0;

        //---------------------------------
        //only after gl context is created
        public int GetServerTextureId()
        {
            if (_textureId == 0)
            {
                if (_createFromBlank)
                {
                    BuildFromBlank();
                }
                else
                {
                    BuildFromExistingBitmap();
                }
            }
            return _textureId;
        }
        public void ReleaseServerSideTexture()
        {
            if (!_createFromBlank && _textureId > 0)
            {
                GL.DeleteTextures(1, ref _textureId);
                _textureId = 0;
            }
        }
        void BuildFromBlank()
        {
            _textureId = GL.GenTexture();
#if DEBUG
            System.Diagnostics.Debug.WriteLine("gen texture_id:" + _textureId);
#endif
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            //set texture parameter
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
            //GL.GenerateMipmap(TextureTarget.Texture2D);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        }
        void BuildFromExistingBitmap()
        {

            GL.GenTextures(1, out _textureId);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("gen texture_id:" + _textureId);
#endif
            //test convert from BGRA to RGBA

            //bind
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            if (_memBitmap != null)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0,
                      PixelInternalFormat.Rgba, _width, _height, 0,
                      PixelFormat.Rgba,
                      PixelType.UnsignedByte, PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(_memBitmap).Ptr);
            }
            else
            {

                IntPtr bmpScan0 = _bmpBufferProvider.GetRawBufferHead();
                GL.TexImage2D(TextureTarget.Texture2D, 0,
                       PixelInternalFormat.Rgba, _width, _height, 0,
                       PixelFormat.Rgba,
                       PixelType.UnsignedByte, bmpScan0);
                _bmpBufferProvider.ReleaseRawBufferHead(bmpScan0);

#if DEBUG
                _bmpBufferProvider.dbugNotifyUsage();
#endif

            }
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        }

        /// <summary>
        /// update texture from the same 'client source'
        /// </summary>
        public void UpdateTexture(Rectangle updateArea)
        {
            if (_createFromBlank) return;
            if (_textureId == 0)
            {
                BuildFromExistingBitmap();
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

            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            if (_memBitmap != null)
            {
                //GL.TexSubImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                // updateArea.X, updateArea.Y, updateArea.Width, updateArea.Height,
                // PixelFormat.Rgba, // 
                // PixelType.UnsignedByte, PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(_memBitmap).Ptr);

                GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                      updateArea.X, updateArea.Y, updateArea.Width, updateArea.Height,
                      PixelFormat.Rgba, // 
                      PixelType.UnsignedByte, PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(_memBitmap).Ptr);
            }
            else
            {
                //use lazy provider 
                IntPtr bmpScan0 = _bmpBufferProvider.GetRawBufferHead();

                //GL.TexSubImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                //     updateArea.X, updateArea.Y, updateArea.Width, updateArea.Height,
                //     PixelFormat.Rgba, // 
                //     PixelType.UnsignedByte, (IntPtr)bmpScan0);

                GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                  updateArea.X, updateArea.Y, updateArea.Width, updateArea.Height,
                  PixelFormat.Rgba, // 
                  PixelType.UnsignedByte, (IntPtr)bmpScan0);

#if DEBUG
                _bmpBufferProvider.dbugNotifyUsage();
#endif
            }
        }
        public override IntPtr GetRawBufferHead()
        {
            System.Diagnostics.Debugger.Break();
            return IntPtr.Zero;
        }
        public override void ReleaseRawBufferHead(IntPtr ptr)
        {
            System.Diagnostics.Debugger.Break();
        }

        public override void Dispose()
        {
            if (TextureContainer != null)
            {
                //after unload-> 
                //OwnerActiveTextureUnit will set OwnerActiveTextureUnit property to null
                TextureContainer.UnloadGLBitmap();
            }
            ReleaseServerSideTexture();
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

      
        internal TextureContainter TextureContainer { get; set; }
#if DEBUG

        public readonly int dbugId = dbugIdTotal++;
        static int dbugIdTotal;
#endif
    }
}