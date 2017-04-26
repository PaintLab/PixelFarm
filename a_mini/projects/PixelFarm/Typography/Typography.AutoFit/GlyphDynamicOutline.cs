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
        /// <summary>
        /// offset in pixel unit from master outline, accept + and -
        /// </summary>
        float _offsetFromMasterOutline = 0; //pixel unit
        float pxScale;

        internal GlyphDynamicOutline(GlyphIntermediateOutline intermediateOutline)
        {

#if DEBUG
            _dbugTempIntermediateOutline = intermediateOutline;
#endif

            //we convert data from GlyphIntermediateOutline to newform (lightweight form).
            //and save it here.
            //1. joints and its bones
            //2. bones and its controlled edge 
            _contours = intermediateOutline.GetContours(); //original contours
            _longVerticalBones = intermediateOutline.LongVerticalBones; //analyzed long bones
            LeftControlPosX = intermediateOutline.LeftControlPos; //left control position  

        }

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

        public void GenerateOutput(IGlyphTranslator tx, float pxScale)
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
            if (_offsetFromMasterOutline == 0)
            {
                //gen with anohter methods
                GenerateOutput(tx, pxScale);
                return;
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
                GenerateFitOutput2(tx, pxScale, contours[i]);
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

        static void GenerateFitOutput2(
             IGlyphTranslator tx,
             float pxscale,
             GlyphContour contour)
        {


            //merge 0 = start
            //double prev_px = 0;
            //double prev_py = 0; 
            float first_px = 0;
            float first_py = 0;
            //---------------
            //1st round for value adjustment
            //---------------

            ////find adjust y
            //List<GlyphPoint> flattenPoints = contour.flattenPoints;
            ////---------------
            //int j = flattenPoints.Count;
            ////---------------
            ////we can draw along a contour or along edge
            //for (int i = 0; i < j; ++i)
            //{
            //    GlyphPoint glyphPoint = flattenPoints[i];
            //    Vector2 p = new Vector2(glyphPoint.x, glyphPoint.y) * pxscale;
            //    if (i == 0)
            //    {
            //        //first point
            //        tx.MoveTo(first_px = p.X, first_py = p.Y);
            //    }
            //    else
            //    {
            //        tx.LineTo(p.X, p.Y);
            //    }
            //}



            List<GlyphEdge> edges = contour.edges;
            int j = edges.Count;
            for (int i = 0; i < j; ++i)
            {
                GlyphEdge e = edges[i];
                Vector2 p = new Vector2(e.newEdgeCut_P_X, e.newEdgeCut_P_Y) * pxscale;
                if (i == 0)
                {
                    //first edge 
                    //move to
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