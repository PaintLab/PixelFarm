//BSD, 2014-present, WinterDev

using PixelFarm.Drawing.Fonts;
using PixelFarm.DrawingGL;
using Typography.FontManagement;

namespace PixelFarm.Drawing.GLES2
{

    public static class GLES2Platform
    {

        static LayoutFarm.OpenFontTextService _textService;

        static GLES2Platform()
        {
            _textService = new LayoutFarm.OpenFontTextService();

        }
        public static LayoutFarm.OpenFontTextService TextService
        {
            get { return _textService; }
            set
            {
                _textService = value;
            }
        }
        public static GLRenderSurface CreateGLRenderSurface(int w, int h, int viewportW, int viewportH)
        {
            //the canvas may need some init modules
            //so we start the canvass internaly here
            return new GLRenderSurface(w, h, viewportW, viewportH); 
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