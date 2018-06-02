//Apache2, 2014-2018, WinterDev

using PaintLab;
using PixelFarm.Drawing;
using LayoutFarm.ContentManagers;
using LayoutFarm.UI;

namespace LayoutFarm
{
    ////---------------
    //struct MyUIElement : IUIElement, IUIBoxElement, System.IEquatable<MyUIElement>
    //{
    //    internal readonly UI.UIElement uiElem;
    //    public MyUIElement(UI.UIElement uiElem)
    //    {
    //        this.uiElem = uiElem;
    //    }
    //    public int Width
    //    {
    //        get
    //        {
    //            UI.UIBox box = uiElem as UI.UIBox;
    //            return (box != null) ? box.Width : 0;
    //        }
    //    }

    //    public int Height
    //    {
    //        get
    //        {
    //            UI.UIBox box = uiElem as UI.UIBox;
    //            return (box != null) ? box.Height : 0;
    //        }
    //    }

    //    public int Top
    //    {
    //        get
    //        {
    //            UI.UIBox box = uiElem as UI.UIBox;
    //            return (box != null) ? box.Top : 0;
    //        }
    //    }

    //    public int Left
    //    {
    //        get
    //        {
    //            UI.UIBox box = uiElem as UI.UIBox;
    //            return (box != null) ? box.Left : 0;
    //        }
    //    }
    //    public bool Equals(MyUIElement other)
    //    {
    //        return this.uiElem == other.uiElem;
    //    }

    //    public void SetLocation(int left, int top)
    //    {
    //        UI.UIBox box = uiElem as UI.UIBox;
    //        if (box == null) return;

    //        box.SetLocation(left, top);
    //    }

    //    public void SetSize(int w, int h)
    //    {
    //        UI.UIBox box = uiElem as UI.UIBox;
    //        if (box == null) return;
    //        box.SetSize(w, h);
    //    }
    //    public bool AttachEventListener(IEventListener uiEventListener)
    //    {

    //        GeneralEventListener genEventListener = uiEventListener as GeneralEventListener;
    //        if (genEventListener != null)
    //        {
    //            uiElem.AttachExternalEventListener(genEventListener);
    //        }
    //        return false;
    //    }
    //}


    //class UIRootElement : IUIRootElement
    //{
    //    internal SampleViewport _viewport;
    //    public void AddContent(IUIElement uiElement)
    //    {
    //        var myUI = (MyUIElement)uiElement;
    //        _viewport.AddContent(myUI.uiElem);
    //    }
    //    public IEventListener CreateEventListener()
    //    {
    //        //create new event listener
    //        var eventListener = new GeneralEventListener();
    //        return eventListener;
    //    }
    //    public IUIElement CreateElement(string elemName)
    //    {
    //        switch (elemName)
    //        {
    //            default:
    //                return Wrap(new CustomWidgets.SimpleBox(10, 10));
    //            case UIElemNameConst.simple_box:
    //                return Wrap(new CustomWidgets.SimpleBox(10, 10));
    //            case UIElemNameConst.h_scroll_bar:
    //                {
    //                    var scBar = new CustomWidgets.ScrollBar(10, 10);
    //                    scBar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
    //                    return Wrap(scBar);
    //                }
    //            case UIElemNameConst.v_scroll_bar:
    //                {
    //                    var scBar = new CustomWidgets.ScrollBar(10, 10);
    //                    scBar.ScrollBarType = CustomWidgets.ScrollBarType.Vertical;
    //                    return Wrap(scBar);
    //                }
    //            case UIElemNameConst.textbox:
    //                {
    //                    var textBox = new CustomWidgets.TextBox(10, 10, false);
    //                    return Wrap(textBox);
    //                }
    //        }
    //    }
    //    static MyUIElement Wrap(LayoutFarm.UI.UIElement ui)
    //    {
    //        return new MyUIElement(ui);
    //    }
    //}
    class MyAppHost : IAppHost
    {
        internal IViewport clientViewport;
    }


    public class GeneralEventListener : IEventListener
    {
        public event UIEventHandler<UIMouseEventArgs> MouseDown;
        public event UIEventHandler<UIMouseEventArgs> MouseUp;
        public event UIEventHandler<UIMouseEventArgs> MouseMove;
        public event UIEventHandler<UIKeyEventArgs> KeyDown;
        public event UIEventHandler<UIKeyEventArgs> KeyPress;
        public event UIEventHandler<UIKeyEventArgs> KeyUp;

        void IEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e)
        {

        }

        void IEventListener.ListenGuestTalk(UIGuestTalkEventArgs e)
        {

        }

        void IEventListener.ListenInterComponentMsg(object sender, int msgcode, string msg)
        {

        }

        void IEventListener.ListenKeyDown(UIKeyEventArgs e)
        {
            KeyDown?.Invoke(e);
        }

        void IEventListener.ListenKeyPress(UIKeyEventArgs e)
        {
            KeyPress?.Invoke(e);
        }

        void IEventListener.ListenKeyUp(UIKeyEventArgs e)
        {
            KeyUp?.Invoke(e);
        }

        void IEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e)
        {

        }

        void IEventListener.ListenLostMouseFocus(UIMouseEventArgs e)
        {

        }

        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {

        }

        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {

        }

        void IEventListener.ListenMouseDown(UIMouseEventArgs e)
        {
            MouseDown?.Invoke(e);
        }

        void IEventListener.ListenMouseLeave(UIMouseEventArgs e)
        {

        }

        void IEventListener.ListenMouseMove(UIMouseEventArgs e)
        {
            MouseMove?.Invoke(e);
        }

        void IEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            MouseUp?.Invoke(e);
        }
        void IEventListener.ListenMouseWheel(UIMouseEventArgs e)
        {

        }
        bool IEventListener.ListenProcessDialogKey(UIKeyEventArgs args)
        {
            return false;
        }
    }

}