//MIT, 2016-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Agg;

using PixelFarm.Drawing.Fonts;

using Typography.OpenFont;
using Typography.TextLayout;


namespace Typography.Contours
{


    public struct GlyphControlParameters
    {
        public float avgXOffsetToFit;
        public short minX;
        public short minY;
        public short maxX;
        public short maxY;

    }
    class GlyphMeshStore
    {

        class GlyphMeshData
        {
            public GlyphDynamicOutline dynamicOutline;
            public VertexStore vxsStore;
            public float avgXOffsetToFit;
            public Bounds orgBounds;

            public GlyphControlParameters GetControlPars()
            {
                var pars = new GlyphControlParameters();
                pars.minX = orgBounds.XMin;
                pars.minY = orgBounds.YMin;
                pars.maxX = orgBounds.XMax;
                pars.maxY = orgBounds.YMax;
                pars.avgXOffsetToFit = avgXOffsetToFit;
                return pars;
            }

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
                glyphMeshData.orgBounds = dynamicOutline.OriginalGlyphControlBounds;
                glyphMeshData.dynamicOutline = dynamicOutline;
                Bounds orgGlyphBounds = dynamicOutline.OriginalGlyphControlBounds;



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
        public GlyphControlParameters GetControlPars(ushort glyphIndex)
        {
            return InternalGetGlyphMesh(glyphIndex).GetControlPars();
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

        struct NewABC
        {
            float pxscale;
            public short offsetX;
            public short offsetY;
            public ushort orgAdvW;

            public float s_offsetX;
            public float s_offsetY;
            public float s_advW;
            public float s_xmin;
            public float s_xmax;
            public GlyphControlParameters controlPars;
            public float s_avgToFit;

            public float org_a;
            public float org_c;

            public float s_a;
            public float s_c;
            float c_per_a;


            public int final_advW; //this is approximate final advWidth for this glyph 


            public float c_diff;
            public float s_xmax_to_final_advance;

            public void SetData(GlyphControlParameters controlPars, short offsetX, short offsetY, ushort orgAdvW)
            {
                this.controlPars = controlPars;
                this.offsetX = offsetX;
                this.offsetY = offsetY;
                this.orgAdvW = orgAdvW;
                s_avgToFit = controlPars.avgXOffsetToFit;
                org_a = controlPars.minX;
                org_c = orgAdvW - controlPars.maxX;
                c_per_a = org_c / org_a;
            }
            public void SetScale(float pxscale)
            {
                this.pxscale = pxscale;
                s_offsetX = pxscale * offsetX;
                s_offsetY = pxscale * offsetY;
                s_advW = pxscale * orgAdvW;
                s_xmin = pxscale * controlPars.minX;
                s_xmax = pxscale * controlPars.maxX;
                //--------------------------------------
                s_a = pxscale * org_a;
                s_c = pxscale * org_c;
                //--------------------------------------

                final_advW = (int)Math.Round(s_advW); //***  
                s_xmax_to_final_advance = final_advW - s_xmax;
                c_diff = final_advW - s_advW;
            }

        }
        public void Layout(IGlyphPositions posStream, List<GlyphPlan> outputGlyphPlanList)
        {

            int finalGlyphCount = posStream.Count;
            float pxscale = _typeface.CalculateToPixelScaleFromPointSize(this._fontSizeInPoints);
            float onepx = 1 / pxscale;
            //
            double cx = 0;
            short cy = 0;
            //
            //at this state, we need exact info at this specific pxscale
            //
            _hintedFontStore.SetFont(_typeface, this._fontSizeInPoints);
            NewABC current_ABC = new NewABC();
            NewABC prev_ABC = new NewABC();

            for (int i = 0; i < finalGlyphCount; ++i)
            {
                short offsetX, offsetY, advW; //all from pen-pos
                ushort glyphIndex = posStream.GetGlyph(i, out offsetX, out offsetY, out advW);
                GlyphControlParameters controlPars = _hintedFontStore.GetControlPars(glyphIndex);
                current_ABC.SetData(controlPars, offsetX, offsetY, (ushort)advW);
                current_ABC.SetScale(pxscale);
                //-------------------------------------------------------------

                if (i > 0)
                {
                    //ideal interspace
                    float idealInterGlyphSpace = prev_ABC.s_c + current_ABC.s_a;
                    if (idealInterGlyphSpace > 1 - 0.5f)
                    {
                        //please ensure that we have interspace atleast 1px
                        //if not we just insert 1 px  ***

                        if (idealInterGlyphSpace < 2 - 0.33f)
                        {
                            float diff1 = current_ABC.s_a + prev_ABC.c_diff;
                            if (diff1 < 0)
                            {
                                //need more space
                                //i-o
                                cx += 1;
                            }
                            else
                            {
                                float test0 = current_ABC.s_a + prev_ABC.c_diff;
                                if (test0 > 1)
                                {

                                    cx -= 1;
                                }
                                else
                                {
                                    //i-i
                                    //o-o
                                    //o-i
                                    float fine_h = -prev_ABC.s_avgToFit + prev_ABC.c_diff + current_ABC.s_a;
                                    //if (fine_h < 0)
                                    //{
                                    //    cx -= 1;
                                    //}

                                    //if (prev_ABC.c_diff * 2 < current_ABC.s_a)
                                    //{
                                    //    cx += 1;
                                    //}
                                    if (fine_h > 1)
                                    {
                                        //o-i
                                        cx -= 1;
                                    }

                                    //Console.WriteLine("avg:" + prev_ABC.s_avgToFit +
                                    //        ", c_diff:" + prev_ABC.c_diff +
                                    //        ", s_a:" + current_ABC.s_a +
                                    //        ", need:" + (prev_ABC.c_diff - prev_ABC.s_avgToFit) +
                                    //        ", test0:" + test0);

                                }
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        if (current_ABC.s_a < 0)
                        {
                            // eg i-j seq
                            cx++;
                        }
                    }
                }
                //------------------------------------------------------------- 
                float exact_x = (float)(cx + current_ABC.s_offsetX);
                float exact_y = (float)(cy + current_ABC.s_offsetY);

                //check if the current position can create a sharp glyph
                int exact_x_floor = (int)exact_x;
                float x_offset_to_fit = controlPars.avgXOffsetToFit;
                //offset range that can produce sharp glyph (by observation)
                //is between x_offset_to_fit - 0.3f to x_offset_to_fit + 0.3f 
                float final_x = exact_x_floor + x_offset_to_fit;
                int final_w = current_ABC.final_advW;

                outputGlyphPlanList.Add(new GlyphPlan(
                    glyphIndex,
                    final_x,
                    exact_y,
                    final_w));
                cx += current_ABC.final_advW;

                //-----------------------------------------------
                prev_ABC = current_ABC;//add to prev

                // Console.WriteLine(exact_x + "+" + (x_offset_to_fit) + "=>" + final_x);
            }
        }
    }
}