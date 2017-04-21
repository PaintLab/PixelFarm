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
        List<CentroidLineHub> _lineHubs;
        List<GlyphBone> _outputVerticalLongBones;
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
            //--------------------
            //analyze ....
            //--------------------
            //1. Generate GlyphTriangle triangle from DelaunayTriangle 
            foreach (DelaunayTriangle delnTri in polygon.Triangles)
            {
                delnTri.MarkAsActualTriangle();
                _triangles.Add(new GlyphTriangle(delnTri)); //all triangles are created from Triangulation process
            }

            //2. create centroid line hubs
            CreateCentroidLineHubs();
            //3. create bone joints
            CreateBoneJoints();
            //4. create bones 
            CreateBones();
            //5. create glyph edges          
            CreateGlyphEdges();
        }

        void CreateCentroidLineHubs()
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
            _lineHubs = new List<CentroidLineHub>(centroidLineHubs.Values);
        }
        void CreateBoneJoints()
        {
            //----------------------------------------
            int lineHubCount = _lineHubs.Count;
            for (int i = 0; i < lineHubCount; ++i)
            {
                _lineHubs[i].CreateBoneJoints();
            }
            for (int i = 0; i < lineHubCount; ++i)
            {
                //after create bone joint ***                 
                LinkEachLineHubTogether(_lineHubs[i], _lineHubs);
            }
        }
        void CreateBones()
        {
            List<GlyphBone> newBones = new List<GlyphBone>();
            int lineHubCount = _lineHubs.Count;
            for (int i = 0; i < lineHubCount; ++i)
            {
                //create bones and collect back to newBones                
                _lineHubs[i].CreateBones(newBones);
            }
            //----------------------------------------
            _outputVerticalLongBones = new List<GlyphBone>();
            AnalyzeBoneLength(newBones, _outputVerticalLongBones);
            //create perpendicular line link from control nodes to glyph bone 
            //----------------------------------------
            _outputVerticalLongBones.Sort((b0, b1) => b0.LeftMostPoint().CompareTo(b1.LeftMostPoint()));

        }

        void CreateGlyphEdges()
        {
            List<GlyphContour> contours = this._contours;
            int j = contours.Count;
            for (int i = 0; i < j; ++i)
            {
                contours[i].CreateGlyphEdges();
            }
        }
        public List<GlyphBone> LongVerticalBones
        {
            get { return this._outputVerticalLongBones; }
        }

        /// <summary>
        /// find link from main triangle of line-hub to another line hub
        /// </summary>
        /// <param name="analyzingHub"></param>
        /// <param name="hubs"></param>
        static void LinkEachLineHubTogether(CentroidLineHub analyzingHub, List<CentroidLineHub> hubs)
        {
            int j = hubs.Count;
            System.Numerics.Vector2 hubHeadPos = analyzingHub.GetCenterPos();
            for (int i = 0; i < j; ++i)
            {
                CentroidLineHub otherHub = hubs[i];
                if (otherHub == analyzingHub)
                {
                    continue;
                }

                GlyphCentroidLine foundOnBr;
                GlyphBoneJoint foundOnJoint;

                //from a given hub,
                //find bone joint that close to the main triangle for of the analyzingHub
                if (otherHub.FindBoneJoint(analyzingHub.MainTriangle, hubHeadPos, out foundOnBr, out foundOnJoint))
                {
                    //add connection from analyzingHub to otherHub
                    otherHub.AddLineHubConnection(analyzingHub);
                    //also set head connection from joint to this analyzing hub
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
            return this._lineHubs;
        }

        public List<GlyphContour> GetContours()
        {
            return this._contours;
        }
        public float LeftControlPos { get; set; }
    }

}