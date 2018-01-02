//MIT, 2016-2018, WinterDev

using PixelFarm.Drawing;
namespace PixelFarm.Agg
{
    class AggRenderVx : PixelFarm.Drawing.RenderVx
    {
        internal VertexStoreSnap snap;
        public AggRenderVx(VertexStoreSnap snap)
        {
            this.snap = snap;
        }
    }
    class AggRenderVxFormattedString : PixelFarm.Drawing.RenderVxFormattedString
    {
        public AggRenderVxFormattedString(string str)
        {
            this.OriginalString = str;
        }
    }
}