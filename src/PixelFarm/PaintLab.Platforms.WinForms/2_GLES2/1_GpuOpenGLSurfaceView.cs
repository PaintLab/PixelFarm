//MIT, 2014-present, WinterDev

 
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
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_winBridge != null)
            {
                _winBridge.PaintToOutputWindow(e.ClipRectangle.ToRect());
            }
            base.OnPaint(e);
        }
        //-----------------------------------------------------------------------------
        protected override void OnGotFocus(EventArgs e)
        {
            _winBridge.HandleGotFocus(e);
            base.OnGotFocus(e);

        }
        protected override void OnLostFocus(EventArgs e)
        {
            _winBridge.HandleGotFocus(e);
            base.OnLostFocus(e);
        }
        //-----------------------------------------------------------------------------
        protected override void OnMouseEnter(EventArgs e)
        {
            _winBridge.HandleMouseEnterToViewport();
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            _winBridge.HandleMouseLeaveFromViewport();
            base.OnMouseLeave(e);
        }
        //
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _winBridge.HandleMouseDown(e);
            base.OnMouseDown(e);

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _winBridge.HandleMouseMove(e);
            base.OnMouseMove(e);

        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _winBridge.HandleMouseUp(e);
            base.OnMouseUp(e);

        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            _winBridge.HandleMouseWheel(e);
            base.OnMouseWheel(e);
        }
        //-----------------------------------------------------------------------------
        protected override void OnKeyDown(KeyEventArgs e)
        {
            _winBridge.HandleKeyDown(e);
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            _winBridge.HandleKeyUp(e);
            base.OnKeyUp(e);
        }
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            _winBridge.HandleKeyPress(e);
            return;
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (_winBridge.HandleProcessDialogKey(keyData))
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

    }
}
 