//MIT, 2016-2017, WinterDev

using Typography.OpenFont;
using System.Collections.Generic;

namespace Typography.Rendering
{


    public class GlyphPathBuilder : GlyphPathBuilderBase
    {
        GlyphFitOutlineAnalyzer _fitShapeAnalyzer = new GlyphFitOutlineAnalyzer();
        Dictionary<ushort, GlyphFitOutline> _fitoutlineCollection = new Dictionary<ushort, GlyphFitOutline>();
        GlyphFitOutline _fitOutline;
        public GlyphPathBuilder(Typeface typeface)
            : base(typeface)
        {
        }
#if DEBUG
        public bool dbugAlwaysDoCurveAnalysis;

#endif 
        protected override void FitCurrentGlyph(ushort glyphIndex, Glyph glyph)
        {
            //not use interperter so we need to scale it with our machnism
            //this demonstrate our auto hint engine ***
            //you can change this to your own hint engine***   
            _fitOutline = null;//reset
            if (this.UseTrueTypeInstructions)
            {
                base.FitCurrentGlyph(glyphIndex, glyph);
            }
            else
            {
                if (this.UseVerticalHinting)
                {
                    if (!_fitoutlineCollection.TryGetValue(glyphIndex, out _fitOutline))
                    {

                        //---------------------------------------------
                        //test code

                        //GlyphContourBuilder contBuilder = new GlyphContourBuilder();
                        //contBuilder.Reset();
                        //int x = 100, y = 120, w = 700, h = 200;

                        //contBuilder.MoveTo(x, y);
                        //contBuilder.LineTo(x + w, y);
                        //contBuilder.LineTo(x + w, y + h);
                        //contBuilder.LineTo(x, y + h);
                        //contBuilder.CloseFigure();

                        //_fitOutline = _fitShapeAnalyzer.dbugAnalyze(contBuilder.CurrentContour, new ushort[] { 3 });

                        //---------------------------------------------
                        _fitOutline = _fitShapeAnalyzer.CreateGlyphFitOutline(
                            this._outputGlyphPoints,
                            this._outputContours);
                        _fitoutlineCollection.Add(glyphIndex, _fitOutline);
                    }
                }
            }

            //#if DEBUG
            //            if (dbugAlwaysDoCurveAnalysis && _fitOutline == null)
            //            {
            //                //---------------------------------------------
            //                //test code 
            //                //GlyphContourBuilder contBuilder = new GlyphContourBuilder();
            //                //contBuilder.Reset();
            //                //int x = 100, y = 120, w = 700, h = 200;

            //                //contBuilder.MoveTo(x, y);
            //                //contBuilder.LineTo(x + w, y);
            //                //contBuilder.LineTo(x + w, y + h);
            //                //contBuilder.LineTo(x, y + h);
            //                //contBuilder.CloseFigure();

            //                //_fitOutline = _fitShapeAnalyzer.dbugAnalyze(contBuilder.CurrentContour, new ushort[] { 3 }); 


            //                _fitOutline = _fitShapeAnalyzer.CreateGlyphFitOutline(
            //                         this._outputGlyphPoints,
            //                         this._outputContours);
            //            }
            //#endif

        }
        public override void ReadShapes(IGlyphTranslator tx)
        {
            if (this.UseTrueTypeInstructions)
            {
                base.ReadShapes(tx);
                return;
            }
            if (this.UseVerticalHinting)
            {
                //read from our auto hint fitoutline
                //need scale from original.
                float toPixelScale = Typeface.CalculateToPixelScale(this.RecentFontSizeInPixels);
                if (toPixelScale < 0)
                {
                    toPixelScale = 1;
                }
               
                _fitOutline.GenerateOutput(tx, toPixelScale);
            }
            else
            {
                base.ReadShapes(tx);
            }
        }

        public GlyphFitOutline LatestGlyphFitOutline
        {
            get
            {
                return _fitOutline;
            }
        }

    }
}