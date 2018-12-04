//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using PixelFarm.Forms;
namespace LayoutFarm.UI
{
    abstract partial class TopWindowBridgeWinNeutral
    {
        RootGraphic _rootGraphic;
        ITopWindowEventRoot _topWinEventRoot;
        CanvasViewport _canvasViewport;
        MouseCursorStyle _currentCursorStyle = MouseCursorStyle.Default;
        //
        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;
        public TopWindowBridgeWinNeutral(RootGraphic rootGraphic, ITopWindowEventRoot topWinEventRoot)
        {
            _topWinEventRoot = topWinEventRoot;
            _rootGraphic = rootGraphic;
        }
        public abstract void BindWindowControl(Control windowControl);
        public abstract void InvalidateRootArea(Rectangle r);
        public RootGraphic RootGfx
        {
            get { return _rootGraphic; }
        }
        protected abstract void ChangeCursorStyle(MouseCursorStyle cursorStyle);
        protected void SetBaseCanvasViewport(CanvasViewport canvasViewport)
        {
            _canvasViewport = canvasViewport;
        }
        internal virtual void OnHostControlLoaded()
        {
        }
        public void PaintToOutputWindowFullMode()
        {
            Rectangle rect = new Rectangle(0, 0, _rootGraphic.Width, _rootGraphic.Height);
            _rootGraphic.InvalidateRootGraphicArea(ref rect);

            this.PaintToOutputWindow();
        }
        public abstract void PaintToOutputWindow();

        public void UpdateCanvasViewportSize(int w, int h)
        {
            _canvasViewport.UpdateCanvasViewportSize(w, h);
        }

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
            //platform specific to hide or show cursor
            //System.Windows.Forms.Cursor.Show();
        }
        public void HandleGotFocus(EventArgs e)
        {
            if (_canvasViewport.IsClosed)
            {
                return;
            }

            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootGotFocus();
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleLostFocus(EventArgs e)
        {
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootLostFocus();
            PrepareRenderAndFlushAccumGraphics();
        }
        //------------------------------------------------------------------------
        public void HandleMouseDown(int x, int y, UIMouseButtons b)
        {
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootMouseDown(
                x + _canvasViewport.ViewportX,
                y + _canvasViewport.ViewportY,
                b);
            if (_currentCursorStyle != _topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(_currentCursorStyle = _topWinEventRoot.MouseCursorStyle);
            }

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
        public void HandleMouseMove(int x, int y, UIMouseButtons b)
        {
            _topWinEventRoot.RootMouseMove(
                    x + _canvasViewport.ViewportX,
                    y + _canvasViewport.ViewportY,
                    b);
            if (_currentCursorStyle != _topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(_currentCursorStyle = _topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();
        }
        //static UIMouseButtons GetMouseButton(System.Windows.Forms.MouseButtons button)
        //{
        //    switch (button)
        //    {
        //        case MouseButtons.Left:
        //            return UIMouseButtons.Left;
        //        case MouseButtons.Right:
        //            return UIMouseButtons.Right;
        //        case MouseButtons.Middle:
        //            return UIMouseButtons.Middle;
        //        default:
        //            return UIMouseButtons.Left;
        //    }
        //}
        public void HandleMouseUp(int x, int y, UIMouseButtons b)
        {
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootMouseUp(
                   x + _canvasViewport.ViewportX,
                   y + _canvasViewport.ViewportY,
                   b);
            if (_currentCursorStyle != _topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(_currentCursorStyle = _topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleMouseWheel(int delta)
        {
            _canvasViewport.FullMode = true;
            _topWinEventRoot.RootMouseWheel(delta);
            if (_currentCursorStyle != _topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(_currentCursorStyle = _topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleKeyDown(int keyValue)
        {
            //#if DEBUG
            //            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            //            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UI.UIKeys)e.KeyCode);
            //            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            //#endif
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootKeyDown(keyValue);
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleKeyUp(int keyValue)
        {
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootKeyUp(keyValue);
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleKeyPress(char c)
        {
            if (char.IsControl(c))
            {
                return;
            }
#if DEBUG
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYPRESS " + c);
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootKeyPress(c);
            PrepareRenderAndFlushAccumGraphics();
        }
        public bool HandleProcessDialogKey(int keyData)
        {
            _canvasViewport.FullMode = false;
            bool result = _topWinEventRoot.RootProcessDialogKey((int)keyData);
            if (result)
            {
                PrepareRenderAndFlushAccumGraphics();
            }
            return result;
        }

        void PrepareRenderAndFlushAccumGraphics()
        {
            _rootGraphic.PrepareRender();
            _rootGraphic.FlushAccumGraphics();
        }
    }
}
