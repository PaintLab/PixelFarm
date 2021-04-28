﻿//BSD, 2014-present, WinterDev
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
// Arc vertex generator
//
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.VectorMath;

namespace PixelFarm.CpuBlit.VertexProcessing
{


    public static class VertexSourceExtensions
    {

        public static void AddMoveTo(this VertexStore vxs, double x, double y, ICoordTransformer tx)
        {
            tx.Transform(ref x, ref y);
            vxs.AddMoveTo(x, y);
        }
        public static void AddLineTo(this VertexStore vxs, double x, double y, ICoordTransformer tx)
        {
            tx.Transform(ref x, ref y);
            vxs.AddLineTo(x, y);
        }



        public static VertexStore MakeVxs(this Ellipse ellipse, ICoordTransformer tx, VertexStore output)
        {
            //1. moveto
            output.AddMoveTo(ellipse.originX + ellipse.radiusX, ellipse.originY, tx);//**

            //2.
            //
            int numSteps = ellipse.NumSteps;
            double anglePerStep = MathHelper.Tau / numSteps;
            double angle = 0;


            double orgX = ellipse.originX;
            double orgY = ellipse.originY;
            double radX = ellipse.radiusX;
            double radY = ellipse.radiusY;
            if (ellipse._cw)
            {
                for (int i = 1; i < numSteps; i++)
                {
                    angle += anglePerStep;
                    output.AddLineTo(
                        orgX + Math.Cos(MathHelper.Tau - angle) * radX,
                        orgY + Math.Sin(MathHelper.Tau - angle) * radY,
                        tx);//**
                }
            }
            else
            {
                for (int i = 1; i < numSteps; i++)
                {
                    angle += anglePerStep;
                    output.AddLineTo(
                     orgX + Math.Cos(angle) * radX,
                     orgY + Math.Sin(angle) * radY,
                     tx);//**
                }
            }


            //3.
            output.AddCloseFigure((int)EndVertexOrientation.CCW, 0);
            //4. 
            output.AddNoMore();

            return output;
        }

        public static VertexStore MakeVxs(this Ellipse ellipse, VertexStore output)
        {
            //1. moveto
            output.AddMoveTo(ellipse.originX + ellipse.radiusX, ellipse.originY);

            //2.
            //
            int numSteps = ellipse.NumSteps;
            double anglePerStep = MathHelper.Tau / numSteps;
            double angle = 0;


            double orgX = ellipse.originX;
            double orgY = ellipse.originY;
            double radX = ellipse.radiusX;
            double radY = ellipse.radiusY;
            if (ellipse._cw)
            {
                for (int i = 1; i < numSteps; i++)
                {
                    angle += anglePerStep;
                    output.AddLineTo(
                        orgX + Math.Cos(MathHelper.Tau - angle) * radX,
                        orgY + Math.Sin(MathHelper.Tau - angle) * radY);
                }
            }
            else
            {
                for (int i = 1; i < numSteps; i++)
                {
                    angle += anglePerStep;
                    output.AddLineTo(
                     orgX + Math.Cos(angle) * radX,
                     orgY + Math.Sin(angle) * radY);
                }
            }


            //3.
            output.AddCloseFigure((int)EndVertexOrientation.CCW, 0);
            //4. 
            output.AddNoMore();

            return output;
        }
        public static void MakeVxs(this Ellipse ellipse, PathWriter writer)
        {
            //1. moveto
            writer.MoveTo(ellipse.originX + ellipse.radiusX, ellipse.originY);
            //2.
            //
            int numSteps = ellipse.NumSteps;
            double anglePerStep = MathHelper.Tau / numSteps;
            double angle = 0;


            double orgX = ellipse.originX;
            double orgY = ellipse.originY;
            double radX = ellipse.radiusX;
            double radY = ellipse.radiusY;
            if (ellipse._cw)
            {
                for (int i = 1; i < numSteps; i++)
                {
                    angle += anglePerStep;
                    writer.LineTo(
                        orgX + Math.Cos(MathHelper.Tau - angle) * radX,
                        orgY + Math.Sin(MathHelper.Tau - angle) * radY);
                }
            }
            else
            {
                for (int i = 1; i < numSteps; i++)
                {
                    angle += anglePerStep;
                    writer.LineTo(
                     orgX + Math.Cos(angle) * radX,
                     orgY + Math.Sin(angle) * radY);
                }
            }


            //3.
            //output.AddCloseFigure((int)EndVertexOrientation.CCW, 0);
            writer.CloseFigure();
            //4.              
            //add no more?
        }
        public static void MakeVxs(this Arc arc, VertexStore v)
        {
            foreach (VertexData vertexData in arc.GetVertexIter())
            {
                if (VertexHelper.IsEmpty(vertexData.command))
                {
                    break;
                }
                v.AddVertex(vertexData.x, vertexData.y, vertexData.command);
            }
        }
        public static IEnumerable<VertexData> GetVertexIter(this Arc arc)
        {
            // go to the start
            if (arc.UseStartEndLimit)
            {
                //---------------------------------------------------------
                VertexData vertexData = new VertexData();
                vertexData.command = VertexCmd.MoveTo;
                vertexData.x = arc.StartX;
                vertexData.y = arc.StartY;
                yield return vertexData;
                //---------------------------------------------------------
                double angle = arc.StartAngle;
                vertexData.command = VertexCmd.LineTo;
                //calculate nsteps
                int calculateNSteps = arc.CalculateNSteps;

                int n = 0;
                double radX = arc.RadiusX;
                double radY = arc.RadiusY;
                double flatternDeltaAngle = arc.FlattenDeltaAngle;
                double orgX = arc.OriginX;
                double orgY = arc.OriginY;

                while (n < calculateNSteps - 1)
                {
                    angle += flatternDeltaAngle;
                    vertexData.x = orgX + Math.Cos(angle) * radX;
                    vertexData.y = orgY + Math.Sin(angle) * radY;
                    yield return vertexData;
                    n++;
                }

                //while ((angle < endAngle - flatenDeltaAngle / 4) == (((int)ArcDirection.CounterClockWise) == 1))
                //{
                //    angle += flatenDeltaAngle;
                //    vertexData.x = originX + Math.Cos(angle) * radiusX;
                //    vertexData.y = originY + Math.Sin(angle) * radiusY;

                //    yield return vertexData;
                //}
                //---------------------------------------------------------
                vertexData.x = arc.EndX;
                vertexData.y = arc.EndY;
                yield return vertexData;
                vertexData.command = VertexCmd.NoMore;
                yield return vertexData;
            }
            else
            {
                double originX = arc.OriginX;
                double originY = arc.OriginY;
                double startAngle = arc.StartAngle;
                double radX = arc.RadiusX;
                double radY = arc.RadiusY;
                VertexData vertexData = new VertexData();
                vertexData.command = VertexCmd.MoveTo;
                vertexData.x = originX + Math.Cos(startAngle) * radX;
                vertexData.y = originY + Math.Sin(startAngle) * radY;
                yield return vertexData;
                //---------------------------------------------------------
                double angle = startAngle;
                double endAngle = arc.EndY;
                double flatternDeltaAngle = arc.FlattenDeltaAngle;
                vertexData.command = VertexCmd.LineTo;
                while ((angle < endAngle - flatternDeltaAngle / 4) == (((int)Arc.ArcDirection.CounterClockWise) == 1))
                {
                    angle += flatternDeltaAngle;
                    vertexData.x = originX + Math.Cos(angle) * radX;
                    vertexData.y = originY + Math.Sin(angle) * radY;
                    yield return vertexData;
                }
                //---------------------------------------------------------
                vertexData.x = originX + Math.Cos(endAngle) * radX;
                vertexData.y = originY + Math.Sin(endAngle) * radY;
                yield return vertexData;
                vertexData.command = VertexCmd.NoMore;
                yield return vertexData;
            }
        }

        public static VertexStore CreateVxs(IEnumerable<VertexData> iter, VertexStore output)
        {

            foreach (VertexData v in iter)
            {
                output.AddVertex(v.x, v.y, v.command);
            }
            return output;
        }


        public static bool IsClockwise(this VertexStore flattenVxs)
        {
            //TODO: review here again***
            //---------------
            //http://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
            //check if hole or not
            //clockwise or counter-clockwise 
            //Some of the suggested methods will fail in the case of a non-convex polygon, such as a crescent. 
            //Here's a simple one that will work with non-convex polygons (it'll even work with a self-intersecting polygon like a figure-eight, telling you whether it's mostly clockwise).

            //Sum over the edges, (x2 − x1)(y2 + y1). 
            //If the result is positive the curve is clockwise,
            //if it's negative the curve is counter-clockwise. (The result is twice the enclosed area, with a +/- convention.)
            double total = 0;
            int j = flattenVxs.Count;

            for (int i = 1; i < j; ++i)
            {
                flattenVxs.GetVertex(i - 1, out double x0, out double y0);
                flattenVxs.GetVertex(i, out double x1, out double y1);
                total += (x1 - x0) * (y1 + y0);
            }
            //the last one
            {
                flattenVxs.GetVertex(j - 1, out double x0, out double y0);
                flattenVxs.GetVertex(0, out double x1, out double y1);
                total += (x1 - x0) * (y1 + y0);
            }
            return total >= 0; //             
        }
    }

}