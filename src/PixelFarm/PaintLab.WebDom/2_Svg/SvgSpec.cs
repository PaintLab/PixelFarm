//Apache2, 2014-2018, WinterDev
//MS-PL,  

using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.Css;
namespace LayoutFarm.Svg
{
    public class SvgVisualSpec
    {
        Color fillColor = Color.Black;
        Color strokeColor = Color.Transparent;
        CssLength cssLen;

        public bool HasFillColor { get; set; }
        public bool HasStrokeColor { get; set; }
        public bool HasStrokeWidth { get; set; }

        public PixelFarm.Agg.Transform.Affine Transform { get; set; }
        public Color FillColor
        {
            get { return this.fillColor; }
            set
            {
                this.fillColor = value;
                this.HasFillColor = true;
            }
        }
        public Color StrokeColor
        {
            get { return this.strokeColor; }
            set
            {
                this.strokeColor = value;
                this.HasStrokeColor = true;
            }
        }
        public CssLength StrokeWidth
        {
            get { return cssLen; }
            set
            {

                cssLen = value;
                this.HasStrokeWidth = true;
            }
        }
        public string Id { get; set; }
        public string Class { get; set; }

    }
    public class SvgRectSpec : SvgVisualSpec
    {
        public CssLength X
        {
            get;
            set;
        }
        public CssLength Y
        {
            get;
            set;
        }
        public CssLength Width
        {
            get;
            set;
        }
        public CssLength Height
        {
            get;
            set;
        }

        public CssLength CornerRadiusX
        {
            get;
            set;
        }
        public CssLength CornerRadiusY
        {
            get;
            set;
        }
    }
    public class SvgCircleSpec : SvgVisualSpec
    {
        public CssLength X
        {
            get;
            set;
        }
        public CssLength Y
        {
            get;
            set;
        }
        public CssLength Radius
        {
            get;
            set;
        }
    }
    public class SvgEllipseSpec : SvgVisualSpec
    {
        public CssLength X
        {
            get;
            set;
        }
        public CssLength Y
        {
            get;
            set;
        }
        public CssLength RadiusX
        {
            get;
            set;
        }
        public CssLength RadiusY
        {
            get;
            set;
        }
    }
    public class SvgPolygonSpec : SvgVisualSpec
    {
        public PointF[] Points { get; set; }
    }
    public class SvgPolylineSpec : SvgVisualSpec
    {
        public PointF[] Points { get; set; }
    }

    public class SvgPathSpec : SvgVisualSpec
    {
        public CssLength X
        {
            get;
            set;
        }
        public CssLength Y
        {
            get;
            set;
        }
        public CssLength Width
        {
            get;
            set;
        }
        public CssLength Height
        {
            get;
            set;
        }
    }
    public class SvgLineSpec : SvgVisualSpec
    {
        public CssLength X1
        {
            get;
            set;
        }
        public CssLength Y1
        {
            get;
            set;
        }
        public CssLength X2
        {
            get;
            set;
        }
        public CssLength Y2
        {
            get;
            set;
        }
    }
    public class StopColorPoint
    {
        public Color StopColor
        {
            get;
            set;
        }
        public CssLength Offset
        {
            get;
            set;
        }
    }
}