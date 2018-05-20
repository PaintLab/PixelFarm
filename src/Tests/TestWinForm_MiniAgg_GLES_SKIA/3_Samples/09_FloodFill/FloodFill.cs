//BSD, 2014-2018, WinterDev
//MatterHackers 

using System;
using PixelFarm.Drawing;
using PixelFarm.Agg.Imaging;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;
using PixelFarm.Drawing.Fonts;
using Mini;

namespace PixelFarm.Agg.Sample_FloodFill
{
    [Info(OrderCode = "09")]
    [Info(DemoCategory.Bitmap, "Demonstration of a flood filling algorithm.")]
    public class FloodFillDemo : DemoBase
    {
        ActualImage imageToFillOn;

        int imgOffsetX = 20;
        int imgOffsetY = 60;

        public FloodFillDemo()
        {
            //
            BackgroundColor = Color.White;
            imageToFillOn = new ActualImage(400, 300);
            AggRenderSurface aggsx = new AggRenderSurface(imageToFillOn);
            AggPainter p = new AggPainter(aggsx);

            p.Clear(Color.White);

            p.FillColor = Color.Black;
            p.FillEllipse(20, 20, 30, 30);


            for (int i = 0; i < 20; i++)
            {
                p.StrokeColor = Color.Black;
                p.DrawEllipse(i * 10, i * 10, 20, 20);
            }
            //
            this.PixelSize = 32;
            this.Gamma = 1;
        }
        [DemoConfig(MinValue = 8, MaxValue = 100)]
        public int PixelSize
        {
            get;
            set;
        }
        [DemoConfig(MaxValue = 3)]
        public double Gamma
        {
            get;
            set;
        }
        public Color BackgroundColor
        {
            get;
            set;
        }

        public override void Draw(Painter p)
        {
            p.Clear(Color.Blue);

            p.DrawImage(imageToFillOn, imgOffsetX, imgOffsetY);

            p.FillColor = Color.Yellow;
            p.FillEllipse(20, 20, 30, 30);

            p.StrokeColor = Color.Red;
            p.DrawLine(0, 0, 100, 100);
        }
        public override void MouseDown(int mx, int my, bool isRightButton)
        {
            int x = mx + imgOffsetX;
            int y = my + imgOffsetY;
            FloodFill filler = new FloodFill(Color.Red);
            filler.Fill(imageToFillOn, x, y);
        }
    }
}
