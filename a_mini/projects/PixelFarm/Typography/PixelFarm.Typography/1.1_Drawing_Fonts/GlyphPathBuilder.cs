//MIT, 2016-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Agg;

using PixelFarm.Drawing.Fonts;

using Typography.OpenFont;
using Typography.TextLayout;


namespace Typography.Contours
{


    class GlyphMeshStore
    {
        class GlyphMeshData
        {
            public GlyphDynamicOutline dynamicOutline;
            public VertexStore vxsStore;
            public float avgXOffsetToFit;
        }
        /// <summary>
        /// store typeface and its builder
        /// </summary>
        Dictionary<Typeface, GlyphPathBuilder> _cacheGlyphPathBuilders = new Dictionary<Typeface, GlyphPathBuilder>();
        /// <summary>
        /// glyph mesh data for specific condition
        /// </summary>
        GlyphMeshCollection<GlyphMeshData> _hintGlyphCollection = new GlyphMeshCollection<GlyphMeshData>();

        GlyphPathBuilder _currentGlyphBuilder;
        Typeface _currentTypeface;
        float _currentFontSizeInPoints;
        HintTechnique _currentHintTech;

        VertexStorePool _vxsPool = new VertexStorePool(); //TODO: review pool again
        GlyphTranslatorToVxs _tovxs = new GlyphTranslatorToVxs();

        public GlyphMeshStore()
        {
            //---------------- 
            //float offsetLenFromMasterOutline = GlyphDynamicEdgeOffset;
            ////we will scale back later, so at this step we devide it with toPixelScale
            //_latestDynamicOutline.SetDynamicEdgeOffsetFromMasterOutline(offsetLenFromMasterOutline / toPixelScale);

        }
        public void SetHintTechnique(HintTechnique hintTech)
        {
            _currentHintTech = hintTech;

        }

        /// <summary>
        /// set current font
        /// </summary>
        /// <param name="typeface"></param>
        /// <param name="fontSizeInPoints"></param>
        public void SetFont(Typeface typeface, float fontSizeInPoints)
        {
            if (_currentGlyphBuilder != null && !_cacheGlyphPathBuilders.ContainsKey(typeface))
            {
                //store current typeface to cache
                _cacheGlyphPathBuilders[_currentTypeface] = _currentGlyphBuilder;
            }
            _currentTypeface = typeface;
            _currentGlyphBuilder = null;
            if (typeface == null) return;

            //----------------------------
            //check if we have this in cache ?
            //if we don't have it, this _currentTypeface will set to null ***                  
            _cacheGlyphPathBuilders.TryGetValue(_currentTypeface, out _currentGlyphBuilder);
            if (_currentGlyphBuilder == null)
            {
                _currentGlyphBuilder = new GlyphPathBuilder(typeface);
            }
            //----------------------------------------------
            this._currentFontSizeInPoints = fontSizeInPoints;
            //------------------------------------------ 
            _hintGlyphCollection.SetCacheInfo(typeface, this._currentFontSizeInPoints, _currentHintTech);
        }
        /// <summary>
        /// get existing or create new one from current font setting
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <returns></returns>
        GlyphMeshData InternalGetGlyphMesh(ushort glyphIndex)
        {
            GlyphMeshData glyphMeshData;
            if (!_hintGlyphCollection.TryGetCacheGlyph(glyphIndex, out glyphMeshData))
            {
                //if not found then create new glyph vxs and cache it
                _currentGlyphBuilder.SetHintTechnique(_currentHintTech);
                _currentGlyphBuilder.BuildFromGlyphIndex(glyphIndex, _currentFontSizeInPoints);
                GlyphDynamicOutline dynamicOutline = _currentGlyphBuilder.LatestGlyphFitOutline;
                //-----------------------------------  
                glyphMeshData = new GlyphMeshData();
                glyphMeshData.avgXOffsetToFit = dynamicOutline.AvgXFitOffset;
                glyphMeshData.dynamicOutline = dynamicOutline;
                _hintGlyphCollection.RegisterCachedGlyph(glyphIndex, glyphMeshData);
                //-----------------------------------    
            }
            return glyphMeshData;
        }
        /// <summary>
        /// get glyph left offset-to-fit value from current font setting
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <returns></returns>
        public float GetGlyphLeftOffsetControl(ushort glyphIndex)
        {
            return InternalGetGlyphMesh(glyphIndex).avgXOffsetToFit;
        }

        /// <summary>
        /// get glyph mesh from current font setting
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <returns></returns>
        public VertexStore GetGlyphMesh(ushort glyphIndex)
        {
            GlyphMeshData glyphMeshData = InternalGetGlyphMesh(glyphIndex);
            if (glyphMeshData.vxsStore == null)
            {
                //build vxs
                _tovxs.Reset();

                float pxscale = _currentTypeface.CalculateToPixelScaleFromPointSize(_currentFontSizeInPoints);
                GlyphDynamicOutline dynamicOutline = glyphMeshData.dynamicOutline;
                dynamicOutline.GenerateOutput(_tovxs, pxscale);
                glyphMeshData.vxsStore = new VertexStore();
                //----------------
                _tovxs.WriteOutput(glyphMeshData.vxsStore, _vxsPool);

            }
            return glyphMeshData.vxsStore;

        }
    }

    class PixelScaleLayoutEngine : IPixelScaleLayout
    {
        Typeface _typeface;
        GlyphMeshStore _hintedFontStore;
        float _fontSizeInPoints;
        public PixelScaleLayoutEngine()
        {
        }
        public GlyphMeshStore HintedFontStore
        {
            get { return _hintedFontStore; }
            set
            {
                _hintedFontStore = value;
            }
        }
        public void SetFont(Typeface typeface, float fontSizeInPoints)
        {
            _typeface = typeface;
            _fontSizeInPoints = fontSizeInPoints;
        }
        public void Layout(IGlyphPositions posStream, List<GlyphPlan> outputGlyphPlanList)
        {

            //if we want to do grid fitting layout
            //:
            //our pxscale should known the best about how to fit the glyph result
            //to specific pixel scale
            //
            int finalGlyphCount = posStream.Count;
            float pxscale = _typeface.CalculateToPixelScaleFromPointSize(this._fontSizeInPoints);
            double cx = 0;
            short cy = 0;
            //
            //at this state, we need exact info at this specific pxscale
            //
            _hintedFontStore.SetFont(_typeface, this._fontSizeInPoints); //?


            for (int i = 0; i < finalGlyphCount; ++i)
            {
                short offsetX, offsetY, advW;
                ushort glyphIndex = posStream.GetGlyph(i, out offsetX, out offsetY, out advW);

                float leftControl = _hintedFontStore.GetGlyphLeftOffsetControl(glyphIndex);

                float exact_w = advW * pxscale;
                float exact_y = (float)(cy + offsetY * pxscale);
                float exact_x = (int)Math.Round((float)(cx + offsetX * pxscale));
                exact_x += leftControl;
                exact_w += leftControl;


                outputGlyphPlanList.Add(new GlyphPlan(
                    glyphIndex,
                    exact_x,
                    exact_y,
                    exact_w));
                //
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
            //float offsetLenFromMasterOutline = GlyphDynamicEdgeOffset;
            //_latestDynamicOutline.SetDynamicEdgeOffsetFromMasterOutline(offsetLenFromMasterOutline / toPixelScale);
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
                        //
                        _latestDynamicOutline.GenerateOutput(null, Typeface.CalculateToPixelScale(RecentFontSizeInPixels));
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

                float toPixelScale = Typeface.CalculateToPixelScale(RecentFontSizeInPixels);
                if (toPixelScale < 0)
                {
                    toPixelScale = 1;
                }
                _latestDynamicOutline.GenerateOutput(tx, toPixelScale);
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