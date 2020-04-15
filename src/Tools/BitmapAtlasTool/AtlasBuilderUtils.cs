//MIT, 2020, WinterDev
//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;

using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;
namespace Mini
{
    static class AtlasBuilderUtils
    {
        public static void BuildBitmapAtlas(AtlasProject atlasProj, Func<string, MemBitmap> imgLoader, bool test_extract = false)
        {

            //demonstrate how to build a bitmap atlas
            List<AtlasItemSourceFile> fileList = atlasProj.Items;
            //1. create builder
            var bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();

            ushort index = 0;
            Dictionary<string, ushort> imgDic = new Dictionary<string, ushort>();
            foreach (AtlasItemSourceFile f in fileList)
            {
                //3. load a bitmap
                MemBitmap itemBmp = imgLoader(f.AbsoluteFilename);
                //4. get information about it

                var atlasItem = new BitmapAtlasItemSource(itemBmp.Width, itemBmp.Height);
                atlasItem.SetImageBuffer(MemBitmap.CopyImgBuffer(itemBmp));
                atlasItem.UniqueInt16Name = index;
                //5. add to builder
                //bmpAtlasBuilder.AddAtlasItemImage(index, atlasItem);
                bmpAtlasBuilder.AddItemSource(atlasItem);

                //get relative filename
                string imgPath = "//" + f.Link;
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


            string atlasInfoFile = atlasProj.OutputFilename + ".info";
            string totalImgFile = atlasProj.OutputFilename + ".png";

            //5. merge all small images into a bigone 
            MemBitmap totalImg = bmpAtlasBuilder.BuildSingleImage(false);
            bmpAtlasBuilder.ImgUrlDict = imgDic;
            bmpAtlasBuilder.SetAtlasInfo(TextureKind.Bitmap, 0);//font size
            //6. save atlas info and total-img (.png file)
            bmpAtlasBuilder.SaveAtlasInfo(atlasInfoFile);
            totalImg.SaveImage(totalImgFile);

            //----------------------
            //test, read data back
            //----------------------
            if (test_extract)
            {
                bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
                SimpleBitmapAtlas bitmapAtlas = bmpAtlasBuilder.LoadAtlasInfo(atlasInfoFile)[0];
                //
                MemBitmap totalAtlasImg = imgLoader(totalImgFile);
                bitmapAtlas.SetMainBitmap(imgLoader(totalImgFile), true);

                //-----
                for (int i = 0; i < index; ++i)
                {
                    if (bitmapAtlas.TryGetItem((ushort)i, out AtlasItem bmpMapData))
                    {
                        //test copy data from bitmap
                        MemBitmap itemImg = totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
                        itemImg.SaveImage("test1_atlas_item" + i + ".png");
                    }
                }
                //test,
                {
                    if (bitmapAtlas.TryGetItem(@"\chk_checked.png", out AtlasItem bmpMapData))
                    {
                        MemBitmap itemImg = totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
                        itemImg.SaveImage("test1_atlas_item_a.png");
                    }
                }
            }
        }

    }
}