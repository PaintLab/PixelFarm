
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
    static class NativeMyFontsLib
    {
        static bool isLoaded = false;
        static NativeMyFontsLib()
        {

        }
        public static bool LoadOrExtract(string dllFilename)
        {
            //dev:
#if DEBUG
            return true;
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

    }
}