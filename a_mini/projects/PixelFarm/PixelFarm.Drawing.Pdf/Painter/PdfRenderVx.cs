//MIT, 2016-2017, WinterDev

using PixelFarm.Agg;
namespace PixelFarm.Drawing.Pdf
{
    class PdfRenderVx : RenderVx
    {
        internal VertexStoreSnap snap;
        //internal System.Drawing.Drawing2D.GraphicsPath path;
        public PdfRenderVx(VertexStoreSnap snap)
        {
            this.snap = snap;
        }
    }
    class PdfRenderVxFormattedString : RenderVxFormattedString
    {
        public PdfRenderVxFormattedString(string str)
        {
            this.OriginalString = str;
        }
    }
}