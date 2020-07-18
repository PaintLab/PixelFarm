////MIT, 2016-present, WinterDev

using System.Collections.Generic;
using PixelFarm.CpuBlit.BitmapAtlas;
using PixelFarm.CpuBlit;

namespace PixelFarm.DrawingGL
{
    class MySimpleGLBitmapFontManager : BitmapFontManager<GLBitmap>
    {
        readonly Dictionary<MemBitmap, GLBitmap> _sharedGlyphImgs = new Dictionary<MemBitmap, GLBitmap>();

        public MySimpleGLBitmapFontManager(Typography.Text.OpenFontTextService textServices)
            : base(textServices)
        {
            SetLoadNewBmpDel(atlas =>
            {
                MemBitmap mainBmp = atlas.MainBitmap;
                if (atlas.UseSharedImage)
                {
                    if (!_sharedGlyphImgs.TryGetValue(mainBmp, out GLBitmap found))
                    {
                        found = new GLBitmap(MemBitmap.CreateFromCopy(mainBmp), true);
                        //set true=> glbmp is the original owner of the membmp, when glbmp is disposed => the membmp is disposed too
                        found.IsYFlipped = false;
                        _sharedGlyphImgs.Add(mainBmp, found);
                    }
                    return found;
                }
                else
                {
                    //create new one                     
                    //load to glbmp  
                    GLBitmap bmp = new GLBitmap(MemBitmap.CreateFromCopy(mainBmp), true);
                    //set true=> glbmp is the original owner of the membmp, when glbmp is disposed => the membmp is disposed too
                    bmp.IsYFlipped = false;
                    return bmp;
                }
            });
        }
    }
}

