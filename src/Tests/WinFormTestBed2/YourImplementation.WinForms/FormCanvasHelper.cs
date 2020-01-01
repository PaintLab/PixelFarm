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
            MyRootGraphic myRootGfx = new MyRootGraphic(w, h, textService);

            //4. create event bridge that will bridge from native window event to root graphics
            AbstractTopWindowBridge bridge = GetTopWindowBridge(internalViewportKind, myRootGfx, myRootGfx.TopWinEventPortal);

            //5.
            var actualWinUI = new LayoutFarm.UI.MyWinFormsControl();
            actualWinUI.Size = new System.Drawing.Size(w, h);
            landingControl.Controls.Add(actualWinUI); 
            MyWin32WindowWrapper win32WindowWrapper = actualWinUI.CreateWindowWrapper(bridge);

            //5.
            
            //----------------------------------------------------------- 
            PixelFarm.Drawing.Rectangle screenClientAreaRect = Conv.ToRect(Screen.PrimaryScreen.WorkingArea);

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

            //canvasViewport.SetBounds(xpos, ypos,
            //        screenClientAreaRect.Width,
            //        screenClientAreaRect.Height);

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

    sealed class MyWinFormsControl : Control
    {
        Win32EventBridge _winBridge;
        MyWin32WindowWrapper _myWin32NativeWindow;
        public MyWinFormsControl()
        {
            _myWin32NativeWindow = new MyWin32WindowWrapper();
            _winBridge = new Win32EventBridge();
            _winBridge.SetMainWindowControl(_myWin32NativeWindow);
        }
        internal MyWin32WindowWrapper CreateWindowWrapper(AbstractTopWindowBridge topWindowBridge)
        {
            IntPtr handle = this.Handle; //force window creation 
            _myWin32NativeWindow.SetTopWinBridge(topWindowBridge);
            _myWin32NativeWindow.SetNativeHwnd(handle, false);
            return _myWin32NativeWindow;
        }
        protected override void WndProc(ref Message m)
        {
            _winBridge?.CustomPanelMsgHandler(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
            base.WndProc(ref m);
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            _winBridge?.SendProcessDialogKey((uint)keyData);
            return base.ProcessDialogKey(keyData);
        }
    }

}