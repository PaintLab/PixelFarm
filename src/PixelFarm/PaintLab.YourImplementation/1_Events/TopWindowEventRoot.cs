//Apache2, 2014-present, WinterDev
using System;
using LayoutFarm.UI;
using LayoutFarm.UI.InputBridge;

namespace LayoutFarm
{
    class TopWindowEventRoot : ITopWindowEventRoot
    {
        readonly UIMouseDownEventArgs _mouseDownEventArgs;
        readonly UIMouseMoveEventArgs _mouseMoveEventArgs;
        readonly UIMouseUpEventArgs _mouseUpEventArgs;
        readonly UIMouseWheelEventArgs _wheelEventArgs;

        readonly RootGraphic _rootgfx;
        readonly RenderElementEventPortal _topWinBoxEventPortal;
        readonly IEventPortal _iTopBoxEventPortal;

        IUIEventListener _currentKbFocusElem;
        IUIEventListener _currentMouseActiveElement;
        IUIEventListener _latestMouseDown;
        IUIEventListener _draggingElement;

        DateTime _lastTimeMouseUp;
        int _dblClickSense = 200;//ms         
        readonly UIHoverMonitorTask _hoverMonitoringTask;

        bool _isMouseDown;
        bool _isDragging;
        bool _lastKeydownWithControl;
        bool _lastKeydownWithAlt;
        bool _lastKeydownWithShift;
        int _prevLogicalMouseX;
        int _prevLogicalMouseY;
        int _localMouseDownX;
        int _localMouseDownY;
        UIMouseButtons _mouseDownButton = UIMouseButtons.None;


        public TopWindowEventRoot(RenderElement topRenderElement)
        {
            _mouseDownEventArgs = new UIMouseDownEventArgs();
            _mouseMoveEventArgs = new UIMouseMoveEventArgs();
            _mouseUpEventArgs = new UIMouseUpEventArgs();
            _wheelEventArgs = new UIMouseWheelEventArgs();

            _iTopBoxEventPortal = _topWinBoxEventPortal = new RenderElementEventPortal(topRenderElement);
            _rootgfx = topRenderElement.Root;
            _hoverMonitoringTask = new UIHoverMonitorTask();
            _hoverMonitoringTask.IntervalInMillisec = 100;//ms
            _hoverMonitoringTask.Enabled = true;
            UIPlatform.RegisterTimerTask(_hoverMonitoringTask);
        }
        public IUIEventListener CurrentKeyboardFocusedElement
        {
            get => _currentKbFocusElem;
            set
            {
                //1. lost keyboard focus
                if (_currentKbFocusElem != null && _currentKbFocusElem != value)
                {
                    _currentKbFocusElem.ListenLostKeyboardFocus(null);
                }
                //2. keyboard focus
                _currentKbFocusElem = value;
            }
        }
        void StartCaretBlink()
        {
            _rootgfx.CaretStartBlink();
        }
        void StopCaretBlink()
        {
            _rootgfx.CaretStopBlink();
        }


        public Cursor RequestCursor { get; private set; }
        public MouseCursorStyle RequestCursorStyle { get; private set; }

        void ITopWindowEventRoot.RootMouseDown(PrimaryMouseEventArgs primaryMouseEventArgs)
        {
            _prevLogicalMouseX = primaryMouseEventArgs.Left;
            _prevLogicalMouseY = primaryMouseEventArgs.Top;
            _isMouseDown = true;
            _isDragging = false;
            _mouseDownButton = primaryMouseEventArgs.Button;
            //-------------- 

            UIMouseEventArgs e = _mouseDownEventArgs;
            AddMouseEventArgsDetail(e, primaryMouseEventArgs);

            _iTopBoxEventPortal.PortalMouseDown(_mouseDownEventArgs);
            //
            _currentMouseActiveElement = _latestMouseDown = e.CurrentContextElement;
            _localMouseDownX = e.X;
            _localMouseDownY = e.Y;

            _draggingElement = null;

            if (e.DraggingElement != null)
            {
                if (e.DraggingElement != e.CurrentContextElement)
                {
                    //change captured element

                    e.DraggingElement.GetGlobalLocation(out int globalX, out int globalY);
                    //find new capture pos
                    _localMouseDownX = e.GlobalX - globalX;
                    _localMouseDownY = e.GlobalY - globalY;
                }
                _draggingElement = e.DraggingElement;
            }
            else
            {
                if (_currentMouseActiveElement != null &&
                    !_currentMouseActiveElement.BypassAllMouseEvents &&
                    !_currentMouseActiveElement.DisableAutoMouseCapture)
                {
                    _draggingElement = _currentMouseActiveElement;
                }
            }
        }
        void ITopWindowEventRoot.RootMouseUp(PrimaryMouseEventArgs primaryMouseEventArgs)
        {
            int xdiff = primaryMouseEventArgs.Left - _prevLogicalMouseX;
            int ydiff = primaryMouseEventArgs.Top - _prevLogicalMouseY;
            _prevLogicalMouseX = primaryMouseEventArgs.Left;
            _prevLogicalMouseY = primaryMouseEventArgs.Top;

            UIMouseEventArgs e = _mouseUpEventArgs;
            AddMouseEventArgsDetail(e, primaryMouseEventArgs);

            e.SetDiff(xdiff, ydiff);
            //----------------------------------
            e.IsDragging = _isDragging;
            _isMouseDown = _isDragging = false;

            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - _lastTimeMouseUp;
            _lastTimeMouseUp = snapMouseUpTime;

            if (e.IsDragging)
            {
                if (_draggingElement != null)
                {
                    //send this to dragging element first 
                    _draggingElement.GetGlobalLocation(out int d_GlobalX, out int d_globalY);
                    e.SetLocation(e.GlobalX - d_GlobalX, e.GlobalY - d_globalY);
                    e.CapturedMouseX = _localMouseDownX;
                    e.CapturedMouseY = _localMouseDownY;
                    if (_draggingElement is IEventPortal iportal)
                    {
                        iportal.PortalMouseUp(_mouseUpEventArgs);
                        if (!e.IsCanceled)
                        {
                            _draggingElement.ListenMouseUp(e);
                        }
                    }
                    else
                    {
                        _draggingElement.ListenMouseUp(e);
                    }
                }
            }
            else
            {
                e.IsAlsoDoubleClick = timediff.Milliseconds < _dblClickSense;
                _iTopBoxEventPortal.PortalMouseUp(_mouseUpEventArgs);

            }
            _localMouseDownX = _localMouseDownY = 0;
        }


        void ITopWindowEventRoot.RootMouseMove(PrimaryMouseEventArgs primaryMouseEventArgs)
        {
            int xdiff = primaryMouseEventArgs.Left - _prevLogicalMouseX;
            int ydiff = primaryMouseEventArgs.Top - _prevLogicalMouseY;
            if (xdiff == 0 && ydiff == 0)
            {
                return;
            }

            _prevLogicalMouseX = primaryMouseEventArgs.Left;
            _prevLogicalMouseY = primaryMouseEventArgs.Top;
            //-------------------------------------------------------
            UIMouseEventArgs e = _mouseMoveEventArgs;
            AddMouseEventArgsDetail(e, primaryMouseEventArgs);
            e.SetDiff(xdiff, ydiff);
            //-------------------------------------------------------

            if (e.IsDragging = _isDragging = _isMouseDown)
            {
                e.Buttons = _mouseDownButton;
                if (_draggingElement != null)
                {
                    if (_draggingElement.DisableAutoMouseCapture)
                    {
                        //TODO: review this
                        //find element under mouse position again
                        _iTopBoxEventPortal.PortalMouseMove(_mouseMoveEventArgs);
                    }
                    else
                    {
                        //send this to dragging element first 
                        _draggingElement.GetGlobalLocation(out int d_GlobalX, out int d_globalY);
                        _draggingElement.GetViewport(out int vwp_left, out int vwp_top);
                        e.SetLocation(e.GlobalX - d_GlobalX + vwp_left, e.GlobalY - d_globalY + vwp_top);
                        e.CapturedMouseX = _localMouseDownX;
                        e.CapturedMouseY = _localMouseDownY;
                        if (_draggingElement is IEventPortal iportal)
                        {
                            iportal.PortalMouseMove(_mouseMoveEventArgs);
                            if (!e.IsCanceled)
                            {
                                _draggingElement.ListenMouseMove(e);
                            }
                        }
                        else
                        {
                            _draggingElement.ListenMouseMove(e);
                        }

                    }
                }
            }
            else
            {
                _iTopBoxEventPortal.PortalMouseMove(_mouseMoveEventArgs);
                _hoverMonitoringTask.SetMonitorElement(e.CurrentContextElement);
            }
        }
        void ITopWindowEventRoot.RootMouseWheel(PrimaryMouseEventArgs primaryMouseEventArgs)
        {
            UIMouseEventArgs e = _wheelEventArgs;
            AddMouseEventArgsDetail(e, primaryMouseEventArgs);
            //find element            

            e.Shift = _lastKeydownWithShift;
            e.Alt = _lastKeydownWithAlt;
            e.Ctrl = _lastKeydownWithControl;
            _iTopBoxEventPortal.PortalMouseWheel(_wheelEventArgs);
        }
        void ITopWindowEventRoot.RootGotFocus(UIFocusEventArgs e)
        {
            _iTopBoxEventPortal.PortalGotFocus(e);
        }
        void ITopWindowEventRoot.RootLostFocus(UIFocusEventArgs e)
        {

            _iTopBoxEventPortal.PortalLostFocus(e);

        }
        void ITopWindowEventRoot.RootKeyPress(UIKeyEventArgs e)
        {
            if (_currentKbFocusElem == null)
            {
                return;
            }

            StopCaretBlink();
            e.ExactHitObject = e.SourceHitElement = _currentKbFocusElem;
            _currentKbFocusElem.ListenKeyPress(e);
            _iTopBoxEventPortal.PortalKeyPress(e);

        }
        void ITopWindowEventRoot.RootKeyDown(UIKeyEventArgs e)
        {
            if (_currentKbFocusElem == null)
            {
                return;
            }
            SetKeyData(e);
            StopCaretBlink();
            e.ExactHitObject = e.SourceHitElement = _currentKbFocusElem;
            _currentKbFocusElem.ListenKeyDown(e);
            _iTopBoxEventPortal.PortalKeyDown(e);
        }

        void ITopWindowEventRoot.RootKeyUp(UIKeyEventArgs e)
        {
            if (_currentKbFocusElem == null)
            {
                _lastKeydownWithShift = _lastKeydownWithAlt = _lastKeydownWithControl = false;

                return;
            }

            StopCaretBlink();

            SetKeyData(e);
            //----------------------------------------------------
            e.ExactHitObject = e.SourceHitElement = _currentKbFocusElem;
            _currentKbFocusElem.ListenKeyUp(e);
            _iTopBoxEventPortal.PortalKeyUp(e);
            //----------------------------------------------------

            StartCaretBlink();

            _lastKeydownWithShift = _lastKeydownWithControl = _lastKeydownWithAlt = false;
        }
        bool ITopWindowEventRoot.RootProcessDialogKey(UIKeyEventArgs e)
        {
            UI.UIKeys k = (UIKeys)e.KeyData;//*** RootProcessDialogKey provide only keydata
            if (_currentKbFocusElem == null)
            {
                //set 
                _lastKeydownWithShift = ((k & UIKeys.Shift) == UIKeys.Shift);
                _lastKeydownWithAlt = ((k & UIKeys.Alt) == UIKeys.Alt);
                _lastKeydownWithControl = ((k & UIKeys.Control) == UIKeys.Control);
                return false;
            }

            StopCaretBlink();

            e.SetEventInfo(
                _lastKeydownWithShift = ((k & UIKeys.Shift) == UIKeys.Shift),
                _lastKeydownWithAlt = ((k & UIKeys.Alt) == UIKeys.Alt),
                _lastKeydownWithControl = ((k & UIKeys.Control) == UIKeys.Control));

            e.ExactHitObject = e.SourceHitElement = _currentKbFocusElem;
            return _currentKbFocusElem.ListenProcessDialogKey(e);
        }
        void SetKeyData(UIKeyEventArgs keyEventArgs)
        {
            keyEventArgs.SetEventInfo(
                _lastKeydownWithShift,
                _lastKeydownWithAlt,
                _lastKeydownWithControl);
        }

        void AddMouseEventArgsDetail(UIMouseEventArgs mouseEventArg, PrimaryMouseEventArgs primaryMouseEventArgs)
        {
            mouseEventArg.Clear();
            //TODO: review here
            mouseEventArg.SetEventInfo(primaryMouseEventArgs.Left, primaryMouseEventArgs.Top, primaryMouseEventArgs.Button, primaryMouseEventArgs.Clicks, primaryMouseEventArgs.Delta);
            mouseEventArg.Alt = _lastKeydownWithAlt;
            mouseEventArg.Shift = _lastKeydownWithShift;
            mouseEventArg.Ctrl = _lastKeydownWithControl;
            mouseEventArg.PreviousMouseDown = _latestMouseDown;
        }
    }
}