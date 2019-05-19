//MIT, 2019-present, WinterDev 

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.BitmapAtlas;

namespace PixelFarm.DrawingGL
{
    public class GLBitmapAtlasPainter
    {
        SimpleBitmaptAtlas _bmpAtlas;
        GLBitmap _glBmp;
        MySimpleGLBitmapAtlasManager _atlasManager;
        string _lastestImgFile = null;

        public GLBitmapAtlasPainter()
        {
            _atlasManager = new MySimpleGLBitmapAtlasManager(TextureKind.Bitmap);
        }

        public void DrawImage(GLPainter glPainter, AtlasImageBinder atlasImgBinder, float left, float top)
        {
            switch (atlasImgBinder.State)
            {
                case LayoutFarm.BinderState.Loaded:
                    {
                        GLBitmap glbmp = LayoutFarm.ImageBinder.GetCacheInnerImage(atlasImgBinder) as GLBitmap;
                        if (glbmp != null)
                        {
                            BitmapMapData mapData = atlasImgBinder.MapData;
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
                                    {
                                        atlasImgBinder.State = LayoutFarm.BinderState.Loaded;
                                        LayoutFarm.ImageBinder.SetCacheInnerImage(atlasImgBinder, _glBmp, false);

                                        atlasImgBinder.MapData = mapData;
                                        glPainter.PainterContext.DrawSubImage(_glBmp,
                                            ref srcRect,
                                            left,
                                            top);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case LayoutFarm.BinderState.Unload:
                    {
                        //load img first
                        if (_lastestImgFile != atlasImgBinder.AtlasName)
                        {
                            _bmpAtlas = _atlasManager.GetBitmapAtlas(atlasImgBinder.AtlasName, out _glBmp);
                            if (_bmpAtlas == null)
                            {
                                //error 
                                atlasImgBinder.State = LayoutFarm.BinderState.Error;//not found
                                return;
                            }
                            _lastestImgFile = atlasImgBinder.AtlasName;
                        }
                        //--------
                        if (_bmpAtlas.TryGetBitmapMapData(atlasImgBinder.ImageName, out BitmapMapData mapData))
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
                                    {
                                        atlasImgBinder.State = LayoutFarm.BinderState.Loaded;
                                        LayoutFarm.ImageBinder.SetCacheInnerImage(atlasImgBinder, _glBmp, false);

                                        atlasImgBinder.MapData = mapData;
                                        glPainter.PainterContext.DrawSubImage(_glBmp,
                                            ref srcRect,
                                            left,
                                            top);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            atlasImgBinder.State = LayoutFarm.BinderState.Error;//not found
                        }
                    }
                    break;
            }
            if (atlasImgBinder.State == LayoutFarm.BinderState.Unload)
            {

            }

        }
    }

}