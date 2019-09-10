//MIT, 2016-present, WinterDev
using System;
using System.Collections.Generic;
//
using PixelFarm.CpuBlit;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
//
using Typography.TextLayout;
using Typography.OpenFont;
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

    public class GLBitmapGlyphTextPrinter : ITextPrinter, IDisposable
    {

        MySimpleGLBitmapFontManager _myGLBitmapFontMx;
        SimpleFontAtlas _fontAtlas;
        GLPainterContext _pcx;
        GLPainter _painter;
        GLBitmap _glBmp;
        RequestFont _font;
        LayoutFarm.OpenFontTextService _textServices;
        float _px_scale = 1;
        TextureCoordVboBuilder _vboBuilder = new TextureCoordVboBuilder();

        WordPlate _wordPlate; //current word plate
        WordPlateMx _wordPlateMx; //word plate mx

#if DEBUG
        public static GlyphTexturePrinterDrawingTechnique s_dbugDrawTechnique = GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering;
        public static bool s_dbugUseVBO = true;
        public static bool s_dbugShowGlyphTexture = false;
        public static bool s_dbugShowMarkers = false;
#endif
        /// <summary>
        /// use vertex buffer object
        /// </summary>

        public GLBitmapGlyphTextPrinter(GLPainter painter, LayoutFarm.OpenFontTextService textServices)
        {
            //create text printer for use with canvas painter           
            _painter = painter;
            _pcx = painter.PainterContext;
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
            DrawingTechnique = GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering; //default 
            UseVBO = true;
            //---------

            ////built in bitmap atlas painter
            //_bmpAtlasPainter = new GLBitmapAtlasPainter();
            //_backBuffer = new Drawing.GLES2.MyGLBackbuffer(800, 600);
            _wordPlateMx = new WordPlateMx();
            _wordPlate = _wordPlateMx.GetNewWordPlate();
#if DEBUG
            if (_wordPlate == null)
            {

            }
#endif
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
                        FontAtlasFile fontAtlas = new FontAtlasFile();
                        fontAtlas.Read(fontTextureInfoStream);
                        SimpleFontAtlas[] resultAtlases = fontAtlas.ResultSimpleFontAtlasList.ToArray();
                        _myGLBitmapFontMx.AddSimpleFontAtlas(resultAtlases, fontTextureImgStream);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }


            //if (System.IO.File.Exists(fontTextureInfoFile))
            //{
            //    using (System.IO.Stream dataStream = new System.IO.FileStream(fontTextureInfoFile, System.IO.FileMode.Open))
            //    {
            //        try
            //        {
            //            FontAtlasFile fontAtlas = new FontAtlasFile();
            //            fontAtlas.Read(dataStream);
            //            SimpleFontAtlas[] resultAtlases = fontAtlas.ResultSimpleFontAtlasList.ToArray();
            //            _myGLBitmapFontMx.AddSimpleFontAtlas(resultAtlases, atlasImgFilename);
            //        }
            //        catch (Exception ex)
            //        {
            //            throw ex;
            //        }
            //    }
            //}
            //else
            //{

            //}

        }
        public bool UseVBO { get; set; }
        public GlyphTexturePrinterDrawingTechnique DrawingTechnique { get; set; }
        public void ChangeFillColor(Color color)
        {
            //called by owner painter  
            _painter.FontFillColor = color;
        }
        public void ChangeStrokeColor(Color strokeColor)
        {
            //TODO: implementation here
        }
        public bool StartDrawOnLeftTop { get; set; }


        public void ChangeFont(RequestFont font)
        {
            if (_font == font || (_font != null && _font.FontKey == font.FontKey))
            {
                //not change -> then return
                return;
            }

            //_loadedFont = _loadFonts.RegisterFont(font);
            //System.Diagnostics.Debug.WriteLine(font.Name + font.SizeInPoints);

            //LoadedFont loadFont = _loadFonts.RegisterFont(font);
            //font has been changed, 
            //resolve for the new one 
            //check if we have this texture-font atlas in our MySimpleGLBitmapFontManager 
            //if not-> request the MySimpleGLBitmapFontManager to create a newone 
            _fontAtlas = _myGLBitmapFontMx.GetFontAtlas(font, out _glBmp);
            _font = font;
            Typeface typeface = _textServices.ResolveTypeface(font);
            _px_scale = typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
        }
        public void Dispose()
        {
            if (_wordPlateMx != null)
            {
                _wordPlateMx.ClearAllPlates();
                _wordPlateMx = null;
            }
            _wordPlate = null;

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
            TextBufferSpan textBufferSpan = new TextBufferSpan(buffer, startAt, len);
            Size s = _textServices.MeasureString(ref textBufferSpan, _painter.CurrentFont);
            w = s.Width;
            h = s.Height;
        }

        public void DrawString(char[] buffer, int startAt, int len, double left, double top)
        {
            _vboBuilder.Clear();
            _vboBuilder.SetTextureInfo(_glBmp.Width, _glBmp.Height, _glBmp.IsYFlipped, _pcx.OriginKind);
            //create temp buffer span that describe the part of a whole char buffer
            TextBufferSpan textBufferSpan = new TextBufferSpan(buffer, startAt, len);
            //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
            //with specific request font
            GlyphPlanSequence glyphPlanSeq = _textServices.CreateGlyphPlanSeq(ref textBufferSpan, _font);
            float px_scale = _px_scale;
            //--------------------------
            //TODO:
            //if (x,y) is left top
            //we need to adjust y again      

            float scaleFromTexture = _font.SizeInPoints / _fontAtlas.OriginalFontSizePts;

            PixelFarm.Drawing.BitmapAtlas.TextureKind textureKind = _fontAtlas.TextureKind;

            float g_left = 0;
            float g_top = 0;
            int baseLine = (int)Math.Round((float)top + _font.AscentInPixels);
            int bottom = (int)Math.Round((float)top + _font.AscentInPixels - _font.DescentInPixels);

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
                _painter.DrawLine(left, baseLine, left + 200, baseLine);
                //
                //draw magenta-line-marker for bottom line
                _painter.StrokeColor = Color.Magenta;
                int bottomLine = (int)Math.Round((float)top + _font.LineSpacingInPixels);
                _painter.DrawLine(left, bottomLine, left + 200, bottomLine);
                //draw blue-line-marker for top line
                _painter.StrokeColor = Color.Blue;
                _painter.DrawLine(0, top, left + 200, top);
            }

            //DrawingTechnique = s_dbugDrawTechnique;//for debug only
            //UseVBO = s_dbugUseVBO;//for debug only 
#endif

            if (textureKind == PixelFarm.Drawing.BitmapAtlas.TextureKind.Msdf)
            {
                DrawingTechnique = GlyphTexturePrinterDrawingTechnique.Msdf;
            }


            //----------
            int seqLen = glyphPlanSeq.Count;
            for (int i = 0; i < seqLen; ++i)
            {
                UnscaledGlyphPlan glyph = glyphPlanSeq[i];
                Typography.Rendering.TextureGlyphMapData glyphData;
                if (!_fontAtlas.TryGetGlyphMapData(glyph.glyphIndex, out glyphData))
                {
                    //if no glyph data, we should render a missing glyph ***
                    continue;
                }
                //--------------------------------------
                //TODO: review precise height in float
                //--------------------------------------  

                //paint src rect
                //temp fix, glyph texture img is not flipped
                //but the associate info is flipped => so
                //we need remap exact Y from the image 

                Rectangle srcRect =
                      new Rectangle(glyphData.Left,
                         _glBmp.Height - (glyphData.Top + glyphData.Height),
                          glyphData.Width,
                          glyphData.Height);

                //offset length from 'base-line'
                float x_offset = acc_x + (float)Math.Round(glyph.OffsetX * px_scale - glyphData.TextureXOffset * scaleFromTexture);
                float y_offset = acc_y + (float)Math.Round(glyph.OffsetY * px_scale - glyphData.TextureYOffset * scaleFromTexture) + srcRect.Height; //***

                //NOTE:
                // -glyphData.TextureXOffset => restore to original pos
                // -glyphData.TextureYOffset => restore to original pos 
                //--------------------------              

                g_left = (float)(left + x_offset);
                g_top = (float)(bottom - y_offset); //***

                acc_x += (float)Math.Round(glyph.AdvanceX * px_scale);
                g_top = (float)Math.Floor(g_top);//adjust to integer num *** 

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
                    _vboBuilder.WriteVboToList(
                           ref srcRect,
                           g_left, g_top, scaleFromTexture);
                }
                else
                {
                    switch (DrawingTechnique)
                    {
                        case GlyphTexturePrinterDrawingTechnique.Msdf:
                            _pcx.DrawSubImageWithMsdf(_glBmp,
                                 ref srcRect,
                                 g_left,
                                 g_top,
                                 scaleFromTexture);
                            break;
                        case GlyphTexturePrinterDrawingTechnique.Stencil:
                            //stencil gray scale with fill-color
                            _pcx.DrawGlyphImageWithStecil(_glBmp,
                                ref srcRect,
                                g_left,
                                g_top,
                                scaleFromTexture);
                            break;
                        case GlyphTexturePrinterDrawingTechnique.Copy:
                            _pcx.DrawSubImage(_glBmp,
                                ref srcRect,
                                g_left,
                                g_top,
                                1);
                            break;
                        case GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering:
                            _pcx.DrawGlyphImageWithSubPixelRenderingTechnique2_GlyphByGlyph(
                                _glBmp,
                                ref srcRect,
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
                switch (DrawingTechnique)
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
        public void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            DrawString(_glBmp, (GLRenderVxFormattedString)renderVx, x, y);
        }
#if DEBUG
        static int _dbugCount;
#endif
        public void DrawString(GLBitmap glBmp, GLRenderVxFormattedString renderVx, double x, double y)
        {
            _pcx.FontFillColor = _painter.FontFillColor;

            switch (DrawingTechnique)
            {
                case GlyphTexturePrinterDrawingTechnique.Stencil:
                    {

                        if (renderVx.WordPlateId > 0)
                        {
                            //-----------------------
                            //TODO: use WordPlate or Not
                            if (renderVx.WordPlateId != _wordPlate._plateId)
                            {
                                //not the same plate, change
                                _wordPlate = _wordPlateMx.GetWordPlate(renderVx.WordPlateId);
#if DEBUG
                                if (_wordPlate == null)
                                {
                                    throw new NotSupportedException();
                                }
#endif
                            }
                            //-----------------------


#if DEBUG
                            //random for debug                            
                            //_painter.FillRect(
                            //    (float)Math.Round(x), (float)Math.Floor(y),
                            //       renderVx.Width, renderVx.SpanHeight,
                            //       ColorEx.dbugGetRandomColor());
#endif

                            _pcx.DrawWordSpanWithStencilTechnique((GLBitmap)_wordPlate._backBuffer.GetImage(),
                                renderVx.WordPlateLeft, -renderVx.WordPlateTop - renderVx.SpanHeight,
                                renderVx.Width, renderVx.SpanHeight,
                                (float)Math.Round(x),
                                (float)Math.Floor(y));
                        }
                        else
                        {
#if DEBUG
                            //random for debug                            
                            //_painter.FillRect(
                            //    (float)Math.Round(x), (float)Math.Floor(y),
                            //       renderVx.Width, renderVx.SpanHeight,
                            //       ColorEx.dbugGetRandomColor());
#endif


                            if (renderVx.UseWithWordPlate)
                            {
                                //this renderVx has WordPlateId == 0,
                                //but it has been assigned to a disposed wordplate.

                                //if we want to use with a live word plate 
                                //then ask the painter first 
                                CreateWordPlateTicketId(renderVx);

#if DEBUG
                                //_pcx.FillRect(ColorEx.dbugGetRandomColor(),
                                //     renderVx.WordPlateLeft, -renderVx.WordPlateTop - renderVx.SpanHeight,
                                //     renderVx.Width, renderVx.SpanHeight);
#endif

                                //success or not
                                if (renderVx.WordPlateId > 0)
                                {
                                    _pcx.DrawWordSpanWithStencilTechnique((GLBitmap)_wordPlate._backBuffer.GetImage(),
                                        renderVx.WordPlateLeft, -renderVx.WordPlateTop - renderVx.SpanHeight,
                                        renderVx.Width, renderVx.SpanHeight,
                                        (float)Math.Round(x),
                                        (float)Math.Floor(y));

                                }
                                else
                                {
                                    //can't create at this time
                                    //render with vbo
                                    _pcx.DrawGlyphImageWithStencilRenderingTechnique4_FromVBO(
                                         glBmp,
                                         renderVx.GetVbo(),
                                         renderVx.IndexArrayCount,
                                         (float)Math.Round(x),
                                         (float)Math.Floor(y));
                                }
                            }
                            else
                            {

                                _pcx.DrawGlyphImageWithStencilRenderingTechnique4_FromVBO(
                                    glBmp,
                                    renderVx.GetVbo(),
                                    renderVx.IndexArrayCount,
                                    (float)Math.Round(x),
                                    (float)Math.Floor(y));
                            }
                        }
                    }
                    break;
                case GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering:
                    {
                        if (renderVx.WordPlateId > 0)
                        {
                            //-----------------------
                            //TODO: use WordPlate or Not
                            if (renderVx.WordPlateId != _wordPlate._plateId)
                            {
                                //not the same plate, change
                                _wordPlate = _wordPlateMx.GetWordPlate(renderVx.WordPlateId);
#if DEBUG
                                if (_wordPlate == null)
                                {
                                    throw new NotSupportedException();
                                }
#endif
                            }
                            //-----------------------


#if DEBUG
                            //random for debug                            
                            //_painter.FillRect(
                            //    (float)Math.Round(x), (float)Math.Floor(y),
                            //       renderVx.Width, renderVx.SpanHeight,
                            //       ColorEx.dbugGetRandomColor());
#endif

                            //TODO: use word plate and draw with Lcd-effect subpixel rendering
                        

                            _pcx.DrawWordSpanWithStencilTechnique((GLBitmap)_wordPlate._backBuffer.GetImage(),
                                renderVx.WordPlateLeft, -renderVx.WordPlateTop - renderVx.SpanHeight,
                                renderVx.Width, renderVx.SpanHeight,
                                (float)Math.Round(x),
                                (float)Math.Floor(y));
                        }
                        else
                        {
                            if (renderVx.UseWithWordPlate)
                            {
                                //this renderVx has WordPlateId == 0,
                                //but it has been assigned to a disposed wordplate.

                                //if we want to use with a live word plate 
                                //then ask the painter first 
                                CreateWordPlateTicketId(renderVx);

#if DEBUG
                                //_pcx.FillRect(ColorEx.dbugGetRandomColor(),
                                //     renderVx.WordPlateLeft, -renderVx.WordPlateTop - renderVx.SpanHeight,
                                //     renderVx.Width, renderVx.SpanHeight);
#endif

                                //success or not
                                if (renderVx.WordPlateId > 0)
                                {
#if DEBUG
                                    //System.Diagnostics.Debug.WriteLine(
                                    //    "word_plate_id:" + renderVx.WordPlateId);
#endif
                                    _pcx.DrawWordSpanWithStencilTechnique((GLBitmap)_wordPlate._backBuffer.GetImage(),
                                        renderVx.WordPlateLeft, -renderVx.WordPlateTop - renderVx.SpanHeight,
                                        renderVx.Width, renderVx.SpanHeight,
                                        (float)Math.Round(x),
                                        (float)Math.Floor(y));

                                }
                                else
                                {
                                    //can't create at this time 
                                    //LCD-Effect****
                                    _pcx.DrawGlyphImageWithSubPixelRenderingTechnique4_FromVBO(
                                      glBmp,
                                      renderVx.GetVbo(),
                                      renderVx.IndexArrayCount,
                                      (float)Math.Round(x),
                                      (float)Math.Floor(y));
                                }
                            }
                            else
                            {

                                //LCD-Effect****
                                _pcx.DrawGlyphImageWithSubPixelRenderingTechnique4_FromVBO(
                                  glBmp,
                                  renderVx.GetVbo(),
                                  renderVx.IndexArrayCount,
                                  (float)Math.Round(x),
                                  (float)Math.Floor(y));
                            }
                        }

                    }
                    break;
            }
        }

        internal static PixelFarm.Drawing.GLES2.MyGLDrawBoard s_currentDrawBoard;

        void PrepareStringForRenderVx(GLRenderVxFormattedString renderVxFormattedString, char[] buffer, int startAt, int len)
        {
            int top = 0;//simulate top
            int left = 0;//simulate left

            _vboBuilder.Clear();
            _vboBuilder.SetTextureInfo(_glBmp.Width, _glBmp.Height, _glBmp.IsYFlipped, _pcx.OriginKind);

            //create temp buffer span that describe the part of a whole char buffer
            TextBufferSpan textBufferSpan = new TextBufferSpan(buffer, startAt, len);

            //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
            //with specific request font
            GlyphPlanSequence glyphPlanSeq = _textServices.CreateGlyphPlanSeq(ref textBufferSpan, _font);
            float px_scale = _px_scale;
            float scaleFromTexture = 1; //TODO: support msdf auto scale

            //-------------------------- 

            Drawing.BitmapAtlas.TextureKind textureKind = _fontAtlas.TextureKind;
            float g_left = 0;
            float g_top = 0;

            //int baseLine = (int)Math.Round((float)top + _font.AscentInPixels);
            int bottom = (int)Math.Round((float)top + _font.AscentInPixels - _font.DescentInPixels);
            float acc_x = 0; //local accumulate x
            float acc_y = 0; //local accumulate y  

            int seqLen = glyphPlanSeq.Count;

            for (int i = 0; i < seqLen; ++i)
            {
                UnscaledGlyphPlan glyph = glyphPlanSeq[i];
                Typography.Rendering.TextureGlyphMapData glyphData;
                if (!_fontAtlas.TryGetGlyphMapData(glyph.glyphIndex, out glyphData))
                {
                    //if no glyph data, we should render a missing glyph ***
                    continue;
                }
                //--------------------------------------
                //TODO: review precise height in float
                //--------------------------------------  
                //paint src rect
                //temp fix, glyph texture img is not flipped
                //but the associate info is flipped => so
                //we need remap exact Y from the image  
                Rectangle srcRect =
                      new Rectangle(glyphData.Left,
                         _glBmp.Height - (glyphData.Top + glyphData.Height),
                          glyphData.Width,
                          glyphData.Height);

                //offset length from 'base-line'
                float x_offset = acc_x + (float)Math.Round(glyph.OffsetX * px_scale - glyphData.TextureXOffset);
                float y_offset = acc_y + (float)Math.Round(glyph.OffsetY * px_scale - glyphData.TextureYOffset) + srcRect.Height; //***

                //NOTE:
                // -glyphData.TextureXOffset => restore to original pos
                // -glyphData.TextureYOffset => restore to original pos 
                //--------------------------              

                g_left = (float)(left + x_offset);
                g_top = (float)(bottom - y_offset); //***

                acc_x += (float)Math.Round(glyph.AdvanceX * px_scale);
                //g_x = (float)Math.Round(g_x); //***
                g_top = (float)Math.Floor(g_top);//adjust to integer num *** 
                //
                _vboBuilder.WriteVboToList(ref srcRect, g_left, g_top, scaleFromTexture);

            }
            //---
            //copy vbo result and store into  renderVx  
            //TODO: review here
            renderVxFormattedString.IndexArrayCount = _vboBuilder._indexList.Count;
            renderVxFormattedString.IndexArray = _vboBuilder._indexList.ToArray();
            renderVxFormattedString.VertexCoords = _vboBuilder._buffer.ToArray();
            renderVxFormattedString.Width = acc_x;
            renderVxFormattedString.SpanHeight = _font.LineSpacingInPixels;

            _vboBuilder.Clear();
        }
        public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] buffer, int startAt, int len)
        {
            var renderVxFormattedString = renderVx as GLRenderVxFormattedString;

#if DEBUG
            if (renderVxFormattedString == null)
            {
                throw new NotSupportedException();
            }
#endif
            //use current font settings
            PrepareStringForRenderVx(renderVxFormattedString, buffer, startAt, len);
            CreateWordPlateTicketId(renderVxFormattedString);
            //{
            //    save output
            //    using (Image img = _backBuffer.CopyToNewMemBitmap())
            //    {
            //        MemBitmap memBmp = img as MemBitmap;
            //        if (memBmp != null)
            //        {
            //            memBmp.SaveImage("d:\\WImageTest\\testx_01.png");
            //        }
            //    }
            //} 
        }

        void CreateWordPlateTicketId(GLRenderVxFormattedString renderVxFormattedString)
        {
            if (s_currentDrawBoard != null && !_wordPlate.Full)
            {
                if (!_wordPlate.HasAvailableSpace(renderVxFormattedString))
                {
                    //create new word-plate
                    _wordPlate = _wordPlateMx.GetNewWordPlate();
                }

                s_currentDrawBoard.EnterNewDrawboardBuffer(_wordPlate._backBuffer);

                GLPainter pp = s_currentDrawBoard.GetGLPainter();
                if (!_wordPlate.CreatePlateTicket(pp, renderVxFormattedString))
                {
                    //we have some error?
                    throw new NotSupportedException();
                }
                s_currentDrawBoard.ExitCurrentDrawboardBuffer();
            }
        }
        //--------------------------------------------------------------------




        class WordPlateMx
        {
            Dictionary<ushort, WordPlate> _wordPlates = new Dictionary<ushort, WordPlate>();
            int _defaultPlateW = 800;
            int _defaultPlateH = 600;

            static ushort s_totalPlateId = 0;

            public WordPlateMx()
            {
                MaxPlateCount = 2;
                AutoRemoveOldestPlate = true;
            }

            public bool AutoRemoveOldestPlate { get; set; }
            public int MaxPlateCount { get; set; }
            public void SetDefaultPlateSize(int w, int h)
            {
                _defaultPlateH = h;
                _defaultPlateW = w;
            }
            public void ClearAllPlates()
            {
                foreach (WordPlate wordPlate in _wordPlates.Values)
                {
                    wordPlate.Dispose();
                }
                _wordPlates.Clear();
            }
            public void RemoveWordPlate(ushort plateId)
            {
                if (_wordPlates.TryGetValue(plateId, out WordPlate found))
                {
                    //clear content in that word-plate
                    found.Dispose();
                    _wordPlates.Remove(plateId);
                }
            }
            public WordPlate GetWordPlate(ushort plateId)
            {
                _wordPlates.TryGetValue(plateId, out WordPlate found);
                return found;
            }

            public WordPlate GetNewWordPlate()
            {
                //create new and register 
                if (_wordPlates.Count == MaxPlateCount)
                {
                    if (AutoRemoveOldestPlate)
                    {
                        WordPlate firstPlate = null;
                        foreach (WordPlate p in _wordPlates.Values)
                        {
                            //remove only 1 plate
                            firstPlate = p;
                            break;
                        }

                        if (firstPlate != null)
                        {
                            //remove 
                            _wordPlates.Remove(firstPlate._plateId);
                            //and dispose
                            firstPlate.Dispose();
                            firstPlate = null;
                        }
                        else
                        {
                            return null;
                        }
                    }

                }

                if (s_totalPlateId + 1 >= ushort.MaxValue)
                {
                    throw new NotSupportedException();
                }

                s_totalPlateId++;  //so plate_id starts at 1

                WordPlate wordPlate = new WordPlate(s_totalPlateId, _defaultPlateW, _defaultPlateH);
                _wordPlates.Add(s_totalPlateId, wordPlate);
                return wordPlate;
            }

        }

        class WordPlate : IDisposable
        {
            bool _isInitBg;
            int _currentX;
            int _currentY;
            int _currentLineHeightMax;
            readonly int _plateWidth;
            readonly int _plateHeight;
            bool _full;

            internal readonly ushort _plateId;
            internal List<GLRenderVxFormattedString> _tickets = new List<GLRenderVxFormattedString>();
            internal Drawing.GLES2.MyGLBackbuffer _backBuffer;

            public WordPlate(ushort plateId, int w, int h)
            {
                _plateId = plateId;
                _plateWidth = w;
                _plateHeight = h;
                _backBuffer = new Drawing.GLES2.MyGLBackbuffer(w, h);
            }

            public void Dispose()
            {
                //clear all
                if (_backBuffer != null)
                {
                    _backBuffer.Dispose();
                    _backBuffer = null;
                }
                int j = _tickets.Count;
                for (int i = 0; i < j; ++i)
                {
                    //essential!
                    _tickets[i].ClearWordPlateId();
                }
                _tickets.Clear();
            }

            public int TicketCount => _tickets.Count;
            public bool Full => _full;

            public bool HasAvailableSpace(GLRenderVxFormattedString renderVxFormattedString)
            {
                //check if we have avaliable space for this?

                float width = renderVxFormattedString.Width;
                float previewY = _currentY;
                float previewX = _currentX;
                if (_currentX + width > _plateWidth)
                {
                    //move to newline                    
                    previewY += _currentLineHeightMax + 4;
                    previewX = 0;
                }

                return previewY + renderVxFormattedString.SpanHeight < _plateHeight;
            }
            public bool CreatePlateTicket(GLPainter painter, GLRenderVxFormattedString renderVxFormattedString)
            {
                //--------------
                //create stencil text buffer                  
                //we use white glyphs on black bg
                //--------------
                if (!_isInitBg)
                {
                    _isInitBg = true;
                    painter.Clear(Color.Black);
                }

                float width = renderVxFormattedString.Width;

                if (_currentX + width > _plateWidth)
                {
                    //move to newline
                    _currentY += _currentLineHeightMax + 4;
                    _currentX = 0;
                    //new line
                    _currentLineHeightMax = (int)Math.Ceiling(renderVxFormattedString.SpanHeight);
                }

                //on current line
                //check available height
                if (_currentY + renderVxFormattedString.SpanHeight > _plateHeight)
                {
                    _full = true;
                    return false;
                }
                //----------------------------------


                if (renderVxFormattedString.SpanHeight > _currentLineHeightMax)
                {
                    _currentLineHeightMax = (int)Math.Ceiling(renderVxFormattedString.SpanHeight);
                }
                //draw string with renderVxFormattedString                
                //float width = renderVxFormattedString.CalculateWidth();

                //PixelFarm.Drawing.GLES2.GLES2Platform.TextService.MeasureString()

                //we need to go to newline or not

                Color prevColor = painter.FontFillColor;
                painter.FontFillColor = Color.White;

                //
                painter.DrawString(renderVxFormattedString, _currentX, _currentY);

                //
                //in this case we can dispose vbo inside renderVx
                //(we can recreate that vbo later)
                renderVxFormattedString.DisposeVbo();

                renderVxFormattedString.WordPlateId = _plateId;
                renderVxFormattedString.WordPlateLeft = (ushort)_currentX;
                renderVxFormattedString.WordPlateTop = (ushort)_currentY;
                renderVxFormattedString.UseWithWordPlate = true;


                _tickets.Add(renderVxFormattedString);
                //--------

                _currentX += (int)Math.Ceiling(renderVxFormattedString.Width) + 1;
                painter.FontFillColor = prevColor;

                return true;
            }
        }
    }

}


