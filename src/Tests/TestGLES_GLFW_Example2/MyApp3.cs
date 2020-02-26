//MIT, 2016-present, WinterDev
using System;
using System.Runtime.InteropServices;

using OpenTK.Graphics.ES20;
using PixelFarm;
using PixelFarm.Forms;
using PixelFarm.Drawing;

using Typography.FontManagement;
using LayoutFarm;
using LayoutFarm.UI;
using PaintLab.Svg;

using Glfw;


namespace TestGlfw
{

    class GlfwWindowWrapper : IGpuOpenGLSurfaceView
    {
        GlFwForm _form;
        public GlfwWindowWrapper(GlFwForm form)
        {
            _form = form;
        }

        public IntPtr NativeWindowHwnd => _form.Handle;

        public int Width => _form.Width;

        public int Height => _form.Height;

        public void Dispose()
        {
        }

        public IntPtr GetEglDisplay()
        {

            return IntPtr.Zero;
        }

        public IntPtr GetEglSurface()
        {
            return IntPtr.Zero;
        }

        public Size GetSize() => new Size(_form.Width, _form.Height);

        public void Invalidate()
        {
            _form.Invalidate();
        }

        public void MakeCurrent()
        {
            _form.MakeCurrent();
        }

        public void SwapBuffers()
        {
            _form.SwapBuffers();
        }
        public void Refresh()
        {
            //???
        }
        public void SetBounds(int left, int top, int width, int height)
        {
            _form.SetBounds(left, top, width, height);
        }

        public void SetSize(int width, int height)
        {
            _form.SetSize(width, height);
        }

    }

    class MyBoxUI : UIElement
    {
        RenderElement _renderElem;
        public MyBoxUI()
        {
        }

        public void SetRenderElement(RenderElement renderE)
        {
            _renderElem = renderE;
        }

        public override RenderElement CurrentPrimaryRenderElement => _renderElem;

        public override void Accept(UIVisitor visitor)
        {
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx) => _renderElem;

        public override void InvalidateGraphics() => _renderElem.InvalidateGraphics();



        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (_isMouseDown)
            {
                //dragging
                Point p1 = _renderElem.Location;
                _renderElem.SetLocation(p1.X + 2, p1.Y);
            }

            base.OnMouseMove(e);
        }
        bool _isMouseDown;
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            _isMouseDown = true;
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            _isMouseDown = false;
            base.OnMouseUp(e);
        }

    }
    class MySprite : RenderElement
    {
        VgVisualElement _renderVx;
        public MySprite(RootGraphic root, int w, int h) : base(root, w, h)
        {
            _renderVx = VgVisualDocHelper.CreateVgVisualDocFromFile(@"lion.svg").VgRootElem;
        }
        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
            //d.SetCanvasOrigin(0, 0);
            //d.SetClipRect(new PixelFarm.Drawing.Rectangle(0, 0, 100, 100));
            //d.FillRectangle(PixelFarm.Drawing.Color.Red, 0, 0, this.Width, this.Height);
            Painter p = d.GetPainter();
            using (VgPaintArgsPool.Borrow(p, out var paintArgs))
            {
                _renderVx.Paint(paintArgs);
            }
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {

        }
    }

    class MyApp3
    {
        static MyBoxUI s_box;
        static MyRootGraphic s_myRootGfx;
        static GraphicsViewRoot s_viewroot;

        static void Init(GlFwForm form)
        {
            GLESInit.InitGLES();

            ////1. init
            //InitWinform();
            //IInstalledTypefaceProvider fontLoader = YourImplementation.CommonTextServiceSetup.FontLoader;
            ////2. 
            ITextService textService = PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.GetTextService();
            //PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetInstalledTypefaceProvider(fontLoader);
            //---------------------------------------------------------------------------
            int w = 800;
            int h = 600;
            textService = null;
            s_myRootGfx = new MyRootGraphic(w, h, textService);
            //---------------------------------------------------------------------------
            //PixelFarm.Drawing.Rectangle screenClientAreaRect = Conv.ToRect(Screen.PrimaryScreen.WorkingArea);


            s_viewroot = new GraphicsViewRoot(w, h);
            MyGlfwTopWindowBridge bridge1 = new MyGlfwTopWindowBridge(s_myRootGfx, s_myRootGfx.TopWinEventPortal);
            ((MyGlfwTopWindowBridge.GlfwEventBridge)(form.WindowEventListener)).SetWindowBridge(bridge1);


            var glfwWindowWrapper = new GlfwWindowWrapper(form);
            bridge1.BindWindowControl(glfwWindowWrapper);

            s_viewroot.InitRootGraphics(s_myRootGfx,
                  s_myRootGfx.TopWinEventPortal,
                  InnerViewportKind.GLES,
                  glfwWindowWrapper,
                  bridge1);



            MySprite sprite = new MySprite(s_myRootGfx, 200, 300);
            MyBoxUI boxUI = new MyBoxUI();
            boxUI.SetRenderElement(sprite);
            sprite.SetController(boxUI);


            s_myRootGfx.AddChild(sprite);
            boxUI.InvalidateGraphics();

            //MyWin32WindowWrapper myNativeWindow = new MyWin32WindowWrapper();
            //var winBridge = new Win32EventBridge();
            //winBridge.SetMainWindowControl(myNativeWindow);

            //var acutualWinUI = new MyGraphicsViewWindow();
            //acutualWinUI.Size = new System.Drawing.Size(w, h);
            ////
            //IntPtr handle = acutualWinUI.Handle; //force window creation ?
            //acutualWinUI.SetWin32EventBridge(winBridge);




            //canvasViewport.InitRootGraphics(
            //    myRootGfx,
            //    myRootGfx.TopWinEventPortal,
            //    InnerViewportKind.GLES,
            //    myNativeWindow);

        }
        public static void Start()
        {
            //bridge from native side to managed side
            var bridge = new MyGlfwTopWindowBridge.GlfwEventBridge();
            var form = new GlFwForm(800, 600, "hello!", bridge);
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


            IntPtr currentContext = Glfw3.glfwGetCurrentContext();
            var contextHandler = new OpenTK.ContextHandle(currentContext);

            //------
            Init(form);
            //------

            var glfwContext = new GLFWContextForOpenTK(contextHandler);
            var context = OpenTK.Graphics.GraphicsContext.CreateExternalContext(glfwContext);



            bool isCurrent = context.IsCurrent;
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
            //form.RenderDel = s_viewroot.PaintMe;
            //---------

            GlfwAppLoop.Run();
        }
    }
}