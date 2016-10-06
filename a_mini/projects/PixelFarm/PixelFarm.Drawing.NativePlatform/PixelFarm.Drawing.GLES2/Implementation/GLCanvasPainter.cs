//2016 MIT, WinterDev

using PixelFarm.Agg;
namespace PixelFarm.DrawingGL
{
    public class GLCanvasPainter : GLCanvasPainterBase
    {
        //WinGdiFontPrinter _winGdiPrinter;
        public GLCanvasPainter(CanvasGL2d canvas, int w, int h)
            : base(canvas, w, h)
        {
            // _win32GdiPrinter = new WinGdiFontPrinter(w, h);
        }
        // public override void DrawString(string text, double x, double y)
        //        {
        //            _win32GdiPrinter.DrawString(_canvas, text, (float)x, (float)y);
        //            //base.DrawString(text, x, y);
        //        }
    }
}