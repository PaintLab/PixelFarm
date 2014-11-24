﻿//MIT 2014,WinterDev

//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using System.IO;

namespace PixelFarm.Font2
{

    [StructLayout(LayoutKind.Sequential)]
    struct ExportTypeFaceInfo
    {
        public bool hasKerning;
        public IntPtr hb_font;
        public IntPtr hb_fontBuffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ExportGlyph
    {

        public short unit_per_em;
        public short ascender;
        public short descender;
        public short height;

        public int advanceX;
        public int advanceY;

        public int bboxXmin;
        public int bboxXmax;
        public int bboxYmin;
        public int bboxYmax;

        public int img_width;
        public int img_height;
        public int img_horiBearingX;
        public int img_horiBearingY;
        public int img_horiAdvance;
        public int img_vertBearingX;
        public int img_vertBearingY;
        public int img_vertAdvance;


        public int bitmap_left;
        public int bitmap_top;

        internal IntPtr bitmap;
        internal IntPtr outline;
        //public FT_Bitmap* bitmap;
        //public FT_Outline* outline;
    }



    static class NativeMyFontsLib
    {
        const string myfontLib = @"myft.dll";
        static object syncObj = new object();
        static bool isInitLib = false;

        static NativeModuleHolder nativeModuleHolder;
        static NativeMyFontsLib()
        {

            //dynamic load dll

            string appBaseDir = AppDomain.CurrentDomain.BaseDirectory;
            LoadLib(appBaseDir + "\\" + myfontLib);


            //---------------
            //init library
            int initResult = 0;
            lock (syncObj)
            {
                if (!isInitLib)
                {
                    initResult = NativeMyFontsLib.MyFtInitLib();
                    isInitLib = true;
                }
            }
            //---------------
            nativeModuleHolder = new NativeModuleHolder();

        }
        [DllImport(myfontLib)]
        public static extern int MyFtLibGetVersion();

        [DllImport(myfontLib)]
        public static extern int MyFtInitLib();
        [DllImport(myfontLib)]
        public static extern void MyFtShutdownLib();

        [DllImport(myfontLib)]
        public static extern IntPtr MyFtNewMemoryFace(IntPtr membuffer, int sizeInBytes, int pxsize);


        [DllImport(myfontLib)]
        public static extern void MyFtDoneFace(IntPtr faceHandle);

        [DllImport(myfontLib)]
        public static extern void MyFtGetFaceInfo(IntPtr faceHandle, ref ExportTypeFaceInfo exportTypeFaceInfo);

        [DllImport(myfontLib)]
        public static extern void MyFtSetPixelSizes(IntPtr myface, int pxsize);
        [DllImport(myfontLib)]
        public static extern void MyFtSetCharSize(IntPtr myface, int char_width, int char_height, int h_device_resolution, int v_device_resolution);



        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MyFtLoadChar(IntPtr faceHandle, int charcode, ref ExportGlyph ftOutline);



        //============================================================================
        //HB shaping ....
        [DllImport(myfontLib, CharSet = CharSet.Ansi)]
        public static extern int MyFtSetupShapingEngine(IntPtr faceHandle, string langName,
            int langNameLen, HBDirection hbDirection, int hbScriptCode, ref ExportTypeFaceInfo exportTypeFaceInfo);

        [DllImport(myfontLib)]
        public static unsafe extern int MyFtShaping(IntPtr my_hb_ft_font,
            IntPtr my_hb_buf,
            byte* text,
            int charCount,
            ProperGlyph* properGlyphs);


        static bool isLoaded = false;
        static bool LoadLib(string dllFilename)
        {
            //dev:
#if DEBUG
            ///return true;

            string dev = @"D:\projects\myagg_cs\agg-sharp\a_mini\external\myfonts\Debug\myft.dll";
            UnsafeMethods.LoadLibrary(dev);
            return true;

#endif
            //for Windows , dynamic load dll       
            if (isLoaded)
            {
                return true;
            }
            if (!File.Exists(dllFilename))
            {
                //extract to it 
                File.WriteAllBytes(dllFilename, global::MiniAgg.Complements.myfonts_dll.myft);
                UnsafeMethods.LoadLibrary(dllFilename);

            }
            isLoaded = true;
            return true;
        }


        class NativeModuleHolder : IDisposable
        {
            ~NativeModuleHolder()
            {
                Dispose();
            }
            public void Dispose()
            {
                NativeMyFontsLib.MyFtShutdownLib();
            }
        }
    }
}