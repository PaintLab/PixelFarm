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

    [Info(OrderCode = "02")]
    [Info("Lines2")]
    public class Lines2 : DemoBase
    {
        public Lines2()
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



        VertexStore vxs = new VertexStore();
        public override void Init()
        {
            base.Init();

            //
            VertexStore myvxs = new VertexStore();
            PathWriter writer = new PathWriter(myvxs);
            
            writer.MoveTo(100, 0);
            writer.Curve4(
                200, 0,
                200, 200,
                100, 200);

            CurveFlattener flattener = new CurveFlattener();
            VertexStore output = new VertexStore();
            flattener.MakeVxs(myvxs, output);
            this.vxs = output;

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
            aggPainter.StrokeWidth = 2f;



            aggPainter.LineJoin = this.LineJoin;
            aggPainter.LineCap = this.LineCap;
            //
            aggPainter.Draw(vxs);
        }
    }
}