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
        public readonly float x;
        public readonly float y;
        public readonly PointKind kind;

        /// <summary>
        /// glyph pointnumber
        /// </summary>
        int _glyphPointNo;
        // 
        float _adjX;
        float _adjY;
        //
        public bool isPartOfHorizontalEdge;
        public bool isUpperSide;


#if DEBUG
        //for debug only
        internal GlyphPart dbugOwnerPart;  //link back to owner part
        public Poly2Tri.TriangulationPoint dbugTriangulationPoint;

#endif
        public GlyphPoint(float x, float y, PointKind kind)
        {
            this.x = x;
            this.y = y;
            this.kind = kind;
        }


        public float AdjustedY
        {
            get { return _adjY; }
            internal set
            {
                _adjY = value;
            }
        }
        public float AdjustedX
        {
            get { return _adjX; }
            internal set
            {
                _adjX = value;
            }
        }

        internal int GlyphPointNo
        {
            get { return this._glyphPointNo; }
            set { this._glyphPointNo = value; }
        }

        internal void ClearAdjustValues()
        {
            _adjX = _adjY = 0;
        }


        internal void AddVerticalEdge(EdgeLine v_edge)
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

            //if (_edges == null)
            //{
            //    _edges = new List<EdgeLine>();
            //}
            //_edges.Add(v_edge);
        }



        public bool IsLeftSide { get; private set; }
        public bool IsPartOfVerticalEdge { get; private set; }
#if DEBUG
        public override string ToString()
        {
            return this._glyphPointNo + " :" + ((AdjustedY != 0) ? "***" : "") +
                    (x + "," + y + " " + kind.ToString());
        }
#endif 
    }


}