//MIT 2014-2016, WinterDev
using System;
using PixelFarm.Drawing;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.VertexSource;
namespace PixelFarm.DrawingGL
{
    public class GLCanvasPainter : GLCanvasPainterBase
    {
        public GLCanvasPainter(CanvasGL2d canvas, int w, int h)
            : base(canvas, w, h)
        {
        }
    }
}
