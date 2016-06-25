//MIT, 2016, WinterDev

using PixelFarm.Agg;
namespace PixelFarm.Drawing.WinGdi
{
    class WinGdiRenderVx : RenderVx
    {
        VertexStoreSnap snap;
        public WinGdiRenderVx(VertexStoreSnap snap)
        {
            this.snap = snap;
        }
    }
}