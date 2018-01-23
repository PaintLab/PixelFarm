//MIT, 2014-2018, WinterDev
//credit : https://www.opengl.org/wiki/Framebuffer_Object_Examples
//credit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{

    public class FrameBufferCreationParameters
    {
        public bool depthBuffer;
        public bool stencilBuffer;
        public bool generateMipMaps;

        public PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba;
        public PixelFormat pixelFormat = PixelFormat.Rgba;
        public TextureMagFilter magFilter = TextureMagFilter.Linear;
        public TextureMinFilter minFilter = TextureMinFilter.LinearMipmapNearest;


    }
    public class FrameBuffer : IDisposable
    {
        int frameBufferId;
        int renderBufferId;
        int textureId;
        int w;
        int h;
        public FrameBuffer(int w, int h)
            : this(w, h, new FrameBufferCreationParameters())
        {
        }
        public FrameBuffer(int w, int h, FrameBufferCreationParameters creationParams)
        {
            this.w = w;
            this.h = h;
            InitFrameBuffer(creationParams);
        }
        public void Dispose()
        {
            //delete framebuffer,render buffer and texture id
            if (frameBufferId > 0)
            {
                GL.DeleteFramebuffers(1, ref frameBufferId);
                this.frameBufferId = 0;
            }
            if (renderBufferId > 0)
            {
                GL.DeleteRenderbuffers(1, ref renderBufferId);
                renderBufferId = 0;
            }
            if (textureId > 0)
            {
                GL.DeleteTexture(textureId);
                textureId = 0;
            }
        }
        public int TextureId { get { return textureId; } }
        public int FrameBufferId { get { return frameBufferId; } }
        public int Width { get { return w; } }
        public int Height { get { return h; } }
        void InitFrameBuffer(FrameBufferCreationParameters creationParams)
        {

            GL.GenFramebuffers(1, out frameBufferId);
            //switch to this (custom) framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);
            //create blank texture
            textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            //set texture parameter
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)creationParams.magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)creationParams.minFilter);
            //GL.GenerateMipmap(TextureTarget.Texture2D);

            GL.TexImage2D(TextureTarget.Texture2D, 0,creationParams.pixelInternalFormat, w, h, 0, creationParams.pixelFormat, PixelType.UnsignedByte, IntPtr.Zero);

            //render buffer
            GL.GenRenderbuffers(1, out renderBufferId);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderBufferId);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferInternalFormat.DepthComponent16, w, h);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, TextureTarget.Texture2D, textureId, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment, RenderbufferTarget.Renderbuffer, renderBufferId);
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
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.GenerateMipmap(TextureTarget.Texture2D);            
        }
        internal void ReleaseCurrent()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0); //unbind texture 
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); //switch back to default -framebuffer
        }
    }
}