//MIT, 2016-present, WinterDev


namespace PixelFarm.Drawing.WinGdi
{
    class WinGdiRenderVx : RenderVx
    {
        internal VertexStore vxs;
        internal System.Drawing.Drawing2D.GraphicsPath path;
        public WinGdiRenderVx(VertexStore vxs)
        {
            this.vxs = vxs;
        }
    }
    class WinGdiRenderVxFormattedString : RenderVxFormattedString
    {
        char[] _buffer;
        public WinGdiRenderVxFormattedString(char[] _buffer)
        {
            this._buffer = _buffer;
        }
        public override string OriginalString
        {
            get { return new string(_buffer); }
        }
        public char[] InternalBuffer
        {
            get { return _buffer; }
        }

    }
}