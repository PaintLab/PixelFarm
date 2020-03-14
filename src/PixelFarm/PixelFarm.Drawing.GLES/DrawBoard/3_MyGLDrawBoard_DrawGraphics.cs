//BSD, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.CpuBlit;

namespace PixelFarm.Drawing.GLES2
{

    partial class MyGLDrawBoard
    {
        class MyGLCanvasException : Exception { }

        Color _latestFillSolidColor;
        bool _latestFillCouldbeUsedAsTextBgHint;

        public override Color StrokeColor
        {
            get => _gpuPainter.StrokeColor;
            set => _gpuPainter.StrokeColor = value;
        }
        public override float StrokeWidth
        {
            get => (float)_gpuPainter.StrokeWidth;
            set => _gpuPainter.StrokeWidth = value;
        }
        public override void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
        {

            //throw new MyGLCanvasException();
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
                           _latestFillSolidColor = solidBrush.Color);
                        _latestFillCouldbeUsedAsTextBgHint = _latestFillSolidColor.A == 255;
                    }
                    break;
                case BrushKind.LinearGradient:
                case BrushKind.PolygonGradient:
                case BrushKind.CircularGraident:
                case BrushKind.Texture:
                    {
                        _latestFillCouldbeUsedAsTextBgHint = false;
                    }
                    break;
                default: throw new NotSupportedException();
            }
        }

        public override void FillRectangle(Color color, float left, float top, float width, float height)
        {
            _gpuPainter.FillRect(left, top, width, height, color);
            _latestFillSolidColor = color;
            _latestFillCouldbeUsedAsTextBgHint = color.A == 255;
        }
        public override void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            Color prev = _gpuPainter.StrokeColor;
            _gpuPainter.StrokeColor = color;
            _gpuPainter.DrawRect(left, top, width, height);
            _gpuPainter.StrokeColor = prev;//restore
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
            get => _gpuPainter.SmoothingMode;
            set => _gpuPainter.SmoothingMode = value;
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
            DrawingGL.GLBitmap glbmp = _gpuPainter.PainterContext.ResolveForGLBitmap(image);
            if (glbmp != null)
            {
#if DEBUG
                glbmp.dbugNotifyUsage();
#endif
                _gpuPainter.PainterContext.DrawSubImage(glbmp, destRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, destRect.Left, destRect.Top);
                _latestFillCouldbeUsedAsTextBgHint = false;
            }
        }
        public override void DrawImage(Image image, int x, int y)
        {
            DrawingGL.GLBitmap glbmp = _gpuPainter.PainterContext.ResolveForGLBitmap(image);
            if (glbmp != null)
            {
#if DEBUG
                glbmp.dbugNotifyUsage();
#endif
                _gpuPainter.PainterContext.DrawSubImage(glbmp, 0, 0, glbmp.Width, glbmp.Height, x, y);
                _latestFillCouldbeUsedAsTextBgHint = false;
            }
        }
        public override void DrawImages(Image image, RectangleF[] destAndSrcPairs)
        {
            //... 
            //throw new MyGLCanvasException();
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
            _latestFillCouldbeUsedAsTextBgHint = false;
        }

        /// <summary>
        /// Draws the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param><param name="destRect"><see cref="T:System.Drawing.Rectangle"/> structure that specifies the location and size of the drawn image. </param><exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override void DrawImage(Image image, RectangleF destRect)
        {
            //1. image from outside
            //resolve to internal presentation 
            if (image is PixelFarm.Drawing.BitmapAtlas.AtlasImageBinder atlasImg)
            {
                _gpuPainter.DrawImage(image, (float)destRect.X, (float)destRect.Y, (int)0, (int)0, (int)destRect.Width, (int)destRect.Height);
                _latestFillCouldbeUsedAsTextBgHint = false;
            }
            else
            {
                DrawingGL.GLBitmap glbmp = _gpuPainter.PainterContext.ResolveForGLBitmap(image);
                if (glbmp != null)
                {
#if DEBUG
                    glbmp.dbugNotifyUsage();
#endif
                    _gpuPainter.PainterContext.DrawImage(glbmp, destRect.Left, destRect.Top, destRect.Width, destRect.Height);
                    _latestFillCouldbeUsedAsTextBgHint = false;
                }
            }

        }
        public override void FillPolygon(Brush brush, PointF[] points)
        {
            //throw new MyGLCanvasException();
            //var pps = ConvPointFArray(points);
            ////use internal solid color            
            //gx.FillPolygon(brush.InnerBrush as System.Drawing.Brush, pps);
            _latestFillCouldbeUsedAsTextBgHint = false;
        }
        public override void FillPolygon(Color color, PointF[] points)
        {
            //throw new MyGLCanvasException();
            //var pps = ConvPointFArray(points);
            //internalSolidBrush.Color = ConvColor(color);
            //gx.FillPolygon(this.internalSolidBrush, pps);
            _latestFillCouldbeUsedAsTextBgHint = false;
        }

    }
}