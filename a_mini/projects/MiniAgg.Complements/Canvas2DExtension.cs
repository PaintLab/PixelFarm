using System;
using System.Collections.Generic;
using FlagsAndCommand = MatterHackers.Agg.ShapePath.FlagsAndCommand;

using MatterHackers.VectorMath;
using MatterHackers.Agg.VertexSource;
namespace MatterHackers.Agg
{
    public static class Canvas2dExtension1
    {
        public static void Circle(this Agg.Graphics2D g, double x, double y, double radius, RGBA_Bytes color)
        {
            Ellipse elipse = new Ellipse(x, y, radius, radius);
            g.Render(elipse, color);
        }
        public static void Circle(this Agg.Graphics2D g, Vector2 origin, double radius, RGBA_Bytes color)
        {
            Circle(g, origin.x, origin.y, radius, color);
        }


    }
}