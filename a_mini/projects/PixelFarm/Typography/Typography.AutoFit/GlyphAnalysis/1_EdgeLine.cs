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

        EdgeLine _ctrlEdge_P;
        EdgeLine _ctrlEdge_Q;
        internal float _newFitX;
        internal float _newFitY;

        internal Vector2 _newDynamicMidPoint;

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
            if (this.IsOutside = isOutside)
            {
                //set back
                p.SetOutsideEdge(this);
                q.SetOutsideEdge(this);
            }
            _newDynamicMidPoint = new Vector2((p.x + q.x) / 2, (p.y + q.y) / 2);
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
        public bool IsTip { get; internal set; }

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

        internal Vector2 GetEdgeVector()
        {
            return new Vector2(
                GlyphPoint_Q.x - _glyphPoint_P.x,
                GlyphPoint_Q.y - _glyphPoint_P.y);
        }
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
            }
            else if (_glyphPoint_Q == controlEdge._glyphPoint_P)
            {
#if DEBUG
                if (_ctrlEdge_Q != null && _ctrlEdge_Q != controlEdge)
                {
                }
#endif
                _ctrlEdge_Q = controlEdge;
            }
            else if (_glyphPoint_Q == controlEdge.GlyphPoint_Q)
            {
#if DEBUG
                if (_ctrlEdge_Q != null && _ctrlEdge_Q != controlEdge)
                {
                }
#endif
                _ctrlEdge_Q = controlEdge;
            }
            else
            {
                //?
            }
        }
#if DEBUG
        public bool dbugNoPerpendicularBone { get; set; }

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
        internal double SlopeAngleNoDirection
        {
            get;
            private set;
        }


        public override string ToString()
        {
            return SlopeKind + ":" + x0 + "," + y0 + "," + x1 + "," + y1;
        }



        static readonly double _85degreeToRad = MyMath.DegreesToRadians(85);
        static readonly double _01degreeToRad = MyMath.DegreesToRadians(1);
        static readonly double _90degreeToRad = MyMath.DegreesToRadians(90);
        internal bool _earlyInsideAnalysis;
        internal bool ContainsGlyphPoint(GlyphPoint p)
        {
            return this._glyphPoint_P == p || this._glyphPoint_Q == p;
        }

        internal void SetDynamicEdgeOffsetFromMasterOutline(float newEdgeOffsetFromMasterOutline)
        {

            //TODO: refactor here...
            //this is relative len from current edge              
            //origianl vector
            Vector2 _o_edgeVector = GetEdgeVector();
            //rotate 90
            Vector2 _rotate = _o_edgeVector.Rotate(90);
            //
            Vector2 _deltaVector = _rotate.NewLength(newEdgeOffsetFromMasterOutline);

            //new dynamic mid point  
            this._newDynamicMidPoint = this.GetMidPoint() + _deltaVector;
        }
        /// <summary>
        /// find common edge of 2 glyph points
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        internal static EdgeLine FindCommonOutsideEdge(GlyphPoint p, GlyphPoint q)
        {
            if (p.InwardEdge == q.InwardEdge ||
                p.InwardEdge == q.OutwardEdge)
            {
                return p.InwardEdge;
            }
            else if (p.OutwardEdge == q.InwardEdge ||
                     p.OutwardEdge == q.OutwardEdge)
            {
                return p.OutwardEdge;
            }
            else
            {

                return null;
            }
        }


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
        internal static float GetVerticalFitDiff(this EdgeLine line)
        {
            return line._newFitY - line.GetMidPoint().Y;
        }
        internal static bool ContainsTriangle(this EdgeLine edge, GlyphTriangle p)
        {
            return (p.e0 == edge ||
                    p.e1 == edge ||
                    p.e2 == edge);
        }

    }
}