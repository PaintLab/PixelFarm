//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
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

        GLPainterCore _pcx;
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
        public GLPainter GetGLPainter() => _glPainter;
        public GLPainterCore GLPainterCore() => _pcx;
        PixelFarm.Drawing.DrawBoard CreateSoftwareDrawBoard(int width, int height, InnerViewportKind innerViewportKind)
        {
            //TODO: use Agg 
            return null;
            //PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface gdiRenderSurface = new PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface(width, height);
            //var drawBoard = new PixelFarm.Drawing.WinGdi.GdiPlusDrawBoard(gdiRenderSurface);

            //return drawBoard;
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

                        _pcx = PixelFarm.DrawingGL.GLPainterCore.Create(max, max, w, h, true);
                        _glPainter = new GLPainter();
                        _glPainter.BindToPainterCore(_pcx);

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
    }
}
