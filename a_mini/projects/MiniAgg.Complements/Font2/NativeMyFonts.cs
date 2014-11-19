//MIT 2014,WinterDev

//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PixelFarm.Font2
{
    [StructLayout(LayoutKind.Sequential)]
    struct FT_Vector
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct FT_Outline
    {
        public short n_contours;      /* number of contours in glyph        */
        public short n_points;        /* number of points in the glyph      */

        public FT_Vector* points;          /* the outline's points               */
        public char* tags;            /* the points flags                   */
        public short* contours;        /* the contour end points             */

        public int flags;           /* outline masks                      */
    }




    static class NativeMyFonts
    {
        const string myfontLib = @"D:\zzshare\zz2\freetype-2.5.3\builds\Debug\myft.dll";

        [DllImport(myfontLib)]
        public static extern int MyFtLibGetVersion();

        [DllImport(myfontLib)]
        public static extern int MyFtInitLib();


        [DllImport(myfontLib, CharSet = CharSet.Ansi)]
        public static extern int MyFtNewFace(string fontfaceName, int pxsize);

        [DllImport(myfontLib)]
        public static extern int MyFtNewMemoryFace(IntPtr membuffer, int sizeInBytes, int pxsize);

        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MyFtLoadChar(int charcode, ref FT_Outline ftOutline);

        [DllImport(myfontLib, CharSet = CharSet.Ansi)]
        public static extern int MyFtSetupShapingEngine(string langName, int langNameLen, int direction);
        [DllImport(myfontLib)]
        public static unsafe extern int MyFtShaping(byte* utf8Buffer, int charCount);
    }




}