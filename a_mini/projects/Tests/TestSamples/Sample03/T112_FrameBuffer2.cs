//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "112")]
    [Info("T112_FrameBuffer")]
    public class T112_FrameBuffer : DemoBase
    {
        GLRenderSurface _glsf;
        GLCanvasPainter painter;
        FrameBuffer frameBuffer;
        GLBitmap glbmp;
        bool isInit;
        bool frameBufferNeedUpdate;
        protected override void OnGLSurfaceReady(GLRenderSurface glsf, GLCanvasPainter painter)
        {
            this._glsf = glsf;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            frameBuffer = _glsf.CreateFrameBuffer(this.Width, this.Height);
            frameBufferNeedUpdate = true;
            //------------ 
        }
        protected override void DemoClosing()
        {
            _glsf.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsf.SmoothMode = CanvasSmoothMode.Smooth;
            _glsf.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsf.Clear(PixelFarm.Drawing.Color.White);
            _glsf.ClearColorBuffer();
            //-------------------------------
            if (!isInit)
            {
                glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
                isInit = true;
            }
            if (frameBuffer.FrameBufferId > 0)
            {
                if (frameBufferNeedUpdate)
                {
                    _glsf.AttachFrameBuffer(frameBuffer);
                    //------------------------------------------------------------------------------------  
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _glsf.Clear(PixelFarm.Drawing.Color.Red);
                    _glsf.DrawImage(glbmp, 0, 300);
                    //------------------------------------------------------------------------------------  
                    _glsf.DetachFrameBuffer();
                    //after release current, we move back to default frame buffer again***
                    frameBufferNeedUpdate = false;
                }
                _glsf.DrawFrameBuffer(frameBuffer, 15, 300);
            }
            else
            {
                _glsf.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            SwapBuffers();
        }
    }
}

