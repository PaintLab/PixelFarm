//MIT, 2014-2016, WinterDev

using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.Skia
{
    partial class MySkiaCanvas
    {
        float strokeWidth = 1f;
        Color fillSolidColor = Color.Transparent;
        Color strokeColor = Color.Black;
        //==========================================================
        public override Color StrokeColor
        {
            get
            {
                return this.strokeColor;
            }
            set
            {
                gx.PenColor = this.strokeColor = value;
            }
        }
        public override float StrokeWidth
        {
            get
            {
                return this.strokeWidth;
            }
            set
            {

                gx.PenWidth = this.strokeWidth = value;
            }
        }

        public override void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
        {
            //render to destination?
            //win32?
            throw new NotSupportedException();
            //MyWin32.SetViewportOrgEx(win32MemDc.DC, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
            //MyWin32.BitBlt(
            //    destHdc, destArea.X, destArea.Y, destArea.Width, destArea.Height, //dest
            //    win32MemDc.DC, sourceX, sourceY, MyWin32.SRCCOPY); //src
            //MyWin32.SetViewportOrgEx(win32MemDc.DC, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);

        }
        public override void ClearSurface(PixelFarm.Drawing.Color c)
        {
            gx.Clear(c);
        }
        public override void DrawPath(GraphicsPath gfxPath)
        {
            //convert graphics path to skia path 
            gx.DrawPath(ResolveGraphicsPath(gfxPath));
        }
        public override void FillRectangle(Brush brush, float left, float top, float width, float height)
        {

            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        //use default solid brush
                        var prevColor = gx.SolidBrushColor;
                        SolidBrush solidBrush = (SolidBrush)brush;
                        gx.SolidBrushColor = solidBrush.Color;
                        gx.FillRectLTRB(left, top, right - left, top + height);
                        gx.SolidBrushColor = prevColor;
                    }
                    break;
                case BrushKind.LinearGradient:
                    {
                        //not - support

                        //draw with gradient
                        //LinearGradientBrush linearBrush = (LinearGradientBrush)brush;
                        //var colors = linearBrush.GetColors();
                        //var points = linearBrush.GetStopPoints();
                        //using (var linearGradBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        //     points[0].ToPointF(),
                        //     points[1].ToPointF(),
                        //     ConvColor(colors[0]),
                        //     ConvColor(colors[1])))
                        //{
                        //    gx.FillRectangle(linearGradBrush, left, top, width, height);
                        //}
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
            var prevColor = gx.SolidBrushColor;
            gx.SolidBrushColor = color;
            gx.FillRectLTRB(left, top, left + width, top + height);
            gx.SolidBrushColor = prevColor;
        }


        public override void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            var prevColor = gx.PenColor;
            gx.PenColor = color;
            gx.DrawRectLTRB(left, top, left + width, top + height);
            gx.PenColor = prevColor;
        }

        public override void DrawLine(float x1, float y1, float x2, float y2)
        {

            gx.DrawLine(x1, y1, x2, y2);
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
        public override SmoothingMode SmoothingMode
        {
            get;
            set;
            //get
            //{ 
            //    return (SmoothingMode)(gx.SmoothingMode);
            //}
            //set
            //{ 
            //    gx.SmoothingMode = (System.Drawing.Drawing2D.SmoothingMode)value;
            //}
        }

        static MySkBmp ResolveInnerBmp(Image image)
        {

            if (image is PixelFarm.Agg.ActualImage)
            {
                //this is known image
                var cacheBmp = Image.GetCacheInnerImage(image) as MySkBmp;
                if (cacheBmp == null)
                {

                    MySkBmp bmp = new MySkBmp(image.Width,
                       image.Height);
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
                return Image.GetCacheInnerImage(image) as MySkBmp;
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
            var bmp = Image.GetCacheInnerImage(image) as MySkBmp;
            if (bmp != null)
            {
                gx.DrawImage(bmp, destRect, srcRect);
            }

        }
        public override void DrawImages(Image image, RectangleF[] destAndSrcPairs)
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
                        destAndSrcPairs[i],
                        destAndSrcPairs[i + 1]);

                    i += 2;
                }
            }
        }
        /// <summary>
        /// Draws the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param><param name="destRect"><see cref="T:System.Drawing.Rectangle"/> structure that specifies the location and size of the drawn image. </param><exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override void DrawImage(Image image, RectangleF destRect)
        {

            MySkBmp inner = ResolveInnerBmp(image);
            if (image.IsReferenceImage)
            {
                gx.DrawImage(inner,
                    destRect,
                     new RectangleF(
                         image.ReferenceX, image.ReferenceY,
                         image.Width, image.Height));
            }
            else
            {
                gx.DrawImage(inner, destRect);
            }
        }
        public override void FillPath(Color color, GraphicsPath gfxPath)
        {
            //solid color

            //internalSolidBrush.Color = ConvColor(color);
            var innerPath = ResolveGraphicsPath(gfxPath);
            gx.FillPath(innerPath, color);
        }
        /// <summary>
        /// Fills the interior of a <see cref="T:System.Drawing.Drawing2D.GraphicsPath"/>.
        /// </summary>
        /// <param name="brush"><see cref="T:System.Drawing.Brush"/> that determines the characteristics of the fill. </param><param name="path"><see cref="T:System.Drawing.Drawing2D.GraphicsPath"/> that represents the path to fill. </param><exception cref="T:System.ArgumentNullException"><paramref name="brush"/> is null.-or-<paramref name="path"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override void FillPath(Brush brush, GraphicsPath path)
        {

            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        //SolidBrush solidBrush = (SolidBrush)brush;
                        //var prevColor = internalSolidBrush.Color;
                        //internalSolidBrush.Color = ConvColor(solidBrush.Color);
                        ////
                        //System.Drawing.Drawing2D.GraphicsPath innerPath = ResolveGraphicsPath(path);
                        //gx.FillPath(internalSolidBrush, innerPath);
                        ////
                        //internalSolidBrush.Color = prevColor;
                    }
                    break;
                case BrushKind.LinearGradient:
                    {
                        //LinearGradientBrush solidBrush = (LinearGradientBrush)brush;
                        //var prevColor = internalSolidBrush.Color;
                        //internalSolidBrush.Color = ConvColor(solidBrush.Color);
                        ////
                        //System.Drawing.Drawing2D.GraphicsPath innerPath = ResolveGraphicsPath(path);
                        //gx.FillPath(internalSolidBrush, innerPath);
                        ////
                        //internalSolidBrush.Color = prevColor;
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
        }
        static SkiaSharp.SKPath ResolveGraphicsPath(GraphicsPath path)
        {
            //convert from graphics path to internal presentation
            SkiaSharp.SKPath innerPath = path.InnerPath as SkiaSharp.SKPath;
            if (innerPath != null)
            {
                return innerPath;
            }
            //--------
            innerPath = new SkiaSharp.SKPath();
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

                        var oval = SkiaSharp.SKRect.Create(points[p_index],
                            points[p_index + 1],
                            points[p_index + 2],
                            points[p_index + 3]);

                        innerPath.ArcTo(oval,
                            points[p_index + 4],
                            points[p_index + 5],
                            true);
                        p_index += 6;
                        break;
                    case PathCommand.Bezier:
                        innerPath.MoveTo(points[p_index]
                            , points[p_index + 1]);
                        innerPath.CubicTo(
                            points[p_index + 2],
                            points[p_index + 3],
                            points[p_index + 4],
                            points[p_index + 5],
                            points[p_index + 6],
                            points[p_index + 7]);
                        p_index += 8;
                        break;
                    case PathCommand.CloseFigure:
                        //?
                        break;
                    case PathCommand.Ellipse:

                        innerPath.AddOval(
                            SkiaSharp.SKRect.Create(
                                points[p_index],
                            points[p_index + 1],
                            points[p_index + 2],
                            points[p_index + 3]
                                ));

                        p_index += 4;
                        break;
                    case PathCommand.Line:

                        innerPath.MoveTo(points[p_index],
                            points[p_index + 1]);
                        innerPath.LineTo(
                            points[p_index + 2],
                            points[p_index + 3]);
                        p_index += 4;
                        break;
                    case PathCommand.Rect:
                        innerPath.AddRect(
                            SkiaSharp.SKRect.Create(
                                points[p_index],
                            points[p_index + 1],
                            points[p_index + 2],
                            points[p_index + 3]
                                ));
                        p_index += 4;
                        break;
                    case PathCommand.StartFigure:
                        break;
                }
            }


            return innerPath;
        }
        public override void FillPolygon(Brush brush, PointF[] points)
        {
            //create polygon path
            if (brush is SolidBrush)
            {
                var prevColor = gx.SolidBrushColor;
                gx.SolidBrushColor = ((SolidBrush)brush).Color;
                gx.FillPolygon(gx.SolidBrushColor, points);
                gx.SolidBrushColor = prevColor;
            }
            else
            {
                throw new NotSupportedException();
            }

            //var pps = ConvPointFArray(points);
            ////use internal solid color            
            //gx.FillPolygon(brush.InnerBrush as System.Drawing.Brush, pps);
        }
        public override void FillPolygon(Color color, PointF[] points)
        {

            var prevColor = gx.SolidBrushColor;
            gx.SolidBrushColor = color;
            gx.FillPolygon(gx.SolidBrushColor, points);
            gx.SolidBrushColor = prevColor;

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