//MIT, 2016-2017, WinterDev 
using System;
using System.Collections.Generic;
using System.IO;
using Typography.OpenFont;
using Typography.TextLayout;
using Typography.Rendering;

using PixelFarm.Agg;

namespace PixelFarm.Drawing.Fonts
{

    public class VxsTextPrinter : DevTextPrinterBase, ITextPrinter
    {
        /// <summary>
        /// target canvas
        /// </summary>
        CanvasPainter canvasPainter;
        IFontLoader _fontLoader;
        RequestFont _font;
        //-----------------------------------------------------------
        GlyphPathBuilder _glyphPathBuilder;
        GlyphLayout _glyphLayout = new GlyphLayout();
        Dictionary<Typeface, GlyphPathBuilder> _cacheGlyphPathBuilders = new Dictionary<Typeface, GlyphPathBuilder>();
        Dictionary<InstalledFont, Typeface> _cachedTypefaces = new Dictionary<InstalledFont, Typeface>();
        List<GlyphPlan> _outputGlyphPlans = new List<GlyphPlan>();
        //         
        GlyphMeshCollection<VertexStore> hintGlyphCollection = new GlyphMeshCollection<VertexStore>();
        VertexStorePool _vxsPool = new VertexStorePool();
        GlyphTranslatorToVxs _tovxs = new GlyphTranslatorToVxs();
        Typeface _currentTypeface;


        public VxsTextPrinter(CanvasPainter canvasPainter, IFontLoader fontLoader)
        {
            this.canvasPainter = canvasPainter;
            this._fontLoader = fontLoader;

        }
        public CanvasPainter TargetCanvasPainter { get; set; }

        public void ChangeFont(RequestFont font)
        {
            //1.  resolve actual font file
            this._font = font;
            InstalledFont installedFont = _fontLoader.GetFont(font.Name, font.Style.ConvToInstalledFontStyle());

            Typeface foundTypeface;
            if (!_cachedTypefaces.TryGetValue(installedFont, out foundTypeface))
            {
                //if not found
                //create the new one
                using (FileStream fs = new FileStream(installedFont.FontPath, FileMode.Open, FileAccess.Read))
                {
                    var reader = new OpenFontReader();
                    foundTypeface = reader.Read(fs);
                }
                _cachedTypefaces[installedFont] = foundTypeface;
            }

            this.Typeface = foundTypeface;
        }
        public void ChangeFillColor(Color fontColor)
        {
            //change font color

#if DEBUG
            Console.Write("please impl change font color");
#endif
        }
        public void ChangeStrokeColor(Color strokeColor)
        {

        }

        protected override void OnFontSizeChanged()
        {
            //update some font matrix property  
            if (_glyphPathBuilder != null)
            {

                Typeface currentTypeface = _glyphPathBuilder.Typeface;
                float pointToPixelScale = currentTypeface.CalculateToPixelScaleFromPointSize(this.FontSizeInPoints);
                this.FontAscendingPx = currentTypeface.Ascender * pointToPixelScale;
                this.FontDescedingPx = currentTypeface.Descender * pointToPixelScale;
                this.FontLineGapPx = currentTypeface.LineGap * pointToPixelScale;
                this.FontLineSpacingPx = FontAscendingPx - FontDescedingPx + FontLineGapPx;
            }
        }
        public override GlyphLayout GlyphLayoutMan
        {
            get
            {
                return _glyphLayout;
            }
        }

        public override Typeface Typeface
        {
            get
            {
                return _currentTypeface;
            }
            set
            {

                if (_currentTypeface == value) return;
                // 
                //switch to another font              
                if (_glyphPathBuilder != null && !_cacheGlyphPathBuilders.ContainsKey(value))
                {
                    //store current typeface to cache
                    _cacheGlyphPathBuilders[_currentTypeface] = _glyphPathBuilder;
                }
                //reset
                _currentTypeface = value;
                _glyphPathBuilder = null;
                if (value == null) return;
                //----------------------------
                //check if we have this in cache ?
                //if we don't have it, this _currentTypeface will set to null ***                  
                _cacheGlyphPathBuilders.TryGetValue(_currentTypeface, out _glyphPathBuilder);

                if (_glyphPathBuilder == null)
                {
                    _glyphPathBuilder = new GlyphPathBuilder(value);
                }
                OnFontSizeChanged();
            }
        }

        public void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            float ox = canvasPainter.OriginX;
            float oy = canvasPainter.OriginY;

            //1. update some props.. 
            //2. update current type face
            UpdateTypefaceAndGlyphBuilder();
            Typeface typeface = _glyphPathBuilder.Typeface;
            //3. layout glyphs with selected layout technique
            //TODO: review this again, we should use pixel? 
            float fontSizePoint = this.FontSizeInPoints;
            float scale = typeface.CalculateToPixelScaleFromPointSize(fontSizePoint);
            RenderVxGlyphPlan[] glyphPlans = renderVx.glyphList;
            int j = glyphPlans.Length;
            //---------------------------------------------------
            //consider use cached glyph, to increase performance 
            hintGlyphCollection.SetCacheInfo(typeface, fontSizePoint, this.HintTechnique);
            //---------------------------------------------------
            GlyphPosPixelSnapKind x_snap = this.GlyphPosPixelSnapX;
            GlyphPosPixelSnapKind y_snap = this.GlyphPosPixelSnapY;
            float g_x = 0;
            float g_y = 0;
            float baseY = (int)y;

            for (int i = 0; i < j; ++i)
            {
                RenderVxGlyphPlan glyphPlan = glyphPlans[i];
                //-----------------------------------
                //TODO: review here ***
                //PERFORMANCE revisit here 
                //if we have create a vxs we can cache it for later use?
                //-----------------------------------  
                VertexStore glyphVxs;
                if (!hintGlyphCollection.TryGetCacheGlyph(glyphPlan.glyphIndex, out glyphVxs))
                {
                    //if not found then create new glyph vxs and cache it
                    _glyphPathBuilder.SetHintTechnique(this.HintTechnique);
                    _glyphPathBuilder.BuildFromGlyphIndex(glyphPlan.glyphIndex, fontSizePoint);
                    //-----------------------------------  
                    _tovxs.Reset();
                    _glyphPathBuilder.ReadShapes(_tovxs);

                    //TODO: review here, 
                    //float pxScale = _glyphPathBuilder.GetPixelScale();
                    glyphVxs = new VertexStore();
                    _tovxs.WriteOutput(glyphVxs, _vxsPool);
                    //
                    hintGlyphCollection.RegisterCachedGlyph(glyphPlan.glyphIndex, glyphVxs);
                }

                g_x = (float)(glyphPlan.x * scale + x);
                g_y = (float)glyphPlan.y * scale;

                switch (x_snap)
                {
                    default: throw new NotSupportedException();
                    case GlyphPosPixelSnapKind.Integer:
                        g_x = GlyphLayoutExtensions.SnapInteger(g_x);
                        break;
                    case GlyphPosPixelSnapKind.Half:
                        g_x = GlyphLayoutExtensions.SnapHalf(g_x);
                        break;
                    case GlyphPosPixelSnapKind.None:
                        break;
                }
                switch (y_snap)
                {
                    default: throw new NotSupportedException();
                    case GlyphPosPixelSnapKind.Integer:
                        g_y = baseY + GlyphLayoutExtensions.SnapInteger(g_y);   //use baseY not y
                        break;
                    case GlyphPosPixelSnapKind.Half:
                        g_y = baseY + GlyphLayoutExtensions.SnapHalf(g_y);
                        break;
                    case GlyphPosPixelSnapKind.None:
                        //use Y not baseY
                        g_y = (float)y + g_y;
                        break;
                }

                canvasPainter.SetOrigin(g_x, g_y);
                canvasPainter.Fill(glyphVxs);
            }
            //restore prev origin
            canvasPainter.SetOrigin(ox, oy);
        }
        public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] text, int startAt, int len)
        {

            //1. update some props.. 
            //2. update current type face
            UpdateTypefaceAndGlyphBuilder();
            Typeface typeface = _glyphPathBuilder.Typeface;
            //3. layout glyphs with selected layout technique
            //TODO: review this again, we should use pixel?

            float fontSizePoint = this.FontSizeInPoints;
            _outputGlyphPlans.Clear();
            _glyphLayout.Layout(typeface, text, startAt, len, _outputGlyphPlans);
            TextPrinterHelper.CopyGlyphPlans(renderVx, _outputGlyphPlans, typeface.CalculateToPixelScaleFromPointSize(fontSizePoint));

        }

        public override void DrawCaret(float x, float y)
        {

            //        public override void DrawCaret(float xpos, float ypos)
            //        {
            //            CanvasPainter p = this.TargetCanvasPainter;
            //            PixelFarm.Drawing.Color prevColor = p.StrokeColor;
            //            p.StrokeColor = PixelFarm.Drawing.Color.Red;
            //            p.Line(xpos, ypos, xpos, ypos + this.FontAscendingPx);
            //            p.StrokeColor = prevColor;
            //        }

            //throw new NotImplementedException();
        }

        void UpdateTypefaceAndGlyphBuilder()
        {
            //1. update _glyphPathBuilder for current typeface
            UpdateGlyphLayoutSettings();
        }
        void UpdateGlyphLayoutSettings()
        {
            if (this._font == null)
            {
                this.ScriptLang = canvasPainter.CurrentFont.GetOpenFontScriptLang();
                ChangeFont(canvasPainter.CurrentFont);
            }

            //2.1 
            _glyphPathBuilder.SetHintTechnique(this.HintTechnique);
            //2.2
            _glyphLayout.Typeface = this.Typeface;
            _glyphLayout.ScriptLang = this.ScriptLang;
            _glyphLayout.PositionTechnique = this.PositionTechnique;
            _glyphLayout.EnableLigature = this.EnableLigature;
            //3.
            //color...
        }


        public override void DrawFromGlyphPlans(List<GlyphPlan> glyphPlanList, int startAt, int len, float x, float y)
        {
            CanvasPainter canvasPainter = this.TargetCanvasPainter;
            Typeface typeface = _glyphPathBuilder.Typeface;
            //3. layout glyphs with selected layout technique
            //TODO: review this again, we should use pixel?

            float fontSizePoint = this.FontSizeInPoints;
            float scale = typeface.CalculateToPixelScaleFromPointSize(fontSizePoint);


            //4. render each glyph
            float ox = canvasPainter.OriginX;
            float oy = canvasPainter.OriginY;
            int endBefore = startAt + len;

            //---------------------------------------------------
            //consider use cached glyph, to increase performance 
            hintGlyphCollection.SetCacheInfo(typeface, fontSizePoint, this.HintTechnique);
            //---------------------------------------------------
            GlyphPosPixelSnapKind x_snap = this.GlyphPosPixelSnapX;
            GlyphPosPixelSnapKind y_snap = this.GlyphPosPixelSnapY;


            float g_x = 0;
            float g_y = 0;
            float baseY = (int)y;
            for (int i = startAt; i < endBefore; ++i)
            {
                GlyphPlan glyphPlan = glyphPlanList[i];
                //-----------------------------------
                //TODO: review here ***
                //PERFORMANCE revisit here 
                //if we have create a vxs we can cache it for later use?
                //-----------------------------------  
                VertexStore glyphVxs;
                if (!hintGlyphCollection.TryGetCacheGlyph(glyphPlan.glyphIndex, out glyphVxs))
                {
                    _tovxs.Reset();
                    //if not found then create new glyph vxs and cache it
                    _glyphPathBuilder.BuildFromGlyphIndex(glyphPlan.glyphIndex, fontSizePoint);
                    _glyphPathBuilder.ReadShapes(_tovxs);
                    //------------------
                    //TODO: review here,  
                    glyphVxs = new VertexStore();
                    _tovxs.WriteOutput(glyphVxs, _vxsPool);
                    //
                    ////------------------
                    ////find bounding box
                    //RectD boundingRect = new RectD();
                    //PixelFarm.Agg.BoundingRect.GetBoundingRect(new VertexStoreSnap(glyphVxs), ref boundingRect);
                      

                    //------------------
                    hintGlyphCollection.RegisterCachedGlyph(glyphPlan.glyphIndex, glyphVxs);
                }

                g_x = (glyphPlan.x * scale + x);
                g_y = glyphPlan.y * scale;

                switch (x_snap)
                {
                    default: throw new NotSupportedException();
                    case GlyphPosPixelSnapKind.Integer:
                        g_x = GlyphLayoutExtensions.SnapInteger(g_x);
                        break;
                    case GlyphPosPixelSnapKind.Half:
                        g_x = GlyphLayoutExtensions.SnapHalf(g_x);
                        break;
                    case GlyphPosPixelSnapKind.None:
                        break;
                }
                switch (y_snap)
                {
                    default: throw new NotSupportedException();
                    case GlyphPosPixelSnapKind.Integer:
                        g_y = baseY + GlyphLayoutExtensions.SnapInteger(g_y);   //use baseY not y
                        break;
                    case GlyphPosPixelSnapKind.Half:
                        g_y = baseY + GlyphLayoutExtensions.SnapHalf(g_y);
                        break;
                    case GlyphPosPixelSnapKind.None:
                        //use Y not baseY
                        g_y = (float)y + g_y;
                        break;
                }


                canvasPainter.SetOrigin(g_x, g_y);
                canvasPainter.Fill(glyphVxs);
            }
            //restore prev origin
            canvasPainter.SetOrigin(ox, oy);
        }

        public void DrawString(char[] text, int startAt, int len, double x, double y)
        {
            UpdateGlyphLayoutSettings();
            _outputGlyphPlans.Clear();
            _glyphLayout.GenerateGlyphPlans(text, startAt, len, _outputGlyphPlans, null);
            DrawFromGlyphPlans(_outputGlyphPlans, (float)x, (float)y);
        }
        public override void DrawString(char[] textBuffer, int startAt, int len, float x, float y)
        {
            UpdateGlyphLayoutSettings();
            _outputGlyphPlans.Clear();
            _glyphLayout.GenerateGlyphPlans(textBuffer, startAt, len, _outputGlyphPlans, null);
            DrawFromGlyphPlans(_outputGlyphPlans, x, y);

        }

    }

    public static class TextPrinterHelper
    {
        public static void CopyGlyphPlans(RenderVxFormattedString renderVx, List<GlyphPlan> glyphPlans, float scale)
        {
            int n = glyphPlans.Count;
            //copy 
            var renderVxGlyphPlans = new RenderVxGlyphPlan[n];
            for (int i = 0; i < n; ++i)
            {
                GlyphPlan glyphPlan = glyphPlans[i];
                renderVxGlyphPlans[i] = new RenderVxGlyphPlan(
                    glyphPlan.glyphIndex,
                    glyphPlan.x * scale,
                    glyphPlan.y * scale,
                    glyphPlan.advX * scale
                    );
            }
            renderVx.glyphList = renderVxGlyphPlans;
        }
    }


}