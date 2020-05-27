//Apache2, 2014-present, WinterDev
using System;
using LayoutFarm.UI;
using LayoutFarm.UI.ForImplementator;

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


        public TopWindowEventRoot(RootGraphic rootgfx, TopWindowRenderBox topRenderElement)
        {
            _mouseDownEventArgs = new UIMouseDownEventArgs();
            _mouseMoveEventArgs = new UIMouseMoveEventArgs();
            _mouseUpEventArgs = new UIMouseUpEventArgs();
            _wheelEventArgs = new UIMouseWheelEventArgs();


            _topWinBoxEventPortal = new RenderElementEventPortal(topRenderElement);
#if DEBUG
            _topWinBoxEventPortal.dbugRootGraphics = (MyRootGraphic)rootgfx;
#endif
            _iTopBoxEventPortal = _topWinBoxEventPortal;
            _rootgfx = rootgfx;

            _hoverMonitoringTask = new UIHoverMonitorTask();
            _hoverMonitoringTask.Interval = 100;//ms
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

            //-------------- 


            UIMouseEventArgs e = _mouseDownEventArgs;
            AddMouseEventArgsDetail(e, primaryMouseEventArgs);

            _topWinBoxEventPortal._prevMouseDownElement = _latestMouseDown;//set this before call portal mouse down
            _iTopBoxEventPortal.PortalMouseDown(_mouseDownEventArgs);
            //
            _currentMouseActiveElement = _latestMouseDown = e.CurrentContextElement; //
            _localMouseDownX = e.X;
            _localMouseDownY = e.Y;

            _draggingElement = null;

            if (e.CapturedElement != null)
            {
                if (e.CapturedElement != e.CurrentContextElement)
                {
                    //change captured element

                    e.CapturedElement.GetGlobalLocation(out int globalX, out int globalY);
                    //find new capture pos
                    _localMouseDownX = e.GlobalX - globalX;
                    _localMouseDownY = e.GlobalY - globalY;
                }
                _draggingElement = e.CapturedElement;
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

            RequestCursorStyle = e.MouseCursorStyle;
            RequestCursor = e.CustomMouseCursor;
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

            _mouseUpEventArgs.SetIsDragging(_isDragging);
            _isMouseDown = _isDragging = false;

            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - _lastTimeMouseUp;
            _lastTimeMouseUp = snapMouseUpTime;

            if (_mouseUpEventArgs.IsDragging)
            {
                if (_draggingElement != null)
                {
                    //when we have dragging element
                    //rediect msg (this mouse up) 
                    //to the dragging element

                    _draggingElement.GetGlobalLocation(out int d_GlobalX, out int d_globalY);
                    e.SetLocation(e.GlobalX - d_GlobalX, e.GlobalY - d_globalY);
                    e.CapturedMouseX = _localMouseDownX;
                    e.CapturedMouseY = _localMouseDownY;
                    if (_draggingElement is IEventPortal iportal)
                    {
                        iportal.PortalMouseUp(_mouseUpEventArgs);
                        if (!e.IsCanceled)
                        {
                            _draggingElement.ListenMouseUp(_mouseUpEventArgs);
                        }
                    }
                    else
                    {
                        _draggingElement.ListenMouseUp(_mouseUpEventArgs);
                    }
                }

                StartCaretBlink();
            }
            else
            {
                _mouseUpEventArgs.SetMouseDoubleClick(timediff.Milliseconds < _dblClickSense);
                _iTopBoxEventPortal.PortalMouseUp(_mouseUpEventArgs);

            }
            _localMouseDownX = _localMouseDownY = 0;

            RequestCursorStyle = e.MouseCursorStyle;
            RequestCursor = e.CustomMouseCursor;
        }

        void ITopWindowEventRoot.RootMouseLeave()
        {
            //mouse leave from viewport             

            _iTopBoxEventPortal.PortalMouseLeaveFromViewport();
            _hoverMonitoringTask.Reset();

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
            _mouseMoveEventArgs.SetIsDragging(_isMouseDown);

            if (_isDragging = _isMouseDown)
            {
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
                                _draggingElement.ListenMouseMove(_mouseMoveEventArgs);
                            }
                        }
                        else
                        {
                            if (_draggingElement.AcceptKeyboardFocus)
                            {
                                StopCaretBlink();
                            }
                            _draggingElement.ListenMouseMove(_mouseMoveEventArgs);
                        }
                    }
                }
            }
            else
            {
                _iTopBoxEventPortal.PortalMouseMove(_mouseMoveEventArgs);
                _hoverMonitoringTask.SetMonitorElement(e.CurrentContextElement);
            }

            RequestCursorStyle = e.MouseCursorStyle;
            RequestCursor = e.CustomMouseCursor;
        }
        void ITopWindowEventRoot.RootMouseWheel(PrimaryMouseEventArgs primaryMouseEventArgs)
        {

            AddMouseEventArgsDetail(_wheelEventArgs, primaryMouseEventArgs);
            //find element            

            _iTopBoxEventPortal.PortalMouseWheel(_wheelEventArgs);

            RequestCursorStyle = _wheelEventArgs.MouseCursorStyle;
            RequestCursor = _wheelEventArgs.CustomMouseCursor;
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
            e.SetExactHitObject(_currentKbFocusElem);
            e.SetCurrentContextElement(_currentKbFocusElem);

            _currentKbFocusElem.ListenKeyPress(e);
            _iTopBoxEventPortal.PortalKeyPress(e);

        }
        void ITopWindowEventRoot.RootKeyDown(UIKeyEventArgs e)
        {
            if (_currentKbFocusElem == null)
            {
                return;
            }
            SetKeyData(e, UIEventName.KeyDown);
            StopCaretBlink();
            e.SetExactHitObject(_currentKbFocusElem);
            e.SetCurrentContextElement(_currentKbFocusElem);
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

            SetKeyData(e, UIEventName.KeyUp);
            //----------------------------------------------------
            e.SetExactHitObject(_currentKbFocusElem);
            e.SetCurrentContextElement(_currentKbFocusElem);
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
                _lastKeydownWithControl = ((k & UIKeys.Control) == UIKeys.Control),
                 UIEventName.ProcessDialogKey);

            e.SetExactHitObject(_currentKbFocusElem);
            e.SetCurrentContextElement(_currentKbFocusElem);
            return _currentKbFocusElem.ListenProcessDialogKey(e);
        }
        void SetKeyData(UIKeyEventArgs keyEventArgs, UIEventName evName)
        {
            keyEventArgs.SetEventInfo(
                _lastKeydownWithShift,
                _lastKeydownWithAlt,
                _lastKeydownWithControl,
                evName);
        }

        void AddMouseEventArgsDetail(UIMouseEventArgs mouseEventArg, PrimaryMouseEventArgs primaryMouseEventArgs)
        {
            mouseEventArg.ResetAll();
            //TODO: review here             
            mouseEventArg.SetEventInfo(primaryMouseEventArgs.Left, primaryMouseEventArgs.Top, primaryMouseEventArgs.Button, primaryMouseEventArgs.Clicks, primaryMouseEventArgs.Delta);
            mouseEventArg.SetEventInfo(_lastKeydownWithControl, _lastKeydownWithAlt, _lastKeydownWithShift);
        }
    }
}