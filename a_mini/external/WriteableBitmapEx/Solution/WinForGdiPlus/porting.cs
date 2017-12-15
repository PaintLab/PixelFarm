using System;
using System.Collections.Generic;

//for .NET 2.0 
namespace System
{
    public delegate R Func<R>();
    public delegate R Func<T, R>(T t1);
    public delegate R Func<T1, T2, R>(T1 t1, T2 t2);
    public delegate R Func<T1, T2, T3, R>(T1 t1, T2 t2, T3 t3);

}
namespace System.Windows.Media.Imaging
{
    public class GeneralTransform
    {
        public Rect TransformBounds(Rect r1)
        {
            return new Rect();
        }
        public Point Transform(Point p)
        {
            return new Point();
        }
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
        public Rect TransformBounds()
        {
            return new Rect();
        }
    }
    public class MatrixTransform : GeneralTransform
    {


    }



    public struct Rect
    {
        public Rect(double left, double top, double width, double height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }
        public Rect(Point location, Size size)
        {
            this.Left = location.Left;
            this.Top = location.Top;
            this.Width = size.Width;
            this.Height = size.Height;
        }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
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
        private bool IntersectsWithInclusive(Rect r)
        {
            return !((Left > r.Right) || (Right < r.Left) ||
                (Top > r.Bottom) || (Bottom < r.Top));
        }
        public static Rect Intersect(Rect a, Rect b)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            if (!a.IntersectsWithInclusive(b))
                return new Rect(0, 0, 0, 0);
            return Rect.FromLTRB(
                Math.Max(a.Left, b.Left),
                Math.Max(a.Top, b.Top),
                Math.Min(a.Right, b.Right),
                Math.Min(a.Bottom, b.Bottom));
        }

        public static Rect FromLTRB(double left, double top, double right, double bottom)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            return new Rect(left, top, right - left, bottom - top);
        }

        /// <summary>
        ///	Intersect Method
        /// </summary>
        ///
        /// <remarks>
        ///	Replaces the Rectangle with the intersection of itself
        ///	and another Rectangle.
        /// </remarks>

        public void Intersect(Rect rect)
        {
            this = Rect.Intersect(this, rect);
        }
    }
    public struct Point
    {
        public Point(int x, int y)
        {
            this.Left = x;
            this.Top = y;
        }
        public Point(double x, double y)
        {
            this.Left = x;
            this.Top = y;
        }
        public double Left { get; set; }
        public double Top { get; set; }
        public double X { get { return this.Left; } }
        public double Y { get { return this.Top; } }
    }
    public struct Size
    {
        public Size(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public Size(double w, double h)
        {
            this.Width = w;
            this.Height = h;
        }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public struct Color
    {
        public byte R, G, B, A;
        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            Color c = new Color();
            c.R = r;
            c.G = g;
            c.B = b;
            c.A = a;
            return c;
        }
        public static bool operator ==(Color c1, Color c2)
        {
            return (uint)((c1.A << 24) | (c1.R << 16) | (c1.G << 8) | (c1.B)) ==
                   (uint)((c2.A << 24) | (c2.R << 16) | (c2.G << 8) | (c2.B));
        }
        public static bool operator !=(Color c1, Color c2)
        {
            return (uint)((c1.A << 24) | (c1.R << 16) | (c1.G << 8) | (c1.B)) !=
                  (uint)((c2.A << 24) | (c2.R << 16) | (c2.G << 8) | (c2.B));
        }
    }
    public struct Colors
    {
        public static Color White = new Color();

    }
    public class WriteableBitmap
    {
        public WriteableBitmap(int w, int h)
        {
            this.PixelWidth = w;
            this.PixelHeight = h;
        }
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public int[] Pixels { get; set; }
    }


    //public static class BitmapContext
    //{
    //    public static void BlockCopy()
    //    {
    //    }
    //}
}
namespace System.Runtime.InteropServices
{
    public partial class TargetedPatchingOptOutAttribute : Attribute
    {
        public TargetedPatchingOptOutAttribute(string msg)
        {

        }
    }
}

namespace System.Runtime.CompilerServices
{
    public partial class ExtensionAttribute : Attribute { }
}

