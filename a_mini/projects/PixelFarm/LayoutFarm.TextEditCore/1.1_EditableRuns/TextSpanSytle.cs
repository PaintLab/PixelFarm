//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.Text
{
    public struct TextSpanStyle
    {
        public RequestFont FontInfo;
        public byte ContentHAlign;
        public bool IsEmpty()
        {
            return this.FontInfo == null;
        }
        public static readonly TextSpanStyle Empty = new TextSpanStyle();
    }
    public class RequestFont { }

}