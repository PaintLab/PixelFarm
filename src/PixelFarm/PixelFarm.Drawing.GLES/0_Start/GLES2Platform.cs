//BSD, 2014-present, WinterDev


using Typography.FontManagement;

namespace PixelFarm.Drawing.GLES2
{

    public static class GLES2Platform
    {

        public static Typography.TextServices.OpenFontTextService TextService { get; set; }
        public static void SetInstalledTypefaceProvider(IInstalledTypefaceProvider provider)
        {
            GLES2PlatformFontMx.SetInstalledTypefaceProvider(provider);
        }
        public static InstalledTypeface GetInstalledFont(string fontName, Typography.FontManagement.TypefaceStyle style)
        {
            return GLES2PlatformFontMx.GetInstalledFont(fontName, style);
        }

#if __MOBILE__
        public static RequestFont DefaultFont = new RequestFont("Droid Sans", 24);
#else
        public static RequestFont DefaultFont = new RequestFont("Source Sans Pro", 10);
#endif 

    }
}