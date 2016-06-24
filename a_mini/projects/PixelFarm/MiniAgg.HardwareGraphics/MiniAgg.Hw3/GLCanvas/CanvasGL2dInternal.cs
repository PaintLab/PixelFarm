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
            //return vertices;
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
        List<Vertex> TessPolygon(float[] vertex2dCoords)
        {
            int ncoords = vertex2dCoords.Length / 2;
            List<Vertex> vertexts = new List<Vertex>(ncoords);
            int nn = 0;
            for (int i = 0; i < ncoords; ++i)
            {
                vertexts.Add(new Vertex(vertex2dCoords[nn++], vertex2dCoords[nn++]));
            }
            //-----------------------
            tessListener.Reset(vertexts);
            //-----------------------
            tess.BeginPolygon();
            tess.BeginContour();
            int j = vertexts.Count;
            for (int i = 0; i < j; ++i)
            {
                Vertex v = vertexts[i];
                tess.AddVertex(v.m_X, v.m_Y, 0, i);
            }
            tess.EndContour();
            tess.EndPolygon();
            return tessListener.resultVertexList;
        }

        unsafe void DrawPolygonUnsafe(float* polygon2dVertices, int npoints)
        {
            this.basicFillShader.DrawLineLoopWithVertexBuffer(polygon2dVertices, npoints, this.strokeColor);
        }
    }
}