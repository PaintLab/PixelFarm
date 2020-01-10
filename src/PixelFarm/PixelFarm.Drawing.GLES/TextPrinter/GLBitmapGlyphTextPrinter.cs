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

namespace PixelFarm.DrawingGL
{
    /// <summary>
    /// texture-based render vx
    /// </summary>
    public class GLRenderVxFormattedString : PixelFarm.Drawing.RenderVxFormattedString
    {
        DrawingGL.VertexBufferObject _vbo;
        internal GLRenderVxFormattedString()
        {
        }

        //--------
        public float[] VertexCoords { get; set; }
        public ushort[] IndexArray { get; set; }
        public int IndexArrayCount { get; set; }
        public RequestFont RequestFont { get; set; }



        public WordPlate OwnerPlate { get; set; }
        public bool Delay { get; set; }
        public bool UseWithWordPlate { get; set; }

        public ushort WordPlateLeft { get; set; }
        public ushort WordPlateTop { get; set; }

        internal void ClearOwnerPlate()
        {
            OwnerPlate = null;
            //State = VxState.NoTicket;
        }

        public DrawingGL.VertexBufferObject GetVbo()
        {
            if (_vbo != null)
            {
                return _vbo;
            }
            _vbo = new VertexBufferObject();
            _vbo.CreateBuffers(this.VertexCoords, this.IndexArray);
            return _vbo;
        }
        public void DisposeVbo()
        {
            //dispose only VBO
            //and we can create the vbo again
            //from VertexCoord and IndexArray 

            if (_vbo != null)
            {
                _vbo.Dispose();
                _vbo = null;
            }
        }
        public override void Dispose()
        {
            //no use this any more
            VertexCoords = null;
            IndexArray = null;

            if (OwnerPlate != null)
            {
                OwnerPlate.RemoveWordStrip(this);
                OwnerPlate = null;
            }

            DisposeVbo();
            base.Dispose();
        }

#if DEBUG
        public string dbugText;
        public override string ToString()
        {
            if (dbugText != null)
            {
                return dbugText;
            }
            return base.ToString();
        }
        public override string dbugName => "GL";
#endif

    }
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

            TextBaseline = TextBaseline.Top;
            //TextBaseline = TextBaseline.Alphabetic;
            //TextBaseline = TextBaseline.Bottom;
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

        public TextBaseline TextBaseline { get; set; }

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
                    _vboBuilder.WriteRect(
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
        public void DrawString(RenderVxFormattedString rendervx, double x, double y)
        {
            _pcx.FontFillColor = _painter.FontFillColor;


            GLRenderVxFormattedString vxFmtStr = (GLRenderVxFormattedString)rendervx;
            switch (DrawingTechnique)
            {
                case GlyphTexturePrinterDrawingTechnique.Stencil:
                    {
                        if (vxFmtStr.Delay && vxFmtStr.OwnerPlate == null)
                        {
                            //add this to queue to create                              
                            return;
                        }
                        if (!vxFmtStr.UseWithWordPlate)
                        {
                            _pcx.DrawGlyphImageWithStencilRenderingTechnique4_FromVBO(
                                   _glBmp,
                                   vxFmtStr.GetVbo(),
                                   vxFmtStr.IndexArrayCount,
                                   (float)Math.Round(x),
                                   (float)Math.Floor(y));
                            return;
                        }
                        //---------
                        //use word plate 
                        if (vxFmtStr.OwnerPlate == null)
                        {
                            //UseWithWordPlate=> this renderVx has beed assign to wordplate,
                            //but when WordPlateId=0, this mean the wordplate was disposed.
                            //so create it again
                            _painter.CreateWordStrip(vxFmtStr);
                        }

                        float base_offset = 0;
                        switch (TextBaseline)
                        {
                            case TextBaseline.Alphabetic:
                                base_offset = -(vxFmtStr.SpanHeight + vxFmtStr.DescendingInPx);
                                break;
                            case TextBaseline.Top:
                                base_offset = vxFmtStr.DescendingInPx;
                                break;
                            case TextBaseline.Bottom:
                                base_offset = -vxFmtStr.SpanHeight;
                                break;
                        }

                        //eval again 
                        if (vxFmtStr.OwnerPlate != null)
                        {
                            _pcx.DrawWordSpanWithStencilTechnique((GLBitmap)vxFmtStr.OwnerPlate._backBuffer.GetImage(),
                                vxFmtStr.WordPlateLeft, -vxFmtStr.WordPlateTop - vxFmtStr.SpanHeight,
                                vxFmtStr.Width, vxFmtStr.SpanHeight,
                                (float)Math.Round(x),
                                (float)Math.Floor(y + base_offset));
                        }
                        else
                        {
                            //can't create at this time
                            //render with vbo 
                            _pcx.DrawGlyphImageWithStencilRenderingTechnique4_FromVBO(
                                 _glBmp,
                                 vxFmtStr.GetVbo(),
                                 vxFmtStr.IndexArrayCount,
                                 (float)Math.Round(x),
                                 (float)Math.Floor(y + base_offset));
                        }
                    }
                    break;
                case GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering:
                    {
                        if (vxFmtStr.Delay && vxFmtStr.OwnerPlate == null)
                        {
                            //add this to queue to create                              
                            return;
                        }
                        //LCD-Effect****
                        if (!vxFmtStr.UseWithWordPlate ||
                            _pcx.FontFillColor != Color.White)//in this version!
                        {
                            _pcx.DrawGlyphImageWithSubPixelRenderingTechnique4_FromVBO(
                              _glBmp,
                              vxFmtStr.GetVbo(),
                              vxFmtStr.IndexArrayCount,
                              (float)Math.Round(x),
                              (float)Math.Floor(y));
                            return;
                        }

                       
                        //use word plate 
                        if (vxFmtStr.OwnerPlate == null)
                        {
                            //UseWithWordPlate=> this renderVx has beed assign to wordplate,
                            //but when WordPlateId=0, this mean the wordplate was disposed.
                            //so create it again
                            _painter.CreateWordStrip(vxFmtStr);
                        }

                        float base_offset = 0;
                        switch (TextBaseline)
                        {
                            case TextBaseline.Alphabetic:
                                base_offset = -(vxFmtStr.SpanHeight + vxFmtStr.DescendingInPx);
                                break;
                            case TextBaseline.Top:
                                base_offset = vxFmtStr.DescendingInPx;
                                break;
                            case TextBaseline.Bottom:
                                base_offset = -vxFmtStr.SpanHeight;
                                break;
                        }
                        //eval again                         
                        if (vxFmtStr.OwnerPlate != null)
                        {
                            //depend on current owner plate bg 
                            // 
                            _pcx.DrawWordSpanWithInvertedColorCopyTechnique((GLBitmap)vxFmtStr.OwnerPlate._backBuffer.GetImage(),
                                vxFmtStr.WordPlateLeft, -vxFmtStr.WordPlateTop - vxFmtStr.SpanHeight,
                                vxFmtStr.Width, vxFmtStr.SpanHeight,
                                (float)Math.Round(x),
                                (float)Math.Floor(y + base_offset));
                        }
                        else
                        {
                            //can't create at this time or we 
                            //render with vbo

                            _pcx.DrawGlyphImageWithSubPixelRenderingTechnique4_FromVBO(
                                _glBmp,
                                vxFmtStr.GetVbo(),
                                vxFmtStr.IndexArrayCount,
                                (float)Math.Round(x),
                                (float)Math.Floor(y + base_offset));
                        }

                    }
                    break;
            }
        }
#if DEBUG
        static int _dbugCount;
#endif



        void CreateTextCoords(GLRenderVxFormattedString vxFmtStr, char[] buffer, int startAt, int len)
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
                _vboBuilder.WriteRect(ref srcRect, g_left, g_top, scaleFromTexture);

            }
            //---
            //copy vbo result and store into  renderVx  
            //TODO: review here
            vxFmtStr.IndexArrayCount = _vboBuilder._indexList.Count;
            vxFmtStr.IndexArray = _vboBuilder._indexList.ToArray();
            vxFmtStr.VertexCoords = _vboBuilder._buffer.ToArray();
            vxFmtStr.Width = acc_x;
            vxFmtStr.SpanHeight = _font.LineSpacingInPixels;
            vxFmtStr.DescendingInPx = (short)_font.DescentInPixels;

            _vboBuilder.Clear();
        }
        public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] buffer, int startAt, int len)
        {

            var vxFmtStr = (GLRenderVxFormattedString)renderVx;
            CreateTextCoords(vxFmtStr, buffer, startAt, len);
            if (vxFmtStr.Delay)
            {
                //when we use delay mode
                //we need to save current font setting  of the _painter
                //with the render vx---
                vxFmtStr.RequestFont = _painter.CurrentFont;
            }
            else
            {
                _painter.CreateWordStrip(vxFmtStr);
            }
        }
    }

    class WordPlateMx
    {

        Dictionary<ushort, WordPlate> _wordPlates = new Dictionary<ushort, WordPlate>();
        //**dictionay not guarantee sorted id**
        Queue<WordPlate> _wordPlatesQueue = new Queue<WordPlate>();
        WordPlate _latestPlate;

        int _defaultPlateW = 800;
        int _defaultPlateH = 600;

        static ushort s_totalPlateId = 0;

        public WordPlateMx()
        {
            MaxPlateCount = 20; //*** important!
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
            _wordPlatesQueue.Clear();
        }
        public WordPlate GetNewWordPlate(GLRenderVxFormattedString fmtPlate)
        {
            if (_latestPlate != null &&
                _latestPlate.HasAvailableSpace(fmtPlate))
            {
                return _latestPlate;
            }
            return GetNewWordPlate();
        }
        public WordPlate GetNewWordPlate()
        {
            //create new and register  
            if (_wordPlates.Count == MaxPlateCount)
            {
                if (AutoRemoveOldestPlate)
                {
                    //**dictionay not guarantee sorted id**
                    //so we use queue, (TODO: use priority queue) 
                    WordPlate oldest = _wordPlatesQueue.Dequeue();
                    _wordPlates.Remove(oldest._plateId);
#if DEBUG
                    if (oldest.dbugUsedCount < 50)
                    {

                    }
                    //oldest.dbugSaveBackBuffer("word_plate_" + oldest._plateId + ".png");
#endif

                    oldest.Dispose();
                    oldest = null;
                }
            }

            if (s_totalPlateId + 1 >= ushort.MaxValue)
            {
                throw new NotSupportedException();
            }

            s_totalPlateId++;  //so plate_id starts at 1 

            WordPlate wordPlate = new WordPlate(s_totalPlateId, _defaultPlateW, _defaultPlateH);
            _wordPlates.Add(s_totalPlateId, wordPlate);
            _wordPlatesQueue.Enqueue(wordPlate);
            wordPlate.Cleared += WordPlate_Cleared;
            return _latestPlate = wordPlate;
        }

        private void WordPlate_Cleared(WordPlate obj)
        {

        }
    }

    public class WordPlate : IDisposable
    {
        bool _isInitBg;
        int _currentX;
        int _currentY;
        int _currentLineHeightMax;
        readonly int _plateWidth;
        readonly int _plateHeight;
        bool _full;

        internal readonly ushort _plateId;
        Dictionary<GLRenderVxFormattedString, bool> _wordStrips = new Dictionary<GLRenderVxFormattedString, bool>();
        internal Drawing.GLES2.MyGLBackbuffer _backBuffer;

        public event Action<WordPlate> Cleared;

        public WordPlate(ushort plateId, int w, int h)
        {
            _plateId = plateId;
            _plateWidth = w;
            _plateHeight = h;
            _backBuffer = new Drawing.GLES2.MyGLBackbuffer(w, h);
        }
#if DEBUG
        internal int dbugUsedCount;
        public void dbugSaveBackBuffer(string filename)
        {
            //save output
            using (Image img = _backBuffer.CopyToNewMemBitmap())
            {
                if (img is MemBitmap memBmp)
                {
                    memBmp.SaveImage(filename);
                }
            }
        }
#endif

        const int INTERLINE_SPACE = 1; //px
        const int INTERWORD_SPACE = 1; //px

        public void Dispose()
        {
            //clear all
            if (_backBuffer != null)
            {
                _backBuffer.Dispose();
                _backBuffer = null;
            }
            foreach (GLRenderVxFormattedString k in _wordStrips.Keys)
            {
                //essential!
                k.ClearOwnerPlate();
            }
            _wordStrips.Clear();
        }


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
                previewY += _currentLineHeightMax + INTERLINE_SPACE;
                previewX = 0;
            }

            return previewY + renderVxFormattedString.SpanHeight < _plateHeight;
        }
        public bool CreateWordStrip(GLPainter painter, GLRenderVxFormattedString renderVxFormattedString)
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
                _currentY += _currentLineHeightMax + INTERLINE_SPACE;//interspace =4 px
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


            renderVxFormattedString.UseWithWordPlate = false;
            //
            painter.DrawString(renderVxFormattedString, _currentX, _currentY);
            renderVxFormattedString.UseWithWordPlate = true;
            //
            //in this case we can dispose vbo inside renderVx
            //(we can recreate that vbo later)
            renderVxFormattedString.DisposeVbo();

            renderVxFormattedString.OwnerPlate = this;
            renderVxFormattedString.WordPlateLeft = (ushort)_currentX;
            renderVxFormattedString.WordPlateTop = (ushort)_currentY;
            renderVxFormattedString.UseWithWordPlate = true;

#if DEBUG
            dbugUsedCount++;
#endif
            _wordStrips.Add(renderVxFormattedString, true);
            //--------

            _currentX += (int)Math.Ceiling(renderVxFormattedString.Width) + INTERWORD_SPACE; //interspace x 1px
            painter.FontFillColor = prevColor;

            return true;
        }

        public void RemoveWordStrip(GLRenderVxFormattedString vx)
        {
            _wordStrips.Remove(vx);
            if (_full && _wordStrips.Count == 0)
            {
                Cleared?.Invoke(this);
            }
        }
    }


}


