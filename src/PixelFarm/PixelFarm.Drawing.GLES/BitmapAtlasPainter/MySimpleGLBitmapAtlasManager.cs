//MIT, 2019-present, WinterDev
//----------------------------------- 
using System;
using System.Collections.Generic;
using PixelFarm.DrawingGL;
namespace PixelFarm.Drawing.BitmapAtlas
{
    //TODO: review class and method names

    class MySimpleGLBitmapAtlasManager : BitmapAtlasManager<GLBitmap>
    {
        public MySimpleGLBitmapAtlasManager(TextureKind textureKind)
            : base(textureKind)
        {
            SetLoadNewBmpDel(atlas =>
            {
                //create new one 
                //load to glbmp  
                GLBitmap found = new GLBitmap(atlas.TotalImg, false);
                found.IsYFlipped = false;
                return found;
            });
        }
    }
}