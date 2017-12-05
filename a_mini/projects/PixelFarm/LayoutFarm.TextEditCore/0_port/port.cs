using System.Collections.Generic;

namespace LayoutFarm.Text
{
    public enum FontStyle : byte
    {
        Regular = 0,
        Bold = 1,
        Italic = 1 << 1,
        Underline = 1 << 2,
        Strikeout = 1 << 3
    }
    public struct FontKey
    {

        public readonly int FontNameIndex;
        public readonly float FontSize;
        public readonly FontStyle FontStyle;

        public FontKey(string fontname, float fontSize, FontStyle fs)
        {
            //font name/ not filename
            this.FontNameIndex = RegisterFontName(fontname.ToLower());
            this.FontSize = fontSize;
            this.FontStyle = fs;
        }

        static Dictionary<string, int> registerFontNames = new Dictionary<string, int>();
        static FontKey()
        {
            RegisterFontName(""); //blank font name
        }
        static int RegisterFontName(string fontName)
        {
            fontName = fontName.ToUpper();
            int found;
            if (!registerFontNames.TryGetValue(fontName, out found))
            {
                int nameIndex = registerFontNames.Count;
                registerFontNames.Add(fontName, nameIndex);
                return nameIndex;
            }
            return found;
        }
    }
    public interface TextSurfaceEventListener { }

    public abstract class RequestFont
    {
        public FontKey FontKey { get; set; }
        public string Name { get; set; }
        public float SizeInPoints { get; set; }
        public RequestFont(string name, float sizeInPnt, FontStyle style)
        {
            this.Name = name;
            this.FontKey = new FontKey(name, sizeInPnt, style);
            this.SizeInPoints = sizeInPnt;
        }
    }

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