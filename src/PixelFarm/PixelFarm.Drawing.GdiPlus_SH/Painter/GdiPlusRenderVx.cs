//MIT, 2016-present, WinterDev


namespace PixelFarm.Drawing.WinGdi
{
    class WinGdiRenderVx : RenderVx
    {
        internal VertexStore _vxs;
        internal System.Drawing.Drawing2D.GraphicsPath _path;
        public WinGdiRenderVx(VertexStore vxs)
        {
            _vxs = vxs;
        }
    }
    class WinGdiRenderVxFormattedString : RenderVxFormattedString
    {
        char[] _buffer;
        public WinGdiRenderVxFormattedString(char[] buffer)
        {
            _buffer = buffer;
        }
        public override int StripCount => throw new System.NotImplementedException();
        public char[] InternalBuffer => _buffer;
#if DEBUG
        public override string dbugName => "WinGdi";
#endif
    }
}