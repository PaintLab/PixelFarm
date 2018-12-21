//MIT, 2014-present, WinterDev
//credit : https://www.opengl.org/wiki/Framebuffer_Object_Examples
//credit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    public class Framebuffer : IDisposable
    {
        int _frameBufferId;
        int _renderBufferId;
        int _textureId;
        int _width;
        int _height;

        GLBitmap _glBmp;
        public Framebuffer(int w, int h)
        {
            _width = w;
            _height = h;
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
            if (_textureId > 0)
            {
                GL.DeleteTexture(_textureId);
                _textureId = 0;
            }
        }
        public int TextureId => _textureId;
        public int FrameBufferId => _frameBufferId;
        public int Width => _width;
        public int Height => _height;
        void InitFrameBuffer()
        {

            GL.GenFramebuffers(1, out _frameBufferId);
            //switch to this (custom) framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferId);
            //create blank texture
            _textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            //set texture parameter
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
            //GL.GenerateMipmap(TextureTarget.Texture2D);
            GL.TexImage2D((TextureTarget2d)TextureTarget.Texture2D, 0, (TextureComponentCount)PixelInternalFormat.Rgba, _width, _height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            //render buffer
            GL.GenRenderbuffers(1, out _renderBufferId);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _renderBufferId);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferInternalFormat.DepthComponent16, _width, _height);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, (FramebufferAttachment)FramebufferSlot.ColorAttachment0, (TextureTarget2d)TextureTarget.Texture2D, _textureId, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, (FramebufferAttachment)FramebufferSlot.DepthAttachment, RenderbufferTarget.Renderbuffer, _renderBufferId);
            //switch back to default framebuffer (system provider framebuffer) 
            GL.BindTexture(TextureTarget.Texture2D, 0);//unbind
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);//unbind
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); //unbind 
        }
        internal void MakeCurrent()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.FrameBufferId);
        }
        internal void UpdateTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            GL.GenerateMipmap(TextureTarget.Texture2D);

        }
        internal void ReleaseCurrent()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0); //unbind texture 
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); //switch back to default -framebuffer
        }
        public GLBitmap GetGLBitmap()
        {
            return (_glBmp != null) ? _glBmp : _glBmp = new GLBitmap(_textureId, _width, _height) { IsBigEndianPixel = true, IsYFlipped = true };
        }
       
       
    }
}