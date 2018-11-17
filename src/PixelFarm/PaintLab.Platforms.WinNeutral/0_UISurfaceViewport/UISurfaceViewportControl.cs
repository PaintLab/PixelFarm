//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Forms;

namespace LayoutFarm.UI.WinNeutral
{

    public partial class UISurfaceViewportControl : Control
    {
        TopWindowBridgeWinNeutral _winBridge;
        RootGraphic _rootgfx;
        ITopWindowEventRoot _topWinEventRoot;
        InnerViewportKind _innerViewportKind;
        List<Form> _subForms = new List<Form>();
        int _width;
        int _height;
        int _left;
        int _top;

        public UISurfaceViewportControl()
        {

        }
        public UIPlatform Platform
        {
            get { return LayoutFarm.UI.UIPlatformWinNeutral.platform; }
        }
        public PixelFarm.Drawing.Size Size
        {
            get { return new PixelFarm.Drawing.Size(_width, _height); }
            set
            {
                this._width = value.Width;
                this._height = value.Height;
            }
        }
        public PixelFarm.Drawing.Rectangle Bounds
        {
            get { return new PixelFarm.Drawing.Rectangle(_left, _top, _width, _height); }
            set
            {
                this._left = value.Left;
                this._top = value.Top;
                this._width = value.Width;
                this._height = value.Height;
            }
        }
        public void SwapBuffers()
        {
        }
        public void MakeCurrent()
        {
        }
        public void InitSetup2d(PixelFarm.Drawing.Rectangle rect)
        {

        }
        //public OpenTK.Graphics.Color4 ClearColor
        //{
        //    get;
        //    set;
        //}
        public void SetupCanvas(PixelFarm.Drawing.DrawBoard canvas)
        {
            bridge.SetupCanvas(canvas);
        }
        //
        OpenGL.MyTopWindowBridgeOpenGL bridge;
        public void InitRootGraphics(
            RootGraphic rootgfx,
            ITopWindowEventRoot topWinEventRoot,
            InnerViewportKind innerViewportKind)
        {
            //1.
            this._rootgfx = rootgfx;
            this._topWinEventRoot = topWinEventRoot;
            this._innerViewportKind = innerViewportKind;
            switch (innerViewportKind)
            {
                case InnerViewportKind.GLES:
                    {

                        ////temp not suppport
                        //PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();
                        bridge = new OpenGL.MyTopWindowBridgeOpenGL(rootgfx, topWinEventRoot);
                        bridge.BindWindowControl(this);
                        //var view = new OpenGL.GpuOpenGLSurfaceView();
                        //view.Width = 800;
                        //view.Height = 600;
                        ////view.Dock = DockStyle.Fill;
                        //this.Controls.Add(view);
                        ////--------------------------------------- 
                        //view.Bind(bridge);
                        this._winBridge = bridge;
                    }
                    break;
                case InnerViewportKind.Skia:
                    {

                        //skiasharp ***
                        var bridge = new Skia.MyTopWindowBridgeSkia(rootgfx, topWinEventRoot);
                        //var view = new CpuSurfaceView();
                        //view.Dock = DockStyle.Fill;
                        //this.Controls.Add(view);
                        ////--------------------------------------- 
                        //view.Bind(bridge);
                        this._winBridge = bridge;
                    }
                    break;
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
                    break;
            }
        }


        //void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        //    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //    this.BackColor = System.Drawing.Color.White;
        //    this.Size = new System.Drawing.Size(387, 277);
        //    this.ResumeLayout(false);
        //}
        protected override void OnLoad(EventArgs e)
        {
            this._winBridge.OnHostControlLoaded();
        }
        public void PaintMe(PixelFarm.DrawingGL.GLRenderSurface glsx)
        {
            glsx.DrawLine(0, 0, 100, 100);
            //this.winBridge.PaintToOutputWindow();
        }
        public void PaintMe()
        {
            //this.winBridge.PaintToOutputWindow();
        }
        public void PaintMeFullMode()
        {
            this._winBridge.PaintToOutputWindowFullMode();
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
            get { return this._winBridge; }
        }
#endif
        public void TopDownRecalculateContent()
        {
            this._rootgfx.TopWindowRenderBox.TopDownReCalculateContentSize();
        }
        public void AddContent(RenderElement vi)
        {
            this._rootgfx.TopWindowRenderBox.AddChild(vi);
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
                    this._rootgfx.TopWindowRenderBox.AddChild(vi);
                }
            }
            else
            {
                this._rootgfx.TopWindowRenderBox.AddChild(vi);
            }
        }

        public RootGraphic RootGfx
        {
            get
            {
                return this._rootgfx;
            }
        }
        public void Close()
        {
            this._winBridge.Close();
        }


        /// <summary>
        /// create new UIViewport based on this control's current platform
        /// </summary>
        /// <returns></returns>
        public UISurfaceViewportControl CreateNewOne(int w, int h)
        {
            //each viewport has its own root graphics 

            UISurfaceViewportControl newViewportControl = new UISurfaceViewportControl();
            newViewportControl.Size = new PixelFarm.Drawing.Size(w, h);

            RootGraphic newRootGraphic = this._rootgfx.CreateNewOne(w, h);
            ITopWindowEventRoot topEventRoot = null;
            if (newRootGraphic is ITopWindowEventRootProvider)
            {
                topEventRoot = ((ITopWindowEventRootProvider)newRootGraphic).EventRoot;
            }
            newViewportControl.InitRootGraphics(
                newRootGraphic,//new root
                topEventRoot,
                this._innerViewportKind);
            return newViewportControl;
        }
        //-----------
    }

    class PlatformWinBoxForm : IPlatformWindowBox
    {
        AbstractCompletionWindow _form;
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
