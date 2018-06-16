//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

namespace PixelFarm.Drawing
{
    public abstract class Brush : System.IDisposable
    {
        public abstract BrushKind BrushKind { get; }
        public abstract void Dispose();

        public abstract object InnerBrush { get; set; }
    }

    public enum BrushKind
    {
        Solid,
        LinearGradient,
        CircularGraident,
        GeometryGradient,
        Texture
    }

    public sealed class SolidBrush : Brush
    {
        object innerBrush;
        public SolidBrush()
        {
            //default
            this.Color = Color.Transparent;
        }
        public SolidBrush(Color color)
        {
            this.Color = color;
        }
        public Color Color { get; set; }
        public override BrushKind BrushKind
        {
            get { return BrushKind.Solid; }
        }
        public override object InnerBrush
        {
            get
            {
                return this.innerBrush;
            }
            set
            {
                this.innerBrush = value;
            }
        }
        public override void Dispose()
        {
        }
    }

    public sealed class TextureBrush : Brush
    {
        object innerBrush;
        Image textureImage;
        public TextureBrush(Image textureImage)
        {
            this.textureImage = textureImage;
        }
        public override BrushKind BrushKind
        {
            get { return BrushKind.Texture; }
        }
        public Image TextureImage
        {
            get { return this.textureImage; }
        }


        public override object InnerBrush
        {
            get
            {
                return this.innerBrush;
            }
            set
            {
                this.innerBrush = value;
            }
        }
        public override void Dispose()
        {
        }
    }


    public abstract class GeometryGraidentBrush : Brush
    {

    }


    public sealed class LinearGradientBrush : GeometryGraidentBrush
    {
        object innerBrush;
        List<Color> stopColors = new List<Color>(2);
        List<PointF> stopPoints = new List<PointF>(2);
        public LinearGradientBrush(PointF stop1, Color c1, PointF stop2, Color c2)
        {
            this.stopColors.Add(c1);
            this.stopColors.Add(c2);
            this.stopPoints.Add(stop1);
            this.stopPoints.Add(stop2);
        }

        public Color Color
        {
            //first stop color
            get { return this.stopColors[0]; }
        }
        public override object InnerBrush
        {
            get
            {
                return this.innerBrush;
            }
            set
            {
                this.innerBrush = value;
            }
        }
        public override BrushKind BrushKind
        {
            get { return BrushKind.LinearGradient; }
        }
        public override void Dispose()
        {
        }
        public List<Color> GetColors()
        {
            return this.stopColors;
        }
        public List<PointF> GetStopPoints()
        {
            return this.stopPoints;
        }

    }


    public abstract class PenBase : System.IDisposable
    {
        public abstract void Dispose();
        public abstract float[] DashPattern { get; set; }
        public abstract float Width { get; set; }
        public abstract DashStyle DashStyle { get; set; }
        public abstract object InnerPen { get; set; }
        public abstract Brush Brush { get; }
    }
    public sealed class Pen : PenBase
    {
        float[] dashPattern;
        object innerPen;
        DashStyle dashStyle;
        float width = 1;//default 
        Brush brush;
        Color strokeColor;
        public Pen(Color color)
        {
            this.strokeColor = color;
            this.brush = new SolidBrush(color);
        }
        public override Brush Brush
        {
            get { return this.brush; }
        }
        public Pen(Brush brush)
        {
            this.brush = brush;
        }
        public Color StrokeColor
        {
            get { return this.strokeColor; }
        }

        public override float[] DashPattern
        {
            get
            {
                return dashPattern;
            }
            set
            {
                dashPattern = value;
            }
        }
        public override object InnerPen
        {
            get
            {
                return this.innerPen;
            }
            set
            {
                this.innerPen = value;
            }
        }
        public override float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }
        public override DashStyle DashStyle
        {
            get
            {
                return this.dashStyle;
            }
            set
            {
                this.dashStyle = value;
            }
        }
        public override void Dispose()
        {
        }
    }
}