//MIT, 2016-present, WinterDev 
using System;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using SkiaSharp;
namespace PixelFarm.Drawing.Skia
{

    public class SkiaPainter : Painter
    {
        RectInt _clipBox;
        Color _fillColor;
        Color _strokeColor;
        double _strokeWidth;
        bool _useSubPixelRendering;
        RequestFont _currentFont;
        Brush _currentBrush;
        int _height;
        int _width;
        CpuBlit.VertexProcessing.RoundedRect roundRect;
        SmoothingMode _smoothingMode;
        //-----------------------
        SKCanvas _skCanvas;
        SKPaint _fill;
        SKPaint _stroke;
        //-----------------------

        public SkiaPainter(int w, int h)
        {

            _fill = new SKPaint();
            _stroke = new SKPaint();
            _stroke.IsStroke = true;
            _width = w;
            _height = h;
        }
        public override float FillOpacity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool EnableMask { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override TargetBuffer TargetBuffer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override void SetClipRgn(VertexStore vxs)
        {
            throw new NotImplementedException();
        }
        public override FillingRule FillingRule { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override void Render(RenderVx renderVx)
        {
            throw new NotImplementedException();
        }
        public override void Fill(Region rgn)
        {
            throw new NotImplementedException();
        }
        public override void Draw(Region rgn)
        {
            throw new NotImplementedException();
        }

        public override Brush CurrentBrush
        {
            get => _currentBrush;
            set => _currentBrush = value;
        }

        Pen _curPen;
        public override Pen CurrentPen
        {
            get
            {
                throw new NotSupportedException();
                return _curPen;
            }
            set
            {
                throw new NotSupportedException();
                _curPen = value;
            }
        }
        public SKCanvas Canvas
        {
            get { return _skCanvas; }
            set { _skCanvas = value; }
        }
        RenderQuality _renderQuality;
        public override RenderQuality RenderQuality
        {
            get { return _renderQuality; }
            set { _renderQuality = value; }
        }

        RenderSurfaceOrientation _orientation;
        public override RenderSurfaceOrientation Orientation
        {
            get { return _orientation; }
            set
            { _orientation = value; }
        }
        public override float OriginX
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override float OriginY
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override void SetOrigin(float ox, float oy)
        {
            throw new NotImplementedException();
        }
        static bool defaultAntiAlias = false;
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
                    case SmoothingMode.AntiAlias:
                        _fill.IsAntialias = _stroke.IsAntialias = true;
                        break;
                    default:
                        _fill.IsAntialias = _stroke.IsAntialias = defaultAntiAlias;
                        break;
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
                //set clip rect to canvas ***
            }
        }

        public override RequestFont CurrentFont
        {
            get
            {
                return _currentFont;
            }

            set
            {
                _currentFont = value;
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
                _fill.Color = ConvToSkColor(_fillColor = value);
            }
        }

        public override int Height
        {
            get
            {
                return _height;
            }
        }
        static SKColor ConvToSkColor(PixelFarm.Drawing.Color c)
        {
            return new SKColor(c.R, c.G, c.B, c.A);
        }
        public override Color StrokeColor
        {
            get
            {
                return _strokeColor;
            }
            set
            {
                _stroke.Color = ConvToSkColor(_strokeColor = value);
            }
        }
        public override double StrokeWidth
        {
            get
            {
                return _strokeWidth;
            }
            set
            {
                _stroke.StrokeWidth = (float)(_strokeWidth = value);
            }
        }

        public override bool UseSubPixelLcdEffect
        {
            get
            {
                return _useSubPixelRendering;
            }
            set
            {
                _useSubPixelRendering = value;
            }
        }

        public override int Width
        {
            get
            {
                return _width;
            }
        }

        public override LineJoin LineJoin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override LineCap LineCap { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IDashGenerator LineDashGen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Clear(Color color)
        {

            _skCanvas.Clear(ConvToSkColor(color));
        }
        public override void ApplyFilter(PixelFarm.Drawing.IImageFilter imgFilter)
        {
            throw new NotImplementedException();
        }
        //public override void DoFilterBlurRecursive(RectInt area, int r)
        //{
        //    //TODO: implement this
        //} 
        //public override void DoFilter(RectInt area, int r)
        //{

        //}

        public override void Draw(VertexStore vxs)
        {
            VxsHelper.DrawVxsSnap(_skCanvas, vxs, _stroke);
        }
        //public override void DrawBezierCurve(float startX, float startY, float endX, float endY, float controlX1, float controlY1, float controlX2, float controlY2)
        //{
        //    using (SKPath p = new SKPath())
        //    {
        //        p.MoveTo(startX, startY);
        //        p.CubicTo(controlX1, controlY1,
        //            controlY1, controlY2,
        //            endX, endY);
        //        _skCanvas.DrawPath(p, _stroke);
        //    }
        //}
        public override void DrawImage(Image actualImage)
        {
            throw new NotImplementedException();
        }
        public override void DrawImage(Image actualImage, params AffinePlan[] affinePlans)
        {
            //1. create special graphics 
            throw new NotSupportedException();
            //using (System.Drawing.Bitmap srcBmp = CreateBmpBRGA(actualImage))
            //{
            //    var bmp = _bmpStore.GetFreeBmp();
            //    using (var g2 = System.Drawing.Graphics.FromImage(bmp))
            //    {
            //        //we can use recycle tmpVxsStore
            //        Affine destRectTransform = Affine.NewMatix(affinePlans);
            //        double x0 = 0, y0 = 0, x1 = bmp.Width, y1 = bmp.Height;
            //        destRectTransform.Transform(ref x0, ref y0);
            //        destRectTransform.Transform(ref x0, ref y1);
            //        destRectTransform.Transform(ref x1, ref y1);
            //        destRectTransform.Transform(ref x1, ref y0);
            //        var matrix = new System.Drawing.Drawing2D.Matrix(
            //           (float)destRectTransform.m11, (float)destRectTransform.m12,
            //           (float)destRectTransform.m21, (float)destRectTransform.m22,
            //           (float)destRectTransform.dx, (float)destRectTransform.dy);
            //        g2.Clear(System.Drawing.Color.Transparent);
            //        g2.Transform = matrix;
            //        //------------------------
            //        g2.DrawImage(srcBmp, new System.Drawing.PointF(0, 0));
            //        _gfx.DrawImage(bmp, new System.Drawing.Point(0, 0));
            //    }
            //    _bmpStore.RelaseBmp(bmp);
            //}
        }
        public override void DrawImage(Image actualImage, double left, double top, int srcX, int srcY, int srcW, int srcH)
        {
            throw new NotImplementedException();
        }
        public override void DrawImage(Image actualImage, double left, double top, ICoordTransformer coordTx)
        {
            throw new NotImplementedException();
        }
        public override void DrawImage(Image img, double left, double top)
        {
            if (img is MemBitmap)
            {
                MemBitmap memBmp = (MemBitmap)img;
                //create Gdi bitmap from actual image
                int w = memBmp.Width;
                int h = memBmp.Height;
                switch (memBmp.PixelFormat)
                {
                    case CpuBlit.Imaging.PixelFormat.ARGB32:
                        {

                            using (SKBitmap newBmp = new SKBitmap(memBmp.Width, memBmp.Height))
                            {
                                newBmp.LockPixels();
                                //byte[] actualImgBuffer = ActualImage.GetBuffer(actualImage);
                                CpuBlit.Imaging.TempMemPtr bufferPtr = MemBitmap.GetBufferPtr(memBmp);
                                unsafe
                                {
                                    byte* actualImgH = (byte*)bufferPtr.Ptr;
                                    MemMx.memcpy((byte*)newBmp.GetPixels(), actualImgH, memBmp.Stride * memBmp.Height);
                                    //System.Runtime.InteropServices.Marshal.Copy(
                                    //    actualImgBuffer,
                                    //    0,
                                    //    newBmp.GetPixels(),
                                    //    actualImgBuffer.Length); 
                                }
                                bufferPtr.Dispose();
                                newBmp.UnlockPixels();
                            }
                            //newBmp.internalBmp.LockPixels();
                            //byte[] actualImgBuffer = ActualImage.GetBuffer(actualImage);

                            //System.Runtime.InteropServices.Marshal.Copy(
                            //     actualImgBuffer,
                            //     0,
                            //      newBmp.internalBmp.GetPixels(),
                            //      actualImgBuffer.Length);

                            //newBmp.internalBmp.UnlockPixels();
                            //return newBmp;

                            //copy data from acutal buffer to internal representation bitmap
                            //using (MySkBmp bmp = MySkBmp.CopyFrom(actualImage))
                            //{
                            //    _skCanvas.DrawBitmap(bmp.internalBmp, (float)x, (float)y);
                            //}
                        }
                        break;
                    case CpuBlit.Imaging.PixelFormat.RGB24:
                        {
                        }
                        break;
                    case CpuBlit.Imaging.PixelFormat.GrayScale8:
                        {
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
        public override void DrawString(string text, double x, double y)
        {
            //use current brush and font
            _skCanvas.DrawText(text, (float)x, (float)y, _stroke);

            //_skCanvas.ResetMatrix();
            //_skCanvas.Translate(0.0F, (float)Height);// Translate the drawing area accordingly   


            ////draw with native win32
            ////------------

            ///*_gfx.DrawString(text,
            //    _latestWinGdiPlusFont.InnerFont,
            //    _currentFillBrush,
            //    new System.Drawing.PointF((float)x, (float)y));
            //*/
            ////------------
            ////restore back
            //_skCanvas.ResetMatrix();//again
            //_skCanvas.Scale(1f, -1f);// Flip the Y-Axis
            //_skCanvas.Translate(0.0F, -(float)Height);// Translate the drawing area accordingly                             
        }
        public override void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            //TODO: review here again 
            _skCanvas.DrawText(renderVx.OriginalString, (float)x, (float)y, _stroke);
        }
        public override RenderVxFormattedString CreateRenderVx(string textspan)
        {
            return new SkiaRenerVxFormattedString(textspan);
        }
        /// <summary>
        /// we do NOT store snap/vxs
        /// </summary>
        /// <param name="vxs"></param>
        public override void Fill(VertexStore vxs)
        {
            VxsHelper.FillVxsSnap(_skCanvas, vxs, _fill);
        }

        public override void FillEllipse(double left, double top, double width, double height)
        {
            _skCanvas.DrawOval(
                new SKRect((float)left, (float)top, (float)(left + width), (float)(top + height)),
                _fill);
        }
        public override void DrawEllipse(double left, double top, double width, double height)
        {
            _skCanvas.DrawOval(
             new SKRect((float)left, (float)top, (float)(left + width), (float)(top + height)),
              _stroke);
        }
        public override void FillRect(double left, double top, double width, double height)
        {

            _skCanvas.DrawRect(
               new SKRect((float)left, (float)top, (float)(left + width), (float)(top + height)),
                _fill);
        }

        //public override void FillRectLBWH(double left, double bottom, double width, double height)
        //{

        //    _skCanvas.DrawRect(
        //      new SKRect((float)left, (float)(bottom - height), (float)(left + width), (float)bottom),
        //        _fill);
        //}

        //VertexStorePool _vxsPool = new VertexStorePool();
        //VertexStore GetFreeVxs()
        //{

        //    return _vxsPool.GetFreeVxs();
        //}
        //void ReleaseVxs(ref VertexStore vxs)
        //{
        //    _vxsPool.Release(ref vxs);
        //}
        //public override void DrawRoundRect(double left, double bottom, double right, double top, double radius)
        //{
        //    if (roundRect == null)
        //    {
        //        roundRect = new PixelFarm.Agg.VertexSource.RoundedRect(left, bottom, right, top, radius);
        //        roundRect.NormalizeRadius();
        //    }
        //    else
        //    {
        //        roundRect.SetRect(left, bottom, right, top);
        //        roundRect.SetRadius(radius);
        //        roundRect.NormalizeRadius();
        //    }

        //    var v1 = GetFreeVxs();
        //    this.Draw(roundRect.MakeVxs(v1));
        //    ReleaseVxs(ref v1);
        //}
        //public override void FillRoundRectangle(double left, double bottom, double right, double top, double radius)
        //{
        //    if (roundRect == null)
        //    {
        //        roundRect = new PixelFarm.Agg.VertexSource.RoundedRect(left, bottom, right, top, radius);
        //        roundRect.NormalizeRadius();
        //    }
        //    else
        //    {
        //        roundRect.SetRect(left, bottom, right, top);
        //        roundRect.SetRadius(radius);
        //        roundRect.NormalizeRadius();
        //    }
        //    var v1 = GetFreeVxs();
        //    this.Fill(roundRect.MakeVxs(v1));
        //    ReleaseVxs(ref v1);
        //}
        public override void DrawLine(double x1, double y1, double x2, double y2)
        {
            _skCanvas.DrawLine((float)x1, (float)y1, (float)x2, (float)y2, _stroke);
        }



        public override void DrawRect(double left, double bottom, double right, double top)
        {
            _skCanvas.DrawLine((float)left, (float)top, (float)right, (float)bottom, _stroke);
        }

        public override void SetClipBox(int x1, int y1, int x2, int y2)
        {
            _skCanvas.ClipRect(new SKRect(x1, y1, x2, y2));
        }
        public override RenderVx CreateRenderVx(VertexStore vxs)
        {
            var renderVx = new WinGdiRenderVx(vxs);
            renderVx.path = VxsHelper.CreateGraphicsPath(vxs);
            return renderVx;
        }

        public override void FillRenderVx(Brush brush, RenderVx renderVx)
        {
            //TODO: review brush implementation here
            WinGdiRenderVx wRenderVx = (WinGdiRenderVx)renderVx;
            VxsHelper.FillPath(_skCanvas, wRenderVx.path, _fill);
        }
        public override void DrawRenderVx(RenderVx renderVx)
        {
            WinGdiRenderVx wRenderVx = (WinGdiRenderVx)renderVx;
            VxsHelper.DrawPath(_skCanvas, wRenderVx.path, _stroke);
        }
        public override void FillRenderVx(RenderVx renderVx)
        {
            WinGdiRenderVx wRenderVx = (WinGdiRenderVx)renderVx;
            VxsHelper.FillPath(_skCanvas, wRenderVx.path, _fill);
        }
    }
}