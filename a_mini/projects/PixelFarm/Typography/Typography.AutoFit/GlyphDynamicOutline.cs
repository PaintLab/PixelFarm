//MIT, 2017, WinterDev
using System;
using System.Numerics;
using System.Collections.Generic;
using Typography.OpenFont;

namespace Typography.Rendering
{

    public class GlyphDynamicOutline
    {

        class StrokeLine
        {
            //a collection of connected stroke segment
            public Vector2 _head;
            public List<StrokeSegment> _segments;
        }
        class StrokeSegment
        {
            //segment link from joint a to b
            public StrokeJoint a;
            public StrokeJoint b;

            public StrokeSegment(StrokeJoint a, StrokeJoint b)
            {
                this.a = a;
                this.b = b;

            }
        }
        class StrokeJoint
        {
            public Vector2 _position;
            public Vector2 _ribA_endAt;
            public Vector2 _ribB_endAt;
            public Vector2 _tip_endAt;

            public bool hasRibA;
            public bool hasRibB;
            public bool hasTip;
#if DEBUG
            static int dbugTotalId;
            public readonly int dbugId = dbugTotalId++;
#endif
            public StrokeJoint(Vector2 pos)
            {
                this._position = pos;
#if DEBUG

#endif
            }

            public static double Atan(StrokeJoint j0, StrokeJoint j1)
            {
                return Math.Atan2(
                       j1._position.Y - j0._position.Y,
                       j1._position.X - j0._position.X);
            }
        }
        class StrokeLineHub
        {
            public Vector2 _center;
            public GlyphBoneJoint _headConnectedJoint;
            public List<StrokeLine> _branches;
        }

        List<StrokeLineHub> _strokeLineHub;
        List<GlyphContour> _contours;
        List<GlyphBone> _longVerticalBones;


        internal GlyphDynamicOutline(GlyphIntermediateOutline intermediateOutline)
        {

#if DEBUG
            _dbugTempIntermediateOutline = intermediateOutline;
#endif

            //we convert data from GlyphIntermediateOutline to newform (lightweight form).
            //and save it here.

            _contours = intermediateOutline.GetContours();
            _longVerticalBones = intermediateOutline.LongVerticalBones;
            LeftControlPosX = intermediateOutline.LeftControlPosX;

            //
            Dictionary<GlyphTriangle, CentroidLineHub> centroidLineHubs = intermediateOutline.GetCentroidLineHubs();
            _strokeLineHub = new List<StrokeLineHub>(centroidLineHubs.Count);
            //
            foreach (CentroidLineHub lineHub in centroidLineHubs.Values)
            {
                Dictionary<GlyphTriangle, GlyphCentroidBranch> branches = lineHub.GetAllBranches();

                //a line hub contains many centriod branches                                 
                StrokeLineHub internalLineHub = new StrokeLineHub();
                var branchList = new List<StrokeLine>(branches.Count);
                foreach (GlyphCentroidBranch branch in branches.Values)
                {
                    //create a stroke line
                    StrokeLine strokeLine = new StrokeLine();
                    //head of this branch
                    Vector2 brHead = branch.GetHeadPosition();
                    strokeLine._head = brHead;

                    //a branch contains small centroid line segments.
                    CreateStrokeSegments(branch, strokeLine);
                    //draw  a line link to centroid of target triangle
                    //WalkFromBranchHeadToHubCenter(brHead, hubCenter);

                    branchList.Add(strokeLine);
                }
                internalLineHub._branches = branchList;
                internalLineHub._center = lineHub.GetCenterPos();
                internalLineHub._headConnectedJoint = lineHub.GetHeadConnectedJoint();
                _strokeLineHub.Add(internalLineHub);
            }
        }
        /// <summary>
        /// set new stroke width for regenerated glyph
        /// </summary>
        /// <param name="relativeStrokeWidth"></param>
        public void SetNewRelativeStrokeWidth(float relativeStrokeWidth)
        {
            //preserve original outline
            //regenerate outline from original outline


        }

        public float LeftControlPosX { get; set; }
        public void Walk()
        {
            //each centroid hub 
            foreach (StrokeLineHub lineHub in _strokeLineHub)
            {
                Vector2 hubCenter = lineHub._center;
                WalkHubCenter(hubCenter);

                //a line hub contains many centriod branches
                //
                List<StrokeLine> branches = lineHub._branches;

                foreach (StrokeLine branch in branches)
                {
                    //head of this branch
                    Vector2 brHead = branch._head;

                    WalkStrokeLine(branch);
                    //draw  a line link to centroid of target triangle
                    WalkFromBranchHeadToHubCenter(brHead, hubCenter);
                }

                GlyphBoneJoint joint = lineHub._headConnectedJoint;
                if (joint != null)
                {
                    WalkFromHubCenterToJoint(joint.Position, hubCenter);
                }
            }
        }
        float pxScale;
        public void GenerateOutput(IGlyphTranslator tx, float pxScale)
        {
            this.pxScale = pxScale;

            List<GlyphContour> contours = this._contours;
            int j = contours.Count;
            for (int i = 0; i < j; ++i)
            {
                //new contour
                contours[i].ClearAllAdjustValues();
            }
#if DEBUG
            s_dbugAffectedPoints.Clear();
            s_dbugAff2.Clear();
#endif
            List<List<Vector2>> genPointList = new List<List<Vector2>>();
            for (int i = 0; i < j; ++i)
            {
                //new contour
                List<Vector2> genPoints = new List<Vector2>();
                GenerateNewFitPoints(genPoints,
                    contours[i], pxScale,
                    false, true, false);
                genPointList.Add(genPoints);
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
                //double avgWidth = longVertBone.CalculateAvgBoneWidth();
                //System.Numerics.Vector2 midBone = longVertBone.JointA.Position;

                ////left side
                //double newLeftAndScale = (midBone.X - (avgWidth / 2)) * pxScale;
                ////then move to fit int
                //minorOffset = MyMath.FindDiffToFitInteger((float)newLeftAndScale);
                //for (int m = 0; m < j; ++m)
                //{
                //    OffsetPoints(genPointList[m], minorOffset);
                //}
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

                    EdgeLine h_edge = p.horizontalEdge;
                    EdgeLine matching_anotherSide = h_edge.GetMatchingOutsideEdge();
                    if (matching_anotherSide != null)
                    {
                        GlyphPoint a_glyph_p = matching_anotherSide.GlyphPoint_P;
                        GlyphPoint a_glyph_q = matching_anotherSide.GlyphPoint_Q;
                        if (a_glyph_p != null)
                        {

                            a_glyph_p.AdjustedY = -remaining;
#if DEBUG
                            if (!s_dbugAff2.ContainsKey(a_glyph_p))
                            {
                                s_dbugAff2.Add(a_glyph_p, true);
                                s_dbugAffectedPoints.Add(a_glyph_p);
                            }

#endif
                        }
                        if (a_glyph_q != null)
                        {

                            a_glyph_q.AdjustedY = -remaining;
#if DEBUG
                            if (!s_dbugAff2.ContainsKey(a_glyph_q))
                            {
                                s_dbugAff2.Add(a_glyph_q, true);
                                s_dbugAffectedPoints.Add(a_glyph_q);
                            }

#endif
                        }
                    }

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
                    EdgeLine rightside = p.GetMatchingVerticalEdge();
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

                if (glyphPoint.AdjustedY != 0)
                {
                    if (i == 0)
                    {
                        //first point
                        tx.MoveTo(first_px = p.X, first_py = (float)(p.Y + glyphPoint.AdjustedY));
                    }
                    else
                    {
                        tx.LineTo(p.X, (float)(p.Y + glyphPoint.AdjustedY));
                    }
                }
                else
                {
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
            }
            //close

            tx.CloseContour();
        }


        void WalkHubCenter(Vector2 hubCenter)
        {

            //#if DEBUG   
            //            painter.FillRectLBWH(hubCenter.X * pxscale, hubCenter.Y * pxscale, 5, 5, PixelFarm.Drawing.Color.Red);
            //#endif

        }
        void WalkFromBranchHeadToHubCenter(Vector2 brHead, Vector2 hubCenter)
        {
            //#if DEBUG
            //            painter.Line(
            //                 brHead.X * pxscale, brHead.Y * pxscale,
            //                 hubCenter.X * pxscale, hubCenter.Y * pxscale,
            //                 PixelFarm.Drawing.Color.Red);
            //#endif
        }
        void WalkFromHubCenterToJoint(Vector2 joint_pos, Vector2 hubCenter)
        {
            //this is a line that link from head of lineHub to ANOTHER branch (at specific joint)
            //#if DEBUG
            //            painter.Line(
            //               joint_pos.X * pxscale, joint_pos.Y * pxscale,
            //               hubCenter.X * pxscale, hubCenter.Y * pxscale,
            //               PixelFarm.Drawing.Color.Magenta);
            //#endif
        }
        void SetJointDetail(GlyphBoneJoint joint, StrokeJoint strokeJoint)
        {
            switch (joint.SelectedEdgePointCount)
            {
                default: throw new NotSupportedException();
                case 0: break;
                case 1:
                    strokeJoint._ribA_endAt = joint.RibEndPointA;
                    strokeJoint.hasRibA = true;
                    break;
                case 2:
                    strokeJoint._ribA_endAt = joint.RibEndPointA;
                    strokeJoint._ribB_endAt = joint.RibEndPointB;
                    strokeJoint.hasRibA = true;
                    strokeJoint.hasRibB = true;//TODO: review here

                    //if (
                    //    Math.Abs((joint.RibA_ArcTan() - joint.RibB_ArcTan())) <
                    //    Math.Atan2(1,1))
                    //{
                    //    strokeJoint.hasRibB = false ;//TODO: review here
                    //}

                    break;
            }
            //check if ribB and A angle
            //if less than 90 degree
            //remove this rib


            if (joint.TipPoint != System.Numerics.Vector2.Zero)
            {
                //TODO: review here, tip point
                strokeJoint.hasTip = true;
                strokeJoint._tip_endAt = joint.TipPoint;
            }
        }

        void WalkToJoint(StrokeJoint joint)
        {

            //mid point
            Vector2 jointPos = joint._position;
            //mid bone point***  
            WalkToCenterOfJoint(jointPos);
            //a
            if (joint.hasRibA)
            {
                WalkRib(joint._ribA_endAt, jointPos);
            }
            //b
            if (joint.hasRibB)
            {
                WalkRib(joint._ribB_endAt, jointPos);
            }
            //
            if (joint.hasTip)
            {
                WalkFromJointToTip(jointPos, joint._tip_endAt);
            }

        }
        void WalkToJoint2(StrokeJoint joint, List<Vector2> output, float pxscale)
        {

            //mid point
            Vector2 jointPos = joint._position;
            ////mid bone point***  
            //WalkToCenterOfJoint(jointPos);
            //a
            if (joint.hasRibA)
            {

                output.Insert(0, joint._ribA_endAt * pxscale);
            }
            //b
            if (joint.hasRibB)
            {
                output.Add(joint._ribB_endAt * pxscale);
            }
            //
            if (joint.hasTip)
            {
                output.Add(joint._tip_endAt * pxscale);
            }

        }
        void WalkToCenterOfJoint(Vector2 jointCenter)
        {
            //#if DEBUG
            //            painter.FillRectLBWH(jointCenter.X * pxscale, jointCenter.Y * pxscale, 4, 4, PixelFarm.Drawing.Color.Yellow);
            //#endif
        }
        void WalkFromJointToTip(Vector2 contactPoint, Vector2 tipPoint)
        {
            //#if DEBUG
            //            painter.Line(
            //               contactPoint.X * pxscale, contactPoint.Y * pxscale,
            //               tipPoint.X * pxscale, tipPoint.Y * pxscale,
            //               PixelFarm.Drawing.Color.White);
            //#endif
        }
        void WalkRib(System.Numerics.Vector2 vec, System.Numerics.Vector2 jointPos)
        {
            //#if DEBUG
            //            //rib attach point         
            //            painter.FillRectLBWH(vec.X * pxscale, vec.Y * pxscale, 4, 4, PixelFarm.Drawing.Color.Green);

            //            //------------------------------------------------------------------
            //            //rib line
            //            painter.Line(
            //                jointPos.X * pxscale, jointPos.Y * pxscale,
            //                vec.X * pxscale, vec.Y * pxscale);
            //            //------------------------------------------------------------------
            //#endif
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
        void RegenerateBorders(List<StrokeSegment> segments, int startAt, int endAt)
        {
            //regenerate stroke border
            List<Vector2> newBorders = new List<Vector2>();
            for (int i = startAt; i < endAt; ++i)
            {
                StrokeSegment segment = segments[i];
                StrokeJoint jointA = segment.a;
                StrokeJoint jointB = segment.b;


                if (jointA != null && jointB != null)
                {

                    //create perpendicular 
                    Vector2 delta;
                    GeneratePerpendicularLines(jointA._position, jointB._position, 5 / pxScale, out delta);
                    //upper and lower
                    newBorders.Insert(0, (jointA._position + new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
                    newBorders.Add((jointA._position - new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
                    //
                    newBorders.Insert(0, (jointB._position + new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
                    newBorders.Add((jointB._position - new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
                }
                if (jointA != null && jointA.hasTip)
                {
                    Vector2 jointAPoint = jointA._position;
                    Vector2 delta;
                    GeneratePerpendicularLines(jointA._position, jointA._tip_endAt, 5 / pxScale, out delta);
                    newBorders.Insert(0, (jointA._position + new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
                    newBorders.Add((jointA._position - new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
                    //
                    newBorders.Insert(0, (jointA._tip_endAt + new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
                    newBorders.Add((jointA._tip_endAt - new Vector2((float)delta.X, (float)delta.Y)) * pxScale);
                }
            }
            ////---------------------------------------------------
            //int newBorderSegmentCount = newBorders.Count;
            //VertexStore vxs = new VertexStore();
            //for (int n = 0; n < newBorderSegmentCount; ++n)
            //{
            //    Vector2 borderSegment = newBorders[n];
            //    if (n == 0)
            //    {
            //        vxs.AddMoveTo(borderSegment.X, borderSegment.Y);
            //    }
            //    else
            //    {
            //        vxs.AddLineTo(borderSegment.X, borderSegment.Y);
            //    }
            //}
            //vxs.AddCloseFigure();
            ////---------------------------------------------------
            //painter.Fill(vxs, PixelFarm.Drawing.Color.Red);
            ////---------------------------------------------------
        }
        void WalkStrokeLine(StrokeLine branch)
        {
            List<StrokeSegment> segments = branch._segments;
            int count = segments.Count;

            int startAt = 0;
            int endAt = startAt + count;
            //#if DEBUG
            //            var prevColor = painter.StrokeColor;
            //            painter.StrokeColor = PixelFarm.Drawing.Color.White;
            //#endif

            if (dbugDrawRegeneratedOutlines)
            {
                //old 
                RegenerateBorders(segments, startAt, endAt);
            }

            for (int i = startAt; i < endAt; ++i)
            {
                StrokeSegment segment = segments[i];
                StrokeJoint jointA = segment.a;
                StrokeJoint jointB = segment.b;
                bool valid = false;
                if (jointA != null && jointB != null)
                {
                    Vector2 jointAPoint = jointA._position;
                    Vector2 jointBPoint = jointB._position;

                    //#if DEBUG
                    //                    painter.Line(
                    //                        jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                    //                        jointBPoint.X * pxscale, jointBPoint.Y * pxscale
                    //                        );
                    //#endif
                    WalkToJoint(jointA);
                    WalkToJoint(jointB);
                    valid = true;
                }
                if (jointA != null && jointA.hasTip)
                {
                    Vector2 jointAPoint = jointA._position;
                    Vector2 tipEnd = jointA._tip_endAt;
                    //#if DEBUG
                    //                    painter.Line(
                    //                        jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                    //                        tipEnd.X * pxscale, tipEnd.Y * pxscale
                    //                        );
                    //#endif
                    WalkToJoint(jointA);
                    valid = true;
                }

                if (i == 0)
                {
                    //for first bone
                    //#if DEBUG
                    //                    Vector2 headpos = branch._head;
                    //                    painter.FillRectLBWH(headpos.X * pxscale, headpos.Y * pxscale, 5, 5);
                    //#endif
                }
                if (!valid)
                {
                    throw new NotSupportedException();
                }
            }
            //#if DEBUG
            //            painter.StrokeColor = prevColor;
            //#endif
        }
        void CreateStrokeSegments(GlyphCentroidBranch branch, StrokeLine strokeLine)
        {

            List<GlyphBone> glyphBones = branch.bones;
            int glyphBoneCount = glyphBones.Count;

            List<StrokeSegment> strokeSegments = new List<StrokeSegment>(glyphBoneCount);
            strokeLine._segments = strokeSegments;

            int startAt = 0;
            int endAt = startAt + glyphBoneCount;

            for (int i = startAt; i < endAt; ++i)
            {
                //draw line
                GlyphBone bone = glyphBones[i];
                GlyphBoneJoint jointA = bone.JointA;
                GlyphBoneJoint jointB = bone.JointB;
                bool valid = false;
                if (jointA != null && jointB != null)
                {
                    StrokeJoint a = new StrokeJoint(jointA.Position);
                    StrokeJoint b = new StrokeJoint(jointB.Position);
                    //position of joint
                    SetJointDetail(jointA, a);
                    SetJointDetail(jointB, b);
                    //
                    StrokeSegment seg = new StrokeSegment(a, b);
                    strokeSegments.Add(seg);
                    valid = true;
                }
                if (jointA != null && bone.TipEdge != null)
                {
                    StrokeJoint a = new StrokeJoint(jointA.Position);
                    SetJointDetail(jointA, a);
                    StrokeSegment seg = new StrokeSegment(a, null);
                    strokeSegments.Add(seg);
                    valid = true;
                }
                if (!valid)
                {
                    throw new NotSupportedException();
                }
            }
        }

#if DEBUG
        internal List<GlyphTriangle> dbugGetGlyphTriangles()
        {
            return _dbugTempIntermediateOutline.GetTriangles();
        }
        internal Dictionary<GlyphTriangle, CentroidLineHub> dbugGetCentroidLineHubs()
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