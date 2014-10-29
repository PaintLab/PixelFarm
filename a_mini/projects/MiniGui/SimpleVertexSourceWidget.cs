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
using MatterHackers.Agg.Image;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg.UI
{
    abstract public class SimpleVertexSourceWidget : GuiWidget
    {

        bool localBoundsComeFromPoints = true;

        public SimpleVertexSourceWidget()
        {
            throw new Exception("this is depricated");
        }

        public SimpleVertexSourceWidget(Vector2 originRelativeParent, bool localBoundsComeFromPoints = true)
        {
            this.localBoundsComeFromPoints = localBoundsComeFromPoints;
            OriginRelativeParent = originRelativeParent;
        }

        public override RectangleDouble LocalBounds
        {
            get
            {
                if (localBoundsComeFromPoints)
                {
                    RectangleDouble localBounds = this.CalculateLocalBounds();
                    //new RectangleDouble(double.PositiveInfinity, double.PositiveInfinity, double.NegativeInfinity, double.NegativeInfinity);

                    //this.RewindZero();
                    //double x;
                    //double y;
                    //ShapePath.FlagsAndCommand cmd;
                    //int numPoint = 0;
                    //while (!ShapePath.IsStop(cmd = GetNextVertex(out x, out y)))
                    //{
                    //    numPoint++;
                    //    localBounds.ExpandToInclude(x, y);
                    //}

                    //if (numPoint == 0)
                    //{
                    //    localBounds = new RectangleDouble();
                    //}

                    return localBounds;
                }
                else
                {
                    return base.LocalBounds;
                }
            }

            set
            {
                if (localBoundsComeFromPoints)
                {
                    //throw new NotImplementedException();
                    base.LocalBounds = value;
                }
                else
                {
                    base.LocalBounds = value;
                }
            }
        }

        public abstract int num_paths();
        public abstract IEnumerable<VertexData> GetVertexIter();

        public abstract void RewindZero();
        public abstract ShapePath.FlagsAndCommand GetNextVertex(out double x, out double y);
        protected abstract RectangleDouble CalculateLocalBounds();

        public virtual ColorRGBA color(int i) { return new ColorRGBAf().ToColorRGBA(); }
        public override void OnDraw(Graphics2D graphics2D)
        {

            var list = new System.Collections.Generic.List<VertexData>();
            var vxs = this.MakeVxs();

            graphics2D.Render(new VertexStoreSnap(vxs, 0),
                color(0));
            base.OnDraw(graphics2D);
        }


        public abstract VertexStorage MakeVxs();
        public VertexStoreSnap MakeVertexSnap()
        {
            return new VertexStoreSnap(this.MakeVxs());
        }

    }
}
