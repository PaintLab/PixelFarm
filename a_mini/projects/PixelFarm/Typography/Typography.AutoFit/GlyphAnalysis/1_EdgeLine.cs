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
    public abstract class EdgeLine
    {
        internal readonly GlyphPoint _glyphPoint_P;
        internal readonly GlyphPoint _glyphPoint_Q;
     

      


        internal Vector2 _newDynamicMidPoint;
        EdgeLine _outsideEdge;
        Vector2 _outsideEdgeCutAt;
        float _outsideEdgeCutLen;

        GlyphTriangle _ownerTriangle;

        internal EdgeLine(GlyphTriangle ownerTriangle, GlyphPoint p, GlyphPoint q)
        {
            //this canbe inside edge or outside edge

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


            //new dynamic mid point is calculate from original X,Y

            _newDynamicMidPoint = new Vector2((p.OX + q.OX) / 2, (p.OY + q.OY) / 2);
            //-------------------------------
            //analyze angle and slope kind
            //-------------------------------  
            SlopeAngleNoDirection = this.GetSlopeAngleNoDirection();
            if (QX == PX)
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

        /// <summary>
        /// original px
        /// </summary>
        public double PX { get { return this._glyphPoint_P.OX; } }
        /// <summary>
        /// original py
        /// </summary>
        public double PY { get { return this._glyphPoint_P.OY; } }
        /// <summary>
        /// original qx
        /// </summary>
        public double QX { get { return this._glyphPoint_Q.OX; } }
        /// <summary>
        /// original qy
        /// </summary>
        public double QY { get { return this._glyphPoint_Q.OY; } }


        public bool IsTip { get; internal set; }

        internal Vector2 GetOriginalEdgeVector()
        {
            return new Vector2(
                Q.OX - _glyphPoint_P.OX,
                Q.OY - _glyphPoint_P.OY);
        }

        internal void SetOutsideEdge(EdgeLine outsideEdge, Vector2 cutPoint, float cutLen)
        {
#if DEBUG
            if (outsideEdge == this) { throw new NotSupportedException(); }
#endif
            _outsideEdge = outsideEdge;
            _outsideEdgeCutAt = cutPoint;
            _outsideEdgeCutLen = cutLen;
        }



        public GlyphPoint P
        {
            get
            {
                return _glyphPoint_P;
            }
        }
        public GlyphPoint Q
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

        public abstract bool IsOutside
        {
            get;

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
            return SlopeKind + ":" + PX + "," + PY + "," + QX + "," + QY;
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
            Vector2 _o_edgeVector = GetOriginalEdgeVector();
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
#if DEBUG
        public bool dbugNoPerpendicularBone { get; set; }
        public static int s_dbugTotalId;
        public readonly int dbugId = s_dbugTotalId++;
#endif

    }

    public class OutsideEdgeLine : EdgeLine
    {
        //if this edge is 'OUTSIDE',
        //it have 1-2 control(s) edge (inside)
        EdgeLine _ctrlEdge_P;
        EdgeLine _ctrlEdge_Q;
        internal OutsideEdgeLine(GlyphTriangle ownerTriangle, GlyphPoint p, GlyphPoint q)
            : base(ownerTriangle, p, q)
        {

            //set back
            p.SetOutsideEdge(this);
            q.SetOutsideEdge(this);

        }
        public override bool IsOutside
        {
            get { return true; }
        }
        public EdgeLine ControlEdge_P
        {
            get { return _ctrlEdge_P; }
        }
        public EdgeLine ControlEdge_Q
        {
            get { return _ctrlEdge_Q; }
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
            else if (_glyphPoint_P == controlEdge.Q)
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
            else if (_glyphPoint_Q == controlEdge.Q)
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
    }
    public class InsideEdgeLine : EdgeLine
    {   /// <summary>
        /// contact to another edge
        /// </summary>
        internal InsideEdgeLine contactToEdge;
        internal GlyphBoneJoint inside_joint;
        internal InsideEdgeLine(GlyphTriangle ownerTriangle, GlyphPoint p, GlyphPoint q)
            : base(ownerTriangle, p, q)
        {
        }
        public override bool IsOutside
        {
            get { return false; }
        }
    }
    public static class EdgeLineExtensions
    {
        public static Vector2 GetMidPoint(this EdgeLine line)
        {
            return new Vector2((float)((line.PX + line.QX) / 2), (float)((line.PY + line.QY) / 2));
        }

        internal static double GetSlopeAngleNoDirection(this EdgeLine line)
        {
            return Math.Abs(Math.Atan2(Math.Abs(line.QY - line.PY), Math.Abs(line.QX - line.PX)));
        }

        internal static bool ContainsTriangle(this EdgeLine edge, GlyphTriangle p)
        {
            return (p.e0 == edge ||
                    p.e1 == edge ||
                    p.e2 == edge);
        }
#if DEBUG
        public static void dbugGetScaledXY(this EdgeLine edge, out double px, out double py, out double qx, out double qy, float scale)
        {
            px = edge.PX * scale;
            py = edge.PY * scale;
            //
            qx = edge.QX * scale;
            qy = edge.QY * scale;

        }
#endif
    }
}