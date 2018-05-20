//MIT, 2016-2018, WinterDev

using System;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;
using PixelFarm.Drawing.PainterExtensions;

namespace PixelFarm.Drawing.WinGdi
{
    public class GdiPlusPainter : Painter
    {


        System.Drawing.Pen _currentPen;
        System.Drawing.SolidBrush _currentFillBrush;

        GdiPlusRenderSurface _renderSurface;
        PixelFarm.Agg.VectorTool _vectorTool;

        public GdiPlusPainter(GdiPlusRenderSurface renderSurface)
        {
            this._renderSurface = renderSurface;

            _currentPen = new System.Drawing.Pen(System.Drawing.Color.Black);
            _currentFillBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            _vectorTool = new PixelFarm.Agg.VectorTool();
        }
        public override PainterExtensions.VectorTool VectorTool
        {
            get { return _vectorTool; }
        }

        public System.Drawing.Drawing2D.CompositingMode CompositingMode
        {
            get { return _renderSurface.gx.CompositingMode; }
            set { _renderSurface.gx.CompositingMode = value; }
        }


        RenderQualtity _renderQuality;
        public override RenderQualtity RenderQuality
        {
            get { return _renderQuality; }
            set { _renderQuality = value; }
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

        public override int Width
        {
            get
            {
                return _renderSurface.Width;
            }
        }

        public override int Height
        {
            get
            {
                return _renderSurface.Height;
            }
        }


        RectInt _clipBox;
        public override RectInt ClipBox
        {
            get { return _clipBox; }
            set
            {
                //TODO: implement here
                _clipBox = value;
            }
        }

        public override double StrokeWidth
        {
            get
            {
                return _currentPen.Width;
            }
            set
            {
                _currentPen.Width = (float)value;
            }
        }
        public override SmoothingMode SmoothingMode
        {
            get
            {
                return _renderSurface.SmoothingMode;
            }
            set
            {
                _renderSurface.SmoothingMode = value;
            }
        }

        bool _useSubPixelRendering;//for text
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
        Color _fillColor;
        public override Color FillColor
        {

            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
                _currentFillBrush.Color = GdiPlusRenderSurface.ConvColor(value);
            }
        }
        Color _strokeColor;
        public override Color StrokeColor
        {
            get { return _strokeColor; }
            set
            {
                _strokeColor = value;
                _currentPen.Color = GdiPlusRenderSurface.ConvColor(value);
            }
        }
        public override DrawBoardOrientation Orientation
        {
            get { return DrawBoardOrientation.LeftTop; }
            set
            {
                //TODO: implement 
            }
        }
        public override RequestFont CurrentFont
        {
            get { return _renderSurface.CurrentFont; }
            set
            {
                _renderSurface.CurrentFont = value;
            }
        }

        public override void Clear(Color color)
        {
            _renderSurface.Clear(color);
        }

        public override RenderVx CreateRenderVx(VertexStoreSnap snap)
        {
            var renderVx = new WinGdiRenderVx(snap);
            renderVx.path = VxsHelper.CreateGraphicsPath(snap);
            return renderVx;
        }
        public override RenderVxFormattedString CreateRenderVx(string textspan)
        {
            return new WinGdiRenderVxFormattedString(textspan.ToCharArray());
        }

        //public override void DoFilterBlurRecursive(RectInt area, int r)
        //{

        //}
        //public override void DoFilter(RectInt area, int r)
        //{

        //}
        //public override void DoFilterBlurStack(RectInt area, int r)
        //{

        //    //    public override void DoFilterBlurStack(RectInt area, int r)
        //    //    {
        //    //        //since area is Windows coord
        //    //        //so we need to invert it 
        //    //        //System.Drawing.Bitmap backupBmp = this._gfxBmp;
        //    //        //int bmpW = backupBmp.Width;
        //    //        //int bmpH = backupBmp.Height;
        //    //        //System.Drawing.Imaging.BitmapData bmpdata = backupBmp.LockBits(
        //    //        //    new System.Drawing.Rectangle(0, 0, bmpW, bmpH),
        //    //        //    System.Drawing.Imaging.ImageLockMode.ReadWrite,
        //    //        //     backupBmp.PixelFormat);
        //    //        ////copy sub buffer to int32 array
        //    //        ////this version bmpdata must be 32 argb 
        //    //        //int a_top = area.Top;
        //    //        //int a_bottom = area.Bottom;
        //    //        //int a_width = area.Width;
        //    //        //int a_stride = bmpdata.Stride;
        //    //        //int a_height = Math.Abs(area.Height);
        //    //        //int[] src_buffer = new int[(a_stride / 4) * a_height];
        //    //        //int[] destBuffer = new int[src_buffer.Length];
        //    //        //int a_lineOffset = area.Left * 4;
        //    //        //unsafe
        //    //        //{
        //    //        //    IntPtr scan0 = bmpdata.Scan0;
        //    //        //    byte* src = (byte*)scan0;
        //    //        //    if (a_top > a_bottom)
        //    //        //    {
        //    //        //        int tmp_a_bottom = a_top;
        //    //        //        a_top = a_bottom;
        //    //        //        a_bottom = tmp_a_bottom;
        //    //        //    }

        //    //        //    //skip  to start line
        //    //        //    src += ((a_stride * a_top) + a_lineOffset);
        //    //        //    int index_start = 0;
        //    //        //    for (int y = a_top; y < a_bottom; ++y)
        //    //        //    {
        //    //        //        //then copy to int32 buffer 
        //    //        //        System.Runtime.InteropServices.Marshal.Copy(new IntPtr(src), src_buffer, index_start, a_width);
        //    //        //        index_start += a_width;
        //    //        //        src += (a_stride + a_lineOffset);
        //    //        //    }
        //    //        //    PixelFarm.Agg.Imaging.StackBlurARGB.FastBlur32ARGB(src_buffer, destBuffer, a_width, a_height, r);
        //    //        //    //then copy back to bmp
        //    //        //    index_start = 0;
        //    //        //    src = (byte*)scan0;
        //    //        //    src += ((a_stride * a_top) + a_lineOffset);
        //    //        //    for (int y = a_top; y < a_bottom; ++y)
        //    //        //    {
        //    //        //        //then copy to int32 buffer 
        //    //        //        System.Runtime.InteropServices.Marshal.Copy(destBuffer, index_start, new IntPtr(src), a_width);
        //    //        //        index_start += a_width;
        //    //        //        src += (a_stride + a_lineOffset);
        //    //        //    }
        //    //        //}
        //    //        ////--------------------------------
        //    //        //backupBmp.UnlockBits(bmpdata);
        //    //    }
        //}
        public override void ApplyFilter(ImageFilter imgFilter)
        {
            throw new NotImplementedException();
        }
        public override void Draw(VertexStore vxs)
        {

            //        for (int i = 0; i < numPath; ++i)
            //        {
            //            VxsHelper.FillVxsSnap(_gfx, new VertexStoreSnap(vxs, pathIndexs[i]), colors[i]);
            //        }
            // throw new NotImplementedException();
        }

        public override void Draw(VertexStoreSnap vxs)
        {
            this.Fill(vxs);
        }

        public override void DrawEllipse(double left, double top, double width, double height)
        {
            _renderSurface.gx.DrawEllipse(_currentPen, new System.Drawing.RectangleF((float)left, (float)top, (float)width, (float)height));

        }

        public override void DrawImage(Image img, double left, double top)
        {


            //        if (this._orientation == DrawBoardOrientation.LeftTop)
            //        {
            //            this._gfx.DrawImage(bmp, new System.Drawing.Point((int)x, this.Height - (int)y - bmp.Height));
            //        }
            //        else
            //        {
            //            this._gfx.DrawImage(bmp, new System.Drawing.Point((int)x, (int)y));
            //        }



            //if (img is ActualImage)
            //{
            //    ActualImage actualImage = (ActualImage)img;
            //    //create Gdi bitmap from actual image
            //    //int w = actualImage.Width;
            //    //int h = actualImage.Height;
            //    switch (actualImage.PixelFormat)
            //    {
            //        case Agg.PixelFormat.ARGB32:
            //            {
            //                //copy data from acutal buffer to internal representation bitmap
            //                var bmp = ResolveForActualBitmap(actualImage);
            //                if (bmp != null)
            //                {
            //                    if (this._orientation == DrawBoardOrientation.LeftTop)
            //                    {
            //                        this._gfx.DrawImageUnscaled(bmp, new System.Drawing.Point((int)left, (int)top));
            //                    }
            //                    else
            //                    {
            //                        this._gfx.DrawImageUnscaled(bmp, new System.Drawing.Point((int)left, (int)(this.Height - top - img.Height)));
            //                    }
            //                }
            //            }
            //            break;
            //        case Agg.PixelFormat.RGB24:
            //            {
            //            }
            //            break;
            //        case Agg.PixelFormat.GrayScale8:
            //            {
            //            }
            //            break;
            //        default:
            //            throw new NotSupportedException();
            //    }
            //}
        }
        //    static System.Drawing.Bitmap ResolveForActualBitmap(ActualImage actualImage)
        //    {
        //        var cacheBmp = Image.GetCacheInnerImage(actualImage) as System.Drawing.Bitmap;
        //        if (cacheBmp != null)
        //        {
        //            return cacheBmp;
        //        }
        //        else
        //        {
        //            //no cached gdi image 
        //            //so we create a new one
        //            //and cache it for later use


        //            int w = actualImage.Width;
        //            int h = actualImage.Height;
        //            //copy data to bitmap
        //            //bgra  
        //            var bmp = new System.Drawing.Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //            Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(actualImage, bmp);
        //            Image.SetCacheInnerImage(actualImage, bmp);
        //            return bmp;
        //            //GLBitmap glBmp = null;
        //            //if (image is ActualImage)
        //            //{
        //            //    ActualImage actualImage = (ActualImage)image;
        //            //    glBmp = new GLBitmap(actualImage.Width, actualImage.Height, ActualImage.GetBuffer(actualImage), false);
        //            //}
        //            //else
        //            //{
        //            //    //TODO: review here
        //            //    //we should create 'borrow' method ? => send direct exact ptr to img buffer 
        //            //    //for now, create a new one -- after we copy we, don't use it 
        //            //    var req = new Image.ImgBufferRequestArgs(32, Image.RequestType.Copy);
        //            //    image.RequestInternalBuffer(ref req);
        //            //    byte[] copy = req.OutputBuffer;
        //            //    glBmp = new GLBitmap(image.Width, image.Height, copy, req.IsInvertedImage);
        //            //}

        //            //Image.SetCacheInnerImage(image, glBmp);
        //            //return glBmp;
        //        }
        //    }
        public override void DrawImage(Image actualImage, params AffinePlan[] affinePlans)
        {
            throw new NotImplementedException();

            //        //1. create special graphics 
            //        throw new NotSupportedException();

            //        //using (System.Drawing.Bitmap srcBmp = CreateBmpBRGA(actualImage))
            //        //{
            //        //    var bmp = _bmpStore.GetFreeBmp();
            //        //    using (var g2 = System.Drawing.Graphics.FromImage(bmp))
            //        //    {
            //        //        //we can use recycle tmpVxsStore
            //        //        Affine destRectTransform = Affine.NewMatix(affinePlans);
            //        //        double x0 = 0, y0 = 0, x1 = bmp.Width, y1 = bmp.Height;
            //        //        destRectTransform.Transform(ref x0, ref y0);
            //        //        destRectTransform.Transform(ref x0, ref y1);
            //        //        destRectTransform.Transform(ref x1, ref y1);
            //        //        destRectTransform.Transform(ref x1, ref y0);
            //        //        var matrix = new System.Drawing.Drawing2D.Matrix(
            //        //           (float)destRectTransform.m11, (float)destRectTransform.m12,
            //        //           (float)destRectTransform.m21, (float)destRectTransform.m22,
            //        //           (float)destRectTransform.dx, (float)destRectTransform.dy);
            //        //        g2.Clear(System.Drawing.Color.Transparent);
            //        //        g2.Transform = matrix;
            //        //        //------------------------
            //        //        g2.DrawImage(srcBmp, new System.Drawing.PointF(0, 0));
            //        //        this._gfx.DrawImage(bmp, new System.Drawing.Point(0, 0));
            //        //    }
            //        //    _bmpStore.RelaseBmp(bmp);
            //        //}
        }

        public override void DrawLine(double x1, double y1, double x2, double y2)
        {
            _renderSurface.gx.DrawLine(_currentPen, new System.Drawing.PointF((float)x1, (float)y1), new System.Drawing.PointF((float)x2, (float)y2));
        }

        public override void DrawRect(double left, double top, double width, double height)
        {
            _renderSurface.gx.DrawRectangle(_currentPen, (float)left, (float)top, (float)width, (float)height);
        }

        public override void DrawRenderVx(RenderVx renderVx)
        {
            WinGdiRenderVx wRenderVx = (WinGdiRenderVx)renderVx;
            VxsHelper.DrawPath(_renderSurface.gx, wRenderVx.path, this._strokeColor);
        }

        public override void DrawString(string text, double x, double y)
        {
            //TODO: review here
            _renderSurface.DrawText(text.ToCharArray(), (int)x, (int)y);


            //    public override void DrawString(string text, double x, double y)
            //    {
            //        ////use current brush and font
            //        //_gfx.ResetTransform();
            //        //_gfx.TranslateTransform(0.0F, (float)Height);// Translate the drawing area accordingly   

            //        ////draw with native win32
            //        ////------------

            //        ///*_gfx.DrawString(text,
            //        //    _latestWinGdiPlusFont.InnerFont,
            //        //    _currentFillBrush,
            //        //    new System.Drawing.PointF((float)x, (float)y));
            //        //*/
            //        ////------------
            //        ////restore back
            //        //_gfx.ResetTransform();//again
            //        //_gfx.ScaleTransform(1.0F, -1.0F);// Flip the Y-Axis
            //        //_gfx.TranslateTransform(0.0F, -(float)Height);// Translate the drawing area accordingly                
            //    }
        }

        public override void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            throw new NotImplementedException();
        }

        public override void Fill(VertexStoreSnap snap)
        {
            VxsHelper.FillVxsSnap(_renderSurface.gx, snap, _fillColor);
        }

        public override void Fill(VertexStore vxs)
        {

            VxsHelper.DrawVxsSnap(_renderSurface.gx, new VertexStoreSnap(vxs), _strokeColor);

        }

        public override void FillEllipse(double left, double top, double width, double height)
        {

            _renderSurface.gx.FillEllipse(_currentFillBrush, new System.Drawing.RectangleF((float)left, (float)top, (float)width, (float)height));

        }

        public override void FillRect(double left, double top, double width, double height)
        {

            _renderSurface.gx.FillRectangle(_currentFillBrush, (float)left, (float)top, (float)width, (float)height);

        }

        public override void FillRenderVx(Brush brush, RenderVx renderVx)
        {
            throw new NotImplementedException();
        }

        public override void FillRenderVx(RenderVx renderVx)
        {
            //TODO: review brush implementation here
            WinGdiRenderVx wRenderVx = (WinGdiRenderVx)renderVx;
            VxsHelper.FillPath(_renderSurface.gx, wRenderVx.path, this.FillColor);
        }
 
        public override void SetClipBox(int x1, int y1, int x2, int y2)
        {

            _renderSurface.gx.SetClip(new System.Drawing.Rectangle(x1, y1, x2 - x1, y2 - y1));

        }

        public override void SetOrigin(float ox, float oy)
        {
            throw new NotImplementedException();
        }

        //--------
        public void DrawImage(System.Drawing.Bitmap bmp, float x, float y)
        {

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

    }





}