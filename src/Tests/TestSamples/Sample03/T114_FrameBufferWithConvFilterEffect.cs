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
        GLPainterContext _glsx;
        GLPainter _painter;
        GLRenderSurface _surface1;

        GLBitmap _glbmp;
        bool _isInit;
        bool _frameBufferNeedUpdate;
        protected override void OnGLSurfaceReady(GLPainterContext glsx, GLPainter painter)
        {
            _glsx = glsx;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            _surface1 = new GLRenderSurface(this.Width, this.Height);
            _frameBufferNeedUpdate = true;
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
            if (_surface1.IsValid)
            {
                if (_frameBufferNeedUpdate)
                {
                    //------------------------------------------------------------------------------------           
                    //framebuffer
                    _glsx.AttachToRenderSurface(_surface1);
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _glsx.Clear(PixelFarm.Drawing.Color.Red);
                    _glsx.DrawImageWithConv3x3(_glbmp, Mat3x3ConvGen.emboss, 0, 0);
                    _glsx.AttachToRenderSurface(null);

                    //after release current, we move back to default frame buffer again***
                    _frameBufferNeedUpdate = false;

                }
                //_glsx.DrawFrameBuffer(_frameBuffer, 0, 0, true);
                _glsx.DrawImage(_surface1.GetGLBitmap(), 0, 0);
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

