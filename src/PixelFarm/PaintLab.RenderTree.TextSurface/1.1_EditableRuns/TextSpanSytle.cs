//Apache2, 2014-present, WinterDev
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    public struct TextSpanStyle
    {
        public Color FontColor;
        public RequestFont FontInfo;
        public byte ContentHAlign;
        
        public bool IsEmpty()
        {
            return this.FontInfo == null;
        }


        public static readonly TextSpanStyle Empty = new TextSpanStyle();
    }
}