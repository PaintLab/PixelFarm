//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.UI.InputBridge;
using PixelFarm.DrawingGL;

namespace LayoutFarm.UI
{

    public sealed class GraphicsViewRoot
    {

        AbstractTopWindowBridge _winBridge;
        RootGraphic _rootgfx;
        ITopWindowEventRoot _topWinEventRoot;
        InnerViewportKind _innerViewportKind;
        IGpuOpenGLSurfaceView _viewport;

        GLPainterContext _pcx;
        GLPainter _glPainter;
        PixelFarm.Drawing.GLES2.MyGLDrawBoard _drawboard;

        int _width;
        int _height;
        public GraphicsViewRoot(int width, int height)
        {
            _width = width;
            _height = height;
        }
        public PixelFarm.Drawing.GLES2.MyGLDrawBoard GetDrawBoard() => _drawboard;

        public IGpuOpenGLSurfaceView MyNativeWindow => _viewport;

        public void Close()
        {
            //1. clear all subForms
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

#if DEBUG
            System.GC.Collect();
#endif
        }

        public void MakeCurrent()
        {
            _viewport.MakeCurrent();

        }
        public void SwapBuffers()
        {
            _viewport.SwapBuffers();
        }
        public void SetBounds(int left, int top, int width, int height)
        {
            _width = width;
            _height = height;
            _viewport.SetBounds(left, top, width, height);
            _winBridge.UpdateCanvasViewportSize(width, height);
        }
        public void SetSize(int width, int height)
        {
            _width = width;
            _height = height;
            _viewport.SetSize(width, height);
            _winBridge.UpdateCanvasViewportSize(width, height);
        }
        public void Invalidate()
        {
            _viewport.Invalidate();
        }
        public void Refresh()
        {
            //invalidate 
            //and update windows
            _viewport.Refresh();
        }

        public int Width => _width;
        public int Height => _height;

        //
        public InnerViewportKind InnerViewportKind => _innerViewportKind;

        //
        public RootGraphic RootGfx => _rootgfx;
        //
        //public OpenTK.MyGraphicsViewWindow GetViewWindow() => (OpenTK.MyGraphicsViewWindow)_viewWindow;
        public GLPainter GetGLPainter() => _glPainter;
        public GLPainterContext GetGLRenderSurface() => _pcx;
        PixelFarm.Drawing.DrawBoard CreateSoftwareDrawBoard(int width, int height, InnerViewportKind innerViewportKind)
        {

            PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface gdiRenderSurface = new PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface(width, height);
            var drawBoard = new PixelFarm.Drawing.WinGdi.GdiPlusDrawBoard(gdiRenderSurface);

            return drawBoard;
        }

        public void InitRootGraphics(RootGraphic rootgfx,
            ITopWindowEventRoot topWinEventRoot,
            InnerViewportKind innerViewportKind,
            IGpuOpenGLSurfaceView nativeWindow,
            AbstractTopWindowBridge bridge)
        {
            //create a proper bridge****
            _rootgfx = rootgfx;
            _topWinEventRoot = topWinEventRoot;
            _innerViewportKind = innerViewportKind;
            _viewport = nativeWindow;
            _winBridge = bridge;

            nativeWindow.SetSize(rootgfx.Width, rootgfx.Height);

            switch (innerViewportKind)
            {
                case InnerViewportKind.GdiPlusOnGLES:
                case InnerViewportKind.AggOnGLES:
                case InnerViewportKind.GLES:
                    {
                        _winBridge.OnHostControlLoaded();
                        try
                        {
                            nativeWindow.MakeCurrent();
                        }
                        catch (Exception ex)
                        {

                        }
                        int w = nativeWindow.Width;
                        int h = nativeWindow.Height;

                        int max = Math.Max(w, h);

                        _pcx = GLPainterContext.Create(max, max, w, h, true);
                        _glPainter = new GLPainter();
                        _glPainter.BindToPainterContext(_pcx);

                        if (PixelFarm.Drawing.GLES2.GLES2Platform.TextService != null)
                        {
                            _glPainter.TextPrinter = new GLBitmapGlyphTextPrinter(_glPainter, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);
                        }
                        else
                        {
                            //warn....!
                        }

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
                        //_glPainter.TextPrinter = new CpuBlitTextSpanPrinter2(_glPainter, 400, 50, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);

                        //TODO: review this again!
                        //3  
                        var drawboard = new PixelFarm.Drawing.GLES2.MyGLDrawBoard(_glPainter);
                        _glPainter.SetDrawboard(drawboard);
                        _drawboard = drawboard;

                        //{
                        //in mixed mode
                        //GDI+ on GLES, Agg on GLES we provide a software rendering layer too
                        PixelFarm.Drawing.DrawBoard cpuDrawBoard = null;// CreateSoftwareDrawBoard(view.Width, view.Height, innerViewportKind);
                        drawboard.SetCpuBlitDrawBoardCreator(() => cpuDrawBoard ?? (cpuDrawBoard = CreateSoftwareDrawBoard(w, h, innerViewportKind)));
                        //}
                        ((OpenGL.MyTopWindowBridgeOpenGL)_winBridge).SetCanvas(drawboard);

                    }
                    break;
            }
        }
        public void PaintToOutputWindow()
        {
            _winBridge.PaintToOutputWindow();
        }
        public void PaintToPixelBuffer(IntPtr outputPixelBuffer)
        {
            _winBridge.CopyOutputPixelBuffer(0, 0, _width, _height, outputPixelBuffer);
        }

#if DEBUG
        public void dbugPaintMeFullMode()
        {
            _winBridge.dbugPaintToOutputWindowFullMode();
        }
        public IdbugOutputWindow IdebugOutputWin => _winBridge;

#endif
        public void TopDownRecalculateContent()
        {
            _rootgfx.TopDownRecalculateContent();
        }

        //protected override void OnVisibleChanged(EventArgs e)
        //{
        //    //s_dbugCount++;
        //    //System.Diagnostics.Debug.WriteLine("focus" + s_dbugCount.ToString());
        //    _rootgfx.InvalidateRectArea(new PixelFarm.Drawing.Rectangle(0, 0, _rootgfx.Width, _rootgfx.Height));
        //    _rootgfx.FlushAccumGraphics();
        //    //#if DEBUG
        //    //            s_dbugCount++;
        //    //System.Diagnostics.Debug.WriteLine("vis" + s_dbugCount.ToString());
        //    //#endif
        //    base.OnVisibleChanged(e);
        //}
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    //on Windows request paint at specific area
        //    if (_rootgfx != null)
        //    {
        //        if (e.ClipRectangle.Width + e.ClipRectangle.Height == 0)
        //        {
        //            //entire window
        //            //_rootgfx.InvalidateRectArea(new PixelFarm.Drawing.Rectangle(0, 0, _rootgfx.Width, _rootgfx.Height));
        //        }
        //        else
        //        {
        //            _rootgfx.InvalidateRectArea(
        //                new PixelFarm.Drawing.Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height));
        //            _rootgfx.FlushAccumGraphics();
        //        }

        //    }

        //    //#if DEBUG
        //    //            s_dbugCount++;
        //    //System.Diagnostics.Debug.WriteLine("p" + s_dbugCount.ToString() + e.ClipRectangle);
        //    //#endif
        //    base.OnPaint(e);
        //}

        //public void AddChild(RenderElement vi)
        //{
        //    _rootgfx.AddChild(vi);
        //}


        //public void AddChild(RenderElement renderElem, object owner)
        //{
        //    //temp disable this feature

        //    if (renderElem is RenderBoxBase &&
        //        owner is ITopWindowBox topWinBox)
        //    {
        //        if (topWinBox.PlatformWinBox == null)
        //        {

        //            FormPopupShadow popupShadow1 = new FormPopupShadow();
        //            popupShadow1.Visible = false;
        //            IntPtr handle1 = popupShadow1.Handle; //***


        //            //create platform winbox 
        //            var newForm = new AbstractCompletionWindow();
        //            newForm.LinkedParentForm = this.FindForm();
        //            newForm.LinkedParentControl = this;
        //            newForm.PopupShadow = popupShadow1;

        //            //TODO: 
        //            //1. review here=> 300,200
        //            //2. how to choose InnerViewportKind 

        //            UISurfaceViewportControl newSurfaceViewport = this.CreateNewOne(300, 200, InnerViewportKind.GLES);
        //            newSurfaceViewport.Location = new System.Drawing.Point(0, 0);
        //            newForm.Controls.Add(newSurfaceViewport);
        //            renderElem.ResetRootGraphics(newSurfaceViewport.RootGfx);
        //            renderElem.SetLocation(0, 0);
        //            newSurfaceViewport.RootGfx.AddChild(renderElem);

        //            //-----------------------------------------------------                        
        //            IntPtr tmpHandle = newForm.Handle;//force newform to create window handle

        //            //-----------------------------------------------------              

        //            var platformWinBox = new PlatformWinBoxForm(newForm);
        //            topWinBox.PlatformWinBox = platformWinBox;
        //            platformWinBox.UseRelativeLocationToParent = true;
        //            platformWinBox.PreviewVisibilityChanged += PlatformWinBox_PreviewVisibilityChanged;
        //            platformWinBox.PreviewBoundChanged += PlatformWinBox_PreviewBoundChanged;
        //            platformWinBox.BoundsChanged += PlatformWinBox_BoundsChanged;
        //            platformWinBox.VisibityChanged += PlatformWinBox_VisibityChanged;
        //            _subForms.Add(newForm);
        //        }
        //    }
        //    else
        //    {
        //        _rootgfx.AddChild(renderElem);
        //    }

        //    // 
        //    //if (renderElem is RenderBoxBase)
        //    //{
        //    //    if (owner is ITopWindowBox)
        //    //    {
        //    //        var topWinBox = owner as ITopWindowBox;
        //    //        if (topWinBox.PlatformWinBox == null)
        //    //        {

        //    //            FormPopupShadow popupShadow1 = new FormPopupShadow();
        //    //            popupShadow1.Visible = false;
        //    //            IntPtr handle1 = popupShadow1.Handle; //***


        //    //            //create platform winbox 
        //    //            var newForm = new AbstractCompletionWindow();
        //    //            newForm.LinkedParentForm = this.FindForm();
        //    //            newForm.LinkedParentControl = this;
        //    //            newForm.PopupShadow = popupShadow1;

        //    //            //TODO: 
        //    //            //1. review here=> 300,200
        //    //            //2. how to choose InnerViewportKind 

        //    //            UISurfaceViewportControl newSurfaceViewport = this.CreateNewOne(300, 200, InnerViewportKind.GLES);
        //    //            newSurfaceViewport.Location = new System.Drawing.Point(0, 0);
        //    //            newForm.Controls.Add(newSurfaceViewport);
        //    //            renderElem.ResetRootGraphics(newSurfaceViewport.RootGfx);
        //    //            renderElem.SetLocation(0, 0);
        //    //            newSurfaceViewport.AddChild(renderElem);
        //    //            //-----------------------------------------------------                        
        //    //            IntPtr tmpHandle = newForm.Handle;//force newform to create window handle

        //    //            //-----------------------------------------------------              

        //    //            var platformWinBox = new PlatformWinBoxForm(newForm);
        //    //            topWinBox.PlatformWinBox = platformWinBox;
        //    //            platformWinBox.UseRelativeLocationToParent = true;
        //    //            platformWinBox.PreviewVisibilityChanged += PlatformWinBox_PreviewVisibilityChanged;
        //    //            platformWinBox.PreviewBoundChanged += PlatformWinBox_PreviewBoundChanged;
        //    //            platformWinBox.BoundsChanged += PlatformWinBox_BoundsChanged;
        //    //            platformWinBox.VisibityChanged += PlatformWinBox_VisibityChanged;
        //    //            _subForms.Add(newForm);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        _rootgfx.AddChild(renderElem);
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    _rootgfx.AddChild(renderElem);
        //    //}
        //}





        ///// <summary>
        ///// create new UIViewport based on this control's current platform
        ///// </summary>
        ///// <returns></returns>
        //public UISurfaceViewportControl CreateNewOne(int w, int h, InnerViewportKind innerViewportKind)
        //{
        //    //each viewport has its own root graphics 

        //    UISurfaceViewportControl newViewportControl = new UISurfaceViewportControl();
        //    newViewportControl.Size = new System.Drawing.Size(w, h);
        //    RootGraphic newRootGraphic = _rootgfx.CreateNewOne(w, h);
        //    ITopWindowEventRoot topEventRoot = null;
        //    if (newRootGraphic is ITopWindowEventRootProvider)
        //    {
        //        topEventRoot = ((ITopWindowEventRootProvider)newRootGraphic).EventRoot;
        //    }
        //    newViewportControl.InitRootGraphics(
        //        newRootGraphic,//new root
        //        topEventRoot,
        //        innerViewportKind);
        //    return newViewportControl;
        //}
    }


}
