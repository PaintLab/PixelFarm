//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "113")]
    [Info("T113_FrameBuffer", SupportedOn = AvailableOn.GLES)]
    public class T113_FramebufferWithBlur : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        GLRenderSurface _surface1;
        GLRenderSurface _surface2;
        GLBitmap _glbmp;
        bool _isInit;
        bool _frameBufferNeedUpdate;
        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            _surface1 = new GLRenderSurface(_pcx.ViewportWidth, _pcx.ViewportHeight);
            _frameBufferNeedUpdate = true;
            //------------ 
            _surface2 = new GLRenderSurface(_pcx.ViewportWidth, _pcx.ViewportHeight);
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
            _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
            if (!_isInit)
            {
                _glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.png");
                _isInit = true;
            }
            if (_surface1.IsValid)
            {
                if (_frameBufferNeedUpdate)
                {
                    //------------------------------------------------------------------------------------           
                    //framebuffer
                    _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                    _pcx.AttachToRenderSurface(_surface1);
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _pcx.Clear(PixelFarm.Drawing.Color.Red);
                    _pcx.DrawImageWithBlurX(_glbmp, 0, 0);
                    _pcx.AttachToRenderSurface(null);//switch to primary render surface
                    //------------------------------------------------------------------------------------  
                    //framebuffer2

                    _pcx.AttachToRenderSurface(_surface2);
                    _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;

                    //GLBitmap bmp2 = new GLBitmap(_frameBuffer.TextureId, _frameBuffer.Width, _frameBuffer.Height);
                    //bmp2.IsYFlipped = true;
                    //bmp2.IsBigEndianPixel = true;

                    _pcx.DrawImageWithBlurY(_surface1.GetGLBitmap(), 0, 0);
                    _pcx.AttachToRenderSurface(null);
                    //------------------------------------------------------------------------------------  
                    //after release current, we move back to default frame buffer again***
                    _frameBufferNeedUpdate = false;
                }
                _pcx.DrawImage(_surface2.GetGLBitmap(), 0, 0);

                //_pcx.DrawFrameBuffer(_frameBuffer2, 0, 0, true);
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

