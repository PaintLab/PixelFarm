//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;

namespace OpenTkEssTest
{
    [Info(OrderCode = "112")]
    [Info("T112_FrameBuffer", AvailableOn = AvailableOn.GLES)]
    public class T112_FrameBuffer : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        GLRenderSurface _surface1;
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
            //_surface1 = new GLRenderSurface(_pcx.ViewportWidth, _pcx.ViewportHeight);
            _surface1 = new GLRenderSurface(400, 200);
            _frameBufferNeedUpdate = true;
        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }

        [DemoAction]
        public void TestSaveCurrentFrameBuffer()
        {
            //attach spefic frame to the _pcx and make current before copy pixel

            //------------
            unsafe
            {
                //test only!
                //copy from gl to MemBitmap
                using (PixelFarm.CpuBlit.MemBitmap outputBuffer = new PixelFarm.CpuBlit.MemBitmap(_surface1.Width, _surface1.Height))
                {
                    _surface1.CopySurface(0, 0, _surface1.Width, _surface1.Height, outputBuffer);
                    //then save ....
                    //need to swap image buffer from opengl surface 
                    outputBuffer.SaveImage("d:\\WImageTest\\outputfrom_framebuffer.png");
                }
            }
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.Clear(PixelFarm.Drawing.Color.White); //set clear color and clear all buffer
            _pcx.ClearColorBuffer(); //test , clear only color buffer
            //-------------------------------
            if (!_isInit)
            {
                _glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
                _isInit = true;
            }

            PixelFarm.Drawing.RenderSurfaceOrientation prevOrgKind = _pcx.OriginKind; //save
            _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;

            if (_surface1.IsValid)
            {
                if (_frameBufferNeedUpdate)
                {
                    _pcx.AttachToRenderSurface(_surface1);

                    _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                    //------------------------------------------------------------------------------------  
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _pcx.Clear(PixelFarm.Drawing.Color.Black);
                    _pcx.DrawImage(_glbmp, 50, 50);
                    //------------------------------------------------------------------------------------  
                    _pcx.AttachToRenderSurface(null);
                    //after release current, we move back to default frame buffer again***
                    _frameBufferNeedUpdate = false;
                }
                _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                //_pcx.DrawFrameBuffer(_frameBuffer, 0, 0, true);
                _pcx.DrawImage(_surface1.GetGLBitmap(), 100, 200);
            }
            else
            {
                _pcx.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            _pcx.OriginKind = prevOrgKind;//restore 
        }
    }
}

