//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using System.Diagnostics;
using PixelFarm.Drawing;

using Mini;
namespace PixelFarm.CpuBlit.Sample_Gradient
{

    [Info(OrderCode = "01_1", SupportedOn = AvailableOn.GLES | AvailableOn.Agg)]
    public class GradientDemo : DemoBase
    {
        public enum BrushKind
        {
            LinearGradient,
            CircularGradient

        }
        Stopwatch _stopwatch = new Stopwatch();
        VertexStore _triangleVxs;
        LinearGradientBrush _linearGrBrush;
        CircularGradientBrush _circularGrBrush;

        public GradientDemo()
        {
            _linearGrBrush = new LinearGradientBrush(
                     new PointF(0, 0),
                     Drawing.Color.Black,
                     new PointF(50, 0),
                     Drawing.Color.Red);
            _linearGrBrush.AddMoreColorStop(new PointF(100, 0), PixelFarm.Drawing.Color.Yellow);
            _linearGrBrush.AddMoreColorStop(new PointF(140, 0), PixelFarm.Drawing.Color.OrangeRed);


            _circularGrBrush = new CircularGradientBrush(new PointF(0, 0),
                     Drawing.Color.Black,
                     new PointF(120, 0),
                     Drawing.Color.Blue);
            //_circularGrBrush.AddMoreColorStop(new PointF(100, 0), PixelFarm.Drawing.Color.Green);
            //_circularGrBrush.AddMoreColorStop(new PointF(140, 0), PixelFarm.Drawing.Color.Yellow);

            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(v1, out PathWriter p))
            {
                p.MoveTo(0, 0);
                p.LineToRel(100, 100);
                p.LineToRel(100, -100);
                p.CloseFigure();
                _triangleVxs = v1.CreateTrim();
            }

        }

        [DemoConfig]
        public BrushKind SelectedBrushKind { get; set; }
        public override void Draw(PixelFarm.Drawing.Painter p)
        {

            p.RenderQuality = RenderQuality.Fast;


            Brush prevBrush = p.CurrentBrush;

            Brush selectedBrush = _linearGrBrush;
            if (SelectedBrushKind == BrushKind.CircularGradient)
            {
                selectedBrush = _circularGrBrush;
            }

            p.CurrentBrush = selectedBrush;
            p.FillRect(0, 100, 150, 50);

            //p.CurrentBrush = selectedBrush;
            p.FillRect(0, 200, 150, 50);

            //------------- 
            //fill path with gradient
            p.Fill(_triangleVxs);
            //------------- 

            //p.FillColor = Color.FromArgb(80, Drawing.Color.Red);
            //p.FillRect(0, 70, 150, 120);

            p.CurrentBrush = prevBrush;

        }

        public override void MouseDown(int mx, int my, bool isRightButton)
        {
        }
        public override void MouseDrag(int mx, int my)
        {
        }
        public override void MouseUp(int x, int y)
        {
        }
    }
}




