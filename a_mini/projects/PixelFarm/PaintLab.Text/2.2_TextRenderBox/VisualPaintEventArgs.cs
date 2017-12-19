//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    public class VisualPaintEventArgs : EventArgs
    {
        public IDrawBoard canvas;
        public Rectangle updateArea;
        public VisualPaintEventArgs(IDrawBoard canvas, Rectangle updateArea)
        {
            this.canvas = canvas;
            this.updateArea = updateArea;
        }
        public IDrawBoard Canvas
        {
            get
            {
                return canvas;
            }
        }
        public Rectangle UpdateArea
        {
            get
            {
                return updateArea;
            }
        }
    }

    public delegate void VisualPaintEventHandler(object sender, VisualPaintEventArgs e);
}