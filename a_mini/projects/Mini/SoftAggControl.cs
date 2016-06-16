//MIT 2016, WinterDev

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PixelFarm.Agg;
using PixelFarm.Agg.Image;
namespace Mini
{
    public partial class SoftAggControl : UserControl
    {
        bool isMouseDown;
        DemoBase exampleBase;
        int myWidth = 800;
        int myHeight = 600;
        WindowsFormsBitmapBackBuffer bitmapBackBuffer = new WindowsFormsBitmapBackBuffer();
        Graphics2D gfx;
        Graphics _g;
        CanvasPainter painter;
        bool _useGdiPlusOutput;
        bool _gdiAntiAlias;
        System.Drawing.Graphics thisGfx;//for output
        Bitmap bufferBmp = null;
        Rectangle bufferBmpRect;
        public SoftAggControl()
        {
            _useGdiPlusOutput = false;
            InitializeComponent();
            this.Load += new EventHandler(SoftAggControl_Load);
        }

        public bool UseGdiPlus
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
                bufferBmp = new Bitmap(bufferBmpRect.Width, bufferBmpRect.Height);
                _g = Graphics.FromImage(bufferBmp);
                if (_gdiAntiAlias)
                {
                    _g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                }
                else
                {
                    _g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                }

                var canvas = new PixelFarm.Drawing.WinGdi.CanvasGraphics2dGdi(_g, bufferBmp);
                this.gfx = canvas;
                this.gfx.Clear(ColorRGBA.White);
                painter = new PixelFarm.Drawing.WinGdi.GdiPlusCanvasPainter(canvas);
            }
            else
            {
                this.gfx = bitmapBackBuffer.Initialize(myWidth, myHeight, 32);
                this.gfx.Clear(ColorRGBA.White);
                painter = new AggCanvasPainter(gfx);
            }
        }

        public void LoadExample(DemoBase exBase)
        {
            this.exampleBase = exBase;
            exBase.RequestNewGfx2d += () => this.bitmapBackBuffer.CreateNewGraphic2D();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.isMouseDown = true;
            exampleBase.MouseDown(e.X, myHeight - e.Y, e.Button == System.Windows.Forms.MouseButtons.Right);
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
            exampleBase.MouseUp(e.X, myHeight - e.Y);
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
                exampleBase.MouseDrag(e.X, myHeight - e.Y);
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
                bitmapBackBuffer.UpdateToHardwareSurface(e.Graphics);
            }
            else
            {
                UpdateOutput();
            }
            base.OnPaint(e);
        }
        void UpdateOutput()
        {
            exampleBase.Draw(painter);
            if (_useGdiPlusOutput)
            {
                //_myBuffGfx.Render();
                thisGfx.DrawImageUnscaledAndClipped(bufferBmp, bufferBmpRect);
            }
        }
    }
}
