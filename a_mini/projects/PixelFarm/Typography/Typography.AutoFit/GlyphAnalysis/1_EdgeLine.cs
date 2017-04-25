//MIT, 2017, WinterDev
using System;
using System.Numerics;
namespace Typography.Rendering
{
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
        readonly GlyphPoint _glyphPoint_P;
        readonly GlyphPoint _glyphPoint_Q;
        /// <summary>
        /// contact to another edge
        /// </summary>
        internal EdgeLine contactToEdge;


        internal GlyphBoneJoint inside_joint;
#if DEBUG
        public static int s_dbugTotalId;
        public readonly int dbugId = s_dbugTotalId++;

#endif
        GlyphTriangle _ownerTriangle;
        internal EdgeLine(GlyphTriangle ownerTriangle, GlyphPoint p, GlyphPoint q, bool isOutside)
        {
            this._ownerTriangle = ownerTriangle;
            //------------------------------------
            //an edge line connects 2 glyph points.
            //it is created from triangulation process.
            //
            //some edge line is either 'INSIDE' edge  OR 'OUTSIDE'.
            //
            //------------------------------------   
            this._glyphPoint_P = p;
            this._glyphPoint_Q = q;
            this.IsOutside = isOutside;
            if (isOutside)
            {
                p.SetOutsideEdge(this);
                q.SetOutsideEdge(this);
            }
            //-------------------------------
            //analyze angle and slope kind
            //-------------------------------  
            SlopeAngleNoDirection = this.GetSlopeAngleNoDirection();
            if (x1 == x0)
            {
                this.SlopeKind = LineSlopeKind.Vertical;
            }
            else
            {

                if (SlopeAngleNoDirection > _85degreeToRad)
                {
                    SlopeKind = LineSlopeKind.Vertical;
                }
                else if (SlopeAngleNoDirection < _01degreeToRad)
                {
                    SlopeKind = LineSlopeKind.Horizontal;
                }
                else
                {
                    SlopeKind = LineSlopeKind.Other;
                }
            }
        }

        public double x0 { get { return this._glyphPoint_P.x; } }
        public double y0 { get { return this._glyphPoint_P.y; } }
        public double x1 { get { return this._glyphPoint_Q.x; } }
        public double y1 { get { return this._glyphPoint_Q.y; } }


#if DEBUG
        public bool dbugNoPerpendicularBone { get; set; }
        public GlyphEdge dbugGlyphEdge { get; set; }
#endif

        public GlyphPoint GlyphPoint_P
        {
            get
            {
                return _glyphPoint_P;
            }
        }
        public GlyphPoint GlyphPoint_Q
        {
            get
            {
                return _glyphPoint_Q;
            }
        }
        public LineSlopeKind SlopeKind
        {
            get;
            private set;
        }

        internal GlyphTriangle OwnerTriangle { get { return this._ownerTriangle; } }

        public bool IsOutside
        {
            get;
            private set;
        }
        public bool IsInside
        {
            get { return !this.IsOutside; }

        }
        internal double SlopeAngleNoDirection
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

        public override string ToString()
        {
            return SlopeKind + ":" + x0 + "," + y0 + "," + x1 + "," + y1;
        }

        static readonly double _88degreeToRad = MyMath.DegreesToRadians(88);
        static readonly double _85degreeToRad = MyMath.DegreesToRadians(85);
        static readonly double _01degreeToRad = MyMath.DegreesToRadians(1);
        static readonly double _90degreeToRad = MyMath.DegreesToRadians(90);
    }


    public static class EdgeLineExtensions
    {
        public static Vector2 GetMidPoint(this EdgeLine line)
        {
            return new Vector2((float)((line.x0 + line.x1) / 2), (float)((line.y0 + line.y1) / 2));
        }

        internal static double GetSlopeAngleNoDirection(this EdgeLine line)
        {
            return Math.Abs(Math.Atan2(Math.Abs(line.y1 - line.y0), Math.Abs(line.x1 - line.x0)));
        }

        internal static bool ContainsTriangle(this EdgeLine edge, GlyphTriangle p)
        {
            return (p.e0 == edge ||
                    p.e1 == edge ||
                    p.e2 == edge);
        }

    }
}