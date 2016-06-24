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
    abstract public class SimpleVertexSourceWidget : IncompleteWidget
    {
        bool localBoundsComeFromPoints = true;
        Vector2 originRelativeParent;
        public SimpleVertexSourceWidget()
        {
            throw new Exception("this is depricated");
        }

        public SimpleVertexSourceWidget(Vector2 originRelativeParent, bool localBoundsComeFromPoints = true)
        {
            this.localBoundsComeFromPoints = localBoundsComeFromPoints;
            this.originRelativeParent = originRelativeParent;
        }

        public override RectD LocalBounds
        {
            get
            {
                if (localBoundsComeFromPoints)
                {
                    RectD localBounds = this.CalculateLocalBounds();
                    return localBounds;
                }
                else
                {
                    return base.LocalBounds;
                }
            }
        }


        public abstract void RewindZero();
        protected abstract RectD CalculateLocalBounds();
        public abstract VertexStore MakeVxs();
    }
}
