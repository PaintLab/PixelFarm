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
using LayoutFarm.Agg.Font;

namespace LayoutFarm.Canvas2dExtension
{
    public static class Canvas2dExtension
    {

        public static void DrawString2(this Graphics2D gx, string text, double x, double y, double pointSize = 12,
           Justification justification = Justification.Left,
           Baseline baseline = Baseline.Text,
           RGBA_Bytes color = new RGBA_Bytes(),
           bool drawFromHintedCache = false,
           RGBA_Bytes backgroundColor = new RGBA_Bytes())
        {

            //1. parse text 
            
            var stringPrinter = new LayoutFarm.Agg.Font.TypeFacePrinter2(
                text,
                pointSize,
                new Vector2(x, y), justification, baseline);

            if (color.Alpha0To255 == 0)
            {
                color = RGBA_Bytes.Black;
            }

            if (backgroundColor.Alpha0To255 != 0)
            {
                gx.FillRectangle(stringPrinter.LocalBounds, backgroundColor);
            }

            stringPrinter.DrawFromHintedCache = drawFromHintedCache;
            stringPrinter.Render(gx, color);
        }

        public static void DrawString3(this Graphics2D gx, string Text,
           double x, double y,
           double pointSize = 12,
           Justification justification = Justification.Left,
           Baseline baseline = Baseline.Text,
           RGBA_Bytes color = new RGBA_Bytes(),
           bool drawFromHintedCache = false,
           RGBA_Bytes backgroundColor = new RGBA_Bytes())
        {

            var stringPrinter = new LayoutFarm.Agg.Font.TypeFacePrinter2(
                Text,
                pointSize,
                new Vector2(x, y),
                justification, baseline);

            if (color.Alpha0To255 == 0)
            {
                color = RGBA_Bytes.Black;
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