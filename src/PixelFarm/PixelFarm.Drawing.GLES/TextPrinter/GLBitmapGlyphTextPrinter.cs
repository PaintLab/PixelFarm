//MIT, 2016-present, WinterDev
using System;
using System.Collections.Generic;
//
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;
using PixelFarm.Drawing;

using Typography.TextLayout;
using Typography.OpenFont;
using Typography.TextBreak;
using Typography.FontManagement;

namespace PixelFarm.DrawingGL
{


    public enum GlyphTexturePrinterDrawingTechnique
    {
        Copy,
        Stencil,
        LcdSubPixelRendering,
        Msdf
    }

    public class GLBitmapGlyphTextPrinter : IGLTextPrinter, IDisposable
    {

        MySimpleGLBitmapFontManager _myGLBitmapFontMx;
        SimpleBitmapAtlas _fontAtlas;
        GLPainterCore _pcx;
        GLPainter _painter;
        GLBitmap _glBmp;
        RequestFont _font;
        ResolvedFont _resolvedFont;
        readonly OpenFontTextService _textServices;
        readonly TextureCoordVboBuilder _vboBuilder = new TextureCoordVboBuilder();

        float _px_scale = 1;

#if DEBUG
        public static GlyphTexturePrinterDrawingTechnique s_dbugDrawTechnique = GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering;
        public static bool s_dbugUseVBO = true;
        public static bool s_dbugShowGlyphTexture = false;
        public static bool s_dbugShowMarkers = false;
#endif
        /// <summary>
        /// use vertex buffer object
        /// </summary>

        public GLBitmapGlyphTextPrinter(GLPainter painter, OpenFontTextService textServices)
        {
            //create text printer for use with canvas painter           
            _painter = painter;
            _pcx = painter.Core;
            _textServices = textServices;

            //_currentTextureKind = TextureKind.Msdf; 
            //_currentTextureKind = TextureKind.StencilGreyScale;

            _myGLBitmapFontMx = new MySimpleGLBitmapFontManager(textServices);

            //LoadFontAtlas("tahoma_set1.multisize_fontAtlas", "tahoma_set1.multisize_fontAtlas.png");

            //test textures...

            //GlyphPosPixelSnapX = GlyphPosPixelSnapKind.Integer;
            //GlyphPosPixelSnapY = GlyphPosPixelSnapKind.Integer;
            //**
            ChangeFont(painter.CurrentFont);
            //
            //TextDrawingTechnique = GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering; //default 
            TextDrawingTechnique = GlyphTexturePrinterDrawingTechnique.Stencil; //default
            UseVBO = true;

            TextBaseline = TextBaseline.Top;
            //TextBaseline = TextBaseline.Alphabetic;
            //TextBaseline = TextBaseline.Bottom; 

            //TODO: temp fix, 
            var myAlternativeTypefaceSelector = new MyAlternativeTypefaceSelector();
            {
                var preferTypefaceList = new MyAlternativeTypefaceSelector.PreferTypefaceList();
                preferTypefaceList.AddTypefaceName("Source Sans Pro");
                preferTypefaceList.AddTypefaceName("Sarabun");
                myAlternativeTypefaceSelector.SetPreferTypefaces(ScriptTagDefs.Latin, preferTypefaceList);
            }
            {
                var preferTypefaceList = new MyAlternativeTypefaceSelector.PreferTypefaceList();
                preferTypefaceList.AddTypefaceName("Twitter Color Emoji");
                myAlternativeTypefaceSelector.SetPerferEmoji(preferTypefaceList);
            }
            AlternativeTypefaceSelector = myAlternativeTypefaceSelector;

        }
        public void LoadFontAtlas(string fontTextureInfoFile, string atlasImgFilename)
        {
            //TODO: extension method
            if (PixelFarm.Platforms.StorageService.Provider.DataExists(fontTextureInfoFile) &&
                PixelFarm.Platforms.StorageService.Provider.DataExists(atlasImgFilename))
            {
                using (System.IO.Stream fontTextureInfoStream = PixelFarm.Platforms.StorageService.Provider.ReadDataStream(fontTextureInfoFile))
                using (System.IO.Stream fontTextureImgStream = PixelFarm.Platforms.StorageService.Provider.ReadDataStream(atlasImgFilename))
                {
                    try
                    {
                        BitmapAtlasFile fontAtlas = new BitmapAtlasFile();
                        fontAtlas.Read(fontTextureInfoStream);
                        SimpleBitmapAtlas[] resultAtlases = fontAtlas.AtlasList.ToArray();
                        _myGLBitmapFontMx.AddSimpleFontAtlas(resultAtlases, fontTextureImgStream);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

        }
        public bool UseVBO { get; set; }

        GlyphTexturePrinterDrawingTechnique _drawingTech;
        public GlyphTexturePrinterDrawingTechnique TextDrawingTechnique
        {
            get => _drawingTech;
            set
            {
#if DEBUG
                if (value == GlyphTexturePrinterDrawingTechnique.Stencil)
                {

                }
#endif
                _drawingTech = value;
            }
        }
        public void ChangeFillColor(Color color)
        {
            //called by owner painter  
            _painter.FontFillColor = color;
        }
        public void ChangeStrokeColor(Color strokeColor)
        {
            //TODO: implementation here
        }

        public TextBaseline TextBaseline { get; set; }

        Typeface _typeface;

        float _white_space_width;
        public void ChangeFont(RequestFont font)
        {
            if (_font == font || (_font != null && _font.FontKey == font.FontKey))
            {
                //no change -> then return
                return;
            }

            ResolvedFont resolvedFont = _textServices.ResolveFont(font);
            _resolvedFont = resolvedFont;
            _fontAtlas = _myGLBitmapFontMx.GetFontAtlas(resolvedFont, out _glBmp);
            _font = font;
            _typeface = resolvedFont.Typeface;
            _px_scale = _typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
            _white_space_width = resolvedFont.WhitespaceWidth;
            return;
        }

        public void Dispose()
        {
            if (_myGLBitmapFontMx != null)
            {
                _myGLBitmapFontMx.Clear();
                _myGLBitmapFontMx = null;
            }


            if (_glBmp != null)
            {
                _glBmp.Dispose();
                _glBmp = null;
            }
        }
        public void MeasureString(char[] buffer, int startAt, int len, out int w, out int h)
        {
            var textBufferSpan = new TextBufferSpan(buffer, startAt, len);
            Size s = _textServices.MeasureString(textBufferSpan, _painter.CurrentFont);
            w = s.Width;
            h = s.Height;
        }

        readonly TextPrinterLineSegmentList<TextPrinterLineSegment> _lineSegs = new TextPrinterLineSegmentList<TextPrinterLineSegment>();
        readonly TextPrinterWordVisitor _textPrinterWordVisitor = new TextPrinterWordVisitor();


#if DEBUG
        void dbugInnerDrawI18NStringNO_WordPlate(char[] buffer, int startAt, int len, double left, double top)
        {
            //input string may not be only Eng+ Num
            //it may contains characters from other unicode ranges (eg. Emoji)
            //to print it correctly we need to split it to multiple part
            //and choose a proper typeface for each part
            //-----------------

            var textBufferSpan = new TextBufferSpan(buffer, startAt, len);

            //_textPrinterLineSegs.Clear();
            //_textPrinterWordVisitor.SetLineSegmentList(_textPrinterLineSegs);
            //_textServices.BreakToLineSegments(textBufferSpan, _textPrinterWordVisitor);
            //_textPrinterWordVisitor.SetLineSegmentList(null);//TODO: not need to set this,

            //check each split segment

            ResolvedFont resolved = _textServices.ResolveFont(_font);
            GlyphPlanSequence glyphPlanSeq = _textServices.CreateGlyphPlanSeq(textBufferSpan, _font);
            //-----------------

            _vboBuilder.Clear();
            _vboBuilder.SetTextureInfo(_glBmp.Width, _glBmp.Height, _glBmp.IsYFlipped, _pcx.OriginKind);

            //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
            //with specific request font

            float px_scale = _px_scale;
            //--------------------------
            //TODO:
            //if (x,y) is left top
            //we need to adjust y again      

            float scaleFromTexture = resolved.SizeInPoints / _fontAtlas.OriginalFontSizePts;

            TextureKind textureKind = _fontAtlas.TextureKind;

            float g_left = 0;
            float g_top = 0;



            float acc_x = 0; //local accumulate x
            float acc_y = 0; //local accumulate y 

#if DEBUG
            if (s_dbugShowMarkers)
            {
                if (s_dbugShowGlyphTexture)
                {
                    //show original glyph texture at top 
                    _pcx.DrawImage(_glBmp, 0, 0);
                }
                //draw red-line-marker for baseLine
                _painter.StrokeColor = Color.Red;
                int baseLine = (int)Math.Round((float)top + resolved.AscentInPixels);
                _painter.DrawLine(left, baseLine, left + 200, baseLine);
                //
                //draw magenta-line-marker for bottom line
                _painter.StrokeColor = Color.Magenta;
                int bottomLine = (int)Math.Round((float)top + resolved.LineSpacingInPixels);
                _painter.DrawLine(left, bottomLine, left + 200, bottomLine);
                //draw blue-line-marker for top line
                _painter.StrokeColor = Color.Blue;
                _painter.DrawLine(0, top, left + 200, top);
            }

            //DrawingTechnique = s_dbugDrawTechnique;//for debug only
            //UseVBO = s_dbugUseVBO;//for debug only 
#endif

            if (textureKind == TextureKind.Msdf)
            {
                TextDrawingTechnique = GlyphTexturePrinterDrawingTechnique.Msdf;
            }


            //----------
            float bottom = (float)top + resolved.AscentInPixels - resolved.DescentInPixels;
            int seqLen = glyphPlanSeq.Count;
            for (int i = 0; i < seqLen; ++i)
            {
                UnscaledGlyphPlan glyph = glyphPlanSeq[i];
                if (!_fontAtlas.TryGetItem(glyph.glyphIndex, out AtlasItem atlasItem))
                {
                    //if no glyph data, we should render a missing glyph ***
                    continue;
                }
                //--------------------------------------
                //TODO: review precise height in float
                //--------------------------------------
                //paint src rect

                var srcRect = new Rectangle(atlasItem.Left, atlasItem.Top, atlasItem.Width, atlasItem.Height);

                //offset length from 'base-line'
                float x_offset = acc_x + (float)Math.Round(glyph.OffsetX * px_scale - atlasItem.TextureXOffset * scaleFromTexture);
                float y_offset = acc_y + (float)Math.Round(glyph.OffsetY * px_scale - atlasItem.TextureYOffset * scaleFromTexture) + srcRect.Height; //***

                //NOTE:
                // -glyphData.TextureXOffset => restore to original pos
                // -glyphData.TextureYOffset => restore to original pos 
                //--------------------------              

                g_left = (float)(left + x_offset);
                g_top = (float)(bottom - y_offset); //***

                switch (TextBaseline)
                {
                    default:
                    case TextBaseline.Alphabetic:
                        //nothing todo
                        break;
                    case TextBaseline.Top:
                        g_top += _resolvedFont.DescentInPixels;
                        break;
                    case TextBaseline.Bottom:

                        break;
                }

                acc_x += (float)Math.Round(glyph.AdvanceX * px_scale);
                g_top = (float)Math.Ceiling(g_top);//adjust to integer num *** 

#if DEBUG
                if (s_dbugShowMarkers)
                {

                    if (s_dbugShowGlyphTexture)
                    {
                        //draw yellow-rect-marker on original texture
                        _painter.DrawRectangle(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, Color.Yellow);
                    }

                    //draw debug-rect box at target glyph position
                    _painter.DrawRectangle(g_left, g_top, srcRect.Width, srcRect.Height, Color.Black);
                    _painter.StrokeColor = Color.Blue; //restore
                }


                //System.Diagnostics.Debug.WriteLine(
                //    "ds:" + buffer[0] + "o=(" + left + "," + top + ")" +
                //    "g=(" + g_left + "," + g_top + ")" + "srcRect=" + srcRect);

#endif

                if (UseVBO)
                {
                    _vboBuilder.WriteRect(
                           srcRect,
                           g_left, g_top, scaleFromTexture);
                }
                else
                {
                    switch (TextDrawingTechnique)
                    {
                        case GlyphTexturePrinterDrawingTechnique.Msdf:
                            _pcx.DrawSubImageWithMsdf(_glBmp,
                                  srcRect,
                                 g_left,
                                 g_top,
                                 scaleFromTexture);
                            break;
                        case GlyphTexturePrinterDrawingTechnique.Stencil:
                            //stencil gray scale with fill-color
                            _pcx.DrawGlyphImageWithStecil(_glBmp,
                                srcRect,
                                g_left,
                                g_top,
                                scaleFromTexture);
                            break;
                        case GlyphTexturePrinterDrawingTechnique.Copy:
                            _pcx.DrawSubImage(_glBmp,
                                srcRect,
                                g_left,
                                g_top,
                                1);
                            break;
                        case GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering:
                            _pcx.DrawGlyphImageWithSubPixelRenderingTechnique2_GlyphByGlyph(
                                _glBmp,
                                srcRect,
                                g_left,
                                g_top,
                                1);
                            break;
                    }
                }

            }
            //-------------------------------------------
            //

            if (UseVBO)
            {
                switch (TextDrawingTechnique)
                {
                    case GlyphTexturePrinterDrawingTechnique.Copy:
                        _pcx.DrawGlyphImageWithCopy_VBO(_glBmp, _vboBuilder);
                        break;
                    case GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering:
                        _pcx.DrawGlyphImageWithSubPixelRenderingTechnique3_DrawElements(_glBmp, _vboBuilder);
                        break;
                    case GlyphTexturePrinterDrawingTechnique.Stencil:
                        _pcx.DrawGlyphImageWithStecil_VBO(_glBmp, _vboBuilder);
                        break;
                    case GlyphTexturePrinterDrawingTechnique.Msdf:
                        _pcx.DrawImagesWithMsdf_VBO(_glBmp, _vboBuilder);
                        break;
                }

                _vboBuilder.Clear();
            }
        }
#endif    

        GLRenderVxFormattedString _reusableFmtString = new GLRenderVxFormattedString();
        public void DrawString(char[] buffer, int startAt, int len, double left, double top)
        {
            //for internal use
            _reusableFmtString.Reuse(); //use sure
            PrepareStringForRenderVx(_reusableFmtString, buffer, startAt, len);
            DrawString(_reusableFmtString, left, top);

            _reusableFmtString.Reuse(); //clear data
        }

        public bool WordPlateCreatingMode { get; set; }

        public void DrawString(GLRenderVxFormattedString vxFmtStr, double x, double y)
        {
            _pcx.FontFillColor = _painter.FontFillColor;
            if (vxFmtStr.Delay && vxFmtStr.OwnerPlate == null)
            {
                //add this to queue to create                              
                return;
            }

            float base_offset = 0;
            switch (TextBaseline)
            {
                case TextBaseline.Alphabetic:
                    //base_offset = -(vxFmtStr.SpanHeight + vxFmtStr.DescendingInPx);
                    break;
                case TextBaseline.Top:
                    base_offset = vxFmtStr.DescendingInPx;
                    break;
                case TextBaseline.Bottom:
                    base_offset = -vxFmtStr.SpanHeight;
                    break;
            }

            if (WordPlateCreatingMode)
            {
                //on word plate mode
                //in this version
                //we prepare only stencil glyph

                switch (vxFmtStr.GlyphMixMode)
                {
                    case GLRenderVxFormattedStringGlyphMixMode.OnlyColorGlyphs:
                        {
                            //all glyph are color
                            List<SameFontWordPlateTextStrip> strips = vxFmtStr._strips;
                            int j = strips.Count;
                            float start_x = (float)Math.Round(x);
                            float start_y = (float)Math.Floor(y + base_offset);

                            for (int n = 0; n < j; ++n)
                            {
                                SameFontWordPlateTextStrip s = strips[n];

                                //change font, bitmap atlas , px_scale
                                ChangeFont(s.ActualFont);


                                _pcx.DrawGlyphImageWithCopyTech_FromVBO(
                                   _glBmp,
                                   s.GetVbo(),
                                   s.IndexArrayCount,
                                   start_x,
                                   start_y);
                            }
                            return;//**
                        }
                    case GLRenderVxFormattedStringGlyphMixMode.MixedStencilAndColorGlyphs:
                    case GLRenderVxFormattedStringGlyphMixMode.OnlyStencilGlyphs:
                        {
                            //in this mode =>don't draw color bitmap

                            List<SameFontWordPlateTextStrip> strips = vxFmtStr._strips;
                            int j = strips.Count;
                            float start_x = (float)Math.Round(x);
                            float start_y = (float)Math.Floor(y + base_offset);

                            switch (TextDrawingTechnique)
                            {
                                case GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering:
                                    {
                                        for (int n = 0; n < j; ++n)
                                        {
                                            SameFontWordPlateTextStrip s = strips[n];
                                            if (s.ColorGlyphOnTransparentBG)
                                            {
                                                continue;
                                            }
                                            //change font, bitmap atlas , px_scale
                                            ChangeFont(s.ActualFont);

                                            _pcx.DrawGlyphImageWithSubPixelRenderingTechnique4_ForWordStrip_FromVBO(
                                                      _glBmp,
                                                      s.GetVbo(),
                                                      s.IndexArrayCount,
                                                      start_x,
                                                      start_y);
                                        }

                                    }
                                    break;
                                case GlyphTexturePrinterDrawingTechnique.Stencil:
                                    {
                                        for (int n = 0; n < j; ++n)
                                        {
                                            SameFontWordPlateTextStrip s = strips[n];

                                            if (s.ColorGlyphOnTransparentBG)
                                            {
                                                continue;
                                            }
                                            //change font, bitmap atlas , px_scale
                                            ChangeFont(s.ActualFont);

                                            _pcx.DrawGlyphImageWithStencilRenderingTechnique4_FromVBO(
                                                   _glBmp,
                                                   s.GetVbo(),
                                                   s.IndexArrayCount,
                                                   start_x,
                                                   start_y);
                                        }
                                    }
                                    break;
                            }
                            return;//** 
                        }
                }
            }


            if (vxFmtStr.GlyphMixMode == GLRenderVxFormattedStringGlyphMixMode.OnlyColorGlyphs)
            {
                //eg. bitmap glyph 
                //---------
                //use word plate 
                if (vxFmtStr.UseWithWordPlate && (vxFmtStr.OwnerPlate != null || _painter.TryCreateWordStrip(vxFmtStr)))
                {
                    //UseWithWordPlate=> this renderVx has beed assign to wordplate,
                    //but when OwnerPlate ==null, this mean the wordplate was disposed.
                    //so create it again
                    //Has WordPlate
                    _pcx.DrawWordSpanWithCopyTechnique((GLBitmap)vxFmtStr.OwnerPlate._backBuffer.GetImage(),
                        vxFmtStr.WordPlateLeft, -vxFmtStr.WordPlateTop - vxFmtStr.SpanHeight,
                        vxFmtStr.Width, vxFmtStr.SpanHeight,
                        (float)Math.Round(x),
                        (float)Math.Floor(y + base_offset));
                }
                else
                {
                    //BUT if it not success => then...

                    List<SameFontWordPlateTextStrip> strips = vxFmtStr._strips;
                    int j = strips.Count;
                    float start_x = (float)Math.Round(x);
                    float start_y = (float)Math.Floor(y + base_offset);

                    for (int n = 0; n < j; ++n)
                    {
                        SameFontWordPlateTextStrip s = strips[n];

                        //change font, bitmap atlas , px_scale
                        ChangeFont(s.ActualFont);


                        _pcx.DrawGlyphImageWithCopyTech_FromVBO(
                           _glBmp,
                           s.GetVbo(),
                           s.IndexArrayCount,
                           start_x,
                           start_y);
                    }
                }
            }
            else
            {
                //draw stencil
                switch (TextDrawingTechnique)
                {
                    default: throw new NotSupportedException();
                    case GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering:
                        {
                            //LCD-Effect**** 
                            //use word plate 
                            Color bgColorHint = _painter.TextBgColorHint;
                            if (vxFmtStr.UseWithWordPlate && bgColorHint.A == 255 && (vxFmtStr.OwnerPlate != null || _painter.TryCreateWordStrip(vxFmtStr)))
                            {
                                //1. opaque bg is candidate for word plate
                                //2. if it already has a WordPlate then render with WordPlate
                                //3. if it does not have a WordPlate try create a WordStrip=>
                                //      if success then render with word plate

                                //review:
                                //solid bg color
                                //TODO: configure this value to range 
                                //since this works with since some light color (near white) too 
                                _pcx.DrawWordSpanWithLcdSubpixForSolidBgColor((GLBitmap)vxFmtStr.OwnerPlate._backBuffer.GetImage(),
                                  vxFmtStr.WordPlateLeft, -vxFmtStr.WordPlateTop - vxFmtStr.SpanHeight - base_offset,
                                  vxFmtStr.Width, vxFmtStr.SpanHeight,
                                  (float)Math.Round(x),
                                  (float)Math.Floor(y + base_offset), //USE base_offset
                                  _painter.TextBgColorHint);
                            }
                            else
                            {
                                //not use wordplate
                                if (_painter.PreparingWordStrip)
                                {
                                    List<SameFontWordPlateTextStrip> strips = vxFmtStr._strips;
                                    int j = strips.Count;
                                    float start_x = (float)Math.Round(x);
                                    float start_y = (float)Math.Floor(y + base_offset);

                                    for (int n = 0; n < j; ++n)
                                    {
                                        SameFontWordPlateTextStrip s = strips[n];

                                        //change font, bitmap atlas , px_scale
                                        ChangeFont(s.ActualFont);

                                        _pcx.DrawGlyphImageWithSubPixelRenderingTechnique4_ForWordStrip_FromVBO(
                                                           _glBmp,
                                                           s.GetVbo(),
                                                           s.IndexArrayCount,
                                                           start_x,
                                                           start_y);
                                    }
                                }
                                else
                                {
                                    List<SameFontWordPlateTextStrip> strips = vxFmtStr._strips;
                                    int j = strips.Count;
                                    float start_x = (float)Math.Round(x);
                                    float start_y = (float)Math.Floor(y + base_offset);
                                    float spanHeight = 0;
                                    float spanWidth = 0;

                                    for (int n = 0; n < j; ++n)
                                    {
                                        SameFontWordPlateTextStrip s = strips[n];
                                        spanHeight = s.SpanHeight;
                                        spanWidth += s.Width;

                                        //change font, bitmap atlas , px_scale
                                        ChangeFont(s.ActualFont);


                                        _pcx.DrawGlyphImageWithSubPixelRenderingTechnique4_FromVBO(
                                                           _glBmp,
                                                           s.GetVbo(),
                                                           s.IndexArrayCount,
                                                           start_x,
                                                           start_y);
                                    }
                                }
                            }

                        }
                        break;
                    case GlyphTexturePrinterDrawingTechnique.Stencil:
                        {

                            if (vxFmtStr.UseWithWordPlate && (vxFmtStr.OwnerPlate != null || _painter.TryCreateWordStrip(vxFmtStr)))
                            {
                                _pcx.DrawWordSpanWithStencilTechnique((GLBitmap)vxFmtStr.OwnerPlate._backBuffer.GetImage(),
                                    vxFmtStr.WordPlateLeft, -vxFmtStr.WordPlateTop - vxFmtStr.SpanHeight,
                                    vxFmtStr.Width, vxFmtStr.SpanHeight,
                                    (float)Math.Round(x),
                                    (float)Math.Floor(y));//*** NO base_offset here
                            }
                            else
                            {
                                //can't create at this time
                                //render with vbo 
                                List<SameFontWordPlateTextStrip> strips = vxFmtStr._strips;
                                int j = strips.Count;
                                float start_x = (float)Math.Round(x);
                                float start_y = (float)Math.Floor(y + base_offset);

                                for (int n = 0; n < j; ++n)
                                {
                                    SameFontWordPlateTextStrip s = strips[n];

                                    //change font, bitmap atlas , px_scale
                                    ChangeFont(s.ActualFont);

                                    _pcx.DrawGlyphImageWithStencilRenderingTechnique4_FromVBO(
                                                       _glBmp,
                                                       s.GetVbo(),
                                                       s.IndexArrayCount,
                                                       start_x,
                                                       start_y);

                                }
                            }
                        }
                        break;
                }

                if (vxFmtStr.GlyphMixMode == GLRenderVxFormattedStringGlyphMixMode.MixedStencilAndColorGlyphs)
                {
                    ////draw color pat
                    List<SameFontWordPlateTextStrip> strips = vxFmtStr._strips;
                    int j = strips.Count;
                    float start_x = (float)Math.Round(x);
                    float start_y = (float)Math.Floor(y + base_offset);

                    for (int n = 0; n < j; ++n)
                    {
                        SameFontWordPlateTextStrip s = strips[n];
                        if (s.ColorGlyphOnTransparentBG)
                        {
                            //change font, bitmap atlas , px_scale
                            ChangeFont(s.ActualFont);
                            //this.TextDrawingTechnique = GlyphTexturePrinterDrawingTechnique.Copy;
                            _pcx.DrawGlyphImageWithCopyBGRATech_FromVBO(
                               _glBmp,
                               s.GetVbo(),
                               s.IndexArrayCount,
                               start_x,
                               start_y + s.AdditionalVerticalOffset);
                        }
                    }
                }
            }
        }
#if DEBUG
        static int _dbugCount;
#endif

        void CreateTextCoords(SameFontWordPlateTextStrip sameFont, List<GLFormattedGlyphPlanSeq> seqs)
        {
            int top = 0;//simulate top
            int left = 0;//simulate left

            //change font once
            RequestFont reqFont = sameFont.ActualFont;
#if DEBUG
            if (reqFont == null) { throw new NotSupportedException(); }
#endif

            ChangeFont(reqFont);

            //TODO: review here, rounding error*** 
            sameFont.SpanHeight = _resolvedFont.LineSpacingInPixels;
            sameFont.DescendingInPx = (short)_resolvedFont.DescentInPixels;

            int count = seqs.Count;
            float g_left = 0;
            float g_top = 0;

            float scaleFromTexture = 1; //TODO: support msdf auto scale 

            float acc_x = 0; //local accumulate x
            float acc_y = 0; //local accumulate y  
            bool hasSomeGlyphs = false;

            //reset after change font....
            float px_scale = _px_scale; //init
            float bottom = (float)top + _resolvedFont.AscentInPixels - _resolvedFont.DescentInPixels;
            int each_white_spaceW = (int)Math.Round(_white_space_width);


            _vboBuilder.Clear();//***
            _vboBuilder.SetTextureInfo(_glBmp.Width, _glBmp.Height, _glBmp.IsYFlipped, _pcx.OriginKind);

            for (int s = 0; s < count; ++s)
            {
                GLFormattedGlyphPlanSeq sq = seqs[s];
                bool isTargetFont = sq.ActualFont == reqFont;

                ChangeFont(sq.ActualFont);

                px_scale = _px_scale; //init
                bottom = (float)top + _resolvedFont.AscentInPixels - _resolvedFont.DescentInPixels;
                each_white_spaceW = (int)Math.Round(_white_space_width);

                if (sq.PrefixWhitespaceCount > 0)
                {
                    acc_x += each_white_spaceW * sq.PrefixWhitespaceCount;
                }


                //create temp buffer span that describe the part of a whole char buffer 
                //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
                //with specific request font   
                GlyphPlanSequence glyphPlanSeq = sq.seq;
                int seqLen = glyphPlanSeq.Count;
                for (int i = 0; i < seqLen; ++i)
                {
                    UnscaledGlyphPlan glyph = glyphPlanSeq[i];
                    if (!_fontAtlas.TryGetItem(glyph.glyphIndex, out AtlasItem atlasItem))
                    {
                        //if no glyph data, we should render a missing glyph ***
                        continue;
                    }
                    if (!isTargetFont)
                    {
                        //not the target
                        //find only advance and go next
                        acc_x += (float)Math.Round(glyph.AdvanceX * px_scale);
                        continue;//***
                    }

                    //--------------------------------------
                    //TODO: review precise height in float
                    //--------------------------------------   

                    var srcRect = new Rectangle(atlasItem.Left, atlasItem.Top, atlasItem.Width, atlasItem.Height);

                    //offset length from 'base-line'
                    float x_offset = acc_x + (float)Math.Round(glyph.OffsetX * px_scale - atlasItem.TextureXOffset);
                    float y_offset = acc_y + (float)Math.Round(glyph.OffsetY * px_scale - atlasItem.TextureYOffset) + srcRect.Height; //***

                    //NOTE:
                    // -glyphData.TextureXOffset => restore to original pos
                    // -glyphData.TextureYOffset => restore to original pos 
                    //--------------------------              

                    g_left = (float)(left + x_offset);
                    g_top = (float)(bottom - y_offset); //***

                    acc_x += (float)Math.Round(glyph.AdvanceX * px_scale);
                    g_top = (float)Math.Floor(g_top);//adjust to integer num ***  

                    hasSomeGlyphs = true;
                    _vboBuilder.WriteRect(srcRect, g_left, g_top, scaleFromTexture);

                }

                //
                if (sq.PostfixWhitespaceCount > 0)
                {
                    //***
                    if (isTargetFont)
                    {
                        _vboBuilder.AppendDegenerativeTriangle();
                    }
                    acc_x += each_white_spaceW * sq.PostfixWhitespaceCount;
                }
            }

            if (hasSomeGlyphs)
            {
                _vboBuilder.AppendDegenerativeTriangle();
            }

            sameFont.IndexArrayCount = _vboBuilder._indexList.Count;
            sameFont.IndexArray = _vboBuilder._indexList.ToArray();
            sameFont.VertexCoords = _vboBuilder._buffer.ToArray();
            sameFont.Width = acc_x;


            _vboBuilder.Clear();
        }

        public AlternativeTypefaceSelector AlternativeTypefaceSelector { get; set; }


        Dictionary<Typeface, RequestFont> _uniqueTypefaces = new Dictionary<Typeface, RequestFont>();
        List<GLFormattedGlyphPlanSeq> _fmtGlyphPlanSeqs = new List<GLFormattedGlyphPlanSeq>();

        public void PrepareStringForRenderVx(GLRenderVxFormattedString vxFmtStr, char[] buffer, int startAt, int len)
        {
            //we need to parse string 
            //since it may contains glyph from multiple font (eg. eng, emoji etc.)
            //see VxsTextPrinter 

            var buffSpan = new TextBufferSpan(buffer, startAt, len);

            RequestFont reqFont = _painter.CurrentFont; //init with default

            //resolve this type face

            ResolvedFont resolvedFont = _textServices.ResolveFont(reqFont);
            Typeface defaultTypeface = resolvedFont.Typeface;
            Typeface curTypeface = defaultTypeface;

            bool needRightToLeftArr = false;

            _lineSegs.Clear();
            _textPrinterWordVisitor.SetLineSegmentList(_lineSegs);
            _textServices.BreakToLineSegments(buffSpan, _textPrinterWordVisitor);
            _textPrinterWordVisitor.SetLineSegmentList(null);
            //typeface may not have a glyph for some char
            //eg eng font + emoji


            //check if we have a mix stencil and color glyph or not
            GLRenderVxFormattedStringGlyphMixMode glyphMixMode = GLRenderVxFormattedStringGlyphMixMode.Unknown;
            GLFormattedGlyphPlanSeq latestFmtGlyphPlanSeq = null;

            _uniqueTypefaces.Clear();
            _fmtGlyphPlanSeqs.Clear();

            int prefix_whitespaceCount = 0;

            int count = _lineSegs.Count;
            for (int i = 0; i < count; ++i)
            {
                TextPrinterLineSegment line_seg = _lineSegs.GetLineSegment(i);
                //find a proper font for each segment
                //so we need to check 
                SpanBreakInfo spBreakInfo = line_seg.BreakInfo;
                if (line_seg.WordKind == WordKind.Whitespace)
                {
                    if (latestFmtGlyphPlanSeq == null)
                    {
                        prefix_whitespaceCount += line_seg.Length;
                    }
                    else
                    {
                        latestFmtGlyphPlanSeq.PostfixWhitespaceCount += line_seg.Length;
                    }
                    continue; //***
                }
                //---------------

                TextBufferSpan buff = new TextBufferSpan(buffer, line_seg.StartAt, line_seg.Length);
                if (spBreakInfo.RightToLeft)
                {
                    needRightToLeftArr = true;
                }
                //each line segment may have different unicode range 
                //and the current typeface may not support that range
                //so we need to ensure that we get a proper typeface,
                //if not => alternative typeface

                ushort glyphIndex = 0;
                char sample_char = buffer[line_seg.StartAt];


                int codepoint = sample_char;
                if (line_seg.Length > 1 && line_seg.WordKind == WordKind.SurrogatePair)
                {
                    //high serogate pair or not
                    glyphIndex = curTypeface.GetGlyphIndex(codepoint = char.ConvertToUtf32(sample_char, buffer[line_seg.StartAt + 1]));
                }
                else
                {
                    glyphIndex = curTypeface.GetGlyphIndex(codepoint);
                }

                //------------
                if (glyphIndex == 0)
                {
                    //not found then => find other typeface                    
                    //we need more information about line seg layout
                    if (AlternativeTypefaceSelector != null)
                    {
                        AlternativeTypefaceSelector.LatestTypeface = curTypeface;
                    }

                    if (_textServices.TryGetAlternativeTypefaceFromCodepoint(codepoint, AlternativeTypefaceSelector, out Typeface alternative))
                    {
                        //change to another
                        curTypeface = alternative;
                    }
                    else
                    {
#if DEBUG
                        if (sample_char >= 0 && sample_char < 255)
                        {


                        }
#endif
                    }
                }


                _textServices.CurrentScriptLang = new ScriptLang(spBreakInfo.ScriptTag, spBreakInfo.LangTag);
                //layout glyphs in each context

                GlyphPlanSequence seq = _textServices.CreateGlyphPlanSeq(buff, curTypeface, reqFont.SizeInPoints);

                seq.IsRightToLeft = spBreakInfo.RightToLeft;


                if (!_uniqueTypefaces.TryGetValue(curTypeface, out RequestFont reqFont2))
                {
                    //typeface can be key, because,each time this method is called,  font size/style must be the same 
                    reqFont2 = new RequestFont(curTypeface.Name, reqFont.SizeInPoints);
                    _uniqueTypefaces[curTypeface] = reqFont2;
                }


                //----------------
                //create an object that hold more information about GlyphPlanSequence

                GLFormattedGlyphPlanSeq formattedGlyphPlanSeq = new GLFormattedGlyphPlanSeq
                {
                    seq = seq,
                    ActualFont = reqFont2,
                    Typeface = curTypeface,

                    ColorGlyphOnTransparentBG = (curTypeface.HasSvgTable() || curTypeface.IsBitmapFont || curTypeface.HasColorTable()),
                    PrefixWhitespaceCount = (ushort)prefix_whitespaceCount//***
                };
                //add to temp location 

                _fmtGlyphPlanSeqs.Add(latestFmtGlyphPlanSeq = formattedGlyphPlanSeq);

                prefix_whitespaceCount = 0;//reset


                switch (glyphMixMode)
                {
                    case GLRenderVxFormattedStringGlyphMixMode.Unknown:
                        {
                            if (formattedGlyphPlanSeq.ColorGlyphOnTransparentBG)
                            {
                                glyphMixMode = GLRenderVxFormattedStringGlyphMixMode.OnlyColorGlyphs;
                            }
                            else
                            {
                                glyphMixMode = GLRenderVxFormattedStringGlyphMixMode.OnlyStencilGlyphs;
                            }
                        }
                        break;
                    case GLRenderVxFormattedStringGlyphMixMode.MixedStencilAndColorGlyphs:
                        //not need to evalute 
                        break;
                    case GLRenderVxFormattedStringGlyphMixMode.OnlyColorGlyphs:
                        {
                            if (formattedGlyphPlanSeq.ColorGlyphOnTransparentBG)
                            {
                                //already color
                            }
                            else
                            {
                                glyphMixMode = GLRenderVxFormattedStringGlyphMixMode.MixedStencilAndColorGlyphs;
                            }
                        }
                        break;
                    case GLRenderVxFormattedStringGlyphMixMode.OnlyStencilGlyphs:
                        {
                            if (formattedGlyphPlanSeq.ColorGlyphOnTransparentBG)
                            {
                                //
                                glyphMixMode = GLRenderVxFormattedStringGlyphMixMode.MixedStencilAndColorGlyphs;
                            }
                            else
                            {

                            }
                        }
                        break;
                }


            }

            //-------------             
            //a fmtGlyphPlanSeqs may contains glyph from  more than 1 font,
            //now, create a overlapped strip for each 

            vxFmtStr._strips.Clear();

            float spanHeight = 0;
            float spanWidth = 0;
            int descendingInPx = 0;

            int maxStripHeight = 0;
            foreach (var kv in _uniqueTypefaces)
            {
                SameFontWordPlateTextStrip sameFontTextStrip = new SameFontWordPlateTextStrip();
                sameFontTextStrip.ActualFont = kv.Value;
                Typeface typeface = kv.Key;
                sameFontTextStrip.ColorGlyphOnTransparentBG = (typeface.HasSvgTable() || typeface.IsBitmapFont || typeface.HasColorTable());

                //**
                CreateTextCoords(sameFontTextStrip, _fmtGlyphPlanSeqs);
                //**
                //use max size of height and descending ?
                descendingInPx = sameFontTextStrip.DescendingInPx;

                maxStripHeight = Math.Max(maxStripHeight, sameFontTextStrip.SpanHeight);

                spanWidth = sameFontTextStrip.Width;//same width for all strip                 
                vxFmtStr._strips.Add(sameFontTextStrip);
            }

            //adjust addition vertical height

            spanHeight = maxStripHeight;
            int stripCount = vxFmtStr._strips.Count;
            for (int i = 0; i < stripCount; ++i)
            {
                SameFontWordPlateTextStrip sameFontTextStrip = vxFmtStr._strips[i];
                sameFontTextStrip.AdditionalVerticalOffset = maxStripHeight - sameFontTextStrip.SpanHeight;
            }


            vxFmtStr.GlyphMixMode = glyphMixMode;
            vxFmtStr.SpanHeight = spanHeight;
            vxFmtStr.Width = spanWidth;
            vxFmtStr.DescendingInPx = (short)descendingInPx;


            //-----------
            //TODO: review here again

            if (vxFmtStr.Delay)
            {
                //when we use delay mode
                //we need to save current font setting  of the _painter
                //with the render vx---
                //vxFmtStr.RequestFont = _painter.CurrentFont;
            }
            else
            {
                //TODO: review here again   
                _painter.TryCreateWordStrip(vxFmtStr);
            }

            ClearTempFormattedGlyphPlanSeqList();
        }



        void ClearTempFormattedGlyphPlanSeqList()
        {
            _uniqueTypefaces.Clear();
            _fmtGlyphPlanSeqs.Clear();
        }
    }


}


