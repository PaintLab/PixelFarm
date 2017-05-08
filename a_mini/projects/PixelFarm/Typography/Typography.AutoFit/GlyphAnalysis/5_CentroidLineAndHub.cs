//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Typography.Rendering
{
    /// <summary>
    /// a collection of connected centroid pairs and bone
    /// </summary>
    class CentroidLine
    {


        //temp store centroid pair, we can clear it after AnalyzeEdgesAndCreateBoneJoints()
        List<GlyphCentroidPair> _centroid_pairs = new List<GlyphCentroidPair>();
        //joint list is created from each centroid pair
        public List<GlyphBoneJoint> _joints = new List<GlyphBoneJoint>();
        //bone list is created from linking each joint list
        public List<GlyphBone> bones = new List<GlyphBone>();
        public List<BoneGroup> boneGroups;


        internal CentroidLine()
        {
        }

#if DEBUG
        internal GlyphTriangle dbugStartTri;
#endif
        /// <summary>
        /// add a centroid pair
        /// </summary>
        /// <param name="pair"></param>
        public void AddCentroidPair(GlyphCentroidPair pair)
        {
            _centroid_pairs.Add(pair);
        }

        /// <summary>
        /// analyze edges of this line
        /// </summary>
        public void AnalyzeEdgesAndCreateBoneJoints()
        {
            List<GlyphCentroidPair> pairs = this._centroid_pairs;
            int j = pairs.Count;
            for (int i = 0; i < j; ++i)
            {
                //create bone joint (and tip edge) in each pair                
                _joints.Add(pairs[i].AnalyzeEdgesAndCreateBoneJoint());
            }
        }
        /// <summary>
        /// apply grid box to all bones in this line
        /// </summary>
        /// <param name="gridW"></param>
        /// <param name="gridH"></param>
        public void ApplyGridBox(int gridW, int gridH)
        {
            //1.
            //apply grid box to each joint
            int j = _joints.Count;
            for (int i = 0; i < j; ++i)
            {
                GlyphBoneJoint joint = _joints[i];
                Vector2 jointPos = joint.OriginalJointPos;
                //set fit (x,y) to joint, then we will evaluate bone slope again (next step)
                joint.SetFitXY(
                    MyMath.FitToHalfGrid(jointPos.X, gridW), //use fit half
                    MyMath.FitToHalfGrid(jointPos.Y, gridH));//use fit half
            }
            //2. (re) calculate slope for all bones.
            j = bones.Count;
            for (int i = 0; i < j; ++i)
            {
                bones[i].EvaluateSlope();
            }
            //3. re-grouping 
            j = bones.Count;
            this.boneGroups = new List<BoneGroup>();

            BoneGroup boneGroup = new BoneGroup(this); //new group
            boneGroup.slopeKind = LineSlopeKind.Other;
            float virtFitLen = 0;
            float ypos_sum = 0; //since we focus on ypos for vertical fitting

            for (int i = 0; i < j; ++i)
            {
                GlyphBone bone = bones[i];
                LineSlopeKind slope = bone.SlopeKind;
                if (slope != boneGroup.slopeKind)
                {
                    //add existing to list and create a new group
                    if (boneGroup.count > 0)
                    {
                        //
                        boneGroup.approxLength = virtFitLen;
                        boneGroup.y_pos = ypos_sum / boneGroup.count;
                        this.boneGroups.Add(boneGroup);
                    }
                    // 
                    boneGroup = new BoneGroup(this);
                    boneGroup.startIndex = i;
                    boneGroup.count++;
                    boneGroup.slopeKind = slope;
                    virtFitLen = bone.EvaluateFitLength(); //reset
                    ypos_sum = bone.GetMidPoint().Y;
                }
                else
                {
                    virtFitLen += bone.EvaluateFitLength(); //append
                    ypos_sum += bone.GetMidPoint().Y;
                    boneGroup.count++;
                }
            }
            if (boneGroup.count > 0)
            {
                boneGroup.approxLength = virtFitLen;
                boneGroup.y_pos = ypos_sum / boneGroup.count;
                this.boneGroups.Add(boneGroup);
            }
        }

        /// <summary>
        /// find nearest joint that contains tri 
        /// </summary>
        /// <param name="tri"></param>
        /// <returns></returns>
        public GlyphBoneJoint FindNearestJoint(GlyphTriangle tri)
        {

            for (int i = _joints.Count - 1; i >= 0; --i)
            {
                GlyphBoneJoint joint = _joints[i];
                //each pair has 1 bone joint 
                //once we have 1 candidate
                if (joint.ComposeOf(tri))
                {
                    //found another joint
                    return joint;
                }
            }
            return null;
        }
    }

    struct BoneGroupingHelper
    {
        //this is helper class
        List<BoneGroup> _selectedHorizontalBoneGroups;
        List<BoneGroup> _selectedVerticalBoneGroups;
        List<EdgeLine> tmpEdges;

        public static BoneGroupingHelper CreateBoneGroupingHelper()
        {
            BoneGroupingHelper helper = new BoneGroupingHelper();
            helper._selectedHorizontalBoneGroups = new List<BoneGroup>();
            helper._selectedVerticalBoneGroups = new List<BoneGroup>();
            helper.tmpEdges = new List<EdgeLine>();
            return helper;
        }
        public void Reset()
        {
            _selectedHorizontalBoneGroups.Clear();
            _selectedVerticalBoneGroups.Clear();
        }
        public List<BoneGroup> SelectedHorizontalBoneGroups { get { return _selectedHorizontalBoneGroups; } }
        public List<BoneGroup> SelectedVerticalBoneGroups { get { return _selectedVerticalBoneGroups; } }
        public void CollectBoneGroups(CentroidLine line)
        {
            List<BoneGroup> boneGroups = line.boneGroups;
            int j = boneGroups.Count;
            for (int i = 0; i < j; ++i)
            {
                //this version, we focus on horizontal bone group
                BoneGroup boneGroup = boneGroups[i];
                switch (boneGroup.slopeKind)
                {
                    case LineSlopeKind.Horizontal:
                        _selectedHorizontalBoneGroups.Add(boneGroup);
                        break;
                    case LineSlopeKind.Vertical:
                        _selectedVerticalBoneGroups.Add(boneGroup);
                        break;
                }
            }
        }
        public void AnalyzeHorizontalBoneGroups()
        {
            MarkTooSmallBones(_selectedHorizontalBoneGroups);
            _selectedHorizontalBoneGroups.Sort((bg0, bg1) => bg0.y_pos.CompareTo(bg1.y_pos));
            //
            //collect outside edge of horizontal group
            for (int i = _selectedHorizontalBoneGroups.Count - 1; i >= 0; --i)
            {
                _selectedHorizontalBoneGroups[i].CollectOutsideEdges(tmpEdges);
            }

        }
        public void AnalyzeVerticalBoneGroups()
        {
            MarkTooSmallBones(_selectedVerticalBoneGroups);
            //arrange again for vertical alignment
            _selectedVerticalBoneGroups.Sort((bg0, bg1) => bg0.y_pos.CompareTo(bg1.y_pos));
            //
            //collect outside edge of vertical group
            for (int i = _selectedVerticalBoneGroups.Count - 1; i >= 0; --i)
            {
                _selectedVerticalBoneGroups[i].CollectOutsideEdges(tmpEdges);
            }

        }
        static void MarkTooSmallBones(List<BoneGroup> boneGroups)
        {
            boneGroups.Sort((bg0, bg1) => bg0.approxLength.CompareTo(bg1.approxLength));
            int groupCount = boneGroups.Count;
            //median
            int mid_index = groupCount / 2;
            BoneGroup bonegroup = boneGroups[mid_index];
            float lower_limit = bonegroup.approxLength / 4;
            for (int i = 0; i < mid_index; ++i)
            {
                bonegroup = boneGroups[i];
                if (bonegroup.approxLength < lower_limit)
                {
                    bonegroup.toBeRemoved = true;
                }
                else
                {
                    //since to list is sorted                    
                    break;
                }
            }
        }
    }

    class BoneGroup
    {
        public LineSlopeKind slopeKind;
        /// <summary>
        /// start index from owner centroid line
        /// </summary>
        public int startIndex;
        /// <summary>
        /// member count in this group
        /// </summary>
        public int count;

        /// <summary>
        /// approximation of summation of bone length in this group
        /// </summary>
        public float approxLength;

        public float y_pos;
        public float minY, maxY;
        public float minX, maxX;



        public EdgeLine[] edges;

        /// <summary>
        /// marked as tobeRemoved, 
        /// </summary>
        public bool toBeRemoved;

        internal readonly CentroidLine ownerCentroidLine;
        public BoneGroup(CentroidLine ownerCentroidLine)
        {
            this.ownerCentroidLine = ownerCentroidLine;
        }

        internal void CollectOutsideEdges(List<EdgeLine> tmpEdges)
        {
            tmpEdges.Clear(); // 
            int index = this.startIndex;
            for (int n = this.count - 1; n >= 0; --n)
            {
                GlyphBone bone = ownerCentroidLine.bones[index];
                //collect all outside edge arround  glyph bone
                bone.CollectOutsideEdge(tmpEdges);
                index++;
            }
            //
            if (tmpEdges.Count == 0) return;
            //---------------------
            EdgeLine[] edges = tmpEdges.ToArray();
            this.edges = edges;
            //find minY and maxY for vertical fit
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            for (int e = edges.Length - 1; e >= 0; --e)
            {
                EdgeLine edge = edges[e];
                Vector2 midPos = edge.GetMidPoint();
                // x
                FindMinMax(ref minX, ref maxX, (float)edge.PX);
                FindMinMax(ref minX, ref maxX, (float)edge.QX);
                // y
                FindMinMax(ref minY, ref maxY, (float)edge.PY);
                FindMinMax(ref minY, ref maxY, (float)edge.QY);
            }
            //-------------------
            this.maxY = maxY;
            this.minY = minY;
            //-------------------
            this.minX = minX;
            this.maxX = maxX;

        }
        static void FindMinMax(ref float currentMin, ref float currentMax, float value)
        {
            if (value < currentMin) { currentMin = value; }
            if (value > currentMax) { currentMax = value; }
        }
#if DEBUG
        public override string ToString()
        {
            return slopeKind + ":y" + y_pos + "s:" + startIndex + ":" + count + " len:" + approxLength;
        }
#endif
    }

    /// <summary>
    /// a collection of centroid line and bone joint
    /// </summary>
    class CentroidLineHub
    {
        //-----------------------------------------------
        //a centroid line hub start at the same mainTri.
        //and can have more than 1 centroid line.
        //----------------------------------------------- 
        readonly GlyphTriangle startTriangle;
        //each centoid line start with main triangle       
        Dictionary<GlyphTriangle, CentroidLine> _lines = new Dictionary<GlyphTriangle, CentroidLine>();
        //-----------------------------------------------
        List<CentroidLineHub> otherConnectedLineHubs;//connections from other hub***
        //-----------------------------------------------
        CentroidLine currentLine;
        GlyphTriangle currentBranchTri;

        public CentroidLineHub(GlyphTriangle startTriangle)
        {
            this.startTriangle = startTriangle;
        }
        public GlyphTriangle StartTriangle
        {
            get { return startTriangle; }
        }
        /// <summary>
        /// set current centroid line to a centroid line that starts with triangle of centroid-line-head
        /// </summary>
        /// <param name="triOfCentroidLineHead"></param>
        public void SetCurrentCentroidLine(GlyphTriangle triOfCentroidLineHead)
        {
            //this method is used during centroid line hub creation
            if (currentBranchTri != triOfCentroidLineHead)
            {
                //check if we have already create it
                if (!_lines.TryGetValue(triOfCentroidLineHead, out currentLine))
                {
                    //if not found then create new
                    currentLine = new CentroidLine();
#if  DEBUG
                    currentLine.dbugStartTri = triOfCentroidLineHead;
#endif
                    _lines.Add(triOfCentroidLineHead, currentLine);
                }
                currentBranchTri = triOfCentroidLineHead;
            }
        }
        /// <summary>
        /// member centoid line count
        /// </summary>
        public int LineCount
        {
            get
            {
                return _lines.Count;
            }
        }
        /// <summary>
        /// add centroid pair to current centroid line
        /// </summary>
        /// <param name="pair"></param>
        public void AddCentroidPair(GlyphCentroidPair pair)
        {
            //add centroid pair to line 
            currentLine.AddCentroidPair(pair);
        }
        /// <summary>
        /// analyze each branch for edge information
        /// </summary>
        public void CreateBoneJoints()
        {
            foreach (CentroidLine line in _lines.Values)
            {
                line.AnalyzeEdgesAndCreateBoneJoints();
            }
        }


        /// <summary>
        /// create a set of GlyphBone
        /// </summary>
        /// <param name="newlyCreatedBones"></param>
        public void CreateBones(List<GlyphBone> newlyCreatedBones)
        {
            foreach (CentroidLine line in _lines.Values)
            {
                List<GlyphBoneJoint> jointlist = line._joints;
                List<GlyphBone> glyphBones = line.bones;
                int j = jointlist.Count;

                for (int i = 0; i < j; ++i)
                {
                    if (i == 0)
                    {
                        //find connection from 
                        //first tri of  centroid line
                        //to other joint
                        GlyphBoneJoint firstJoint = jointlist[i];
                        GlyphTriangle firstTri = firstJoint.P_Tri;

                        //test 3 edges, find edge that is inside
                        //and the joint is not the same as first_pair.BoneJoint
                        CreateTipBoneIfNeed(firstTri.e0 as InsideEdgeLine, firstJoint, newlyCreatedBones, glyphBones);
                        CreateTipBoneIfNeed(firstTri.e1 as InsideEdgeLine, firstJoint, newlyCreatedBones, glyphBones);
                        CreateTipBoneIfNeed(firstTri.e2 as InsideEdgeLine, firstJoint, newlyCreatedBones, glyphBones);
                        //------------
                    }

                    //for each GlyphCentroidPair                    
                    //create bone that link the GlyphBoneJoint of the pair 

                    GlyphBoneJoint joint = jointlist[i];
                    if (joint.TipEdgeP != null)
                    {

                        GlyphBone tipBone = new GlyphBone(joint, joint.TipEdgeP);
                        newlyCreatedBones.Add(tipBone);
                        glyphBones.Add(tipBone);
                    }

                    if (joint.TipEdgeQ != null)
                    {
                        GlyphBone tipBone = new GlyphBone(joint, joint.TipEdgeQ);
                        newlyCreatedBones.Add(tipBone);
                        glyphBones.Add(tipBone);
                    }
                    //----------------------------------------------------- 
                    if (i < j - 1)
                    {
                        //not the last one 
                        //has tip end 
                        GlyphBoneJoint nextJoint = jointlist[i + 1];
                        GlyphBone bone = new GlyphBone(joint, nextJoint);
                        newlyCreatedBones.Add(bone);
                        glyphBones.Add(bone);
                    }
                    else
                    {
                        //the last one ...
                        if (j > 1)
                        {

                            GlyphBoneJoint last_joint = jointlist[j - 1];
                            //P
                            GlyphTriangle lastTri = last_joint.P_Tri;
                            CreateTipBoneIfNeed(lastTri.e0 as InsideEdgeLine, last_joint, newlyCreatedBones, glyphBones);
                            CreateTipBoneIfNeed(lastTri.e1 as InsideEdgeLine, last_joint, newlyCreatedBones, glyphBones);
                            CreateTipBoneIfNeed(lastTri.e2 as InsideEdgeLine, last_joint, newlyCreatedBones, glyphBones);

                            // Q
                            lastTri = last_joint.Q_Tri;
                            CreateTipBoneIfNeed(lastTri.e0 as InsideEdgeLine, last_joint, newlyCreatedBones, glyphBones);
                            CreateTipBoneIfNeed(lastTri.e1 as InsideEdgeLine, last_joint, newlyCreatedBones, glyphBones);
                            CreateTipBoneIfNeed(lastTri.e2 as InsideEdgeLine, last_joint, newlyCreatedBones, glyphBones);

                        }
                    }
                }
            }
        }

        static void CreateTipBoneIfNeed(
            InsideEdgeLine insideEdge, GlyphBoneJoint joint,
            List<GlyphBone> newlyCreatedBones, List<GlyphBone> glyphBones)
        {
            if (insideEdge != null &&
                insideEdge.inside_joint != null &&
                insideEdge.inside_joint != joint)
            {
                //create connection 
                GlyphBone tipBone = new GlyphBone(insideEdge.inside_joint, joint);
                newlyCreatedBones.Add(tipBone);
                glyphBones.Add(tipBone);
            }
        }

        public void CreateBoneLinkBetweenCentroidLine(List<GlyphBone> newlyCreatedBones)
        {
            foreach (CentroidLine line in _lines.Values)
            {
                List<GlyphBone> glyphBones = line.bones;
                GlyphBoneJoint firstJoint = line._joints[0];
                GlyphTriangle first_p_tri = firstJoint.P_Tri;
                //                 
                CreateBoneJointIfNeed(first_p_tri.e0 as InsideEdgeLine, first_p_tri, firstJoint, newlyCreatedBones, glyphBones);
                CreateBoneJointIfNeed(first_p_tri.e1 as InsideEdgeLine, first_p_tri, firstJoint, newlyCreatedBones, glyphBones);
                CreateBoneJointIfNeed(first_p_tri.e2 as InsideEdgeLine, first_p_tri, firstJoint, newlyCreatedBones, glyphBones);
            }
        }
        static void CreateBoneJointIfNeed(
            InsideEdgeLine insideEdge,
            GlyphTriangle first_p_tri,
            GlyphBoneJoint firstJoint,
            List<GlyphBone> newlyCreatedBones,
            List<GlyphBone> glyphBones)
        {
            if (insideEdge != null &&
                insideEdge.inside_joint == null)
            {
                InsideEdgeLine mainEdge = insideEdge;
                EdgeLine nbEdge = null;
                if (FindSameCoordEdgeLine(first_p_tri.N0, mainEdge, out nbEdge) ||
                    FindSameCoordEdgeLine(first_p_tri.N1, mainEdge, out nbEdge) ||
                    FindSameCoordEdgeLine(first_p_tri.N2, mainEdge, out nbEdge))
                {

                    //confirm that nbEdge is INSIDE edge
                    if (nbEdge.IsInside)
                    {
                        GlyphBoneJoint joint = new GlyphBoneJoint((InsideEdgeLine)nbEdge, mainEdge);
                        GlyphBone bone = new GlyphBone(mainEdge.inside_joint, firstJoint);
                        newlyCreatedBones.Add(bone);
                        glyphBones.Add(bone);
                    }
                    else
                    {
                        //?
                    }
                }
                else
                {
                    //?
                }
            }
        }
        /// <summary>
        /// find nb triangle that has the same edgeLine
        /// </summary>
        /// <param name="tri"></param>
        /// <returns></returns>
        static bool FindSameCoordEdgeLine(GlyphTriangle tri, EdgeLine edgeLine, out EdgeLine foundEdge)
        {
            foundEdge = null;
            if (tri == null)
            {
                return false;
            }

            if (SameCoord(foundEdge = tri.e0, edgeLine) ||
                SameCoord(foundEdge = tri.e1, edgeLine) ||
                SameCoord(foundEdge = tri.e2, edgeLine))
            {
                return true;
            }
            foundEdge = null; //not found
            return false;
        }
        static bool SameCoord(EdgeLine a, EdgeLine b)
        {
            //TODO: review this again
            return (a.P == b.P ||
                    a.P == b.Q) &&
                   (a.Q == b.P ||
                    a.Q == b.Q);
        }

        public Dictionary<GlyphTriangle, CentroidLine> GetAllCentroidLines()
        {
            return _lines;
        }

        public bool FindBoneJoint(GlyphTriangle tri,
            out CentroidLine foundOnBranch,
            out GlyphBoneJoint foundOnJoint)
        {
            foreach (CentroidLine line in _lines.Values)
            {
                if ((foundOnJoint = line.FindNearestJoint(tri)) != null)
                {
                    foundOnBranch = line;
                    return true;
                }
            }
            foundOnBranch = null;
            foundOnJoint = null;
            return false;

        }
        public void AddLineHubConnection(CentroidLineHub anotherHub)
        {
            if (otherConnectedLineHubs == null)
            {
                otherConnectedLineHubs = new List<CentroidLineHub>();
            }
            otherConnectedLineHubs.Add(anotherHub);
        }

        CentroidLine anotherCentroidLine;
        GlyphBoneJoint foundOnJoint;

        public void SetHeadConnnection(CentroidLine anotherCentroidLine, GlyphBoneJoint foundOnJoint)
        {
            this.anotherCentroidLine = anotherCentroidLine;
            this.foundOnJoint = foundOnJoint;
        }
        public GlyphBoneJoint GetHeadConnectedJoint()
        {
            return foundOnJoint;
        }
        public List<CentroidLineHub> GetConnectedLineHubs()
        {
            return this.otherConnectedLineHubs;
        }

    }




    static class CentroidLineExtensions
    {  //utils

        public static Vector2 GetHeadPosition(this CentroidLine line)
        {
            //after create bone process
            var bones = line.bones;
            if (bones.Count == 0)
            {
                return Vector2.Zero;
            }
            else
            {
                //TODO: review here
                //use jointA of bone of join B of bone
                return bones[0].JointA.OriginalJointPos;
            }
        }

        public static Vector2 CalculateAvgHeadPosition(this CentroidLineHub lineHub)
        {
            Dictionary<GlyphTriangle, CentroidLine> _lines = lineHub.GetAllCentroidLines();
            int j = _lines.Count;
            if (j == 0) return Vector2.Zero;
            //---------------------------------
            double cx = 0;
            double cy = 0;
            foreach (CentroidLine line in _lines.Values)
            {
                Vector2 headpos = line.GetHeadPosition();
                cx += headpos.X;
                cy += headpos.Y;
            }
            return new Vector2((float)(cx / j), (float)(cy / j));
        }

    }
}