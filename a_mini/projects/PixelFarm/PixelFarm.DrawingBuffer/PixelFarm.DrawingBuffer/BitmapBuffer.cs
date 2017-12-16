using System;
namespace PixelFarm.DrawingBuffer
{
    public abstract class GeneralTransform
    {
        public abstract RectD TransformBounds(RectD r1);
        public abstract PointD Transform(PointD p);
        MatrixTransform _inverseVersion;
        public MatrixTransform Inverse
        {
            get
            {
                if (_inverseVersion == null)
                {
                    //create inverse version
                    _inverseVersion = new MatrixTransform();
                }
                return _inverseVersion;
            }
        }

    }
    public class MatrixTransform : GeneralTransform
    {
        //System.Drawing.Drawing2D.Matrix mm1 = new System.Drawing.Drawing2D.Matrix(); 
        //System.Drawing.PointF[] _tmp = new System.Drawing.PointF[1]; 
        public override PointD Transform(PointD p)
        {
            throw new System.NotImplementedException();
            //_tmp[0] = new System.Drawing.PointF((float)p.X, (float)p.Y);
            //mm1.TransformPoints(_tmp);
            //return new PointD(_tmp[0].X, _tmp[0].Y);
        }

        //System.Drawing.PointF[] _tmp2 = new System.Drawing.PointF[4];
        public override RectD TransformBounds(RectD r1)
        {
            throw new System.NotImplementedException();
            //_tmp2[0] = new System.Drawing.PointF((float)r1.Left, (float)r1.Top);
            //_tmp2[1] = new System.Drawing.PointF((float)r1.Right, (float)r1.Top);
            //_tmp2[2] = new System.Drawing.PointF((float)r1.Right, (float)r1.Bottom);
            //_tmp2[3] = new System.Drawing.PointF((float)r1.Left, (float)r1.Bottom);
            ////find a new bound

            //return new RectD(_tmp2[0].X, _tmp2[0].Y, _tmp2[2].X - _tmp[0].X, _tmp2[2].Y - _tmp2[1].Y);
        }
    }
    public struct RectD
    {
        public RectD(double left, double top, double width, double height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }
        public RectD(PointD location, SizeD size)
        {
            this.Left = location.Left;
            this.Top = location.Top;
            this.Width = size.Width;
            this.Height = size.Height;
        }
        public double Left { get; private set; }
        public double Top { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public double X { get { return this.Left; } }
        public double Y { get { return this.Top; } }
        public double Bottom { get { return Y + Height; } }
        public double Right { get { return Left + Width; } }
        public bool IsEmpty
        {
            get
            {
                //TODO: eval once
                return this.Left == 0 && this.Top == 0
                    && this.Width == 0 && this.Height == 0;
            }
        }
        private bool IntersectsWithInclusive(RectD r)
        {
            return !((Left > r.Right) || (Right < r.Left) ||
                (Top > r.Bottom) || (Bottom < r.Top));
        }
        public static RectD Intersect(RectD a, RectD b)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            if (!a.IntersectsWithInclusive(b))
                return new RectD(0, 0, 0, 0);
            return RectD.FromLTRB(
                Math.Max(a.Left, b.Left),
                Math.Max(a.Top, b.Top),
                Math.Min(a.Right, b.Right),
                Math.Min(a.Bottom, b.Bottom));
        }

        public static RectD FromLTRB(double left, double top, double right, double bottom)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            return new RectD(left, top, right - left, bottom - top);
        }

        /// <summary>
        ///	Intersect Method
        /// </summary>
        ///
        /// <remarks>
        ///	Replaces the Rectangle with the intersection of itself
        ///	and another Rectangle.
        /// </remarks>

        public void Intersect(RectD rect)
        {
            this = RectD.Intersect(this, rect);
        }
    }
    public struct PointD
    {
        public PointD(int x, int y)
        {
            this.Left = x;
            this.Top = y;
        }
        public PointD(double x, double y)
        {
            this.Left = x;
            this.Top = y;
        }
        public double Left { get; private set; }
        public double Top { get; private set; }
        public double X
        {
            get
            {
                return this.Left;
            }

            set
            {
                this.Left = value;
            }
        }
        public double Y
        {
            get
            {
                return this.Top;
            }
            set
            {
                this.Top = value;
            }

        }
    }
    public struct SizeD
    {
        public SizeD(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public SizeD(double w, double h)
        {
            this.Width = w;
            this.Height = h;
        }
        public double Width { get; private set; }
        public double Height { get; private set; }
    }
    public struct ColorInt
    {
        //see https://github.com/PaintLab/PixelFarm/issues/12
        //we store color as 'straight alpha'
        byte _r, _g, _b, _a;

        public byte R { get { return _r; } }
        public byte G { get { return _g; } }
        public byte B { get { return _b; } }
        public byte A { get { return _a; } }

        public static ColorInt CreateNew(ColorInt oldColor, byte a)
        {
            ColorInt c = new ColorInt();
            c._r = oldColor._r;
            c._g = oldColor._g;
            c._b = oldColor._b;
            c._a = a;
            return c;
        }
        public static ColorInt FromArgb(byte a, byte r, byte g, byte b)
        {
            ColorInt c = new ColorInt();
            c._r = r;
            c._g = g;
            c._b = b;
            c._a = a;
            return c;
        }

        public static bool operator ==(ColorInt c1, ColorInt c2)
        {
            return (uint)((c1.A << 24) | (c1.R << 16) | (c1.G << 8) | (c1.B)) ==
                   (uint)((c2.A << 24) | (c2.R << 16) | (c2.G << 8) | (c2.B));
        }
        public static bool operator !=(ColorInt c1, ColorInt c2)
        {
            return (uint)((c1.A << 24) | (c1.R << 16) | (c1.G << 8) | (c1.B)) !=
                  (uint)((c2.A << 24) | (c2.R << 16) | (c2.G << 8) | (c2.B));
        }
        public override bool Equals(object obj)
        {
            if (obj is ColorInt)
            {
                ColorInt c = (ColorInt)obj;
                return c.A == this.A &&
                    c.B == this.B &&
                    c.R == this.R &&
                    c.G == this.G;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public struct Colors
    {
        public static ColorInt White = ColorInt.FromArgb(255, 255, 255, 255);
        public static ColorInt Black = ColorInt.FromArgb(255, 0, 0, 0);
        public static ColorInt Red = ColorInt.FromArgb(255, 255, 0, 0);
        public static ColorInt Blue = ColorInt.FromArgb(255, 0, 0, 255);
    }
    public class BitmapBuffer
    {
        //in this version , only 32 bits 
        public BitmapBuffer(int w, int h)
        {
            this.PixelWidth = w;
            this.PixelHeight = h;
            this.Pixels = new int[w * h];
        }
        public BitmapBuffer(int w, int h, int[] orgBuffer)
        {
            this.PixelWidth = w;
            this.PixelHeight = h;
            this.Pixels = orgBuffer;
        }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public int[] Pixels { get; private set; }
    }
}
