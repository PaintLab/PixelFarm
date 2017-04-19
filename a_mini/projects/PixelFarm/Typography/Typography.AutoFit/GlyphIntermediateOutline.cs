//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using Typography.OpenFont;
using Poly2Tri;
using System.Numerics;

namespace Typography.Rendering
{

    class GlyphIntermediateOutline
    {

        List<GlyphTriangle> _triangles = new List<GlyphTriangle>();
        List<GlyphContour> _contours;
#if DEBUG
        Polygon _dbugpolygon;
#endif 
        public GlyphIntermediateOutline(Polygon polygon, List<GlyphContour> contours)
        {
            this._contours = contours;
#if DEBUG
            this._dbugpolygon = polygon; //for debug only ***
            EdgeLine.s_dbugTotalId = 0;//reset
#endif

            //1. Generate GlyphTriangle triangle from DelaunayTriangle 
            foreach (DelaunayTriangle delnTri in polygon.Triangles)
            {
                delnTri.MarkAsActualTriangle();
                _triangles.Add(new GlyphTriangle(delnTri));
            }
            //2. 
            Analyze();
        }

        Dictionary<GlyphTriangle, CentroidLineHub> centroidLineHubs;
        List<CentroidLineHub> lineHubs;
        List<GlyphBone> outputVerticalLongBones;

        void Analyze()
        {
            //we analyze each triangle here 
            int triCount = _triangles.Count;

            //-------------------------------------------------
            //1. create a list of CentroidLineHub (and its members)
            //-------------------------------------------------           
            centroidLineHubs = new Dictionary<GlyphTriangle, CentroidLineHub>();
            CentroidLineHub currentCentroidLineHub = null;
            //2. 
            List<GlyphTriangle> usedTriList = new List<GlyphTriangle>();
            GlyphTriangle latestTri = null;

            //we may walk forward and backward on each tri
            //so we record the used triangle into a usedTriList.

            for (int i = 0; i < triCount; ++i)
            {
                GlyphTriangle tri = _triangles[i];
                if (i == 0)
                {
                    centroidLineHubs[tri] = currentCentroidLineHub = new CentroidLineHub(tri);
                    usedTriList.Add(latestTri = tri);
                }
                else
                {
                    //at a branch 
                    //one tri may connect with 3 NB triangle
                    int foundIndex = FindLatestConnectedTri(usedTriList, tri);
                    if (foundIndex < 0)
                    {
                        //?
                        throw new NotSupportedException();
                    }
                    else
                    {
                        //record used triangle
                        usedTriList.Add(tri);

                        GlyphTriangle connectWithPrevTri = usedTriList[foundIndex];
                        if (connectWithPrevTri != latestTri)
                        {
                            //branch
                            CentroidLineHub lineHub;
                            if (!centroidLineHubs.TryGetValue(connectWithPrevTri, out lineHub))
                            {
                                //if not found then=> //start new CentroidLineHub 
                                centroidLineHubs[connectWithPrevTri] = lineHub = new CentroidLineHub(connectWithPrevTri);
                            }
                            else
                            {
                                //this is multiple facets triangle for  CentroidLineHub
                            }

                            currentCentroidLineHub = lineHub;
                            //ensure start triangle of the branch
                            lineHub.SetBranch(tri);
                            //create centroid line and add to currrent hub
                            var centroidLine = new GlyphCentroidLine(connectWithPrevTri, tri);
                            currentCentroidLineHub.AddChild(centroidLine);
                        }
                        else
                        {
                            //add centroid line to current multifacet joint 
                            if (currentCentroidLineHub.BranchCount == 0)
                            {
                                //ensure start triangle of the branch
                                currentCentroidLineHub.SetBranch(tri);
                            }
                            //create centroid line and add to currrent hub
                            currentCentroidLineHub.AddChild(new GlyphCentroidLine(connectWithPrevTri, tri));
                        }
                        latestTri = tri;
                    }
                }
            }
            //-------------------------------------------------

            if (triCount > 2)
            {
                //connect the last tri to the first tri
                //if it is connected
                GlyphTriangle firstTri = _triangles[0];
                GlyphTriangle lastTri = _triangles[triCount - 1];
                if (firstTri.IsConnectedWith(lastTri))
                {
                    currentCentroidLineHub.AddChild(new GlyphCentroidLine(lastTri, firstTri) { SpecialConnectFromLastToFirst = true });
                }
            }
            //----------------------------------------
            //collect all line hub into a lineHubs list
            lineHubs = new List<CentroidLineHub>(centroidLineHubs.Values.Count);
            foreach (CentroidLineHub hub in centroidLineHubs.Values)
            {

                hub.AnalyzeEachBranchForEdgeInfo();
                lineHubs.Add(hub);
            }
            //----------------------------------------
            //link each hub start point
            //----------------------------------------
            List<GlyphBone> newBones = new List<GlyphBone>();
            foreach (CentroidLineHub hub in centroidLineHubs.Values)
            {
                //create bone and add into newBones list
                hub.CreateBones(newBones);
            }
            //----------------------------------------


            //----------------------------------------
            int lineHubCount = lineHubs.Count;
            for (int i = 0; i < lineHubCount; ++i)
            {
                //after create bone
                //link each hub to proper bone
                FindStartHubLinkConnection(lineHubs[i], lineHubs);
            }
            //------------- 
            outputVerticalLongBones = new List<GlyphBone>();
            AnalyzeBoneLength(newBones, outputVerticalLongBones);
            //------------- 
            //create perpendicular line link from control nodes to glyph bone 
            //----------------------------------------
            outputVerticalLongBones.Sort((b0, b1) => b0.LeftMostPoint().CompareTo(b1.LeftMostPoint()));
            //----------------------------------------

            //connect glyph part/contour with bone
            //----------------------------------------
            //this is optional ?...
            //AnalyzeLinksToGlyphJoints();
            //----------------------------------------
        }
        void AnalyzeLinksToGlyphJoints()
        {

            int j = _contours.Count;
            Dictionary<object, TempSqLengthResult> tempSqLenDic = new Dictionary<object, TempSqLengthResult>();
            for (int i = 0; i < j; ++i)
            {
                GlyphContour cont = _contours[i];
                List<GlyphPoint2D> flattenPoints = cont.flattenPoints;
                int p_count = flattenPoints.Count;
                for (int n = 0; n < p_count; ++n)
                {
                    //from point, add assoc joint

                    GlyphPoint2D glyphPoint = flattenPoints[n];
                    System.Numerics.Vector2 glyph_point_xy = new System.Numerics.Vector2((float)glyphPoint.x, (float)glyphPoint.y);
                    if (glyphPoint.kind != PointKind.CurveInbetween)
                    {
                        tempSqLenDic.Clear();
                        List<GlyphBoneJoint> assocJoints = glyphPoint._assocJoints;
                        if (assocJoints != null)
                        {
                            //create a perpendicular line to 
                            //shortest glyph bone 
                            int jointCount = assocJoints.Count;
                            for (int m = 0; m < jointCount; ++m)
                            {
                                GlyphBoneJoint joint = assocJoints[m];
                                if (tempSqLenDic.ContainsKey(joint))
                                {
                                    continue;
                                }
                                //-----------
                                //distance to joint
                                {
                                    TempSqLengthResult result = new TempSqLengthResult();
                                    result.joint = joint;
                                    result.cutPoint = joint.Position;
                                    result.sq_distance = MyMath.SquareDistance(result.cutPoint, glyph_point_xy);
                                    tempSqLenDic.Add(joint, result);
                                }
                                //-----------
                                //distance to associate bone
                                List<GlyphBone> bones = joint._assocBones;
                                int b_count = bones.Count;
                                for (int b = 0; b < b_count; ++b)
                                {
                                    GlyphBone bone = bones[b];
                                    if (tempSqLenDic.ContainsKey(bone))
                                    {
                                        continue;
                                    }
                                    TempSqLengthResult result = new TempSqLengthResult();
                                    result.bone = bone;

                                    if (MyMath.FindPerpendicularCutPoint(bone, glyph_point_xy, out result.cutPoint))
                                    {
                                        result.sq_distance = MyMath.SquareDistance(result.cutPoint, glyph_point_xy);
                                        tempSqLenDic.Add(bone, result);
                                    }
                                }
                            }



                            double shortest = double.MaxValue;
                            TempSqLengthResult shortestResult = new TempSqLengthResult();

                            bool foundSomeResult = false; //for debug
                            foreach (TempSqLengthResult r in tempSqLenDic.Values)
                            {
                                //find shortest  
                                if (r.sq_distance < shortest)
                                {
                                    shortest = r.sq_distance;
                                    shortestResult = r;
                                    foundSomeResult = true;
                                }

                            }
                            //---------
                            if (!foundSomeResult)
                            {
                                throw new NotSupportedException();
                            }
                            //--------- 
                            foreach (TempSqLengthResult r in tempSqLenDic.Values)
                            {

                                if (r.bone != null)
                                {
                                    r.bone.AddPerpendicularPoint(glyphPoint, r.cutPoint);
                                }
                            }
                            if (shortestResult.joint != null)
                            {
                                shortestResult.joint.AddAssociatedGlyphPoint(glyphPoint);
                            }
                        }

                    }
                }
            }
        }
        struct TempSqLengthResult
        {
            public System.Numerics.Vector2 cutPoint;
            public double sq_distance;
            public GlyphBone bone;
            public GlyphBoneJoint joint;
        }

        public List<GlyphBone> LongVerticalBones
        {
            get { return this.outputVerticalLongBones; }
        }

        static void FindStartHubLinkConnection(CentroidLineHub analyzingHub, List<CentroidLineHub> hubs)
        {
            int j = hubs.Count;
            System.Numerics.Vector2 hubHeadPos = analyzingHub.GetCenterPos();
            for (int i = 0; i < j; ++i)
            {

                CentroidLineHub hub = hubs[i];
                if (hub == analyzingHub)
                {
                    continue;
                }
                GlyphCentroidBranch foundOnBr;
                GlyphBoneJoint foundOnJoint;

                if (hub.FindBoneJoint(analyzingHub.MainTriangle, hubHeadPos, out foundOnBr, out foundOnJoint))
                {
                    //found
                    hub.AddLineHubConnection(analyzingHub);
                    //
                    analyzingHub.SetHeadConnnection(foundOnBr, foundOnJoint);
                    return;
                }
            }
        }

        static void AnalyzeBoneLength(List<GlyphBone> newBones, List<GlyphBone> outputVerticalLongBones)
        {
            //----------------------------------------
            //collect major bones
            newBones.Sort((b0, b1) => b0.Length.CompareTo(b1.Length));
            //----------------------------------------
            //find exact glyph bounding box
            //median
            int n = newBones.Count;
            GlyphBone medianBone = newBones[n / 2];
            //classify bone len

            double medianLen = medianBone.Length;
            double median_x2 = medianLen + medianLen;
            //----------------------------------------

            int boneCount = newBones.Count;
            for (int i = boneCount - 1; i >= 0; --i)
            {
                GlyphBone b = newBones[i];
                if (b.Length >= median_x2)
                {
                    b.IsLongBone = true;
                    if (b.SlopeKind == LineSlopeKind.Vertical)
                    {
                        outputVerticalLongBones.Add(b);
                    }
                }
                else
                {
                    //since all bones are sorted
                    //not need to go more
                    break;
                }
            }
            //----------------------------------------
        }

        static int FindLatestConnectedTri(List<GlyphTriangle> usedTriList, GlyphTriangle tri)
        {
            //search back ***
            for (int i = usedTriList.Count - 1; i >= 0; --i)
            {
                if (usedTriList[i].IsConnectedWith(tri))
                {
                    return i;
                }
            }
            return -1;
        }
        public List<GlyphTriangle> GetTriangles()
        {
            return _triangles;
        }
        public Dictionary<GlyphTriangle, CentroidLineHub> GetCentroidLineHubs()
        {
            return this.centroidLineHubs;
        }

        public List<GlyphContour> GetContours()
        {
            return this._contours;
        }
        public float LeftControlPosX { get; set; }
    }

}