//MIT, 2016-2018, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.VertexSource;


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
        RenderQualtity _renderQuality;


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
        Color _fontFillColor;
        public Color FontFillColor
        {
            get
            {
                return _fontFillColor;
            }
            set
            {
                _fontFillColor = value; 
            }
        }
        DrawBoardOrientation _orientation;
        public override DrawBoardOrientation Orientation
        {
            get { return _orientation; }
            set
            { _orientation = value; }
        }
        public bool UseVertexBufferObjectForRenderVx { get; set; }
        public override void SetOrigin(float ox, float oy)
        {
            _glsx.SetCanvasOrigin((int)ox, (int)oy);
        }
        public GLRenderSurface Canvas { get { return this._glsx; } }
        public override RenderQualtity RenderQuality
        {
            get { return _renderQuality; }
            set { _renderQuality = value; }
        }
        public override RequestFont CurrentFont
        {
            get
            {
                return _requestFont;
            }
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
            get
            {
                return _clipBox;
            }

            set
            {
                _clipBox = value;
            }
        }
        public override SmoothingMode SmoothingMode
        {
            get
            {
                return _smoothingMode;
            }
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
            get { return _textPrinter; }
            set
            {
                _textPrinter = value;
                if (value != null && _requestFont != null)
                {
                    _textPrinter.ChangeFont(this._requestFont);
                }
            }
        }
        public override Color FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
                _glsx.FontFillColor = value;
            }
        }
        public override int Height
        {
            get
            {
                return this._height;
            }
        }

        public override Color StrokeColor
        {
            get
            {
                return _strokeColor;
            }
            set
            {
                _strokeColor = value;
                _glsx.StrokeColor = value;
            }
        }

        public override double StrokeWidth
        {
            get
            {
                return _glsx.StrokeWidth;
            }
            set
            {
                _glsx.StrokeWidth = (float)value;
                _aggStroke.Width = (float)value;
            }
        }

        public override bool UseSubPixelLcdEffect
        {
            get
            {
                return _glsx.SmoothMode == SmoothMode.Smooth;
            }

            set
            {
                _glsx.SmoothMode = value ? SmoothMode.Smooth : SmoothMode.No;
            }
        }

        public override int Width
        {
            get
            {
                return _width;
            }
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
            _glsx.DrawGfxPath(this._strokeColor,
                _igfxPathBuilder.CreateGraphicsPath(vxs));
        }


        DrawingGL.GLBitmap ResolveForGLBitmap(Image image)
        {
            var cacheBmp = Image.GetCacheInnerImage(image) as DrawingGL.GLBitmap;
            if (cacheBmp != null)
            {
                return cacheBmp;
            }
            else
            {
                GLBitmap glBmp = null;
                if (image is ActualImage)
                {
                    ActualImage actualImage = (ActualImage)image;
                    glBmp = new GLBitmap(actualImage);
                }
                else
                {
                    //TODO: review here
                    //we should create 'borrow' method ? => send direct exact ptr to img buffer 
                    //for now, create a new one -- after we copy we, don't use it 
                    var req = new Image.ImgBufferRequestArgs(32, Image.RequestType.Copy);
                    image.RequestInternalBuffer(ref req);
                    int[] copy = req.OutputBuffer32;
                    glBmp = new GLBitmap(image.Width, image.Height, copy, req.IsInvertedImage);
                }

                Image.SetCacheInnerImage(image, glBmp);
                return glBmp;
            }
        }
        public override void DrawImage(Image actualImage, params AffinePlan[] affinePlans)
        {
            //create gl bmp
            GLBitmap glBmp = ResolveForGLBitmap(actualImage);// new GLBitmap(actualImage.Width, actualImage.Height, ActualImage.GetBuffer(actualImage), false);
            if (glBmp != null)
            {
                _glsx.DrawImage(glBmp, 0, 0);
            }

        }
        public override void DrawImage(Image actualImage, double left, double top)
        {

            GLBitmap glBmp = ResolveForGLBitmap(actualImage);
            if (glBmp == null) return;

            if (this._orientation == DrawBoardOrientation.LeftTop)
            {
                //place left upper corner at specific x y 
                _glsx.DrawImage(glBmp, (float)left, _glsx.ViewportHeight - (float)top);
            }
            else
            {
                //left-bottom as original
                //place left-lower of the img at specific (x,y)
                _glsx.DrawImage(glBmp, (float)left, (float)top);
            }
        }
        float[] rect_coords = new float[8];
        public override void FillRect(double left, double top, double width, double height)
        {
            if (_orientation == DrawBoardOrientation.LeftBottom)
            {
                CreateRectTessCoordsTriStrip((float)left, (float)(top - height), (float)width, (float)height, rect_coords);
            }
            else
            {
                int canvasH = _glsx.ViewportHeight;
                CreateRectTessCoordsTriStrip((float)left, canvasH - (float)(top + height), (float)width, (float)height, rect_coords);
            }
            _glsx.FillTriangleStrip(_fillColor, rect_coords, 4);
        }
        public override void DrawEllipse(double left, double top, double width, double height)
        {

            //double ox = (left + right) * 0.5;
            //double oy = (left + right) * 0.5;
            //if (this._orientation == DrawBoardOrientation.LeftTop)
            //{
            //    //modified
            //    oy = this.Height - oy;
            //}
            //ellipse.Reset(ox,
            //              oy,
            //             (right - left) * 0.5,
            //             (top - bottom) * 0.5,
            //              ellipseGenNSteps);
            //var v1 = GetFreeVxs();
            //var v2 = GetFreeVxs();
            //_aggsx.Render(stroke.MakeVxs(ellipse.MakeVxs(v1), v2), this.strokeColor);
            //ReleaseVxs(ref v1);
            //ReleaseVxs(ref v2);



            double x = (left + width / 2);
            double y = (top + height / 2);
            double rx = Math.Abs(width / 2);
            double ry = Math.Abs(height / 2);



            if (this._orientation == DrawBoardOrientation.LeftTop)
            {
                y = _glsx.ViewportHeight - y; //set new y
            }

            ellipse.Reset(x, y, rx, ry);
            VertexStore vxs = ellipse.MakeVxs(GetFreeVxs());
            VertexStore v3 = _aggStroke.MakeVxs(vxs, GetFreeVxs());
            //***
            //we fill the stroke's path
            _glsx.FillGfxPath(_strokeColor, _igfxPathBuilder.CreateGraphicsPath(v3));

            ReleaseVxs(ref vxs);
            ReleaseVxs(ref v3);
        }
        public override void FillEllipse(double left, double top, double width, double height)
        {
            //version 2:
            //agg's ellipse tools with smooth border

            double x = (left + width / 2);
            double y = (top + height / 2);
            double rx = Math.Abs(width / 2);
            double ry = Math.Abs(height / 2);



            if (this._orientation == DrawBoardOrientation.LeftTop)
            {
                y = _glsx.ViewportHeight - y; //set new y
            }

            ellipse.Reset(x, y, rx, ry);
            VertexStore vxs = ellipse.MakeVxs(GetFreeVxs());
            //***
            //we fill  
            _glsx.FillGfxPath(_strokeColor, _igfxPathBuilder.CreateGraphicsPath(vxs));
            ReleaseVxs(ref vxs);

            //-------------------------------------------------------------
            //
            //version 1,just triangular fans, no smooth border

            //double x = (left + right) / 2;
            //double y = (bottom + top) / 2;
            //double rx = Math.Abs(right - x);
            //double ry = Math.Abs(top - y);

            //if (this._orientation == DrawBoardOrientation.LeftTop)
            //{
            //    y = _glsx.ViewportHeight - y; //set new y
            //}

            //ellipse.Reset(x, y, rx, ry);
            //var v1 = GetFreeVxs();
            //ellipse.MakeVxs(v1);
            ////other mode
            //int n = v1.Count;
            ////make triangular fan*** 

            //float[] coords = new float[(n * 2) + 4];
            //int i = 0;
            //int nn = 0;
            //int npoints = 0;
            //double vx, vy;
            ////center
            //coords[nn++] = (float)x;
            //coords[nn++] = (float)y;
            //npoints++;
            //var cmd = v1.GetVertex(i, out vx, out vy);
            //while (i < n)
            //{
            //    switch (cmd)
            //    {
            //        case VertexCmd.MoveTo:
            //            {
            //                coords[nn++] = (float)vx;
            //                coords[nn++] = (float)vy;
            //                npoints++;
            //            }
            //            break;
            //        case VertexCmd.LineTo:
            //            {
            //                coords[nn++] = (float)vx;
            //                coords[nn++] = (float)vy;
            //                npoints++;
            //            }
            //            break;
            //        case VertexCmd.NoMore:
            //            {
            //            }
            //            break;
            //        default:
            //            {
            //            }
            //            break;
            //    }
            //    i++;
            //    cmd = v1.GetVertex(i, out vx, out vy);
            //} 
            ////close circle
            //coords[nn++] = coords[2];
            //coords[nn++] = coords[3];
            //npoints++;
            ////----------------------------------------------
            //_glsx.FillTriangleFan(_fillColor, coords, npoints);
            //ReleaseVxs(ref v1);
            ////---------------------------------------------- 
        }


        public override void DrawRect(double left, double top, double width, double height)
        {
            if (_orientation == DrawBoardOrientation.LeftBottom)
            {
                _glsx.DrawRect((float)left, (float)top, (float)width, (float)height);

            }
            else
            {
                int canvasH = _glsx.ViewportHeight;
                _glsx.DrawRect((float)left + 0.5f, canvasH - (float)(top + height + 0.5f), (float)width, (float)height);
            }

        }

        public override float OriginX
        {
            get
            {
                return _glsx.OriginX;
            }
        }
        public override float OriginY
        {
            get
            {
                return _glsx.OriginY;
            }
        }
        public override void DrawString(string text, double x, double y)
        {
            if (_textPrinter != null)
            {
                if (_orientation == DrawBoardOrientation.LeftBottom)
                {
                    _textPrinter.DrawString(text, x, y);
                }
                else
                {
                    int canvasH = _glsx.ViewportHeight;
                    _textPrinter.DrawString(text, x, canvasH - y);

                }

            }
        }
        public override RenderVxFormattedString CreateRenderVx(string textspan)
        {
            var renderVxFmtStr = new GLRenderVxFormattedString(textspan);
            if (_textPrinter != null)
            {
                char[] buffer = textspan.ToCharArray();
                _textPrinter.PrepareStringForRenderVx(renderVxFmtStr, buffer, 0, buffer.Length);

            }
            return renderVxFmtStr;
        }
        public override void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            //
            if (_textPrinter != null)
            {
                _textPrinter.DrawString(renderVx, x, y);
            }
        }
        public override void Fill(VertexStore vxs)
        {
            _glsx.FillGfxPath(
                _fillColor,
                _igfxPathBuilder.CreateGraphicsPath(vxs)
                );
        }

        public override void Fill(VertexStoreSnap snap)
        {
            _glsx.FillGfxPath(
                _fillColor,
              _igfxPathBuilder.CreateGraphicsPath(snap));
        }
        public override void Draw(VertexStoreSnap snap)
        {
            _glsx.DrawGfxPath(
             this._strokeColor,
             _igfxPathBuilder.CreateGraphicsPath(snap)
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


        /// <summary>
        /// create rect tess for openGL
        /// </summary>
        /// <param name="x">left</param>
        /// <param name="y">bottom</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="output"></param>
        static void CreateRectTessCoordsTriStrip(float x, float y, float w, float h, float[] output)
        {
            //float x0 = x;
            //float y0 = y + h;
            //float x1 = x;
            //float y1 = y;
            //float x2 = x + w;
            //float y2 = y + h;
            //float x3 = x + w;
            //float y3 = y;
            output[0] = x; output[1] = y + h;
            output[2] = x; output[3] = y;
            output[4] = x + w; output[5] = y + h;
            output[6] = x + w; output[7] = y;


        }

        public override void DrawLine(double x1, double y1, double x2, double y2)
        {
            _glsx.StrokeColor = _strokeColor;
            if (this._orientation == DrawBoardOrientation.LeftBottom)
            {
                //as OpenGL original
                _glsx.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
            }
            else
            {
                int h = _glsx.ViewportHeight;
                _glsx.DrawLine((float)x1, h - (float)y1, (float)x2, h - (float)y2);
            }


        }

        public override void PaintSeries(VertexStore vxs, Color[] colors, int[] pathIndexs, int numPath)
        {
            //TODO: review here.
            //
            for (int i = 0; i < numPath; ++i)
            {
                _glsx.FillGfxPath(colors[i],
                    _igfxPathBuilder.CreateGraphicsPath(
                        new VertexStoreSnap(vxs, pathIndexs[i])));
            }
        }
        public override void SetClipBox(int x1, int y1, int x2, int y2)
        {
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
        public override RenderVx CreateRenderVx(VertexStoreSnap snap)
        {
            //store internal gfx path inside render vx 

            //1.
            InternalGraphicsPath p = _igfxPathBuilder.CreateGraphicsPathForRenderVx(snap);
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

        VertexStorePool _vxsPool = new VertexStorePool();
        VertexStore GetFreeVxs()
        {
            return _vxsPool.GetFreeVxs();
        }
        void ReleaseVxs(ref VertexStore vxs)
        {
            _vxsPool.Release(ref vxs);
        }
        //---------------------------------------------------------------------
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
            _arcTool.Init(centerFormArc.cx, centerFormArc.cy, rx, ry,
                centerFormArc.radStartAngle,
                (centerFormArc.radStartAngle + centerFormArc.radSweepDiff));

            VertexStore v1 = GetFreeVxs();
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
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Scale, scaleRatio, scaleRatio),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Rotate, DegToRad(xaxisRotationAngleDec)),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    var v2 = GetFreeVxs();
                    mat.TransformToVxs(v1, v2);
                    ReleaseVxs(ref v1);
                    v1 = v2;
                }
                else
                {
                    //not scalue
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Rotate, DegToRad(xaxisRotationAngleDec)),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    var v2 = GetFreeVxs();
                    mat.TransformToVxs(v1, v2);
                    ReleaseVxs(ref v1);
                    v1 = v2;
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
                    var v2 = GetFreeVxs();
                    mat.TransformToVxs(v1, v2);
                    ReleaseVxs(ref v1);
                    v1 = v2;
                }
            }

            _aggStroke.Width = this.StrokeWidth;


            var v3 = _aggStroke.MakeVxs(v1, GetFreeVxs());
            _glsx.DrawGfxPath(_glsx.StrokeColor, _igfxPathBuilder.CreateGraphicsPath(v3));

            ReleaseVxs(ref v3);
            ReleaseVxs(ref v1);

        }
        static double DegToRad(double degree)
        {
            return degree * (Math.PI / 180d);
        }
        static double RadToDeg(double degree)
        {
            return degree * (180d / Math.PI);
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
            public InternalGraphicsPath CreateGraphicsPath(VertexStoreSnap vxsSnap)
            {
                return CreateGraphicsPath(vxsSnap, false);
            }
            public InternalGraphicsPath CreateGraphicsPath(VertexStore vxs)
            {
                return CreateGraphicsPath(new VertexStoreSnap(vxs), false);
            }
            public InternalGraphicsPath CreateGraphicsPathForRenderVx(VertexStoreSnap vxsSnap)
            {
                return CreateGraphicsPath(vxsSnap, true);
            }


            InternalGraphicsPath CreateGraphicsPath(VertexStoreSnap vxsSnap, bool buildForRenderVx)
            {
                VertexSnapIter vxsIter = vxsSnap.GetVertexSnapIter();
                double prevX = 0;
                double prevY = 0;
                double prevMoveToX = 0;
                double prevMoveToY = 0;

                xylist.Clear();
                //TODO: reivew here 
                //about how to reuse this list 

                bool isAddToList = true;
                //result...
                List<Figure> figures = new List<Figure>();

                for (; ; )
                {
                    double x, y;
                    switch (vxsIter.GetNextVertex(out x, out y))
                    {
                        case PixelFarm.Agg.VertexCmd.MoveTo:
                            if (!isAddToList)
                            {
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
                        case PixelFarm.Agg.VertexCmd.Close:
                            //from current point 
                            xylist.Add((float)prevMoveToX);
                            xylist.Add((float)prevMoveToY);
                            prevX = prevMoveToX;
                            prevY = prevMoveToY;

                            break;
                        case VertexCmd.CloseAndEndFigure:
                            //from current point 
                            {
                                xylist.Add((float)prevMoveToX);
                                xylist.Add((float)prevMoveToY);
                                prevX = prevMoveToX;
                                prevY = prevMoveToY;
                                // 
                                Figure newfig = new Figure(xylist.ToArray());
                                newfig.SupportVertexBuffer = buildForRenderVx;
                                figures.Add(newfig);
                                //-----------
                                xylist.Clear();
                                isAddToList = false;
                            }
                            break;
                        case PixelFarm.Agg.VertexCmd.NoMore:
                            goto EXIT_LOOP;
                        default:
                            throw new System.NotSupportedException();
                    }
                }
                EXIT_LOOP:

                if (figures.Count == 0)
                {
                    Figure newfig = new Figure(xylist.ToArray());
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