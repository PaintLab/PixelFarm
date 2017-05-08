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



        public GlyphTriangle(DelaunayTriangle tri)
        {
            this._tri = tri;
            //---------------------------------------------
            TriangulationPoint p0 = _tri.P0;
            TriangulationPoint p1 = _tri.P1;
            TriangulationPoint p2 = _tri.P2;

            //we do not store triangulation points (p0,p1,02)
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
            //----------------

            //early analyze
            AnalyzeInsideEdge(e0, e1, e2);
            AnalyzeInsideEdge(e1, e0, e2);
            AnalyzeInsideEdge(e2, e0, e1);
            //at this point, 
            //we should know the direction of this triangle
            //then we known that if this triangle is left/right/upper/lower of the 'stroke' line
        }
        void AnalyzeInsideEdge(EdgeLine d0, EdgeLine d1, EdgeLine d2)
        {
            if (d0._earlyInsideAnalysis) return;
            if (!d0.IsInside) return;
            //-------------------------------------------------
            //maybeInsideEdge is Inside ***
            //check another
            if (d1.IsInside)
            {
                if (d2.IsInside)
                {
                    //3 inside edges

                }
                else
                {
                    //1 outside edge (d2)


                    //2 inside edges (d0,d1)
                    //find a perpendicular line
                    FindPerpendicular(d2, d0);
                    FindPerpendicular(d2, d1);

                }
            }
            else if (d2.IsInside)
            {
                if (d1.IsInside)
                {
                    //3 inside edges

                }
                else
                {
                    //1 outside edge (d1)
                    //2 inside edges (d0,d2)
                    FindPerpendicular(d1, d0);
                    FindPerpendicular(d1, d2);

                }
            }
        }
        void FindPerpendicular(EdgeLine outsideEdge, EdgeLine inside)
        {
            System.Numerics.Vector2 m0 = inside.GetMidPoint();
            System.Numerics.Vector2 cut_fromM0;
            if (MyMath.FindPerpendicularCutPoint(outsideEdge, new System.Numerics.Vector2(m0.X, m0.Y), out cut_fromM0))
            {
                ((OutsideEdgeLine)outsideEdge).SetControlEdge(inside);
            }
            else
            {

            }
            outsideEdge._earlyInsideAnalysis = inside._earlyInsideAnalysis = true;

        }
        EdgeLine NewEdgeLine(TriangulationPoint p, TriangulationPoint q, bool isOutside)
        {
            return isOutside ?
                (EdgeLine)(new OutsideEdgeLine(this, p.userData as GlyphPoint, q.userData as GlyphPoint)) :
                new InsideEdgeLine(this, p.userData as GlyphPoint, q.userData as GlyphPoint);
        }

        public void CalculateCentroid(out float centroidX, out float centroidY)
        {
            _tri.GetCentroid(out centroidX, out centroidY);
        }
        public bool IsConnectedTo(GlyphTriangle anotherTri)
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
 
    }


    static class GlyphTriangleExtensions
    {

        static EdgeLine GetFirstFoundOutsidEdge(GlyphTriangle tri)
        {
            if (tri.e0.IsOutside) { return tri.e0; }
            if (tri.e1.IsOutside) { return tri.e1; }
            if (tri.e2.IsOutside) { return tri.e2; }
            return null; //not found               
        }
        internal static EdgeLine FindOppositeEdge(this GlyphTriangle tri, EdgeLine testEdge)
        {
            //find opposite edge to this testEdge
            //1. opposite side
            //2. 
            EdgeLine f_e0, f_e1, f_e2;
            switch (tri.GetOutsideEdgeLine(out f_e0, out f_e1, out f_e2))
            {
                default: throw new NotSupportedException();
                case 0:
                    return null;
                case 1:

                    if (f_e0.SlopeKind == testEdge.SlopeKind)
                    {
                        return f_e0;
                    }
                    return null;
                case 2:

                    //2 outside
                    if (f_e0.SlopeKind == testEdge.SlopeKind)
                    {
                        return f_e0;
                    }
                    if (f_e1.SlopeKind == testEdge.SlopeKind)
                    {
                        return f_e1;
                    }
                    return null;

                case 3:

                    //not possible! 
                    throw new NotSupportedException();

            }
        }
        internal static int GetOutsideEdgeLine(this GlyphTriangle tri,
            out EdgeLine foundE0,
            out EdgeLine foundE1,
            out EdgeLine foundE2)
        {
            foundE0 = foundE1 = foundE2 = null;
            int outsideEdgeCount = 0;
            if (tri.e0 != null && tri.e0.IsOutside)
            {
                switch (outsideEdgeCount)
                {
                    case 0:
                        foundE0 = tri.e0;
                        break;
                    case 1:
                        foundE1 = tri.e0;
                        break;
                    case 2:
                        foundE2 = tri.e0;
                        break;
                }
                outsideEdgeCount++;
            }
            //---------------------------------
            if (tri.e1 != null && tri.e1.IsOutside)
            {
                switch (outsideEdgeCount)
                {
                    case 0:
                        foundE0 = tri.e1;
                        break;
                    case 1:
                        foundE1 = tri.e1;
                        break;
                    case 2:
                        foundE2 = tri.e1;
                        break;
                }
                outsideEdgeCount++;
            }
            //---------------------------------
            if (tri.e2 != null && tri.e2.IsOutside)
            {
                switch (outsideEdgeCount)
                {
                    case 0:
                        foundE0 = tri.e2;
                        break;
                    case 1:
                        foundE1 = tri.e2;
                        break;
                    case 2:
                        foundE2 = tri.e2;
                        break;
                }
                outsideEdgeCount++;
            }
            return outsideEdgeCount;
        }

    }

}