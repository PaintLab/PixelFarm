//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
using OpenTK.Graphics.ES20;
namespace OpenTkEssTest
{
    [Info(OrderCode = "111")]
    [Info("T111_FrameBuffer")]
    public class T111_FrameBuffer : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter painter;
        FrameBuffer frameBuffer;
        bool isInit;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            int max = Math.Max(this.Width, this.Height);
            frameBuffer = _glsx.CreateFrameBuffer(max, max);
        }
        protected override void DemoClosing()
        {
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.Clear(PixelFarm.Drawing.Color.White);
            _glsx.ClearColorBuffer();
            //-------------------------------
            if (!isInit)
            {
                isInit = true;
            }
            if (frameBuffer.FrameBufferId > 0)//valid buffer
            {
                //------------------------------------------------------------------------------------
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer.FrameBufferId);
                //--------
                //draw to frame buffer here
                GL.ClearColor(OpenTK.Graphics.Color4.Red);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                //------------------------------------------------------------------------------------



                //------------------------------------------------------------------------------------ 
                GL.BindTexture(TextureTarget.Texture2D, frameBuffer.TextureId);
                GL.GenerateMipmap(TextureTarget.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                //------------------------------------------------------------------------------------

                GLBitmap bmp = new GLBitmap(frameBuffer.TextureId, frameBuffer.Width, frameBuffer.Height);
                bmp.IsBigEndianPixel = true;
                _glsx.DrawImage(bmp, 15, 300);
            }
            else
            {
                _glsx.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            SwapBuffers();
        }
    }
}

