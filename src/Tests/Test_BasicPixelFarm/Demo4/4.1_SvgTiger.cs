//MIT, 2014-2018, WinterDev

using PixelFarm.Drawing;
using PaintLab.Svg;
using LayoutFarm.UI;
using PaintLab;
using PixelFarm.Agg;

namespace LayoutFarm
{
    [DemoNote("4.1 DemoSvgTiger")]
    class Demo_SvgTiger : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            PaintLab.Svg.SvgParser parser = new SvgParser();

            //load lion svg
            string file = @"d:\\WImageTest\\lion.svg";
            string svgContent = System.IO.File.ReadAllText(file);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);

            //
            SvgRenderVx svgRenderVx = parser.GetResultAsRenderVx();

            var uiSprite = new UISprite(10, 10);
            uiSprite.LoadSvg(svgRenderVx);
            viewport.AddContent(uiSprite);

            var evListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(evListener);

            int count = 0;

            IUIEventListener uiEvListener = (IUIEventListener)evListener;
            uiEvListener.MouseDown += (e) =>
            {
                System.Console.WriteLine("click :" + (count++));
            };
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

    class GeneralEventListener : IUIEventListener, UI.IEventListener
    {

        internal UI.IEventListener uiElement;// bind to owner

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
        event UIEventHandler<IEventArgs> IUIEventListener.MouseDown
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
        event UIEventHandler<IEventArgs> IUIEventListener.MouseMove
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
        event UIEventHandler<IEventArgs> IUIEventListener.MouseUp
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

        event UIEventHandler<IEventArgs> IUIEventListener.KeyDown
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
        event UIEventHandler<IEventArgs> IUIEventListener.KeyPress
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
        event UIEventHandler<IEventArgs> IUIEventListener.KeyUp
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

}