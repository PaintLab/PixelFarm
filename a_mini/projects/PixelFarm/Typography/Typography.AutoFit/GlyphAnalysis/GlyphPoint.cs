//MIT, 2016-2017, WinterDev

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
        public float fit_NewX;
        public float fit_NewY;
        public bool fit_analyzed;
        //------------------------------------- 

        /// <summary>
        /// outside inward edge ?, TODO: review inward, outward concept again 
        /// </summary>
        OutsideEdgeLine _e0;
        /// <summary>
        /// outside outward edge ? TODO: review inward, outward concept again 
        /// </summary>
        OutsideEdgeLine _e1;

        public GlyphPoint(float x, float y, PointKind kind)
        {
            this.x = this.newX = x;
            this.y = this.newY = y;
            this.kind = kind;
        }
        public int SeqNo { get; internal set; }

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
        internal OutsideEdgeLine E0
        {
            //TODO: review inward and outward edge again
            get { return this._e0; }

        }
        /// <summary>
        /// outside outward edge
        /// </summary>
        internal OutsideEdgeLine E1
        {
            //TODO: review inward and outward edge again
            get { return this._e1; }
        }

        /// <summary>         
        /// set outside edge that link with this glyph point
        /// </summary>
        /// <param name="edge">edge must be outside edge</param>
        internal void SetOutsideEdgeUnconfirmEdgeDirection(OutsideEdgeLine edge)
        {
            //at this stage, we don't known the edge is outward or inward.
            //so just set it
            //------------------------------------------
            //e0 and e1 will be swaped later for this point SetCorrectInwardAndOutWardEdge() ***

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

