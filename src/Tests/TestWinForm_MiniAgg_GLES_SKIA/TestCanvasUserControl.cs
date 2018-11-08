//MIT, 2016-present, WinterDev

using System;
using System.Drawing;
using System.Windows.Forms;

using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.Imaging;
using PixelFarm.Drawing.Fonts;


namespace Mini
{


    partial class TestCanvasUserControl : UserControl
    {
         
        //this user control is for test only.
        //in HtmlRenderer we use UISurfaceViewportControl

        bool _isMouseDown;
        DemoBase _exampleBase;
        int _myWidth = 800;
        int _myHeight = 600;
        Win32.NativeWin32MemoryDc _nativeWin32Dc; //use this as gdi back buffer
        PixelFarm.Drawing.Painter _painter;

        bool _useGdiPlusOutput;
        bool _gdiAntiAlias;
        Graphics _thisGfx;//for output
        PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface _sx;
        System.Drawing.Rectangle _bufferBmpRect;

        public TestCanvasUserControl()
        {

            _useGdiPlusOutput = false;
            InitializeComponent();
            this.Load += TestCanvasUserControl_Load;
        }

        public bool UseGdiPlusOutput
        {
            get { return _useGdiPlusOutput; }
            set { _useGdiPlusOutput = value; }
        }
        public bool UseGdiAntiAlias
        {
            get { return _gdiAntiAlias; }
            set { _gdiAntiAlias = value; }
        }
        void TestCanvasUserControl_Load(object sender, EventArgs e)
        {
            if (_useGdiPlusOutput)
            {

                _thisGfx = this.CreateGraphics();  //for render to output
                _bufferBmpRect = this.DisplayRectangle;
                _sx = new PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface(0, 0, _bufferBmpRect.Width, _bufferBmpRect.Height);
                var gdiPlusCanvasPainter = new PixelFarm.Drawing.WinGdi.GdiPlusPainter(_sx);

                gdiPlusCanvasPainter.SmoothingMode = _gdiAntiAlias ? PixelFarm.Drawing.SmoothingMode.AntiAlias : PixelFarm.Drawing.SmoothingMode.HighSpeed;
                _painter = gdiPlusCanvasPainter;
                _painter.CurrentFont = new PixelFarm.Drawing.RequestFont("tahoma", 14);
            }
            else
            {

                //1. gdi+ create backbuffer
                _nativeWin32Dc = new Win32.NativeWin32MemoryDc(_myWidth, _myHeight);

                //2. create actual bitmap that share bitmap data from native _nativeWin32Dc
                var actualImage = new ActualBitmap(_myWidth, _myHeight, _nativeWin32Dc.PPVBits);

                //----------------------------------------------------------------
                //3. create render surface from bitmap => provide basic bitmap fill operations
                AggRenderSurface aggsx = new AggRenderSurface(actualImage);
                //4. painter wraps the render surface  => provide advance operations
                AggPainter aggPainter = new AggPainter(aggsx);

                //---------------------------------------------------------------
                //Text functions
                //5. set text printer for agg canvas painter
                aggPainter.CurrentFont = new PixelFarm.Drawing.RequestFont("tahoma", 14);

                //TODO: review text printer here again***                 
                FontAtlasTextPrinter textPrinter = new FontAtlasTextPrinter(aggPainter);
                //VxsTextPrinter textPrinter = new VxsTextPrinter(aggPainter);
                aggPainter.TextPrinter = textPrinter;
                _painter = aggPainter;
            }
            _painter.Clear(PixelFarm.Drawing.Color.White);
        }

        public void LoadExample(DemoBase exBase)
        {
            this._exampleBase = exBase;
            if (_painter != null)
            {
                DemoBase.InvokePainterReady(exBase, _painter);
            }
            //exBase.RequestNewGfx2d += () => this.bitmapBackBuffer.CreateNewGraphic2D();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            _exampleBase.KeyDown((int)e.KeyCode);
            base.OnKeyDown(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this._isMouseDown = true;
            //exampleBase.MouseDown(e.X, myHeight - e.Y, e.Button == System.Windows.Forms.MouseButtons.Right);
            _exampleBase.MouseDown(e.X, e.Y, e.Button == System.Windows.Forms.MouseButtons.Right);
            _exampleBase.NeedRedraw = true;
            base.OnMouseDown(e);
            if (!_useGdiPlusOutput)
            {
                Invalidate();
            }
            else
            {
                UpdateOutput();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this._isMouseDown = false;
            //exampleBase.MouseUp(e.X, myHeight - e.Y);
            _exampleBase.MouseUp(e.X, e.Y);
            //force redraw when mouse up
            _exampleBase.NeedRedraw = true;
            base.OnMouseUp(e);
            if (!_useGdiPlusOutput)
            {
                Invalidate();
            }
            else
            {
                UpdateOutput();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this._isMouseDown)
            {
                _exampleBase.MouseDrag(e.X, e.Y);

                //force redraw when drag 
                _exampleBase.NeedRedraw = true;

                if (!_useGdiPlusOutput)
                {
                    Invalidate();
                }
                else
                {
                    UpdateOutput();
                }
            }
            base.OnMouseMove(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this._exampleBase == null)
            {
                base.OnPaint(e);
                return;
            }
            if (!_useGdiPlusOutput)
            {
                //check if the example output need to be redraw 
                //or not, if not then use img cache
                if (_exampleBase.NeedRedraw)
                {
                    _exampleBase.Draw(_painter);
                    _exampleBase.NeedRedraw = false;
                }
                Graphics g = e.Graphics;
                IntPtr displayDC = g.GetHdc();

                _nativeWin32Dc.BitBltTo(displayDC);
                //bool result = Win32.MyWin32.BitBlt(displayHdc, 0, 0,
                //     width,
                //     height,
                //     nativeWin32Dc.DC, 0, 0, SRCCOPY);

                //_bitmapBackBuffer.UpdateToHardwareSurface(displayDC);
                g.ReleaseHdc(displayDC);
            }
            else
            {
                _exampleBase.Draw(_painter);
                Graphics g = e.Graphics;
                IntPtr displayDC = g.GetHdc();

                _sx.RenderTo(displayDC, 0, 0, new PixelFarm.Drawing.Rectangle(0, 0, _bufferBmpRect.Width, _bufferBmpRect.Height));
                g.ReleaseHdc(displayDC);
            }
            base.OnPaint(e);
        }
        void UpdateOutput()
        {
            _exampleBase.Draw(_painter);
            if (_useGdiPlusOutput)
            {
                IntPtr destHdc = _thisGfx.GetHdc();
                _sx.RenderTo(destHdc, 0, 0, new PixelFarm.Drawing.Rectangle(0, 0, _bufferBmpRect.Width, _bufferBmpRect.Height));
                _thisGfx.ReleaseHdc(destHdc);
            }
        }
    }
}
