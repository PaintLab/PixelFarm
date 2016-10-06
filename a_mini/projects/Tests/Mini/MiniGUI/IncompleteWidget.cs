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
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.Transform;
using PixelFarm.VectorMath;
namespace PixelFarm.Agg.UI
{
    /// <summary>
    /// incomplete widget, for test Agg Core Only
    /// </summary>
    public class IncompleteWidget
    {
        // this should probably some type of dirty rects with the current invalid set stored.

        Transform.Affine parentToChildTransform = Affine.IdentityMatrix;
        RectD localBounds;
        public virtual Vector2 OriginRelativeParent
        {
            get
            {
                Affine tempLocalToParentTransform = ParentToChildTransform;
                Vector2 originRelParent = new Vector2(tempLocalToParentTransform.tx, tempLocalToParentTransform.ty);
                return originRelParent;
            }
        }

        public virtual RectD LocalBounds
        {
            get
            {
                return localBounds;
            }
        }
        private IncompleteWidget parentBackingStore = null;
        public IncompleteWidget Parent
        {
            get
            {
                return parentBackingStore;
            }
        }

        public void Invalidate()
        {
            Invalidate(LocalBounds);
        }

        public virtual void Invalidate(RectD rectToInvalidate)
        {
            if (Parent != null)
            {
                rectToInvalidate.Offset(OriginRelativeParent);
                Parent.Invalidate(rectToInvalidate);
            }
        }


        public virtual void OnDraw(CanvasPainter p) { }

        public virtual void OnMouseDown(MouseEventArgs mouseEvent)
        {
        }

        public virtual void OnMouseMove(MouseEventArgs mouseEvent)
        {
        }
        public virtual void OnMouseUp(MouseEventArgs mouseEvent)
        {
        }



        public Affine ParentToChildTransform
        {
            get
            {
                return parentToChildTransform;
            }
            set
            {
                parentToChildTransform = value;
            }
        }
    }
}
