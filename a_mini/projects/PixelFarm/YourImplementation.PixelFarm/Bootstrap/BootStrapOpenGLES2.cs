//MIT, 2017, WinterDev
using System;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

namespace YourImplementation
{


    public static class BootStrapOpenGLES2
    {
        public static readonly IFontLoader myFontLoader = new WindowsFontLoader();
        public static void SetupDefaultValues()
        {
            Typography.TextBreak.CustomBreakerBuilder.Setup(@"../../PixelFarm/Typography/Typography.TextBreak/icu58/brkitr_src/dictionaries");
            PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(YourImplementation.BootStrapOpenGLES2.myFontLoader);
        }
    }


}