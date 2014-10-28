//BSD 2014, WinterDev

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;
using MatterHackers.Agg.Font;


namespace MatterHackers.Agg
{
    public class Painter
    {
        Graphics2D gx;
        public Painter(Graphics2D graphic2d)
        {
            this.gx = graphic2d;
        }
        public Graphics2D Graphics
        {
            get { return this.gx; }
        }
        public void Circle(double x, double y, double radius, ColorRGBA color)
        {
            Ellipse elipse = new Ellipse(x, y, radius, radius);
            gx.Render(elipse.MakeVxs(), color);
        }
        public void Line(double x1, double y1, double x2, double y2, ColorRGBA color)
        {
            PathStorage m_LinesToDraw = new PathStorage();
            m_LinesToDraw.Clear();
            m_LinesToDraw.MoveTo(x1, y1);
            m_LinesToDraw.LineTo(x2, y2);
            gx.Render(new Stroke(1).MakeVxs(m_LinesToDraw.MakeVxs()), color);
        }
        public void FillRectangle(double left, double bottom, double right, double top, ColorRGBA fillColor)
        {
            if (right < left || top < bottom)
            {
                throw new ArgumentException();
            }
            RoundedRect rect = new RoundedRect(left, bottom, right, top, 0);
            gx.Render(rect.MakeVertexSnap(), fillColor);
        }
        public void Rectangle(double left, double bottom, double right, double top, ColorRGBA color, double strokeWidth = 1)
        {
            RoundedRect rect = new RoundedRect(left + .5, bottom + .5, right - .5, top - .5, 0);
            gx.Render(new Stroke(strokeWidth).MakeVxs(rect.MakeVxs()), color);
        }
        public void DrawString(
            string text,
            double x,
            double y,
            double pointSize = 12,
            Justification justification = Justification.Left,
            Baseline baseline = Baseline.Text,
            ColorRGBA color = new ColorRGBA(),
            bool drawFromHintedCache = false,
            ColorRGBA backgroundColor = new ColorRGBA())
        {

            TypeFacePrinter stringPrinter = new TypeFacePrinter(text, pointSize, new Vector2(x, y), justification, baseline);
            if (color.Alpha0To255 == 0)
            {
                color = ColorRGBA.Black;
            }

            if (backgroundColor.Alpha0To255 != 0)
            {
                gx.FillRectangle(stringPrinter.LocalBounds, backgroundColor);
            }

            stringPrinter.DrawFromHintedCache = drawFromHintedCache;
            stringPrinter.Render(gx, color);
        }
        public void DrawString2(
           string text,
           double x,
           double y,
           double pointSize = 12,
           Justification justification = Justification.Left,
           Baseline baseline = Baseline.Text,
           ColorRGBA color = new ColorRGBA(),
           bool drawFromHintedCache = false,
           ColorRGBA backgroundColor = new ColorRGBA())
        {

            //1. parse text  
            var stringPrinter = new LayoutFarm.Agg.Font.TypeFacePrinter2(
                text,
                pointSize,
                new Vector2(x, y), justification, baseline);

            if (color.Alpha0To255 == 0)
            {
                color = ColorRGBA.Black;
            }

            if (backgroundColor.Alpha0To255 != 0)
            {
                gx.FillRectangle(stringPrinter.LocalBounds, backgroundColor);
            }

            stringPrinter.DrawFromHintedCache = drawFromHintedCache;
            stringPrinter.Render(gx, color);
        }

    }
}