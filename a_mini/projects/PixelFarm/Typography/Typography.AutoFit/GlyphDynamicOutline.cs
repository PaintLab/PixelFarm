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
        List<CentroidLine> _allCentroidLines;

        /// <summary>
        /// offset in pixel unit from master outline, accept + and -
        /// </summary>
        float _offsetFromMasterOutline = 0; //pixel unit
        float _pxScale;
        bool _needRefreshBoneGroup;
        bool _needAdjustGridFitValues;

        BoneGroupStatisticCollector _statCollector = new BoneGroupStatisticCollector();

        internal GlyphDynamicOutline(GlyphIntermediateOutline intermediateOutline)
        {
            //setup default values
            _needRefreshBoneGroup = true; //first time
            _needAdjustGridFitValues = true;//first time
            this.GridBoxWidth = 1; //pixels
            this.GridBoxHeight = 50; //pixels 
            _statCollector._selectedHorizontalBoneGroups = new List<BoneGroup>();
            //-------------------------------- 

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

            //3.
            CollectAllCentroidLines(intermediateOutline.GetCentroidLineHubs());

            //--------
            LeftControlPosX = intermediateOutline.LeftControlPos; //left control position  
        }


        /// <summary>
        ///classify bone group by gridbox(w,h) and apply to current master outline
        /// </summary>
        /// <param name="gridBoxW"></param>
        /// <param name="gridBoxH"></param>
        public void PrepareFitValues(int gridBoxW, int gridBoxH)
        {

            this.GridBoxHeight = gridBoxH;
            this.GridBoxWidth = gridBoxW;
            //
            _statCollector.Reset();
            int centroidLineCount = _allCentroidLines.Count;

            for (int i = 0; i < centroidLineCount; ++i)
            {
                //apply new grid to this centroid line
                CentroidLine line = _allCentroidLines[i];
                line.ApplyGridBox(gridBoxW, gridBoxH);
                _statCollector.CollectBoneGroups(line);
            }

            //analyze bone group (stem) as a whole
            _statCollector.AnalyzeBoneGroups();
            List<EdgeLine> tmpEdges = new List<EdgeLine>();
            for (int i = 0; i < centroidLineCount; ++i)
            {
                _allCentroidLines[i].CollectOutsideEdges(tmpEdges);
            }
        }

        /// <summary>
        /// new stroke width offset from master outline
        /// </summary>
        /// <param name="offsetFromMasterOutline"></param>
        public void SetDynamicEdgeOffsetFromMasterOutline(float offsetFromMasterOutline)
        {
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

        public float LeftControlPosX { get; set; }

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
            LeftControlPosX = 0;
            //
            int j = contours.Count;
            tx.BeginRead(j);
            for (int i = 0; i < j; ++i)
            {
                //generate in order of contour
                GenerateContourOutput(tx, contours[i]);
            }
            tx.EndRead();
            //-------------
        }
        void AdjustFitValues()
        {

            //assign fit y pos in order
            List<BoneGroup> selectedHBoneGroups = _statCollector._selectedHorizontalBoneGroups;
            for (int i = selectedHBoneGroups.Count - 1; i >= 0; --i)
            {
                //arrange selected horizontal
                BoneGroup boneGroup = selectedHBoneGroups[i];
                if (boneGroup.toBeRemoved)
                {
                    continue;
                }

                EdgeLine[] h_edges = boneGroup.edges;
                int edgeCount = h_edges.Length;

                //we need to calculate the avg of the glyph point
                //and add a total summary to this

                float negative_diff = 0;
                float positive_diff = 0;
                int negativeCount = 0;
                int positiveCount = 0;
                for (int e = 0; e < edgeCount; ++e)
                {
                    EdgeLine ee = h_edges[e];
                    GlyphPoint p_pnt = ee.P;
                    GlyphPoint q_pnt = ee.Q;
                    //this version we focus on vertical hint only  
                    float diff = MyMath.CalculateDiffToFit(p_pnt.newY * _pxScale);
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
                    //
                    //evaluate diff
                    //
                    diff = MyMath.CalculateDiffToFit(q_pnt.newY * _pxScale);
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

                float avg_ydiff = 0;
                if (positiveCount != 0 && negativeCount != 0)
                {
                    //check if we should move to positive or negative
                    float avg_pos = positive_diff / positiveCount;
                    //check only 'amount', not sign ,
                    //make nagative to positive
                    float avg_neg = -(negative_diff / negativeCount);

                    //choose minimum move to reach the target
                    if (avg_pos > avg_neg)
                    {
                        //move to negative***
                        avg_ydiff = -(avg_pos + avg_neg) / 2;

                    }
                    else
                    {
                        //avg to positive
                        avg_ydiff = (avg_pos + avg_neg) / 2;
                    }

                }
                else if (positiveCount != 0)
                {
                    //only positive side
                    avg_ydiff = positive_diff / positiveCount;
                }
                else if (negativeCount != 0)
                {
                    //only negative side, preserve negative sign
                    avg_ydiff = negative_diff / negativeCount;
                }

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
            if (useGridFit && p.fit_analyzed)
            {
                tx.MoveTo(p.fit_NewX, p.fit_NewY);
            }
            else
            {
                tx.MoveTo(p.newX * pxscale, p.newY * pxscale);
            }

            //2. others
            for (int i = 1; i < j; ++i)
            {
                //try to fit to grid 
                p = points[i];
                if (useGridFit && p.fit_analyzed)
                {
                    tx.LineTo(p.fit_NewX, p.fit_NewY);
                }
                else
                {
                    tx.LineTo(p.newX * pxscale, p.newY * pxscale);
                }
            }
            //close 
            tx.CloseContour();

        }


#if DEBUG
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