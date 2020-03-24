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
                        if (LayoutFarm.ImageBinder.GetCacheInnerImage(atlasImgBinder) is GLBitmap glbmp)
                        {
                            AtlasItem atlasItem = atlasImgBinder.AtlasItem;
                            Rectangle srcRect =
                               new Rectangle(atlasItem.Left,
                                   atlasItem.Top,  //diff from font atlas***
                                   atlasItem.Width,
                                   atlasItem.Height);

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

                                        atlasImgBinder.AtlasItem = atlasItem;
                                        glPainter.PainterContext.DrawSubImage(_glBmp,
                                            srcRect,
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

                        if (_bmpAtlas.TryGetItem(atlasImgBinder.ImageName, out AtlasItem atlasItem))
                        {
                            //found map data
                            Rectangle srcRect =
                                 new Rectangle(atlasItem.Left,
                                     atlasItem.Top,  //diff from font atlas***
                                     atlasItem.Width,
                                     atlasItem.Height);

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
                                        atlasImgBinder.AtlasItem = atlasItem;
                                        atlasImgBinder.SetPreviewImageSize(atlasItem.Width, atlasItem.Height);
                                        atlasImgBinder.RaiseImageChanged();

                                        glPainter.PainterContext.DrawSubImage(_glBmp,
                                            srcRect,
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