//MIT, 2017, WinterDev
using System;
using System.Numerics;
using System.Collections.Generic;
using Typography.OpenFont;

using PixelFarm.Agg;

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

#if DEBUG
        CanvasPainter painter;
        float pxscale;
        public bool dbugDrawRegeneratedOutlines { get; set; }
#endif

        List<StrokeLineHub> _strokeLineHub;
        public GlyphDynamicOutline(GlyphFitOutline fitOutline)
        {


            Dictionary<GlyphTriangle, CentroidLineHub> centroidLineHubs = fitOutline.GetCentroidLineHubs();
            _strokeLineHub = new List<StrokeLineHub>(centroidLineHubs.Count);

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
#if DEBUG
        public void dbugSetCanvasPainter(CanvasPainter painter, float pxscale)
        {
            this.painter = painter;
            this.pxscale = pxscale;
        }
#endif
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
        public void GenerateOutput(IGlyphTranslator tx, float pxScale)
        {

        }
        void WalkHubCenter(Vector2 hubCenter)
        {

#if DEBUG   
            painter.FillRectLBWH(hubCenter.X * pxscale, hubCenter.Y * pxscale, 5, 5, PixelFarm.Drawing.Color.Red);
#endif

        }
        void WalkFromBranchHeadToHubCenter(Vector2 brHead, Vector2 hubCenter)
        {
#if DEBUG
            painter.Line(
                 brHead.X * pxscale, brHead.Y * pxscale,
                 hubCenter.X * pxscale, hubCenter.Y * pxscale,
                 PixelFarm.Drawing.Color.Red);
#endif
        }
        void WalkFromHubCenterToJoint(Vector2 joint_pos, Vector2 hubCenter)
        {
            //this is a line that link from head of lineHub to ANOTHER branch (at specific joint)
#if DEBUG
            painter.Line(
               joint_pos.X * pxscale, joint_pos.Y * pxscale,
               hubCenter.X * pxscale, hubCenter.Y * pxscale,
               PixelFarm.Drawing.Color.Magenta);
#endif
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
#if DEBUG
            painter.FillRectLBWH(jointCenter.X * pxscale, jointCenter.Y * pxscale, 4, 4, PixelFarm.Drawing.Color.Yellow);
#endif
        }
        void WalkFromJointToTip(Vector2 contactPoint, Vector2 tipPoint)
        {
#if DEBUG
            painter.Line(
               contactPoint.X * pxscale, contactPoint.Y * pxscale,
               tipPoint.X * pxscale, tipPoint.Y * pxscale,
               PixelFarm.Drawing.Color.White);
#endif
        }
        void WalkRib(System.Numerics.Vector2 vec, System.Numerics.Vector2 jointPos)
        {
#if DEBUG
            //rib attach point         
            painter.FillRectLBWH(vec.X * pxscale, vec.Y * pxscale, 4, 4, PixelFarm.Drawing.Color.Green);

            //------------------------------------------------------------------
            //rib line
            painter.Line(
                jointPos.X * pxscale, jointPos.Y * pxscale,
                vec.X * pxscale, vec.Y * pxscale);
            //------------------------------------------------------------------
#endif
        }

        static void GeneratePerpendicularLines(
             float x0, float y0, float x1, float y1, float len,
             out PixelFarm.VectorMath.Vector delta)
        {
            PixelFarm.VectorMath.Vector v0 = new PixelFarm.VectorMath.Vector(x0, y0);
            PixelFarm.VectorMath.Vector v1 = new PixelFarm.VectorMath.Vector(x1, y1);

            delta = (v1 - v0) / 2;
            delta = delta.NewLength(len);
            delta.Rotate(90);

        }
        static void GeneratePerpendicularLines(
          Vector2 p0, Vector2 p1, float len,
          out PixelFarm.VectorMath.Vector delta)
        {
            PixelFarm.VectorMath.Vector v0 = new PixelFarm.VectorMath.Vector(p0.X, p0.Y);
            PixelFarm.VectorMath.Vector v1 = new PixelFarm.VectorMath.Vector(p1.X, p1.Y);

            delta = (v1 - v0) / 2;
            delta = delta.NewLength(len);
            delta.Rotate(90);

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
                    PixelFarm.VectorMath.Vector delta;
                    GeneratePerpendicularLines(jointA._position, jointB._position, 5 / pxscale, out delta);
                    //upper and lower
                    newBorders.Insert(0, (jointA._position + new Vector2((float)delta.X, (float)delta.Y)) * pxscale);
                    newBorders.Add((jointA._position - new Vector2((float)delta.X, (float)delta.Y)) * pxscale);
                    //
                    newBorders.Insert(0, (jointB._position + new Vector2((float)delta.X, (float)delta.Y)) * pxscale);
                    newBorders.Add((jointB._position - new Vector2((float)delta.X, (float)delta.Y)) * pxscale);
                }
                if (jointA != null && jointA.hasTip)
                {
                    Vector2 jointAPoint = jointA._position;
                    PixelFarm.VectorMath.Vector delta;
                    GeneratePerpendicularLines(jointA._position, jointA._tip_endAt, 5 / pxscale, out delta);
                    newBorders.Insert(0, (jointA._position + new Vector2((float)delta.X, (float)delta.Y)) * pxscale);
                    newBorders.Add((jointA._position - new Vector2((float)delta.X, (float)delta.Y)) * pxscale);
                    //
                    newBorders.Insert(0, (jointA._tip_endAt + new Vector2((float)delta.X, (float)delta.Y)) * pxscale);
                    newBorders.Add((jointA._tip_endAt - new Vector2((float)delta.X, (float)delta.Y)) * pxscale);
                }
            }
            //---------------------------------------------------
            int newBorderSegmentCount = newBorders.Count;
            VertexStore vxs = new VertexStore();
            for (int n = 0; n < newBorderSegmentCount; ++n)
            {
                Vector2 borderSegment = newBorders[n];
                if (n == 0)
                {
                    vxs.AddMoveTo(borderSegment.X, borderSegment.Y);
                }
                else
                {
                    vxs.AddLineTo(borderSegment.X, borderSegment.Y);
                }
            }
            vxs.AddCloseFigure();
            //---------------------------------------------------
            painter.Fill(vxs, PixelFarm.Drawing.Color.Red);
            //---------------------------------------------------
        }
        void WalkStrokeLine(StrokeLine branch)
        {
            List<StrokeSegment> segments = branch._segments;
            int count = segments.Count;

            int startAt = 0;
            int endAt = startAt + count;
#if DEBUG
            var prevColor = painter.StrokeColor;
            painter.StrokeColor = PixelFarm.Drawing.Color.White;
#endif

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

#if DEBUG
                    painter.Line(
                        jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                        jointBPoint.X * pxscale, jointBPoint.Y * pxscale
                        );
#endif
                    WalkToJoint(jointA);
                    WalkToJoint(jointB);
                    valid = true;
                }
                if (jointA != null && jointA.hasTip)
                {
                    Vector2 jointAPoint = jointA._position;
                    Vector2 tipEnd = jointA._tip_endAt;
#if DEBUG
                    painter.Line(
                        jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                        tipEnd.X * pxscale, tipEnd.Y * pxscale
                        );
#endif
                    WalkToJoint(jointA);
                    valid = true;
                }

                if (i == 0)
                {
                    //for first bone
#if DEBUG
                    Vector2 headpos = branch._head;
                    painter.FillRectLBWH(headpos.X * pxscale, headpos.Y * pxscale, 5, 5);
#endif
                }
                if (!valid)
                {
                    throw new NotSupportedException();
                }
            }
#if DEBUG
            painter.StrokeColor = prevColor;
#endif
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

    }
}