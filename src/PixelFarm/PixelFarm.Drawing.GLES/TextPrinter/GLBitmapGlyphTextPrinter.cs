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
using Typography.TextServices;
using Typography.FontManagement;
using Typography.OpenFont.Extensions;

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

        ResolvedFont _resolvedFont;
        readonly TextServiceClient _txtClient;
        readonly TextureCoordVboBuilder _vboBuilder = new TextureCoordVboBuilder();



        float _px_scale = 1;
        int _white_space_width;
        float _ascending;
        float _descending;
        float _lineSpacingInPx;
        float _fontSizeInPoints;

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

            _txtClient = textServices.CreateNewServiceClient();

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
                var preferTypefaces = new Typography.TextServices.PreferredTypefaceList();
                preferTypefaces.AddTypefaceName("Source Sans Pro");
                preferTypefaces.AddTypefaceName("Sarabun");


                myAlternativeTypefaceSelector.SetPreferredTypefaces(
                     new[]{Typography.TextBreak.Unicode13RangeInfoList.C0_Controls_and_Basic_Latin,
                               Typography.TextBreak.Unicode13RangeInfoList.C1_Controls_and_Latin_1_Supplement,
                               Typography.TextBreak.Unicode13RangeInfoList.Latin_Extended_A,
                               Typography.TextBreak.Unicode13RangeInfoList.Latin_Extended_B,
                     },
                    preferTypefaces);
            }
            AlternativeTypefaceSelector = myAlternativeTypefaceSelector;
        }
        public TextServiceClient TextClient => _txtClient;
        void ChangeFont(SameFontTextStrip s)
        {
            ResolvedFont r_font = s.ResolvedFont;
#if DEBUG
            if (r_font == null) { throw new NotSupportedException(); }
#endif
            _px_scale = r_font.GetScaleToPixelFromPointInSize();
            _white_space_width = r_font.WhitespaceWidth;
            _ascending = r_font.AscentInPixels;
            _descending = r_font.DescentInPixels;
            _lineSpacingInPx = r_font.LineSpacingInPixels;

            _fontAtlas = _myGLBitmapFontMx.GetFontAtlas(r_font, out _glBmp);
        }
        void ChangeFont(FormattedGlyphPlanSeq s)
        {

            //_resolvedFont = font;
            ResolvedFont r_font = s.ResolvedFont;
#if DEBUG
            if (r_font == null) { throw new NotSupportedException(); }
#endif
            Typeface typeface = r_font.Typeface;
            _fontSizeInPoints = r_font.SizeInPoints;
            _px_scale = r_font.GetScaleToPixelFromPointInSize();

            _white_space_width = r_font.WhitespaceWidth;//(int)Math.Round(typeface.GetWhitespaceWidth() * _px_scale);
            _ascending = r_font.AscentInPixels;
            _descending = r_font.DescentInPixels;
            _lineSpacingInPx = r_font.LineSpacingInPixels; //typeface.CalculateMaxLineClipHeight() * _px_scale;

            _fontAtlas = _myGLBitmapFontMx.GetFontAtlas(r_font, out _glBmp);

        }
        public void ChangeFont(ResolvedFont font)
        {
            if (_resolvedFont != null && font != null && _resolvedFont.FontKey == font.FontKey)
            {
                return;
            }
#if DEBUG
            //if (font.Name.ToLower().Contains("emoji"))
            //{

            //}
#endif

            _resolvedFont = font;

            _fontSizeInPoints = font.SizeInPoints;
            _px_scale = font.GetScaleToPixelFromPointInSize();
            _white_space_width = font.WhitespaceWidth;
            _ascending = font.AscentInPixels;
            _descending = font.DescentInPixels;
            _lineSpacingInPx = font.LineSpacingInPixels;

            //-------
            _fontAtlas = _myGLBitmapFontMx.GetFontAtlas(font, out _glBmp);
        }
        public void ChangeFont(RequestFont font)
        {
            if (_resolvedFont != null && font != null && _resolvedFont.FontKey == font.FontKey)
            {
                return;
            }
            ChangeFont(_txtClient.ResolveFont(font));
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
            Size s = _txtClient.MeasureString(textBufferSpan, _painter.CurrentFont);
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


            GlyphPlanSequence glyphPlanSeq = _txtClient.CreateGlyphPlanSeq(textBufferSpan, _resolvedFont);
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

            float scaleFromTexture = _resolvedFont.SizeInPoints / _fontAtlas.OriginalFontSizePts;

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
                int baseLine = (int)Math.Round((float)top + _ascending);
                _painter.DrawLine(left, baseLine, left + 200, baseLine);
                //
                //draw magenta-line-marker for bottom line
                _painter.StrokeColor = Color.Magenta;
                int bottomLine = (int)Math.Round((float)top + _lineSpacingInPx);
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
            float bottom = (float)top + _ascending - _descending;
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
                        g_top += _descending;
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

        readonly GLRenderVxFormattedString _reusableFmtString = new GLRenderVxFormattedString();
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
                            List<SameFontTextStrip> strips = vxFmtStr._strips;
                            int j = strips.Count;
                            float start_x = (float)Math.Round(x);
                            float start_y = (float)Math.Floor(y + base_offset);

                            for (int n = 0; n < j; ++n)
                            {
                                SameFontTextStrip s = strips[n];

                                //change font, bitmap atlas , px_scale

                                ChangeFont(s);

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

                            List<SameFontTextStrip> strips = vxFmtStr._strips;
                            int j = strips.Count;
                            float start_x = (float)Math.Round(x);
                            float start_y = (float)Math.Floor(y + base_offset);

                            switch (TextDrawingTechnique)
                            {
                                case GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering:
                                    {
                                        for (int n = 0; n < j; ++n)
                                        {
                                            SameFontTextStrip s = strips[n];
                                            if (s.ColorGlyphOnTransparentBG)
                                            {
                                                continue;
                                            }
                                            //change font, bitmap atlas , px_scale
                                            ChangeFont(s);
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
                                            SameFontTextStrip s = strips[n];

                                            if (s.ColorGlyphOnTransparentBG)
                                            {
                                                continue;
                                            }
                                            //change font, bitmap atlas , px_scale
                                            ChangeFont(s);

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

                    List<SameFontTextStrip> strips = vxFmtStr._strips;
                    int j = strips.Count;
                    float start_x = (float)Math.Round(x);
                    float start_y = (float)Math.Floor(y + base_offset);

                    for (int n = 0; n < j; ++n)
                    {
                        SameFontTextStrip s = strips[n];
                        //change font, bitmap atlas , px_scale
                        ChangeFont(s);

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
                                    List<SameFontTextStrip> strips = vxFmtStr._strips;
                                    int j = strips.Count;
                                    float start_x = (float)Math.Round(x);
                                    float start_y = (float)Math.Floor(y + base_offset);

                                    for (int n = 0; n < j; ++n)
                                    {
                                        SameFontTextStrip s = strips[n];

                                        //change font, bitmap atlas , px_scale
                                        ChangeFont(s);

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
                                    List<SameFontTextStrip> strips = vxFmtStr._strips;
                                    int j = strips.Count;
                                    float start_x = (float)Math.Round(x);
                                    float start_y = (float)Math.Floor(y + base_offset);
                                    float spanHeight = 0;
                                    float spanWidth = 0;

                                    for (int n = 0; n < j; ++n)
                                    {
                                        SameFontTextStrip s = strips[n];
                                        spanHeight = s.SpanHeight;
                                        spanWidth += s.Width;

                                        //change font, bitmap atlas , px_scale
                                        ChangeFont(s);

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
                                List<SameFontTextStrip> strips = vxFmtStr._strips;
                                int j = strips.Count;
                                float start_x = (float)Math.Round(x);
                                float start_y = (float)Math.Floor(y + base_offset);

                                for (int n = 0; n < j; ++n)
                                {
                                    SameFontTextStrip s = strips[n];

                                    //change font, bitmap atlas , px_scale
                                    ChangeFont(s);

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
                    List<SameFontTextStrip> strips = vxFmtStr._strips;
                    int j = strips.Count;
                    float start_x = (float)Math.Round(x);
                    float start_y = (float)Math.Floor(y + base_offset);

                    for (int n = 0; n < j; ++n)
                    {
                        SameFontTextStrip s = strips[n];
                        if (s.ColorGlyphOnTransparentBG)
                        {
                            //change font, bitmap atlas , px_scale

                            ChangeFont(s);
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


        void CreateTextCoords(SameFontTextStrip txtStrip, List<FormattedGlyphPlanSeq> seqs)
        {
            int top = 0;//simulate top
            int left = 0;//simulate left

            //change font once 
            ResolvedFont expected_resolvedFont = txtStrip.ResolvedFont;
            Typeface expectedTypeface = expected_resolvedFont.Typeface;
#if DEBUG
            if (expectedTypeface == null) { throw new NotSupportedException(); }
#endif

            ChangeFont(txtStrip);

            float px_scale = expected_resolvedFont.GetScaleToPixelFromPointInSize();

            //TODO: review here, rounding error*** 
            txtStrip.SpanHeight = expected_resolvedFont.LineSpacingInPixels;// (int)Math.Round(expectedTypeface.CalculateMaxLineClipHeight() * px_scale);// expectedFont.LineSpacingInPixels;

            //TODO: review here, Rounding error
            int descend_px = (int)expected_resolvedFont.DescentInPixels;// (int)(expectedTypeface.Descender * px_scale);
            int ascentd_px = (int)expected_resolvedFont.AscentInPixels;// (int)(expectedTypeface.Ascender * px_scale);
            txtStrip.DescendingInPx = descend_px;// (short)(expectedTypeface.Descender * px_scale);  //expectedFont.DescentInPixels;

            int count = seqs.Count;
            float g_left = 0;
            float g_top = 0;

            float scaleFromTexture = 1; //TODO: support msdf auto scale 

            float acc_x = 0; //local accumulate x
            float acc_y = 0; //local accumulate y  
            bool hasSomeGlyphs = false;

            //reset after change font....
            //float px_scale = _px_scale; //init
            float bottom = (float)top + ascentd_px - descend_px;//  //(float)top + expectedFont.AscentInPixels - expectedFont.DescentInPixels;
            int each_white_spaceW = _white_space_width;

            //we will create a txtstrip for an expected font only
            //for another font, just skip and wait

            _vboBuilder.Clear();//***
            _vboBuilder.SetTextureInfo(_glBmp.Width, _glBmp.Height, _glBmp.IsYFlipped, _pcx.OriginKind);

            for (int s = 0; s < count; ++s)
            {
                FormattedGlyphPlanSeq sq = seqs[s];
                bool isTargetFont = sq.ResolvedFont.Typeface == expectedTypeface;

                //if this seq use another font=> just calculate entire advance with

                ChangeFont(sq);


                px_scale = _px_scale; //init
                //use current resolve font
                bottom = (float)top + _ascending - _descending;
                each_white_spaceW = _white_space_width;

                if (sq.PrefixWhitespaceCount > 0)
                {
                    acc_x += each_white_spaceW * sq.PrefixWhitespaceCount;
                }


                //create temp buffer span that describe the part of a whole char buffer 
                //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
                //with specific request font   
                GlyphPlanSequence glyphPlanSeq = sq.Seq;
                int seqLen = glyphPlanSeq.Count;
                for (int i = 0; i < seqLen; ++i)
                {
                    UnscaledGlyphPlan glyphPlan = glyphPlanSeq[i];
                    if (!isTargetFont)
                    {
                        //not the target
                        //find only advance and go next
                        acc_x += (float)Math.Round(glyphPlan.AdvanceX * px_scale);
                        continue;//***
                    }

                    if (!_fontAtlas.TryGetItem(glyphPlan.glyphIndex, out AtlasItem atlasItem))
                    {
                        //if no glyph data, we should render a missing glyph ***
                        continue;
                    }

                    //--------------------------------------
                    //TODO: review precise height in float
                    //--------------------------------------   

                    var srcRect = new Rectangle(atlasItem.Left, atlasItem.Top, atlasItem.Width, atlasItem.Height);

                    //offset length from 'base-line'
                    float x_offset = acc_x + (float)Math.Round(glyphPlan.OffsetX * px_scale - atlasItem.TextureXOffset);
                    float y_offset = acc_y + (float)Math.Round(glyphPlan.OffsetY * px_scale - atlasItem.TextureYOffset) + srcRect.Height; //***

                    //NOTE:
                    // -glyphData.TextureXOffset => restore to original pos
                    // -glyphData.TextureYOffset => restore to original pos 
                    //--------------------------              

                    g_left = (float)(left + x_offset);
                    g_top = (float)(bottom - y_offset); //***

                    acc_x += (float)Math.Round(glyphPlan.AdvanceX * px_scale);
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

                txtStrip.IndexArrayCount = _vboBuilder._indexList.Count;
                txtStrip.IndexArray = _vboBuilder._indexList.ToArray();
                txtStrip.VertexCoords = _vboBuilder._buffer.ToArray();

            }

            txtStrip.Width = acc_x;

            _vboBuilder.Clear();
        }

        public AlternativeTypefaceSelector AlternativeTypefaceSelector { get; set; }


        readonly Dictionary<int, ResolvedFont> _uniqueResolvedFonts = new Dictionary<int, ResolvedFont>();
        readonly Dictionary<int, ResolvedFont> _localResolvedFonts = new Dictionary<int, ResolvedFont>();

        readonly List<FormattedGlyphPlanSeq> _fmtGlyphPlanSeqs = new List<FormattedGlyphPlanSeq>();

        ResolvedFont LocalResolveFont(Typeface typeface, float sizeInPoint, FontStyle style)
        {
            //find local resolved font cache

            //check if we have a cache key or not
            int typefaceKey = TypefaceExtensions.GetCustomTypefaceKey(typeface);
            if (typefaceKey == 0)
            {
                throw new System.NotSupportedException();
                ////calculate and cache
                //TypefaceExtensions.SetCustomTypefaceKey(typeface,
                //    typefaceKey = RequestFont.CalculateTypefaceKey(typeface.Name));
            }

            int key = RequestFont.CalculateFontKey(typefaceKey, sizeInPoint, style);
            if (!_localResolvedFonts.TryGetValue(key, out ResolvedFont found))
            {
                return _localResolvedFonts[key] = new ResolvedFont(typeface, sizeInPoint, key);
            }
            return found;
        }

        public void PrepareStringForRenderVx(GLRenderVxFormattedString vxFmtStr, char[] buffer, int startAt, int len)
        {
            //we need to parse string 
            //since it may contains glyph from multiple font (eg. eng, emoji etc.)
            //see VxsTextPrinter 

            var buffSpan = new TextBufferSpan(buffer, startAt, len);

            RequestFont reqFont = _painter.CurrentFont; //init with default 
            ResolvedFont resolvedFont = _txtClient.ResolveFont(reqFont);
            //Typeface defaultTypeface = resolvedFont.Typeface;
            Typeface curTypeface = resolvedFont.Typeface;

            bool needRightToLeftArr = false;

            _lineSegs.Clear();
            _textPrinterWordVisitor.SetLineSegmentList(_lineSegs);
            _txtClient.BreakToLineSegments(buffSpan, _textPrinterWordVisitor);
            _textPrinterWordVisitor.SetLineSegmentList(null);
            //typeface may not have a glyph for some char
            //eg eng font + emoji


            //check if we have a mix stencil and color glyph or not

            FormattedGlyphPlanSeq latestFmtGlyphPlanSeq = null;
            int prefix_whitespaceCount = 0;

            _uniqueResolvedFonts.Clear();
            _uniqueResolvedFonts.Add(resolvedFont.FontKey, resolvedFont);

            _fmtGlyphPlanSeqs.Clear();




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
                //--------------

                if (spBreakInfo.RightToLeft)
                {
                    needRightToLeftArr = true;
                }

                TextBufferSpan buff = new TextBufferSpan(buffer, line_seg.StartAt, line_seg.Length);

                //each line segment may have different unicode range 
                //and the current typeface may not support that range
                //so we need to ensure that we get a proper typeface,
                //if not => alternative typeface

                ushort sample_glyphIndex = 0;
                char sample_char = buffer[line_seg.StartAt];
                int codepoint = sample_char;

                if (line_seg.Length > 1 && line_seg.WordKind == WordKind.SurrogatePair)
                {
                    sample_glyphIndex = curTypeface.GetGlyphIndex(codepoint = char.ConvertToUtf32(sample_char, buffer[line_seg.StartAt + 1]));
                }
                else
                {
                    sample_glyphIndex = curTypeface.GetGlyphIndex(codepoint);
                }

                //------------

                if (sample_glyphIndex == 0)
                {
                    //not found then => find other typeface                    
                    //we need more information about line seg layout
                    if (AlternativeTypefaceSelector != null)
                    {
                        AlternativeTypefaceSelector.LatestTypeface = curTypeface;
                    }

                    if (_txtClient.TryGetAlternativeTypefaceFromCodepoint(codepoint, AlternativeTypefaceSelector, out Typeface alternative))
                    {
                        //change to another
                        //create local resolved font
                        curTypeface = alternative;
                        resolvedFont = LocalResolveFont(curTypeface, _fontSizeInPoints, FontStyle.Regular);
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


                _txtClient.CurrentScriptLang = new ScriptLang(spBreakInfo.ScriptTag, spBreakInfo.LangTag);

                //in some text context (+typeface)=>user can disable gsub, gpos
                //this is an example
                if (line_seg.WordKind == WordKind.Tab || line_seg.WordKind == WordKind.Number ||
                       (spBreakInfo.UnicodeRange == Unicode13RangeInfoList.C0_Controls_and_Basic_Latin))
                {
                    _txtClient.EnableGpos = false;
                    _txtClient.EnableGsub = false;
                }
                else
                {
                    _txtClient.EnableGpos = true;
                    _txtClient.EnableGsub = true;
                }
                //layout glyphs in each context
                GlyphPlanSequence seq = _txtClient.CreateGlyphPlanSeq(buff, curTypeface, reqFont.SizeInPoints);
                seq.IsRightToLeft = spBreakInfo.RightToLeft;

                //calculate font key => get resolved font


                if (!_uniqueResolvedFonts.ContainsKey(resolvedFont.FontKey))
                {
                    _uniqueResolvedFonts.Add(resolvedFont.FontKey, resolvedFont);
                }

                //----------------
                //create an object that hold more information about GlyphPlanSequence

                FormattedGlyphPlanSeq formattedGlyphPlanSeq = new FormattedGlyphPlanSeq
                {
                    PrefixWhitespaceCount = (ushort)prefix_whitespaceCount//***
                };
                prefix_whitespaceCount = 0;//reset 

                formattedGlyphPlanSeq.SetData(seq, resolvedFont);


                _fmtGlyphPlanSeqs.Add(latestFmtGlyphPlanSeq = formattedGlyphPlanSeq);
            }

            //-------------             
            //a fmtGlyphPlanSeqs may contains glyph from  more than 1 font,
            //now, create a overlapped strip for each 
#if DEBUG
            if (vxFmtStr._strips.Count > 0) { throw new NotSupportedException(); }
#endif


            float spanHeight = 0;
            float spanWidth = 0;
            int descendingInPx = 0;
            int maxStripHeight = 0;

            GlyphMixModeSummary mixModeSummary = new GlyphMixModeSummary();

            foreach (var kv in _uniqueResolvedFonts)
            {
                //once for each typeface***
                ResolvedFont resolvedFont1 = kv.Value;

#if DEBUG
                if (resolvedFont1 == null) { throw new NotSupportedException(); }
#endif

                Typeface typeface = resolvedFont1.Typeface;

                SameFontTextStrip sameFontTextStrip = new SameFontTextStrip
                {
                    ResolvedFont = resolvedFont1
                };



                //assign ColorGlyphOnTransparentBG and update mix mode state
                mixModeSummary.AddGlyphMixMode(
                    sameFontTextStrip.ColorGlyphOnTransparentBG =
                            (typeface.HasSvgTable() || typeface.IsBitmapFont || typeface.HasColorTable()));


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
                SameFontTextStrip sameFontTextStrip = vxFmtStr._strips[i];
                sameFontTextStrip.AdditionalVerticalOffset = maxStripHeight - sameFontTextStrip.SpanHeight;
            }


            vxFmtStr.GlyphMixMode = mixModeSummary.FinalMixMode;

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

            //restore prev typeface & settings
        }



        void ClearTempFormattedGlyphPlanSeqList()
        {
            _uniqueResolvedFonts.Clear();
            _fmtGlyphPlanSeqs.Clear();
        }


        /// <summary>
        /// helper struct for summarize glyph mixed mode
        /// </summary>
        struct GlyphMixModeSummary
        {
            public GLRenderVxFormattedStringGlyphMixMode FinalMixMode { get; private set; }
            public void AddGlyphMixMode(bool colorGlyphOnTransparentBG)
            {
                switch (FinalMixMode)
                {
                    case GLRenderVxFormattedStringGlyphMixMode.Unknown:

                        if (colorGlyphOnTransparentBG)
                        {
                            FinalMixMode = GLRenderVxFormattedStringGlyphMixMode.OnlyColorGlyphs;
                        }
                        else
                        {
                            FinalMixMode = GLRenderVxFormattedStringGlyphMixMode.OnlyStencilGlyphs;
                        }

                        break;
                    case GLRenderVxFormattedStringGlyphMixMode.MixedStencilAndColorGlyphs:
                        //not need to evalute 
                        break;
                    case GLRenderVxFormattedStringGlyphMixMode.OnlyColorGlyphs:

                        if (colorGlyphOnTransparentBG)
                        {
                            //already color
                        }
                        else
                        {
                            FinalMixMode = GLRenderVxFormattedStringGlyphMixMode.MixedStencilAndColorGlyphs;
                        }

                        break;
                    case GLRenderVxFormattedStringGlyphMixMode.OnlyStencilGlyphs:

                        if (colorGlyphOnTransparentBG)
                        {
                            //
                            FinalMixMode = GLRenderVxFormattedStringGlyphMixMode.MixedStencilAndColorGlyphs;
                        }
                        else
                        {

                        }

                        break;
                }
            }
        }

    }


}


