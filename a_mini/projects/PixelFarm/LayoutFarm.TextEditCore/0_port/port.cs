
namespace LayoutFarm.Text
{
    public interface TextSurfaceEventListener { }

    public class RequestFont { }

    public static class FontService1
    {
        static TextLayerFontService s_fontService;
        public static void RegisterFontService(TextLayerFontService fontService)
        {
            s_fontService = fontService;
        }
        public static RequestFont DefaultFont
        {
            get
            {
                return s_fontService.DefaultFont;
            }
        } 
        public static void CalculateGlyphAdvancePos(char[] buffer, int start, int len, RequestFont font, int[] outputGlyphPos)
        {
            s_fontService.CalculateGlyphAdvancePos(buffer, start, len, font, outputGlyphPos);
        }
        public static Size MeasureString(char[] buffer, int start, int len, RequestFont r)
        {
            return s_fontService.MeasureString(buffer, start, len, r); 
        }
    }

    public interface TextLayerFontService
    {
        RequestFont DefaultFont { get; }
        void CalculateGlyphAdvancePos(char[] buffer, int start, int len, RequestFont font, int[] outputGlyphPos);
        Size MeasureString(char[] buffer, int start, int len, RequestFont r);
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