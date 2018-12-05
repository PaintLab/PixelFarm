//MIT, 2017-present, WinterDev
using Typography.FontManagement;

namespace YourImplementation
{


    public static class FrameworkInitWinGDI
    {
        public static IInstalledTypefaceProvider GetFontLoader()
        {
            return CommonTextServiceSetup.FontLoader;
        }
        public static void SetupDefaultValues()
        {
            //-------------------------------
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            ////------------------------------- 
            //1. select view port kind

            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetInstalledTypefaceProvider(CommonTextServiceSetup.FontLoader);
        }
    }
}