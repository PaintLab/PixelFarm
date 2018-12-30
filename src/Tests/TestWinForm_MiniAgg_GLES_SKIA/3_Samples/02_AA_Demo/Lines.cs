//BSD, 2014-present, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using Mini;

namespace PixelFarm.CpuBlit.Sample_Draw
{

    public abstract class StrokeBasedDemo : DemoBase
    {
        InnerJoin _innerJoin;
        LineJoin _lineJoin;
        LineCap _lineCap;
        protected bool _needUpdate;

        public StrokeBasedDemo()
        {
            _needUpdate = true;
        }
        [DemoConfig]
        public LineJoin LineJoin
        {
            get => _lineJoin;
            set
            {
                _lineJoin = value;
                _needUpdate = true;
            }
        }
        [DemoConfig]
        public LineCap LineCap
        {
            get => _lineCap;
            set
            {
                _lineCap = value;
                _needUpdate = true;
            }
        }

        [DemoConfig]
        public InnerJoin InnerJoin
        {
            get => _innerJoin;
            set
            {
                _innerJoin = value;
                _needUpdate = true;
            }
        }

        [DemoConfig]
        public bool ShowOnlyStrokeOutline { get; set; }

    }

    [Info(OrderCode = "02")]
    [Info("Lines")]
    public class Lines : StrokeBasedDemo
    {

        VertexStore _strokePath;
        VertexStore _orgVxs;
        Stroke _strokeGen = new Stroke(30.0);

        public Lines()
        {
            _orgVxs = new VertexStore();

            using (VectorToolBox.Borrow(_orgVxs, out PathWriter writer))
            {
                int y_offset = 20;
                writer.MoveTo(120, y_offset + 0);
                writer.LineTo(200, y_offset + 100);
                writer.LineTo(120, y_offset + 200);
            }

            //---------------------
            //our agg has built-in stroke-generator tool            
            //so when we call Draw() from the Vxs => a new stroke path is created from the original input vxs

            //---------------------

            //but we can generate stroke before send it
            //by use another stroke gnernator

            UpdateStroke();
        }
        void UpdateStroke()
        {

            _strokeGen.LineJoin = this.LineJoin;
            _strokeGen.LineCap = this.LineCap;
            _strokeGen.InnerJoin = this.InnerJoin;

            _strokePath = new VertexStore();
            _strokeGen.MakeVxs(_orgVxs, _strokePath);

        }


        public override void Draw(Painter p)
        {

            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 30.0f;


            p.LineJoin = this.LineJoin;
            p.LineCap = this.LineCap;
            //

            if (_needUpdate || _strokePath == null)
            {
                UpdateStroke();
            }
            Color c1 = p.FillColor;//save
            p.FillColor = p.StrokeColor;

            if (ShowOnlyStrokeOutline)
            {
                double prevW = p.StrokeWidth;
                p.StrokeWidth = 1;
                p.Draw(_strokePath);
                p.StrokeWidth = prevW;
            }
            else
            {
                p.Fill(_strokePath);
            }
            //restore
            p.FillColor = c1;

        }

    }

    [Info(OrderCode = "02")]
    [Info("Lines2")]
    public class Lines2 : StrokeBasedDemo
    {
        VertexStore _vxs;
        public Lines2()
        {
        }

        public override void Init()
        {
            base.Init();

            using (VxsTemp.Borrow(out var v1, out var v2))
            using (VectorToolBox.Borrow(out CurveFlattener f))
            using (VectorToolBox.Borrow(v1, out PathWriter writer))
            {
                int y_offset = 20;
                writer.MoveTo(100, y_offset + 0);
                writer.Curve4(
                    300, y_offset + 0,
                    300, y_offset + 200,
                    100, y_offset + 200);
                _vxs = f.MakeVxs(v1, v2).CreateTrim();
            }
        }
        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 2f;
            p.LineJoin = this.LineJoin;
            p.LineCap = this.LineCap;
            //
            p.Draw(_vxs);
        }

    }


    public enum RawStrokeMath1Step
    {
        Cap_01,
        Join_012,
        Cap_01_Join_012,
        Cap_21,
        Cap_01_Join_012_Cap21,
        Cap_01_Join_012_Cap21_Join_012,
        Finish
    }

    [Info(OrderCode = "02")]
    [Info("RawStroke")]
    public class RawStrokeMath1 : StrokeBasedDemo
    {
        //This expose how 'Stroke' works
        //not intended to be used directly by general user api


        StrokeMath _strokeMath;
        VertexStore _outputStrokeVxs;
        RawStrokeMath1Step _step;

        Vertex2d _v0;
        Vertex2d _v1;
        Vertex2d _v2;

        public RawStrokeMath1()
        {
            _strokeMath = new StrokeMath();
            LineJoin = LineJoin.Round;
            LineCap = LineCap.Butt;
            _strokeMath.Width = 10;

            float yoffset = 50;

            //assume we have two lines that connect 3 points
            //(120,0+yoffset), (200,100+yoffset), (120,200+yoffset) 
            _v0 = new Vertex2d(120, 0 + yoffset);
            _v1 = new Vertex2d(200, 100 + yoffset);
            _v2 = new Vertex2d(120, 200 + yoffset);


        }
        [DemoConfig]
        public RawStrokeMath1Step Step
        {
            get => _step;
            set
            {
                _step = value;
                _outputStrokeVxs = null;
            }
        }


        void UpdateVxsOutput()
        {
            _outputStrokeVxs = new VertexStore();
            //
            _strokeMath.Width = 50;
            _strokeMath.LineJoin = this.LineJoin;
            _strokeMath.LineCap = this.LineCap;
            _strokeMath.InnerJoin = this.InnerJoin;

            using (VxsTemp.Borrow(out var vxs1))
            {
                switch (Step)
                {
                    case RawStrokeMath1Step.Cap_01:
                        {
                            _strokeMath.CreateCap(vxs1, _v0, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);

                            vxs1.Clear();

                        }
                        break;
                    case RawStrokeMath1Step.Join_012:
                        {
                            //create join
                            _strokeMath.CreateJoin(vxs1, _v0, _v1, _v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);

                            vxs1.Clear();
                        }
                        break;
                    case RawStrokeMath1Step.Cap_01_Join_012:
                        {
                            _strokeMath.CreateCap(vxs1, _v0, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            //
                            _strokeMath.CreateJoin(vxs1, _v0, _v1, _v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                        }
                        break;
                    case RawStrokeMath1Step.Cap_21: //please note the different direction, 
                        {
                            _strokeMath.CreateCap(vxs1, _v2, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                        }
                        break;
                    case RawStrokeMath1Step.Cap_01_Join_012_Cap21: //please note the different direction, 
                        {
                            _strokeMath.CreateCap(vxs1, _v0, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            //
                            _strokeMath.CreateJoin(vxs1, _v0, _v1, _v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();


                            //--- turn back----
                            _strokeMath.CreateCap(vxs1, _v2, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                        }
                        break;
                    case RawStrokeMath1Step.Cap_01_Join_012_Cap21_Join_012:
                        {
                            _strokeMath.CreateCap(vxs1, _v0, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            //
                            _strokeMath.CreateJoin(vxs1, _v0, _v1, _v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();


                            //--- turn back----
                            _strokeMath.CreateCap(vxs1, _v2, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            _strokeMath.CreateJoin(vxs1, _v2, _v1, _v0);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                        }
                        break;
                    case RawStrokeMath1Step.Finish:
                        {
                            _strokeMath.CreateCap(vxs1, _v0, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            //
                            _strokeMath.CreateJoin(vxs1, _v0, _v1, _v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();


                            //--- turn back----
                            _strokeMath.CreateCap(vxs1, _v2, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            _strokeMath.CreateJoin(vxs1, _v2, _v1, _v0);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            _outputStrokeVxs.GetVertex(0, out double first_moveX, out double first_moveY);
                            _outputStrokeVxs.AddLineTo(first_moveX, first_moveY);
                            _outputStrokeVxs.AddCloseFigure();
                        }
                        break;

                }

            }

        }
        public override void Draw(Painter p)
        {

            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;

            if (_needUpdate || _outputStrokeVxs == null)
            {
                UpdateVxsOutput();
            }

            Color c1 = p.FillColor;//save
            p.FillColor = p.StrokeColor;
            double prevW = p.StrokeWidth;
            p.StrokeWidth = 2;
            p.Draw(_outputStrokeVxs);



            p.Line(_v0.x, _v0.y, _v1.x, _v1.y, Color.Red);
            p.Line(_v1.x, _v1.y, _v2.x, _v2.y, Color.Red);


            p.StrokeWidth = prevW;
            //restore
            p.FillColor = c1;

        }

    }




    public enum RawStrokeMath2Choices
    {
        OuterBorder,
        InnerBorder,
        OuterAndInner,

        Auto_OuterAndInner,
    }
    [Info(OrderCode = "02")]
    [Info("RawStroke2")]
    public class RawStrokeMath2 : StrokeBasedDemo
    {

        //This expose how 'Stroke' works
        //not intended to be used directly by general user api 
        StrokeMath _strokeMath;
        VertexStore _outputStrokeVxs;
        RawStrokeMath2Choices _outlineChoice;

        Vertex2d _v0;
        Vertex2d _v1;
        Vertex2d _v2;
        Vertex2d _v3;

        int _drawSteps;

        public RawStrokeMath2()
        {
            _strokeMath = new StrokeMath();
            LineJoin = LineJoin.Round;
            LineCap = LineCap.Butt;
            _strokeMath.Width = 10;


            float yoffset = 50;
            //assume we have two lines that connect 3 points
            //(120,0+yoffset), (200,100+yoffset), (120,200+yoffset) 
            _v0 = new Vertex2d(100, 0 + yoffset);
            _v1 = new Vertex2d(200, 0 + yoffset);
            _v2 = new Vertex2d(200, 200 + yoffset);
            _v3 = new Vertex2d(100, 200 + yoffset);

            _drawSteps = 0;
        }

        [DemoConfig(MaxValue = 15)]
        public int Steps
        {
            get => _drawSteps;
            set
            {
                _drawSteps = value;
                _needUpdate = true;
            }
        }
        [DemoConfig]
        public RawStrokeMath2Choices OutlineChoices
        {
            get => _outlineChoice;
            set
            {
                _outlineChoice = value;
                _outputStrokeVxs = null;
            }
        }
        void UpdateVxsOutput()
        {
            _outputStrokeVxs = new VertexStore();
            //
            _strokeMath.Width = 10;
            _strokeMath.LineJoin = this.LineJoin;
            _strokeMath.LineCap = this.LineCap;
            _strokeMath.InnerJoin = this.InnerJoin;


            int stepCount = 0;

            using (VxsTemp.Borrow(out var vxs1))
            {
                switch (OutlineChoices)
                {
                    case RawStrokeMath2Choices.Auto_OuterAndInner:
                        {
                            using (VxsTemp.Borrow(out var vxs3, out var vxs4))
                            using (VectorToolBox.Borrow(vxs3, out PathWriter pw))
                            using (VectorToolBox.Borrow(out Stroke stroke))
                            {
                                stroke.Width = 10;
                                stroke.LineCap = this.LineCap;
                                stroke.LineJoin = this.LineJoin;

                                //pw.MoveTo(_v0.x, _v0.y);
                                //pw.LineTo(_v1.x, _v1.y);
                                //pw.LineTo(_v2.x, _v2.y);
                                //pw.LineTo(_v3.x, _v3.y);
                                //pw.CloseFigure();

                                pw.MoveTo(_v0.x, _v0.y);
                                pw.LineTo(_v3.x, _v3.y);
                                pw.LineTo(_v2.x, _v2.y);
                                pw.LineTo(_v1.x, _v1.y);
                                pw.CloseFigure();


                                _outputStrokeVxs = vxs3.CreateTrim();
                                //vxs3.AddMoveTo(_v0.x, _v0.y);
                                //vxs3.AddLineTo(_v1.x, _v1.y);
                                //vxs3.AddLineTo(_v2.x, _v2.y);
                                //vxs3.AddLineTo(_v3.x, _v3.y);
                                //vxs3.AddLineTo(_v0.x, _v0.y);
                                //vxs3.AddCloseFigure();

                                //stroke.MakeVxs(vxs3, vxs4);
                                //_outputStrokeVxs = vxs4.CreateTrim();

                            }

                        }
                        break;
                    case RawStrokeMath2Choices.OuterBorder:
                        {


                            _strokeMath.CreateJoin(vxs1, _v0, _v1, _v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                            _strokeMath.CreateJoin(vxs1, _v1, _v2, _v3);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                            _strokeMath.CreateJoin(vxs1, _v2, _v3, _v0);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                            _strokeMath.CreateJoin(vxs1, _v3, _v0, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                            _outputStrokeVxs.GetVertex(0, out double first_moveX, out double first_moveY);
                            _outputStrokeVxs.AddLineTo(first_moveX, first_moveY);
                            _outputStrokeVxs.AddCloseFigure();
                        }
                        break;
                    case RawStrokeMath2Choices.InnerBorder:
                        {
                            _strokeMath.CreateJoin(vxs1, _v1, _v0, _v3);
                            vxs1.GetVertex(0, out double first_moveX, out double first_moveY);
                            _outputStrokeVxs.AddMoveTo(first_moveX, first_moveY);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                            _strokeMath.CreateJoin(vxs1, _v0, _v3, _v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                            _strokeMath.CreateJoin(vxs1, _v3, _v2, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                            _strokeMath.CreateJoin(vxs1, _v2, _v1, _v0);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                            _outputStrokeVxs.AddCloseFigure();
                        }
                        break;
                    case RawStrokeMath2Choices.OuterAndInner:
                        {
                            //outer
                            _strokeMath.CreateJoin(vxs1, _v0, _v1, _v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                            _strokeMath.CreateJoin(vxs1, _v1, _v2, _v3);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------


                            _strokeMath.CreateJoin(vxs1, _v2, _v3, _v0);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------


                            _strokeMath.CreateJoin(vxs1, _v3, _v0, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------


                            _outputStrokeVxs.GetVertex(0, out double first_moveX, out double first_moveY);
                            _outputStrokeVxs.AddLineTo(first_moveX, first_moveY);
                            _outputStrokeVxs.AddCloseFigure();
                            stepCount++; if (stepCount > Steps) break; //demo only

                            //----------------------------------------------------
                            //inner  


                            _strokeMath.CreateJoin(vxs1, _v2, _v1, _v0);
                            vxs1.GetVertex(0, out first_moveX, out first_moveY);
                            _outputStrokeVxs.AddMoveTo(first_moveX, first_moveY);
                            //_outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                                                                       //---------


                            _strokeMath.CreateJoin(vxs1, _v1, _v0, _v3);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------


                            _strokeMath.CreateJoin(vxs1, _v0, _v3, _v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------


                            _strokeMath.CreateJoin(vxs1, _v3, _v2, _v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                            _outputStrokeVxs.AddLineTo(first_moveX, first_moveY);
                            _outputStrokeVxs.AddCloseFigure();
                            stepCount++; if (stepCount > Steps) break; //demo only
                            //---------

                        }
                        break;
                }

            }

        }
        public override void Draw(Painter p)
        {

            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;

            if (_needUpdate || _outputStrokeVxs == null)
            {
                UpdateVxsOutput();
            }

            Color c1 = p.FillColor;//save
            double prevW = p.StrokeWidth;
            p.StrokeWidth = 2;

            p.Line(_v0.x, _v0.y, _v1.x, _v1.y, Color.Red);
            p.Line(_v1.x, _v1.y, _v2.x, _v2.y, Color.Red);
            p.Line(_v2.x, _v2.y, _v3.x, _v3.y, Color.Red);
            p.Line(_v3.x, _v3.y, _v0.x, _v0.y, Color.Red);


            p.FillColor = p.StrokeColor;

            p.Draw(_outputStrokeVxs);

            p.StrokeWidth = prevW;
            //restore
            p.FillColor = c1;

        }

    }


}