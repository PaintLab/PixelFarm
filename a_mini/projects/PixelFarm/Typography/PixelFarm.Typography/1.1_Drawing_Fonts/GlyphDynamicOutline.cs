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

        }
        class StrokeLinHub
        {
            public Vector2 _center;
            public GlyphBoneJoint _headConnectedJoint;
            public List<StrokeLine> _branches;
        }

#if DEBUG
        CanvasPainter painter;
        float pxscale;
#endif

        List<StrokeLinHub> _strokeLineHub;
        public GlyphDynamicOutline(GlyphFitOutline fitOutline)
        {


            Dictionary<GlyphTriangle, CentroidLineHub> centroidLineHubs = fitOutline.GetCentroidLineHubs();
            _strokeLineHub = new List<StrokeLinHub>(centroidLineHubs.Count);

            foreach (CentroidLineHub lineHub in centroidLineHubs.Values)
            {
                Dictionary<GlyphTriangle, GlyphCentroidBranch> branches = lineHub.GetAllBranches();

                //a line hub contains many centriod branches                                 
                StrokeLinHub internalLineHub = new StrokeLinHub();
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
            foreach (StrokeLinHub lineHub in _strokeLineHub)
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
                    //a branch contains small centroid line segments.                     
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

            painter.Line(
                 brHead.X * pxscale, brHead.Y * pxscale,
                 hubCenter.X * pxscale, hubCenter.Y * pxscale,
                 PixelFarm.Drawing.Color.Red);

        }
        void WalkFromHubCenterToJoint(Vector2 joint_pos, Vector2 hubCenter)
        {
            //this is a line that link from head of lineHub to ANOTHER branch (at specific joint)
            painter.Line(
               joint_pos.X * pxscale, joint_pos.Y * pxscale,
               hubCenter.X * pxscale, hubCenter.Y * pxscale,
               PixelFarm.Drawing.Color.Magenta);
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
                    break;
            }
            if (joint.TipPoint != System.Numerics.Vector2.Zero)
            {
                //TODO: review here, tip point
                strokeJoint.hasTip = true;
                strokeJoint._tip_endAt = joint.TipPoint;
            }
        }

        void WalkBoneJoint(StrokeJoint joint)
        {

            //mid point
            Vector2 jointPos = joint._position;
            //mid bone point***  
            WalkToCenterOfBoneJoint(jointPos);
            //a
            if (joint.hasRibA)
            {
                WalkBoneRib(joint._ribA_endAt, jointPos);
            }
            //b
            if (joint.hasRibB)
            {
                WalkBoneRib(joint._ribB_endAt, jointPos);
            }
            //
            if (joint.hasTip)
            {
                WalkFromJointToTip(jointPos, joint._tip_endAt);
            }

        }
        void WalkToCenterOfBoneJoint(Vector2 jointCenter)
        {
            painter.FillRectLBWH(jointCenter.X * pxscale, jointCenter.Y * pxscale, 4, 4, PixelFarm.Drawing.Color.Yellow);

        }
        void WalkFromJointToTip(Vector2 contactPoint, Vector2 tipPoint)
        {

            painter.Line(
               contactPoint.X * pxscale, contactPoint.Y * pxscale,
               tipPoint.X * pxscale, tipPoint.Y * pxscale,
               PixelFarm.Drawing.Color.White);

        }
        void WalkBoneRib(System.Numerics.Vector2 vec, System.Numerics.Vector2 jointPos)
        {
            if (vec == Vector2.Zero)
            {
            }
            //rib attach point         
            painter.FillRectLBWH(vec.X * pxscale, vec.Y * pxscale, 4, 4, PixelFarm.Drawing.Color.Green);

            //------------------------------------------------------------------
            //rib line
            painter.Line(
                jointPos.X * pxscale, jointPos.Y * pxscale,
                vec.X * pxscale, vec.Y * pxscale);
            //------------------------------------------------------------------
        }
        void WalkStrokeLine(StrokeLine branch)
        {
            List<StrokeSegment> segments = branch._segments;
            int count = segments.Count;

            int startAt = 0;
            int endAt = startAt + count;
            var prevColor = painter.StrokeColor;
            painter.StrokeColor = PixelFarm.Drawing.Color.White;
            for (int i = startAt; i < endAt; ++i)
            {
                StrokeSegment segment = segments[i];
                StrokeJoint jointA = segment.a;
                StrokeJoint jointB = segment.b;
                bool valid = false;
                if (jointA != null && jointB != null)
                {
                    var jointAPoint = jointA._position;
                    var jointBPoint = jointB._position;

                    painter.Line(
                        jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                        jointBPoint.X * pxscale, jointBPoint.Y * pxscale
                        );
                    WalkBoneJoint(jointA);
                    WalkBoneJoint(jointB);
                    valid = true;
                }
                if (jointA != null && jointA.hasTip)
                {
                    var jointAPoint = jointA._position;
                    Vector2 tipEnd = jointA._tip_endAt;
                    painter.Line(
                        jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                        tipEnd.X * pxscale, tipEnd.Y * pxscale
                        );
                    WalkBoneJoint(jointA);
                    valid = true;
                }
                if (i == 0)
                {
                    //for first bone
                    var headpos = branch._head;
                    painter.FillRectLBWH(headpos.X * pxscale, headpos.Y * pxscale, 5, 5);

                }
                if (!valid)
                {
                    throw new NotSupportedException();
                }
            }
            painter.StrokeColor = prevColor;
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