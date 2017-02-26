//MIT, 2014-2017, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing.Text
{
    public class ManagedShapingService : TextShapingService
    {
        protected override void GetGlyphPosImpl(ActualFont actualFont, char[] buffer, int startAt, int len, ProperGlyph[] properGlyphs)
        {
            //do shaping and set text layout
        }
    }
}