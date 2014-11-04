//2014 BSD,WinterDev
//MatterHackers

using System;
using System.Diagnostics;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.UI;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Agg.Transform;


using Mini;
namespace PixelFarm.Agg.Sample_Gradient
{



    [Info(OrderCode = "07_1")]
    [Info("This �sphere� is rendered with color gradients only. Initially there was an idea to compensate so called Mach Bands effect. To do so I added a gradient profile functor. Then the concept was extended to set a color profile. As a result you can render simple geometrical objects in 2D looking like 3D ones. In this example you can construct your own color profile and select the gradient function. There're not so many gradient functions in AGG, but you can easily add your own. Also, drag the �gradient� with the left mouse button, scale and rotate it with the right one.")]
    public class GradientDemo : DemoBase
    {


        Stopwatch stopwatch = new Stopwatch();
        public GradientDemo()
        {


        }
        public override void Draw(Graphics2D g)
        {
            OnDraw(g);
        }
        public void OnDraw(Graphics2D graphics2D)
        {
            CanvasPainter painter = new CanvasPainter(graphics2D);
            painter.FillColor = ColorRGBA.Blue;
            painter.FillRectangle(10, 10, 50, 50);


            //-------------------------------------------

            Agg.Gradients.GvcRadial innerGradient = new Gradients.GvcRadial();
            SpanInterpolatorLinear linerInterpolator = new SpanInterpolatorLinear(Affine.IdentityMatrix);
            LinearGradientColorsProvider linearGradFunc = new LinearGradientColorsProvider(ColorRGBA.Red, ColorRGBA.Yellow);
            SpanGenGradient spanGenGradient = new SpanGenGradient(linerInterpolator, innerGradient, linearGradFunc, 100, 150);

            SimpleRect srect = new SimpleRect(100, 10, 150, 50);
            painter.Fill(srect.MakeVxs(), spanGenGradient);

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




