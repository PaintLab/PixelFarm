//MIT, 2017-present, WinterDev


using Typography.TextServices;

namespace YourImplementation
{


    public static class BootStrapWinGdi
    {
        public static IFontLoader GetFontLoader()
        {
            return CommonTextServiceSetup.FontLoader;
        }
        public static void SetupDefaultValues()
        {
            CommonTextServiceSetup.SetupDefaultValues();
            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(CommonTextServiceSetup.FontLoader);
        }
    }
}