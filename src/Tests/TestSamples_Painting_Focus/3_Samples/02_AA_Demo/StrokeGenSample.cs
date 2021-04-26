//BSD, 2014-present, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using System.Collections.Generic;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using Mini;

namespace PixelFarm.CpuBlit.Sample_Draw
{
    public enum DrawStrokeSample
    {
        A,
        B,
        B1,
        C,
        C1,

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
                case DrawStrokeSample.B1:
                    DrawB1(p);
                    break;
                case DrawStrokeSample.C:
                    DrawC(p);
                    break;
                case DrawStrokeSample.C1:
                    DrawC1(p);
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
            using (Tools.BorrowShapeBuilder(out var b))
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

        class ExampleVxsLineDashSegmentWalkerOutput : ILineSegmentWalkerOutput
        {
            VertexStore _vxs;
            public ExampleVxsLineDashSegmentWalkerOutput()
            {
            }
            public void SetOutput(VertexStore vsx)
            {
                _vxs = vsx;
            }
            public void AddLineTo(LineWalkerMark maker, double x, double y) => _vxs.AddLineTo(x, y);
            public void AddMoveTo(LineWalkerMark maker, double x, double y) => _vxs.AddMoveTo(x, y);
        }

        class ExampleVxsLineDash2Walker : ILineSegmentWalkerOutput
        {
            //example
            double _latestX;
            double _latestY;
            Painter _p;
            VertexStore _vxs;

            public ExampleVxsLineDash2Walker()
            {
                using (Tools.BorrowVxs(out var v1))
                {
                    v1 = new VertexStore();
                    v1.AddMoveTo(0, -3);
                    v1.AddLineTo(4, 0);
                    v1.AddLineTo(0, 3);
                    v1.AddCloseFigure();
                    _vxs = v1.CreateTrim();
                }

            }
            public void SetPainter(Painter p)
            {
                _p = p;
            }
            public void AddLineTo(LineWalkerMark marker, double x, double y)
            {
                double cx = (x + _latestX) / 2;
                double cy = (y + _latestY) / 2;

                if (marker.Index == 0)
                {

                    //_p.FillRect(cx, cy, 4, 4, Color.Red);
                    Color prev = _p.FillColor;
                    _p.SetOrigin((float)cx, (float)cy);
                    _p.FillColor = Color.Red;

                    using (Tools.BorrowVxs(out var v1))
                    {
                        _vxs.RotateRadToNewVxs(System.Math.Atan2(y - _latestY, x - _latestX), v1);
                        _p.Fill(v1);
                    } 
                    _p.FillColor = prev;
                    _p.SetOrigin(0, 0);//restore
                }
                else
                {
                    _p.FillRect(cx, cy, 2, 2, Color.Blue);
                }


                _latestX = x;
                _latestY = y;
            }
            public void AddMoveTo(LineWalkerMark marker, double x, double y)
            {
                _latestX = x;
                _latestY = y;
            }
        }


        void DrawB(Painter p)
        {

            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 2.0f;


            using (Tools.BorrowVxs(out var v1, out var v2))
            using (Tools.BorrowPathWriter(v1, out var writer))
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


                _dashGenLineWalker.Reset();  //clear previous markers
                //***
                //you can customize what happend with the line segment
                _dashGenLineWalker.AddMark(10, (output, marker, cmd, x, y) =>
                {
                    //solid               
                    switch (cmd)
                    {
                        case VertexCmd.MoveTo:
                            output.AddMoveTo(marker, x, y);
                            break;
                        case VertexCmd.LineTo:
                            output.AddLineTo(marker, x, y);
                            break;
                    }
                });
                _dashGenLineWalker.AddMark(10, (output, marker, cmd, x, y) =>
                {
                    //whitespace, do nothing
                });


                //-------------------------------------------
                //then generate dash by walking along v1
                ExampleVxsLineDashSegmentWalkerOutput walkerOutput = new ExampleVxsLineDashSegmentWalkerOutput();
                walkerOutput.SetOutput(v2);
                _dashGenLineWalker.Walk(v1, walkerOutput);
                //test drawline
                int n = v2.Count;
                double px = 0, py = 0;

                for (int i = 0; i < n; ++i)
                {
                    VertexCmd cmd = v2.GetVertex(i, out double x, out double y);
                    switch (cmd)
                    {
                        case VertexCmd.MoveTo:
                            break;
                        case VertexCmd.LineTo:
                            p.DrawLine(px, py, x, y);
                            break;
                    }
                    px = x;
                    py = y;
                }
                //-------------------------------------------
            }

        }

        void DrawB1(Painter p)
        {

            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 2.0f;


            using (Tools.BorrowVxs(out var v1, out var v2))
            using (Tools.BorrowPathWriter(v1, out PathWriter writer))
            {
                writer.MoveTo(20, 10);
                writer.LineTo(60, 10);
                writer.LineTo(20, 200);
                writer.CloseFigure();
                //
                _dashGenLineWalker.Reset();  //clear previous markers
                                             //***
                                             //you can customize what happend with the line segment

                _dashGenLineWalker.AddMark(3, (output, marker, cmd, x, y) =>
                {
                    //solid               
                    switch (cmd)
                    {
                        case VertexCmd.MoveTo:
                            output.AddMoveTo(marker, x, y);
                            break;
                        case VertexCmd.LineTo:
                            output.AddLineTo(marker, x, y);
                            break;
                    }
                });
                _dashGenLineWalker.AddMark(3, (output, marker, cmd, x, y) =>
                {
                    //whitespace, do nothing
                });


                //-------------------------------------------
                //then generate dash by walking along v1
                ExampleVxsLineDashSegmentWalkerOutput walkerOutput = new ExampleVxsLineDashSegmentWalkerOutput();
                walkerOutput.SetOutput(v2);
                _dashGenLineWalker.Walk(v1, walkerOutput);
                //test drawline
                int n = v2.Count;
                double px = 0, py = 0;

                for (int i = 0; i < n; ++i)
                {
                    VertexCmd cmd = v2.GetVertex(i, out double x, out double y);
                    switch (cmd)
                    {
                        case VertexCmd.MoveTo:
                            break;
                        case VertexCmd.LineTo:
                            {
                                //instead of drawing a line 
                                //write write other shape
                                //along the line (px,py)=> (x,y)
                                double cx = (px + x) / 2;
                                double cy = (py + y) / 2;
                                p.FillRect(cx, cy, 3, 3, Color.Red);
                            }
                            break;
                    }
                    px = x;
                    py = y;
                }
                //-------------------------------------------
            }
        }
        void DrawC(Painter p)
        {
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

            //
            using (Tools.BorrowShapeBuilder(out var b))
            {
                IDashGenerator tmp = p.LineDashGen;
                p.LineDashGen = _lineDashGen;
                //b.MoveTo(20.5f, 10.5f);
                //b.LineTo(60.5f, 10.5f);
                //b.LineTo(20.5f, 200.5f);

                b.MoveTo(20f, 10f);
                b.LineTo(60f, 10f);
                b.LineTo(20f, 200f);

                b.CloseFigure();

                p.Draw(b.CurrentSharedVxs);

                p.LineDashGen = tmp;
            }

        }
        void DrawC1(Painter p)
        {
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
            //
            using (Tools.BorrowShapeBuilder(out var b))
            {
                IDashGenerator tmp = p.LineDashGen;
                p.LineDashGen = _lineDashGen;
                //b.MoveTo(20.5f, 10.5f);
                //b.LineTo(60.5f, 10.5f);
                //b.LineTo(20.5f, 200.5f);

                b.MoveTo(20f, 10f);
                b.LineTo(60f, 10f);
                b.LineTo(20f, 200f);

                b.CloseFigure();

                //-----------------------------------
                //in this example, we draw line pattern along dash output       

                var exampleWalker = new ExampleVxsLineDash2Walker();
                exampleWalker.SetPainter(p);
                _lineDashGen.CreateDash(b.CurrentSharedVxs, exampleWalker);

            }

        }
        void DrawD(Painter p)
        {
            p.Clear(PixelFarm.Drawing.Color.White);
            p.StrokeColor = PixelFarm.Drawing.Color.Red;

            using (Tools.BorrowShapeBuilder(out var b))
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

            using (Tools.BorrowVxs(out var v1, out var v2))
            using (Tools.BorrowPathWriter(v1, out var w))
            {
                w.Clear();
                w.MoveTo(150, 10);
                w.LineTo(110, 10);
                w.LineTo(150, 20);


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