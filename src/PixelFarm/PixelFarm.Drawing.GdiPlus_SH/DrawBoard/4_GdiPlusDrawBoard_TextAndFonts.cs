//BSD, 2014-present, WinterDev
//ArthurHub, Jose Manuel Menendez Poo

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"


namespace PixelFarm.Drawing.WinGdi
{
    partial class GdiPlusDrawBoard
    {
        public override RenderVxFormattedString CreateFormattedString(int[] buffer, int startAt, int len, bool delay)
        {
            throw new System.NotImplementedException();
        }
        public override RenderVxFormattedString CreateFormattedString(char[] buffer, int startAt, int len, bool delay)
        {
            //TODO: review here
            //copy
            char[] copy1 = new char[len];
            System.Array.Copy(buffer, startAt, copy1, 0, len);
            return new WinGdiRenderVxFormattedString(copy1);
        }
        public override void DrawRenderVx(RenderVx renderVx, float x, float y)
        {
            //TODO: review here
            //temp here          
            var vxFormattedString = renderVx as WinGdiRenderVxFormattedString;
            if (vxFormattedString != null)
            {
                _gdigsx.DrawText(vxFormattedString.InternalBuffer, (int)x, (int)y);
            }
            else
            {
                var svxRenderVx = renderVx as PixelFarm.CpuBlit.VxsRenderVx;
                if (svxRenderVx != null)
                {
                    //TODO: review fill color here
                    _gdigsx.FillPath(svxRenderVx);
                }
                else
                {

                }
            }
        }

        public override void DrawText(char[] buffer, int x, int y)
        {
            _gdigsx.DrawText(buffer, x, y);
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            _gdigsx.DrawText(buffer, logicalTextBox, textAlignment);
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            _gdigsx.DrawText(str, startAt, len, logicalTextBox, textAlignment);
        }

        public override RequestFont CurrentFont
        {
            get => _gdigsx.CurrentFont;

            set => _gdigsx.CurrentFont = value;
        }
        public override Color CurrentTextColor
        {
            get => _gdigsx.CurrentTextColor;

            set => _gdigsx.CurrentTextColor = value;
        }
#if DEBUG
        public void dbugTestDrawString()
        {
            _gdigsx.dbugTestDrawText();
        }
#endif
    }
}