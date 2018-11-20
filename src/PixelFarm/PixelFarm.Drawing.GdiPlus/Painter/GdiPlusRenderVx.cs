//MIT, 2016-present, WinterDev


namespace PixelFarm.Drawing.WinGdi
{
    class WinGdiRenderVx : RenderVx
    {
        internal VertexStore _vxs;
        internal System.Drawing.Drawing2D.GraphicsPath _path;
        public WinGdiRenderVx(VertexStore vxs)
        {
            this._vxs = vxs;
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