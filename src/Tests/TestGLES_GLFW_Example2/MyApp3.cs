//MIT, 2016-present, WinterDev
using System;
using System.Runtime.InteropServices;

using OpenTK.Graphics.ES20;
using PixelFarm;
using PixelFarm.Forms;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;

using LayoutFarm;
using LayoutFarm.UI;
using PaintLab.Svg;

using Glfw;
using LayoutFarm.CustomWidgets;
using PixelFarm.DrawingGL;

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

        public Cursor CurrentCursor
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

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

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx) => _renderElem;

        public override void InvalidateGraphics() => _renderElem.InvalidateGraphics();
        protected override void OnMouseMove(UIMouseMoveEventArgs e)
        {
            if (_isMouseDown)
            {
                //dragging

                _renderElem.SetLocation(_renderElem.X + e.XDiff, _renderElem.Y + e.YDiff);
            }

            base.OnMouseMove(e);
        }
        bool _isMouseDown;
        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            _isMouseDown = true;
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(UIMouseUpEventArgs e)
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
             
            using (Tools.More.BorrowVgPaintArgs(d.GetPainter(), out var paintArgs))
            {
                _renderVx.Paint(paintArgs);
            }
            //d.FillRectangle(Color.Blue, 0, 0, 50, 50);
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {

        }
    }

    class MyApp3
    {


        public static int s_formW = 1024;
        public static int s_formH = 1024;

        static void Init(GlFwForm form)
        {
            GLESInit.InitGLES();


            string icu_datadir = "brkitr"; //see brkitr folder, we link data from Typography project and copy to output if newer
            if (!System.IO.Directory.Exists(icu_datadir))
            {
                throw new System.NotSupportedException("dic");
            }
            var dicProvider = new Typography.TextBreak.IcuSimpleTextFileDictionaryProvider() { DataDir = icu_datadir };
            Typography.TextBreak.CustomBreakerBuilder.Setup(dicProvider);

            PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new PixelFarm.Drawing.WinGdi.GdiBitmapIO();

            PixelFarm.Platforms.StorageService.RegisterProvider(new YourImplementation.LocalFileStorageProvider(""));

            OpenFontTextService textService = new OpenFontTextService();

            textService.LoadFontsFromFolder("Fonts");

            //---------------------------------------------------------------------------

            var s_myRootGfx = new MyRootGraphic(s_formW, s_formH, textService);
            //---------------------------------------------------------------------------


            var s_viewroot = new GraphicsViewRoot(s_formW, s_formH);
            MyGlfwTopWindowBridge bridge1 = new MyGlfwTopWindowBridge(s_myRootGfx, s_myRootGfx.TopWinEventPortal);
            ((MyGlfwTopWindowBridge.GlfwEventBridge)(form.WindowEventListener)).SetWindowBridge(bridge1);


            var glfwWindowWrapper = new GlfwWindowWrapper(form);
            bridge1.BindWindowControl(glfwWindowWrapper);

            s_viewroot.InitRootGraphics(s_myRootGfx,
                  s_myRootGfx.TopWinEventPortal,
                  InnerViewportKind.GLES,
                  glfwWindowWrapper,
                  bridge1);
            if (s_viewroot.GetGLPainter() is GLPainter glPainter)
            {
                glPainter.SmoothingMode = SmoothingMode.AntiAlias;
            }


            //----------------------
            Box bgBox = new Box(s_formW, s_formH);
            bgBox.BackColor = Color.White;

            s_myRootGfx.AddChild(bgBox.GetPrimaryRenderElement(s_myRootGfx));

            //----------------------
            MySprite sprite = new MySprite(s_myRootGfx, 200, 300);
            MyBoxUI boxUI = new MyBoxUI();
            boxUI.SetRenderElement(sprite);
            sprite.SetController(boxUI);

            bgBox.Add(boxUI);

            //bgBox.InvalidateGraphics();
        }
        public static void Start()
        {



            string versionStr3 = Marshal.PtrToStringAnsi(Glfw.Glfw3.glfwGetVersionString());

            var bridge = new MyGlfwTopWindowBridge.GlfwEventBridge();



            var form = new GlFwForm(s_formW, s_formH, "hello!", bridge);
            form.MakeCurrent();

            OpenTK.Platform.Factory.GetCustomPlatformFactory = () => OpenTK.Platform.Egl.EglAngle.NewFactory();
            OpenTK.Toolkit.Init(new OpenTK.ToolkitOptions {
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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            //--------------------------------------------------------------------------------
            //setup viewport size
            //set up canvas
            int ww_w = s_formW;
            int ww_h = s_formH;
            int max = Math.Max(ww_w, ww_h);
            GL.Viewport(0, 0, max, max);


            //---------
            //form.RenderDel = s_viewroot.PaintMe;
            //---------  
            GlfwAppLoop.Run();
        }
    }
}