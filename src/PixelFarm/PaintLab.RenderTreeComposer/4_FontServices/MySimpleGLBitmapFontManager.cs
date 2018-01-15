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
            GlyphImage totalGlyphsImg = null;
            if (!_createdAtlases.TryGetValue(fontKey, out fontAtlas))
            {

                //check from pre-built cache (if availiable)
                //
                Typeface resolvedTypeface = textServices.ResolveTypeface(reqFont);
                //GlyphImage cacheImage = ReadGlyphImages("d:\\WImageTest\\test1.png");


                //if we don't have 
                //the create it 


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


                //#if DEBUG
                //                //save image to cache
                //                SaveImgBufferToFile(totalGlyphsImg, "d:\\WImageTest\\test1.png");
                //                //save font info to cache
                //                atlasBuilder.SaveFontInfo("d:\\WImageTest\\test002.info");
                //#endif
            }

            glBmp = _loadedGlyphs.GetOrCreateNewOne(fontAtlas);

#if DEBUG

            _dbugStopWatch.Stop();
            System.Diagnostics.Debug.WriteLine("build font atlas: " + _dbugStopWatch.ElapsedMilliseconds + " ms");
#endif

            return fontAtlas;
        }

        static GlyphImage ReadGlyphImages(string filename)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
            {
                Hjg.Pngcs.PngReader reader = new Hjg.Pngcs.PngReader(fs, filename);
                Hjg.Pngcs.ImageInfo imgInfo = reader.ImgInfo;
                int imgH = imgInfo.Rows;
                int imgW = imgInfo.Cols;
                int bytesPerRow = imgInfo.BytesPerRow;
                int widthPx = imgInfo.Cols;

                int[] buffer = new int[(bytesPerRow / 4) * imgH];
                //read each row 
                //and fill the glyph image 
                int startWriteAt = 0;
                for (int row = 0; row < imgH; row++)
                {
                    Hjg.Pngcs.ImageLine iline = reader.ReadRowByte(row);
                    byte[] scline = iline.ScanlineB;
                    Buffer.BlockCopy(scline, 0, buffer, startWriteAt, bytesPerRow);
                    startWriteAt += bytesPerRow;
                }


            }
            return null;
        }
        static void SaveImgBufferToFile(GlyphImage glyphImg, string filename)
        {
            //-------------
            int[] intBuffer = glyphImg.GetImageBuffer();
            //byte[] imgBuff = new byte[intBuffer.Length * 4];
            //Buffer.BlockCopy(intBuffer, 0, imgBuff, 0, imgBuff.Length);
            //PixelFarm.Agg.ExternalImageService.SaveImage(imgBuff, glyphImg.Width, glyphImg.Height);
            ////-------------


            //ImageTools.IO.Png.PngEncoder enc = new ImageTools.IO.Png.PngEncoder();
            //ImageTools.ExtendedImage extImage = new ImageTools.ExtendedImage(glyphImg.Width, glyphImg.Height);
            //extImage.SetPixels(glyphImg.Width, glyphImg.Height, imgBuff);


            using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create))
            {

                int imgW = glyphImg.Width;
                int imgH = glyphImg.Height;

                Hjg.Pngcs.ImageInfo imgInfo = new Hjg.Pngcs.ImageInfo(imgW, imgH, 8, true); //8 bits per channel with alpha
                Hjg.Pngcs.PngWriter writer = new Hjg.Pngcs.PngWriter(fs, imgInfo);


                Hjg.Pngcs.ImageLine iline = new Hjg.Pngcs.ImageLine(imgInfo);
                int startReadAt = 0;

                for (int row = 0; row < imgH; row++)
                {
                    int[] scline = iline.Scanline;
                    Array.Copy(intBuffer, startReadAt, scline, 0, imgW);
                    startReadAt += imgW;
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