//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using Poly2Tri;

namespace Typography.Rendering
{

    class GlyphIntermediateOutline
    {


        List<GlyphTriangle> _triangles = new List<GlyphTriangle>();
        List<GlyphContour> _contours;
        //
        List<CentroidLineHub> lineHubs;
        List<GlyphBone> outputVerticalLongBones;
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
                _triangles.Add(new GlyphTriangle(delnTri)); //all triangles are created from Triangulation process
            }

            //2. 
            Analyze();
        }

        void Analyze()
        {

            //----------------------------
            //create centroid line hub
            //----------------------------
            var centroidLineHubs = new Dictionary<GlyphTriangle, CentroidLineHub>();
            CentroidLineHub currentCentroidLineHub = null;
            //2. 
            List<GlyphTriangle> usedTriList = new List<GlyphTriangle>();
            GlyphTriangle latestTri = null;

            //we may walk forward and backward on each tri
            //so we record the used triangle into a usedTriList.
            int triCount = _triangles.Count;
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
                            var pair = new GlyphCentroidPair(connectWithPrevTri, tri);
                            currentCentroidLineHub.AddChild(pair);
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
                            currentCentroidLineHub.AddChild(new GlyphCentroidPair(connectWithPrevTri, tri));
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
                    currentCentroidLineHub.AddChild(new GlyphCentroidPair(lastTri, firstTri) { SpecialConnectFromLastToFirst = true });
                }
            }
            //----------------------------------------
            //collect all CentroidLineHub into a lineHubs list
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
            //iterate each contour's point again
            //create relation 

            //----------------------------------------
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
                GlyphCentroidLine foundOnBr;
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
        public List<CentroidLineHub> GetCentroidLineHubs()
        {
            return this.lineHubs;
        }

        public List<GlyphContour> GetContours()
        {
            return this._contours;
        }
        public float LeftControlPos { get; set; }
    }

}