//MIT, 2019-present, WinterDev
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
        static ExtMsdfgen.Shape CreateShape(VertexStore vxs)
        {
            //create msdf shape from vertex store
            ExtMsdfgen.Shape shape1 = new ExtMsdfgen.Shape();
            int i = 0;
            double x, y;
            VertexCmd cmd;
            ExtMsdfgen.Contour cnt = null;
            double latestMoveToX = 0;
            double latestMoveToY = 0;
            double latestX = 0;
            double latestY = 0;
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
                                    cnt.AddLine(latestX, latestY, latestMoveToX, latestMoveToY);
                                }
                            }
                        }
                        break;
                    case VertexCmd.P2c:
                        {
                            //C3 curve (Quadratic)
                            if (cnt == null)
                            {
                                cnt = new ExtMsdfgen.Contour();
                            }

                            var cmd1 = vxs.GetVertex(i + 1, out double x1, out double y1);
                            i++;

                            if (cmd1 != VertexCmd.LineTo)
                            {
                                throw new NotSupportedException();
                            }

                            cnt.AddQuadraticSegment(latestX, latestY, x, y, x1, y1);

                            latestX = x1;
                            latestY = y1;

                        }
                        break;
                    case VertexCmd.P3c:
                        {
                            //C4 curve (Cubic)
                            if (cnt == null)
                            {
                                cnt = new ExtMsdfgen.Contour();
                            }

                            var cmd1 = vxs.GetVertex(i + 1, out double x2, out double y2);
                            var cmd2 = vxs.GetVertex(i + 2, out double x3, out double y3);
                            i += 2;

                            if (cmd1 != VertexCmd.P3c || cmd2 != VertexCmd.LineTo)
                            {
                                throw new NotSupportedException();
                            }

                            cnt.AddCubicSegment(latestX, latestY, x, y, x2, y2, x3, y3);

                            latestX = x2;
                            latestY = y2;

                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            if (cnt == null)
                            {
                                cnt = new ExtMsdfgen.Contour();
                            }
                            cnt.AddLine(latestX, latestY, x, y);
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
                cnt = null;
            }
            return shape1;
        }

        private void cmdTestMsdfGen_Click(object sender, EventArgs e)
        {
            List<PixelFarm.Drawing.PointF> points = new List<PixelFarm.Drawing.PointF>();
            points.AddRange(new PixelFarm.Drawing.PointF[]{
                    new PixelFarm.Drawing.PointF(10, 20),
                    new PixelFarm.Drawing.PointF(50, 60),
                    new PixelFarm.Drawing.PointF(80, 20),
                    new PixelFarm.Drawing.PointF(50, 10),
                    //new PixelFarm.Drawing.PointF(10, 20)
            });
            //1. 
            ExtMsdfgen.Shape shape1 = null;
            RectD bounds = RectD.ZeroIntersection;
            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(v1, out PathWriter w))
            {
                int count = points.Count;
                PixelFarm.Drawing.PointF pp = points[0];
                w.MoveTo(pp.X, pp.Y);
                for (int i = 1; i < count; ++i)
                {
                    pp = points[i];
                    w.LineTo(pp.X, pp.Y);
                }
                w.CloseFigure();

                bounds = v1.GetBoundingRect();
                shape1 = CreateShape(v1);

            }

            //using (VxsTemp.Borrow(out var v1))
            //using (VectorToolBox.Borrow(v1, out PathWriter w))
            //{


            //    w.MoveTo(15, 20);
            //    w.LineTo(50, 60);
            //    w.LineTo(60, 20);
            //    w.LineTo(50, 10);
            //    w.CloseFigure();

            //    bounds = v1.GetBoundingRect();
            //    shape1 = CreateShape(v1);
            //}

            //2.
            ExtMsdfgen.MsdfGenParams msdfGenParams = new ExtMsdfgen.MsdfGenParams();
            ExtMsdfgen.GlyphImage glyphImg = ExtMsdfgen.MsdfGlyphGen.CreateMsdfImage(shape1, msdfGenParams);
            using (Bitmap bmp = new Bitmap(glyphImg.Width, glyphImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                int[] buffer = glyphImg.GetImageBuffer();

                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg.Width, glyphImg.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                bmp.UnlockBits(bmpdata);
                bmp.Save("d:\\WImageTest\\msdf_shape.png");
                //
            }
        }

        class ShapeCornerArms
        {
            public PixelFarm.Drawing.Color leftExtededColor;
            public PixelFarm.Drawing.Color rightExtendedColor;

            public int LeftIndex;
            public int MiddleIndex;
            public int RightIndex;

            public PixelFarm.Drawing.PointF leftPoint;
            public PixelFarm.Drawing.PointF middlePoint;
            public PixelFarm.Drawing.PointF rightPoint;



            //-----------
            /// <summary>
            /// extended point of left->middle line
            /// </summary>
            public PixelFarm.Drawing.PointF leftExtendedPoint_Outer;
            public PixelFarm.Drawing.PointF leftExtendedPoint_OuterGap;
            public PixelFarm.Drawing.PointF leftExtendedPoint_Inner;
            /// <summary>
            /// extended point of right->middle line
            /// </summary>
            public PixelFarm.Drawing.PointF rightExtendedPoint_Outer;
            public PixelFarm.Drawing.PointF rightExtendedPoint_OuterGap;
            public PixelFarm.Drawing.PointF rightExtendedPoint_Inner;



            public PixelFarm.Drawing.PointF leftExtendedPointDest_Inner;
            /// <summary>
            /// destination point of left-extened point (to right extened point of other)
            /// </summary>
            public PixelFarm.Drawing.PointF leftExtendedPointDest_Outer;

            /// <summary>
            /// destination point right-extended point
            /// </summary>
            public PixelFarm.Drawing.PointF rightExtendedPointDest_Outer;
            public PixelFarm.Drawing.PointF rightExtendedPointDest_Inner;
            //-----------


            public ShapeCornerArms()
            {

            }

            public void Offset(float dx, float dy)
            {
                //
                leftPoint.Offset(dx, dy);
                middlePoint.Offset(dx, dy);
                rightPoint.Offset(dx, dy);

                leftExtendedPoint_Outer.Offset(dx, dy);
                rightExtendedPoint_Outer.Offset(dx, dy);
                leftExtendedPointDest_Outer.Offset(dx, dy);
                rightExtendedPointDest_Outer.Offset(dx, dy);
                //

                leftExtendedPoint_Inner.Offset(dx, dy);
                rightExtendedPoint_Inner.Offset(dx, dy);
                leftExtendedPointDest_Inner.Offset(dx, dy);
                rightExtendedPointDest_Inner.Offset(dx, dy);
                //
                leftExtendedPoint_OuterGap.Offset(dx, dy);
                rightExtendedPoint_OuterGap.Offset(dx, dy);
            }
            static double CurrentLen(PixelFarm.Drawing.PointF p0, PixelFarm.Drawing.PointF p1)
            {
                float dx = p1.X - p0.X;
                float dy = p1.Y - p0.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
            public void CreateExtendedEdges()
            {
                //
                rightExtendedPoint_Outer = CreateExtendedOuterEdges(rightPoint, middlePoint);
                rightExtendedPoint_OuterGap = CreateExtendedOuterGapEdges(rightPoint, middlePoint);
                //
                leftExtendedPoint_Outer = CreateExtendedOuterEdges(leftPoint, middlePoint);
                leftExtendedPoint_OuterGap = CreateExtendedOuterGapEdges(leftPoint, middlePoint);
                //
                rightExtendedPoint_Inner = CreateExtendedInnerEdges(rightPoint, middlePoint);
                leftExtendedPoint_Inner = CreateExtendedInnerEdges(leftPoint, middlePoint);
                // 
            }
            PixelFarm.Drawing.PointF CreateExtendedOuterEdges(PixelFarm.Drawing.PointF p0, PixelFarm.Drawing.PointF p1)
            {

                double rad = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
                double currentLen = CurrentLen(p0, p1);
                double newLen = currentLen + 3;

                double new_dx = Math.Cos(rad) * newLen;
                double new_dy = Math.Sin(rad) * newLen;


                return new PixelFarm.Drawing.PointF((float)(p0.X + new_dx), (float)(p0.Y + new_dy));
            }
            PixelFarm.Drawing.PointF CreateExtendedOuterGapEdges(PixelFarm.Drawing.PointF p0, PixelFarm.Drawing.PointF p1)
            {

                double rad = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
                double currentLen = CurrentLen(p0, p1);
                double newLen = currentLen + 2;

                double new_dx = Math.Cos(rad) * newLen;
                double new_dy = Math.Sin(rad) * newLen;

                return new PixelFarm.Drawing.PointF((float)(p0.X + new_dx), (float)(p0.Y + new_dy));
            }
            PixelFarm.Drawing.PointF CreateExtendedInnerEdges(PixelFarm.Drawing.PointF p0, PixelFarm.Drawing.PointF p1)
            {

                double rad = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
                double currentLen = CurrentLen(p0, p1);
                if (currentLen - 3 < 0)
                {
                    return p0;//***
                }

                double newLen = currentLen - 3;
                double new_dx = Math.Cos(rad) * newLen;
                double new_dy = Math.Sin(rad) * newLen;
                return new PixelFarm.Drawing.PointF((float)(p0.X + new_dx), (float)(p0.Y + new_dy));
            }
            public override string ToString()
            {
                return LeftIndex + "," + MiddleIndex + "," + RightIndex;
            }
        }


        List<ShapeCornerArms> CreateCornerAndArmList(List<PixelFarm.Drawing.PointF> points)
        {
            List<ShapeCornerArms> cornerAndArms = new List<ShapeCornerArms>();
            int j = points.Count;

            for (int i = 1; i < j - 1; ++i)
            {
                PixelFarm.Drawing.PointF p0 = points[i - 1];
                PixelFarm.Drawing.PointF p1 = points[i];
                PixelFarm.Drawing.PointF p2 = points[i + 1];


                ShapeCornerArms cornerArm = new ShapeCornerArms();
                cornerArm.leftPoint = p0;
                cornerArm.middlePoint = p1;
                cornerArm.rightPoint = p2;

                cornerArm.LeftIndex = i - 1;
                cornerArm.MiddleIndex = i;
                cornerArm.RightIndex = i + 1;


                cornerArm.CreateExtendedEdges();
                cornerAndArms.Add(cornerArm);
            }

            {
                //
                PixelFarm.Drawing.PointF p0 = points[j - 2];
                PixelFarm.Drawing.PointF p1 = points[j - 1];
                PixelFarm.Drawing.PointF p2 = points[0];


                ShapeCornerArms cornerArm = new ShapeCornerArms();
                cornerArm.leftPoint = p0;
                cornerArm.middlePoint = p1;
                cornerArm.rightPoint = p2;

                cornerArm.LeftIndex = j - 2;
                cornerArm.MiddleIndex = j - 1;
                cornerArm.RightIndex = 0;

                cornerArm.CreateExtendedEdges();
                cornerAndArms.Add(cornerArm);
            }

            {
                //
                PixelFarm.Drawing.PointF p0 = points[j - 1];
                PixelFarm.Drawing.PointF p1 = points[0];
                PixelFarm.Drawing.PointF p2 = points[1];


                ShapeCornerArms cornerArm = new ShapeCornerArms();
                cornerArm.leftPoint = p0;
                cornerArm.middlePoint = p1;
                cornerArm.rightPoint = p2;

                cornerArm.LeftIndex = j - 1;
                cornerArm.MiddleIndex = 0;
                cornerArm.RightIndex = 1;

                cornerArm.CreateExtendedEdges();
                cornerAndArms.Add(cornerArm);
            }
            return cornerAndArms;
        }
        void TranslateArms(List<ShapeCornerArms> cornerArms, double dx, double dy)
        {
            //test 2 if each edge has unique color
            int j = cornerArms.Count;
            for (int i = 0; i < j; ++i)
            {
                ShapeCornerArms arm = cornerArms[i];
                arm.Offset((float)dx, (float)dy);
            }
        }
        void ColorShapeCornerArms(List<ShapeCornerArms> cornerArms)
        {
            //test 2 if each edge has unique color
            // 
            int currentColor = 0;
            int j = cornerArms.Count;

            List<PixelFarm.Drawing.Color> colorList = new List<PixelFarm.Drawing.Color>();
            for (int i = 0; i < j + 1; ++i)
            {
                colorList.Add(new PixelFarm.Drawing.Color(255, (byte)(100 + i * 20), (byte)(100 + i * 20), (byte)(100 + i * 20)));
            }

            int max_colorCount = colorList.Count;

            for (int i = 1; i < j; ++i)
            {
                ShapeCornerArms c_prev = cornerArms[i - 1];
                ShapeCornerArms c_current = cornerArms[i];
                //
                PixelFarm.Drawing.Color selColor = colorList[currentColor];
                c_prev.rightExtendedColor = c_current.leftExtededColor = selColor; //same color
                //
                c_prev.leftExtendedPointDest_Outer = c_current.rightExtendedPoint_Outer;
                c_prev.leftExtendedPointDest_Inner = c_current.rightExtendedPoint_Inner;
                //
                c_current.rightExtendedPointDest_Outer = c_prev.leftExtendedPoint_Outer;
                c_current.rightExtendedPointDest_Inner = c_prev.leftExtendedPoint_Inner;
                //
                currentColor++;
                if (currentColor > max_colorCount)
                {
                    //make it ready for next round
                    currentColor = 0;
                }
            }

            {
                //the last one
                ShapeCornerArms c_prev = cornerArms[j - 1];
                ShapeCornerArms c_current = cornerArms[0];
                PixelFarm.Drawing.Color selColor = colorList[currentColor];
                c_prev.rightExtendedColor = c_current.leftExtededColor = selColor; //same color
                                                                                   //
                c_prev.leftExtendedPointDest_Outer = c_current.rightExtendedPoint_Outer;
                c_prev.leftExtendedPointDest_Inner = c_current.rightExtendedPoint_Inner;
                //
                c_current.rightExtendedPointDest_Outer = c_prev.leftExtendedPoint_Outer;
                c_current.rightExtendedPointDest_Inner = c_prev.leftExtendedPoint_Inner;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //test fake msdf

            List<PixelFarm.Drawing.PointF> points = new List<PixelFarm.Drawing.PointF>();
            points.AddRange(new PixelFarm.Drawing.PointF[]{
                    new PixelFarm.Drawing.PointF(10, 20),
                    new PixelFarm.Drawing.PointF(50, 60),
                    new PixelFarm.Drawing.PointF(80, 20),
                    new PixelFarm.Drawing.PointF(50, 10),
                    //new PixelFarm.Drawing.PointF(10, 20)
            });
            //--------------------
            //create outside connected line
            List<ShapeCornerArms> cornerAndArms = CreateCornerAndArmList(points);
            ColorShapeCornerArms(cornerAndArms);



            //--------------------

            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(v1, out PathWriter w))
            {
                int count = points.Count;
                PixelFarm.Drawing.PointF pp = points[0];
                w.MoveTo(pp.X, pp.Y);
                for (int i = 1; i < count; ++i)
                {
                    pp = points[i];
                    w.LineTo(pp.X, pp.Y);
                }
                w.CloseFigure();

                RectD bounds = v1.GetBoundingRect();
                bounds.Inflate(15);

                //---------
                //Poly2Tri.Polygon polygon = CreatePolygon(points, bounds);
                //Poly2Tri.P2T.Triangulate(polygon);


                using (MemBitmap bmp = new MemBitmap(100, 100))
                using (AggPainterPool.Borrow(bmp, out AggPainter painter))
                {
                    painter.Clear(PixelFarm.Drawing.Color.Black);
                    painter.Fill(v1, PixelFarm.Drawing.Color.White);
                    //DrawTessTriangles(polygon, painter);


                    painter.StrokeColor = PixelFarm.Drawing.Color.Red;
                    painter.StrokeWidth = 1;

                    int cornerArmCount = cornerAndArms.Count;
                    for (int n = 1; n < cornerArmCount; ++n)
                    {
                        ShapeCornerArms p0 = cornerAndArms[n - 1];
                        ShapeCornerArms p1 = cornerAndArms[n];

                        using (VxsTemp.Borrow(out var v2))
                        using (VectorToolBox.Borrow(v2, out PathWriter writer))
                        {
                            //outer
                            writer.MoveTo(p0.middlePoint.X, p0.middlePoint.Y);
                            writer.LineTo(p0.rightExtendedPoint_Outer.X, p0.rightExtendedPoint_Outer.Y);
                            writer.LineTo(p0.rightExtendedPointDest_Outer.X, p0.rightExtendedPointDest_Outer.Y);
                            writer.LineTo(p1.middlePoint.X, p1.middlePoint.Y);
                            writer.LineTo(p0.middlePoint.X, p0.middlePoint.Y);
                            writer.CloseFigure();
                            //
                            painter.Fill(v2, p0.rightExtendedColor);

                            //inner
                            v2.Clear();
                            writer.MoveTo(p0.middlePoint.X, p0.middlePoint.Y);
                            writer.LineTo(p0.rightExtendedPoint_Inner.X, p0.rightExtendedPoint_Inner.Y);
                            writer.LineTo(p0.rightExtendedPointDest_Inner.X, p0.rightExtendedPointDest_Inner.Y);
                            writer.LineTo(p1.middlePoint.X, p1.middlePoint.Y);
                            writer.LineTo(p0.middlePoint.X, p0.middlePoint.Y);
                            writer.CloseFigure();
                            //
                            painter.Fill(v2, p0.rightExtendedColor);

                        }
                    }
                    //--------------------------------------------------------------------------------
                    {
                        //the last one
                        ShapeCornerArms p0 = cornerAndArms[cornerArmCount - 1];
                        ShapeCornerArms p1 = cornerAndArms[0];

                        using (VxsTemp.Borrow(out var v2))
                        using (VectorToolBox.Borrow(v2, out PathWriter writer))
                        {
                            //
                            writer.MoveTo(p0.middlePoint.X, p0.middlePoint.Y);
                            writer.LineTo(p0.rightExtendedPoint_Outer.X, p0.rightExtendedPoint_Outer.Y);
                            writer.LineTo(p0.rightExtendedPointDest_Outer.X, p0.rightExtendedPointDest_Outer.Y);
                            writer.LineTo(p1.middlePoint.X, p1.middlePoint.Y);
                            writer.LineTo(p0.middlePoint.X, p0.middlePoint.Y);
                            writer.CloseFigure();
                            //
                            painter.Fill(v2, p0.rightExtendedColor);

                            //inner
                            v2.Clear();
                            writer.MoveTo(p0.middlePoint.X, p0.middlePoint.Y);
                            writer.LineTo(p0.rightExtendedPoint_Inner.X, p0.rightExtendedPoint_Inner.Y);
                            writer.LineTo(p0.rightExtendedPointDest_Inner.X, p0.rightExtendedPointDest_Inner.Y);
                            writer.LineTo(p1.middlePoint.X, p1.middlePoint.Y);
                            writer.LineTo(p0.middlePoint.X, p0.middlePoint.Y);
                            writer.CloseFigure();
                            //
                            painter.Fill(v2, p0.rightExtendedColor);
                        }
                    }
                    painter.Fill(v1, PixelFarm.Drawing.Color.White);

                    //foreach (ShapeCornerArms cornerArm in cornerAndArms)
                    //{

                    //    //right arm
                    //    painter.StrokeColor = cornerArm.rightExtendedColor;
                    //    painter.DrawLine(cornerArm.middlePoint.X, cornerArm.middlePoint.Y,
                    //        cornerArm.rightExtendedPoint.X, cornerArm.rightExtendedPoint.Y);

                    //    //left arm
                    //    painter.StrokeColor = cornerArm.leftExtededColor;
                    //    painter.DrawLine(cornerArm.middlePoint.X, cornerArm.middlePoint.Y,
                    //        cornerArm.leftExtendedPoint.X, cornerArm.leftExtendedPoint.Y);

                    //    using (VxsTemp.Borrow(out var v2))
                    //    using (VectorToolBox.Borrow(v2, out PathWriter writer))
                    //    {
                    //        writer.MoveTo(cornerArm.middlePoint.X, cornerArm.middlePoint.Y);
                    //        writer.LineTo(cornerArm.rightExtendedPoint.X, cornerArm.rightExtendedPoint.Y);
                    //        writer.LineTo(cornerArm.rightDestConnectedPoint.X, cornerArm.rightDestConnectedPoint.Y);
                    //        writer.LineTo(cornerArm.rightDestConnectedPoint.X, cornerArm.rightDestConnectedPoint.Y);

                    //    } 
                    //}
                    bmp.SaveImage("d:\\WImageTest\\msdf_fake1.png");
                }

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



        /// <summary>
        /// create polygon from GlyphContour
        /// </summary>
        /// <param name="cnt"></param>
        /// <returns></returns>
        static Poly2Tri.Polygon CreatePolygon(List<PixelFarm.Drawing.PointF> flattenPoints, double dx, double dy)
        {
            List<Poly2Tri.TriangulationPoint> points = new List<Poly2Tri.TriangulationPoint>();

            //limitation: poly tri not accept duplicated points! *** 
            double prevX = 0;
            double prevY = 0;

            int j = flattenPoints.Count;
            //pass
            for (int i = 0; i < j; ++i)
            {
                PixelFarm.Drawing.PointF pp = flattenPoints[i];

                double x = pp.X + dx; //start from original X***
                double y = pp.Y + dy; //start from original Y***

                if (x == prevX && y == prevY)
                {
                    if (i > 0)
                    {
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    var triPoint = new Poly2Tri.TriangulationPoint(prevX = x, prevY = y) { userData = pp };
                    //#if DEBUG
                    //                    p.dbugTriangulationPoint = triPoint;
                    //#endif
                    points.Add(triPoint);

                }
            }

            return new Poly2Tri.Polygon(points.ToArray());

        }

        static Poly2Tri.Polygon CreateInvertedPolygon(List<PixelFarm.Drawing.PointF> flattenPoints, RectD bounds)
        {

            Poly2Tri.Polygon mainPolygon = new Poly2Tri.Polygon(new Poly2Tri.TriangulationPoint[]
            {
                new Poly2Tri.TriangulationPoint( bounds.Left,   bounds.Bottom),
                new Poly2Tri.TriangulationPoint( bounds.Right,  bounds.Bottom),
                new Poly2Tri.TriangulationPoint( bounds.Right,  bounds.Top),
                new Poly2Tri.TriangulationPoint( bounds.Left,   bounds.Top)
            });

            //find bounds

            List<Poly2Tri.TriangulationPoint> points = new List<Poly2Tri.TriangulationPoint>();

            //limitation: poly tri not accept duplicated points! *** 
            double prevX = 0;
            double prevY = 0;

            int j = flattenPoints.Count;
            //pass
            for (int i = 0; i < j; ++i)
            {
                PixelFarm.Drawing.PointF pp = flattenPoints[i];

                double x = pp.X; //start from original X***
                double y = pp.Y; //start from original Y***

                if (x == prevX && y == prevY)
                {
                    if (i > 0)
                    {
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    var triPoint = new Poly2Tri.TriangulationPoint(prevX = x, prevY = y) { userData = pp };
                    //#if DEBUG
                    //                    p.dbugTriangulationPoint = triPoint;
                    //#endif
                    points.Add(triPoint);

                }
            }

            Poly2Tri.Polygon p2 = new Poly2Tri.Polygon(points.ToArray());

            mainPolygon.AddHole(p2);
            return mainPolygon;
        }



        List<PixelFarm.Drawing.PointF> GetSamplePointList()
        {
            List<PixelFarm.Drawing.PointF> points = new List<PixelFarm.Drawing.PointF>();

            ////counter-clockwise
            //points.AddRange(new PixelFarm.Drawing.PointF[]{
            //        new PixelFarm.Drawing.PointF(10 , 20),
            //        new PixelFarm.Drawing.PointF(50 , 60),
            //        new PixelFarm.Drawing.PointF(70 , 20),
            //        new PixelFarm.Drawing.PointF(50 , 10),
            //       //close figure
            //});

            //counter-clockwise
            points.AddRange(new PixelFarm.Drawing.PointF[]{
                    new PixelFarm.Drawing.PointF(10 , 20),
                    new PixelFarm.Drawing.PointF(30 , 80),
                    new PixelFarm.Drawing.PointF(50 , 20 ),
                    new PixelFarm.Drawing.PointF(40 , 20 ),
                    new PixelFarm.Drawing.PointF(30 , 50 ),
                    new PixelFarm.Drawing.PointF(20 , 20 ),
                    //close figure
            });

            int j = points.Count;
            for (int i = 0; i < j; ++i)
            {
                PixelFarm.Drawing.PointF p = points[i];
                points[i] = new PixelFarm.Drawing.PointF(p.X * scale, p.Y * scale);
            }



            return points;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //test fake msdf (this is not real msdf gen)
            //--------------------

            List<PixelFarm.Drawing.PointF> points = GetSamplePointList();

            //create outside connected line
            List<ShapeCornerArms> cornerAndArms = CreateCornerAndArmList(points);
            ColorShapeCornerArms(cornerAndArms);

            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(v1, out PathWriter w))
            {
                int count = points.Count;
                PixelFarm.Drawing.PointF pp = points[0];
                w.MoveTo(pp.X, pp.Y);
                for (int i = 1; i < count; ++i)
                {
                    pp = points[i];
                    w.LineTo(pp.X, pp.Y);
                }
                w.CloseFigure();

                RectD bounds;
                ExtMsdfgen.Shape shape1 = null;
                using (VxsTemp.Borrow(out var v4))
                using (VectorToolBox.Borrow(v4, out PathWriter w4))
                {
                    int count4 = points.Count;
                    PixelFarm.Drawing.PointF pp4 = points[0];
                    w4.MoveTo(pp4.X, pp4.Y);
                    for (int i = 1; i < count; ++i)
                    {
                        pp4 = points[i];
                        w4.LineTo(pp4.X, pp4.Y);
                    }
                    w4.CloseFigure();
                    bounds = v4.GetBoundingRect();
                    shape1 = CreateShape(v4);
                }


                ExtMsdfgen.MsdfGenParams previewGenParams = new ExtMsdfgen.MsdfGenParams();
                ExtMsdfgen.MsdfGlyphGen.PreviewSizeAndLocation(
                   shape1,
                   previewGenParams,
                   out int imgW, out int imgH, out ExtMsdfgen.Vector2 translateVec);

                //---------
                TranslateArms(cornerAndArms, translateVec.x, translateVec.y);
                //Poly2Tri.Polygon polygon1 = CreatePolygon(points, translateVec.x, translateVec.y);
                //Poly2Tri.P2T.Triangulate(polygon1);
                //---------

                using (MemBitmap bmpLut = new MemBitmap(imgW, imgH))
                using (VxsTemp.Borrow(out var v5))
                using (AggPainterPool.Borrow(bmpLut, out AggPainter painter))
                {

                    painter.RenderQuality = RenderQuality.Fast;
                    painter.Clear(PixelFarm.Drawing.Color.Black);

                    v1.TranslateToNewVxs(translateVec.x, translateVec.y, v5);
                    painter.Fill(v5, PixelFarm.Drawing.Color.White);

                    painter.StrokeColor = PixelFarm.Drawing.Color.Red;
                    painter.StrokeWidth = 1;

                    int cornerArmCount = cornerAndArms.Count;
                    for (int n = 1; n < cornerArmCount; ++n)
                    {
                        ShapeCornerArms c0 = cornerAndArms[n - 1];
                        ShapeCornerArms c1 = cornerAndArms[n];

                        using (VxsTemp.Borrow(out var v2))
                        using (VectorToolBox.Borrow(v2, out PathWriter writer))
                        {
                            //counter-clockwise
                            writer.MoveTo(c0.middlePoint.X, c0.middlePoint.Y);
                            writer.LineTo(c0.leftExtendedPoint_Outer.X, c0.leftExtendedPoint_Outer.Y);
                            writer.LineTo(c0.leftExtendedPointDest_Outer.X, c0.leftExtendedPointDest_Outer.Y);
                            writer.LineTo(c1.middlePoint.X, c1.middlePoint.Y);
                            writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                            writer.CloseFigure();
                            //
                            painter.Fill(v2, c0.rightExtendedColor);

                            //------------------
                            //inner
                            v2.Clear();
                            writer.MoveTo(c0.leftExtendedPoint_Inner.X, c0.leftExtendedPoint_Inner.Y);
                            writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                            writer.LineTo(c1.middlePoint.X, c1.middlePoint.Y);
                            writer.LineTo(c1.rightExtendedPoint_Inner.X, c1.rightExtendedPoint_Inner.Y);
                            writer.LineTo(c0.leftExtendedPoint_Inner.X, c0.leftExtendedPoint_Inner.Y);
                            writer.CloseFigure();
                            ////
                            painter.Fill(v2, c0.rightExtendedColor);


                            //------------------
                            //outer gap
                            v2.Clear();
                            writer.MoveTo(c0.middlePoint.X, c0.middlePoint.Y);
                            writer.LineTo(c0.rightExtendedPoint_OuterGap.X, c0.rightExtendedPoint_OuterGap.Y);
                            writer.LineTo(c0.leftExtendedPoint_OuterGap.X, c0.leftExtendedPoint_OuterGap.Y);
                            writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                            writer.CloseFigure();

                            painter.Fill(v2, c0.rightExtendedColor);
                        }
                    }
                    {
                        //the last one
                        ShapeCornerArms c0 = cornerAndArms[cornerArmCount - 1];
                        ShapeCornerArms c1 = cornerAndArms[0];

                        using (VxsTemp.Borrow(out var v2))
                        using (VectorToolBox.Borrow(v2, out PathWriter writer))
                        {

                            //counter-clockwise

                            //------------------
                            //outer
                            writer.MoveTo(c0.middlePoint.X, c0.middlePoint.Y);
                            writer.LineTo(c0.leftExtendedPoint_Outer.X, c0.leftExtendedPoint_Outer.Y);
                            writer.LineTo(c0.leftExtendedPointDest_Outer.X, c0.leftExtendedPointDest_Outer.Y);
                            writer.LineTo(c1.middlePoint.X, c1.middlePoint.Y);
                            writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                            writer.CloseFigure();
                            painter.Fill(v2, c0.rightExtendedColor);
                            //
                            //------------------
                            //inner
                            v2.Clear();
                            writer.MoveTo(c0.leftExtendedPoint_Inner.X, c0.leftExtendedPoint_Inner.Y);
                            writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                            writer.LineTo(c1.middlePoint.X, c1.middlePoint.Y);
                            writer.LineTo(c1.rightExtendedPoint_Inner.X, c1.rightExtendedPoint_Inner.Y);
                            writer.LineTo(c0.leftExtendedPoint_Inner.X, c0.leftExtendedPoint_Inner.Y);
                            writer.CloseFigure();
                            //
                            painter.Fill(v2, c0.rightExtendedColor);

                            //------------------
                            //outer gap
                            v2.Clear();

                            writer.MoveTo(c0.middlePoint.X, c0.middlePoint.Y);

                            writer.LineTo(c0.rightExtendedPoint_OuterGap.X, c0.rightExtendedPoint_OuterGap.Y);
                            writer.LineTo(c0.leftExtendedPoint_OuterGap.X, c0.leftExtendedPoint_OuterGap.Y);
                            writer.LineTo(c0.middlePoint.X, c0.middlePoint.Y);
                            writer.CloseFigure();

                            painter.Fill(v2, c0.rightExtendedColor);
                        }
                    }

                    //DrawTessTriangles(polygon1, painter); 

                    bmpLut.SaveImage("d:\\WImageTest\\msdf_shape_lut2.png");

                    //
                    int[] lutBuffer = bmpLut.CopyImgBuffer(bmpLut.Width, bmpLut.Height);
                    ExtMsdfgen.BmpEdgeLut bmpLut2 = new ExtMsdfgen.BmpEdgeLut(bmpLut.Width, bmpLut.Height, lutBuffer);

                    //
                    ExtMsdfgen.MsdfGenParams msdfGenParams = new ExtMsdfgen.MsdfGenParams();

                    //bmpLut2 = null;
                    var bmp5 = MemBitmap.LoadBitmap("d:\\WImageTest\\msdf_shape_lut.png");
                    int[] lutBuffer5 = bmp5.CopyImgBuffer(bmpLut.Width, bmpLut.Height);
                    ExtMsdfgen.BmpEdgeLut bmpLut5 = new ExtMsdfgen.BmpEdgeLut(bmpLut.Width, bmpLut.Height, lutBuffer5);
                    ExtMsdfgen.GlyphImage glyphImg = ExtMsdfgen.MsdfGlyphGen.CreateMsdfImage(shape1, msdfGenParams, bmpLut5);
                    //
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
