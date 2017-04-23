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

        public readonly double x0;
        public readonly double y0;
        public readonly double x1;
        public readonly double y1;


        //------------------------------
        /// <summary>
        /// contact to another edge
        /// </summary>
        internal EdgeLine contactToEdge;
        //------------------------------

        readonly GlyphPoint _glyphPoint_P;
        readonly GlyphPoint _glyphPoint_Q;
#if DEBUG
        public static int s_dbugTotalId;
        public readonly int dbugId = s_dbugTotalId++;
        internal GlyphTriangle dbugOwner;
#endif
        GlyphTriangle ownerTriangle;
        internal EdgeLine(GlyphTriangle ownerTriangle, GlyphPoint p, GlyphPoint q, bool isOutside)
        {
            this.ownerTriangle = ownerTriangle;
            //------------------------------------
            //an edge line connects 2 glyph points.
            //it is created from triangulation process.
            //
            //some edge line is either 'INSIDE' edge  OR 'OUTSIDE'.
            //
            //------------------------------------   
            this.IsOutside = isOutside;
            this._glyphPoint_P = p;
            this._glyphPoint_Q = q;

            x0 = p.x;
            y0 = p.y;
            x1 = q.x;
            y1 = q.y;
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
            //-----------------------
            if (isOutside)
            {
                p.SetRelatedEdgeLine(this);
                q.SetRelatedEdgeLine(this);
            }
            //-----------------------
        }
#if DEBUG
        public bool dbugNoPerpendicularBone { get; set; }
        public GlyphEdge dbugGlyphEdge { get; set; }
#endif
        internal GlyphTriangle OwnerTriangle { get { return this.ownerTriangle; } }
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

        internal double GetSlopeAngleNoDirection()
        {
            return Math.Abs(Math.Atan2(Math.Abs(y1 - y0), Math.Abs(x1 - x0)));
        }
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
        public Vector2 GetMidPoint()
        {
            return new Vector2((float)((x0 + x1) / 2), (float)((y0 + y1) / 2));
        }
        public override string ToString()
        {
            return SlopeKind + ":" + x0 + "," + y0 + "," + x1 + "," + y1;
        }
        GlyphBone _relBone;
        internal GlyphBone RelatedBone
        {
            get { return _relBone; }
            set
            {
                _relBone = value;
            }
        }


#if DEBUG
        public bool dbugHasRelatedBone { get { return this.RelatedBone != null; } }
#endif
        static readonly double _88degreeToRad = MyMath.DegreesToRadians(88);
        static readonly double _85degreeToRad = MyMath.DegreesToRadians(85);
        static readonly double _01degreeToRad = MyMath.DegreesToRadians(1);
        static readonly double _90degreeToRad = MyMath.DegreesToRadians(90);


    }
}