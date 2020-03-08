﻿//MIT, 2019-present, WinterDev 
//based on MIT, 2016, Viktor Chlumsky, Multi-channel signed distance field generator, from https://github.com/Chlumsky/msdfge)
//-----------------------------------  

using System;
using System.Collections.Generic;

using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;

namespace ExtMsdfGen
{
    /// <summary>
    /// msdf texture generator
    /// </summary>
    public class MsdfGen3
    {
        PixelFarm.CpuBlit.Rasterization.PrebuiltGammaTable _prebuiltThresholdGamma_100;
        PixelFarm.CpuBlit.Rasterization.PrebuiltGammaTable _prebuiltThresholdGamma_OverlappedBorder;
        PixelFarm.CpuBlit.Rasterization.PrebuiltGammaTable _prebuiltThresholdGamma_50;
        MsdfEdgePixelBlender _msdfEdgePxBlender = new MsdfEdgePixelBlender();

        public MsdfGen3()
        {

            _prebuiltThresholdGamma_OverlappedBorder = PixelFarm.CpuBlit.Rasterization.PrebuiltGammaTable.CreateSameValuesGammaTable(PixelFarm.CpuBlit.Rasterization.ScanlineRasterizer.AA_MASK);

            _prebuiltThresholdGamma_50 = new PixelFarm.CpuBlit.Rasterization.PrebuiltGammaTable(
                new PixelFarm.CpuBlit.FragmentProcessing.GammaThreshold(0.5f));//***50% coverage 

            _prebuiltThresholdGamma_100 = new PixelFarm.CpuBlit.Rasterization.PrebuiltGammaTable(
                new PixelFarm.CpuBlit.FragmentProcessing.GammaThreshold(1f));//*** 100% coverage 
        }
        public MsdfGenParams MsdfGenParams { get; set; }
#if DEBUG
        public bool dbugWriteMsdfTexture { get; set; }

#endif
        static void CreateOuterBorder(VertexStore vxs, double x0, double y0, double x1, double y1, double w)
        {
            //create 'outer border box' of a line (x0,y0)=>(x1,y1)
            PixelFarm.VectorMath.Vector2 vector = new PixelFarm.VectorMath.Vector2(x1 - x0, y1 - y0);

            //for outer border, we need to extend both endpoints with len w
            //this will create overlapped area outside the shape.

            PixelFarm.VectorMath.Vector2 ext_vec = vector.NewLength(w);
            x0 = x0 - ext_vec.x;
            y0 = y0 - ext_vec.y;
            x1 = x1 + ext_vec.x;
            y1 = y1 + ext_vec.y;

            //rotate 90 degree to create a height vector that point to 'outside' of the 'rectbox' shape.
            //the box height= w
            PixelFarm.VectorMath.Vector2 h_vec = vector.RotateInDegree(90).NewLength(w);
            vxs.AddMoveTo(x0, y0);
            vxs.AddLineTo(x0 + h_vec.x, y0 + h_vec.y);
            vxs.AddLineTo(x1 + h_vec.x, y1 + h_vec.y);
            vxs.AddLineTo(x1, y1);
            vxs.AddCloseFigure();
        }
        static void CreateInnerBorder(VertexStore vxs, double x0, double y0, double x1, double y1, double w)
        {
            //create 'inner border box' of a line a line (x0,y0)=>(x1,y1)

            PixelFarm.VectorMath.Vector2 vector = new PixelFarm.VectorMath.Vector2(x1 - x0, y1 - y0);

            //for inner border, we don't extend both endpoint
            //rotate 270 degree to create a height vector that point 'inside' of the 'rectbox' shape.
            //the box height= w
            PixelFarm.VectorMath.Vector2 vdiff = vector.RotateInDegree(270).NewLength(w);
            vxs.AddMoveTo(x0, y0);
            vxs.AddLineTo(x1, y1);
            vxs.AddLineTo(x1 + vdiff.x, y1 + vdiff.y);
            vxs.AddLineTo(x0 + vdiff.x, y0 + vdiff.y);
            vxs.AddCloseFigure();
        }

        const int INNER_BORDER_W = 1;
        const int OUTER_BORDER_W = 1;
        const int CURVE_STROKE_EACHSIDE = 1;

        double _dx;
        double _dy;

        /// <summary>
        /// fill inner and outer border from corner0 to corner1
        /// </summary>
        /// <param name="painter"></param>
        /// <param name="c0"></param>
        /// <param name="c1"></param>
        void FillBorders(AggPainter painter, ContourCorner c0, ContourCorner c1)
        {

            //counter-clockwise
            if (!c0.MiddlePoint_IsTouchPoint) { return; }
            //-------------------------------------------------------
            if (c0.RightPoint_IsTouchPoint)
            {
                using (VxsTemp.Borrow(out var v9))
                {
                    _msdfEdgePxBlender.FillMode = MsdfEdgePixelBlender.BlenderFillMode.InnerBorder;
                    CreateInnerBorder(v9,
                     c0.MiddlePoint.X, c0.MiddlePoint.Y,
                     c1.MiddlePoint.X, c1.MiddlePoint.Y, INNER_BORDER_W);
                    painter.Fill(v9, c0.InnerColor);

                    //-------------
                    v9.Clear(); //reuse
                    _msdfEdgePxBlender.FillMode = MsdfEdgePixelBlender.BlenderFillMode.OuterBorder;
                    CreateOuterBorder(v9,
                        c0.MiddlePoint.X, c0.MiddlePoint.Y,
                        c1.MiddlePoint.X, c1.MiddlePoint.Y, OUTER_BORDER_W);
                    painter.Fill(v9, c0.OuterColor);
                }
            }
            else
            {
                painter.CurrentBxtBlendOp = null;//**

                //right side may be Curve2 or Curve3
                EdgeSegment ownerSeg = c1.CenterSegment;
                switch (ownerSeg.SegmentKind)
                {
                    default: throw new NotSupportedException();
                    case EdgeSegmentKind.CubicSegment:
                        {
                            //approximate 
                            CubicSegment cs = (CubicSegment)ownerSeg;


                            using (VxsTemp.Borrow(out var v1))
                            using (VectorToolBox.Borrow(out ShapeBuilder s))
                            using (VectorToolBox.Borrow(out Stroke strk))
                            {
                                s.InitVxs()
                                 .MoveTo(cs.P0.x + _dx, cs.P0.y + _dy) //...
                                 .Curve4To(cs.P1.x + _dx, cs.P1.y + _dy,
                                           cs.P2.x + _dx, cs.P2.y + _dy,
                                           cs.P3.x + _dx, cs.P3.y + _dy)
                                 .NoMore()
                                 .Flatten();


                                //-----------------------
                                //fill outside part of the curve
                                strk.Width = CURVE_STROKE_EACHSIDE * 2;
                                strk.StrokeSideForOpenShape = StrokeSideForOpenShape.Outside;
                                strk.MakeVxs(s.CurrentSharedVxs, v1);
                                painter.Fill(v1, c0.OuterColor);
                                //-----------------------
                                //fill inside part of the curve
                                v1.Clear();
                                strk.StrokeSideForOpenShape = StrokeSideForOpenShape.Inside;
                                strk.MakeVxs(s.CurrentSharedVxs, v1);
                                painter.Fill(v1, c0.InnerColor);
                                //-----------------------
                                 
                            }
                        }
                        break;
                    case EdgeSegmentKind.QuadraticSegment:
                        {
                            QuadraticSegment qs = (QuadraticSegment)ownerSeg;
                            using (VectorToolBox.Borrow(out ShapeBuilder s))
                            using (VxsTemp.Borrow(out var v1))
                            using (VectorToolBox.Borrow(out Stroke strk))
                            {
                                s.InitVxs()
                                 .MoveTo(qs.P0.x + _dx, qs.P0.y + _dy)//...
                                 .Curve3To(qs.P1.x + _dx, qs.P1.y + _dy,
                                           qs.P2.x + _dx, qs.P2.y + _dy)
                                 .NoMore()
                                 .Flatten();

                                //-----------------------
                                //fill outside part of the curve
                                strk.Width = CURVE_STROKE_EACHSIDE * 2;
                                strk.StrokeSideForOpenShape = StrokeSideForOpenShape.Outside;
                                strk.MakeVxs(s.CurrentSharedVxs, v1);
                                painter.Fill(v1, c0.OuterColor);
                                //-----------------------
                                //fill inside part of the curve
                                v1.Clear();
                                strk.StrokeSideForOpenShape = StrokeSideForOpenShape.Inside;
                                strk.MakeVxs(s.CurrentSharedVxs, v1);
                                painter.Fill(v1, c0.InnerColor);
                                //----------------------- 
                            }
                        }
                        break;
                }
            }
        }


        const double MAX = 1e240;
        static void PreviewSizeAndLocation(Shape shape, MsdfGenParams genParams,
           out int imgW, out int imgH,
           out Vector2 translate1)
        {
            double left = MAX;
            double bottom = MAX;
            double right = -MAX;
            double top = -MAX;

            shape.findBounds(ref left, ref bottom, ref right, ref top);
            int w = (int)Math.Ceiling((right - left));
            int h = (int)Math.Ceiling((top - bottom));

            if (w < genParams.minImgWidth)
            {
                w = genParams.minImgWidth;
            }
            if (h < genParams.minImgHeight)
            {
                h = genParams.minImgHeight;
            }

            //temp, for debug with glyph 'I', tahoma font
            //double edgeThreshold = 1.00000001;//default, if edgeThreshold < 0 then  set  edgeThreshold=1 
            //Msdfgen.Vector2 scale = new Msdfgen.Vector2(0.98714652956298199, 0.98714652956298199);
            //double pxRange = 4;
            //translate = new Msdfgen.Vector2(12.552083333333332, 4.0520833333333330);
            //double range = pxRange / Math.Min(scale.x, scale.y);


            int borderW = (int)((float)w / 5f) + 3;

            //org
            //var translate = new ExtMsdfgen.Vector2(left < 0 ? -left + borderW : borderW, bottom < 0 ? -bottom + borderW : borderW);
            //test
            var translate = new Vector2(-left + borderW, -bottom + borderW);

            w += borderW * 2; //borders,left- right
            h += borderW * 2; //borders, top- bottom

            imgW = w;
            imgH = h;
            translate1 = translate;
        }

        public SpriteTextureMapData<MemBitmap> GenerateMsdfTexture(VertexStore v1)
        {

            Shape shape = CreateShape(v1, out EdgeBmpLut edgeBmpLut);

            if (MsdfGenParams == null)
            {
                MsdfGenParams = new MsdfGenParams();//use default
            }

            //---preview v1 bounds-----------
            PreviewSizeAndLocation(
               shape,
               MsdfGenParams,
               out int imgW, out int imgH,
               out Vector2 translateVec);

            _dx = translateVec.x;
            _dy = translateVec.y;
            //------------------------------------
            List<ContourCorner> corners = edgeBmpLut.Corners;
            TranslateCorners(corners, _dx, _dy);

            //[1] create lookup table (lut) bitmap that contains area/corner/shape information
            //each pixel inside it contains data that map to area/corner/shape

            //
            using (MemBitmap bmpLut = new MemBitmap(imgW, imgH))
            using (AggPainterPool.Borrow(bmpLut, out AggPainter painter))
            using (VectorToolBox.Borrow(out ShapeBuilder sh))
            {


                _msdfEdgePxBlender.ClearOverlapList();//reset
                painter.RenderSurface.SetCustomPixelBlender(_msdfEdgePxBlender);

                //1. clear all bg to black 
                painter.Clear(PixelFarm.Drawing.Color.Black);

                sh.InitVxs(v1) //...
                    .TranslateToNewVxs(_dx, _dy)
                    .Flatten();


                //---------
                //2. force fill the shape (this include hole(s) inside shape to)
                //( we set threshold to 50 and do force fill)
                painter.RenderSurface.SetGamma(_prebuiltThresholdGamma_50);
                _msdfEdgePxBlender.FillMode = MsdfEdgePixelBlender.BlenderFillMode.Force;
                painter.Fill(sh.CurrentSharedVxs, EdgeBmpLut.EncodeToColor(0, AreaKind.AreaInsideCoverage50));

                painter.RenderSurface.SetGamma(_prebuiltThresholdGamma_50);//restore
#if DEBUG
                //debug for output
                //painter.Fill(v7, Color.Red);
                //bmpLut.SaveImage("dbug_step0.png");
                //int curr_step = 1;
#endif
                //---------

                int cornerCount = corners.Count;
                List<int> cornerOfNextContours = edgeBmpLut.CornerOfNextContours;
                int startAt = 0;
                int n = 1;
                int corner_index = 1;

                for (int cnt_index = 0; cnt_index < cornerOfNextContours.Count; ++cnt_index)
                {
                    //contour scope
                    int next_corner_startAt = cornerOfNextContours[cnt_index];

                    //-----------
                    //AA-borders of the contour
                    painter.RenderSurface.SetGamma(_prebuiltThresholdGamma_OverlappedBorder); //this creates overlapped area 
                    for (; n < next_corner_startAt; ++n)
                    {
                        //0-> 1
                        //1->2 ... n
                        FillBorders(painter, corners[n - 1], corners[n]);

#if DEBUG
                        //bmpLut.SaveImage("dbug_step" + curr_step + ".png");
                        //curr_step++;
#endif
                    }
                    {
                        //the last one 
                        //close contour, n-> 0
                        FillBorders(painter, corners[next_corner_startAt - 1], corners[startAt]);
#if DEBUG
                        //bmpLut.SaveImage("dbug_step" + curr_step + ".png");
                        //curr_step++;
#endif
                    }

                    startAt = next_corner_startAt;
                    n++;
                    corner_index++;
                }
#if DEBUG
                //bmpLut.SaveImage("dbug_step2.png");
#endif


                //painter.RenderSurface.SetGamma(_prebuiltThresholdGamma_100);
                //_msdfEdgePxBlender.FillMode = MsdfEdgePixelBlender.BlenderFillMode.InnerAreaX;
                //painter.Fill(sh.CurrentSharedVxs, EdgeBmpLut.EncodeToColor(0, AreaKind.AreaInsideCoverage100));



                painter.RenderSurface.SetCustomPixelBlender(null);
                painter.RenderSurface.SetGamma(null);

                //
                List<CornerList> overlappedList = MakeUniqueList(_msdfEdgePxBlender._overlapList);
                edgeBmpLut.SetOverlappedList(overlappedList);

#if DEBUG

                if (dbugWriteMsdfTexture)
                {
                    //save for debug 
                    //we save to msdf_shape_lut2.png
                    //and check it from external program
                    //but we generate msdf bitmap from msdf_shape_lut.png 
                    bmpLut.SaveImage(dbug_msdf_shape_lutName);
                    var bmp5 = MemBitmap.LoadBitmap(dbug_msdf_shape_lutName);
                    int[] lutBuffer5 = bmp5.CopyImgBuffer(bmpLut.Width, bmpLut.Height);
                    if (bmpLut.Width == 338 && bmpLut.Height == 477)
                    {
                        dbugBreak = true;
                    }
                    edgeBmpLut.SetBmpBuffer(bmpLut.Width, bmpLut.Height, lutBuffer5);
                    //generate actual sprite
                    SpriteTextureMapData<MemBitmap> spriteTextureMapData = CreateMsdfImage(shape, MsdfGenParams, edgeBmpLut);
                    //save msdf bitmap to file              
                    spriteTextureMapData.Source.SaveImage(dbug_msdf_output);
                    return spriteTextureMapData;
                }

#endif

                //[B] after we have a lookup table
                int[] lutBuffer = bmpLut.CopyImgBuffer(bmpLut.Width, bmpLut.Height);
                edgeBmpLut.SetBmpBuffer(bmpLut.Width, bmpLut.Height, lutBuffer);
                return CreateMsdfImage(shape, MsdfGenParams, edgeBmpLut);
            }
        }

#if DEBUG
        public string dbug_msdf_shape_lutName = "msdf_shape_lut2.png";
        public string dbug_msdf_output = "msdf_shape.png";
        public static bool dbugBreak;
#endif




        Dictionary<int, bool> _uniqueCorners = new Dictionary<int, bool>();

        List<CornerList> MakeUniqueList(List<CornerList> primaryOverlappedList)
        {

            List<CornerList> list = new List<CornerList>();
            //copy data to bmpLut
            int j = primaryOverlappedList.Count;
            for (int k = 0; k < j; ++k)
            {
                _uniqueCorners.Clear();
                CornerList overlapped = primaryOverlappedList[k];
                //each group -> make unique 
                CornerList newlist = new CornerList();
                int m = overlapped.Count;
                for (int n = 0; n < m; ++n)
                {
                    ushort corner = overlapped[n];
                    if (!_uniqueCorners.ContainsKey(corner))
                    {
                        _uniqueCorners.Add(corner, true);
                        newlist.Append(corner);
                    }
                }
                _uniqueCorners.Clear();
                // 
                list.Add(newlist);
            }
            return list;

        }
        static void TranslateCorners(List<ContourCorner> corners, double dx, double dy)
        {
            //test 2 if each edge has unique color
            int j = corners.Count;
            for (int i = 0; i < j; ++i)
            {
                corners[i].Offset(dx, dy);
            }
        }
        static void FlattenPoints(EdgeSegment segment, List<Vec2Info> points)
        {
            switch (segment.SegmentKind)
            {
                default: throw new NotSupportedException();
                case EdgeSegmentKind.LineSegment:
                    {
                        LinearSegment seg = (LinearSegment)segment;
                        points.Add(new Vec2Info(segment, Vec2PointKind.Touch1, seg.P0));
                    }
                    break;
                case EdgeSegmentKind.QuadraticSegment:
                    {
                        QuadraticSegment seg = (QuadraticSegment)segment;
                        points.Add(new Vec2Info(segment, Vec2PointKind.Touch1, seg.P0));
                        points.Add(new Vec2Info(segment, Vec2PointKind.C2, seg.P1));
                    }
                    break;
                case EdgeSegmentKind.CubicSegment:
                    {
                        CubicSegment seg = (CubicSegment)segment;
                        points.Add(new Vec2Info(segment, Vec2PointKind.Touch1, seg.P0));
                        points.Add(new Vec2Info(segment, Vec2PointKind.C3, seg.P1));
                        points.Add(new Vec2Info(segment, Vec2PointKind.C3, seg.P2));
                    }
                    break;
            }

        }
        static void CreateCorners(List<Vec2Info> points, List<ContourCorner> corners)
        {

            int j = points.Count;
            int beginAt = corners.Count;
            if (beginAt >= ushort.MaxValue)
            {
                throw new NotSupportedException();
            }

            for (int i = 1; i < j - 1; ++i)
            {
                ContourCorner corner = new ContourCorner(corners.Count, points[i - 1], points[i], points[i + 1]);
                corners.Add(corner);

#if DEBUG
                corner.dbugLeftIndex = beginAt + i - 1;
                corner.dbugMiddleIndex = beginAt + i;
                corner.dbugRightIndex = beginAt + i + 1;
#endif

            }

            {

                ContourCorner corner = new ContourCorner(corners.Count, points[j - 2], points[j - 1], points[0]);
                corners.Add(corner);
#if DEBUG
                corner.dbugLeftIndex = beginAt + j - 2;
                corner.dbugMiddleIndex = beginAt + j - 1;
                corner.dbugRightIndex = beginAt + 0;
#endif

            }

            {

                ContourCorner corner = new ContourCorner(corners.Count, points[j - 1], points[0], points[1]);
                corners.Add(corner);
#if DEBUG
                corner.dbugLeftIndex = beginAt + j - 1;
                corner.dbugMiddleIndex = beginAt + 0;
                corner.dbugRightIndex = beginAt + 1;
#endif

            }

        }
        static void CreateCorners(Contour contour, List<ContourCorner> output)
        {
            //create corner-arm relation for a given contour
            List<EdgeSegment> edges = contour.edges;
            int j = edges.Count;
            List<Vec2Info> flattenPoints = new List<Vec2Info>();
            for (int i = 0; i < j; ++i)
            {
                FlattenPoints(edges[i], flattenPoints);
            }
            CreateCorners(flattenPoints, output);
        }

        static Shape CreateShape(VertexStore vxs, out EdgeBmpLut bmpLut)
        {
            List<EdgeSegment> flattenEdges = new List<EdgeSegment>();
            Shape shape = new Shape(); //start with blank shape

            int i = 0;
            double x, y;
            VertexCmd cmd;
            Contour cnt = null;
            double latestMoveToX = 0;
            double latestMoveToY = 0;
            double latestX = 0;
            double latestY = 0;

            List<ContourCorner> corners = new List<ContourCorner>();
            List<int> edgeOfNextContours = new List<int>();
            List<int> cornerOfNextContours = new List<int>();

            while ((cmd = vxs.GetVertex(i, out x, out y)) != VertexCmd.NoMore)
            {
                switch (cmd)
                {
                    case VertexCmd.Close:
                        {
                            //close current cnt

                            if ((latestMoveToX != latestX) ||
                                (latestMoveToY != latestY))
                            {
                                //add line to close the shape
                                if (cnt != null)
                                {
                                    flattenEdges.Add(cnt.AddLine(latestX, latestY, latestMoveToX, latestMoveToY));
                                }
                            }
                            if (cnt != null)
                            {
                                //***                                
                                CreateCorners(cnt, corners);
                                edgeOfNextContours.Add(flattenEdges.Count);
                                cornerOfNextContours.Add(corners.Count);
                                shape.contours.Add(cnt);
                                //***
                                cnt = null;
                            }
                        }
                        break;
                    case VertexCmd.C3:
                        {

                            //C3 curve (Quadratic)                            
                            if (cnt == null)
                            {
                                cnt = new Contour();
                                cnt.winding();
                            }
                            VertexCmd cmd1 = vxs.GetVertex(i + 1, out double x1, out double y1);
                            i++;
                            if (cmd1 != VertexCmd.LineTo)
                            {
                                throw new NotSupportedException();
                            }

                            //in this version, 
                            //we convert Quadratic to Cubic (https://stackoverflow.com/questions/9485788/convert-quadratic-curve-to-cubic-curve)

                            //Control1X = StartX + ((2f/3) * (ControlX - StartX))
                            //Control2X = EndX + ((2f/3) * (ControlX - EndX))


                            //flattenEdges.Add(cnt.AddCubicSegment(
                            //    latestX, latestY,
                            //    ((2f / 3) * (x - latestX)) + latestX, ((2f / 3) * (y - latestY)) + latestY,
                            //    ((2f / 3) * (x - x1)) + x1, ((2f / 3) * (y - y1)) + y1,
                            //    x1, y1));

                            flattenEdges.Add(cnt.AddQuadraticSegment(latestX, latestY, x, y, x1, y1));

                            latestX = x1;
                            latestY = y1;

                        }
                        break;
                    case VertexCmd.C4:
                        {
                            //C4 curve (Cubic)
                            if (cnt == null)
                            {
                                cnt = new Contour();
                            }

                            VertexCmd cmd1 = vxs.GetVertex(i + 1, out double x2, out double y2);
                            VertexCmd cmd2 = vxs.GetVertex(i + 2, out double x3, out double y3);
                            i += 2;

                            if (cmd1 != VertexCmd.C4 || cmd2 != VertexCmd.LineTo)
                            {
                                throw new NotSupportedException();
                            }

                            flattenEdges.Add(cnt.AddCubicSegment(latestX, latestY, x, y, x2, y2, x3, y3));

                            latestX = x3;
                            latestY = y3;

                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            if (cnt == null)
                            {
                                cnt = new Contour();
                            }
                            LinearSegment lineseg = cnt.AddLine(latestX, latestY, x, y);
                            flattenEdges.Add(lineseg);

                            latestX = x;
                            latestY = y;
                        }
                        break;
                    case VertexCmd.MoveTo:
                        {
                            latestX = latestMoveToX = x;
                            latestY = latestMoveToY = y;
                            if (cnt != null)
                            {
                                shape.contours.Add(cnt);
                                cnt = null;
                            }
                        }
                        break;
                }
                i++;
            }

            if (cnt != null)
            {
                shape.contours.Add(cnt);
                CreateCorners(cnt, corners);
                edgeOfNextContours.Add(flattenEdges.Count);
                cornerOfNextContours.Add(corners.Count);
                cnt = null;
            }

            GroupingOverlapContours(shape);

            //from a given shape we create a corner-arm for each corner  
            bmpLut = new EdgeBmpLut(corners, flattenEdges, edgeOfNextContours, cornerOfNextContours);

            return shape;
        }
        static void GroupingOverlapContours(Shape shape)
        {

            //if (shape.contours.Count > 1)
            //{
            //    //group contour into intersect group
            //    List<Contour> contours = shape.contours;
            //    int n = contours.Count;

            //    RectD[] boundsList = new RectD[n];
            //    for (int i = 0; i < n; ++i)
            //    {
            //        Contour c = contours[i];
            //        boundsList[i] = c.GetRectBounds();
            //    }

            //    //collapse all connected rgn

            //    List<ConnectedContours> connectedCnts = new List<ConnectedContours>();

            //    for (int i = 1; i < n; ++i)
            //    {
            //        Contour c0 = contours[i - 1];
            //        Contour c1 = contours[i];
            //        RectD b0 = c0.GetRectBounds();
            //        RectD b1 = c1.GetRectBounds();
            //        if (b0.IntersectWithRectangle(b1))
            //        {
            //            //if yes then we create a map
            //            ConnectedContours connContours = new ConnectedContours();
            //            connContours._members.Add(c0);
            //            connContours._members.Add(c1);
            //            connectedCnts.Add(connContours);
            //            i++;
            //        }
            //    }

            //}
        }

        public static SpriteTextureMapData<PixelFarm.CpuBlit.MemBitmap> CreateMsdfImage(ExtMsdfGen.Shape shape, MsdfGenParams genParams, EdgeBmpLut lutBuffer = null)
        {
            double left = MAX;
            double bottom = MAX;
            double right = -MAX;
            double top = -MAX;

            shape.findBounds(ref left, ref bottom, ref right, ref top);
            int w = (int)Math.Ceiling((right - left));
            int h = (int)Math.Ceiling((top - bottom));

            if (w < genParams.minImgWidth)
            {
                w = genParams.minImgWidth;
            }
            if (h < genParams.minImgHeight)
            {
                h = genParams.minImgHeight;
            }


            //temp, for debug with glyph 'I', tahoma font
            //double edgeThreshold = 1.00000001;//default, if edgeThreshold < 0 then  set  edgeThreshold=1 
            //Msdfgen.Vector2 scale = new Msdfgen.Vector2(0.98714652956298199, 0.98714652956298199);
            //double pxRange = 4;
            //translate = new Msdfgen.Vector2(12.552083333333332, 4.0520833333333330);
            //double range = pxRange / Math.Min(scale.x, scale.y);


            int borderW = (int)((float)w / 5f) + 3;

            //org
            //var translate = new ExtMsdfgen.Vector2(left < 0 ? -left + borderW : borderW, bottom < 0 ? -bottom + borderW : borderW);
            //test
            var translate = new Vector2(-left + borderW, -bottom + borderW);

            w += borderW * 2; //borders,left- right
            h += borderW * 2; //borders, top- bottom

            double edgeThreshold = genParams.edgeThreshold;
            if (edgeThreshold < 0)
            {
                edgeThreshold = 1.00000001; //use default if  edgeThreshold <0
            }

            var scale = new Vector2(genParams.scaleX, genParams.scaleY); //scale               
            double range = genParams.pxRange / Math.Min(scale.x, scale.y);
            //---------
            FloatRGBBmp frgbBmp = new FloatRGBBmp(w, h);
            EdgeColoring.edgeColoringSimple(shape, genParams.angleThreshold);

            if (lutBuffer != null)
            {
                MsdfGenerator.generateMSDF3(frgbBmp,
                  shape,
                  range,
                  scale,
                  translate,//translate to positive quadrant
                  edgeThreshold,
                  lutBuffer);
            }
            else
            {
                MsdfGenerator.generateMSDF(frgbBmp,
                  shape,
                  range,
                  scale,
                  translate,//translate to positive quadrant
                  edgeThreshold);
            }

            var spriteData = new SpriteTextureMapData<PixelFarm.CpuBlit.MemBitmap>(0, 0, w, h);
            spriteData.Source = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(w, h, MsdfGenerator.ConvertToIntBmp(frgbBmp));
            spriteData.TextureXOffset = (float)translate.x;
            spriteData.TextureYOffset = (float)translate.y;
            return spriteData;
        }



    }
}