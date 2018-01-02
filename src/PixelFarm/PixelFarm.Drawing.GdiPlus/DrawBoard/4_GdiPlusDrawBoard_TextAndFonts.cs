//BSD, 2014-2018, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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


using Win32;
namespace PixelFarm.Drawing.WinGdi
{
    partial class GdiPlusDrawBoard
    {

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
        //====================================================
        public override RequestFont CurrentFont
        {
            get
            {
                return _gdigsx.CurrentFont;
            }
            set
            {
                _gdigsx.CurrentFont = value;
            }
        }
        public override Color CurrentTextColor
        {
            get
            {
                return _gdigsx.CurrentTextColor;
            }
            set
            {
                _gdigsx.CurrentTextColor = value;
            }
        }
    }
}