//BSD, 2014-present, WinterDev


using Typography.FontCollections;

namespace PixelFarm.Drawing.GLES2
{

    public static class GLES2Platform
    {

        public static Typography.Text.OpenFontTextService TextService { get; set; }
        public static void SetInstalledTypefaceProvider(IInstalledTypefaceProvider provider)
        {
            GLES2PlatformFontMx.SetInstalledTypefaceProvider(provider);
        }
        public static InstalledTypeface GetInstalledFont(string fontName, TypefaceStyle style, RequestFontWeight weight)
        {
            return GLES2PlatformFontMx.GetInstalledFont(fontName, style, weight);
        }

#if __MOBILE__
        public static RequestFont DefaultFont = new RequestFont("Droid Sans", 24);
#else
        public static RequestFont DefaultFont = new RequestFont("Source Sans Pro", 10);
#endif 

    }
}