//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using OpenTK.Graphics.OpenGL;
using Tesselate;

using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;

namespace OpenTkEssTest
{

    public partial class CanvasGL2d
    {
        LayoutFarm.Drawing.Color fillColor = LayoutFarm.Drawing.Color.Black;

        Tesselator tess = new Tesselator();
        TessListener2 tessListener = new TessListener2();
        //tools---------------------------------
        RoundedRect roundRect = new RoundedRect();
        Ellipse ellipse = new Ellipse();
        PathStore ps = new PathStore();
        Stroke stroke1 = new Stroke(1);
        GLScanlineRasterizer sclineRas;
        GLScanlineRasToDestBitmapRenderer sclineRasToGL;
        GLScanlinePacked8 sclinePack8;
        Arc arcTool = new Arc();

        public CanvasGL2d()
        {
            sclineRas = new GLScanlineRasterizer();
            sclineRasToGL = new GLScanlineRasToDestBitmapRenderer();
            sclinePack8 = new GLScanlinePacked8();
            tessListener.Connect(tess, Tesselate.Tesselator.WindingRuleType.Odd, true);

            SetupFonts();
        }
        public CanvasSmoothMode SmoothMode
        {
            get;
            set;
        }

        public void Clear(LayoutFarm.Drawing.Color c)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.AccumBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(c);
        }
        public double StrokeWidth
        {
            get { return this.stroke1.Width; }
            set { this.stroke1.Width = value; }
        }


        public void DrawLine(float x1, float y1, float x2, float y2)
        {

            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        //--------------------------------------
                        ps.Clear();
                        ps.MoveTo(x1, y1);
                        ps.LineTo(x2, y2);
                        VertexStore vxs = stroke1.MakeVxs(ps.Vxs);
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);
                        //--------------------------------------
                    } break;
                default:
                    {
                        unsafe
                        {
                            float* arr = stackalloc float[4];
                            arr[0] = x1; arr[1] = y1;
                            arr[2] = x2; arr[3] = y2;
                            //byte* indices = stackalloc byte[2];
                            //indices[0] = 0; indices[1] = 1;

                            GL.EnableClientState(ArrayCap.VertexArray); //***
                            //vertex
                            GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                            //GL.DrawElements(BeginMode.Lines, 2, DrawElementsType.UnsignedByte, (IntPtr)indices);
                            GL.DrawArrays(BeginMode.Lines, 0, 2);
                            GL.DisableClientState(ArrayCap.VertexArray);
                        }
                    } break;

            }
        }


        public void DrawImage(GLBitmapTexture bmp, float x, float y)
        {
            unsafe
            {

                GL.Enable(EnableCap.Texture2D);
                {
                    GL.BindTexture(TextureTarget.Texture2D, bmp.TextureId);
                    GL.EnableClientState(ArrayCap.TextureCoordArray); //***

                    float* arr = stackalloc float[8];
                    arr[0] = 0; arr[1] = 1;
                    arr[2] = 1; arr[3] = 1;
                    arr[4] = 1; arr[5] = 0;
                    arr[6] = 0; arr[7] = 0;

                    byte* indices = stackalloc byte[6];
                    indices[0] = 0; indices[1] = 1; indices[2] = 2;
                    indices[3] = 2; indices[4] = 3; indices[5] = 0;

                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                    //------------------------------------------ 
                    //fill rect with texture
                    FillRect(x, y, bmp.Width, bmp.Height);
                    GL.DisableClientState(ArrayCap.TextureCoordArray);

                } GL.Disable(EnableCap.Texture2D);
            }
        }
        public void DrawImage(GLBitmapTexture bmp, float x, float y, float w, float h)
        {
            unsafe
            {

                GL.Enable(EnableCap.Texture2D);
                {
                    GL.BindTexture(TextureTarget.Texture2D, bmp.TextureId);
                    GL.EnableClientState(ArrayCap.TextureCoordArray); //***

                    float* arr = stackalloc float[8];
                    arr[0] = 0; arr[1] = 1;
                    arr[2] = 1; arr[3] = 1;
                    arr[4] = 1; arr[5] = 0;
                    arr[6] = 0; arr[7] = 0;

                    byte* indices = stackalloc byte[6];
                    indices[0] = 0; indices[1] = 1; indices[2] = 2;
                    indices[3] = 2; indices[4] = 3; indices[5] = 0;

                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                    //------------------------------------------ 
                    //fill rect with texture
                    FillRect(x, y, w, h);
                    GL.DisableClientState(ArrayCap.TextureCoordArray);

                } GL.Disable(EnableCap.Texture2D);
            }
        }

        public void FillVxs(VertexStore vxs)
        {
            sclineRas.Reset();
            sclineRas.AddPath(vxs);
            sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);
        }
        public void DrawVxs(VertexStore vxs)
        {
          
            sclineRas.Reset();
            sclineRas.AddPath(stroke1.MakeVxs(vxs));
            sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);
        }
        public void DrawPolygon(float[] polygon2dVertices, int npoints)
        {
            //closed polyline
            //draw polyline
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        //draw polyon

                        ps.Clear();
                        //closed polygon
                        int j = npoints / 2;
                        //first point
                        if (j < 2)
                        {
                            return;
                        }
                        ps.MoveTo(polygon2dVertices[0], polygon2dVertices[1]);
                        int nn = 2;
                        for (int i = 1; i < j; ++i)
                        {
                            ps.LineTo(polygon2dVertices[nn++],
                                polygon2dVertices[nn++]);
                        }
                        //close
                        ps.ClosePolygon();

                        VertexStore vxs = stroke1.MakeVxs(ps.Vxs);
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);
                        //--------------------------------------


                    } break;
                default:
                    {
                        unsafe
                        {
                            fixed (float* arr = &polygon2dVertices[0])
                            {
                                DrawPolygonUnsafe(arr, npoints);
                            }
                        }
                    } break;

            }

        }

        public void DrawEllipse(float x, float y, double rx, double ry)
        {

            ellipse.Reset(x, y, rx, ry);

            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {

                        VertexStore vxs = stroke1.MakeVxs(ellipse.MakeVxs());
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);

                    } break;
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
                                    case ShapePath.CmdAndFlags.MoveTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                        } break;
                                    case ShapePath.CmdAndFlags.LineTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                        } break;
                                    case ShapePath.CmdAndFlags.Empty:
                                        {
                                        } break;
                                    default:
                                        {

                                        } break;
                                }
                                i++;
                                cmd = vxs.GetVertex(i, out vx, out vy);
                            }
                            //-------------------------------------- 
                            DrawPolygonUnsafe(coords, nn / 2);

                        }
                    } break;
            }

        }
        public void DrawCircle(float x, float y, double radius)
        {

            DrawEllipse(x, y, radius, radius);
        }

        public void DrawRect(float x, float y, float w, float h)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        unsafe
                        {
                            float* arr = stackalloc float[8];
                            byte* indices = stackalloc byte[6];
                            CreateRectCoords(arr, indices, x, y, w, h);
                            GL.EnableClientState(ArrayCap.VertexArray); //***
                            //vertex
                            GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedByte, (IntPtr)indices);
                            GL.DisableClientState(ArrayCap.VertexArray);
                        }
                    } break;
                default:
                    {
                        unsafe
                        {
                            float* arr = stackalloc float[8];
                            byte* indices = stackalloc byte[6];
                            CreateRectCoords(arr, indices, x, y, w, h);
                            GL.EnableClientState(ArrayCap.VertexArray); //***
                            //vertex
                            GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedByte, (IntPtr)indices);
                            GL.DisableClientState(ArrayCap.VertexArray);
                        }
                    } break;
            }
        }
        public void DrawRoundRect(float x, float y, float w, float h, float rx, float ry)
        {
            roundRect.SetRect(x, y, x + w, y + h);
            roundRect.SetRadius(rx, ry);
            var vxs = this.stroke1.MakeVxs(roundRect.MakeVxs());

            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);
                    } break;
                default:
                    {

                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);
                    } break;
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

                    case ShapePath.CmdAndFlags.Empty:
                        stopLoop = true;
                        break;
                    default:
                        vxs.AddVertex(vertexData);
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

            vxs = stroke1.MakeVxs(vxs);
            sclineRas.Reset();
            sclineRas.AddPath(vxs);
            sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);
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

            vxs = this.stroke1.MakeVxs(vxs);

            sclineRas.Reset();
            sclineRas.AddPath(vxs);
            sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);

        }
        //==================================================================================
        public LayoutFarm.Drawing.Color FillColor
        {
            get
            {
                return this.fillColor;
            }
            set
            {
                this.fillColor = value;
                GL.Color4(value);
            }
        }
        public void FillRect(float x, float y, float w, float h)
        {
            //2d
            unsafe
            {
                float* arr = stackalloc float[8];
                byte* indices = stackalloc byte[6];
                CreateRectCoords(arr, indices, x, y, w, h);
                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedByte, (IntPtr)indices);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
        }
        public void FillRoundRect(float x, float y, float w, float h, float rx, float ry)
        {
            roundRect.SetRect(x, y, x + w, y + h);
            roundRect.SetRadius(rx, ry);
            var vxs = roundRect.MakeVxs();

            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {

                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, this.fillColor);

                    } break;
                default:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);


                    } break;
            }

        }
        public void FillEllipse(float x, float y, double rx, double ry)
        {
            ellipse.Reset(x, y, rx, ry);
            VertexStore vxs = ellipse.MakeVxs();
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, this.fillColor);
                        return;
                    }
            }

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
                        case ShapePath.CmdAndFlags.MoveTo:
                            {
                                coords[nn++] = (float)vx;
                                coords[nn++] = (float)vy;
                                npoints++;
                            } break;
                        case ShapePath.CmdAndFlags.LineTo:
                            {
                                coords[nn++] = (float)vx;
                                coords[nn++] = (float)vy;
                                npoints++;
                            } break;
                        case ShapePath.CmdAndFlags.Empty:
                            {
                            } break;
                        default:
                            {

                            } break;
                    }
                    i++;
                    cmd = vxs.GetVertex(i, out vx, out vy);
                }

                //close circle
                coords[nn++] = coords[2];
                coords[nn++] = coords[3];
                npoints++;

                //int* indx = stackalloc int[npoints];
                //for (i = 0; i < npoints; ++i)
                //{
                //    indx[i] = i;
                //}

                //fill triangular fan
                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex 2d
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)coords);
                //GL.DrawElements(BeginMode.TriangleFan, npoints, DrawElementsType.UnsignedInt, (IntPtr)indx);
                GL.DrawArrays(BeginMode.TriangleFan, 0, npoints);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
        }
        public void FillCircle(float x, float y, double radius)
        {
            FillEllipse(x, y, radius, radius);
        }

        public void FillPolygon(float[] vertex2dCoords)
        {
            FillPolygon(vertex2dCoords, vertex2dCoords.Length);
        }
        public void FillPolygon(float[] vertex2dCoords, int npoints)
        {
            //-------------
            //Tesselate
            //2d coods lis
            //n point 
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        //closed polygon

                        //closed polygon
                        int j = npoints / 2;
                        //first point
                        if (j < 2)
                        {
                            return;
                        }
                        ps.MoveTo(vertex2dCoords[0], vertex2dCoords[1]);
                        int nn = 2;
                        for (int i = 1; i < j; ++i)
                        {
                            ps.LineTo(vertex2dCoords[nn++],
                                vertex2dCoords[nn++]);
                        }
                        //close
                        ps.ClosePolygon();
                        VertexStore vxs = ps.Vxs;
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, this.fillColor);
                        //-------------------------------------- 

                    } break;
                default:
                    {
                        var vertextList = TessPolygon(vertex2dCoords);
                        //-----------------------------
                        FillTriangles(vertextList);
                        //-----------------------------
                    } break;
            }

        }
        //-----------------------------------------------------

    }
}