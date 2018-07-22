//Apache2, 2014-present, WinterDev
//MS-PL,  

using LayoutFarm.WebDom;

using LayoutFarm.Css;
namespace LayoutFarm.Svg
{


    public class SvgVisualSpec
    {
        CssColor fillColor = CssColor.Black;
        CssColor strokeColor = CssColor.Transparent;
        CssLength cssLen;

        public bool HasFillColor { get; set; }
        public bool HasStrokeColor { get; set; }
        public bool HasStrokeWidth { get; set; }

        public SvgTransform Transform { get; set; }
        public CssColor FillColor
        {
            get { return this.fillColor; }
            set
            {
                this.fillColor = value;
                this.HasFillColor = true;
            }
        }
        public CssColor StrokeColor
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

        public SvgAttributeLink ClipPathLink { get; set; }        
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
        public CssPoint[] Points { get; set; }
    }
    public class SvgPolylineSpec : SvgVisualSpec
    {
        public CssPoint[] Points { get; set; }
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

        public string D { get; set; }
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
        public CssColor StopColor
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