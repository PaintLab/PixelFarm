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

    public delegate void UpdateCpuBlitSurface(AggPainter painter, Rectangle updateArea);

    /// <summary>
    /// CpuBlit to GLES UIElement
    /// </summary>
    public class CpuBlitGLESUIElement : UIElement
    {


        protected int _width;
        protected int _height;
        CpuBlitGLCanvasRenderElement _canvasRenderE;
        UpdateCpuBlitSurface _updateCpuBlitSurfaceDel;
        //----------------------------------------------

        //software part
        protected MemBitmap _memBmp;
        protected AggPainter _aggPainter;
        protected MemBitmapBinder _memBitmapBinder;


        public CpuBlitGLESUIElement(int width, int height)
        {
            _width = width;
            _height = height;
            //
            SetupCpuBlitRenderSurface();//*** 
        }

        public AggPainter GetAggPainter() => _aggPainter;
      

        protected virtual bool HasSomeExtension => false;//class that override 

        public void CreatePrimaryRenderElement(GLPainterContext pcx, GLPainter painter, RootGraphic rootgfx)
        {
            if (_canvasRenderE == null)
            {

                var glBmp = new GLBitmap(_memBitmapBinder);
                glBmp.IsYFlipped = false;
                //
                var glRenderElem = new CpuBlitGLCanvasRenderElement(rootgfx, _width, _height, glBmp);
                glRenderElem.SetController(this); //connect to event system
                glRenderElem.SetOwnerDemoUI(this);
                _canvasRenderE = glRenderElem;
            }
        }

        internal CpuBlitGLCanvasRenderElement CpuBlitCanvasRenderElement => _canvasRenderE;

        //
        public override RenderElement CurrentPrimaryRenderElement => _canvasRenderE;
        protected override bool HasReadyRenderElement => _canvasRenderE != null;
        //

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





        //----------------------------------------------
        public virtual DrawBoard GetDrawBoard() { return null; }
        //----------------------------------------------
        /// <summary>
        /// set-up CpuBlit(software-rendering surface) 
        /// </summary>
        protected virtual void SetupCpuBlitRenderSurface()
        {
            //***
            _memBmp = new MemBitmap(_width, _height);
#if DEBUG
            _memBmp._dbugNote = "CpuBlitGLESUIElement.SetupCpuBlitRenderSurface()";
#endif
            _aggPainter = AggPainter.Create(_memBmp);

            //optional if we want to print text on agg surface
            _aggPainter.CurrentFont = new PixelFarm.Drawing.RequestFont("Tahoma", 10);
            _aggPainter.TextPrinter = new PixelFarm.Drawing.Fonts.FontAtlasTextPrinter(_aggPainter);

            _memBitmapBinder = new MemBitmapBinder(_memBmp, false);
            //
        }
        /// <summary>
        /// update cpublit surface with external handler
        /// </summary>
        /// <param name="updateArea"></param>
        internal virtual void UpdateCpuBlitSurface(Rectangle updateArea)
        {
            //update only specific part
            //***
            //_aggPainter.Clear(PixelFarm.Drawing.Color.White);
            //TODO:
            //if the content of _aggBmp is not changed
            //we should not draw again  
            // 
            _updateCpuBlitSurfaceDel?.Invoke(_aggPainter, updateArea);
            //
            ////test print some text
            //_aggPainter.FillColor = PixelFarm.Drawing.Color.Black; //set font 'fill' color
            //_aggPainter.DrawString("Hello! 12345", 0, 500);
        }
        internal void RaiseUpdateCpuBlitSurface(Rectangle updateArea)
        {
            _updateCpuBlitSurfaceDel?.Invoke(_aggPainter, updateArea);
        }
        internal bool HasCpuBlitUpdateSurfaceDel => _updateCpuBlitSurfaceDel != null;
        public void SetUpdateCpuBlitSurfaceDelegate(UpdateCpuBlitSurface updateCpuBlitSurfaceDel)
        {
            _updateCpuBlitSurfaceDel = updateCpuBlitSurfaceDel;
        }
#if DEBUG
        public virtual void dbugSaveAggBmp(string filename)
        {
            //using (System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(_aggBmp.Width, _aggBmp.Height))
            //{
            //    var bmpData = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, _aggBmp.Width, _aggBmp.Height),
            //         System.Drawing.Imaging.ImageLockMode.ReadWrite,
            //         System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //    PixelFarm.CpuBlit.Imaging.TempMemPtr ptr = ActualBitmap.GetBufferPtr(_aggBmp);

            //    unsafe
            //    {
            //        PixelFarm.CpuBlit.NativeMemMx.MemCopy((byte*)bmpData.Scan0, (byte*)ptr.Ptr, bmpData.Stride * bmpData.Height);
            //    }


            //    bmp1.UnlockBits(bmpData);
            //    bmp1.Save(filename);
            //}
        }
#endif
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
        internal override void UpdateCpuBlitSurface(Rectangle updateArea)
        {
            //_gdiDrawBoard.RenderSurface.Win32DC.SetClipRect(updateArea.X, updateArea.Y, updateArea.Width, updateArea.Height);
            //_gdiDrawBoard.RenderSurface.Win32DC.PatBlt(Win32.NativeWin32MemoryDC.PatBltColor.White);
            //_gdiDrawBoard.RenderSurface.Win32DC.ClearClipRect();
            //------------
            //update software part content
            RenderBoxBase primElem = base.CpuBlitCanvasRenderElement;
            if (HasCpuBlitUpdateSurfaceDel)
            {
                RaiseUpdateCpuBlitSurface(updateArea);
            }
        }
        //------------ 
        protected override bool HasSomeExtension => true;
        protected override void SetupCpuBlitRenderSurface()
        {
            //don't call base
            //1. we create gdi plus draw board
            var renderSurface = new PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface(_width, _height);
            _gdiDrawBoard = new PixelFarm.Drawing.WinGdi.GdiPlusDrawBoard(renderSurface);
            _gdiDrawBoard.CurrentFont = new RequestFont("Tahoma", 10);

            //2. create actual bitmap that share 'bitmap mem' with gdiPlus Render surface                 
            _memBmp = renderSurface.GetMemBitmap();
            //3. create painter from the agg bmp (then we will copy the 'client' gdi mem surface to the GL)
            _aggPainter = renderSurface.GetAggPainter();//**

            //***
            //
            //... 
            //
            _memBitmapBinder = new MemBitmapBinder(_memBmp, false);
            _memBitmapBinder.BitmapFormat = BitmapBufferFormat.BGR;//**
        }
    }

    class CpuBlitGLCanvasRenderElement : RenderBoxBase, IDisposable
    {

        CpuBlitGLESUIElement _ui;
        GLBitmap _glBmp;
        RootGraphic _rootgfx;

        public CpuBlitGLCanvasRenderElement(RootGraphic rootgfx, int w, int h, GLBitmap glBmp)
            : base(rootgfx, w, h)
        {
            _rootgfx = rootgfx;
            _glBmp = glBmp;
        }
        public void SetOwnerDemoUI(CpuBlitGLESUIElement ui)
        {
            _ui = ui;
        }

        protected override void DrawBoxContent(DrawBoard d, Rectangle updateArea)
        {
            //canvas here should be glcanvas

            //TODO: 
            //1. if the content of glBmp is not changed
            //we should not render again 
            //2. if we only update some part of texture
            //may can transfer only that part to the glBmp
            //-------------------------------------------------------------------------  
            if (_rootgfx.HasRenderTreeInvalidateAccumRect)
            {

                //update cpu surface part***  
                DrawBoard board = _ui.GetDrawBoard();
                if (board != null)
                {
                    board.SetClipRect(updateArea);
                    board.Clear(Color.White); //clear background
                    //board.SetClipRect(new Rectangle(0, 0, 1200, 1200)); 
                    DrawDefaultLayer(board, ref updateArea);
#if DEBUG
                    //_ui.dbugSaveAggBmp("c:\\WImageTest\\a001.png");
#endif
                }

                if (_ui.HasCpuBlitUpdateSurfaceDel)
                {
                    _ui.UpdateCpuBlitSurface(updateArea);
                }
            }

            _glBmp.UpdateTexture(updateArea);

            //------------------------------------------------------------------------- 
            d.DrawImage(_glBmp, 0, 0);
            //_pcx.DrawImage(_glBmp, 0, 0);

            //test print text from our GLTextPrinter 
            //_glPainter.FillColor = PixelFarm.Drawing.Color.Black;
            //_glPainter.DrawString("Hello2", 0, 400);
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