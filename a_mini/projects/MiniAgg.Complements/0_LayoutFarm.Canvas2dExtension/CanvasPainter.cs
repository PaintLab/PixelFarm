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
    public class CanvasPainter
    {
        Graphics2D gx;
        Stroke stroke;

        public CanvasPainter(Graphics2D graphic2d)
        {
            this.gx = graphic2d;
            stroke = new Stroke(1);//default
        }
        public void Clear(ColorRGBA color)
        {
            gx.Clear(color);
        }
        public Graphics2D Graphics
        {
            get { return this.gx; }
        }
        /// <summary>
        /// draw circle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public void Circle(double x, double y, double radius, ColorRGBA color)
        {
            Ellipse elipse = new Ellipse(x, y, radius, radius);
            gx.Render(elipse.MakeVxs(), color);
        }
        /// <summary>
        /// draw line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public void Line(double x1, double y1, double x2, double y2, ColorRGBA color)
        {
            PathStorage m_LinesToDraw = new PathStorage();
            m_LinesToDraw.Clear();
            m_LinesToDraw.MoveTo(x1, y1);
            m_LinesToDraw.LineTo(x2, y2);
            gx.Render(stroke.MakeVxs(m_LinesToDraw.Vxs), color);
        }
        public double StrokeWidth
        {
            get { return this.stroke.Width; }
            set { this.stroke.Width = value; }
        }
        /// <summary>
        /// draw rectangle
        /// </summary>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="color"></param>
        /// <param name="strokeWidth"></param>
        public void Rectangle(double left, double bottom, double right, double top, ColorRGBA color)
        {
            SimpleRect simpleRect = new SimpleRect(left + .5, bottom + .5, right - .5, top - .5);
            gx.Render(stroke.MakeVxs(simpleRect.MakeVxs()), color);
        }

        public void FillRectangle(double left, double bottom, double right, double top, ColorRGBA fillColor)
        {
            if (right < left || top < bottom)
            {
                throw new ArgumentException();
            }
            SimpleRect rect = new SimpleRect(left, bottom, right, top);
            gx.Render(rect.MakeVertexSnap(), fillColor);
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