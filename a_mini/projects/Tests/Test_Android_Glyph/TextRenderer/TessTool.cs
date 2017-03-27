//MIT, 2016-2017, WinterDev

using System.Collections.Generic;
using Tesselate;
namespace PixelFarm.DrawingGL
{
    struct TessTool
    {
        internal readonly Tesselate.Tesselator tess;
        internal readonly TessListener2 tessListener;
        public TessTool(Tesselate.Tesselator tess)
        {
            this.tess = tess;
            this.tessListener = new TessListener2();
            tessListener.Connect(tess, true);
        }
        public List<Vertex> TessPolygon(float[] vertex2dCoords)
        {
            int ncoords = vertex2dCoords.Length / 2;
            if (ncoords == 0) { return null; }
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
    }
}