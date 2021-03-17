//MIT, 2014-present, WinterDev
//credit : https://www.opengl.org/wiki/Framebuffer_Object_Examples
//credit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class Framebuffer : IDisposable
    {
        int _frameBufferId;
        int _renderBufferId;

        readonly int _width;
        readonly int _height;
        readonly bool _isBmpOwner;
        GLBitmap _glBmp;

        public Framebuffer(GLBitmap glBmp, bool isBmpOwner)
        {
            _glBmp = glBmp;
            _width = glBmp.Width;
            _height = glBmp.Height;
            _isBmpOwner = isBmpOwner;

            InitFrameBuffer();
        }
        public void Dispose()
        {
            //delete framebuffer,render buffer and texture id
            if (_frameBufferId > 0)
            {
                GL.DeleteFramebuffers(1, ref _frameBufferId);
                _frameBufferId = 0;
            }
            if (_renderBufferId > 0)
            {
                GL.DeleteRenderbuffers(1, ref _renderBufferId);
                _renderBufferId = 0;
            }
            if (_glBmp != null && _isBmpOwner)
            {
                _glBmp.Dispose();
                _glBmp = null;
            }
        }
        public GLBitmap GetGLBitmap() => _glBmp;
        public int FrameBufferId => _frameBufferId;
        public int Width => _width;
        public int Height => _height;
        void InitFrameBuffer()
        {

            GL.GenFramebuffers(1, out _frameBufferId);
            //switch to this (custom) framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferId);

            //---------
            //render buffer
            GL.GenRenderbuffers(1, out _renderBufferId);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _renderBufferId);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferInternalFormat.DepthComponent16, _width, _height);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, TextureTarget.Texture2D, _glBmp.GetServerTextureId(), 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment, RenderbufferTarget.Renderbuffer, _renderBufferId);
            //switch back to default framebuffer (system provider framebuffer) 

            GL.BindTexture(TextureTarget.Texture2D, 0);//unbind
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);//unbind => default framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); //unbind 
        }
        internal void MakeCurrent()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.FrameBufferId);
        }
        internal void UpdateTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, _glBmp.GetServerTextureId());
            GL.GenerateMipmap(TextureTarget.Texture2D);
        }
        internal void ReleaseCurrent()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0); //unbind texture 
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); //switch back to default -framebuffer
        }
    }
}