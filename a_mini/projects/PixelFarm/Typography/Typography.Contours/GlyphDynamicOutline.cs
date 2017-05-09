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

        BoneGroupingHelper _groupingHelper;

        internal GlyphDynamicOutline(GlyphIntermediateOutline intermediateOutline)
        {

            //setup default values
            _needRefreshBoneGroup = true; //first time
            _needAdjustGridFitValues = true;//first time
            this.GridBoxWidth = 1; //pixels
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

            //left control from vertical long bone
            //-------- 

            SetupLeftPositionX();
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
        ///classify bone group by gridbox(w,h) and apply to current master outline
        /// </summary>
        /// <param name="gridBoxW"></param>
        /// <param name="gridBoxH"></param>
        public void PrepareFitValues(int gridBoxW, int gridBoxH)
        {


            //bone grouping depends on grid size.

            this.GridBoxHeight = gridBoxH;
            this.GridBoxWidth = gridBoxW;
            //
            _groupingHelper.Reset();
            int centroidLineCount = _allCentroidLines.Count;

            for (int i = 0; i < centroidLineCount; ++i)
            {
                //apply new grid to this centroid line
                CentroidLine line = _allCentroidLines[i];
                line.ApplyGridBox(gridBoxW, gridBoxH);
                _groupingHelper.CollectBoneGroups(line);
            }
            //analyze bone group (stem) as a whole
            _groupingHelper.AnalyzeHorizontalBoneGroups();
            _groupingHelper.AnalyzeVerticalBoneGroups();
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
                int j = cnts.Count;
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
                    PrepareFitValues(GridBoxWidth, GridBoxHeight);
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

            public float negative_diff;
            public float positive_diff;
            public int negativeCount;
            public int positiveCount;
            public void Collect(float diff)
            {
                if (diff < 0)
                {
                    negative_diff += diff;
                    negativeCount++;
                }
                else
                {
                    positive_diff += diff;
                    positiveCount++;
                }
            }
            public float CalculateProperDiff()
            {
                if (positiveCount != 0 && negativeCount != 0)
                {
                    //check if we should move to positive or negative  
                    //tech: choose minimum move to reach the target
                    if (positive_diff > -negative_diff)
                    {
                        //move to negative***
                        return -(positive_diff - negative_diff) / (positiveCount + negativeCount);
                    }
                    else
                    {
                        //avg to positive
                        return (positive_diff - negative_diff) / (positiveCount + negativeCount);
                    }
                }
                else if (positiveCount != 0)
                {
                    //only positive side
                    return positive_diff / positiveCount;
                }
                else if (negativeCount != 0)
                {
                    //only negative side, preserve negative sign
                    return negative_diff / negativeCount;
                }
                else
                {
                    return 0;
                }
            }
        }
        float _avg_xdiff = 0;
        /// <summary>
        /// adjust vertical fitting value
        /// </summary>
        void AdjustFitValues()
        {
            //adjust the value when we move to new pixel scale (pxscale)
            //if we known adjust values for that pxscale before( and cache it)
            //we can use that without recalculation

            //assign fit y pos in order
            List<BoneGroup> selectedHBoneGroups = _groupingHelper.SelectedHorizontalBoneGroups;
            for (int i = selectedHBoneGroups.Count - 1; i >= 0; --i)
            {
                //arrange selected horizontal
                BoneGroup boneGroup = selectedHBoneGroups[i];
                if (boneGroup.toBeRemoved)
                {
                    continue;
                }
                EdgeLine[] h_edges = boneGroup.edges;
                if (h_edges == null)
                {
                    continue;
                }
                int edgeCount = h_edges.Length;
                //we need to calculate the avg of the glyph point
                //and add a total summary to this 
                FitDiffCollector y_fitDiffCollector = new FitDiffCollector();
                for (int e = 0; e < edgeCount; ++e)
                {
                    EdgeLine ee = h_edges[e];
                    GlyphPoint p_pnt = ee.P;
                    GlyphPoint q_pnt = ee.Q;
                    //p

                    y_fitDiffCollector.Collect(MyMath.CalculateDiffToFit(p_pnt.newY * _pxScale));
                    //q

                    y_fitDiffCollector.Collect(MyMath.CalculateDiffToFit(q_pnt.newY * _pxScale));
                }

                float avg_ydiff = y_fitDiffCollector.CalculateProperDiff();

                //compare abs max /min   
                //distribute all adjust value to specific glyph points
                for (int e = 0; e < edgeCount; ++e)
                {
                    EdgeLine ee = h_edges[e];
                    GlyphPoint p_pnt = ee.P;
                    GlyphPoint q_pnt = ee.Q;

                    //this version apply only Y ?
                    //apply from newX and newY
                    p_pnt.fit_NewX = p_pnt.newX * _pxScale;
                    p_pnt.fit_NewY = (p_pnt.newY * _pxScale) + avg_ydiff;
                    //
                    q_pnt.fit_NewX = q_pnt.newX * _pxScale;
                    q_pnt.fit_NewY = (q_pnt.newY * _pxScale) + avg_ydiff;

                    p_pnt.fit_analyzed = q_pnt.fit_analyzed = true;
                }
            }
            //---------------------------------------------------------
            //vertical group

            List<BoneGroup> verticalGroups = _groupingHelper.SelectedVerticalBoneGroups;
            FitDiffCollector x_fitDiffCollector = new FitDiffCollector();
            for (int i = verticalGroups.Count - 1; i >= 0; --i)
            {
                BoneGroup boneGroup = verticalGroups[i];
                if (boneGroup.toBeRemoved)
                {
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

                for (int e = 0; e < edgeCount; ++e)
                {
                    EdgeLine ee = v_edges[e];
                    GlyphPoint p_pnt = ee.P;
                    GlyphPoint q_pnt = ee.Q;
                    //p
                    x_fitDiffCollector.Collect(MyMath.CalculateDiffToFit(p_pnt.newX * _pxScale));
                    //q
                    x_fitDiffCollector.Collect(MyMath.CalculateDiffToFit(q_pnt.newX * _pxScale));
                }
            }
            _avg_xdiff = x_fitDiffCollector.CalculateProperDiff();
            ////experiment
            ////for subpixel rendering
            //_avg_xdiff = -0.33f; //use use with subpixel, we shift it to the left 1/3 of 1 px 
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

        void SetupLeftPositionX()
        {

            PrepareFitValues(GridBoxWidth, GridBoxHeight);
            _needRefreshBoneGroup = false;
            //
            List<BoneGroup> arrangedVerticalBoneGroups = _groupingHelper.SelectedVerticalBoneGroups;
            //left most
            //find adjust values
            if (arrangedVerticalBoneGroups != null && arrangedVerticalBoneGroups.Count > 0)
            {
                this.LeftControlPositionX = arrangedVerticalBoneGroups[0].x_pos;
            }
            else
            {
                this.LeftControlPositionX = 0;
            }
            //LeftControlPositionX = 0;
        }

        void GenerateContourOutput(
            IGlyphTranslator tx,
            GlyphContour contour)
        {
            //walk along the edge in the contour to generate new edge output

#if DEBUG 
            Console.WriteLine("===begin===" + _avg_xdiff);
            float offset = _avg_xdiff;
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

            float pre_x = 0, post_x = 0;

            if (useGridFit && p.fit_analyzed)
            {
                pre_x = p.fit_NewX;
                post_x = pre_x + offset;
                //
                tx.MoveTo(post_x, p.fit_NewY);
#if DEBUG
                dbugWriteOutput("M", pre_x, post_x, p.fit_NewY);
#endif
            }
            else
            {
                pre_x = p.newX * pxscale;
                post_x = pre_x + offset;

                tx.MoveTo(post_x, p.newY * pxscale);
#if DEBUG
                dbugWriteOutput("M", pre_x, post_x, p.newY * pxscale);
#endif
            }

            //2. others
            for (int i = 1; i < j; ++i)
            {
                //try to fit to grid 
                p = points[i];
                if (useGridFit && p.fit_analyzed)
                {
                    pre_x = p.fit_NewX;
                    post_x = pre_x + offset;
                    tx.LineTo(post_x, p.fit_NewY);
#if DEBUG 
                    dbugWriteOutput("L", pre_x, post_x, p.fit_NewY);
#endif
                }
                else
                {
                    pre_x = p.newX * pxscale;
                    post_x = pre_x + offset;
                    tx.LineTo(post_x, p.newY * pxscale);
#if DEBUG                  
                    //for debug
                    dbugWriteOutput("L", pre_x, post_x, p.newY * pxscale);
#endif
                }
            }
            //close 
            tx.CloseContour();
#if DEBUG
            Console.WriteLine("===end===");
#endif
        }


#if DEBUG
        void dbugWriteOutput(string cmd, float pre_x, float post_x, float y)
        {
            Console.WriteLine(cmd + "pre_x:" + pre_x + ",post_x:" + post_x + ",y" + y);
        }
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