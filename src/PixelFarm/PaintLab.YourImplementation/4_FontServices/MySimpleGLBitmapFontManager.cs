////MIT, 2016-present, WinterDev 
#if GL_ENABLE


using PixelFarm.Drawing.Fonts;
using Typography.Rendering;

namespace PixelFarm.DrawingGL
{
    class MySimpleGLBitmapFontManager : BitmapFontManager<GLBitmap>
    {
        public MySimpleGLBitmapFontManager(TextureKind textureKind, LayoutFarm.OpenFontTextService textServices)
            : base(textureKind, textServices)
        {
            SetLoadNewBmpDel(atlas =>
            {
                //create new one
                GlyphImage totalGlyphImg = atlas.TotalGlyph;
                //load to glbmp 
                GLBitmap found = new GLBitmap(totalGlyphImg.Width, totalGlyphImg.Height, totalGlyphImg.GetImageBuffer(), false);
                found.IsInvert = false;
                return found;
            });
        }
    }
}

#endif