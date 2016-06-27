//MIT 2014-2016, WinterDev
//credit : https://www.opengl.org/wiki/Framebuffer_Object_Examples
//credit : http://learningwebgl.com/lessons/lesson16/index.html
using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    public class FrameBuffer
    {
        int frameBufferId;
        int renderBufferId;
        int textureId;
        int w;
        int h;
        CanvasGL2d canvas2d;
        public FrameBuffer(CanvasGL2d canvas2d, int w, int h)
        {
            this.canvas2d = canvas2d;
            this.w = 150;
            this.h = 150;

            InitFrameBuffer();
        }
        public int TextureId { get { return textureId; } }
        public int FrameBufferId { get { return frameBufferId; } }
        public int Width { get { return w; } }
        public int Height { get { return h; } }
        void InitFrameBuffer()
        {
            //test only ****


            GL.GenFramebuffers(1, out frameBufferId);

            //switch to this (custom) framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);

            //create blank texture
            textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            //set texture parameter
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
            //GL.GenerateMipmap(TextureTarget.Texture2D);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, w, h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);


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

            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId); //unbind
            ////-----------------------------------------             
            //Drawing.Color c = Drawing.Color.White;
            //GL.ClearColor(
            //   (float)c.R / 255f,
            //   (float)c.G / 255f,
            //   (float)c.B / 255f,
            //   (float)c.A / 255f);
            ////-----------------------------------------
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); //unbind

        }
    }
}