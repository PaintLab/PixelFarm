//MIT, 2014-2016, WinterDev

//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Runtime.InteropServices;
using System.IO;

namespace PixelFarm.Drawing.Fonts
{
    [StructLayout(LayoutKind.Sequential)]
    struct ExportTypeFaceInfo
    {
        public bool hasKerning;
        public IntPtr hb_font;
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
        public static extern IntPtr MyFtNewMemoryFace(IntPtr membuffer, int memSizeInBytes);
        [DllImport(myfontLib)]
        public static extern void MyFtDoneFace(IntPtr faceHandle);
        [DllImport(myfontLib)]
        public static extern void MyFtGetFaceInfo(IntPtr faceHandle, ref ExportTypeFaceInfo exportTypeFaceInfo);
        [DllImport(myfontLib)]
        public static extern void MyFtSetPixelSizes(IntPtr myface, int pxsize);
        [DllImport(myfontLib)]
        public static extern void MyFtSetCharSize(IntPtr faceHandle, int char_width26_6,
            int char_height26_6,
            int h_device_resolution,
            int v_device_resolution);
        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MyFtLoadChar(IntPtr faceHandle, int charcode, ref ExportGlyph ftOutline);
        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MyFtLoadGlyph(IntPtr faceHandle, uint codepoint, ref ExportGlyph ftOutline);
        //============================================================================
        //HB shaping ....
        [DllImport(myfontLib, CharSet = CharSet.Ansi)]
        public static extern int MyFtSetupShapingEngine(IntPtr faceHandle, string langName,
            int langNameLen, HBDirection hbDirection, int hbScriptCode, ref ExportTypeFaceInfo exportTypeFaceInfo);
        [DllImport(myfontLib)]
        public static unsafe extern int MyFtShaping(IntPtr my_hb_ft_font,
            char* text,
            int charCount,
            ProperGlyph* properGlyphs);
        static bool isLoaded = false;
        static bool LoadLib(string dllFilename)
        {
                 
            if (isLoaded)
            {
                return true;
            }
            if (!File.Exists(dllFilename))
            {
                //extract to it 
                //File.WriteAllBytes(dllFilename, global::project_resources.myfonts_dll.myft);
                //UnsafeMethods.LoadLibrary(dllFilename);

            }
            isLoaded = true;
            return true;
        }


        [DllImport(myfontLib)]
        public static extern void DeleteUnmanagedObj(IntPtr unmanagedObject);
        [DllImport(myfontLib)]
        public static extern IntPtr stbi_load(string filename, out int w, out int h, out int comp, int requestOutputComponent);


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