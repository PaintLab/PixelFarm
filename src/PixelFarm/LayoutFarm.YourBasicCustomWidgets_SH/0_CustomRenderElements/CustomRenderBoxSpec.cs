//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    /// <summary>
    /// box spec
    /// </summary>
    public class CustomRenderBoxSpec
    {
        //similar to CssBoxSpec
        public RequestFont ReqFont;
        public Len Width; //before resolve
        public Len Height; //before resolve 
        public SideSpec TopSide;
        public SideSpec LeftSide;
        public SideSpec RightSide;
        public SideSpec Bottom;

        public Color? BackColor;
        public Color? FontColor;
        public Color? BorderColor;        
    }

    public readonly struct SideSpec
    {
        public readonly Len Margin;
        public readonly Len Border;
        public readonly Len Padding;
    }
}