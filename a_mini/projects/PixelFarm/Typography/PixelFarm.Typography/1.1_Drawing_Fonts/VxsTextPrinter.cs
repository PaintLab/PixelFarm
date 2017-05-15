//MIT, 2016-2017, WinterDev 
using System;
using System.Collections.Generic;
using System.IO;

using PixelFarm.Agg;

using Typography.Contours;
using Typography.OpenFont;
using Typography.Rendering;
using Typography.TextLayout;

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

        GlyphLayout _glyphLayout = new GlyphLayout();
        List<GlyphPlan> _outputGlyphPlans = new List<GlyphPlan>();
        Typeface _currentTypeface;
        PixelScaleLayoutEngine _pxScaleEngine;
        HintedFontStore _hintFontStore;


        public VxsTextPrinter(CanvasPainter canvasPainter, IFontLoader fontLoader)
        {
            this.canvasPainter = canvasPainter;
            this._fontLoader = fontLoader;
            _hintFontStore = new HintedFontStore();

            //assign px scale layout engine for final layout
            _pxScaleEngine = new PixelScaleLayoutEngine();
            _pxScaleEngine.HintedFontStore = _hintFontStore;
            _glyphLayout.PxScaleLayout = _pxScaleEngine;
            //
        }
        public CanvasPainter TargetCanvasPainter { get; set; }

        public void ChangeFont(RequestFont font)
        {
            //1.  resolve actual font file
            this._font = font;
            InstalledFont installedFont = _fontLoader.GetFont(font.Name, font.Style.ConvToInstalledFontStyle());
            Typeface foundTypeface;

            if (!_hintFontStore.TryGetTypeface(installedFont, out foundTypeface))
            {
                //if not found then create a new one
                //if not found
                //create the new one
                using (FileStream fs = new FileStream(installedFont.FontPath, FileMode.Open, FileAccess.Read))
                {
                    var reader = new OpenFontReader();
                    foundTypeface = reader.Read(fs);
                }
                _hintFontStore.RegisterTypeface(installedFont, foundTypeface);
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
            //update some font matrics property   
            Typeface currentTypeface = _currentTypeface;
            if (currentTypeface != null)
            {
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
                // 
                //switch to another font              
                _hintFontStore.SetFont(value);
                //reset
                _currentTypeface = value;
                OnFontSizeChanged();
            }
        }


        public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] text, int startAt, int len)
        {

            //1. update some props.. 
            //2. update current type face
            UpdateTypefaceAndGlyphBuilder();
            Typeface typeface = _currentTypeface;// _glyphPathBuilder.Typeface;
            //3. layout glyphs with selected layout technique
            //TODO: review this again, we should use pixel?

            float pxscale = typeface.CalculateToPixelScaleFromPointSize(FontSizeInPoints);
            _outputGlyphPlans.Clear();
            _glyphLayout.Layout(typeface, text, startAt, len, _outputGlyphPlans);
            TextPrinterHelper.CopyGlyphPlans(renderVx, _outputGlyphPlans, pxscale);
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
                //this.ScriptLang = canvasPainter.CurrentFont.GetOpenFontScriptLang();
                ChangeFont(canvasPainter.CurrentFont);
            }

            //2.1              
            _hintFontStore.SetHintTech(this.HintTechnique);
            //2.2
            _glyphLayout.Typeface = this.Typeface;
            _glyphLayout.ScriptLang = this.ScriptLang;
            _glyphLayout.PositionTechnique = this.PositionTechnique;
            _glyphLayout.EnableLigature = this.EnableLigature;
            //3.
            //color...
        }

        public void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            float ox = canvasPainter.OriginX;
            float oy = canvasPainter.OriginY;

            //1. update some props.. 
            //2. update current type face
            UpdateTypefaceAndGlyphBuilder();
            _hintFontStore.SetFont(_currentTypeface, this.FontSizeInPoints);
            //3. layout glyphs with selected layout technique
            //TODO: review this again, we should use pixel? 
            float fontSizePoint = this.FontSizeInPoints;
            float scale = _currentTypeface.CalculateToPixelScaleFromPointSize(fontSizePoint);
            RenderVxGlyphPlan[] glyphPlans = renderVx.glyphList;
            int j = glyphPlans.Length;
            //---------------------------------------------------
            //consider use cached glyph, to increase performance 

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
                GlyphMeshData glyphMeshData = _hintFontStore.GetGlyphMesh(glyphPlan.glyphIndex);
                g_x = (float)(glyphPlan.x * scale + x);
                g_y = (float)glyphPlan.y * scale;

                switch (x_snap)
                {
                    default: throw new NotSupportedException();
                    case GlyphPosPixelSnapKind.Integer:
                        g_x = GlyphLayoutExtensions.SnapToFitInteger(g_x);
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
                        g_y = baseY + GlyphLayoutExtensions.SnapToFitInteger(g_y);   //use baseY not y
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
                canvasPainter.Fill(glyphMeshData.vxsStore);
            }
            //restore prev origin
            canvasPainter.SetOrigin(ox, oy);
        }
        public override void DrawFromGlyphPlans(List<GlyphPlan> glyphPlanList, int startAt, int len, float x, float y)
        {
            CanvasPainter canvasPainter = this.TargetCanvasPainter;
            //Typeface typeface = _glyphPathBuilder.Typeface;
            //3. layout glyphs with selected layout technique
            //TODO: review this again, we should use pixel?

            float fontSizePoint = this.FontSizeInPoints;
            float scale = _currentTypeface.CalculateToPixelScaleFromPointSize(fontSizePoint);


            //4. render each glyph
            float ox = canvasPainter.OriginX;
            float oy = canvasPainter.OriginY;
            int endBefore = startAt + len;

            //---------------------------------------------------
            //consider use cached glyph, to increase performance 
            _hintFontStore.SetFont(_currentTypeface, fontSizePoint);
            //_hintGlyphCollection.SetCacheInfo(typeface, fontSizePoint, this.HintTechnique);
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
                GlyphMeshData glyphMeshData = _hintFontStore.GetGlyphMesh(glyphPlan.glyphIndex);

                //if (!_hintGlyphCollection.TryGetCacheGlyph(glyphPlan.glyphIndex, out glyphMeshData))
                //{
                //    _tovxs.Reset();
                //    //if not found then create new glyph vxs and cache it
                //    _glyphPathBuilder.BuildFromGlyphIndex(glyphPlan.glyphIndex, fontSizePoint);
                //    _glyphPathBuilder.ReadShapes(_tovxs);
                //    //------------------
                //    //TODO: review here,  
                //    glyphMeshData = new GlyphMeshData();
                //    glyphMeshData.vxsStore = new VertexStore();//create vertex store to hold a result                    
                //    glyphMeshData.avgXOffsetToFit = _glyphPathBuilder.AvgLeftXOffsetToFit;
                //    _tovxs.WriteOutput(glyphMeshData.vxsStore, _vxsPool);
                //    //------------------
                //    _hintGlyphCollection.RegisterCachedGlyph(glyphPlan.glyphIndex, glyphMeshData);
                //}

                g_x = glyphPlan.ExactX + x;
                g_y = glyphPlan.ExactY;

                switch (x_snap)
                {
                    default: throw new NotSupportedException();
                    case GlyphPosPixelSnapKind.Integer:
                        g_x = GlyphLayoutExtensions.SnapToFitInteger(g_x);
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
                        g_y = baseY + GlyphLayoutExtensions.SnapToFitInteger(g_y);   //use baseY not y
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
                canvasPainter.Fill(glyphMeshData.vxsStore);
            }
            //restore prev origin
            canvasPainter.SetOrigin(ox, oy);
        }

        public void DrawString(char[] text, int startAt, int len, double x, double y)
        {
            UpdateGlyphLayoutSettings();
            _outputGlyphPlans.Clear();

            //
            float pxscale = _currentTypeface.CalculateToPixelScaleFromPointSize(this.FontSizeInPoints);
            _glyphLayout.GenerateGlyphPlans(text, startAt, len, _outputGlyphPlans, null);
            //-----
            //we (fine) adjust horizontal fit here

            //-----
            DrawFromGlyphPlans(_outputGlyphPlans, (float)x, (float)y);
        }
        public override void DrawString(char[] textBuffer, int startAt, int len, float x, float y)
        {
            UpdateGlyphLayoutSettings();
            _outputGlyphPlans.Clear();
            //             
            _glyphLayout.FontSizeInPoints = this.FontSizeInPoints;
            _glyphLayout.GenerateGlyphPlans(textBuffer, startAt, len, _outputGlyphPlans, null);

            //-----
            //we (fine) adjust horizontal fit here
            //this step we need grid fitting information

            //-----
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
                    glyphPlan.ExactX,
                    glyphPlan.ExactY,
                    glyphPlan.AdvanceX
                    );
            }
            renderVx.glyphList = renderVxGlyphPlans;
        }
    }


}