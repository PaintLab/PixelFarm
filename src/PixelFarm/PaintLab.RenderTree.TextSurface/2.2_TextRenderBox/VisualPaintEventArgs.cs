//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    public class VisualPaintEventArgs : EventArgs
    {
        public DrawBoard _canvas;
        public Rectangle _updateArea;
        public VisualPaintEventArgs(DrawBoard canvas, Rectangle updateArea)
        {
            _canvas = canvas;
            _updateArea = updateArea;
        }
        //
        public DrawBoard Canvas => _canvas;
        //
        public Rectangle UpdateArea => _updateArea;
        //
    }

    public delegate void VisualPaintEventHandler(object sender, VisualPaintEventArgs e);
}