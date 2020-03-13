//BSD, 2014-present, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.


using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using Mini;

namespace PixelFarm.CpuBlit.Sample_Draw
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
        LineWalker _dashGenLineWalker = new LineWalker();
        LineDashGenerator _lineDashGen;

        public StrokeGenSample()
        {
            DrawStrokeSample = DrawStrokeSample.E;
        }
        [DemoConfig]
        public LineJoin LineJoin { get; set; }
        [DemoConfig]
        public LineCap LineCap { get; set; }
        [DemoConfig]
        public DrawStrokeSample DrawStrokeSample { get; set; }

        public override void Draw(Painter p)
        {

            switch (DrawStrokeSample)
            {
                default: throw new System.NotSupportedException();
                case DrawStrokeSample.A:
                    DrawA(p);
                    break;
                case DrawStrokeSample.B:
                    DrawB(p);
                    break;
                case DrawStrokeSample.C:

                    DrawC(p);

                    break;
                case DrawStrokeSample.D:
                    DrawD(p);
                    break;
                case DrawStrokeSample.E:
                    DrawE(p);
                    break;
            }
        }


        void DrawA(Painter p)
        {

            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 2.0f;
            p.FillColor = PixelFarm.Drawing.Color.Black;
            //
            using (VectorToolBox.Borrow(out ShapeBuilder b))
            {
                b.MoveTo(30, 10);
                b.LineTo(60, 10);
                b.MoveTo(10, 100);
                b.LineTo(10, 50);
                p.Draw(b.CurrentSharedVxs);
            }



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

        void DrawB(Painter p)
        {

            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 2.0f;


            using (VxsTemp.Borrow(out var v1, out var v2))
            using (VectorToolBox.Borrow(v1, out PathWriter writer))
            {
                writer.MoveTo(20, 10);
                writer.LineTo(60, 10);
                writer.LineTo(20, 200);
                writer.CloseFigure();

                //aggPainter.LineJoin = this.LineJoin;
                //aggPainter.LineCap = this.LineCap;
                //
                //----------------------------------------------------
                //create a dash line 


                _dashGenLineWalker.ClearMarks();  //clear previous markers
                //***
                //you can customize what happend with the line segment
                _dashGenLineWalker.AddMark(10, (outputVxs, cmd, x, y) =>
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
                _dashGenLineWalker.AddMark(10, (outputVxs, cmd, x, y) =>
                {
                    //whitespace, do nothing
                });

                //then generate dash by walking along v1
                _dashGenLineWalker.Walk(v1, v2);
                //aggPainter.Draw(vxs);

                //test drawline
                int n = v2.Count;
                double px = 0, py = 0;

                for (int i = 0; i < n; ++i)
                {
                    VertexCmd cmd = v2.GetVertex(i, out double x, out double y);
                    switch (cmd)
                    {
                        case VertexCmd.MoveTo:
                            px = x;
                            py = y;

                            break;
                        case VertexCmd.LineTo:
                            p.DrawLine(px, py, x, y);

                            break;
                    }
                    px = x;
                    py = y;
                }
            }
            //aggPainter.Draw(newvxs);
        }
        void DrawC(Painter p)
        {
            if (!(p is AggPainter aggPainter)) return;//temp


            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 2.0f;

            if (_lineDashGen == null)
            {
                //create line dash gen
                _lineDashGen = new LineDashGenerator();
                _lineDashGen.SetDashPattern(4, 2, 2, 2);
            }

            aggPainter.LineDashGen = _lineDashGen;
            //
            using (VectorToolBox.Borrow(out ShapeBuilder b))
            {

                b.MoveTo(20, 10);
                b.LineTo(60, 10);
                b.LineTo(20, 200);
                b.CloseFigure();
                p.Draw(b.CurrentSharedVxs);
            }

            aggPainter.LineDashGen = null;

        }
        void DrawD(Painter p)
        {
            p.Clear(PixelFarm.Drawing.Color.White);
            p.StrokeColor = PixelFarm.Drawing.Color.Red;

            using (VectorToolBox.Borrow(out ShapeBuilder b))
            {
                b.MoveTo(10, 10);
                b.LineTo(50, 10);
                b.LineTo(50, 50);
                b.LineTo(10, 50);
                b.CloseFigure();
                p.Draw(b.CurrentSharedVxs, Color.Red);
            }


        }
        void DrawE(Painter p)
        {

            p.Clear(PixelFarm.Drawing.Color.White);
            p.StrokeColor = PixelFarm.Drawing.Color.Red;

            using (VxsTemp.Borrow(out var v1, out var v2))
            using (VectorToolBox.Borrow(v1, out PathWriter ps))
            {
                ps.Clear();
                ps.MoveTo(150, 10);
                ps.LineTo(110, 10);
                ps.LineTo(150, 20);


                StrokeGen2 gen2 = new StrokeGen2(); //under construction!
                gen2.LineCapStyle = LineCap.Butt;
                gen2.LineJoinStyle = LineJoin.Miter;
                gen2.HalfStrokeWidth = 7;//  
                gen2.Generate(v1, v2);
                //-----------------------------------------------------
                p.Fill(v2, PixelFarm.Drawing.Color.Red);
                p.StrokeWidth = 1f;
                p.Draw(v1, PixelFarm.Drawing.Color.Black);

            }

        }
    }
}