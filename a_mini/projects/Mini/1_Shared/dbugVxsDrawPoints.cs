//MIT 2014, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
namespace Mini
{
#if DEBUG
    public static class dbugVxsDrawPoints
    {
        public static void DrawVxsPoints(VertexStore vxs, Graphics2D g)
        {
            CanvasPainter p = new CanvasPainter(g);
            int j = vxs.Count;
            for (int i = 0; i < j; ++i)
            {
                double x, y;
                var cmd = vxs.GetVertex(i, out x, out y);
                switch (cmd)
                {
                    case VertexCmd.MoveTo:
                        {
                            p.FillColor = ColorRGBA.DarkGray;
                            p.FillRectLBWH(x, y, 5, 5);
                            p.DrawString(i.ToString(), x, y + 5);
                        }
                        break;
                    case VertexCmd.P2c:
                        {
                            p.FillColor = ColorRGBA.Red;
                            p.FillRectLBWH(x, y, 5, 5);
                            p.DrawString(i.ToString(), x, y + 5);
                        }
                        break;
                    case VertexCmd.P3c:
                        {
                            p.FillColor = ColorRGBA.Blue;
                            p.FillRectLBWH(x, y, 5, 5);
                            p.DrawString(i.ToString(), x, y + 5);
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            p.FillColor = ColorRGBA.Yellow;
                            p.FillRectLBWH(x, y, 5, 5);
                            p.DrawString(i.ToString(), x, y + 5);
                        }
                        break;
                }
            }
        }
    }

#endif
}