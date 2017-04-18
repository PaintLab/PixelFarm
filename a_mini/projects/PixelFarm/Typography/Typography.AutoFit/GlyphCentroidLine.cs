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

            List<GlyphCentroidLine> lineList = this.lines;
            int j = lineList.Count;
            for (int i = 0; i < j; ++i)
            {
                //for each centroid line
                //analyze for its bone joint
                lineList[i].AnalyzeAndMarkEdges();
            }
            if (j > 1)
            {

                //add special tip
                //get first line and last 
                //check if this is loop
                GlyphCentroidLine first_line = lineList[0];
                GlyphCentroidLine last_line = lineList[j - 1];
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

                GlyphCentroidLine line = lineList[0];
                AssignTipInfo(line, true);
            }
        }
        static void AssignTipInfo(GlyphCentroidLine line, bool twoside)
        {
            GlyphBoneJoint joint = line.BoneJoint;
            //get another edge for endtip

            if (IsOwnerOf(line.p, joint.TipEdge))
            {
                //tip edge is from p side
                //so another side is q.

                var tipPoint = joint.TipPoint;
                GlyphTip tip = new GlyphTip(line, tipPoint, joint.TipEdge);
                line.P_Tip = tip;

                if (twoside)
                {
                    EdgeLine tipEdge = FindTip(line, line.q);
                    if (tipEdge == null) throw new NotSupportedException();
                    //-----
                    tip = new GlyphTip(line, tipEdge.GetMidPoint(), tipEdge);
                    line.Q_Tip = tip;
                }
            }
            else if (IsOwnerOf(line.q, joint.TipEdge))
            {

                var tipPoint = joint.TipPoint;
                GlyphTip tip = new GlyphTip(line, tipPoint, joint.TipEdge);
                line.Q_Tip = tip;


                //tip edge is from q side
                //so another side is p.

                if (twoside)
                {
                    //find proper tip edge
                    EdgeLine tipEdge = FindTip(line, line.p);
                    if (tipEdge == null)
                    {
                        //some time no tip found ***
                        return;
                    }
                    //-----
                    tip = new GlyphTip(line, tipEdge.GetMidPoint(), tipEdge);
                    line.P_Tip = tip;
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        static EdgeLine FindTip(GlyphCentroidLine line, GlyphTriangle triangle)
        {
            GlyphBoneJoint boneJoint = line.BoneJoint;
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
            //
            return (edge.IsOutside &&
                    edge != compareJoint.RibEndEdgeA &&
                    edge != compareJoint.RibEndEdgeB);
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
            //{
            //    return true;
            //}
            //return false;
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

        /// <summary>
        /// create a set of GlyphBone bone
        /// </summary>
        /// <param name="newlyCreatedBones"></param>
        internal void CreateBones(List<GlyphBone> newlyCreatedBones)
        {
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
                        else
                        {
                            //glyph 'o' -> no tip point
                            if (j > 1)
                            {
                                GlyphCentroidLine nextline = lineList[0];
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


    public class GlyphTip
    {
        public GlyphTip(GlyphCentroidLine ownerLine, Vector2 pos, EdgeLine edge)
        {
            this.OwnerLine = ownerLine;
            this.Pos = pos;
            this.Edge = edge;
        }
        public GlyphCentroidLine OwnerLine { get; set; }
        public Vector2 Pos { get; set; }
        public EdgeLine Edge { get; set; }
    }
    /// <summary>
    /// a line that connects between centroid of 2 GlyphTriangle(p => q)
    /// </summary>
    public class GlyphCentroidLine
    {

        public readonly GlyphTriangle p, q;
        public readonly double boneLength;
        GlyphBoneJoint _boneJoint;

        public GlyphCentroidLine(GlyphTriangle p, GlyphTriangle q)
        {
            this.p = p;
            this.q = q;

            double dy = q.CentroidY - p.CentroidY;
            double dx = q.CentroidX - p.CentroidX;
            this.boneLength = Math.Sqrt((dy * dy) + (dx * dx));
        }
        public bool SpecialConnectFromLastToFirst { get; set; }
        public GlyphBoneJoint BoneJoint { get { return _boneJoint; } }

        public double SlopeAngle { get; private set; }


        public LineSlopeKind SlopeKind { get; private set; }

        /// <summary>
        /// add information about edges to each triangle
        /// </summary>
        internal void AnalyzeAndMarkEdges()
        {
            //[A]
            //each triangle has 1 centroid point
            //a centrod line connects between 2 adjacent triangles via centroid 
            //
            //
            //
            //p triangle=> (x0,y0)  (centroid of p)
            //q triangle=> (x1,y1)  (centroid of q)
            //a centroid line  move from p to q 
            //...
            //tasks:
            //1. find slop angle
            //2. find slope kind
            //3. mark edge info

            //[B]
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
                    //assume
                    SlopeKind = LineSlopeKind.Vertical;
                }
                else if (SlopeAngle < MyMath._03degreeToRad) //_15degreeToRad
                {
                    //assume
                    SlopeKind = LineSlopeKind.Horizontal;
                }
                else
                {
                    SlopeKind = LineSlopeKind.Other;
                }
            }

#if DEBUG
            if (p == q)
            {
                throw new NotSupportedException();
            }
#endif
            //-------------------------------------- 
            //[C]
            //Find relation between edges of 2 triangle p and q
            //....
            //pick up a edge of p and compare to all edge of q
            //do until complete
            AddEdgesInformation(q, p.e0);
            AddEdgesInformation(q, p.e1);
            AddEdgesInformation(q, p.e2);
            //
            AddEdgesInformation(p, q.e0);
            AddEdgesInformation(p, q.e1);
            AddEdgesInformation(p, q.e2);
            //after this process, a boneJoint should be created
            //------------------------------------

            if (_boneJoint != null)
            {
                //a joint has 2 contact edge (they are mathcing edge, but not the same).
                //we add 'information' about other edge compare to the contact edges
                //---
                //both contact edge is INSIDE edge***
                //then, we mark outside edge compare to the known inside edge          
#if DEBUG
                if (_boneJoint._p_contact_edge == _boneJoint._q_contact_edge)
                {
                    throw new NotSupportedException();
                }
#endif
                MarkProperOppositeOutsideEdges(p, _boneJoint, _boneJoint._p_contact_edge);
                MarkProperOppositeOutsideEdges(q, _boneJoint, _boneJoint._q_contact_edge);
            }
        }

        public GlyphTip P_Tip { get; set; }
        public GlyphTip Q_Tip { get; set; }

        /// <summary>
        /// get on curve points 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns>0 =p, 1= q, none = -1, both=2</returns>
        static int GetOnCurvePoints(GlyphPoint2D p, GlyphPoint2D q)
        {
            if (p.kind != PointKind.CurveInbetween && q.kind != PointKind.CurveInbetween)
            {
                //both are ONCurve point
                return 2;
            }
            else if (p.kind != PointKind.CurveInbetween)
            {
                return 0;
            }
            else if (q.kind != PointKind.CurveInbetween)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }


        static void ClassifyTriangleEdges(
            GlyphTriangle triangle,
            EdgeLine knownInsideEdge,
            out EdgeLine anotherInsideEdge,
            out EdgeLine outside0,
            out EdgeLine outside1,
            out EdgeLine outside2,
            out int outsideCount)
        {
            outsideCount = 0;
            outside0 = outside1 = outside2 = anotherInsideEdge = null;

            if (triangle.e0.IsOutside)
            {
                switch (outsideCount)
                {
                    case 0: outside0 = triangle.e0; break;
                    case 1: outside1 = triangle.e0; break;
                    case 2: outside2 = triangle.e0; break;
                }
                outsideCount++;
            }
            else
            {
                //e0 is not known inside edge
                if (triangle.e0 != knownInsideEdge)
                {
                    anotherInsideEdge = triangle.e0;
                }
            }
            //
            if (triangle.e1.IsOutside)
            {
                switch (outsideCount)
                {
                    case 0: outside0 = triangle.e1; break;
                    case 1: outside1 = triangle.e1; break;
                    case 2: outside2 = triangle.e1; break;
                }
                outsideCount++;
            }
            else
            {
                if (triangle.e1 != knownInsideEdge)
                {
                    anotherInsideEdge = triangle.e1;
                }
            }
            //
            if (triangle.e2.IsOutside)
            {
                switch (outsideCount)
                {
                    case 0: outside0 = triangle.e2; break;
                    case 1: outside1 = triangle.e2; break;
                    case 2: outside2 = triangle.e2; break;
                }
                outsideCount++;
            }
            else
            {
                if (triangle.e2 != knownInsideEdge)
                {
                    anotherInsideEdge = triangle.e2;
                }
            }
        }


        static void SelectMostProperTipEdge(GlyphBoneJoint ownerEdgeJoint,
            EdgeLine outside0,
            EdgeLine outside1,
            out EdgeLine tipEdge,
            out EdgeLine notTipEdge)
        {
            GlyphCentroidLine ownerCentroidLine = ownerEdgeJoint.OwnerCentroidLine;
            //p
            double x0 = ownerCentroidLine.p.CentroidX;
            double y0 = ownerCentroidLine.p.CentroidY;
            //q
            double x1 = ownerCentroidLine.q.CentroidX;
            double y1 = ownerCentroidLine.q.CentroidY;

            LineSlopeKind centroidLineSlope = LineSlopeKind.Other;
            double slopeAngle = 0;
            if (x1 == x0)
            {
                centroidLineSlope = LineSlopeKind.Vertical;
                slopeAngle = 1;
            }
            else
            {
                slopeAngle = Math.Abs(Math.Atan2(Math.Abs(y1 - y0), Math.Abs(x1 - x0)));
                if (slopeAngle > MyMath._85degreeToRad)
                {
                    //assume
                    centroidLineSlope = LineSlopeKind.Vertical;
                }
                else if (slopeAngle < MyMath._03degreeToRad) //_15degreeToRad
                {
                    //assume
                    centroidLineSlope = LineSlopeKind.Horizontal;
                }
                else
                {
                    centroidLineSlope = LineSlopeKind.Other;
                }
            }
            //---------------------
            switch (centroidLineSlope)
            {
                default: throw new NotSupportedException();
                case LineSlopeKind.Horizontal:
                    if (outside0.SlopeKind == LineSlopeKind.Vertical)
                    {
                        tipEdge = outside0;
                        notTipEdge = outside1;
                    }
                    else
                    {
                        tipEdge = outside1;
                        notTipEdge = outside0;
                    }
                    break;
                case LineSlopeKind.Vertical:
                    if (outside0.SlopeKind == LineSlopeKind.Horizontal)
                    {
                        tipEdge = outside0;
                        notTipEdge = outside1;
                    }
                    else
                    {
                        tipEdge = outside1;
                        notTipEdge = outside0;
                    }
                    break;
                case LineSlopeKind.Other:
                    //select 1
                    //choose the horizontal one 
                    if (outside0.SlopeKind == LineSlopeKind.Horizontal)
                    {
                        tipEdge = outside0;
                        notTipEdge = outside1;
                    }
                    else
                    {
                        tipEdge = outside1;
                        notTipEdge = outside0;
                    }
                    break;
            }
        }
        /// <summary>
        /// add information about each edge of a triangle, compare to the contactEdge of a ownerEdgeJoint
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="ownerEdgeJoint"></param>
        /// <param name="contactEdge"></param>
        void MarkProperOppositeOutsideEdges(GlyphTriangle triangle, GlyphBoneJoint ownerEdgeJoint, EdgeLine contactEdge)
        {

            int outsideCount;
            EdgeLine outside0, outside1, outside2, anotherInsideEdge;
            ClassifyTriangleEdges(
                triangle,
                contactEdge,
                out anotherInsideEdge,
                out outside0,
                out outside1,
                out outside2,
                out outsideCount);


            //-------------------------------------------------------------------------------------------------------
            //1. check each edge of triangle 
            //if an edge is outside  edge (since it is outside edge, it is not the contactEdge)


            //count represent OUTSIDE edge count, compare to the contactEdge
            //if count ==0 ,=> no outside edge found
            //count==1, => only 1 outside edge
            //count==2, => this is tip of the centroid branch, ***



            //a contact edge has 2 glyph points
            //a GlyphPoint from p triangle=> GlyphPoint_P
            //and
            //a GlyphPoint from q triangle=> GlyphPoint_Q

            switch (outsideCount)
            {
                default: throw new NotSupportedException();
                case 0: break;
                case 3: throw new NotImplementedException();//TODO: implement this 
                case 1:
                    {

                        //----------------------------------------------------------------------------
                        //primary ribs
                        //find shortest part from boneJoint to  edge or to corner.
                        //draw perpendicular line to outside edge
                        //and to the  corner of current edge.
                        GlyphPoint2D p_ = contactEdge.GlyphPoint_P;
                        GlyphPoint2D q_ = contactEdge.GlyphPoint_Q;

                        //TODO: review 
                        switch (GetOnCurvePoints(p_, q_))
                        {
                            default: throw new NotSupportedException();
                            case 2:

                                //both connect with ON-curve point 
                                //select p?
                                p_.AddAssociatedBoneJoint(ownerEdgeJoint);
                                ownerEdgeJoint.AddRibEndAt(contactEdge, new Vector2((float)contactEdge.p.X, (float)contactEdge.p.Y));
                                break;
                            case 0:
                                //select p 
                                p_.AddAssociatedBoneJoint(ownerEdgeJoint);
                                ownerEdgeJoint.AddRibEndAt(contactEdge, new Vector2((float)contactEdge.p.X, (float)contactEdge.p.Y));
                                break;
                            //break;
                            case 1:
                                //select q 
                                q_.AddAssociatedBoneJoint(ownerEdgeJoint);
                                ownerEdgeJoint.AddRibEndAt(contactEdge, new Vector2((float)contactEdge.q.X, (float)contactEdge.q.Y));
                                break;
                            //break;
                            case -1:
                                //both p and q are curve in between
                                break;
                        }
                        //----------------------------------------------------------------------------
                        //seconday ribs: a perpendicular line from edge to the abstract glyph bone
                        //only 1 outside
                        //other is (outside1,2) is inside edge
                        //create a line between mid point of contactEdge (inside) and newly found anotherInsideEdge
                        //this call 'abstract glyph-bone'
#if DEBUG
                        if (anotherInsideEdge == contactEdge)
                        {
                            //should not occur
                            throw new NotSupportedException();
                        }
#endif
                        //create a perpedicular line from midpoint of contact edge to the outside        
                        Vector2 cut1;
                        if (MyMath.FindPerpendicularCutPoint(outside0, contactEdge.GetMidPoint(), out cut1))
                        {
                            outside0.AddCutPoints(
                                cut1,
                                contactEdge.GetMidPoint());
                        }
                    }
                    break;
                case 2:
                    {
                        //tip end 
                        //find which edge should be 'tip edge'                         
                        //in this version we compare each edge slope to centroid line slope.
                        //the most diff angle should be opposite edge (to the centroid) => tip edge
                        //-------------------------------------------------------------------------

                        EdgeLine tipEdge, notTipEdge;
                        SelectMostProperTipEdge(ownerEdgeJoint,
                            outside0,
                            outside1,
                            out tipEdge,
                            out notTipEdge);
                        //for TipEdge
                        ownerEdgeJoint.SetTipEdge(tipEdge);

                        //fot notTipEdge 
                        Vector2 perpend_B;
                        if (MyMath.FindPerpendicularCutPoint(notTipEdge, ownerEdgeJoint.Position, out perpend_B))
                        {
                            ownerEdgeJoint.AddRibEndAt(notTipEdge, perpend_B);
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// add information on each edge of the given triangle compare the given EdgeLine
        /// </summary>
        /// <param name="edgeLine"></param>
        /// <param name="anotherTriangle"></param>
        void AddEdgesInformation(GlyphTriangle anotherTriangle, EdgeLine edgeLine)
        {
            if (edgeLine.IsOutside)
            {
                //if edgeLine is outside edge,
                //mark the relation of this to anotherTriangle.
                MarkMatchingOutsideEdge(edgeLine, anotherTriangle);
            }
            else
            {
                //TODO: review here
                //if edge is inside =>
                //we will evaluate if _boneJoint== null

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
        /// <summary>
        /// from a knownInsideEdge, find a matching-inside-edge on another triangle.
        /// </summary>
        /// <param name="knownInsideEdge"></param>
        /// <param name="another"></param>
        /// <returns></returns>
        static bool MarkMatchingInsideEdge(EdgeLine knownInsideEdge, GlyphTriangle another)
        {

            //evalute side-by-side
            //
            //if the two contact together
            //it must have only 1  contact edge.
            //so ... find side-by-side

            if (MarkMatchingInsideEdge(knownInsideEdge, another.e0) ||
                MarkMatchingInsideEdge(knownInsideEdge, another.e1) ||
                MarkMatchingInsideEdge(knownInsideEdge, another.e2))
            {
                //found!
                return true;
            }
            return false;
        }
        /// <summary>
        /// check if 
        /// </summary>
        /// <param name="knownInsideEdge"></param>
        /// <param name="anotherEdge"></param>
        /// <returns></returns>
        static bool MarkMatchingInsideEdge(EdgeLine knownInsideEdge, EdgeLine anotherEdge)
        {
            //another edge must be inside edge too, then check if the two is matching or not
            if (anotherEdge.IsInside && IsMatchingEdge(knownInsideEdge, anotherEdge))
            {   //if yes                
                //mark contact toEdge
                knownInsideEdge.contactToEdge = anotherEdge;
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// check if the 2 triangle is matching or not
        /// </summary>
        /// <param name="a_edge"></param>
        /// <param name="b_edge"></param>
        /// <returns></returns>
        static bool IsMatchingEdge(EdgeLine a_edge, EdgeLine b_edge)
        {
            //x-axis
            if ((a_edge.x0 == b_edge.x0 && a_edge.x1 == b_edge.x1) ||
                (a_edge.x0 == b_edge.x1 && a_edge.x1 == b_edge.x0))
            {
                //pass x-axis
                //
                //y_axis
                if ((a_edge.y0 == b_edge.y0 && a_edge.y1 == b_edge.y1) ||
                    (a_edge.y0 == b_edge.y1 && a_edge.y1 == b_edge.y0))
                {
                    return true;
                }
            }
            //otherwise...
            return false;
        }
        /// <summary>
        /// analyze relation between each edge of q and knownOutsideEdge.
        /// </summary>
        /// <param name="knownOutsideEdge"></param>
        /// <param name="q"></param>
        static void MarkMatchingOutsideEdge(EdgeLine knownOutsideEdge, GlyphTriangle q)
        {

            EdgeLine matchingEdgeLine;
            int matchingEdgeSideNo;
            if (FindMatchingOuterSide(knownOutsideEdge, q, out matchingEdgeLine, out matchingEdgeSideNo))
            {
                //assign matching edge line   
                //mid point of each edge
                //p-triangle's edge midX,midY

                var pe = knownOutsideEdge.GetMidPoint();
                double pe_midX = pe.X, pe_midY = pe.Y;

                //q-triangle's edge midX,midY
                var qe = matchingEdgeLine.GetMidPoint();
                double qe_midX = qe.X, qe_midY = qe.Y;


                if (knownOutsideEdge.SlopeKind == LineSlopeKind.Vertical)
                {
                    //TODO: review same side edge (Fan shape)
                    if (pe_midX < qe_midX)
                    {
                        knownOutsideEdge.IsLeftSide = true;
                        if (matchingEdgeLine.IsOutside && matchingEdgeLine.SlopeKind == LineSlopeKind.Vertical)
                        {
                            knownOutsideEdge.AddMatchingOutsideEdge(matchingEdgeLine);
                        }
                    }
                    else
                    {
                        //matchingEdgeLine.IsLeftSide = true;
                        if (matchingEdgeLine.IsOutside && matchingEdgeLine.SlopeKind == LineSlopeKind.Vertical)
                        {
                            knownOutsideEdge.AddMatchingOutsideEdge(matchingEdgeLine);
                        }
                    }
                }
                else if (knownOutsideEdge.SlopeKind == LineSlopeKind.Horizontal)
                {
                    //TODO: review same side edge (Fan shape)

                    if (pe_midY > qe_midY)
                    {
                        //p side is upper , q side is lower
                        if (knownOutsideEdge.SlopeKind == LineSlopeKind.Horizontal)
                        {
                            knownOutsideEdge.IsUpper = true;
                            if (matchingEdgeLine.IsOutside && matchingEdgeLine.SlopeKind == LineSlopeKind.Horizontal)
                            {
                                knownOutsideEdge.AddMatchingOutsideEdge(matchingEdgeLine);
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
                                knownOutsideEdge.AddMatchingOutsideEdge(matchingEdgeLine);
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
            int minDiffSide = FindMinByIndex(diff0, diff1, diff2);
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
        /// <summary>
        /// compare d0, d1, d2 return min value by index 0 or 1 or 2
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        static int FindMinByIndex(double d0, double d1, double d2)
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

        public override string ToString()
        {
            return p + " -> " + q;
        }
    }
}