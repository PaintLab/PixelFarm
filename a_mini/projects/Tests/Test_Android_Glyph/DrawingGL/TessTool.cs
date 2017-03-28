//MIT, 2014-2017, WinterDev  

using System.Collections.Generic;
using PixelFarm.DrawingGL;
using Tesselate;

namespace DrawingGL
{
    class TessTool
    {
        internal readonly Tesselate.Tesselator tess;
        internal readonly TessListener2 tessListener;
        List<Vertex> vertexts = new List<Vertex>();
        public TessTool() : this(new Tesselator() { WindingRule = Tesselator.WindingRuleType.Odd }) { }
        public TessTool(Tesselate.Tesselator tess)
        {
            this.tess = tess;
            this.tessListener = new TessListener2();
            tessListener.Connect(tess, true);
        }
        public float[] TessPolygon(float[] vertex2dCoords, int[] contourEndPoints, out int areaCount)
        {
            vertexts.Clear();//reset
            //
            int ncoords = vertex2dCoords.Length / 2;
            if (ncoords == 0) { areaCount = 0; return null; }

            int nn = 0;
            for (int i = 0; i < ncoords; ++i)
            {
                vertexts.Add(new Vertex(vertex2dCoords[nn++], vertex2dCoords[nn++]));
            }
            //-----------------------
            tessListener.Reset(vertexts);
            //-----------------------
            tess.BeginPolygon();

            int nContourCount = contourEndPoints.Length;
            int beginAt = 0;
            for (int m = 0; m < nContourCount; ++m)
            {
                int thisContourEndAt = (contourEndPoints[m] + 1) / 2;
                tess.BeginContour();
                for (int i = beginAt; i < thisContourEndAt; ++i)
                {
                    Vertex v = vertexts[i];
                    tess.AddVertex(v.m_X, v.m_Y, 0, i);
                }
                beginAt = thisContourEndAt + 1;
                tess.EndContour();
            }


            tess.EndPolygon();
            //-----------------------
            List<Vertex> vertextList = tessListener.resultVertexList;
            //-----------------------------   
            //switch how to fill polygon
            int j = vertextList.Count;
            float[] vtx = new float[j * 2];
            int n = 0;
            for (int p = 0; p < j; ++p)
            {
                var v = vertextList[p];
                vtx[n] = (float)v.m_X;
                vtx[n + 1] = (float)v.m_Y;
                n += 2;
            }
            //triangle list
            areaCount = j;
            return vtx;
        }
    }
}