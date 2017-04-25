//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Typography.Rendering
{
    /// <summary>
    /// a collection of connected centroid pairs
    /// </summary>
    class GlyphCentroidLine
    {
        //at least 1 pair
        public List<GlyphCentroidPair> pairs = new List<GlyphCentroidPair>();
        public List<GlyphBone> bones = new List<GlyphBone>();
        internal readonly GlyphTriangle startTri;
        internal GlyphCentroidLine(GlyphTriangle startTri)
        {
            this.startTri = startTri;
        }
        public GlyphTriangle GetLastTriangle()
        {
            //get last triangle
            int j = pairs.Count;
            //TODO: review p or q is the last one?
            return pairs[j - 1].q;
        }
        /// <summary>
        /// add a centroid pair
        /// </summary>
        /// <param name="pair"></param>
        public void AddCentroidPair(GlyphCentroidPair pair)
        {
            pairs.Add(pair);
        }

        /// <summary>
        /// analyze edges of this line
        /// </summary>
        public void AnalyzeEdgesAndCreateBoneJoints()
        {
            List<GlyphCentroidPair> pairs = this.pairs;
            int j = pairs.Count;
            for (int i = 0; i < j; ++i)
            {
                //create bone joint (and tip edge) in each pair                
                pairs[i].AnalyzeEdgesAndCreateBoneJoint();
            }


        }


        public GlyphCentroidPair FindNearestPair(GlyphTriangle tri)
        {
            for (int i = pairs.Count - 1; i >= 0; --i)
            {
                GlyphCentroidPair pair = pairs[i];
                if (pair.p.IsConnectedWith(tri) ||
                    pair.q.IsConnectedWith(tri))
                {
                    //found
                    return pair;
                }
            }
            //not found
            return null;
        }

        /// <summary>
        /// find nearest joint that contains tri 
        /// </summary>
        /// <param name="tri"></param>
        /// <returns></returns>
        public GlyphBoneJoint FindNearestJoint(GlyphTriangle tri)
        {

            for (int i = pairs.Count - 1; i >= 0; --i)
            {
                GlyphBoneJoint joint = pairs[i].BoneJoint;
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
                return MyMath.MinDistanceFirst(pos, foundOnA.Position, foundOnB.Position) ? foundOnA : foundOnB;
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
        public Vector2 GetHeadPosition()
        {
            //after create bone process
            if (bones.Count == 0)
            {
                return Vector2.Zero;
            }
            else
            {
                //TODO: review here
                //use jointA of bone of join B of bone
                return bones[0].JointA.Position;
            }
        }
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

        Dictionary<GlyphTriangle, GlyphCentroidLine> _lines = new Dictionary<GlyphTriangle, GlyphCentroidLine>();
        //-----------------------------------------------
        List<CentroidLineHub> otherConnectedLineHubs;//connection from other hub***

        //-----------------------------------------------

        //
        GlyphCentroidLine currentLine;
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
                    currentLine = new GlyphCentroidLine(triOfCentroidLineHead);
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
            foreach (GlyphCentroidLine line in _lines.Values)
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
            foreach (GlyphCentroidLine line in _lines.Values)
            {
                List<GlyphCentroidPair> pairList = line.pairs;
                List<GlyphBone> glyphBones = line.bones;
                int j = pairList.Count;

                for (int i = 0; i < j; ++i)
                {
                    if (i == 0)
                    {
                        //find connection from 
                        //first tri of  centroid line
                        //to other joint
                        GlyphCentroidPair first_pair = pairList[i];
                        GlyphTriangle firstTri = first_pair.p;

                        //test 3 edges, find edge that is inside
                        //and the joint is not the same as first_pair.BoneJoint

                        if (firstTri.e0.IsInside &&
                            firstTri.e0.inside_joint != null &&
                            firstTri.e0.inside_joint != first_pair.BoneJoint)
                        {
                            //create connection 
                            GlyphBone tipBone = new GlyphBone(firstTri.e0.inside_joint, first_pair.BoneJoint);
                            newlyCreatedBones.Add(tipBone);
                            glyphBones.Add(tipBone);
                        }
                        //
                        if (firstTri.e1.IsInside &&
                            firstTri.e1.inside_joint != null &&
                            firstTri.e1.inside_joint != first_pair.BoneJoint)
                        {
                            GlyphBone tipBone = new GlyphBone(firstTri.e1.inside_joint, first_pair.BoneJoint);
                            newlyCreatedBones.Add(tipBone);
                            glyphBones.Add(tipBone);
                        }
                        //
                        if (firstTri.e2.IsInside &&
                            firstTri.e2.inside_joint != null &&
                            firstTri.e2.inside_joint != first_pair.BoneJoint)
                        {
                            GlyphBone tipBone = new GlyphBone(firstTri.e2.inside_joint, first_pair.BoneJoint);
                            newlyCreatedBones.Add(tipBone);
                            glyphBones.Add(tipBone);
                        }
                        //------------
                    }

                    //for each GlyphCentroidPair                    
                    //create bone that link the GlyphBoneJoint of the pair 
                    GlyphCentroidPair pair = pairList[i];
                    GlyphBoneJoint joint = pair.BoneJoint;
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
                        GlyphCentroidPair nextline = pairList[i + 1];
                        GlyphBoneJoint nextJoint = nextline.BoneJoint;
                        GlyphBone bone = new GlyphBone(joint, nextJoint);
                        newlyCreatedBones.Add(bone);
                        glyphBones.Add(bone);
                    }
                    else
                    {
                        //the last one ...
                        if (j > 1)
                        {
                            //check if  the last bone is connected to the first or not 
                            GlyphCentroidPair nextPair = pairList[0];
                            if (pair.IsAdjacentTo(nextPair))
                            {
                                GlyphBone bone = new GlyphBone(joint, nextPair.BoneJoint);
                                bone.IsLinkBack = true;
                                newlyCreatedBones.Add(bone);
                                glyphBones.Add(bone);
                            }

                            GlyphCentroidPair last_pair = pairList[j - 1];
                            GlyphTriangle lastTri = last_pair.p;


                            if (lastTri.e0.IsInside &&
                                lastTri.e0.inside_joint != null &&
                                lastTri.e0.inside_joint != last_pair.BoneJoint)
                            {
                                //create connection 
                                GlyphBone tipBone = new GlyphBone(lastTri.e0.inside_joint, last_pair.BoneJoint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                            //
                            if (lastTri.e1.IsInside &&
                                lastTri.e1.inside_joint != null &&
                                lastTri.e1.inside_joint != last_pair.BoneJoint)
                            {
                                GlyphBone tipBone = new GlyphBone(lastTri.e1.inside_joint, last_pair.BoneJoint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                            //
                            if (lastTri.e2.IsInside &&
                                lastTri.e2.inside_joint != null &&
                                lastTri.e2.inside_joint != last_pair.BoneJoint)
                            {
                                GlyphBone tipBone = new GlyphBone(lastTri.e2.inside_joint, last_pair.BoneJoint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                            //------------------

                            lastTri = last_pair.q;
                            if (lastTri.e0.IsInside &&
                                    lastTri.e0.inside_joint != null &&
                                    lastTri.e0.inside_joint != last_pair.BoneJoint)
                            {
                                //create connection 
                                GlyphBone tipBone = new GlyphBone(lastTri.e0.inside_joint, last_pair.BoneJoint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                            //
                            if (lastTri.e1.IsInside &&
                                lastTri.e1.inside_joint != null &&
                                lastTri.e1.inside_joint != last_pair.BoneJoint)
                            {
                                GlyphBone tipBone = new GlyphBone(lastTri.e1.inside_joint, last_pair.BoneJoint);
                                newlyCreatedBones.Add(tipBone);
                                glyphBones.Add(tipBone);
                            }
                            //
                            if (lastTri.e2.IsInside &&
                                lastTri.e2.inside_joint != null &&
                                lastTri.e2.inside_joint != last_pair.BoneJoint)
                            {
                                GlyphBone tipBone = new GlyphBone(lastTri.e2.inside_joint, last_pair.BoneJoint);
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
            foreach (GlyphCentroidLine line in _lines.Values)
            {
                GlyphTriangle s_tri = line.startTri;
                List<GlyphCentroidPair> pairList = line.pairs;
                List<GlyphBone> glyphBones = line.bones;

                GlyphCentroidPair firstPair = pairList[0];
                GlyphTriangle first_p_tri = firstPair.p;
                GlyphBoneJoint firstPairJoint = firstPair.BoneJoint;

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

        public Dictionary<GlyphTriangle, GlyphCentroidLine> GetAllCentroidLines()
        {
            return _lines;
        }
        //--------------------------------------------------------
        public GlyphCentroidPair FindTriangle(GlyphTriangle tri)
        {
            foreach (GlyphCentroidLine line in _lines.Values)
            {
                GlyphCentroidPair p = line.FindNearestPair(tri);
                if (p != null)
                {

                }
            }
            return null;
        }
        public bool FindBoneJoint(GlyphTriangle tri,
        out GlyphCentroidLine foundOnBranch,
        out GlyphBoneJoint foundOnJoint)
        {
            foreach (GlyphCentroidLine line in _lines.Values)
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


        GlyphCentroidLine anotherCentroidLine;
        GlyphBoneJoint foundOnJoint;

        public void SetHeadConnnection(GlyphCentroidLine anotherCentroidLine, GlyphBoneJoint foundOnJoint)
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




    static class GlyphCentroidLineExtensions
    {  //utils
        public static Vector2 CalculateAvgHeadPosition(this CentroidLineHub lineHub)
        {
            Dictionary<GlyphTriangle, GlyphCentroidLine> _lines = lineHub.GetAllCentroidLines();
            int j = _lines.Count;
            if (j == 0) return Vector2.Zero;
            //---------------------------------
            double cx = 0;
            double cy = 0;
            foreach (GlyphCentroidLine line in _lines.Values)
            {
                Vector2 headpos = line.GetHeadPosition();
                cx += headpos.X;
                cy += headpos.Y;
            }
            return new Vector2((float)(cx / j), (float)(cy / j));
        }

        public static EdgeLine FindTip(this GlyphCentroidPair pair, GlyphTriangle triangle)
        {
            GlyphBoneJoint boneJoint = pair.BoneJoint;
            if (CanbeTipEdge(triangle.e0, boneJoint))
            {
                return triangle.e0;
            }
            if (CanbeTipEdge(triangle.e1, boneJoint))
            {
                return triangle.e1;
            }
            if (CanbeTipEdge(triangle.e2, boneJoint))
            {
                return triangle.e2;
            }
            //not found
            return null;
        }
        static bool CanbeTipEdge(EdgeLine edge, GlyphBoneJoint compareJoint)
        {
            return edge.IsOutside;
            //
            //return (edge.IsOutside &&
            //        edge != compareJoint.RibEndEdgeA &&
            //        edge != compareJoint.RibEndEdgeB);
            //{
            //    return true;
            //}
            //return false;
        }
    }
}