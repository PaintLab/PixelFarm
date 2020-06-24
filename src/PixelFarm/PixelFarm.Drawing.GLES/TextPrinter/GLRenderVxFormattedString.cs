//MIT, 2016-present, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

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
        internal List<SameFontWordPlateTextStrip> _strips = new List<SameFontWordPlateTextStrip>();

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
        internal void Reuse()
        {
            WordPlateLeft = WordPlateTop = 0;
            ClearOwnerPlate();
            OwnerPlate = null;

            Delay = false;
            UseWithWordPlate = true;
            GlyphMixMode = GLRenderVxFormattedStringGlyphMixMode.Unknown;
            int j = _strips.Count;
            for (int i = 0; i < j; ++i)
            {
                _strips[i].DisposeVbo();
            }

            _strips.Clear();
        }

        public void DisposeVbo()
        {
            //dispose only its vbo
            //preserve coord data
            foreach (SameFontWordPlateTextStrip s in _strips)
            {
                s.DisposeVbo();
            }
        }

        public override void Dispose()
        {
            //no use this any more
            //VertexCoords = null;
            //IndexArray = null;

            ClearOwnerPlate();
            DisposeVbo();
            _strips.Clear();

            base.Dispose();
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

    class GLFormattedGlyphPlanSeq : FormattedGlyphPlanSeq
    {
        public GLFormattedGlyphPlanSeq() { }
        /// <summary>
        /// whitespace count append at the end of this seq
        /// </summary>
        public ushort PostfixWhitespaceCount { get; set; }

        public RequestFont ActualFont { get; set; }
    }

    /// <summary>
    /// same font text-strip of specific WordPlate
    /// </summary>
    class SameFontWordPlateTextStrip
    {
        public SameFontWordPlateTextStrip() { }
        public DrawingGL.VertexBufferObject _vbo;

        public float[] VertexCoords { get; set; }
        public ushort[] IndexArray { get; set; }
        public int IndexArrayCount { get; set; }

        public float Width { get; set; }
        public int SpanHeight { get; set; }
        public int DescendingInPx { get; set; }

        public RequestFont ActualFont { get; set; }
        public bool ColorGlyphOnTransparentBG { get; set; }

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
    }

}