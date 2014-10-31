using System;
using System.Collections.Generic;

using System.Text;

namespace PixelFarm.Agg.UI
{
    public class InvalidateEventArgs : EventArgs
    {
        RectangleDouble invalidRectangle;
        public RectangleDouble InvalidRectangle
        {
            get
            {
                return invalidRectangle;
            }
        }

        public InvalidateEventArgs(RectangleDouble invalidRectangle)
        {
            this.invalidRectangle = invalidRectangle;
        }
    }
}
