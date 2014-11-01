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
// bounding_rect function template
//
//----------------------------------------------------------------------------
using PixelFarm.Agg.VertexSource;

namespace PixelFarm.Agg
{
    public static class BoundingRect
    {


        public static bool GetBoundingRect(PathStorage vs, int[] gi,
                           int num,
                           out RectangleDouble boundingRect)
        {
            return GetBoundingRect(vs, gi, num, out boundingRect.Left, out boundingRect.Bottom, out boundingRect.Right, out boundingRect.Top);
        }
        public static bool GetBoundingRectSingle(VertexStoreSnap vs, ref RectangleDouble rect)
        {
            double x1, y1, x2, y2;
            bool rValue = GetBoundingRectSingle(vs, out x1, out y1, out x2, out y2);
            rect.Left = x1;
            rect.Bottom = y1;
            rect.Right = x2;
            rect.Top = y2;
            return rValue;
        }

        //----------------------------------
        static bool GetBoundingRect(PathStorage vs, int[] gi,
                         int num,
                         out double x1,
                         out double y1,
                         out double x2,
                         out double y2)
        {
            int i;
            double x = 0;
            double y = 0;
            bool first = true;
            var vxs = vs.Vsx;

            x1 = 1;
            y1 = 1;
            x2 = 0;
            y2 = 0;


            int iterindex = 0;
            for (i = 0; i < num; i++)
            {

                ShapePath.FlagsAndCommand flags;
                while ((flags = vxs.GetVertex(iterindex++, out x, out y)) != ShapePath.FlagsAndCommand.CommandStop)
                {
                    switch (flags)
                    {
                        //if is vertext cmd
                        case ShapePath.FlagsAndCommand.CommandLineTo:
                        case ShapePath.FlagsAndCommand.CommandMoveTo:
                        case ShapePath.FlagsAndCommand.CommandCurve3:
                        case ShapePath.FlagsAndCommand.CommandCurve4:
                            {
                                if (first)
                                {
                                    x1 = x;
                                    y1 = y;
                                    x2 = x;
                                    y2 = y;
                                    first = false;
                                }
                                else
                                {
                                    if (x < x1) x1 = x;
                                    if (y < y1) y1 = y;
                                    if (x > x2) x2 = x;
                                    if (y > y2) y2 = y;
                                }

                            } break;
                    }
                }
            }
            return x1 <= x2 && y1 <= y2;
        }

        //-----------------------------------------------------bounding_rect_single
        //template<class VertexSource, class CoordT> 
        static bool GetBoundingRectSingle(
          VertexStoreSnap vs,
          out double x1, out double y1,
          out double x2, out double y2)
        {
            double x = 0;
            double y = 0;
            bool first = true;

            x1 = 1;
            y1 = 1;
            x2 = 0;
            y2 = 0;

            var vsnapIter = vs.GetVertexSnapIter();

            ShapePath.FlagsAndCommand PathAndFlags;
            while (!ShapePath.IsStop(PathAndFlags = vsnapIter.GetNextVertex(out x, out y)))
            {


                if (ShapePath.IsVertextCommand(PathAndFlags))
                {
                    if (first)
                    {
                        x1 = x;
                        y1 = y;
                        x2 = x;
                        y2 = y;
                        first = false;
                    }
                    else
                    {
                        if (x < x1) x1 = x;
                        if (y < y1) y1 = y;
                        if (x > x2) x2 = x;
                        if (y > y2) y2 = y;
                    }
                }
            }
            return x1 <= x2 && y1 <= y2;
        }
    }


    //----------------------------------------------------
    public static class BoundingRectInt
    {


        public static bool GetBoundingRect(PathStorage vs, int[] gi,
                           int num,
                           out RectangleInt boundingRect)
        {
            return GetBoundingRect(vs, gi, num, out boundingRect.Left, out boundingRect.Bottom, out boundingRect.Right, out boundingRect.Top);
        }
        public static bool GetBoundingRectSingle(VertexStoreSnap vs, ref RectangleInt rect)
        {
            int x1, y1, x2, y2;
            bool rValue = GetBoundingRectSingle(vs, out x1, out y1, out x2, out y2);
            rect.Left = x1;
            rect.Bottom = y1;
            rect.Right = x2;
            rect.Top = y2;
            return rValue;
        }

        static bool GetBoundingRect(PathStorage vs, int[] gi,
                         int num,
                         out int x1,
                         out int y1,
                         out int x2,
                         out int y2)
        {
            int i;

            int x = 0;
            int y = 0;

            bool first = true;
            var vxs = vs.Vsx;

            x1 = 1;
            y1 = 1;
            x2 = 0;
            y2 = 0;

            double x_d = 0;
            double y_d = 0;
            int iterindex = 0;
            for (i = 0; i < num; i++)
            {

                ShapePath.FlagsAndCommand flags;
                while ((flags = vxs.GetVertex(iterindex++, out x_d, out y_d)) != ShapePath.FlagsAndCommand.CommandStop)
                {
                    x = (int)x_d;
                    y = (int)y_d;

                    switch (flags)
                    {
                        //if is vertext cmd
                        case ShapePath.FlagsAndCommand.CommandLineTo:
                        case ShapePath.FlagsAndCommand.CommandMoveTo:
                        case ShapePath.FlagsAndCommand.CommandCurve3:
                        case ShapePath.FlagsAndCommand.CommandCurve4:
                            {
                                if (first)
                                {
                                    x1 = x;
                                    y1 = y;
                                    x2 = x;
                                    y2 = y;
                                    first = false;
                                }
                                else
                                {
                                    if (x < x1) x1 = x;
                                    if (y < y1) y1 = y;
                                    if (x > x2) x2 = x;
                                    if (y > y2) y2 = y;
                                }

                            } break;
                    }
                }
            }
            return x1 <= x2 && y1 <= y2;
        }

        //-----------------------------------------------------bounding_rect_single
        //template<class VertexSource, class CoordT> 
        static bool GetBoundingRectSingle(
          VertexStoreSnap vs,
          out int x1, out int y1,
          out int x2, out int y2)
        {
            double x_d = 0;
            double y_d = 0;

            int x = 0;
            int y = 0;
            bool first = true;

            x1 = 1;
            y1 = 1;
            x2 = 0;
            y2 = 0;

            var vsnapIter = vs.GetVertexSnapIter();

            ShapePath.FlagsAndCommand PathAndFlags;
            while (!ShapePath.IsStop(PathAndFlags = vsnapIter.GetNextVertex(out x_d, out y_d)))
            {
                x = (int)x_d;
                y = (int)y_d;
                if (ShapePath.IsVertextCommand(PathAndFlags))
                {
                    if (first)
                    {
                        x1 = x;
                        y1 = y;
                        x2 = x;
                        y2 = y;
                        first = false;
                    }
                    else
                    {
                        if (x < x1) x1 = x;
                        if (y < y1) y1 = y;
                        if (x > x2) x2 = x;
                        if (y > y2) y2 = y;
                    }
                }
            }
            return x1 <= x2 && y1 <= y2;
        }
    }
}

//#endif
