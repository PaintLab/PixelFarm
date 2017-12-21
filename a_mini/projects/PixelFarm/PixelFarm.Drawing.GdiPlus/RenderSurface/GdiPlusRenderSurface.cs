//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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
using Win32;
namespace PixelFarm.Drawing.WinGdi
{


    public partial class GdiPlusRenderSurface : IDisposable
    {

        bool isDisposed;
        //-------------------------------
        NativeWin32MemoryDc win32MemDc;
        //-------------------------------

        IntPtr originalHdc = IntPtr.Zero;
        internal System.Drawing.Graphics gx;

        //-------------------------------
        Stack<System.Drawing.Rectangle> clipRectStack = new Stack<System.Drawing.Rectangle>();
        //-------------------------------

        System.Drawing.Color currentTextColor = System.Drawing.Color.Black;
        System.Drawing.Pen internalPen;
        System.Drawing.SolidBrush internalSolidBrush;
        System.Drawing.Rectangle currentClipRect;
        //-------------------------------

        public GdiPlusRenderSurface(int left, int top, int width, int height)
        {
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif

            //2. dimension
            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;
            currentClipRect = new System.Drawing.Rectangle(0, 0, width, height);

            CreateGraphicsFromNativeHdc(width, height);
            this.gx = System.Drawing.Graphics.FromHdc(win32MemDc.DC);
            //-------------------------------------------------------     
            //managed object
            internalPen = new System.Drawing.Pen(System.Drawing.Color.Black);
            internalSolidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            this.StrokeWidth = 1;
        }
        void CreateGraphicsFromNativeHdc(int width, int height)
        {
            win32MemDc = new NativeWin32MemoryDc(width, height, true);
            win32MemDc.PatBlt(NativeWin32MemoryDc.PatBltColor.White);
            win32MemDc.SetBackTransparent(true);
            win32MemDc.SetClipRect(0, 0, width, height);

            this.originalHdc = win32MemDc.DC;
            //--------------
            //set default font and default text color
            this.CurrentFont = new RequestFont("tahoma", 14);
            this.CurrentTextColor = Color.Black;
            //--------------

        }
#if DEBUG
        public override string ToString()
        {
            return "visible_clip" + this.gx.VisibleClipBounds.ToString();
        }
#endif




        public void CloseCanvas()
        {
            if (isDisposed)
            {
                return;
            }
            if (win32MemDc != null)
            {
                win32MemDc.Dispose();
                win32MemDc = null;
            }
            isDisposed = true;

            ReleaseUnManagedResource();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (isDisposed)
            {
                return;
            }
            this.CloseCanvas();
        }
        internal void ClearPreviousStoredValues()
        {
            this.gx.RenderingOrigin = new System.Drawing.Point(0, 0);
            this.canvasOriginX = 0;
            this.canvasOriginY = 0;
            this.clipRectStack.Clear();
        }

        internal void ReleaseUnManagedResource()
        {
            if (win32MemDc != null)
            {
                win32MemDc.Dispose();
                win32MemDc = null;
                originalHdc = IntPtr.Zero;
            }

            clipRectStack.Clear();
            currentClipRect = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);
#if DEBUG

            debug_releaseCount++;
#endif
        }

        public void Reuse(int hPageNum, int vPageNum)
        {

            int w = this.Width;
            int h = this.Height;
            this.ClearPreviousStoredValues();
            currentClipRect = new System.Drawing.Rectangle(0, 0, w, h);
            win32MemDc.PatBlt(NativeWin32MemoryDc.PatBltColor.White);
            win32MemDc.SetClipRect(0, 0, w, h);
            left = hPageNum * w;
            top = vPageNum * h;
            right = left + w;
            bottom = top + h;
        }
        public void Reset(int hPageNum, int vPageNum, int newWidth, int newHeight)
        {

            this.ReleaseUnManagedResource();
            this.ClearPreviousStoredValues();

            currentClipRect = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
            CreateGraphicsFromNativeHdc(newWidth, newHeight);
            this.gx = System.Drawing.Graphics.FromHdc(win32MemDc.DC);


            left = hPageNum * newWidth;
            top = vPageNum * newHeight;
            right = left + newWidth;
            bottom = top + newHeight;
#if DEBUG
            debug_resetCount++;
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
            int canvas_top = this.top;
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
        //public override bool PushClipAreaForNativeScrollableElement(Rect updateArea)
        //{

        //    clipRectStack.Push(currentClipRect);

        //    System.Drawing.Rectangle intersectResult = System.Drawing.Rectangle.Intersect(
        //        currentClipRect,
        //        updateArea.ToRectangle().ToRect());

        //    if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
        //    {
        //        currentClipRect = intersectResult;
        //        return false;
        //    }

        //    gx.SetClip(intersectResult);
        //    currentClipRect = intersectResult;
        //    return true;
        //}

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
        int left;
        int top;
        int right;
        int bottom;
        int canvasOriginX = 0;
        int canvasOriginY = 0;
        Rectangle invalidateArea;

        bool isEmptyInvalidateArea;
        //--------------------------------------------------------------------
        public void SetCanvasOrigin(int x, int y)
        {

            //----------- 
            int total_dx = x - canvasOriginX;
            int total_dy = y - canvasOriginY;
            this.gx.TranslateTransform(total_dx, total_dy);
            //clip rect move to another direction***
            this.currentClipRect.Offset(-total_dx, -total_dy);
            this.canvasOriginX = x;
            this.canvasOriginY = y;
        }

        public int OriginX
        {
            get { return this.canvasOriginX; }
        }
        public int OriginY
        {
            get { return this.canvasOriginY; }
        }


        /// <summary>
        /// Sets the clipping region of this <see cref="T:System.Drawing.Graphics"/> to the result of the specified operation combining the current clip region and the rectangle specified by a <see cref="T:System.Drawing.RectangleF"/> structure.
        /// </summary>
        /// <param name="rect"><see cref="T:System.Drawing.RectangleF"/> structure to combine. </param>
        /// <param name="combineMode">Member of the <see cref="T:System.Drawing.Drawing2D.CombineMode"/> enumeration that specifies the combining operation to use. </param>
        public void SetClipRect(Rectangle rect, CombineMode combineMode = CombineMode.Replace)
        {

            gx.SetClip(
               this.currentClipRect = new System.Drawing.Rectangle(
                    rect.X, rect.Y,
                    rect.Width, rect.Height),
                    (System.Drawing.Drawing2D.CombineMode)combineMode);
        }
        public bool IntersectsWith(Rectangle clientRect)
        {
            return clientRect.IntersectsWith(left, top, right, bottom);
        }

        public bool PushClipAreaRect(int width, int height, ref Rectangle updateArea)
        {
            this.clipRectStack.Push(currentClipRect);
            System.Drawing.Rectangle intersectResult =
                  System.Drawing.Rectangle.Intersect(
                  System.Drawing.Rectangle.FromLTRB(updateArea.Left, updateArea.Top, updateArea.Right, updateArea.Bottom),
                  new System.Drawing.Rectangle(0, 0, width, height));
            currentClipRect = intersectResult;
            if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                //not intersec?
                return false;
            }
            else
            {
                updateArea = Conv.ToRect(intersectResult);
                gx.SetClip(intersectResult);
                return true;
            }
        }
        public void PopClipAreaRect()
        {
            if (clipRectStack.Count > 0)
            {

                currentClipRect = clipRectStack.Pop();
                gx.SetClip(currentClipRect);
            }
        }



        public Rectangle CurrentClipRect
        {
            get
            {
                return currentClipRect.ToRect();
            }
        }



        public int Top
        {
            get
            {
                return top;
            }
        }
        public int Left
        {
            get
            {
                return left;
            }
        }

        public int Width
        {
            get
            {
                return right - left;
            }
        }
        public int Height
        {
            get
            {
                return bottom - top;
            }
        }
        public int Bottom
        {
            get
            {
                return bottom;
            }
        }
        public int Right
        {
            get
            {
                return right;
            }
        }
        public Rectangle Rect
        {
            get
            {
                return Rectangle.FromLTRB(left, top, right, bottom);
            }
        }
        public Rectangle InvalidateArea
        {
            get
            {
                return invalidateArea;
            }
        }

        public void ResetInvalidateArea()
        {
            this.invalidateArea = Rectangle.Empty;
            this.isEmptyInvalidateArea = true;//set
        }
        public void Invalidate(Rectangle rect)
        {
            if (isEmptyInvalidateArea)
            {
                invalidateArea = rect;
                isEmptyInvalidateArea = false;
            }
            else
            {
                invalidateArea = Rectangle.Union(rect, invalidateArea);
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
        float strokeWidth = 1f;
        Color fillSolidColor = Color.Transparent;
        Color strokeColor = Color.Black;
        //==========================================================
        public Color StrokeColor
        {
            get
            {
                return this.strokeColor;
            }
            set
            {
                this.internalPen.Color = ConvColor(this.strokeColor = value);
            }
        }
        public float StrokeWidth
        {
            get
            {
                return this.strokeWidth;
            }
            set
            {
                this.internalPen.Width = this.strokeWidth = value;
            }
        }

        public void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
        {

            MyWin32.SetViewportOrgEx(win32MemDc.DC, canvasOriginX, canvasOriginY, IntPtr.Zero);
            MyWin32.BitBlt(
                destHdc, destArea.X, destArea.Y, destArea.Width, destArea.Height, //dest
                win32MemDc.DC, sourceX, sourceY, MyWin32.SRCCOPY); //src
            MyWin32.SetViewportOrgEx(win32MemDc.DC, -canvasOriginX, -canvasOriginY, IntPtr.Zero);

        }
        public void Clear(PixelFarm.Drawing.Color c)
        {

            gx.Clear(System.Drawing.Color.FromArgb(
                c.A,
                c.R,
                c.G,
                c.B));
        }
        public void DrawPath(GraphicsPath gfxPath)
        {
            gx.DrawPath(internalPen, ResolveGraphicsPath(gfxPath));
        }
        public void FillRectangle(Brush brush, float left, float top, float width, float height)
        {

            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        //use default solid brush
                        SolidBrush solidBrush = (SolidBrush)brush;
                        var prevColor = internalSolidBrush.Color;
                        internalSolidBrush.Color = ConvColor(solidBrush.Color);
                        gx.FillRectangle(internalSolidBrush, left, top, width, height);
                        internalSolidBrush.Color = prevColor;
                    }
                    break;
                case BrushKind.LinearGradient:
                    {
                        //draw with gradient
                        LinearGradientBrush linearBrush = (LinearGradientBrush)brush;
                        var colors = linearBrush.GetColors();
                        var points = linearBrush.GetStopPoints();
                        using (var linearGradBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                             points[0].ToPointF(),
                             points[1].ToPointF(),
                             ConvColor(colors[0]),
                             ConvColor(colors[1])))
                        {
                            gx.FillRectangle(linearGradBrush, left, top, width, height);
                        }
                    }
                    break;
                case BrushKind.GeometryGradient:
                    {
                    }
                    break;
                case BrushKind.CircularGraident:
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

            internalSolidBrush.Color = ConvColor(color);
            gx.FillRectangle(internalSolidBrush, left, top, width, height);
        }


        public void DrawRectangle(Color color, float left, float top, float width, float height)
        {

            internalPen.Color = ConvColor(color);
            gx.DrawRectangle(internalPen, left, top, width, height);
        }

        public void DrawLine(float x1, float y1, float x2, float y2)
        {

            gx.DrawLine(internalPen, x1, y1, x2, y2);
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
            get
            {

                return (SmoothingMode)(gx.SmoothingMode);
            }
            set
            {

                gx.SmoothingMode = (System.Drawing.Drawing2D.SmoothingMode)value;
            }
        }

        static System.Drawing.Bitmap ResolveInnerBmp(Image image)
        {

            if (image is PixelFarm.Agg.ActualImage)
            {
                //this is known image
                var cacheBmp = Image.GetCacheInnerImage(image) as System.Drawing.Bitmap;
                if (cacheBmp == null)
                {

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image.Width,
                        image.Height,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    //
                    PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize((PixelFarm.Agg.ActualImage)image, bmp);
                    //
                    Image.SetCacheInnerImage(image, bmp);
                    return bmp;
                }
                else
                {
                    //check if cache image is update or not 
                    return cacheBmp;
                }
            }
            else
            {
                //other image
                return Image.GetCacheInnerImage(image) as System.Drawing.Bitmap;
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

            gx.DrawImage(ResolveInnerBmp(image),
                destRect.ToRectF(),
                srcRect.ToRectF(),
                System.Drawing.GraphicsUnit.Pixel);
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
                //loop draw
                var inner = ResolveInnerBmp(image);
                for (int i = 0; i < j;)
                {
                    gx.DrawImage(inner,
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

            System.Drawing.Bitmap inner = ResolveInnerBmp(image);
            if (image.IsReferenceImage)
            {
                gx.DrawImage(inner,
                    destRect.ToRectF(),
                     new System.Drawing.RectangleF(
                         image.ReferenceX, image.ReferenceY,
                         image.Width, image.Height),
                    System.Drawing.GraphicsUnit.Pixel);
            }
            else
            {
                gx.DrawImage(inner, destRect.ToRectF());
            }
        }
        public void FillPath(Color color, GraphicsPath gfxPath)
        {

            //solid color
            var prevColor = internalSolidBrush.Color;
            internalSolidBrush.Color = ConvColor(color);
            System.Drawing.Drawing2D.GraphicsPath innerPath = ResolveGraphicsPath(gfxPath);
            gx.FillPath(internalSolidBrush, innerPath);
            internalSolidBrush.Color = prevColor;
        }
        /// <summary>
        /// Fills the interior of a <see cref="T:System.Drawing.Drawing2D.GraphicsPath"/>.
        /// </summary>
        /// <param name="brush"><see cref="T:System.Drawing.Brush"/> that determines the characteristics of the fill. </param><param name="path"><see cref="T:System.Drawing.Drawing2D.GraphicsPath"/> that represents the path to fill. </param><exception cref="T:System.ArgumentNullException"><paramref name="brush"/> is null.-or-<paramref name="path"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public void FillPath(Brush brush, GraphicsPath path)
        {

            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        SolidBrush solidBrush = (SolidBrush)brush;
                        var prevColor = internalSolidBrush.Color;
                        internalSolidBrush.Color = ConvColor(solidBrush.Color);
                        //
                        System.Drawing.Drawing2D.GraphicsPath innerPath = ResolveGraphicsPath(path);
                        gx.FillPath(internalSolidBrush, innerPath);
                        //
                        internalSolidBrush.Color = prevColor;
                    }
                    break;
                case BrushKind.LinearGradient:
                    {
                        LinearGradientBrush solidBrush = (LinearGradientBrush)brush;
                        var prevColor = internalSolidBrush.Color;
                        internalSolidBrush.Color = ConvColor(solidBrush.Color);
                        //
                        System.Drawing.Drawing2D.GraphicsPath innerPath = ResolveGraphicsPath(path);
                        gx.FillPath(internalSolidBrush, innerPath);
                        //
                        internalSolidBrush.Color = prevColor;
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
        }
        static System.Drawing.Drawing2D.GraphicsPath ResolveGraphicsPath(GraphicsPath path)
        {
            //convert from graphics path to internal presentation
            System.Drawing.Drawing2D.GraphicsPath innerPath = path.InnerPath as System.Drawing.Drawing2D.GraphicsPath;
            if (innerPath != null)
            {
                return innerPath;
            }
            //--------
            innerPath = new System.Drawing.Drawing2D.GraphicsPath();
            path.InnerPath = innerPath;
            List<float> points;
            List<PathCommand> cmds;
            GraphicsPath.GetPathData(path, out points, out cmds);
            int j = cmds.Count;
            int p_index = 0;
            for (int i = 0; i < j; ++i)
            {
                PathCommand cmd = cmds[i];
                switch (cmd)
                {
                    default:
                        throw new NotSupportedException();
                    case PathCommand.Arc:
                        innerPath.AddArc(
                            points[p_index],
                            points[p_index + 1],
                            points[p_index + 2],
                            points[p_index + 3],
                            points[p_index + 4],
                            points[p_index + 5]);
                        p_index += 6;
                        break;
                    case PathCommand.Bezier:
                        innerPath.AddBezier(
                            points[p_index],
                            points[p_index + 1],
                            points[p_index + 2],
                            points[p_index + 3],
                            points[p_index + 4],
                            points[p_index + 5],
                            points[p_index + 6],
                            points[p_index + 7]);
                        p_index += 8;
                        break;
                    case PathCommand.CloseFigure:
                        innerPath.CloseFigure();
                        break;
                    case PathCommand.Ellipse:
                        innerPath.AddEllipse(
                            points[p_index],
                            points[p_index + 1],
                            points[p_index + 2],
                            points[p_index + 3]);
                        p_index += 4;
                        break;
                    case PathCommand.Line:
                        innerPath.AddLine(
                            points[p_index],
                            points[p_index + 1],
                            points[p_index + 2],
                            points[p_index + 3]);
                        p_index += 4;
                        break;
                    case PathCommand.Rect:
                        innerPath.AddRectangle(
                           new System.Drawing.RectangleF(
                          points[p_index],
                          points[p_index + 1],
                          points[p_index + 2],
                          points[p_index + 3]));
                        p_index += 4;
                        break;
                    case PathCommand.StartFigure:
                        break;
                }
            }


            return innerPath;
        }
        public void FillPolygon(Brush brush, PointF[] points)
        {

            var pps = ConvPointFArray(points);
            //use internal solid color            
            gx.FillPolygon(brush.InnerBrush as System.Drawing.Brush, pps);
        }
        public void FillPolygon(Color color, PointF[] points)
        {

            var pps = ConvPointFArray(points);
            internalSolidBrush.Color = ConvColor(color);
            gx.FillPolygon(this.internalSolidBrush, pps);
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
        RequestFont currentTextFont = null;
        Color mycurrentTextColor = Color.Black;
        //public override float GetCharWidth(RequestFont f, char c)
        //{
        //    WinGdiFont winFont = WinGdiFontSystem.GetWinGdiFont(f);
        //    return winFont.GetGlyph(c).horiz_adv_x >> 6;
        //}
        public void DrawText(char[] buffer, int x, int y)
        {

            var clipRect = currentClipRect;
            clipRect.Offset(canvasOriginX, canvasOriginY);
            //1.
            win32MemDc.SetClipRect(clipRect.Left, clipRect.Top, clipRect.Width, clipRect.Height);
            //2.
            NativeTextWin32.TextOut(win32MemDc.DC, canvasOriginX + x, canvasOriginY + y, buffer, buffer.Length);
            //3
            win32MemDc.ClearClipRect();
        }
        public void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {


            var clipRect = System.Drawing.Rectangle.Intersect(logicalTextBox.ToRect(), currentClipRect);
            //1.
            clipRect.Offset(canvasOriginX, canvasOriginY);
            //2.
            win32MemDc.SetClipRect(clipRect.Left, clipRect.Top, clipRect.Width, clipRect.Height);
            //3.
            NativeTextWin32.TextOut(win32MemDc.DC, canvasOriginX + logicalTextBox.X, canvasOriginY + logicalTextBox.Y, buffer, buffer.Length);
            //4.
            win32MemDc.ClearClipRect();


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
                    new Rectangle(currentClipRect.Left,
                        currentClipRect.Top,
                        currentClipRect.Width,
                        currentClipRect.Height));
                //2. offset to canvas origin 
                clipRect.Offset(canvasOriginX, canvasOriginY);
                //3. set rect rgn
                win32MemDc.SetClipRect(clipRect.X, clipRect.Y, clipRect.Width, clipRect.Height);

                unsafe
                {
                    fixed (char* startAddr = &str[0])
                    {
                        //4.
                        NativeTextWin32.TextOutUnsafe(originalHdc,
                            (int)logicalTextBox.X + canvasOriginX,
                            (int)logicalTextBox.Y + canvasOriginY,
                            (startAddr + startAt), len);
                    }
                }
                //5. clear rect rgn
                win32MemDc.ClearClipRect();
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
                    new Rectangle(currentClipRect.Left,
                        currentClipRect.Top,
                        currentClipRect.Width,
                        currentClipRect.Height));
                //2. offset to canvas origin 
                clipRect.Offset(canvasOriginX, canvasOriginY);
                //3. set rect rgn
                win32MemDc.SetClipRect(clipRect.X, clipRect.Y, clipRect.Width, clipRect.Height);

                unsafe
                {
                    fixed (char* startAddr = &str[0])
                    {
                        //4.
                        NativeTextWin32.TextOutUnsafe(originalHdc,
                            (int)logicalTextBox.X + canvasOriginX,
                            (int)logicalTextBox.Y + canvasOriginY,
                            (startAddr + startAt), len);
                    }
                }
                //5. clear rect rgn
                win32MemDc.ClearClipRect();
#if DEBUG
                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
                //        logicalTextBox.X + canvasOriginX,
                //        logicalTextBox.Y + canvasOriginY);
#endif



            }
        }
        //====================================================
        public RequestFont CurrentFont
        {
            get
            {
                return currentTextFont;
            }
            set
            {

                this.currentTextFont = value;
                win32MemDc.SetFont(WinGdiFontSystem.GetWinGdiFont(value).CachedHFont());
            }
        }
        public Color CurrentTextColor
        {
            get
            {
                return mycurrentTextColor;
            }
            set
            {
                mycurrentTextColor = value;
                win32MemDc.SetSolidTextColor(value.R, value.G, value.B);
                //int rgb = (value.B & 0xFF) << 16 | (value.G & 0xFF) << 8 | value.R;
                //MyWin32.SetTextColor(originalHdc, rgb); 
                //SetTextColor(value);
                //this.currentTextColor = ConvColor(value);
                //IntPtr hdc = gx.GetHdc();
                //MyWin32.SetTextColor(hdc, MyWin32.ColorToWin32(value));
                //gx.ReleaseHdc();
            }
        }
    }
}