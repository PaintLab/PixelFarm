//MIT, 2016-2017, WinterDev

using Typography.OpenFont;
using System.Collections.Generic;

namespace Typography.Rendering
{


    public class GlyphPathBuilder : GlyphPathBuilderBase
    {
        GlyphOutlineAnalyzer _fitShapeAnalyzer = new GlyphOutlineAnalyzer();
        Dictionary<ushort, GlyphDynamicOutline> _fitoutlineCollection = new Dictionary<ushort, GlyphDynamicOutline>();
        GlyphDynamicOutline _latestDynamicOutline;

        public GlyphPathBuilder(Typeface typeface)
            : base(typeface)
        {
        }
#if DEBUG
        public bool dbugAlwaysDoCurveAnalysis;

#endif 
        public float LeftXControl { get; set; }
        public float GlyphEdgeOffset { get; set; }
        protected override void FitCurrentGlyph(ushort glyphIndex, Glyph glyph)
        {
            //not use interperter so we need to scale it with our machnism
            //this demonstrate our auto hint engine ***
            //you can change this to your own hint engine***   
            _latestDynamicOutline = null;//reset
            if (this.UseTrueTypeInstructions)
            {
                base.FitCurrentGlyph(glyphIndex, glyph);
            }
            else
            {
                if (this.UseVerticalHinting)
                {
                    if (!_fitoutlineCollection.TryGetValue(glyphIndex, out _latestDynamicOutline))
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
                        _latestDynamicOutline = _fitShapeAnalyzer.CreateDynamicOutline(
                            this._outputGlyphPoints,
                            this._outputContours);
                        _fitoutlineCollection.Add(glyphIndex, _latestDynamicOutline);
                        this.LeftXControl = _latestDynamicOutline.LeftControlPosX;
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
                float offsetLenFromMasterOutline = GlyphEdgeOffset;
                //we will scale back later, so at this step we devide it with toPixelScale
                _latestDynamicOutline.SetNewEdgeOffsetFromMasterOutline(offsetLenFromMasterOutline / toPixelScale);

                _latestDynamicOutline.GenerateOutput(tx, toPixelScale);
                this.LeftXControl = _latestDynamicOutline.LeftControlPosX;
            }
            else
            {
                base.ReadShapes(tx);
            }
        }

        public GlyphDynamicOutline LatestGlyphFitOutline
        {
            get
            {
                return _latestDynamicOutline;
            }
        }

    }
}