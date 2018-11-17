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
            this._iTopBoxEventPortal = this._topWinBoxEventPortal = new RenderElementEventPortal(topRenderElement);
            this._rootgfx = topRenderElement.Root;
            this._hoverMonitoringTask = new UIHoverMonitorTask(OnMouseHover);
            //
            UIPlatform.RegisterTimerTask(_hoverMonitoringTask);

        }
        public IUIEventListener CurrentKeyboardFocusedElement
        {
            get
            {
                return this._currentKbFocusElem;
            }
            set
            {
                //1. lost keyboard focus
                if (this._currentKbFocusElem != null && this._currentKbFocusElem != value)
                {
                    _currentKbFocusElem.ListenLostKeyboardFocus(null);
                }
                //2. keyboard focus
                _currentKbFocusElem = value;
            }
        }
        void StartCaretBlink()
        {
            this._rootgfx.CaretStartBlink();
        }
        void StopCaretBlink()
        {
            this._rootgfx.CaretStopBlink();
        }

        MouseCursorStyle ITopWindowEventRoot.MouseCursorStyle
        {
            get { return this._mouseCursorStyle; }
        }
        void ITopWindowEventRoot.RootMouseDown(int x, int y, UIMouseButtons button)
        {
            this._prevLogicalMouseX = x;
            this._prevLogicalMouseY = y;
            this._isMouseDown = true;
            this._isDragging = false;
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, x, y, button, 0);
            //
            e.Shift = _lastKeydownWithShift;
            e.Alt = _lastKeydownWithAlt;
            e.Ctrl = _lastKeydownWithControl;
            //
            e.PreviousMouseDown = this._latestMouseDown;
            //
            _iTopBoxEventPortal.PortalMouseDown(e);
            //
            this._currentMouseActiveElement = this._latestMouseDown = e.CurrentContextElement;
            this._localMouseDownX = e.X;
            this._localMouseDownY = e.Y;
            if (e.DraggingElement != null)
            {
                if (e.DraggingElement != e.CurrentContextElement)
                {
                    //change captured element
                    int globalX, globalY;
                    e.DraggingElement.GetGlobalLocation(out globalX, out globalY);
                    //find new capture pos
                    this._localMouseDownX = e.GlobalX - globalX;
                    this._localMouseDownY = e.GlobalY - globalY;
                }
                this._draggingElement = e.DraggingElement;
            }
            else
            {
                this._draggingElement = this._currentMouseActiveElement;
            }


            this._mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);
        }
        void ITopWindowEventRoot.RootMouseUp(int x, int y, UIMouseButtons button)
        {
            int xdiff = x - _prevLogicalMouseX;
            int ydiff = y - _prevLogicalMouseY;
            this._prevLogicalMouseX = x;
            this._prevLogicalMouseY = y;
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, x, y, button, 0);

            e.SetDiff(xdiff, ydiff);
            //----------------------------------
            e.IsDragging = _isDragging;
            this._isMouseDown = this._isDragging = false;
            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - _lastTimeMouseUp;
            this._lastTimeMouseUp = snapMouseUpTime;

            if (this._isDragging)
            {
                if (_draggingElement != null)
                {
                    //send this to dragging element first
                    int d_GlobalX, d_globalY;
                    _draggingElement.GetGlobalLocation(out d_GlobalX, out d_globalY);
                    e.SetLocation(e.GlobalX - d_GlobalX, e.GlobalY - d_globalY);
                    e.CapturedMouseX = this._localMouseDownX;
                    e.CapturedMouseY = this._localMouseDownY;
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


            this._localMouseDownX = this._localMouseDownY = 0;
            this._mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);


        }
        void ITopWindowEventRoot.RootMouseMove(int x, int y, UIMouseButtons button)
        {
            int xdiff = x - _prevLogicalMouseX;
            int ydiff = y - _prevLogicalMouseY;
            this._prevLogicalMouseX = x;
            this._prevLogicalMouseY = y;
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
            e.IsDragging = this._isDragging = this._isMouseDown;
            if (this._isDragging)
            {
                if (_draggingElement != null)
                {
                    //send this to dragging element first 
                    int d_GlobalX, d_globalY;
                    _draggingElement.GetGlobalLocation(out d_GlobalX, out d_globalY);


                    int vwp_x, vwp_y;
                    _draggingElement.GetViewport(out vwp_x, out vwp_y);
                    e.SetLocation(e.GlobalX - d_GlobalX + vwp_x, e.GlobalY - d_globalY + vwp_y);

                    e.CapturedMouseX = this._localMouseDownX;
                    e.CapturedMouseY = this._localMouseDownY;

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

            this._mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);
        }
        void ITopWindowEventRoot.RootMouseWheel(int delta)
        {
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, 0, 0, 0, delta);
            if (_currentMouseActiveElement != null)
            {
                _currentMouseActiveElement.ListenMouseWheel(e);
            }
            _iTopBoxEventPortal.PortalMouseWheel(e);
            this._mouseCursorStyle = e.MouseCursorStyle;
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
                this._lastKeydownWithShift = this._lastKeydownWithAlt = this._lastKeydownWithControl = false;

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
                this._lastKeydownWithShift = ((k & UIKeys.Shift) == UIKeys.Shift);
                this._lastKeydownWithAlt = ((k & UIKeys.Alt) == UIKeys.Alt);
                this._lastKeydownWithControl = ((k & UIKeys.Control) == UIKeys.Control);

                return false;
            }


            StopCaretBlink();

            UIKeyEventArgs e = GetFreeKeyEvent();
            e.KeyData = (int)keyData;
            e.SetEventInfo(
                (int)keyData,
                this._lastKeydownWithShift = ((k & UIKeys.Shift) == UIKeys.Shift),
                this._lastKeydownWithAlt = ((k & UIKeys.Alt) == UIKeys.Alt),
                this._lastKeydownWithControl = ((k & UIKeys.Control) == UIKeys.Control));
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
            if (this._stockFocusEvents.Count == 0)
            {
                return new UIFocusEventArgs();
            }
            else
            {
                return this._stockFocusEvents.Pop();
            }
        }
        void ReleaseFocusEvent(UIFocusEventArgs e)
        {
            e.Clear();
            this._stockFocusEvents.Push(e);
        }
        UIKeyEventArgs GetFreeKeyEvent()
        {
            if (this._stockKeyEvents.Count == 0)
            {
                return new UIKeyEventArgs();
            }
            else
            {
                return this._stockKeyEvents.Pop();
            }
        }
        void ReleaseKeyEvent(UIKeyEventArgs e)
        {
            e.Clear();
            this._stockKeyEvents.Push(e);
        }
        UIMouseEventArgs GetFreeMouseEvent()
        {
            if (this._stockMouseEvents.Count == 0)
            {
                return new UIMouseEventArgs();
            }
            else
            {
                return this._stockMouseEvents.Pop();
            }
        }
        void ReleaseMouseEvent(UIMouseEventArgs e)
        {
            e.Clear();
            //TODO: review event stock here again
            this._stockMouseEvents.Push(e);
        }
    }
}