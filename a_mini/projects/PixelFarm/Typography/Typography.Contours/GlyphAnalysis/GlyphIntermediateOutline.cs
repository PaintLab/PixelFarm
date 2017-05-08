//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using Poly2Tri;

namespace Typography.Contours
{

    class GlyphIntermediateOutline
    {

        List<GlyphContour> _contours;
        List<CentroidLineHub> _lineHubs;


        float _bounds_minX, _bounds_minY, _bounds_maxX, _bounds_maxY;

        public GlyphIntermediateOutline(Polygon polygon, List<GlyphContour> contours)
        {
            //init value
            _bounds_minX = _bounds_minY = float.MaxValue;
            _bounds_maxX = _bounds_maxY = float.MinValue;


            this._contours = contours;

            //1. create centroid line hubs: 
            CreateCentroidLineHubs(polygon);
            //2. create bone joints (create joint before bone)
            CreateBoneJoints();
            //3. create bones 
            CreateBones();
            //4. create glyph edges          
            CreateGlyphEdges();
        }

        void CreateCentroidLineHubs(Polygon polygon)
        {

            List<GlyphTriangle> triangles = new List<GlyphTriangle>();
#if DEBUG
            this._dbugpolygon = polygon; //for debug only ***
            EdgeLine.s_dbugTotalId = 0;//reset
            this._dbugTriangles = triangles;
#endif

            //create triangle list from given DelaunayTriangle polygon.
            // Generate GlyphTriangle triangle from DelaunayTriangle 
            foreach (DelaunayTriangle delnTri in polygon.Triangles)
            {
                delnTri.MarkAsActualTriangle();
                triangles.Add(new GlyphTriangle(delnTri)); //all triangles are created from Triangulation process
            }

            //----------------------------
            //create centroid line hub
            //----------------------------
            //1.
            var centroidLineHubs = new Dictionary<GlyphTriangle, CentroidLineHub>();
            CentroidLineHub currentCentroidLineHub = null;
            //2. temporary list of used triangles
            List<GlyphTriangle> usedTriList = new List<GlyphTriangle>();
            GlyphTriangle latestTri = null;

            //we may walk forward and backward on each tri
            //so we record the used triangle into a usedTriList.
            int triCount = triangles.Count;
            for (int i = 0; i < triCount; ++i)
            {
                GlyphTriangle tri = triangles[i];
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
                                //create linehub to line hub connection
                                //TODO: review here 
                                //create centroid pair at link point 
                            }
                            else
                            {
                                //this is multiple facets triangle for  CentroidLineHub
                            }

                            currentCentroidLineHub = lineHub;
                            //ensure start triangle of the branch
                            lineHub.SetCurrentCentroidLine(tri);
                            //create centroid line and add to currrent hub 
                            currentCentroidLineHub.AddCentroidPair(new GlyphCentroidPair(connectWithPrevTri, tri));
                        }
                        else
                        {
                            //add centroid line to current multifacet joint 
                            if (currentCentroidLineHub.LineCount == 0)
                            {
                                //ensure start triangle of the branch
                                currentCentroidLineHub.SetCurrentCentroidLine(tri);
                            }
                            //create centroid line and add to currrent hub
                            currentCentroidLineHub.AddCentroidPair(new GlyphCentroidPair(connectWithPrevTri, tri));
                        }
                        latestTri = tri;
                    }
                }
            }

            //--------------------------------------------------------------
            //copy to list, we not use the centroidLineHubs anymore
            _lineHubs = new List<CentroidLineHub>(centroidLineHubs.Values);

        }
        void CreateBoneJoints()
        {
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
            //check connection between head of each centroid line
            for (int i = 0; i < lineHubCount; ++i)
            {
                _lineHubs[i].CreateBoneLinkBetweenCentroidLine(newBones);
            }
        }

        void CreateGlyphEdges()
        {

            List<GlyphContour> contours = this._contours;
            int j = contours.Count;
            for (int i = 0; i < j; ++i)
            {
                GlyphContour cnt = contours[i];
                cnt.CreateGlyphEdges();
                //
                cnt.FindBounds(ref _bounds_minX, ref _bounds_minY, ref _bounds_maxX, ref _bounds_maxY);
            }
        }

        public float MinX { get { return _bounds_minX; } }
        public float MinY { get { return _bounds_minY; } }
        public float MaxX { get { return _bounds_maxX; } }
        public float MaxY { get { return _bounds_maxY; } }

        /// <summary>
        /// find link from main triangle of line-hub to another line hub
        /// </summary>
        /// <param name="analyzingHub"></param>
        /// <param name="hubs"></param>
        static void LinkEachLineHubTogether(CentroidLineHub analyzingHub, List<CentroidLineHub> hubs)
        {
            int j = hubs.Count;
            for (int i = 0; i < j; ++i)
            {
                CentroidLineHub otherHub = hubs[i];
                if (otherHub == analyzingHub)
                {
                    continue;
                }

                CentroidLine foundOnBr;
                GlyphBoneJoint foundOnJoint;
                //from a given hub,
                //find bone joint that close to the main triangle for of the analyzingHub
                if (otherHub.FindBoneJoint(analyzingHub.StartTriangle, out foundOnBr, out foundOnJoint))
                {
                    //create a new bone joint 
                    // FindNearestEdge(analyzingHub.MainTriangle, foundOnJoint); 
                    //add connection from analyzingHub to otherHub
                    otherHub.AddLineHubConnection(analyzingHub);
                    //also set head connection from joint to this analyzing hub
                    analyzingHub.SetHeadConnnection(foundOnBr, foundOnJoint);
                    return;
                }
            }
        }

        static int FindLatestConnectedTri(List<GlyphTriangle> usedTriList, GlyphTriangle tri)
        {
            //search back ***
            for (int i = usedTriList.Count - 1; i >= 0; --i)
            {
                if (usedTriList[i].IsConnectedTo(tri))
                {
                    return i;
                }
            }
            return -1;
        }

        public List<CentroidLineHub> GetCentroidLineHubs()
        {
            return this._lineHubs;
        }

        public List<GlyphContour> GetContours()
        {
            return this._contours;
        }


#if DEBUG

        Polygon _dbugpolygon;
        List<GlyphTriangle> _dbugTriangles;
        public List<GlyphTriangle> dbugGetTriangles()
        {
            return _dbugTriangles;
        }
#endif
    }

}