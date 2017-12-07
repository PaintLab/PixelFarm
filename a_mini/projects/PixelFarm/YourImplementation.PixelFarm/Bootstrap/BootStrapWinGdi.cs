//MIT, 2017, WinterDev
using System;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

namespace YourImplementation
{
    public static class BootStrapWinGdi
    {
        public static readonly IFontLoader myFontLoader = new WindowsFontLoader();
        public static void SetupDefaultValues()
        {
            //test Typography's custom text break, 
            Typography.TextBreak.CustomBreakerBuilder.Setup(@"../../PixelFarm/Typography/Typography.TextBreak/icu58/brkitr_src/dictionaries");
          
            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(YourImplementation.BootStrapWinGdi.myFontLoader);
        }

    }
}