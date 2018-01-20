//MIT, 2016-2018, WinterDev 
#if GL_ENABLE
using System;
using System.Collections.Generic;
//

using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

using Typography.OpenFont;
using Typography.Rendering;
using Typography.OpenFont.Extensions;

using PixelFarm.Platforms;

namespace PixelFarm.DrawingGL
{
    class MySimpleGLBitmapFontManager
    {
        static GLBitmapCache<SimpleFontAtlas> _loadedGlyphs;
        static Dictionary<int, SimpleFontAtlas> _createdAtlases = new Dictionary<int, SimpleFontAtlas>();

        LayoutFarm.OpenFontTextService textServices;

        ScriptLang[] _currentScriptLangs;


        TextureKind _textureKind;
        public MySimpleGLBitmapFontManager(TextureKind textureKind, LayoutFarm.OpenFontTextService textServices)
        {
            this.textServices = textServices;
            //glyph cahce for specific atlas
            _loadedGlyphs = new GLBitmapCache<SimpleFontAtlas>(atlas =>
            {
                //create new one
                Typography.Rendering.GlyphImage totalGlyphImg = atlas.TotalGlyph;
                //load to glbmp 
                GLBitmap found = new GLBitmap(totalGlyphImg.Width, totalGlyphImg.Height, totalGlyphImg.GetImageBuffer(), false);
                found.IsInvert = false;
                return found;
            });

            _textureKind = textureKind;
        }
        GlyphTextureBuildDetail[] _textureBuildDetails;
        public void SetCurrentScriptLangs(ScriptLang[] currentScriptLangs)
        {
            this._currentScriptLangs = currentScriptLangs;

            //TODO: review here again,
            //this is a fixed version for tahoma font

            //temp fix here

            _textureBuildDetails = new GlyphTextureBuildDetail[]
            {
                new GlyphTextureBuildDetail{ ScriptLang= ScriptLangs.Latin, DoFilter= false, HintTechnique = Typography.Contours.HintTechnique.TrueTypeInstruction_VerticalOnly },
                new GlyphTextureBuildDetail{ OnlySelectedGlyphIndices=new char[]{ 'x', 'X', '7','k','K','Z','z','R','Y','%' },
                    DoFilter = false ,  HintTechnique = Typography.Contours.HintTechnique.None},
                new GlyphTextureBuildDetail{ ScriptLang= ScriptLangs.Thai, DoFilter= false, HintTechnique = Typography.Contours.HintTechnique.None},
            };
        }

#if DEBUG
        System.Diagnostics.Stopwatch _dbugStopWatch = new System.Diagnostics.Stopwatch();
#endif 
        /// <summary>
        /// get from cache or create a new one
        /// </summary>
        /// <param name="reqFont"></param>
        /// <returns></returns>
        public SimpleFontAtlas GetFontAtlas(RequestFont reqFont, out GLBitmap glBmp)
        {

#if DEBUG
            _dbugStopWatch.Reset();
            _dbugStopWatch.Start();
#endif

            int fontKey = reqFont.FontKey;
            SimpleFontAtlas fontAtlas;
            if (!_createdAtlases.TryGetValue(fontKey, out fontAtlas))
            {
                //check from pre-built cache (if availiable) 
                Typeface resolvedTypeface = textServices.ResolveTypeface(reqFont);


                //--------
                string fontTextureFile = "total_tahoma_n_" + reqFont.SizeInPoints;
                string resolveFontFile = "d:\\WImageTest\\" + fontTextureFile + ".info";
                string fontTextureInfoFile = "d:\\WImageTest\\total_tahoma_n_" + reqFont.SizeInPoints + ".info";
                string fontTextureImg = "d:\\WImageTest\\" + fontTextureFile + ".png";
                //---------- 


                if (StorageService.Provider.DataExists(fontTextureInfoFile))
                {
                    SimpleFontAtlasBuilder atlasBuilder2 = new SimpleFontAtlasBuilder();

                    using (System.IO.Stream dataStream = StorageService.Provider.ReadDataStream(fontTextureInfoFile))
                    {
                        try
                        {
                            fontAtlas = atlasBuilder2.LoadAtlasInfo(dataStream);
                            fontAtlas.TotalGlyph = ReadGlyphImages(fontTextureImg);
                            fontAtlas.OriginalFontSizePts = reqFont.SizeInPoints;
                            _createdAtlases.Add(fontKey, fontAtlas);
                            //
                            //calculate some commonly used values
                            fontAtlas.SetTextureScaleInfo(
                                resolvedTypeface.CalculateScaleToPixelFromPointSize(fontAtlas.OriginalFontSizePts),
                                resolvedTypeface.CalculateScaleToPixelFromPointSize(reqFont.SizeInPoints));
                            //TODO: review here, use scaled or unscaled values
                            fontAtlas.SetCommonFontMetricValues(
                                resolvedTypeface.Ascender,
                                resolvedTypeface.Descender,
                                resolvedTypeface.LineGap,
                                resolvedTypeface.CalculateRecommendLineSpacing());
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                }
                else
                {

                    GlyphImage totalGlyphsImg = null;
                    SimpleFontAtlasBuilder atlasBuilder = null;
                    var textureGen = new GlyphTextureBitmapGenerator();
                    textureGen.CreateTextureFontFromScriptLangs(
                        resolvedTypeface,
                        reqFont.SizeInPoints,
                       _textureKind,
                       _textureBuildDetails,
                        (glyphIndex, glyphImage, outputAtlasBuilder) =>
                        {
                            if (outputAtlasBuilder != null)
                            {
                                //finish
                                atlasBuilder = outputAtlasBuilder;
                            }
                        }
                    );

                    //
                    totalGlyphsImg = atlasBuilder.BuildSingleImage();
                    //if (reqFont.SizeInPoints == 14 && cacheImg != null)
                    //{
                    //    totalGlyphsImg = cacheImg;
                    //}
                    //totalGlyphsImg = Sharpen(totalGlyphsImg, 1); //test shapen primary image
                    //-               
                    //
                    //create atlas
                    fontAtlas = atlasBuilder.CreateSimpleFontAtlas();
                    fontAtlas.TotalGlyph = totalGlyphsImg;
#if DEBUG
                    //save glyph image for debug
                    //PixelFarm.Agg.ActualImage.SaveImgBufferToPngFile(
                    //    totalGlyphsImg.GetImageBuffer(),
                    //    totalGlyphsImg.Width * 4,
                    //    totalGlyphsImg.Width, totalGlyphsImg.Height,
                    //    "d:\\WImageTest\\total_" + reqFont.Name + "_" + reqFont.SizeInPoints + ".png");
                    ////save image to cache
                    SaveImgBufferToFile(totalGlyphsImg, fontTextureImg);
#endif

                    //cache the atlas
                    _createdAtlases.Add(fontKey, fontAtlas);
                    //
                    //calculate some commonly used values
                    fontAtlas.SetTextureScaleInfo(
                        resolvedTypeface.CalculateScaleToPixelFromPointSize(fontAtlas.OriginalFontSizePts),
                        resolvedTypeface.CalculateScaleToPixelFromPointSize(reqFont.SizeInPoints));
                    //TODO: review here, use scaled or unscaled values
                    fontAtlas.SetCommonFontMetricValues(
                        resolvedTypeface.Ascender,
                        resolvedTypeface.Descender,
                        resolvedTypeface.LineGap,
                        resolvedTypeface.CalculateRecommendLineSpacing());

                    ///
#if DEBUG

                    _dbugStopWatch.Stop();
                    System.Diagnostics.Debug.WriteLine("build font atlas: " + _dbugStopWatch.ElapsedMilliseconds + " ms");
#endif

                    //save font info to cache
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        atlasBuilder.SaveAtlasInfo(ms);
                        StorageService.Provider.SaveData(fontTextureInfoFile, ms.ToArray());
                    } 
                }
            }

            glBmp = _loadedGlyphs.GetOrCreateNewOne(fontAtlas);
            return fontAtlas;
        }

        static GlyphImage ReadGlyphImages(string filename)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
            {
                Hjg.Pngcs.PngReader reader = new Hjg.Pngcs.PngReader(fs, filename);
                Hjg.Pngcs.ImageInfo imgInfo = reader.ImgInfo;
                Hjg.Pngcs.ImageLine iline2 = new Hjg.Pngcs.ImageLine(imgInfo, Hjg.Pngcs.ImageLine.ESampleType.BYTE);

                int imgH = imgInfo.Rows;
                int imgW = imgInfo.Cols;
                int stride = imgInfo.BytesPerRow;
                int widthPx = imgInfo.Cols;

                int[] buffer = new int[(stride / 4) * imgH];
                //read each row 
                //and fill the glyph image 
                int startWriteAt = (imgW * (imgH - 1));
                int destIndex = startWriteAt;

                for (int row = 0; row < imgH; row++)
                {
                    Hjg.Pngcs.ImageLine iline = reader.ReadRowByte(row);
                    byte[] scline = iline.ScanlineB;

                    int b_src = 0;
                    destIndex = startWriteAt;

                    for (int mm = 0; mm < imgW; ++mm)
                    {
                        byte b = scline[b_src];
                        byte g = scline[b_src + 1];
                        byte r = scline[b_src + 2];
                        byte a = scline[b_src + 3];
                        b_src += 4;
                        buffer[destIndex] = (b << 16) | (g << 8) | (r) | (a << 24);
                        destIndex++;
                    }
                    startWriteAt -= imgW;
                }

                GlyphImage img = new GlyphImage(imgW, imgH);
                img.SetImageBuffer(buffer, true);
                return img;
            }

        }
        static void SaveImgBufferToFile(GlyphImage glyphImg, string filename)
        {
            //-------------
            int[] intBuffer = glyphImg.GetImageBuffer();
            using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create))
            {

                int imgW = glyphImg.Width;
                int imgH = glyphImg.Height;

                Hjg.Pngcs.ImageInfo imgInfo = new Hjg.Pngcs.ImageInfo(imgW, imgH, 8, true); //8 bits per channel with alpha
                Hjg.Pngcs.PngWriter writer = new Hjg.Pngcs.PngWriter(fs, imgInfo);
                Hjg.Pngcs.ImageLine iline = new Hjg.Pngcs.ImageLine(imgInfo, Hjg.Pngcs.ImageLine.ESampleType.BYTE);
                int startReadAt = 0;

                int imgStride = imgW * 4;

                int srcIndex = 0;
                int srcIndexRowHead = intBuffer.Length - imgW;

                for (int row = 0; row < imgH; row++)
                {
                    byte[] scanlineBuffer = iline.ScanlineB;
                    srcIndex = srcIndexRowHead;
                    for (int b = 0; b < imgStride;)
                    {
                        int srcInt = intBuffer[srcIndex];
                        srcIndex++;
                        scanlineBuffer[b] = (byte)((srcInt >> 16) & 0xff);
                        scanlineBuffer[b + 1] = (byte)((srcInt >> 8) & 0xff);
                        scanlineBuffer[b + 2] = (byte)((srcInt) & 0xff);
                        scanlineBuffer[b + 3] = (byte)((srcInt >> 24) & 0xff);
                        b += 4;
                    }
                    srcIndexRowHead -= imgW;
                    startReadAt += imgStride;
                    writer.WriteRow(iline, row);
                }
                writer.End();

            }

        }
#if DEBUG
        /// <summary>
        /// test only, shapen org image with Paint.net sharpen filter
        /// </summary>
        /// <param name="org"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        static GlyphImage Sharpen(GlyphImage org, int radius)
        {
            GlyphImage newImg = new GlyphImage(org.Width, org.Height);
            Agg.Imaging.ShapenFilterPdn sharpen1 = new Agg.Imaging.ShapenFilterPdn();
            int[] orgBuffer = org.GetImageBuffer();
            unsafe
            {
                fixed (int* orgHeader = &orgBuffer[0])
                {
                    int[] output = sharpen1.Sharpen(orgHeader, org.Width, org.Height, org.Width * 4, radius);
                    newImg.SetImageBuffer(output, org.IsBigEndian);
                }
            }

            return newImg;
        }
#endif
        public void Clear()
        {
            _loadedGlyphs.Clear();
        }
    }

}

#endif