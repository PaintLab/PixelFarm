//MIT, 2019-present, WinterDev 

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.BitmapAtlas;

namespace PixelFarm.DrawingGL
{
    public class GLBitmapAtlasPainter
    {
        SimpleBitmapAtlas _bmpAtlas;
        GLBitmap _glBmp;//current bitmap
        MySimpleGLBitmapAtlasManager _atlasManager;
        string _lastestImgFile = null;

        public GLBitmapAtlasPainter(TextureKind textureKind = TextureKind.Bitmap)
        {
            _atlasManager = new MySimpleGLBitmapAtlasManager(textureKind);
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
                            TextureGlyphMapData mapData = atlasImgBinder.MapData;
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

                        if (_bmpAtlas.TryGetGlyphMapData(atlasImgBinder.ImageName, out TextureGlyphMapData mapData))
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
                                        atlasImgBinder.SetPreviewImageSize(mapData.Width, mapData.Height);
                                        atlasImgBinder.RaiseImageChanged();

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
#if DEBUG
            if (atlasImgBinder.State == LayoutFarm.BinderState.Unload)
            {

            }
#endif

        }
    }
}