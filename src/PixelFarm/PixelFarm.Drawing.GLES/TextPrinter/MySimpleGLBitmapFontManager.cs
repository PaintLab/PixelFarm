////MIT, 2016-present, WinterDev

using System.Collections.Generic;
using Typography.Rendering;

namespace PixelFarm.DrawingGL
{
    class MySimpleGLBitmapFontManager : BitmapFontManager<GLBitmap>
    {
        Dictionary<GlyphImage, GLBitmap> _sharedGlyphImgs = new Dictionary<GlyphImage, GLBitmap>();

        public MySimpleGLBitmapFontManager(LayoutFarm.OpenFontTextService textServices)
            : base(textServices)
        {
            SetLoadNewBmpDel(atlas =>
            {
                GlyphImage totalGlyphImg = atlas.TotalGlyph;
                if (atlas.UseSharedGlyphImage)
                {
                    if (!_sharedGlyphImgs.TryGetValue(totalGlyphImg, out GLBitmap found))
                    {
                        found = new GLBitmap(
                             PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(
                                 totalGlyphImg.Width,
                                 totalGlyphImg.Height,
                                 totalGlyphImg.GetImageBuffer()),
                             true); //set true=> glbmp is the original owner of the membmp, when glbmp is disposed => the membmp is disposed too
                        found.IsYFlipped = false;
                        _sharedGlyphImgs.Add(totalGlyphImg, found);
                    }
                    return found;
                }
                else
                {
                    //create new one                     
                    //load to glbmp  
                    GLBitmap bmp = new GLBitmap(
                        PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(
                            totalGlyphImg.Width,
                            totalGlyphImg.Height,
                            totalGlyphImg.GetImageBuffer()),
                        true); //set true=> glbmp is the original owner of the membmp, when glbmp is disposed => the membmp is disposed too
                    bmp.IsYFlipped = false;
                    return bmp;
                }
            });
        }
    }
}

