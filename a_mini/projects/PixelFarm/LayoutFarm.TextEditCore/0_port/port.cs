 
namespace LayoutFarm.Text
{
    public interface TextSurfaceEventListener { }

    public class RequestFont { }

    public static class FontService1
    {
        public static RequestFont DefaultFont
        {
            get;
            set;
        }
        public static void CalculateGlyphAdvancePos(char[] buffer, int start, int len, RequestFont font, int[] outputGlyphPos)
        {

            //glyphPositions = new int[len];

            //Root.IFonts.CalculateGlyphAdvancePos(mybuffer, 0, len, GetFont(), glyphPositions);
        }
        public static Size MeasureString(char[] buffer, int start, int len, RequestFont r)
        {
            throw new System.NotSupportedException();
            //return this.Root.IFonts.MeasureString(buffer, 0,
            //     length, fontInfo);
            return new Size();
        }

    }

    public struct Point
    {
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }
    public struct Size
    {
        public Size(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public int Width { get; set; }
        public int Height { get; set; }

        public static readonly Size Empty = new Size();
    }
    public struct Rectangle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle(int x, int y, int w, int h)
        {
            this.X = x;
            this.Y = y;
            this.Height = h;
            this.Width = w;
        }
    }
}