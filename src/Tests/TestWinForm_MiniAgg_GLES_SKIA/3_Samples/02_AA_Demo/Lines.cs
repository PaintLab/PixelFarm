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
        Stroke strokeGen = new Stroke(30.0);
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
                strokeGen.LineJoin = this.LineJoin;
                strokeGen.LineCap = this.LineCap;

                _strokePath = new VertexStore();
                strokeGen.MakeVxs(_orgVxs, _strokePath);
            }
        }

        [DemoConfig]
        public LineJoin LineJoin
        {
            get { return _lineJoin; }
            set { _lineJoin = value; _strokePath = null; }
        }
        [DemoConfig]
        public LineCap LineCap
        {
            get { return _lineCap; }
            set { _lineCap = value; _strokePath = null; }
        }


        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            var aggPainter = p as PixelFarm.CpuBlit.AggPainter;
            if (aggPainter == null)
            {
                return;
            }
            Draw(aggPainter);
        }
        void Draw(PixelFarm.CpuBlit.AggPainter aggPainter)
        {

            aggPainter.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            aggPainter.StrokeColor = PixelFarm.Drawing.Color.Black;
            aggPainter.StrokeWidth = 30.0f;


            aggPainter.LineJoin = this.LineJoin;
            aggPainter.LineCap = this.LineCap;
            //

            UpdateStroke();
            if (_strokePath != null)
            {
                Color c1 = aggPainter.FillColor;//save
                aggPainter.FillColor = aggPainter.StrokeColor;
                aggPainter.Fill(_strokePath);
                //restore
                aggPainter.FillColor = c1;
            }
            else
            {
                aggPainter.Draw(_orgVxs);
            }

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
        VertexStore _vxs;
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
            var aggPainter = p as PixelFarm.CpuBlit.AggPainter;
            if (aggPainter == null)
            {
                return;
            }
            Draw(aggPainter);
        }
        void Draw(PixelFarm.CpuBlit.AggPainter aggPainter)
        {

            aggPainter.Clear(PixelFarm.Drawing.Color.White);
            //--------------------------
            aggPainter.StrokeColor = PixelFarm.Drawing.Color.Black;
            aggPainter.StrokeWidth = 2f;



            aggPainter.LineJoin = this.LineJoin;
            aggPainter.LineCap = this.LineCap;
            //
            aggPainter.Draw(_vxs);
        }
    }
}