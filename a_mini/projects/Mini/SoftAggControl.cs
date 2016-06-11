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
        public SoftAggControl()
        {
            InitializeComponent();
            this.Load += new EventHandler(SoftAggControl_Load);
        }
        void SoftAggControl_Load(object sender, EventArgs e)
        {
            OnInitialize(myWidth, myHeight);
        }
        void OnInitialize(int width, int height)
        {
            this.gfx = bitmapBackBuffer.Initialize(width, height, 32);
            this.gfx.Clear(ColorRGBA.White);
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
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.isMouseDown = false;
            exampleBase.MouseUp(e.X, myHeight - e.Y);
            base.OnMouseUp(e);
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.isMouseDown)
            {
                exampleBase.MouseDrag(e.X, myHeight - e.Y);
                Invalidate();
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
            //--------------------------------
            exampleBase.Draw(gfx);
            //-------------------------------- 
            bitmapBackBuffer.UpdateToHardwareSurface(e.Graphics);
            //-------------------------------- 
            base.OnPaint(e);
        }
    }
}
