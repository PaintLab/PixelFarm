//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.DrawingGL
{
    partial class GLPainter
    {
        Color _fillColor;
        Brush _currentBrush;
        Brush _defaultBrush;
        float _fillOpacity;
        bool _hasFillOpacity;
       

        public override Color FillColor
        {
            get => _fillColor;
            set => _fillColor = value;
        }
        public override FillingRule FillingRule
        {
            //TODO: implement filling rule for GL
            //this need to change to tess level
            get => _pcx.FillingRule;
            set => _pcx.FillingRule = value;
        }
        public override float FillOpacity
        {
            get => _fillOpacity;
            set
            {
                //apply to all brush

                _fillOpacity = value;
                if (value < 0)
                {
                    _fillOpacity = 0;
                    _hasFillOpacity = true;
                }
                else if (value >= 1)
                {
                    _fillOpacity = 1;
                    _hasFillOpacity = false;
                }
                else
                {
                    _fillOpacity = value;
                    _hasFillOpacity = true;
                }
            }
        }
        public override Brush CurrentBrush
        {
            get => _currentBrush;
            set
            {
                //brush with its detail             
                //------------------------
                if (value == null)
                {
                    _currentBrush = _defaultBrush;
                    return;
                }
                //
                //
                switch (value.BrushKind)
                {
                    default:
                        break;
                    case BrushKind.Solid:
                        {
                            SolidBrush solidBrush = (SolidBrush)value;
                            _fillColor = solidBrush.Color;
                        }
                        break;
                    case BrushKind.LinearGradient:
                        {
                        }
                        break;
                    case BrushKind.CircularGraident:
                        break;

                    case BrushKind.Texture:

                        break;
                }
                _currentBrush = value;
            }

        }

        //

        public override void Fill(VertexStore vxs)
        {
            PathRenderVx pathRenderVx = null;
            if (!vxs.IsShared)
            {
                //check if we have cached PathRenderVx or not
                pathRenderVx = VertexStore.GetAreaRenderVx(vxs) as PathRenderVx;
                //
                if (pathRenderVx == null)
                {
                    VertexStore.SetAreaRenderVx(
                        vxs,
                        pathRenderVx = _pathRenderVxBuilder.CreatePathRenderVx(vxs));
                }

            }
            else
            {
                pathRenderVx = _pathRenderVxBuilder.CreatePathRenderVx(vxs);
            }


            switch (_currentBrush.BrushKind)
            {
                default:
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("unknown brush!");
#endif
                    break;
                case BrushKind.CircularGraident:
                case BrushKind.LinearGradient:

                    //resolve internal linear gradient brush impl
                    _pcx.FillGfxPath(_currentBrush, pathRenderVx);
                    break;
                case BrushKind.PolygonGradient:
                    //....
                    break;
                case BrushKind.Solid:
                    {
                        _pcx.FillGfxPath(
                            _fillColor,
                            pathRenderVx
                        );
                    }
                    break;
                case BrushKind.Texture:
                    break;
            }



        }

        public override void FillRenderVx(Brush brush, RenderVx renderVx)
        {
            _pcx.FillRenderVx(brush, renderVx);
        }
        public override void FillRenderVx(RenderVx renderVx)
        {
            _pcx.FillRenderVx(_fillColor, renderVx);
        }
        public override void FillRect(double left, double top, double width, double height)
        {
            switch (_currentBrush.BrushKind)
            {
                default:
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("unknown brush!");
#endif
                    break;
                case BrushKind.CircularGraident:
                case BrushKind.LinearGradient:
                case BrushKind.PolygonGradient:
                    {
                        //resolve internal linear gradient brush impl

                        using (VxsTemp.Borrow(out var v1))
                        using (VectorToolBox.Borrow(out SimpleRect rect))
                        {
                            rect.SetRectFromLTWH(left, top, width, height);
                            rect.MakeVxs(v1);

                            //convert to render vx
                            //TODO: optimize here ***
                            //we don't want to create path render vx everytime
                            //
                            //
                            PathRenderVx pathRenderVx = _pathRenderVxBuilder.CreatePathRenderVx(v1);
                            _pcx.FillGfxPath(_currentBrush, pathRenderVx);
                        }
                    }
                    break;
                case BrushKind.Solid:
                    _pcx.FillRect(_fillColor, left, top, width, height);
                    break;
                case BrushKind.Texture:
                    break;
            }

        }
        public override void FillEllipse(double left, double top, double width, double height)
        {
            //version 2:
            //agg's ellipse tools with smooth border

            double x = (left + width / 2);
            double y = (top + height / 2);
            double rx = Math.Abs(width / 2);
            double ry = Math.Abs(height / 2);
            //
            using (VectorToolBox.Borrow(out Ellipse ellipse))
            using (VxsTemp.Borrow(out var vxs))
            {
                ellipse.MakeVxs(vxs);
                //***
                //we fill  
                _pcx.FillGfxPath(_strokeColor, _pathRenderVxBuilder.CreatePathRenderVx(vxs));
            }

        }
        public void FillCircle(float x, float y, double radius)
        {
            FillEllipse(x - radius, y - radius, x + radius, y + radius);
        }
    }
}