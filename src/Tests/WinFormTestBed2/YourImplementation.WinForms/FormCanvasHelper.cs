//Apache2, 2014-present, WinterDev


using System;
using System.Windows.Forms;

using PixelFarm.Drawing;
using Typography.FontManagement;


namespace LayoutFarm.UI
{
    public static class FormCanvasHelper
    {
        static UIPlatformWinForm s_platform;

        static void InitWinform()
        {
            if (s_platform != null) return;
            //----------------------------------------------------
            s_platform = new UIPlatformWinForm();
        }
        public static Form CreateNewFormCanvas(
           int w, int h,
           InnerViewportKind internalViewportKind,
           out GraphicsViewRoot canvasViewport)
        {
            return CreateNewFormCanvas(0, 0, w, h, internalViewportKind, out canvasViewport);
        }

        public static Form CreateNewFormCanvas(
            int xpos, int ypos,
            int w, int h,
            InnerViewportKind internalViewportKind,
            out GraphicsViewRoot canvasViewport)
        {
            //create new form with new user control
            Form form1 = new Form();

            CreateCanvasControlOnExistingControl(
                form1,
                xpos, ypos, w, h, internalViewportKind,
                out canvasViewport);

            //----------------------
            MakeFormCanvas(form1, canvasViewport);

            GraphicsViewRoot innerViewport = canvasViewport;

            form1.SizeChanged += (s, e) =>
            {
                if (form1.WindowState == FormWindowState.Maximized)
                {
                    Screen currentScreen = GetScreenFromX(form1.Left);
                    //make full screen ?
                    if (innerViewport != null)
                    {
                        var size = Screen.PrimaryScreen.WorkingArea.Size;
                        innerViewport.SetSize(size.Width, size.Height);
                    }
                }
            };
            //----------------------
            return form1;
        }
        public static void MakeFormCanvas(Form form1, GraphicsViewRoot surfaceViewportControl)
        {
            form1.FormClosing += (s, e) =>
            {
                surfaceViewportControl.Close();
            };
        }
        static Screen GetScreenFromX(int xpos)
        {
            Screen[] allScreens = Screen.AllScreens;
            int j = allScreens.Length;
            int accX = 0;
            for (int i = 0; i < j; ++i)
            {
                Screen sc1 = allScreens[i];
                if (accX + sc1.WorkingArea.Width > xpos)
                {
                    return sc1;
                }
            }
            return Screen.PrimaryScreen;
        }

        static AbstractTopWindowBridge GetTopWindowBridge(
            InnerViewportKind innerViewportKind,
            RootGraphic rootgfx,
            LayoutFarm.UI.InputBridge.ITopWindowEventRoot topWindowEventRoot)
        {
            switch (innerViewportKind)
            {
                default: throw new NotSupportedException();
                case InnerViewportKind.GdiPlusOnGLES:
                case InnerViewportKind.AggOnGLES:
                case InnerViewportKind.GLES:
                    return new OpenGL.MyTopWindowBridgeOpenGL(rootgfx, topWindowEventRoot);
                case InnerViewportKind.PureAgg:
                    return new GdiPlus.MyTopWindowBridgeAgg(rootgfx, topWindowEventRoot); //bridge to agg     

                case InnerViewportKind.GdiPlus:

                    return new GdiPlus.MyTopWindowBridgeAgg(rootgfx, topWindowEventRoot); //bridge to agg       
            }
        }



        public static void CreateCanvasControlOnExistingControl(
             Control landingControl,
              int xpos, int ypos,
              int w, int h,
              InnerViewportKind internalViewportKind,
              out GraphicsViewRoot canvasViewport)
        {
            //1. init
            InitWinform();
            IInstalledTypefaceProvider fontLoader = YourImplementation.CommonTextServiceSetup.FontLoader;
            //2. 
            ITextService textService = null;
            switch (internalViewportKind)
            {
                default:
                    //gdi, gdi on gles
                    textService = PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.GetTextService();
                    break;
                case InnerViewportKind.PureAgg:
                case InnerViewportKind.AggOnGLES:
                case InnerViewportKind.GLES:
                    textService = new OpenFontTextService();
                    break;
            }

            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetInstalledTypefaceProvider(fontLoader);
            //--------------------------------------------------------------------------- 
            //3. root graphics

            PixelFarm.Drawing.Rectangle screenClientAreaRect = Conv.ToRect(Screen.PrimaryScreen.WorkingArea);
            w = screenClientAreaRect.Width;
            h = screenClientAreaRect.Height;

            MyRootGraphic myRootGfx = new MyRootGraphic(w, h, textService);

            //4. create event bridge that will bridge from native window event to root graphics
            AbstractTopWindowBridge bridge = GetTopWindowBridge(internalViewportKind, myRootGfx, myRootGfx.TopWinEventPortal);

            //5.
            var actualWinUI = new LayoutFarm.UI.MyWinFormsControl();
            actualWinUI.Size = new System.Drawing.Size(w, h);
            landingControl.Controls.Add(actualWinUI);
            actualWinUI.Init();
            IGpuOpenGLSurfaceView win32WindowWrapper = actualWinUI.CreateWindowWrapper(bridge);

            //5.

            //----------------------------------------------------------- 


            var innerViewport = canvasViewport = new GraphicsViewRoot(
                screenClientAreaRect.Width,
                screenClientAreaRect.Height);

            canvasViewport.InitRootGraphics(
                myRootGfx,
                myRootGfx.TopWinEventPortal,
                internalViewportKind,
                win32WindowWrapper,
                bridge);

            //TODO: review here

            canvasViewport.SetBounds(xpos, ypos,
                    screenClientAreaRect.Width,
                    screenClientAreaRect.Height);

            //new System.Drawing.Rectangle(xpos, ypos,
            //    screenClientAreaRect.Width,
            //    screenClientAreaRect.Height);

            //landingControl.Controls.Add(canvasViewport);
            //
            Form ownerForm = landingControl.FindForm();
            if (ownerForm != null)
            {
                ownerForm.FormClosing += (s, e) =>
                {
                    innerViewport.Close();
                };
            }

        }
    }
}
namespace LayoutFarm.UI
{

    sealed class MyWinFormsControl : UserControl, IGpuOpenGLSurfaceView
    {
        AbstractTopWindowBridge _topWindowBridge;

        GLESContext _myContext;
        UIMouseEventArgs _mouseEventArgs = new UIMouseEventArgs();
        UIKeyEventArgs _keyEventArgs = new UIKeyEventArgs();
        UIPaintEventArgs _paintEventArgs = new UIPaintEventArgs();


        public MyWinFormsControl()
        {
        }
        public void Init()
        {
            _myContext = new GLESContext(this.Handle);
        }
        public IntPtr NativeWindowHwnd => this.Handle;



        internal IGpuOpenGLSurfaceView CreateWindowWrapper(AbstractTopWindowBridge topWindowBridge)
        {
            _topWindowBridge = topWindowBridge;
            _topWindowBridge.BindWindowControl(this);

            return this;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            System.Drawing.Rectangle r = e.ClipRectangle;
            _paintEventArgs.Left = r.Left;
            _paintEventArgs.Top = r.Top;
            _paintEventArgs.Right = r.Right;
            _paintEventArgs.Bottom = r.Bottom;

            _topWindowBridge.PaintToOutputWindow(
                new Rectangle(
                    r.Left,
                    r.Top,
                    r.Width,
                    r.Height));
            base.OnPaint(e);
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (_topWindowBridge.HandleProcessDialogKey((UIKeys)keyData))
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _keyEventArgs.UIEventName = UIEventName.KeyDown;
            _keyEventArgs.SetEventInfo((uint)e.KeyCode, e.Shift, e.Alt, e.Control);
            _topWindowBridge.HandleKeyDown(_keyEventArgs);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            _keyEventArgs.UIEventName = UIEventName.KeyPress;
            _topWindowBridge.HandleKeyPress(_keyEventArgs, e.KeyChar);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            _keyEventArgs.UIEventName = UIEventName.KeyUp;
            _keyEventArgs.SetEventInfo((uint)e.KeyCode, e.Shift, e.Alt, e.Control);
            _topWindowBridge.HandleKeyUp(_keyEventArgs);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_topWindowBridge != null)
            {
                LayoutFarm.UI.UIMouseButtons buttons = UIMouseButtons.Left;
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        buttons = UIMouseButtons.Left;
                        break;
                    case MouseButtons.Middle:
                        buttons = UIMouseButtons.Middle;
                        break;
                    case MouseButtons.Right:
                        buttons = UIMouseButtons.Right;
                        break;
                }
                _mouseEventArgs.UIEventName = UIEventName.MouseDown;
                _mouseEventArgs.SetEventInfo(e.X, e.Y, buttons, e.Clicks, e.Delta);
                _topWindowBridge.HandleMouseDown(_mouseEventArgs);
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_topWindowBridge != null)
            {
                _mouseEventArgs.UIEventName = UIEventName.MouseMove;
                _mouseEventArgs.SetEventInfo(e.X, e.Y, UIMouseButtons.None, e.Clicks, e.Delta);
                _topWindowBridge.HandleMouseMove(_mouseEventArgs);
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_topWindowBridge != null)
            {
                LayoutFarm.UI.UIMouseButtons buttons = UIMouseButtons.Left;
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        buttons = UIMouseButtons.Left;
                        break;
                    case MouseButtons.Middle:
                        buttons = UIMouseButtons.Middle;
                        break;
                    case MouseButtons.Right:
                        buttons = UIMouseButtons.Right;
                        break;
                }
                _mouseEventArgs.UIEventName = UIEventName.MouseUp;
                _mouseEventArgs.SetEventInfo(e.X, e.Y, buttons, e.Clicks, e.Delta);
                _topWindowBridge.HandleMouseUp(_mouseEventArgs);
            }
            base.OnMouseUp(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (_topWindowBridge != null)
            {
                LayoutFarm.UI.UIMouseButtons buttons = UIMouseButtons.Left;
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        buttons = UIMouseButtons.Left;
                        break;
                    case MouseButtons.Middle:
                        buttons = UIMouseButtons.Middle;
                        break;
                    case MouseButtons.Right:
                        buttons = UIMouseButtons.Right;
                        break;
                }
                _mouseEventArgs.UIEventName = UIEventName.Wheel;
                _mouseEventArgs.SetEventInfo(e.X, e.Y, buttons, e.Clicks, e.Delta);
                _topWindowBridge.HandleMouseWheel(_mouseEventArgs);
            }
            base.OnMouseWheel(e);
        }

        public Size GetSize() => new Size(this.Width, this.Height);

        public void MakeCurrent() => _myContext.MakeCurrent();

        public void SwapBuffers() => _myContext.SwapBuffers();

        public void SetSize(int width, int height)
        {
            base.Size = new System.Drawing.Size(width, height);
        }
        public IntPtr GetEglDisplay() => _myContext.GetEglDisplay();

        public IntPtr GetEglSurface() => _myContext.GetEglSurface();
        //---------

    }
    //sealed class MyWinFormsControl : Control
    //{
    //    Win32EventBridge _winBridge;
    //    MyWin32WindowWrapper _myWin32NativeWindow;
    //    bool _overrideWndProc = true;

    //    public MyWinFormsControl()
    //    {
    //        _myWin32NativeWindow = new MyWin32WindowWrapper();
    //        _winBridge = new Win32EventBridge();
    //        _winBridge.SetMainWindowControl(_myWin32NativeWindow);
    //    }
    //    internal MyWin32WindowWrapper CreateWindowWrapper(AbstractTopWindowBridge topWindowBridge)
    //    {
    //        IntPtr handle = this.Handle; //force window creation 
    //        _myWin32NativeWindow.SetTopWinBridge(topWindowBridge);
    //        _myWin32NativeWindow.SetNativeHwnd(handle, false);
    //        return _myWin32NativeWindow;
    //    }
    //    protected override void WndProc(ref Message m)
    //    {
    //        if (_overrideWndProc && _winBridge != null)
    //        {
    //            //if we handle this then return true
    //            if (_winBridge.CustomPanelMsgHandler(m.HWnd, (uint)m.Msg, m.WParam, m.LParam))
    //            {
    //                return;
    //            }
    //        }
    //        base.WndProc(ref m);
    //    }

    //    protected override void OnPaint(PaintEventArgs e)
    //    {
    //        System.Drawing.Rectangle r = e.ClipRectangle;
    //        _winBridge.InvokeOnPaint(r.Left, r.Top, r.Width, r.Height);
    //    }
    //    protected override bool ProcessDialogKey(Keys keyData)
    //    {
    //        _winBridge?.InvokeProcessDialogKey((uint)keyData);
    //        return base.ProcessDialogKey(keyData);
    //    }
    //    protected override void OnMouseDown(MouseEventArgs e)
    //    {
    //        if (_winBridge != null)
    //        {
    //            LayoutFarm.UI.UIMouseButtons bottons = UIMouseButtons.Left;
    //            switch (e.Button)
    //            {
    //                case MouseButtons.Left:
    //                    bottons = UIMouseButtons.Left;
    //                    break;
    //                case MouseButtons.Middle:
    //                    bottons = UIMouseButtons.Middle;
    //                    break;
    //                case MouseButtons.Right:
    //                    bottons = UIMouseButtons.Right;
    //                    break;
    //            }
    //            _winBridge.InvokeOnMouseDown(e.X, e.Y, bottons);
    //        }
    //        base.OnMouseDown(e);
    //    }
    //    protected override void OnMouseMove(MouseEventArgs e)
    //    {
    //        if (_winBridge != null)
    //        {
    //            _winBridge.InvokeOnMouseMove(e.X, e.Y);
    //        }
    //        base.OnMouseMove(e);
    //    }
    //    protected override void OnMouseUp(MouseEventArgs e)
    //    {
    //        if (_winBridge != null)
    //        {
    //            LayoutFarm.UI.UIMouseButtons bottons = UIMouseButtons.Left;
    //            switch (e.Button)
    //            {
    //                case MouseButtons.Left:
    //                    bottons = UIMouseButtons.Left;
    //                    break;
    //                case MouseButtons.Middle:
    //                    bottons = UIMouseButtons.Middle;
    //                    break;
    //                case MouseButtons.Right:
    //                    bottons = UIMouseButtons.Right;
    //                    break;
    //            }
    //            _winBridge.InvokeOnMouseUp(e.X, e.Y, bottons);
    //        }
    //        base.OnMouseUp(e);
    //    }
    //    protected override void OnMouseWheel(MouseEventArgs e)
    //    {
    //        base.OnMouseWheel(e);
    //    }
    //}

}