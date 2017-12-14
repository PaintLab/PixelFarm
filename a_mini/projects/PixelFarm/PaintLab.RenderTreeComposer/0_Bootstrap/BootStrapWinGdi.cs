//MIT, 2017, WinterDev
using System;
using PixelFarm.Drawing;
using Typography.TextServices;

namespace YourImplementation
{
    public static class BootStrapWinGdi
    {
        static IFontLoader myFontLoader;
        public static IFontLoader GetFontLoader()
        {
#if DEBUG
            if (myFontLoader == null)
            {
            }
#endif
            return myFontLoader;
        }
        public static void SetupDefaultValues()
        {
            myFontLoader = new OpenFontStore();
            //test Typography's custom text break, 
            //check if we have that data?

            //string typographyDir = @"../../PixelFarm/Typography/Typography.TextBreak/icu58/brkitr_src/dictionaries";
            string typographyDir = @"../../PixelFarm/Typography/Typography.TextBreak/icu60/brkitr_src/dictionaries";
            if (System.IO.Directory.Exists(typographyDir))
            {
                Typography.TextBreak.CustomBreakerBuilder.Setup(typographyDir);
            }


            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(YourImplementation.BootStrapWinGdi.myFontLoader);
        }

    }
}