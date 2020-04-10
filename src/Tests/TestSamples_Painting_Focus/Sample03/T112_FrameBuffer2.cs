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
        GLPainterCore _pcx;
        GLPainter _painter;
        GLRenderSurface _surface1;
        GLBitmap _glbmp;
        bool _isInit;
        bool _frameBufferNeedUpdate;
        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.Core;
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
                    outputBuffer.SaveImage("outputfrom_framebuffer.png");
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

            PixelFarm.Drawing.RenderSurfaceOriginKind prevOrgKind = _pcx.OriginKind; //save
            _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOriginKind.LeftTop;

            if (_surface1.IsValid)
            {
                if (_frameBufferNeedUpdate)
                {
                    _pcx.AttachToRenderSurface(_surface1);

                    _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOriginKind.LeftTop;
                    //------------------------------------------------------------------------------------  
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _pcx.Clear(PixelFarm.Drawing.Color.Black);
                    _pcx.DrawImage(_glbmp, 5, 5);
                    //------------------------------------------------------------------------------------  
                    _pcx.AttachToRenderSurface(null);
                    //after release current, we move back to default frame buffer again***
                    _frameBufferNeedUpdate = false;
                }
                _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOriginKind.LeftTop;
                //_pcx.DrawFrameBuffer(_frameBuffer, 0, 0, true);

                for (int i = 0; i < 1; ++i)
                {
                    _pcx.DrawImage(_surface1.GetGLBitmap(), 100 + (i * 30), 200 + (i * 30));
                }

            }
            else
            {
                _pcx.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            _pcx.OriginKind = prevOrgKind;//restore 
        }
    }


    [Info(OrderCode = "112_1")]
    [Info("T112_FrameBuffer_BGRA_to_RGBA", AvailableOn = AvailableOn.GLES)]
    public class T112_FrameBuffer_BGRA_To_RGBA : DemoBase
    {
        GLPainterCore _pcx;
        GLPainter _painter;
        GLBitmap _glbmp;
        bool _isInit;

        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.Core;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }


        GLBitmap ConvertFromBGRA_To_RGBA(GLBitmap bgraBmp)
        {

            GLBitmap rgba_result = new GLBitmap(bgraBmp.Width, bgraBmp.Height);//create another from blank with the same size 
            using (GLRenderSurface surface1 = new GLRenderSurface(bgraBmp.Width, bgraBmp.Height))
            using (GLRenderSurface surface2 = new GLRenderSurface(rgba_result, false))
            {
                _pcx.AttachToRenderSurface(surface1);
                _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOriginKind.LeftTop;
                //------------------------------------------------------------------------------------  
                //after make the frameBuffer current
                //then all drawing command will apply to frameBuffer
                //do draw to frame buffer here                                        
                _pcx.Clear(PixelFarm.Drawing.Color.Black);
                _pcx.DrawImage(bgraBmp, 0, 0);
                //------------------------------------------------------------------------------------  
                _pcx.AttachToRenderSurface(null);
                //after release current, we move back to default frame buffer again***  

                _pcx.AttachToRenderSurface(surface2);
                _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOriginKind.LeftTop;
                _pcx.Clear(PixelFarm.Drawing.Color.Black);
                _pcx.DrawImage(surface1.GetGLBitmap(), 0, 0);
                _pcx.AttachToRenderSurface(null);
            }
            return rgba_result;
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
                _glbmp = ConvertFromBGRA_To_RGBA(DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg"));
                _isInit = true;
            }

            PixelFarm.Drawing.RenderSurfaceOriginKind prevOrgKind = _pcx.OriginKind; //save
            _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOriginKind.LeftTop;

            _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOriginKind.LeftTop;
            //_pcx.DrawFrameBuffer(_frameBuffer, 0, 0, true); 
            for (int i = 0; i < 1; ++i)
            {
                _pcx.DrawImage(_glbmp, 100 + (i * 30), 200 + (i * 30));
            }

            //-------------------------------
            _pcx.OriginKind = prevOrgKind;//restore 
        }
    }
}

