//MIT, 2016-2017, WinterDev

using Typography.OpenFont;
using System.Collections.Generic;
using System;
using Typography.TextLayout;

namespace Typography.Contours
{


    public class GlyphPathBuilder : GlyphPathBuilderBase, Typography.TextLayout.IGridFittingEngine
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
        //TODO: remove this
        public float LeftXControl { get; set; }
        /// <summary>
        /// glyph dynamic edge offset
        /// </summary>
        public float GlyphDynamicEdgeOffset { get; set; }

        protected override void FitCurrentGlyph(ushort glyphIndex, Glyph glyph)
        {
            //not use interperter so we need to scale it with our mechanism
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
                        //--------------------------------------------- 
                        _latestDynamicOutline = _fitShapeAnalyzer.CreateDynamicOutline(
                            this._outputGlyphPoints,
                            this._outputContours);
                        _latestDynamicOutline.OriginalAdvanceWidth = glyph.AdvanceWidth;
                        _latestDynamicOutline.OriginalGlyphControlBounds = glyph.Bounds;


                        //--------------------------------------------- 
                        _fitoutlineCollection.Add(glyphIndex, _latestDynamicOutline);
                        this.LeftXControl = 0;
                    }
                }
            }
        }
        public override void ReadShapes(IGlyphTranslator tx)
        {
            //read output shape from dynamic outline

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
                float offsetLenFromMasterOutline = GlyphDynamicEdgeOffset;
                //we will scale back later, so at this step we devide it with toPixelScale
                _latestDynamicOutline.SetDynamicEdgeOffsetFromMasterOutline(offsetLenFromMasterOutline / toPixelScale);
                //-------------------------------------------------
                //this is original control bounds
                //we use this to decide minor shift direction
                //scaled values

                //float one_px = 1 / pxscale;
                //bool atLeast1PxLeft = controlBounds.XMin >= one_px;  //at least 1 px left

                //float s_xmin = controlBounds.XMin * pxscale;
                //float s_ymin = controlBounds.YMin * pxscale;
                //float s_xmax = controlBounds.XMax * pxscale;
                //float s_ymax = controlBounds.YMax * pxscale;
                //float s_advance_w = OriginalAdvanceWidth * pxscale; 
                //------------------------------------------------- 
                //experiment
                //for subpixel rendering 
                //fit_x_offset -= -0.33f; //use use with subpixel, we shift it to the left 1/3 of 1 px 
                _latestDynamicOutline.GenerateOutput(tx, toPixelScale);

                //average horizontal diff to fit the grid, this result come from fitting process
                float avg_xdiff = _latestDynamicOutline.AvgXFitOffset;

                this.LeftXControl = 0;
            }
            else
            {
                base.ReadShapes(tx);
            }
        }

        //-----------------------------------------------------
        public bool NeedFitting(float pxscale)
        {
            return true;
        }

        float _fit_pxscale;
        public void SetPixelScale(float pxscale)
        {
            _fit_pxscale = pxscale;
        }
        public ABC GetABC(ushort glyphIndex)
        {

            GlyphDynamicOutline found;
            if (_fitoutlineCollection.TryGetValue(glyphIndex, out found))
            {
                //evaluate at current pxscale
                float avg_xdiffOffset = found.AvgXFitOffset - 0.33f;//-0.33f for subpix rendering
                Bounds orgBounds = found.OriginalGlyphControlBounds;
                //---
                //this is the scaled of original value
                float s_advanced = found.OriginalAdvanceWidth * _fit_pxscale;
                float s_minX = orgBounds.XMin * _fit_pxscale;
                float s_maxX = orgBounds.XMax * _fit_pxscale;
                //---
                float new_xmin = s_minX + avg_xdiffOffset;
                float new_xmax = s_maxX + avg_xdiffOffset;
                float new_advanced = s_advanced + avg_xdiffOffset;

                //---
                ABC abc = new ABC();

                if (s_minX >= 0 && new_xmin < 0)
                {
                    abc.x_offset = 1;
                    //move org to left 1 px
                    if (new_xmax + 0.66f > s_maxX)
                    {
                        new_advanced = (int)Math.Ceiling(new_advanced);
                    }
                }
                //else if (s_minX < 0.5f)
                //{
                //    //abc.x_offset = 1;
                //    ////move org to left 1 px
                //    //if (new_xmax + 0.66f > new_advanced)
                //    //{
                //    //    new_advanced = (int)Math.Ceiling(new_advanced);
                //    //}
                //}
                abc.w = (short)Math.Round(new_advanced);
                return abc;
            }
            else
            {
                return new ABC();
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