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
                p.Fill(_strokePath);
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



}