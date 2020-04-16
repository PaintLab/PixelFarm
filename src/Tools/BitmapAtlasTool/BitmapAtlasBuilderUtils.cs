//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;
using PixelFarm.Drawing;
using SampleWinForms;

namespace Mini
{
    static class BitmapAtlasBuilderUtils
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
                if (f.Kind != AtlasItemSourceKind.Image)
                {
                    continue;
                }

                //3. load a bitmap

                BitmapAtlasItemSource atlasItem = null;
                using (MemBitmap itemBmp = imgLoader(f.AbsoluteFilename))
                {
                    //4. get information about it 
                    atlasItem = new BitmapAtlasItemSource(itemBmp.Width, itemBmp.Height);
                    atlasItem.SetImageBuffer(MemBitmap.CopyImgBuffer(itemBmp));
                }

                atlasItem.UniqueInt16Name = index;
                //5. add to builder                
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
            if (imgDic.Count == 0)
            {
                //no file
                return;
            }

            string atlasInfoFile = atlasProj.OutputFilename + ".info";
            string totalImgFile = atlasProj.OutputFilename + ".png";

            //5. merge all small images into a bigone 
            using (MemBitmap totalImg = bmpAtlasBuilder.BuildSingleImage(false))
            {
                bmpAtlasBuilder.ImgUrlDict = imgDic;
                bmpAtlasBuilder.SetAtlasInfo(TextureKind.Bitmap, 0);//font size
                                                                    //6. save atlas info and total-img (.png file)
                bmpAtlasBuilder.SaveAtlasInfo(atlasInfoFile);
                totalImg.SaveImage(totalImgFile);
            }


            //----------------------
            //7. create an atlas file in a source file version, user can embed the source to file
            //easy, just read .info and .png then convert to binary buffer

            BuildAtlasInEmbededSourceVersion(atlasProj, atlasInfoFile, totalImgFile, imgDic);

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

        static void BuildAtlasInEmbededSourceVersion(AtlasProject atlasProj, string info, string img, Dictionary<string, ushort> imgUrlDic)
        {
            //7. create an atlas file in a source file version, user can embed the source to file
            //easy, just read .info and .png then convert to binary buffer

            //PART1: 
            string timestamp = DateTime.Now.ToString("s");
            {
                StringBuilder outputFile = new StringBuilder();
                outputFile.AppendLine("//AUTOGEN, " + timestamp);
                outputFile.AppendLine("//source: " + atlasProj.FullFilename);
                outputFile.AppendLine("//tools: " + System.Windows.Forms.Application.ExecutablePath);

                string onlyFilename = Path.GetFileNameWithoutExtension(atlasProj.Filename);

                //TODO: config this
                outputFile.AppendLine("namespace " + atlasProj.CsSourceNamespace + "{");

                outputFile.AppendLine("public partial class RawAtlasData{");

                outputFile.AppendLine("public const string NAME=\"" + onlyFilename + "\";");

                outputFile.AppendLine("//img_links:");
                foreach (string url in imgUrlDic.Keys)
                {
                    outputFile.AppendLine("// " + url);
                }
                outputFile.AppendLine("");


                outputFile.AppendLine("//items names");
                outputFile.AppendLine("public static class Names{");
                foreach (string url in imgUrlDic.Keys)
                {
                    string url2 = url.Replace("\\", "_");
                    url2 = url2.Replace("//", "_");
                    url2 = url2.Replace(".", "_");

                    outputFile.AppendLine("public const string " + url2 + "=\"" + url + "\";");
                }
                outputFile.AppendLine("}");

                StringBuilder info_sb = ReadBinaryAndConvertToHexArr(info);

                StringBuilder img_sb = ReadBinaryAndConvertToHexArr(img);

                outputFile.AppendLine("//bitmap_atlas_info");
                outputFile.AppendLine("//" + info);
                outputFile.AppendLine("public static readonly byte[] info=" + info_sb.ToString() + ";");

                outputFile.AppendLine("//bitmap_atlas_total_img");
                outputFile.AppendLine("//" + img);
                outputFile.AppendLine("public static readonly byte[] img=" + img_sb.ToString() + ";");

                outputFile.AppendLine("}");
                outputFile.AppendLine("}");


                string dirname = Path.GetDirectoryName(atlasProj.OutputFilename);

                File.WriteAllText(dirname + Path.DirectorySeparatorChar + "x_" + onlyFilename + "_Atlas_AUTOGEN.cs", outputFile.ToString());
            }
            //----------------------
            //PART2: autogen atlas binder
            {
                StringBuilder outputFile = new StringBuilder();
                outputFile.AppendLine("//AUTOGEN, " + timestamp);
                outputFile.AppendLine("//source: " + atlasProj.FullFilename);
                outputFile.AppendLine("//tools: " + System.Windows.Forms.Application.ExecutablePath);

                string onlyFilename = Path.GetFileNameWithoutExtension(atlasProj.Filename);


                outputFile.AppendLine("using PixelFarm.Drawing;");

                outputFile.AppendLine("namespace " + atlasProj.CsSourceNamespace + "{");

                outputFile.AppendLine("public partial class BitmapAtlas<T>{");

                foreach (string url in imgUrlDic.Keys)
                {
                    string url2 = url.Replace("\\", "_");
                    url2 = url2.Replace("//", "_");
                    url2 = url2.Replace(".", "_");

                    outputFile.AppendLine("public static readonly AtlasImageBinder " + url2 + "=new AtlasImageBinder(RawAtlasData.NAME, \"" + url + "\");");
                }
                outputFile.AppendLine("}"); //class
                outputFile.AppendLine("}"); //namespace

                string dirname = Path.GetDirectoryName(atlasProj.OutputFilename);
                File.WriteAllText(dirname + Path.DirectorySeparatorChar + "x_" + onlyFilename + "_Atlas_AUTOGEN_BINDERS.cs", outputFile.ToString());
            }

        }

    }

    static class FontAtlasBuilderUtils
    {
        public static void BuildFontAtlas(AtlasProject atlasProj)
        {

            foreach (AtlasItemSourceFile atlasSourceFile in atlasProj.Items)
            {
                if (atlasSourceFile.Kind == AtlasItemSourceKind.Config &&
                    atlasSourceFile.FontBuilderConfig != null)
                {
                    FontBuilderConfig config = atlasSourceFile.FontBuilderConfig;
                    foreach (FontBuilderTask builderTask in config.BuilderTasks)
                    {
                        //1. create glyph-texture-bitmap generator
                        var glyphTextureGen = new GlyphTextureBitmapGenerator();
                        //2. generate the glyphs
                        if (builderTask.TextureKind == TextureKind.Msdf)
                        {
                            glyphTextureGen.MsdfGenVersion = 3;
                        }

                        Typography.OpenFont.Typeface typeface = atlasProj.GetTypeface(config.FontFilename);

                        //TODO: add other font styles 
                        RequestFont reqFont = new RequestFont(typeface.Name, builderTask.Size, FontStyle.Regular);


                        string textureName = typeface.Name.ToLower() + "_" + reqFont.FontKey;
                        string outputDir = Path.GetDirectoryName(atlasProj.OutputFilename);
                        FontAtlasBuilderHelper builderHelper = new FontAtlasBuilderHelper();

                        builderHelper.TextureInfoFilename = outputDir + Path.DirectorySeparatorChar + textureName;
                        builderHelper.OutputImgFilename = outputDir + Path.DirectorySeparatorChar + textureName + ".png";

                        builderHelper.Build(glyphTextureGen,
                            typeface,
                            builderTask.Size,
                            builderTask.TextureKind,
                            builderTask.TextureBuildDetails.ToArray(),
                            reqFont.FontKey
                            );
                    }
                }

            }
        }
    }
}