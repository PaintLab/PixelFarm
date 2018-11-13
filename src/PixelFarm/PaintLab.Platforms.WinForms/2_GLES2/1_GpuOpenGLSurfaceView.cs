//MIT, 2014-present, WinterDev

#if GL_ENABLE
using System;
using System.Windows.Forms;
namespace LayoutFarm.UI.OpenGL
{
    //app specific
    partial class GpuOpenGLSurfaceView : OpenTK.MyGLControl
    {
        MyTopWindowBridgeOpenGL _winBridge;
        public GpuOpenGLSurfaceView()
        {
        }

        public void Bind(MyTopWindowBridgeOpenGL winBridge)
        {
            //1. 
            this._winBridge = winBridge;
            this._winBridge.BindWindowControl(this);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this._winBridge != null)
            {
                this._winBridge.UpdateCanvasViewportSize(this.Width, this.Height);
            }
            base.OnSizeChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this._winBridge.HandleMouseEnterToViewport();
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            this._winBridge.HandleMouseLeaveFromViewport();
            base.OnMouseLeave(e);
        }
        //-----------------------------------------------------------------------------
        protected override void OnGotFocus(EventArgs e)
        {
            this._winBridge.HandleGotFocus(e);
            base.OnGotFocus(e);

        }
        protected override void OnLostFocus(EventArgs e)
        {
            this._winBridge.HandleGotFocus(e);
            base.OnLostFocus(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this._winBridge.HandleMouseDown(e);
            base.OnMouseDown(e);

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this._winBridge.HandleMouseMove(e);
            base.OnMouseMove(e);

        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this._winBridge.HandleMouseUp(e);
            base.OnMouseUp(e);

        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this._winBridge.HandleMouseWheel(e);
            base.OnMouseWheel(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            this._winBridge.HandleKeyDown(e);
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            this._winBridge.HandleKeyUp(e);
            base.OnKeyUp(e);
        }
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            this._winBridge.HandleKeyPress(e);
            return;
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this._winBridge.HandleProcessDialogKey(keyData))
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

    }
}
#endif