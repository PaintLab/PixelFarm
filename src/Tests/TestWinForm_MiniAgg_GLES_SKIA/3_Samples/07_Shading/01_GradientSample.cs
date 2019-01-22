//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using System.Diagnostics;
using PixelFarm.Drawing;

using Mini;
namespace PixelFarm.CpuBlit.Sample_Gradient
{

    [Info(OrderCode = "01_1", AvailableOn = AvailableOn.GLES | AvailableOn.Agg)]
    public class GradientDemo : DemoBase
    {
        public enum BrushKind
        {
            LinearGradient,
            CircularGradient,
            PolygonGradient,
        }


        VertexStore _triangleVxs;
        LinearGradientBrush _linearGrBrush;

        CircularGradientBrush _circularGrBrush;
        PolygonGraidentBrush _polygonGradientBrush;


        public GradientDemo()
        {

            //1. linear gradient
            _linearGrBrush = new LinearGradientBrush(
                     new PointF(0, 0),
                     Drawing.Color.Black,
                     new PointF(50, 0),
                     Drawing.Color.Red);
            _linearGrBrush.AddMoreColorStop(new PointF(100, 0), PixelFarm.Drawing.Color.Yellow);
            _linearGrBrush.AddMoreColorStop(new PointF(140, 0), PixelFarm.Drawing.Color.OrangeRed);


            //2. circular gradient
            _circularGrBrush = new CircularGradientBrush(new PointF(0, 0),
                     Drawing.Color.Black,
                     new PointF(50, 0),
                     Drawing.Color.Blue);
            _circularGrBrush.AddMoreColorStop(new PointF(100, 0), PixelFarm.Drawing.Color.Green);
            //_circularGrBrush.AddMoreColorStop(new PointF(140, 0), PixelFarm.Drawing.Color.Yellow);




            //3. polygon gradient: this version, just a simple rect 
            PolygonGraidentBrush.ColorVertex2d[] vertices = new PolygonGraidentBrush.ColorVertex2d[]
            {
                new PolygonGraidentBrush.ColorVertex2d(0,0,Color.OrangeRed),
                new PolygonGraidentBrush.ColorVertex2d(300,0,Color.Black),
                new PolygonGraidentBrush.ColorVertex2d(300,400,Color.Yellow),
                new PolygonGraidentBrush.ColorVertex2d(0,400,Color.Blue),
            };
            _polygonGradientBrush = new PolygonGraidentBrush(vertices);

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

            p.Clear(Color.White);

            switch (SelectedBrushKind)
            {
                case BrushKind.LinearGradient:
                    break;
                case BrushKind.CircularGradient:
                    selectedBrush = _circularGrBrush;
                    break;
                case BrushKind.PolygonGradient:
                    selectedBrush = _polygonGradientBrush;
                    break;
            }

            //
            p.CurrentBrush = selectedBrush;

            p.FillRect(0, 100, 200, 50);
            ////p.CurrentBrush = selectedBrush;
            p.FillRect(0, 200, 200, 50);
            ////------------- 
            //fill path with gradient
            p.Fill(_triangleVxs);
            //------------- s             

            p.CurrentBrush = prevBrush;

        }

    }
}




