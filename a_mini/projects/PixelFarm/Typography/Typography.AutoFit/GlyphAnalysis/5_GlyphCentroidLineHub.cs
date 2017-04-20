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
                //for each centroid line
                //analyze for its bone joint
                pairs[i].AnalyzeEdgesAndCreateBoneJoint();
            }
            if (j > 1)
            {

                //add special tip
                //get first line and last 
                //check if this is loop
                GlyphCentroidPair first_line = pairs[0];
                GlyphCentroidPair last_line = pairs[j - 1];
                //open end or close end

                if (!last_line.SpecialConnectFromLastToFirst)
                {
                    //no connection from last to first (eg. o)
                    //one side is tip edge
                    if (first_line.BoneJoint.TipEdge != null)
                    {
                        //create tip info
                        AssignTipInfo(first_line, false);
                    }
                    if (last_line.BoneJoint.TipEdge != null)
                    {
                        //create tip info
                        AssignTipInfo(last_line, false);
                    }
                }
            }
            else if (j == 1)
            {
                //single line
                //eg 'l' letter

                GlyphCentroidPair line = pairs[0];
                AssignTipInfo(line, true);
            }
        }
        static void AssignTipInfo(GlyphCentroidPair pair, bool twoside)
        {
            GlyphBoneJoint joint = pair.BoneJoint;
            //get another edge for endtip

            if (IsOwnerOf(pair.p, joint.TipEdge))
            {
                //tip edge is from p side
                //so another side is q.

                var tipPoint = joint.TipPoint;
                GlyphTip tip = new GlyphTip(pair, tipPoint, joint.TipEdge);
                pair.P_Tip = tip;

                if (twoside)
                {
                    EdgeLine tipEdge = FindTip(pair, pair.q);
                    if (tipEdge == null) throw new NotSupportedException();
                    //-----
                    tip = new GlyphTip(pair, tipEdge.GetMidPoint(), tipEdge);
                    pair.Q_Tip = tip;
                }
            }
            else if (IsOwnerOf(pair.q, joint.TipEdge))
            {

                var tipPoint = joint.TipPoint;
                GlyphTip tip = new GlyphTip(pair, tipPoint, joint.TipEdge);
                pair.Q_Tip = tip;


                //tip edge is from q side
                //so another side is p.

                if (twoside)
                {
                    //find proper tip edge
                    EdgeLine tipEdge = FindTip(pair, pair.p);
                    if (tipEdge == null)
                    {
                        //some time no tip found ***
                        return;
                    }
                    //-----
                    tip = new GlyphTip(pair, tipEdge.GetMidPoint(), tipEdge);
                    pair.P_Tip = tip;
                }
            }
            else
            {
                throw new NotSupportedException();
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
                if (JointContainsTri(joint, tri))
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
            if (b.JointA != null && JointContainsTri(b.JointA, tri))
            {
                foundOnA = b.JointA;
            }
            if (b.JointB != null && JointContainsTri(b.JointB, tri))
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
        /// <summary>
        /// check if the joint contains this triangle
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="tri"></param>
        /// <returns></returns>
        static bool JointContainsTri(GlyphBoneJoint joint, GlyphTriangle tri)
        {
            GlyphCentroidPair ownerPair = joint.OwnerCentrodPair;
            return ownerPair.p == tri || ownerPair.q == tri;
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
        public void AddChild(GlyphCentroidPair pair)
        {
            //add centroid line to current branch
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
                    //create bone that link the GlyphBoneJoint of the pair.

                    GlyphCentroidPair pair = lineList[i];
                    GlyphBoneJoint joint = pair.BoneJoint;
                    //first one
                    if (joint.TipEdge != null)
                    {
                        //has tip point
                        //create bone that link this joint 
                        //and the edge
                        if (i != j - 1)
                        {
                            //not the last one
                            GlyphBone bone = new GlyphBone(joint, joint.TipEdge);
                            newlyCreatedBones.Add(bone);
                            glyphBones.Add(bone);
                        }
                    }

                    if (i < j - 1)
                    {
                        //not the last one 
                        GlyphCentroidPair nextline = lineList[i + 1];
                        GlyphBoneJoint nextJoint = nextline.BoneJoint;
                        GlyphBone bone = new GlyphBone(joint, nextJoint);
                        newlyCreatedBones.Add(bone);
                        glyphBones.Add(bone);
                    }
                    else
                    {
                        //last one
                        if (joint.TipEdge != null)
                        {
                            //not the last one
                            GlyphBone bone = new GlyphBone(joint, joint.TipEdge);
                            newlyCreatedBones.Add(bone);
                            glyphBones.Add(bone);
                        }
                        else
                        {
                            //glyph 'o' -> no tip point
                            if (j > 1)
                            {
                                GlyphCentroidPair nextline = lineList[0];
                                GlyphBone bone = new GlyphBone(joint, nextline.BoneJoint);
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


    class GlyphTip
    {
        public GlyphTip(GlyphCentroidPair ownerLine, Vector2 pos, EdgeLine edge)
        {
            this.OwnerLine = ownerLine;
            this.Pos = pos;
            this.Edge = edge;
        }
        public GlyphCentroidPair OwnerLine { get; set; }
        public Vector2 Pos { get; set; }
        public EdgeLine Edge { get; set; }
    }


}