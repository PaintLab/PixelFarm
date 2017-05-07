//MIT, 2016-2017, WinterDev
using System.Collections.Generic;
using System.Numerics;
namespace Typography.Rendering
{

    public enum PointKind : byte
    {
        LineStart,
        LineStop,
        //
        C3Start,
        C3Control1,
        C3End,
        //
        C4Start,
        C4Control1,
        C4Control2,
        C4End,

        CurveInbetween,
    }

    public class GlyphPoint
    {
        readonly float x; //original x
        readonly float y; //original y
        public readonly PointKind kind;

        /// <summary>
        /// calculated x and y  
        /// </summary>
        public float newX;
        public float newY;
        //----------------------------------------

        //temp ***
        public float fit_NewX;
        public float fit_NewY;
        public bool fit_analyzed;
        //----------------------------------------
        public bool isPartOfHorizontalEdge;
        public bool isUpperSide;

        /// <summary>
        /// outside inward edge
        /// </summary>
        EdgeLine _inwardEdge;
        /// <summary>
        /// outside outward edge
        /// </summary>
        EdgeLine _outwardEdge;

        public GlyphPoint(float x, float y, PointKind kind)
        {
            this.x = this.newX = x;
            this.y = this.newY = y;
            this.kind = kind;
        }
        public int SeqNo { get; internal set; }
        public bool IsLeftSide { get; private set; }
        public bool IsPartOfVerticalEdge { get; private set; }

        /// <summary>
        /// original X
        /// </summary>
        public float OX { get { return this.x; } }
        /// <summary>
        /// original Y
        /// </summary>
        public float OY { get { return this.y; } }


        /// <summary>
        /// outside inward edge
        /// </summary>
        internal EdgeLine InwardEdge
        {
            get { return this._inwardEdge; }
            set { _inwardEdge = value; }
        }
        /// <summary>
        /// outside outward edge
        /// </summary>
        internal EdgeLine OutwardEdge
        {
            get { return this._outwardEdge; }
            set { _outwardEdge = value; }
        }

        /// <summary>         
        /// set outside edge that link with this glyph point
        /// </summary>
        /// <param name="edge">edge must be outside edge</param>
        internal void SetOutsideEdge(EdgeLine edge)
        {
            //at this stage, we don't known the edge is outward or inward.
            //so just set it
            //------------------------------------------
            //e0 and e1 will be swaped later for this point SetCorrectInwardAndOutWardEdge() ***

            if (_inwardEdge == null)
            {
                _inwardEdge = edge;
            }
            else if (_outwardEdge == null)
            {
                _outwardEdge = edge;
            }
            else
            {
                throw new System.NotSupportedException();
            }
            //----
#if DEBUG
            if (edge == null)
            {

            }
            if (_inwardEdge == _outwardEdge)
            {
                throw new System.NotSupportedException();
            }
#endif
        }

        internal void NotifyVerticalEdge(EdgeLine v_edge)
        {
            //associated 
            if (!this.IsPartOfVerticalEdge)
            {
                this.IsPartOfVerticalEdge = true;
            }
            if (!this.IsLeftSide)
            {
                this.IsLeftSide = v_edge.IsLeftSide;
            }
        }
        internal static bool SameCoordAs(GlyphPoint a, GlyphPoint b)
        {
            return a.x == b.x && a.y == b.y;
        }

#if DEBUG
        public readonly int dbugId = dbugTotalId++;
        static int dbugTotalId;
        internal GlyphPart dbugOwnerPart;  //link back to owner part
        public Poly2Tri.TriangulationPoint dbugTriangulationPoint;

        public override string ToString()
        {
            //TODO: review adjust value again
            return this.dbugId + " :" +
                    (x + "," + y + " " + kind.ToString());
        }

#endif
    }

}

