//MIT, 2016, WinterDev

using PixelFarm.Agg;
namespace PixelFarm.Drawing.WinGdi
{
    class WinGdiRenderVx : RenderVx
    {
        internal VertexStoreSnap snap;
        internal SkiaSharp.SKPath path;
        public WinGdiRenderVx(VertexStoreSnap snap)
        {
            this.snap = snap;
        }
    }
}