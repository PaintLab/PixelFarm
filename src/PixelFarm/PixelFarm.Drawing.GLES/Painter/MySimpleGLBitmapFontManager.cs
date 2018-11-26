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
                PixelFarm.CpuBlit.MemBitmap memBitmap = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(totalGlyphImg.Width, totalGlyphImg.Height, totalGlyphImg.GetImageBuffer());

                GLBitmap found = new GLBitmap(memBitmap);
                found.IsYFlipped = false;
                return found;
            });
        }
    }
}

