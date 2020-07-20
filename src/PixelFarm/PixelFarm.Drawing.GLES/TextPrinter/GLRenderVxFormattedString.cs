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

    /// <summary>
    /// texture-based render vx
    /// </summary>
    public class GLRenderVxFormattedString : PixelFarm.Drawing.RenderVxFormattedString
    {
        List<SameFontTextStrip> _strips = new List<SameFontTextStrip>();
        internal ArrayList<float> _sh_vertexList = new ArrayList<float>();
        internal ArrayList<ushort> _sh_indexList = new ArrayList<ushort>();

        internal GLRenderVxFormattedString()
        {

        }

        public ushort WordPlateLeft { get; set; }
        public ushort WordPlateTop { get; set; }
        internal WordPlate OwnerPlate { get; set; }
        internal bool Delay { get; set; }
        internal bool UseWithWordPlate { get; set; }
        internal GLRenderVxFormattedStringGlyphMixMode GlyphMixMode { get; set; }
        internal void ClearOwnerPlate()
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
            OwnerPlate = null;

            Delay = false;
            UseWithWordPlate = true;
            GlyphMixMode = GLRenderVxFormattedStringGlyphMixMode.Unknown;

            DisposeVbo();
            _strips.Clear();
        }

        internal void DisposeVbo()
        {
            //dispose only its vbo
            //preserve coord data
            int j = _strips.Count;
            for (int i = 0; i < j; ++i)
            {
                _strips[i].DisposeVbo();
            }
        }

        public override void Dispose()
        {
            ClearData();
            base.Dispose();
        }

        internal SameFontTextStrip AppendNewStrip()
        {
            var newstrip = new SameFontTextStrip();
            //get buffer from pool 
            _strips.Add(newstrip);
            return newstrip;
        }

        internal int StripCount => _strips.Count;
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
        public override string dbugName => "GL";
#endif

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
        //we need to separate it into multiple SameFontTextStrip.


        public SameFontTextStrip()
        {
        }
        public DrawingGL.VertexBufferObject _vbo;


        public ArrayListSpan<float> VertexCoords { get; set; }
        public ArrayListSpan<ushort> IndexArray { get; set; }
        public int IndexArrayCount => IndexArray.Count;
        public float Width { get; set; }
        public int SpanHeight { get; set; }
        public int DescendingInPx { get; set; }
        public int AdditionalVerticalOffset { get; set; }
        public bool ColorGlyphOnTransparentBG { get; set; }

        internal DrawingGL.VertexBufferObject GetVbo()
        {
            if (_vbo != null)
            {
                return _vbo;
            }

            _vbo = new VertexBufferObject();
            _vbo.CreateBuffers(VertexCoords, IndexArray);
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

        public ResolvedFont ResolvedFont { get; set; }
        public SpanBreakInfo BreakInfo { get; set; }
    }


}