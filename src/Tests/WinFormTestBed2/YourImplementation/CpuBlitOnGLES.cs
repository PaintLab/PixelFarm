//MIT, 2014-present, WinterDev
//MIT, 2018-present, WinterDev


using System;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;

using PixelFarm.Drawing;
using LayoutFarm;
using LayoutFarm.UI;


namespace YourImplementation
{

    public delegate void UpdateCpuBlitSurface(AggPainter painter);

    /// <summary>
    /// CpuBlit to GLES UIElement
    /// </summary>
    public class CpuBlitGLESUIElement : UIElement
    {
        //DemoBase _demoBase;
        protected int _width;
        protected int _height;
        CpuBlitGLCanvasRenderElement _canvasRenderE;
        UpdateCpuBlitSurface _updateCpuBlitSurfaceDel;

        public CpuBlitGLESUIElement(int width, int height)
        {
            _width = width;
            _height = height;
            //
            SetupAggCanvas();//***
            ContentMayChanged = true;

        }
        public AggPainter GetAggPainter() => _aggPainter;

        public void SetUpdateCpuBlitSurfaceDelegate(UpdateCpuBlitSurface updateCpuBlitSurfaceDel)
        {
            _updateCpuBlitSurfaceDel = updateCpuBlitSurfaceDel;
        }
        //
        protected virtual bool HasSomeExtension => false;//class that override 

        public void CreatePrimaryRenderElement(GLRenderSurface glsx, GLPainter painter, RootGraphic rootgfx)
        {
            if (_canvasRenderE == null)
            {
                var glRenderElem = new CpuBlitGLCanvasRenderElement(rootgfx, _width, _height, _lazyImgProvider);
                glRenderElem.SetPainter(glsx, painter);
                glRenderElem.SetController(this); //connect to event system
                glRenderElem.SetOwnerDemoUI(this);
                _canvasRenderE = glRenderElem;
            }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get
            {
                return _canvasRenderE;
            }
        }

        protected override bool HasReadyRenderElement => _canvasRenderE != null;

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            //for this Elem  => please call CreatePrimaryRenderElement first  
            return _canvasRenderE;
        }

        public override void InvalidateGraphics()
        {
            if (_canvasRenderE != null)
            {
                _canvasRenderE.InvalidateGraphics();
            }
        }
        public override void Walk(UIVisitor visitor)
        {

        }
        //handle event
        public bool ContentMayChanged { get; set; }


        //----------------------------------------------
        protected ActualBitmap _aggBmp;
        protected AggPainter _aggPainter;
        protected LazyActualBitmapBufferProvider _lazyImgProvider;


        //----------------------------------------------
        public virtual DrawBoard GetDrawBoard() { return null; }
        //----------------------------------------------
        protected virtual void SetupAggCanvas()
        {
            //***
            _aggBmp = new ActualBitmap(_width, _height);
            _aggPainter = AggPainter.Create(_aggBmp);

            //optional if we want to print text on agg surface
            _aggPainter.CurrentFont = new PixelFarm.Drawing.RequestFont("Tahoma", 10);
            _aggPainter.TextPrinter = new PixelFarm.Drawing.Fonts.FontAtlasTextPrinter(_aggPainter);
            _lazyImgProvider = new LazyActualBitmapBufferProvider(_aggBmp);
            //
        }
        protected virtual void ClearSurface()
        {
            _aggPainter.Clear(PixelFarm.Drawing.Color.White);
        }
        internal void UpdateCpuBlitSurface()
        {
            ClearSurface();
            //TODO:
            //if the content of _aggBmp is not changed
            //we should not draw again  
            _updateCpuBlitSurfaceDel(_aggPainter);
            //
            //test print some text
            _aggPainter.FillColor = PixelFarm.Drawing.Color.Black; //set font 'fill' color
            _aggPainter.DrawString("Hello! 12345", 0, 500);
        }
    }

    /// <summary>
    /// CpuBlit and Gdi+  Bridge UIElement
    /// </summary>
    public class GdiOnGLESUIElement : CpuBlitGLESUIElement
    {
        PixelFarm.Drawing.WinGdi.GdiPlusDrawBoard _gdiDrawBoard;
        public GdiOnGLESUIElement(int width, int height)
            : base(width, height)
        {
        }
        public override DrawBoard GetDrawBoard()
        {
            return _gdiDrawBoard;
        }
        protected override void ClearSurface()
        {
            _gdiDrawBoard.RenderSurface.Win32DC.PatBlt(Win32.NativeWin32MemoryDC.PatBltColor.White);
        }
        protected override bool HasSomeExtension => true;
        protected override void SetupAggCanvas()
        {
            //don't call base
            //1. we create gdi plus draw board
            _gdiDrawBoard = new PixelFarm.Drawing.WinGdi.GdiPlusDrawBoard(0, 0, _width, _height);
            _gdiDrawBoard.CurrentFont = new RequestFont("Tahoma", 10);

            //2. create actual bitmap that share 'bitmap mem' with gdiPlus Render surface                 
            _aggBmp = new ActualBitmap(_width, _height, _gdiDrawBoard.RenderSurface.Win32DC.PPVBits);
            //3. create painter from the agg bmp (then we will copy the 'client' gdi mem surface to the GL)
            _aggPainter = AggPainter.Create(_aggBmp);

            //...
            //optional if we want to print text on agg surface
            _aggPainter.CurrentFont = new PixelFarm.Drawing.RequestFont("Tahoma", 10);
            var aggTextPrinter = new PixelFarm.Drawing.Fonts.FontAtlasTextPrinter(_aggPainter);
            _aggPainter.TextPrinter = aggTextPrinter;
            //
            _lazyImgProvider = new LazyActualBitmapBufferProvider(_aggBmp);
            _lazyImgProvider.BitmapFormat = GLBitmapFormat.BGR;
        }
    }

    public class CpuBlitGLCanvasRenderElement : RenderElement, IDisposable
    {

        CpuBlitGLESUIElement _ui;
        //
        GLRenderSurface _glsx;
        GLPainter _glPainter;
        GLBitmap _glBmp;
        //
        LazyActualBitmapBufferProvider _lzBmpProvider;
        public CpuBlitGLCanvasRenderElement(RootGraphic rootgfx, int w, int h, LazyActualBitmapBufferProvider lzBmpProvider)
            : base(rootgfx, w, h)
        {
            _lzBmpProvider = lzBmpProvider;// 
        }
        public void SetOwnerDemoUI(CpuBlitGLESUIElement ui)
        {
            _ui = ui;
        }
        public void SetPainter(GLRenderSurface glsx, GLPainter canvasPainter)
        {
            _glsx = glsx;
            _glPainter = canvasPainter;
        }
        public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {

            //TODO: 
            //1. if the content of glBmp is not changed
            //we should not render again 
            //2. if we only update some part of texture
            //may can transfer only that part to the glBmp
            //-------------------------------------------------------------------------  
            if (_ui.ContentMayChanged)
            {
                _ui.UpdateCpuBlitSurface();
                _ui.ContentMayChanged = false;

                //load new glBmp 
                if (_glBmp != null)
                {
                    _glBmp.Dispose();
                    _glBmp = null;
                }
            }
            //------------------------------------------------------------------------- 
            //copy from 
            if (_glBmp == null)
            {
                _glBmp = new GLBitmap(_lzBmpProvider);
                _glBmp.IsYFlipped = false;
            }
            //------------------------------------------------------------------------- 
            _glsx.DrawImage(_glBmp, 0, 0);
            //test print text from our GLTextPrinter 
            _glPainter.FillColor = PixelFarm.Drawing.Color.Black;
            _glPainter.DrawString("Hello2", 0, 400);
            //------------------------------------------------------------------------- 
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {

        }
        public void Dispose()
        {

        }
    }


}