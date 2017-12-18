//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "113")]
    [Info("T113_FrameBuffer")]
    public class T113_FrameBufferWithBlur : DemoBase
    {
        GLRenderSurface _glsf;
        GLCanvasPainter painter;
        FrameBuffer frameBuffer;
        FrameBuffer frameBuffer2;
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
            frameBuffer2 = _glsf.CreateFrameBuffer(this.Width, this.Height);
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
                glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.png");
                isInit = true;
            }
            if (frameBuffer.FrameBufferId > 0)
            {
                if (frameBufferNeedUpdate)
                {
                    //------------------------------------------------------------------------------------           
                    //framebuffer
                    _glsf.AttachFrameBuffer(frameBuffer);
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _glsf.Clear(PixelFarm.Drawing.Color.Red);
                    _glsf.DrawImageWithBlurX(glbmp, 0, 300);
                    _glsf.DetachFrameBuffer();
                    //------------------------------------------------------------------------------------  
                    //framebuffer2
                    _glsf.AttachFrameBuffer(frameBuffer2);
                    GLBitmap bmp2 = new GLBitmap(frameBuffer.TextureId, frameBuffer.Width, frameBuffer.Height);
                    bmp2.IsBigEndianPixel = true;
                    _glsf.DrawImageWithBlurY(bmp2, 0, 300);
                    _glsf.DetachFrameBuffer();
                    //------------------------------------------------------------------------------------  
                    //after release current, we move back to default frame buffer again***
                    frameBufferNeedUpdate = false;

                }
                _glsf.DrawFrameBuffer(frameBuffer2, 15, 300);
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

