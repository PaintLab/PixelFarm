//2014 BSD,WinterDev   
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
namespace PixelFarm.Agg.VertexSource
{
    public sealed class Stroke
    {

        StrokeGenerator strokeGen;
        public Stroke(double inWidth)
        {
            this.strokeGen = new StrokeGenerator();
            this.Width = inWidth;
        }

        public LineCap LineCap
        {
            get { return strokeGen.LineCap; }
            set { strokeGen.LineCap = value; }
        }
        public LineJoin LineJoin
        {
            get { return strokeGen.LineJoin; }
            set { strokeGen.LineJoin = value; }
        }
        public InnerJoin InnerJoin
        {
            get { return strokeGen.InnerJoin; }
            set { strokeGen.InnerJoin = value; }
        }
        public double MiterLimit
        {
            get { return strokeGen.MiterLimit; }
            set { strokeGen.MiterLimit = value; }
        }
        public double InnerMiterLimit
        {
            get { return strokeGen.InnerMiterLimit; }
            set { strokeGen.InnerMiterLimit = value; }
        }
        public double Width
        {
            get { return strokeGen.Width; }
            set { strokeGen.Width = value; }
        }

        public void SetMiterLimitTheta(double t)
        {
            strokeGen.SetMiterLimitTheta(t);
        }
        public double ApproximateScale
        {
            get { return strokeGen.ApproximateScale; }
            set { strokeGen.ApproximateScale = value; }
        }
        public double Shorten
        {
            get { return strokeGen.Shorten; }
            set { strokeGen.Shorten = value; }
        }
        public VertexStore MakeVxs(VertexStore sourceVxs)
        {

            StrokeGenerator stgen = strokeGen;
            VertexStore vxs = new VertexStore();

            int j = sourceVxs.Count;
            double x, y;

            stgen.RemoveAll();
            //1st vertex

            sourceVxs.GetVertex(0, out x, out y);
            stgen.AddVertex(x, y, ShapePath.FlagsAndCommand.CommandMoveTo);

            double startX = x, startY = y;
            bool hasMoreThanOnePart = false;
            for (int i = 0; i < j; ++i)
            {
                var cmd = sourceVxs.GetVertex(i, out x, out y);
                switch (ShapePath.FlagsAndCommand.CommandsMask & cmd)
                {
                    case ShapePath.FlagsAndCommand.CommandStop:
                        {

                        } break;
                    case ShapePath.FlagsAndCommand.CommandEndPoly:
                        {
                            stgen.AddVertex(x, y, cmd);
                            if (i < j - 2)
                            {
                                stgen.AddVertex(startX, startY, ShapePath.FlagsAndCommand.CommandLineTo);
                                stgen.WriteTo(vxs);
                                stgen.RemoveAll();
                                hasMoreThanOnePart = true;
                            }
                            //end this polygon

                        } break;
                    case ShapePath.FlagsAndCommand.CommandLineTo:
                    case ShapePath.FlagsAndCommand.CommandCurve3:
                    case ShapePath.FlagsAndCommand.CommandCurve4:
                        {

                            stgen.AddVertex(x, y, cmd);

                        } break;
                    case ShapePath.FlagsAndCommand.CommandMoveTo:
                        {
                            stgen.AddVertex(x, y, cmd);
                            startX = x;
                            startY = y;
                        } break;
                }
            }
            stgen.WriteTo(vxs);
            stgen.RemoveAll();

            vxs.HasMoreThanOnePart = hasMoreThanOnePart;
            return vxs;

        }


    }
    public static class StrokeHelp
    {
        public static VertexStore MakeVxs(VertexStore vxs, double w)
        {
            Stroke stroke = new Stroke(w);
            return stroke.MakeVxs(vxs);
        }

    }
}