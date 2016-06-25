//MIT 2014-2016, WinterDev

using System;
using System.Collections.Generic;
namespace PixelFarm.DrawingGL
{
    public partial class CanvasGL2d
    {
        static float[] CreateRectTessCoordsTriStrip(float x, float y, float w, float h)
        {
            //float x0 = x;
            //float y0 = y + h;
            //float x1 = x;
            //float y1 = y;
            //float x2 = x + w;
            //float y2 = y + h;
            //float x3 = x + w;
            //float y3 = y;
            return new float[]{
               x,y + h,
               x,y,
               x + w, y + h,
               x + w, y,
            };
        }
        static float[] CreatePolyLineRectCoords2(
                float x, float y, float w, float h)
        {
            return new float[]
            {
                x,y,
                x+w,y,
                x+w,y+h,
                x,x+h
            };
        }

        unsafe void DrawPolygonUnsafe(float* polygon2dVertices, int npoints)
        {
            this.basicFillShader.DrawLineLoopWithVertexBuffer(polygon2dVertices, npoints, this.strokeColor);
        }
    }
}