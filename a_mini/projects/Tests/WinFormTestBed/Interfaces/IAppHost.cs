//Apache2, 2017, WinterDev
namespace PaintLab
{
    public interface IAppHost
    {

    }
    public interface IViewport
    {
        IAppHost AppHost { get; }
        IUIRootElement Root { get; }
    }
    public interface IUIEvent
    {

    }
    public interface IUIElement
    {

    }
    public interface IUIRootElement
    {
        IUIElement CreateElement(string elemName);
        void AddContent(IUIElement uiElement);
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

