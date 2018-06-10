//BSD, 2014-2018, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using PixelFarm;
using PixelFarm.Drawing.Fonts;


using Mini;


using Typography.OpenFont;
using Typography.OpenFont.Extensions;
using Typography.Rendering;
using Typography.TextLayout;
using Typography.TextServices;


using PixelFarm.Platforms;


namespace PixelFarm.Agg.Sample_Draw
{

    [Info(OrderCode = "01")]
    public class DrawSample04 : DemoBase
    {

        Stroke stroke = new Stroke(1);
        LayoutFarm.OpenFontTextService _textServices;
        BitmapFontManager<ActualBitmap> _bmpFontMx;
        SimpleFontAtlas _fontAtlas;
        RequestFont _font;

        ActualBitmap _fontBmp;

        public override void Init()
        {
            //steps : detail ... 
            //1. create a text service (or get it from a singleton class)       

            _textServices = new LayoutFarm.OpenFontTextService();

            //2. create manager
            _bmpFontMx = new BitmapFontManager<ActualBitmap>(
                TextureKind.StencilLcdEffect,
                _textServices,
                atlas =>
                {
                    GlyphImage totalGlyphImg = atlas.TotalGlyph;
                    return new ActualBitmap(totalGlyphImg.Width, totalGlyphImg.Height, totalGlyphImg.GetImageBuffer());
                }
            );
            _bmpFontMx.SetCurrentScriptLangs(new ScriptLang[]
            {
                ScriptLangs.Latin
            });

            //3.  
            _font = new RequestFont("tahoma", 14);
            _fontAtlas = _bmpFontMx.GetFontAtlas(_font, out _fontBmp);

        }
        float _finalTextureScale = 1;
        List<float> _vboBufferList = new List<float>();
        List<ushort> _indexList = new List<ushort>();
        public void DrawString(Painter p, string text, double x, double y)
        {
            if (text != null)
            {
                DrawString(p, text.ToCharArray(), 0, text.Length, x, y);
            }

        }
        public void DrawString(Painter p, char[] buffer, int startAt, int len, double x, double y)
        {

            int j = buffer.Length;
            //create temp buffer span that describe the part of a whole char buffer
            TextBufferSpan textBufferSpan = new TextBufferSpan(buffer, startAt, len);

            //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
            //with specific request font
            GlyphPlanSequence glyphPlanSeq = _textServices.CreateGlyphPlanSeq(ref textBufferSpan, _font);

            float scale = _fontAtlas.TargetTextureScale;
            int recommendLineSpacing = _fontAtlas.OriginalRecommendLineSpacing;
            //--------------------------
            //TODO:
            //if (x,y) is left top
            //we need to adjust y again
            y -= ((_fontAtlas.OriginalRecommendLineSpacing) * scale);

            // 
            float scaleFromTexture = _finalTextureScale;
            TextureKind textureKind = _fontAtlas.TextureKind;

            float g_x = 0;
            float g_y = 0;
            int baseY = (int)Math.Round(y);
            int n = glyphPlanSeq.len;
            int endBefore = glyphPlanSeq.startAt + n;

            //-------------------------------------
            //load texture 
            //_glsx.LoadTexture1(_glBmp);
            //-------------------------------------

            _vboBufferList.Clear(); //clear before use
            _indexList.Clear(); //clear before use


            float acc_x = 0;
            float acc_y = 0;


            UnscaledGlyphPlanList glyphPlanList = GlyphPlanSequence.UnsafeGetInteralGlyphPlanList(glyphPlanSeq);
            for (int i = glyphPlanSeq.startAt; i < endBefore; ++i)
            {
                UnscaledGlyphPlan glyph = glyphPlanList[i];
                Typography.Rendering.TextureFontGlyphData glyphData;
                if (!_fontAtlas.TryGetGlyphDataByGlyphIndex(glyph.glyphIndex, out glyphData))
                {
                    //if no glyph data, we should render a missing glyph ***
                    continue;
                }
                //--------------------------------------
                //TODO: review precise height in float
                //-------------------------------------- 
                int srcX, srcY, srcW, srcH;
                glyphData.GetGlyphRect(out srcX, out srcY, out srcW, out srcH);

                float ngx = acc_x + (float)Math.Round(glyph.OffsetX * scale);
                float ngy = acc_y + (float)Math.Round(glyph.OffsetY * scale);
                //NOTE:
                // -glyphData.TextureXOffset => restore to original pos
                // -glyphData.TextureYOffset => restore to original pos 
                //--------------------------
                g_x = (float)(x + (ngx - glyphData.TextureXOffset) * scaleFromTexture); //ideal x
                g_y = (float)(y + (ngy - glyphData.TextureYOffset + srcH) * scaleFromTexture);

                acc_x += (float)Math.Round(glyph.AdvanceX * scale);
                //g_x = (float)Math.Round(g_x);
                g_y = (float)Math.Floor(g_y);

                p.RenderQuality = RenderQualtity.Fast;

                //*** the atlas is in inverted form
                p.DrawImage(_fontBmp, g_x, g_y, srcX, _fontBmp.Height - srcY, srcW, srcH);


                //copy some part from the bitmap 
                //switch (textureKind)
                //{
                //    case TextureKind.Msdf: 
                //        _glsx.DrawSubImageWithMsdf(_glBmp,
                //            ref srcRect,
                //            g_x,
                //            g_y,
                //            scaleFromTexture); 
                //        break;
                //    case TextureKind.StencilGreyScale: 
                //        //stencil gray scale with fill-color
                //        _glsx.DrawGlyphImageWithStecil(_glBmp,
                //         ref srcRect,
                //            g_x,
                //            g_y,
                //            scaleFromTexture); 
                //        break;
                //    case TextureKind.Bitmap:
                //        _glsx.DrawSubImage(_glBmp,
                //         ref srcRect,
                //            g_x,
                //            g_y,
                //            scaleFromTexture);
                //        break;
                //    case TextureKind.StencilLcdEffect: 
                //        _glsx.WriteVboToList(
                //          _vboBufferList,
                //          _indexList,
                //          ref srcRect,
                //          g_x,
                //          g_y,
                //          scaleFromTexture);

                //        break;
                //}
            }
            //-------
            //we create vbo first 
            //then render 

        }

        [DemoConfig]
        public bool UseBitmapExt
        {
            get;
            set;
        }
        public override void Draw(Painter p)
        {

            if (UseBitmapExt)
            {
                p.RenderQuality = RenderQualtity.Fast;
            }
            else
            {
                p.RenderQuality = RenderQualtity.HighQuality;
            }

            p.Orientation = DrawBoardOrientation.LeftBottom;

            int width = 800;
            int height = 600;
            //clear the image to white         
            // draw a circle
            p.Clear(Drawing.Color.White);
            p.FillColor = Color.Black;
            //-------- 

            DrawString(p, "1234567890", 10, 20);

            //----
            //test draw img 
            //g.Render(littlePoly.MakeVertexSnap(), ColorRGBA.Cyan);
            // draw some text
            // draw some text  

            //var textPrinter = new TextPrinter();
            //textPrinter.CurrentActualFont = svgFontStore.LoadFont(SvgFontStore.DEFAULT_SVG_FONTNAME, 30);
            //new TypeFacePrinter("Printing from a printer", 30, justification: Justification.Center);

            //VertexStore vxs = textPrinter.CreateVxs("Printing from a printer".ToCharArray());
            //var affTx = Affine.NewTranslation(width / 2, height / 4 * 3);
            //VertexStore s1 = affTx.TransformToVxs(vxs);
            //p.FillColor = Drawing.Color.Black;
            //p.Fill(s1);
            ////g.Render(s1, ColorRGBA.Black);
            //p.FillColor = Drawing.Color.Red;
            //p.Fill(StrokeHelp.MakeVxs(s1, 1));
            ////g.Render(StrokeHelp.MakeVxs(s1, 1), ColorRGBA.Red);
            //var aff2 = Affine.NewMatix(
            //    AffinePlan.Rotate(MathHelper.DegreesToRadians(90)),
            //    AffinePlan.Translate(40, height / 2));
            //p.FillColor = Drawing.Color.Black;
            //p.Fill(aff2.TransformToVertexSnap(vxs));
            ////g.Render(aff2.TransformToVertexSnap(vxs), ColorRGBA.Black);
        }
    }


    delegate U LoadNewBmpDelegate<T, U>(T src);

    class FontBitmapCache<T, U> : IDisposable
        where U : IDisposable
    {
        Dictionary<T, U> _loadBmps = new Dictionary<T, U>();
        LoadNewBmpDelegate<T, U> _loadNewBmpDel;
        public FontBitmapCache(LoadNewBmpDelegate<T, U> loadNewBmpDel)
        {
            _loadNewBmpDel = loadNewBmpDel;
        }
        public U GetOrCreateNewOne(T key)
        {
            U found;
            if (!_loadBmps.TryGetValue(key, out found))
            {
                return _loadBmps[key] = _loadNewBmpDel(key);
            }
            return found;
        }
        public void Dispose()
        {
            Clear();
        }
        public void Clear()
        {
            foreach (U glbmp in _loadBmps.Values)
            {
                glbmp.Dispose();
            }
            _loadBmps.Clear();
        }
        public void Delete(T key)
        {
            U found;
            if (_loadBmps.TryGetValue(key, out found))
            {
                found.Dispose();
                _loadBmps.Remove(key);
            }
        }
    }




    class BitmapFontManager<B>
        where B : IDisposable
    {
        static FontBitmapCache<SimpleFontAtlas, B> _loadedGlyphs;
        static Dictionary<int, SimpleFontAtlas> _createdAtlases = new Dictionary<int, SimpleFontAtlas>();

        LayoutFarm.OpenFontTextService textServices;
        ScriptLang[] _currentScriptLangs;
        TextureKind _textureKind;
        public BitmapFontManager(TextureKind textureKind,
            LayoutFarm.OpenFontTextService textServices,
            LoadNewBmpDelegate<SimpleFontAtlas, B> _createNewDel)
        {
            this.textServices = textServices;
            //glyph cahce for specific atlas
            //_loadedGlyphs = new FontBitmapCache<SimpleFontAtlas, B>(atlas =>
            //{
            //    //create new one
            //    Typography.Rendering.GlyphImage totalGlyphImg = atlas.TotalGlyph;
            //    //load to glbmp 
            //    return new ActualBitmap(totalGlyphImg.Width, totalGlyphImg.Height, totalGlyphImg.GetImageBuffer());
            //});
            _loadedGlyphs = new FontBitmapCache<SimpleFontAtlas, B>(_createNewDel);
            _textureKind = textureKind;
        }

        GlyphTextureBuildDetail[] _textureBuildDetails;
        public void SetCurrentScriptLangs(ScriptLang[] currentScriptLangs)
        {
            this._currentScriptLangs = currentScriptLangs;

            //TODO: review here again,
            //this is a fixed version for tahoma font
            //temp fix here ...
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
        public SimpleFontAtlas GetFontAtlas(RequestFont reqFont, out B glBmp)
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

                string fontTextureFile = reqFont.Name + " " + fontKey;
                string resolveFontFile = fontTextureFile + ".info";
                string fontTextureInfoFile = resolveFontFile;
                string fontTextureImg = fontTextureInfoFile + ".png";

                //check if the file exist

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

                    atlasBuilder.SpaceCompactOption = SimpleFontAtlasBuilder.CompactOption.ArrangeByHeight;
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
                        System.IO.File.WriteAllBytes(fontTextureInfoFile, ms.ToArray());

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
                bool isInverted = false;
                if (isInverted)
                {
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
                else
                {
                    //read each row 
                    //and fill the glyph image 
                    int startWriteAt = 0;
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
                        startWriteAt += imgW;
                    }
                    GlyphImage img = new GlyphImage(imgW, imgH);
                    img.SetImageBuffer(buffer, true);
                    return img;
                }
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