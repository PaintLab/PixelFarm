//MIT, 2019-present, WinterDev
//based on  ...
//(MIT, 2016, Viktor Chlumsky, Multi-channel signed distance field generator, from https://github.com/Chlumsky/msdfge)
//-----------------------------------  
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using System;
using System.Collections.Generic;

namespace ExtMsdfGen
{
    class CustomBlendOp1 : BitmapBufferEx.CustomBlendOp
    {
        const int WHITE = (255 << 24) | (255 << 16) | (255 << 8) | 255;
        const int BLACK = (255 << 24);
        const int GREEN = (255 << 24) | (255 << 8);
        const int RED = (255 << 24) | (255 << 16);

        public override int Blend(int currentExistingColor, int inputColor)
        {
            //this is our custom blending 
            if (currentExistingColor != WHITE && currentExistingColor != BLACK)
            {
                //return RED;
                //WINDOWS: ABGR
                int existing_R = currentExistingColor & 0xFF;
                int existing_G = (currentExistingColor >> 8) & 0xFF;
                int existing_B = (currentExistingColor >> 16) & 0xFF;

                int new_R = inputColor & 0xFF;
                int new_G = (inputColor >> 8) & 0xFF;
                int new_B = (inputColor >> 16) & 0xFF;

                if (new_R == existing_R && new_B == existing_B)
                {
                    return inputColor;
                }

                //***
                //Bitmap extension arrange this to ARGB?
                return RED;
                //return base.Blend(currentExistingColor, inputColor);
            }
            else
            {
#if DEBUG
                int new_R = inputColor & 0xFF;
                int new_G = (inputColor >> 8) & 0xFF;
                int new_B = (inputColor >> 16) & 0xFF;

                if (new_R == 4)
                {

                }
#endif
                return inputColor;
            }
        }
    }

    /// <summary>
    /// msdf texture generator
    /// </summary>
    public class MsdfGen3
    {
        CustomBlendOp1 _customBlendOp = new CustomBlendOp1();
        public MsdfGen3()
        {

        }
        public MsdfGenParams MsdfGenParams { get; set; }
#if DEBUG
        public bool dbugWriteMsdfTexture { get; set; }

#endif


        void Fill(AggPainter painter, PathWriter writer,
            CurveFlattener flattener,
            VertexStore v2, double dx, double dy,
            ContourCorner c0, ContourCorner c1)
        {

            //counter-clockwise
            if (!c0.MiddlePointKindIsTouchPoint) { return; }
            //
            //-------------------------------------------------------
            if (c0.RightPointKindIsTouchPoint)
            {
                //outer
                writer.MoveTo(c0.middlePoint.X, c0.middlePoint.Y);
                writer.LineTo(c0.ExtPoint_LeftOuter.X, c0.ExtPoint_LeftOuter.Y);
                writer.LineTo(c0.ExtPoint_LeftOuterDest.X, c0.ExtPoint_LeftOuterDest.Y);
                writer.LineTo(c1.middlePoint.X, c1.middlePoint.Y);
                writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                writer.CloseFigure();
                // 
                painter.Fill(v2, c0.OuterColor);

                //inner
                v2.Clear();
                writer.MoveTo(c0.ExtPoint_LeftInner.X, c0.ExtPoint_LeftInner.Y);
                writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                writer.LineTo(c1.middlePoint.X, c1.middlePoint.Y);
                writer.LineTo(c1.ExtPoint_RightInner.X, c1.ExtPoint_RightInner.Y);
                writer.LineTo(c0.ExtPoint_LeftInner.X, c0.ExtPoint_LeftInner.Y);
                writer.CloseFigure();
                ////
                painter.Fill(v2, c0.InnerColor);

                //gap
                v2.Clear();
                //large corner that cover gap
                writer.MoveTo(c0.ExtPoint_LeftInner.X, c0.ExtPoint_LeftInner.Y);
                writer.LineTo(c0.ExtPoint_RightOuter.X, c0.ExtPoint_RightOuter.Y);
                writer.LineTo(c0.ExtPoint_LeftOuter.X, c0.ExtPoint_LeftOuter.Y);
                writer.LineTo(c0.ExtPoint_RightInner.X, c0.ExtPoint_RightInner.Y);
                writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                writer.CloseFigure();

                //TODO: predictable overlap area ....
                painter.Fill(v2, PixelFarm.Drawing.Color.Red);

            }
            else
            {
                painter.CurrentBxtBlendOp = null;//**
                                                 //right may be Curve2 or Curve3
                EdgeSegment ownerSeg = c1.CenterSegment;
                switch (ownerSeg.SegmentKind)
                {
                    default: throw new NotSupportedException();
                    case EdgeSegmentKind.CubicSegment:
                        {
                            //approximate 
                            CubicSegment cs = (CubicSegment)ownerSeg;

                            using (VxsTemp.Borrow(out var v3, out var v4, out var v7))
                            using (VectorToolBox.Borrow(out Stroke s))
                            {
                                double rad0 = Math.Atan2(cs.P0.y - cs.P1.y, cs.P0.x - cs.P1.x);
                                v3.AddMoveTo(cs.P0.x + dx + Math.Cos(rad0) * 4, cs.P0.y + dy + Math.Sin(rad0) * 4);
                                v3.AddLineTo(cs.P0.x + dx, cs.P0.y + dy);
                                v3.AddCurve4To(cs.P1.x + dx, cs.P1.y + dy,
                                               cs.P2.x + dx, cs.P2.y + dy,
                                               cs.P3.x + dx, cs.P3.y + dy);

                                double rad1 = Math.Atan2(cs.P3.y - cs.P2.y, cs.P3.x - cs.P2.x);
                                v3.AddLineTo((cs.P3.x + dx) + Math.Cos(rad1) * 4, (cs.P3.y + dy) + Math.Sin(rad1) * 4);
                                v3.AddNoMore();//

                                //
                                flattener.MakeVxs(v3, v4);
                                s.Width = 4;//2 px on each side
                                s.MakeVxs(v4, v7);

                                painter.RenderQuality = RenderQuality.HighQuality;
                                painter.Fill(v7, c0.OuterColor);
                                painter.RenderQuality = RenderQuality.Fast;


                                writer.MoveTo(c0.ExtPoint_LeftInner.X, c0.ExtPoint_LeftInner.Y);
                                writer.LineTo(c0.ExtPoint_RightOuter.X, c0.ExtPoint_RightOuter.Y);
                                v7.GetVertex(0, out double v7x, out double v7y);
                                //writer.LineTo(v7x - 2, v7y - 2);
                                writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                                writer.CloseFigure();
                                painter.Fill(v2, PixelFarm.Drawing.Color.Red);
                            }
                        }
                        break;
                    case EdgeSegmentKind.QuadraticSegment:
                        {
                            QuadraticSegment qs = (QuadraticSegment)ownerSeg;
                            using (VxsTemp.Borrow(out var v3, out var v4, out var v7))
                            using (VectorToolBox.Borrow(out Stroke s))
                            {
                                double rad0 = Math.Atan2(qs.P0.y - qs.P1.y, qs.P0.x - qs.P1.x);
                                v3.AddMoveTo(qs.P0.x + dx + Math.Cos(rad0) * 4, qs.P0.y + dy + Math.Sin(rad0) * 4);
                                v3.AddLineTo(qs.P0.x + dx, qs.P0.y + dy);
                                v3.AddCurve3To(qs.P1.x + dx, qs.P1.y + dy,
                                               qs.P2.x + dx, qs.P2.y + dy);

                                double rad1 = Math.Atan2(qs.P2.y - qs.P1.y, qs.P2.x - qs.P1.x);
                                v3.AddLineTo((qs.P2.x + dx) + Math.Cos(rad1) * 4, (qs.P2.y + dy) + Math.Sin(rad1) * 4);
                                v3.AddNoMore();//
                                               //
                                flattener.MakeVxs(v3, v4);
                                s.Width = 4;//2 px on each side
                                s.MakeVxs(v4, v7);


                                painter.RenderQuality = RenderQuality.HighQuality;
                                painter.Fill(v7, c0.OuterColor);
                                painter.RenderQuality = RenderQuality.Fast;


                                writer.MoveTo(c0.ExtPoint_LeftInner.X, c0.ExtPoint_LeftInner.Y);
                                writer.LineTo(c0.ExtPoint_RightOuter.X, c0.ExtPoint_RightOuter.Y);
                                v7.GetVertex(0, out double v7x, out double v7y);

                                writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                                writer.CloseFigure();
                                painter.Fill(v2, PixelFarm.Drawing.Color.Red);
                            }
                        }
                        break;
                }
            }
        }


        public SpriteTextureMapData<MemBitmap> GenerateMsdfTexture(VertexStore v1)
        {
            //create shape and edge-bmp-lut from a given v1
            Shape shape = CreateShape(v1, out EdgeBmpLut edgeBmpLut);

            if (MsdfGenParams == null)
            {
                MsdfGenParams = new MsdfGenParams();//use default
            }

            //---preview v1 bounds-----------
            MsdfGlyphGen.PreviewSizeAndLocation(
               shape,
               MsdfGenParams,
               out int imgW, out int imgH,
               out Vector2 translateVec);

            //------------------------------------
            List<ContourCorner> corners = edgeBmpLut.Corners;
            TranslateCorners(corners, translateVec.x, translateVec.y);


            using (MemBitmap bmpLut = new MemBitmap(imgW, imgH)) //intermediate data for 
            using (VxsTemp.Borrow(out var v2, out var v5, out var v6))
            using (VectorToolBox.Borrow(out CurveFlattener flattener))
            using (VectorToolBox.Borrow(v2, out PathWriter writer))
            using (AggPainterPool.Borrow(bmpLut, out AggPainter painter))
            {
                painter.RenderQuality = RenderQuality.Fast;
                painter.Clear(PixelFarm.Drawing.Color.Black);

                v1.TranslateToNewVxs(translateVec.x, translateVec.y, v5);
                flattener.MakeVxs(v5, v6);
                painter.Fill(v6, PixelFarm.Drawing.Color.White);

                painter.StrokeColor = PixelFarm.Drawing.Color.Red;
                painter.StrokeWidth = 1;


                int cornerCount = corners.Count;
                List<int> cornerOfNextContours = edgeBmpLut.CornerOfNextContours;
                int n = 1;
                int startAt = 0;
                for (int cc = 0; cc < cornerOfNextContours.Count; ++cc)
                {
                    int nextStartAt = cornerOfNextContours[cc];
                    for (; n <= nextStartAt - 1; ++n)
                    {
                        painter.CurrentBxtBlendOp = _customBlendOp; //**
                        Fill(painter, writer, flattener, v2, translateVec.x, translateVec.y, corners[n - 1], corners[n]);
                        writer.Clear();//**
                    }
                    {
                        //the last one 
                        painter.CurrentBxtBlendOp = _customBlendOp; //**
                        Fill(painter, writer, flattener, v2, translateVec.x, translateVec.y, corners[nextStartAt - 1], corners[startAt]);
                        writer.Clear();//**
                    }

                    startAt = nextStartAt;
                    n++;
                }

#if DEBUG

                if (dbugWriteMsdfTexture)
                {
                    //save for debug 
                    //we save to msdf_shape_lut2.png
                    //and check it from external program
                    //but we generate msdf bitmap from msdf_shape_lut.png 
                    bmpLut.SaveImage("d:\\WImageTest\\msdf_shape_lut2.png");//intern
                    var bmp5 = MemBitmap.LoadBitmap("d:\\WImageTest\\msdf_shape_lut.png");
                    int[] lutBuffer5 = bmp5.CopyImgBuffer(bmpLut.Width, bmpLut.Height);
                    edgeBmpLut.SetBmpBuffer(bmpLut.Width, bmpLut.Height, lutBuffer5);
                    //generate actual sprite
                    SpriteTextureMapData<MemBitmap> spriteTextureMapData = MsdfGlyphGen.CreateMsdfImage(shape, MsdfGenParams, edgeBmpLut);
                    //save msdf bitmap to file              
                    spriteTextureMapData.Source.SaveImage("d:\\WImageTest\\msdf_shape.png");
                    return spriteTextureMapData;
                }

#endif

                int[] lutBuffer = bmpLut.CopyImgBuffer(bmpLut.Width, bmpLut.Height);
                edgeBmpLut.SetBmpBuffer(bmpLut.Width, bmpLut.Height, lutBuffer);
                return MsdfGlyphGen.CreateMsdfImage(shape, MsdfGenParams, edgeBmpLut);
            }
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
            for (int i = 1; i < j - 1; ++i)
            {
                ContourCorner corner = new ContourCorner(points[i - 1], points[i], points[i + 1]);
                corner.CornerNo = corners.Count; //**
                corners.Add(corner);

#if DEBUG
                corner.dbugLeftIndex = beginAt + i - 1;
                corner.dbugMiddleIndex = beginAt + i;
                corner.dbugRightIndex = beginAt + i + 1;
#endif

            }

            {

                ContourCorner corner = new ContourCorner(points[j - 2], points[j - 1], points[0]);
                corner.CornerNo = corners.Count; //**
                corners.Add(corner);
#if DEBUG
                corner.dbugLeftIndex = beginAt + j - 2;
                corner.dbugMiddleIndex = beginAt + j - 1;
                corner.dbugRightIndex = beginAt + 0;
#endif

            }

            {

                ContourCorner corner = new ContourCorner(points[j - 1], points[0], points[1]);
                corner.CornerNo = corners.Count; //**
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

            //from a given shape we create a corner-arm for each corner  
            bmpLut = new EdgeBmpLut(corners, flattenEdges, edgeOfNextContours, cornerOfNextContours);

            return shape;
        }

    }
}