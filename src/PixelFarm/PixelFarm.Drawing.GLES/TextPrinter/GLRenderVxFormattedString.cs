//MIT, 2016-present, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.CpuBlit;
using PixelFarm.Drawing;
using Typography.Text;
using Typography.TextBreak;

namespace PixelFarm.DrawingGL
{
    enum GLRenderVxFormattedStringGlyphMixMode : byte
    {
        Unknown,
        OnlyStencilGlyphs,
        OnlyColorGlyphs,
        MixedStencilAndColorGlyphs
    }

    enum GLRenderVxFormattedStringState : byte
    {
        S0_Init,
        S1_VertexList,
        S2_TextureStrip,
        S4_Reset

    }
    /// <summary>
    /// texture-based render vx
    /// </summary>
    public class GLRenderVxFormattedString : PixelFarm.Drawing.RenderVxFormattedString
    {
        List<SameFontTextStrip> _strips;

        internal ArrayList<float> _sh_vertexList; //temp src vertice buffer for each SameFontTextStrip
        internal ArrayList<ushort> _sh_indexList; //temp src indice buffer for each SameFontTextStrip

        internal GLRenderVxFormattedString()
        {

        }
        public ushort WordPlateLeft { get; set; }
        public ushort WordPlateTop { get; set; }

        internal WordPlate OwnerPlate { get; set; }
        internal bool UseWithWordPlate { get; set; }
        internal int CreationCycle { get; set; }
        internal bool SkipCreation { get; set; }
        internal bool Delay { get; set; }
        internal GLRenderVxFormattedStringGlyphMixMode GlyphMixMode { get; set; }
        internal GLRenderVxFormattedStringState CreationState { get; set; } //creation state

        void ClearOwnerPlate()
        {
            if (OwnerPlate != null)
            {
                //TODO: review clear owner plate again
                OwnerPlate.RemoveWordStrip(this);
                OwnerPlate = null;
            }
        }

        internal void ClearData()
        {
            WordPlateLeft = WordPlateTop = 0;
            ClearOwnerPlate();

            Delay = false;
            UseWithWordPlate = true;
            GlyphMixMode = GLRenderVxFormattedStringGlyphMixMode.Unknown;

            DisposeWordStripsVbo();


            if (_sh_vertexList != null)
            {
                _sh_vertexList.Clear();
                s_vertextListPool.Push(_sh_vertexList);
                _sh_vertexList = null;
            }
            if (_sh_indexList != null)
            {
                _sh_indexList.Clear();
                s_indexListPool.Push(_sh_indexList);
                _sh_indexList = null;
            }

            if (_strips != null)
            {
                int j = _strips.Count;
                for (int i = 0; i < j; ++i)
                {
                    SameFontTextStrip s = _strips[i];
                    s.Reset();
                    s_textStripPool.Push(s);
                }
                _strips.Clear();
                _strips = null;
            }
        }
        /// <summary>
        /// dispose only vbo inside each word-strip
        /// </summary>
        internal void DisposeWordStripsVbo()
        {
            //dispose only its vbo
            //preserve coord data
            if (_strips != null)
            {
                int j = _strips.Count;
                for (int i = 0; i < j; ++i)
                {
                    _strips[i].DisposeVbo();
                }
            }
        }

        public override void Dispose()
        {
            ClearData();
            base.Dispose();
        }


        internal SameFontTextStrip AppendNewStrip()
        {
            var newstrip = (s_textStripPool.Count > 0) ? s_textStripPool.Pop() : new SameFontTextStrip();
            //get buffer from pool 
            _strips.Add(newstrip);
            return newstrip;
        }


#if DEBUG
        internal bool dbugPreparing = false;
#endif
        public override int StripCount => (_strips == null) ? 0 : _strips.Count;

        //{
        //    get
        //    {

        //        if (_strips == null)
        //        {
        //            if (!dbugPreparing)
        //            {

        //            }
        //            return 0;
        //        }
        //        else
        //        {
        //            return _strips.Count;
        //        }
        //        //return (_strips == null) ? 0 : _strips.Count;
        //    }
        //}

        internal SameFontTextStrip this[int index] => _strips[index];

        internal void ApplyAdditionalVerticalOffset(int maxStripHeight)
        {
            int j = _strips.Count;
            for (int i = 0; i < j; ++i)
            {
                SameFontTextStrip s = _strips[i];
                s.AdditionalVerticalOffset = maxStripHeight - s.SpanHeight;
            }
        }
        internal void PrepareIntermediateStructures(bool useSingleSeqIndexList)
        {
            if (_strips == null)
            {
                _strips = (s_sameFontTextStripListPool.Count > 0) ? s_sameFontTextStripListPool.Pop() : new List<SameFontTextStrip>();
            }
            if (_sh_vertexList == null)
            {
                _sh_vertexList = (s_vertextListPool.Count > 0) ? s_vertextListPool.Pop() : new ArrayList<float>();
            }
            if (!useSingleSeqIndexList)
            {
                _sh_indexList = (s_indexListPool.Count > 0) ? s_indexListPool.Pop() : new ArrayList<ushort>();
            }

            CreationState = GLRenderVxFormattedStringState.S0_Init;
        }

        internal void ReleaseIntermediateStructures()
        {

            if (GlyphMixMode == GLRenderVxFormattedStringGlyphMixMode.MixedStencilAndColorGlyphs)
            {
                return;
            }

            //------------------
            //state2: strips
            //if (_strips != null)
            //{
            //    int j = _strips.Count;
            //    for (int i = 0; i < j; ++i)
            //    {
            //        _strips[i].Reset();
            //    }
            //    _strips.Clear();
            //    s_sameFontTextStripListPool.Push(_strips);
            //    _strips = null;
            //}
            //------------------
            //state1: Data 
            //if (_sh_vertexList != null)
            //{
            //    _sh_vertexList.Clear();
            //    s_vertextListPool.Push(_sh_vertexList);
            //    _sh_vertexList = null;
            //}
            //if (_sh_indexList != null)
            //{
            //    _sh_indexList.Clear();
            //    s_indexListPool.Push(_sh_indexList);
            //    _sh_indexList = null;
            //} 

            CreationState = GLRenderVxFormattedStringState.S4_Reset;
        }

#if DEBUG
        static int s_sameStrCount;
        static int s_dbugTotalId;
        public readonly int dbugId = ++s_dbugTotalId;
        public string dbugText;

        public override string ToString()
        {
            if (dbugText != null)
            {
                return dbugText;
            }
            return base.ToString();
        }
        public override string dbugName => "GL";
#endif

        readonly static Stack<ArrayList<float>> s_vertextListPool = new Stack<ArrayList<float>>();
        readonly static Stack<ArrayList<ushort>> s_indexListPool = new Stack<ArrayList<ushort>>();
        readonly static Stack<SameFontTextStrip> s_textStripPool = new Stack<SameFontTextStrip>();
        readonly static Stack<List<SameFontTextStrip>> s_sameFontTextStripListPool = new Stack<List<SameFontTextStrip>>();

    }
    public class GLRenderVxFormattedStringSpan
    {
        DrawingGL.VertexBufferObject _vbo;
        internal GLRenderVxFormattedStringSpan()
        {
        }

        //--------
        public float[] VertexCoords { get; set; }
        public ushort[] IndexArray { get; set; }
        public int IndexArrayCount { get; set; }

        public ushort WordPlateLeft { get; set; }
        public ushort WordPlateTop { get; set; }

        internal RequestFont RequestFont { get; set; }
        internal WordPlate OwnerPlate { get; set; }
        internal bool Delay { get; set; }
        internal bool UseWithWordPlate { get; set; }

        internal void ClearOwnerPlate()
        {
            OwnerPlate = null;
        }

        internal DrawingGL.VertexBufferObject GetVbo()
        {
            if (_vbo != null)
            {
                return _vbo;
            }
            _vbo = new VertexBufferObject();
            _vbo.CreateBuffers(this.VertexCoords, this.IndexArray);
            return _vbo;
        }

        internal void DisposeVbo()
        {
            //dispose only VBO
            //and we can create the vbo again
            //from VertexCoord and IndexArray 

            if (_vbo != null)
            {
                _vbo.Dispose();
                _vbo = null;
            }
        }
        public void Dispose()
        {
            //no use this any more
            VertexCoords = null;
            IndexArray = null;

            //if (OwnerPlate != null)
            //{
            //    OwnerPlate.RemoveWordStrip(this);
            //    OwnerPlate = null;
            //}

            DisposeVbo();

        }

#if DEBUG
        public string dbugText;
        public override string ToString()
        {
            if (dbugText != null)
            {
                return dbugText;
            }
            return base.ToString();
        }
#endif

    }



    /// <summary>
    /// same font text-strip of specific WordPlate
    /// </summary>
    class SameFontTextStrip
    {
        //our _vbo is used with 1 font texture
        //so if a text-strip use multiple font 
        //we need to separate it into multiple SameFontTextStrip
#if DEBUG
        public SameFontTextStrip()
        {

        }
#endif
        public DrawingGL.VertexBufferObject _vbo;

        public ArrayListSegment<float> VertexCoords { get; set; }
        public ArrayListSegment<ushort> IndexArray { get; set; }
        public int IndexArrayCount => UseSeqIndexList ? _seqIndexCount : IndexArray.Count;

        public float Width { get; set; }
        public int SpanHeight { get; set; }
        public int DescendingInPx { get; set; }
        public int SpanDescendingInPx { get; set; }
        public int AdditionalVerticalOffset { get; set; }
        public bool ColorGlyphOnTransparentBG { get; set; }


        static readonly Stack<int> s_sharedIndexBufferStack = new Stack<int>();
        const int SHARED_INDEX_BUFFER_SIZE = 1024;
        internal bool UseSeqIndexList { get; set; }

        int _seqIndexCount;
        internal void SetIndexCount(int seqIndexCount)
        {
            _seqIndexCount = seqIndexCount;
        }

        int _sharedIndexBufferId; //use shared index buffer or not
        internal DrawingGL.VertexBufferObject GetVbo()
        {
            _sharedIndexBufferId = 0;
            if (_vbo != null)
            {
                return _vbo;
            }

            //for our index list:
            //our generator generate sequence number 1,2,3,4,5,6....
            //so we can use shared index-list buffer

            _vbo = new VertexBufferObject();
            if (IndexArray.len >= SHARED_INDEX_BUFFER_SIZE)
            {
                _vbo.CreateBuffers(VertexCoords, IndexArray);
            }
            else
            {
                //
                if (s_sharedIndexBufferStack.Count > 0)
                {
                    _sharedIndexBufferId = s_sharedIndexBufferStack.Pop();
                }
                else
                {
                    //create a new one
                    _sharedIndexBufferId = SharedIndexBufferHelper.NewElementArrayBuffer(SHARED_INDEX_BUFFER_SIZE);
                }

                //use a simple index buffer here
                _vbo.CreateBuffers(VertexCoords, _sharedIndexBufferId, IndexArray.len);
            }

            return _vbo;
        }

        internal void DisposeVbo()
        {
            //dispose only VBO
            //and we can create the vbo again
            //from VertexCoord and IndexArray 

            if (_vbo != null)
            {
                _vbo.Dispose();
                _vbo = null;
            }

            if (_sharedIndexBufferId > 0)
            {
                s_sharedIndexBufferStack.Push(_sharedIndexBufferId);
                _sharedIndexBufferId = 0;
            }
        }

        public ResolvedFont ResolvedFont { get; set; }
        public SpanBreakInfo BreakInfo { get; set; }
        public void Reset()
        {
            DisposeVbo();
            Width = 0;
            SpanHeight = DescendingInPx = AdditionalVerticalOffset = SpanDescendingInPx = 0;
            ColorGlyphOnTransparentBG = false;
            VertexCoords = ArrayListSegment<float>.Empty;
            IndexArray = ArrayListSegment<ushort>.Empty;
            ResolvedFont = null;
            BreakInfo = null;
        }
    }


}