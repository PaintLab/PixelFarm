//MIT, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using System.Numerics;
using PixelFarm.Agg;
using PixelFarm.Drawing.Fonts;
using Typography.OpenFont;
using Typography.Rendering;
namespace SampleWinForms.UI
{
    class GlyphTriangleInfo
    {
        public GlyphTriangleInfo(int triangleId, EdgeLine e0, EdgeLine e1, EdgeLine e2, double centroidX, double centroidY)
        {
            this.Id = triangleId;
            this.E0 = e0;
            this.E1 = e1;
            this.E2 = e2;
            this.CentroidX = centroidX;
            this.CentroidY = centroidY;
        }
        public int Id { get; private set; }
        public double CentroidX { get; set; }
        public double CentroidY { get; set; }
        public EdgeLine E0 { get; set; }
        public EdgeLine E1 { get; set; }
        public EdgeLine E2 { get; set; }
    }

    class DebugGlyphVisualizer : GlyphOutlineWalker
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
        public void DrawMarker(float x, float y, PixelFarm.Drawing.Color color, float sizeInPx = 8)
        {
            painter.FillRectLBWH(x, y, sizeInPx, sizeInPx, color);
        }
        public void RenderChar(char testChar, HintTechnique hint)
        {
            builder.SetHintTechnique(hint);
#if DEBUG
            GlyphBoneJoint.dbugTotalId = 0;//reset
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
            this._infoView.PxScale = scale;

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
                GlyphDynamicOutline dynamicOutline = builder.LatestGlyphFitOutline;
                DynamicOutline(painter, dynamicOutline, scale, DrawRegenerateOutline);
            }

        }

        public void RenderTessTesult()
        {
#if DEBUG

            GlyphDynamicOutline dynamicOutline = builder.LatestGlyphFitOutline;
            if (dynamicOutline != null)
            {
                this.Walk(dynamicOutline);
            }
#endif
        }
        public float LeftXControl
        {
            get { return builder.LeftXControl; }
        }

        public bool DrawRibs { get; set; }
        public bool DrawTrianglesAndEdges { get; set; }
        public bool DrawDynamicOutline { get; set; }
        public bool DrawRegenerateOutline { get; set; }
        //
#if DEBUG
        void DrawPointKind(CanvasPainter painter, GlyphPoint point, float scale)
        {
            switch (point.kind)
            {
                case PointKind.C3Start:
                case PointKind.C3End:
                case PointKind.C4Start:
                case PointKind.C4End:
                case PointKind.LineStart:
                case PointKind.LineStop:
                    painter.FillRectLBWH(point.x * scale, point.y * scale, 5, 5, PixelFarm.Drawing.Color.Red);
                    break;

            }
        }
        void DrawEdge(CanvasPainter painter, EdgeLine edge, float scale)
        {
            if (edge.IsOutside)
            {
                //free side      

                GlyphPoint u_data_p = edge.GlyphPoint_P;
                GlyphPoint u_data_q = edge.GlyphPoint_Q;


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
                        painter.StrokeColor = PixelFarm.Drawing.Color.Blue;
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
        }

        void DrawBoneJoint(CanvasPainter painter, GlyphBoneJoint joint, float pxscale)
        {
            //-------------- 
            EdgeLine p_contactEdge = joint._p_contact_edge;
            //mid point
            Vector2 jointPos = joint.Position * pxscale;//scaled joint pos
            painter.FillRectLBWH(jointPos.X, jointPos.Y, 4, 4, PixelFarm.Drawing.Color.Yellow);

            if (DrawRibs)
            {
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
            }

            if (joint.TipPoint != Vector2.Zero)
            {
                EdgeLine tipEdge = joint.TipEdge;
                float p_x = tipEdge.GlyphPoint_P.x * pxscale;
                float p_y = tipEdge.GlyphPoint_P.y * pxscale;
                float q_x = tipEdge.GlyphPoint_Q.x * pxscale;
                float q_y = tipEdge.GlyphPoint_Q.y * pxscale;

                //
                painter.Line(
                   jointPos.X, jointPos.Y,
                   p_x, p_y,
                   PixelFarm.Drawing.Color.White);
                painter.FillRectLBWH(p_x, p_y, 3, 3, PixelFarm.Drawing.Color.Green); //marker

                //
                painter.Line(
                    jointPos.X, jointPos.Y,
                    q_x, q_y,
                    PixelFarm.Drawing.Color.White);
                painter.FillRectLBWH(q_x, q_y, 3, 3, PixelFarm.Drawing.Color.Green); //marker
            }
        }
        void DrawAssocGlyphPoints(Vector2 pos, List<GlyphPoint> points)
        {
            int j = points.Count;
            for (int i = 0; i < j; ++i)
            {
                GlyphPoint p = points[i];
                painter.Line(
                      pos.X * _pxscale, pos.Y * _pxscale,
                      p.x * _pxscale, p.y * _pxscale,
                      PixelFarm.Drawing.Color.Yellow);
            }
        }
        void DrawAssocGlyphPoints(Vector2 pos, List<GlyphPoint> points, float newRelativeLen)
        {
            int j = points.Count;
            for (int i = 0; i < j; ++i)
            {
                DrawAssocGlyphPoint(pos, points[i], newRelativeLen);
            }
        }
        void DrawAssocGlyphPoint(Vector2 pos, GlyphPoint p, float newRelativeLen)
        {
            //test draw marker on different len....
            //create  

            PixelFarm.VectorMath.Vector delta = new PixelFarm.VectorMath.Vector(
                new PixelFarm.VectorMath.Vector(pos.X, pos.Y),
                new PixelFarm.VectorMath.Vector(p.x, p.y));

            double currentLen = delta.Magnitude;
            delta = delta.NewLength(currentLen * newRelativeLen);
            //
            PixelFarm.VectorMath.Vector v2 = new PixelFarm.VectorMath.Vector((float)pos.X, (float)pos.Y) + delta;
            painter.Line(
                pos.X * _pxscale, pos.Y * _pxscale,
                v2.X * _pxscale, v2.Y * _pxscale,
                PixelFarm.Drawing.Color.Red);
        }


        System.Numerics.Vector2 _branchHeadPos;
        protected override void OnBeginDrawingBoneLinks(System.Numerics.Vector2 branchHeadPos, int startAt, int endAt)
        {
            _branchHeadPos = branchHeadPos;
        }
        protected override void OnEndDrawingBoneLinks()
        {

        }


        protected override void OnDrawBone(GlyphBone bone, int boneIndex)
        {

            float newRelativeLen = 0.5f;
            float pxscale = this._pxscale;
            GlyphBoneJoint jointA = bone.JointA;
            GlyphBoneJoint jointB = bone.JointB;

            bool valid = false;
            if (jointA != null && jointB != null)
            {

                Vector2 jointAPoint = jointA.Position;
                Vector2 jointBPoint = jointB.Position;

                painter.Line(
                    jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                    jointBPoint.X * pxscale, jointBPoint.Y * pxscale,
                    bone.IsLongBone ? PixelFarm.Drawing.Color.Yellow : PixelFarm.Drawing.Color.Magenta);
                valid = true;

                _infoView.ShowBone(bone, jointA, jointB);


                if (jointA._assocGlyphPoints != null)
                {
                    DrawAssocGlyphPoints(jointA.Position, jointA._assocGlyphPoints);
                    DrawAssocGlyphPoints(jointA.Position, jointA._assocGlyphPoints, newRelativeLen);
                }
                if (jointB._assocGlyphPoints != null)
                {
                    DrawAssocGlyphPoints(jointB.Position, jointB._assocGlyphPoints);
                    DrawAssocGlyphPoints(jointB.Position, jointB._assocGlyphPoints, newRelativeLen);
                }
            }
            if (jointA != null && bone.TipEdge != null)
            {
                Vector2 jointAPoint = jointA.Position;
                Vector2 mid = bone.TipEdge.GetMidPoint();

                painter.Line(
                    jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                    mid.X * pxscale, mid.Y * pxscale,
                    bone.IsLongBone ? PixelFarm.Drawing.Color.Yellow : PixelFarm.Drawing.Color.Magenta);
                valid = true;
                _infoView.ShowBone(bone, jointA, bone.TipEdge);

                if (jointA._assocGlyphPoints != null)
                {

                    DrawAssocGlyphPoints(jointA.Position, jointA._assocGlyphPoints);
                    DrawAssocGlyphPoints(jointA.Position, jointA._assocGlyphPoints, newRelativeLen);
                }
            }

            if (bone.hasCutPointOnEdge)
            {
                Vector2 midBone = bone.GetMidPoint();
                painter.Line(
                    bone.cutPoint_onEdge.X * pxscale, bone.cutPoint_onEdge.Y * pxscale,
                    midBone.X * pxscale, midBone.Y * pxscale,
                    PixelFarm.Drawing.Color.White);
            }

            //--------
            //draw a perpendicular line from bone to associated glyph point
            List<GlyphPointToBoneLink> linkToGlyphPoints = bone._perpendiculatPoints;
            if (linkToGlyphPoints != null)
            {
                int n = linkToGlyphPoints.Count;
                for (int m = 0; m < n; ++m)
                {
                    GlyphPointToBoneLink link = linkToGlyphPoints[m];
                    painter.Line(
                      link.glyphPoint.x * pxscale, link.glyphPoint.y * pxscale,
                      link.bonePoint.X * pxscale, link.bonePoint.Y * pxscale,
                      PixelFarm.Drawing.Color.Yellow);

                    //test
                    DrawAssocGlyphPoint(link.bonePoint, link.glyphPoint, newRelativeLen);
                }
            }
            //--------
            if (boneIndex == 0)
            {
                //for first bone 
                painter.FillRectLBWH(_branchHeadPos.X * pxscale, _branchHeadPos.Y * pxscale, 5, 5, PixelFarm.Drawing.Color.DeepPink);
            }
            if (!valid)
            {
                throw new NotSupportedException();
            }
        }


        protected override void OnTriangle(int triangleId, EdgeLine e0, EdgeLine e1, EdgeLine e2, double centroidX, double centroidY)
        {
            if (DrawTrianglesAndEdges)
            {
                DrawEdge(painter, e0, _pxscale);
                DrawEdge(painter, e1, _pxscale);
                DrawEdge(painter, e2, _pxscale);

                _infoView.ShowTriangles(new GlyphTriangleInfo(triangleId, e0, e1, e2, centroidX, centroidY));
            }
        }
        protected override void OnCentroidLine(double px, double py, double qx, double qy)
        {

            float pxscale = this._pxscale;
            painter.Line(
                px * pxscale, py * pxscale,
                qx * pxscale, qy * pxscale,
                PixelFarm.Drawing.Color.Red);

            painter.FillRectLBWH(px * pxscale, py * pxscale, 2, 2, PixelFarm.Drawing.Color.Yellow);
            painter.FillRectLBWH(qx * pxscale, qy * pxscale, 2, 2, PixelFarm.Drawing.Color.Yellow);
        }
        protected override void OnCentroidLineTip_P(double px, double py, double tip_px, double tip_py)
        {
            float pxscale = this._pxscale;
            painter.Line(px * pxscale, py * pxscale,
                         tip_px * pxscale, tip_py * pxscale,
                         PixelFarm.Drawing.Color.Blue);
        }
        protected override void OnCentroidLineTip_Q(double qx, double qy, double tip_qx, double tip_qy)
        {
            float pxscale = this._pxscale;
            painter.Line(qx * pxscale, qy * pxscale,
                         tip_qx * pxscale, tip_qy * pxscale,
                         PixelFarm.Drawing.Color.Blue);
        }
        protected override void OnBoneJoint(GlyphBoneJoint joint)
        {
            DrawBoneJoint(painter, joint, _pxscale);
            _infoView.ShowJoint(joint);
        }
        //----------------------
        protected override void OnBegingLineHub(float centerX, float centerY)
        {

        }
        protected override void OnEndLineHub(float centerX, float centerY, GlyphBoneJoint joint)
        {
            painter.Line(
                      (float)_branchHeadPos.X * _pxscale, (float)_branchHeadPos.Y * _pxscale,
                      centerX * _pxscale, centerX * _pxscale,
                       PixelFarm.Drawing.Color.Red);

            painter.FillRectLBWH(centerX * _pxscale, centerY * _pxscale, 7, 7,
                   PixelFarm.Drawing.Color.White);


            if (joint != null)
            {
                Vector2 joint_pos = joint.Position;
                painter.Line(
                        joint_pos.X * _pxscale, joint_pos.Y * _pxscale,
                        centerX * _pxscale, centerY * _pxscale,
                        PixelFarm.Drawing.Color.Magenta);
            }
        }

        public void DynamicOutline(CanvasPainter painter, GlyphDynamicOutline dynamicOutline, float pxscale, bool withRegenerateOutlines)
        {

#if DEBUG 
            dynamicOutline.dbugDrawRegeneratedOutlines = withRegenerateOutlines;
#endif
            dynamicOutline.Walk();
        }
        void DrawBoneRib(CanvasPainter painter, Vector2 vec, GlyphBoneJoint joint, float pixelScale)
        {
            Vector2 jointPos = joint.Position;
            painter.FillRectLBWH(vec.X * pixelScale, vec.Y * pixelScale, 4, 4, PixelFarm.Drawing.Color.Green);
            painter.Line(jointPos.X * pixelScale, jointPos.Y * pixelScale,
                vec.X * pixelScale,
                vec.Y * pixelScale, PixelFarm.Drawing.Color.White);
        }

#endif
    }

}