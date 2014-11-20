//2014 BSD,WinterDev   
using System;
using System.Collections.Generic;

using System.Text;

namespace PixelFarm.Agg
{

    public static class ShapePath
    {
        [Flags]
        public enum FlagsAndCommand : byte
        {
            //first lower 4 bits compact flags
            CommandEmpty = 0x00,

            CommandMoveTo = 0x01,
            CommandLineTo = 0x02,
            CommandCurve3 = 0x03,
            CommandCurve4 = 0x04,


            CmdEndFigure = 0x0F,

            CommandsMask = 0x0F,
            //-----------------------
            //upper 4 bits
            FlagNone = 0x00,
            FlagCCW = 1 << (5 - 1),
            FlagCW = 1 << (6 - 1),
            FlagClose = 1 << (7 - 1),

            FlagsMask = 0xF0
        }

        public static bool IsVertextCommand(FlagsAndCommand c)
        {
            return c >= FlagsAndCommand.CommandMoveTo
                && c < FlagsAndCommand.CmdEndFigure;
        }


        public static bool IsStop(FlagsAndCommand c)
        {
            return c == FlagsAndCommand.CommandEmpty;
        }

        public static bool IsMoveTo(FlagsAndCommand c)
        {
            return c == FlagsAndCommand.CommandMoveTo;
        }

        public static bool IsCurve(FlagsAndCommand c)
        {
            return c == FlagsAndCommand.CommandCurve3
                || c == FlagsAndCommand.CommandCurve4;
        }



        public static bool IsEndPoly(FlagsAndCommand c)
        {
            return (c & FlagsAndCommand.CommandsMask) == FlagsAndCommand.CmdEndFigure;
        }

        public static bool IsClose(FlagsAndCommand c)
        {
            return (c & ~(FlagsAndCommand.FlagCW | FlagsAndCommand.FlagCCW)) ==
                   (FlagsAndCommand.CmdEndFigure | FlagsAndCommand.FlagClose);
        }

        public static bool IsNextPoly(FlagsAndCommand c)
        {
            return IsStop(c) || IsMoveTo(c) || IsEndPoly(c);
        }

        public static bool IsCw(FlagsAndCommand c)
        {
            return (c & FlagsAndCommand.FlagCW) != 0;
        }

        public static bool IsCcw(FlagsAndCommand c)
        {
            return (c & FlagsAndCommand.FlagCCW) != 0;
        }

        public static bool HasOrientationInfo(FlagsAndCommand c)
        {
            return (c & (FlagsAndCommand.FlagCW | FlagsAndCommand.FlagCCW)) != 0;
        }

        //public static bool IsClosed(FlagsAndCommand c)
        //{
        //    return (c & FlagsAndCommand.FlagClose) != 0;
        //}

        public static FlagsAndCommand GetCloseFlags(FlagsAndCommand c)
        {
            return (FlagsAndCommand)(c & FlagsAndCommand.FlagClose);
        }

        public static FlagsAndCommand ClearOritentation(FlagsAndCommand c)
        {
            return c & ~(FlagsAndCommand.FlagCW | FlagsAndCommand.FlagCCW);
        }

        public static FlagsAndCommand GetOrientation(FlagsAndCommand c)
        {
            return c & (FlagsAndCommand.FlagCW | FlagsAndCommand.FlagCCW);
        }

        /*
        //---------------------------------------------------------set_orientation
        public static path_flags_e set_orientation(int c, path_flags_e o)
        {
            return clear_orientation(c) | o;
        }
         */

        //static public void shorten_path(MatterHackers.Agg.VertexSequence vs, double s)
        //{
        //    shorten_path(vs, s, 0);
        //}

        public static void ShortenPath(VertexSequence vs, double s, int closed)
        {
            if (s > 0.0 && vs.Count > 1)
            {
                double d;
                int n = (int)(vs.Count - 2);
                while (n != 0)
                {
                    d = vs[n].dist;
                    if (d > s) break;
                    vs.RemoveLast();
                    s -= d;
                    --n;
                }
                if (vs.Count < 2)
                {
                    vs.Clear();
                }
                else
                {
                    n = (int)vs.Count - 1;
                    VertexDistance prev = vs[n - 1];
                    VertexDistance last = vs[n];
                    d = (prev.dist - s) / prev.dist;
                    double x = prev.x + (last.x - prev.x) * d;
                    double y = prev.y + (last.y - prev.y) * d;
                    last.x = x;
                    last.y = y;
                    if (!prev.IsEqual(last))
                    {
                        vs.RemoveLast();
                    }
                    vs.Close(closed != 0);
                }
            }
        }



        public static void ArrangeOrientationsAll(VertexStore myvxs, bool closewise)
        {
            int start = 0;
            while (start < myvxs.Count)
            {
                start = ArrangeOrientations(myvxs, start, closewise);
            }
        }

        //----------------------------------------------------------------

        // Arrange the orientation of a polygon, all polygons in a path, 
        // or in all paths. After calling arrange_orientations() or 
        // arrange_orientations_all_paths(), all the polygons will have 
        // the same orientation, i.e. path_flags_cw or path_flags_ccw
        //--------------------------------------------------------------------
        static int ArrangePolygonOrientation(VertexStore myvxs, int start, bool clockwise)
        {
            //if (orientation == ShapePath.FlagsAndCommand.FlagNone) return start;

            // Skip all non-vertices at the beginning
            //ShapePath.FlagsAndCommand orientFlags = clockwise ? ShapePath.FlagsAndCommand.FlagCW : ShapePath.FlagsAndCommand.FlagCCW;

            int vcount = myvxs.Count;
            while (start < vcount &&
                  !ShapePath.IsVertextCommand(myvxs.GetCommand(start)))
            {
                ++start;
            }

            // Skip all insignificant move_to
            while (start + 1 < vcount &&
                  ShapePath.IsMoveTo(myvxs.GetCommand(start)) &&
                  ShapePath.IsMoveTo(myvxs.GetCommand(start + 1)))
            {
                ++start;
            }

            // Find the last vertex
            int end = start + 1;
            while (end < vcount &&
                  !ShapePath.IsNextPoly(myvxs.GetCommand(end)))
            {
                ++end;
            }
            if (end - start > 2)
            {
                bool isCW;
                if ((isCW = IsCW(myvxs, start, end)) != clockwise)
                {
                    // Invert polygon, set orientation flag, and skip all end_poly
                    InvertPolygon(myvxs, start, end);
                    ShapePath.FlagsAndCommand flags;
                    int myvxs_count = myvxs.Count;

                    var orientFlags = isCW ? ShapePath.FlagsAndCommand.FlagCW : ShapePath.FlagsAndCommand.FlagCCW;

                    while (end < myvxs_count &&
                          ShapePath.IsEndPoly(flags = myvxs.GetCommand(end)))
                    {
                        myvxs.ReplaceCommand(end++, flags | orientFlags);// Path.set_orientation(cmd, orientation));
                    }
                }
            }
            return end;
        }

        static int ArrangeOrientations(VertexStore myvxs, int start, bool closewise)
        {

            while (start < myvxs.Count)
            {
                start = ArrangePolygonOrientation(myvxs, start, closewise);
                if (ShapePath.IsStop(myvxs.GetCommand(start)))
                {
                    ++start;
                    break;
                }
            }

            return start;
        }
        static bool IsCW(VertexStore myvxs, int start, int end)
        {
            // Calculate signed area (double area to be exact)
            //---------------------
            int np = end - start;
            double area = 0.0;
            int i;
            for (i = 0; i < np; i++)
            {
                double x1, y1, x2, y2;
                myvxs.GetVertexXY(start + i, out x1, out y1);
                myvxs.GetVertexXY(start + (i + 1) % np, out x2, out y2);
                area += x1 * y2 - y1 * x2;
            }
            return (area < 0.0);
            //return (area < 0.0) ? ShapePath.FlagsAndCommand.FlagCW : ShapePath.FlagsAndCommand.FlagCCW;
        }
        //--------------------------------------------------------------------
        public static void InvertPolygon(VertexStore myvxs, int start)
        {
            // Skip all non-vertices at the beginning
            int vcount = myvxs.Count;

            while (start < vcount &&
                  !ShapePath.IsVertextCommand(myvxs.GetCommand(start))) { ++start; }

            // Skip all insignificant move_to
            while (start + 1 < vcount &&
                  ShapePath.IsMoveTo(myvxs.GetCommand(start)) &&
                  ShapePath.IsMoveTo(myvxs.GetCommand(start + 1))) { ++start; }

            // Find the last vertex
            int end = start + 1;
            while (end < vcount  &&
                  !ShapePath.IsNextPoly(myvxs.GetCommand(end))) { ++end; }

            InvertPolygon(myvxs, start, end);
        }



        static void InvertPolygon(VertexStore myvxs, int start, int end)
        {
            int i;
            ShapePath.FlagsAndCommand tmp_PathAndFlags = myvxs.GetCommand(start);

            --end; // Make "end" inclusive

            // Shift all commands to one position
            for (i = start; i < end; i++)
            {
                myvxs.ReplaceCommand(i, myvxs.GetCommand(i + 1));
            }

            // Assign starting command to the ending command
            myvxs.ReplaceCommand(end, tmp_PathAndFlags);

            // Reverse the polygon
            while (end > start)
            {
                myvxs.SwapVertices(start++, end--);
            }
        }


    }
}
