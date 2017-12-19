//BSD, 2014-2017, WinterDev

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007-2011
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
//
// Class StringPrinter.cs
// 
// Class to output the vertex source of a string as a run of glyphs.
//----------------------------------------------------------------------------

using PixelFarm.Drawing;
namespace PixelFarm.Agg
{
    public static class PainterExtensions
    {

        public static void Line(this Painter p, double x1, double y1, double x2, double y2, Color color)
        {
            Color prevColor = p.StrokeColor;
            p.StrokeColor = color;
            p.Line(x1, y1, x2, y2);
            p.StrokeColor = prevColor;
        }
        public static void Rectangle(this Painter p, double left, double bottom, double right, double top, Color color)
        {
            Color prevColor = p.StrokeColor;
            p.StrokeColor = color;
            p.Rectangle(left, bottom, right, top);
            p.StrokeColor = prevColor;
        }
        public static void FillCircle(this Painter p, double x, double y, double radius, Color color)
        {
            Color prevColor = p.FillColor;
            p.FillColor = color;
            p.FillCircle(x, y, radius);
            p.FillColor = prevColor;
        }
        public static void FillRectangle(this Painter p, double left, double bottom, double right, double top, Color color)
        {
            Color prevColor = p.FillColor;
            p.FillColor = color;
            p.FillRectangle(left, bottom, right, top);
            p.FillColor = prevColor;
        }
        public static void FillRectLBWH(this Painter p, double left, double bottom, double width, double height, Color color)
        {
            Color prevColor = p.FillColor;
            p.FillColor = color;
            p.FillRectLBWH(left, bottom, width, height);
            p.FillColor = prevColor;
        }
        public static void Fill(this Painter p, VertexStoreSnap snap, Color color)
        {
            Color prevColor = p.FillColor;
            p.FillColor = color;
            p.Fill(snap);
            p.FillColor = prevColor;
        }
        public static void Fill(this Painter p, VertexStore vxs, Color color)
        {
            Color prevColor = p.FillColor;
            p.FillColor = color;
            p.Fill(vxs);
            p.FillColor = prevColor;
        }
        public static void Draw(this Painter p, VertexStore vxs, Color color)
        {
            Color prevColor = p.StrokeColor;
            p.StrokeColor = color;
            p.Draw(vxs);
            p.StrokeColor = prevColor;
        }
        public static void Draw(this Painter p, VertexStoreSnap vxs, Color color)
        {
            Color prevColor = p.StrokeColor;
            p.StrokeColor = color;
            p.Draw(vxs);
            p.StrokeColor = prevColor;
        }

    }


}