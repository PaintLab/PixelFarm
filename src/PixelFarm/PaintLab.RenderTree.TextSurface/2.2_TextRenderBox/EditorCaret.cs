//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.TextFlow
{
   
    struct EditorCaret
    {
        //implement caret for text edit 
        int _w;
        int _h;
        public EditorCaret(int w, int h)
        {
            _w = w;
            _h = h;
        }
        public void SetHeight(int h)
        {
            _h = h;
        }
        internal void DrawCaret(DrawBoard d, int x, int y)
        {
            //TODO: config? color or shape of caret
            d.FillRectangle(Color.Black, x, y, _w, _h);
        }
    }
}