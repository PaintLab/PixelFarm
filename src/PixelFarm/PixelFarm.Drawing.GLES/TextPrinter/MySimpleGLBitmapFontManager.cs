////MIT, 2016-present, WinterDev

using System.Collections.Generic;
using Typography.Rendering;
using PixelFarm.CpuBlit;

namespace PixelFarm.DrawingGL
{
    class MySimpleGLBitmapFontManager : BitmapFontManager<GLBitmap>
    {
        Dictionary<PixelFarm.CpuBlit.MemBitmap, GLBitmap> _sharedGlyphImgs = new Dictionary<PixelFarm.CpuBlit.MemBitmap, GLBitmap>();

        public MySimpleGLBitmapFontManager(LayoutFarm.OpenFontTextService textServices)
            : base(textServices)
        {
            SetLoadNewBmpDel(atlas =>
            {
                PixelFarm.CpuBlit.MemBitmap totalGlyphImg = atlas.TotalGlyph;
                if (atlas.UseSharedGlyphImage)
                {
                    if (!_sharedGlyphImgs.TryGetValue(totalGlyphImg, out GLBitmap found))
                    {
                        found = new GLBitmap(MemBitmap.CreateFromCopy(totalGlyphImg), true);
                        //set true=> glbmp is the original owner of the membmp, when glbmp is disposed => the membmp is disposed too
                        found.IsYFlipped = false;
                        _sharedGlyphImgs.Add(totalGlyphImg, found);
                    }
                    return found;
                }
                else
                {
                    //create new one                     
                    //load to glbmp  
                    GLBitmap bmp = new GLBitmap(MemBitmap.CreateFromCopy(totalGlyphImg), true);
                    //set true=> glbmp is the original owner of the membmp, when glbmp is disposed => the membmp is disposed too
                    bmp.IsYFlipped = false;
                    return bmp;
                }
            });
        }
    }
}

