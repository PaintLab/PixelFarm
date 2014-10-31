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
using System;
using System.Collections.Generic;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;

namespace PixelFarm.Agg.UI
{
    abstract public class MySimpleVertexSourceWidget : IncompleteWidget
    {

        bool localBoundsComeFromPoints = true;
        Vector2 originRelativeParent;
        public MySimpleVertexSourceWidget()
        {
            throw new Exception("this is depricated");
        }

        public MySimpleVertexSourceWidget(Vector2 originRelativeParent, bool localBoundsComeFromPoints = true)
        {
            this.localBoundsComeFromPoints = localBoundsComeFromPoints;
            this.originRelativeParent = originRelativeParent;
        }

        public override RectangleDouble LocalBounds
        {
            get
            {
                if (localBoundsComeFromPoints)
                {
                    RectangleDouble localBounds = this.CalculateLocalBounds(); 
                    return localBounds;
                }
                else
                {
                    return base.LocalBounds;
                }
            } 
        }

    
        public abstract void RewindZero(); 
        protected abstract RectangleDouble CalculateLocalBounds();

        public virtual ColorRGBA WidgetColor() { return new ColorRGBAf().ToColorRGBA(); }
        public override void OnDraw(Graphics2D graphics2D)
        { 
            graphics2D.Render(new VertexStoreSnap(this.MakeVxs()), WidgetColor(0=));

            base.OnDraw(graphics2D);
        } 
        public abstract VertexStore MakeVxs();
        
    }
}
