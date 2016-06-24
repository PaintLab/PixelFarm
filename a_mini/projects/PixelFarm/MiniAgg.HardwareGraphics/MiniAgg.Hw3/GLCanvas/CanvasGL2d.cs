//MIT 2014, WinterDev

using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES20;
using Tesselate;
using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;
namespace PixelFarm.DrawingGL
{
    public partial class CanvasGL2d
    {
        BasicShader basicShader;
        SmoothLineShader smoothLineShader;
        InvertAlphaFragmentShader invertAlphaFragmentShader;
        BasicFillShader basicFillShader;
        RectFillShader rectFillShader;
        SimpleTextureShader textureShader;
        //-----------------------------------------------------------

        PixelFarm.Drawing.Color strokeColor = PixelFarm.Drawing.Color.Black;
        Tesselator tess = new Tesselator();
        TessListener2 tessListener = new TessListener2();
        //tools---------------------------------
        RoundedRect roundRect = new RoundedRect();
        Ellipse ellipse = new Ellipse();
        PathWriter ps = new PathWriter();
        Stroke aggStroke = new Stroke(1);
        Arc arcTool = new Arc();
        CurveFlattener curveFlattener = new CurveFlattener();
        GLTextPrinter textPriner;
        int canvasOriginX = 0;
        int canvasOriginY = 0;
        int canvasW;
        int canvasH;
        MyMat4 orthoView;
        public CanvasGL2d(int canvasW, int canvasH)
        {
            this.canvasW = canvasW;
            this.canvasH = canvasH;
            basicShader = new BasicShader();
            smoothLineShader = new SmoothLineShader();
            basicFillShader = new BasicFillShader();
            rectFillShader = new RectFillShader();
            textureShader = new SimpleTextureShader();
            invertAlphaFragmentShader = new InvertAlphaFragmentShader(); //used with stencil  ***
                                                                         // tessListener.Connect(tess, Tesselate.Tesselator.WindingRuleType.Odd, true);
            tess.WindingRule = Tesselator.WindingRuleType.Odd;
            tessListener.Connect(tess, true);
            textPriner = new GLTextPrinter(this);
            SetupFonts();
            ////--------------------------------------------------------------------------------
            //GL.Enable(EnableCap.CullFace);
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.CullFace(CullFaceMode.Back);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            ////setup viewport size
            int max = Math.Max(canvasW, canvasH);
            ////square viewport 
            orthoView = MyMat4.ortho(0, max, 0, max, 0, 1);
            //-------------------------------------------------------------------------------

            smoothLineShader.OrthoView = orthoView;
            basicFillShader.OrthoView = orthoView;
            textureShader.OrthoView = orthoView;
            basicFillShader.OrthoView = orthoView;
            invertAlphaFragmentShader.OrthoView = orthoView;
            GL.Viewport(0, 0, canvasW, canvasH);
        }
        public void Dispose()
        {
        }

        public CanvasSmoothMode SmoothMode
        {
            get;
            set;
        }


        public void Clear(PixelFarm.Drawing.Color c)
        {
            //set value for clear color buffer 
            GLHelper.ClearColor(c);
            GL.ClearStencil(0);
            //actual clear here !
            GL.Clear(ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit |
                ClearBufferMask.StencilBufferBit);
        }
        public void ClearColorBuffer()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }
        public double StrokeWidth
        {
            get { return this.aggStroke.Width; }
            set
            {
                //agg stroke
                this.aggStroke.Width = value;
            }
        }
        public PixelFarm.Drawing.Color StrokeColor
        {
            get { return this.strokeColor; }
            set { this.strokeColor = value; }
        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        smoothLineShader.StrokeColor = this.strokeColor;
                        smoothLineShader.StrokeWidth = (float)this.StrokeWidth;
                        this.smoothLineShader.DrawLine(x1, y1, x2, y2);
                    }
                    break;
                default:
                    {
                        this.basicShader.DrawLine(x1, y1, x2, y2, this.strokeColor);
                    }
                    break;
            }
        }

        //-------------------------------------------------------------------------------
        public void DrawImage(GLBitmap bmp, float x, float y)
        {
            DrawImage(bmp,
                   new PixelFarm.Drawing.RectangleF(0, 0, bmp.Width, bmp.Height),
                   x, y, bmp.Width, bmp.Height);
        }
        public void DrawImage(GLBitmap bmp, float x, float y, float w, float h)
        {
            DrawImage(bmp,
                new PixelFarm.Drawing.RectangleF(0, 0, bmp.Width, bmp.Height),
                x, y, w, h);
        }
        public void DrawImage(GLBitmap bmp,
            PixelFarm.Drawing.RectangleF srcRect,
            float x, float y, float w, float h)
        {
            this.textureShader.Render(bmp, x, y, w, h);
        }
        //-------------------------------------------------------------------------------
        public void DrawImage(GLBitmapReference bmp, float x, float y)
        {
            this.DrawImage(bmp.OwnerBitmap,
                 bmp.GetRectF(),
                 x, y, bmp.Width, bmp.Height);
        }

        public static List<List<float>> CreateGraphicsPath(VertexStoreSnap vxsSnap)
        {
            VertexSnapIter vxsIter = vxsSnap.GetVertexSnapIter();
            double prevX = 0;
            double prevY = 0;
            double prevMoveToX = 0;
            double prevMoveToY = 0;
            List<List<float>> allXYlist = new List<List<float>>(); //all include sub path
            List<float> xylist = new List<float>();
            allXYlist.Add(xylist);
            bool isAddToList = true;
            for (;;)
            {
                double x, y;
                VertexCmd cmd = vxsIter.GetNextVertex(out x, out y);
                switch (cmd)
                {
                    case PixelFarm.Agg.VertexCmd.MoveTo:
                        if (!isAddToList)
                        {
                            allXYlist.Add(xylist);
                            isAddToList = true;
                        }
                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        xylist.Add((float)x);
                        xylist.Add((float)y);
                        break;
                    case PixelFarm.Agg.VertexCmd.LineTo:
                        xylist.Add((float)x);
                        xylist.Add((float)y);
                        prevX = x;
                        prevY = y;
                        break;
                    case PixelFarm.Agg.VertexCmd.CloseAndEndFigure:
                        //from current point 
                        xylist.Add((float)prevMoveToX);
                        xylist.Add((float)prevMoveToY);
                        prevX = prevMoveToX;
                        prevY = prevMoveToY;
                        //start the new one
                        xylist = new List<float>();
                        isAddToList = false;
                        break;
                    case PixelFarm.Agg.VertexCmd.EndFigure:
                        break;
                    case PixelFarm.Agg.VertexCmd.Stop:
                        goto EXIT_LOOP;
                    default:
                        throw new NotSupportedException();
                }
            }
        EXIT_LOOP:
            return allXYlist;
        }

        public void FillVxsSnap(PixelFarm.Drawing.Color color, VertexStoreSnap snap)
        {
            List<List<float>> allXYList = CreateGraphicsPath(snap);
            switch (SmoothMode)
            {
                case CanvasSmoothMode.No:
                    {
                        int subPathCount = allXYList.Count;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            //fill polygon with color
                            float[] subpathXYList = allXYList[i].ToArray();
                            FillPolygon(color, subpathXYList);
                        }
                    }
                    break;
                case CanvasSmoothMode.Smooth:
                    {
                        int subPathCount = allXYList.Count;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            //fill polygon with color
                            float[] subpathXYList = allXYList[i].ToArray();
                            FillPolygon(color, subpathXYList);
                            //with anti-alias border
                            strokeColor = color;
                            StrokeWidth = 0.5f;
                            DrawPolygon(subpathXYList, subpathXYList.Length);
                        }
                    }
                    break;
            }
        }
        public void DrawVxsSnap(PixelFarm.Drawing.Color color, VertexStoreSnap snap)
        {
            List<List<float>> allXYList = CreateGraphicsPath(snap);
            switch (SmoothMode)
            {
                case CanvasSmoothMode.No:
                    {
                        int subPathCount = allXYList.Count;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            //fill polygon with color
                            float[] subpathXYList = allXYList[i].ToArray();
                            DrawPolygon(subpathXYList, subpathXYList.Length);
                        }
                    }
                    break;
                case CanvasSmoothMode.Smooth:
                    {
                        int subPathCount = allXYList.Count;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            strokeColor = color;
                            StrokeWidth = 1f;
                            float[] subpathXYList = allXYList[i].ToArray();
                            DrawPolygon(subpathXYList, subpathXYList.Length);
                        }
                    }
                    break;
            }
        }
        public void DrawPolygon(float[] polygon2dVertices, int npoints)
        {
            //closed polyline
            //draw polyline
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        smoothLineShader.StrokeColor = this.strokeColor;
                        smoothLineShader.StrokeWidth = (float)this.StrokeWidth;
                        smoothLineShader.DrawPolygon(polygon2dVertices, npoints);
                    }
                    break;
                default:
                    {
                        unsafe
                        {
                            fixed (float* arr = &polygon2dVertices[0])
                            {
                                DrawPolygonUnsafe(arr, npoints);
                            }
                        }
                    }
                    break;
            }
        }
        public void DrawEllipse(float x, float y, double rx, double ry)
        {
            ellipse.Reset(x, y, rx, ry);
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        VertexStore vxs = ellipse.MakeVxs();
                        int n = vxs.Count;
                        float[] coords = new float[n * 2];
                        int i = 0;
                        int nn = 0;
                        double vx, vy;
                        var cmd = vxs.GetVertex(i, out vx, out vy);
                        while (i < n)
                        {
                            switch (cmd)
                            {
                                case VertexCmd.MoveTo:
                                    {
                                        coords[nn++] = (float)vx;
                                        coords[nn++] = (float)vy;
                                    }
                                    break;
                                case VertexCmd.LineTo:
                                    {
                                        coords[nn++] = (float)vx;
                                        coords[nn++] = (float)vy;
                                    }
                                    break;
                                default:
                                    {
                                        i = n + 1; //stop
                                    }
                                    break;
                            }
                            i++;
                            cmd = vxs.GetVertex(i, out vx, out vy);
                        }
                        //--------------------------------------
                        smoothLineShader.StrokeColor = this.strokeColor;
                        smoothLineShader.DrawPolygon(coords, nn);
                    }
                    break;
                default:
                    {
                        VertexStore vxs = ellipse.MakeVxs();
                        int n = vxs.Count;
                        unsafe
                        {
                            float* coords = stackalloc float[n * 2];
                            int i = 0;
                            int nn = 0;
                            double vx, vy;
                            var cmd = vxs.GetVertex(i, out vx, out vy);
                            while (i < n)
                            {
                                switch (cmd)
                                {
                                    case VertexCmd.MoveTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                        }
                                        break;
                                    case VertexCmd.LineTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                        }
                                        break;
                                    case VertexCmd.Stop:
                                        {
                                        }
                                        break;
                                    default:
                                        {
                                        }
                                        break;
                                }
                                i++;
                                cmd = vxs.GetVertex(i, out vx, out vy);
                            }
                            //-------------------------------------- 
                            DrawPolygonUnsafe(coords, nn / 2);
                        }
                    }
                    break;
            }
        }
        public void DrawCircle(float x, float y, double radius)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        ellipse.Reset(x, y, radius, radius);
                        VertexStore vxs = ellipse.MakeVxs();
                        int n = vxs.Count;
                        float[] coords = new float[n * 2];
                        int i = 0;
                        int nn = 0;
                        double vx, vy;
                        var cmd = vxs.GetVertex(i, out vx, out vy);
                        while (i < n)
                        {
                            switch (cmd)
                            {
                                case VertexCmd.MoveTo:
                                    {
                                        coords[nn++] = (float)vx;
                                        coords[nn++] = (float)vy;
                                    }
                                    break;
                                case VertexCmd.LineTo:
                                    {
                                        coords[nn++] = (float)vx;
                                        coords[nn++] = (float)vy;
                                    }
                                    break;
                                default:
                                    {
                                        i = n + 1; //stop
                                    }
                                    break;
                            }
                            i++;
                            cmd = vxs.GetVertex(i, out vx, out vy);
                        }
                        //--------------------------------------
                        smoothLineShader.StrokeColor = this.strokeColor;
                        smoothLineShader.DrawPolygon(coords, nn);
                    }
                    break;
                default:
                    {
                        DrawEllipse(x, y, radius, radius);
                    }
                    break;
            }
        }
        public void DrawRect(float x, float y, float w, float h)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        smoothLineShader.StrokeColor = this.strokeColor;
                        smoothLineShader.StrokeWidth = (float)this.StrokeWidth;
                        CoordList2f coords = new CoordList2f();
                        CreatePolyLineRectCoords2(coords, x, y, w, h);
                        float[] internalArr = coords.GetInternalArray();
                        smoothLineShader.DrawPolygon(internalArr, coords.Count << 1);
                    }
                    break;
                default:
                    {
                        //this.basicShader.DrawLine(x1, y1, x2, y2, this.strokeColor);
                    }
                    break;
            }
        }
        public void DrawRoundRect(float x, float y, float w, float h, float rx, float ry)
        {
            throw new NotSupportedException();
            roundRect.SetRect(x, y, x + w, y + h);
            roundRect.SetRadius(rx, ry);
            var vxs = this.aggStroke.MakeVxs(roundRect.MakeVxs());
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        //sclineRas.Reset();
                        //sclineRas.AddPath(vxs);
                        //sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
                    }
                    break;
                default:
                    {
                        //    sclineRas.Reset();
                        //    sclineRas.AddPath(vxs);
                        //    sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
                    }
                    break;
            }
        }



        static double DegToRad(double degree)
        {
            return degree * (Math.PI / 180d);
        }
        static double RadToDeg(double degree)
        {
            return degree * (180d / Math.PI);
        }

        public void DrawArc(float fromX, float fromY, float endX, float endY,
            float xaxisRotationAngleDec, float rx, float ry,
            SvgArcSize arcSize, SvgArcSweep arcSweep)
        {
            //------------------
            //SVG Elliptical arc ...
            //from Apache Batik
            //-----------------

            CenterFormArc centerFormArc = new CenterFormArc();
            ComputeArc2(fromX, fromY, rx, ry,
                 DegToRad(xaxisRotationAngleDec),
                 arcSize == SvgArcSize.Large,
                 arcSweep == SvgArcSweep.Negative,
                 endX, endY, ref centerFormArc);
            arcTool.Init(centerFormArc.cx, centerFormArc.cy, rx, ry,
                centerFormArc.radStartAngle,
                (centerFormArc.radStartAngle + centerFormArc.radSweepDiff));
            VertexStore vxs = new VertexStore();
            bool stopLoop = false;
            foreach (VertexData vertexData in arcTool.GetVertexIter())
            {
                switch (vertexData.command)
                {
                    case VertexCmd.Stop:
                        stopLoop = true;
                        break;
                    default:
                        vxs.AddVertex(vertexData.x, vertexData.y, vertexData.command);
                        //yield return vertexData;
                        break;
                }
                //------------------------------
                if (stopLoop) { break; }
            }


            double scaleRatio = 1;
            if (centerFormArc.scaleUp)
            {
                int vxs_count = vxs.Count;
                double px0, py0, px_last, py_last;
                vxs.GetVertex(0, out px0, out py0);
                vxs.GetVertex(vxs_count - 1, out px_last, out py_last);
                double distance1 = Math.Sqrt((px_last - px0) * (px_last - px0) + (py_last - py0) * (py_last - py0));
                double distance2 = Math.Sqrt((endX - fromX) * (endX - fromX) + (endY - fromY) * (endY - fromY));
                if (distance1 < distance2)
                {
                    scaleRatio = distance2 / distance1;
                }
                else
                {
                }
            }

            if (xaxisRotationAngleDec != 0)
            {
                //also  rotate 
                if (centerFormArc.scaleUp)
                {
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Scale, scaleRatio, scaleRatio),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Rotate, DegToRad(xaxisRotationAngleDec)),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    vxs = mat.TransformToVxs(vxs);
                }
                else
                {
                    //not scalue
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Rotate, DegToRad(xaxisRotationAngleDec)),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    vxs = mat.TransformToVxs(vxs);
                }
            }
            else
            {
                //no rotate
                if (centerFormArc.scaleUp)
                {
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Scale, scaleRatio, scaleRatio),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    vxs = mat.TransformToVxs(vxs);
                }
            }

            vxs = aggStroke.MakeVxs(vxs);
            FillVxsSnap(this.strokeColor, new VertexStoreSnap(vxs));
        }

        struct CenterFormArc
        {
            public double cx;
            public double cy;
            public double radStartAngle;
            public double radSweepDiff;
            public bool scaleUp;
        }

        static void ComputeArc2(double x0, double y0,
                             double rx, double ry,
                             double xAngleRad,
                             bool largeArcFlag,
                             bool sweepFlag,
                             double x, double y, ref CenterFormArc result)
        {
            //from  SVG1.1 spec
            //----------------------------------
            //step1: Compute (x1dash,y1dash)
            //----------------------------------

            double dx2 = (x0 - x) / 2.0;
            double dy2 = (y0 - y) / 2.0;
            double cosAngle = Math.Cos(xAngleRad);
            double sinAngle = Math.Sin(xAngleRad);
            double x1 = (cosAngle * dx2 + sinAngle * dy2);
            double y1 = (-sinAngle * dx2 + cosAngle * dy2);
            // Ensure radii are large enough
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);
            double prx = rx * rx;
            double pry = ry * ry;
            double px1 = x1 * x1;
            double py1 = y1 * y1;
            // check that radii are large enough


            double radiiCheck = px1 / prx + py1 / pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
                prx = rx * rx;
                pry = ry * ry;
                result.scaleUp = true;
            }

            //----------------------------------
            //step2: Compute (cx1,cy1)
            //----------------------------------
            double sign = (largeArcFlag == sweepFlag) ? -1 : 1;
            double sq = ((prx * pry) - (prx * py1) - (pry * px1)) / ((prx * py1) + (pry * px1));
            sq = (sq < 0) ? 0 : sq;
            double coef = (sign * Math.Sqrt(sq));
            double cx1 = coef * ((rx * y1) / ry);
            double cy1 = coef * -((ry * x1) / rx);
            //----------------------------------
            //step3:  Compute (cx, cy) from (cx1, cy1)
            //----------------------------------
            double sx2 = (x0 + x) / 2.0;
            double sy2 = (y0 + y) / 2.0;
            double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);
            //----------------------------------
            //step4: Compute theta and anfkediff
            double ux = (x1 - cx1) / rx;
            double uy = (y1 - cy1) / ry;
            double vx = (-x1 - cx1) / rx;
            double vy = (-y1 - cy1) / ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt((ux * ux) + (uy * uy));
            p = ux; // (1 * ux) + (0 * uy)
            sign = (uy < 0) ? -1d : 1d;
            double angleStart = (sign * Math.Acos(p / n));  // Math.toDegrees(sign * Math.Acos(p / n));
            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
            double angleExtent = (sign * Math.Acos(p / n));// Math.toDegrees(sign * Math.Acos(p / n));
            //if (!sweepFlag && angleExtent > 0)
            //{
            //    angleExtent -= 360f;
            //}
            //else if (sweepFlag && angleExtent < 0)
            //{
            //    angleExtent += 360f;
            //}

            result.cx = cx;
            result.cy = cy;
            result.radStartAngle = angleStart;
            result.radSweepDiff = angleExtent;
        }
        static Arc ComputeArc(double x0, double y0,
                              double rx, double ry,
                              double angle,
                              bool largeArcFlag,
                              bool sweepFlag,
                               double x, double y)
        {
            /** 
         * This constructs an unrotated Arc2D from the SVG specification of an 
         * Elliptical arc.  To get the final arc you need to apply a rotation
         * transform such as:
         * 
         * AffineTransform.getRotateInstance
         *     (angle, arc.getX()+arc.getWidth()/2, arc.getY()+arc.getHeight()/2);
         */
            //
            // Elliptical arc implementation based on the SVG specification notes
            //

            // Compute the half distance between the current and the final point
            double dx2 = (x0 - x) / 2.0;
            double dy2 = (y0 - y) / 2.0;
            // Convert angle from degrees to radians
            angle = ((angle % 360.0) * Math.PI / 180f);
            double cosAngle = Math.Cos(angle);
            double sinAngle = Math.Sin(angle);
            //
            // Step 1 : Compute (x1, y1)
            //
            double x1 = (cosAngle * dx2 + sinAngle * dy2);
            double y1 = (-sinAngle * dx2 + cosAngle * dy2);
            // Ensure radii are large enough
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);
            double Prx = rx * rx;
            double Pry = ry * ry;
            double Px1 = x1 * x1;
            double Py1 = y1 * y1;
            // check that radii are large enough
            double radiiCheck = Px1 / Prx + Py1 / Pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
                Prx = rx * rx;
                Pry = ry * ry;
            }

            //
            // Step 2 : Compute (cx1, cy1)
            //
            double sign = (largeArcFlag == sweepFlag) ? -1 : 1;
            double sq = ((Prx * Pry) - (Prx * Py1) - (Pry * Px1)) / ((Prx * Py1) + (Pry * Px1));
            sq = (sq < 0) ? 0 : sq;
            double coef = (sign * Math.Sqrt(sq));
            double cx1 = coef * ((rx * y1) / ry);
            double cy1 = coef * -((ry * x1) / rx);
            //
            // Step 3 : Compute (cx, cy) from (cx1, cy1)
            //
            double sx2 = (x0 + x) / 2.0;
            double sy2 = (y0 + y) / 2.0;
            double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);
            //
            // Step 4 : Compute the angleStart (angle1) and the angleExtent (dangle)
            //
            double ux = (x1 - cx1) / rx;
            double uy = (y1 - cy1) / ry;
            double vx = (-x1 - cx1) / rx;
            double vy = (-y1 - cy1) / ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt((ux * ux) + (uy * uy));
            p = ux; // (1 * ux) + (0 * uy)
            sign = (uy < 0) ? -1d : 1d;
            double angleStart = (sign * Math.Acos(p / n));  // Math.toDegrees(sign * Math.Acos(p / n));
            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
            double angleExtent = (sign * Math.Acos(p / n));// Math.toDegrees(sign * Math.Acos(p / n));
            if (!sweepFlag && angleExtent > 0)
            {
                angleExtent -= 360f;
            }
            else if (sweepFlag && angleExtent < 0)
            {
                angleExtent += 360f;
            }
            //angleExtent %= 360f;
            //angleStart %= 360f;

            //
            // We can now build the resulting Arc2D in double precision
            //
            //Arc2D.Double arc = new Arc2D.Double();
            //arc.x = cx - rx;
            //arc.y = cy - ry;
            //arc.width = rx * 2.0;
            //arc.height = ry * 2.0;
            //arc.start = -angleStart;
            //arc.extent = -angleExtent;
            Arc arc = new Arc();
            arc.Init(x, y, rx, ry, -(angleStart), -(angleExtent));
            return arc;
        }
        public void DrawBezierCurve(float startX, float startY, float endX, float endY,
            float controlX1, float controlY1,
            float controlX2, float controlY2)
        {
            VertexStore vxs = new VertexStore();
            BezierCurve.CreateBezierVxs4(vxs,
                new PixelFarm.VectorMath.Vector2(startX, startY),
                new PixelFarm.VectorMath.Vector2(endX, endY),
                new PixelFarm.VectorMath.Vector2(controlX1, controlY1),
                new PixelFarm.VectorMath.Vector2(controlY2, controlY2));
            vxs = this.aggStroke.MakeVxs(vxs);
            DrawVxsSnap(this.strokeColor, new VertexStoreSnap(vxs));
        }

        public void FillRect(PixelFarm.Drawing.Color color, float x, float y, float w, float h)
        {
            float[] coords = CreateRectTessCoordsTriStrip(x, y, w, h);
            basicFillShader.FillTriangleStripWithVertexBuffer(coords, 4, color);
        }

        public void FillRoundRect(PixelFarm.Drawing.Color color, float x, float y, float w, float h, float rx, float ry)
        {
            roundRect.SetRect(x, y, x + w, y + h);
            roundRect.SetRadius(rx, ry);
            //create round rect vxs
            var vxs = roundRect.MakeVxs();
            DrawVxsSnap(color, new VertexStoreSnap(vxs));
        }
        public void FillEllipse(PixelFarm.Drawing.Color color, float x, float y, float rx, float ry)
        {
            switch (this.SmoothMode)
            {
                default:
                    {
                        ellipse.Reset(x, y, rx, ry);
                        var vxs = ellipse.MakeVxs();
                        //other mode
                        int n = vxs.Count;
                        //make triangular fan*** 
                        unsafe
                        {
                            float* coords = stackalloc float[(n * 2) + 4];
                            int i = 0;
                            int nn = 0;
                            int npoints = 0;
                            double vx, vy;
                            //center
                            coords[nn++] = (float)x;
                            coords[nn++] = (float)y;
                            npoints++;
                            var cmd = vxs.GetVertex(i, out vx, out vy);
                            while (i < n)
                            {
                                switch (cmd)
                                {
                                    case VertexCmd.MoveTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                            npoints++;
                                        }
                                        break;
                                    case VertexCmd.LineTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                            npoints++;
                                        }
                                        break;
                                    case VertexCmd.Stop:
                                        {
                                        }
                                        break;
                                    default:
                                        {
                                        }
                                        break;
                                }
                                i++;
                                cmd = vxs.GetVertex(i, out vx, out vy);
                            }
                            //close circle
                            coords[nn++] = coords[2];
                            coords[nn++] = coords[3];
                            npoints++;
                            this.basicFillShader.FillTriangleFan(coords, npoints, color);
                        }
                    }
                    break;
            }
        }
        public void FillCircle(PixelFarm.Drawing.Color color, float x, float y, float radius)
        {
            FillEllipse(color, x, y, radius, radius);
        }

        public void FillPolygon(PixelFarm.Drawing.Color color, float[] vertex2dCoords)
        {
            FillPolygon(color, vertex2dCoords, vertex2dCoords.Length);
        }
        public void FillPolygon(PixelFarm.Drawing.Brush brush, float[] vertex2dCoords, int npoints)
        {
            //-------------
            //Tesselate
            //2d coods lis
            //n point  
            //-----------------------------   
            //switch how to fill polygon
            switch (brush.BrushKind)
            {
                case Drawing.BrushKind.Solid:
                    {
                        var solidBrush = brush as PixelFarm.Drawing.SolidBrush;
                        FillPolygon(solidBrush.Color, vertex2dCoords);
                    }
                    break;
                case Drawing.BrushKind.LinearGradient:
                case Drawing.BrushKind.Texture:
                    {
                        List<Vertex> vertextList = TessPolygon(vertex2dCoords);
                        var linearGradientBrush = brush as PixelFarm.Drawing.LinearGradientBrush;
                        GL.ClearStencil(0); //set value for clearing stencil buffer 
                                            //actual clear here
                        GL.Clear(ClearBufferMask.StencilBufferBit);
                        //-------------------
                        //disable rendering to color buffer
                        GL.ColorMask(false, false, false, false);
                        //start using stencil
                        GL.Enable(EnableCap.StencilTest);
                        //place a 1 where rendered
                        GL.StencilFunc(StencilFunction.Always, 1, 1);
                        //replace where rendered
                        GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
                        //render  to stencill buffer
                        //-----------------

                        //switch how to fill polygon ***
                        int j = vertextList.Count;
                        float[] vtx = new float[j * 2];
                        int n = 0;
                        for (int i = 0; i < j; ++i)
                        {
                            var v = vertextList[i];
                            vtx[n] = (float)v.m_X;
                            vtx[n + 1] = (float)v.m_Y;
                            n += 2;
                        }
                        //-------------------------------------   
                        this.basicFillShader.FillTriangles(vtx, j, PixelFarm.Drawing.Color.Black);
                        //-------------------------------------- 
                        //render color
                        //--------------------------------------  
                        //reenable color buffer 
                        GL.ColorMask(true, true, true, true);
                        //where a 1 was not rendered
                        GL.StencilFunc(StencilFunction.Equal, 1, 1);
                        //freeze stencill buffer
                        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
                        //------------------------------------------
                        //we already have valid ps from stencil step
                        //------------------------------------------

                        //-------------------------------------------------------------------------------------
                        //1.  we draw only alpha chanel of this black color to destination color
                        //so we use  BlendFuncSeparate  as follow ... 
                        //-------------------------------------------------------------------------------------
                        //1.  we draw only alpha channel of this black color to destination color
                        //so we use  BlendFuncSeparate  as follow ... 
                        GL.ColorMask(false, false, false, true);
                        //GL.BlendFuncSeparate(
                        //     BlendingFactorSrc.DstColor, BlendingFactorDest.DstColor, //the same
                        //     BlendingFactorSrc.One, BlendingFactorDest.Zero);

                        //use alpha chanel from source***
                        GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.Zero);
                        invertAlphaFragmentShader.OrthoView = orthoView;
                        invertAlphaFragmentShader.StrokeColor = PixelFarm.Drawing.Color.Black;
                        invertAlphaFragmentShader.DrawPolygon(vertex2dCoords, vertex2dCoords.Length);
                        //at this point alpha component is fill in to destination 
                        //-------------------------------------------------------------------------------------
                        //2. then fill again!, 
                        //we use alpha information from dest, 
                        //so we set blend func to ... GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha)    
                        GL.ColorMask(true, true, true, true);
                        GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha);
                        {
                            //draw box*** of gradient color
                            switch (brush.BrushKind)
                            {
                                case Drawing.BrushKind.LinearGradient:
                                    {
                                        var colors = linearGradientBrush.GetColors();
                                        var points = linearGradientBrush.GetStopPoints();
                                        float[] v2f, color4f;
                                        GLGradientColorProvider.CalculateLinearGradientVxs2(
                                             points[0].X, points[0].Y,
                                             points[1].X, points[1].Y,
                                             colors[0],
                                             colors[1], out v2f, out color4f);
                                        rectFillShader.OrthoView = orthoView;
                                        rectFillShader.Render(v2f, color4f);
                                    }
                                    break;
                                case Drawing.BrushKind.Texture:
                                    {
                                        //draw texture image ***
                                        PixelFarm.Drawing.TextureBrush tbrush = (PixelFarm.Drawing.TextureBrush)brush;
                                        GLImage img = tbrush.TextureImage as GLImage;
                                        GLBitmap bmpTexture = (GLBitmap)img.InnerImage;
                                        //TODO: review here 
                                        //where text start?
                                        this.DrawImage(bmpTexture, 0, 300);
                                    }
                                    break;
                            }
                        }
                        //restore back 
                        //3. switch to normal blending mode 
                        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                        GL.Disable(EnableCap.StencilTest);
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
        }
        public void FillPolygon(PixelFarm.Drawing.Color color, float[] vertex2dCoords, int npoints)
        {
            //-------------
            //Tesselate
            //2d coods lis
            //n point 
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        var vertextList = TessPolygon(vertex2dCoords);
                        //-----------------------------   
                        //switch how to fill polygon
                        int j = vertextList.Count;
                        //-----------------------------    
                        unsafe
                        {
                            float* vtx = stackalloc float[j * 2];
                            int n = 0;
                            for (int i = 0; i < j; ++i)
                            {
                                var v = vertextList[i];
                                vtx[n] = (float)v.m_X;
                                vtx[n + 1] = (float)v.m_Y;
                                n += 2;
                            }
                            //-------------------------------------                              
                            this.basicFillShader.FillTriangles(vtx, j, color);
                        }
                    }
                    break;
                default:
                    {
                        var vertextList = TessPolygon(vertex2dCoords);
                        //-----------------------------   
                        //switch how to fill polygon
                        int j = vertextList.Count;
                        //-----------------------------  
                        unsafe
                        {
                            float* vtx = stackalloc float[j * 2];
                            int n = 0;
                            for (int i = 0; i < j; ++i)
                            {
                                var v = vertextList[i];
                                vtx[n] = (float)v.m_X;
                                vtx[n + 1] = (float)v.m_Y;
                                n += 2;
                            }
                            //------------------------------------- 
                            this.basicFillShader.FillTriangles(vtx, j, color);
                        }
                    }
                    break;
            }
        }
        //-----------------------------------------------------


        public int CanvasOriginX
        {
            get { return this.canvasOriginX; }
        }
        public int CanvasOriginY
        {
            get { return this.canvasOriginY; }
        }

        public void SetCanvasOrigin(int x, int y)
        {
            //int originalW = 800;
            //set new viewport
            GL.Viewport(x, y, canvasW, canvasH);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, originalW, 0, originalW, 0.0, 100.0);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();
        }
        public void EnableClipRect()
        {
            GL.Enable(EnableCap.ScissorTest);
        }
        public void DisableClipRect()
        {
            GL.Disable(EnableCap.ScissorTest);
        }
        public void SetClipRect(int x, int y, int w, int h)
        {
            GL.Scissor(x, y, w, h);
        }
    }
}