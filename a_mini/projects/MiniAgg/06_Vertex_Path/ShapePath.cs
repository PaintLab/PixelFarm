using System;
using System.Collections.Generic;

using System.Text;

namespace MatterHackers.Agg
{

    public static class ShapePath
    {
        [Flags]
        public enum FlagsAndCommand : byte
        {
            //first lower 4 bits compact flags
            CommandStop = 0x00,

            CommandMoveTo = 0x01,
            CommandLineTo = 0x02,
            CommandCurve3 = 0x03,
            CommandCurve4 = 0x04,
            
            CommandEndPoly = 0x0F,
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
                && c < FlagsAndCommand.CommandEndPoly;
        }
 

        public static bool IsStop(FlagsAndCommand c)
        {
            return c == FlagsAndCommand.CommandStop;
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

        //public static bool IsCurve3(FlagsAndCommand c)
        //{
        //    return c == FlagsAndCommand.CommandCurve3;
        //}

        //public static bool IsCurve4(FlagsAndCommand c)
        //{
        //    return c == FlagsAndCommand.CommandCurve4;
        //}

        public static bool IsEndPoly(FlagsAndCommand c)
        {
            return (c & FlagsAndCommand.CommandsMask) == FlagsAndCommand.CommandEndPoly;
        }

        public static bool IsClose(FlagsAndCommand c)
        {
            return (c & ~(FlagsAndCommand.FlagCW | FlagsAndCommand.FlagCCW)) ==
                   (FlagsAndCommand.CommandEndPoly | FlagsAndCommand.FlagClose);
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

        static public void ShortenPath(VertexSequence vs, double s, int closed)
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
    }
}
