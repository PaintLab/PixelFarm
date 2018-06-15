//BSD, 2014-present, WinterDev

using PixelFarm.Drawing.Fonts;
using PixelFarm.DrawingGL;
using Typography.TextServices;

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
            var glsx = new GLRenderSurface(w, h);
            glsx.SetViewport(viewportW, viewportH);
            return glsx;
        }

        public static void SetFontLoader(IFontLoader fontLoader)
        {
            GLES2PlatformFontMx.SetFontLoader(fontLoader);
        }
        public static InstalledFont GetInstalledFont(string fontName, InstalledFontStyle style)
        {
            return GLES2PlatformFontMx.GetInstalledFont(fontName, style);
        }
    }
}