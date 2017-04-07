//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Typography.Rendering
{

    public class GlyphCentroidBranch
    {
        public List<GlyphCentroidLine> lines = new List<GlyphCentroidLine>();
        public List<GlyphBone> bones = new List<GlyphBone>();
        public readonly GlyphTriangle startTri;
        public GlyphCentroidBranch(GlyphTriangle startTri)
        {
            this.startTri = startTri;
        }
        public void AddCentroidLine(GlyphCentroidLine line)
        {
            lines.Add(line);
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
        /// analyze this branch
        /// </summary>
        public void AnalyzeEdges()
        {
            //for each branch
            List<GlyphCentroidLine> lineList = this.lines;
            int j = lineList.Count;
            for (int i = 0; i < j; ++i)
            {
                //for each centroid line
                //analyze for its bone joint
                lineList[i].AnalyzeAndMarkEdges();
            }
        }

        /// <summary>
        /// find nearest joint that contains tri 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="tri"></param>
        /// <returns></returns>
        public GlyphBoneJoint FindNearestJoint(Vector2 pos, GlyphTriangle tri)
        {
            for (int i = bones.Count - 1; i >= 0; --i)
            {
                GlyphBoneJoint joint1 = FindJoint(bones[i], pos, tri);
                if (joint1 != null)
                {
                    if (i == 0)
                    {
                        //this is the last one 
                        //so just return
                        return joint1;
                    }
                    else
                    {
                        //not the last one
                        //compare again with sibling joint
                        GlyphBoneJoint joint2 = FindJoint(bones[i - 1], pos, tri);
                        if (joint2 == null)
                        {
                            return joint1;
                        }
                        else
                        {
                            //compare distance again
                            return MyMath.MinDistanceFirst(pos, joint1.Position, joint2.Position) ? joint1 : joint2;
                        }
                    }
                }
            }
            //not found
            return null;
        }
        static GlyphBoneJoint FindJoint(GlyphBone b, Vector2 pos, GlyphTriangle tri)
        {
            //bone link 2 joint
            //find what joint 

            GlyphBoneJoint foundOnA = null;
            GlyphBoneJoint foundOnB = null;
            if (b.JointA != null && FoundTriOnJoint(b.JointA, tri))
            {
                foundOnA = b.JointA;
            }
            if (b.JointB != null && FoundTriOnJoint(b.JointB, tri))
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
        static bool FoundTriOnJoint(GlyphBoneJoint joint, GlyphTriangle tri)
        {
            GlyphCentroidLine ownerCentroidLine = joint.OwnerCentroidLine;
            if (ownerCentroidLine.p == tri || ownerCentroidLine.q == tri)
            {
                //found
                return true;
            }
            return false;
        }
    }




    /// <summary>
    /// a collection of centroid line
    /// </summary>
    public class CentroidLineHub
    {
        readonly GlyphTriangle mainTri;
        Dictionary<GlyphTriangle, GlyphCentroidBranch> branches = new Dictionary<GlyphTriangle, GlyphCentroidBranch>();
        List<CentroidLineHub> connectedLineHubs;
        GlyphCentroidBranch currentBranchList;
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
            int j = branches.Count;
            if (j == 0) return Vector2.Zero;
            //---------------------------------
            double cx = 0;
            double cy = 0;
            foreach (GlyphCentroidBranch branch in branches.Values)
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
                if (!branches.TryGetValue(tri, out currentBranchList))
                {
                    //if not found then create new
                    currentBranchList = new GlyphCentroidBranch(tri);
                    branches.Add(tri, currentBranchList);
                }
                currentBranchTri = tri;
            }
        }
        public int BranchCount
        {
            get
            {
                return branches.Count;
            }
        }
        public void AddChild(GlyphCentroidLine centroidLine)
        {
            //add centroid line to current branch
            currentBranchList.AddCentroidLine(centroidLine);
        }
        /// <summary>
        /// analyze each branch for edge information
        /// </summary>
        public void AnalyzeEachBranchForEdgeInfo()
        {
            foreach (GlyphCentroidBranch branch in branches.Values)
            {
                branch.AnalyzeEdges();
            }
        }


        internal void CreateBones(List<GlyphBone> newlyCreatedBones)
        {
            //int centroidLineCount = _centroidLines.Count;
            ////do bone length histogram
            ////boneList2 = new List<GlyphCentroidBone>(boneCount);
            ////boneList2.AddRange(bones);
            //////----------------------------------------
            ////AnalyzeBoneLength();

            ////----------------------------------------
            //for (int i = 0; i < centroidLineCount; ++i)
            //{
            //    //each bone has 2 triangles at its ends
            //    //we analyze both triangles' roles
            //    //eg...
            //    //left -right 
            //    //top-bottom
            //    _centroidLines[i].Analyze();
            //}
            ////---------------------------------------- 
            ////collect bone joint, along centroid line?



            foreach (GlyphCentroidBranch branch in branches.Values)
            {
                List<GlyphCentroidLine> lineList = branch.lines;
                List<GlyphBone> glyphBones = branch.bones;
                int j = lineList.Count;

                for (int i = 0; i < j; ++i)
                {
                    //for each centroid line
                    //create bone that link the joint
                    GlyphCentroidLine line = lineList[i];
                    GlyphBoneJoint joint = line.BoneJoint;
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

                        GlyphCentroidLine nextline = lineList[i + 1];
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
                    }
                }
            }
        }

        public Dictionary<GlyphTriangle, GlyphCentroidBranch> GetAllBranches()
        {
            return branches;
        }


        //--------------------------------------------------------
        public bool FindBoneJoint(GlyphTriangle tri, Vector2 pos, out GlyphCentroidBranch foundOnBranch, out GlyphBoneJoint foundOnJoint)
        {
            foreach (GlyphCentroidBranch br in branches.Values)
            {
                foundOnJoint = br.FindNearestJoint(pos, tri);
                if (foundOnJoint != null)
                {
                    foundOnBranch = br;
                    return true;
                }
            }
            foundOnBranch = null;
            foundOnJoint = null;
            return false;

        }
        public void AddLineHubConnection(CentroidLineHub anotherHub)
        {
            if (connectedLineHubs == null)
            {
                connectedLineHubs = new List<CentroidLineHub>();
            }
            connectedLineHubs.Add(anotherHub);
        }


        GlyphCentroidBranch anotherCentroidBranch;
        GlyphBoneJoint foundOnJoint;

        public void SetHeadConnnection(GlyphCentroidBranch anotherCentroidBranch, GlyphBoneJoint foundOnJoint)
        {
            this.anotherCentroidBranch = anotherCentroidBranch;
            this.foundOnJoint = foundOnJoint;
        }


        public GlyphBoneJoint GetHeadConnectedJoint()
        {
            return foundOnJoint;
        }
        public List<CentroidLineHub> GetConnectedLineHubs()
        {
            return this.connectedLineHubs;
        }

    }


    /// <summary>
    /// a line that connects between centroid of 2 GlyphTriangle(p => q)
    /// </summary>
    public class GlyphCentroidLine
    {

        public readonly GlyphTriangle p, q;
        public readonly double boneLength;


        GlyphBoneJoint _boneJoint; //1L1


        public GlyphCentroidLine(GlyphTriangle p, GlyphTriangle q)
        {
            this.p = p;
            this.q = q;

            double dy = q.CentroidY - p.CentroidY;
            double dx = q.CentroidX - p.CentroidX;
            this.boneLength = Math.Sqrt(
                (dy * dy) + (dx * dx)
                );
        }

        public GlyphBoneJoint BoneJoint { get { return _boneJoint; } }

        public double SlopeAngle { get; private set; }


        public LineSlopeKind SlopeKind { get; private set; }


        internal void AnalyzeAndMarkEdges()
        {

            //
            //p => (x0,y0)
            //q => (x1,y1)
            //line move from p to q 
            //...
            //tasks:
            //1. find slop angle
            //2. find slope kind
            //3. mark edge info


            //check if q is upper or lower when compare with p
            //check if q is on left side or right side of p
            //then we know the direction
            //....
            //p
            double x0 = p.CentroidX;
            double y0 = p.CentroidY;
            //q
            double x1 = q.CentroidX;
            double y1 = q.CentroidY;

            if (x1 == x0)
            {
                this.SlopeKind = LineSlopeKind.Vertical;
                SlopeAngle = 1;
            }
            else
            {
                SlopeAngle = Math.Abs(Math.Atan2(Math.Abs(y1 - y0), Math.Abs(x1 - x0)));
                if (SlopeAngle > MyMath._85degreeToRad)
                {
                    SlopeKind = LineSlopeKind.Vertical;
                }
                else if (SlopeAngle < MyMath._03degreeToRad) //_15degreeToRad
                {
                    SlopeKind = LineSlopeKind.Horizontal;
                }
                else
                {
                    SlopeKind = LineSlopeKind.Other;
                }
            }
            //--------------------------------------
            //for p and q, count number of outside edge
            //if outsideEdgeCount of triangle >=2 -> this triangle is tip part

            //int p_outsideEdgeCount = OutSideEdgeCount(p);
            //int q_outsideEdgeCount = OutSideEdgeCount(q);
            //bool p_isTip = false;
            //bool q_isTip = false;

            //if (p_outsideEdgeCount >= 2)
            //{
            //    //tip bone
            //    p_isTip = true;
            //}
            //if (q_outsideEdgeCount >= 2)
            //{
            //    //tipbone
            //    q_isTip = true;
            //}

            //-------------------------------------- 
            //p_isTip && q_isTip is possible eg. dot or dot of  i etc.
            //-------------------------------------- 
            //find matching side:
            //the bone connects between triangle p and q (via centroid)
            //

            MarkEdgeSides(p.e0, q);
            MarkEdgeSides(p.e1, q);
            MarkEdgeSides(p.e2, q);
            //
            MarkEdgeSides(q.e0, p);
            MarkEdgeSides(q.e1, p);
            MarkEdgeSides(q.e2, p);
            //-------------------------------------- 


            if (_boneJoint != null)
            {
                //add more information
                //find proper 'outside' edge
                MarkProperOpositeEdge(p, _boneJoint, _boneJoint._p_contact_edge);
                MarkProperOpositeEdge(q, _boneJoint, _boneJoint._q_contact_edge);
            }
        }


        static int AssignResult(EdgeLine result, ref EdgeLine edgeA, ref EdgeLine edgeB)
        {
            if (edgeA == null)
            {
                edgeA = result;
                return 1;
            }
            else
            {
                edgeB = result;
                return 2;
            }
        }

        void MarkProperOpositeEdge(GlyphTriangle triangle, GlyphBoneJoint boneJoint, EdgeLine edge)
        {
            //find shortest part from contactsite to edge or to corner
            //draw perpendicular line to outside edge
            //and to the  corner of current edge 
            EdgeLine edgeA = null;
            EdgeLine edgeB = null;
            int count = 0;
            if (triangle.e0 != edge && triangle.e0.IsOutside)
            {
                count = AssignResult(triangle.e0, ref edgeA, ref edgeB);
            }
            if (triangle.e1 != edge && triangle.e1.IsOutside)
            {
                count = AssignResult(triangle.e1, ref edgeA, ref edgeB);
            }
            if (triangle.e2 != edge && triangle.e2.IsOutside)
            {
                count = AssignResult(triangle.e2, ref edgeA, ref edgeB);
            }
            //-------------------------------------------------------------------------------------
            switch (count)
            {
                default: throw new NotSupportedException();
                case 0:
                    break;
                case 1:
                    {
                        Vector2 perpend_A = MyMath.FindPerpendicularCutPoint(edgeA, boneJoint.Position);
                        Vector2 corner = new Vector2((float)edge.p.X, (float)edge.p.Y);
                        //find distance from contactSite to specific point 
                        double sqDistanceToEdgeA = boneJoint.CalculateSqrDistance(perpend_A);
                        double sqDistanceTo_P = boneJoint.CalculateSqrDistance(corner);

                        if (sqDistanceToEdgeA < sqDistanceTo_P)
                        {
                            boneJoint.AddRibEndAt(edgeA, perpend_A);
                        }
                        else
                        {
                            boneJoint.AddRibEndAt(edge, corner);
                        }
                    }
                    break;
                case 2:
                    {

                        Vector2 perpend_A = MyMath.FindPerpendicularCutPoint(edgeA, boneJoint.Position);
                        Vector2 perpend_B = MyMath.FindPerpendicularCutPoint(edgeB, boneJoint.Position);

                        Vector2 corner = new Vector2((float)edge.p.X, (float)edge.p.Y);
                        //find distance from contactSite to specific point 
                        double sqDistanceToEdgeA = boneJoint.CalculateSqrDistance(perpend_A);
                        double sqDistanceToEdgeB = boneJoint.CalculateSqrDistance(perpend_B);
                        double sqDistanceTo_P = boneJoint.CalculateSqrDistance(corner);

                        int minAt = MyMath.Min(sqDistanceToEdgeA, sqDistanceToEdgeB, sqDistanceTo_P);
                        switch (minAt)
                        {
                            default: throw new NotSupportedException();
                            case 0:
                                {
                                    switch (boneJoint.OwnerCentroidLine.SlopeKind)
                                    {
                                        case LineSlopeKind.Horizontal:
                                            {
                                                //centroid horizontal, tip-> vertical
                                                if (edgeA.SlopeKind == LineSlopeKind.Vertical)
                                                {
                                                    boneJoint.SetTipEdge(edgeA);
                                                    boneJoint.AddRibEndAt(edgeB, perpend_B);

                                                }
                                                else if (edgeB.SlopeKind == LineSlopeKind.Vertical)
                                                {
                                                    //b
                                                    boneJoint.SetTipEdge(edgeB);
                                                    boneJoint.AddRibEndAt(edgeA, perpend_A);
                                                }
                                                else
                                                {
                                                    goto default;
                                                }
                                            }
                                            break;
                                        case LineSlopeKind.Vertical:
                                            {
                                                //centroid vertical, -> tip horizontal
                                                if (edgeA.SlopeKind == LineSlopeKind.Horizontal)
                                                {
                                                    boneJoint.SetTipEdge(edgeA);
                                                    boneJoint.AddRibEndAt(edgeB, perpend_B);

                                                }
                                                else if (edgeB.SlopeKind == LineSlopeKind.Horizontal)
                                                {
                                                    //b
                                                    boneJoint.SetTipEdge(edgeB);
                                                    boneJoint.AddRibEndAt(edgeA, perpend_A);

                                                }
                                                else
                                                {
                                                    goto default;
                                                }
                                            }
                                            break;
                                        default:
                                            {
                                                boneJoint.AddRibEndAt(edgeA, perpend_A);
                                                //check if B side is tip part
                                                boneJoint.SetTipEdge(edgeB);
                                            }
                                            break;
                                    }
                                }
                                break;
                            case 1:
                                {
                                    switch (boneJoint.OwnerCentroidLine.SlopeKind)
                                    {
                                        case LineSlopeKind.Horizontal:
                                            {
                                                //centroid horizontal, tip-> vertical
                                                if (edgeA.SlopeKind == LineSlopeKind.Vertical)
                                                {
                                                    boneJoint.SetTipEdge(edgeA);
                                                    boneJoint.AddRibEndAt(edgeB, perpend_B);

                                                }
                                                else if (edgeB.SlopeKind == LineSlopeKind.Vertical)
                                                {
                                                    //b
                                                    boneJoint.SetTipEdge(edgeB);
                                                    boneJoint.AddRibEndAt(edgeA, perpend_A);
                                                }
                                                else
                                                {
                                                    goto default;
                                                }
                                            }
                                            break;
                                        case LineSlopeKind.Vertical:
                                            {
                                                //centroid vertical, -> tip horizontal
                                                if (edgeA.SlopeKind == LineSlopeKind.Horizontal)
                                                {
                                                    boneJoint.SetTipEdge(edgeA);
                                                    boneJoint.AddRibEndAt(edgeB, perpend_B);

                                                }
                                                else if (edgeB.SlopeKind == LineSlopeKind.Horizontal)
                                                {
                                                    //b
                                                    boneJoint.SetTipEdge(edgeB);
                                                    boneJoint.AddRibEndAt(edgeA, perpend_A);

                                                }
                                                else
                                                {
                                                    goto default;
                                                }
                                            }
                                            break;
                                        default:
                                            {
                                                boneJoint.AddRibEndAt(edgeB, perpend_B);
                                                boneJoint.SetTipEdge(edgeA);
                                            }
                                            break;
                                    }
                                }
                                break;
                            case 2:
                                boneJoint.AddRibEndAt(edge, corner);
                                break;
                        }
                    }
                    break;
            }
        }

        void MarkEdgeSides(EdgeLine edgeLine, GlyphTriangle anotherTriangle)
        {
            if (edgeLine.IsOutside)
            {
                MarkMatchingOutsideEdge(edgeLine, anotherTriangle);
            }
            else
            {
                //inside
                if (_boneJoint == null)
                {

                    if (MarkMatchingInsideEdge(edgeLine, anotherTriangle))
                    {
                        _boneJoint = new GlyphBoneJoint(
                            this,
                            edgeLine,
                            edgeLine.contactToEdge);
                    }
                }
            }
        }
        static bool MarkMatchingInsideEdge(EdgeLine insideEdge, GlyphTriangle another)
        {

            //side-by-side  
            if (another.e0.IsInside && MarkMatchingInsideEdge(insideEdge, another.e0))
            {
                //inside                 
                return true;
            }
            //
            if (another.e1.IsInside && MarkMatchingInsideEdge(insideEdge, another.e1))
            {
                return true;
            }
            //
            if (another.e2.IsInside && MarkMatchingInsideEdge(insideEdge, another.e2))
            {
                //check matching slope and coord?
                return true;
            }
            return false;
        }
        static bool MarkMatchingInsideEdge(EdgeLine p_edge, EdgeLine q_edge)
        {


            if (p_edge.x0 == q_edge.x0 && p_edge.x1 == q_edge.x1)
            {

            }
            else if (p_edge.x0 == q_edge.x1 && p_edge.x1 == q_edge.x0)
            {

            }
            else
            {
                return false; //no match in x-axis
            }
            //--------------------------------
            //y_axis
            if (p_edge.y0 == q_edge.y0 && p_edge.y1 == q_edge.y1)
            {
                p_edge.contactToEdge = q_edge;
            }
            else if (p_edge.y0 == q_edge.y1 && p_edge.y1 == q_edge.y0)
            {
                p_edge.contactToEdge = q_edge;
            }
            else
            {
                return false; //no match in y-axis
            }
            //--------------------------------
            return true;
        }
        static void MarkMatchingOutsideEdge(EdgeLine targetEdge, GlyphTriangle q)
        {

            EdgeLine matchingEdgeLine;
            int matchingEdgeSideNo;
            if (FindMatchingOuterSide(targetEdge, q, out matchingEdgeLine, out matchingEdgeSideNo))
            {
                //assign matching edge line   
                //mid point of each edge
                //p-triangle's edge midX,midY

                var pe = targetEdge.GetMidPoint();
                double pe_midX = pe.X, pe_midY = pe.Y;

                //q-triangle's edge midX,midY
                var qe = matchingEdgeLine.GetMidPoint();
                double qe_midX = qe.X, qe_midY = qe.Y;


                if (targetEdge.SlopeKind == LineSlopeKind.Vertical)
                {
                    //TODO: review same side edge (Fan shape)
                    if (pe_midX < qe_midX)
                    {
                        targetEdge.IsLeftSide = true;
                        if (matchingEdgeLine.IsOutside && matchingEdgeLine.SlopeKind == LineSlopeKind.Vertical)
                        {
                            targetEdge.AddMatchingOutsideEdge(matchingEdgeLine);
                        }
                    }
                    else
                    {
                        //matchingEdgeLine.IsLeftSide = true;
                        if (matchingEdgeLine.IsOutside && matchingEdgeLine.SlopeKind == LineSlopeKind.Vertical)
                        {
                            targetEdge.AddMatchingOutsideEdge(matchingEdgeLine);
                        }
                    }
                }
                else if (targetEdge.SlopeKind == LineSlopeKind.Horizontal)
                {
                    //TODO: review same side edge (Fan shape)

                    if (pe_midY > qe_midY)
                    {
                        //p side is upper , q side is lower
                        if (targetEdge.SlopeKind == LineSlopeKind.Horizontal)
                        {
                            targetEdge.IsUpper = true;
                            if (matchingEdgeLine.IsOutside && matchingEdgeLine.SlopeKind == LineSlopeKind.Horizontal)
                            {
                                targetEdge.AddMatchingOutsideEdge(matchingEdgeLine);
                            }
                        }
                    }
                    else
                    {
                        if (matchingEdgeLine.SlopeKind == LineSlopeKind.Horizontal)
                        {
                            // matchingEdgeLine.IsUpper = true;
                            if (matchingEdgeLine.IsOutside && matchingEdgeLine.SlopeKind == LineSlopeKind.Horizontal)
                            {
                                targetEdge.AddMatchingOutsideEdge(matchingEdgeLine);
                            }
                        }
                    }
                }
            }
        }




        static bool FindMatchingOuterSide(EdgeLine compareEdge,
            GlyphTriangle another,
            out EdgeLine result,
            out int edgeIndex)
        {
            //compare by radian of edge line
            double compareSlope = Math.Abs(compareEdge.SlopAngle);
            double diff0 = double.MaxValue;
            double diff1 = double.MaxValue;
            double diff2 = double.MaxValue;

            diff0 = Math.Abs(Math.Abs(another.e0.SlopAngle) - compareSlope);

            diff1 = Math.Abs(Math.Abs(another.e1.SlopAngle) - compareSlope);

            diff2 = Math.Abs(Math.Abs(another.e2.SlopAngle) - compareSlope);

            //find min
            int minDiffSide = FindMinIndex(diff0, diff1, diff2);
            if (minDiffSide > -1)
            {
                edgeIndex = minDiffSide;
                switch (minDiffSide)
                {
                    default: throw new NotSupportedException();
                    case 0:
                        result = another.e0;
                        break;
                    case 1:
                        result = another.e1;
                        break;
                    case 2:
                        result = another.e2;
                        break;
                }
                return true;
            }
            else
            {
                edgeIndex = -1;
                result = null;
                return false;
            }
        }
        static int FindMinIndex(double d0, double d1, double d2)
        {
            unsafe
            {
                double* tmpArr = stackalloc double[3];
                tmpArr[0] = d0;
                tmpArr[1] = d1;
                tmpArr[2] = d2;

                int minAt = -1;
                double currentMin = double.MaxValue;
                for (int i = 0; i < 3; ++i)
                {
                    double d = tmpArr[i];
                    if (d < currentMin)
                    {
                        currentMin = d;
                        minAt = i;
                    }
                }
                return minAt;
            }
        }
        ///// <summary>
        ///// count number of outside edge
        ///// </summary>
        ///// <param name="t"></param>
        ///// <returns></returns>
        //static int OutSideEdgeCount(GlyphTriangle t)
        //{
        //    int n = 0;
        //    n += t.e0.IsOutside ? 1 : 0;
        //    n += t.e1.IsOutside ? 1 : 0;
        //    n += t.e2.IsOutside ? 1 : 0;
        //    return n;
        //}

        public override string ToString()
        {
            return p + " -> " + q;
        }
    }
}