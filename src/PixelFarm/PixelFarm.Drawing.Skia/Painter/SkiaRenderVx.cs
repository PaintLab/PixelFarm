//MIT, 2016-present, WinterDev

 
namespace PixelFarm.Drawing.Skia
{
    class WinGdiRenderVx : RenderVx
    {
        internal VertexStore vxs;
        internal SkiaSharp.SKPath path;
        public WinGdiRenderVx(VertexStore vxs)
        {
            this.vxs = vxs;
        }
    }
    class SkiaRenerVxFormattedString : RenderVxFormattedString
    {
        string str;
        public SkiaRenerVxFormattedString(string str)
        {
            this.str = str;
        }
        public override string OriginalString
        {
            get { return str; }
        }
    }
}