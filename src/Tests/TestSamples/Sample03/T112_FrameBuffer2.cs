//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "112")]
    [Info("T112_FrameBuffer", SupportedOn = AvailableOn.GLES)]
    public class T112_FrameBuffer : DemoBase
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

            _frameBuffer = _glsx.CreateFramebuffer(_glsx.CanvasWidth, _glsx.CanvasHeight);
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
            _glsx.Clear(PixelFarm.Drawing.Color.White); //set clear color and clear all buffer
            _glsx.ClearColorBuffer(); //test , clear only color buffer
            //-------------------------------
            if (!_isInit)
            {
                _glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
                _isInit = true;
            }

            PixelFarm.Drawing.RenderSurfaceOrientation prevOrgKind = _glsx.OriginKind; //save
            _glsx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;

            if (_frameBuffer.FrameBufferId > 0)
            {
                if (_frameBufferNeedUpdate)
                {

                    _glsx.AttachFramebuffer(_frameBuffer);
                    //------------------------------------------------------------------------------------  
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _glsx.Clear(PixelFarm.Drawing.Color.Black);
                    _glsx.DrawImage(_glbmp, 0, 0);
                    //------------------------------------------------------------------------------------  
                    _glsx.DetachFramebuffer();
                    //after release current, we move back to default frame buffer again***
                    _frameBufferNeedUpdate = false;
                }
                _glsx.DrawFrameBuffer(_frameBuffer, 0, 0);
            }
            else
            {
                _glsx.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            _glsx.OriginKind = prevOrgKind;//restore



        }
    }
}

