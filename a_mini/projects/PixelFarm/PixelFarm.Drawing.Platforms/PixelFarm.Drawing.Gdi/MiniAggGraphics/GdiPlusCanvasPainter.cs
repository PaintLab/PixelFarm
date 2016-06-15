//2016 MIT, WinterDev

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;
namespace PixelFarm.Drawing.WinGdi
{
    public class GdiPlusCanvasPainter : CanvasPainter
    {
        CanvasGraphics2dGdi _gfx;
        RectInt _clipBox;
        ColorRGBA _fillColor;
        int _width, _height;
        Agg.Fonts.Font _font;
        ColorRGBA _strokeColor;
        double _strokeWidth;
        bool _useSubPixelRendering;
        Graphics _internalGfx;
        PixelFarm.Agg.VertexSource.CurveFlattener curveFlattener;
        public GdiPlusCanvasPainter(CanvasGraphics2dGdi gfx)
        {
            _width = 800;
            _height = 600;
            _gfx = gfx;
            _internalGfx = gfx.InternalGraphics;
            //credit:
            //http://stackoverflow.com/questions/1485745/flip-coordinates-when-drawing-to-control
            _internalGfx.ScaleTransform(1.0F, -1.0F);// Flip the Y-Axis
            _internalGfx.TranslateTransform(0.0F, -(float)Height);// Translate the drawing area accordingly            
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
            }
        }

        public override Agg.Fonts.Font CurrentFont
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }
        public override ColorRGBA FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
            }
        }

        public override Graphics2D Graphics
        {
            get
            {
                return _gfx;
            }
        }

        public override int Height
        {
            get
            {
                return _height;
            }
        }

        public override ColorRGBA StrokeColor
        {
            get
            {
                return _strokeColor;
            }
            set
            {
                _strokeColor = value;
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
                _strokeWidth = value;
            }
        }

        public override bool UseSubPixelRendering
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

        public override void Clear(ColorRGBA color)
        {
            _gfx.Clear(color);
        }
        public override void DoFilterBlurRecursive(RectInt area, int r)
        {
            throw new NotImplementedException();
        }

        public override void DoFilterBlurStack(RectInt area, int r)
        {
            throw new NotImplementedException();
        }

        public override void Draw(VertexStore vxs)
        {
            VxsHelper.DrawVxsSnap(_internalGfx, new VertexStoreSnap(vxs), _fillColor);
        }

        public override void DrawBezierCurve(float startX, float startY, float endX, float endY, float controlX1, float controlY1, float controlX2, float controlY2)
        {
            throw new NotImplementedException();
        }

        public override void DrawEllipse()
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(ActualImage actualImage, params AffinePlan[] affinePlans)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(ActualImage actualImage, double x, double y)
        {
            throw new NotImplementedException();
        }

        public override void DrawRoundRect(double left, double bottom, double right, double top, double radius)
        {
            throw new NotImplementedException();
        }

        public override void DrawString(string text, double x, double y)
        {
            //use current brush and font
            throw new NotImplementedException();
        }

        public override void Fill(VertexStore vxs)
        {
            VxsHelper.DrawVxsSnap(_internalGfx, new VertexStoreSnap(vxs), _fillColor);
        }

        public override void Fill(VertexStoreSnap snap)
        {
            VxsHelper.DrawVxsSnap(_internalGfx, snap, _fillColor);
        }

        public override void Fill(VertexStore vxs, ISpanGenerator spanGen)
        {
            //fill with ispan generator
            throw new NotImplementedException();
        }

        public override void FillCircle(double x, double y, double radius)
        {
            throw new NotImplementedException();
        }

        public override void FillCircle(double x, double y, double radius, ColorRGBA color)
        {
            throw new NotImplementedException();
        }

        public override void FillEllipse(double left, double bottom, double right, double top, int nsteps)
        {
            throw new NotImplementedException();
        }

        public override void FillRectangle(double left, double bottom, double right, double top)
        {
            using (System.Drawing.SolidBrush br = new System.Drawing.SolidBrush(VxsHelper.ToDrawingColor(_fillColor)))
            {
                _internalGfx.FillRectangle(br, System.Drawing.RectangleF.FromLTRB((float)left, (float)top, (float)right, (float)bottom));
            }
        }
        public override void FillRectangle(double left, double bottom, double right, double top, ColorRGBA fillColor)
        {
            using (System.Drawing.SolidBrush br = new System.Drawing.SolidBrush(VxsHelper.ToDrawingColor(fillColor)))
            {
                _internalGfx.FillRectangle(br, System.Drawing.RectangleF.FromLTRB((float)left, (float)top, (float)right, (float)bottom));
            }
        }
        public override void FillRectLBWH(double left, double bottom, double width, double height)
        {
            using (System.Drawing.SolidBrush br = new System.Drawing.SolidBrush(VxsHelper.ToDrawingColor(_fillColor)))
            {
                _internalGfx.FillRectangle(br, new System.Drawing.RectangleF((float)left, (float)(bottom - height), (float)width, (float)height));
            }
        }

        public override void FillRoundRectangle(double left, double bottom, double right, double top, double radius)
        {
            throw new NotImplementedException();
        }

        public override VertexStore FlattenCurves(VertexStore srcVxs)
        {
            if (curveFlattener == null)
            {
                curveFlattener = new Agg.VertexSource.CurveFlattener();
            }
            return curveFlattener.MakeVxs(srcVxs);
        }

        public override void Line(double x1, double y1, double x2, double y2)
        {
            using (System.Drawing.Pen p = new System.Drawing.Pen(VxsHelper.ToDrawingColor(_strokeColor)))
            {
                _internalGfx.DrawLine(p, new System.Drawing.PointF((float)x1, (float)y1), new System.Drawing.PointF((float)x2, (float)y2));
            }
        }

        public override void Line(double x1, double y1, double x2, double y2, ColorRGBA color)
        {
            using (System.Drawing.Pen p = new System.Drawing.Pen(VxsHelper.ToDrawingColor(color)))
            {
                _internalGfx.DrawLine(p, new System.Drawing.PointF((float)x1, (float)y1), new System.Drawing.PointF((float)x2, (float)y2));
            }
        }
        public override void PaintSeries(VertexStore vxs, ColorRGBA[] colors, int[] pathIndexs, int numPath)
        {
            for (int i = 0; i < numPath; ++i)
            {
                VxsHelper.DrawVxsSnap(_internalGfx, new VertexStoreSnap(vxs, pathIndexs[i]), colors[i]);
            }
        }

        public override void Rectangle(double left, double bottom, double right, double top)
        {
            using (System.Drawing.Pen p = new System.Drawing.Pen(VxsHelper.ToDrawingColor(_strokeColor)))
            {
                _internalGfx.DrawRectangle(p, System.Drawing.Rectangle.FromLTRB((int)left, (int)top, (int)right, (int)bottom));
            }
        }

        public override void Rectangle(double left, double bottom, double right, double top, ColorRGBA color)
        {
            using (System.Drawing.Pen p = new System.Drawing.Pen(VxsHelper.ToDrawingColor(color)))
            {
                _internalGfx.DrawRectangle(p, System.Drawing.Rectangle.FromLTRB((int)left, (int)top, (int)right, (int)bottom));
            }
        }

        public override void SetClipBox(int x1, int y1, int x2, int y2)
        {
        }
    }
}