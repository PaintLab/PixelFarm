using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.VectorMath;
namespace PixelFarm.Agg.UI
{
    public enum MouseButtons
    {
        None = 0,
        Left = 1048576,
        Right = 2097152,
        Middle = 4194304,
        XButton1 = 8388608,
        XButton2 = 16777216,
    }

    public class MouseEventArgs : EventArgs
    {
        private MouseButtons mouseButtons;
        private int numClicks;
        private double x;
        private double y;
        private int wheelDelta;
        public MouseEventArgs(MouseEventArgs original, double newX, double newY)
            : this(original.Button, original.Clicks, newX, newY, original.WheelDelta)
        {
        }

        public MouseEventArgs(MouseButtons button, int clicks, double x, double y, int wheelDelta)
        {
            mouseButtons = button;
            numClicks = clicks;
            this.x = x;
            this.y = y;
            this.wheelDelta = wheelDelta;
        }

        public MouseButtons Button { get { return mouseButtons; } }
        public int Clicks { get { return numClicks; } }
        public int WheelDelta { get { return wheelDelta; } set { wheelDelta = value; } }

        public double X { get { return x; } set { x = value; } }
        public double Y { get { return y; } set { y = value; } }
        public Vector2 Position { get { return new Vector2(x, y); } }
    }
}
