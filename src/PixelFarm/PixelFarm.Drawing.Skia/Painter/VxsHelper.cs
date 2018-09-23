//MIT, 2016-present, WinterDev

using System;
using SkiaSharp;
using PixelFarm.CpuBlit;
namespace PixelFarm.Drawing.Skia
{
    static class VxsHelper
    {
        /// <summary>
        /// we do NOT VXS
        /// </summary>
        /// <param name="vxs"></param>
        /// <returns></returns>
        public static SKPath CreateGraphicsPath(VertexStore vxs)
        {
            //render vertice in store
            int vcount = vxs.Count;
            double prevX = 0;
            double prevY = 0;
            double prevMoveToX = 0;
            double prevMoveToY = 0;
            //var brush_path = new System.Drawing.Drawing2D.GraphicsPath(FillMode.Winding);//*** winding for overlapped path
            var brushPath = new SKPath();
            //how to set widening mode 
            for (int i = 0; i < vcount; ++i)
            {
                double x, y;
                PixelFarm.CpuBlit.VertexCmd cmd = vxs.GetVertex(i, out x, out y);
                switch (cmd)
                {
                    case PixelFarm.CpuBlit.VertexCmd.MoveTo:
                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        //brush_path.StartFigure();
                        brushPath.MoveTo((float)x, (float)y);
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.LineTo:
                        //brush_path.AddLine((float)prevX, (float)prevY, (float)x, (float)y);
                        brushPath.LineTo((float)x, (float)y);
                        prevX = x;
                        prevY = y;
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.Close:
                    case PixelFarm.CpuBlit.VertexCmd.CloseAndEndFigure:
                        brushPath.LineTo((float)prevMoveToX, (float)prevMoveToY);
                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        brushPath.Close();
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.NoMore:
                        i = vcount + 1;//exit from loop
                        goto EXIT_LOOP;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            EXIT_LOOP:
            return brushPath;
        }


        public static void FillVxsSnap(SKCanvas g, VertexStore vxs, SKPaint fill)
        {
            using (var p = CreateGraphicsPath(vxs))
            {
                g.DrawPath(p, fill);
            }
        }
        public static void DrawVxsSnap(SKCanvas g, VertexStore vxs, SKPaint stroke)
        {
            using (var p = CreateGraphicsPath(vxs))
            {
                g.DrawPath(p, stroke);
            }
        }
        public static void FillPath(SKCanvas g, SKPath p, SKPaint fill)
        {
            g.DrawPath(p, fill);
        }
        public static void DrawPath(SKCanvas g, SKPath p, SKPaint stroke)
        {
            g.DrawPath(p, stroke);
        }

    }
}