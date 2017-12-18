//BSD, 2014-2017, WinterDev 
 
using Typography.TextServices;
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
        public static void SetFontLoader(IFontLoader fontLoader)
        {
            WinGdiFontFace.SetFontLoader(fontLoader);
        }
        public static ITextService GetIFonts()
        {
            return new Gdi32IFonts();
        }
    }
     



}