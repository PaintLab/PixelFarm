﻿//Apache2, 2014-present, WinterDev

using System;
using System.Windows.Forms;
using PixelFarm.Drawing;
namespace LayoutFarm.UI
{
    /// <summary>
    /// this class is specific bridge for WinForms***
    /// </summary>
    public abstract partial class TopWindowBridgeWinForm
    {
        RootGraphic _rootGraphic;
        ITopWindowEventRoot _topWinEventRoot;
        CanvasViewport _canvasViewport;
        MouseCursorStyle _currentCursorStyle = MouseCursorStyle.Default;
        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;

        public TopWindowBridgeWinForm(RootGraphic rootGraphic, ITopWindowEventRoot topWinEventRoot)
        {

#if DEBUG
            if (!PixelFarm.CpuBlit.ExternalImageService.HasExternalImgCodec)
            {
                PixelFarm.CpuBlit.ExternalImageService.RegisterExternalImageEncodeDelegate(SaveImage);
            }

#endif
            this._topWinEventRoot = topWinEventRoot;
            this._rootGraphic = rootGraphic;
        }
#if DEBUG
        static void SaveImage(byte[] imgBuffer, int pixelWidth, int pixelHeight)
        {
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(pixelWidth, pixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, pixelWidth, pixelHeight), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                System.Runtime.InteropServices.Marshal.Copy(imgBuffer, 0, bmpdata.Scan0, imgBuffer.Length);
                bmp.UnlockBits(bmpdata);
                bmp.Save("d:\\WImageTest\\test002.png");
            }
        }
#endif

        public abstract void BindWindowControl(Control windowControl);
        public abstract void InvalidateRootArea(Rectangle r);
        public RootGraphic RootGfx
        {
            get { return this._rootGraphic; }
        }
        protected abstract void ChangeCursorStyle(MouseCursorStyle cursorStyle);
        internal void SetBaseCanvasViewport(CanvasViewport canvasViewport)
        {
            this._canvasViewport = canvasViewport;
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
            this._canvasViewport.UpdateCanvasViewportSize(w, h);
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
        public void HandleGotFocus(EventArgs e)
        {
            if (_canvasViewport.IsClosed)
            {
                return;
            }

            _canvasViewport.FullMode = false;
            this._topWinEventRoot.RootGotFocus();
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleLostFocus(EventArgs e)
        {
            _canvasViewport.FullMode = false;
            this._topWinEventRoot.RootLostFocus();
            PrepareRenderAndFlushAccumGraphics();
        }
        //------------------------------------------------------------------------
        public void HandleMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            _canvasViewport.FullMode = false;
            this._topWinEventRoot.RootMouseDown(
                e.X + this._canvasViewport.ViewportX,
                e.Y + this._canvasViewport.ViewportY,
                GetMouseButton(e.Button));
            if (_currentCursorStyle != this._topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this._currentCursorStyle = this._topWinEventRoot.MouseCursorStyle);
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
        public void HandleMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            this._topWinEventRoot.RootMouseMove(
                    e.X + this._canvasViewport.ViewportX,
                    e.Y + this._canvasViewport.ViewportY,
                    GetMouseButton(e.Button));
            if (_currentCursorStyle != this._topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this._currentCursorStyle = this._topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();
        }
        static UIMouseButtons GetMouseButton(System.Windows.Forms.MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return UIMouseButtons.Left;
                case MouseButtons.Right:
                    return UIMouseButtons.Right;
                case MouseButtons.Middle:
                    return UIMouseButtons.Middle;
                default:
                    return UIMouseButtons.Left;
            }
        }
        public void HandleMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            _canvasViewport.FullMode = false;
            _topWinEventRoot.RootMouseUp(
                     e.X + this._canvasViewport.ViewportX,
                     e.Y + this._canvasViewport.ViewportY,
                    GetMouseButton(e.Button));
            if (_currentCursorStyle != this._topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this._currentCursorStyle = this._topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            _canvasViewport.FullMode = true;
            this._topWinEventRoot.RootMouseWheel(e.Delta);
            if (_currentCursorStyle != this._topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this._currentCursorStyle = this._topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();
        }

        //#if DEBUG
        //        static int dbug_keydown_count = 0;
        //#endif
        public void HandleKeyDown(System.Windows.Forms.KeyEventArgs e)
        {


#if DEBUG
            //Console.WriteLine("keydown" + (dbug_keydown_count++));
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UI.UIKeys)e.KeyCode);
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif
            _canvasViewport.FullMode = false;
            this._topWinEventRoot.RootKeyDown(e.KeyValue);
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            _canvasViewport.FullMode = false;
            this._topWinEventRoot.RootKeyUp(e.KeyValue);
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
            this._topWinEventRoot.RootKeyPress(e.KeyChar);
            PrepareRenderAndFlushAccumGraphics();
        }

        //#if DEBUG
        //        static int dbug_preview_dialogKey_count = 0;
        //#endif

        public bool HandleProcessDialogKey(Keys keyData)
        {
            //#if DEBUG
            //            Console.WriteLine("prev_dlgkey" + (dbug_preview_dialogKey_count++));
            //#endif
            _canvasViewport.FullMode = false;
            bool result = this._topWinEventRoot.RootProcessDialogKey((int)keyData);
            if (result)
            {
                PrepareRenderAndFlushAccumGraphics();
            }
            return result;
        }

        void PrepareRenderAndFlushAccumGraphics()
        {
            this._rootGraphic.PrepareRender();
            this._rootGraphic.FlushAccumGraphics();
        }
    }
}
