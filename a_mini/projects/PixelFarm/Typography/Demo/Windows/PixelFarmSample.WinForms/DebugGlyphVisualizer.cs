//MIT, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using Typography.Rendering;
using PixelFarm.Agg;
namespace SampleWinForms.UI
{

    class DebugGlyphVisualizer
    {

        public bool DrawCentroidBone { get; set; }
        public bool DrawGlyphBone { get; set; }

        public bool DrawTrianglesAndEdges { get; set; }
#if DEBUG
        static void DrawPointKind(CanvasPainter painter, GlyphPoint2D point, float scale)
        {
            //var prevColor = painter.FillColor;
            //painter.FillColor = PixelFarm.Drawing.Color.Red;
            //if (point.AdjustedY != 0)
            //{
            //    painter.FillRectLBWH(point.x * scale, point.y * scale + 30, 5, 5);
            //}
            //else
            //{
            //    painter.FillRectLBWH(point.x * scale, point.y * scale, 2, 2);
            //}
            //painter.FillColor = prevColor;


            switch (point.kind)
            {
                case PointKind.C3Start:
                case PointKind.C3End:
                case PointKind.C4Start:
                case PointKind.C4End:
                case PointKind.LineStart:
                case PointKind.LineStop:
                    {
                        if (point.AdjustedY != 0)
                        {
                            painter.FillRectLBWH(point.x * scale, point.y * scale + 30, 5, 5, PixelFarm.Drawing.Color.Red);
                        }
                        else
                        {
                            painter.FillRectLBWH(point.x * scale, point.y * scale, 2, 2, PixelFarm.Drawing.Color.Red);
                        }

                    }
                    break;

            }
        }
        static void DrawEdge(CanvasPainter painter, EdgeLine edge, float scale)
        {
            if (edge.IsOutside)
            {
                //free side     

                Poly2Tri.TriangulationPoint p = edge.p;
                Poly2Tri.TriangulationPoint q = edge.q;

                var u_data_p = p.userData as GlyphPoint2D;
                var u_data_q = q.userData as GlyphPoint2D;

                //if show control point
                DrawPointKind(painter, u_data_p, scale);
                DrawPointKind(painter, u_data_q, scale);


                switch (edge.SlopeKind)
                {
                    default:
                        painter.StrokeColor = PixelFarm.Drawing.Color.Green;
                        break;
                    case LineSlopeKind.Vertical:
                        if (edge.IsLeftSide)
                        {
                            painter.StrokeColor = PixelFarm.Drawing.Color.Blue;
                        }
                        else
                        {
                            painter.StrokeColor = PixelFarm.Drawing.Color.LightGray;
                        }
                        break;
                    case LineSlopeKind.Horizontal:

                        if (edge.IsUpper)
                        {
                            painter.StrokeColor = PixelFarm.Drawing.Color.Red;
                        }
                        else
                        {
                            //lower edge
                            painter.StrokeColor = PixelFarm.Drawing.Color.Magenta;
                        }
                        break;
                }
            }
            else
            {
                switch (edge.SlopeKind)
                {
                    default:
                        painter.StrokeColor = PixelFarm.Drawing.Color.LightGray;
                        break;
                    case LineSlopeKind.Vertical:
                        painter.StrokeColor = PixelFarm.Drawing.Color.Blue;
                        break;
                    case LineSlopeKind.Horizontal:
                        painter.StrokeColor = PixelFarm.Drawing.Color.Yellow;
                        break;
                }
            }

            painter.Line(edge.x0 * scale, edge.y0 * scale, edge.x1 * scale, edge.y1 * scale);


            //contact edge
            //if (edge.contactToEdge != null)
            //{
            //    //has contact edge. this edge is inside edge
            //    //draw marker  
            //    double midX = (edge.x1 + edge.x0) / 2;
            //    double midY = (edge.y1 + edge.y0) / 2;
            //    painter.FillRectLBWH(midX * scale, midY * scale, 4, 4); 
            //}
        }
        void dbugDrawCentroidLine(CanvasPainter painter, GlyphCentroidLine line, float pxscale)
        {

            painter.Line(
                line.p.CentroidX * pxscale, line.p.CentroidY * pxscale,
                line.q.CentroidX * pxscale, line.q.CentroidY * pxscale,
                PixelFarm.Drawing.Color.Red);

            painter.FillRectLBWH(line.p.CentroidX * pxscale, line.p.CentroidY * pxscale, 2, 2, PixelFarm.Drawing.Color.Yellow);
            painter.FillRectLBWH(line.q.CentroidX * pxscale, line.q.CentroidY * pxscale, 2, 2, PixelFarm.Drawing.Color.Yellow);

        }
        void dbugDrawBoneJoint(CanvasPainter painter, GlyphBoneJoint joint, float pxscale)
        {
            //-------------- 
            EdgeLine p_contactEdge = joint._p_contact_edge;
            //mid point
            var jointPos = joint.Position;
            painter.FillRectLBWH(jointPos.X * pxscale, jointPos.Y * pxscale, 4, 4, PixelFarm.Drawing.Color.Yellow);

            switch (joint.SelectedEdgePointCount)
            {
                default: throw new NotSupportedException();
                case 0: break;
                case 1:
                    DrawBoneRib(painter, joint.RibEndPointA, joint, pxscale);
                    break;
                case 2:

                    DrawBoneRib(painter, joint.RibEndPointA, joint, pxscale);
                    DrawBoneRib(painter, joint.RibEndPointB, joint, pxscale);
                    break;
            }
            if (joint.TipPoint != System.Numerics.Vector2.Zero)
            {
                painter.Line(
                   jointPos.X * pxscale, jointPos.Y * pxscale,
                   joint.TipPoint.X * pxscale, joint.TipPoint.Y * pxscale,
                   PixelFarm.Drawing.Color.White);
            }
        }
        void dbugDrawBoneLinks(CanvasPainter painter, GlyphCentroidBranch branch, float pxscale)
        {
            List<GlyphBone> glyphBones = branch.bones;
            int glyphBoneCount = glyphBones.Count;
            var prev_color = painter.StrokeColor;
            int startAt = 0;
            //--------------------
            //read user input from ui ...
            //int startAt = int.Parse(txtGlyphBoneStartAt.Text);
            //int reqGlyphBoneCount = int.Parse(txtGlyphBoneCount.Text);
            //if (reqGlyphBoneCount >= 0)
            //{
            //    //reset to all 
            //    glyphBoneCount = Math.Min(reqGlyphBoneCount, glyphBoneCount);
            //    txtGlyphBoneCount.Text = glyphBoneCount.ToString();
            //}
            //else
            //{
            //    txtGlyphBoneCount.Text = glyphBoneCount.ToString();
            //}
            //--------------------
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

                    var jointAPoint = jointA.Position;
                    var jointBPoint = jointB.Position;

                    painter.Line(
                        jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                        jointBPoint.X * pxscale, jointBPoint.Y * pxscale,
                        bone.IsLongBone ? PixelFarm.Drawing.Color.Yellow : PixelFarm.Drawing.Color.Magenta);
                    valid = true;
                }
                if (jointA != null && bone.TipEdge != null)
                {
                    var jointAPoint = jointA.Position;
                    var mid = bone.TipEdge.GetMidPoint();

                    painter.Line(
                        jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                        mid.X * pxscale, mid.Y * pxscale,
                        bone.IsLongBone ? PixelFarm.Drawing.Color.Yellow : PixelFarm.Drawing.Color.Magenta);
                    valid = true;
                }
                if (i == 0)
                {
                    //for first bone
                    var headpos = branch.GetHeadPosition();
                    painter.FillRectLBWH(headpos.X * pxscale, headpos.Y * pxscale, 5, 5, PixelFarm.Drawing.Color.DeepPink);

                }
                if (!valid)
                {
                    throw new NotSupportedException();
                }

            }
            painter.StrokeColor = prev_color;
        }
        public void dbugDrawTriangulatedGlyph(CanvasPainter painter, GlyphFitOutline glyphFitOutline, float pxscale)
        {
            painter.StrokeColor = PixelFarm.Drawing.Color.Magenta;
            List<GlyphTriangle> triAngles = glyphFitOutline.GetTriangles();
            int j = triAngles.Count;
            // 
            bool drawCentroidBone = this.DrawCentroidBone;
            bool drawGlyphBone = this.DrawGlyphBone;

            if (DrawTrianglesAndEdges)
            {
                for (int i = 0; i < j; ++i)
                {
                    //---------------
                    GlyphTriangle tri = triAngles[i];
                    EdgeLine e0 = tri.e0;
                    EdgeLine e1 = tri.e1;
                    EdgeLine e2 = tri.e2;
                    //---------------
                    //draw each triangles
                    DrawEdge(painter, e0, pxscale);
                    DrawEdge(painter, e1, pxscale);
                    DrawEdge(painter, e2, pxscale);
                }
            }
            //---------------
            //draw bone 
            if (!drawCentroidBone && !drawGlyphBone)
            {
                return;
            }
            //--------------- 
            Dictionary<GlyphTriangle, CentroidLineHub> centroidLineHub = glyphFitOutline.GetCentroidLineHubs();
            //--------------- 



            foreach (CentroidLineHub lineHub in centroidLineHub.Values)
            {
                Dictionary<GlyphTriangle, GlyphCentroidBranch> branches = lineHub.GetAllBranches();
                System.Numerics.Vector2 hubCenter = lineHub.GetCenterPos();
                foreach (GlyphCentroidBranch branch in branches.Values)
                {
                    int lineCount = branch.lines.Count;
                    for (int i = 0; i < lineCount; ++i)
                    {
                        GlyphCentroidLine line = branch.lines[i];
                        if (drawCentroidBone)
                        {
                            dbugDrawCentroidLine(painter, line, pxscale);
                        }
                        if (drawGlyphBone)
                        {
                            dbugDrawBoneJoint(painter, line.BoneJoint, pxscale);
                        }
                    }

                    if (drawGlyphBone)
                    {
                        //draw bone list
                        dbugDrawBoneLinks(painter, branch, pxscale);

                        //draw link between each branch to center of hub
                        var brHead = branch.GetHeadPosition();
                        painter.Line(
                            hubCenter.X * pxscale, hubCenter.Y * pxscale,
                            brHead.X * pxscale, brHead.Y * pxscale);

                        //draw  a line link to centroid of target triangle

                        painter.Line(
                            (float)brHead.X * pxscale, (float)brHead.Y * pxscale,
                             hubCenter.X * pxscale, hubCenter.Y * pxscale,
                             PixelFarm.Drawing.Color.Red);

                    }
                }

                painter.FillRectLBWH(hubCenter.X * pxscale, hubCenter.Y * pxscale, 7, 7,
                    PixelFarm.Drawing.Color.White);

                if (drawGlyphBone)
                {
                    GlyphBoneJoint joint = lineHub.GetHeadConnectedJoint();
                    if (joint != null)
                    {
                        var joint_pos = joint.Position;
                        painter.Line(
                                joint_pos.X * pxscale, joint_pos.Y * pxscale,
                                hubCenter.X * pxscale, hubCenter.Y * pxscale,
                                PixelFarm.Drawing.Color.Magenta);
                    }
                }
            }
        }
        public void dbugDynamicOutline(CanvasPainter painter, GlyphFitOutline glyphFitOutline, float pxscale)
        {
            GlyphDynamicOutline dynamicOutline = new GlyphDynamicOutline(glyphFitOutline);
#if DEBUG
            dynamicOutline.dbugSetCanvasPainter(painter, pxscale);
#endif
            dynamicOutline.Walk();
        }
        void DrawBoneRib(CanvasPainter painter, System.Numerics.Vector2 vec, GlyphBoneJoint joint, float pixelScale)
        {
            var jointPos = joint.Position;
            painter.FillRectLBWH(vec.X * pixelScale, vec.Y * pixelScale, 4, 4, PixelFarm.Drawing.Color.Green);
            painter.Line(jointPos.X * pixelScale, jointPos.Y * pixelScale,
                vec.X * pixelScale,
                vec.Y * pixelScale, PixelFarm.Drawing.Color.White);
        }

#endif
    }

}