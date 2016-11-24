//2016 MIT, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;

namespace System.Drawing
{
    public class Graphics
    {

        //TODO: implement IDISPOSE
        Bitmap bufferBmp;
        SkiaSharp.SKBitmap bmp;
        internal SkiaSharp.SKCanvas canvas;
        SkiaSharp.SKPaint penPainter;
        SkiaSharp.SKPaint brushPainter;

        int w;
        int h;
        CompositingMode compositeMode;
        PixelFarm.Drawing.SmoothingMode smoothingMode;
        public Graphics(int w, int h)
        {
            this.w = w;
            this.h = h;
            bmp = new SkiaSharp.SKBitmap(w, h);
            canvas = new SkiaSharp.SKCanvas(bmp);
            //
            penPainter = new SkiaSharp.SKPaint();
            penPainter.IsStroke = true;
            //
            brushPainter = new SkiaSharp.SKPaint();
            brushPainter.IsStroke = false;

        }
        private Graphics()
        {

        }
        public void Dispose()
        {

        }
        public static Graphics FromImage(System.Drawing.Bitmap bmp)
        {
            Graphics g = new Drawing.Graphics();
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
        public CompositingMode CompositingMode
        {
            get { return compositeMode; }
            set
            {
                this.compositeMode = value;
            }
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
            brushPainter.Color = new SkiaSharp.SKColor(c.R, c.G, c.B, c.A);
            canvas.DrawPath(path, brushPainter);
        }
        public void DrawPath(SkiaSharp.SKPath path, PixelFarm.Drawing.Color c)
        {
            penPainter.Color = new SkiaSharp.SKColor(c.R, c.G, c.B, c.A);
            canvas.DrawPath(path, penPainter);
        }
        public void ClipRect(SkiaSharp.SKRect rect)
        {
            canvas.ClipRect(rect);
        }
        public void SetClip(SkiaSharp.SKRect rect)
        {
            canvas.ClipRect(rect);
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
                penPainter.Color = new SkiaSharp.SKColor(value.R, value.G, value.B, value.A);
            }
        }
        float _penWidth;
        public float PenWidth
        {
            get { return _penWidth; }
            set
            {
                _penWidth = value;
                penPainter.StrokeWidth = value;
            }
        }
        public void FillPolygon(PixelFarm.Drawing.Color color, PixelFarm.Drawing.PointF[] points)
        {
            using (var polygon = CreatePolygon(points))
            {
                var prevColor = brushPainter.Color;//save
                brushPainter.Color = new SkiaSharp.SKColor(color.R, color.G, color.B, color.A);
                canvas.DrawPath(polygon, brushPainter);
                brushPainter.Color = prevColor;
            } 
        }
        public void DrawPolygon(PixelFarm.Drawing.Color color, PixelFarm.Drawing.PointF[] points)
        {
            using (var polygon = CreatePolygon(points))
            {
                var prevColor = penPainter.Color;    //save              
                penPainter.Color = new SkiaSharp.SKColor(color.R, color.G, color.B, color.A);
                canvas.DrawPath(polygon, penPainter);
                penPainter.Color = prevColor; //reset
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
    }
    public class Bitmap
    {
        internal SkiaSharp.SKBitmap internalBmp;
        int w;
        int h;
        public Bitmap(int w, int h)
        {
            this.w = w;
            this.h = h;
            internalBmp = new SkiaSharp.SKBitmap(w, h);
        }
        public int Width
        {
            get { return w; }
        }
        public int Height
        {
            get { return h; }
        }

    }
    public class Pen
    {

        PixelFarm.Drawing.Color innerColor;
        public Pen(PixelFarm.Drawing.Color color)
        {
            this.innerColor = color;
        }
        public float Width
        {
            get;
            set;
        }
        public PixelFarm.Drawing.Color Color
        {
            get { return innerColor; }
            set
            {
                innerColor = value;
            }
        }

    }
    public class SolidBrush
    {
        PixelFarm.Drawing.Color color;
        public SolidBrush(PixelFarm.Drawing.Color color)
        {
            this.color = color;
        }
        public PixelFarm.Drawing.Color Color
        {
            get { return color; }
            set
            {
                color = value;
            }
        }
    }

    public enum CompositingMode
    {

    }

}
