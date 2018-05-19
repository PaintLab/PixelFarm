//MIT, 2016-2018, WinterDev

using System;
using System.Collections.Generic;
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
        BitmapBuffer _bxt;

        public AggPainter(AggRenderSurface aggsx)
        {
            //painter paint to target surface
            this._aggsx = aggsx;
            this.sclineRas = _aggsx.ScanlineRasterizer;
            this.stroke = new Stroke(1);//default
            this.scline = aggsx.ScanlinePacked8;
            this.sclineRasToBmp = aggsx.ScanlineRasToDestBitmap;
            _orientation = DrawBoardOrientation.LeftBottom;
            //from membuffer
            _bxt = new BitmapBuffer(aggsx.Width,
                aggsx.Height,
                PixelFarm.Agg.ActualImage.GetBuffer(aggsx.DestActualImage));
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
                        this.RenderQuality = RenderQualtity.HighQuality;
                        _aggsx.UseSubPixelRendering = true;
                        break;
                    case Drawing.SmoothingMode.HighSpeed:
                    default:
                        this.RenderQuality = RenderQualtity.Fast;
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
            //BitmapExt
            if (this.RenderQuality == RenderQualtity.Fast)
            {
                this._bxt.DrawLine(
                    (int)Math.Round(x1),
                    (int)Math.Round(y1),
                    (int)Math.Round(x2),
                    (int)Math.Round(y2),
                    this.strokeColor.ToARGB()
                    );

                return;
            }

            //----------------------------------------------------------
            //Agg
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

            //BitmapExt
            if (this.RenderQuality == RenderQualtity.Fast)
            {

                if (this._orientation == DrawBoardOrientation.LeftBottom)
                {

                    this._bxt.DrawRectangle(
                    (int)Math.Round(left),
                    (int)Math.Round(top),
                    (int)Math.Round(left + width),
                    (int)Math.Round(top + height),

                    ColorInt.FromArgb(this.strokeColor.ToARGB()));
                }
                else
                {
                    //TODO: review here
                    throw new NotSupportedException();
                    //int canvasH = this.Height; 
                    ////_simpleRectVxsGen.SetRect(left + 0.5, canvasH - (bottom + 0.5 + height), right - 0.5, canvasH - (top - 0.5 + height));
                    //this._bmpBuffer.DrawRectangle(
                    //(int)Math.Round(left),
                    //(int)Math.Round(top),
                    //(int)Math.Round(left + width),
                    //(int)Math.Round(top + height),
                    //ColorInt.FromArgb(this.strokeColor.ToARGB()));
                }
                return; //exit
            }

            //----------------------------------------------------------
            //Agg
            if (this._orientation == DrawBoardOrientation.LeftBottom)
            {
                double right = left + width;
                double bottom = top + height;
                _simpleRectVxsGen.SetRect(left + 0.5, bottom + 0.5, right - 0.5, top - 0.5);
            }
            else
            {
                double right = left + width;
                double bottom = top - height;
                int canvasH = this.Height;
                //_simpleRectVxsGen.SetRect(left + 0.5, canvasH - (bottom + 0.5), right - 0.5, canvasH - (top - 0.5));
                _simpleRectVxsGen.SetRect(left + 0.5, canvasH - (bottom + 0.5 + height), right - 0.5, canvasH - (top - 0.5 + height));
            }

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

            //---------------------------------------------------------- 
            //BitmapExt
            if (this._renderQuality == RenderQualtity.Fast)
            {

                this._bxt.DrawEllipseCentered(
                   (int)Math.Round(ox), (int)Math.Round(oy),
                   (int)Math.Round(width / 2),
                   (int)Math.Round(height / 2),
                   this.strokeColor.ToARGB());

                return;
            }

            //Agg
            //---------------------------------------------------------- 
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
            //---------------------------------------------------------- 
            //BitmapExt
            if (this._renderQuality == RenderQualtity.Fast)
            {
                this._bxt.FillEllipseCentered(
                   (int)Math.Round(ox), (int)Math.Round(oy),
                   (int)Math.Round(width / 2),
                   (int)Math.Round(height / 2),
                   this.fillColor.ToARGB());
                return;
            }


            //Agg
            //---------------------------------------------------------- 
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


            //---------------------------------------------------------- 
            //BitmapExt
            if (this._renderQuality == RenderQualtity.Fast)
            {
                this._bxt.FillRectangle(
                      (int)Math.Round(left),
                      (int)Math.Round(top),
                      (int)Math.Round(left + width),
                      (int)Math.Round(top + height),

                      ColorInt.FromArgb(this.fillColor.ToARGB()));
                return;
            }

            //Agg
            //---------------------------------------------------------- 
            if (this._orientation == DrawBoardOrientation.LeftBottom)
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

                _simpleRectVxsGen.SetRect(left + 0.5, (bottom + 0.5) + height, right - 0.5, (top - 0.5) + height);
            }
            else
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


        List<int> _reusablePolygonList = new List<int>();
        /// <summary>
        /// fill with BitmapBufferExtension lib
        /// </summary>
        void FillWithBxt(VertexStoreSnap snap)
        {
            //transate the vxs/snap to command
            double x = 0;
            double y = 0;
            double offsetOrgX = this.OriginX;
            double offsetOrgY = this.OriginY;

            VertexSnapIter snapIter = snap.GetVertexSnapIter();
            VertexCmd cmd;

            int latestMoveToX = 0, latestMoveToY = 0;
            int latestX = 0, latestY = 0;


            bool closed = false;

            _reusablePolygonList.Clear();

            while ((cmd = snapIter.GetNextVertex(out x, out y)) != VertexCmd.NoMore)
            {
                x += offsetOrgX;
                y += offsetOrgY;

                switch (cmd)
                {
                    case VertexCmd.MoveTo:
                        {
                            if (_reusablePolygonList.Count > 0)
                            {
                                //no drawline
                                _reusablePolygonList.Clear();
                            }

                            closed = false;
                            _reusablePolygonList.Add(latestMoveToX = latestX = (int)Math.Round(x));
                            _reusablePolygonList.Add(latestMoveToY = latestY = (int)Math.Round(y));

                        }
                        break;
                    case VertexCmd.LineTo:
                    case VertexCmd.P2c:
                    case VertexCmd.P3c:
                        {
                            //collect to the polygon
                            _reusablePolygonList.Add(latestX = (int)Math.Round(x));
                            _reusablePolygonList.Add(latestY = (int)Math.Round(y));
                        }
                        break;
                    case VertexCmd.Close:
                    case VertexCmd.CloseAndEndFigure:
                        {
                            if (_reusablePolygonList.Count > 0)
                            {
                                //flush by draw line
                                _reusablePolygonList.Add(latestX = latestMoveToX);
                                _reusablePolygonList.Add(latestY = latestMoveToY);

                                _bxt.FillPolygon(_reusablePolygonList.ToArray(),
                                    this.fillColor.ToARGB());
                            }

                            _reusablePolygonList.Clear();
                            closed = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            //---------------
            if (!closed && (_reusablePolygonList.Count > 0) &&
               (latestX == latestMoveToX) && (latestY == latestMoveToY))
            {

                //flush by draw line
                _reusablePolygonList.Add(latestMoveToX);
                _reusablePolygonList.Add(latestMoveToY);

                _bxt.FillPolygon(_reusablePolygonList.ToArray(),
                    this.fillColor.ToARGB());


            }
        }
        /// <summary>
        /// fill vertex store, we do NOT store snap
        /// </summary>
        /// <param name="vxs"></param>
        /// <param name="c"></param>
        public override void Fill(VertexStoreSnap snap)
        {

            //BitmapExt
            if (this._renderQuality == RenderQualtity.Fast)
            {
                FillWithBxt(snap);
                return;
            }

            //Agg
            sclineRas.AddPath(snap);
            sclineRasToBmp.RenderWithColor(this._aggsx.DestImage, sclineRas, scline, fillColor);
        }
        /// <summary>
        /// fill vxs, we do NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        public override void Fill(VertexStore vxs)
        {
            //
            if (this._renderQuality == RenderQualtity.Fast)
            {
                FillWithBxt(new VertexStoreSnap(vxs));
                return;
            }

            //
            sclineRas.AddPath(vxs);
            sclineRasToBmp.RenderWithColor(this._aggsx.DestImage, sclineRas, scline, fillColor);
        }


        public override bool UseSubPixelLcdEffect
        {
            get
            {
                return this.sclineRas.ExtendWidthX3ForSubPixelLcdEffect;
            }
            set
            {
                if (value)
                {
                    //TODO: review here again             
                    this.sclineRas.ExtendWidthX3ForSubPixelLcdEffect = true;
                    this.sclineRasToBmp.ScanlineRenderMode = ScanlineRenderMode.SubPixelLcdEffect;
                }
                else
                {
                    this.sclineRas.ExtendWidthX3ForSubPixelLcdEffect = false;
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
            ActualImage actualImg = img as ActualImage;
            if (actualImg == null)
            {
                //? TODO
                return;
            }
            //check image caching system 
            if (this._renderQuality == RenderQualtity.Fast)
            {
                //DrawingBuffer.RectD destRect = new DrawingBuffer.RectD(left, top, img.Width, img.Height);
                //DrawingBuffer.RectD srcRect = new DrawingBuffer.RectD(0, 0, img.Width, img.Height);
                BitmapBuffer srcBmp = new BitmapBuffer(img.Width, img.Height, ActualImage.GetBuffer(actualImg));
                this._bxt.CopyBlit(left, top, srcBmp);
                return;
            }

            this.sharedImageWriterReader.ReloadImage(actualImg);

            //save, restore later... 
            bool useSubPix = UseSubPixelLcdEffect;
            //before render an image we turn off vxs subpixel rendering
            this.UseSubPixelLcdEffect = false;
            _aggsx.UseSubPixelRendering = false;

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

            //restore...
            this.UseSubPixelLcdEffect = useSubPix;
            _aggsx.UseSubPixelRendering = useSubPix;



        }
        public override void DrawImage(Image img, params Transform.AffinePlan[] affinePlans)
        {
            ActualImage actualImg = img as ActualImage;
            if (actualImg == null)
            {
                //? TODO
                return;
            }

            //if (this._renderQuality == RenderQualtity.Fast)
            //{
            //    //todo, review here again
            //    BitmapBuffer srcBmp = new BitmapBuffer(img.Width, img.Height, ActualImage.GetBuffer(actualImg));
            //    //this._bxt.CopyBlit(left, top, srcBmp); 
            //    DrawingBuffer.MatrixTransform mx = new MatrixTransform(new DrawingBuffer.AffinePlan[]{
            //        DrawingBuffer.AffinePlan.Translate(-img.Width/2,-img.Height/2),
            //        DrawingBuffer.AffinePlan.Rotate(AggMath.deg2rad(-70))
            //        //DrawingBuffer.AffinePlan.Translate(100,100)
            //    });
            //    //DrawingBuffer.MatrixTransform mx = new MatrixTransform(DrawingBuffer.Affine.IdentityMatrix);
            //    this._bxt.BlitRender(srcBmp, false, 1, mx);
            //    return;
            //}


            this.sharedImageWriterReader.ReloadImage((ActualImage)img);

            bool useSubPix = UseSubPixelLcdEffect; //save, restore later... 
                                                   //before render an image we turn off vxs subpixel rendering
            this.UseSubPixelLcdEffect = false;
            _aggsx.UseSubPixelRendering = false;


            this._aggsx.Render(sharedImageWriterReader, affinePlans);



            //restore...
            this.UseSubPixelLcdEffect = useSubPix;
            _aggsx.UseSubPixelRendering = useSubPix;

        }

        public override void ApplyFilter(ImageFilter imgFilter)
        {
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