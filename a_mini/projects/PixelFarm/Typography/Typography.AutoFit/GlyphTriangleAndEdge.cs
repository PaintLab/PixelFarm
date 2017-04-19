//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using Poly2Tri;
using System.Numerics;
namespace Typography.Rendering
{

    public class GlyphTriangle
    {
        DelaunayTriangle _tri;
        public EdgeLine e0;
        public EdgeLine e1;
        public EdgeLine e2;

        //centroid of edge mass
        double centroidX;
        double centroidY;

        public GlyphTriangle(DelaunayTriangle tri)
        {
            this._tri = tri;
            TriangulationPoint p0 = _tri.P0;
            TriangulationPoint p1 = _tri.P1;
            TriangulationPoint p2 = _tri.P2;
            e0 = new EdgeLine(this, p0, p1);
            e1 = new EdgeLine(this, p1, p2);
            e2 = new EdgeLine(this, p2, p0);
            tri.Centroid2(out centroidX, out centroidY);

            e0.IsOutside = tri.EdgeIsConstrained(tri.FindEdgeIndex(tri.P0, tri.P1));
            e1.IsOutside = tri.EdgeIsConstrained(tri.FindEdgeIndex(tri.P1, tri.P2));
            e2.IsOutside = tri.EdgeIsConstrained(tri.FindEdgeIndex(tri.P2, tri.P0));
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
            //else 
            return this._tri.N0 == t2 ||
                   this._tri.N1 == t2 ||
                   this._tri.N2 == t2;
        }

#if DEBUG
        public override string ToString()
        {
            return this._tri.ToString();
        }
#endif
    }

    public enum LineSlopeKind : byte
    {
        Vertical,
        Horizontal,
        Other
    }

    /// <summary>
    /// edge of GlyphTriangle
    /// </summary>
    public class EdgeLine
    {

        public double x0;
        public double y0;
        public double x1;
        public double y1;

        static readonly double _88degreeToRad = MyMath.DegreesToRadians(88);
        static readonly double _85degreeToRad = MyMath.DegreesToRadians(85);
        static readonly double _01degreeToRad = MyMath.DegreesToRadians(1);
        static readonly double _90degreeToRad = MyMath.DegreesToRadians(90);

        public TriangulationPoint p;
        public TriangulationPoint q;

        Dictionary<EdgeLine, bool> matchingEdges;

        //------------------------------
        /// <summary>
        /// contact to 
        /// </summary>
        public EdgeLine contactToEdge;
        //------------------------------

#if DEBUG
        public static int s_dbugTotalId;
        public readonly int dbugId = s_dbugTotalId++;
#endif
        public EdgeLine(GlyphTriangle owner, TriangulationPoint p, TriangulationPoint q)
        {
            this.OwnerTriangle = owner;
            this.p = p;
            this.q = q;

            x0 = p.X;
            y0 = p.Y;
            x1 = q.X;
            y1 = q.Y;
            //-------------------
            if (x1 == x0)
            {
                this.SlopeKind = LineSlopeKind.Vertical;
                SlopAngle = 1;
            }
            else
            {
                SlopAngle = Math.Abs(Math.Atan2(Math.Abs(y1 - y0), Math.Abs(x1 - x0)));
                if (SlopAngle > _85degreeToRad)
                {
                    SlopeKind = LineSlopeKind.Vertical;
                }
                else if (SlopAngle < _01degreeToRad)
                {
                    SlopeKind = LineSlopeKind.Horizontal;
                }
                else
                {
                    SlopeKind = LineSlopeKind.Other;
                }
            }
        }
        public GlyphTriangle OwnerTriangle { get; private set; }
        public LineSlopeKind SlopeKind
        {
            get;
            private set;
        }

        public bool IsOutside
        {
            get;
            internal set;
        }
        public bool IsInside
        {
            get { return !this.IsOutside; }

        }
        public double SlopAngle
        {
            get;
            private set;
        }
        public bool IsUpper
        {
            get;
            internal set;
        }
        public bool IsLeftSide
        {
            get;
            internal set;
        }
        public Vector2 GetMidPoint()
        {
            return new Vector2((float)((x0 + x1) / 2), (float)((y0 + y1) / 2));
        }
        public override string ToString()
        {
            return SlopeKind + ":" + x0 + "," + y0 + "," + x1 + "," + y1;
        }

        public EdgeLine GetMatchingOutsideEdge()
        {
            if (matchingEdges == null) { return null; }

            if (matchingEdges.Count == 1)
            {
                foreach (EdgeLine line in matchingEdges.Keys)
                {
                    return line;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public GlyphPoint GlyphPoint_P
        {
            get
            {
                return p.userData as GlyphPoint;
            }

        }
        public GlyphPoint GlyphPoint_Q
        {
            get
            {
                return q.userData as GlyphPoint;
            }
        }
        public void AddMatchingOutsideEdge(EdgeLine edgeLine)
        {
#if DEBUG
            if (edgeLine == this) { throw new NotSupportedException(); }
#endif
            if (matchingEdges == null)
            {
                matchingEdges = new Dictionary<EdgeLine, bool>();
            }
            if (!matchingEdges.ContainsKey(edgeLine))
            {
                matchingEdges.Add(edgeLine, true);
            }
#if DEBUG
            if (matchingEdges.Count > 1)
            {

            }
#endif
        } 
    } 
}