//BSD, 2014-present, WinterDev 

using Typography.FontManagement;
namespace PixelFarm.Drawing.WinGdi
{
    public static class WinGdiPlusPlatform
    {

        static WinGdiPlusPlatform()
        { 
        }
     
        public static void SetFontEncoding(System.Text.Encoding encoding)
        {
            WinGdiTextService.SetDefaultEncoding(encoding);
        }
        public static void SetInstalledTypefaceProvider(IInstalledTypefaceProvider provider)
        {
            WinGdiFontFace.SetInstalledTypefaceProvider(provider);
        }
        public static ITextService GetTextService()
        {
            return new Gdi32IFonts();
        }
    }
     



}