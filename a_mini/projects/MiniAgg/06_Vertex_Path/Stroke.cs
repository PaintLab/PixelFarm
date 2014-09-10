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
    public sealed class Stroke
    {

        StrokeGenerator strokeGen;
        public Stroke(double inWidth)
        {
            this.strokeGen = new StrokeGenerator();
            this.Width = inWidth;
        }
        StrokeGenerator GetGenerator()
        {
            return this.strokeGen;
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
            get { return this.GetGenerator().Width; }
            set { this.GetGenerator().Width = value; }
        }

        public void SetMiterLimitTheta(double t)
        {
            this.GetGenerator().SetMiterLimitTheta(t);
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
        public VertexStorage MakeVxs(VertexStorage vxs)
        {
            List<VertexData> list = new List<VertexData>();
            StrokeGenerator generator = GetGenerator();

            int j = vxs.Count;
            double x, y;

            generator.RemoveAll();
            //1st vertex

            vxs.GetVertex(0, out x, out y);
            generator.AddVertex(x, y, ShapePath.FlagsAndCommand.CommandMoveTo);
            double startX = x, startY = y;
            bool hasMoreThanOnePart = false;
            for (int i = 0; i < j; ++i)
            {
                var cmd = vxs.GetVertex(i, out x, out y);
                switch (ShapePath.FlagsAndCommand.CommandsMask & cmd)
                {
                    case ShapePath.FlagsAndCommand.CommandStop:
                        {

                        } break;
                    case ShapePath.FlagsAndCommand.CommandEndPoly:
                        {
                            generator.AddVertex(x, y, cmd);
                            if (i < j - 2)
                            {
                                generator.AddVertex(startX, startY, ShapePath.FlagsAndCommand.CommandLineTo);
                                generator.MakeVxs(list);
                                generator.RemoveAll();
                                hasMoreThanOnePart = true;
                            }
                            //end this polygon

                        } break;
                    case ShapePath.FlagsAndCommand.CommandLineTo:
                    case ShapePath.FlagsAndCommand.CommandCurve3:
                    case ShapePath.FlagsAndCommand.CommandCurve4:
                        {

                            generator.AddVertex(x, y, cmd);

                        } break;
                    case ShapePath.FlagsAndCommand.CommandMoveTo:
                        {
                            generator.AddVertex(x, y, cmd);
                            startX = x;
                            startY = y;
                        } break;
                }
            }


            generator.MakeVxs(list);
            generator.RemoveAll();
            return new VertexStorage(list, hasMoreThanOnePart);

        }


    }
    public static class StrokeHelp
    {
        public static VertexStorage MakeVxs2(VertexStorage vxs, double w)
        {
            Stroke stroke = new Stroke(w);
            return stroke.MakeVxs(vxs);
        }
        public static VertexStorage CreateStrokeVxs(VertexSnap p, double w)
        {
            Stroke stroke = new Stroke(w);
            return stroke.MakeVxs(p.GetInternalVxs());
        }
    }
}