//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.DrawingGL
{
    partial class GLPainter
    {
        Color _strokeColor;
        Pen _currentPen;
        Stroke _stroke = new Stroke(1);
        SimpleRectBorderBuilder _simpleBorderRectBuilder = new SimpleRectBorderBuilder();
        LineDashGenerator _lineDashGen;

        float[] _reuseableRectBordersXYs = new float[16];

        public override Pen CurrentPen
        {
            get => _currentPen;
            set => _currentPen = value;
        }
        public override Color StrokeColor
        {
            get => _strokeColor;
            set
            {
                _strokeColor = value;
                _pcx.StrokeColor = value;
            }
        }

        public override double StrokeWidth
        {
            get => _pcx.StrokeWidth;
            set
            {
                _pcx.StrokeWidth = (float)value;
                _stroke.Width = (float)value;
            }
        }
        /// <summary>
        /// we do NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        public override void Draw(VertexStore vxs)
        {
            if (!vxs.IsShared)
            {
                //
                PathRenderVx renderVx = VertexStore.GetBorderRenderVx(vxs) as PathRenderVx;
                if (renderVx == null)
                {

                }

            }

            if (StrokeWidth > 1)
            {
                using (VxsTemp.Borrow(out VertexStore v1))
                using (VectorToolBox.Borrow(out Stroke stroke))
                {
                    //convert large stroke to vxs
                    stroke.Width = StrokeWidth;
                    stroke.MakeVxs(vxs, v1);

                    Color prevColor = this.FillColor;
                    FillColor = this.StrokeColor;
                    Fill(v1);
                    FillColor = prevColor;
                }
            }
            else
            {
                _pcx.DrawGfxPath(_strokeColor,
                    _pathRenderVxBuilder.CreatePathRenderVx(vxs));
            }
        }

        public override void DrawEllipse(double left, double top, double width, double height)
        {
            double x = (left + width / 2);
            double y = (top + height / 2);
            double rx = Math.Abs(width / 2);
            double ry = Math.Abs(height / 2);

            using (VectorToolBox.Borrow(out Ellipse ellipse))
            using (VxsTemp.Borrow(out var v1, out var v2))
            {
                ellipse.Set(x, y, rx, ry);

                ellipse.MakeVxs(v1);
                _stroke.MakeVxs(v1, v2);
                //***
                //we fill the stroke's path
                _pcx.FillGfxPath(_strokeColor, _pathRenderVxBuilder.CreatePathRenderVx(v2));
            }


        }

        public override void DrawRect(double left, double top, double width, double height)
        {
            switch (_pcx.SmoothMode)
            {
                case SmoothMode.Smooth:
                    {
                        _pcx.StrokeColor = this.StrokeColor;
                        using (PixelFarm.Drawing.VxsTemp.Borrow(out Drawing.VertexStore v1))
                        using (PixelFarm.Drawing.VectorToolBox.Borrow(out CpuBlit.VertexProcessing.SimpleRect r))
                        {
                            r.SetRect(left + 0.5f, top + height + 0.5f, left + width - 0.5f, top - 0.5f);
                            r.MakeVxs(v1);
                            Draw(v1);
                        }
                    }
                    break;
                default:
                    {
                        //draw boarder with
                        if (StrokeWidth > 0 && StrokeColor.A > 0)
                        {
                            _simpleBorderRectBuilder.SetBorderWidth((float)StrokeWidth);
                            //_simpleBorderRectBuilder.BuildAroundInnerRefBounds(
                            //    (float)left, (float)top + (float)height, (float)left + (float)width, (float)top,
                            //    _reuseableRectBordersXYs);
                            _simpleBorderRectBuilder.BuildAroundInnerRefBounds(
                               (float)left, (float)top, (float)width, (float)height,
                               _reuseableRectBordersXYs);
                            //
                            _pcx.FillTessArea(StrokeColor,
                                _reuseableRectBordersXYs,
                                _simpleBorderRectBuilder.GetPrebuiltRectTessIndices());
                        }
                    }
                    break;
            }
        }
        //

        public override void DrawRenderVx(RenderVx renderVx)
        {
            _pcx.DrawRenderVx(_strokeColor, renderVx);
        }


        public override void DrawLine(double x1, double y1, double x2, double y2)
        {
            _pcx.StrokeColor = _strokeColor;
            if (_lineDashGen == null)
            {
                _pcx.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
            }
            else
            {
                //TODO: line dash pattern cache
                _pcx.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
            }
        }
         
        public override LineJoin LineJoin
        {
            get => _stroke.LineJoin;
            set => _stroke.LineJoin = value;
        }
        public override LineCap LineCap
        {
            get => _stroke.LineCap;
            set => _stroke.LineCap = value;
        }

        public override IDashGenerator LineDashGen
        {
            get => _lineDashGen;
            set => _lineDashGen = (LineDashGenerator)value;
        }

        public void DrawCircle(float centerX, float centerY, double radius)
        {
            DrawEllipse(centerX - radius, centerY - radius, radius + radius, radius + radius);
        } 

    }
}