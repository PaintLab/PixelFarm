//MIT, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
using PixelFarm.Drawing;
namespace Mini
{
#if DEBUG
    public static class dbugVxsDrawPoints
    {
        public static void DrawVxsPoints(VertexStore vxs, PixelFarm.Drawing.CanvasPainter p)
        {
            int j = vxs.Count;
            for (int i = 0; i < j; ++i)
            {
                double x, y;
                var cmd = vxs.GetVertex(i, out x, out y);
                switch (cmd)
                {
                    case VertexCmd.MoveTo:
                        {
                            p.FillColor = PixelFarm.Drawing.Color.Blue;
                            p.FillRectLBWH(x, y, 5, 5);
                            p.DrawString(i.ToString(), x, y + 5);
                        }
                        break;
                    case VertexCmd.P2c:
                        {
                            p.FillColor = PixelFarm.Drawing.Color.Red;
                            p.FillRectLBWH(x, y, 5, 5);
                            p.DrawString(i.ToString(), x, y + 5);
                        }
                        break;
                    case VertexCmd.P3c:
                        {
                            p.FillColor = PixelFarm.Drawing.Color.Gray;
                            p.FillRectLBWH(x, y, 5, 5);
                            p.DrawString(i.ToString(), x, y + 5);
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            p.FillColor = PixelFarm.Drawing.Color.Yellow;
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