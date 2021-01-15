//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;
using PixelFarm.Drawing;

using PaintLab;
using Typography.OpenFont;

namespace Mini
{
    enum BinCompression
    {
        None,
        Deflate,
    }

    static class DeflateCompressionUtils
    {

        public static byte[] Compress(byte[] inputdata)
        {
            using (MemoryStream compressedFileStream = new MemoryStream())
            {
                using (DeflateStream compressionStream = new DeflateStream(compressedFileStream, CompressionMode.Compress))
                {
                    compressionStream.Write(inputdata, 0, inputdata.Length);
                }
                compressedFileStream.Flush();
                return compressedFileStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] compressedData)
        {

            using (MemoryStream outputStream = new MemoryStream())
            {
                using (MemoryStream input = new MemoryStream(compressedData))
                {
                    using (DeflateStream decompressionStream = new DeflateStream(input, CompressionMode.Decompress))
                    {

                        byte[] readBuffer = new byte[1024];
                        int byteRead = decompressionStream.Read(readBuffer, 0, readBuffer.Length);
                        while (byteRead > 0)
                        {
                            outputStream.Write(readBuffer, 0, byteRead);
                            byteRead = decompressionStream.Read(readBuffer, 0, readBuffer.Length);
                        }
                    }
                }
                return outputStream.ToArray();
            }
        }
    }
    static class BinToHexUtils
    {
        public static StringBuilder ReadBinaryAndConvertToHexArr(string filename, BinCompression compression)
        {
            byte[] rawBuffer = File.ReadAllBytes(filename);
            switch (compression)
            {
                case BinCompression.Deflate:
                    {

                        byte[] compressedData = DeflateCompressionUtils.Compress(rawBuffer);
#if DEBUG                         
                        byte[] decompressedData = DeflateCompressionUtils.Decompress(compressedData);
                        if (decompressedData.Length != rawBuffer.Length)
                        {
                            throw new NotSupportedException();
                        }
#endif

                        return ReadBinaryAndConvertToHexArr(compressedData);
                    }
                default:

                    return ReadBinaryAndConvertToHexArr(rawBuffer);
            }
        }
        public static StringBuilder ReadBinaryAndConvertToHexArr(ConvertedFile file, BinCompression compression)
        {
            byte[] rawBuffer = File.ReadAllBytes(file.itemSourceFile.AbsoluteFilename);
            file.OriginalFileLength = rawBuffer.Length;
            switch (compression)
            {
                case BinCompression.Deflate:
                    {

                        byte[] compressedData = DeflateCompressionUtils.Compress(rawBuffer);
                        file.CompressedFileLength = compressedData.Length;
#if DEBUG
                        byte[] decompressedData = DeflateCompressionUtils.Decompress(compressedData);
                        if (decompressedData.Length != rawBuffer.Length)
                        {
                            throw new NotSupportedException();
                        }
#endif

                        return ReadBinaryAndConvertToHexArr(compressedData);
                    }
                default:

                    return ReadBinaryAndConvertToHexArr(rawBuffer);
            }
        }
        public static StringBuilder ReadBinaryAndConvertToHexArr(byte[] buffer)
        {
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
                if (charCountInLine > 32)
                {
                    stbuilder.Append("\r\n");
                    charCountInLine = 0; //reset
                }
            }
            stbuilder.AppendLine();
            stbuilder.AppendLine("}");
            return stbuilder;
        }
    }
    static class BitmapAtlasBuilderUtils
    {
        public static void BuildBitmapAtlas(AtlasProject atlasProj, Func<string, MemBitmap> imgLoader, bool test_extract = false)
        {

            //demonstrate how to build a bitmap atlas
            List<AtlasItemSourceFile> fileList = atlasProj.Items;
            //1. create builder
            var bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
            bmpAtlasBuilder.SpaceCompactOption = SimpleBitmapAtlasBuilder.CompactOption.ArrangeByHeight;

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

                outputFile.AppendLine("public static readonly string NAME=\"" + onlyFilename + "\";");

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

                StringBuilder info_sb = BinToHexUtils.ReadBinaryAndConvertToHexArr(info, BinCompression.None);

                StringBuilder img_sb = BinToHexUtils.ReadBinaryAndConvertToHexArr(img, BinCompression.None);

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

                outputFile.AppendLine("public partial class Binders{");

                foreach (string url in imgUrlDic.Keys)
                {
                    string url2 = url.Replace("\\", "_");
                    url2 = url2.Replace("//", "_");
                    url2 = url2.Replace(".", "_");

                    outputFile.AppendLine("public readonly AtlasImageBinder " + url2 + "=new AtlasImageBinder(RawAtlasData.NAME, \"" + url + "\");");
                }

                outputFile.AppendLine(@"

                    static bool s_registered;
                    public Binders(){
                            if(!s_registered){        
                                    try{
                                         PixelFarm.Platforms.InMemStorage.AddData(RawAtlasData.NAME + "".info"", RawAtlasData.info);
                                         PixelFarm.Platforms.InMemStorage.AddData(RawAtlasData.NAME + "".png"", RawAtlasData.img);
                                    }catch(System.Exception ex){
                                    }
                            s_registered= true;
                            }
                    }
                ");

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
                if (atlasSourceFile.Kind == AtlasItemSourceKind.FontAtlasConfig &&
                    atlasSourceFile.FontBuilderConfig != null)
                {
                    FontBuilderConfig config = atlasSourceFile.FontBuilderConfig;
                    foreach (FontBuilderTask builderTask in config.BuilderTasks)
                    {
                        //1. create glyph-texture-bitmap generator
                        var glyphTextureGen = new GlyphTextureBitmapGenerator();
                        glyphTextureGen.SetSvgBmpBuilderFunc(SvgBuilderHelper.ParseAndRenderSvg);
                        //2. generate the glyphs
                        if (builderTask.TextureKind == TextureKind.Msdf)
                        {
                            glyphTextureGen.MsdfGenVersion = 3;
                        }

                        Typography.OpenFont.Typeface typeface = atlasProj.GetTypeface(config.FontFilename);

                        //TODO: add other font styles 
                        RequestFont reqFont = new RequestFont(typeface.Name, builderTask.Size);

                        string random_suffix = Guid.NewGuid().ToString().Substring(0, 7);
                        string textureName = typeface.Name.ToLower() + "_" + random_suffix + ".info";
                        string output_imgFilename = textureName + ".png";


                        string outputDir = Path.GetDirectoryName(atlasProj.OutputFilename);

                        FontAtlasBuilderHelper builderHelper = new FontAtlasBuilderHelper();

                        builderHelper.TextureInfoFilename = outputDir + Path.DirectorySeparatorChar + textureName;
                        builderHelper.OutputImgFilename = outputDir + Path.DirectorySeparatorChar + textureName + ".png";

                        builderHelper.Build(glyphTextureGen,
                            typeface,
                            builderTask.Size,
                            builderTask.TextureKind,
                            builderTask.TextureBuildDetails.ToArray()
                            );
                    }
                }

            }
        }
    }
    class ConvertedFile
    {
        public AtlasItemSourceFile itemSourceFile;
        public StringBuilder Data;
        public string Name;
        public BinCompression Compression;
        public int OriginalFileLength;
        public int CompressedFileLength;
    }

    static class ResourceBuilderUtils
    {

        public static void BuildResources(AtlasProject atlasProj)
        {
            //1. load resouce
            //2. convert buffer to C# code
            List<ConvertedFile> selectedItems = new List<ConvertedFile>();
            foreach (AtlasItemSourceFile f in atlasProj.Items)
            {
                if (f.IsConfig)
                {
                    continue;
                }
                //------
                //load and convert
                ConvertedFile convertedFile = new ConvertedFile();
                convertedFile.Compression = BinCompression.Deflate;
                convertedFile.itemSourceFile = f;
                convertedFile.Data = BinToHexUtils.ReadBinaryAndConvertToHexArr(convertedFile, convertedFile.Compression);


                string url2 = f.Link.Replace("\\", "_");
                url2 = url2.Replace("//", "_");
                url2 = url2.Replace(".", "_");
                convertedFile.Name = url2;//field name
                selectedItems.Add(convertedFile);
            }
            BuildCsSource(atlasProj, selectedItems);
        }

        static void BuildCsSource(AtlasProject atlasProj, List<ConvertedFile> selectedItems)
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
                outputFile.AppendLine("public partial class RawResourceData{");
                foreach (ConvertedFile f in selectedItems)
                {
                    if (f.Compression != BinCompression.None)
                    {
                        outputFile.AppendLine("///<summary>");
                        outputFile.AppendLine("///compression: " + f.Compression + ",org_file_length=" + f.OriginalFileLength + ", compressed_length=" + f.CompressedFileLength);
                        outputFile.AppendLine("///</summary>");
                    }
                    outputFile.AppendLine("public readonly byte[] " + f.Name + "=" + f.Data.ToString() + ";");
                }

                outputFile.AppendLine("}");//class
                outputFile.AppendLine("}");//namespace                

                string dirname = Path.GetDirectoryName(atlasProj.OutputFilename);

                File.WriteAllText(dirname + Path.DirectorySeparatorChar + "x_" + onlyFilename + "_Resource_AUTOGEN.cs", outputFile.ToString());
            }

        }
    }

    struct FontAtlasBuilderHelper
    {
        public string TextureInfoFilename { get; set; }
        public string OutputImgFilename { get; set; }

#if DEBUG
        public long dbugBuildTimeMillisec { get; set; }
#endif
        public void Build(
            GlyphTextureBitmapGenerator glyphTextureGen,
            Typeface typeface, float fontSizeInPoints,
            TextureKind textureKind,
            GlyphTextureBuildDetail[] buildDetails)
        {
#if DEBUG
            //overall, glyph atlas generation time
            System.Diagnostics.Stopwatch dbugStopWatch = new System.Diagnostics.Stopwatch();
            dbugStopWatch.Start();
#endif
            var atlasBuilder = new SimpleBitmapAtlasBuilder();
            glyphTextureGen.CreateTextureFontFromBuildDetail(
                atlasBuilder,
                typeface,
                fontSizeInPoints,
                textureKind,
                buildDetails);

            //3. set information before write to font-info
            atlasBuilder.SpaceCompactOption = SimpleBitmapAtlasBuilder.CompactOption.ArrangeByHeight;
            atlasBuilder.SetAtlasFontInfo(typeface.Name, fontSizeInPoints);

            //4. merge all glyph in the builder into a single image
            using (MemBitmap totalGlyphsImg = atlasBuilder.BuildSingleImage(true))
            {

                if (TextureInfoFilename == null)
                {
                    //use random suffix
                    string random_suffix = Guid.NewGuid().ToString().Substring(0, 7);
                    string textureName = typeface.Name.ToLower() + "_" + random_suffix + ".info";
                    string output_imgFilename = textureName + ".png";

                    TextureInfoFilename = textureName;
                    OutputImgFilename = output_imgFilename;
                }


                //5. save atlas info to disk
                using (FileStream fs = new FileStream(TextureInfoFilename, FileMode.Create))
                {
                    atlasBuilder.SaveAtlasInfo(fs);
                }

                //6. save total-glyph-image to disk
                totalGlyphsImg.SaveImage(OutputImgFilename);
            }

#if DEBUG
            dbugStopWatch.Stop();
            dbugBuildTimeMillisec = dbugStopWatch.ElapsedMilliseconds;
#endif

        }

    }
}