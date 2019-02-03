//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Forms;
using LayoutFarm.UI.InputBridge;
using LayoutFarm.UI.OpenGL;
using Pencil.Gaming;

namespace LayoutFarm.UI.WinNeutral
{
    partial class GpuOpenGLSurfaceView : MyGLControl
    {
        MyTopWindowBridgeOpenGL _winBridge;
        public GpuOpenGLSurfaceView()
        {
        }
        //----------------------------------------------------------------------------
        public void Bind(MyTopWindowBridgeOpenGL winBridge)
        {
            //1. 
            _winBridge = winBridge;
            _winBridge.BindWindowControl(this);
        }
        //----------------------------------------------------------------------------
        protected override void OnSizeChanged(EventArgs e)
        {
            if (_winBridge != null)
            {
                _winBridge.UpdateCanvasViewportSize(this.Width, this.Height);
            }
            base.OnSizeChanged(e);
        }
        protected override void OnFocus()
        {
            _winBridge.HandleGotFocus(EventArgs.Empty);
            base.OnFocus();
        }
        protected override void OnLostFocus()
        {
            _winBridge.HandleLostFocus(EventArgs.Empty);
            base.OnLostFocus();
        }
        protected override void OnMouseDown(MouseButton btn, int x, int y)
        {
            _winBridge.HandleMouseDown(btn, x, y);
            base.OnMouseDown(btn, x, y);
        }

        protected override void OnMouseMove(double x, double y)
        {
            //TODO: review int cast here
            _winBridge.HandleMouseMove((int)x, (int)y);
            base.OnMouseMove(x, y);
        }
        protected override void OnMouseUp(MouseButton btn, double x, double y)
        {
            _winBridge.HandleMouseUp(btn, (int)x, (int)y);
            base.OnMouseUp(btn, x, y);
        }
        protected override void OnKeyDown(Key key, int scanCode, KeyModifiers mods)
        {
            base.OnKeyDown(key, scanCode, mods);
        }
        protected override void OnKeyPress(char c)
        {
            base.OnKeyPress(c);
        }
        protected override void OnKeyUp(Key key, int scanCode, KeyModifiers mods)
        {
            base.OnKeyUp(key, scanCode, mods);
        }
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }
        protected override void OnKeyRepeat(Key key, int scanCode, KeyModifiers mods)
        {
            base.OnKeyRepeat(key, scanCode, mods);
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    if (_winBridge != null)
        //    {
        //        _winBridge.PaintToOutputWindow(e.ClipRectangle.ToRect());
        //    }
        //    base.OnPaint(e);
        //}

        ////-----------------------------------------------------------------------------
        //protected override void OnMouseEnter(EventArgs e)
        //{
        //    _winBridge.HandleMouseEnterToViewport();
        //    base.OnMouseEnter(e);
        //}
        //protected override void OnMouseLeave(EventArgs e)
        //{
        //    _winBridge.HandleMouseLeaveFromViewport();
        //    base.OnMouseLeave(e);
        //}
        ////

        //protected override void OnMouseWheel(MouseEventArgs e)
        //{
        //    _winBridge.HandleMouseWheel(e);
        //    base.OnMouseWheel(e);
        //}
        ////-----------------------------------------------------------------------------
        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    _winBridge.HandleKeyDown(e);
        //    base.OnKeyDown(e);
        //}
        //protected override void OnKeyUp(KeyEventArgs e)
        //{
        //    _winBridge.HandleKeyUp(e);
        //    base.OnKeyUp(e);
        //}
        //protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        //{
        //    _winBridge.HandleKeyPress(e);
        //    return;
        //}
        //protected override bool ProcessDialogKey(Keys keyData)
        //{
        //    if (_winBridge.HandleProcessDialogKey(keyData))
        //    {
        //        return true;
        //    }
        //    return base.ProcessDialogKey(keyData);
        //}

    }
}