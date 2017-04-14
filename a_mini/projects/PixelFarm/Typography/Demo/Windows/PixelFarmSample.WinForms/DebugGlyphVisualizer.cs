//MIT, 2014-2017, WinterDev
using System;
using System.Collections.Generic;

using PixelFarm.Agg;
using PixelFarm.Drawing.Fonts;
using Typography.OpenFont;
using Typography.Rendering;
namespace SampleWinForms.UI
{

    class DebugGlyphVisualizer
    {
        DebugGlyphVisualizerInfoView _infoView;

        //

        Typeface _typeface;
        float _sizeInPoint;
        GlyphPathBuilder builder;
        VertexStorePool _vxsPool = new VertexStorePool();
        CanvasPainter painter;
        float _pxscale;
        HintTechnique _latestHint;
        char _testChar;
        public CanvasPainter CanvasPainter { get { return painter; } set { painter = value; } }
        public void SetFont(Typeface typeface, float sizeInPoint)
        {
            _typeface = typeface;
            _sizeInPoint = sizeInPoint;
            builder = new GlyphPathBuilder(typeface);
            FillBackGround = true;//default 

        }

        public bool UseLcdTechnique { get; set; }
        public bool FillBackGround { get; set; }
        public bool DrawBorder { get; set; }
        public bool OffsetMinorX { get; set; }
        public bool ShowTess { get; set; }

        public string MinorOffsetInfo { get; set; }
        public DebugGlyphVisualizerInfoView VisualizeInfoView
        {
            get { return _infoView; }
            set
            {
                _infoView = value;
                value.Owner = this;
                value.RequestGlyphRender += (s, e) =>
                {
                    //refresh render output 
                    RenderChar(_testChar, _latestHint);
                };
            }
        }
        public void DrawMarker(float x, float y, PixelFarm.Drawing.Color color)
        {
            painter.FillRectLBWH(x, y, 4, 4, color);
        }
        public void RenderChar(char testChar, HintTechnique hint)
        {
            builder.SetHintTechnique(hint);
#if DEBUG
            builder.dbugAlwaysDoCurveAnalysis = true;
#endif
            _infoView.Clear();
            _latestHint = hint;
            _testChar = testChar;
            //----------------------------------------------------
            builder.Build(testChar, _sizeInPoint);
            var txToVxs1 = new GlyphTranslatorToVxs();
            builder.ReadShapes(txToVxs1);

#if DEBUG 
            var ps = txToVxs1.dbugGetPathWriter();
            _infoView.ShowOrgBorderInfo(ps.Vxs);
#endif
            VertexStore vxs = new VertexStore();
            txToVxs1.WriteOutput(vxs, _vxsPool);
            //----------------------------------------------------

            //----------------------------------------------------
            painter.UseSubPixelRendering = this.UseLcdTechnique;
            //5. use PixelFarm's Agg to render to bitmap...
            //5.1 clear background
            painter.Clear(PixelFarm.Drawing.Color.White);

            RectD bounds = new RectD();
            BoundingRect.GetBoundingRect(new VertexStoreSnap(vxs), ref bounds);
            //----------------------------------------------------
            float scale = _typeface.CalculateToPixelScaleFromPointSize(_sizeInPoint);
            _pxscale = scale;
            var leftControl = this.LeftXControl;
            var left2 = leftControl * scale;
            int floor_1 = (int)left2;
            float diff = left2 - floor_1;
            //----------------------------------------------------
            if (OffsetMinorX)
            {
                MinorOffsetInfo = left2.ToString() + " =>" + floor_1 + ",diff=" + diff;
            }
            else
            {
                MinorOffsetInfo = left2.ToString();
            }


            //5. use PixelFarm's Agg to render to bitmap...
            //5.1 clear background
            painter.Clear(PixelFarm.Drawing.Color.White);

            if (FillBackGround)
            {
                //5.2 
                painter.FillColor = PixelFarm.Drawing.Color.Black;

                float xpos = 5;// - diff;
                if (OffsetMinorX)
                {
                    xpos -= diff;
                }

                painter.SetOrigin(xpos, 10);
                painter.Fill(vxs);
            }
            if (DrawBorder)
            {
                //5.4  
                painter.StrokeColor = PixelFarm.Drawing.Color.Green;
                //user can specific border width here... 
                //5.5 
                painter.Draw(vxs);
                //--------------
                int markOnVertexNo = _infoView.DebugMarkVertexCommand;
                double x, y;
                vxs.GetVertex(markOnVertexNo, out x, out y);
                painter.FillRectLBWH(x, y, 4, 4, PixelFarm.Drawing.Color.Red);
                //--------------
                _infoView.ShowFlatternBorderInfo(vxs);
                //--------------
            }
#if DEBUG
            builder.dbugAlwaysDoCurveAnalysis = false;
#endif

            if (ShowTess)
            {
                RenderTessTesult();
            }

            if (DrawDynamicOutline)
            {
                GlyphFitOutline fitOutline = builder.LatestGlyphFitOutline;
                dbugDynamicOutline(painter, fitOutline, scale, true);
            }

        }
        public void RenderTessTesult()
        {
#if DEBUG

            GlyphFitOutline fitOutline = builder.LatestGlyphFitOutline;
            if (fitOutline != null)
            {
                dbugDrawTriangulatedGlyph(painter, fitOutline, _pxscale);
            }
#endif
        }
        public float LeftXControl
        {
            get { return builder.LeftXControl; }
        }
        public bool DrawCentroidBone { get; set; }
        public bool DrawGlyphBone { get; set; }
        public bool DrawTrianglesAndEdges { get; set; }
        public bool DrawDynamicOutline { get; set; }
        public bool DrawRegenerateOutline { get; set; }
        //
#if DEBUG
        void DrawPointKind(CanvasPainter painter, GlyphPoint2D point, float scale)
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
                        painter.FillRectLBWH(point.x * scale, point.y * scale, 2, 2, PixelFarm.Drawing.Color.Red);
                        //if (point.AdjustedY != 0)
                        //{
                        //    painter.FillRectLBWH(point.x * scale, point.y * scale + 30, 5, 5, PixelFarm.Drawing.Color.Red);
                        //}
                        //else
                        //{
                        //    painter.FillRectLBWH(point.x * scale, point.y * scale, 2, 2, PixelFarm.Drawing.Color.Red);
                        //}

                    }
                    break;

            }
        }
        void DrawEdge(CanvasPainter painter, EdgeLine edge, float scale)
        {
            if (edge.IsOutside)
            {
                //free side     

                Poly2Tri.TriangulationPoint p = edge.p;
                Poly2Tri.TriangulationPoint q = edge.q;

                var u_data_p = p.userData as GlyphPoint2D;
                var u_data_q = q.userData as GlyphPoint2D;


                DrawPointKind(painter, u_data_p, scale);
                DrawPointKind(painter, u_data_q, scale);
                _infoView.ShowEdge(edge);

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

                //show info: => edge point
                if (_infoView.HasDebugMark)
                {
                    double prevWidth = painter.StrokeWidth;
                    painter.StrokeWidth = 3;
                    painter.Line(edge.x0 * scale, edge.y0 * scale, edge.x1 * scale, edge.y1 * scale, PixelFarm.Drawing.Color.Yellow);
                    painter.StrokeWidth = prevWidth;
                }
                else
                {
                    painter.Line(edge.x0 * scale, edge.y0 * scale, edge.x1 * scale, edge.y1 * scale);
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
                painter.Line(edge.x0 * scale, edge.y0 * scale, edge.x1 * scale, edge.y1 * scale);
            }




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
        public void dbugDynamicOutline(CanvasPainter painter, GlyphFitOutline glyphFitOutline, float pxscale, bool withRegenerateOutlines)
        {
            GlyphDynamicOutline dynamicOutline = new GlyphDynamicOutline(glyphFitOutline);
#if DEBUG
            dynamicOutline.dbugSetCanvasPainter(painter, pxscale);
            dynamicOutline.dbugDrawRegeneratedOutlines = withRegenerateOutlines;
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