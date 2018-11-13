//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;


#if GL_ENABLE
using PixelFarm.DrawingGL;
#endif


namespace LayoutFarm.UI
{
    public partial class UISurfaceViewportControl : UserControl
    {
        TopWindowBridgeWinForm winBridge;
        RootGraphic rootgfx;
        ITopWindowEventRoot topWinEventRoot;
        InnerViewportKind innerViewportKind;
        List<Form> subForms = new List<Form>();
        public UISurfaceViewportControl()
        {
            InitializeComponent();

            //this.panel1.Visible = false; 
        }
        public InnerViewportKind InnerViewportKind => innerViewportKind;


#if DEBUG
        static int s_dbugCount;
#endif
        //protected override void OnGotFocus(EventArgs e)
        //{
        //    s_dbugCount++;
        //    Console.WriteLine("focus" + s_dbugCount.ToString());
        //    base.OnGotFocus(e);
        //}
        protected override void OnVisibleChanged(EventArgs e)
        {
            //s_dbugCount++;
            //Console.WriteLine("focus" + s_dbugCount.ToString());
            rootgfx.InvalidateRectArea(new PixelFarm.Drawing.Rectangle(0, 0, rootgfx.Width, rootgfx.Height));
            rootgfx.FlushAccumGraphics();
            //#if DEBUG
            //            s_dbugCount++;
            //            Console.WriteLine("vis" + s_dbugCount.ToString());
            //#endif
            base.OnVisibleChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //on Windows request paint at specific area
            if (e.ClipRectangle.Width + e.ClipRectangle.Height == 0)
            {
                //entire window
                rootgfx.InvalidateRectArea(new PixelFarm.Drawing.Rectangle(0, 0, rootgfx.Width, rootgfx.Height));
            }
            else
            {
                rootgfx.InvalidateRectArea(
                    new PixelFarm.Drawing.Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height));
            }
            rootgfx.FlushAccumGraphics();
            //#if DEBUG
            //            s_dbugCount++;
            //            Console.WriteLine("p" + s_dbugCount.ToString() + e.ClipRectangle);
            //#endif
            base.OnPaint(e);
        }
        public UIPlatform Platform
        {
            get
            {
                return UIPlatformWinForm.GetDefault();
            }
        }

#if GL_ENABLE

        IntPtr hh1;
        OpenGL.GpuOpenGLSurfaceView _gpuSurfaceViewUserControl;
        GLRenderSurface _glsx;
        GLPainter _glPainter;

        public OpenTK.MyGLControl GetOpenTKControl()
        {
            return _gpuSurfaceViewUserControl;
        }
        public GLPainter GetGLPainter()
        {
            return _glPainter;
        }
        public GLRenderSurface GetGLRenderSurface()
        {
            return _glsx;
        }
#endif


        public void InitRootGraphics(
            RootGraphic rootgfx,
            ITopWindowEventRoot topWinEventRoot,
            InnerViewportKind innerViewportKind)
        {

            //create a proper bridge****

            this.rootgfx = rootgfx;
            this.topWinEventRoot = topWinEventRoot;
            this.innerViewportKind = innerViewportKind;
            switch (innerViewportKind)
            {
#if GL_ENABLE
                case InnerViewportKind.GdiPlusOnGLES:
                    {
                        //similar to agg on GLES
                        var bridge = new OpenGL.MyTopWindowBridgeOpenGL(rootgfx, topWinEventRoot);
                        var view = new OpenGL.GpuOpenGLSurfaceView();
                        view.Width = 1200;
                        view.Height = 1200;
                        _gpuSurfaceViewUserControl = view;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);
                        this.winBridge = bridge;
                        //--------------------------------------- 
                        view.SetGLPaintHandler(null);
                        hh1 = view.Handle; //force real window handle creation
                        view.MakeCurrent();


                        int max = Math.Max(view.Width, view.Height);
                        _glsx = PixelFarm.Drawing.GLES2.GLES2Platform.CreateGLRenderSurface(max, max, view.Width, view.Height);
                        //---------------
                        //canvas2d.FlipY = true;//
                        //---------------
                        _glPainter = new GLPainter(_glsx);

                        //canvasPainter.SmoothingMode = PixelFarm.Drawing.SmoothingMode.HighQuality;
                        //----------------------
                        //1. win gdi based
                        //var printer = new WinGdiFontPrinter(canvas2d, view.Width, view.Height);
                        //canvasPainter.TextPrinter = printer;
                        //----------------------
                        //2. raw vxs
                        //var printer = new PixelFarm.Drawing.Fonts.VxsTextPrinter(canvasPainter);
                        //canvasPainter.TextPrinter = printer;
                        //----------------------
                        //3. agg texture based font texture
                        //var printer = new AggTextSpanPrinter(canvasPainter, 400, 50);
                        //printer.HintTechnique = Typography.Rendering.HintTechnique.TrueTypeInstruction_VerticalOnly;
                        //printer.UseSubPixelRendering = true;
                        //canvasPainter.TextPrinter = printer; 
                        //3 
                        var printer = new GLBitmapGlyphTextPrinter(_glPainter, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);
                        _glPainter.TextPrinter = printer;

                        //
                        var myGLCanvas1 = new PixelFarm.Drawing.GLES2.MyGLDrawBoard(_glPainter, _glsx.CanvasWidth, _glsx.CanvasHeight);
                        bridge.SetCanvas(myGLCanvas1);


                    }
                    break;
                case InnerViewportKind.AggOnGLES:
                case InnerViewportKind.GLES:
                    {

                        //temp not suppport  


                        var bridge = new OpenGL.MyTopWindowBridgeOpenGL(rootgfx, topWinEventRoot);
                        var view = new OpenGL.GpuOpenGLSurfaceView();
                        view.Width = 1200;
                        view.Height = 1200;
                        _gpuSurfaceViewUserControl = view;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);
                        this.winBridge = bridge;
                        //--------------------------------------- 
                        view.SetGLPaintHandler(null);
                        hh1 = view.Handle; //force real window handle creation
                        view.MakeCurrent();


                        int max = Math.Max(view.Width, view.Height);
                        _glsx = PixelFarm.Drawing.GLES2.GLES2Platform.CreateGLRenderSurface(max, max, view.Width, view.Height);
                        //---------------
                        //canvas2d.FlipY = true;//
                        //---------------
                        _glPainter = new GLPainter(_glsx);

                        //canvasPainter.SmoothingMode = PixelFarm.Drawing.SmoothingMode.HighQuality;
                        //----------------------
                        //1. win gdi based
                        //var printer = new WinGdiFontPrinter(canvas2d, view.Width, view.Height);
                        //canvasPainter.TextPrinter = printer;
                        //----------------------
                        //2. raw vxs
                        //var printer = new PixelFarm.Drawing.Fonts.VxsTextPrinter(canvasPainter);
                        //canvasPainter.TextPrinter = printer;
                        //----------------------
                        //3. agg texture based font texture
                        //var printer = new AggTextSpanPrinter(canvasPainter, 400, 50);
                        //printer.HintTechnique = Typography.Rendering.HintTechnique.TrueTypeInstruction_VerticalOnly;
                        //printer.UseSubPixelRendering = true;
                        //canvasPainter.TextPrinter = printer; 
                        //3 
                        var printer = new GLBitmapGlyphTextPrinter(_glPainter, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);
                        _glPainter.TextPrinter = printer;

                        //
                        var myGLCanvas1 = new PixelFarm.Drawing.GLES2.MyGLDrawBoard(_glPainter, _glsx.CanvasWidth, _glsx.CanvasHeight);
                        bridge.SetCanvas(myGLCanvas1);

                    }
                    break;
#endif
#if __SKIA__
                case InnerViewportKind.Skia:
                    {
                        //skiasharp ***

                        var bridge = new Skia.MyTopWindowBridgeSkia(rootgfx, topWinEventRoot);
                        var view = new CpuSurfaceView();
                        view.Dock = DockStyle.Fill;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);
                        this.winBridge = bridge;

                    }
                    break;
#endif
                case InnerViewportKind.PureAgg:
                    {
                        var bridge = new GdiPlus.MyTopWindowBridgeAgg(rootgfx, topWinEventRoot);
                        var view = new CpuSurfaceView();
                        view.Dock = DockStyle.Fill;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);
                        this.winBridge = bridge;
                    }
                    break;

                case InnerViewportKind.GdiPlus:
                default:
                    {
                        var bridge = new GdiPlus.MyTopWindowBridgeGdiPlus(rootgfx, topWinEventRoot);
                        var view = new CpuSurfaceView();
                        view.Dock = DockStyle.Fill;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);
                        this.winBridge = bridge;
                    }
                    break;
            }
        }
        void InitializeComponent()
        {
            //this.panel1 = new System.Windows.Forms.Panel();
            //this.SuspendLayout();
            //// 
            //// panel1
            //// 
            //this.panel1.BackColor = System.Drawing.Color.Gray;
            //this.panel1.Location = new System.Drawing.Point(4, 4);
            //this.panel1.Name = "panel1";
            //this.panel1.Size = new System.Drawing.Size(851, 753);
            //this.panel1.TabIndex = 0;
            // 
            // UISurfaceViewportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            //this.Controls.Add(this.panel1);
#if DEBUG
            this.Name = "UISurfaceViewportControl";
#endif
            this.Size = new System.Drawing.Size(863, 760);
            this.ResumeLayout(false);

        }
        protected override void OnLoad(EventArgs e)
        {
            this.winBridge.OnHostControlLoaded();
        }
        public void PaintMe()
        {
            this.winBridge.PaintToOutputWindow();
        }
        public void PaintToPixelBuffer(IntPtr outputPixelBuffer)
        {
            winBridge.CopyOutputPixelBuffer(0, 0, this.Width, this.Height, outputPixelBuffer);
        }

#if DEBUG
        public void dbugPaintMeFullMode()
        {
            this.winBridge.dbugPaintToOutputWindowFullMode();
        }
        public IdbugOutputWindow IdebugOutputWin
        {
            get { return this.winBridge; }
        }
#endif
        public void TopDownRecalculateContent()
        {
            this.rootgfx.TopWindowRenderBox.TopDownReCalculateContentSize();
        }
        public void AddChild(RenderElement vi)
        {
            this.rootgfx.TopWindowRenderBox.AddChild(vi);
        }
        static IntPtr s_tmpHandle;

        public void AddChild(RenderElement renderElem, object owner)
        {
            if (renderElem is RenderBoxBase)
            {
                if (owner is ITopWindowBox)
                {
                    var topWinBox = owner as ITopWindowBox;
                    if (topWinBox.PlatformWinBox == null)
                    {

                        FormPopupShadow popupShadow1 = new FormPopupShadow();
                        popupShadow1.Visible = false;
                        IntPtr handle1 = popupShadow1.Handle; //***


                        //create platform winbox 
                        var newForm = new AbstractCompletionWindow();
                        newForm.LinkedParentForm = this.FindForm();
                        newForm.LinkedParentControl = this;
                        newForm.PopupShadow = popupShadow1;

                        //TODO: review here=> 300,200

                        UISurfaceViewportControl newSurfaceViewport = this.CreateNewOne(300, 200);
                        newSurfaceViewport.Location = new System.Drawing.Point(0, 0);
                        newForm.Controls.Add(newSurfaceViewport);
                        renderElem.ResetRootGraphics(newSurfaceViewport.RootGfx);
                        renderElem.SetLocation(0, 0);
                        newSurfaceViewport.AddChild(renderElem);
                        //-----------------------------------------------------                        
                        s_tmpHandle = newForm.Handle;//force newform to create window handle

                        //-----------------------------------------------------              

                        var platformWinBox = new PlatformWinBoxForm(newForm);
                        topWinBox.PlatformWinBox = platformWinBox;
                        platformWinBox.UseRelativeLocationToParent = true;
                        platformWinBox.PreviewVisibilityChanged += PlatformWinBox_PreviewVisibilityChanged;
                        platformWinBox.PreviewBoundChanged += PlatformWinBox_PreviewBoundChanged;
                        platformWinBox.BoundsChanged += PlatformWinBox_BoundsChanged;
                        platformWinBox.VisibityChanged += PlatformWinBox_VisibityChanged;
                        subForms.Add(newForm);
                        s_tmpHandle = IntPtr.Zero;

                    }
                }
                else
                {
                    this.rootgfx.TopWindowRenderBox.AddChild(renderElem);
                }
            }
            else
            {
                this.rootgfx.TopWindowRenderBox.AddChild(renderElem);
            }
        }


        PixelFarm.Drawing.Rectangle _winBoxAccumInvalidateArea;
        bool _hasInvalidateAreaAccum;

        void UpdateInvalidateAccumArea(PlatformWinBoxForm winform)
        {
            winform.GetLocalBoundsIncludeShadow(out int x, out int y, out int w, out int h);
            if (_hasInvalidateAreaAccum)
            {
                _winBoxAccumInvalidateArea = PixelFarm.Drawing.Rectangle.Union(
                    new PixelFarm.Drawing.Rectangle(x, y, w, h),
                    _winBoxAccumInvalidateArea);
            }
            else
            {
                _winBoxAccumInvalidateArea =
                    new PixelFarm.Drawing.Rectangle(x, y, w, h);

                _hasInvalidateAreaAccum = true;
            }
        }
        private void PlatformWinBox_PreviewBoundChanged(object sender, EventArgs e)
        {
            UpdateInvalidateAccumArea((PlatformWinBoxForm)sender);
        }
        private void PlatformWinBox_PreviewVisibilityChanged(object sender, EventArgs e)
        {
            UpdateInvalidateAccumArea((PlatformWinBoxForm)sender);
        }

        private void PlatformWinBox_VisibityChanged(object sender, EventArgs e)
        {
            UpdateInvalidateAccumArea((PlatformWinBoxForm)sender);


            rootgfx.InvalidateRectArea(_winBoxAccumInvalidateArea);
            _hasInvalidateAreaAccum = false;
        }

        private void PlatformWinBox_BoundsChanged(object sender, EventArgs e)
        {
            UpdateInvalidateAccumArea((PlatformWinBoxForm)sender);
            rootgfx.InvalidateRectArea(_winBoxAccumInvalidateArea);
            _hasInvalidateAreaAccum = false;
        }

        public RootGraphic RootGfx
        {
            get
            {
                return this.rootgfx;
            }
        }
        public void Close()
        {
            this.winBridge.Close();
        }


        /// <summary>
        /// create new UIViewport based on this control's current platform
        /// </summary>
        /// <returns></returns>
        public UISurfaceViewportControl CreateNewOne(int w, int h)
        {
            //each viewport has its own root graphics 

            UISurfaceViewportControl newViewportControl = new UISurfaceViewportControl();
            newViewportControl.Size = new System.Drawing.Size(w, h);
            RootGraphic newRootGraphic = this.rootgfx.CreateNewOne(w, h);
            ITopWindowEventRoot topEventRoot = null;
            if (newRootGraphic is ITopWindowEventRootProvider)
            {
                topEventRoot = ((ITopWindowEventRootProvider)newRootGraphic).EventRoot;
            }
            newViewportControl.InitRootGraphics(
                newRootGraphic,//new root
                topEventRoot,
                this.innerViewportKind);
            return newViewportControl;
        }
        //-----------
    }

    class PlatformWinBoxForm : IPlatformWindowBox
    {
        AbstractCompletionWindow _form;
        bool _evalLocationRelativeToDesktop;
        System.Drawing.Point _locationRelToDesktop;
        int _formLocalX;
        int _formLocalY;

        public event EventHandler VisibityChanged;
        public event EventHandler BoundsChanged;
        public event EventHandler PreviewBoundChanged;
        public event EventHandler PreviewVisibilityChanged;

        public PlatformWinBoxForm(AbstractCompletionWindow form)
        {

            this._form = form;
            _form.Move += (s, e) => _evalLocationRelativeToDesktop = false;

        }
        public bool UseRelativeLocationToParent
        {
            get;
            set;
        }
        public bool Visible
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
                        PreviewVisibilityChanged?.Invoke(this, EventArgs.Empty);
                        _form.ShowForm();
                        VisibityChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (_form.Visible)
                    {
                        PreviewVisibilityChanged?.Invoke(this, EventArgs.Empty);
                        _form.Hide();
                        VisibityChanged?.Invoke(this, EventArgs.Empty);
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
            //invoke Before accept new location
            PreviewBoundChanged?.Invoke(this, EventArgs.Empty);
            //------------------ 
            _formLocalX = x;
            _formLocalY = y;
            //Console.WriteLine("set location " + x + "," + y);
            //
            if (this.UseRelativeLocationToParent)
            {

                if (!_evalLocationRelativeToDesktop)
                {
                    _locationRelToDesktop = new System.Drawing.Point();
                    if (_form.LinkedParentControl != null)
                    {
                        //get location of this control relative to desktop
                        _locationRelToDesktop = _form.LinkedParentControl.PointToScreen(_form.LinkedParentControl.Location);
                    }
                    _evalLocationRelativeToDesktop = true;
                }
                _form.Location = new System.Drawing.Point(
                    _locationRelToDesktop.X + x,
                    _locationRelToDesktop.Y + y);
            }
            else
            {
                _form.Location = new System.Drawing.Point(x, y);

            }
            BoundsChanged?.Invoke(this, EventArgs.Empty);
        }

        void IPlatformWindowBox.SetSize(int w, int h)
        {
            PreviewBoundChanged?.Invoke(this, EventArgs.Empty);
            _form.Size = new System.Drawing.Size(w, h);
            BoundsChanged?.Invoke(this, EventArgs.Empty);
        }
        public void GetLocalBounds(out int x, out int y, out int w, out int h)
        {
            System.Drawing.Size size = _form.Size;
            x = _formLocalX;
            y = _formLocalY;
            w = size.Width;
            h = size.Height;
        }
        public void GetLocalBoundsIncludeShadow(out int x, out int y, out int w, out int h)
        {
            System.Drawing.Size size = _form.Size;
            x = _formLocalX;
            y = _formLocalY;
            w = size.Width + FormPopupShadow.SHADOW_SIZE + 5;
            h = size.Height + FormPopupShadow.SHADOW_SIZE + 5;
        }
    }
}
