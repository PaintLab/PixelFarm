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
        BufferedGraphics myBuffer;
        System.Drawing.Graphics _g;
        CanvasPainter painter;
        bool useGdiPlusOutput;
        public SoftAggControl()
        {
            useGdiPlusOutput = false;
            InitializeComponent();
            this.Load += new EventHandler(SoftAggControl_Load);
        }

        public void UseGdiPlus(bool useGdiPlusOutput)
        {
            this.useGdiPlusOutput = useGdiPlusOutput;
        }
        void SoftAggControl_Load(object sender, EventArgs e)
        {
            if (useGdiPlusOutput)
            {
                // This example assumes the existence of a form called Form1.
                // Gets a reference to the current BufferedGraphicsContext
                BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
                // Creates a BufferedGraphics instance associated with Form1, and with 
                // dimensions the same size as the drawing surface of Form1.
                myBuffer = currentContext.Allocate(this.CreateGraphics(),
                   this.DisplayRectangle);
                _g = myBuffer.Graphics;
                var canvas = new PixelFarm.Drawing.WinGdi.CanvasGraphics2dGdi(_g);
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
            if (!useGdiPlusOutput)
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
            if (!useGdiPlusOutput)
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
                if (!useGdiPlusOutput)
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
            if (!useGdiPlusOutput)
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
            if (useGdiPlusOutput)
            {
                myBuffer.Render();
            }
        }
    }
}
