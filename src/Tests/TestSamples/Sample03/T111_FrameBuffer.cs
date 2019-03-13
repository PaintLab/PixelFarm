//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
using OpenTK.Graphics.ES20;
namespace OpenTkEssTest
{
    [Info(OrderCode = "111")]
    [Info("T111_FrameBuffer", AvailableOn = AvailableOn.GLES)]
    public class T111_FrameBuffer : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        GLRenderSurface _surface1;
        bool _isInit;
        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            int max = Math.Max(this.Width, this.Height);
            _surface1 = new GLRenderSurface(max, max);
        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.Clear();
            //-------------------------------
            if (!_isInit)
            {
                _isInit = true;
            }
            if (_surface1.IsValid)
            {

                GLRenderSurface.InnerGLData innerData = _surface1.GetInnerGLData();
                //------------------------------------------------------------------------------------
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, innerData.FramebufferId);
                //--------
                //do draw to frame buffer here
                GL.ClearColor(OpenTK.Graphics.Color4.Red); //clear with red color
                GL.Clear(ClearBufferMask.ColorBufferBit);


                //------------------------------------------------------------------------------------ 
                GL.BindTexture(TextureTarget.Texture2D, innerData.GLBmp.GetServerTextureId());
                GL.GenerateMipmap(TextureTarget.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);//switch to default framewbuffer
                //------------------------------------------------------------------------------------

                //create gl bmp from framebuffer  
                _pcx.DrawImage(innerData.GLBmp, 15, 0);

                //
                GL.ClearColor(OpenTK.Graphics.Color4.White); //clear with red color
            }
            else
            {
                _pcx.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            SwapBuffers();
        }
    }
}

