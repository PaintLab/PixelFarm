//BSD, 2014-2018, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using PixelFarm.Agg.VertexSource;
using PixelFarm.Drawing;
using Mini;

namespace PixelFarm.Agg.Sample_Draw
{
    [Info(OrderCode = "02")]
    [Info("Lines")]
    public class Lines : DemoBase
    {
        public Lines()
        {
        }
        [DemoConfig]
        public LineJoin LineJoin
        {
            get;
            set;
        }
        [DemoConfig]
        public LineCap LineCap
        {
            get;
            set;
        }

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            var aggPainter = p as PixelFarm.Agg.AggPainter;
            if (aggPainter == null)
            {
                return;
            }
            Draw(aggPainter);
        }
        void Draw(PixelFarm.Agg.AggPainter aggPainter)
        {

            aggPainter.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            aggPainter.StrokeColor = PixelFarm.Drawing.Color.Black;
            aggPainter.StrokeWidth = 30.0f;

            //
            VertexStore vxs = new VertexStore();
            PathWriter writer = new PathWriter(vxs);

            writer.MoveTo(20, 0);
            writer.LineTo(100, 100);
            writer.LineTo(20, 200);

            aggPainter.LineJoin = this.LineJoin;
            aggPainter.LineCap = this.LineCap;
            //
            aggPainter.Draw(vxs);
        }
    }
}