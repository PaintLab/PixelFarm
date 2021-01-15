//BSD, 2014-present, WinterDev
//ArthurHub, Jose Manuel Menendez Poo

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using Typography.TextBreak;
using Typography.Text;
using Win32;
using PixelFarm.CpuBlit;

namespace PixelFarm.Drawing.WinGdi
{


    //RenderSurface => a basic object that
    //1. holds drawing states
    //2. help us do some primitive operations, (eg. scanline)



    public partial class GdiPlusRenderSurface : IDisposable
    {

        bool _isDisposed;
        //-------------------------------
        NativeWin32MemoryDC _win32MemDc;
        //-------------------------------

        IntPtr _originalHdc = IntPtr.Zero;
        internal System.Drawing.Graphics _gx;

        //-------------------------------
        Stack<System.Drawing.Rectangle> _clipRectStack = new Stack<System.Drawing.Rectangle>();
        //-------------------------------

        System.Drawing.Color _currentTextColor = System.Drawing.Color.Black;
        System.Drawing.Pen _internalPen;
        System.Drawing.SolidBrush _internalSolidBrush;
        System.Drawing.Rectangle _currentClipRect;
        //------------------------------- 
        OpenFontTextService _openFontTextServices;

        PixelFarm.CpuBlit.MemBitmap _memBmp;
        CpuBlit.AggPainter _painter;
        //------------------------------- 

        IntPtr _hFont;

        public GdiPlusRenderSurface(int width, int height)
        {
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif

            //2. dimension
            _left = 0;
            _top = 0;
            _right = _left + width;
            _bottom = _top + height;
            _currentClipRect = new System.Drawing.Rectangle(0, 0, width, height);

            //--------------
            _win32MemDc = new NativeWin32MemoryDC(width, height, false);
            _win32MemDc.PatBlt(NativeWin32MemoryDC.PatBltColor.White);
            _win32MemDc.SetBackTransparent(true);
            _win32MemDc.SetClipRect(0, 0, width, height);
            //--------------
            _memBmp = new CpuBlit.MemBitmap(width, height, _win32MemDc.PPVBits);


            _originalHdc = _win32MemDc.DC;
            _gx = System.Drawing.Graphics.FromHdc(_win32MemDc.DC);


            //--------------
            //TODO: review here how to set default font***
            //set default font
            Win32.Win32Font font = Win32.FontHelper.CreateWin32Font("Tahoma", 10, false, false);
            _hFont = font.GetHFont();
            _win32MemDc.SetFont(_hFont);
            //---------------------

            //--------------
            //set default font and default text color
            this.CurrentFont = WinGdiPlusPlatform.DefaultFont;
            this.CurrentTextColor = Color.Black;
            //-------------------------------------------------------     
            //managed object
            _internalPen = new System.Drawing.Pen(System.Drawing.Color.Black);
            _internalSolidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            this.StrokeWidth = 1;


        }

        public NativeWin32MemoryDC Win32DC => _win32MemDc;

        public CpuBlit.MemBitmap GetMemBitmap() => _memBmp;

        public CpuBlit.AggPainter GetAggPainter()
        {
            if (_painter == null)
            {
                CpuBlit.AggPainter aggPainter = CpuBlit.AggPainter.Create(_memBmp);
                aggPainter.CurrentFont = this.CurrentFont;

                if (_openFontTextServices == null)
                {
                    _openFontTextServices = new OpenFontTextService();
                }
                //optional if we want to print text on agg surface


                //aggPainter.TextPrinter = new PixelFarm.Drawing.Fonts.VxsTextPrinter(aggPainter, _openFontTextServices); //1.
                //aggPainter.TextPrinter = new PixelFarm.Drawing.Fonts.FontAtlasTextPrinter(aggPainter);//2.
                aggPainter.TextPrinter = new GdiPlusTextPrinter(this);
                //
                _painter = aggPainter;
                _painter.SetOrigin(this.OriginX, this.OriginY);
            }
            return _painter;
        }

        internal Typography.Text.OpenFontTextService OpenFontTextService => _openFontTextServices;

#if DEBUG
        public void dbugTestDrawText()
        {
            _win32MemDc.SetFont(_hFont);
            this.CurrentTextColor = Color.Black;
            DrawText("ABCDE012345".ToCharArray(), 0, 0);
        }
        public override string ToString()
        {
            return "visible_clip" + _gx.VisibleClipBounds.ToString();
        }
#endif 
        public void CloseCanvas()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            ReleaseUnManagedResource();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            this.CloseCanvas();
        }
        internal void ClearPreviousStoredValues()
        {
            _gx.RenderingOrigin = new System.Drawing.Point(0, 0);
            _canvasOriginX = 0;
            _canvasOriginY = 0;
            _clipRectStack.Clear();
        }

        internal void ReleaseUnManagedResource()
        {
            if (_win32MemDc != null)
            {
                _win32MemDc.Dispose();
                _win32MemDc = null;
                _originalHdc = IntPtr.Zero;
            }

            if (_hFont != IntPtr.Zero)
            {
                Win32.MyWin32.DeleteObject(_hFont);
                _hFont = IntPtr.Zero;
            }

            _clipRectStack.Clear();
            _currentClipRect = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);



#if DEBUG

            debug_releaseCount++;
#endif
        }



        const int CANVAS_UNUSED = 1 << (1 - 1);
        const int CANVAS_DIMEN_CHANGED = 1 << (2 - 1);

        static System.Drawing.PointF[] ConvPointFArray(PointF[] points)
        {
            int j = points.Length;
            System.Drawing.PointF[] outputPoints = new System.Drawing.PointF[j];
            for (int i = j - 1; i >= 0; --i)
            {
                outputPoints[i] = points[i].ToPointF();
            }
            return outputPoints;
        }

        internal static System.Drawing.Color ConvColor(Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }



        //debug
#if DEBUG
        public static int dbugDrawStringCount;


        public void dbug_DrawRuler(int x)
        {
            int canvas_top = _top;
            int canvas_bottom = this.Bottom;
            for (int y = canvas_top; y < canvas_bottom; y += 10)
            {
                this.DrawText(y.ToString().ToCharArray(), x, y);
            }
        }
        public void dbug_DrawCrossRect(Color color, Rectangle rect)
        {
            var prevColor = this.StrokeColor;
            this.StrokeColor = color;
            DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom);
            DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Top);
            this.StrokeColor = prevColor;
        }
#endif

#if DEBUG
        public static int dbug_canvasCount = 0;
        public int debug_resetCount = 0;
        public int debug_releaseCount = 0;
        public int debug_canvas_id = 0;

#endif 

    }


    //--------------------------------------------

    //coordinate
    partial class GdiPlusRenderSurface
    {
        int _left;
        int _top;
        int _right;
        int _bottom;
        int _canvasOriginX = 0;
        int _canvasOriginY = 0;
        Rectangle _invalidateArea;

        bool _isEmptyInvalidateArea;
        //--------------------------------------------------------------------
        public void SetCanvasOrigin(int x, int y)
        {

            //----------- 
            int total_dx = x - _canvasOriginX;
            int total_dy = y - _canvasOriginY;
            _gx.TranslateTransform(total_dx, total_dy);
            //clip rect move to another direction***
            _currentClipRect.Offset(-total_dx, -total_dy);
            _canvasOriginX = x;
            _canvasOriginY = y;
        }

        public int OriginX => _canvasOriginX;

        public int OriginY => _canvasOriginY;

        /// <summary>
        /// Sets the clipping region of this <see cref="T:System.Drawing.Graphics"/> to the result of the specified operation combining the current clip region and the rectangle specified by a <see cref="T:System.Drawing.RectangleF"/> structure.
        /// </summary>
        /// <param name="rect"><see cref="T:System.Drawing.RectangleF"/> structure to combine. </param>
        /// <param name="combineMode">Member of the <see cref="T:System.Drawing.Drawing2D.CombineMode"/> enumeration that specifies the combining operation to use. </param>
        public void SetClipRect(Rectangle rect, CombineMode combineMode = CombineMode.Replace)
        {
            _gx.SetClip(
               _currentClipRect = new System.Drawing.Rectangle(
                    rect.X, rect.Y,
                    rect.Width, rect.Height),
                    (System.Drawing.Drawing2D.CombineMode)combineMode);
        }

        public bool PushClipAreaRect(int width, int height, UpdateArea updateArea)
        {
            Rectangle intersectResult = updateArea.LocalIntersects(width, height);
            if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                return false;
            }
            else
            {
                _clipRectStack.Push(_currentClipRect);
                _currentClipRect = Conv.ToRect(intersectResult);

                updateArea.MakeBackup();
                updateArea.CurrentRect = intersectResult;

                _gx.SetClip(_currentClipRect);
                return true;
            }
        }
        public bool PushClipAreaRect(int left, int top, int width, int height, UpdateArea updateArea)
        {
            Rectangle intersectResult = updateArea.Intersects(left, top, width, height);
            if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                //not intersect?
                return false;
            }
            else
            {
                _clipRectStack.Push(_currentClipRect);
                _currentClipRect = Conv.ToRect(intersectResult);

                updateArea.MakeBackup();
                updateArea.CurrentRect = intersectResult;

                _gx.SetClip(_currentClipRect);
                return true;
            }
        }
        public void PopClipAreaRect()
        {
            if (_clipRectStack.Count > 0)
            {
                _currentClipRect = _clipRectStack.Pop();
                _gx.SetClip(_currentClipRect);
            }
        }

        public Rectangle CurrentClipRect => _currentClipRect.ToRect();

        public int Top => _top;

        public int Left => _left;

        public int Width => _right - _left;

        public int Height => _bottom - _top;

        public int Bottom => _bottom;

        public int Right => _right;

        public Rectangle Rect => Rectangle.FromLTRB(_left, _top, _right, _bottom);

        public Rectangle InvalidateArea => _invalidateArea;

        public void Invalidate(Rectangle rect)
        {
            if (_isEmptyInvalidateArea)
            {
                _invalidateArea = rect;
                _isEmptyInvalidateArea = false;
            }
            else
            {
                _invalidateArea = Rectangle.Union(rect, _invalidateArea);
            }

            //need to draw again
            this.IsContentReady = false;
        }

        public bool IsContentReady { get; set; }
    }

    //--------------------------------------------
    //drawing
    partial class GdiPlusRenderSurface
    {
        float _strokeWidth = 1f;
        Color _fillSolidColor = Color.Transparent;
        Color _strokeColor = Color.Black;
        //==========================================================
        public Color StrokeColor
        {
            get => _strokeColor;

            set => _internalPen.Color = ConvColor(_strokeColor = value);
        }
        public float StrokeWidth
        {
            get => _strokeWidth;

            set => _internalPen.Width = _strokeWidth = value;
        }

        public void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
        {

            MyWin32.SetViewportOrgEx(_win32MemDc.DC, _canvasOriginX, _canvasOriginY, IntPtr.Zero);
            MyWin32.BitBlt(
                destHdc, destArea.X, destArea.Y, destArea.Width, destArea.Height, //dest
                _win32MemDc.DC, sourceX, sourceY, MyWin32.SRCCOPY); //src
            MyWin32.SetViewportOrgEx(_win32MemDc.DC, -_canvasOriginX, -_canvasOriginY, IntPtr.Zero);
        }
        public unsafe void RenderTo(byte* outputBuffer)
        {
            MyWin32.SetViewportOrgEx(_win32MemDc.DC, _canvasOriginX, _canvasOriginY, IntPtr.Zero);
            _win32MemDc.CopyPixelBitsToOutput(outputBuffer);
            MyWin32.SetViewportOrgEx(_win32MemDc.DC, -_canvasOriginX, -_canvasOriginY, IntPtr.Zero);
        }
        public void Clear(PixelFarm.Drawing.Color c)
        {
            _gx.Clear(System.Drawing.Color.FromArgb(
                c.A,
                c.R,
                c.G,
                c.B));
        }
        public void FillRectangle(Brush brush, float left, float top, float width, float height)
        {

            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        //use default solid brush
                        SolidBrush solidBrush = (SolidBrush)brush;
                        var prevColor = _internalSolidBrush.Color;
                        _internalSolidBrush.Color = ConvColor(solidBrush.Color);
                        _gx.FillRectangle(_internalSolidBrush, left, top, width, height);
                        _internalSolidBrush.Color = prevColor;
                    }
                    break;
                case BrushKind.LinearGradient:
                    {
                        //draw with gradient
                        LinearGradientBrush linearBrush = (LinearGradientBrush)brush;
                        // this version, for gdi, impl only 1 pair

                        //LinearGradientPair firstPair = null;
                        //foreach (Drawing.LinearGradientPair p in linearBrush.GetColorPairIter())
                        //{
                        //    firstPair = p;
                        //    break;
                        //}

                        //using (var linearGradBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        //    new System.Drawing.PointF(firstPair.x1, firstPair.y1),
                        //    new System.Drawing.PointF(firstPair.x2, firstPair.y2),
                        //    ConvColor(firstPair.c1),
                        //    ConvColor(firstPair.c2)))
                        //{
                        //    _gx.FillRectangle(linearGradBrush, left, top, width, height);
                        //}
                    }
                    break;
                case BrushKind.PolygonGradient:
                    {
                    }
                    break;
                case BrushKind.CircularGradient:
                    {
                    }
                    break;
                case BrushKind.Texture:
                    {
                    }
                    break;
            }
        }
        public void FillRectangle(Color color, float left, float top, float width, float height)
        {

            _internalSolidBrush.Color = ConvColor(color);
            _gx.FillRectangle(_internalSolidBrush, left, top, width, height);
        }


        public void DrawRectangle(Color color, float left, float top, float width, float height)
        {

            _internalPen.Color = ConvColor(color);
            _gx.DrawRectangle(_internalPen, left, top, width, height);
        }

        public void DrawLine(float x1, float y1, float x2, float y2)
        {

            _gx.DrawLine(_internalPen, x1, y1, x2, y2);
        }


        //public override void DrawRoundRect(int x, int y, int w, int h, Size cornerSize)
        //{

        //    int cornerSizeW = cornerSize.Width;
        //    int cornerSizeH = cornerSize.Height;

        //    System.Drawing.Drawing2D.GraphicsPath gxPath = new System.Drawing.Drawing2D.GraphicsPath();
        //    gxPath.AddArc(new System.Drawing.Rectangle(x, y, cornerSizeW * 2, cornerSizeH * 2), 180, 90);
        //    gxPath.AddLine(new System.Drawing.Point(x + cornerSizeW, y), new System.Drawing.Point(x + w - cornerSizeW, y));

        //    gxPath.AddArc(new System.Drawing.Rectangle(x + w - cornerSizeW * 2, y, cornerSizeW * 2, cornerSizeH * 2), -90, 90);
        //    gxPath.AddLine(new System.Drawing.Point(x + w, y + cornerSizeH), new System.Drawing.Point(x + w, y + h - cornerSizeH));

        //    gxPath.AddArc(new System.Drawing.Rectangle(x + w - cornerSizeW * 2, y + h - cornerSizeH * 2, cornerSizeW * 2, cornerSizeH * 2), 0, 90);
        //    gxPath.AddLine(new System.Drawing.Point(x + w - cornerSizeW, y + h), new System.Drawing.Point(x + cornerSizeW, y + h));

        //    gxPath.AddArc(new System.Drawing.Rectangle(x, y + h - cornerSizeH * 2, cornerSizeW * 2, cornerSizeH * 2), 90, 90);
        //    gxPath.AddLine(new System.Drawing.Point(x, y + cornerSizeH), new System.Drawing.Point(x, y + h - cornerSizeH));

        //    gx.FillPath(System.Drawing.Brushes.Yellow, gxPath);
        //    gx.DrawPath(System.Drawing.Pens.Red, gxPath);
        //    gxPath.Dispose();
        //}


        /// <summary>
        /// Gets or sets the rendering quality for this <see cref="T:System.Drawing.Graphics"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Drawing.Drawing2D.SmoothingMode"/> values.
        /// </returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public SmoothingMode SmoothingMode
        {
            get => (SmoothingMode)(_gx.SmoothingMode);

            set => _gx.SmoothingMode = (System.Drawing.Drawing2D.SmoothingMode)value;
        }

        static Win32.NativeWin32MemoryDC ResolveForWin32Dc(Image image)
        {
            if (image is PixelFarm.CpuBlit.MemBitmap)
            {
                //this is known image
                var win32Dc = Image.GetCacheInnerImage(image) as Win32.NativeWin32MemoryDC;
                if (win32Dc != null)
                {
                    return win32Dc;
                }
            }
            return null;
        }

        static System.Drawing.Bitmap ResolveInnerBmp(Image image)
        {
            if (Image.GetCacheInnerImage(image) is System.Drawing.Bitmap cacheBmp)
            {
                //check if cache image is update or not 
                return cacheBmp;
            }

            if (image is ImageBinder binder)
            {
                //convert bmp to gdi+ bmp
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image.Width,
                 image.Height,
                 System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                      System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                IntPtr bufferHeaderPtr = binder.GetRawBufferHead();
                unsafe
                {
                    PixelFarm.Drawing.Internal.MemMx.memcpy((byte*)bmpdata.Scan0, (byte*)bufferHeaderPtr, bmpdata.Stride * bmpdata.Height);
                }
                bmp.UnlockBits(bmpdata);
                //
                Image.SetCacheInnerImage(image, bmp, true);
                return bmp;
            }



            if (image is PixelFarm.CpuBlit.MemBitmap)
            {
                //this is known image 
                //TODO: check mem leak too!
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image.Width,
                    image.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                PixelFarm.CpuBlit.BitmapHelper.CopyToGdiPlusBitmapSameSizeNotFlip((PixelFarm.CpuBlit.MemBitmap)image, bmp);
                //
                Image.SetCacheInnerImage(image, bmp, true);
                return bmp;

            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Draws the specified portion of the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param>
        /// <param name="destRect"><see cref="T:System.Drawing.RectangleF"/> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle. </param>
        /// <param name="srcRect"><see cref="T:System.Drawing.RectangleF"/> structure that specifies the portion of the <paramref name="image"/> object to draw. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception>
        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect)
        {

            _gx.DrawImage(ResolveInnerBmp(image),
                destRect.ToRectF(),
                srcRect.ToRectF(),
                System.Drawing.GraphicsUnit.Pixel);
        }
        public void DrawImage(Image image, int x, int y)
        {
            PixelFarm.CpuBlit.MemBitmap bmp = image as PixelFarm.CpuBlit.MemBitmap;
            if (bmp != null)
            {
                Win32.NativeWin32MemoryDC win32DC = ResolveForWin32Dc(image);
                if (win32DC != null)
                {
                    _win32MemDc.BlendWin32From(win32DC.DC, 0, 0, image.Width, image.Height, x, y);
                    return;
                }

                System.Drawing.Bitmap resolvedImg = ResolveInnerBmp(image);
                _gx.DrawImageUnscaled(resolvedImg, x, y);
            }
            else
            {
                System.Drawing.Bitmap resolvedImg = ResolveInnerBmp(image);
                _gx.DrawImageUnscaled(resolvedImg, x, y);
            }
        }
        public void DrawImages(Image image, RectangleF[] destAndSrcPairs)
        {

            int j = destAndSrcPairs.Length;
            if (j > 1)
            {
                if ((j % 2) != 0)
                {
                    //make it even number
                    j -= 1;
                }
                //loop  
                System.Drawing.Bitmap inner = ResolveInnerBmp(image);
                for (int i = 0; i < j;)
                {
                    _gx.DrawImage(inner,
                        destAndSrcPairs[i].ToRectF(),
                        destAndSrcPairs[i + 1].ToRectF(),
                        System.Drawing.GraphicsUnit.Pixel);
                    i += 2;
                }
            }
        }
        /// <summary>
        /// Draws the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param><param name="destRect"><see cref="T:System.Drawing.Rectangle"/> structure that specifies the location and size of the drawn image. </param><exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public void DrawImage(Image image, RectangleF destRect)
        {

            //if (image is PixelFarm.CpuBlit.ActualBitmap)
            //{

            //    if (image.Width == destRect.Width &&
            //        image.Height == destRect.Height)
            //    {
            //        Painter painter = GetAggPainter();
            //        //RenderQuality prev = painter.RenderQuality;
            //        //painter.RenderQuality = RenderQuality.Fast;
            //        //draw image at current position

            //        painter.DrawImage((PixelFarm.CpuBlit.ActualBitmap)image);

            //        //painter.RenderQuality = prev;
            //        return;
            //    }
            //}

            //check if we need scale?
            if (image.Width == destRect.Width &&
                image.Height == destRect.Height)
            {
                System.Drawing.Bitmap inner2 = ResolveInnerBmp(image);
                if (inner2.HorizontalResolution == _gx.DpiX)
                {
                    DrawImage(image, (int)destRect.Left, (int)destRect.Top);
                    return;
                }
            }

            System.Drawing.Bitmap inner = ResolveInnerBmp(image);
            if (image.IsReferenceImage)
            {
                _gx.DrawImage(inner,
                    destRect.ToRectF(),
                     new System.Drawing.RectangleF(
                         image.ReferenceX, image.ReferenceY,
                         image.Width, image.Height),
                    System.Drawing.GraphicsUnit.Pixel);
            }
            else
            {
                _gx.DrawImage(inner, destRect.ToRectF());
            }
        }

        static System.Drawing.Drawing2D.GraphicsPath ResolveGraphicsPath(PixelFarm.CpuBlit.VxsRenderVx vxsRenderVx)
        {

            var gpath = PixelFarm.CpuBlit.VxsRenderVx.GetResolvedObject(vxsRenderVx) as System.Drawing.Drawing2D.GraphicsPath;
            if (gpath != null) return gpath;

            gpath = CreateGraphicsPath(vxsRenderVx._vxs);
            PixelFarm.CpuBlit.VxsRenderVx.SetResolvedObject(vxsRenderVx, gpath);
            return gpath;
        }
        static System.Drawing.Drawing2D.GraphicsPath CreateGraphicsPath(VertexStore vxs)
        {
            //
            //elsse create a new one 
            var gpath = new System.Drawing.Drawing2D.GraphicsPath();

            int j = vxs.Count;
            float latestMoveX = 0, latestMoveY = 0, latestX = 0, latestY = 0;
            bool isOpen = false;
            for (int i = 0; i < j; ++i)
            {
                var cmd = vxs.GetVertex(i, out double x, out double y);
                switch (cmd)
                {
                    case VertexCmd.MoveTo:
                        {
                            latestMoveX = latestX = (float)x;
                            latestMoveY = latestY = (float)y;
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            isOpen = true;
                            gpath.AddLine(latestX, latestY, latestX = (float)x, latestY = (float)y);
                        }
                        break;
                    case VertexCmd.Close:
                        {
                            latestX = latestMoveX;
                            latestY = latestMoveY;

                            gpath.CloseFigure();
                            isOpen = false;
                        }
                        break;
                    case VertexCmd.NoMore: break;
                    default:
                        throw new System.NotSupportedException();
                }
            }

            return gpath;
        }
        public void FillPath(Color color, PixelFarm.CpuBlit.VxsRenderVx vxsRenderVx)
        {

            //solid color
            var prevColor = _internalSolidBrush.Color;
            _internalSolidBrush.Color = ConvColor(color);
            System.Drawing.Drawing2D.GraphicsPath innerPath = ResolveGraphicsPath(vxsRenderVx);
            _gx.FillPath(_internalSolidBrush, innerPath);
            _internalSolidBrush.Color = prevColor;
        }
        public void FillPath(PixelFarm.CpuBlit.VxsRenderVx vxsRenderVx)
        {
            //solid color 
            System.Drawing.Drawing2D.GraphicsPath innerPath = ResolveGraphicsPath(vxsRenderVx);
            _gx.FillPath(_internalSolidBrush, innerPath);
        }

        public void FillPath(Brush brush, PixelFarm.CpuBlit.VxsRenderVx vxsRenderVx)
        {

            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        SolidBrush solidBrush = (SolidBrush)brush;
                        var prevColor = _internalSolidBrush.Color;
                        _internalSolidBrush.Color = ConvColor(solidBrush.Color);
                        //
                        System.Drawing.Drawing2D.GraphicsPath innerPath = ResolveGraphicsPath(vxsRenderVx);
                        _gx.FillPath(_internalSolidBrush, innerPath);
                        //
                        _internalSolidBrush.Color = prevColor;
                    }
                    break;
                case BrushKind.LinearGradient:
                    {
                        //???
                        //WHY???
                        //LinearGradientBrush gradientBrush = (LinearGradientBrush)brush;
                        //var prevColor = _internalSolidBrush.Color;
                        //_internalSolidBrush.Color = ConvColor(gradientBrush.Color);
                        ////
                        //System.Drawing.Drawing2D.GraphicsPath innerPath = ResolveGraphicsPath(vxsRenderVx);
                        //_gx.FillPath(_internalSolidBrush, innerPath);
                        ////
                        //_internalSolidBrush.Color = prevColor;
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            var pps = ConvPointFArray(points);
            //use internal solid color            
            _gx.FillPolygon(brush.InnerBrush as System.Drawing.Brush, pps);
        }
        public void FillPolygon(Color color, PointF[] points)
        {
            var pps = ConvPointFArray(points);
            _internalSolidBrush.Color = ConvColor(color);
            _gx.FillPolygon(_internalSolidBrush, pps);
        }

        ////==========================================================
        //public override void CopyFrom(Canvas sourceCanvas, int logicalSrcX, int logicalSrcY, Rectangle destArea)
        //{
        //    MyCanvas s1 = (MyCanvas)sourceCanvas;

        //    if (s1.gx != null)
        //    {
        //        int phySrcX = logicalSrcX - s1.left;
        //        int phySrcY = logicalSrcY - s1.top;

        //        System.Drawing.Rectangle postIntersect =
        //            System.Drawing.Rectangle.Intersect(currentClipRect, destArea.ToRect());
        //        phySrcX += postIntersect.X - destArea.X;
        //        phySrcY += postIntersect.Y - destArea.Y;
        //        destArea = postIntersect.ToRect();

        //        IntPtr gxdc = gx.GetHdc();

        //        MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
        //        IntPtr source_gxdc = s1.gx.GetHdc();
        //        MyWin32.SetViewportOrgEx(source_gxdc, s1.CanvasOrgX, s1.CanvasOrgY, IntPtr.Zero);


        //        MyWin32.BitBlt(gxdc, destArea.X, destArea.Y, destArea.Width, destArea.Height, source_gxdc, phySrcX, phySrcY, MyWin32.SRCCOPY);


        //        MyWin32.SetViewportOrgEx(source_gxdc, -s1.CanvasOrgX, -s1.CanvasOrgY, IntPtr.Zero);

        //        s1.gx.ReleaseHdc();

        //        MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
        //        gx.ReleaseHdc();



        //    }
        //}
    }

    //--------------------------------------------
    //text and font
    partial class GdiPlusRenderSurface
    {
        Typography.Text.ResolvedFont _resolvedFont;
        RequestFont _currentTextFont = null;
        Color _mycurrentTextColor = Color.Black;

        public void DrawText(char[] buffer, int x, int y)
        {
            var clipRect = _currentClipRect;
            clipRect.Offset(_canvasOriginX, _canvasOriginY);
            //1.
            _win32MemDc.SetClipRect(clipRect.Left, clipRect.Top, clipRect.Width, clipRect.Height);
            //2.
            NativeTextWin32.TextOut(_win32MemDc.DC, _canvasOriginX + x, _canvasOriginY + y, buffer, buffer.Length);
            //3
            _win32MemDc.ClearClipRect();
        }
        public void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            var clipRect = System.Drawing.Rectangle.Intersect(logicalTextBox.ToRect(), _currentClipRect);
            //1.
            clipRect.Offset(_canvasOriginX, _canvasOriginY);
            //2.
            _win32MemDc.SetClipRect(clipRect.Left, clipRect.Top, clipRect.Width, clipRect.Height);
            //3.
            NativeTextWin32.TextOut(_win32MemDc.DC, _canvasOriginX + logicalTextBox.X, _canvasOriginY + logicalTextBox.Y, buffer, buffer.Length);
            //4.
            _win32MemDc.ClearClipRect();
        }

        public void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            //this is the most common used function for text drawing
            //return;
#if DEBUG
            dbugDrawStringCount++;
#endif
            var color = this.CurrentTextColor;
            if (color.A == 255)
            {
                //1. find clip rect
                var clipRect = Rectangle.Intersect(logicalTextBox,
                    new Rectangle(_currentClipRect.Left,
                        _currentClipRect.Top,
                        _currentClipRect.Width,
                        _currentClipRect.Height));
                //2. offset to canvas origin 
                clipRect.Offset(_canvasOriginX, _canvasOriginY);
                //3. set rect rgn
                _win32MemDc.SetClipRect(clipRect.X, clipRect.Y, clipRect.Width, clipRect.Height);

                unsafe
                {
                    fixed (char* startAddr = &str[0])
                    {
                        //4.

                        NativeTextWin32.TextOutUnsafe(_originalHdc,
                            (int)logicalTextBox.X + _canvasOriginX,
                            (int)logicalTextBox.Y + _canvasOriginY,
                            (startAddr + startAt), len);
                    }
                }
                //5. clear rect rgn
                _win32MemDc.ClearClipRect();
#if DEBUG
                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
                //        logicalTextBox.X + canvasOriginX,
                //        logicalTextBox.Y + canvasOriginY);
#endif

            }
            else
            {

                //-------------------------------------------
                //not support translucent text in this version,
                //so=> draw opaque (like above)
                //-------------------------------------------
                //1. find clip rect
                var clipRect = Rectangle.Intersect(logicalTextBox,
                    new Rectangle(_currentClipRect.Left,
                        _currentClipRect.Top,
                        _currentClipRect.Width,
                        _currentClipRect.Height));
                //2. offset to canvas origin 
                clipRect.Offset(_canvasOriginX, _canvasOriginY);
                //3. set rect rgn
                _win32MemDc.SetClipRect(clipRect.X, clipRect.Y, clipRect.Width, clipRect.Height);

                unsafe
                {
                    fixed (char* startAddr = &str[0])
                    {
                        //4.
                        NativeTextWin32.TextOutUnsafe(_originalHdc,
                            (int)logicalTextBox.X + _canvasOriginX,
                            (int)logicalTextBox.Y + _canvasOriginY,
                            (startAddr + startAt), len);
                    }
                }
                //5. clear rect rgn
                _win32MemDc.ClearClipRect();
#if DEBUG
                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
                //        logicalTextBox.X + canvasOriginX,
                //        logicalTextBox.Y + canvasOriginY);
#endif



            }
        }


        public void MeasureString(char[] str, int startAt, int len, Rectangle logicalTextBox, out int w, out int h)
        {
            //test
            _win32MemDc.MeasureTextSize(str, startAt, len, out w, out h);
        }

        public Typography.Text.ResolvedFont CurrentResolvedFont
        {
            get => _resolvedFont;
            set
            {
                _resolvedFont = value;
            }
        }

        public RequestFont CurrentFont
        {
            get => _currentTextFont;

            set
            {
                _currentTextFont = value;
                _win32MemDc.SetFont(WinGdiFontSystem.GetWinGdiFont(value).CachedHFont());
            }
        }
        public Color CurrentTextColor
        {
            get => _mycurrentTextColor;

            set
            {
                _mycurrentTextColor = value;
                _win32MemDc.SetTextColor(value.R, value.G, value.B);
            }
        }



        //static void ResolveGraphicsPath(GraphicsPath path, VertexStore outputVxs)
        //{
        //    //convert from graphics path to internal presentation
        //    VertexStore innerPath = path.InnerPath as VertexStore;
        //    if (innerPath != null)
        //    {
        //        return;
        //        //return innerPath;
        //    }
        //    //-------- 

        //    path.InnerPath = outputVxs;
        //    using (VectorToolBox.Borrow(outputVxs, out PathWriter writer))
        //    {
        //        List<float> points;
        //        List<PathCommand> cmds;

        //        GraphicsPath.GetPathData(path, out points, out cmds);
        //        int j = cmds.Count;
        //        int p_index = 0;


        //        for (int i = 0; i < j; ++i)
        //        {
        //            PathCommand cmd = cmds[i];
        //            switch (cmd)
        //            {
        //                default:
        //                    throw new NotSupportedException();
        //                case PathCommand.Arc:
        //                    {
        //                        //TODO: review arc
        //                        //convert to curve?
        //                    }
        //                    //innerPath.AddArc(
        //                    //    points[p_index],
        //                    //    points[p_index + 1],
        //                    //    points[p_index + 2],
        //                    //    points[p_index + 3],
        //                    //    points[p_index + 4],
        //                    //    points[p_index + 5]);
        //                    p_index += 6;
        //                    break;
        //                case PathCommand.Bezier:

        //                    writer.MoveTo(points[p_index],
        //                        points[p_index + 1]);
        //                    writer.Curve4(
        //                        points[p_index + 2],
        //                        points[p_index + 3],
        //                        points[p_index + 4],
        //                        points[p_index + 5],
        //                        points[p_index + 6],
        //                        points[p_index + 7]);

        //                    p_index += 8;
        //                    break;
        //                case PathCommand.CloseFigure:
        //                    writer.CloseFigure();
        //                    //innerPath.CloseFigure();
        //                    break;
        //                case PathCommand.Ellipse:
        //                    using (VectorToolBox.Borrow(out CpuBlit.VertexProcessing.Ellipse ellipse))
        //                    {
        //                        ellipse.SetFromLTWH(
        //                            points[p_index],
        //                            points[p_index + 1],
        //                            points[p_index + 2],
        //                            points[p_index + 3]);
        //                        ellipse.MakeVxs(writer);
        //                    }

        //                    p_index += 4;
        //                    break;
        //                case PathCommand.Line:
        //                    {
        //                        writer.MoveTo(points[p_index],
        //                                      points[p_index + 1]);
        //                        writer.LineTo(points[p_index + 2],
        //                                      points[p_index + 3]);
        //                    }
        //                    p_index += 4;
        //                    break;
        //                case PathCommand.Rect:
        //                    using (VectorToolBox.Borrow(out CpuBlit.VertexProcessing.SimpleRect simpleRect))
        //                    {
        //                        simpleRect.SetRectFromLTWH(
        //                            points[p_index],
        //                            points[p_index + 1],
        //                            points[p_index + 2],
        //                            points[p_index + 3]
        //                            );
        //                        simpleRect.MakeVxs(writer);
        //                    }

        //                    p_index += 4;
        //                    break;
        //                case PathCommand.StartFigure:
        //                    break;
        //            }
        //        }
        //    }
        //}
    }


    sealed class GdiPlusTextPrinter : CpuBlit.IAggTextPrinter
    {

        readonly GdiPlusRenderSurface _rendersx;
        public GdiPlusTextPrinter(GdiPlusRenderSurface rendersx)
        {
            _rendersx = rendersx;
        }

        public void ChangeFont(RequestFont font)
        {
            _rendersx.CurrentFont = font;
            _rendersx.OpenFontTextService.ResolveFont(font);
        }

        public RequestFont CurrentFont => _rendersx.CurrentFont;

        public TextBaseline TextBaseline { get; set; }

        public int CurrentLineSpaceHeight => throw new NotImplementedException();

        public void ChangeFillColor(Color fontColor)
        {
            //change font color
            _rendersx.CurrentTextColor = fontColor;
        }
        public void ChangeStrokeColor(Color strokeColor)
        {
            //TODO: ...
        }

        public void DrawString(char[] text, int startAt, int len, double left, double top)
        {
            //TODO: review TextBaseline ***

            _rendersx.DrawText(text,
                startAt, len,
                new Rectangle((int)left,
                (int)(top - _rendersx.CurrentResolvedFont.LineSpacingInPixels),
                2480, //temp we,not need clip
                1024),//temp we,not need clip
                0);
        }
        public void DrawString(CpuBlit.AggRenderVxFormattedString renderVx, double left, double top)
        {
            //TODO: implement this
        }
        public void PrepareStringForRenderVx(CpuBlit.AggRenderVxFormattedString renderVx, char[] text, int startAt, int len)
        {
            //TODO: implement this
        }
        public void MeasureString(char[] buffer, int startAt, int len, out int w, out int h)
        {
            _rendersx.MeasureString(buffer, startAt, len, new Rectangle(), out w, out h);
        }

        public void PrepareStringForRenderVx(AggRenderVxFormattedString renderVx, IFormattedGlyphPlanList fmtGlyphPlans)
        {
            throw new NotImplementedException();
        }
    }
}