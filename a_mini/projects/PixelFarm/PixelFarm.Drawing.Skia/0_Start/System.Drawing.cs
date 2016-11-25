//MIT, 2016, WinterDev

using System;
using PixelFarm.Agg;
namespace PixelFarm.Drawing.Skia
{
    class SkGraphics
    {

        //TODO: implement IDISPOSE
        MySkBmp bufferBmp;
        SkiaSharp.SKBitmap bmp;
        internal SkiaSharp.SKCanvas canvas;
        SkiaSharp.SKPaint stroke;
        SkiaSharp.SKPaint fill;

        int w;
        int h;

        PixelFarm.Drawing.SmoothingMode smoothingMode;
        public SkGraphics(int w, int h)
        {
            this.w = w;
            this.h = h;
            bmp = new SkiaSharp.SKBitmap(w, h);
            canvas = new SkiaSharp.SKCanvas(bmp);
            //
            stroke = new SkiaSharp.SKPaint();
            stroke.IsStroke = true;
            //
            fill = new SkiaSharp.SKPaint();
            fill.IsStroke = false;

        }
        private SkGraphics()
        {

        }
        public void Dispose()
        {

        }
        public static SkGraphics FromImage(MySkBmp bmp)
        {
            SkGraphics g = new SkGraphics();
            g.bufferBmp = bmp;
            g.bmp = bmp.internalBmp;
            g.w = bmp.Width;
            g.h = bmp.Height;
            return g;
        }
        public void ScaleTransform(float x, float y)
        {
            canvas.Scale(x, y);
        }
        public void TranslateTransform(float dx, float dy)
        {
            canvas.Translate(dx, dy);
        }
        public void ResetTransform()
        {
            canvas.ResetMatrix();
        }

        public PixelFarm.Drawing.SmoothingMode SmoothMode
        {
            get { return smoothingMode; }
            set
            {
                smoothingMode = value;
            }
        }
        public void Clear(PixelFarm.Drawing.Color color)
        {
            canvas.Clear(new SkiaSharp.SKColor(color.R, color.G, color.B, color.A));
        }
        public void FillPath(SkiaSharp.SKPath path, PixelFarm.Drawing.Color c)
        {
            var prevColor = fill.Color;
            fill.Color = new SkiaSharp.SKColor(c.R, c.G, c.B, c.A);
            canvas.DrawPath(path, fill);
            fill.Color = prevColor;
        }
        public void DrawPath(SkiaSharp.SKPath path, PixelFarm.Drawing.Color c)
        {
            var prevColor = stroke.Color;
            stroke.Color = new SkiaSharp.SKColor(c.R, c.G, c.B, c.A);
            canvas.DrawPath(path, stroke);
            stroke.Color = prevColor;
        }
        public void DrawPath(SkiaSharp.SKPath path)
        {

            canvas.DrawPath(path, stroke);
        }
        public void ClipRect(SkiaSharp.SKRect rect)
        {
            canvas.ClipRect(rect);
        }
        public void SetClip(SkiaSharp.SKRect rect)
        {
            canvas.ClipRect(rect);
        }
        public void SetClip(PixelFarm.Drawing.Rectangle rect)
        {
            canvas.ClipRect(new SkiaSharp.SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom));
        }
        public void ClearClip()
        {
            //?
            throw new NotSupportedException();
        }
        PixelFarm.Drawing.Color _penColor;
        public PixelFarm.Drawing.Color PenColor
        {
            get
            {
                return _penColor;
            }
            set
            {
                _penColor = value;
                stroke.Color = new SkiaSharp.SKColor(value.R, value.G, value.B, value.A);
            }
        }
        float _penWidth;
        public float PenWidth
        {
            get { return _penWidth; }
            set
            {
                _penWidth = value;
                stroke.StrokeWidth = value;
            }
        }
        public void FillPolygon(PixelFarm.Drawing.Color color, PixelFarm.Drawing.PointF[] points)
        {
            using (var polygon = CreatePolygon(points))
            {
                var prevColor = fill.Color;//save
                fill.Color = new SkiaSharp.SKColor(color.R, color.G, color.B, color.A);
                canvas.DrawPath(polygon, fill);
                fill.Color = prevColor;
            }
        }
        public void DrawPolygon(PixelFarm.Drawing.Color color, PixelFarm.Drawing.PointF[] points)
        {
            using (var polygon = CreatePolygon(points))
            {
                var prevColor = stroke.Color;    //save              
                stroke.Color = new SkiaSharp.SKColor(color.R, color.G, color.B, color.A);
                canvas.DrawPath(polygon, stroke);
                stroke.Color = prevColor; //reset
            }
        }
        static SkiaSharp.SKPath CreatePolygon(PixelFarm.Drawing.PointF[] points)
        {
            SkiaSharp.SKPath p = new SkiaSharp.SKPath();
            int j = points.Length;
            PixelFarm.Drawing.PointF p0 = new PixelFarm.Drawing.PointF();
            for (int i = 0; i < j; ++i)
            {
                if (i == 0)
                {
                    p0 = points[0];
                    p.MoveTo(p0.X, p0.Y);
                }
                else if (i == j - 1)
                {
                    //last one
                    var point = points[i];
                    p.LineTo(point.X, point.Y);
                    p.LineTo(p0.X, p0.Y);
                    p.Close();
                    break;
                }
                else
                {
                    var point = points[i];
                    p.LineTo(point.X, point.Y);
                }
            }
            return p;
        }
        public void DrawImage(MySkBmp bmp, float x, float y)
        {
            canvas.DrawBitmap(bmp.internalBmp, x, y);
        }
        public void DrawImage(MySkBmp image, PixelFarm.Drawing.RectangleF destRect, PixelFarm.Drawing.RectangleF srcRect)
        {
            canvas.DrawBitmap(image.internalBmp,
                new SkiaSharp.SKRect(srcRect.Left, srcRect.Top, srcRect.Right, srcRect.Bottom),
                new SkiaSharp.SKRect(destRect.Left, destRect.Top, destRect.Right, destRect.Bottom));


        }
        public void DrawImage(MySkBmp bmp, PixelFarm.Drawing.RectangleF dest)
        {
            var destRect = new SkiaSharp.SKRect(dest.X, dest.Y, dest.Right, dest.Bottom);
            canvas.DrawBitmap(bmp.internalBmp, destRect);
        }
        PixelFarm.Drawing.Color _solidBrushColor;
        public PixelFarm.Drawing.Color SolidBrushColor
        {
            get { return _solidBrushColor; }
            set
            {
                _solidBrushColor = value;
                fill.Color = new SkiaSharp.SKColor(value.R, value.G, value.B, value.A);
            }
        }
        public void FillEllipse(float cx, float cy, float rx, float ry)
        {
            canvas.DrawOval(cx, cy, rx, ry, fill);
        }
        public void DrawEllipse(float cx, float cy, float rx, float ry)
        {
            canvas.DrawOval(cx, cy, rx, ry, stroke);
        }
        public void DrawLine(float x0, float y0, float x1, float y1)
        {
            canvas.DrawLine(x0, y0, x1, y1, stroke);
        }

        public void DrawRectLTRB(float left, float top, float right, float bottom)
        {
            canvas.DrawRect(new SkiaSharp.SKRect(left, top, right, bottom), stroke);
        }
        public void FillRectLTRB(float left, float top, float right, float bottom)
        {
            canvas.DrawRect(new SkiaSharp.SKRect(left, top, right, bottom), fill);
        }
        public void DrawBezierCurve(float startX, float startY, float endX, float endY, float controlX1, float controlY1, float controlX2, float controlY2)
        {
            using (SkiaSharp.SKPath p = new SkiaSharp.SKPath())
            {
                p.MoveTo(startX, startY);
                p.CubicTo(controlX1, controlY1,
                    controlY1, controlY2,
                    endX, endY);
                canvas.DrawPath(p, stroke);
            }
        }
        public void DrawString(char[] buffer, int x, int y)
        {
            canvas.DrawText(new string(buffer, 0, buffer.Length), x, y, stroke);
        }
        public void DrawString(char[] buffer, PixelFarm.Drawing.Rectangle logicalTextBox)
        {

            //not fully support
            throw new NotSupportedException();
        }
        public PixelFarm.Drawing.RequestFont CurrentFont
        {
            get;
            set;
        }
        public PixelFarm.Drawing.Color CurrentTextColor
        {
            get;
            set;
        }
    }

    class MySkBmp : IDisposable
    {
        internal SkiaSharp.SKBitmap internalBmp;
        int w;
        int h;
        public MySkBmp(int w, int h)
        {
            this.w = w;
            this.h = h;
            internalBmp = new SkiaSharp.SKBitmap(w, h);
        }
        public void Dispose()
        {
            if (internalBmp != null)
            {
                internalBmp.Dispose();
                internalBmp = null;
            }
        }
        public int Width
        {
            get { return w; }
        }
        public int Height
        {
            get { return h; }
        }
        //----------------------         
        public static MySkBmp CopyFrom(ActualImage actualImage)
        {

            MySkBmp newBmp = new MySkBmp(actualImage.Width, actualImage.Height);
            newBmp.internalBmp.LockPixels();
            byte[] actualImgBuffer = ActualImage.GetBuffer(actualImage);

            System.Runtime.InteropServices.Marshal.Copy(
                 actualImgBuffer,
                 0,
                  newBmp.internalBmp.GetPixels(),
                  actualImgBuffer.Length);

            newBmp.internalBmp.UnlockPixels();
            return newBmp;
        }

    }




}
