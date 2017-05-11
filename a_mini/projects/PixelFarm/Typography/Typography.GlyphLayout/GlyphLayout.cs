//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;
using Typography.OpenFont;
namespace Typography.TextLayout
{

    public struct GlyphPlan
    {
        public readonly ushort glyphIndex;//2
        ///// <summary>
        ///// exact x pos, start from start pos 0 of span
        ///// </summary>
        //public readonly int x;//4, //TODO: review here=> change to relative pos 
        //public readonly short y;//2
        //public readonly ushort advX;//2
        //public GlyphPlan(ushort glyphIndex, int x, short y, ushort advX)
        //{
        //    this.glyphIndex = glyphIndex;
        //    this.x = x;
        //    this.y = y;
        //    this.advX = advX;
        //}
        public GlyphPlan(ushort glyphIndex, float exactX, short exactY, float extactAdvX)
        {
            this.glyphIndex = glyphIndex;
            this.ExactX = exactX;
            this.ExactY = exactY;
            this.AdvanceX = extactAdvX;

        }
        public float ExactY { get; private set; }
        public float ExactX { get; private set; }
        public float ExactRight
        {
            get
            {
                return ExactX + AdvanceX;
            }
        }
        public float AdvanceX
        {
            get;
            private set;
        }
        public bool AdvanceMoveForward
        {
            get { return this.AdvanceX > 0; }
        }
#if DEBUG
        public override string ToString()
        {
            return "(" + ExactX + "," + ExactY + "), adv:" + AdvanceX;
        }
#endif
    }


    public enum PositionTechnique
    {
        None,
        /// <summary>
        /// use kerning table (old)
        /// </summary>
        Kerning, //old technique
        /// <summary>
        /// use openfont gpos table
        /// </summary>
        OpenFont,
    }

    class GlyphLayoutPlanCollection
    {
        Dictionary<GlyphLayoutPlanKey, GlyphLayoutPlanContext> collection = new Dictionary<GlyphLayoutPlanKey, GlyphLayoutPlanContext>();
        /// <summary>
        /// get glyph layout plan or create if not exists
        /// </summary>
        /// <param name="typeface"></param>
        /// <param name="scriptLang"></param>
        /// <returns></returns>
        public GlyphLayoutPlanContext GetPlanOrCreate(Typeface typeface, ScriptLang scriptLang)
        {
            GlyphLayoutPlanKey key = new GlyphLayoutPlanKey(typeface, scriptLang.internalName);
            GlyphLayoutPlanContext context;
            if (!collection.TryGetValue(key, out context))
            {
                var glyphSubstitution = (typeface.GSUBTable != null) ? new GlyphSubStitution(typeface, scriptLang.shortname) : null;
                var glyphPosition = (typeface.GPOSTable != null) ? new GlyphSetPosition(typeface, scriptLang.shortname) : null;
                collection.Add(key, context = new GlyphLayoutPlanContext(glyphSubstitution, glyphPosition));
            }
            return context;
        }

    }
    struct GlyphLayoutPlanKey
    {
        public Typeface t;
        public int scriptInternameName;
        public GlyphLayoutPlanKey(Typeface t, int scriptInternameName)
        {
            this.t = t;
            this.scriptInternameName = scriptInternameName;
        }
    }
    struct GlyphLayoutPlanContext
    {
        public readonly GlyphSubStitution _glyphSub;
        public readonly GlyphSetPosition _glyphPos;
        public GlyphLayoutPlanContext(GlyphSubStitution _glyphSub, GlyphSetPosition glyphPos)
        {
            this._glyphSub = _glyphSub;
            this._glyphPos = glyphPos;
        }
    }




    public class GlyphLayout
    {
        GlyphLayoutPlanCollection _layoutPlanCollection = new GlyphLayoutPlanCollection();
        Typeface _typeface;
        ScriptLang _scriptLang;
        GlyphSubStitution _gsub;
        GlyphSetPosition _gpos;
        bool _needPlanUpdate;

        internal GlyphIndexList _inputGlyphs = new GlyphIndexList();
        internal List<GlyphPos> _glyphPositions = new List<GlyphPos>();

        public GlyphLayout()
        {
            PositionTechnique = PositionTechnique.OpenFont;
            ScriptLang = ScriptLangs.Latin;
            PixelScale = 1;//default
        }
        public PositionTechnique PositionTechnique { get; set; }
        public ScriptLang ScriptLang
        {
            get { return _scriptLang; }
            set
            {
                if (_scriptLang != value)
                {
                    _needPlanUpdate = true;
                }
                _scriptLang = value;
            }
        }
        public bool EnableLigature { get; set; }

        void UpdateLayoutPlan()
        {
            GlyphLayoutPlanContext context = _layoutPlanCollection.GetPlanOrCreate(this._typeface, this._scriptLang);
            this._gpos = context._glyphPos;
            this._gsub = context._glyphSub;
            _needPlanUpdate = false;
        }

        public Typeface Typeface
        {
            get { return _typeface; }
            set
            {
                if (_typeface != value)
                {
                    _typeface = value;
                    _needPlanUpdate = true;
                }
            }
        }

        /// <summary>
        /// do glyph shaping and glyph out
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startAt"></param>
        /// <param name="len"></param>
        public void Layout(
            char[] str,
            int startAt,
            int len)
        {
            if (_needPlanUpdate)
            {
                UpdateLayoutPlan();
            }

            Typeface typeface = this._typeface;
            //clear before use
            _inputGlyphs.Clear();
            for (int i = 0; i < len; ++i)
            {
                //convert input char to input glyphs
                char c = str[startAt + i];
                _inputGlyphs.AddGlyph(c, (ushort)typeface.LookupIndex(c));
            }
            //----------------------------------------------  
            //glyph substitution            
            if (_gsub != null & len > 0)
            {
                //TODO: review perf here
                _gsub.EnableLigation = this.EnableLigature;
                _gsub.DoSubstitution(_inputGlyphs);
                //
                _inputGlyphs.CreateMapFromUserCharToGlyphIndics();
            }
            //----------------------------------------------  
            //after glyph substitution,
            //number of input glyph MAY changed (increase or decrease).***

            //so count again.
            int finalGlyphCount = _inputGlyphs.Count;
            //----------------------------------------------  
            //glyph position
            _glyphPositions.Clear();
            for (int i = 0; i < finalGlyphCount; ++i)
            {
                //at this stage _inputGlyphs and _glyphPositions 
                //has member 1:1
                ushort glyIndex = _inputGlyphs[i];
                //
                Glyph orgGlyph = typeface.GetGlyphByIndex(glyIndex);
                if (!orgGlyph.HasAdvWidth)
                {
                    orgGlyph.AdvanceWidth = typeface.GetHAdvanceWidthFromGlyphIndex(glyIndex);
                }
                //
                //this is original value WITHOUT fit-to-grid adjust
                _glyphPositions.Add(new GlyphPos(
                    glyIndex,
                    orgGlyph)
                   );
            }
            PositionTechnique posTech = this.PositionTechnique;
            if (_gpos != null && len > 1 && posTech == PositionTechnique.OpenFont)
            {
                _gpos.DoGlyphPosition(_glyphPositions);
            }
        }

        //
        internal List<GlyphPlan> _myGlyphPlans = new List<GlyphPlan>();

        public float PixelScale { get; set; }
    }


    public delegate void GlyphReadOutputDelegate(int index, GlyphPlan glyphPlan);

    public static class GlyphLayoutExtensions
    {

        public static float SnapToFitInteger(float value)
        {
            int floor_value = (int)value;
            return (value - floor_value >= (1f / 2f)) ? floor_value + 1 : floor_value;
        }
        public static float SnapHalf(float value)
        {
            int floor_value = (int)value;
            //round to int 0, 0.5,1.0
            return (value - floor_value >= (2f / 3f)) ? floor_value + 1 : //else->
                   (value - floor_value >= (1f / 3f)) ? floor_value + 0.5f : floor_value;
        }
        static int SnapUpper(float value)
        {
            int floor_value = (int)value;
            return floor_value + 1;
        }

        /// <summary>
        /// read latest layout output into outputGlyphPlanList
        /// </summary>
        /// <param name="glyphLayout"></param>
        /// <param name="outputGlyphPlanList"></param>
        public static void ReadOutput(this GlyphLayout glyphLayout, List<UserCharToGlyphIndexMap> outputGlyphPlanList)
        {
            outputGlyphPlanList.AddRange(glyphLayout._inputGlyphs._mapUserCharToGlyphIndics);
        }
        /// <summary>
        /// read latest layout output into outputGlyphPlanList
        /// </summary>
        public static void ReadOutput(this GlyphLayout glyphLayout, List<GlyphPlan> outputGlyphPlanList)
        {
            Typeface typeface = glyphLayout.Typeface;
            List<GlyphPos> glyphPositions = glyphLayout._glyphPositions;
            //3.read back
            int finalGlyphCount = glyphPositions.Count;
            int cx = 0;
            short cy = 0;

            PositionTechnique posTech = glyphLayout.PositionTechnique;
            float pxscale = glyphLayout.PixelScale;

            switch (posTech)
            {
                default: throw new NotSupportedException();
                case PositionTechnique.OpenFont:
                    { 

                        for (int i = 0; i < finalGlyphCount; ++i)
                        {

                            GlyphPos glyphPos = glyphPositions[i];
                            float actual_adv = glyphPos.AdvWidth * pxscale;
                            Bounds glyphBounds = glyphPos.Bounds;
                            short leftBearing, rightBearing;
                            glyphPos.GetLeftAndRightBearing(out leftBearing, out rightBearing);
                            float scaled_leftBearing = leftBearing * pxscale;
                            float scaled_rightBearing = rightBearing * pxscale;
                            float scaled_xmin = glyphBounds.XMin * pxscale;
                            float scaled_xmax = glyphBounds.XMax * pxscale;

                            //--------------------------------------------------
                            //preview fitting values
                            float preview_adv_width = SnapToFitInteger(actual_adv);

                            float adv_width_diff = preview_adv_width - actual_adv;
                            if (adv_width_diff < 0)
                            {
                                //actual diff is shorter than expect 
                                float diff_from_xmax = preview_adv_width - scaled_xmax;

                            }
                            else
                            {
                                //actual diff is longer than expect
                            }
                            //for good readable we should ensure
                            //left bearing space at least 1 px 
                            //if not then we need to adjust it.
                            //int floor_left_bearing = (int)scaled_leftBearing;
                            //int floor_right_bearing = (int)scaled_rightBearing;
                            //int bearing_sum = (int)(scaled_leftBearing + scaled_rightBearing);
                            //float left_bearing_adjust = 1.3f;

                            //if (bearing_sum == 0)
                            //{
                            //    //this could cause congest glyph
                            //    //so we adjust it
                            //    //left_bearing_adjust = 1 - (scaled_leftBearing);
                            //    preview_adv_width += left_bearing_adjust;
                            //}

                            int x_pos = cx + glyphPos.xoffset;
                            if (scaled_leftBearing < 0.5f)
                            {
                                x_pos += 1;
                            }
                            //-----------------------
                            outputGlyphPlanList.Add(new GlyphPlan(
                                glyphPos.glyphIndex,
                                x_pos,
                                (short)(cy + glyphPos.yoffset),
                                glyphPos.AdvWidth));

                            cx += (int)preview_adv_width;

                        }
                    }
                    break;
            }


            //for (int i = 0; i < finalGlyphCount; ++i)
            //{

            //    GlyphPos glyphPos = glyphPositions[i];
            //    //----------------------------------   
            //    switch (posTech)
            //    {
            //        default: throw new NotSupportedException();
            //        case PositionTechnique.None:
            //            {
            //                throw new NotSupportedException();
            //                //outputGlyphPlanList.Add(new GlyphPlan(glyphPos.glyphIndex, cx, cy, glyphPos.AdvWidth));
            //                //cx += glyphPos.AdvWidth;
            //            }
            //            break;
            //        case PositionTechnique.OpenFont:
            //            {
            //                //if want grid fitting
            //                //in this version  we must ensure
            //                //that we fit each grid to integer pos 

            //                //--------------------------------------------------
            //                //version 1:
            //                //original, no horizontal grid fit
            //                //outputGlyphPlanList.Add(new GlyphPlan(
            //                //    glyphPos.glyphIndex,
            //                //    cx + glyphPos.xoffset,
            //                //    (short)(cy + glyphPos.yoffset),
            //                //    glyphPos.advWidth));
            //                //cx += glyphPos.advWidth; 
            //                //--------------------------------------------------
            //                //version 2:

            //                //float actual_adv = glyphPos.AdvWidth * pxscale;
            //                //float fitting_adv = SnapInteger(actual_adv);

            //                //short leftBearing, rightBearing;
            //                //glyphPos.GetLeftAndRightBearing(out leftBearing, out rightBearing);
            //                //float scaled_leftBearing = leftBearing * pxscale;
            //                //float scaled_rightBearing = rightBearing * pxscale;
            //                ////
            //                ////for good readable we should ensure
            //                ////left bearing space at least 1 px 
            //                ////if not then we need to adjust it.
            //                //int floor_left_bearing = (int)scaled_leftBearing;
            //                //int floor_right_bearing = (int)scaled_rightBearing;
            //                //int bearing_sum = (int)(scaled_leftBearing + scaled_rightBearing);
            //                //float left_bearind_adjust = 0;

            //                //if (bearing_sum == 0)
            //                //{
            //                //    //this could cause congest glyph
            //                //    //so we adjust it
            //                //    left_bearind_adjust = 1 - (scaled_leftBearing);
            //                //    fitting_adv += left_bearind_adjust;
            //                //}

            //                //outputGlyphPlanList.Add(new GlyphPlan(
            //                //    glyphPos.glyphIndex,
            //                //    cx + glyphPos.xoffset + (int)(left_bearind_adjust / pxscale),
            //                //    (short)(cy + glyphPos.yoffset),
            //                //    glyphPos.AdvWidth));
            //                ////this will be scaled again later
            //                //cx += (int)(fitting_adv / pxscale);
            //                //--------------------------------------------------
            //                //version 3:
            //                //acutal value after scale
            //                float actual_adv = glyphPos.AdvWidth * pxscale;
            //                Bounds glyphBounds = glyphPos.Bounds;
            //                short leftBearing, rightBearing;
            //                glyphPos.GetLeftAndRightBearing(out leftBearing, out rightBearing);
            //                float scaled_leftBearing = leftBearing * pxscale;
            //                float scaled_rightBearing = rightBearing * pxscale;
            //                float scaled_xmin = glyphBounds.XMin * pxscale;
            //                float scaled_xmax = glyphBounds.XMax * pxscale;

            //                //--------------------------------------------------
            //                //preview fitting values
            //                float preview_adv_width = SnapToFitInteger(actual_adv);

            //                float adv_width_diff = preview_adv_width - actual_adv;
            //                if (adv_width_diff < 0)
            //                {
            //                    //actual diff is shorter than expect 
            //                    float diff_from_xmax = preview_adv_width - scaled_xmax;

            //                }
            //                else
            //                {
            //                    //actual diff is longer than expect
            //                }
            //                //for good readable we should ensure
            //                //left bearing space at least 1 px 
            //                //if not then we need to adjust it.
            //                //int floor_left_bearing = (int)scaled_leftBearing;
            //                //int floor_right_bearing = (int)scaled_rightBearing;
            //                //int bearing_sum = (int)(scaled_leftBearing + scaled_rightBearing);
            //                //float left_bearing_adjust = 1.3f;

            //                //if (bearing_sum == 0)
            //                //{
            //                //    //this could cause congest glyph
            //                //    //so we adjust it
            //                //    //left_bearing_adjust = 1 - (scaled_leftBearing);
            //                //    preview_adv_width += left_bearing_adjust;
            //                //}

            //                int x_pos = cx + glyphPos.xoffset;
            //                if (scaled_leftBearing < 0.5f)
            //                {
            //                    x_pos += 1;
            //                }
            //                //-----------------------
            //                outputGlyphPlanList.Add(new GlyphPlan(
            //                    glyphPos.glyphIndex,
            //                    x_pos,
            //                    (short)(cy + glyphPos.yoffset),
            //                    glyphPos.AdvWidth));

            //                cx += (int)preview_adv_width;

            //            }
            //            break;
            //        case PositionTechnique.Kerning:
            //            {
            //                throw new NotSupportedException();
            //                ////TODO: review this again, this should be merged with openfont layout
            //                //if (i > 0)
            //                //{
            //                //    cx += typeface.GetKernDistance(prev_index, glyphPos.glyphIndex);
            //                //}
            //                //outputGlyphPlanList.Add(new GlyphPlan(
            //                //   prev_index = glyphPos.glyphIndex,
            //                //   cx,
            //                //   cy,
            //                //   glyphPos.AdvWidth));
            //                //cx += glyphPos.AdvWidth;
            //            }
            //            break;
            //    }
            //}
        }
        ///// <summary>
        ///// read latest layout output into outputGlyphPlanList
        ///// </summary>
        //public static void ReadOutput(this GlyphLayout glyphLayout, List<GlyphPlan> outputGlyphPlanList)
        //{
        //    Typeface typeface = glyphLayout.Typeface;
        //    List<GlyphPos> glyphPositions = glyphLayout._glyphPositions;
        //    //3.read back
        //    int finalGlyphCount = glyphPositions.Count;
        //    int cx = 0;
        //    short cy = 0;

        //    PositionTechnique posTech = glyphLayout.PositionTechnique;
        //    ushort prev_index = 0;

        //    float pxscale = glyphLayout.PixelScale;

        //    for (int i = 0; i < finalGlyphCount; ++i)
        //    {

        //        GlyphPos glyphPos = glyphPositions[i];
        //        //----------------------------------   
        //        switch (posTech)
        //        {
        //            default: throw new NotSupportedException();
        //            case PositionTechnique.None:
        //                {
        //                    throw new NotSupportedException();
        //                    //outputGlyphPlanList.Add(new GlyphPlan(glyphPos.glyphIndex, cx, cy, glyphPos.AdvWidth));
        //                    //cx += glyphPos.AdvWidth;
        //                }
        //                break;
        //            case PositionTechnique.OpenFont:
        //                {
        //                    //if want grid fitting
        //                    //in this version  we must ensure
        //                    //that we fit each grid to integer pos 

        //                    //--------------------------------------------------
        //                    //version 1:
        //                    //original, no horizontal grid fit
        //                    //outputGlyphPlanList.Add(new GlyphPlan(
        //                    //    glyphPos.glyphIndex,
        //                    //    cx + glyphPos.xoffset,
        //                    //    (short)(cy + glyphPos.yoffset),
        //                    //    glyphPos.advWidth));
        //                    //cx += glyphPos.advWidth; 
        //                    //--------------------------------------------------
        //                    //version 2:

        //                    //float actual_adv = glyphPos.AdvWidth * pxscale;
        //                    //float fitting_adv = SnapInteger(actual_adv);

        //                    //short leftBearing, rightBearing;
        //                    //glyphPos.GetLeftAndRightBearing(out leftBearing, out rightBearing);
        //                    //float scaled_leftBearing = leftBearing * pxscale;
        //                    //float scaled_rightBearing = rightBearing * pxscale;
        //                    ////
        //                    ////for good readable we should ensure
        //                    ////left bearing space at least 1 px 
        //                    ////if not then we need to adjust it.
        //                    //int floor_left_bearing = (int)scaled_leftBearing;
        //                    //int floor_right_bearing = (int)scaled_rightBearing;
        //                    //int bearing_sum = (int)(scaled_leftBearing + scaled_rightBearing);
        //                    //float left_bearind_adjust = 0;

        //                    //if (bearing_sum == 0)
        //                    //{
        //                    //    //this could cause congest glyph
        //                    //    //so we adjust it
        //                    //    left_bearind_adjust = 1 - (scaled_leftBearing);
        //                    //    fitting_adv += left_bearind_adjust;
        //                    //}

        //                    //outputGlyphPlanList.Add(new GlyphPlan(
        //                    //    glyphPos.glyphIndex,
        //                    //    cx + glyphPos.xoffset + (int)(left_bearind_adjust / pxscale),
        //                    //    (short)(cy + glyphPos.yoffset),
        //                    //    glyphPos.AdvWidth));
        //                    ////this will be scaled again later
        //                    //cx += (int)(fitting_adv / pxscale);
        //                    //--------------------------------------------------
        //                    //version 3:
        //                    //acutal value after scale
        //                    float actual_adv = glyphPos.AdvWidth * pxscale;
        //                    Bounds glyphBounds = glyphPos.Bounds;
        //                    short leftBearing, rightBearing;
        //                    glyphPos.GetLeftAndRightBearing(out leftBearing, out rightBearing);
        //                    float scaled_leftBearing = leftBearing * pxscale;
        //                    float scaled_rightBearing = rightBearing * pxscale;
        //                    float scaled_xmin = glyphBounds.XMin * pxscale;
        //                    float scaled_xmax = glyphBounds.XMax * pxscale;

        //                    //--------------------------------------------------
        //                    //preview fitting values
        //                    float preview_adv_width = SnapToFitInteger(actual_adv);

        //                    float adv_width_diff = preview_adv_width - actual_adv;
        //                    if (adv_width_diff < 0)
        //                    {
        //                        //actual diff is shorter than expect 
        //                        float diff_from_xmax = preview_adv_width - scaled_xmax;

        //                    }
        //                    else
        //                    {
        //                        //actual diff is longer than expect
        //                    }
        //                    //for good readable we should ensure
        //                    //left bearing space at least 1 px 
        //                    //if not then we need to adjust it.
        //                    //int floor_left_bearing = (int)scaled_leftBearing;
        //                    //int floor_right_bearing = (int)scaled_rightBearing;
        //                    //int bearing_sum = (int)(scaled_leftBearing + scaled_rightBearing);
        //                    //float left_bearing_adjust = 1.3f;

        //                    //if (bearing_sum == 0)
        //                    //{
        //                    //    //this could cause congest glyph
        //                    //    //so we adjust it
        //                    //    //left_bearing_adjust = 1 - (scaled_leftBearing);
        //                    //    preview_adv_width += left_bearing_adjust;
        //                    //}

        //                    int x_pos = cx + glyphPos.xoffset;
        //                    if (scaled_leftBearing < 0.5f)
        //                    {
        //                        x_pos += 1;
        //                    }
        //                    //-----------------------
        //                    outputGlyphPlanList.Add(new GlyphPlan(
        //                        glyphPos.glyphIndex,
        //                        x_pos,
        //                        (short)(cy + glyphPos.yoffset),
        //                        glyphPos.AdvWidth));

        //                    cx += (int)preview_adv_width;

        //                }
        //                break;
        //            case PositionTechnique.Kerning:
        //                {
        //                    throw new NotSupportedException();
        //                    ////TODO: review this again, this should be merged with openfont layout
        //                    //if (i > 0)
        //                    //{
        //                    //    cx += typeface.GetKernDistance(prev_index, glyphPos.glyphIndex);
        //                    //}
        //                    //outputGlyphPlanList.Add(new GlyphPlan(
        //                    //   prev_index = glyphPos.glyphIndex,
        //                    //   cx,
        //                    //   cy,
        //                    //   glyphPos.AdvWidth));
        //                    //cx += glyphPos.AdvWidth;
        //                }
        //                break;
        //        } 
        //    }
        //}



        /// <summary>
        /// read latest layout output
        /// </summary>
        /// <param name="glyphLayout"></param>
        /// <param name="readDel"></param>
        public static void ReadOutput(this GlyphLayout glyphLayout, GlyphReadOutputDelegate readDel)
        {
            throw new NotSupportedException();

            //Typeface typeface = glyphLayout.Typeface;
            //List<GlyphPos> glyphPositions = glyphLayout._glyphPositions;
            ////3.read back
            //int finalGlyphCount = glyphPositions.Count;
            //int cx = 0;
            //short cy = 0;

            //PositionTechnique posTech = glyphLayout.PositionTechnique;
            //ushort prev_index = 0;
            //for (int i = 0; i < finalGlyphCount; ++i)
            //{

            //    GlyphPos glyphPos = glyphPositions[i];
            //    //----------------------------------   
            //    switch (posTech)
            //    {
            //        default: throw new NotSupportedException();
            //        case PositionTechnique.None:
            //            readDel(i, new GlyphPlan(glyphPos.glyphIndex, cx, cy, glyphPos.AdvWidth));
            //            break;
            //        case PositionTechnique.OpenFont:
            //            readDel(i, new GlyphPlan(
            //                glyphPos.glyphIndex,
            //                cx + glyphPos.xoffset,
            //                (short)(cy + glyphPos.yoffset),
            //                glyphPos.AdvWidth));
            //            break;
            //        case PositionTechnique.Kerning:

            //            if (i > 0)
            //            {
            //                cx += typeface.GetKernDistance(prev_index, glyphPos.glyphIndex);
            //            }
            //            readDel(i, new GlyphPlan(
            //                 prev_index = glyphPos.glyphIndex,
            //               cx,
            //               cy,
            //               glyphPos.AdvWidth));

            //            break;
            //    }
            //    cx += glyphPos.AdvWidth;
            //}
        }
        public static void Layout(this GlyphLayout glyphLayout, Typeface typeface, char[] str, int startAt, int len, List<GlyphPlan> outputGlyphList)
        {
            glyphLayout.Typeface = typeface;
            glyphLayout.Layout(str, startAt, len);
            glyphLayout.ReadOutput(outputGlyphList);
        }
        public static void Layout(this GlyphLayout glyphLayout, char[] str, int startAt, int len, List<GlyphPlan> outputGlyphList)
        {
            glyphLayout.Layout(str, startAt, len);
            glyphLayout.ReadOutput(outputGlyphList);
        }
        public static void Layout(this GlyphLayout glyphLayout, char[] str, int startAt, int len, GlyphReadOutputDelegate readDel)
        {
            glyphLayout.Layout(str, startAt, len);
            glyphLayout.ReadOutput(readDel);

        }
        public static void GenerateGlyphPlans(this GlyphLayout glyphLayout,
                  char[] textBuffer,
                  int startAt,
                  int len,
                  List<GlyphPlan> userGlyphPlanList,
                  List<UserCharToGlyphIndexMap> charToGlyphMapList)
        {
            //generate glyph plan based on its current setting
            glyphLayout.Layout(textBuffer, startAt, len, userGlyphPlanList);
            //note that we print to userGlyphPlanList
            //---------------- 
            //3. user char to glyph index map
            if (charToGlyphMapList != null)
            {
                glyphLayout.ReadOutput(charToGlyphMapList);
            }

        }
        public static void MeasureString(
                this GlyphLayout glyphLayout,
                char[] textBuffer,
                int startAt,
                int len, out MeasuredStringBox strBox, float scale = 1)
        {
            //TODO: consider extension method
            List<GlyphPlan> outputGlyphPlans = glyphLayout._myGlyphPlans;
            outputGlyphPlans.Clear();
            glyphLayout.Layout(textBuffer, startAt, len, outputGlyphPlans);
            //
            int j = outputGlyphPlans.Count;


            Typeface currentTypeface = glyphLayout.Typeface;
            if (j == 0)
            {
                //not scale
                strBox = new MeasuredStringBox(0,
                    currentTypeface.Ascender * scale,
                    currentTypeface.Descender * scale,
                    currentTypeface.LineGap * scale);

            }
            else
            {
                GlyphPlan lastOne = outputGlyphPlans[j - 1];
                strBox = new MeasuredStringBox((lastOne.ExactRight) * scale,
                        currentTypeface.Ascender * scale,
                        currentTypeface.Descender * scale,
                        currentTypeface.LineGap * scale);
            }
        }
    }

    /// <summary>
    /// how to pos a glyph on specific point
    /// </summary>
    public enum GlyphPosPixelSnapKind : byte
    {
        Integer,//default
        Half,
        None
    }

}