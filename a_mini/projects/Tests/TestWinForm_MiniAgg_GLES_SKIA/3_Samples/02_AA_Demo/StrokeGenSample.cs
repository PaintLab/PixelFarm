//BSD, 2014-2017, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;
using PixelFarm.Agg.Transform;

using Mini;

namespace PixelFarm.Agg.Sample_Draw
{
    public enum DrawStrokeSample
    {
        A,
        B,
        C,
    }

    [Info(OrderCode = "02")]
    [Info("StrokeGenSample")]
    public class StrokeGenSample : DemoBase
    {
        public StrokeGenSample()
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
        [DemoConfig]
        public DrawStrokeSample DrawStrokeSample
        {
            get; set;
        }
        public override void Draw(CanvasPainter p)
        {
            var aggPainter = p as PixelFarm.Agg.AggCanvasPainter;
            if (aggPainter == null)
            {
                return;
            }

            switch (DrawStrokeSample)
            {
                default: throw new System.NotSupportedException();
                case DrawStrokeSample.A:
                    DrawA(aggPainter);
                    break;
                case DrawStrokeSample.B:
                    DrawB(aggPainter);
                    break;
                case DrawStrokeSample.C:
                    DrawC(aggPainter);
                    break;
            }
        }
        void DrawB(PixelFarm.Agg.AggCanvasPainter aggPainter)
        {

            aggPainter.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            aggPainter.StrokeColor = PixelFarm.Drawing.Color.Black;
            aggPainter.StrokeWidth = 2.0f;

            //
            VertexStore vxs = new VertexStore();
            PathWriter writer = new PathWriter(vxs);

            writer.MoveTo(20, 10);
            writer.LineTo(60, 10);
            writer.LineTo(20, 200);
            writer.CloseFigure();

            //writer.MoveTo(100, 100);
            //writer.LineTo(20, 200);
            //aggPainter.LineJoin = this.LineJoin;
            //aggPainter.LineCap = this.LineCap;
            //
            //----------------------------------------------------
            //create a dash line 
            VertexStore newvxs = new VertexStore();
            LineWalker dashGenLineWalker = new LineWalker();
            //***
            //you can customize what happend with the line segment
            dashGenLineWalker.AddMark(10, (outputVxs, cmd, x, y) =>
            {
                //solid               
                switch (cmd)
                {
                    case VertexCmd.MoveTo:
                        outputVxs.AddMoveTo(x, y);
                        break;
                    case VertexCmd.LineTo:
                        outputVxs.AddLineTo(x, y);
                        break;
                }
            });
            dashGenLineWalker.AddMark(10, (outputVxs, cmd, x, y) =>
            {
                //whitespace, do nothing
            });

            dashGenLineWalker.Walk(vxs, newvxs);
            //aggPainter.Draw(vxs);

            //test drawline
            int n = newvxs.Count;
            double px = 0, py = 0;

            for (int i = 0; i < n; ++i)
            {
                double x, y;
                VertexCmd cmd = newvxs.GetVertex(i, out x, out y);

                switch (cmd)
                {
                    case VertexCmd.MoveTo:
                        px = x;
                        py = y;

                        break;
                    case VertexCmd.LineTo:
                        aggPainter.Line(px, py, x, y);

                        break;
                }
                px = x;
                py = y;
            }
            //aggPainter.Draw(newvxs);
        }
        void DrawA(PixelFarm.Agg.AggCanvasPainter aggPainter)
        {

            aggPainter.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            aggPainter.StrokeColor = PixelFarm.Drawing.Color.Black;
            aggPainter.StrokeWidth = 2.0f;
            aggPainter.FillColor = PixelFarm.Drawing.Color.Black;
            //
            VertexStore vxs = new VertexStore();
            PathWriter writer = new PathWriter(vxs);

            writer.MoveTo(20, 10);
            writer.LineTo(60, 10);
            writer.LineTo(20, 200);
            writer.CloseFigure();

            //writer.MoveTo(100, 100);
            //writer.LineTo(20, 200);
            //aggPainter.LineJoin = this.LineJoin;
            //aggPainter.LineCap = this.LineCap;
            //
            //----------------------------------------------------
            //create a dash line 
            VertexStore newvxs = new VertexStore();
            LineWalker dashGenLineWalker = new LineWalker();
            //***
            //you can customize what happend with the line segment
            dashGenLineWalker.AddMark(10, LineWalkDashStyle.Solid);
            dashGenLineWalker.AddMark(10, LineWalkDashStyle.Blank);
            //dashGenLineWalker.AddMark(2, LineWalkDashStyle.Solid);
            //dashGenLineWalker.AddMark(2, LineWalkDashStyle.Blank);

            dashGenLineWalker.Walk(vxs, newvxs);

            ////test drawline
            //int n = newvxs.Count;
            //double px = 0, py = 0;
            //for (int i = 0; i < n; ++i)
            //{
            //    double x, y;
            //    VertexCmd cmd = newvxs.GetVertex(i, out x, out y);
            //    switch (cmd)
            //    {
            //        case VertexCmd.MoveTo:
            //            px = x;
            //            py = y;

            //            break;
            //        case VertexCmd.LineTo:
            //            aggPainter.Line(px, py, x, y);

            //            break;
            //    }
            //    px = x;
            //    py = y;
            //}

            aggPainter.Draw(newvxs);
        }
        void DrawC(PixelFarm.Agg.AggCanvasPainter aggPainter)
        {

            aggPainter.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            aggPainter.StrokeColor = PixelFarm.Drawing.Color.Black;
            aggPainter.StrokeWidth = 2.0f;
            //aggPainter.SetLineDashPattern(1);
            //
            VertexStore vxs = new VertexStore();
            PathWriter writer = new PathWriter(vxs);

            writer.MoveTo(20, 10);
            writer.LineTo(60, 10);
            writer.LineTo(20, 200);
            writer.CloseFigure();

            aggPainter.Draw(vxs);
            aggPainter.SetLineDashPattern(0);
        }
    }
}