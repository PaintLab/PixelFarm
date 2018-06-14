//MIT, 2016-present, WinterDev

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
        string _userString;
        public PdfRenderVxFormattedString(string str)
        {
            this._userString = str;
        }
        public override string OriginalString
        {
            get { return _userString; }
        }

    }
}