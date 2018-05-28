//Apache2, 2017-2018, WinterDev
using LayoutFarm.UI;
namespace PaintLab
{

    //this is an optional*** interface
    //that bridge our platform to another app world
    //with this interface,
    //the app dose not know about the backend engine.
    //suite for general client

    public interface IAppHost
    {

    }
    public interface IViewport
    {
        IAppHost AppHost { get; }
        IUIRootElement Root { get; }
    }
     

    public interface IUIBoxElement : IUIElement
    {
        int Width { get; }
        int Height { get; }
        int Top { get; }
        int Left { get; }
        void SetSize(int w, int h);
        void SetLocation(int left, int top);
    }



    public enum BasicUIElementKind
    {
        SimpleBox,
        TextBox,
        HScrollBar,
        VScrollBar,
    }

    public static class UIElemNameConst
    {
        public const string simple_box = "simple_box";
        public const string v_scroll_bar = "v_scroll_bar";
        public const string h_scroll_bar = "h_scroll_bar";
        public const string textbox = "textbox";
    }
    public static class UIRootElementExtensions
    {
        public static IUIElement CreateElement2(this IUIRootElement rootElem, BasicUIElementKind elemKind)
        {
            switch (elemKind)
            {
                default:
                case BasicUIElementKind.SimpleBox:
                    return rootElem.CreateElement(UIElemNameConst.simple_box);
                case BasicUIElementKind.VScrollBar:
                    return rootElem.CreateElement(UIElemNameConst.simple_box);
                case BasicUIElementKind.HScrollBar:
                    return rootElem.CreateElement(UIElemNameConst.h_scroll_bar);
                case BasicUIElementKind.TextBox:
                    return rootElem.CreateElement(UIElemNameConst.textbox);
            }
        }
    }
}

