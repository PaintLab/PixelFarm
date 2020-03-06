//MIT, 2019-present,WinterDev

using System;
using System.Collections.Generic;
using Mini;

using PixelFarm.CpuBlit;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing;
using PixelFarm.Drawing.BitmapAtlas;

namespace OpenTkEssTest
{

    [Info(OrderCode = "406")]
    [Info("T406_GLBitmapAtlas", AvailableOn = AvailableOn.GLES)]
    public class T406_GLBitmapAtlas : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        LayoutFarm.ImageBinder _chk_checked;
        LayoutFarm.ImageBinder _chk_unchecked;
        protected override void OnGLPainterReady(GLPainter painter)
        {
            //example;
            //test1_atlas=> atlas filename
            _chk_checked = new AtlasImageBinder("test1_atlas", "\\chk_checked.png");
            _chk_unchecked = new AtlasImageBinder("test1_atlas", "\\chk_unchecked.png");

            _pcx = painter.PainterContext;
            _painter = painter;
            //
            //string atlasInfoFile = "test1_atlas"; //see SampleFontAtlasBuilder below
            //_bmpAtlasPainter.ChangeBitmapAtlas(atlasInfoFile);

        }
        protected override void OnReadyForInitGLShaderProgram()
        {
        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.ClearColorBuffer();

            _painter.DrawImage(_chk_checked, 0, 0);
            _painter.DrawImage(_chk_unchecked, 20, 0);

            SwapBuffers();
        }
    }

    public static class TestBitmapAtlasBuilder
    {
        public static void Test(string imgdir, Func<string, MemBitmap> imgLoader)
        {

            SimpleBitmapAtlasBuilder bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
            //test!
            int imgdirNameLen = imgdir.Length;
            string[] filenames = System.IO.Directory.GetFiles(imgdir, "*.png");
            ushort index = 0;

            Dictionary<string, ushort> imgDic = new Dictionary<string, ushort>();
            foreach (string f in filenames)
            {
                MemBitmap itemBmp = imgLoader(f);
                AtlasItemImage atlasItem = new AtlasItemImage(itemBmp.Width, itemBmp.Height);
                atlasItem.OriginalBounds = new PixelFarm.Drawing.RectangleF(0, 0, itemBmp.Width, itemBmp.Height);
                atlasItem.SetBitmap(itemBmp, false);
                //
                bmpAtlasBuilder.AddAtlasItemImage(index, atlasItem);
                string imgPath = f.Substring(imgdirNameLen);
                imgDic.Add(imgPath, index);
                index++;

                //------------
#if DEBUG
                if (index >= ushort.MaxValue)
                {
                    throw new NotSupportedException();
                }
#endif
                //------------
            }

            //string atlasInfoFile = "test1_atlas.info";
            //string totalImgFile = "test1_atlas.png";
            string atlasInfoFile = "test1_atlas.info";
            string totalImgFile = "test1_atlas.png";
            //test, write data to disk
            AtlasItemImage totalImg = bmpAtlasBuilder.BuildSingleImage();
            bmpAtlasBuilder.ImgUrlDict = imgDic;
            bmpAtlasBuilder.SetAtlasInfo(TextureKind.Bitmap);
            bmpAtlasBuilder.SaveAtlasInfo(atlasInfoFile); //save to filename
            totalImg.Bitmap.SaveImage(totalImgFile);

            //-----
            //test, read data back
            bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
            SimpleBitmaptAtlas bitmapAtlas = bmpAtlasBuilder.LoadAtlasInfo(atlasInfoFile);
            //
            MemBitmap totalAtlasImg = imgLoader(totalImgFile);
            AtlasItemImage atlasImg = new AtlasItemImage(totalAtlasImg.Width, totalAtlasImg.Height);
            bitmapAtlas.TotalImg = atlasImg;

            for (int i = 0; i < index; ++i)
            {
                if (bitmapAtlas.TryGetBitmapMapData((ushort)i, out BitmapMapData bmpMapData))
                {
                    //test copy data from bitmap
                    MemBitmap itemImg = totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
                    itemImg.SaveImage("test1_atlas_item" + i + ".png");
                }
            }

            //test,
            {
                if (bitmapAtlas.TryGetBitmapMapData(@"\chk_checked.png", out BitmapMapData bmpMapData))
                {
                    MemBitmap itemImg = totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
                    itemImg.SaveImage("test1_atlas_item_a.png");
                }
            }
        }

    }


}

