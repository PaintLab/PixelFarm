//MIT, 2016-2017, WinterDev

using System;
using System.Collections.Generic;
//
using PixelFarm.Agg;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
//
using Typography.TextLayout;
using Typography.TextServices;
using Typography.OpenFont;
using Typography.OpenFont.Extensions;

#if GL_ENABLE

namespace PixelFarm.DrawingGL
{



    public class AggTextSpanPrinter : ITextPrinter
    {
        ActualImage _actualImage;
        AggRenderSurface _aggsx;
        AggPainter _aggPainter;
        VxsTextPrinter _vxsTextPrinter;
        int bmpWidth;
        int bmpHeight;
        GLRenderSurface _glsx;
        GLPainter canvasPainter;

        public AggTextSpanPrinter(GLPainter canvasPainter, int w, int h)
        {
            //this class print long text into agg canvas
            //then copy pixel buffer from aff canvas to gl-bmp
            //then draw the  gl-bmp into target gl canvas


            //TODO: review here
            this.canvasPainter = canvasPainter;
            this._glsx = canvasPainter.Canvas;
            bmpWidth = w;
            bmpHeight = h;

            _actualImage = new ActualImage(bmpWidth, bmpHeight, PixelFormat.ARGB32);
            _aggsx = new AggRenderSurface(_actualImage);
            _aggPainter = new AggPainter(_aggsx);
            _aggPainter.FillColor = Color.Black;
            _aggPainter.StrokeColor = Color.Black;

            //set default1
            _aggPainter.CurrentFont = canvasPainter.CurrentFont;
            var openFontStore = new Typography.TextServices.OpenFontStore();
            _vxsTextPrinter = new VxsTextPrinter(_aggPainter, openFontStore);
            _aggPainter.TextPrinter = _vxsTextPrinter;
        }
        public bool StartDrawOnLeftTop { get; set; }
        public Typography.Contours.HintTechnique HintTechnique
        {
            get { return _vxsTextPrinter.HintTechnique; }
            set { _vxsTextPrinter.HintTechnique = value; }
        }
        public bool UseSubPixelRendering
        {
            get { return _aggPainter.UseSubPixelRendering; }
            set
            {
                _aggPainter.UseSubPixelRendering = value;
            }
        }
        public void ChangeFont(RequestFont font)
        {

            _aggPainter.CurrentFont = font;
        }
        public void ChangeFillColor(Color fillColor)
        {
            //we use agg canvas to draw a font glyph
            //so we must set fill color for this
            _aggPainter.FillColor = fillColor;
        }
        public void ChangeStrokeColor(Color strokeColor)
        {
            //we use agg canvas to draw a font glyph
            //so we must set fill color for this
            _aggPainter.StrokeColor = strokeColor;
        }
        public void DrawString(char[] text, int startAt, int len, double x, double y)
        {


            if (this.UseSubPixelRendering)
            {
                //1. clear prev drawing result
                _aggPainter.Clear(Drawing.Color.FromArgb(0, 0, 0, 0));
                //aggPainter.Clear(Drawing.Color.White);
                //aggPainter.Clear(Drawing.Color.FromArgb(0, 0, 0, 0));
                //2. print text span into Agg Canvas
                _vxsTextPrinter.DrawString(text, startAt, len, 0, 0);
                //3.copy to gl bitmap
                byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(_actualImage);
                //------------------------------------------------------
                GLBitmap glBmp = new GLBitmap(bmpWidth, bmpHeight, buffer, true);
                glBmp.IsInvert = false;
                //TODO: review font height
                if (StartDrawOnLeftTop)
                {
                    y -= _vxsTextPrinter.FontLineSpacingPx;
                }
                _glsx.DrawGlyphImageWithSubPixelRenderingTechnique(glBmp, (float)x, (float)y);
                glBmp.Dispose();
            }
            else
            {

                //1. clear prev drawing result
                _aggPainter.Clear(Drawing.Color.White);
                _aggPainter.StrokeColor = Color.Black;

                //2. print text span into Agg Canvas
                _vxsTextPrinter.StartDrawOnLeftTop = false;

                float dyOffset = _vxsTextPrinter.FontDescedingPx;
                _vxsTextPrinter.DrawString(text, startAt, len, 0, -dyOffset);
                //------------------------------------------------------
                //debug save image from agg's buffer
#if DEBUG
                //actualImage.dbugSaveToPngFile("d:\\WImageTest\\aa1.png");
#endif
                //------------------------------------------------------

                //3.copy to gl bitmap
                byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(_actualImage);
                //------------------------------------------------------
                //debug save image from agg's buffer

                //------------------------------------------------------
                GLBitmap glBmp = new GLBitmap(bmpWidth, bmpHeight, buffer, true);
                glBmp.IsInvert = false;
                //TODO: review font height 
                //if (StartDrawOnLeftTop)
                //{
                y += _vxsTextPrinter.FontLineSpacingPx;
                //}
                _glsx.DrawGlyphImage(glBmp, (float)x, (float)y + dyOffset);
                glBmp.Dispose();
            }
        }
        public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] text, int start, int len)
        {
            throw new NotImplementedException();
        }
        public void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            throw new NotImplementedException();
        }
    }


    delegate GLBitmap LoadNewGLBitmapDel<T>(T src);

    class GLBitmapCache<T> : IDisposable
    {
        Dictionary<T, GLBitmap> _loadedGLBmps = new Dictionary<T, GLBitmap>();
        LoadNewGLBitmapDel<T> _loadNewGLBmpDel;
        public GLBitmapCache(LoadNewGLBitmapDel<T> loadNewGLBmpDel)
        {
            _loadNewGLBmpDel = loadNewGLBmpDel;
        }
        public GLBitmap GetOrCreateNewOne(T key)
        {
            GLBitmap found;
            if (!_loadedGLBmps.TryGetValue(key, out found))
            {

                return _loadedGLBmps[key] = _loadNewGLBmpDel(key);
            }
            return found;
        }
        public void Dispose()
        {
            Clear();
        }
        public void Clear()
        {
            foreach (GLBitmap glbmp in _loadedGLBmps.Values)
            {
                glbmp.Dispose();
            }
            _loadedGLBmps.Clear();
        }
        public void Delete(T key)
        {
            GLBitmap found;
            if (_loadedGLBmps.TryGetValue(key, out found))
            {
                found.Dispose();
                _loadedGLBmps.Remove(key);
            }
        }
    }


    public class GLBitmapGlyphTextPrinter : ITextPrinter, IDisposable
    {

        GLBitmapCache<SimpleFontAtlas> _loadedGlyphs;

        //--------
        GLRenderSurface _glsx;
        GLPainter painter;
        SimpleFontAtlas simpleFontAtlas;
        GLBitmap _glBmp;
        RequestFont font;
        TextureKind _currentTextureKind;

        int _currentFontRecommendLineSpacing;
        int _currentFontAscending;
        int _lineGap;
        int _lineHeight;

        LayoutFarm.OpenFontTextService _textServices = new LayoutFarm.OpenFontTextService();
        public GLBitmapGlyphTextPrinter(GLPainter painter)
        {

            //create text printer for use with canvas painter 
            //use typography font service?

            this.painter = painter;
            this._glsx = painter.Canvas;
            //_currentTextureKind = TextureKind.Msdf;

            //_currentTextureKind = TextureKind.StencilGreyScale;
            _currentTextureKind = TextureKind.StencilLcdEffect;
            //_currentTextureKind = TextureKind.Msdf;


            //GlyphPosPixelSnapX = GlyphPosPixelSnapKind.Integer;
            //GlyphPosPixelSnapY = GlyphPosPixelSnapKind.Integer; 

            ChangeFont(painter.CurrentFont);
            _loadedGlyphs = new GLBitmapCache<SimpleFontAtlas>(atlas =>
            {
                //create new one
                Typography.Rendering.GlyphImage totalGlyphImg = atlas.TotalGlyph;
                //load to glbmp 
                GLBitmap found = new GLBitmap(totalGlyphImg.Width, totalGlyphImg.Height, totalGlyphImg.GetImageBuffer(), false);
                found.IsInvert = false;
                return found;
            });

        }
        public void ChangeFillColor(Color color)
        {
            //called by owner painter   
            _glsx.FontFillColor = color;
        }
        public void ChangeStrokeColor(Color strokeColor)
        {
            //TODO: implementation here

        }
        public bool StartDrawOnLeftTop { get; set; }
        public void ChangeFont(RequestFont font)
        {
            //from request font
            //we resolve it to actual font

            this.font = font;

            TextureFontCreationParams creationParams = new TextureFontCreationParams();
            creationParams.originalFontSizeInPoint = font.SizeInPoints;
            //TODO: review here, langBits can be created with scriptLang ?
            creationParams.scriptLangs = new ScriptLang[]
            {
                ScriptLangs.Latin,
                ScriptLangs.Thai //eg. Thai, for test with complex script, you can change to your own
            };
            //
            creationParams.textureKind = _currentTextureKind;


            SimpleFontAtlas foundFontAtlas;
            ActualFont fontImp = ActiveFontAtlasService.GetTextureFontAtlasOrCreateNew(_textServices,
                font,
                creationParams,
                out foundFontAtlas);

            if (foundFontAtlas != this.simpleFontAtlas)
            {
                //change to another font atlas
                _glBmp = null;
                this.simpleFontAtlas = foundFontAtlas;
            }

            _typeface = (Typography.OpenFont.Typeface)fontImp.FontFace.GetInternalTypeface();
            float srcTextureScale = _typeface.CalculateScaleToPixelFromPointSize(simpleFontAtlas.OriginalFontSizePts);
            //scale at request
            float targetTextureScale = _typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
            _finalTextureScale = targetTextureScale / srcTextureScale;
            _currentFontRecommendLineSpacing = _typeface.CalculateRecommendLineSpacing();
            _currentFontAscending = _typeface.Ascender;
            _lineGap = _typeface.LineGap;
            _lineHeight = _typeface.CalculateLineSpacing(LineSpacingChoice.TypoMetric);

        }
        public void Dispose()
        {
            _loadedGlyphs.Clear();

            if (_glBmp != null)
            {
                _glBmp.Dispose();
                _glBmp = null;
            }
        }
        static PixelFarm.Drawing.Rectangle ConvToRect(Typography.Contours.Rectangle r)
        {
            //TODO: review here
            return PixelFarm.Drawing.Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
        }



        Typography.OpenFont.Typeface _typeface;
        float _finalTextureScale = 1;


        void EnsureLoadGLBmp()
        {
            if (_glBmp == null)
            {
                _glBmp = _loadedGlyphs.GetOrCreateNewOne(simpleFontAtlas);
            }
        }

        public void DrawString(char[] buffer, int startAt, int len, double x, double y)
        {

            _glsx.FontFillColor = painter.FillColor;

            int j = buffer.Length;
            TextBuffer textBuffer = new TextBuffer(buffer);

            GlyphPlanSequence glyphPlanSeq = _textServices.CreateGlyphPlanSeq(textBuffer, startAt, len, font);

            float scale = _typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
            int recommendLineSpacing = _typeface.CalculateRecommendLineSpacing();
            //--------------------------
            //TODO:
            //if (x,y) is left top
            //we need to adjust y again
            y -= ((_lineHeight) * scale);

            EnsureLoadGLBmp();
            // 
            float scaleFromTexture = _finalTextureScale;
            TextureKind textureKind = simpleFontAtlas.TextureKind;

            //--------------------------

            //TODO: review render steps 
            //NOTE:
            // -glyphData.TextureXOffset => restore to original pos
            // -glyphData.TextureYOffset => restore to original pos
            // ideal_x = (float)(x + (glyph.x * scale - glyphData.TextureXOffset) * scaleFromTexture);
            // ideal_y = (float)(y + (glyph.y * scale - glyphData.TextureYOffset + srcRect.Height) * scaleFromTexture);
            //--------------------------

            float g_x = 0;
            float g_y = 0;
            int baseY = (int)Math.Round(y);
            int n = glyphPlanSeq.len;
            int endBefore = glyphPlanSeq.startAt + n;
            for (int i = glyphPlanSeq.startAt; i < endBefore; ++i)
            {
                GlyphPlanList glyphPlanList = GlyphPlanSequence.UnsafeGetInteralGlyphPlanList(glyphPlanSeq);
                GlyphPlan glyph = glyphPlanList[i];

                Typography.Rendering.TextureFontGlyphData glyphData;
                if (!simpleFontAtlas.TryGetGlyphDataByCodePoint(glyph.glyphIndex, out glyphData))
                {
                    //if no glyph data, we should render a missing glyph ***
                    continue;
                }
                //--------------------------------------
                //TODO: review precise height in float
                //-------------------------------------- 
                PixelFarm.Drawing.Rectangle srcRect = ConvToRect(glyphData.Rect);
                g_x = (float)(x + (glyph.ExactX * scale - glyphData.TextureXOffset) * scaleFromTexture); //ideal x
                g_y = (float)(y + (glyph.ExactY * scale - glyphData.TextureYOffset + srcRect.Height) * scaleFromTexture);


                //for sharp glyph
                //we adjust g_x,g_y to integer value                
                g_x = (float)Math.Round(g_x);
                g_y = (float)Math.Floor(g_y);

                switch (textureKind)
                {
                    case TextureKind.Msdf:

                        _glsx.DrawSubImageWithMsdf(_glBmp,
                            ref srcRect,
                            g_x,
                            g_y,
                            scaleFromTexture);

                        break;
                    case TextureKind.StencilGreyScale:

                        //stencil gray scale with fill-color
                        _glsx.DrawGlyphImageWithStecil(_glBmp,
                         ref srcRect,
                            g_x,
                            g_y,
                            scaleFromTexture);

                        break;
                    case TextureKind.Bitmap:
                        _glsx.DrawSubImage(_glBmp,
                         ref srcRect,
                            g_x,
                            g_y,
                            scaleFromTexture);
                        break;
                    case TextureKind.StencilLcdEffect:

                        _glsx.DrawGlyphImageWithSubPixelRenderingTechnique(_glBmp,
                             ref srcRect,
                             g_x,
                             g_y,
                             scaleFromTexture);

                        break;
                }
            }
        }
        public void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            RenderVxGlyphPlan[] glyphPlans = renderVx.glyphList;
            int n = glyphPlans.Length;
            EnsureLoadGLBmp();

            //PERF:
            //TODO: review here, can we cache the glbmp for later use
            //not to create it every time 

            float scaleFromTexture = _finalTextureScale;

            TextureKind textureKind = simpleFontAtlas.TextureKind;
            float g_x = 0;
            float g_y = 0;
            int baseY = (int)Math.Round(y);
            float scale = 1;

            for (int i = 0; i < n; ++i)
            {
                //PERF:
                //TODO: 
                //render a set of glyph instead of one glyph per time ***
                RenderVxGlyphPlan glyph = glyphPlans[i];
                Typography.Rendering.TextureFontGlyphData glyphData;
                if (!simpleFontAtlas.TryGetGlyphDataByCodePoint(glyph.glyphIndex, out glyphData))
                {
                    //if no glyph data, we should render a missing glyph ***

                    continue;
                }
                //--------------------------------------
                //TODO: review precise height in float
                //-------------------------------------- 
                PixelFarm.Drawing.Rectangle srcRect = ConvToRect(glyphData.Rect);
                //--------------------------
                g_x = (float)(x + (glyph.x * scale - glyphData.TextureXOffset) * scaleFromTexture); //ideal x
                g_y = (float)((glyph.y * scale - glyphData.TextureYOffset + srcRect.Height) * scaleFromTexture);

                switch (textureKind)
                {
                    case TextureKind.Msdf:

                        _glsx.DrawSubImageWithMsdf(_glBmp,
                            ref srcRect,
                            g_x,
                            g_y,
                            scaleFromTexture);

                        break;
                    case TextureKind.StencilGreyScale:

                        //stencil gray scale with fill-color
                        _glsx.DrawGlyphImageWithStecil(_glBmp,
                         ref srcRect,
                            g_x,
                            g_y,
                            scaleFromTexture);

                        break;
                    case TextureKind.Bitmap:
                        _glsx.DrawSubImage(_glBmp,
                         ref srcRect,
                            g_x,
                            g_y,
                            scaleFromTexture);
                        break;
                    case TextureKind.StencilLcdEffect:
                        _glsx.DrawGlyphImageWithSubPixelRenderingTechnique(_glBmp,
                                ref srcRect,
                                g_x,
                                g_y,
                                scaleFromTexture);
                        break;
                }

            }
        }

        public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] buffer, int startAt, int len)
        {

            //TextPrinterHelper.CopyGlyphPlans(renderVx, glyphPlans, _typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints));
        }
    }


    //public class GLBitmapGlyphTextPrinter2 : ITextPrinter, IDisposable
    //{

    //    GLBitmapCache<SimpleFontAtlas> _loadedGlyphs;

    //    //--------
    //    GLRenderSurface _glsx;

    //    GlyphLayout _glyphLayout = new GlyphLayout();
    //    GLPainter painter;
    //    SimpleFontAtlas simpleFontAtlas;
    //    Typography.TextServices.IFontLoader _fontLoader;
    //    GLBitmap _glBmp;
    //    RequestFont font;
    //    TextServices textServices = new TextServices();
    //    public GLBitmapGlyphTextPrinter2(GLPainter painter, IFontLoader fontLoader)
    //    {

    //        //create text printer for use with canvas painter 
    //        //use typography font service?



    //        this.painter = painter;
    //        this._glsx = painter.Canvas;
    //        //GlyphPosPixelSnapX = GlyphPosPixelSnapKind.Integer;
    //        //GlyphPosPixelSnapY = GlyphPosPixelSnapKind.Integer;

    //        _fontLoader = fontLoader;
    //        ChangeFont(painter.CurrentFont);
    //        this._glyphLayout.ScriptLang = ScriptLangConv.GetOpenFontScriptLang(_defaultScriptLang.shortname);

    //        _loadedGlyphs = new GLBitmapCache<SimpleFontAtlas>(atlas =>
    //        {
    //            //create new one
    //            Typography.Rendering.GlyphImage totalGlyphImg = atlas.TotalGlyph;
    //            //load to glbmp 
    //            GLBitmap found = new GLBitmap(totalGlyphImg.Width, totalGlyphImg.Height, totalGlyphImg.GetImageBuffer(), false);
    //            found.IsInvert = false;
    //            return found;
    //        });

    //    }
    //    public void ChangeFillColor(Color color)
    //    {
    //        //called by owner painter   
    //        _glsx.FontFillColor = color;
    //    }
    //    public void ChangeStrokeColor(Color strokeColor)
    //    {
    //        //TODO: implementation here

    //    }
    //    public bool StartDrawOnLeftTop { get; set; }
    //    public void ChangeFont(RequestFont font)
    //    {
    //        //from request font
    //        //we resolve it to actual font

    //        this.font = font;
    //        this._glyphLayout.ScriptLang = ScriptLangConv.GetOpenFontScriptLang(_defaultScriptLang.shortname);

    //        SimpleFontAtlas foundFontAtlas;
    //        ActualFont fontImp = ActiveFontAtlasService.GetTextureFontAtlasOrCreateNew(_fontLoader, font, out foundFontAtlas);
    //        if (foundFontAtlas != this.simpleFontAtlas)
    //        {
    //            //change to another font atlas
    //            _glBmp = null;
    //            this.simpleFontAtlas = foundFontAtlas;
    //        }

    //        _typeface = (Typography.OpenFont.Typeface)fontImp.FontFace.GetInternalTypeface();
    //        float srcTextureScale = _typeface.CalculateScaleToPixelFromPointSize(simpleFontAtlas.OriginalFontSizePts);
    //        //scale at request
    //        float targetTextureScale = _typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
    //        _finalTextureScale = targetTextureScale / srcTextureScale;
    //    }
    //    public void Dispose()
    //    {
    //        _loadedGlyphs.Clear();

    //        if (_glBmp != null)
    //        {
    //            _glBmp.Dispose();
    //            _glBmp = null;
    //        }
    //    }
    //    static PixelFarm.Drawing.Rectangle ConvToRect(Typography.Contours.Rectangle r)
    //    {
    //        //TODO: review here
    //        return PixelFarm.Drawing.Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
    //    }


    //    //-----------
    //    GlyphPlanList glyphPlans = new GlyphPlanList();
    //    Typography.OpenFont.Typeface _typeface;
    //    float _finalTextureScale = 1;
    //    //-----------

    //    void EnsureLoadGLBmp()
    //    {
    //        if (_glBmp == null)
    //        {
    //            _glBmp = _loadedGlyphs.GetOrCreateNewOne(simpleFontAtlas);
    //        }
    //    }

    //    public void DrawString(char[] buffer, int startAt, int len, double x, double y)
    //    {


    //        int j = buffer.Length;
    //        //resolve font from painter?  
    //        glyphPlans.Clear();
    //        _glyphLayout.Layout(_typeface, buffer, startAt, len, glyphPlans);
    //        float scale = _typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);

    //        //--------------------------
    //        //TODO:
    //        //if (x,y) is left top
    //        //we need to adjust y again
    //        y -= (_typeface.Ascender - _typeface.Descender + _typeface.LineGap) * scale;

    //        int n = glyphPlans.Count;
    //        EnsureLoadGLBmp();
    //        // 
    //        float scaleFromTexture = _finalTextureScale;
    //        Typography.Rendering.TextureKind textureKind = simpleFontAtlas.TextureKind;
    //        //--------------------------

    //        //TODO: review render steps 
    //        //NOTE:
    //        // -glyphData.TextureXOffset => restore to original pos
    //        // -glyphData.TextureYOffset => restore to original pos
    //        // ideal_x = (float)(x + (glyph.x * scale - glyphData.TextureXOffset) * scaleFromTexture);
    //        // ideal_y = (float)(y + (glyph.y * scale - glyphData.TextureYOffset + srcRect.Height) * scaleFromTexture);
    //        //--------------------------

    //        float g_x = 0;
    //        float g_y = 0;
    //        int baseY = (int)Math.Round(y);
    //        for (int i = 0; i < n; ++i)
    //        {
    //            GlyphPlan glyph = glyphPlans[i];
    //            Typography.Rendering.TextureFontGlyphData glyphData;
    //            if (!simpleFontAtlas.TryGetGlyphDataByCodePoint(glyph.glyphIndex, out glyphData))
    //            {
    //                //if no glyph data, we should render a missing glyph ***
    //                continue;
    //            }
    //            //--------------------------------------
    //            //TODO: review precise height in float
    //            //-------------------------------------- 
    //            PixelFarm.Drawing.Rectangle srcRect = ConvToRect(glyphData.Rect);


    //            switch (textureKind)
    //            {
    //                case Typography.Rendering.TextureKind.Msdf:

    //                    _glsx.DrawSubImageWithMsdf(_glBmp,
    //                        ref srcRect,
    //                        g_x,
    //                        g_y,
    //                        scaleFromTexture);

    //                    break;
    //                case Typography.Rendering.TextureKind.AggGrayScale:

    //                    _glsx.DrawSubImage(_glBmp,
    //                     ref srcRect,
    //                        g_x,
    //                        g_y,
    //                        scaleFromTexture);

    //                    break;
    //                case Typography.Rendering.TextureKind.AggSubPixel:

    //                    _glsx.DrawGlyphImageWithSubPixelRenderingTechnique(_glBmp,
    //                         ref srcRect,
    //                         g_x,
    //                         g_y,
    //                         scaleFromTexture);

    //                    break;
    //            }
    //        }
    //    }
    //    public void DrawString(RenderVxFormattedString renderVx, double x, double y)
    //    {
    //        RenderVxGlyphPlan[] glyphPlans = renderVx.glyphList;
    //        int n = glyphPlans.Length;
    //        EnsureLoadGLBmp();

    //        //PERF:
    //        //TODO: review here, can we cache the glbmp for later use
    //        //not to create it every time 

    //        float scaleFromTexture = _finalTextureScale;

    //        Typography.Rendering.TextureKind textureKind = simpleFontAtlas.TextureKind;
    //        float g_x = 0;
    //        float g_y = 0;
    //        int baseY = (int)Math.Round(y);
    //        float scale = 1;

    //        for (int i = 0; i < n; ++i)
    //        {
    //            //PERF:
    //            //TODO: 
    //            //render a set of glyph instead of one glyph per time ***
    //            RenderVxGlyphPlan glyph = glyphPlans[i];
    //            Typography.Rendering.TextureFontGlyphData glyphData;
    //            if (!simpleFontAtlas.TryGetGlyphDataByCodePoint(glyph.glyphIndex, out glyphData))
    //            {
    //                //if no glyph data, we should render a missing glyph ***

    //                continue;
    //            }
    //            //--------------------------------------
    //            //TODO: review precise height in float
    //            //-------------------------------------- 
    //            PixelFarm.Drawing.Rectangle srcRect = ConvToRect(glyphData.Rect);
    //            //--------------------------
    //            g_x = (float)(x + (glyph.x * scale - glyphData.TextureXOffset) * scaleFromTexture); //ideal x
    //            g_y = (float)((glyph.y * scale - glyphData.TextureYOffset + srcRect.Height) * scaleFromTexture);

    //            switch (textureKind)
    //            {
    //                case Typography.Rendering.TextureKind.Msdf:

    //                    _glsx.DrawSubImageWithMsdf(_glBmp,
    //                        ref srcRect,
    //                        g_x,
    //                        g_y,
    //                        scaleFromTexture);

    //                    break;
    //                case Typography.Rendering.TextureKind.AggGrayScale:

    //                    _glsx.DrawSubImage(_glBmp,
    //                     ref srcRect,
    //                        g_x,
    //                        g_y,
    //                        scaleFromTexture);

    //                    break;
    //                case Typography.Rendering.TextureKind.AggSubPixel:
    //                    _glsx.DrawGlyphImageWithSubPixelRenderingTechnique(_glBmp,
    //                            ref srcRect,
    //                            g_x,
    //                            g_y,
    //                            scaleFromTexture);
    //                    break;
    //            }

    //        }
    //    }

    //    public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] buffer, int startAt, int len)
    //    {
    //        glyphPlans.Clear();
    //        _glyphLayout.Layout(_typeface, buffer, startAt, len, glyphPlans);

    //        TextPrinterHelper.CopyGlyphPlans(renderVx, glyphPlans, _typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints));
    //    }
    //}
}


#endif