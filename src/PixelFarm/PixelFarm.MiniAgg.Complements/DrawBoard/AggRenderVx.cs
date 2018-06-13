//MIT, 2016-present, WinterDev

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
        string str;
        public AggRenderVxFormattedString(string str)
        {
            this.str = str;

        }
        public override string OriginalString
        {
            get { return this.str; }
        }
    }
}