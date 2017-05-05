//MIT, 2017, WinterDev
using System;
using System.Numerics;
using System.Collections.Generic;
using Typography.OpenFont;

namespace Typography.Rendering
{

    public class GlyphDynamicOutline
    {

        internal List<GlyphContour> _contours;
        List<GlyphBone> _longVerticalBones;
        List<CentroidLineHub> _centroidLineHubs;
        List<CentroidLine> _allCentroidLines;
        List<GlyphBone> _allBones;
        /// <summary>
        /// offset in pixel unit from master outline, accept + and -
        /// </summary>
        float _offsetFromMasterOutline = 0; //pixel unit
        float pxScale;


        internal GlyphDynamicOutline(GlyphIntermediateOutline intermediateOutline)
        {

            this.GridBoxWidth = 1; //pixels
            this.GridBoxHeight = 50; //pixels
#if DEBUG
            this.GridBoxHeight = dbugGridHeight; //pixels
            _dbugTempIntermediateOutline = intermediateOutline;
#endif

            //we convert data from GlyphIntermediateOutline to newform (lightweight form).
            //and save it here.
            //1. joints and its bones
            //2. bones and its controlled edge 
            _contours = intermediateOutline.GetContours(); //original contours
            _longVerticalBones = intermediateOutline.LongVerticalBones; //analyzed long bones
            _centroidLineHubs = intermediateOutline.GetCentroidLineHubs();

            LoadGlyphBones();

            //--------
            LeftControlPosX = intermediateOutline.LeftControlPos; //left control position  
        }
        void LoadGlyphBones()
        {
            _allBones = new List<GlyphBone>();
            _allCentroidLines = new List<CentroidLine>();
            int j = _centroidLineHubs.Count;
            for (int i = 0; i < j; ++i)
            {
                CentroidLineHub hub = _centroidLineHubs[i];
                foreach (CentroidLine line in hub.GetAllCentroidLines().Values)
                {
                    _allCentroidLines.Add(line);
                    //*** 
                    //all bones in a centroid
                    _allBones.AddRange(line.bones);
                }
            }
        }

        public void ApplyGridToMasterOutline(int gridBoxW, int gridBoxH)
        {
            this.GridBoxHeight = gridBoxH;
            this.GridBoxWidth = gridBoxW;


            int centroidLineCount = _allCentroidLines.Count;

            for (int i = 0; i < centroidLineCount; ++i)
            {
                CentroidLine line = _allCentroidLines[i];
                //apply new grid to this centroid line
                line.ApplyGridBox(gridBoxW, gridBoxH);
            }


            //apply to line hub
            for (int i = 0; i < centroidLineCount; ++i)
            {
                CentroidLine line = _allCentroidLines[i];
                List<List<EdgeLine>> sel_H_edges = line.selectedHorizontalEdges;
                if (sel_H_edges != null)
                {
                    //apply gridBoxW and H per 'stem group'
                    int n = sel_H_edges.Count;
                    for (int m = 0; m < n; ++m)
                    {
                        //all edge lines in this list are
                        //selected horizontal edges
                        List<EdgeLine> h_edges = sel_H_edges[m];
                        int edgeCount = h_edges.Count;

                        //we need to calculate the avg of the glyph point
                        //and add a total summary to this
                        float diffTotal = 0;

                        float min_diff = float.MaxValue;
                        float max_diff = float.MinValue;

                        for (int e = 0; e < edgeCount; ++e)
                        {
                            EdgeLine ee = h_edges[e];
                            GlyphPoint p_pnt = ee.GlyphPoint_P;
                            GlyphPoint q_pnt = ee.GlyphPoint_Q;

                            //this version we focus on vertical hint only 

                            float floorRemaining, diff;
                            MyMath.FitToGrid2(p_pnt.y * pxScale, 1, out floorRemaining, out diff);
                            if (diff < min_diff)
                            {
                                min_diff = diff;
                            }
                            if (diff > max_diff)
                            {
                                max_diff = diff;
                            }
                            diffTotal += Math.Abs(diff);
                            //
                            //evaluate diff
                            //
                            MyMath.FitToGrid2(q_pnt.y * pxScale, 1, out floorRemaining, out diff);
                            if (diff < min_diff)
                            {
                                min_diff = diff;
                            }
                            if (diff > max_diff)
                            {
                                max_diff = diff;
                            }
                            diffTotal += Math.Abs(diff);
                        }

                        float avg_ydiff = diffTotal / (edgeCount * 2);
                        //compare abs max /min 

                        if (Math.Abs(min_diff) > max_diff && min_diff < 0)
                        {
                            avg_ydiff = -avg_ydiff;
                        }


                        //distribute all adjust value to specific glyph points
                        for (int e = 0; e < edgeCount; ++e)
                        {
                            EdgeLine ee = h_edges[e];
                            GlyphPoint p_pnt = ee.GlyphPoint_P;
                            GlyphPoint q_pnt = ee.GlyphPoint_Q;
                            p_pnt.fit_NewX = p_pnt.x * pxScale;
                            p_pnt.fit_NewY = (p_pnt.y * pxScale) + avg_ydiff;
                            p_pnt.fit_analyzed = true;
                            //
                            q_pnt.fit_NewX = q_pnt.x * pxScale;
                            q_pnt.fit_NewY = (q_pnt.y * pxScale) + avg_ydiff;
                            q_pnt.fit_analyzed = true;
                            //
                        }


#if DEBUG
                        //if ((diffTotal / (edgeCount * 2)) > 1)
                        //{

                        //}
                        //Console.WriteLine("ydiff:" + m + " =" + (diffTotal / (edgeCount * 2)));
#endif
                    }
                }
            }


        }

        //public void ApplyGridToMasterOutline(int gridBoxW, int gridBoxH)
        //{
        //    this.GridBoxHeight = gridBoxH;
        //    this.GridBoxWidth = gridBoxW;


        //    int centroidLineCount = _allCentroidLines.Count;
        //    //apply to line hub
        //    for (int i = 0; i < centroidLineCount; ++i)
        //    {
        //        CentroidLine line = _allCentroidLines[i];
        //        List<List<EdgeLine>> sel_H_edges = line.selectedHorizontalEdges;
        //        if (sel_H_edges != null)
        //        {
        //            //apply gridBoxW and H per 'stem group'
        //            int n = sel_H_edges.Count;
        //            for (int m = 0; m < n; ++m)
        //            {
        //                //all edge lines in this list are
        //                //selected horizontal edges
        //                List<EdgeLine> h_edges = sel_H_edges[m];
        //                int edgeCount = h_edges.Count;
        //                for (int e = 0; e < edgeCount; ++e)
        //                {

        //                }
        //            }
        //        }
        //    }
        //    //---------------------------------------------
        //    //fit bone to grid 
        //    int j = _allBones.Count;
        //    for (int i = 0; i < j; ++i)
        //    {
        //        GlyphBone bone = _allBones[i];
        //        GlyphBoneJoint jointA = bone.JointA;
        //        Vector2 jointPos = jointA.Position;
        //        jointA.SetFitXY(MyMath.FitToGrid(jointPos.X, gridBoxW), MyMath.FitToGrid(jointPos.Y, gridBoxH));
        //        if (bone.JointB != null)
        //        {
        //            GlyphBoneJoint jointB = bone.JointB;
        //            jointPos = jointB.Position;
        //            jointB.SetFitXY(MyMath.FitToGrid(jointPos.X, gridBoxW), MyMath.FitToGrid(jointPos.Y, gridBoxH));
        //        }
        //        else
        //        {
        //            //this is tip
        //            //add information about tip too 
        //        }
        //    }
        //    //--------------------------------------------
        //    //after all bones are fit, then => add fit hint to each contour
        //    List<GlyphContour> cnts = _contours;
        //    j = cnts.Count;

        //    for (int i = 0; i < j; ++i)
        //    {
        //        cnts[i].ApplyFitPositions();
        //    }
        //}
        /// <summary>
        /// new stroke width offset from master outline
        /// </summary>
        /// <param name="offsetFromMasterOutline"></param>
        public void SetNewEdgeOffsetFromMasterOutline(float offsetFromMasterOutline)
        {
            //preserve original outline
            //regenerate outline from original outline
            // if (_relativeStrokeWidth == relativeStrokeWidth) { return; }
            //----------------------------------------------------------
            this._offsetFromMasterOutline = offsetFromMasterOutline;

            if (offsetFromMasterOutline != 0)
            {
                //if 0, new other action
                List<GlyphContour> cnts = _contours;
                int j = cnts.Count;
                for (int i = 0; i < j; ++i)
                {
                    cnts[i].ApplyNewEdgeOffsetFromMasterOutline(offsetFromMasterOutline);
                }
            }
        }

        public float LeftControlPosX { get; set; }
        /// <summary>
        /// grid box width in pixels
        /// </summary>
        public int GridBoxWidth { get; private set; }
        /// <summary>
        /// grid box height in pixels
        /// </summary>
        public int GridBoxHeight { get; private set; }



#if DEBUG
        public static bool dbugTestNewGridFitting { get; set; }
        public static int dbugGridHeight = 50;
#endif


        void GenerateOutput(IGlyphTranslator tx, float pxScale)
        {
            this.pxScale = pxScale;

            List<GlyphContour> contours = this._contours;
            int j = contours.Count;

#if DEBUG
            s_dbugAffectedPoints.Clear();
            s_dbugAff2.Clear();
#endif
            List<List<Vector2>> genPointList = new List<List<Vector2>>();
            for (int i = 0; i < j; ++i)
            {
                //generate vector list for each contour
                List<Vector2> outputPoints = new List<Vector2>();
                GenerateNewFitPoints(outputPoints,
                    contours[i], pxScale,
                    false, true, false);
                genPointList.Add(outputPoints);
            }

            //-------------
            //TEST:
            //fit make the glyph look sharp
            //we try to adjust the vertical bone to fit 
            //the pixel (prevent blur) 

            j = genPointList.Count;
            double minorOffset = 0;
            LeftControlPosX = 0;
            int longBoneCount = 0;
            if (_longVerticalBones != null && (longBoneCount = _longVerticalBones.Count) > 0)
            {
                ////only longest bone 
                //the first one is the longest bone.
                GlyphBone longVertBone = _longVerticalBones[0];
                var leftTouchPos = longVertBone.LeftMostPoint();
                LeftControlPosX = leftTouchPos;
            }
            else
            {
                //no vertical long bone
                //so we need left most point
                float leftmostX = FindLeftMost(genPointList);
                LeftControlPosX = leftmostX;
            }
            //-------------

            tx.BeginRead(j);
            for (int i = 0; i < j; ++i)
            {
                GenerateFitOutput(tx, genPointList[i], contours[i]);
            }
            tx.EndRead();
            //-------------
        }

        public void GenerateOutput2(IGlyphTranslator tx, float pxScale)
        {
            this.pxScale = pxScale;
            //-------------------------------------------------
            if (!dbugTestNewGridFitting)
            {
                if (_offsetFromMasterOutline == 0)
                {
                    //gen with anohter methods
                    GenerateOutput(tx, pxScale);
                    return;
                }
            }
            else
            {
                //test grid fitting
                ApplyGridToMasterOutline(GridBoxWidth, GridBoxHeight);
            }
            //-------------------------------------------------
            List<GlyphContour> contours = this._contours;
            int j = contours.Count;
#if DEBUG
            s_dbugAffectedPoints.Clear();
            s_dbugAff2.Clear();
#endif

            LeftControlPosX = 0;
            tx.BeginRead(j);
            for (int i = 0; i < j; ++i)
            {
                GenerateFitOutput3(tx, pxScale, contours[i]);
            }
            tx.EndRead();
            //-------------
        }
        const int GRID_SIZE = 1;
        const float GRID_SIZE_25 = 1f / 4f;
        const float GRID_SIZE_50 = 2f / 4f;
        const float GRID_SIZE_75 = 3f / 4f;

        const float GRID_SIZE_33 = 1f / 3f;
        const float GRID_SIZE_66 = 2f / 3f;

        static float FindLeftMost(List<List<Vector2>> genPointList)
        {
            //find left most x value
            float min = float.MaxValue;
            for (int i = genPointList.Count - 1; i >= 0; --i)
            {
                //new contour
                List<Vector2> genPoints = genPointList[i];
                for (int m = genPoints.Count - 1; m >= 0; --m)
                {
                    Vector2 p = genPoints[m];
                    if (p.X < min)
                    {
                        min = p.X;
                    }
                }
            }
            return min;
        }
        static float RoundToNearestY(GlyphPoint p, float org, bool useHalfPixel)
        {
            float floo_int = (int)org;//floor 
            float remaining = org - floo_int;
            if (useHalfPixel)
            {
                if (remaining > GRID_SIZE_66)
                {
                    return (floo_int + 1f);
                }
                else if (remaining > (GRID_SIZE_33))
                {
                    return (floo_int + 0.5f);
                }
                else
                {
                    return floo_int;
                }
            }
            else
            {
                if (remaining > GRID_SIZE_50)
                {
                    return (floo_int + 1f);
                }
                else
                {
                    //we we move this point down
                    //the upper part point may affect the other(lower side)
                    //1.horizontal edge

                    //                    EdgeLine h_edge = p.horizontalEdge;
                    //                    EdgeLine matching_anotherSide = h_edge.GetMatchingOutsideEdge();
                    //                    if (matching_anotherSide != null)
                    //                    {
                    //                        GlyphPoint a_glyph_p = matching_anotherSide.GlyphPoint_P;
                    //                        GlyphPoint a_glyph_q = matching_anotherSide.GlyphPoint_Q;
                    //                        if (a_glyph_p != null)
                    //                        {

                    //                            a_glyph_p.AdjustedY = -remaining;
                    //#if DEBUG
                    //                            if (!s_dbugAff2.ContainsKey(a_glyph_p))
                    //                            {
                    //                                s_dbugAff2.Add(a_glyph_p, true);
                    //                                s_dbugAffectedPoints.Add(a_glyph_p);
                    //                            }

                    //#endif
                    //                        }
                    //                        if (a_glyph_q != null)
                    //                        {

                    //                            a_glyph_q.AdjustedY = -remaining;
                    //#if DEBUG
                    //                            if (!s_dbugAff2.ContainsKey(a_glyph_q))
                    //                            {
                    //                                s_dbugAff2.Add(a_glyph_q, true);
                    //                                s_dbugAffectedPoints.Add(a_glyph_q);
                    //                            }

                    //#endif
                    //                        }
                    //                    }

                    return floo_int;
                }
            }
        }
        static float RoundToNearestX(float org)
        {
            float actual1 = org;
            float integer1 = (int)(actual1);//lower
            float floatModulo = actual1 - integer1;

            if (floatModulo >= (GRID_SIZE_50))
            {
                return (integer1 + 1);
            }
            else
            {
                return integer1;
            }
        }
        static void GenerateNewFitPoints(
            List<Vector2> genPoints,
            GlyphContour contour,
            float pixelScale,
            bool x_axis,
            bool y_axis,
            bool useHalfPixel)
        {
            List<GlyphPoint> flattenPoints = contour.flattenPoints;

            int j = flattenPoints.Count;
            //merge 0 = start
            //double prev_px = 0;
            //double prev_py = 0;
            double p_x = 0;
            double p_y = 0;
            double first_px = 0;
            double first_py = 0;

            //---------------
            //1st round for value adjustment
            //--------------- 
            //find adjust y 
            {
                GlyphPoint p = flattenPoints[0];
                p_x = p.x * pixelScale;
                p_y = p.y * pixelScale;

                if (y_axis && p.isPartOfHorizontalEdge && p.isUpperSide) //TODO: review here
                {
                    //vertical fitting, fit p_y to grid
                    //adjust if p is not part of curve
                    switch (p.kind)
                    {
                        case PointKind.LineStart:
                        case PointKind.LineStop:
                            p_y = RoundToNearestY(p, (float)p_y, useHalfPixel);
                            break;
                    }

                }
                if (x_axis && p.IsPartOfVerticalEdge && p.IsLeftSide)
                {
                    //horizontal fitting, fix p_x to grid
                    float new_x = RoundToNearestX((float)p_x);
                    p_x = new_x;
                    //adjust right-side vertical edge

                }

                genPoints.Add(new Vector2((float)p_x, (float)p_y));
                //-------------
                first_px = p_x;
                first_py = p_y;
            }

            for (int i = 1; i < j; ++i)
            {
                //all merge point is polygon point
                GlyphPoint p = flattenPoints[i];
                p_x = p.x * pixelScale;
                p_y = p.y * pixelScale;


                if (y_axis && p.isPartOfHorizontalEdge && p.isUpperSide)  //TODO: review here
                {
                    //vertical fitting, fit p_y to grid
                    p_y = RoundToNearestY(p, (float)p_y, useHalfPixel);
                }

                if (x_axis && p.IsPartOfVerticalEdge && p.IsLeftSide)
                {
                    //horizontal fitting, fix p_x to grid
                    float new_x = RoundToNearestX((float)p_x);
                    p_x = new_x;
                }
                genPoints.Add(new Vector2((float)p_x, (float)p_y));
            }
        }
        void GenerateFitOutput2(
           IGlyphTranslator tx,
           float pxscale,
           GlyphContour contour)
        {
            //walk along the edge in the contour to generate new edge output

            //List<GlyphEdge> edges = contour.edges; 
            //int j = edges.Count;
            //if (j > 0)
            //{
            //    GlyphEdge e;
            //    {
            //        //1st 
            //        e = edges[0];
            //        Vector2 p = new Vector2(e.newEdgeCut_P_X, e.newEdgeCut_P_Y) * pxscale;
            //        tx.MoveTo(p.X, p.Y);
            //    }
            //    for (int i = 1; i < j; ++i)
            //    {

            //        e = edges[i];
            //        Vector2 p = new Vector2(e.newEdgeCut_P_X, e.newEdgeCut_P_Y) * pxscale;
            //        tx.LineTo(p.X, p.Y);
            //    }

            //    //close 
            //    tx.CloseContour();
            //}

            GridFitter gridFitterX = new GridFitter(1, pxscale);
            GridFitter gridFitterY = new GridFitter(1, pxscale);
            List<GlyphPoint> points = contour.flattenPoints;
            int j = points.Count;
            if (j > 0)
            {
                //1.
                GlyphPoint p = points[0];
                float x, y;
                gridFitterX.GetFitPosX(p, out x);
                gridFitterY.GetFitPosY(p, out y);
                tx.MoveTo(x, y);
                //2. others
                for (int i = 1; i < j; ++i)
                {
                    //try to fit to grid 
                    p = points[i];
                    gridFitterX.GetFitPosX(p, out x);
                    gridFitterY.GetFitPosY(p, out y);
                    tx.LineTo(x, y);
                }
                //close 
                tx.CloseContour();
            }
        }
        void GenerateFitOutput3(
            IGlyphTranslator tx,
            float pxscale,
            GlyphContour contour)
        {
            //walk along the edge in the contour to generate new edge output

            //------------------------------------------------------------------------------------
            GridFitter gridFitterX = new GridFitter(1, pxscale); //use struct, no alloc on heap
            GridFitter gridFitterY = new GridFitter(1, pxscale);
            List<GlyphPoint> points = contour.flattenPoints;
            int j = points.Count;
            if (j > 0)
            {
                //1.
                GlyphPoint p = points[0];
                float x, y;
                if (p.fit_analyzed)
                {
                    if (dbugTestNewGridFitting)
                    {
                        tx.MoveTo(p.fit_NewX, p.fit_NewY);
                    }
                    else
                    {
                        tx.MoveTo(p.x * pxscale, p.y * pxscale);
                    }
                }
                else
                {
                    //gridFitterX.GetFitPosX(p, out x);
                    //gridFitterY.GetFitPosY(p, out y);
                    tx.MoveTo(p.x * pxscale, p.y * pxscale);
                }
                //2. others
                for (int i = 1; i < j; ++i)
                {
                    //try to fit to grid 
                    p = points[i];
                    if (p.fit_analyzed)
                    {

                        if (dbugTestNewGridFitting)
                        {
                            tx.LineTo(p.fit_NewX, p.fit_NewY);
                        }
                        else
                        {
                            tx.LineTo(p.x * pxscale, p.y * pxscale);
                        }
                    }
                    else
                    {
                        tx.LineTo(p.x * pxscale, p.y * pxscale);
                        //gridFitterX.GetFitPosX(p, out x);
                        //gridFitterY.GetFitPosY(p, out y);
                        //tx.LineTo(x, y);
                    }


                }
                //close 
                tx.CloseContour();
            }
        }
        struct GridFitter
        {
            readonly int _gridSize;
            readonly float _scale;
            public GridFitter(int gridSize, float scale)
            {
                _gridSize = gridSize;
                _scale = scale;
            }
            public void GetFitPosX(GlyphPoint p, out float result)
            {
                float value = p.x * _scale;
                //
                int floor = ((int)(value / _gridSize) * _gridSize);
                //2. midpoint
                float remaining = value - floor;

                float halfGrid = _gridSize / 2f;
                if (remaining >= (2 / 3f) * _gridSize)
                {
                    result = floor + _gridSize;
                }
                else if (remaining >= (1 / 3f) * _gridSize)
                {
                    result = (floor + _gridSize * (1 / 2f));
                }
                else
                {
                    result = floor;
                }
            }
            public void GetFitPosY(GlyphPoint p, out float result)
            {
                //we may have a special treatment for vertical axis

                float value = p.y * _scale;
                //result = value;
                //return;

                float guide_y = p.newY * _scale;
                int floor = ((int)(value / _gridSize) * _gridSize);
                float remaining = value - floor;
                float halfGrid = _gridSize / 2f;


                //if (p.isPartOfHorizontalEdge)
                //{
                //    //fit horizontal edge

                //    if (value < 3)
                //    {

                //    }
                //    int ceilling = floor + _gridSize;

                //    if (value > (floor + halfGrid))
                //    {
                //        //move up
                //    }
                //    else
                //    {
                //        //move down
                //    }



                //    result = value + 0.30f;
                //    Console.WriteLine(p.dbugId + " pre: " + value + ",post:" + result);
                //    return;
                //    //if (p.isUpperSide)
                //    //{
                //    //    result = value + 0.125f;
                //    //}
                //    //else
                //    //{
                //    //    result = value - 0.125f;
                //    //}
                //    //return;
                //}



                if (remaining >= (2 / 3f) * _gridSize)
                {
                    result = floor + _gridSize;
                }
                else if (remaining >= (1 / 3f) * _gridSize)
                {
                    result = (floor + _gridSize * (1 / 2f));
                }
                else
                {
                    result = floor;
                }
            }
        }

        //void WriteFitEdge(int srcIndex, IGlyphTranslator tx, GlyphEdge edge)
        //{
        //    EdgeLine internalEdgeLine = edge.dbugGetInternalEdgeLine();
        //    float _pxscale = this.pxScale;
        //    Vector2 p = new Vector2(edge.newEdgeCut_P_X, edge.newEdgeCut_P_Y) * _pxscale;
        //    Vector2 regen0 = edge._newRegen0;
        //    Vector2 regen1 = edge._newRegen1;
        //    bool foundSomePerpendicularEdge = false;

        //    bool moveTo = false;


        //    if (internalEdgeLine._controlE0 != null)
        //    {
        //        //Vector2 v2 = internalEdgeLine._controlE0.GetMidPoint();
        //        //Vector2 cutpoint = internalEdgeLine._controlE0_cutAt;
        //        //painter.Line(
        //        //    v2.X * _pxscale, v2.Y * _pxscale,
        //        //    cutpoint.X * _pxscale, cutpoint.Y * _pxscale,
        //        //    PixelFarm.Drawing.Color.Green); 
        //        foundSomePerpendicularEdge = true;
        //        if (srcIndex == 0 && !moveTo)
        //        {
        //            tx.MoveTo(regen0.X * _pxscale, regen0.Y * _pxscale);
        //            moveTo = true;
        //        }
        //        else
        //        {
        //            tx.LineTo(regen0.X * _pxscale, regen0.Y * _pxscale);
        //        }
        //    }


        //    if (internalEdgeLine._controlE0 != null && internalEdgeLine._controlE1 != null)
        //    {
        //        //Vector2 m0 = internalEdgeLine._controlE0.GetMidPoint();
        //        //Vector2 m1 = internalEdgeLine._controlE1.GetMidPoint();

        //        ////find angle from m0-> m1

        //        //Vector2 v2 = (m0 + m1) / 2;
        //        ////find perpendicular line  from  midpoint_m0m1 to edge
        //        //Vector2 cutpoint;
        //        //if (MyMath.FindPerpendicularCutPoint(internalEdgeLine, v2, out cutpoint))
        //        //{
        //        //    painter.Line(
        //        //       v2.X * _pxscale, v2.Y * _pxscale,
        //        //       cutpoint.X * _pxscale, cutpoint.Y * _pxscale,
        //        //       PixelFarm.Drawing.Color.Red);
        //        //    foundSomePerpendicularEdge = true;
        //        //}

        //        //Vector2 e0_fitpos = internalEdgeLine._controlE0.GetFitPos() * _pxscale;
        //        //Vector2 e1_fitpos = internalEdgeLine._controlE1.GetFitPos() * _pxscale;

        //        //painter.Line(
        //        //      e0_fitpos.X, e0_fitpos.Y,
        //        //      regen0.X, regen0.Y,
        //        //      PixelFarm.Drawing.Color.Yellow);
        //        //painter.Line(
        //        //    e1_fitpos.X, e1_fitpos.Y,
        //        //    regen1.X, regen1.Y,
        //        //    PixelFarm.Drawing.Color.Yellow);
        //        //if (srcIndex == 0)
        //        //{ 
        //        //}
        //        //else
        //        //{ 
        //        //}
        //    }



        //    if (internalEdgeLine._controlE1 != null)
        //    {
        //        //Vector2 v2 = internalEdgeLine._controlE1.GetMidPoint();
        //        //Vector2 cutpoint = internalEdgeLine._controlE1_cutAt;
        //        //painter.Line(
        //        //    v2.X * _pxscale, v2.Y * _pxscale,
        //        //    cutpoint.X * _pxscale, cutpoint.Y * _pxscale,
        //        //    PixelFarm.Drawing.Color.Green); 

        //        foundSomePerpendicularEdge = true;

        //        if (srcIndex == 0 && !moveTo)
        //        {
        //            tx.MoveTo(regen1.X * _pxscale, regen1.Y * _pxscale);
        //            moveTo = true;
        //        }
        //        else
        //        {
        //            tx.LineTo(regen1.X * _pxscale, regen1.Y * _pxscale);
        //        }
        //    }


        //    if (!foundSomePerpendicularEdge)
        //    {
        //        tx.LineTo(p.X, p.Y);
        //    }


        //    //if (!foundSomePerpendicularEdge)
        //    //{
        //    //    Vector2 midpoint = edge.GetMidPoint();
        //    //    //painter.FillRectLBWH(midpoint.X, midpoint.Y, 5, 5, PixelFarm.Drawing.Color.White);
        //    //}
        //}
        static void GenerateFitOutput(
          IGlyphTranslator tx,
          List<Vector2> genPoints,
          GlyphContour contour)
        {

            int j = genPoints.Count;
            //merge 0 = start
            //double prev_px = 0;
            //double prev_py = 0; 
            float first_px = 0;
            float first_py = 0;
            //---------------
            //1st round for value adjustment
            //---------------

            //find adjust y
            List<GlyphPoint> flattenPoints = contour.flattenPoints;
            //---------------
            if (j != flattenPoints.Count)
            {
                throw new NotSupportedException();
            }
            //---------------
            for (int i = 0; i < j; ++i)
            {
                GlyphPoint glyphPoint = flattenPoints[i];
                Vector2 p = genPoints[i];
                if (i == 0)
                {
                    //first point
                    tx.MoveTo(first_px = p.X, first_py = p.Y);
                }
                else
                {
                    tx.LineTo(p.X, p.Y);
                }
            }
            //close 
            tx.CloseContour();
        }
        static void GeneratePerpendicularLines(
             float x0, float y0, float x1, float y1, float len,
             out Vector2 delta)
        {
            Vector2 v0 = new Vector2(x0, y0);
            Vector2 v1 = new Vector2(x1, y1);

            delta = (v1 - v0) / 2;
            delta = delta.NewLength(len);
            delta = delta.Rotate(90);
        }
        static void GeneratePerpendicularLines(
          Vector2 p0, Vector2 p1, float len,
          out Vector2 delta)
        {
            Vector2 v0 = new Vector2(p0.X, p0.Y);
            Vector2 v1 = new Vector2(p1.X, p1.Y);

            delta = (v1 - v0) / 2;
            delta = delta.NewLength(len);
            delta = delta.Rotate(90);
        }
        //void RegenerateBorders(List<StrokeSegment> segments, int startAt, int endAt)
        //{
        //    //regenerate stroke border
        //    List<Vector2> newBorders = new List<Vector2>();
        //    for (int i = startAt; i < endAt; ++i)
        //    {
        //        StrokeSegment segment = segments[i];
        //        StrokeJoint jointA = segment.a;
        //        StrokeJoint jointB = segment.b;


        //        if (jointA != null && jointB != null)
        //        {

        //            //create perpendicular 
        //            Vector2 delta;
        //            GeneratePerpendicularLines(jointA._position, jointB._position, 5 / pxScale, out delta);
        //            //upper and lower
        //            newBorders.Insert(0, (jointA._position + new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
        //            newBorders.Add((jointA._position - new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
        //            //
        //            newBorders.Insert(0, (jointB._position + new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
        //            newBorders.Add((jointB._position - new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
        //        }
        //        if (jointA != null && jointA.hasTip)
        //        {
        //            Vector2 jointAPoint = jointA._position;
        //            Vector2 delta;
        //            GeneratePerpendicularLines(jointA._position, jointA._tip_endAt, 5 / pxScale, out delta);
        //            newBorders.Insert(0, (jointA._position + new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
        //            newBorders.Add((jointA._position - new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
        //            //
        //            newBorders.Insert(0, (jointA._tip_endAt + new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
        //            newBorders.Add((jointA._tip_endAt - new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
        //        }
        //    }
        //    ////---------------------------------------------------
        //    //int newBorderSegmentCount = newBorders.Count;
        //    //VertexStore vxs = new VertexStore();
        //    //for (int n = 0; n < newBorderSegmentCount; ++n)
        //    //{
        //    //    Vector2 borderSegment = newBorders[n];
        //    //    if (n == 0)
        //    //    {
        //    //        vxs.AddMoveTo(borderSegment.X, borderSegment.Y);
        //    //    }
        //    //    else
        //    //    {
        //    //        vxs.AddLineTo(borderSegment.X, borderSegment.Y);
        //    //    }
        //    //}
        //    //vxs.AddCloseFigure();
        //    ////---------------------------------------------------
        //    //painter.Fill(vxs, PixelFarm.Drawing.Color.Red);
        //    ////---------------------------------------------------
        //}

#if DEBUG
        internal List<GlyphTriangle> dbugGetGlyphTriangles()
        {
            return _dbugTempIntermediateOutline.GetTriangles();
        }
        internal List<CentroidLineHub> dbugGetCentroidLineHubs()
        {
            return _dbugTempIntermediateOutline.GetCentroidLineHubs();
        }
        public static List<GlyphPoint> s_dbugAffectedPoints = new List<GlyphPoint>();
        public static Dictionary<GlyphPoint, bool> s_dbugAff2 = new Dictionary<GlyphPoint, bool>();
        GlyphIntermediateOutline _dbugTempIntermediateOutline;
        public bool dbugDrawRegeneratedOutlines { get; set; }
#endif

    }
}