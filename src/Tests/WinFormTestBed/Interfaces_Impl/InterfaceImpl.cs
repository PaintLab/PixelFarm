//Apache2, 2014-2018, WinterDev

using PaintLab;
using PixelFarm.Drawing;
using LayoutFarm.ContentManagers;
using LayoutFarm.UI;

namespace LayoutFarm
{
    //---------------
    struct MyUIElement : IUIElement, IUIBoxElement, System.IEquatable<MyUIElement>
    {
        internal readonly UI.UIElement uiElem;
        public MyUIElement(UI.UIElement uiElem)
        {
            this.uiElem = uiElem;
        }
        public int Width
        {
            get
            {
                UI.UIBox box = uiElem as UI.UIBox;
                return (box != null) ? box.Width : 0;
            }
        }

        public int Height
        {
            get
            {
                UI.UIBox box = uiElem as UI.UIBox;
                return (box != null) ? box.Height : 0;
            }
        }

        public int Top
        {
            get
            {
                UI.UIBox box = uiElem as UI.UIBox;
                return (box != null) ? box.Top : 0;
            }
        }

        public int Left
        {
            get
            {
                UI.UIBox box = uiElem as UI.UIBox;
                return (box != null) ? box.Left : 0;
            }
        }
        public bool Equals(MyUIElement other)
        {
            return this.uiElem == other.uiElem;
        }

        public void SetLocation(int left, int top)
        {
            UI.UIBox box = uiElem as UI.UIBox;
            if (box == null) return;

            box.SetLocation(left, top);
        }

        public void SetSize(int w, int h)
        {
            UI.UIBox box = uiElem as UI.UIBox;
            if (box == null) return;
            box.SetSize(w, h);
        }
        public bool AttachEventListener(IEventListener uiEventListener)
        {
            GeneralEventListener genEventListener = uiEventListener as GeneralEventListener;
            if (genEventListener != null)
            {
                genEventListener.uiElement = uiElem;
                uiElem.AttachExternalEventListener(genEventListener);
            }
            return false;
        }
    }

    struct MouseEventArgs : IEventArgs
    {
        internal UIMouseEventArgs _uiMouseEventArgs;
        public MouseEventArgs(UIMouseEventArgs e) { this._uiMouseEventArgs = e; }

        public IEventName EventName
        {
            get
            {
                switch (_uiMouseEventArgs.UIEventName)
                {
                    case UIEventName.MouseDown:
                        return IEventName.MouseDown;
                    case UIEventName.MouseMove:
                        return IEventName.MouseMove;
                    case UIEventName.MouseUp:
                        return IEventName.MouseUp;
                    case UIEventName.KeyDown:
                        return IEventName.KeyDown;
                    case UIEventName.KeyPress:
                        return IEventName.KeyPress;
                    case UIEventName.KeyUp:
                        return IEventName.KeyUp;
                    default:
                        return IEventName.Custom;
                }
            }
        }

        public int X
        {
            get
            {
                return _uiMouseEventArgs.X;
            }
        }
        public int Y
        {
            get
            {
                return _uiMouseEventArgs.Y;
            }
        }
    }

    struct KeyEventArgs : IEventArgs
    {
        internal UIKeyEventArgs _uiKeyEventArgs;
        public KeyEventArgs(UIKeyEventArgs keyEventArgs)
        {
            this._uiKeyEventArgs = keyEventArgs;
        }
        public IEventName EventName
        {
            get
            {
                switch (_uiKeyEventArgs.UIEventName)
                {
                    case UIEventName.KeyDown:
                        return IEventName.KeyDown;
                    case UIEventName.KeyPress:
                        return IEventName.KeyPress;
                    case UIEventName.KeyUp:
                        return IEventName.KeyUp;
                    default:
                        return IEventName.Custom;//?
                }
            }
        }

        public int X
        {
            get { return _uiKeyEventArgs.X; }
        }

        public int Y
        {
            get { return _uiKeyEventArgs.Y; }
        }
    }

    class GeneralEventListener : IEventListener, UI.IUIEventListener
    {

        internal UI.IUIEventListener uiElement;// bind to owner

        public bool BypassAllMouseEvents
        {
            get { return uiElement.BypassAllMouseEvents; }
        }
        public bool AutoStopMouseEventPropagation
        {
            get { return uiElement.AutoStopMouseEventPropagation; ; }
        }

        public void GetGlobalLocation(out int x, out int y)
        {
            uiElement.GetGlobalLocation(out x, out y);
        }
        public void HandleContentLayout()
        {

        }

        public void HandleContentUpdate()
        {

        }

        public void HandleElementUpdate()
        {

        }

        public void ListenGotKeyboardFocus(UIFocusEventArgs e)
        {

        }

        public void ListenGuestTalk(UIGuestTalkEventArgs e)
        {
        }

        public void ListenInterComponentMsg(object sender, int msgcode, string msg)
        {

        }

        public void ListenKeyDown(UIKeyEventArgs e)
        {
            KeyEventArgs keyEventArgs = new KeyEventArgs(e);
            _keydown?.Invoke(keyEventArgs);

        }

        public void ListenKeyPress(UIKeyEventArgs e)
        {
            KeyEventArgs keyEventArgs = new KeyEventArgs(e);
            _keypress?.Invoke(keyEventArgs);
        }

        public void ListenKeyUp(UIKeyEventArgs e)
        {
            KeyEventArgs keyEventArgs = new KeyEventArgs(e);
            _keyup?.Invoke(keyEventArgs);
        }

        public void ListenLostKeyboardFocus(UIFocusEventArgs e)
        {

        }

        public void ListenLostMouseFocus(UIMouseEventArgs e)
        {

        }

        public void ListenMouseClick(UIMouseEventArgs e)
        {

        }

        public void ListenMouseDoubleClick(UIMouseEventArgs e)
        {

        }

        public void ListenMouseDown(UIMouseEventArgs e)
        {
            MouseEventArgs mouseEventArgs = new MouseEventArgs(e);
            _mouseDown?.Invoke(mouseEventArgs);
        }

        public void ListenMouseLeave(UIMouseEventArgs e)
        {
        }

        public void ListenMouseMove(UIMouseEventArgs e)
        {
            MouseEventArgs mouseEventArgs = new MouseEventArgs(e);
            _mouseMove?.Invoke(mouseEventArgs);
        }

        public void ListenMouseUp(UIMouseEventArgs e)
        {
            MouseEventArgs mouseEventArgs = new MouseEventArgs(e);
            _mouseUp?.Invoke(mouseEventArgs);
        }
        public void ListenMouseWheel(UIMouseEventArgs e)
        {

        }
        public bool ListenProcessDialogKey(UIKeyEventArgs args)
        {
            return false;
        }

        //
        UIEventHandler<IEventArgs> _mouseDown;
        UIEventHandler<IEventArgs> _mouseMove;
        UIEventHandler<IEventArgs> _mouseUp;
        UIEventHandler<IEventArgs> _keydown;
        UIEventHandler<IEventArgs> _keypress;
        UIEventHandler<IEventArgs> _keyup;
        event UIEventHandler<IEventArgs> IEventListener.MouseDown
        {
            add
            {
                //replace?
                this._mouseDown = value;
            }
            remove
            {
                //remove ?
                this._mouseDown = null;
            }
        }
        event UIEventHandler<IEventArgs> IEventListener.MouseMove
        {
            add
            {
                //replace?
                this._mouseMove = value;
            }
            remove
            {
                //remove ?
                this._mouseMove = null;
            }
        }
        event UIEventHandler<IEventArgs> IEventListener.MouseUp
        {
            add
            {
                //replace?
                this._mouseUp = value;
            }
            remove
            {
                //remove ?
                this._mouseUp = null;
            }
        }

        event UIEventHandler<IEventArgs> IEventListener.KeyDown
        {
            add
            {
                //replace?
                this._keydown = value;
            }
            remove
            {
                //remove ?
                this._keydown = null;
            }
        }
        event UIEventHandler<IEventArgs> IEventListener.KeyPress
        {
            add
            {
                //replace?
                this._keypress = value;
            }
            remove
            {
                //remove ?
                this._keypress = null;
            }
        }
        event UIEventHandler<IEventArgs> IEventListener.KeyUp
        {
            add
            {
                //replace?
                this._keyup = value;
            }
            remove
            {
                //remove ?
                this._keyup = null;
            }
        }
    }

    class UIRootElement : IUIRootElement
    {
        internal SampleViewport _viewport;
        public void AddContent(IUIElement uiElement)
        {
            var myUI = (MyUIElement)uiElement;
            _viewport.AddContent(myUI.uiElem);
        }
        public IEventListener CreateEventListener()
        {
            //create new event listener
            var eventListener = new GeneralEventListener();
            return eventListener;
        }
        public IUIElement CreateElement(string elemName)
        {
            switch (elemName)
            {
                default:
                    return Wrap(new CustomWidgets.SimpleBox(10, 10));
                case UIElemNameConst.simple_box:
                    return Wrap(new CustomWidgets.SimpleBox(10, 10));
                case UIElemNameConst.h_scroll_bar:
                    {
                        var scBar = new CustomWidgets.ScrollBar(10, 10);
                        scBar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                        return Wrap(scBar);
                    }
                case UIElemNameConst.v_scroll_bar:
                    {
                        var scBar = new CustomWidgets.ScrollBar(10, 10);
                        scBar.ScrollBarType = CustomWidgets.ScrollBarType.Vertical;
                        return Wrap(scBar);
                    }
                case UIElemNameConst.textbox:
                    {
                        var textBox = new CustomWidgets.TextBox(10, 10, false);
                        return Wrap(textBox);
                    }
            }
        }
        static MyUIElement Wrap(LayoutFarm.UI.UIElement ui)
        {
            return new MyUIElement(ui);
        }
    }
    class MyAppHost : IAppHost
    {
        internal IViewport clientViewport;
    }
}