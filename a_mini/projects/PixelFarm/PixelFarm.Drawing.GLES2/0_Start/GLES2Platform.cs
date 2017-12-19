//BSD, 2014-2017, WinterDev

using PixelFarm.Drawing.Fonts;
using PixelFarm.DrawingGL;
using Typography.TextServices;

namespace PixelFarm.Drawing.GLES2
{

    public static class GLES2Platform
    {

        public static void AddTextureFont(string fontName, string xmlGlyphPos, string glypBitmap)
        {
            GLES2PlatformFontMx.AddTextureFontInfo(fontName, xmlGlyphPos, glypBitmap);
        }

        public static GLRenderSurface CreateGLRenderSurface(int w, int h, int viewportW, int viewportH)
        {
            //the canvas may need some init modules
            //so we start the canvass internaly here
            var rdsf = new GLRenderSurface(w, h);
            rdsf.SetViewport(viewportW, viewportH);
            return rdsf;
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