//Apache2, 2014-2018, WinterDev

using Typography.TextServices;
namespace LayoutFarm.UI
{
    //platform specific code 
    public class UIPlatformWinNeutral : UIPlatform
    {
        OpenFontStore s_fontStore;

        static UIPlatformWinNeutral()
        {
            //setup, 
        }

        private UIPlatformWinNeutral()
        {
            LayoutFarm.UI.Clipboard.SetUIPlatform(this);

            s_fontStore = new OpenFontStore();

            //no gdi+
            // PixelFarm.Drawing.WinGdi.WinGdiFontFace.SetFontLoader(s_fontStore);
            //gles2 
            //
            PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(s_fontStore);
            //skia

            if (!YourImplementation.BootStrapSkia.IsNativeLibAvailable())
            {
                //set font not found handler
                PixelFarm.Drawing.Skia.SkiaGraphicsPlatform.SetFontLoader(s_fontStore);

            }
        }


        public override void ClearClipboardData()
        {
            throw new System.NotSupportedException();
        }
        public override string GetClipboardData()
        {
            throw new System.NotSupportedException();
        }
        public override void SetClipboardData(string textData)
        {
            throw new System.NotSupportedException();
        }

        // PixelFarm.Drawing.WinGdi.Gdi32IFonts _gdiPlusIFonts = new PixelFarm.Drawing.WinGdi.Gdi32IFonts();
        public PixelFarm.Drawing.ITextService GetIFonts()
        {
            throw new System.NotSupportedException();

            //    return this._gdiPlusIFonts;
        }

        public static readonly UIPlatformWinNeutral platform = new UIPlatformWinNeutral();
    }
}