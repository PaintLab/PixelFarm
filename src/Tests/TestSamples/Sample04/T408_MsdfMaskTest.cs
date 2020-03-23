//MIT, 2014-present,WinterDev

using System;
using Mini;

using PixelFarm.Drawing;
using PixelFarm.DrawingGL;

using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;
using PixelFarm.CpuBlit.VertexProcessing;

namespace OpenTkEssTest
{
    [Info(OrderCode = "408", AvailableOn = AvailableOn.GLES)]
    [Info("T408_MsdfMaskTest")]
    public class T408_MsdfMaskTest : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;

        //
        MemBitmap _colorBmp;
        MemBitmap _msdfMaskBmp;
        GLBitmap _colorGLBmp;
        GLBitmap _msdfMaskGLBmp;

        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            //1. create color bmp  
            _colorBmp = new MemBitmap(28, 23);
            using (AggPainterPool.Borrow(_colorBmp, out AggPainter painter))
            {
                painter.Clear(Color.White);
                painter.FillRect(2, 2, 30, 30, Color.Red);
            }
            //2. create mask bmp

            using (VectorToolBox.Borrow(out Ellipse ellipse))
            using (VxsTemp.Borrow(out var v1, out var v2))
            {
                var msdf = new Msdfgen.MsdfGen3();
                 
                v1.AddMoveTo(0, 0);
                v1.AddLineTo(20, 0);
                v1.AddLineTo(10, 15);
                v1.AddCloseFigure(); 

                v1.ScaleToNewVxs(1, -1, v2); //flipY

                //msdf.MsdfGenParams = new Msdfgen.MsdfGenParams() { UseCustomImageSize = true, CustomWidth = 64, CustomHeight = 64 };
                BitmapAtlasItemSource itemSrc = msdf.GenerateMsdfTexture(v2);
                _msdfMaskBmp = MemBitmap.CreateFromCopy(itemSrc.Width, itemSrc.Height, itemSrc.Source);
#if DEBUG
                //_msdfMaskBmp.SaveImage("mask_msdf.png");
#endif
            }
            _colorGLBmp = new GLBitmap(_colorBmp);
            _msdfMaskGLBmp = new GLBitmap(_msdfMaskBmp);

        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {

            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();


            _pcx.DrawImageWithMsdfMask(_colorGLBmp, _msdfMaskGLBmp, 0, 0);


            SwapBuffers();
        }
    }
}

