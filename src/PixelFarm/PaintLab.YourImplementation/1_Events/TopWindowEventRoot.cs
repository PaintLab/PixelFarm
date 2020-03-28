//Apache2, 2014-present, WinterDev
using System;
using LayoutFarm.UI;
using LayoutFarm.UI.InputBridge;

namespace LayoutFarm
{
    class TopWindowEventRoot : ITopWindowEventRoot
    {

        RootGraphic _rootgfx;
        RenderElementEventPortal _topWinBoxEventPortal;
        IEventPortal _iTopBoxEventPortal;
        IUIEventListener _currentKbFocusElem;
        IUIEventListener _currentMouseActiveElement;
        IUIEventListener _latestMouseDown;
        IUIEventListener _draggingElement;



        DateTime _lastTimeMouseUp;
        int _dblClickSense = 200;//ms         
        UIHoverMonitorTask _hoverMonitoringTask;


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


        void ITopWindowEventRoot.RootMouseDown(UIMouseEventArgs e)
        {
            _prevLogicalMouseX = e.X;
            _prevLogicalMouseY = e.Y;
            _isMouseDown = true;
            _isDragging = false;
            _mouseDownButton = e.Button;

            AddMouseEventArgsDetail(e);

            //
            e.Shift = _lastKeydownWithShift;
            e.Alt = _lastKeydownWithAlt;
            e.Ctrl = _lastKeydownWithControl;
            //
            e.PreviousMouseDown = _latestMouseDown;
            //
            _iTopBoxEventPortal.PortalMouseDown(e);
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
        void ITopWindowEventRoot.RootMouseUp(UIMouseEventArgs e)
        {
            int xdiff = e.X - _prevLogicalMouseX;
            int ydiff = e.Y - _prevLogicalMouseY;
            _prevLogicalMouseX = e.X;
            _prevLogicalMouseY = e.Y;

            AddMouseEventArgsDetail(e);

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
                        iportal.PortalMouseUp(e);
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
                _iTopBoxEventPortal.PortalMouseUp(e);

            }
            _localMouseDownX = _localMouseDownY = 0;
        }


        void ITopWindowEventRoot.RootMouseMove(UIMouseEventArgs e)
        {
            int xdiff = e.X - _prevLogicalMouseX;
            int ydiff = e.Y - _prevLogicalMouseY;
            if (xdiff == 0 && ydiff == 0)
            {
                return;
            }

            _prevLogicalMouseX = e.X;
            _prevLogicalMouseY = e.Y;
            //-------------------------------------------------------

            AddMouseEventArgsDetail(e);
            e.SetDiff(xdiff, ydiff);
            //-------------------------------------------------------



            if (e.IsDragging = _isDragging = _isMouseDown)
            {
                e.Button = _mouseDownButton;
                if (_draggingElement != null)
                {
                    if (_draggingElement.DisableAutoMouseCapture)
                    {
                        //TODO: review this
                        //find element under mouse position again
                        _iTopBoxEventPortal.PortalMouseMove(e);
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
                            iportal.PortalMouseMove(e);
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
                _iTopBoxEventPortal.PortalMouseMove(e);
                _hoverMonitoringTask.SetMonitorElement(e.CurrentContextElement);
            }
        }
        void ITopWindowEventRoot.RootMouseWheel(UIMouseEventArgs e)
        {
            //find element            
            AddMouseEventArgsDetail(e);
            e.Shift = _lastKeydownWithShift;
            e.Alt = _lastKeydownWithAlt;
            e.Ctrl = _lastKeydownWithControl;
            _iTopBoxEventPortal.PortalMouseWheel(e);
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

        void AddMouseEventArgsDetail(UIMouseEventArgs mouseEventArg)
        {
            mouseEventArg.Alt = _lastKeydownWithAlt;
            mouseEventArg.Shift = _lastKeydownWithShift;
            mouseEventArg.Ctrl = _lastKeydownWithControl;
        }


    }
}