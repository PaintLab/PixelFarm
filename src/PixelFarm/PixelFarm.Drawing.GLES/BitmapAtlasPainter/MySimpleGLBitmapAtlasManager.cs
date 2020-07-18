//MIT, 2019-present, WinterDev
//----------------------------------- 
using System;
using System.Collections.Generic;
using PixelFarm.DrawingGL;

namespace PixelFarm.CpuBlit.BitmapAtlas
{
    //TODO: review class and method names

    class MySimpleGLBitmapAtlasManager : BitmapAtlasManager<GLBitmap>
    {
        //create new one 
        //load to glbmp  
        public MySimpleGLBitmapAtlasManager(TextureKind textureKind)
            : base(atlas => new GLBitmap(atlas.MainBitmap, false) { IsYFlipped = false })
        {
        }
    }
}