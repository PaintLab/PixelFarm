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

        internal void EvaluatePerpendicularBone()
        {
            _assocBones.EvaluatePerpendicularBone(this);
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
        bool hasEvaluatedPerpendicularBones;

        Vector2 _cutPoint;

        int exactPerpendicularBone;

        internal AssocBoneCollection()
        {
            exactPerpendicularBone = -1;//not found
        }
        internal void CloseCollection()
        {
            if (closeCollection) return;
            //convert from GlyphBone to 
            _assocBoneList = new List<GlyphBone>(_assocBones.Keys);
            _assocBones = null; //clear
            closeCollection = true;
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

        internal void EvaluatePerpendicularBone(GlyphPoint ownerPoint)
        {
            //find a perpendicular line  and cutpoint from ownerPoint 
            //to one of glyphBone, or avg of glyphBones
            CloseCollection();
            if (hasEvaluatedPerpendicularBones) return;
            hasEvaluatedPerpendicularBones = true; //change state
            //
            //---------------------------------------------------------
            Vector2 o_point = new Vector2(ownerPoint.x, ownerPoint.y);
            int b_count = _assocBoneList.Count;
            for (int i = 0; i < b_count; ++i)
            {
                GlyphBone b = _assocBoneList[i];
                if (MyMath.FindPerpendicularCutPoint(b, o_point, out _cutPoint))
                {
                    exactPerpendicularBone = i;
                    break;
                }
            }
            //------------------------------------
            if (exactPerpendicularBone > -1) { return; }
            //------------------------------------
            //if not found exact bone
            //we use middle area cutpoint
            switch (b_count)
            {
                case 0: throw new System.NotSupportedException(); //?
                case 1:
                    {
                        //only 1 bone and no cutpoint found
                        //so no exact perpendicular cut point
                        //use mid point
                        _cutPoint = _assocBoneList[0].GetMidPoint();
                    }
                    break;
                case 2:
                    {
                        if (!FindAvgCutPoint(_assocBoneList[0], _assocBoneList[1], o_point, out _cutPoint))
                        {
                            //if not found 

                        }
                    }
                    break;
                default:
                    {
                        //we start at the middle 
                        //and expand left and right

                        int mm_mid = b_count / 2;
                        int startAt = mm_mid - 1;
                        int endAt = mm_mid + 1;
                        bool foundResult = false;
                        for (; startAt >= 0 && endAt < b_count;)
                        {
                            if (FindAvgCutPoint(_assocBoneList[startAt], _assocBoneList[endAt], o_point, out _cutPoint))
                            {

                                foundResult = true;
                                break; //from loop
                            }
                        }

                        if (!foundResult)
                        {
                            //no result found

                        } 
                    }
                    break;
            }
        }

        static bool FindAvgCutPoint(GlyphBone beginAt, GlyphBone endAt, Vector2 p, out Vector2 exactCutPoint)
        {
            //find avg cutpoint of 2 bone 
            Vector2 first_v = beginAt.GetMidPoint();
            Vector2 last_v = endAt.GetMidPoint();
            return MyMath.FindPerpendicularCutPoint2(first_v, last_v, p, out exactCutPoint);
        }

    }
}

