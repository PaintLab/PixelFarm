//BSD, 2014-2016, WinterDev 

using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing.Skia
{
    public class WinGdiPlusPlatform : GraphicsPlatform
    {
        static InstalledFontCollection s_installFontCollection = new InstalledFontCollection();
        static WinGdiPlusPlatform()
        {
            var installFontsWin32 = new PixelFarm.Drawing.Win32.InstallFontsProviderWin32();
            s_installFontCollection.LoadInstalledFont(installFontsWin32.GetInstalledFontIter());

            //3. set default encoing
            //WinGdiTextService.SetDefaultEncoding(System.Text.Encoding.ASCII);
        }
        public WinGdiPlusPlatform()
        {


        }

        public override Canvas CreateCanvas(int left, int top, int width, int height, CanvasInitParameters canvasInitPars = new CanvasInitParameters())
        {
            return new MySkiaCanvas(0, 0, left, top, width, height);
        }


        public static void SetFontNotFoundHandler(FontNotFoundHandler fontNotFoundHandler)
        {
            s_installFontCollection.SetFontNotFoundHandler(fontNotFoundHandler);
        }
    }



}