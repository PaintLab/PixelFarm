//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using System.Diagnostics;
using PixelFarm.Drawing;
using Mini;
namespace PixelFarm.CpuBlit.Sample_Gradient
{

    [Info(OrderCode = "01_1")]
    public class GradientDemo : DemoBase
    {
        Stopwatch stopwatch = new Stopwatch();
        VertexStore triangleVxs;
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
            // _circularGrBrush.AddMoreColorStop(new PointF(100, 0), PixelFarm.Drawing.Color.Green);
            //_circularGrBrush.AddMoreColorStop(new PointF(140, 0), PixelFarm.Drawing.Color.Yellow);

           
            PathWriter p = new PathWriter();
            p.MoveTo(0, 0);
            p.LineToRel(100, 100);
            p.LineToRel(100, -100);
            p.CloseFigure();
            triangleVxs = p.Vxs.CreateTrim();

        }

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            if (p is AggPainter)
            {
                //solid color
                var p2 = (AggPainter)p;
                p.RenderQuality = RenderQualtity.Fast;

                ////solid color + alpha
                p.FillColor = Color.FromArgb(80, Drawing.Color.Red);
                p.FillRect(180, 70, 150, 120);
                //-------------


                var prevBrush = p.CurrentBrush;
                p.CurrentBrush = _circularGrBrush;// gradientBrush;

                p2.FillRect(0, 100, 150, 50);

                p.CurrentBrush = _linearGrBrush;
                p2.FillRect(0, 200, 150, 50);

                //------------- 
                //fill path with gradient
                p2.Fill(triangleVxs);
                //------------- 



                p.CurrentBrush = prevBrush;

            }
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




