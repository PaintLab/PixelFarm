//MIT, 2016-present, WinterDev
using System;
using System.Runtime.InteropServices;

using OpenTK.Graphics.ES20;
using PixelFarm;
using PixelFarm.Forms;
using PixelFarm.Drawing;

using Typography.FontManagement;
using LayoutFarm.UI;

using Glfw;

namespace TestGlfw
{
    class MyApp3
    {
        static void Init()
        {
            ////1. init
            //InitWinform();
            //IInstalledTypefaceProvider fontLoader = YourImplementation.CommonTextServiceSetup.FontLoader;
            ////2. 
            ITextService textService = PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.GetTextService();


            //PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetInstalledTypefaceProvider(fontLoader);
            //---------------------------------------------------------------------------
            int w = 800;
            int h = 600;
            MyRootGraphic myRootGfx = new MyRootGraphic(w, h, textService);
            GraphicsViewRoot canvasViewport = null;
            //---------------------------------------------------------------------------
            //PixelFarm.Drawing.Rectangle screenClientAreaRect = Conv.ToRect(Screen.PrimaryScreen.WorkingArea);
            var innerViewport = canvasViewport = new GraphicsViewRoot(
                w,
                h);

            //MyWin32WindowWrapper myNativeWindow = new MyWin32WindowWrapper();
            //var winBridge = new Win32EventBridge();
            //winBridge.SetMainWindowControl(myNativeWindow);

            //var acutualWinUI = new MyGraphicsViewWindow();
            //acutualWinUI.Size = new System.Drawing.Size(w, h);
            ////
            //IntPtr handle = acutualWinUI.Handle; //force window creation ?
            //acutualWinUI.SetWin32EventBridge(winBridge);

            GlfwEventBridge bridge = new GlfwEventBridge();


            //canvasViewport.InitRootGraphics(
            //    myRootGfx,
            //    myRootGfx.TopWinEventPortal,
            //    InnerViewportKind.GLES,
            //    myNativeWindow);

        }
        public static void Start()
        {

            GlfwWindowEventListener winEventListener = new GlfwWindowEventListener();
            var form = new GlFwForm(800, 600, "hello!", winEventListener);
            form.MakeCurrent();

            //----------
            //(test) use gles2.1
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CLIENT_API, Glfw.Glfw3.GLFW_OPENGL_ES_API);
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CONTEXT_CREATION_API, Glfw.Glfw3.GLFW_EGL_CONTEXT_API);
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CONTEXT_VERSION_MAJOR, 2);
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CONTEXT_VERSION_MINOR, 1);
            Glfw.Glfw3.glfwSwapInterval(1);
            //----------


            string versionStr3 = Marshal.PtrToStringAnsi(Glfw3.glfwGetVersionString());


            OpenTK.Platform.Factory.GetCustomPlatformFactory = () => OpenTK.Platform.Egl.EglAngle.NewFactory();
            OpenTK.Toolkit.Init(new OpenTK.ToolkitOptions
            {
                Backend = OpenTK.PlatformBackend.PreferNative,
            });
            OpenTK.Graphics.PlatformAddressPortal.GetAddressDelegate = OpenTK.Platform.Utilities.CreateGetAddress();

            //-----------
            IntPtr currentContext = Glfw3.glfwGetCurrentContext();
            var contextHandler = new OpenTK.ContextHandle(currentContext);

            var glfwContext = new GLFWContextForOpenTK(contextHandler);
            var context = OpenTK.Graphics.GraphicsContext.CreateExternalContext(glfwContext);


            bool isCurrent = context.IsCurrent;
            var demoContext = new Mini.GLDemoContext(800, 600);
            demoContext.LoadDemo(new OpenTkEssTest.T108_LionFill());

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //--------------------------------------------------------------------------------
            //setup viewport size
            //set up canvas
            int ww_w = 800;
            int ww_h = 600;
            int max = Math.Max(ww_w, ww_h);
            GL.Viewport(0, 0, max, max);

            //---------
            form.RenderDel = demoContext.Render;
            //---------

            GlfwAppLoop.Run();
        }
    }
}