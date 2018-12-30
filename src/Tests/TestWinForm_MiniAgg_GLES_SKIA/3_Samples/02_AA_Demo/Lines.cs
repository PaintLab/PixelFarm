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
    [Info(OrderCode = "02")]
    [Info("Lines")]
    public class Lines : DemoBase
    {

        VertexStore _strokePath;
        VertexStore _orgVxs;
        Stroke _strokeGen = new Stroke(30.0);
        LineJoin _lineJoin;
        LineCap _lineCap;

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
            if (_strokePath == null)
            {
                _strokeGen.LineJoin = this.LineJoin;
                _strokeGen.LineCap = this.LineCap;

                _strokePath = new VertexStore();
                _strokeGen.MakeVxs(_orgVxs, _strokePath);
            }
        }

        [DemoConfig]
        public LineJoin LineJoin
        {
            get => _lineJoin;
            set
            {
                _lineJoin = value;
                _strokePath = null;
            }
        }
        [DemoConfig]
        public LineCap LineCap
        {
            get => _lineCap;
            set
            {
                _lineCap = value;
                _strokePath = null;
            }
        }
        [DemoConfig]
        public bool ShowOnlyStrokeOutline { get; set; }

        public override void Draw(Painter p)
        {

            p.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            p.StrokeColor = PixelFarm.Drawing.Color.Black;
            p.StrokeWidth = 30.0f;


            p.LineJoin = this.LineJoin;
            p.LineCap = this.LineCap;
            //

            UpdateStroke();
            if (_strokePath != null)
            {
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
            else
            {
                p.Draw(_orgVxs);
            }
        }

    }

    [Info(OrderCode = "02")]
    [Info("Lines2")]
    public class Lines2 : DemoBase
    {
        VertexStore _vxs;
        public Lines2()
        {
        }
        [DemoConfig]
        public LineJoin LineJoin { get; set; }
        [DemoConfig]
        public LineCap LineCap { get; set; }
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


    public enum RawStrokeMathStep
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
    public class RawStrokeMath : DemoBase
    {
        LineJoin _lineJoin;
        LineCap _lineCap;
        StrokeMath _strokeMath;


        VertexStore _outputStrokeVxs;
        RawStrokeMathStep _step;
        public RawStrokeMath()
        {
            _strokeMath = new StrokeMath();
            LineJoin = LineJoin.Round;
            LineCap = LineCap.Butt;
            _strokeMath.Width = 10;
        }
        [DemoConfig]
        public RawStrokeMathStep Step
        {
            get => _step;
            set
            {
                _step = value;
                _outputStrokeVxs = null;
            }
        }

        [DemoConfig]
        public LineJoin LineJoin
        {
            get => _lineJoin;
            set
            {
                _lineJoin = value;
                _outputStrokeVxs = null;
            }
        }
        [DemoConfig]
        public LineCap LineCap
        {
            get => _lineCap;
            set
            {
                _lineCap = value;
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

            float yoffset = 20;

            //assume we have two lines that connect 3 points
            //(120,0+yoffset), (200,100+yoffset), (120,200+yoffset) 
            Vertex2d v0 = new Vertex2d(120, 0 + yoffset);
            Vertex2d v1 = new Vertex2d(200, 100 + yoffset);
            Vertex2d v2 = new Vertex2d(120, 200 + yoffset);

            using (VxsTemp.Borrow(out var vxs1))
            {
                switch (Step)
                {
                    case RawStrokeMathStep.Cap_01:
                        {
                            _strokeMath.CreateCap(vxs1, v0, v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);

                            vxs1.Clear();

                        }
                        break;
                    case RawStrokeMathStep.Join_012:
                        {
                            //create join
                            _strokeMath.CreateJoin(vxs1, v0, v1, v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);

                            vxs1.Clear();
                        }
                        break;
                    case RawStrokeMathStep.Cap_01_Join_012:
                        {
                            _strokeMath.CreateCap(vxs1, v0, v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            //
                            _strokeMath.CreateJoin(vxs1, v0, v1, v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                        }
                        break;
                    case RawStrokeMathStep.Cap_21: //please note the different direction, 
                        {
                            _strokeMath.CreateCap(vxs1, v2, v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                        }
                        break;
                    case RawStrokeMathStep.Cap_01_Join_012_Cap21: //please note the different direction, 
                        {
                            _strokeMath.CreateCap(vxs1, v0, v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            //
                            _strokeMath.CreateJoin(vxs1, v0, v1, v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();


                            //--- turn back----
                            _strokeMath.CreateCap(vxs1, v2, v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();
                        }
                        break;
                    case RawStrokeMathStep.Cap_01_Join_012_Cap21_Join_012:
                        {
                            _strokeMath.CreateCap(vxs1, v0, v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            //
                            _strokeMath.CreateJoin(vxs1, v0, v1, v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();


                            //--- turn back----
                            _strokeMath.CreateCap(vxs1, v2, v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            _strokeMath.CreateJoin(vxs1, v2, v1, v0);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                        }
                        break;
                    case RawStrokeMathStep.Finish:
                        {
                            _strokeMath.CreateCap(vxs1, v0, v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            //
                            _strokeMath.CreateJoin(vxs1, v0, v1, v2);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();


                            //--- turn back----
                            _strokeMath.CreateCap(vxs1, v2, v1);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            _strokeMath.CreateJoin(vxs1, v2, v1, v0);
                            _outputStrokeVxs.AppendVertexStore(vxs1);
                            vxs1.Clear();

                            _outputStrokeVxs.GetVertex(0, out double first_moveX, out double first_moveY);
                            _outputStrokeVxs.AddMoveTo(first_moveX, first_moveY);
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

            if (_outputStrokeVxs == null)
            {
                UpdateVxsOutput();
            }

            Color c1 = p.FillColor;//save
            p.FillColor = p.StrokeColor;
            double prevW = p.StrokeWidth;
            p.StrokeWidth = 2;
            p.Draw(_outputStrokeVxs);
            p.StrokeWidth = prevW;
            //restore
            p.FillColor = c1;

        }

    }

}