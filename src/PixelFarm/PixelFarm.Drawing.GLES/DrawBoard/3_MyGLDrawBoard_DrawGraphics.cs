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
using PixelFarm.CpuBlit;
namespace PixelFarm.Drawing.GLES2
{

    partial class MyGLDrawBoard
    {
        class MyGLCanvasException : Exception { }

        float _strokeWidth = 1f;
        Color _fillSolidColor = Color.Transparent;
        Color _strokeColor = Color.Black;

        //==========================================================
        public override Color StrokeColor
        {
            get
            {
                return this._strokeColor;
            }
            set
            {
                _gpuPainter.StrokeColor = this._strokeColor = value;
            }
        }
        public override float StrokeWidth
        {
            get
            {
                return this._strokeWidth;
            }
            set
            {
                _gpuPainter.StrokeWidth = this._strokeWidth = value;
            }
        }

        public override void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
        {
            throw new MyGLCanvasException();
            //IntPtr gxdc = gx.GetHdc();
            //MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
            //MyWin32.BitBlt(destHdc, destArea.X, destArea.Y,
            //destArea.Width, destArea.Height, gxdc, sourceX, sourceY, MyWin32.SRCCOPY);
            //MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
            //gx.ReleaseHdc();
        }
        public override void Clear(PixelFarm.Drawing.Color c)
        {
            _gpuPainter.Clear(c);
        }
        public override void DrawPath(GraphicsPath gfxPath)
        {
            throw new MyGLCanvasException();
            //gx.DrawPath(internalPen, gfxPath.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }
        public override void FillRectangle(Brush brush, float left, float top, float width, float height)
        {

            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        //use default solid brush
                        SolidBrush solidBrush = (SolidBrush)brush;
                        _gpuPainter.FillRect(
                            left, top,
                            width, height,
                            solidBrush.Color);
                    }
                    break;
                case BrushKind.LinearGradient:
                    {
                        throw new MyGLCanvasException();
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
        public override void FillRectangle(Color color, float left, float top, float width, float height)
        {
            _gpuPainter.FillRect(left, top, width, height, color);
        }
        public override void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            _gpuPainter.DrawRect(left, top, width, height);
        }
        public override void DrawLine(float x1, float y1, float x2, float y2)
        {
            _gpuPainter.DrawLine(x1, y1, x2, y2);
        }



        /// <summary>
        /// Gets or sets the rendering quality for this <see cref="T:System.Drawing.Graphics"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Drawing.Drawing2D.SmoothingMode"/> values.
        /// </returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override SmoothingMode SmoothingMode
        {
            get
            {
                return _gpuPainter.SmoothingMode;
            }
            set
            {
                _gpuPainter.SmoothingMode = value;
            }
        }

        /// <summary>
        /// Draws the specified portion of the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param>
        /// <param name="destRect"><see cref="T:System.Drawing.RectangleF"/> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle. </param>
        /// <param name="srcRect"><see cref="T:System.Drawing.RectangleF"/> structure that specifies the portion of the <paramref name="image"/> object to draw. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception>
        public override void DrawImage(Image image, RectangleF destRect, RectangleF srcRect)
        {
            DrawingGL.GLBitmap glbmp = ResolveForGLBitmap(image);
            if (glbmp != null)
            {
                _gpuPainter.Canvas.DrawSubImage(glbmp, destRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, destRect.Left, destRect.Top);
            }
        }
        public override void DrawImage(Image image, int x, int y)
        {
            DrawingGL.GLBitmap glbmp = ResolveForGLBitmap(image);
            if (glbmp != null)
            {

                _gpuPainter.Canvas.DrawSubImage(glbmp, 0, 0, glbmp.Width, glbmp.Height, x, y);
            }
        }
        public override void DrawImages(Image image, RectangleF[] destAndSrcPairs)
        {
            //...

            throw new MyGLCanvasException();
            //int j = destAndSrcPairs.Length;
            //if (j > 1)
            //{
            //    if ((j % 2) != 0)
            //    {
            //        //make it even number
            //        j -= 1;
            //    }
            //    //loop draw
            //    var inner = image.InnerImage as System.Drawing.Image;
            //    for (int i = 0; i < j;)
            //    {
            //        gx.DrawImage(inner,
            //            destAndSrcPairs[i].ToRectF(),
            //            destAndSrcPairs[i + 1].ToRectF(),
            //            System.Drawing.GraphicsUnit.Pixel);
            //        i += 2;
            //    }
            //}
        }

        static DrawingGL.GLBitmap ResolveForGLBitmap(Image image)
        {
            //1.
            DrawingGL.GLBitmap glBmp = image as DrawingGL.GLBitmap;
            if (glBmp != null) return glBmp;
            //

            var cacheBmp = Image.GetCacheInnerImage(image) as DrawingGL.GLBitmap;
            if (cacheBmp != null)
            {
                return cacheBmp;
            }
            else
            {
                //TODO: review here
                //we should create 'borrow' method ? => send direct exact ptr to img buffer
                //for now, create a new one -- after we copy we, don't use it
                MemBitmap bmp = image as MemBitmap;
                if (bmp != null)
                {
                    glBmp = new DrawingGL.GLBitmap(new DrawingGL.LazyActualBitmapBufferProvider(bmp));
                    Image.SetCacheInnerImage(image, glBmp);
                    return glBmp;
                }
                else
                {
                    var req = new Image.ImgBufferRequestArgs(32, Image.RequestType.Copy);
                    image.RequestInternalBuffer(ref req);
                    //**
                    glBmp = new DrawingGL.GLBitmap(image.Width, image.Height, req.OutputBuffer32, req.IsInvertedImage);
                    Image.SetCacheInnerImage(image, glBmp);
                    return glBmp;
                }
            }
        }
        /// <summary>
        /// Draws the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param><param name="destRect"><see cref="T:System.Drawing.Rectangle"/> structure that specifies the location and size of the drawn image. </param><exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override void DrawImage(Image image, RectangleF destRect)
        {
            //1. image from outside
            //resolve to internal presentation 
            DrawingGL.GLBitmap glbmp = ResolveForGLBitmap(image);
            if (glbmp != null)
            {
                _gpuPainter.Canvas.DrawImage(glbmp, destRect.Left, destRect.Top, destRect.Width, destRect.Height);
            }

        }
        public override void FillPath(Color color, GraphicsPath gfxPath)
        {
            throw new MyGLCanvasException();
            //solid color
            //var prevColor = internalSolidBrush.Color;
            //internalSolidBrush.Color = ConvColor(color);
            //gx.FillPath(internalSolidBrush,
            //    gfxPath.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
            //internalSolidBrush.Color = prevColor;
        }
        /// <summary>
        /// Fills the interior of a <see cref="T:System.Drawing.Drawing2D.GraphicsPath"/>.
        /// </summary>
        /// <param name="brush"><see cref="T:System.Drawing.Brush"/> that determines the characteristics of the fill. </param><param name="path"><see cref="T:System.Drawing.Drawing2D.GraphicsPath"/> that represents the path to fill. </param><exception cref="T:System.ArgumentNullException"><paramref name="brush"/> is null.-or-<paramref name="path"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override void FillPath(Brush brush, GraphicsPath path)
        {
            throw new MyGLCanvasException();
            //switch (brush.BrushKind)
            //{
            //    case BrushKind.Solid:
            //        {
            //            SolidBrush solidBrush = (SolidBrush)brush;
            //            var prevColor = internalSolidBrush.Color;
            //            internalSolidBrush.Color = ConvColor(solidBrush.Color);
            //            gx.FillPath(internalSolidBrush,
            //                path.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
            //            internalSolidBrush.Color = prevColor;
            //        }
            //        break;
            //    case BrushKind.LinearGradient:
            //        {
            //            LinearGradientBrush solidBrush = (LinearGradientBrush)brush;
            //            var prevColor = internalSolidBrush.Color;
            //            internalSolidBrush.Color = ConvColor(solidBrush.Color);
            //            gx.FillPath(internalSolidBrush,
            //                path.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
            //            internalSolidBrush.Color = prevColor;
            //        }
            //        break;
            //    default:
            //        {
            //        }
            //        break;
            //}
        }

        public override void FillPolygon(Brush brush, PointF[] points)
        {
            throw new MyGLCanvasException();
            //var pps = ConvPointFArray(points);
            ////use internal solid color            
            //gx.FillPolygon(brush.InnerBrush as System.Drawing.Brush, pps);
        }
        public override void FillPolygon(Color color, PointF[] points)
        {
            throw new MyGLCanvasException();
            //var pps = ConvPointFArray(points);
            //internalSolidBrush.Color = ConvColor(color);
            //gx.FillPolygon(this.internalSolidBrush, pps);
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
}