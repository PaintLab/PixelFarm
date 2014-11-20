//2014 BSD,WinterDev   
using System;
using System.Collections.Generic;

using System.Text;

namespace PixelFarm.Agg
{


    public static class ShapePath
    {
        public enum ShapeOrientation
        {
            Unknown,
            CCW,
            CW
        }

        /// <summary>
        /// vertex command and flags
        /// </summary>
        [Flags]
        public enum CmdAndFlags : byte
        {
            //---------------------------------
            //order of this value is significant!
            //---------------------------------
            //first lower 4 bits compact flags
            Empty = 0x00,
            //-----------------------
            //end figure command 2 lower bits 
            //is end command when 2 lower bit > 0
            EndFigure = 0x01,
            EndAndCloseFigure = 0x02,
            //----------------------- 
            //start from move to is 
            MoveTo = 0x04,
            LineTo = 0x05,
            Curve3 = 0x06,
            Curve4 = 0x07,
            //-----------------------       
            MASK = 0x0F,
            //----------------------- 
            //upper 4 bits for shape ShapeOrientation
            //0 = unknown
            //1 = CCW
            //2 = CW
            FlagCCW = (1 << 4),
            FlagCW = (2 << 4)
            //-----------------------            
        }
        public static bool IsVertextCommand(CmdAndFlags c)
        {
            return (CmdAndFlags.MASK & c) >= CmdAndFlags.MoveTo;
        }
        public static bool IsEmpty(CmdAndFlags c)
        {
            return c == CmdAndFlags.Empty;
        }

        public static bool IsMoveTo(CmdAndFlags c)
        {
            return c == CmdAndFlags.MoveTo;
        }
        public static bool IsCurve(CmdAndFlags c)
        {
            return c == CmdAndFlags.Curve3
                || c == CmdAndFlags.Curve4;
        }
        public static bool IsEndFigure(CmdAndFlags c)
        {
            return ((int)c & 0x3) > 0;
        }
        public static bool IsClose(CmdAndFlags c)
        {
            return ((CmdAndFlags.MASK) & c) == CmdAndFlags.EndAndCloseFigure;
        }
        public static bool IsNextPoly(CmdAndFlags c)
        {
            return c <= CmdAndFlags.MoveTo;
        }
        public static ShapeOrientation GetOrientation(CmdAndFlags c)
        {
            return (ShapeOrientation)(((int)c) >> 4);
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

        public static void ShortenPath(VertexDistanceList vseq, double s, bool closed)
        {
            if (s > 0.0 && vseq.Count > 1)
            {
                double d;
                int n = (int)(vseq.Count - 2);
                while (n != 0)
                {
                    d = vseq[n].dist;
                    if (d > s) break;
                    vseq.RemoveLast();
                    s -= d;
                    --n;
                }
                if (vseq.Count < 2)
                {
                    vseq.Clear();
                }
                else
                {
                    n = (int)vseq.Count - 1;
                    VertexDistance prev = vseq[n - 1];
                    VertexDistance last = vseq[n];
                    d = (prev.dist - s) / prev.dist;
                    double x = prev.x + (last.x - prev.x) * d;
                    double y = prev.y + (last.y - prev.y) * d;
                    last.x = x;
                    last.y = y;
                    if (!prev.IsEqual(last))
                    {
                        vseq.RemoveLast();
                    }
                    vseq.Close(closed);
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
            while (end < vcount && !ShapePath.IsNextPoly(myvxs.GetCommand(end)))
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
                    ShapePath.CmdAndFlags flags;
                    int myvxs_count = myvxs.Count;

                    var orientFlags = isCW ? ShapePath.CmdAndFlags.FlagCW : ShapePath.CmdAndFlags.FlagCCW;

                    while (end < myvxs_count &&
                          ShapePath.IsEndFigure(flags = myvxs.GetCommand(end)))
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
                if (ShapePath.IsEmpty(myvxs.GetCommand(start)))
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
            while (end < vcount && !ShapePath.IsNextPoly(myvxs.GetCommand(end))) { ++end; }

            InvertPolygon(myvxs, start, end);
        }



        static void InvertPolygon(VertexStore myvxs, int start, int end)
        {
            int i;
            ShapePath.CmdAndFlags tmp_PathAndFlags = myvxs.GetCommand(start);

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
