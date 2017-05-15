//MIT, 2016-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Agg;

using Typography.OpenFont;
using Typography.TextLayout;

namespace Typography.Contours
{
    class GlyphMeshData
    {
        public VertexStore vxsStore;
        public float avgXOffsetToFit;
    }

    class PixelScaleLayoutEngine : IPixelScaleLayout
    {
        Typeface _typeface;
        float _pxscale = 1;//default
        GlyphMeshCollection<GlyphMeshData> _hintGlyphCollection;
        public PixelScaleLayoutEngine()
        {
        }
        public GlyphMeshCollection<GlyphMeshData> MeshCollection
        {
            get { return _hintGlyphCollection; }
            set
            {
                _hintGlyphCollection = value;
            }
        }
        public void SetFont(Typeface typeface, float pxscale)
        {
            _typeface = typeface;
            _pxscale = pxscale;
        }

        public void Layout(IGlyphPositions posStream, List<GlyphPlan> outputGlyphPlanList)
        {

            //if we want to do grid fitting layout
            //:
            //our pxscale should known the best about how to fit the glyph result
            //to specific pixel scale
            //
            int finalGlyphCount = posStream.Count;
            float pxscale = this._pxscale;
            double cx = 0;
            short cy = 0;
            for (int i = 0; i < finalGlyphCount; ++i)
            {
                short offsetX, offsetY, advW;
                ushort glyphIndex = posStream.GetGlyph(i, out offsetX, out offsetY, out advW);

                float exact_w = advW * pxscale;
                float exact_x = (float)(cx + offsetX * pxscale);
                float exact_y = (float)(cy + offsetY * pxscale);



                outputGlyphPlanList.Add(new GlyphPlan(
                    glyphIndex,
                    exact_x,
                    exact_y,
                    exact_w));
                cx += exact_w;
            }
        }
        //public ABC GetABC(ushort glyphIndex)
        //{

        //    GlyphDynamicOutline found;
        //    if (_fitoutlineCollection.TryGetValue(glyphIndex, out found))
        //    {
        //        //evaluate at current pxscale
        //        float avg_xdiffOffset = found.AvgXFitOffset - 0.33f;//-0.33f for subpix rendering
        //        Bounds orgBounds = found.OriginalGlyphControlBounds;
        //        //---
        //        //this is the scaled of original value
        //        float s_advanced = found.OriginalAdvanceWidth * _fit_pxscale;
        //        float s_minX = orgBounds.XMin * _fit_pxscale;
        //        float s_maxX = orgBounds.XMax * _fit_pxscale;
        //        //---
        //        float new_xmin = s_minX + avg_xdiffOffset;
        //        float new_xmax = s_maxX + avg_xdiffOffset;
        //        float new_advanced = s_advanced + avg_xdiffOffset;

        //        //---
        //        ABC abc = new ABC();

        //        if (s_minX >= 0 && new_xmin < 0)
        //        {
        //            abc.x_offset = 1;
        //            //move org to left 1 px
        //            if (new_xmax + 0.66f > s_maxX)
        //            {
        //                new_advanced = (int)Math.Ceiling(new_advanced);
        //            }
        //        }
        //        //else if (s_minX < 0.5f)
        //        //{
        //        //    //abc.x_offset = 1;
        //        //    ////move org to left 1 px
        //        //    //if (new_xmax + 0.66f > new_advanced)
        //        //    //{
        //        //    //    new_advanced = (int)Math.Ceiling(new_advanced);
        //        //    //}
        //        //}
        //        abc.w = (short)Math.Round(new_advanced);
        //        return abc;
        //    }
        //    else
        //    {
        //        return new ABC();
        //    }

        //}
    }

    public class GlyphPathBuilder : GlyphPathBuilderBase
    {
        GlyphOutlineAnalyzer _fitShapeAnalyzer = new GlyphOutlineAnalyzer();
        Dictionary<ushort, GlyphDynamicOutline> _fitoutlineCollection = new Dictionary<ushort, GlyphDynamicOutline>();
        GlyphDynamicOutline _latestDynamicOutline;

        public GlyphPathBuilder(Typeface typeface)
            : base(typeface)
        {
            //for specific typeface ***
            //
        }

#if DEBUG
        public bool dbugAlwaysDoCurveAnalysis;

#endif


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
                        //add more information for later scaling process
                        _latestDynamicOutline.OriginalAdvanceWidth = glyph.OriginalAdvanceWidth;
                        _latestDynamicOutline.OriginalGlyphControlBounds = glyph.Bounds;
                        //--------------------------------------------- 
                        _fitoutlineCollection.Add(glyphIndex, _latestDynamicOutline);
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
                _latestDynamicOutline.GenerateOutput(tx, toPixelScale);
                //average horizontal diff to fit the grid, this result come from fitting process ***
                this.AvgLeftXOffsetToFit = _latestDynamicOutline.AvgXFitOffset;
            }
            else
            {
                base.ReadShapes(tx);
            }
        }
        /// <summary>
        /// (pxscale-specific) average left x offset to fit point,
        /// </summary>
        public float AvgLeftXOffsetToFit { get; set; }
        public GlyphDynamicOutline LatestGlyphFitOutline
        {
            get
            {
                return _latestDynamicOutline;
            }
        }


    }
}