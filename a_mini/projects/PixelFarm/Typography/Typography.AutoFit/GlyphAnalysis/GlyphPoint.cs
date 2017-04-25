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
            ApplyNewRelativeLen(1);
        }
        internal void ApplyNewRelativeLen(float newRelativeLen)
        {
            if (newRelativeLen == 1)
            {
                this.newX = this.x;
                this.newY = this.y;
            }
            else
            {
                Vector2 newRadiusEnd = _assocBones.CalculateCutPoint(newRelativeLen, new Vector2(x, y));
                this.newX = newRadiusEnd.X;
                this.newY = newRadiusEnd.Y;
            }
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

    public enum BoneCutPointKind
    {
        Unknown,
        PerpendicularToSingleBone,
        PerpendicularToBoneGroup,
        MoreThanOnePerpendicularBones,
        NotPendicularCutPoint

    }
    public class AssocBoneCollection
    {
        Dictionary<GlyphBone, bool> _assocBones = new Dictionary<GlyphBone, bool>();
        List<GlyphBone> _assocBoneList;
        bool closeCollection;
        bool hasEvaluatedPerpendicularBones;

        Vector2 _cutPoint;//original cut point

        int _startIndexAt = -1;
        int _endIndexAt = -1;

        internal AssocBoneCollection()
        {

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
        public BoneCutPointKind CutPointKind
        {
            get;
            set;
        }
        public Vector2 CutPoint { get { return _cutPoint; } }
        public int StartIndexAt { get { return _startIndexAt; } }
        public int EndIndexAt { get { return _endIndexAt; } }




        struct TmpCutPoint
        {
            public Vector2 cutpoint;
            public int index;
            public TmpCutPoint(int index, Vector2 cutpoint)
            {
                this.index = index;
                this.cutpoint = cutpoint;
            }
        }
        public static double AngleBetween(Vector2 vector1, Vector2 vector2)
        {
            double rad1 = System.Math.Atan2(vector1.Y, vector1.X);
            double rad2 = System.Math.Atan2(vector2.Y, vector2.X);
            //we want to find diff

            if (rad1 < 0)
            {
                rad1 = System.Math.PI + rad1;
            }
            if (rad2 < 0)
            {
                rad2 = System.Math.PI + rad2;
            }

            return rad1 - rad2;
        }
        const float Epsilon = 0.0001f;
        //
        internal void EvaluatePerpendicularBone(GlyphPoint ownerPoint)
        {
            //find a perpendicular line  and cutpoint from ownerPoint 
            //to one of glyphBone, or avg of glyphBones
            CloseCollection();
            if (hasEvaluatedPerpendicularBones) return;
            hasEvaluatedPerpendicularBones = true; //change state

            //
            //---------------------------------------------------------
            //TODO: review tmpCutPoints again 
            List<TmpCutPoint> tmpCutPoints = new List<TmpCutPoint>();

            Vector2 o_point = new Vector2(ownerPoint.x, ownerPoint.y);
            int b_count = _assocBoneList.Count;
            int perpendcut_count = 0;

            for (int i = 0; i < b_count; ++i)
            {
                GlyphBone b = _assocBoneList[i];
                Vector2 tempCutPoint;
                if (MyMath.FindPerpendicularCutPoint(b, o_point, out tempCutPoint))
                {

                    _cutPoint = tempCutPoint;
                    _startIndexAt = _endIndexAt = i;
                    this.CutPointKind = BoneCutPointKind.PerpendicularToSingleBone;
                    tmpCutPoints.Add(new TmpCutPoint(i, _cutPoint));
                    perpendcut_count++;
                }
            }
            //---------------------------------------------------------
            if (perpendcut_count > 1)
            {
                //1.
                this.CutPointKind = BoneCutPointKind.MoreThanOnePerpendicularBones;
                _startIndexAt = _endIndexAt = 0;
            }
            else
            {
                // 
            }
            //------------------------------------
            if (_startIndexAt > -1) { return; }
            //------------------------------------
            //if not found exact bone
            //we use middle area cutpoint
            switch (b_count)
            {
                case 0:
                    return;
                case 1:
                    {
                        //only 1 bone and no cutpoint found
                        //so no exact perpendicular cut point
                        //use mid point
                        _cutPoint = _assocBoneList[0].GetMidPoint();
                        this.CutPointKind = BoneCutPointKind.NotPendicularCutPoint;
                        _startIndexAt = _endIndexAt = 0;
                    }
                    break;
                case 2:
                    {
                        if (!FindAvgCutPoint(_assocBoneList[0], _assocBoneList[1], o_point, out _cutPoint))
                        {
                            //if not found 
                            //-> no cutpoint
                            //link to min distance
                            //if (MyMath.MinDistanceFirst(_assocBoneList[0].GetMidPoint(), _assocBoneList[1].GetMidPoint(), o_point))
                            //{
                            //    _cutPoint = _assocBoneList[0].GetMidPoint();
                            //    _startIndexAt = _endIndexAt = 0;
                            //}
                            //else
                            //{
                            //    _cutPoint = _assocBoneList[1].GetMidPoint();
                            //    _startIndexAt = _endIndexAt = 1;
                            //}
                            this.CutPointKind = BoneCutPointKind.NotPendicularCutPoint;
                        }
                        else
                        {

                            _startIndexAt = 0;
                            _endIndexAt = 1;
                            this.CutPointKind = BoneCutPointKind.PerpendicularToBoneGroup;
                        }
                    }
                    break;
                default:
                    {
                        //we start at the middle 
                        //and expand left and right

                        int mid_index = b_count / 2;
                        int startAt = mid_index - 1;
                        int endAt = mid_index + 1;
                        bool foundResult = false;

                        for (; startAt >= 0 && endAt < b_count;)
                        {
                            if (FindAvgCutPoint(_assocBoneList[startAt], _assocBoneList[endAt], o_point, out _cutPoint))
                            {
                                this.CutPointKind = BoneCutPointKind.PerpendicularToBoneGroup;
                                _startIndexAt = startAt;
                                _endIndexAt = endAt;
                                foundResult = true;
                                break; //from loop for
                            }
                            startAt--; //expand wider to left
                            if (startAt >= 0 &&
                                FindAvgCutPoint(_assocBoneList[startAt], _assocBoneList[endAt], o_point, out _cutPoint))
                            {

                                this.CutPointKind = BoneCutPointKind.PerpendicularToBoneGroup;
                                _startIndexAt = startAt;
                                _endIndexAt = endAt;
                                foundResult = true;
                                break; //from loop for

                            }

                            endAt++; //expand wider to right
                        }
                        //if (!foundResult)
                        //{
                        //    //no result found
                        //    _cutPoint = _assocBoneList[mid_index].GetMidPoint();
                        //    _startIndexAt = _endIndexAt = mid_index;
                        //    this.CutPointKind = BoneCutPointKind.NotPendicularCutPoint;
                        //}
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
        internal Vector2 CalculateCutPoint(float relativeLen, Vector2 orgVector)
        {
            if (this.CutPointKind == BoneCutPointKind.NotPendicularCutPoint)
            {
                return orgVector;
            }
            else
            {
                Vector2 delta = orgVector - _cutPoint;
                Vector2 newDelta = delta.NewLength(delta.Length() * relativeLen);
                return _cutPoint + newDelta;
            }
        }

    }
}

