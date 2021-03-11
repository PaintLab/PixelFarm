//Apache2, 2014-present, WinterDev
using PixelFarm.Drawing;
namespace LayoutFarm.TextFlow
{
    
    public struct TextSpanStyle
    {
        public Color FontColor;
        public RequestFont ReqFont;
        public byte ContentHAlign;

        public bool IsEmpty()
        {
            return this.ReqFont == null;
        }
        public static readonly TextSpanStyle Empty = new TextSpanStyle();
    }

}