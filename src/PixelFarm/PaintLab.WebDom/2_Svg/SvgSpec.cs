//Apache2, 2014-present, WinterDev
//MS-PL,  

using LayoutFarm.Css;
using PixelFarm.Drawing;

namespace LayoutFarm.Svg
{

    public abstract class SvgElemSpec
    {
        //TODO: review here
        public string Id { get; set; }
    }

    public class SvgVisualSpec : SvgElemSpec
    {
        Color fillColor = Color.Black;
        Color strokeColor = Color.Transparent;
        CssLength cssLen;


        public bool HasFillColor { get; set; }
        public bool HasStrokeColor { get; set; }
        public bool HasStrokeWidth { get; set; }

        public SvgTransform Transform { get; set; }

        public PixelFarm.Drawing.Color FillColor
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

   
        public string Class { get; set; }

        public SvgAttributeLink ClipPathLink { get; set; }
        public object ResolvedClipPath { get; set; }
    }

    public class SvgStyleSpec : SvgElemSpec
    {
        public string RawTextContent { get; set; }
    }

    public enum SvgAttributeLinkKind
    {
        Id,
    }
    public class SvgAttributeLink
    {
        public SvgAttributeLink(SvgAttributeLinkKind kind, string value)
        {
            this.Kind = kind;
            this.Value = value;
        }
        public string Value { get; private set; }
        public SvgAttributeLinkKind Kind { get; private set; }
#if DEBUG
        public override string ToString()
        {
            return Value;
        }
#endif
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
    public class SvgImageSpec : SvgVisualSpec
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

        public string ImageSrc
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
    public class SvgLinearGradientSpec : SvgVisualSpec
    {
        public System.Collections.Generic.List<StopColorPoint> StopList { get; set; }
        public CssLength X1 { get; set; }
        public CssLength Y1 { get; set; }
        public CssLength X2 { get; set; }
        public CssLength Y2 { get; set; }
    }
    public class SvgPolygonSpec : SvgVisualSpec
    {
        public PixelFarm.Drawing.PointF[] Points { get; set; }
    }
    public class SvgPolylineSpec : SvgVisualSpec
    {
        public PixelFarm.Drawing.PointF[] Points { get; set; }
    }

    public class SvgPathSpec : SvgVisualSpec
    {
        public SvgPathSpec()
        {
        }

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

        public string D
        {
            get;
            set;
        }
    }

    public class SvgTextSpec : SvgVisualSpec
    {
        public string FontFace { get; set; }
        public CssLength FontSize { get; set; }

        public string TextContent { get; set; }
        public object ExternalTextNode { get; set; }
        public CssLength X { get; set; }
        public CssLength Y { get; set; }

        public float ActualX { get; set; }
        public float ActualY { get; set; }
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