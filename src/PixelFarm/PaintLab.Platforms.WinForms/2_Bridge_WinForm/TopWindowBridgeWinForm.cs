//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using PixelFarm.Drawing;
using LayoutFarm.UI.InputBridge;

namespace LayoutFarm.UI
{
    /// <summary>
    /// this class is a specific bridge for WinForms***
    /// </summary>
    public abstract partial class TopWindowBridgeWinForm
    {
        RootGraphic _rootGraphic;
        ITopWindowEventRoot _topWinEventRoot;
        CanvasViewport _canvasViewport;
        MouseCursorStyle _currentCursorStyle = MouseCursorStyle.Default;

        Stack<UIMouseEventArgs> _mouseEventStack = new Stack<UIMouseEventArgs>(); //reusable
        Stack<UIKeyEventArgs> _keyEventStack = new Stack<UIKeyEventArgs>(); //reusable
        Stack<UIFocusEventArgs> _focusEventStack = new Stack<UIFocusEventArgs>(); //resuable

        public TopWindowBridgeWinForm(RootGraphic rootGraphic, ITopWindowEventRoot topWinEventRoot)
        {
            _topWinEventRoot = topWinEventRoot;
            _rootGraphic = rootGraphic;
        }

        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;


        public abstract void BindWindowControl(Control windowControl);
        public abstract void InvalidateRootArea(Rectangle r);
        //
        public RootGraphic RootGfx => _rootGraphic;
        //
        protected abstract void ChangeCursor(MouseCursorStyle cursorStyle);
        protected abstract void ChangeCursor(ImageBinder imgbinder);
        //
        internal void SetBaseCanvasViewport(CanvasViewport canvasViewport)
        {
            _canvasViewport = canvasViewport;
        }
        internal virtual void OnHostControlLoaded()
        {
        }
#if DEBUG
        public void dbugPaintToOutputWindowFullMode()
        {
            Rectangle rect = new Rectangle(0, 0, _rootGraphic.Width, _rootGraphic.Height);
            _rootGraphic.InvalidateRootGraphicArea(ref rect);
            this.PaintToOutputWindow();
        }
#endif


        //-------------------------------------------------------------------
        public abstract void PaintToOutputWindow();
        public abstract void PaintToOutputWindow(Rectangle invalidateArea);
        //-------------------------------------------------------------------
        public abstract void CopyOutputPixelBuffer(int x, int y, int w, int h, IntPtr outputBuffer);
        public void UpdateCanvasViewportSize(int w, int h)
        {
            _canvasViewport.UpdateCanvasViewportSize(w, h);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll"), System.Security.SuppressUnmanagedCodeSecurity]
        protected static extern IntPtr GetDC(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll"), System.Security.SuppressUnmanagedCodeSecurity]
        protected static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);



        public void Close()
        {
            OnClosing();
            _canvasViewport.Close();
        }
        protected virtual void OnClosing()
        {
        }
        //---------------------------------------------------------------------
        public void EvaluateScrollbar()
        {
            ScrollSurfaceRequestEventArgs hScrollSupportEventArgs;
            ScrollSurfaceRequestEventArgs vScrollSupportEventArgs;
            _canvasViewport.EvaluateScrollBar(out hScrollSupportEventArgs, out vScrollSupportEventArgs);
            if (hScrollSupportEventArgs != null)
            {
                viewport_HScrollRequest(this, hScrollSupportEventArgs);
            }
            if (vScrollSupportEventArgs != null)
            {
                viewport_VScrollRequest(this, vScrollSupportEventArgs);
            }
        }
        public void ScrollBy(int dx, int dy)
        {
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            _canvasViewport.ScrollByNotRaiseEvent(dx, dy, out hScrollEventArgs, out vScrollEventArgs);
            if (vScrollEventArgs != null)
            {
                viewport_VScrollChanged(this, vScrollEventArgs);
            }
            if (hScrollEventArgs != null)
            {
                viewport_HScrollChanged(this, hScrollEventArgs);
            }
            PaintToOutputWindow();
        }
        public void ScrollTo(int x, int y)
        {
            Point viewporyLocation = _canvasViewport.LogicalViewportLocation;
            if (viewporyLocation.Y == y && viewporyLocation.X == x)
            {
                return;
            }
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            _canvasViewport.ScrollToNotRaiseScrollChangedEvent(x, y, out hScrollEventArgs, out vScrollEventArgs);
            if (vScrollEventArgs != null)
            {
                viewport_VScrollChanged(this, vScrollEventArgs);
            }
            if (hScrollEventArgs != null)
            {
                viewport_HScrollChanged(this, vScrollEventArgs);
            }

            PaintToOutputWindow();
        }

        void viewport_HScrollChanged(object sender, UIScrollEventArgs e)
        {
            if (HScrollChanged != null)
            {
                HScrollChanged.Invoke(sender, e);
            }
        }
        void viewport_HScrollRequest(object sender, ScrollSurfaceRequestEventArgs e)
        {
            if (HScrollRequest != null)
            {
                HScrollRequest.Invoke(sender, e);
            }
        }
        void viewport_VScrollChanged(object sender, UIScrollEventArgs e)
        {
            if (VScrollChanged != null)
            {
                VScrollChanged.Invoke(sender, e);
            }
        }
        void viewport_VScrollRequest(object sender, ScrollSurfaceRequestEventArgs e)
        {
            if (VScrollRequest != null)
            {
                VScrollRequest.Invoke(sender, e);
            }
        }
        public void HandleMouseEnterToViewport()
        {
            //System.Windows.Forms.Cursor.Hide();
        }
        public void HandleMouseLeaveFromViewport()
        {
            System.Windows.Forms.Cursor.Show();
        }


        UIFocusEventArgs GetFreeFocusEventArgs() => _focusEventStack.Count > 0 ? _focusEventStack.Pop() : new UIFocusEventArgs();
        void ReleaseFocusEventArgs(UIFocusEventArgs e)
        {
            e.Clear();
            _focusEventStack.Push(e);
        }
        public void HandleGotFocus(EventArgs e)
        {
            if (_canvasViewport.IsClosed)
            {
                return;
            }
            _canvasViewport.FullMode = false;

            UIFocusEventArgs e1 = GetFreeFocusEventArgs();
            _topWinEventRoot.RootGotFocus(e1);
            ReleaseFocusEventArgs(e1);
            //
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleLostFocus(EventArgs e)
        {
            _canvasViewport.FullMode = false;
            //
            UIFocusEventArgs e1 = GetFreeFocusEventArgs();
            _topWinEventRoot.RootLostFocus(e1);
            ReleaseFocusEventArgs(e1);
            //
            PrepareRenderAndFlushAccumGraphics();
        }
        //------------------------------------------------------------------------
        public void HandleMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            _canvasViewport.FullMode = false;

            UIMouseEventArgs mouseEventArgs = GetTranslatedUIMouseEventArgs(e);
            _topWinEventRoot.RootMouseDown(mouseEventArgs);

            if (_currentCursorStyle != mouseEventArgs.MouseCursorStyle)
            {
                ChangeCursor(_currentCursorStyle = mouseEventArgs.MouseCursorStyle);
            }

            ReleaseUIMouseEventArgs(mouseEventArgs);

            PrepareRenderAndFlushAccumGraphics();
#if DEBUG
            RootGraphic visualroot = this.dbugTopwin.dbugVRoot;
            if (visualroot.dbug_RecordHitChain)
            {
                dbug_rootDocHitChainMsgs.Clear();
                visualroot.dbug_DumpCurrentHitChain(dbug_rootDocHitChainMsgs);
                dbug_InvokeHitChainMsg();
            }
#endif
        }

        //------------------
        void ReleaseUIMouseEventArgs(UIMouseEventArgs mouseEventArgs)
        {
            mouseEventArgs.Clear();
            _mouseEventStack.Push(mouseEventArgs);
        }
        UIMouseEventArgs GetTranslatedUIMouseEventArgs(System.Windows.Forms.MouseEventArgs e)
        {
            UIMouseButtons mouseButton = UIMouseButtons.Left;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    mouseButton = UIMouseButtons.Left;
                    break;
                case MouseButtons.Right:
                    mouseButton = UIMouseButtons.Right;
                    break;
                case MouseButtons.Middle:
                    mouseButton = UIMouseButtons.Middle;
                    break;
                default:
                    mouseButton = UIMouseButtons.Left;
                    break;
            }

            UIMouseEventArgs mouseEventArgs = (_mouseEventStack.Count > 0) ? _mouseEventStack.Pop() : new UIMouseEventArgs();
            mouseEventArgs.SetEventInfo(
                e.X + _canvasViewport.ViewportX,
                e.Y + _canvasViewport.ViewportY,
                mouseButton,
                e.Clicks,
                e.Delta);
            return mouseEventArgs;
        }

        //------------------
        public void HandleMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            UIMouseEventArgs mouseEventArgs = GetTranslatedUIMouseEventArgs(e);
            _topWinEventRoot.RootMouseMove(mouseEventArgs);
            if (_currentCursorStyle != mouseEventArgs.MouseCursorStyle)
            {
                ChangeCursor(_currentCursorStyle = mouseEventArgs.MouseCursorStyle);
            }
            ReleaseUIMouseEventArgs(mouseEventArgs);
            PrepareRenderAndFlushAccumGraphics();
        }

        public void HandleMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            _canvasViewport.FullMode = false;
            UIMouseEventArgs mouseEventArgs = GetTranslatedUIMouseEventArgs(e);
            _topWinEventRoot.RootMouseUp(mouseEventArgs);

            if (_currentCursorStyle != mouseEventArgs.MouseCursorStyle)
            {
                ChangeCursor(_currentCursorStyle = mouseEventArgs.MouseCursorStyle);
            }

            ReleaseUIMouseEventArgs(mouseEventArgs);
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            _canvasViewport.FullMode = true;
            UIMouseEventArgs mouseEventArgs = GetTranslatedUIMouseEventArgs(e);
            _topWinEventRoot.RootMouseWheel(mouseEventArgs);
            if (_currentCursorStyle != mouseEventArgs.MouseCursorStyle)
            {
                ChangeCursor(_currentCursorStyle = mouseEventArgs.MouseCursorStyle);
            }
            ReleaseUIMouseEventArgs(mouseEventArgs);
            PrepareRenderAndFlushAccumGraphics();
        }


        //------------------------------------------------------
        UIKeyEventArgs GetTranslatedUIKeyEventArg(System.Windows.Forms.KeyEventArgs e)
        {
            UIKeyEventArgs keyEventArg = _keyEventStack.Count > 0 ? _keyEventStack.Pop() : new UIKeyEventArgs();
            keyEventArg.SetEventInfo((int)e.KeyData, e.Shift, e.Alt, e.Control);
            return keyEventArg;
        }
        UIKeyEventArgs GetFreeUIKeyEventArg()
        {
            return _keyEventStack.Count > 0 ? _keyEventStack.Pop() : new UIKeyEventArgs();
        }
        UIKeyEventArgs GetTranslatedUIKeyEventArg(System.Windows.Forms.KeyPressEventArgs e)
        {
            UIKeyEventArgs keyEventArg = _keyEventStack.Count > 0 ? _keyEventStack.Pop() : new UIKeyEventArgs();
            keyEventArg.SetKeyChar(e.KeyChar);
            return keyEventArg;
        }
        void ReleaseUIKeyEventArgs(UIKeyEventArgs e)
        {
            e.Clear();
            _keyEventStack.Push(e);
        }
        //------------------------------------------------------
        public void HandleKeyDown(System.Windows.Forms.KeyEventArgs e)
        {


#if DEBUG
            //System.Diagnostics.Debug.WriteLine("keydown" + (dbug_keydown_count++));
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UI.UIKeys)e.KeyCode);
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif
            _canvasViewport.FullMode = false;
            UIKeyEventArgs keyEventArgs = GetTranslatedUIKeyEventArg(e);
            _topWinEventRoot.RootKeyDown(keyEventArgs);
            ReleaseUIKeyEventArgs(keyEventArgs);

            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            _canvasViewport.FullMode = false;
            UIKeyEventArgs keyEventArgs = GetTranslatedUIKeyEventArg(e);
            _topWinEventRoot.RootKeyUp(keyEventArgs);
            ReleaseUIKeyEventArgs(keyEventArgs);

            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }
#if DEBUG
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYPRESS " + e.KeyChar);
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif
            _canvasViewport.FullMode = false;

            UIKeyEventArgs keyEventArgs = GetTranslatedUIKeyEventArg(e);
            keyEventArgs.SetKeyChar(e.KeyChar);
            _topWinEventRoot.RootKeyPress(keyEventArgs);
            ReleaseUIKeyEventArgs(keyEventArgs);

            PrepareRenderAndFlushAccumGraphics();
        }

        //#if DEBUG
        //        static int dbug_preview_dialogKey_count = 0;
        //#endif

        public bool HandleProcessDialogKey(Keys keyData)
        {
            //#if DEBUG
            //          System.Diagnostics.Debug.WriteLine("prev_dlgkey" + (dbug_preview_dialogKey_count++));
            //#endif
            _canvasViewport.FullMode = false;

            UIKeyEventArgs keyEventArgs = GetFreeUIKeyEventArg();
            keyEventArgs.SetEventInfo((int)keyData, false, false, false);//f-f-f will be set later
            bool result = _topWinEventRoot.RootProcessDialogKey(keyEventArgs);
            if (result)
            {
                PrepareRenderAndFlushAccumGraphics();
            }
            ReleaseUIKeyEventArgs(keyEventArgs);
            return result;
        }

        void PrepareRenderAndFlushAccumGraphics()
        {
            _rootGraphic.PrepareRender();
            _rootGraphic.FlushAccumGraphics();
        }
    }
}
