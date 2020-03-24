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
            _colorBmp = new MemBitmap(64, 64);
            using (VxsTemp.Borrow(out var v1, out var v2))
            using (Tools.BorrowAggPainter(_colorBmp, out var painter))
            {
                painter.Clear(Color.White);
                //painter.FillRect(2, 2, 15, 15, Color.Red);

                //v1.AddMoveTo(0, 0);
                //v1.AddLineTo(20, 0);
                //v1.AddLineTo(10, 15);
                //v1.AddCloseFigure();


                v1.AddMoveTo(0, 32);
                v1.AddLineTo(20, 32);
                v1.AddLineTo(10, 5);
                v1.AddCloseFigure();

                //v1.ScaleToNewVxs(1, -1, v2);

                painter.Fill(v1, Color.Red);

                //_colorBmp.SaveImage("test_color.png");
            }
            //2. create mask bmp
            _maskBmp = new MemBitmap(32, 32);
            using (Tools.BorrowAggPainter(_maskBmp, out var painter))
            {
                //white=> opaque
                //black => transparent
                painter.Clear(Color.Black);
                painter.FillCircle(10, 10, 10, Color.White);
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
             
            _painter.Clear(Color.Blue);

            PixelFarm.Drawing.RectangleF maskSrc = new RectangleF(0, 0, _maskBmp.Width, _maskBmp.Height);
            _pcx.DrawImageWithMask(_maskGLBmp, _colorGLBmp, maskSrc, 5, 3, 0, 0);
            _pcx.DrawImageWithMask(_maskGLBmp, _colorGLBmp, maskSrc, 5, 3, 30, 30);

            SwapBuffers();
        }
    }
}

