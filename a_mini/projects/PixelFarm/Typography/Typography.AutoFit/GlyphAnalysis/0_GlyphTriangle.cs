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
                    //1 outside edge
                    //2 inside edges
                    //find a perpendicular line
                    FindPerpendicular_1(d2, d0, d1);
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
                    //2 inside edges
                    FindPerpendicular_1(d1, d0, d2);
                }
            }
        }
        void FindPerpendicular_1(EdgeLine outsideEdge, EdgeLine inside0, EdgeLine inside1)
        {
            System.Numerics.Vector2 m0 = inside0.GetMidPoint();
            System.Numerics.Vector2 cut_fromM0;
            bool foundOnePerpendicularLine = false;
            //
            if (MyMath.FindPerpendicularCutPoint(outsideEdge, new System.Numerics.Vector2(m0.X, m0.Y), out cut_fromM0))
            {
                foundOnePerpendicularLine = true;
                outsideEdge._controlE0 = inside0;
                outsideEdge._controlE0_cutAt = cut_fromM0;
            }
            else
            {

            }
            //------
            System.Numerics.Vector2 m1 = inside1.GetMidPoint();
            System.Numerics.Vector2 cut_fromM1;
            if (MyMath.FindPerpendicularCutPoint(outsideEdge, new System.Numerics.Vector2(m1.X, m1.Y), out cut_fromM1))
            {
                foundOnePerpendicularLine = true;
                outsideEdge._controlE1 = inside1;
                outsideEdge._controlE1_cutAt = cut_fromM1;
            }
            else
            {

            }
            //------
            if (!foundOnePerpendicularLine)
            {

            }

            //all edges are analyzed
            outsideEdge._earlyInsideAnalysis =
                inside0._earlyInsideAnalysis =
                inside1._earlyInsideAnalysis = true;

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