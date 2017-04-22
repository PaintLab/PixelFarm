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


        GlyphCentroidLine ownerCentroidLine;

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

            e0 = NewEdgeLine(p0, p1, tri.EdgeIsConstrained(tri.FindEdgeIndex(p0, p1)));
            e1 = NewEdgeLine(p1, p2, tri.EdgeIsConstrained(tri.FindEdgeIndex(p1, p2)));
            e2 = NewEdgeLine(p2, p0, tri.EdgeIsConstrained(tri.FindEdgeIndex(p2, p0)));

#if DEBUG
            e0.dbugOwner = e1.dbugOwner = e2.dbugOwner = this;
#endif

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

        public GlyphCentroidLine OwnerCentroidLine
        {
            get
            {
                return ownerCentroidLine;
            }
            set
            {
#if DEBUG
                if (ownerCentroidLine != null)
                {
                }
#endif
                ownerCentroidLine = value;
            }

        }
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