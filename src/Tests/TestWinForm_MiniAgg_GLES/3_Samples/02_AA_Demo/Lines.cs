//BSD, 2014-present, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using System;
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

            using (Tools.BorrowShapeBuilder(out var b))
            {
                int y_offset = 20;

                b.MoveTo(100, y_offset + 0);
                b.Curve4To(
                    300, y_offset + 0,
                    300, y_offset + 200,
                    100, y_offset + 200);
                b.Flatten();

                _vxs = b.CurrentSharedVxs.CreateTrim();
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
            //this example show each component of line stroke
            //we scale line-width to 50px 

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
        EachBorders,

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
                InvalidateGraphics();
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
                InvalidateGraphics();
            }
        }


        delegate void MySimpleAction();

        void UpdateVxsOutput()
        {

            //demo only, demonstrate stroke detail
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
                            using (Tools.BorrowStroke(out var stroke))
                            using (Tools.BorrowShapeBuilder(out var b))
                            {

                                stroke.Width = 10;
                                stroke.LineCap = this.LineCap;
                                stroke.LineJoin = this.LineJoin;

                                b.MoveTo(_v0.x, _v0.y);
                                b.LineTo(_v3.x, _v3.y);
                                b.LineTo(_v2.x, _v2.y);
                                b.LineTo(_v1.x, _v1.y);
                                b.CloseFigure();

                                b.Stroke(stroke);

                                _outputStrokeVxs = b.CurrentSharedVxs.CreateTrim();
                            }
                        }
                        break;
                    case RawStrokeMath2Choices.OuterBorder:
                        {

                            //demo only, show step-by-step

                            MySimpleAction[] actions = new MySimpleAction[]
                            {
                                //create join
                                //since we want to see data inside each step, so 
                                //we generate data into vxs1 first and the copy it to output_vxs
                                //(but you can generate data directly to _outputStrokeVxs

                                new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v0, _v1, _v2);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                                //
                               new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v1, _v2, _v3);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v2, _v3, _v0);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v3, _v0, _v1);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                                new MySimpleAction(()=>{
                                    _outputStrokeVxs.GetVertex(0, out double first_moveX, out double first_moveY);
                                    _outputStrokeVxs.AddLineTo(first_moveX, first_moveY);
                                    _outputStrokeVxs.AddCloseFigure();
                                }),
                            };

                            for (int i = stepCount; i < actions.Length && i < Steps; ++i)
                            {
                                actions[i]();
                            }
                        }
                        break;
                    case RawStrokeMath2Choices.InnerBorder:
                        {


                            MySimpleAction[] actions = new MySimpleAction[]
                            {
                                //create join
                                //since we want to see data inside each step, so 
                                //we generate data into vxs1 first and the copy it to output_vxs
                                //(but you can generate data directly to _outputStrokeVxs

                                new MySimpleAction(()=>{
                                   _strokeMath.CreateJoin(vxs1, _v1, _v0, _v3);
                                    vxs1.GetVertex(0, out double first_moveX, out double first_moveY);
                                    _outputStrokeVxs.AddMoveTo(first_moveX, first_moveY);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                                //
                               new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v0, _v3, _v2);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v3, _v2, _v1);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v2, _v1, _v0);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                     _outputStrokeVxs.AddCloseFigure();
                                }),
                            };

                            for (int i = stepCount; i < actions.Length && i < Steps; ++i)
                            {
                                actions[i]();
                            }
                        }
                        break;
                    case RawStrokeMath2Choices.OuterAndInner:
                        {
                            //outer
                            double first_moveX = 0;
                            double first_moveY = 0;

                            MySimpleAction[] actions = new MySimpleAction[]
                            {
                                //create join
                                //since we want to see data inside each step, so 
                                //we generate data into vxs1 first and the copy it to output_vxs
                                //(but you can generate data directly to _outputStrokeVxs

                                new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v0, _v1, _v2);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                                //
                               new MySimpleAction(()=>{
                                   _strokeMath.CreateJoin(vxs1, _v1, _v2, _v3);
                                   _outputStrokeVxs.AppendVertexStore(vxs1);
                                   vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                   _strokeMath.CreateJoin(vxs1, _v2, _v3, _v0);
                                   _outputStrokeVxs.AppendVertexStore(vxs1);
                                   vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v3, _v0, _v1);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                               }),
                               new MySimpleAction(()=>{
                                    _outputStrokeVxs.GetVertex(0, out first_moveX, out first_moveY);
                                    _outputStrokeVxs.AddLineTo(first_moveX, first_moveY);
                                    _outputStrokeVxs.AddCloseFigure();
                                }),


                                //----------------------------------------------------
                                //inner
                               new MySimpleAction(()=>{
                                    //**please note the different direction compare to the outer**                                     
                                    _strokeMath.CreateJoin(vxs1, _v2, _v1, _v0);
                                    vxs1.GetVertex(0, out first_moveX, out first_moveY);
                                    _outputStrokeVxs.AddMoveTo(first_moveX, first_moveY);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v1, _v0, _v3);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                    _strokeMath.CreateJoin(vxs1, _v0, _v3, _v2);
                                    _outputStrokeVxs.AppendVertexStore(vxs1);
                                    vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                   _strokeMath.CreateJoin(vxs1, _v3, _v2, _v1);
                                   _outputStrokeVxs.AppendVertexStore(vxs1);
                                   vxs1.Clear();
                                }),
                               new MySimpleAction(()=>{
                                   _outputStrokeVxs.AddLineTo(first_moveX, first_moveY);
                                  _outputStrokeVxs.AddCloseFigure();
                                }),
                            };

                            for (int i = stepCount; i < actions.Length && i < Steps; ++i)
                            {
                                actions[i]();
                            }

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

            switch (OutlineChoices)
            {
                case RawStrokeMath2Choices.Auto_OuterAndInner:
                    {
                        p.Fill(_outputStrokeVxs);
                    }
                    break;
                default:
                    {
                        p.Draw(_outputStrokeVxs);
                    }
                    break;
                case RawStrokeMath2Choices.EachBorders:
                    {
                        //demonstrate Msdf3 borders

                        InnerJoin saveInnerJoin = _strokeMath.InnerJoin;
                        LineJoin saveOuterJoin = _strokeMath.LineJoin;

                        FillEachBordersForMsdf3(p);

                        _strokeMath.InnerJoin = saveInnerJoin; //restore
                        _strokeMath.LineJoin = saveOuterJoin; //restore
                    }
                    break;
            }

            p.StrokeWidth = prevW;
            //restore
            p.FillColor = c1;
        }


        struct CircularIndexProvider
        {
            int _vertexCount;
            public void SetTotalVertice(int vertexCount)
            {
                //at least 3
                _vertexCount = vertexCount;
                if (vertexCount < 3)
                {
                    throw new NotSupportedException();
                }
            }

            public int CurrentCounter { get; set; }
            public void GetIndiceForward(int index, out int prev, out int next0, out int next1)
            {

                prev = index - 1;
                if (prev < 0)
                {
                    prev = _vertexCount - 1;
                }
                next0 = index + 1;
                if (next0 >= _vertexCount)
                {
                    next0 = 0;
                }
                next1 = next0 + 1;
                if (next1 >= _vertexCount)
                {
                    next1 = 0;
                }

                CurrentCounter++;
            }
            public void GetIndiceBackward(int index, out int prev, out int next0, out int next1)
            {
                prev = index + 1;
                if (prev >= _vertexCount)
                {
                    prev = 0;
                }
                next0 = index - 1;
                if (next0 < 0)
                {
                    next0 = _vertexCount - 1;
                }
                next1 = next0 - 1;
                if (next1 < 0)
                {
                    next1 = _vertexCount - 1;
                }

                CurrentCounter++;
            }

            public int GetIndexForward()
            {
                if (CurrentCounter + 1 >= _vertexCount)
                {
                    CurrentCounter = -1;
                }

                return ++CurrentCounter;
            }
        }

        void FillEachBordersForMsdf3(Painter p)
        {
            _strokeMath.InnerJoin = InnerJoin.Miter;
            _strokeMath.LineJoin = LineJoin.Miter;


            void CreateBorder(Vertex2d prev, Vertex2d now, Vertex2d next0, Vertex2d next1, Color fillColor)
            {
                //NESTED method
                //outer join and inner join for each line
                using (VxsTemp.Borrow(out var vxs1, out var vxs2))
                {

                    //now we are on now
                    vxs2.AddMoveTo(now.x, now.y);

                    //create outer line-join
                    _strokeMath.CreateJoin(vxs1, prev, now, next0);
                    vxs2.AppendVertexStore(vxs1);
                    //create inner line join

                    //next outer line join
                    vxs1.Clear();//reuse
                    _strokeMath.CreateJoin(vxs1, now, next0, next1);
                    vxs2.AppendVertexStore(vxs1);

                    vxs2.AddLineTo(next0.x, next0.y);
                    vxs2.AddCloseFigure();

                    p.FillColor = fillColor;
                    p.Fill(vxs2);
                    //------------- 
                }
            }


            Vertex2d[] vertices = new Vertex2d[] { _v0, _v1, _v2, _v3 };
            Color[] outer_colors = new Color[] { Color.FromArgb(255, 0, 0), Color.FromArgb(0, 255, 0), Color.FromArgb(0, 0, 255), Color.FromArgb(0, 255, 0) };
            Color[] inner_colors = new Color[] { Color.FromArgb(0, 255, 255), Color.FromArgb(255, 0, 255), Color.FromArgb(255, 255, 0), Color.FromArgb(255, 0, 255) };

            var vertxIndiceProvider = new CircularIndexProvider();
            vertxIndiceProvider.SetTotalVertice(vertices.Length);

            var colorIndiceProvider = new CircularIndexProvider();
            colorIndiceProvider.SetTotalVertice(outer_colors.Length);
            colorIndiceProvider.CurrentCounter = 0;

            for (int i = 0; i < vertices.Length; ++i)
            {
                //outer borders
                //eg. 
                //v0->v1->v2
                //v1->v2->v3
                vertxIndiceProvider.GetIndiceForward(i, out int prev, out int next0, out int next1);
                CreateBorder(vertices[prev], vertices[i], vertices[next0], vertices[next1], outer_colors[colorIndiceProvider.GetIndexForward()]);
            }


            colorIndiceProvider.CurrentCounter = vertices.Length - 1;
            for (int i = vertices.Length - 1; i >= 0; --i)
            {
                //inner borders
                //different direction
                vertxIndiceProvider.GetIndiceBackward(i, out int prev, out int next0, out int next1);
                CreateBorder(vertices[prev], vertices[i], vertices[next0], vertices[next1], inner_colors[colorIndiceProvider.GetIndexForward()]);
            }
        }

    }


}