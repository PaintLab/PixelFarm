﻿//MIT, 2016-present, WinterDev

using System;
using System.Drawing;
using System.Windows.Forms;

using PixelFarm.Agg;
using PixelFarm.Agg.Imaging;
using PixelFarm.Drawing.Fonts;


namespace Mini
{

    public partial class SoftAggControl : UserControl
    {
        bool isMouseDown;
        DemoBase exampleBase;
        int myWidth = 800;
        int myHeight = 600;
        GdiBitmapBackBuffer bitmapBackBuffer;
        PixelFarm.Drawing.Painter painter;

        bool _useGdiPlusOutput;
        bool _gdiAntiAlias;
        Graphics thisGfx;//for output
        PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface sx;

        //Bitmap bufferBmp = null;
        System.Drawing.Rectangle bufferBmpRect;
        public SoftAggControl()
        {
            bitmapBackBuffer = new GdiBitmapBackBuffer();
            _useGdiPlusOutput = false;
            InitializeComponent();
            this.Load += new EventHandler(SoftAggControl_Load);
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




        void SoftAggControl_Load(object sender, EventArgs e)
        {
            if (_useGdiPlusOutput)
            {
                // This example assumes the existence of a form called Form1.
                // Gets a reference to the current BufferedGraphicsContext
                //BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
                //_myBuffGfx = currentContext.Allocate(this.CreateGraphics(),
                //   this.DisplayRectangle);

                // Creates a BufferedGraphics instance associated with Form1, and with 
                // dimensions the same size as the drawing surface of Form1. 
                thisGfx = this.CreateGraphics();  //for render to output
                bufferBmpRect = this.DisplayRectangle;
                //bufferBmp = new Bitmap(bufferBmpRect.Width, bufferBmpRect.Height);


                sx = new PixelFarm.Drawing.WinGdi.GdiPlusRenderSurface(0, 0, bufferBmpRect.Width, bufferBmpRect.Height);
                var gdiPlusCanvasPainter = new PixelFarm.Drawing.WinGdi.GdiPlusPainter(sx);

                gdiPlusCanvasPainter.SmoothingMode = _gdiAntiAlias ? PixelFarm.Drawing.SmoothingMode.AntiAlias : PixelFarm.Drawing.SmoothingMode.HighSpeed;
                painter = gdiPlusCanvasPainter;
                painter.CurrentFont = new PixelFarm.Drawing.RequestFont("tahoma", 14);
            }
            else
            {
                AggRenderSurface aggsx = CreateAggRenderSurface(myWidth, myHeight, 32);
                AggPainter aggPainter = new AggPainter(aggsx);
                //set text printer for agg canvas painter
                aggPainter.CurrentFont = new PixelFarm.Drawing.RequestFont("tahoma", 14);

                //TODO: review text printer here again***
                //VxsTextPrinter textPrinter = new VxsTextPrinter(aggPainter, YourImplementation.BootStrapWinGdi.GetFontLoader());
                //aggPainter.TextPrinter = textPrinter;
                painter = aggPainter;
            }
            painter.Clear(PixelFarm.Drawing.Color.White);
        }
        AggRenderSurface CreateAggRenderSurface(int width, int height, int bitDepth)
        {
            if (width > 0 && height > 0)
            {

                if (bitDepth != 32)
                {
                    throw new NotImplementedException("Don't support this bit depth yet.");
                }
                else
                {
                    var actualImage = new ActualBitmap(width, height);
                    bitmapBackBuffer.Initialize(width, height, bitDepth, actualImage);
                    return new AggRenderSurface(actualImage);
                }
            }
            throw new NotSupportedException();
        }
        public void LoadExample(DemoBase exBase)
        {
            this.exampleBase = exBase;
            if (painter != null)
            {
                DemoBase.InvokePainterReady(exBase, painter);
            }
            //exBase.RequestNewGfx2d += () => this.bitmapBackBuffer.CreateNewGraphic2D();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            exampleBase.KeyDown((int)e.KeyCode);
            base.OnKeyDown(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.isMouseDown = true;
            //exampleBase.MouseDown(e.X, myHeight - e.Y, e.Button == System.Windows.Forms.MouseButtons.Right);
            exampleBase.MouseDown(e.X, e.Y, e.Button == System.Windows.Forms.MouseButtons.Right);
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
            this.isMouseDown = false;
            //exampleBase.MouseUp(e.X, myHeight - e.Y);
            exampleBase.MouseUp(e.X, e.Y);
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
            if (this.isMouseDown)
            {
                //exampleBase.MouseDrag(e.X, myHeight - e.Y);
                exampleBase.MouseDrag(e.X, e.Y);
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
            if (this.exampleBase == null)
            {
                base.OnPaint(e);
                return;
            }
            if (!_useGdiPlusOutput)
            {
                exampleBase.Draw(painter);

                Graphics g = e.Graphics;
                IntPtr displayDC = g.GetHdc();
                bitmapBackBuffer.UpdateToHardwareSurface(displayDC);
                g.ReleaseHdc(displayDC);
            }
            else
            {
                exampleBase.Draw(painter);
                Graphics g = e.Graphics;
                IntPtr displayDC = g.GetHdc();

                sx.RenderTo(displayDC, 0, 0, new PixelFarm.Drawing.Rectangle(0, 0, bufferBmpRect.Width, bufferBmpRect.Height));
                g.ReleaseHdc(displayDC);
            }
            base.OnPaint(e);
        }
        void UpdateOutput()
        {
            exampleBase.Draw(painter);
            if (_useGdiPlusOutput)
            {
                IntPtr destHdc = thisGfx.GetHdc();
                sx.RenderTo(destHdc, 0, 0, new PixelFarm.Drawing.Rectangle(0, 0, bufferBmpRect.Width, bufferBmpRect.Height));
                thisGfx.ReleaseHdc(destHdc);
            }
        }
    }
}
