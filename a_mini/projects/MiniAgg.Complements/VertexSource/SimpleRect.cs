//MIT 2014,WinterDev 
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// Rounded rectangle vertex generator
//
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg.VertexSource
{
    //------------------------------------------------------------rounded_rect
    //
    // See Implemantation agg_rounded_rect.cpp
    //
    public class SimpleRect
    {
        RectangleDouble bounds; 
        public SimpleRect(double left, double bottom, double right, double top)
        {
           
            bounds = new RectangleDouble(left, bottom, right, top); 
            if (left > right)
            {
                bounds.Left = right;
                bounds.Right = left;
            }

            if (bottom > top)
            {
                bounds.Bottom = top;
                bounds.Top = bottom;
            }
        } 
        public void SetRect(double left, double bottom, double right, double top)
        {
            bounds = new RectangleDouble(left, bottom, right, top);
            if (left > right) { bounds.Left = right; bounds.Right = left; }
            if (bottom > top) { bounds.Bottom = top; bounds.Top = bottom; }
        }
          
        public VertexStorage MakeVxs()
        {
            PathStorage m_LinesToDraw = new PathStorage();
            m_LinesToDraw.Clear();             
            m_LinesToDraw.MoveTo(bounds.Left, bounds.Bottom);
            m_LinesToDraw.LineTo(bounds.Right, bounds.Bottom);
            m_LinesToDraw.LineTo(bounds.Right, bounds.Top);
            m_LinesToDraw.LineTo(bounds.Left, bounds.Top);
            m_LinesToDraw.ClosePolygon();
            return m_LinesToDraw.Vxs;
        }
        public VertexStoreSnap MakeVertexSnap()
        {
            return new VertexStoreSnap(this.MakeVxs());
        }

    }
}

