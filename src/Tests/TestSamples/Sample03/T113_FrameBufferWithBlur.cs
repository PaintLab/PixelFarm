//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "113")]
    [Info("T113_FrameBuffer")]
    public class T113_FramebufferWithBlur : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter _painter;
        Framebuffer _frameBuffer;
        Framebuffer _frameBuffer2;
        GLBitmap _glbmp;
        bool _isInit;
        bool _frameBufferNeedUpdate;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            _glsx = glsx;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            _frameBuffer = _glsx.CreateFramebuffer(this.Width, this.Height);
            _frameBufferNeedUpdate = true;
            //------------ 
            _frameBuffer2 = _glsx.CreateFramebuffer(this.Width, this.Height);
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
            if (!_isInit)
            {
                _glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.png");
                _isInit = true;
            }
            if (_frameBuffer.FrameBufferId > 0)
            {
                if (_frameBufferNeedUpdate)
                {
                    //------------------------------------------------------------------------------------           
                    //framebuffer
                    _glsx.AttachFramebuffer(_frameBuffer);
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _glsx.Clear(PixelFarm.Drawing.Color.Red);
                    _glsx.DrawImageWithBlurX(_glbmp, 0, 300);
                    _glsx.DetachFramebuffer();
                    //------------------------------------------------------------------------------------  
                    //framebuffer2
                    _glsx.AttachFramebuffer(_frameBuffer2);
                    GLBitmap bmp2 = new GLBitmap(_frameBuffer.TextureId, _frameBuffer.Width, _frameBuffer.Height);
                    bmp2.IsBigEndianPixel = true;
                    _glsx.DrawImageWithBlurY(bmp2, 0, 300);
                    _glsx.DetachFramebuffer();
                    //------------------------------------------------------------------------------------  
                    //after release current, we move back to default frame buffer again***
                    _frameBufferNeedUpdate = false;

                }
                _glsx.DrawFrameBuffer(_frameBuffer2, 15, 0);
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

