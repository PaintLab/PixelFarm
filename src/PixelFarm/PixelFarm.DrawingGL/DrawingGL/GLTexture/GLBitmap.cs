//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using OpenTK.Graphics.ES20;
using PixelFarm.Drawing;

namespace PixelFarm.DrawingGL
{
    public class GLBitmapOwner
    {
        Dictionary<GLBitmap, bool> _registerGLBitmaps = new Dictionary<GLBitmap, bool>();
        Dictionary<GLBitmap, bool> _activeGLBitmaps = new Dictionary<GLBitmap, bool>();

        internal void RegisterGLBitmap(GLBitmap glBitmap)
        {
            _registerGLBitmaps.Add(glBitmap, true);
        }
        internal void AddToActiveGLBitmaps(GLBitmap glBitmap)
        {
            _activeGLBitmaps.Add(glBitmap, true);
        }
        internal void RemoveFromActiveGLBitmaps(GLBitmap glBitmap)
        {
            _activeGLBitmaps.Remove(glBitmap);
        }
    }
    public class GLBitmap : Image
    {
        int _textureId;
        int _width;
        int _height;
        bool _isOwner;
        PixelFarm.CpuBlit.MemBitmap _memBitmap;

        BitmapBufferProvider _bmpBufferProvider;//bmp binder 
        GLBitmapOwner _owner;
        public GLBitmap(int textureId, int w, int h, GLBitmapOwner owner = null)
        {
            _textureId = textureId;
            _width = w;
            _height = h;
            (_owner = owner)?.RegisterGLBitmap(this);

        }
        public GLBitmap(BitmapBufferProvider bmpBuffProvider, GLBitmapOwner owner = null)
        {
            _width = bmpBuffProvider.Width;
            _height = bmpBuffProvider.Height;
            _bmpBufferProvider = bmpBuffProvider;


            this.IsYFlipped = bmpBuffProvider.IsYFlipped;
            this.BitmapFormat = bmpBuffProvider.BitmapFormat;

            //
            (_owner = owner)?.RegisterGLBitmap(this);

        }
        public GLBitmap(PixelFarm.CpuBlit.MemBitmap srcBmp, bool isOwner = false, GLBitmapOwner owner = null)
        {

            _width = srcBmp.Width;
            _height = srcBmp.Height;
            //
            _memBitmap = srcBmp;


            _isOwner = isOwner;

            //
            (_owner = owner)?.RegisterGLBitmap(this);

        }
        public GLBitmapOwner Owner => _owner;

        internal void NotifyUsage()
        {
            if (_bmpBufferProvider != null)
            {
                _bmpBufferProvider.NotifyUsage();
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
        internal void ReleaseServerTextureId()
        {
            if (_textureId > 0)
            {
                GL.DeleteTextures(1, ref _textureId);
                
                if (_owner != null)
                {
                    _owner.RemoveFromActiveGLBitmaps(this);
                }
            }
        }
        void BuildTexture()
        {

            GL.GenTextures(1, out this._textureId);

            //if success then register this
            if (this._textureId > 0 && _owner != null)
            {
                _owner.AddToActiveGLBitmaps(this);
            }

#if DEBUG

            System.Diagnostics.Debug.WriteLine("texture_id" + this._textureId);
#endif


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

                IntPtr bmpScan0 = _bmpBufferProvider.GetRawBufferHead();
                GL.TexImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                       (TextureComponentCount)PixelInternalFormat.Rgba, this._width, this._height, 0,
                       PixelFormat.Rgba,
                       PixelType.UnsignedByte, (IntPtr)bmpScan0);
                _bmpBufferProvider.ReleaseBufferHead();
                _bmpBufferProvider.NotifyUsage();

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
                IntPtr bmpScan0 = _bmpBufferProvider.GetRawBufferHead();
                GL.TexSubImage2D((TextureTarget2d)TextureTarget.Texture2D, 0,
                     updateArea.X, updateArea.Y, updateArea.Width, updateArea.Height,
                     PixelFormat.Rgba, // 
                     PixelType.UnsignedByte, (IntPtr)bmpScan0);
                _bmpBufferProvider.ReleaseBufferHead();
                _bmpBufferProvider.NotifyUsage();
            }
        }
        public override void Dispose()
        {   
            ReleaseServerTextureId();
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

#if DEBUG

        public readonly int dbugId = dbugIdTotal++;
        static int dbugIdTotal;
#endif
    }
}