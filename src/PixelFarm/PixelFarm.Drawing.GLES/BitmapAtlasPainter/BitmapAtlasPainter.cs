//MIT, 2019-present, WinterDev 

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.BitmapAtlas;

namespace PixelFarm.DrawingGL
{
    public class BitmapAtlasPainter
    {
        SimpleBitmaptAtlas _bmpAtlas;
        GLBitmap _glBmp;
        MySimpleGLBitmapAtlasManager _atlasManager;

        public BitmapAtlasPainter()
        {
            _atlasManager = new MySimpleGLBitmapAtlasManager(TextureKind.Bitmap);
        }
        public void ChangeBitmapAtlas(string bmpAtlasFileName)
        {
            //we may have more than 1 bitmap atlas
            _bmpAtlas = _atlasManager.GetBitmapAtlas(bmpAtlasFileName, out _glBmp);
        }
        public void DrawImage(GLPainter painter, string imgName, float left, float top)
        {
            if (_bmpAtlas.TryGetBitmapMapData(imgName, out BitmapMapData mapData))
            {
                //found map data
                Rectangle srcRect =
                     new Rectangle(mapData.Left,
                         mapData.Top,  //diff from font atlas***
                         mapData.Width,
                         mapData.Height);

                TextureKind textureKind = _bmpAtlas.TextureKind;
                switch (textureKind)
                {
                    default:
                    case TextureKind.Msdf:
                        throw new NotSupportedException();
                    case TextureKind.Bitmap:
                        painter.PainterContext.DrawSubImage(_glBmp,
                            ref srcRect,
                            left,
                            top);
                        break;
                }
            }
        }
    }

}