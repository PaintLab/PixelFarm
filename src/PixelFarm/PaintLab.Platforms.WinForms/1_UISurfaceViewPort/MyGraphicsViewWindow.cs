//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenTK
{
    public sealed class MyGraphicsViewWindow : UserControl
    {
        MyNativeWindow _myNativeWindow;
        Win32EventBridge _winBridge;

        public MyGraphicsViewWindow()
        {

        }

        public void SetGpuSurfaceViewportControl(MyNativeWindow nativeWindow)
        {
            _myNativeWindow = nativeWindow;
        }
        protected override void WndProc(ref Message m)
        {
            _winBridge?.CustomPanelMsgHandler(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
            base.WndProc(ref m);
        }
        /// <summary>Raises the HandleCreated event.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            _myNativeWindow.SetNativeHwnd(this.Handle, false);
            //translator
            _winBridge = new Win32EventBridge();
            _winBridge.SetMainWindowControl(_myNativeWindow);
            base.OnHandleCreated(e);
        }
        public MyNativeWindow SurfaceControl => _myNativeWindow;
    }
}
