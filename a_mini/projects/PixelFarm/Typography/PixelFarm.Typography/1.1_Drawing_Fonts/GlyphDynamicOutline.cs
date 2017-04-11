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
#if DEBUG
        CanvasPainter painter;
        float pxscale;
#endif
        GlyphFitOutline fitOutline;
        public GlyphDynamicOutline(GlyphFitOutline fitOutline)
        {
            this.fitOutline = fitOutline;
        }
#if DEBUG
        public void dbugSetCanvasPainter(CanvasPainter painter, float pxscale)
        {
            this.painter = painter;
            this.pxscale = pxscale;
        }
#endif
        public void Analyze()
        {
            //each centroid hub

            Dictionary<GlyphTriangle, CentroidLineHub> centroidLineHubs = fitOutline.GetCentroidLineHubs();
            foreach (CentroidLineHub lineHub in centroidLineHubs.Values)
            {
                //a line hub contains many centriod branches
                //
                Dictionary<GlyphTriangle, GlyphCentroidBranch> branches = lineHub.GetAllBranches();
                Vector2 hubCenter = lineHub.GetCenterPos();

                foreach (GlyphCentroidBranch branch in branches.Values)
                {
                    //head of this branch
                    Vector2 brHead = branch.GetHeadPosition();
                    //a branch contains small centroid line segments.
                    WalkBoneLinks(branch);
                    //draw  a line link to centroid of target triangle
                    WalkFromBranchHeadToHubCenter(brHead, hubCenter);
                }

                WalkHubCenter(hubCenter);

                GlyphBoneJoint joint = lineHub.GetHeadConnectedJoint();
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

        void WalkBoneJoint(GlyphBoneJoint joint)
        {

            //mid point
            Vector2 jointPos = joint.Position;
            //mid bone point***  
            WalkToCenterOfBoneJoint(jointPos);
            switch (joint.SelectedEdgePointCount)
            {
                default: throw new NotSupportedException();
                case 0: break;
                case 1:
                    WalkBoneRib(joint.RibEndPointA, joint);
                    break;
                case 2:

                    WalkBoneRib(joint.RibEndPointA, joint);
                    WalkBoneRib(joint.RibEndPointB, joint);
                    break;
            }
            if (joint.TipPoint != System.Numerics.Vector2.Zero)
            {
                //if we have tip point 
                WalkFromJointToTip(jointPos, joint.TipPoint);
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
        void WalkBoneRib(System.Numerics.Vector2 vec, GlyphBoneJoint joint)
        {
            var jointPos = joint.Position;
            double mid_x = jointPos.X;
            double mid_y = jointPos.Y;

            //rib attach point            

            painter.FillRectLBWH(vec.X * pxscale, vec.Y * pxscale, 4, 4, PixelFarm.Drawing.Color.Green);

            //------------------------------------------------------------------
            //rib line
            painter.Line(
                mid_x * pxscale, mid_y * pxscale,
                vec.X * pxscale, vec.Y * pxscale);
            //------------------------------------------------------------------
        }
        void WalkBoneLinks(GlyphCentroidBranch branch)
        {

            List<GlyphBone> glyphBones = branch.bones;
            int glyphBoneCount = glyphBones.Count;
            int startAt = 0;
            int endAt = startAt + glyphBoneCount;

            var prevColor = painter.StrokeColor;
            painter.StrokeColor = PixelFarm.Drawing.Color.White;
            for (int i = startAt; i < endAt; ++i)
            {
                //draw line

                GlyphBone bone = glyphBones[i];
                GlyphBoneJoint jointA = bone.JointA;
                GlyphBoneJoint jointB = bone.JointB;
                bool valid = false;
                if (jointA != null && jointB != null)
                {

                    var jointAPoint = jointA.Position;
                    var jointBPoint = jointB.Position;

                    painter.Line(
                        jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                        jointBPoint.X * pxscale, jointBPoint.Y * pxscale
                        );
                    WalkBoneJoint(jointA);
                    WalkBoneJoint(jointB);
                    valid = true;
                }
                if (jointA != null && bone.TipEdge != null)
                {

                    var jointAPoint = jointA.Position;
                    var mid = bone.TipEdge.GetMidPoint();


                    painter.Line(
                        jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                        mid.X * pxscale, mid.Y * pxscale
                        );
                    WalkBoneJoint(jointA);
                    valid = true;
                }
                if (i == 0)
                {
                    //for first bone
                    var headpos = branch.GetHeadPosition();
                    painter.FillRectLBWH(headpos.X * pxscale, headpos.Y * pxscale, 5, 5);

                }
                if (!valid)
                {
                    throw new NotSupportedException();
                }
            }
            painter.StrokeColor = prevColor;

        }
    }
}