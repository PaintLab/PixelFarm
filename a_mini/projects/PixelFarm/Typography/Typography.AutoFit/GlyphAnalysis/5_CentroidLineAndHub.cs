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
        internal List<BoneGroup> selectedHorizontalBoneGroups;
        internal readonly GlyphTriangle startTri;

        internal CentroidLine(GlyphTriangle startTri)
        {
            this.startTri = startTri;
        }


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
            //apply grid box to each joint
            int j = _joints.Count;
            for (int i = 0; i < j; ++i)
            {
                GlyphBoneJoint joint = _joints[i];
                Vector2 jointPos = joint.OriginalJointPos;
                joint.SetFitXY(MyMath.FitToHalfGrid(jointPos.X, gridW), MyMath.FitToHalfGrid(jointPos.Y, gridH));
            }
            //calculate slope for all bones
            j = bones.Count;
            for (int i = 0; i < j; ++i)
            {
                bones[i].EvaluateSlope();
            }
        }
        /// <summary>
        /// separate GlyphBone into groups
        /// </summary>
        public void AnalyzeBoneGroups()
        {
            int j = bones.Count;
            this.boneGroups = new List<BoneGroup>(); //clear 

            BoneGroup boneGroup = new BoneGroup(); //new group
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
                        boneGroup.virtFitLen = virtFitLen;
                        boneGroup.y_pos = ypos_sum / boneGroup.count;
                        this.boneGroups.Add(boneGroup);
                    }
                    // 
                    boneGroup = new BoneGroup();
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
                boneGroup.virtFitLen = virtFitLen;
                boneGroup.y_pos = ypos_sum / boneGroup.count;
                this.boneGroups.Add(boneGroup);
            }
            //----------------

        }
        public void CollectOutsideEdges()
        {
            int j = this.boneGroups.Count;

            List<EdgeLine> tmpEdges = new List<EdgeLine>();
            for (int i = 0; i < j; ++i)
            {
                BoneGroup bonegroup = this.boneGroups[i];
                if (!bonegroup.toBeRemoved && bonegroup.slopeKind == LineSlopeKind.Horizontal)
                {
                    //this is horizontal group

                    if (selectedHorizontalBoneGroups == null)
                    {
                        selectedHorizontalBoneGroups = new List<BoneGroup>();
                    }
                    tmpEdges.Clear();

                    //
                    int startAt = bonegroup.startIndex;
                    for (int n = bonegroup.count - 1; n >= 0; --n)
                    {
                        GlyphBone bone = bones[startAt];
                        //collect all outside edge arround  glyph bone
                        bone.CollectOutsideEdge(tmpEdges);
                        startAt++;
                    }
                    //
                    if (tmpEdges.Count > 0)
                    {
                        EdgeLine[] edges = tmpEdges.ToArray();
                        bonegroup.edges = edges;
                        //find minY and maxY for vertical fit
                        float minY = float.MaxValue;
                        float maxY = float.MinValue;
                        for (int e = edges.Length - 1; e >= 0; --e)
                        {
                            EdgeLine edge = edges[e];
                            Vector2 midPos = edge.GetMidPoint();
                            FindMinMax(ref minY, ref maxY, (float)edge.y0);
                            FindMinMax(ref minY, ref maxY, (float)edge.y1);
                        }
                        //-------------------
                        bonegroup.minY = minY;
                        bonegroup.maxY = maxY;

                        selectedHorizontalBoneGroups.Add(bonegroup);
                    }
                }
            }
        }
        static void FindMinMax(ref float currentMin, ref float currentMax, float value)
        {
            if (value < currentMin) { currentMin = value; }
            if (value > currentMax) { currentMax = value; }
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

    struct BoneGroupStatisticCollector
    {

        public List<BoneGroup> _selectedHorizontalBoneGroups;
        public void Reset()
        {
            _selectedHorizontalBoneGroups.Clear();
        }
        public void CollectBoneGroup(CentroidLine ownerline)
        {
            List<BoneGroup> boneGroups = ownerline.boneGroups;
            int j = boneGroups.Count;
            for (int i = 0; i < j; ++i)
            {
                //this version, we focus on horizontal bone group
                BoneGroup boneGroup = boneGroups[i];
                if (boneGroup.slopeKind == LineSlopeKind.Horizontal)
                {
                    _selectedHorizontalBoneGroups.Add(boneGroup);
                }

            }
        }
        public void AnalyzeBoneGroups()
        {
            //remove too small bone group
            //
            //statistic ?
            //quantization?
            //
            //this version, just use median len
            _selectedHorizontalBoneGroups.Sort((bg0, bg1) => bg0.virtFitLen.CompareTo(bg1.virtFitLen));
            int groupCount = _selectedHorizontalBoneGroups.Count;
            //median
            int mid_index = groupCount / 2;
            BoneGroup bonegroup = _selectedHorizontalBoneGroups[mid_index];
            float lower_limit = bonegroup.virtFitLen / 4;
            for (int i = 0; i < mid_index; ++i)
            {
                bonegroup = _selectedHorizontalBoneGroups[i];
                if (bonegroup.virtFitLen < lower_limit)
                {
                    bonegroup.toBeRemoved = true;
                }
                else
                {
                    //since to list is sorted                    
                    break;
                }
            }
            //--------------------
            //arrange again for vertical alignment
            _selectedHorizontalBoneGroups.Sort((bg0, bg1) => bg0.y_pos.CompareTo(bg1.y_pos));
        }
    }
    class BoneGroup
    {
        //small stem

        public int startIndex;
        public int count;
        public LineSlopeKind slopeKind;
        public float virtFitLen;
        public float y_pos;//avg
        public float minY;
        public float maxY;
        public EdgeLine[] edges;
        public bool toBeRemoved;

#if DEBUG
        public override string ToString()
        {
            return slopeKind + ":y" + y_pos + "s:" + startIndex + ":" + count + " len:" + virtFitLen;
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
        List<CentroidLineHub> otherConnectedLineHubs;//connection from other hub***
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
                    currentLine = new CentroidLine(triOfCentroidLineHead);
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

                        if (firstTri.e0.IsInside &&
                            firstTri.e0.inside_joint != null &&
                            firstTri.e0.inside_joint != firstJoint)
                        {
                            //create connection 
                            GlyphBone tipBone = new GlyphBone(firstTri.e0.inside_joint, firstJoint);
                            newlyCreatedBones.Add(tipBone);
                            glyphBones.Add(tipBone);
                        }
                        //
                        if (firstTri.e1.IsInside &&
                            firstTri.e1.inside_joint != null &&
                            firstTri.e1.inside_joint != firstJoint)
                        {
                            GlyphBone tipBone = new GlyphBone(firstTri.e1.inside_joint, firstJoint);
                            newlyCreatedBones.Add(tipBone);
                            glyphBones.Add(tipBone);
                        }
                        //
                        if (firstTri.e2.IsInside &&
                            firstTri.e2.inside_joint != null &&
                            firstTri.e2.inside_joint != firstJoint)
                        {
                            GlyphBone tipBone = new GlyphBone(firstTri.e2.inside_joint, firstJoint);
                            newlyCreatedBones.Add(tipBone);
                            glyphBones.Add(tipBone);
                        }
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
                            GlyphTriangle lastTri = last_joint.P_Tri;


                            if (lastTri.e0.IsInside &&
                                lastTri.e0.inside_joint != null &&
                                lastTri.e0.inside_joint != last_joint)
                            {
                                //create connection 
                                GlyphBone tipBone = new GlyphBone(lastTri.e0.inside_joint, last_joint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                            //
                            if (lastTri.e1.IsInside &&
                                lastTri.e1.inside_joint != null &&
                                lastTri.e1.inside_joint != last_joint)
                            {
                                GlyphBone tipBone = new GlyphBone(lastTri.e1.inside_joint, last_joint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                            //
                            if (lastTri.e2.IsInside &&
                                lastTri.e2.inside_joint != null &&
                                lastTri.e2.inside_joint != last_joint)
                            {
                                GlyphBone tipBone = new GlyphBone(lastTri.e2.inside_joint, last_joint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                            //------------------

                            lastTri = last_joint.Q_Tri;
                            if (lastTri.e0.IsInside &&
                                    lastTri.e0.inside_joint != null &&
                                    lastTri.e0.inside_joint != last_joint)
                            {
                                //create connection 
                                GlyphBone tipBone = new GlyphBone(lastTri.e0.inside_joint, last_joint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                            //
                            if (lastTri.e1.IsInside &&
                                lastTri.e1.inside_joint != null &&
                                lastTri.e1.inside_joint != last_joint)
                            {
                                GlyphBone tipBone = new GlyphBone(lastTri.e1.inside_joint, last_joint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                            //
                            if (lastTri.e2.IsInside &&
                                lastTri.e2.inside_joint != null &&
                                lastTri.e2.inside_joint != last_joint)
                            {
                                GlyphBone tipBone = new GlyphBone(lastTri.e2.inside_joint, last_joint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                        }
                    }
                }
            }
        }
        public void CreateBoneLinkBetweenCentroidLine(List<GlyphBone> newlyCreatedBones)
        {
            foreach (CentroidLine line in _lines.Values)
            {
                GlyphTriangle s_tri = line.startTri;

                List<GlyphBone> glyphBones = line.bones;

                GlyphBoneJoint firstPairJoint = line._joints[0];
                GlyphTriangle first_p_tri = firstPairJoint.P_Tri;

                if (first_p_tri.e0.IsInside &&
                    first_p_tri.e0.inside_joint == null)
                {
                    EdgeLine mainEdge = first_p_tri.e0;
                    EdgeLine nbEdge = null;
                    if (FindSameCoordEdgeLine(first_p_tri.N0, mainEdge, out nbEdge) ||
                        FindSameCoordEdgeLine(first_p_tri.N1, mainEdge, out nbEdge) ||
                        FindSameCoordEdgeLine(first_p_tri.N2, mainEdge, out nbEdge))
                    {

                        //confirm that nbEdge is INSIDE edge
                        if (nbEdge.IsInside)
                        {
                            GlyphBoneJoint joint = new GlyphBoneJoint(nbEdge, mainEdge);
                            GlyphBone bone = new GlyphBone(mainEdge.inside_joint, firstPairJoint);
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
                //---------------------------------------------------------------------
                if (first_p_tri.e1.IsInside &&
                    first_p_tri.e1.inside_joint == null)
                {
                    EdgeLine mainEdge = first_p_tri.e1;
                    EdgeLine nbEdge = null;
                    if (FindSameCoordEdgeLine(first_p_tri.N0, mainEdge, out nbEdge) ||
                        FindSameCoordEdgeLine(first_p_tri.N1, mainEdge, out nbEdge) ||
                        FindSameCoordEdgeLine(first_p_tri.N2, mainEdge, out nbEdge))
                    {

                        //confirm that nbEdge is INSIDE edge
                        if (nbEdge.IsInside)
                        {
                            GlyphBoneJoint joint = new GlyphBoneJoint(nbEdge, mainEdge);
                            GlyphBone bone = new GlyphBone(mainEdge.inside_joint, firstPairJoint);
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
                //---------------------------------------------------------------------
                if (first_p_tri.e2.IsInside &&
                    first_p_tri.e2.inside_joint == null)
                {
                    EdgeLine mainEdge = first_p_tri.e2;
                    EdgeLine nbEdge = null;
                    if (FindSameCoordEdgeLine(first_p_tri.N0, mainEdge, out nbEdge) ||
                        FindSameCoordEdgeLine(first_p_tri.N1, mainEdge, out nbEdge) ||
                        FindSameCoordEdgeLine(first_p_tri.N2, mainEdge, out nbEdge))
                    {

                        //confirm that nbEdge is INSIDE edge
                        if (nbEdge.IsInside)
                        {
                            GlyphBoneJoint joint = new GlyphBoneJoint(nbEdge, mainEdge);
                            GlyphBone bone = new GlyphBone(mainEdge.inside_joint, firstPairJoint);
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
            return (a.GlyphPoint_P == b.GlyphPoint_P ||
                    a.GlyphPoint_P == b.GlyphPoint_Q) &&
                   (a.GlyphPoint_Q == b.GlyphPoint_P ||
                    a.GlyphPoint_Q == b.GlyphPoint_Q);
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
        static GlyphBoneJoint FindJoint(GlyphBone b, Vector2 pos, GlyphTriangle tri)
        {
            //bone link 2 joint
            //find what joint 

            GlyphBoneJoint foundOnA = null;
            GlyphBoneJoint foundOnB = null;
            if (b.JointA != null && b.JointA.ComposeOf(tri))
            {
                foundOnA = b.JointA;
            }
            if (b.JointB != null && b.JointB.ComposeOf(tri))
            {
                foundOnB = b.JointB;
            }

            if (b.TipEdge != null)
            {

            }

            if (foundOnA != null && foundOnB != null)
            {
                //select 1
                //nearest distance (pos to joint a) or (pos to joint b) 
                return MyMath.MinDistanceFirst(pos, foundOnA.OriginalJointPos, foundOnB.OriginalJointPos) ? foundOnA : foundOnB;
            }
            else if (foundOnA != null)
            {
                return foundOnA;
            }
            else if (foundOnB != null)
            {
                return foundOnB;
            }
            return null;
        }
    }
}