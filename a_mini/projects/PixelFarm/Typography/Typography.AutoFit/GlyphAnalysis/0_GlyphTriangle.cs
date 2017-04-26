//MIT, 2017, WinterDev
using System;
using Poly2Tri;
namespace Typography.Rendering
{

    class GlyphTriangle
    {

        //TODO: review here...
        //if we not use the 
        DelaunayTriangle _tri;
        public readonly EdgeLine e0;
        public readonly EdgeLine e1;
        public readonly EdgeLine e2;

        //centroid of edge mass
        float centroidX;
        float centroidY;
        public GlyphTriangle(DelaunayTriangle tri)
        {
            this._tri = tri;
            tri.GetCentroid(out centroidX, out centroidY);

            //---------------------------------------------
            TriangulationPoint p0 = _tri.P0;
            TriangulationPoint p1 = _tri.P1;
            TriangulationPoint p2 = _tri.P2;



            //we do not store triangulation point
            //an EdgeLine is created after we create GlyphTriangles.

            //triangulate point p0->p1->p2 is CCW ***             
            e0 = NewEdgeLine(p0, p1, tri.EdgeIsConstrained(tri.FindEdgeIndex(p0, p1)));
            e1 = NewEdgeLine(p1, p2, tri.EdgeIsConstrained(tri.FindEdgeIndex(p1, p2)));
            e2 = NewEdgeLine(p2, p0, tri.EdgeIsConstrained(tri.FindEdgeIndex(p2, p0)));

            //if the order of original glyph point is CW
            //we may want to reverse the order of edge creation :
            //p2->p1->p0 
  

            //link back 
            tri.userData = this;


        }
        EdgeLine NewEdgeLine(TriangulationPoint p, TriangulationPoint q, bool isOutside)
        {
            return new EdgeLine(this, p.userData as GlyphPoint, q.userData as GlyphPoint, isOutside);
        }
        public double CentroidX
        {
            get { return centroidX; }
        }
        public double CentroidY
        {
            get { return centroidY; }
        }


        public bool IsConnectedWith(GlyphTriangle anotherTri)
        {
            DelaunayTriangle t2 = anotherTri._tri;
            if (t2 == this._tri)
            {
                throw new NotSupportedException();
            }
            //compare each neighbor 
            return this._tri.N0 == t2 ||
                   this._tri.N1 == t2 ||
                   this._tri.N2 == t2;
        }
        public GlyphTriangle N0
        {
            get
            {
                return GetGlyphTriFromUserData(_tri.N0);
            }
        }
        public GlyphTriangle N1
        {
            get
            {
                return GetGlyphTriFromUserData(_tri.N1);
            }
        }
        public GlyphTriangle N2
        {
            get
            {
                return GetGlyphTriFromUserData(_tri.N2);
            }
        }
        static GlyphTriangle GetGlyphTriFromUserData(DelaunayTriangle tri)
        {
            if (tri == null) return null;
            return tri.userData as GlyphTriangle;
        }
#if DEBUG
        public override string ToString()
        {
            return "centroid:" + centroidX + "," + centroidY;
        }
#endif
    }


}