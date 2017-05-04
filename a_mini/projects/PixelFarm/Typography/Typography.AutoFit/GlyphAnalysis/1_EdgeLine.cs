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


        //---
        EdgeLine _ctrlEdge_P;
        EdgeLine _ctrlEdge_Q;
        internal float _newFitX;
        internal float _newFitY;
        public bool _hasNewFitValues;
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

        public EdgeLine ControlEdge_P
        {
            get { return _ctrlEdge_P; }
        }
        public EdgeLine ControlEdge_Q
        {
            get { return _ctrlEdge_Q; }
        }


        internal EdgeLine GetControlEdgeThatContains(GlyphPoint p)
        {
            if (_ctrlEdge_P != null && _ctrlEdge_P.ContainsGlyphPoint(p))
            {
                return _ctrlEdge_P;
            }
            if (_ctrlEdge_Q != null && _ctrlEdge_Q.ContainsGlyphPoint(p))
            {
                return _ctrlEdge_Q;
            }
            return null; //not found 
        }
        //--- 
        //public Vector2 _ctrlEdge_P_cutAt { get; private set; }
        //public Vector2 _ctrlEdge_Q_cutAt { get; private set; }

        //public float _ctrlEdge_P_cutLen { get; private set; }
        //public float _ctrlEdge_Q_cutLen { get; private set; }

        EdgeLine _outsideEdge;
        Vector2 _outsideEdgeCutAt;
        float _outsideEdgeCutLen;
        internal void SetOutsideEdge(EdgeLine outsideEdge, Vector2 cutPoint, float cutLen)
        {
#if DEBUG
            if (outsideEdge == this) { throw new NotSupportedException(); }
#endif
            _outsideEdge = outsideEdge;
            _outsideEdgeCutAt = cutPoint;
            _outsideEdgeCutLen = cutLen;
        }
        internal void SetControlEdge(EdgeLine controlEdge)
        {
            //check if edge is connect to p or q

#if DEBUG
            if (!controlEdge.IsInside)
            {

            }
#endif
            //----------------
            if (_glyphPoint_P == controlEdge._glyphPoint_P)
            {
#if DEBUG
                if (_ctrlEdge_P != null && _ctrlEdge_P != controlEdge)
                {
                }
#endif
                //map this p to p of the control edge
                _ctrlEdge_P = controlEdge;

            }
            else if (_glyphPoint_P == controlEdge.GlyphPoint_Q)
            {
#if DEBUG
                if (_ctrlEdge_P != null && _ctrlEdge_P != controlEdge)
                {
                }
#endif
                _ctrlEdge_P = controlEdge;
                //_ctrlEdge_P_cutAt = cutPoint;
                //_ctrlEdge_P_cutLen = (float)cutLen; //TODO: review float or double
            }
            else if (_glyphPoint_Q == controlEdge._glyphPoint_P)
            {
#if DEBUG
                if (_ctrlEdge_Q != null && _ctrlEdge_Q != controlEdge)
                {
                }
#endif
                _ctrlEdge_Q = controlEdge;
                //_ctrlEdge_Q_cutAt = cutPoint;
                //_ctrlEdge_Q_cutLen = (float)cutLen; //TODO: review float or double
            }
            else if (_glyphPoint_Q == controlEdge.GlyphPoint_Q)
            {
#if DEBUG
                if (_ctrlEdge_Q != null && _ctrlEdge_Q != controlEdge)
                {
                }
#endif
                _ctrlEdge_Q = controlEdge;
                //_ctrlEdge_Q_cutAt = cutPoint;
                //_ctrlEdge_Q_cutLen = (float)cutLen; //TODO: review float or double
            }
            else
            {
                //?
            }
        }
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

        public Vector2 GetFitPos() { return new Vector2(_newFitX, _newFitY); }

        static readonly double _85degreeToRad = MyMath.DegreesToRadians(85);
        static readonly double _01degreeToRad = MyMath.DegreesToRadians(1);
        static readonly double _90degreeToRad = MyMath.DegreesToRadians(90);
        internal bool _earlyInsideAnalysis;
        internal bool ContainsGlyphPoint(GlyphPoint p)
        {
            return this._glyphPoint_P == p || this._glyphPoint_Q == p;
        }
    }


    public static class EdgeLineExtensions
    {
        public static Vector2 GetMidPoint(this EdgeLine line)
        {
            return new Vector2((float)((line.x0 + line.x1) / 2), (float)((line.y0 + line.y1) / 2));
        }
        //public static Vector2 GetNewEdgeOutsideCutPoint(this EdgeLine line)
        //{
        //    return new Vector2(line._ctrlEdge_P_cutAt.X, line._ctrlEdge_Q_cutAt.Y + line.GetVerticalFitDiff());

        //}
        internal static double GetSlopeAngleNoDirection(this EdgeLine line)
        {
            return Math.Abs(Math.Atan2(Math.Abs(line.y1 - line.y0), Math.Abs(line.x1 - line.x0)));
        }
        internal static float GetVerticalFitDiff(this EdgeLine line)
        {
            return line.GetMidPoint().Y - line._newFitY;
        }
        internal static bool ContainsTriangle(this EdgeLine edge, GlyphTriangle p)
        {
            return (p.e0 == edge ||
                    p.e1 == edge ||
                    p.e2 == edge);
        }

    }
}