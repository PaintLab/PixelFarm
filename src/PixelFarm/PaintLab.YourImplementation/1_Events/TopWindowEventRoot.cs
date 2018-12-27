//Apache2, 2014-present, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.UI;
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
        MouseCursorStyle _mouseCursorStyle;
        bool _isMouseDown;
        bool _isDragging;
        bool _lastKeydownWithControl;
        bool _lastKeydownWithAlt;
        bool _lastKeydownWithShift;
        int _prevLogicalMouseX;
        int _prevLogicalMouseY;
        int _localMouseDownX;
        int _localMouseDownY;
        //-------
        //event stock
        Stack<UIMouseEventArgs> _stockMouseEvents = new Stack<UIMouseEventArgs>();
        Stack<UIKeyEventArgs> _stockKeyEvents = new Stack<UIKeyEventArgs>();
        Stack<UIFocusEventArgs> _stockFocusEvents = new Stack<UIFocusEventArgs>();
        //-------


        public TopWindowEventRoot(RenderElement topRenderElement)
        {
            _iTopBoxEventPortal = _topWinBoxEventPortal = new RenderElementEventPortal(topRenderElement);
            _rootgfx = topRenderElement.Root;
            _hoverMonitoringTask = new UIHoverMonitorTask(OnMouseHover);
            //
            UIPlatform.RegisterTimerTask(_hoverMonitoringTask);

        }
        public IUIEventListener CurrentKeyboardFocusedElement
        {
            get
            {
                return _currentKbFocusElem;
            }
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
        //
        MouseCursorStyle ITopWindowEventRoot.MouseCursorStyle => _mouseCursorStyle;
        //
        void ITopWindowEventRoot.RootMouseDown(int x, int y, UIMouseButtons button)
        {
            _prevLogicalMouseX = x;
            _prevLogicalMouseY = y;
            _isMouseDown = true;
            _isDragging = false;
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, x, y, button, 0);
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
                    !_currentMouseActiveElement.BypassAllMouseEvents)
                {
                    _draggingElement = _currentMouseActiveElement;
                }
            }


            _mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);
        }
        void ITopWindowEventRoot.RootMouseUp(int x, int y, UIMouseButtons button)
        {
            int xdiff = x - _prevLogicalMouseX;
            int ydiff = y - _prevLogicalMouseY;
            _prevLogicalMouseX = x;
            _prevLogicalMouseY = y;
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, x, y, button, 0);

            e.SetDiff(xdiff, ydiff);
            //----------------------------------
            e.IsDragging = _isDragging;
            _isMouseDown = _isDragging = false;
            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - _lastTimeMouseUp;
            _lastTimeMouseUp = snapMouseUpTime;

            if (_isDragging)
            {
                if (_draggingElement != null)
                {
                    //send this to dragging element first 
                    _draggingElement.GetGlobalLocation(out int d_GlobalX, out int d_globalY);
                    e.SetLocation(e.GlobalX - d_GlobalX, e.GlobalY - d_globalY);
                    e.CapturedMouseX = _localMouseDownX;
                    e.CapturedMouseY = _localMouseDownY;
                    var iportal = _draggingElement as IEventPortal;
                    if (iportal != null)
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
                if (e.IsAlsoDoubleClick)
                {

                }
                _iTopBoxEventPortal.PortalMouseUp(e);
            }


            _localMouseDownX = _localMouseDownY = 0;
            _mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);


        }
        void ITopWindowEventRoot.RootMouseMove(int x, int y, UIMouseButtons button)
        {
            int xdiff = x - _prevLogicalMouseX;
            int ydiff = y - _prevLogicalMouseY;
            _prevLogicalMouseX = x;
            _prevLogicalMouseY = y;
            if (xdiff == 0 && ydiff == 0)
            {
                return;
            }

            //-------------------------------------------------------
            //when mousemove -> reset hover!            
            _hoverMonitoringTask.Reset();
            _hoverMonitoringTask.Enabled = true;
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, x, y, button, 0);
            e.SetDiff(xdiff, ydiff);
            //-------------------------------------------------------
            e.IsDragging = _isDragging = _isMouseDown;
            if (_isDragging)
            {
                if (_draggingElement != null)
                {
                    //send this to dragging element first 

                    _draggingElement.GetGlobalLocation(out int d_GlobalX, out int d_globalY);

                    _draggingElement.GetViewport(out int vwp_left, out int vwp_top);
                    e.SetLocation(e.GlobalX - d_GlobalX + vwp_left, e.GlobalY - d_globalY + vwp_top);

                    e.CapturedMouseX = _localMouseDownX;
                    e.CapturedMouseY = _localMouseDownY;

                    var iportal = _draggingElement as IEventPortal;
                    if (iportal != null)
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
            else
            {
                _iTopBoxEventPortal.PortalMouseMove(e);
                _draggingElement = null;
            }
            //-------------------------------------------------------

            _mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);
        }
        void ITopWindowEventRoot.RootMouseWheel(int delta)
        {
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, 0, 0, 0, delta);

            //find element

            SetUIMouseEventArgsInfo(e, _prevLogicalMouseX, _prevLogicalMouseY, 0, delta);
            e.Shift = _lastKeydownWithShift;
            e.Alt = _lastKeydownWithAlt;
            e.Ctrl = _lastKeydownWithControl;

            _iTopBoxEventPortal.PortalMouseWheel(e);

            _mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);
        }
        void ITopWindowEventRoot.RootGotFocus()
        {
            UIFocusEventArgs e = GetFreeFocusEvent();
            _iTopBoxEventPortal.PortalGotFocus(e);
            ReleaseFocusEvent(e);
        }
        void ITopWindowEventRoot.RootLostFocus()
        {
            UIFocusEventArgs e = GetFreeFocusEvent();
            _iTopBoxEventPortal.PortalLostFocus(e);
            ReleaseFocusEvent(e);
        }
        void ITopWindowEventRoot.RootKeyPress(char c)
        {
            if (_currentKbFocusElem == null)
            {
                return;
            }

            StopCaretBlink();
            UIKeyEventArgs e = GetFreeKeyEvent();
            e.SetKeyChar(c);
            e.ExactHitObject = e.SourceHitElement = _currentKbFocusElem;
            _currentKbFocusElem.ListenKeyPress(e);
            _iTopBoxEventPortal.PortalKeyPress(e);
            ReleaseKeyEvent(e);
        }
        void ITopWindowEventRoot.RootKeyDown(int keydata)
        {
            if (_currentKbFocusElem == null)
            {
                return;
            }

            UIKeyEventArgs e = GetFreeKeyEvent();
            SetKeyData(e, keydata);
            StopCaretBlink();
            e.ExactHitObject = e.SourceHitElement = _currentKbFocusElem;
            _currentKbFocusElem.ListenKeyDown(e);
            _iTopBoxEventPortal.PortalKeyDown(e);
            ReleaseKeyEvent(e);
        }

        void ITopWindowEventRoot.RootKeyUp(int keydata)
        {
            if (_currentKbFocusElem == null)
            {
                _lastKeydownWithShift = _lastKeydownWithAlt = _lastKeydownWithControl = false;

                return;
            }

            StopCaretBlink();
            UIKeyEventArgs e = GetFreeKeyEvent();
            SetKeyData(e, keydata);
            //----------------------------------------------------

            e.ExactHitObject = e.SourceHitElement = _currentKbFocusElem;
            _currentKbFocusElem.ListenKeyUp(e);
            _iTopBoxEventPortal.PortalKeyUp(e);
            //----------------------------------------------------
            ReleaseKeyEvent(e);
            StartCaretBlink();

            _lastKeydownWithShift = _lastKeydownWithControl = _lastKeydownWithAlt = false;
        }
        bool ITopWindowEventRoot.RootProcessDialogKey(int keyData)
        {
            UI.UIKeys k = (UIKeys)keyData;

            if (_currentKbFocusElem == null)
            {
                //set 
                _lastKeydownWithShift = ((k & UIKeys.Shift) == UIKeys.Shift);
                _lastKeydownWithAlt = ((k & UIKeys.Alt) == UIKeys.Alt);
                _lastKeydownWithControl = ((k & UIKeys.Control) == UIKeys.Control);

                return false;
            }


            StopCaretBlink();

            UIKeyEventArgs e = GetFreeKeyEvent();
            e.KeyData = (int)keyData;
            e.SetEventInfo(
                (int)keyData,
                _lastKeydownWithShift = ((k & UIKeys.Shift) == UIKeys.Shift),
                _lastKeydownWithAlt = ((k & UIKeys.Alt) == UIKeys.Alt),
                _lastKeydownWithControl = ((k & UIKeys.Control) == UIKeys.Control));
            bool result = false;
            e.ExactHitObject = e.SourceHitElement = _currentKbFocusElem;
            result = _currentKbFocusElem.ListenProcessDialogKey(e);
            ReleaseKeyEvent(e);
            return result;
        }


        void SetKeyData(UIKeyEventArgs keyEventArgs, int keydata)
        {

            keyEventArgs.SetEventInfo(keydata, _lastKeydownWithShift, _lastKeydownWithAlt, _lastKeydownWithControl);
        }

        void SetUIMouseEventArgsInfo(UIMouseEventArgs mouseEventArg, int x, int y, UIMouseButtons button, int delta)
        {
            mouseEventArg.SetEventInfo(
                x, y,
                button,
                0,
                delta);

            mouseEventArg.Alt = _lastKeydownWithAlt;
            mouseEventArg.Shift = _lastKeydownWithShift;
            mouseEventArg.Ctrl = _lastKeydownWithControl;
        }
        //--------------------------------------------------------------------
        void OnMouseHover(UITimerTask timerTask)
        {
            return;
            //HitTestCoreWithPrevChainHint(hitPointChain.LastestRootX, hitPointChain.LastestRootY);
            //RenderElement hitElement = this.hitPointChain.CurrentHitElement as RenderElement;
            //if (hitElement != null && hitElement.IsTestable)
            //{
            //    DisableGraphicOutputFlush = true;
            //    Point hitElementGlobalLocation = hitElement.GetGlobalLocation();

            //    UIMouseEventArgs e2 = new UIMouseEventArgs();
            //    e2.WinTop = this.topwin;
            //    e2.Location = hitPointChain.CurrentHitPoint;
            //    e2.SourceHitElement = hitElement;
            //    IEventListener ui = hitElement.GetController() as IEventListener;
            //    if (ui != null)
            //    {
            //        ui.ListenMouseEvent(UIMouseEventName.MouseHover, e2);
            //    }

            //    DisableGraphicOutputFlush = false;
            //    FlushAccumGraphicUpdate();
            //}
            //hitPointChain.SwapHitChain();
            //hoverMonitoringTask.SetEnable(false, this.topwin);
        }
        //------------------------------------------------
        UIFocusEventArgs GetFreeFocusEvent()
        {
            if (_stockFocusEvents.Count == 0)
            {
                return new UIFocusEventArgs();
            }
            else
            {
                return _stockFocusEvents.Pop();
            }
        }
        void ReleaseFocusEvent(UIFocusEventArgs e)
        {
            e.Clear();
            _stockFocusEvents.Push(e);
        }
        UIKeyEventArgs GetFreeKeyEvent()
        {
            if (_stockKeyEvents.Count == 0)
            {
                return new UIKeyEventArgs();
            }
            else
            {
                return _stockKeyEvents.Pop();
            }
        }
        void ReleaseKeyEvent(UIKeyEventArgs e)
        {
            e.Clear();
            _stockKeyEvents.Push(e);
        }
        UIMouseEventArgs GetFreeMouseEvent()
        {
            if (_stockMouseEvents.Count == 0)
            {
                return new UIMouseEventArgs();
            }
            else
            {
                return _stockMouseEvents.Pop();
            }
        }
        void ReleaseMouseEvent(UIMouseEventArgs e)
        {
            e.Clear();
            //TODO: review event stock here again
            _stockMouseEvents.Push(e);
        }
    }
}