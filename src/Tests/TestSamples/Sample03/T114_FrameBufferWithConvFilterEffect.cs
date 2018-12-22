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
        GLPainterContext _pcx;
        GLPainter _painter;
        GLRenderSurface _surface1;

        GLBitmap _glbmp;
        bool _isInit;
        bool _frameBufferNeedUpdate;
        protected override void OnGLPainterReady(GLPainterContext pcx, GLPainter painter)
        {
            _pcx = pcx;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            _surface1 = new GLRenderSurface(this.Width, this.Height);
            _frameBufferNeedUpdate = true;
        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.Clear(PixelFarm.Drawing.Color.White);
            _pcx.ClearColorBuffer();
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
                    _pcx.AttachToRenderSurface(_surface1);
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _pcx.Clear(PixelFarm.Drawing.Color.Red);
                    _pcx.DrawImageWithConv3x3(_glbmp, Mat3x3ConvGen.emboss, 0, 0);
                    _pcx.AttachToRenderSurface(null);

                    //after release current, we move back to default frame buffer again***
                    _frameBufferNeedUpdate = false;

                }
                //_pcx.DrawFrameBuffer(_frameBuffer, 0, 0, true);
                _pcx.DrawImage(_surface1.GetGLBitmap(), 0, 0);
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

