//Apache2, 2014-present, WinterDev
using System;
using System.Windows.Forms;
namespace LayoutFarm.UI
{

    partial class CpuSurfaceView : UserControl
    {
        //this ui support gdi+ and skia on WinForms

        TopWindowBridgeWinForm _winBridge;
        public CpuSurfaceView()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(CpuGdiPlusSurfaceView_MouseWheel);
        }
        //----------------------------------------------------------------------------
        public void Bind(TopWindowBridgeWinForm winBridge)
        {
            //1. 
            _winBridge = winBridge;
            _winBridge.BindWindowControl(this);
        }
        //----------------------------------------------------------------------------
        protected override void OnPaint(PaintEventArgs e)
        {
            _winBridge.PaintToOutputWindow(e.ClipRectangle.ToRect());
            base.OnPaint(e);
        }

#if DEBUG
        public IdbugOutputWindow IdebugOutputWin
        {
            get { return _winBridge; }
        }
#endif
        protected override void OnSizeChanged(EventArgs e)
        {
            if (_winBridge != null)
            {
                _winBridge.UpdateCanvasViewportSize(this.Width, this.Height);
            }
            base.OnSizeChanged(e);
        }
        //----------------------------------------------------------------------------
       
        
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

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            _winBridge.HandleMouseDown(e);
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            _winBridge.HandleMouseMove(e);
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            _winBridge.HandleMouseUp(e);
            base.OnMouseUp(e);
        } 
        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            _winBridge.HandleMouseWheel(e);
            //not call to base class
        }
        //-----------------------------------------------------------------------------
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            _winBridge.HandleKeyDown(e);
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            _winBridge.HandleKeyUp(e);
            base.OnKeyUp(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            _winBridge.HandleKeyPress(e);
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (_winBridge.HandleProcessDialogKey(keyData))
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        void CpuGdiPlusSurfaceView_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _winBridge.HandleMouseWheel(e);
        }
    }
}
