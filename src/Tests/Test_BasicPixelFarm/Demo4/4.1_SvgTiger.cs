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
        LayoutFarm.CustomWidgets.RectBoxController rectBoxController = new CustomWidgets.RectBoxController();
        LayoutFarm.CustomWidgets.SimpleBox box1;
        BackDrawBoardUI _backBoard; 

        protected override void OnStartDemo(SampleViewport viewport)
        {


            PaintLab.Svg.SvgParser parser = new SvgParser();
            _backBoard = new BackDrawBoardUI(400, 400);
            _backBoard.BackColor = Color.White;
            viewport.AddContent(_backBoard);



            box1 = new LayoutFarm.CustomWidgets.SimpleBox(50, 50);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            //box1.dbugTag = 1;
            SetupActiveBoxProperties(box1);
            _backBoard.AddChild(box1);

            //----------------------

            //load lion svg
            string file = @"d:\\WImageTest\\lion.svg";
            string svgContent = System.IO.File.ReadAllText(file);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);
            //
            SvgRenderVx svgRenderVx = parser.GetResultAsRenderVx();
            var uiSprite = new UISprite(10, 10);
            uiSprite.LoadSvg(svgRenderVx);
            _backBoard.AddChild(uiSprite); 
            //-------- 
            rectBoxController.Init();
            //------------
            foreach (var ui in rectBoxController.GetControllerIter())
            {
                viewport.AddContent(ui);
            }

            //--------
            var evListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(evListener);

            IEventListener uiEvListener = (IEventListener)evListener;
            uiEvListener.MouseDown += (e) =>
            {

                //e.MouseCursorStyle = MouseCursorStyle.Pointer;
                ////--------------------------------------------
                //e.SetMouseCapture(rectBoxController.ControllerBoxMain);
                rectBoxController.UpdateControllerBoxes(box1);
                rectBoxController.Focus();
                //System.Console.WriteLine("click :" + (count++));
            };
            rectBoxController.ControllerBoxMain.KeyDown += (s1, e1) =>
            {
                if (e1.Ctrl && e1.KeyCode == UIKeys.X)
                {
                    //test copy back image buffer from current rect area

#if DEBUG
                    //test save some area
                    int w = rectBoxController.ControllerBoxMain.Width;
                    int h = rectBoxController.ControllerBoxMain.Height;

                    using (DrawBoard gdiDrawBoard = DrawBoardCreator.CreateNewDrawBoard(1, w, h))
                    {
                        gdiDrawBoard.OffsetCanvasOrigin(rectBoxController.ControllerBoxMain.Left, rectBoxController.ControllerBoxMain.Top);
                        _backBoard.CurrentPrimaryRenderElement.CustomDrawToThisCanvas(gdiDrawBoard, new Rectangle(0, 0, w, h));
                        var img2 = new ActualImage(w, h);
                        //copy content from drawboard to target image and save
                        gdiDrawBoard.RenderTo(img2, 0, 0, w, h);

                        img2.dbugSaveToPngFile("d:\\WImageTest\\ddd001.png");
                    }
#endif                    

                }
            };
        }
        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.EaseBox box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //--------------------------------------------
                e.SetMouseCapture(rectBoxController.ControllerBoxMain);
                rectBoxController.UpdateControllerBoxes(box);

            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;
                //controllerBox1.Visible = false;
                //controllerBox1.TargetBox = null;
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



    class GeneralEventListener : IEventListener
    {

        //internal UI.IUIEventListener uiElement;// bind to owner

        //public bool BypassAllMouseEvents
        //{
        //    get { return uiElement.BypassAllMouseEvents; }
        //}
        //public bool AutoStopMouseEventPropagation
        //{
        //    get { return uiElement.AutoStopMouseEventPropagation; ; }
        //}

        //public void GetGlobalLocation(out int x, out int y)
        //{
        //    uiElement.GetGlobalLocation(out x, out y);
        //}
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

}