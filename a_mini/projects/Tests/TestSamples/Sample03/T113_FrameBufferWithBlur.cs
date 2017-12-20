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
        GLRenderSurface _glsx;
        GLPainter painter;
        FrameBuffer frameBuffer;
        FrameBuffer frameBuffer2;
        GLBitmap glbmp;
        bool isInit;
        bool frameBufferNeedUpdate;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            frameBuffer = _glsx.CreateFrameBuffer(this.Width, this.Height);
            frameBufferNeedUpdate = true;
            //------------ 
            frameBuffer2 = _glsx.CreateFrameBuffer(this.Width, this.Height);
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
                glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.png");
                isInit = true;
            }
            if (frameBuffer.FrameBufferId > 0)
            {
                if (frameBufferNeedUpdate)
                {
                    //------------------------------------------------------------------------------------           
                    //framebuffer
                    _glsx.AttachFrameBuffer(frameBuffer);
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _glsx.Clear(PixelFarm.Drawing.Color.Red);
                    _glsx.DrawImageWithBlurX(glbmp, 0, 300);
                    _glsx.DetachFrameBuffer();
                    //------------------------------------------------------------------------------------  
                    //framebuffer2
                    _glsx.AttachFrameBuffer(frameBuffer2);
                    GLBitmap bmp2 = new GLBitmap(frameBuffer.TextureId, frameBuffer.Width, frameBuffer.Height);
                    bmp2.IsBigEndianPixel = true;
                    _glsx.DrawImageWithBlurY(bmp2, 0, 300);
                    _glsx.DetachFrameBuffer();
                    //------------------------------------------------------------------------------------  
                    //after release current, we move back to default frame buffer again***
                    frameBufferNeedUpdate = false;

                }
                _glsx.DrawFrameBuffer(frameBuffer2, 15, 300);
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

