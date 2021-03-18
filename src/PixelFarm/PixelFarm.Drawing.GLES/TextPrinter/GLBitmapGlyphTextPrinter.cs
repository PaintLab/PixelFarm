//MIT, 2016-present, WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;
using PixelFarm.Drawing;

using Typography.OpenFont;
using Typography.OpenFont.Extensions;
using Typography.Text;
using Typography.TextBreak;
using Typography.TextLayout;

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


        readonly GLPainterCore _pcx;
        readonly GLPainter _painter;
        readonly TextServiceClient _txtClient;
        readonly TextureCoordVboBuilder _vboBuilder = new TextureCoordVboBuilder();

        MySimpleGLBitmapFontManager _myGLBitmapFontMx;
        SimpleBitmapAtlas _fontAtlas;
        GLBitmap _glBmp;
        ResolvedFont _resolvedFont;

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

            //--------
            //load preview of pre-built texture font
            //temp fix, TODO: review this again


            string[] maybeTextureInfoFiles = GetCacheFontTextureFilenames();
            if (maybeTextureInfoFiles.Length > 0)
            {
                for (int i = 0; i < maybeTextureInfoFiles.Length; ++i)
                {
                    //try read
                    using (Stream s = PixelFarm.Platforms.StorageService.Provider.ReadDataStream(maybeTextureInfoFiles[i]))
                    {
                        try
                        {
                            //try load from specific stream
                            _myGLBitmapFontMx.LoadBitmapAtlasPreview(s);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }

            //LoadFontAtlas("tahoma_set1.multisize_fontAtlas", "tahoma_set1.multisize_fontAtlas.png");

            //test textures...

            //GlyphPosPixelSnapX = GlyphPosPixelSnapKind.Integer;
            //GlyphPosPixelSnapY = GlyphPosPixelSnapKind.Integer;
            //**
            //ChangeFont(painter.CurrentFont);
            //
            //TextDrawingTechnique = GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering; //default 
            TextDrawingTechnique = GlyphTexturePrinterDrawingTechnique.Stencil; //default
            UseVBO = true;

            TextBaseline = PixelFarm.Drawing.TextBaseline.Top;
            //TextBaseline = TextBaseline.Alphabetic;
            //TextBaseline = TextBaseline.Bottom; 


            //TODO: temp fix, 
            //...
            var myAlternativeTypefaceSelector = new AlternativeTypefaceSelector();
            {
                var preferTypefaces = new Typography.FontCollections.PreferredTypefaceList();
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
            {
                var preferTypefaces = new Typography.FontCollections.PreferredTypefaceList();
                preferTypefaces.AddTypefaceName("Twitter Color Emoji");
                myAlternativeTypefaceSelector.SetPerferredEmoji(preferTypefaces);
            }

            AlternativeTypefaceSelector = myAlternativeTypefaceSelector;
        }
        static string[] GetCacheFontTextureFilenames()
        {
            List<string> alldirs = new List<string>();
            foreach (string s in PixelFarm.Platforms.StorageService.Provider.GetDataNameList("."))
            {
                if (System.IO.Path.GetExtension(s) == ".tx_info")
                {
                    alldirs.Add(s);
                }
            }
            return alldirs.ToArray();
        }
        public AlternativeTypefaceSelector AlternativeTypefaceSelector
        {
            get => _txtClient.AlternativeTypefaceSelector;
            set => _txtClient.AlternativeTypefaceSelector = value;
        }

        void ChangeFont(SameFontTextStrip s)
        {
            UnicodeRangeInfo unicodeRng = s.BreakInfo.UnicodeRange;
            if (unicodeRng == null)
            {
                ChangeFont(s.ResolvedFont, 0, 255);
            }
            else
            {
                ChangeFont(s.ResolvedFont, unicodeRng.StartCodepoint, unicodeRng.EndCodepoint);
            }
        }
        void ChangeFont(FormattedGlyphPlanSeq s)
        {
            UnicodeRangeInfo unicodeRng = s.BreakInfo.UnicodeRange;
            if (unicodeRng == null)
            {
                ChangeFont(s.ResolvedFont, 0, 255);
            }
            else
            {
                ChangeFont(s.ResolvedFont, unicodeRng.StartCodepoint, unicodeRng.EndCodepoint);
            }
        }
        public void ChangeFont(ResolvedFont font, int startCodepoint, int endCodepoint)
        {
            if (_resolvedFont == font ||
               (_resolvedFont != null && font != null && _resolvedFont.RuntimeResolvedKey == font.RuntimeResolvedKey))
            {
                return;
            }

            _resolvedFont = font;
            _fontSizeInPoints = font.SizeInPoints;
            _px_scale = font.GetScaleToPixelFromPointUnit();
            _white_space_width = font.WhitespaceWidth;
            _ascending = font.AscentInPixels;
            _descending = font.DescentInPixels;
            _lineSpacingInPx = font.LineSpacingInPixels;

            //-------
            _fontAtlas = _myGLBitmapFontMx.GetFontAtlas(font, startCodepoint, endCodepoint, out _glBmp);
        }

        public void ChangeFont(RequestFont font)
        {
            //check if request font is diff from current _resolvedFont
            if (_resolvedFont != null && font != null && GlobalTextService.TxtClient.Eq(_resolvedFont, font))
            {
                return;
            }
            //
            ChangeFont(_txtClient.ResolveFont(font), 0, 0);
        }
        public bool UseVBO { get; set; }


        public GlyphTexturePrinterDrawingTechnique TextDrawingTechnique { get; set; }
        public void ChangeFillColor(Color color)
        {
            //called by owner painter  
            _painter.FontFillColor = color;
        }
        public void ChangeStrokeColor(Color strokeColor)
        {
            //TODO: implementation here
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

        internal bool IsInWordPlateCreatingMode { get; set; }
        public PixelFarm.Drawing.TextBaseline TextBaseline { get; set; }

#if DEBUG
        readonly PixelFarm.CpuBlit.ArrayList<float> _dbugVertexList = new PixelFarm.CpuBlit.ArrayList<float>();
        readonly PixelFarm.CpuBlit.ArrayList<ushort> _dbugIndexList = new PixelFarm.CpuBlit.ArrayList<ushort>();
        void dbugInnerDrawI18NStringNO_WordPlate(char[] buffer, int startAt, int len, double left, double top)
        {
            //input string may not be only Eng+ Num
            //it may contains characters from other unicode ranges (eg. Emoji)
            //to print it correctly we need to split it to multiple part
            //and choose a proper typeface for each part
            //-----------------

            var textBufferSpan = new Typography.Text.TextBufferSpan(buffer, startAt, len);

            //_textPrinterLineSegs.Clear();
            //_textPrinterWordVisitor.SetLineSegmentList(_textPrinterLineSegs);
            //_textServices.BreakToLineSegments(textBufferSpan, _textPrinterWordVisitor);
            //_textPrinterWordVisitor.SetLineSegmentList(null);//TODO: not need to set this,

            //check each split segment


            GlyphPlanSequence glyphPlanSeq = _txtClient.CreateGlyphPlanSeq(textBufferSpan, _resolvedFont);
            //-----------------

            _dbugVertexList.Clear();
            _dbugIndexList.Clear();
            _vboBuilder.SetArrayLists(_dbugVertexList, _dbugIndexList);
            _vboBuilder.SetTextureInfo(_glBmp.Width, _glBmp.Height, _glBmp.IsYFlipped, _pcx.OriginKind);

            //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
            //with specific request font

            float px_scale = _px_scale;
            //--------------------------
            //TODO:
            //if (x,y) is left top
            //we need to adjust y again      
            SimpleBitmapAtlas f_atlas = _fontAtlas;

            const float scaleFromTexture = 1;// _resolvedFont.SizeInPoints / f_atlas.OriginalFontSizePts;
            TextureKind textureKind = f_atlas.TextureKind;

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
                if (!f_atlas.TryGetItem(glyph.glyphIndex, out AtlasItem atlasItem))
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
                    case PixelFarm.Drawing.TextBaseline.Alphabetic:
                        //nothing todo
                        break;
                    case PixelFarm.Drawing.TextBaseline.Top:
                        g_top += _descending;
                        break;
                    case PixelFarm.Drawing.TextBaseline.Bottom:

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
            }
        }
#endif

        readonly GLRenderVxFormattedString _reusableFmtString = new GLRenderVxFormattedString();

        public void DrawString(char[] buffer, int startAt, int len, double left, double top)
        {
            //for internal use
            _reusableFmtString.ClearData();
            PrepareStringForRenderVx(_reusableFmtString, buffer, startAt, len);
            DrawString(_reusableFmtString, left, top);

            _reusableFmtString.ClearData();
        }

        public void DrawString(GLRenderVxFormattedString vxFmtStr, double x, double y)
        {
            _pcx.FontFillColor = _painter.FontFillColor;
            if (vxFmtStr.Delay && vxFmtStr.OwnerPlate == null)
            {
                //add this to queue to create                              
                return;
            }

            //TODO: review here
            //the following part is a drawing intensive part.



            float base_offset = 0;
            switch (TextBaseline)
            {
                case PixelFarm.Drawing.TextBaseline.Alphabetic:
                    //base_offset = -(vxFmtStr.SpanHeight + vxFmtStr.DescendingInPx);
                    break;
                case PixelFarm.Drawing.TextBaseline.Top:

                    base_offset = vxFmtStr.DescendingInPx;                    
                    //base_offset += 10; 
                    //base_offset -= vxFmtStr.DescendingInPx;
                    break;
                case PixelFarm.Drawing.TextBaseline.Bottom:
                    base_offset = -vxFmtStr.SpanHeight;
                    break;
            }

            if (IsInWordPlateCreatingMode)
            {
                //on word plate mode
                //in this version
                //we prepare only stencil glyph

                switch (vxFmtStr.GlyphMixMode)
                {
                    case GLRenderVxFormattedStringGlyphMixMode.OnlyColorGlyphs:
                        {
                            //all glyph are color

                            int j = vxFmtStr.StripCount;
                            float start_x = (float)Math.Round(x);
                            float start_y = (float)Math.Floor(y + base_offset);

                            for (int n = 0; n < j; ++n)
                            {
                                SameFontTextStrip s = vxFmtStr[n];

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


                            int j = vxFmtStr.StripCount;
                            float start_x = (float)Math.Round(x);
                            float start_y = (float)Math.Floor(y + base_offset);


                            switch (TextDrawingTechnique)
                            {
                                case GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering:
                                    {
                                        for (int n = 0; n < j; ++n)
                                        {
                                            SameFontTextStrip s = vxFmtStr[n];
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
                                            SameFontTextStrip s = vxFmtStr[n];

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


                    int j = vxFmtStr.StripCount;
                    float start_x = (float)Math.Round(x);
                    float start_y = (float)Math.Floor(y + base_offset);

                    for (int n = 0; n < j; ++n)
                    {
                        SameFontTextStrip s = vxFmtStr[n];
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

                                    int j = vxFmtStr.StripCount;
                                    float start_x = (float)Math.Round(x);
                                    float start_y = (float)Math.Floor(y + base_offset);

                                    for (int n = 0; n < j; ++n)
                                    {
                                        SameFontTextStrip s = vxFmtStr[n];

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

                                    int j = vxFmtStr.StripCount;
                                    float start_x = (float)Math.Round(x);
                                    float start_y = (float)Math.Floor(y + base_offset);
                                    float spanHeight = 0;
                                    float spanWidth = 0;

                                    for (int n = 0; n < j; ++n)
                                    {
                                        SameFontTextStrip s = vxFmtStr[n];
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

                                int j = vxFmtStr.StripCount;
                                float start_x = (float)Math.Round(x);
                                float start_y = (float)Math.Floor(y + base_offset);

                                for (int n = 0; n < j; ++n)
                                {
                                    SameFontTextStrip s = vxFmtStr[n];

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

                    int j = vxFmtStr.StripCount;
                    float start_x = (float)Math.Round(x);
                    float start_y = (float)Math.Floor(y + base_offset);

                    for (int n = 0; n < j; ++n)
                    {
                        SameFontTextStrip s = vxFmtStr[n];
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


        void CreateTextCoords(SameFontTextStrip txtStrip)
        {
            int top = 0;//simulate top
            int left = 0;//simulate left

            //change font once 
            ResolvedFont expected_resolvedFont = txtStrip.ResolvedFont;
            Typeface expectedTypeface = expected_resolvedFont.Typeface;
            SpanBreakInfo expectedBreakInfo = txtStrip.BreakInfo;

#if DEBUG
            if (expectedTypeface == null) { throw new NotSupportedException(); }
#endif

            ChangeFont(txtStrip);

            float px_scale = expected_resolvedFont.GetScaleToPixelFromPointUnit();

            //TODO: review here, rounding error*** 
            txtStrip.SpanHeight = expected_resolvedFont.LineSpacingInPixels;// (int)Math.Round(expectedTypeface.CalculateMaxLineClipHeight() * px_scale);// expectedFont.LineSpacingInPixels;

            //TODO: review here, Rounding error
            int descend_px = (int)expected_resolvedFont.DescentInPixels;// (int)(expectedTypeface.Descender * px_scale);
            int ascentd_px = (int)expected_resolvedFont.AscentInPixels;// (int)(expectedTypeface.Ascender * px_scale);
            txtStrip.DescendingInPx = descend_px;// (short)(expectedTypeface.Descender * px_scale);  //expectedFont.DescentInPixels;
            txtStrip.SpanDescendingInPx = (int)expected_resolvedFont.UsDescendingInPixels;

            int count = _fmtGlyphPlans.Count;
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

            _vboBuilder.SetArrayLists(_sh_vertexList, _sh_indexList); //set output***
            _vboBuilder.SetTextureInfo(_glBmp.Width, _glBmp.Height, _glBmp.IsYFlipped, _pcx.OriginKind);

            if (_fmtGlyphPlans.Count > 0)
            {

                FormattedGlyphPlanSeq sq = _fmtGlyphPlans.GetFirst();
                while (sq != null)
                {


                    bool isTargetFont = sq.ResolvedFont.Typeface == expectedTypeface &&
                                         sq.BreakInfo == expectedBreakInfo;

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

                    //------------
                    //
                    sq = sq.Next;
                }

            }



            if (hasSomeGlyphs)
            {
                _vboBuilder.AppendDegenerativeTriangle();
            }

            txtStrip.Width = acc_x;
        }


        readonly FormattedGlyphPlanSeqPool _fmtGlyphPlans = new FormattedGlyphPlanSeqPool();
        readonly Dictionary<SpanFormattedInfo, bool> _uniqueResolvedFonts = new Dictionary<SpanFormattedInfo, bool>();

        readonly struct SpanFormattedInfo
        {
            //just intermediate structure
            public readonly ResolvedFont resolvedFont;
            public readonly SpanBreakInfo breakInfo;
            public SpanFormattedInfo(ResolvedFont resolvedFont, SpanBreakInfo breakInfo)
            {
                this.resolvedFont = resolvedFont;
                this.breakInfo = breakInfo;
            }
        }

        PixelFarm.CpuBlit.ArrayList<float> _sh_vertexList;
        PixelFarm.CpuBlit.ArrayList<ushort> _sh_indexList;
        bool _useSingleSequentialIndexList;
        PixelFarm.CpuBlit.ArrayList<ushort> _sh_globalIndexList = new PixelFarm.CpuBlit.ArrayList<ushort>();

        void PrepareStringForRenderVx(GLRenderVxFormattedString vxFmtStr, FormattedGlyphPlanSeq fmt_seq)
        {

            _uniqueResolvedFonts.Clear();

            while (fmt_seq != null)
            {
                //find unique atlas
                SpanFormattedInfo spFmt = new SpanFormattedInfo(fmt_seq.ResolvedFont, fmt_seq.BreakInfo);
                if (!_uniqueResolvedFonts.ContainsKey(spFmt))
                {
                    _uniqueResolvedFonts.Add(spFmt, true);
                }
                //
                fmt_seq = fmt_seq.Next;
            }



            //a fmtGlyphPlanSeqs may contains glyph from  more than 1 font,
            //now, create a overlapped strip for each 
#if DEBUG
            vxFmtStr.dbugPreparing = true;
            if (vxFmtStr.StripCount > 0)
            {
                throw new NotSupportedException();
            }
#endif
            float spanHeight = 0;
            float spanWidth = 0;
            int descendingInPx = 0;
            int spanDescendingInPx = 0;
            int maxStripHeight = 0;

            GlyphMixModeSummary mixModeSummary = new GlyphMixModeSummary();//light-weight state helper

            _useSingleSequentialIndexList = (_uniqueResolvedFonts.Count < 2);

            vxFmtStr.PrepareIntermediateStructures(_useSingleSequentialIndexList);
            //get blank indexList and vertexList from a fmtString,
            //this text printer will create a data set for it.
            _sh_vertexList = vxFmtStr._sh_vertexList;//temp, 
            if (_useSingleSequentialIndexList)
            {
                _sh_indexList = _sh_globalIndexList;
            }
            else
            {
                _sh_indexList = vxFmtStr._sh_indexList; //temp,             
            }

            foreach (SpanFormattedInfo spFmt in _uniqueResolvedFonts.Keys)
            {
                //once for each typeface***
                //SpanFormattedInfo spFmt = kv.Key;

#if DEBUG
                if (spFmt.resolvedFont == null) { throw new NotSupportedException(); }
#endif

                Typeface typeface = spFmt.resolvedFont.Typeface;

                //TODO: review here again, use pool
                SameFontTextStrip sameFontTextStrip = vxFmtStr.AppendNewStrip();
                sameFontTextStrip.UseSeqIndexList = _useSingleSequentialIndexList;
                sameFontTextStrip.ResolvedFont = spFmt.resolvedFont;
                sameFontTextStrip.BreakInfo = spFmt.breakInfo;

                //assign ColorGlyphOnTransparentBG and update mix mode state
                mixModeSummary.AddGlyphMixMode(
                    sameFontTextStrip.ColorGlyphOnTransparentBG =
                            (typeface.HasSvgTable() || typeface.IsBitmapFont || typeface.HasColorTable()));

                //**                  

                int sh_vertex_begin = _sh_vertexList.Count;

                int sh_index_begin = _useSingleSequentialIndexList ? 0 : _sh_indexList.Count;

                CreateTextCoords(sameFontTextStrip);
                //**
                //use max size of height and descending ?
                descendingInPx = sameFontTextStrip.DescendingInPx;
                spanDescendingInPx = sameFontTextStrip.SpanDescendingInPx;

                maxStripHeight = Math.Max(maxStripHeight, sameFontTextStrip.SpanHeight);

                spanWidth = sameFontTextStrip.Width;//same width for all strip                 

                if (_useSingleSequentialIndexList)
                {
                    sameFontTextStrip.SetIndexCount(_sh_globalIndexList.Length);
                    _sh_globalIndexList.Clear();
                }
                else
                {
                    sameFontTextStrip.IndexArray = _sh_indexList.CreateSpan(sh_index_begin, _sh_indexList.Length - sh_index_begin);
                }
                sameFontTextStrip.VertexCoords = _sh_vertexList.CreateSpan(sh_vertex_begin, _sh_vertexList.Length - sh_vertex_begin);
            }

            //adjust addition vertical height

            spanHeight = maxStripHeight;

            vxFmtStr.ApplyAdditionalVerticalOffset(maxStripHeight);
            vxFmtStr.GlyphMixMode = mixModeSummary.FinalMixMode;
            vxFmtStr.SpanHeight = spanHeight;
            vxFmtStr.Width = spanWidth;
            vxFmtStr.DescendingInPx = (short)descendingInPx;
            vxFmtStr.SpanDescendingInPx = (short)spanDescendingInPx;
            vxFmtStr.CreationState = GLRenderVxFormattedStringState.S1_VertexList;
            //-----------
            //TODO: review here again

            if (!vxFmtStr.Delay)
            {
                //TODO: review here again   
                _painter.TryCreateWordStrip(vxFmtStr);
            }

            //contains chain 
            _uniqueResolvedFonts.Clear();

            _sh_indexList = null; //reset
            _sh_vertexList = null;//reset
#if DEBUG
            vxFmtStr.dbugPreparing = false;

#endif
        }

        public void PrepareStringForRenderVx(GLRenderVxFormattedString vxFmtStr, IFormattedGlyphPlanList fmtGlyphPlans)
        {
            if (!(fmtGlyphPlans is FormattedGlyphPlanListHolder fmtHolder)) { return; }

            PrepareStringForRenderVx(vxFmtStr, fmtHolder._chainFmtGlyphPlans);
        }
        public void PrepareStringForRenderVx(GLRenderVxFormattedString vxFmtStr, char[] buffer, int startAt, int len)
        {
            //we need to parse string 
            //since it may contains glyph from multiple font (eg. eng, emoji etc.)
            //see VxsTextPrinter   

            //resolved font has information about typeface, size  
            _txtClient.SetCurrentFont(
                _txtClient.ResolveFont(_painter.CurrentFont).Typeface,
                _fontSizeInPoints,
                _txtClient.CurrentScriptLang);

            if (vxFmtStr.IsReset)
            {
                _painter.TryCreateWordStrip(vxFmtStr);
            }
            else
            {
                _fmtGlyphPlans.Clear(); //reuse 
                _txtClient.PrepareFormattedStringList(buffer, startAt, len, _fmtGlyphPlans);
                if (_fmtGlyphPlans.Count > 0)
                {
                    PrepareStringForRenderVx(vxFmtStr, _fmtGlyphPlans.GetFirst());
                }
            }
        }
        public void PrepareStringForRenderVx(GLRenderVxFormattedString vxFmtStr, int[] buffer, int startAt, int len)
        {
            //we need to parse string 
            //since it may contains glyph from multiple font (eg. eng, emoji etc.)
            //see VxsTextPrinter   

            //resolved font has information about typeface, size  
            _txtClient.SetCurrentFont(
                _txtClient.ResolveFont(_painter.CurrentFont).Typeface,
                _fontSizeInPoints,
                _txtClient.CurrentScriptLang);

            _fmtGlyphPlans.Clear(); //reuse 
            _txtClient.PrepareFormattedStringList(buffer, startAt, len, _fmtGlyphPlans);

            if (_fmtGlyphPlans.Count > 0)
            {
                PrepareStringForRenderVx(vxFmtStr, _fmtGlyphPlans.GetFirst());
            }

        }
        /// <summary>
        /// helper struct for summarize glyph mixed mode
        /// </summary>
        ref struct GlyphMixModeSummary
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


