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
            _surface1 = new GLRenderSurface(_pcx.ViewportWidth, _pcx.ViewportHeight);
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
            //unsafe
            //{   
            //    //test only!
            //    //copy from gl to MemBitmap
            //    using (PixelFarm.CpuBlit.MemBitmap outputBuffer = new PixelFarm.CpuBlit.MemBitmap(_pcx.ViewportWidth, _pcx.ViewportHeight))
            //    {
            //        _frameBuffer.CopyPixel(0, 0, _pcx.ViewportWidth, _pcx.ViewportHeight, PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(outputBuffer).Ptr);
            //        //then save ....
            //    } 
            //    //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(_pcx.ViewportWidth, _pcx.ViewportHeight);
            //    //var bmpdata = bmp.LockBits(
            //    //    new System.Drawing.Rectangle(0, 0, _pcx.ViewportWidth, _pcx.ViewportHeight),
            //    //    System.Drawing.Imaging.ImageLockMode.ReadWrite,
            //    //    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //    ////fixed (byte* outputPtr = &outputBuffer[0])
            //    ////{
            //    ////_frameBuffer.CopyPixel(0, _pcx.ViewportHeight - 100, 100, 100, (IntPtr)outputPtr);
            //    //_frameBuffer.CopyPixel(0, 0, _pcx.ViewportWidth, _pcx.ViewportHeight, bmpdata.Scan0);
            //    ////} 
            //    //bmp.UnlockBits(bmpdata);
            //    //bmp.Save("d:\\WImageTest\\outputfrom_framebuffer.png");
            //}
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
                    _pcx.DrawImage(_glbmp, 10, 10);
                    //------------------------------------------------------------------------------------  
                    _pcx.AttachToRenderSurface(null);
                    //after release current, we move back to default frame buffer again***
                    _frameBufferNeedUpdate = false;
                }
                _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                //_pcx.DrawFrameBuffer(_frameBuffer, 0, 0, true);
                _pcx.DrawImage(_surface1.GetGLBitmap(), 0, 0);
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

