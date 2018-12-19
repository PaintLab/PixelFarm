//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    public class VisualPaintEventArgs : EventArgs
    {
        public VisualPaintEventArgs(DrawBoard canvas, Rectangle updateArea)
        {
            Canvas = canvas;
            UpdateArea = updateArea;
        }
        //
        public DrawBoard Canvas { get; }
        public Rectangle UpdateArea { get; }
    }

}