//MIT, 2016-2017, WinterDev

using System;
using PixelFarm.Drawing;
using PixelFarm.Agg.VertexSource;
using PixelFarm.DrawingBuffer;

namespace PixelFarm.Agg
{
    public class AggPainter : Painter
    {
        AggRenderSurface _aggsx; //target rendering surface
        //low-level rasterizer
        ScanlinePacked8 scline;
        ScanlineRasterizer sclineRas;
        /// <summary>
        /// scanline rasterizer to bitmap
        /// </summary>
        ScanlineRasToDestBitmapRenderer sclineRasToBmp;

        //--------------------
        //pen 
        Stroke stroke;
        Color strokeColor;
        //--------------------
        //brush
        Color fillColor;
        //--------------------
        //image processing,
        FilterMan filterMan = new FilterMan();

        //font
        RequestFont currentFont;
        //-------------
        //vector generators for various object
        SimpleRect _simpleRectVxsGen = new SimpleRect();
        Ellipse ellipse = new Ellipse();
        PathWriter _lineGen = new PathWriter();

        MyImageReaderWriter sharedImageWriterReader = new MyImageReaderWriter();

        LineDashGenerator _lineDashGen;
        int ellipseGenNSteps = 20;
        SmoothingMode _smoothingMode;

        public AggPainter(AggRenderSurface aggsx)
        {
            this._aggsx = aggsx;
            this.sclineRas = _aggsx.ScanlineRasterizer;
            this.stroke = new Stroke(1);//default
            this.scline = aggsx.ScanlinePacked8;
            this.sclineRasToBmp = aggsx.ScanlineRasToDestBitmap;
        }
        DrawBoardOrientation _orientation;
        public override DrawBoardOrientation Orientation
        {
            get { return _orientation; }
            set
            { _orientation = value; }
        }
        public AggRenderSurface RenderSurface
        {
            get { return this._aggsx; }
        }
        public override int Width
        {
            get
            {
                return _aggsx.Width;
            }
        }
        public override int Height
        {
            get
            {
                return _aggsx.Height;
            }
        }
        public override void Clear(Color color)
        {
            _aggsx.Clear(color);
        }
        public override float OriginX
        {
            get { return sclineRas.OffsetOriginX; }
        }
        public override float OriginY
        {
            get { return sclineRas.OffsetOriginY; }
        }
        public override void SetOrigin(float x, float y)
        {
            sclineRas.OffsetOriginX = x;
            sclineRas.OffsetOriginY = y;
        }
        RenderQualtity _renderQuality;
        public override RenderQualtity RenderQuality
        {
            get { return _renderQuality; }
            set { _renderQuality = value; }
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
                    case Drawing.SmoothingMode.HighQuality:
                    case Drawing.SmoothingMode.AntiAlias:
                        //TODO: review here
                        //anti alias != lcd technique 
                        _aggsx.UseSubPixelRendering = true;
                        break;
                    case Drawing.SmoothingMode.HighSpeed:
                    default:
                        _aggsx.UseSubPixelRendering = false;
                        break;
                }
            }
        }
        public override RectInt ClipBox
        {
            get { return this._aggsx.GetClippingRect(); }
            set { this._aggsx.SetClippingRect(value); }
        }
        public override void SetClipBox(int x1, int y1, int x2, int y2)
        {
            this._aggsx.SetClippingRect(new RectInt(x1, y1, x2, y2));
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
        public override void Draw(VertexStoreSnap vxs)
        {
            this.Fill(vxs);
        }


        /// <summary>
        /// draw line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public override void DrawLine(double x1, double y1, double x2, double y2)
        {
            //coordinate system
            if (this.RenderQuality == RenderQualtity.Fast)
            {

                BitmapBuffer bmpBuffer= new BitmapBuffer()
                //BitmapBufferExtensions.DrawLine()
            }

            if (_orientation == DrawBoardOrientation.LeftBottom)
            {
                //as original
                _lineGen.Clear();
                _lineGen.MoveTo(x1, y1);
                _lineGen.LineTo(x2, y2);
                var v1 = GetFreeVxs();
                _aggsx.Render(stroke.MakeVxs(_lineGen.Vxs, v1), this.strokeColor);
                ReleaseVxs(ref v1);
            }
            else
            {
                //left-top
                int h = this.Height;

                _lineGen.Clear();
                _lineGen.MoveTo(x1, h - y1);
                _lineGen.LineTo(x2, h - y2);


                var v1 = GetFreeVxs();
                _aggsx.Render(stroke.MakeVxs(_lineGen.Vxs, v1), this.strokeColor);
                ReleaseVxs(ref v1);
            }


        }
        public override double StrokeWidth
        {
            get { return this.stroke.Width; }
            set { this.stroke.Width = value; }
        }
        public override void Draw(VertexStore vxs)
        {
            if (_lineDashGen == null)
            {
                //no line dash
                var v1 = GetFreeVxs();
                _aggsx.Render(stroke.MakeVxs(vxs, v1), this.strokeColor);
                ReleaseVxs(ref v1);
            }
            else
            {
                var v1 = GetFreeVxs();
                var v2 = GetFreeVxs();
                _lineDashGen.CreateDash(vxs, v1);
                stroke.MakeVxs(v1, v2);
                _aggsx.Render(v2, this.strokeColor);

                ReleaseVxs(ref v1);
                ReleaseVxs(ref v2);
            }
        }

        public override void DrawRect(double left, double top, double width, double height)
        {

            double right = left + width;
            double bottom = top - height;

            if (this._orientation == DrawBoardOrientation.LeftBottom)
            {
                _simpleRectVxsGen.SetRect(left + 0.5, bottom + 0.5, right - 0.5, top - 0.5);
            }
            else
            {
                int canvasH = this.Height;
                //_simpleRectVxsGen.SetRect(left + 0.5, canvasH - (bottom + 0.5), right - 0.5, canvasH - (top - 0.5));
                _simpleRectVxsGen.SetRect(left + 0.5, canvasH - (bottom + 0.5 + height), right - 0.5, canvasH - (top - 0.5 + height));
            }
            //----------------
            var v1 = GetFreeVxs();
            var v2 = GetFreeVxs();
            //
            _aggsx.Render(stroke.MakeVxs(_simpleRectVxsGen.MakeVxs(v1), v2), this.strokeColor);
            //
            ReleaseVxs(ref v1);
            ReleaseVxs(ref v2);
        }

        public override void DrawEllipse(double left, double top, double width, double height)
        {
            double ox = (left + width / 2);
            double oy = (top + height / 2);
            if (this._orientation == DrawBoardOrientation.LeftTop)
            {
                //modified
                oy = this.Height - oy;
            }
            ellipse.Reset(ox,
                         oy,
                         width / 2,
                         height / 2,
                         ellipseGenNSteps);
            var v1 = GetFreeVxs();
            var v2 = GetFreeVxs();
            _aggsx.Render(stroke.MakeVxs(ellipse.MakeVxs(v1), v2), this.strokeColor);
            ReleaseVxs(ref v1);
            ReleaseVxs(ref v2);
        }
        public override void FillEllipse(double left, double top, double width, double height)
        {
            double ox = (left + width / 2);
            double oy = (top + height / 2);
            if (this._orientation == DrawBoardOrientation.LeftTop)
            {
                //modified
                oy = this.Height - oy;
            }
            ellipse.Reset(ox,
                          oy,
                          width / 2,
                          height / 2,
                          ellipseGenNSteps);
            var v1 = GetFreeVxs();
            _aggsx.Render(ellipse.MakeVxs(v1), this.fillColor);
            ReleaseVxs(ref v1);
        }
        public override void FillRect(double left, double top, double width, double height)
        {
            double right = left + width;
            double bottom = top - height;
            if (right < left || top < bottom)
            {
#if DEBUG
                throw new ArgumentException();
#else
                return;
#endif
            }



            if (this._orientation == DrawBoardOrientation.LeftBottom)
            {
                _simpleRectVxsGen.SetRect(left + 0.5, bottom + 0.5, right - 0.5, top - 0.5);
            }
            else
            {
                int canvasH = this.Height;
                _simpleRectVxsGen.SetRect(left + 0.5, canvasH - (bottom + 0.5 + height), right - 0.5, canvasH - (top - 0.5 + height));
            }

            var v1 = GetFreeVxs();
            _aggsx.Render(_simpleRectVxsGen.MakeVertexSnap(v1), this.fillColor);
            ReleaseVxs(ref v1);
        }

        public override RequestFont CurrentFont
        {
            get
            {
                return this.currentFont;
            }
            set
            {
                this.currentFont = value;
                //this request font must resolve to actual font
                //within canvas *** 
                //TODO: review drawing string  with agg here 
                if (_textPrinter != null && value != null)
                {
                    _textPrinter.ChangeFont(value);
                }
            }
        }

        public override void DrawString(
           string text,
           double x,
           double y)
        {
            //TODO: review drawing string  with agg here   
            if (_textPrinter != null)
            {
                if (this._orientation == DrawBoardOrientation.LeftBottom)
                {
                    _textPrinter.DrawString(text, x, y);
                }
                else
                {
                    //from current point size 
                    //we need line height of current font size
                    //then we will start on 'base line'

                    _textPrinter.DrawString(text, x, this.Height - y);
                }

            }
        }
        public override void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            //draw string from render vx
            if (_textPrinter != null)
            {
                _textPrinter.DrawString(renderVx, x, y);
            }
        }
        public override RenderVxFormattedString CreateRenderVx(string textspan)
        {

            var renderVxFmtStr = new AggRenderVxFormattedString(textspan);
            if (_textPrinter != null)
            {
                char[] buffer = textspan.ToCharArray();
                _textPrinter.PrepareStringForRenderVx(renderVxFmtStr, buffer, 0, buffer.Length);

            }
            return renderVxFmtStr;
        }

        ITextPrinter _textPrinter;
        public ITextPrinter TextPrinter
        {
            get
            {
                return _textPrinter;
            }
            set
            {
                _textPrinter = value;
                if (_textPrinter != null)
                {
                    _textPrinter.ChangeFont(this.currentFont);
                }
            }

        }
        /// <summary>
        /// fill vertex store, we do NOT store snap
        /// </summary>
        /// <param name="vxs"></param>
        /// <param name="c"></param>
        public override void Fill(VertexStoreSnap snap)
        {
            sclineRas.AddPath(snap);
            sclineRasToBmp.RenderWithColor(this._aggsx.DestImage, sclineRas, scline, fillColor);
        }
        /// <summary>
        /// fill vxs, we do NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        public override void Fill(VertexStore vxs)
        {
            sclineRas.AddPath(vxs);
            try
            {
                sclineRasToBmp.RenderWithColor(this._aggsx.DestImage, sclineRas, scline, fillColor);
            }
            catch (Exception ex)
            {

            }
        }


        public override bool UseSubPixelRendering
        {
            get
            {
                return this.sclineRasToBmp.ScanlineRenderMode == ScanlineRenderMode.SubPixelRendering;
            }
            set
            {
                if (value)
                {
                    //TODO: review here again             
                    this.sclineRas.ExtendX3ForSubPixelRendering = true;
                    this.sclineRasToBmp.ScanlineRenderMode = ScanlineRenderMode.SubPixelRendering;
                }
                else
                {
                    this.sclineRas.ExtendX3ForSubPixelRendering = false;
                    this.sclineRasToBmp.ScanlineRenderMode = ScanlineRenderMode.Default;
                }
            }
        }
        public override Color FillColor
        {
            get { return fillColor; }
            set { this.fillColor = value; }
        }
        public override Color StrokeColor
        {
            get { return strokeColor; }
            set { this.strokeColor = value; }
        }
        public override void PaintSeries(VertexStore vxs, Color[] colors, int[] pathIndexs, int numPath)
        {
            sclineRasToBmp.RenderSolidAllPaths(this._aggsx.DestImage,
                this.sclineRas,
                this.scline,
                vxs,
                colors,
                pathIndexs,
                numPath);
        }
        /// <summary>
        /// we do NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        /// <param name="spanGen"></param>
        public void Fill(VertexStore vxs, ISpanGenerator spanGen)
        {
            this.sclineRas.AddPath(vxs);
            sclineRasToBmp.RenderWithSpan(this._aggsx.DestImage, sclineRas, scline, spanGen);
        }
        public override void DrawImage(Image img, double left, double top)
        {
            //check image caching system
            if (img is ActualImage)
            {
                this.sharedImageWriterReader.ReloadImage((ActualImage)img);
                if (this._orientation == DrawBoardOrientation.LeftTop)
                {
                    //place left upper corner at specific x y
                    this._aggsx.Render(this.sharedImageWriterReader, left, this.Height - (top + img.Height));

                }
                else
                {
                    //left-bottom as original
                    //place left-lower of the img at specific (x,y)
                    this._aggsx.Render(this.sharedImageWriterReader, left, top);
                }

            }
            else
            {
                //TODO:
            }

        }
        public override void DrawImage(Image img, params Transform.AffinePlan[] affinePlans)
        {
            if (img is ActualImage)
            {
                this.sharedImageWriterReader.ReloadImage((ActualImage)img);
                this._aggsx.Render(sharedImageWriterReader, affinePlans);
            }
            else
            {
                //TODO:
            }
        }

        ////----------------------
        ///// <summary>
        ///// do filter at specific area
        ///// </summary>
        ///// <param name="filter"></param>
        ///// <param name="area"></param>
        //public override void DoFilterBlurStack(RectInt area, int r)
        //{
        //    ChildImage img = new ChildImage(this._aggsx.DestImage, _aggsx.PixelBlender,
        //        area.Left, area.Bottom, area.Right, area.Top);
        //    filterMan.DoStackBlur(img, r);
        //}
        //public override void DoFilterBlurRecursive(RectInt area, int r)
        //{
        //    ChildImage img = new ChildImage(this._aggsx.DestImage, _aggsx.PixelBlender,
        //        area.Left, area.Bottom, area.Right, area.Top);
        //    filterMan.DoRecursiveBlur(img, r);
        //}
        //public override void DoFilter(RectInt area, int r)
        //{
        //    ChildImage img = new ChildImage(this._aggsx.DestImage, _aggsx.PixelBlender,
        //      area.Left, area.Top, area.Right, area.Bottom);
        //    filterMan.DoSharpen(img, r);
        //}
        public override void ApplyFilter(ImageFilter imgFilter)
        {
            //TODO: implement this
            //resolve internal img filter
            //switch (imgFilter.Name)
            //{

            //} 
        }
        public override RenderVx CreateRenderVx(VertexStoreSnap snap)
        {
            return new AggRenderVx(snap);
        }
        public override void DrawRenderVx(RenderVx renderVx)
        {
            AggRenderVx aggRenderVx = (AggRenderVx)renderVx;
            Draw(aggRenderVx.snap);
        }
        public override void FillRenderVx(Brush brush, RenderVx renderVx)
        {
            AggRenderVx aggRenderVx = (AggRenderVx)renderVx;
            //fill with brush 
            if (brush is SolidBrush)
            {
                SolidBrush solidBrush = (SolidBrush)brush;
                var prevColor = this.fillColor;
                this.fillColor = solidBrush.Color;
                Fill(aggRenderVx.snap);
                this.fillColor = prevColor;
            }
            else
            {
                Fill(aggRenderVx.snap);
            }
        }
        public override void FillRenderVx(RenderVx renderVx)
        {
            AggRenderVx aggRenderVx = (AggRenderVx)renderVx;
            Fill(aggRenderVx.snap);
        }
        public LineJoin LineJoin
        {
            get { return stroke.LineJoin; }
            set
            {
                stroke.LineJoin = value;
            }
        }
        public LineCap LineCap
        {
            get { return stroke.LineCap; }
            set
            {
                stroke.LineCap = value;
            }
        }
        public LineDashGenerator LineDashGen
        {
            get { return this._lineDashGen; }
            set { this._lineDashGen = value; }
        }

    }
}