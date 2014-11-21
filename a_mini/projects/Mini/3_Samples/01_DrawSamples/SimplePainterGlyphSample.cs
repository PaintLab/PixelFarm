//2014 BSD,WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using System;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Agg.Font;
using PixelFarm.VectorMath;
using PixelFarm.Agg.Transform;


using Mini;
namespace PixelFarm.Agg.SimplePainter
{

    [Info(OrderCode = "01")]
    [Info("SimplePainterGlyph")]
    public class SimplePainterGlyphSample : DemoBase
    {
        public override void Init()
        {

        }
        public override void Draw(Graphics2D g)
        {
            //1.
            // clear the image to white
            CanvasPainter p = new CanvasPainter(g);
            p.Clear(ColorRGBA.White);
            p.FillColor = ColorRGBA.Blue;

            p.DrawBezierCurve(120, 500 - 160, 220, 500 - 40, 35, 500 - 200, 220, 500 - 260);
            //---------------------------------------------------

            //test font ***
            var font = PixelFarm.Font2.MyFonts.LoadFont("c:\\Windows\\Fonts\\Tahoma.ttf", 48);
            var fontGlyph = font.GetGlyph('ด');
            CurveFlattener curveFlatter = new CurveFlattener();
            //var fontVxsFlatten = curveFlatter.MakeVxs(fontGlyph.vxs);
            p.Fill(fontGlyph.vxs);
            //p.Fill(fontVxsFlatten);

            
        }
    }


}