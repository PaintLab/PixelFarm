//BSD, 2014-2017, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.


using PixelFarm.Agg.VertexSource;
using Mini;

namespace PixelFarm.Agg.Sample_Draw
{
    public enum DrawStrokeSample
    {
        A,
        B,
        C,
        D,
        E,

    }

    [Info(OrderCode = "02")]
    [Info("StrokeGenSample")]
    public class StrokeGenSample : DemoBase
    {
        public StrokeGenSample()
        {

            DrawStrokeSample = DrawStrokeSample.E;
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
                case DrawStrokeSample.D:
                    DrawD(aggPainter);
                    break;
                case DrawStrokeSample.E:
                    DrawE(aggPainter);
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

            writer.MoveTo(30, 10);
            writer.LineTo(60, 10);


            writer.MoveTo(10, 100);
            writer.LineTo(10, 50);

            aggPainter.Draw(vxs);

            //writer.MoveTo(100, 100);
            //writer.LineTo(20, 200);
            //aggPainter.LineJoin = this.LineJoin;
            //aggPainter.LineCap = this.LineCap;
            //
            //----------------------------------------------------
            ////create a dash line 
            //VertexStore dashOutputVxs = new VertexStore();
            //LineWalker dashGenLineWalker = new LineWalker();
            ////***
            ////you can customize what happend with the line segment
            //dashGenLineWalker.AddMark(10, LineWalkDashStyle.Solid);
            //dashGenLineWalker.AddMark(10, LineWalkDashStyle.Blank);
            ////dashGenLineWalker.AddMark(2, LineWalkDashStyle.Solid);
            ////dashGenLineWalker.AddMark(2, LineWalkDashStyle.Blank);
            //dashGenLineWalker.Walk(vxs, dashOutputVxs);
            ////----------------------------------------------------



            //aggPainter.Draw(dashOutputVxs);
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


        }


        LineDashGenerator lineDashGen;
        void DrawC(PixelFarm.Agg.AggCanvasPainter aggPainter)
        {

            aggPainter.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            aggPainter.StrokeColor = PixelFarm.Drawing.Color.Black;
            aggPainter.StrokeWidth = 2.0f;

            if (lineDashGen == null)
            {
                //create line dash gen
                lineDashGen = new LineDashGenerator();
                lineDashGen.SetDashPattern(4, 2, 2, 2);
            }

            aggPainter.LineDashGen = lineDashGen;
            //
            VertexStore vxs = new VertexStore();
            PathWriter writer = new PathWriter(vxs);

            writer.MoveTo(20, 10);
            writer.LineTo(60, 10);
            writer.LineTo(20, 200);
            writer.LineTo(20, 0);



            //writer.MoveTo(20, 100);
            //writer.LineTo(20, 15);
            //writer.CloseFigure();

            aggPainter.Draw(vxs);
            aggPainter.LineDashGen = null;
        }
        void DrawD(CanvasPainter painter)
        {
            PathWriter ps = new PathWriter();
            painter.Clear(PixelFarm.Drawing.Color.White);
            painter.StrokeColor = PixelFarm.Drawing.Color.Red;
            //p.Line(10, 10, 50, 10);
            //p.Line(50, 10, 50, 50);
            //p.Line(50, 50, 10, 50);
            //p.Line(50, 10, 10, 10);

            ps.Clear();
            ps.MoveTo(10, 10);
            ps.LineTo(50, 10);
            ps.LineTo(50, 50);
            ps.LineTo(10, 50);
            ps.CloseFigure();
            //
            //ps.MoveTo(15, 15);
            //ps.LineTo(15, 45);
            //ps.LineTo(45, 45);
            //ps.LineTo(45, 15);
            //ps.CloseFigure();
            //
            //p.Fill(ps.Vxs, PixelFarm.Drawing.Color.Black);
            painter.Draw(ps.Vxs, PixelFarm.Drawing.Color.Red);
        }
        void DrawE(CanvasPainter painter)
        {
            PathWriter ps = new PathWriter();
            painter.Clear(PixelFarm.Drawing.Color.White);
            painter.StrokeColor = PixelFarm.Drawing.Color.Red;
            StrokeGen2 gen2 = new StrokeGen2();

            //p.Line(10, 10, 50, 10);
            //p.Line(50, 10, 50, 50);
            //p.Line(50, 50, 10, 50);
            //p.Line(50, 10, 10, 10);

            ps.Clear();
            ps.MoveTo(10, 10);
            ps.LineTo(50, 10);
            ps.LineTo(50, 50);
            ps.LineTo(20, 50);
            ps.LineTo(80, 80);

            //ps.LineTo(10, 80);
            //ps.CloseFigure();

            //p.Fill(ps.Vxs, PixelFarm.Drawing.Color.Black);
            VertexStore output = new VertexStore();

            gen2.SetEdgeWidth(7f, 7f);
            //gen2.SetEdgeWidth(2f, 2f);
            gen2.Generate(ps.Vxs, output);
            painter.Fill(output, PixelFarm.Drawing.Color.Red);

            //painter.StrokeWidth = 1f;
            //painter.Draw(ps.Vxs, PixelFarm.Drawing.Color.Red);

        }
    }
}