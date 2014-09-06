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
// conv_stroke
//
//----------------------------------------------------------------------------
using System.Collections.Generic;
namespace MatterHackers.Agg.VertexSource
{
    public sealed class Stroke : VertexSourceAdapter
    {
        public Stroke(IVertexSource vertexSource, double inWidth = 1)
            : base(vertexSource, new StrokeGenerator())
        {
            this.Width = inWidth;
        }
        public LineCap LineCap
        {
            get { return this.GetGenerator().LineCap; }
            set { this.GetGenerator().LineCap = value; }
        }
        public LineJoin LineJoin
        {
            get { return this.GetGenerator().LineJoin; }
            set { this.GetGenerator().LineJoin = value; }
        }
        public InnerJoin InnerJoin
        {
            get { return this.GetGenerator().InnerJoin; }
            set { this.GetGenerator().InnerJoin = value; }
        }
        public double MiterLimit
        {
            get { return this.GetGenerator().MiterLimit; }
            set { this.GetGenerator().MiterLimit = value; }
        }
        public double InnerMiterLimit
        {
            get { return this.GetGenerator().InnerMiterLimit; }
            set { this.GetGenerator().InnerMiterLimit = value; }
        }
        public double Width
        {
            get { return base.GetGenerator().Width; }
            set { this.GetGenerator().Width = value; }
        }

        public void SetMiterLimitTheta(double t)
        {
            base.GetGenerator().SetMiterLimitTheta(t);
        }
        public double ApproximateScale
        {
            get { return this.GetGenerator().ApproximateScale; }
            set { this.GetGenerator().ApproximateScale = value; }
        }
        public double Shorten
        {
            get { return this.GetGenerator().Shorten; }
            set { this.GetGenerator().Shorten = value; }
        }
    
       


    }
}