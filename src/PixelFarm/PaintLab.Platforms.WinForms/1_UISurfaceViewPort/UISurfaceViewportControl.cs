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
        //private Panel panel1;
        List<Form> subForms = new List<Form>();
        public UISurfaceViewportControl()
        {
            InitializeComponent();

            //this.panel1.Visible = false; 
        }
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
            //s_dbugCount++;
            //Console.WriteLine(s_dbugCount.ToString() + e.ClipRectangle);
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
        OpenGL.GpuOpenGLSurfaceView openGLSurfaceView;
        GLRenderSurface _glsx;
        GLPainter canvasPainter;
#endif
        void HandleGLPaint(object sender, System.EventArgs e)
        {
            //canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            //canvas2d.StrokeColor = PixelFarm.Drawing.Color.Black;
            //canvas2d.ClearColorBuffer();
            ////example
            //canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
            //canvasPainter.FillRectLBWH(20, 20, 150, 150);
            ////load bmp image 
            //////------------------------------------------------------------------------- 
            ////if (exampleBase != null)
            ////{
            ////    exampleBase.Draw(canvasPainter);
            ////}
            ////draw data 

            //openGLSurfaceView.SwapBuffers();
        }
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
                case InnerViewportKind.GL:
                    {
#if GL_ENABLE
                        //temp not suppport  
                        //TODO: review here
                        //PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

                        var bridge = new OpenGL.MyTopWindowBridgeOpenGL(rootgfx, topWinEventRoot);
                        var view = new OpenGL.GpuOpenGLSurfaceView();

                        view.Width = 1200;
                        view.Height = 1200;
                        openGLSurfaceView = view;
                        //view.Dock = DockStyle.Fill;
                        this.Controls.Add(view);
                        //this.panel1.Visible = true;
                        //this.panel1.Controls.Add(view);

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
                        canvasPainter = new GLPainter(_glsx);

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
                        var printer = new GLBitmapGlyphTextPrinter(canvasPainter, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);
                        canvasPainter.TextPrinter = printer;

                        //
                        var myGLCanvas1 = new PixelFarm.Drawing.GLES2.MyGLDrawBoard(canvasPainter, _glsx.CanvasWidth, _glsx.CanvasHeight);
                        bridge.SetCanvas(myGLCanvas1);
#endif
                    }
                    break;
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
            this.Name = "UISurfaceViewportControl";
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

#if DEBUG
        public void dbugPaintMeFullMode()
        {
            this.winBridge.dbugPaintToOutputWindowFullMode();
        }
#endif
        public void PaintToPixelBuffer(IntPtr outputPixelBuffer)
        {
            winBridge.CopyOutputPixelBuffer(0, 0, this.Width, this.Height, outputPixelBuffer);
        }

#if DEBUG
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
        public void AddChild(RenderElement vi, object owner)
        {
            if (vi is RenderBoxBase)
            {
                if (owner is ITopWindowBox)
                {
                    var topWinBox = owner as ITopWindowBox;
                    if (topWinBox.PlatformWinBox == null)
                    {

                        FormPopupShadow2 popupShadow1 = new FormPopupShadow2();
                        IntPtr handle1 = popupShadow1.Handle;


                        //create platform winbox 
                        var newForm = new AbstractCompletionWindow();
                        newForm.LinkedParentForm = this.FindForm();
                        newForm.LinkedParentControl = this;
                        newForm.PopupShadow = popupShadow1;


                        //TODO: review here=> 300,200

                        UISurfaceViewportControl newSurfaceViewport = this.CreateNewOne(300, 200);
                        newSurfaceViewport.Location = new System.Drawing.Point(0, 0);
                        newForm.Controls.Add(newSurfaceViewport);
                        vi.ResetRootGraphics(newSurfaceViewport.RootGfx);
                        vi.SetLocation(0, 0);
                        newSurfaceViewport.AddChild(vi);
                        //-----------------------------------------------------                        
                        s_tmpHandle = newForm.Handle;//force newform to create window handle

                        //-----------------------------------------------------              

                        var platformWinBox = new PlatformWinBoxForm(newForm);
                        topWinBox.PlatformWinBox = platformWinBox;
                        platformWinBox.UseRelativeLocationToParent = true;
                        subForms.Add(newForm);
                        s_tmpHandle = IntPtr.Zero;

                    }
                }
                else
                {
                    this.rootgfx.TopWindowRenderBox.AddChild(vi);
                }
            }
            else
            {
                this.rootgfx.TopWindowRenderBox.AddChild(vi);
            }
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


        public PlatformWinBoxForm(AbstractCompletionWindow form)
        {
            this._form = form;
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
                    _evalLocationRelativeToDesktop = false;
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
            if (this.UseRelativeLocationToParent)
            {
                //#if DEBUG
                //                if (!_form.IsHandleCreated)
                //                {
                //                }
                //#endif
                //1. find parent form/control  
                if (!_evalLocationRelativeToDesktop)
                {
                    _locationRelToDesktop = new System.Drawing.Point();// _form.LinkedParentForm.Location;
                    if (_form.LinkedParentControl != null)
                    {
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

        }

        void IPlatformWindowBox.SetSize(int w, int h)
        {
            _form.Size = new System.Drawing.Size(w, h);
        }
    }
}
