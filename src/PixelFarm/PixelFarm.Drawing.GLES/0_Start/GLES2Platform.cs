//BSD, 2014-present, WinterDev

using PixelFarm.Drawing.Fonts;
using PixelFarm.DrawingGL;
using Typography.FontManagement;

namespace PixelFarm.Drawing.GLES2
{

    public static class GLES2Platform
    {

        static LayoutFarm.OpenFontTextService s_textService;

        static GLES2Platform()
        {
            s_textService = new LayoutFarm.OpenFontTextService();
        }
        public static LayoutFarm.OpenFontTextService TextService
        {
            get => s_textService;
            set
            {
                s_textService = value;
            }
        }

      

        public static void SetInstalledTypefaceProvider(IInstalledTypefaceProvider provider)
        {
            GLES2PlatformFontMx.SetInstalledTypefaceProvider(provider);
        }
        public static InstalledTypeface GetInstalledFont(string fontName, Typography.FontManagement.TypefaceStyle style)
        {
            return GLES2PlatformFontMx.GetInstalledFont(fontName, style);
        }
    }
}