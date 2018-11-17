//MIT, 2017-present, WinterDev
using Typography.FontManagement;

namespace YourImplementation
{


    public static class BootStrapWinGdi
    {
        public static IInstalledTypefaceProvider GetFontLoader()
        {
            return CommonTextServiceSetup.FontLoader;
        }
        public static void SetupDefaultValues()
        {
            CommonTextServiceSetup.SetupDefaultValues();
            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetInstalledTypefaceProvider(CommonTextServiceSetup.FontLoader);
        }
    }
}