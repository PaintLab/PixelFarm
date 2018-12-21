//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "114")]
    [Info("T114_FrameBuffer", SupportedOn = AvailableOn.GLES)]
    public class T114_FrameBufferWithConvFilterEffect : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter _painter;
        Framebuffer _frameBuffer;

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
                _glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\leaves.jpg");
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
                    _glsx.DrawImageWithConv3x3(_glbmp, Mat3x3ConvGen.emboss, 0, 0);
                    _glsx.DetachFramebuffer();

                    //after release current, we move back to default frame buffer again***
                    _frameBufferNeedUpdate = false;

                }
                //_glsx.DrawFrameBuffer(_frameBuffer, 0, 0, true);
                _glsx.DrawImage(_frameBuffer.GetGLBitmap(), 0, 0);
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

