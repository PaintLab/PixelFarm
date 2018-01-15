//MIT, 2016-2018, WinterDev


#if GL_ENABLE
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
            get { return _aggPainter.UseSubPixelLcdEffect; }
            set
            {
                _aggPainter.UseSubPixelLcdEffect = value;
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
                //byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(_actualImage);
                //------------------------------------------------------
                GLBitmap glBmp = new GLBitmap(_actualImage);
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
                //byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(_actualImage);
                //------------------------------------------------------
                //debug save image from agg's buffer 

                //------------------------------------------------------
                //GLBitmap glBmp = new GLBitmap(bmpWidth, bmpHeight, buffer, true);
                GLBitmap glBmp = new GLBitmap(_actualImage);
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
        MySimpleGLBitmapFontManager _myGLBitmapFontMx;

        SimpleFontAtlas _fontAtlas;

        GLRenderSurface _glsx;
        GLPainter painter;
        GLBitmap _glBmp;
        RequestFont font;
        LayoutFarm.OpenFontTextService _textServices;
        public GLBitmapGlyphTextPrinter(GLPainter painter, LayoutFarm.OpenFontTextService textServices)
        {
            //create text printer for use with canvas painter           
            this.painter = painter;
            this._glsx = painter.Canvas;
            this._textServices = textServices;

            //_currentTextureKind = TextureKind.Msdf; 
            //_currentTextureKind = TextureKind.StencilGreyScale;

            _myGLBitmapFontMx = new MySimpleGLBitmapFontManager(TextureKind.StencilLcdEffect, textServices);
            //change this to fit yours.
            _myGLBitmapFontMx.SetCurrentScriptLangs(
                new ScriptLang[]
                {
                    ScriptLangs.Latin,
                    ScriptLangs.Thai //eg. Thai, for test with complex script, you can change to your own
                });

            //test textures...

            //GlyphPosPixelSnapX = GlyphPosPixelSnapKind.Integer;
            //GlyphPosPixelSnapY = GlyphPosPixelSnapKind.Integer;  


            ChangeFont(painter.CurrentFont);
        }

        public void ChangeFillColor(Color color)
        {
            //called by owner painter  
            painter.FontFillColor = color;

        }
        public void ChangeStrokeColor(Color strokeColor)
        {
            //TODO: implementation here

        }
        public bool StartDrawOnLeftTop { get; set; }


        public void ChangeFont(RequestFont font)
        {
            if (this.font == font)
            {
                return;
            }
            //font has been changed, 
            //resolve for the new one 
            //check if we have this texture-font atlas in our MySimpleGLBitmapFontManager 
            //if not-> request the MySimpleGLBitmapFontManager to create a newone 
            _fontAtlas = _myGLBitmapFontMx.GetFontAtlas(font, out _glBmp);
            this.font = font;
        }
        public void Dispose()
        {
            _myGLBitmapFontMx.Clear();
            _myGLBitmapFontMx = null;

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




        float _finalTextureScale = 1;


        void EnsureLoadGLBmp()
        {
            //if (_glBmp == null)
            //{
            //    _glBmp = _loadedGlyphs.GetOrCreateNewOne(_fontAtlas);
            //}
        }
        //public void DrawString(char[] buffer, int startAt, int len, double x, double y)
        //{

        //    _glsx.FontFillColor = painter.FontFillColor;

        //    int j = buffer.Length;

        //    //create temp buffer span that describe the part of a whole char buffer
        //    TextBufferSpan textBufferSpan = new TextBufferSpan(buffer, startAt, len);

        //    //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
        //    //with specific request font
        //    GlyphPlanSequence glyphPlanSeq = _textServices.CreateGlyphPlanSeq(ref textBufferSpan, font);

        //    float scale = _fontAtlas.TargetTextureScale;
        //    int recommendLineSpacing = _fontAtlas.OriginalRecommendLineSpacing;
        //    //--------------------------
        //    //TODO:
        //    //if (x,y) is left top
        //    //we need to adjust y again
        //    y -= ((_fontAtlas.OriginalRecommendLineSpacing) * scale);

        //    EnsureLoadGLBmp();
        //    // 
        //    float scaleFromTexture = _finalTextureScale;
        //    TextureKind textureKind = _fontAtlas.TextureKind;

        //    //--------------------------

        //    //TODO: review render steps 
        //    //NOTE:
        //    // -glyphData.TextureXOffset => restore to original pos
        //    // -glyphData.TextureYOffset => restore to original pos
        //    // ideal_x = (float)(x + (glyph.x * scale - glyphData.TextureXOffset) * scaleFromTexture);
        //    // ideal_y = (float)(y + (glyph.y * scale - glyphData.TextureYOffset + srcRect.Height) * scaleFromTexture);
        //    //--------------------------

        //    float g_x = 0;
        //    float g_y = 0;
        //    int baseY = (int)Math.Round(y);
        //    int n = glyphPlanSeq.len;
        //    int endBefore = glyphPlanSeq.startAt + n;

        //    _glsx.LoadTexture1(_glBmp);

        //    for (int i = glyphPlanSeq.startAt; i < endBefore; ++i)
        //    {
        //        GlyphPlanList glyphPlanList = GlyphPlanSequence.UnsafeGetInteralGlyphPlanList(glyphPlanSeq);
        //        GlyphPlan glyph = glyphPlanList[i];

        //        Typography.Rendering.TextureFontGlyphData glyphData;
        //        if (!_fontAtlas.TryGetGlyphDataByCodePoint(glyph.glyphIndex, out glyphData))
        //        {
        //            //if no glyph data, we should render a missing glyph ***
        //            continue;
        //        }
        //        //--------------------------------------
        //        //TODO: review precise height in float
        //        //-------------------------------------- 
        //        PixelFarm.Drawing.Rectangle srcRect = ConvToRect(glyphData.Rect);
        //        g_x = (float)(x + (glyph.ExactX * scale - glyphData.TextureXOffset) * scaleFromTexture); //ideal x
        //        g_y = (float)(y + (glyph.ExactY * scale - glyphData.TextureYOffset + srcRect.Height) * scaleFromTexture);


        //        //for sharp glyph
        //        //we adjust g_x,g_y to integer value                
        //        float g_y2 = (float)Math.Floor(g_y);

        //        g_x = (float)Math.Round(g_x);
        //        g_y = (float)Math.Floor(g_y);


        //        switch (textureKind)
        //        {
        //            case TextureKind.Msdf:

        //                _glsx.DrawSubImageWithMsdf(_glBmp,
        //                    ref srcRect,
        //                    g_x,
        //                    g_y,
        //                    scaleFromTexture);

        //                break;
        //            case TextureKind.StencilGreyScale:

        //                //stencil gray scale with fill-color
        //                _glsx.DrawGlyphImageWithStecil(_glBmp,
        //                 ref srcRect,
        //                    g_x,
        //                    g_y,
        //                    scaleFromTexture);

        //                break;
        //            case TextureKind.Bitmap:
        //                _glsx.DrawSubImage(_glBmp,
        //                 ref srcRect,
        //                    g_x,
        //                    g_y,
        //                    scaleFromTexture);
        //                break;
        //            case TextureKind.StencilLcdEffect:
        //                //_glsx.DrawGlyphImageWithSubPixelRenderingTechnique(
        //                //    ref srcRect,
        //                //    g_x,
        //                //    g_y,
        //                //    scaleFromTexture);
        //                _glsx.DrawGlyphImageWithSubPixelRenderingTechnique2(
        //                    ref srcRect,
        //                    g_x,
        //                    g_y,
        //                    scaleFromTexture);
        //                //_glsx.DrawGlyphImageWithSubPixelRenderingTechnique(_glBmp,
        //                //     ref srcRect,
        //                //     g_x,
        //                //     g_y,
        //                //     scaleFromTexture);

        //                break;
        //        }
        //    }
        //}

        List<float> _vboBufferList = new List<float>();
        List<ushort> _indexList = new List<ushort>();
        public void DrawString(char[] buffer, int startAt, int len, double x, double y)
        {

            _glsx.FontFillColor = painter.FontFillColor;

            int j = buffer.Length;

            //create temp buffer span that describe the part of a whole char buffer
            TextBufferSpan textBufferSpan = new TextBufferSpan(buffer, startAt, len);

            //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
            //with specific request font
            GlyphPlanSequence glyphPlanSeq = _textServices.CreateGlyphPlanSeq(ref textBufferSpan, font);

            float scale = _fontAtlas.TargetTextureScale;
            int recommendLineSpacing = _fontAtlas.OriginalRecommendLineSpacing;
            //--------------------------
            //TODO:
            //if (x,y) is left top
            //we need to adjust y again
            y -= ((_fontAtlas.OriginalRecommendLineSpacing) * scale);

            EnsureLoadGLBmp();
            // 
            float scaleFromTexture = _finalTextureScale;
            TextureKind textureKind = _fontAtlas.TextureKind;

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

            //-------------------------------------
            _glsx.LoadTexture1(_glBmp);
            //-------------------------------------

            _vboBufferList.Clear(); //clear before use
            _indexList.Clear(); //clear before use

            for (int i = glyphPlanSeq.startAt; i < endBefore; ++i)
            {
                GlyphPlanList glyphPlanList = GlyphPlanSequence.UnsafeGetInteralGlyphPlanList(glyphPlanSeq);
                GlyphPlan glyph = glyphPlanList[i];

                Typography.Rendering.TextureFontGlyphData glyphData;
                if (!_fontAtlas.TryGetGlyphDataByCodePoint(glyph.glyphIndex, out glyphData))
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
                float g_y2 = (float)Math.Floor(g_y);

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
                        //_glsx.DrawGlyphImageWithSubPixelRenderingTechnique(
                        //    ref srcRect,
                        //    g_x,
                        //    g_y,
                        //    scaleFromTexture);
                        //_glsx.DrawGlyphImageWithSubPixelRenderingTechnique2(
                        //    ref srcRect,
                        //    g_x,
                        //    g_y,
                        //    scaleFromTexture);
                        _glsx.WriteVboToList(
                          _vboBufferList,
                          _indexList,
                          ref srcRect,
                          g_x,
                          g_y,
                          scaleFromTexture);
                        //_glsx.DrawGlyphImageWithSubPixelRenderingTechnique(_glBmp,
                        //     ref srcRect,
                        //     g_x,
                        //     g_y,
                        //     scaleFromTexture);

                        break;
                }
            }
            //-------
            //we create vbo first 
            //then render 
            _glsx.DrawGlyphImageWithSubPixelRenderingTechnique3(_vboBufferList.ToArray(), _indexList.ToArray());

        }
        public void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            _glsx.LoadTexture1(_glBmp);

            _glsx.FontFillColor = painter.FontFillColor;
            DrawingGL.GLRenderVxFormattedString renderVxString1 = (DrawingGL.GLRenderVxFormattedString)renderVx;
            DrawingGL.VertexBufferObject2 vbo = renderVxString1.GetVbo();

            vbo.Bind();
            _glsx.DrawGlyphImageWithSubPixelRenderingTechnique4(renderVxString1.VertexCount, (float)x, (float)y);
            vbo.UnBind();

        }

        public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] buffer, int startAt, int len)
        {


            int j = buffer.Length;

            //create temp buffer span that describe the part of a whole char buffer
            TextBufferSpan textBufferSpan = new TextBufferSpan(buffer, startAt, len);

            //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
            //with specific request font
            GlyphPlanSequence glyphPlanSeq = _textServices.CreateGlyphPlanSeq(ref textBufferSpan, font);

            float scale = _fontAtlas.TargetTextureScale;
            int recommendLineSpacing = _fontAtlas.OriginalRecommendLineSpacing;

            //--------------------------
            //TODO:
            //if (x,y) is left top
            //we need to adjust y again
            float x = 0;
            float y = 0;

            y -= ((recommendLineSpacing) * scale);
            renderVx.RecommmendLineSpacing = (int)(recommendLineSpacing * scale);

            // 
            float scaleFromTexture = _finalTextureScale;
            TextureKind textureKind = _fontAtlas.TextureKind;

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


            _glsx.SetAssociatedTextureInfo(_glBmp);

            List<float> vboBufferList = new List<float>();
            List<ushort> indexList = new List<ushort>();

            for (int i = glyphPlanSeq.startAt; i < endBefore; ++i)
            {
                GlyphPlanList glyphPlanList = GlyphPlanSequence.UnsafeGetInteralGlyphPlanList(glyphPlanSeq);
                GlyphPlan glyph = glyphPlanList[i];

                Typography.Rendering.TextureFontGlyphData glyphData;
                if (!_fontAtlas.TryGetGlyphDataByCodePoint(glyph.glyphIndex, out glyphData))
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
                float g_y2 = (float)Math.Floor(g_y);

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
                        //_glsx.DrawGlyphImageWithSubPixelRenderingTechnique(
                        //    ref srcRect,
                        //    g_x,
                        //    g_y,
                        //    scaleFromTexture); 
                        _glsx.WriteVboToList(
                          vboBufferList,
                          indexList,
                          ref srcRect,
                          g_x,
                          g_y,
                          scaleFromTexture);
                        //_glsx.DrawGlyphImageWithSubPixelRenderingTechnique(_glBmp,
                        //     ref srcRect,
                        //     g_x,
                        //     g_y,
                        //     scaleFromTexture);

                        break;
                }
            }
            //---------


            DrawingGL.GLRenderVxFormattedString renderVxFormattedString = (DrawingGL.GLRenderVxFormattedString)renderVx;
            renderVxFormattedString.IndexArray = indexList.ToArray();
            renderVxFormattedString.VertexCoords = vboBufferList.ToArray();
            renderVxFormattedString.VertexCount = indexList.Count;
        }
    }

}


#endif