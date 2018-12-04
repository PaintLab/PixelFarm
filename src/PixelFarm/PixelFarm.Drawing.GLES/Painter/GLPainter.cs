//MIT, 2016-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.DrawingGL
{
    public sealed class GLPainter : Painter
    {
        GLRenderSurface _glsx;
        SmoothingMode _smoothingMode; //smoothing mode of this  painter

        int _width;
        int _height;

        Color _fillColor;
        Color _strokeColor;
        RectInt _clipBox;


        InternalGraphicsPathBuilder _igfxPathBuilder;

        //agg's vertex generators
        Stroke _aggStroke = new Stroke(1);
        Ellipse ellipse = new Ellipse();

        Arc _arcTool;

        //fonts
        RequestFont _requestFont;
        ITextPrinter _textPrinter;
        RenderQuality _renderQuality;



        public GLPainter(GLRenderSurface glsx)
        {
            _glsx = glsx;
            _width = glsx.CanvasWidth;
            _height = glsx.CanvasHeight;

            _clipBox = new RectInt(0, 0, _width, _height);
            _arcTool = new Arc();
            CurrentFont = new RequestFont("tahoma", 14);
            UseVertexBufferObjectForRenderVx = true;
            //tools
            _igfxPathBuilder = InternalGraphicsPathBuilder.CreateNew();
        }
        public override void SetClipRgn(VertexStore vxs)
        {

#if DEBUG
            System.Diagnostics.Debug.WriteLine("please impl GLPainter: SetClipRgn");
#endif
            //throw new NotImplementedException();
        }
        public override void Render(RenderVx renderVx)
        {
            throw new NotImplementedException();
        }
        public void DetachCurrentShader() => _glsx.DetachCurrentShader();

        public Color FontFillColor
        {
            get => _glsx.FontFillColor;
            set => _glsx.FontFillColor = value;
        }


        RenderSurfaceOrientation _orientation = RenderSurfaceOrientation.LeftTop;
        public override RenderSurfaceOrientation Orientation
        {
            get => _orientation;
            set => _orientation = value;
        }

        public bool UseVertexBufferObjectForRenderVx { get; set; }

        public override void SetOrigin(float ox, float oy)
        {
            _glsx.SetCanvasOrigin((int)ox, (int)oy);
        }
        //
        public GLRenderSurface Canvas => _glsx;
        //
        public override RenderQuality RenderQuality
        {
            get => _renderQuality;
            set => _renderQuality = value;
        }
        public override RequestFont CurrentFont
        {
            get => _requestFont;
            set
            {
                _requestFont = value;
                if (_textPrinter != null)
                {
                    _textPrinter.ChangeFont(value);
                }
            }
        }
        public override RectInt ClipBox
        {
            get => _clipBox;
            set => _clipBox = value;
        }
        public override SmoothingMode SmoothingMode
        {
            get => _smoothingMode;
            set
            {
                switch (_smoothingMode = value)
                {
                    case SmoothingMode.HighQuality:
                    case SmoothingMode.AntiAlias:
                        _glsx.SmoothMode = SmoothMode.Smooth;
                        break;
                    default:
                        _glsx.SmoothMode = SmoothMode.No;
                        break;
                }

            }
        }
        public ITextPrinter TextPrinter
        {
            get => _textPrinter;
            set
            {
                _textPrinter = value;
                if (value != null && _requestFont != null)
                {
                    _textPrinter.ChangeFont(_requestFont);
                }
            }
        }
        public override Color FillColor
        {
            get => _fillColor;
            set => _fillColor = value;
        }
        Brush _currentBrush;
        public override Brush CurrentBrush
        {
            get => _currentBrush;
            set => _currentBrush = value;
        }

        Pen _currentPen;
        public override Pen CurrentPen
        {
            get => _currentPen;
            set => _currentPen = value;
        }

        public override int Width => _width;
        public override int Height => _height;

        public override Color StrokeColor
        {
            get => _strokeColor;
            set
            {
                _strokeColor = value;
                _glsx.StrokeColor = value;
            }
        }

        public override double StrokeWidth
        {
            get => _glsx.StrokeWidth;
            set
            {
                _glsx.StrokeWidth = (float)value;
                _aggStroke.Width = (float)value;
            }
        }

        public override bool UseSubPixelLcdEffect
        {
            get => _glsx.SmoothMode == SmoothMode.Smooth;
            set => _glsx.SmoothMode = value ? SmoothMode.Smooth : SmoothMode.No;
        }

        public override void Clear(Color color)
        {
            _glsx.Clear(color);
        }
        public override void ApplyFilter(ImageFilter imgFilter)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// we do NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        public override void Draw(VertexStore vxs)
        {
            if (StrokeWidth > 1)
            {
                using (VxsTemp.Borrow(out VertexStore v1))
                using (VectorToolBox.Borrow(out Stroke st))
                {
                    //convert large stroke to vxs
                    st.Width = StrokeWidth;
                    st.MakeVxs(vxs, v1);

                    Color prevColor = this.FillColor;
                    FillColor = this.StrokeColor;
                    Fill(v1);
                    ////-----------------------------------------------
                    //InternalGraphicsPath pp = _igfxPathBuilder.CreateGraphicsPath(v1);
                    //_glsx.FillGfxPath(
                    //    _fillColor, pp
                    //);
                    //-----------------------------------------------
                    FillColor = prevColor;
                }
            }
            else
            {
                _glsx.DrawGfxPath(_strokeColor,
                    _igfxPathBuilder.CreateGraphicsPath(vxs));
            }
        }



        public override void DrawImage(Image actualImage, params AffinePlan[] affinePlans)
        {
            //create gl bmp
            //TODO: affinePlans***
            GLBitmap glBmp = _glsx.ResolveForGLBitmap(actualImage);
            if (glBmp != null)
            {
                _glsx.DrawImage(glBmp, 0, 0);
            }
        }
        public override void DrawImage(Image actualImage, double left, double top, ICoordTransformer coordTx)
        {
            //TODO: implement transformation matrix
            GLBitmap glBmp = _glsx.ResolveForGLBitmap(actualImage);
            if (glBmp != null)
            {
                if (this.OriginX != 0 || this.OriginY != 0)
                {
                    //TODO: review here
                }

                //  coordTx = aff = aff * Affine.NewTranslation(this.OriginX, this.OriginY);

                Affine aff = coordTx as Affine;
                if (aff != null)
                {
                    _glsx.DrawImageToQuad(glBmp, aff);
                }
                else
                {

                }

                //_glsx.DrawImage(glBmp, (float)left, (float)top);
            }
        }
        //public override void DrawImage(Image actualImage, double left, double top, ICoordTransformer coordTx)
        //{
        //    //draw img with transform coord
        //    //
        //    MemBitmap memBmp = actualImage as MemBitmap;
        //    if (memBmp == null)
        //    {
        //        //? TODO
        //        return;
        //    }

        //    if (_renderQuality == RenderQuality.Fast)
        //    {
        //        //todo, review here again
        //        //TempMemPtr tmp = ActualBitmap.GetBufferPtr(actualImg);
        //        //BitmapBuffer srcBmp = new BitmapBuffer(actualImage.Width, actualImage.Height, tmp.Ptr, tmp.LengthInBytes); 
        //        //_bxt.BlitRender(srcBmp, false, 1, new BitmapBufferEx.MatrixTransform(affinePlans));

        //        //if (affinePlans != null && affinePlans.Length > 0)
        //        //{
        //        //    _bxt.BlitRender(srcBmp, false, 1, new BitmapBufferEx.MatrixTransform(affinePlans));
        //        //}
        //        //else
        //        //{
        //        //    //_bxt.BlitRender(srcBmp, false, 1, null);
        //        //    _bxt.Blit(0, 0, srcBmp.PixelWidth, srcBmp.PixelHeight, srcBmp, 0, 0, srcBmp.PixelWidth, srcBmp.PixelHeight,
        //        //        ColorInt.FromArgb(255, 255, 255, 255),
        //        //        BitmapBufferExtensions.BlendMode.Alpha);
        //        //}
        //        return;
        //    }

        //    bool useSubPix = UseSubPixelLcdEffect; //save, restore later... 
        //                                           //before render an image we turn off vxs subpixel rendering
        //    this.UseSubPixelLcdEffect = false;

        //    if (coordTx is Affine)
        //    {
        //        Affine aff = (Affine)coordTx;
        //        if (this.OriginX != 0 || this.OriginY != 0)
        //        {
        //            coordTx = aff = aff * Affine.NewTranslation(this.OriginX, this.OriginY);
        //        }
        //    }

        //    //_aggsx.SetScanlineRasOrigin(OriginX, OriginY);

        //    _aggsx.Render(memBmp, coordTx);

        //    //_aggsx.SetScanlineRasOrigin(xx, yy);
        //    //restore...
        //    this.UseSubPixelLcdEffect = useSubPix;
        //}
        public override void DrawImage(Image actualImage)
        {
            GLBitmap glBmp = _glsx.ResolveForGLBitmap(actualImage);
            if (glBmp == null) return;
            _glsx.DrawImage(glBmp, 0, 0);
        }
        public override void DrawImage(Image actualImage, double left, double top)
        {
            GLBitmap glBmp = _glsx.ResolveForGLBitmap(actualImage);
            if (glBmp == null) return;
            _glsx.DrawImage(glBmp, (float)left, (float)top);
        }
        public override void DrawImage(Image actualImage, double left, double top, int srcX, int srcY, int srcW, int srcH)
        {
            throw new NotImplementedException();
        }

        public override void FillRect(double left, double top, double width, double height)
        {
            _glsx.FillRect(_fillColor, left, top, width, height);
        }
        public override void DrawEllipse(double left, double top, double width, double height)
        {


            double x = (left + width / 2);
            double y = (top + height / 2);
            double rx = Math.Abs(width / 2);
            double ry = Math.Abs(height / 2);
            ellipse.Set(x, y, rx, ry);

            using (VxsTemp.Borrow(out var v1, out var v2))
            {
                ellipse.MakeVxs(v1);
                _aggStroke.MakeVxs(v1, v2);
                //***
                //we fill the stroke's path
                _glsx.FillGfxPath(_strokeColor, _igfxPathBuilder.CreateGraphicsPath(v2));
            }

        }
        public override void FillEllipse(double left, double top, double width, double height)
        {
            //version 2:
            //agg's ellipse tools with smooth border

            double x = (left + width / 2);
            double y = (top + height / 2);
            double rx = Math.Abs(width / 2);
            double ry = Math.Abs(height / 2);
            ellipse.Set(x, y, rx, ry);
            using (VxsTemp.Borrow(out var vxs))
            {
                ellipse.MakeVxs(vxs);
                //***
                //we fill  
                _glsx.FillGfxPath(_strokeColor, _igfxPathBuilder.CreateGraphicsPath(vxs));
            }

        }

        public override void DrawRect(double left, double top, double width, double height)
        {
            switch (_glsx.SmoothMode)
            {
                case SmoothMode.Smooth:
                    {
                        _glsx.StrokeColor = this.StrokeColor;
                        using (PixelFarm.Drawing.VxsTemp.Borrow(out Drawing.VertexStore v1))
                        using (PixelFarm.Drawing.VectorToolBox.Borrow(out CpuBlit.VertexProcessing.SimpleRect r))
                        {

                            r.SetRect(left + 0.5f, top + height + 0.5f, left + width - 0.5f, top - 0.5f);
                            r.MakeVxs(v1);
                            //create stroke around 
                            RenderVx renderVX = CreateRenderVx(v1);
                            DrawRenderVx(renderVX);
                        }
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
        }
        //
        public override float OriginX => _glsx.OriginX;
        public override float OriginY => _glsx.OriginY;
        //
        public override void DrawString(string text, double left, double top)
        {
            _textPrinter?.DrawString(text, left, top);
        }
        public override RenderVxFormattedString CreateRenderVx(string textspan)
        {

            if (_textPrinter != null)
            {
                char[] buffer = textspan.ToCharArray();
                var renderVxFmtStr = new GLRenderVxFormattedString(buffer);
                _textPrinter?.PrepareStringForRenderVx(renderVxFmtStr, buffer, 0, buffer.Length);
                return renderVxFmtStr;
            }
            else
            {
                return null;
            }

        }
        public override void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            // 
            _textPrinter?.DrawString(renderVx, x, y);
        }
        public override void Fill(VertexStore vxs)
        {
            //return;
            //at GL-layer 
            _glsx.FillGfxPath(
                _fillColor,
                _igfxPathBuilder.CreateGraphicsPath(vxs)
                );
        }

        public override void FillRenderVx(Brush brush, RenderVx renderVx)
        {
            _glsx.FillRenderVx(brush, renderVx);
        }
        public override void FillRenderVx(RenderVx renderVx)
        {
            _glsx.FillRenderVx(_fillColor, renderVx);
        }
        public override void DrawRenderVx(RenderVx renderVx)
        {
            _glsx.DrawRenderVx(_strokeColor, renderVx);
        }


        public override void DrawLine(double x1, double y1, double x2, double y2)
        {
            _glsx.StrokeColor = _strokeColor;
            _glsx.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
        }
        public override void SetClipBox(int left, int top, int right, int bottom)
        {
            _glsx.SetClipRect(left, top, right - left, bottom - top);
        }
        public void DrawCircle(float centerX, float centerY, double radius)
        {
            DrawEllipse(centerX - radius, centerY - radius, radius + radius, radius + radius);
        }
        public void FillCircle(float x, float y, double radius)
        {
            FillEllipse(x - radius, y - radius, x + radius, y + radius);
        }
        //-----------------------------------------------------------------------------------------------------------------
        public override RenderVx CreateRenderVx(VertexStore vxs)
        {
            //store internal gfx path inside render vx 

            //1.
            InternalGraphicsPath p = _igfxPathBuilder.CreateGraphicsPathForRenderVx(vxs);
            return new GLRenderVx(p);
        }
        public RenderVx CreatePolygonRenderVx(float[] xycoords)
        {
            //store internal gfx path inside render vx
            Figure fig = new Figure(xycoords);
            fig.SupportVertexBuffer = true;
            return new GLRenderVx(new InternalGraphicsPath(fig));
        }
        public MultiPartTessResult CreateMultiPartTessResult(MultiPartPolygon multipartPolygon)
        {
            //store internal gfx path inside render vx
            MultiPartTessResult multipartTessResult = new MultiPartTessResult();

            _igfxPathBuilder.CreateGraphicsPathForMultiPartRenderVx(multipartPolygon,
                multipartTessResult,
                _glsx.GetTessTool(),
                _glsx.GetSmoothBorderBuilder());
            //
            return multipartTessResult;

        }
        struct CenterFormArc
        {
            public double cx;
            public double cy;
            public double radStartAngle;
            public double radSweepDiff;
            public bool scaleUp;
        }


        //---------------------------------------------------------------------
        public void DrawArc(
            float fromX, float fromY, float endX, float endY,
            float xaxisRotationAngleDec, float rx, float ry,
            SvgArcSize arcSize, SvgArcSweep arcSweep)
        {
            //------------------
            //SVG Elliptical arc ...
            //from Apache Batik
            //-----------------

            CenterFormArc centerFormArc = new CenterFormArc();
            ComputeArc2(fromX, fromY, rx, ry,
                 AggMath.deg2rad(xaxisRotationAngleDec),
                 arcSize == SvgArcSize.Large,
                 arcSweep == SvgArcSweep.Negative,
                 endX, endY, ref centerFormArc);
            _arcTool.Init(centerFormArc.cx, centerFormArc.cy, rx, ry,
                centerFormArc.radStartAngle,
                (centerFormArc.radStartAngle + centerFormArc.radSweepDiff));

            using (VxsTemp.Borrow(out var v1, out var v2, out var v3))
            {
                bool stopLoop = false;
                foreach (VertexData vertexData in _arcTool.GetVertexIter())
                {
                    switch (vertexData.command)
                    {
                        case VertexCmd.NoMore:
                            stopLoop = true;
                            break;
                        default:
                            v1.AddVertex(vertexData.x, vertexData.y, vertexData.command);
                            //yield return vertexData;
                            break;
                    }
                    //------------------------------
                    if (stopLoop) { break; }
                }

                double scaleRatio = 1;
                if (centerFormArc.scaleUp)
                {
                    int vxs_count = v1.Count;
                    double px0, py0, px_last, py_last;
                    v1.GetVertex(0, out px0, out py0);
                    v1.GetVertex(vxs_count - 1, out px_last, out py_last);
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

                        //var mat = Affine.NewMatix(
                        //        new AffinePlan(AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                        //        new AffinePlan(AffineMatrixCommand.Scale, scaleRatio, scaleRatio),
                        //        new AffinePlan(AffineMatrixCommand.Rotate, DegToRad(xaxisRotationAngleDec)),
                        //        new AffinePlan(AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                        //mat1.TransformToVxs(v1, v2);
                        //v1 = v2;

                        AffineMat mat = AffineMat.Iden;
                        mat.Translate(-centerFormArc.cx, -centerFormArc.cy);
                        mat.Scale(scaleRatio);
                        mat.RotateDeg(xaxisRotationAngleDec);
                        mat.Translate(centerFormArc.cx, centerFormArc.cy);
                        VertexStoreTransformExtensions.TransformToVxs(ref mat, v1, v2);
                        v1 = v2;
                    }
                    else
                    {
                        //not scale
                        //var mat = Affine.NewMatix(
                        //        AffinePlan.Translate(-centerFormArc.cx, -centerFormArc.cy),
                        //        AffinePlan.RotateDeg(xaxisRotationAngleDec),
                        //        AffinePlan.Translate(centerFormArc.cx, centerFormArc.cy)); 
                        //mat.TransformToVxs(v1, v2);
                        //v1 = v2;

                        AffineMat mat = AffineMat.Iden;
                        mat.Translate(-centerFormArc.cx, -centerFormArc.cy);
                        mat.RotateDeg(xaxisRotationAngleDec);
                        mat.Translate(centerFormArc.cx, centerFormArc.cy);
                        VertexStoreTransformExtensions.TransformToVxs(ref mat, v1, v2);
                        v1 = v2;
                    }
                }
                else
                {
                    //no rotate
                    if (centerFormArc.scaleUp)
                    {
                        //var mat = Affine.NewMatix(
                        //        new AffinePlan(AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                        //        new AffinePlan(AffineMatrixCommand.Scale, scaleRatio, scaleRatio),
                        //        new AffinePlan(AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));

                        //mat.TransformToVxs(v1, v2);
                        //v1 = v2; 
                        AffineMat mat = AffineMat.Iden;
                        mat.Translate(-centerFormArc.cx, -centerFormArc.cy);
                        mat.RotateDeg(scaleRatio);
                        mat.Translate(centerFormArc.cx, centerFormArc.cy);
                        //
                        VertexStoreTransformExtensions.TransformToVxs(ref mat, v1, v2);
                        v1 = v2;

                    }
                }

                _aggStroke.Width = this.StrokeWidth;
                _aggStroke.MakeVxs(v1, v3);
                _glsx.DrawGfxPath(_glsx.StrokeColor, _igfxPathBuilder.CreateGraphicsPath(v3));

            }
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
        //================

        struct InternalGraphicsPathBuilder
        {
            //helper struct

            List<float> xylist;
            public static InternalGraphicsPathBuilder CreateNew()
            {
                InternalGraphicsPathBuilder builder = new InternalGraphicsPathBuilder();
                builder.xylist = new List<float>();
                return builder;
            }

            public InternalGraphicsPath CreateGraphicsPath(VertexStore vxs)
            {
                return CreateGraphicsPath(vxs, false);
            }
            public InternalGraphicsPath CreateGraphicsPathForRenderVx(VertexStore vxs)
            {
                return CreateGraphicsPath(vxs, true);
            }


            InternalGraphicsPath CreateGraphicsPath(VertexStore vxs, bool buildForRenderVx)
            {

                double prevX = 0;
                double prevY = 0;
                double prevMoveToX = 0;
                double prevMoveToY = 0;

                xylist.Clear();
                //TODO: reivew here 
                //about how to reuse this list 


                //result...
                List<Figure> figures = new List<Figure>();

                int index = 0;
                VertexCmd cmd;

                double x, y;
                while ((cmd = vxs.GetVertex(index++, out x, out y)) != VertexCmd.NoMore)
                {
                    switch (cmd)
                    {
                        case PixelFarm.CpuBlit.VertexCmd.MoveTo:

                            prevMoveToX = prevX = x;
                            prevMoveToY = prevY = y;
                            xylist.Add((float)x);
                            xylist.Add((float)y);
                            break;
                        case PixelFarm.CpuBlit.VertexCmd.LineTo:
                            xylist.Add((float)x);
                            xylist.Add((float)y);
                            prevX = x;
                            prevY = y;
                            break;
                        case PixelFarm.CpuBlit.VertexCmd.Close:
                            {
                                //from current point 
                                xylist.Add((float)prevMoveToX);
                                xylist.Add((float)prevMoveToY);
                                prevX = prevMoveToX;
                                prevY = prevMoveToY;
                                //-----------
                                Figure newfig = new Figure(xylist.ToArray());
                                newfig.IsClosedFigure = true;
                                newfig.SupportVertexBuffer = buildForRenderVx;
                                figures.Add(newfig);
                                //-----------
                                xylist.Clear(); //clear temp list

                            }
                            break;
                        case VertexCmd.CloseAndEndFigure:
                            {
                                //from current point 
                                xylist.Add((float)prevMoveToX);
                                xylist.Add((float)prevMoveToY);
                                prevX = prevMoveToX;
                                prevY = prevMoveToY;
                                // 
                                Figure newfig = new Figure(xylist.ToArray());
                                newfig.IsClosedFigure = true;
                                newfig.SupportVertexBuffer = buildForRenderVx;
                                figures.Add(newfig);
                                //-----------
                                xylist.Clear();//clear temp list
                            }
                            break;
                        case PixelFarm.CpuBlit.VertexCmd.NoMore:
                            goto EXIT_LOOP;
                        default:
                            throw new System.NotSupportedException();
                    }
                }
                EXIT_LOOP:

                if (figures.Count == 0)
                {
                    Figure newfig = new Figure(xylist.ToArray());
                    newfig.IsClosedFigure = false;
                    newfig.SupportVertexBuffer = buildForRenderVx;
                    figures.Add(newfig);
                }
                else if (xylist.Count > 1)
                {
                    xylist.Add((float)prevMoveToX);
                    xylist.Add((float)prevMoveToY);
                    prevX = prevMoveToX;
                    prevY = prevMoveToY;
                    Figure newfig = new Figure(xylist.ToArray());
                    newfig.IsClosedFigure = true; //?
                    newfig.SupportVertexBuffer = buildForRenderVx;
                    figures.Add(newfig);
                }
                return new InternalGraphicsPath(figures);
            }

            internal void CreateGraphicsPathForMultiPartRenderVx(
               MultiPartPolygon multipartPolygon,
               MultiPartTessResult multipartTessResult,
               TessTool tessTool,
               SmoothBorderBuilder borderBuilder)
            {
                //a multipart polygon contains a  list of  expand coord (x,y) set.

                List<float[]> expandCoordsList = multipartPolygon.expandCoordsList;
                List<int[]> endPointList = multipartPolygon.contourEndPoints;


                int listCount = expandCoordsList.Count;
                for (int i = 0; i < listCount; ++i)
                {
                    //expand x,y
                    float[] expandCoords = expandCoordsList[i];
                    int[] endPoints = endPointList[i];
                    //area
                    int localVertexCount;

                    tessTool.TessAndAddToMultiPartResult(expandCoords,
                        endPoints,
                        multipartTessResult,
                        out localVertexCount);

                    //borders  
                    //build smooth border  
                    int m = endPoints.Length;
                    int latest_endPoint = 0;
                    multipartTessResult.BeginBorderPart();
                    for (int n = 0; n < m; ++n)
                    {
                        int endPoint = endPoints[n]; //'x' , not include 'y'
                        int len = (endPoint - latest_endPoint) + 1; //so len we add +1 for 'y'
                        int borderTriangleStripCount;
                        //expand coords for draw array
                        float[] smoothBorderXYs = borderBuilder.BuildSmoothBorders(expandCoords, latest_endPoint, len, out borderTriangleStripCount);
                        latest_endPoint += len + 2;
                        multipartTessResult.AddSmoothBorders(smoothBorderXYs, borderTriangleStripCount);
                    }
                    multipartTessResult.EndBorderPart();
                }
            }
        }
    }
}