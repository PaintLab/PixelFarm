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

            //4. graphics view root: host  
            AbstractTopWindowBridge bridge = GetTopWindowBridge(internalViewportKind, myRootGfx, myRootGfx.TopWinEventPortal);

            var myNativeWindow = new MyWin32WindowWrapper();
            var win32EvBridge = new Win32EventBridge();
            win32EvBridge.SetMainWindowControl(myNativeWindow);

            var actualWinUI = new LayoutFarm.UI.MyWinFormsControl();
            actualWinUI.Size = new System.Drawing.Size(w, h);
            //
            IntPtr handle = actualWinUI.Handle; //force window creation ? 
            actualWinUI.SetWin32EventBridge(win32EvBridge);


            myNativeWindow.SetTopWinBridge(bridge);
            myNativeWindow.SetNativeHwnd(actualWinUI.Handle, false);
            //----------------------------------------------------------- 
            PixelFarm.Drawing.Rectangle screenClientAreaRect = Conv.ToRect(Screen.PrimaryScreen.WorkingArea);

            var innerViewport = canvasViewport = new GraphicsViewRoot(
                screenClientAreaRect.Width,
                screenClientAreaRect.Height);

            canvasViewport.InitRootGraphics(
                myRootGfx,
                myRootGfx.TopWinEventPortal,
                internalViewportKind,
                myNativeWindow,
                bridge);

            landingControl.Controls.Add(actualWinUI);

            //IntPtr prevHandle = Win32.MyWin32.SetParent(actualWinUI.Handle, landingControl.Handle);


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
        public MyWinFormsControl()
        {

        }
        public void SetWin32EventBridge(Win32EventBridge winBridge)
        {
            _winBridge = winBridge;
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