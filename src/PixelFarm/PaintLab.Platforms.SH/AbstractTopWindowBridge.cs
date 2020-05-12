//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.UI.ForImplementator;

namespace LayoutFarm.UI
{
    public abstract partial class AbstractTopWindowBridge
    {
        readonly RootGraphic _rootgfx;
        readonly ITopWindowEventRoot _topWinEventRoot;
        CanvasViewport _canvasViewport;
        MouseCursorStyle _currentCursorStyle = MouseCursorStyle.Default;


        readonly UIKeyEventArgs _keyEventArgs = new UIKeyEventArgs();
        readonly UIFocusEventArgs _focusEventArgs = new UIFocusEventArgs();

        public AbstractTopWindowBridge(RootGraphic rootgfx, ITopWindowEventRoot topWinEventRoot)
        {
            GlobalRootGraphic.CurrentRootGfx = rootgfx;//temp fix

           _topWinEventRoot = topWinEventRoot;
            _rootgfx = rootgfx;
        }
        public abstract void BindWindowControl(IGpuOpenGLSurfaceView windowControl);
        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;

        public RootGraphic RootGfx => _rootgfx;
        //
        protected abstract void ChangeCursor(MouseCursorStyle cursorStyle);
        protected abstract void ChangeCursor(ImageBinder imgbinder);
        protected abstract void ChangeCursor(Cursor cursor);
        //
        internal void SetBaseCanvasViewport(CanvasViewport canvasViewport)
        {
            _canvasViewport = canvasViewport;
        }
        public virtual void OnHostControlLoaded()
        {
        }
#if DEBUG
        public void dbugPaintToOutputWindowFullMode()
        {
            Rectangle rect = new Rectangle(0, 0, _rootgfx.Width, _rootgfx.Height);
            RootGraphic.InvalidateRectArea(_rootgfx, rect);
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

            _canvasViewport.EvaluateScrollBar(
                out ScrollSurfaceRequestEventArgs hScrollSupportEventArgs,
                out ScrollSurfaceRequestEventArgs vScrollSupportEventArgs);
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

            _canvasViewport.ScrollByNotRaiseEvent(dx, dy,
                out UIScrollEventArgs hScrollEventArgs,
                out UIScrollEventArgs vScrollEventArgs);
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

            _canvasViewport.ScrollToNotRaiseScrollChangedEvent(x, y,
                out UIScrollEventArgs hScrollEventArgs,
                out UIScrollEventArgs vScrollEventArgs);
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

        public void HandleGotFocus(EventArgs e)
        {
            if (_canvasViewport.IsClosed)
            {
                return;
            }
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootGotFocus(_focusEventArgs);
            _focusEventArgs.ResetAll();
            //
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleLostFocus(EventArgs e)
        {
            _canvasViewport.FullMode = false;
            //

            _topWinEventRoot.RootLostFocus(_focusEventArgs);
            _focusEventArgs.ResetAll();
            //
            PrepareRenderAndFlushAccumGraphics();
        }
        //------------------------------------------------------------------------


        public void HandleMouseDown(PrimaryMouseEventArgs mouseEventArgs)
        {
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootMouseDown(mouseEventArgs);
            //
            UpdateCursor();

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


        public void HandleMouseMove(PrimaryMouseEventArgs mouseEventArgs)
        {
            _topWinEventRoot.RootMouseMove(mouseEventArgs);
            UpdateCursor();
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleMouseLeaveFromViewport()
        {
            //move leave from viewport
            _topWinEventRoot.RootMouseLeave();
            UpdateCursor();
            PrepareRenderAndFlushAccumGraphics();
        }

        public void HandleMouseUp(PrimaryMouseEventArgs mouseEventArgs)
        {
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootMouseUp(mouseEventArgs);
            UpdateCursor();

            PrepareRenderAndFlushAccumGraphics();
        }

        Cursor _latestCustomCursor;
        void UpdateCursor()
        {

            if (_topWinEventRoot.RequestCursor != null)
            {
                //specific custom cursor
                _currentCursorStyle = MouseCursorStyle.CustomStyle;
                if (_latestCustomCursor != _topWinEventRoot.RequestCursor)
                {
                    //some change                     
                    ChangeCursor(_latestCustomCursor = _topWinEventRoot.RequestCursor);
                }
            }
            else
            {
                _latestCustomCursor = null;
                if (_currentCursorStyle != _topWinEventRoot.RequestCursorStyle)
                {
                    ChangeCursor(_currentCursorStyle = _topWinEventRoot.RequestCursorStyle);
                }
            }


        }
        public void HandleMouseWheel(PrimaryMouseEventArgs mouseEventArgs)
        {
            _canvasViewport.FullMode = true;
            _topWinEventRoot.RootMouseWheel(mouseEventArgs);
            UpdateCursor();


            PrepareRenderAndFlushAccumGraphics();
        }


        //------------------------------------------------------
        public void HandleKeyDown(UIKeyEventArgs keyEventArgs)
        {

#if DEBUG
            //System.Diagnostics.Debug.WriteLine("keydown" + (dbug_keydown_count++));
            dbugTopwin.dbugVisualRoot?.dbug_PushLayoutTraceMessage("======");
            dbugTopwin.dbugVisualRoot?.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UI.UIKeys)keyEventArgs.KeyCode);
            dbugTopwin.dbugVisualRoot?.dbug_PushLayoutTraceMessage("======");
#endif
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootKeyDown(keyEventArgs);
            keyEventArgs.ResetAll();
            PrepareRenderAndFlushAccumGraphics();
        }


        public void HandleKeyUp(UIKeyEventArgs keyEventArgs)
        {
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootKeyUp(keyEventArgs);
            keyEventArgs.ResetAll();
            PrepareRenderAndFlushAccumGraphics();
        }

        public void HandleKeyPress(UIKeyEventArgs keyEventArgs, char keyChar)
        {
            if (char.IsControl(keyChar))
            {
                return;
            }
#if DEBUG
            dbugTopwin.dbugVisualRoot?.dbug_PushLayoutTraceMessage("======");
            dbugTopwin.dbugVisualRoot?.dbug_PushLayoutTraceMessage("KEYPRESS " + keyChar);
            dbugTopwin.dbugVisualRoot?.dbug_PushLayoutTraceMessage("======");
#endif
            _canvasViewport.FullMode = false;

            keyEventArgs.SetKeyChar(keyChar);
            _topWinEventRoot.RootKeyPress(keyEventArgs);
            keyEventArgs.ResetAll();

            PrepareRenderAndFlushAccumGraphics();
        }
        //#if DEBUG
        //        static int dbug_preview_dialogKey_count = 0;
        //#endif

        public bool HandleProcessDialogKey(UIKeys keyData)
        {
            //#if DEBUG
            //          System.Diagnostics.Debug.WriteLine("prev_dlgkey" + (dbug_preview_dialogKey_count++));
            //#endif
            _canvasViewport.FullMode = false;
            _keyEventArgs.SetEventInfo((uint)keyData, false, false, false, UIEventName.ProcessDialogKey);//f-f-f will be set later
            bool result = _topWinEventRoot.RootProcessDialogKey(_keyEventArgs);
            if (result)
            {
                PrepareRenderAndFlushAccumGraphics();
            }

            return result;
        }

        void PrepareRenderAndFlushAccumGraphics()
        {
            //TODO: review here
            RootGraphic backup = GlobalRootGraphic.CurrentRootGfx;
            GlobalRootGraphic.CurrentRootGfx = _rootgfx;
            _rootgfx.PrepareRender();
            _rootgfx.FlushAccumGraphics();
            GlobalRootGraphic.CurrentRootGfx = backup;//restore
        }
    }
}
