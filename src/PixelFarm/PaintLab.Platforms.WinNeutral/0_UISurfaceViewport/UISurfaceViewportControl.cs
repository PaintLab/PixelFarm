//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Forms;
using LayoutFarm.UI.InputBridge;

using PixelFarm.DrawingGL;

namespace LayoutFarm.UI.WinNeutral
{

    public partial class UISurfaceViewportControl : Control
    {
        TopWindowBridgeWinNeutral _winBridge;
        RootGraphic _rootgfx;
        ITopWindowEventRoot _topWinEventRoot;
        InnerViewportKind _innerViewportKind;
        List<Form> _subForms = new List<Form>();


        public UISurfaceViewportControl()
        {
            InitializeComponent();
        }

        public void InvokePaint(PaintEventArgs e)
        {
            if (_winBridge != null)
            {
                _winBridge.PaintToOutputWindow();
            }
        }
        public void Close()
        {
            if (_rootgfx != null)
            {
                _rootgfx.CloseWinRoot();
                _rootgfx = null;
            }
            if (_winBridge != null)
            {
                _winBridge.Close();
                _winBridge = null;
            }
        }
        public RootGraphic RootGfx => _rootgfx;

        public UIPlatform Platform => LayoutFarm.UI.UIPlatformWinNeutral.platform;

        public PixelFarm.Drawing.Size Size
        {
            get => new PixelFarm.Drawing.Size(Width, Height);
            set
            {
                SetSize(value.Width, value.Height);
            }
        }
        //public PixelFarm.Drawing.Rectangle Bounds
        //{
        //    get => new PixelFarm.Drawing.Rectangle(_left, _top, _width, _height);
        //    set
        //    {
        //        _left = value.Left;
        //        _top = value.Top;
        //        _width = value.Width;
        //        _height = value.Height;
        //    }
        //}

        GpuOpenGLSurfaceView _gpuSurfaceViewUserControl;
        GLPainterContext _pcx;
        GLPainter _glPainter;

        //public OpenTK.MyGLControl GetOpenTKControl()
        //{
        //    return _gpuSurfaceViewUserControl;
        //}
        public GLPainter GetGLPainter() => _glPainter;
        public GLPainterContext GetGLRenderSurface() => _pcx;

        //TODO: check this
        //PixelFarm.Drawing.DrawBoard CreateSoftwareDrawBoard(int width, int height, InnerViewportKind innerViewportKind)
        //{ 
        //    PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface gdiRenderSurface = new PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface(width, height);
        //    var drawBoard = new PixelFarm.Drawing.WinGdi.GdiPlusDrawBoard(gdiRenderSurface);
        //    drawBoard.CurrentFont = new PixelFarm.Drawing.RequestFont("Tahoma", 10);
        //    return drawBoard;
        //}

        public void InitRootGraphics(
            RootGraphic rootgfx,
            ITopWindowEventRoot topWinEventRoot,
            InnerViewportKind innerViewportKind)
        {
            //1.
            _rootgfx = rootgfx;
            _topWinEventRoot = topWinEventRoot;
            _innerViewportKind = innerViewportKind;

            switch (innerViewportKind)
            {
                case InnerViewportKind.GLES:
                    {

                        ////temp not suppport
                        //PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();
                        var bridge = new OpenGL.MyTopWindowBridgeOpenGL(rootgfx, topWinEventRoot);
                        var view = new GpuOpenGLSurfaceView();
                        view.Width = rootgfx.Width;
                        view.Height = rootgfx.Height;
                        _gpuSurfaceViewUserControl = view;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);
                        _winBridge = bridge;
                        //--------------------------------------- 
                        IntPtr hh1 = view.Handle;
                        view.MakeCurrent();
                        int max = Math.Max(view.Width, view.Height);

                        _pcx = GLPainterContext.Create(max, max, view.Width, view.Height, true);

                        _glPainter = new GLPainter();
                        _glPainter.BindToPainterContext(_pcx);
                        _glPainter.TextPrinter = new GLBitmapGlyphTextPrinter(_glPainter, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);

                        var myGLCanvas1 = new PixelFarm.Drawing.GLES2.MyGLDrawBoard(_glPainter);
                        bridge.SetCanvas(myGLCanvas1);
                    }
                    break;
                //case InnerViewportKind.Skia:
                //    {

                //        //skiasharp ***
                //        var bridge = new Skia.MyTopWindowBridgeSkia(rootgfx, topWinEventRoot);
                //        //var view = new CpuSurfaceView();
                //        //view.Dock = DockStyle.Fill;
                //        //this.Controls.Add(view);
                //        ////--------------------------------------- 
                //        //view.Bind(bridge);
                //        _winBridge = bridge;
                //    }
                //    break;
                case InnerViewportKind.GdiPlus:
                default:
                    {
                        throw new NotSupportedException();
                        //var bridge = new GdiPlus.MyTopWindowBridgeGdiPlus(rootgfx, topWinEventRoot);
                        //var view = new CpuSurfaceView();
                        //view.Dock = DockStyle.Fill;
                        //this.Controls.Add(view);
                        ////--------------------------------------- 
                        //view.Bind(bridge);
                        //this.winBridge = bridge;
                    }
            }
        }


        void InitializeComponent()
        {
        }
        protected override void OnLoad(EventArgs e)
        {
            _winBridge.OnHostControlLoaded();
        }
        public void PaintMe(PixelFarm.DrawingGL.GLPainterContext pcx)
        {
            pcx.DrawLine(0, 0, 100, 100);
            //this.winBridge.PaintToOutputWindow();
        }
        public void PaintMe()
        {
            //this.winBridge.PaintToOutputWindow();
        }
        public void PaintMeFullMode()
        {
            _winBridge.PaintToOutputWindowFullMode();
        }
        public void PrintMe(object targetCanvas)
        {
            //paint to external canvas    
            //var winBridge = (GdiPlus.MyTopWindowBridgeGdiPlus)this.winBridge;
            //if (winBridge != null)
            //{
            //    winBridge.PrintToCanvas(targetCanvas);
            //}
        }
#if DEBUG
        public IdbugOutputWindow IdebugOutputWin
        {
            get { return _winBridge; }
        }
#endif
        public void TopDownRecalculateContent()
        {
            _rootgfx.TopDownRecalculateContent();
        }
        public void AddContent(RenderElement vi)
        {
            _rootgfx.AddChild(vi);
        }

        public void AddContent(RenderElement vi, object owner)
        {
            if (vi is RenderBoxBase)
            {
                if (owner is ITopWindowBox)
                {
                    var topWinBox = owner as ITopWindowBox;
                    if (topWinBox.PlatformWinBox == null)
                    {
                        throw new NotSupportedException();
                        ////create platform winbox 
                        //var newForm = new AbstractCompletionWindow();
                        //newForm.LinkedParentForm = this.FindForm();
                        //newForm.LinkedParentControl = this;
                        //UISurfaceViewportControl newSurfaceViewport = this.CreateNewOne(300, 200);
                        //newSurfaceViewport.Location = new System.Drawing.Point(0, 0);
                        //newForm.Controls.Add(newSurfaceViewport);
                        //vi.ResetRootGraphics(newSurfaceViewport.RootGfx);
                        //vi.SetLocation(0, 0);
                        //newSurfaceViewport.AddContent(vi);
                        ////------------------------------------------------------                       
                        //var platformWinBox = new PlatformWinBoxForm(newForm);
                        //topWinBox.PlatformWinBox = platformWinBox;
                        //platformWinBox.UseRelativeLocationToParent = true;
                        //subForms.Add(newForm);
                    }
                }
                else
                {
                    _rootgfx.AddChild(vi);
                }
            }
            else
            {
                _rootgfx.AddChild(vi);
            }
        }




    }

    class PlatformWinBoxForm : IPlatformWindowBox
    {
        AbstractCompletionWindow _form;
        public PlatformWinBoxForm(AbstractCompletionWindow form)
        {
            _form = form;
        }
        public bool UseRelativeLocationToParent
        {
            get;
            set;
        }
        bool IPlatformWindowBox.Visible
        {
            get
            {
                return _form.Visible;
            }
            set
            {
                if (value)
                {
                    if (!_form.Visible)
                    {
                        _form.ShowForm();
                    }
                }
                else
                {
                    if (_form.Visible)
                    {
                        _form.Hide();
                    }
                }
            }
        }
        void IPlatformWindowBox.Close()
        {
            _form.Close();
            _form = null;
        }

        void IPlatformWindowBox.SetLocation(int x, int y)
        {
            //if (this.UseRelativeLocationToParent)
            //{
            //    //1. find parent form/control 
            //    var parentLoca = form.LinkedParentForm.Location;
            //    form.Location = new System.Drawing.Point(parentLoca.X + x, parentLoca.Y + y);
            //}
            //else
            //{
            //    form.Location = new System.Drawing.Point(x, y);
            //}
        }

        void IPlatformWindowBox.SetSize(int w, int h)
        {
            //form.Size = new System.Drawing.Size(w, h);
        }
    }
}
