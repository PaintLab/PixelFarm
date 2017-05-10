//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using Typography.OpenFont;

namespace Typography.Contours
{

    public class GlyphDynamicOutline
    {

        internal List<GlyphContour> _contours;
        List<CentroidLine> _allCentroidLines;

        /// <summary>
        /// offset in pixel unit from master outline, accept + and -
        /// </summary>
        float _offsetFromMasterOutline = 0; //pixel unit
        float _pxScale;
        bool _needRefreshBoneGroup;
        bool _needAdjustGridFitValues;
        float _avg_xdiff = 0;
        BoneGroupingHelper _groupingHelper;
        internal GlyphDynamicOutline(GlyphIntermediateOutline intermediateOutline)
        {

            //setup default values
            _needRefreshBoneGroup = true; //first time
            _needAdjustGridFitValues = true;//first time
            this.GridBoxWidth = 32; //pixels
            this.GridBoxHeight = 50; //pixels 
            _groupingHelper = BoneGroupingHelper.CreateBoneGroupingHelper();
#if DEBUG
            this.GridBoxHeight = dbugGridHeight; //pixels
            _dbugTempIntermediateOutline = intermediateOutline;
#endif

            //we convert data from GlyphIntermediateOutline to newform (lightweight form).
            //and save it here.
            //1. joints and its bones
            //2. bones and its controlled edge 
            _contours = intermediateOutline.GetContours(); //original contours
            //3.
            CollectAllCentroidLines(intermediateOutline.GetCentroidLineHubs());

        }
        private GlyphDynamicOutline()
        {
            //for empty dynamic outline

        }
        internal static GlyphDynamicOutline CreateBlankDynamicOutline()
        {
            return new GlyphDynamicOutline();
        }

        public List<GlyphContour> GetContours()
        {
            return _contours;
        }
        /// <summary>
        ///classify bone group by gridbox(w,h) 
        /// </summary>
        /// <param name="gridBoxW"></param>
        /// <param name="gridBoxH"></param>
        public void AnalyzeBoneGroups(int gridBoxW, int gridBoxH)
        {
            //bone grouping depends on grid size.
            this.GridBoxHeight = gridBoxH;
            this.GridBoxWidth = gridBoxW;
            //
            _groupingHelper.Reset(gridBoxW, gridBoxH);

            for (int i = _allCentroidLines.Count - 1; i >= 0; --i)
            {
                //apply new grid to this centroid line 
                _groupingHelper.CollectBoneGroups(_allCentroidLines[i]);
            }
            //analyze bone group (stem) as a whole
            _groupingHelper.AnalyzeHorizontalBoneGroups();
            _groupingHelper.AnalyzeVerticalBoneGroups();

            //at this state we have a list of BoneGroup. 
            //but we don't know the  'fit-adjust' value for each GlyphPoint
            //we will known the 'fit-adjust' value after we know the pxscale
        }

        /// <summary>
        /// new stroke width offset from master outline
        /// </summary>
        /// <param name="offsetFromMasterOutline"></param>
        public void SetDynamicEdgeOffsetFromMasterOutline(float offsetFromMasterOutline)
        {

            if (_contours == null) return; //blank
            //preserve original outline
            //regenerate outline from original outline
            //----------------------------------------------------------        
            if ((this._offsetFromMasterOutline = offsetFromMasterOutline) != 0)
            {
                //if 0, new other action
                List<GlyphContour> cnts = _contours;
                for (int i = cnts.Count - 1; i >= 0; --i)
                {
                    cnts[i].ApplyNewEdgeOffsetFromMasterOutline(offsetFromMasterOutline);
                }
            }
            //***
            //changing offset from master outline affects the grid fit-> need to recalculate 
            _needRefreshBoneGroup = true;
        }

        public float LeftControlPositionX { get; set; }

        /// <summary>
        /// use grid fit or not
        /// </summary>
        public bool EnableGridFit { get; set; }
        /// <summary>
        /// grid box width in pixels
        /// </summary>
        public int GridBoxWidth { get; private set; }
        /// <summary>
        /// grid box height in pixels
        /// </summary>
        public int GridBoxHeight { get; private set; }


        /// <summary>
        /// generate output with specific pixel scale
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="pxScale"></param>
        public void GenerateOutput(IGlyphTranslator tx, float pxScale)
        {
            if (_contours == null) return; //blank
#if DEBUG
            this.EnableGridFit = dbugTestNewGridFitting;
#endif

            if (_pxScale != pxScale)
            {
                //new scale need to adjust fit value again
                _needAdjustGridFitValues = true;
            }
            //
            this._pxScale = pxScale;
            //
            if (EnableGridFit)
            {
                if (_needRefreshBoneGroup)
                {
                    //change scale not affact the grid fit ***
                    AnalyzeBoneGroups(GridBoxWidth, GridBoxHeight);
                    _needRefreshBoneGroup = false;
                }
                //
                if (_needAdjustGridFitValues)
                {
                    AdjustFitValues();
                    _needAdjustGridFitValues = false;
                }
            }

            List<GlyphContour> contours = this._contours;
            LeftControlPositionX = 0;
            //
            int j = contours.Count;
            tx.BeginRead(j);
            for (int i = 0; i < j; ++i)
            {
                //generate in order of contour
                GenerateContourOutput(tx, contours[i]);
            }
            tx.EndRead();
        }

        struct FitDiffCollector
        {

            float negative_diff;
            float positive_diff;

            float weighted_sum_negativeDiff;
            float weighted_sum_positiveDiff;

            public void Collect(float diff, float groupLen)
            {
                if (diff < 0)
                {
                    negative_diff += (diff * groupLen);
                    weighted_sum_negativeDiff += groupLen;
                }
                else
                {
                    positive_diff += (diff * groupLen);
                    weighted_sum_positiveDiff += groupLen;
                }
            }

            public float CalculateProperDiff()
            {
                if (weighted_sum_positiveDiff != 0 && weighted_sum_negativeDiff != 0)
                {
                    //check if we should move to positive or negative  
                    //tech: choose minimum move to reach the target
                    if (positive_diff > -negative_diff)
                    {
                        //move to negative***
                        return -(positive_diff - negative_diff) / (weighted_sum_positiveDiff + weighted_sum_negativeDiff);
                    }
                    else
                    {
                        //avg to positive
                        return (positive_diff - negative_diff) / (weighted_sum_positiveDiff + weighted_sum_negativeDiff);
                    }
                }
                else if (weighted_sum_positiveDiff != 0)
                {
                    //only positive side
                    return positive_diff / weighted_sum_positiveDiff;
                }
                else if (weighted_sum_negativeDiff != 0)
                {
                    //only negative side, preserve negative sign
                    return negative_diff / weighted_sum_negativeDiff;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// adjust vertical fitting value
        /// </summary>
        void AdjustFitValues()
        {
            //clear all prev adjust value
            for (int i = _contours.Count - 1; i >= 0; --i)
            {
                List<GlyphPoint> pnts = _contours[i].flattenPoints;
                for (int m = pnts.Count - 1; m >= 0; --m)
                {
                    pnts[m].ResetFitAdjustValues();
                }
            }

            //adjust the value when we move to new pixel scale (pxscale)
            //if we known adjust values for that pxscale before( and cache it)
            //we can use that without recalculation

            //--------------------
            //select Horizontal BoneGroups for Vertical fitting:
            //for veritical fitting, we apply fitting value to each group.
            //each group may not need the same value.
            //--------------------
            List<BoneGroup> selectedHBoneGroups = _groupingHelper.SelectedHorizontalBoneGroups;

            for (int i = selectedHBoneGroups.Count - 1; i >= 0; --i)
            {

                BoneGroup boneGroup = selectedHBoneGroups[i];
                if (boneGroup._lengKind == BoneGroupSumLengthKind.Short)
                {
                    continue;
                }
                EdgeLine[] h_edges = boneGroup.edges;
                if (h_edges == null)
                {
                    continue;
                }
                //
                int edgeCount = h_edges.Length;
                //we need to calculate the avg of the glyph point
                //and add a total summary to this 
                FitDiffCollector y_fitDiffCollector = new FitDiffCollector();
                float groupLen = boneGroup.approxLength;
                for (int e = 0; e < edgeCount; ++e)
                {
                    EdgeLine ee = h_edges[e];
                    //p                    
                    y_fitDiffCollector.Collect(MyMath.CalculateDiffToFit(ee.P.Y * _pxScale), groupLen);
                    //q
                    y_fitDiffCollector.Collect(MyMath.CalculateDiffToFit(ee.Q.Y * _pxScale), groupLen);
                }

                float avg_ydiff = y_fitDiffCollector.CalculateProperDiff();

                for (int e = 0; e < edgeCount; ++e)
                {
                    EdgeLine ee = h_edges[e];
                    GlyphPoint p_pnt = ee.P;
                    GlyphPoint q_pnt = ee.Q;
                    p_pnt.FitAdjustY = avg_ydiff;
                    // p_pnt.fit_NewX = p_pnt.X * _pxScale;
                    //p_pnt.fit_NewY = (p_pnt.Y * _pxScale) + avg_ydiff;

                    //
                    q_pnt.FitAdjustY = avg_ydiff;
                    //q_pnt.fit_NewX = q_pnt.X * _pxScale;
                    //q_pnt.fit_NewY = (q_pnt.Y * _pxScale) + avg_ydiff;

                    //
                    //p_pnt.fit_analyzed = q_pnt.fit_analyzed = true;
                }
            }
            //---------------------------------------------------------
            //vertical group for horizontal fit:
            //this different from the vertical fitting.
            //we calculate the value as a whole.
            //and apply it as a whole in later state 
            List<BoneGroup> verticalGroups = _groupingHelper.SelectedVerticalBoneGroups;
            FitDiffCollector x_fitDiffCollector = new FitDiffCollector();

            int j = verticalGroups.Count;
            for (int i = 0; i < j; ++i)
            {
                //1. the verticalGroup list is sorted, left to right
                //2. analyze in order left-> right

                BoneGroup boneGroup = verticalGroups[i];
                if (boneGroup._lengKind != BoneGroupSumLengthKind.Long)
                {
                    //in this case we focus on long-length bone group only
                    continue;
                }
                EdgeLine[] v_edges = boneGroup.edges;
                if (v_edges == null)
                {
                    continue;
                }

                int edgeCount = v_edges.Length;
                //we need to calculate the avg of the glyph point
                //and add a total summary to this 
                float groupLen = boneGroup.approxLength;
                for (int e = 0; e < edgeCount; ++e)
                {
                    EdgeLine ee = v_edges[e];
                    //TODO: review this
                    //if (ee.IsLeftSide)
                    //{
                    //focus on leftside edge
                    //p
                    x_fitDiffCollector.Collect(MyMath.CalculateDiffToFit(ee.P.X * _pxScale), groupLen);
                    //q
                    x_fitDiffCollector.Collect(MyMath.CalculateDiffToFit(ee.Q.X * _pxScale), groupLen);
                    //}
                }
                break; //only left most first long group
            }
            _avg_xdiff = x_fitDiffCollector.CalculateProperDiff();

        }
        void CollectAllCentroidLines(List<CentroidLineHub> lineHubs)
        {

            //collect all centroid lines from each line CentroidLineHub
            _allCentroidLines = new List<CentroidLine>();
            int j = lineHubs.Count;
            for (int i = 0; i < j; ++i)
            {
                _allCentroidLines.AddRange(lineHubs[i].GetAllCentroidLines().Values);
            }
        }
        void GenerateContourOutput(
            IGlyphTranslator tx,
            GlyphContour contour)
        {
            //walk along the edge in the contour to generate new edge output
            float fit_x_offset = _avg_xdiff;
            ////experiment
            ////for subpixel rendering
            //offset -= -0.33f; //use use with subpixel, we shift it to the left 1/3 of 1 px 

#if DEBUG
            dbugWriteLine("===begin===" + _avg_xdiff);
            if (!dbugUseHorizontalFitValue)
            {
                fit_x_offset = 0;
            }
#endif
            List<GlyphPoint> points = contour.flattenPoints;
            int j = points.Count;
            if (j == 0) return;
            //
            // 
            //1.
            GlyphPoint p = points[0];
            float pxscale = this._pxScale;
            bool useGridFit = EnableGridFit;
            //TODO: review here

            float pre_x = p.GetFitX(pxscale);
            float post_x = pre_x + fit_x_offset;
            float post_y = p.GetFitY(pxscale);

            if (useGridFit && p.NeedFitAdjust)
            {
                tx.MoveTo(post_x, post_y);
            }
            else
            {
                tx.MoveTo(post_x, post_y); 
            }
#if DEBUG
            dbugWriteOutput("M", pre_x, post_x, post_y);
#endif
            //2. others
            for (int i = 1; i < j; ++i)
            {
                //try to fit to grid 
                p = points[i];

                pre_x = p.GetFitX(pxscale);
                post_x = pre_x + fit_x_offset;
                post_y = p.GetFitY(pxscale);

                if (useGridFit && p.NeedFitAdjust)
                {
                    tx.LineTo(post_x, post_y);
                }
                else
                {
                    tx.LineTo(post_x, post_y);
                }
#if DEBUG
                //for debug
                dbugWriteOutput("L", pre_x, post_x, post_y);
#endif
            }
            //close 
            tx.CloseContour();
#if DEBUG
            dbugWriteLine("===end===");
#endif
        }


#if DEBUG
        void dbugWriteLine(string text)
        {
            //Console.WriteLine(text);
        }
        void dbugWriteOutput(string cmd, float pre_x, float post_x, float y)
        {
            //Console.WriteLine(cmd + "pre_x:" + pre_x + ",post_x:" + post_x + ",y" + y);
        }
        public static bool dbugActualPosToConsole { get; set; }
        public static bool dbugUseHorizontalFitValue { get; set; }
        public static bool dbugTestNewGridFitting { get; set; }
        public static int dbugGridHeight = 50;
        internal List<GlyphTriangle> dbugGetGlyphTriangles()
        {
            return _dbugTempIntermediateOutline.dbugGetTriangles();
        }
        internal List<CentroidLineHub> dbugGetCentroidLineHubs()
        {
            return _dbugTempIntermediateOutline.GetCentroidLineHubs();
        }
        GlyphIntermediateOutline _dbugTempIntermediateOutline;
        public bool dbugDrawRegeneratedOutlines { get; set; }
#endif

    }
}