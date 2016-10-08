//BSD, 2014-2016, WinterDev
//MatterHackers 

using System;
using PixelFarm.Drawing;
using PixelFarm.Agg.Image;
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
        Point2D imageOffset = new Point2D(20, 60);
        public FloodFillDemo()
        {
            BackgroundColor = Color.White;
            imageToFillOn = new ActualImage(400, 300, PixelFormat.ARGB32);
            ImageGraphics2D imageToFillGraphics = Graphics2D.CreateFromImage(imageToFillOn, PixelFarm.Drawing.WinGdi.WinGdiPortal.P);
            imageToFillGraphics.Clear(Color.White);
            imageToFillGraphics.DrawString("Click to fill", 20, 30);
            imageToFillGraphics.Circle(new Vector2(200, 150), 35, Color.Black);
            imageToFillGraphics.Circle(new Vector2(200, 150), 30, Color.Green);
            imageToFillGraphics.Rectangle(20, 50, 210, 280, Color.Black);
            imageToFillGraphics.Rectangle(imageToFillOn.Bounds, Color.Blue);
            Random rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                Ellipse elipse = new Ellipse(rand.Next(imageToFillOn.Width), rand.Next(imageToFillOn.Height), rand.Next(10, 60), rand.Next(10, 60));
                imageToFillGraphics.Render(new Stroke(1).MakeVxs(elipse.MakeVxs()), Color.Black);
            }

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

        public override void Draw(CanvasPainter p)
        {
            p.DrawImage(imageToFillOn, imageOffset.x, imageOffset.y);
        }
        public override void MouseDown(int mx, int my, bool isRightButton)
        {
            int x = mx - imageOffset.x;
            int y = my - imageOffset.y;
            FloodFill filler = new FloodFill(Color.Red);
            filler.Fill(imageToFillOn, x, y);
        }
    }
}
