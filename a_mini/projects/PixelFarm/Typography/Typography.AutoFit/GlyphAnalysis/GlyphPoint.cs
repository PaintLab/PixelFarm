//MIT, 2016-2017, WinterDev
using System.Collections.Generic;
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

        AssocBoneCollection _assocBones = new AssocBoneCollection();
        // 
        float _adjX;
        float _adjY;
        public float newX;
        public float newY;
        //
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
            this.kind = kind;
        }
        public bool IsLeftSide { get; private set; }
        public bool IsPartOfVerticalEdge { get; private set; }
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


        internal void ClearAdjustValues()
        {
            _adjX = _adjY = 0;
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



        internal void AddAssociateBone(GlyphBone bone)
        {
            _assocBones.AddAssocBone(bone);
        }


#if DEBUG
        /// <summary>
        /// glyph pointnumber
        /// </summary>
        int dbug_GlyphPointNo;
        //for debug only
        public readonly int dbugId = dbugTotalId++;
        static int dbugTotalId;
        internal GlyphPart dbugOwnerPart;  //link back to owner part
        public Poly2Tri.TriangulationPoint dbugTriangulationPoint;
        public AssocBoneCollection dbugGetAssocBones() { return _assocBones; }
        public override string ToString()
        {
            return this.dbugId + " :" + ((AdjustedY != 0) ? "***" : "") +
                    (x + "," + y + " " + kind.ToString());
        }
        internal int dbugGlyphPointNo
        {
            get { return this.dbug_GlyphPointNo; }
            set { this.dbug_GlyphPointNo = value; }
        }
#endif 
    }

    public class AssocBoneCollection 
    {
        Dictionary<GlyphBone, bool> _assocBones = new Dictionary<GlyphBone, bool>();
        List<GlyphBone> _assocBoneList;
        bool closeCollection;
        internal void CloseCollection()
        {
            if (closeCollection) return;
            //convert from GlyphBone to 
            _assocBoneList = new List<GlyphBone>(_assocBones.Keys);
            closeCollection = true;
            _assocBones = null; //clear
        }
        internal void AddAssocBone(GlyphBone bone)
        {
            if (!_assocBones.ContainsKey(bone))
            {
                _assocBones.Add(bone, true);
            }
        }
        public int GetBoneCount()
        {
            //close first            
            CloseCollection();
            return _assocBoneList.Count;
        }
        public GlyphBone this[int index] { get { return _assocBoneList[index]; } }
        public GlyphBone GetGlyphBone(int index)
        {
            //close first            
            CloseCollection();
            return _assocBoneList[index];
        }
         
        public IEnumerator<GlyphBone> GetEnumerator()
        {
            //close first            
            CloseCollection();
            //
            int j = _assocBoneList.Count;
            for (int i = 0; i < j; ++i)
            {
                yield return _assocBoneList[i];
            }
        }
    }
}

