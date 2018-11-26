////MIT, 2016-present, WinterDev 



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
                GLBitmap found = new GLBitmap(
                    PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(
                        totalGlyphImg.Width,
                        totalGlyphImg.Height,
                        totalGlyphImg.GetImageBuffer()),
                    true); //set true=> glbmp is the original owner of the membmp, when glbmp is disposed => the membmp is disposed too
                found.IsYFlipped = false;
                return found;
            });
        }
    }
}

