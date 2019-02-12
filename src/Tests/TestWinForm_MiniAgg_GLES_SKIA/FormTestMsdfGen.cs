﻿//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;

using System.IO;
using System.Windows.Forms;

using Typography.OpenFont;
using Typography.Rendering;
using Typography.Contours;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;

namespace Mini
{
    public partial class FormTestMsdfGen : Form
    {
        public FormTestMsdfGen()
        {
            InitializeComponent();
        }


        static void CreateSampleMsdfTextureFont(string fontfile, float sizeInPoint, ushort startGlyphIndex, ushort endGlyphIndex, string outputFile)
        {
            //sample
            var reader = new OpenFontReader();

            using (var fs = new FileStream(fontfile, FileMode.Open))
            {
                //1. read typeface from font file
                Typeface typeface = reader.Read(fs);
                //sample: create sample msdf texture 
                //-------------------------------------------------------------
                var builder = new GlyphPathBuilder(typeface);
                //builder.UseTrueTypeInterpreter = this.chkTrueTypeHint.Checked;
                //builder.UseVerticalHinting = this.chkVerticalHinting.Checked;
                //-------------------------------------------------------------
                var atlasBuilder = new SimpleFontAtlasBuilder();


                for (ushort gindex = startGlyphIndex; gindex <= endGlyphIndex; ++gindex)
                {
                    //build glyph
                    builder.BuildFromGlyphIndex(gindex, sizeInPoint);

                    var glyphContourBuilder = new GlyphContourBuilder();
                    //glyphToContour.Read(builder.GetOutputPoints(), builder.GetOutputContours());
                    var genParams = new MsdfGenParams();
                    builder.ReadShapes(glyphContourBuilder);
                    //genParams.shapeScale = 1f / 64; //we scale later (as original C++ code use 1/64)
                    GlyphImage glyphImg = MsdfGlyphGen.CreateMsdfImage(glyphContourBuilder, genParams);
                    atlasBuilder.AddGlyph(gindex, glyphImg);

                    using (Bitmap bmp = new Bitmap(glyphImg.Width, glyphImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        int[] buffer = glyphImg.GetImageBuffer();

                        var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg.Width, glyphImg.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                        System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                        bmp.UnlockBits(bmpdata);
                        bmp.Save("d:\\WImageTest\\a001_xn2_" + gindex + ".png");
                    }
                }

                GlyphImage glyphImg2 = atlasBuilder.BuildSingleImage();
                using (Bitmap bmp = new Bitmap(glyphImg2.Width, glyphImg2.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg2.Width, glyphImg2.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                    int[] intBuffer = glyphImg2.GetImageBuffer();

                    System.Runtime.InteropServices.Marshal.Copy(intBuffer, 0, bmpdata.Scan0, intBuffer.Length);
                    bmp.UnlockBits(bmpdata);
                    bmp.Save(outputFile);
                }
                atlasBuilder.SaveFontInfo("d:\\WImageTest\\a_info.bin");
                //
                //-----------
                //test read texture info back
                var atlasBuilder2 = new SimpleFontAtlasBuilder();
                var readbackFontAtlas = atlasBuilder2.LoadFontInfo("d:\\WImageTest\\a_info.bin");
            }
        }

        static void CreateSampleMsdfImg(GlyphContourBuilder tx, string outputFile)
        {
            //sample

            MsdfGenParams msdfGenParams = new MsdfGenParams();
            GlyphImage glyphImg = MsdfGlyphGen.CreateMsdfImage(tx, msdfGenParams);
            int w = glyphImg.Width;
            int h = glyphImg.Height;
            using (Bitmap bmp = new Bitmap(glyphImg.Width, glyphImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                int[] imgBuffer = glyphImg.GetImageBuffer();
                System.Runtime.InteropServices.Marshal.Copy(imgBuffer, 0, bmpdata.Scan0, imgBuffer.Length);
                bmp.UnlockBits(bmpdata);
                bmp.Save(outputFile);
            }
        }

         

        static void FlattenPoints(ExtMsdfGen.EdgeSegment segment, List<ExtMsdfGen.Vec2Info> points)
        {
            switch (segment.SegmentKind)
            {
                default: throw new NotSupportedException();
                case ExtMsdfGen.EdgeSegmentKind.LineSegment:
                    {
                        ExtMsdfGen.LinearSegment seg = (ExtMsdfGen.LinearSegment)segment;
                        points.Add(new ExtMsdfGen.Vec2Info(segment) { Kind = ExtMsdfGen.Vec2PointKind.Touch1, x = seg.P0.x, y = seg.P0.y });
                    }
                    break;
                case ExtMsdfGen.EdgeSegmentKind.QuadraticSegment:
                    {
                        ExtMsdfGen.QuadraticSegment seg = (ExtMsdfGen.QuadraticSegment)segment;
                        points.Add(new ExtMsdfGen.Vec2Info(segment) { Kind = ExtMsdfGen.Vec2PointKind.Touch1, x = seg.P0.x, y = seg.P0.y });
                        points.Add(new ExtMsdfGen.Vec2Info(segment) { Kind = ExtMsdfGen.Vec2PointKind.C2, x = seg.P1.x, y = seg.P1.y });
                    }
                    break;
                case ExtMsdfGen.EdgeSegmentKind.CubicSegment:
                    {
                        ExtMsdfGen.CubicSegment seg = (ExtMsdfGen.CubicSegment)segment;
                        points.Add(new ExtMsdfGen.Vec2Info(segment) { Kind = ExtMsdfGen.Vec2PointKind.Touch1, x = seg.P0.x, y = seg.P0.y });
                        points.Add(new ExtMsdfGen.Vec2Info(segment) { Kind = ExtMsdfGen.Vec2PointKind.C3, x = seg.P1.x, y = seg.P1.y });
                        points.Add(new ExtMsdfGen.Vec2Info(segment) { Kind = ExtMsdfGen.Vec2PointKind.C3, x = seg.P2.x, y = seg.P2.y });
                    }
                    break;
            }

        }
        static void CreateCorners(List<ExtMsdfGen.Vec2Info> points, List<ExtMsdfGen.ContourCorner> cornerAndArms)
        {

            int j = points.Count;
            int beginAt = cornerAndArms.Count;
            for (int i = 1; i < j - 1; ++i)
            {
                ExtMsdfGen.ContourCorner corner = new ExtMsdfGen.ContourCorner(points[i - 1], points[i], points[i + 1]);
                corner.CornerNo = cornerAndArms.Count; //**
                cornerAndArms.Add(corner);

#if DEBUG
                corner.dbugLeftIndex = beginAt + i - 1;
                corner.dbugMiddleIndex = beginAt + i;
                corner.dbugRightIndex = beginAt + i + 1;
#endif

            }

            {

                ExtMsdfGen.ContourCorner corner = new ExtMsdfGen.ContourCorner(points[j - 2], points[j - 1], points[0]);
                corner.CornerNo = cornerAndArms.Count; //**
                cornerAndArms.Add(corner);
#if DEBUG
                corner.dbugLeftIndex = beginAt + j - 2;
                corner.dbugMiddleIndex = beginAt + j - 1;
                corner.dbugRightIndex = beginAt + 0;
#endif

            }

            {

                ExtMsdfGen.ContourCorner corner = new ExtMsdfGen.ContourCorner(points[j - 1], points[0], points[1]);
                corner.CornerNo = cornerAndArms.Count; //**
                cornerAndArms.Add(corner);
#if DEBUG
                corner.dbugLeftIndex = beginAt + j - 1;
                corner.dbugMiddleIndex = beginAt + 0;
                corner.dbugRightIndex = beginAt + 1;
#endif


            }

        }
        static void CreateCorner(ExtMsdfGen.Contour contour, List<ExtMsdfGen.ContourCorner> output)
        {   
            //create corner-arm relation for a given contour
            List<ExtMsdfGen.EdgeHolder> edges = contour.edges;
            int j = edges.Count;
            List<ExtMsdfGen.Vec2Info> flattenPoints = new List<ExtMsdfGen.Vec2Info>();
            for (int i = 0; i < j; ++i)
            {
                FlattenPoints(edges[i].edgeSegment, flattenPoints);
            }
            CreateCorners(flattenPoints, output);
        }
        static ExtMsdfGen.Shape CreateShape(VertexStore vxs, out ExtMsdfGen.BmpEdgeLut bmpLut)
        {
            List<ExtMsdfGen.EdgeSegment> flattenEdges = new List<ExtMsdfGen.EdgeSegment>();
            ExtMsdfGen.Shape shape1 = new ExtMsdfGen.Shape();

            int i = 0;
            double x, y;
            VertexCmd cmd;
            ExtMsdfGen.Contour cnt = null;
            double latestMoveToX = 0;
            double latestMoveToY = 0;
            double latestX = 0;
            double latestY = 0;


            List<ExtMsdfGen.ContourCorner> corners = new List<ExtMsdfGen.ContourCorner>();
            List<int> edgeOfNextContours = new List<int>();//
            List<int> cornerOfNextContours = new List<int>();//

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
                                CreateCorner(cnt, corners);
                                edgeOfNextContours.Add(flattenEdges.Count);
                                cornerOfNextContours.Add(corners.Count);
                                shape1.contours.Add(cnt);
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
                                cnt = new ExtMsdfGen.Contour();
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
                                cnt = new ExtMsdfGen.Contour();
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
                                cnt = new ExtMsdfGen.Contour();
                            }
                            ExtMsdfGen.LinearSegment lineseg = cnt.AddLine(latestX, latestY, x, y);
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
                                shape1.contours.Add(cnt);
                                cnt = null;
                            }
                        }
                        break;
                }
                i++;
            }

            if (cnt != null)
            {
                shape1.contours.Add(cnt);
                CreateCorner(cnt, corners);
                edgeOfNextContours.Add(flattenEdges.Count);
                cornerOfNextContours.Add(corners.Count);
                cnt = null;
            }

            //from a given shape we create a corner-arm for each corner  
            bmpLut = new ExtMsdfGen.BmpEdgeLut(corners, flattenEdges, edgeOfNextContours, cornerOfNextContours);

            return shape1;
        }

         
        void TranslateArms(List<ExtMsdfGen.ContourCorner> corners, double dx, double dy)
        {
            //test 2 if each edge has unique color
            int j = corners.Count;
            for (int i = 0; i < j; ++i)
            {
                corners[i].Offset(dx, dy);
            }
        }



        void DrawTessTriangles(Poly2Tri.Polygon polygon, AggPainter painter)
        {
            return;
            foreach (var triangle in polygon.Triangles)
            {
                Poly2Tri.TriangulationPoint p0 = triangle.P0;
                Poly2Tri.TriangulationPoint p1 = triangle.P1;
                Poly2Tri.TriangulationPoint p2 = triangle.P2;


                ////we do not store triangulation points (p0,p1,02)
                ////an EdgeLine is created after we create GlyphTriangles.

                ////triangulate point p0->p1->p2 is CCW ***             
                //e0 = NewEdgeLine(p0, p1, tri.EdgeIsConstrained(tri.FindEdgeIndex(p0, p1)));
                //e1 = NewEdgeLine(p1, p2, tri.EdgeIsConstrained(tri.FindEdgeIndex(p1, p2)));
                //e2 = NewEdgeLine(p2, p0, tri.EdgeIsConstrained(tri.FindEdgeIndex(p2, p0)));

                painter.RenderQuality = RenderQuality.HighQuality;
                painter.StrokeColor = PixelFarm.Drawing.Color.Green;
                painter.StrokeWidth = 1.5f;
                painter.DrawLine(p0.X, p0.Y, p1.X, p1.Y);
                painter.DrawLine(p1.X, p1.Y, p2.X, p2.Y);
                painter.DrawLine(p2.X, p2.Y, p0.X, p0.Y);
            }
        }

         
        void GetExampleVxs(VertexStore outputVxs)
        {
            //counter-clockwise 
            //a triangle
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddCloseFigure();

            //a quad
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddLineTo(50, 10);
            //outputVxs.AddCloseFigure();



            //curve4
            //outputVxs.AddMoveTo(5, 5);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddCurve4To(70, 20, 50, 10, 10, 5);
            //outputVxs.AddCloseFigure();

            //curve3
            outputVxs.AddMoveTo(5, 5);
            outputVxs.AddLineTo(50, 60);
            outputVxs.AddCurve3To(70, 20, 10, 5);
            outputVxs.AddCloseFigure();


            //a quad with hole
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddLineTo(50, 10);
            //outputVxs.AddCloseFigure();

            //outputVxs.AddMoveTo(30, 30);
            //outputVxs.AddLineTo(40, 30);
            //outputVxs.AddLineTo(40, 35);
            //outputVxs.AddLineTo(30, 35);
            //outputVxs.AddCloseFigure();



        }


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
                    return inputColor;
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //test fake msdf (this is not real msdf gen)
            //--------------------  
            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(v1, out PathWriter w))
            {
                //--------
                GetExampleVxs(v1);
                //--------

                ExtMsdfGen.Shape shape1 = CreateShape(v1, out ExtMsdfGen.BmpEdgeLut bmpLut7);
                ExtMsdfGen.MsdfGenParams msdfGenParams = new ExtMsdfGen.MsdfGenParams();
                ExtMsdfGen.MsdfGlyphGen.PreviewSizeAndLocation(
                   shape1,
                   msdfGenParams,
                   out int imgW, out int imgH,
                   out ExtMsdfGen.Vector2 translateVec);

                //---------
                List<ExtMsdfGen.ContourCorner> corner = bmpLut7.Corners;
                TranslateArms(corner, translateVec.x, translateVec.y);
                //---------


                //Poly2Tri.Polygon polygon1 = CreatePolygon(points, translateVec.x, translateVec.y);
                //Poly2Tri.P2T.Triangulate(polygon1);
                //---------

                using (MemBitmap bmpLut = new MemBitmap(imgW, imgH))
                using (VxsTemp.Borrow(out var v5, out var v6))
                using (VectorToolBox.Borrow(out CurveFlattener flattener))
                using (AggPainterPool.Borrow(bmpLut, out AggPainter painter))
                {
                    painter.RenderQuality = RenderQuality.Fast;
                    painter.Clear(PixelFarm.Drawing.Color.Black);

                    v1.TranslateToNewVxs(translateVec.x, translateVec.y, v5);
                    flattener.MakeVxs(v5, v6);
                    painter.Fill(v6, PixelFarm.Drawing.Color.White);

                    painter.StrokeColor = PixelFarm.Drawing.Color.Red;
                    painter.StrokeWidth = 1;

                    CustomBlendOp1 customBlendOp1 = new CustomBlendOp1();

                    int cornerArmCount = corner.Count;
                    List<int> cornerOfNextContours = bmpLut7.CornerOfNextContours;
                    int n = 1;
                    int startAt = 0;
                    for (int cc = 0; cc < cornerOfNextContours.Count; ++cc)
                    {
                        int nextStartAt = cornerOfNextContours[cc];
                        for (; n <= nextStartAt - 1; ++n)
                        {

                            ExtMsdfGen.ContourCorner c0 = corner[n - 1];
                            ExtMsdfGen.ContourCorner c1 = corner[n];

                            using (VxsTemp.Borrow(out var v2))
                            using (VectorToolBox.Borrow(v2, out PathWriter writer))
                            {
                                painter.CurrentBxtBlendOp = customBlendOp1; //**

                                //counter-clockwise
                                if (c0.MiddlePointKindIsTouchPoint)
                                {
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
                                        painter.Fill(v2, PixelFarm.Drawing.Color.Red);

                                    }
                                    else
                                    {
                                        painter.CurrentBxtBlendOp = null;//**
                                                                         //right may be Curve2 or Curve3
                                        ExtMsdfGen.EdgeSegment ownerSeg = c1.CenterSegment;
                                        switch (ownerSeg.SegmentKind)
                                        {
                                            default: throw new NotSupportedException();
                                            case ExtMsdfGen.EdgeSegmentKind.CubicSegment:
                                                {
                                                    //approximate 
                                                    ExtMsdfGen.CubicSegment cs = (ExtMsdfGen.CubicSegment)ownerSeg;

                                                    double dx = translateVec.x;
                                                    double dy = translateVec.y;

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
                                                        s.Width = 4;
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
                                            case ExtMsdfGen.EdgeSegmentKind.QuadraticSegment:
                                                {
                                                    ExtMsdfGen.QuadraticSegment qs = (ExtMsdfGen.QuadraticSegment)ownerSeg;
                                                    double dx = translateVec.x;
                                                    double dy = translateVec.y;

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
                                                        s.Width = 4;
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
                                        }
                                    }
                                }
                            }
                        }

                        {
                            //the last one
                            ExtMsdfGen.ContourCorner c0 = corner[nextStartAt - 1];
                            ExtMsdfGen.ContourCorner c1 = corner[startAt];

                            using (VxsTemp.Borrow(out var v2))
                            using (VectorToolBox.Borrow(v2, out PathWriter writer))
                            {
                                painter.CurrentBxtBlendOp = customBlendOp1; //**
                                                                            //counter-clockwise

                                //counter-clockwise
                                if (c0.MiddlePointKindIsTouchPoint)
                                {
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
                                        painter.CurrentBxtBlendOp = null;//**

                                        //large corner that cover gap
                                        writer.MoveTo(c0.ExtPoint_LeftInner.X, c0.ExtPoint_LeftInner.Y);
                                        writer.LineTo(c0.ExtPoint_RightOuter.X, c0.ExtPoint_RightOuter.Y);
                                        writer.LineTo(c0.ExtPoint_LeftOuter.X, c0.ExtPoint_LeftOuter.Y);
                                        writer.LineTo(c0.ExtPoint_RightInner.X, c0.ExtPoint_RightInner.Y);
                                        writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                                        writer.CloseFigure();
                                        painter.Fill(v2, PixelFarm.Drawing.Color.Red);

                                    }
                                    else
                                    {
                                        painter.CurrentBxtBlendOp = null;//**
                                                                         //right may be Curve2 or Curve3
                                        ExtMsdfGen.EdgeSegment ownerSeg = c1.CenterSegment;
                                        switch (ownerSeg.SegmentKind)
                                        {
                                            default: throw new NotSupportedException();
                                            case ExtMsdfGen.EdgeSegmentKind.CubicSegment:
                                                {
                                                    //approximate 
                                                    ExtMsdfGen.CubicSegment cs = (ExtMsdfGen.CubicSegment)ownerSeg;

                                                    double dx = translateVec.x;
                                                    double dy = translateVec.y;

                                                    using (VxsTemp.Borrow(out var v3, out var v4, out var v7))
                                                    using (VectorToolBox.Borrow(out Stroke s))
                                                    {
                                                        v3.AddMoveTo(cs.P0.x + dx, cs.P0.y + dy);
                                                        v3.AddCurve4To(cs.P1.x + dx, cs.P1.y + dy,
                                                                cs.P2.x + dx, cs.P2.y + dy,
                                                                cs.P3.x + dx, cs.P3.y + dy);

                                                        v3.AddNoMore();//

                                                        flattener.MakeVxs(v3, v4);
                                                        s.Width = 4;
                                                        s.MakeVxs(v4, v7);

                                                        painter.RenderQuality = RenderQuality.HighQuality;
                                                        painter.Fill(v7, PixelFarm.Drawing.Color.Red);
                                                        painter.RenderQuality = RenderQuality.Fast;


                                                        writer.MoveTo(c0.ExtPoint_LeftInner.X, c0.ExtPoint_LeftInner.Y);
                                                        writer.LineTo(c0.ExtPoint_RightOuter.X, c0.ExtPoint_RightOuter.Y);
                                                        v7.GetVertex(0, out double v7x, out double v7y);
                                                        writer.LineTo(v7x - 2, v7y - 2);
                                                        writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                                                        writer.CloseFigure();
                                                        painter.Fill(v2, PixelFarm.Drawing.Color.Red);
                                                    }
                                                }
                                                break;
                                            case ExtMsdfGen.EdgeSegmentKind.QuadraticSegment:
                                                {
                                                    ExtMsdfGen.QuadraticSegment qs = (ExtMsdfGen.QuadraticSegment)ownerSeg;
                                                    double dx = translateVec.x;
                                                    double dy = translateVec.y;

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
                                                        s.Width = 4;
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
                                        }

                                    }
                                }
                            }
                        }

                        startAt = nextStartAt;
                        n++;
                    }
                    //DrawTessTriangles(polygon1, painter); 

                    bmpLut.SaveImage("d:\\WImageTest\\msdf_shape_lut2.png");
                    //
                    int[] lutBuffer = bmpLut.CopyImgBuffer(bmpLut.Width, bmpLut.Height);
                    //ExtMsdfgen.BmpEdgeLut bmpLut2 = new ExtMsdfgen.BmpEdgeLut(bmpLut.Width, bmpLut.Height, lutBuffer);


                    //bmpLut2 = null;
                    var bmp5 = MemBitmap.LoadBitmap("d:\\WImageTest\\msdf_shape_lut.png");

                    int[] lutBuffer5 = bmp5.CopyImgBuffer(bmpLut.Width, bmpLut.Height);
                    bmpLut7.SetBmpBuffer(bmpLut.Width, bmpLut.Height, lutBuffer5);

                    ExtMsdfGen.GlyphImage glyphImg = ExtMsdfGen.MsdfGlyphGen.CreateMsdfImage(shape1, msdfGenParams, bmpLut7);
                    //                     
                    using (Bitmap bmp3 = new Bitmap(glyphImg.Width, glyphImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        int[] buffer = glyphImg.GetImageBuffer();

                        var bmpdata = bmp3.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg.Width, glyphImg.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp3.PixelFormat);
                        System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                        bmp3.UnlockBits(bmpdata);
                        bmp3.Save("d:\\WImageTest\\msdf_shape.png");
                        //
                    }
                }
            }
        }
    }
}
