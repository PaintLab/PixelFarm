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
            this._canvas = canvas;
            this._updateArea = updateArea;
        }
        public DrawBoard Canvas
        {
            get
            {
                return _canvas;
            }
        }
        public Rectangle UpdateArea
        {
            get
            {
                return _updateArea;
            }
        }
    }

    public delegate void VisualPaintEventHandler(object sender, VisualPaintEventArgs e);
}