//2014 BSD,WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using System;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
 
using PixelFarm.VectorMath;
using PixelFarm.Agg.Transform;


using Mini;
namespace PixelFarm.Agg.SimplePainter
{

    [Info(OrderCode = "01")]
    [Info("SimplePainter")]
    public class SimplePainterSample1 : DemoBase
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
            // draw a circle  
            p.FillColor = ColorRGBA.Blue;
            p.FillCircle(50, 50, 30);

            p.StrokeColor = new ColorRGBA(20, 200, 200);
            p.Line(10, 100, 520, 50);

            // draw a filled box
            p.FillRectangle(60, 260, 200, 280, ColorRGBA.Yellow);
            // and an outline around it
            p.Rectangle(60, 260, 200, 280, ColorRGBA.Magenta);


           

            p.DrawString("A Simple Example", 300, 400);

            p.DrawString("A Simple Example2", 300, 350);

            p.DrawString("A Simple Example3", 300, 300);

            p.DrawString("A Simple Example4", 300, 250);
           
        }
    }


}