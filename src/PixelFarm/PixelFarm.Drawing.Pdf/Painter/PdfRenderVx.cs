//MIT, 2016-present, WinterDev

namespace PixelFarm.Drawing.Pdf
{
    class PdfRenderVx : RenderVx
    {
        internal VertexStore vxs;
        //internal System.Drawing.Drawing2D.GraphicsPath path;
        public PdfRenderVx(VertexStore vxs)
        {
            this.vxs = vxs;
        }
    }
    class PdfRenderVxFormattedString : RenderVxFormattedString
    {
        string _userString;
        public PdfRenderVxFormattedString(string str)
        {
            _userString = str;
        }
        public override string OriginalString
        {
            get { return _userString; }
        }

    }
}