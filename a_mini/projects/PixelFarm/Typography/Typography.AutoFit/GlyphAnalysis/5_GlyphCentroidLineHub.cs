//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Typography.Rendering
{

    class GlyphCentroidLine
    {
        public List<GlyphCentroidPair> pairs = new List<GlyphCentroidPair>();
        public List<GlyphBone> bones = new List<GlyphBone>();
        internal readonly GlyphTriangle startTri;
        internal GlyphCentroidLine(GlyphTriangle startTri)
        {
            this.startTri = startTri;
        }
        public void AddCentroidLine(GlyphCentroidPair pair)
        {
            pairs.Add(pair);
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
                return bones[0].JointA.Position;
            }
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

            //---------------
            if (j > 1)
            {

                //add special tip
                //get first line and last 
                //check if this is loop
                GlyphCentroidPair first_pair = pairs[0];
                GlyphCentroidPair last_pair = pairs[j - 1];
                //open end or close end

                if (!last_pair.SpecialConnectFromLastToFirst)
                {
                    first_pair.UpdateTips();
                    last_pair.UpdateTips();
                }
            }
            else if (j == 1)
            {
                //single line
                //eg 'l' letter
                pairs[0].UpdateTips();
            }
        }

        static EdgeLine FindTip(GlyphCentroidPair pair, GlyphTriangle triangle)
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
        static bool IsOwnerOf(GlyphTriangle p, EdgeLine edge)
        {
            return (p.e0 == edge ||
                    p.e1 == edge ||
                    p.e2 == edge);
        }
        /// <summary>
        /// find nearest joint that contains tri 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="tri"></param>
        /// <returns></returns>
        public GlyphBoneJoint FindNearestJoint(Vector2 pos, GlyphTriangle tri)
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

    }

    /// <summary>
    /// a collection of centroid line
    /// </summary>
    class CentroidLineHub
    {
        readonly GlyphTriangle mainTri;
        Dictionary<GlyphTriangle, GlyphCentroidLine> _lines = new Dictionary<GlyphTriangle, GlyphCentroidLine>();
        List<CentroidLineHub> otherConnectedLineHubs;//connection from other
        //
        GlyphCentroidLine currentLine;
        GlyphTriangle currentBranchTri;

        public CentroidLineHub(GlyphTriangle mainTri)
        {
            this.mainTri = mainTri;
        }
        public GlyphTriangle MainTriangle
        {
            get { return mainTri; }
        }

        public Vector2 GetCenterPos()
        {
            int j = _lines.Count;
            if (j == 0) return Vector2.Zero;
            //---------------------------------
            double cx = 0;
            double cy = 0;
            foreach (GlyphCentroidLine branch in _lines.Values)
            {
                Vector2 headpos = branch.GetHeadPosition();
                cx += headpos.X;
                cy += headpos.Y;
            }
            return new Vector2((float)(cx / j), (float)(cy / j));
        }
        public void SetBranch(GlyphTriangle tri)
        {
            if (currentBranchTri != tri)
            {
                //check if we have already create it
                if (!_lines.TryGetValue(tri, out currentLine))
                {
                    //if not found then create new
                    currentLine = new GlyphCentroidLine(tri);
                    _lines.Add(tri, currentLine);
                }
                currentBranchTri = tri;
            }
        }
        public int BranchCount
        {
            get
            {
                return _lines.Count;
            }
        }
        /// <summary>
        /// add centroid line to current centroid line
        /// </summary>
        /// <param name="pair"></param>
        public void AddCentroidPair(GlyphCentroidPair pair)
        {
            //add centroid pair to line
            pair.SetCentroidLine(currentLine);
            currentLine.AddCentroidLine(pair);
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
                List<GlyphCentroidPair> lineList = line.pairs;
                List<GlyphBone> glyphBones = line.bones;
                int j = lineList.Count;

                for (int i = 0; i < j; ++i)
                {
                    //for each GlyphCentroidPair                    
                    //create bone that link the GlyphBoneJoint of the pair 
                    GlyphCentroidPair pair = lineList[i];
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
                        GlyphCentroidPair nextline = lineList[i + 1];
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

                            GlyphCentroidPair nextline = lineList[0];
                            if (pair.IsAdjacentTo(nextline))
                            {
                                GlyphBone bone = new GlyphBone(joint, nextline.BoneJoint);
                                bone.IsLinkBack = true;
                                newlyCreatedBones.Add(bone);
                                glyphBones.Add(bone);
                            }
                        }
                    }
                }
                //----------------
            }
        }

        public Dictionary<GlyphTriangle, GlyphCentroidLine> GetAllBranches()
        {
            return _lines;
        }


        //--------------------------------------------------------
        public bool FindBoneJoint(GlyphTriangle tri, Vector2 pos,
            out GlyphCentroidLine foundOnBranch,
            out GlyphBoneJoint foundOnJoint)
        {
            foreach (GlyphCentroidLine line in _lines.Values)
            {
                if ((foundOnJoint = line.FindNearestJoint(pos, tri)) != null)
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
}