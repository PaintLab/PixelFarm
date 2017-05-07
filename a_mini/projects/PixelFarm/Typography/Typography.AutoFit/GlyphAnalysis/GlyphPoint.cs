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
        public readonly float x;
        public readonly float y;
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
        /// outside edge0
        /// </summary>
        EdgeLine _e0;
        /// <summary>
        /// outside edge 1
        /// </summary>
        EdgeLine _e1;

        public GlyphPoint(float x, float y, PointKind kind)
        {
            this.x = x;
            this.y = y;
            this.newX = this.x;
            this.newY = this.y;
            this.kind = kind;
        }
        public int SeqNo { get; internal set; }

        public bool IsLeftSide { get; private set; }
        public bool IsPartOfVerticalEdge { get; private set; }


        internal EdgeLine E0
        {
            get { return this._e0; }
        }
        internal EdgeLine E1
        {
            get { return this._e1; }
        }
        /// <summary>         
        /// set outside edge that link with this glyph point
        /// </summary>
        /// <param name="edge">edge must be outside edge</param>
        internal void SetOutsideEdge(EdgeLine edge)
        {
            if (_e0 == null)
            {
                _e0 = edge;
            }
            else if (_e1 == null)
            {
                _e1 = edge;
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
            if (_e0 == _e1)
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

