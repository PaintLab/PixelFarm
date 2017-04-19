//MIT, 2017, WinterDev
using System;
using Poly2Tri;
namespace Typography.Rendering
{

    class GlyphTriangle 
    {
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
            TriangulationPoint p0 = _tri.P0;
            TriangulationPoint p1 = _tri.P1;
            TriangulationPoint p2 = _tri.P2;


            e0 = NewEdgeLine(p0, p1);
            e1 = NewEdgeLine(p1, p2);
            e2 = NewEdgeLine(p2, p0);

#if DEBUG
            e0.dbugOwner = e1.dbugOwner = e2.dbugOwner = this;
#endif

            tri.GetCentroid(out centroidX, out centroidY);

            e0.IsOutside = tri.EdgeIsConstrained(tri.FindEdgeIndex(tri.P0, tri.P1));
            e1.IsOutside = tri.EdgeIsConstrained(tri.FindEdgeIndex(tri.P1, tri.P2));
            e2.IsOutside = tri.EdgeIsConstrained(tri.FindEdgeIndex(tri.P2, tri.P0));
        }
        static EdgeLine NewEdgeLine(TriangulationPoint p, TriangulationPoint q)
        {
            return new EdgeLine(p.userData as GlyphPoint, q.userData as GlyphPoint);
        }
        public double CentroidX
        {
            get { return centroidX; }
        }
        public double CentroidY
        {
            get { return centroidY; }
        }
        public EdgeLine E0 { get { return e0; } }
        public EdgeLine E1 { get { return e1; } }
        public EdgeLine E2 { get { return e2; } }
        //
        internal bool IsConnectedWith(GlyphTriangle anotherTri)
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

#if DEBUG
        public override string ToString()
        {
            return "centroid:" + centroidX + "," + centroidY;
        }
#endif
    }


}