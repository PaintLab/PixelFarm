//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    public class VisualPaintEventArgs : EventArgs
    {
        public DrawBoard canvas;
        public Rectangle updateArea;
        public VisualPaintEventArgs(DrawBoard canvas, Rectangle updateArea)
        {
            this.canvas = canvas;
            this.updateArea = updateArea;
        }
        public DrawBoard Canvas
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