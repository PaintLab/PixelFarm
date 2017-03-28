//MIT, 2017, Zou Wei(github/zwcloud), WinterDev
using System.Collections.Generic;
using Typography.OpenFont;
using Typography.TextLayout;
using Typography.Rendering;

namespace DrawingGL.Text
{
    /// <summary>
    /// text printer
    /// </summary>
    class TextPrinter
    {
        //funcs:
        //1. layout glyph
        //2. measure glyph
        //3. generate glyph runs into textrun

        private readonly GlyphLayout glyphLayout = new GlyphLayout();

        private readonly List<GlyphPlan> outputGlyphPlans = new List<GlyphPlan>();
        private GlyphTranslatorToPath pathTranslator;
        private string currentFontFile;
        private GlyphPathBuilder currentGlyphPathBuilder;

        //
        // for tess
        // 
        SimpleCurveFlattener _curveFlattener;
        TessTool _tessTool;

        public TextPrinter()
        {
            FontSizeInPoints = 14;
            ScriptLang = ScriptLangs.Latin;

            //
            _curveFlattener = new SimpleCurveFlattener();

            _tessTool = new TessTool();
        }

        public void Measure(char[] textBuffer, int startAt, int len, out float width, out float height)
        {
            glyphLayout.Typeface = this.CurrentTypeFace;
            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSizeInPoints);
            MeasuredStringBox strBox;
            glyphLayout.MeasureString(textBuffer, startAt, len, out strBox, scale);
            width = strBox.width;
            height = strBox.CalculateLineHeight();
        }

        /// <summary>
        /// Font file path
        /// </summary>
        public string FontFilename
        {
            get { return currentFontFile; }
            set
            {
                if (currentFontFile != value)
                {
                    currentFontFile = value;

                    //TODO: review here
                    using (var stream = Utility.ReadFile(value))
                    {
                        var reader = new OpenFontReader();
                        CurrentTypeFace = reader.Read(stream);
                    }

                    //2. glyph builder
                    currentGlyphPathBuilder = new GlyphPathBuilder(CurrentTypeFace);
                    currentGlyphPathBuilder.UseTrueTypeInstructions = false; //reset
                    currentGlyphPathBuilder.UseVerticalHinting = false; //reset
                    switch (this.HintTechnique)
                    {
                        case HintTechnique.TrueTypeInstruction:
                            currentGlyphPathBuilder.UseTrueTypeInstructions = true;
                            break;
                        case HintTechnique.TrueTypeInstruction_VerticalOnly:
                            currentGlyphPathBuilder.UseTrueTypeInstructions = true;
                            currentGlyphPathBuilder.UseVerticalHinting = true;
                            break;
                        case HintTechnique.CustomAutoFit:
                            //custom agg autofit 
                            break;
                    }

                    //3. glyph translater
                    pathTranslator = new GlyphTranslatorToPath();

                    //4. Update GlyphLayout
                    glyphLayout.ScriptLang = this.ScriptLang;
                    glyphLayout.PositionTechnique = this.PositionTechnique;
                    glyphLayout.EnableLigature = this.EnableLigature;
                }
            }
        }

        public HintTechnique HintTechnique { get; set; }
        public float FontSizeInPoints { get; set; }
        public ScriptLang ScriptLang { get; set; }
        public PositionTechnique PositionTechnique { get; set; }
        public bool EnableLigature { get; set; }
        public Typeface CurrentTypeFace { get; private set; }

        /// <summary>
        /// generate glyph run into a given textRun
        /// </summary>
        /// <param name="outputTextRun"></param>
        /// <param name="charBuffer"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        public void GenerateGlyphRuns(TextRun outputTextRun, char[] charBuffer, int start, int len)
        {
            // layout glyphs with selected layout technique
            float sizeInPoints = this.FontSizeInPoints;
            outputTextRun.typeface = this.CurrentTypeFace;
            outputTextRun.sizeInPoints = sizeInPoints;
            //
            outputGlyphPlans.Clear();
            glyphLayout.Typeface = this.CurrentTypeFace;
            glyphLayout.GenerateGlyphPlans(charBuffer, start, len, outputGlyphPlans, null);
            // render each glyph 
            int planCount = outputGlyphPlans.Count;
            for (var i = 0; i < planCount; ++i)
            {

                pathTranslator.Reset();
                //----
                //glyph path 
                //---- 
                GlyphPlan glyphPlan = outputGlyphPlans[i];
                //
                //1. check if we have this glyph in cache?
                //if yes, not need to build it again 
                WritablePath writablePath = new WritablePath();
                pathTranslator.SetOutput(writablePath);
                currentGlyphPathBuilder.BuildFromGlyphIndex(glyphPlan.glyphIndex, sizeInPoints);
                currentGlyphPathBuilder.ReadShapes(pathTranslator);
                //---------- 
                //create glyph mesh
                //TODO: review performance here
                GlyphRun glyphRun = new GlyphRun(writablePath, glyphPlan);
                //-----------------
                //do tess  

                float[] flattenPoints = _curveFlattener.Flatten(writablePath._points);
                List<PixelFarm.DrawingGL.Vertex> vertextList = _tessTool.TessPolygon(flattenPoints);
                //-----------------------------   
                //switch how to fill polygon
                int vxcount = vertextList.Count;
                float[] vtx = new float[vxcount * 2];
                int n = 0;
                for (int p = 0; p < vxcount; ++p)
                {
                    var v = vertextList[p];
                    vtx[n] = (float)v.m_X;
                    vtx[n + 1] = (float)v.m_Y;
                    n += 2;
                }

                //-------------------------------------     
                glyphRun.nTessElements = vxcount;
                glyphRun.tessData = vtx;
                outputTextRun.AddGlyph(glyphRun);
                //------------ 
            }
        }


    }
}