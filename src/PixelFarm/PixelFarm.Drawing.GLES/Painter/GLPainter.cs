//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.DrawingGL
{
    public sealed class GLPainter : Painter
    {
        GLPainterContext _pcx;
        SmoothingMode _smoothingMode; //smoothing mode of this  painter
        RenderSurfaceOrientation _orientation = RenderSurfaceOrientation.LeftTop;

        int _width;
        int _height;

        Color _fillColor;
        Color _strokeColor;
        RectInt _clipBox;

        PathRenderVxBuilder _pathRenderVxBuilder;

        Stroke _stroke = new Stroke(1);
        RequestFont _requestFont;
        ITextPrinter _textPrinter;
        RenderQuality _renderQuality;
        Brush _currentBrush;
        Brush _defaultBrush;

        Pen _currentPen;

        SimpleRectBorderBuilder _simpleBorderRectBuilder = new SimpleRectBorderBuilder();
        float[] _reuseableRectBordersXYs = new float[16];
        ClipingTechnique _currentClipTech;

        public GLPainter()
        {

            CurrentFont = new RequestFont("tahoma", 14);
            UseVertexBufferObjectForRenderVx = true;
            //tools
            _pathRenderVxBuilder = PathRenderVxBuilder.CreateNew();
            _defaultBrush = _currentBrush = new SolidBrush(Color.Black); //default brush

        }
        public GLPainterContext PainterContext => _pcx;
        public bool EnableBuiltInMaskComposite { get; set; }

        public void BindToPainterContext(GLPainterContext pcx)
        {
            if (_pcx == pcx)
            {
                return;
            }
            //
            _pcx = pcx;
            _width = pcx.CanvasWidth;
            _height = pcx.CanvasHeight;
            _clipBox = new RectInt(0, 0, _width, _height);
        }
        public override void SetClipRgn(VertexStore vxs)
        {

            //TODO: review mask combine mode
            //1. Append
            //2. Replace 

            //this version we use replace 
            //clip rgn implementation
            //this version replace only
            //TODO: add append clip rgn 

            //TODO: implement complex framebuffer-based mask

            if (vxs != null)
            {
                if (PixelFarm.Drawing.SimpleRectClipEvaluator.EvaluateRectClip(vxs, out RectangleF clipRect))
                {
                    this.SetClipBox(
                        (int)Math.Floor(clipRect.X), (int)Math.Floor(clipRect.Y),
                        (int)Math.Ceiling(clipRect.Right), (int)Math.Ceiling(clipRect.Bottom));

                    _currentClipTech = ClipingTechnique.ClipSimpleRect;
                }
                else
                {
                    //not simple rect => 
                    //use mask technique
                    _currentClipTech = ClipingTechnique.ClipMask;
                    //1. switch to mask buffer  
                    PathRenderVx pathRenderVx = _pathRenderVxBuilder.CreatePathRenderVx(vxs);
                    _pcx.EnableMask(pathRenderVx);
                }
            }
            else
            {
                //remove clip rgn if exists**
                switch (_currentClipTech)
                {
                    case ClipingTechnique.ClipMask:
                        _pcx.DisableMask();
                        break;
                    case ClipingTechnique.ClipSimpleRect:
                        this.SetClipBox(0, 0, this.Width, this.Height);
                        break;
                }
                _currentClipTech = ClipingTechnique.None;
            }
        }
        public override void Render(RenderVx renderVx)
        {
            throw new NotImplementedException();
        }

        public void DetachCurrentShader() => _pcx.DetachCurrentShader();

        public Color FontFillColor
        {
            get => _pcx.FontFillColor;
            set => _pcx.FontFillColor = value;
        }

        public override RenderSurfaceOrientation Orientation
        {
            get => _orientation;
            set => _orientation = value;
        }

        public bool UseVertexBufferObjectForRenderVx { get; set; }

        public override void SetOrigin(float ox, float oy)
        {
            _pcx.SetCanvasOrigin((int)ox, (int)oy);
        }
        //
        public GLPainterContext Canvas => _pcx;
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
                        _pcx.SmoothMode = SmoothMode.Smooth;
                        break;
                    default:
                        _pcx.SmoothMode = SmoothMode.No;
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

        public override Brush CurrentBrush
        {
            get => _currentBrush;
            set
            {
                //brush with its detail             
                //------------------------
                if (value == null)
                {
                    _currentBrush = _defaultBrush;
                    return;
                }
                //
                //
                switch (value.BrushKind)
                {
                    default:
                        break;
                    case BrushKind.Solid:
                        {
                            SolidBrush solidBrush = (SolidBrush)value;
                            _fillColor = solidBrush.Color;
                        }
                        break;
                    case BrushKind.LinearGradient:
                        {
                            LinearGradientBrush linear = (LinearGradientBrush)value;



                        }
                        break;
                    case BrushKind.CircularGraident:
                        break;

                    case BrushKind.Texture:

                        break;
                }
                _currentBrush = value;
            }

        }


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
                _pcx.StrokeColor = value;
            }
        }

        public override double StrokeWidth
        {
            get => _pcx.StrokeWidth;
            set
            {
                _pcx.StrokeWidth = (float)value;
                _stroke.Width = (float)value;
            }
        }

        public override bool UseSubPixelLcdEffect
        {
            get => _pcx.SmoothMode == SmoothMode.Smooth;
            set => _pcx.SmoothMode = value ? SmoothMode.Smooth : SmoothMode.No;
        }

        public override void Clear(Color color)
        {
            _pcx.Clear(color);
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
            if (!vxs.IsShared)
            {
                //
                PathRenderVx renderVx = VertexStore.GetBorderRenderVx(vxs) as PathRenderVx;
                if (renderVx == null)
                {

                }

            }

            if (StrokeWidth > 1)
            {
                using (VxsTemp.Borrow(out VertexStore v1))
                using (VectorToolBox.Borrow(out Stroke stroke))
                {
                    //convert large stroke to vxs
                    stroke.Width = StrokeWidth;
                    stroke.MakeVxs(vxs, v1);

                    Color prevColor = this.FillColor;
                    FillColor = this.StrokeColor;
                    Fill(v1);
                    FillColor = prevColor;
                }
            }
            else
            {
                _pcx.DrawGfxPath(_strokeColor,
                    _pathRenderVxBuilder.CreatePathRenderVx(vxs));
            }
        }



        public override void DrawImage(Image actualImage, params AffinePlan[] affinePlans)
        {
            //create gl bmp
            //TODO: affinePlans***
            GLBitmap glBmp = _pcx.ResolveForGLBitmap(actualImage);
            if (glBmp != null)
            {
                _pcx.DrawImage(glBmp, 0, 0);
            }
        }
        public override void DrawImage(Image actualImage, double left, double top, ICoordTransformer coordTx)
        {
            //TODO: implement transformation matrix
            GLBitmap glBmp = _pcx.ResolveForGLBitmap(actualImage);
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
                    _pcx.DrawImageToQuad(glBmp, aff);
                }
                else
                {

                }

                //_pcx.DrawImage(glBmp, (float)left, (float)top);
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
            GLBitmap glBmp = _pcx.ResolveForGLBitmap(actualImage);
            if (glBmp == null) return;
            _pcx.DrawImage(glBmp, 0, 0);
        }
        public override void DrawImage(Image actualImage, double left, double top)
        {
            GLBitmap glBmp = _pcx.ResolveForGLBitmap(actualImage);
            if (glBmp == null) return;
            _pcx.DrawImage(glBmp, (float)left, (float)top);
        }
        public override void DrawImage(Image actualImage, double left, double top, int srcX, int srcY, int srcW, int srcH)
        {
            throw new NotImplementedException();
        }


        public override void FillRect(double left, double top, double width, double height)
        {
            switch (_currentBrush.BrushKind)
            {
                default:
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("unknown brush!");
#endif
                    break;
                case BrushKind.LinearGradient:
                    {
                        //resolve internal linear gradient brush impl

                        using (VxsTemp.Borrow(out var v1))
                        using (VectorToolBox.Borrow(out SimpleRect rect))
                        {
                            rect.SetRectFromLTWH(left, top, width, height);
                            rect.MakeVxs(v1);

                            //convert to render vx
                            //TODO: optimize here ***
                            //we don't want to create path render vx everytime
                            //
                            //
                            PathRenderVx pathRenderVx = _pathRenderVxBuilder.CreatePathRenderVx(v1);
                            _pcx.FillGfxPath(_currentBrush, pathRenderVx);
                        }
                    }
                    break;
                case BrushKind.CircularGraident:
                    break;
                case BrushKind.GeometryGradient:
                    break;
                case BrushKind.Solid:
                    _pcx.FillRect(_fillColor, left, top, width, height);
                    break;
                case BrushKind.Texture:
                    break;
            }

        }
        public override void DrawEllipse(double left, double top, double width, double height)
        {
            double x = (left + width / 2);
            double y = (top + height / 2);
            double rx = Math.Abs(width / 2);
            double ry = Math.Abs(height / 2);

            using (VectorToolBox.Borrow(out Ellipse ellipse))
            using (VxsTemp.Borrow(out var v1, out var v2))
            {
                ellipse.Set(x, y, rx, ry);

                ellipse.MakeVxs(v1);
                _stroke.MakeVxs(v1, v2);
                //***
                //we fill the stroke's path
                _pcx.FillGfxPath(_strokeColor, _pathRenderVxBuilder.CreatePathRenderVx(v2));
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
            //
            using (VectorToolBox.Borrow(out Ellipse ellipse))
            using (VxsTemp.Borrow(out var vxs))
            {
                ellipse.MakeVxs(vxs);
                //***
                //we fill  
                _pcx.FillGfxPath(_strokeColor, _pathRenderVxBuilder.CreatePathRenderVx(vxs));
            }

        }

        public override void DrawRect(double left, double top, double width, double height)
        {
            switch (_pcx.SmoothMode)
            {
                case SmoothMode.Smooth:
                    {
                        _pcx.StrokeColor = this.StrokeColor;
                        using (PixelFarm.Drawing.VxsTemp.Borrow(out Drawing.VertexStore v1))
                        using (PixelFarm.Drawing.VectorToolBox.Borrow(out CpuBlit.VertexProcessing.SimpleRect r))
                        {
                            r.SetRect(left + 0.5f, top + height + 0.5f, left + width - 0.5f, top - 0.5f);
                            r.MakeVxs(v1);
                            Draw(v1);
                        }
                    }
                    break;
                default:
                    {
                        //draw boarder with
                        if (StrokeWidth > 0 && StrokeColor.A > 0)
                        {
                            _simpleBorderRectBuilder.SetBorderWidth((float)StrokeWidth);
                            //_simpleBorderRectBuilder.BuildAroundInnerRefBounds(
                            //    (float)left, (float)top + (float)height, (float)left + (float)width, (float)top,
                            //    _reuseableRectBordersXYs);
                            _simpleBorderRectBuilder.BuildAroundInnerRefBounds(
                               (float)left, (float)top, (float)width, (float)height,
                               _reuseableRectBordersXYs);
                            //
                            _pcx.FillTessArea(StrokeColor,
                                _reuseableRectBordersXYs,
                                _simpleBorderRectBuilder.GetPrebuiltRectTessIndices());
                        }
                    }
                    break;
            }
        }
        //
        public override float OriginX => _pcx.OriginX;
        public override float OriginY => _pcx.OriginY;
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
                var renderVxFmtStr = new GLRenderVxFormattedString();
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
            PathRenderVx pathRenderVx = null;
            if (!vxs.IsShared)
            {
                //check if we have cached PathRenderVx or not
                pathRenderVx = VertexStore.GetAreaRenderVx(vxs) as PathRenderVx;
                //
                if (pathRenderVx == null)
                {
                    VertexStore.SetAreaRenderVx(
                        vxs,
                        pathRenderVx = _pathRenderVxBuilder.CreatePathRenderVx(vxs));
                }

            }
            else
            {
                pathRenderVx = _pathRenderVxBuilder.CreatePathRenderVx(vxs);
            }


            switch (_currentBrush.BrushKind)
            {
                default:
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("unknown brush!");
#endif
                    break;
                case BrushKind.LinearGradient:

                    //resolve internal linear gradient brush impl
                    _pcx.FillGfxPath(_currentBrush, pathRenderVx);
                    break;
                case BrushKind.CircularGraident:
                    //...
                    break;
                case BrushKind.GeometryGradient:
                    //....
                    break;
                case BrushKind.Solid:
                    {
                        _pcx.FillGfxPath(
                            _fillColor,
                            pathRenderVx
                        );
                    }
                    break;
                case BrushKind.Texture:
                    break;
            }



        }

        public override void FillRenderVx(Brush brush, RenderVx renderVx)
        {
            _pcx.FillRenderVx(brush, renderVx);
        }
        public override void FillRenderVx(RenderVx renderVx)
        {
            _pcx.FillRenderVx(_fillColor, renderVx);
        }
        public override void DrawRenderVx(RenderVx renderVx)
        {
            _pcx.DrawRenderVx(_strokeColor, renderVx);
        }


        public override void DrawLine(double x1, double y1, double x2, double y2)
        {
            _pcx.StrokeColor = _strokeColor;
            _pcx.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
        }
        public override void SetClipBox(int left, int top, int right, int bottom)
        {
            _pcx.SetClipRect(left, top, right - left, bottom - top);
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
            return _pathRenderVxBuilder.CreatePathRenderVx(vxs);
        }
        public RenderVx CreatePolygonRenderVx(float[] xycoords)
        {
            //store internal gfx path inside render vx

            return new PathRenderVx(new Figure(xycoords));
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

            //
            using (VectorToolBox.Borrow(out Arc arcTool))
            using (VxsTemp.Borrow(out var v1, out var v2, out var v3))
            {
                arcTool.Init(centerFormArc.cx, centerFormArc.cy, rx, ry,
                  centerFormArc.radStartAngle,
                  (centerFormArc.radStartAngle + centerFormArc.radSweepDiff));
                bool stopLoop = false;
                foreach (VertexData vertexData in arcTool.GetVertexIter())
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

                _stroke.Width = this.StrokeWidth;
                _stroke.MakeVxs(v1, v3);
                _pcx.DrawGfxPath(_pcx.StrokeColor, _pathRenderVxBuilder.CreatePathRenderVx(v3));

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

            //from Apache2, https://xmlgraphics.apache.org/
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

        struct PathRenderVxBuilder
        {
            //helper struct

            List<float> _xylist;
            public static PathRenderVxBuilder CreateNew()
            {
                PathRenderVxBuilder builder = new PathRenderVxBuilder();
                builder._xylist = new List<float>();
                return builder;
            }

            public PathRenderVx CreatePathRenderVx(VertexStore vxs)
            {

                double prevX = 0;
                double prevY = 0;
                double prevMoveToX = 0;
                double prevMoveToY = 0;

                _xylist.Clear();
                //TODO: reivew here 
                //about how to reuse this list  
                //result...

                MultiFigures figures = new MultiFigures();
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
                            _xylist.Add((float)x);
                            _xylist.Add((float)y);
                            break;
                        case PixelFarm.CpuBlit.VertexCmd.LineTo:
                            _xylist.Add((float)x);
                            _xylist.Add((float)y);
                            prevX = x;
                            prevY = y;
                            break;
                        case PixelFarm.CpuBlit.VertexCmd.Close:
                            {
                                //from current point 
                                _xylist.Add((float)prevMoveToX);
                                _xylist.Add((float)prevMoveToY);
                                prevX = prevMoveToX;
                                prevY = prevMoveToY;
                                //-----------
                                Figure newfig = new Figure(_xylist.ToArray());
                                newfig.IsClosedFigure = true;
                                figures.AddFigure(newfig);
                                //-----------
                                _xylist.Clear(); //clear temp list

                            }
                            break;
                        case VertexCmd.CloseAndEndFigure:
                            {
                                //from current point 
                                _xylist.Add((float)prevMoveToX);
                                _xylist.Add((float)prevMoveToY);
                                prevX = prevMoveToX;
                                prevY = prevMoveToY;
                                // 
                                Figure newfig = new Figure(_xylist.ToArray());
                                newfig.IsClosedFigure = true;
                                figures.AddFigure(newfig);
                                //-----------
                                _xylist.Clear();//clear temp list
                            }
                            break;
                        case PixelFarm.CpuBlit.VertexCmd.NoMore:
                            goto EXIT_LOOP;
                        default:
                            throw new System.NotSupportedException();
                    }
                }
                EXIT_LOOP:

                if (figures.FigureCount == 0)
                {
                    Figure newfig = new Figure(_xylist.ToArray());
                    newfig.IsClosedFigure = false;

                    return new PathRenderVx(newfig);
                }
                else if (_xylist.Count > 1)
                {
                    _xylist.Add((float)prevMoveToX);
                    _xylist.Add((float)prevMoveToY);
                    prevX = prevMoveToX;
                    prevY = prevMoveToY;
                    //
                    Figure newfig = new Figure(_xylist.ToArray());
                    newfig.IsClosedFigure = true; //? 
                    figures.AddFigure(newfig);
                }
                return new PathRenderVx(figures);
            }

        }
    }
}