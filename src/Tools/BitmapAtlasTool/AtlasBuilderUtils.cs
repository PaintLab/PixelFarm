//MIT, 2020, WinterDev
//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            //7. create an atlas file in a source file version, user can embed the source to file
            //easy, just read .info and .png then convert to binary buffer

            BuildAtlasInEmbededSourceVersion(atlasProj, atlasInfoFile, totalImgFile);

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

        static StringBuilder ReadBinaryAndConvertToHexArr(string file)
        {
            byte[] buffer = File.ReadAllBytes(file);

            StringBuilder stbuilder = new StringBuilder();

            int charCountInLine = 0;

            stbuilder.AppendLine("new byte[]{");
            for (int b = 0; b < buffer.Length; ++b)
            {
                if (b > 0)
                {
                    stbuilder.Append(',');
                }
                stbuilder.Append("0x" + buffer[b].ToString("X2") + "");
                charCountInLine++;
                if (charCountInLine > 63)
                {
                    stbuilder.Append("\r\n");
                    charCountInLine = 0; //reset
                }
            }
            stbuilder.AppendLine();
            stbuilder.AppendLine("}");
            return stbuilder;
        }

        static void BuildAtlasInEmbededSourceVersion(AtlasProject atlasProj, string info, string img)
        {
            //7. create an atlas file in a source file version, user can embed the source to file
            //easy, just read .info and .png then convert to binary buffer

            StringBuilder outputFile = new StringBuilder();
            outputFile.AppendLine("//AUTOGEN, " + DateTime.Now.ToString("s"));
            outputFile.AppendLine("//source: " + atlasProj.FullFilename);
            outputFile.AppendLine("//tools: " + System.Windows.Forms.Application.ExecutablePath);

            string onlyFilename = Path.GetFileNameWithoutExtension(atlasProj.Filename);

            //TODO: config this
            outputFile.AppendLine("namespace " + onlyFilename + "_Atlas_AUTOGEN{");
            outputFile.AppendLine("public static class Resource{");

            StringBuilder info_sb = ReadBinaryAndConvertToHexArr(info);

            StringBuilder img_sb = ReadBinaryAndConvertToHexArr(img);

            outputFile.AppendLine("//" + info);
            outputFile.AppendLine("public static readonly byte[] info=" + info_sb.ToString() + ";");

            outputFile.AppendLine("//" + img);
            outputFile.AppendLine("public static readonly byte[] img=" + img_sb.ToString() + ";");

            outputFile.AppendLine("}");
            outputFile.AppendLine("}");

            File.WriteAllText(atlasProj.OutputFilename + "_Atlas_AUTOGEN.cs", outputFile.ToString());
        }

    }
}