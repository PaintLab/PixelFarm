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
            useGdiPlusOutput = true;
            InitializeComponent();
            this.Load += new EventHandler(SoftAggControl_Load);
        }
        void SoftAggControl_Load(object sender, EventArgs e)
        {
            OnInitialize(myWidth, myHeight);
        }
        void OnInitialize(int width, int height)
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
                this.gfx = new PixelFarm.Drawing.WinGdi.CanvasGraphics2dGdi(_g);
                this.gfx.Clear(ColorRGBA.White);
                painter = new CanvasPainter(gfx);
            }
            else
            {
                this.gfx = bitmapBackBuffer.Initialize(width, height, 32);
                this.gfx.Clear(ColorRGBA.White);
                painter = new CanvasPainter(gfx);
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
