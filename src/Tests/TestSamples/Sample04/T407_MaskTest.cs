//MIT, 2014-present,WinterDev

using System;
using Mini;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.DrawingGL;


namespace OpenTkEssTest
{
    [Info(OrderCode = "407", AvailableOn = AvailableOn.GLES)]
    [Info("T407_MaskTest")]
    public class T407_MaskTest : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;

        //
        MemBitmap _colorBmp;
        MemBitmap _maskBmp;
        GLBitmap _colorGLBmp;
        GLBitmap _maskGLBmp;

        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            //1. create color bmp  
            _colorBmp = new MemBitmap(32, 32);
            using (AggPainterPool.Borrow(_colorBmp, out AggPainter painter))
            {
                painter.Clear(Color.White);
                painter.FillRect(2, 2, 15, 15, Color.Red);
            }
            //2. create mask bmp
            _maskBmp = new MemBitmap(32, 32);
            using (AggPainterPool.Borrow(_maskBmp, out AggPainter painter))
            {
                //white=> opaque
                //black => transparent
                painter.Clear(Color.Black);
                painter.FillCircle(10, 10, 4, Color.White);
            }
            _colorGLBmp = new GLBitmap(_colorBmp);
            _maskGLBmp = new GLBitmap(_maskBmp);

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
            _painter.FillColor = PixelFarm.Drawing.Color.Black;

            _pcx.DrawImageWithMask(_colorGLBmp, _maskGLBmp, 0, 0);

            //2. create mask img,

            SwapBuffers();
        }
    }
}

