using System;

using MatterHackers.Agg.UI;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;

using MatterHackers.VectorMath;

using Mini;
namespace MatterHackers.Agg.Sample_FloodFill
{
    [Info(OrderCode = "09")]
    [Info(DemoCategory.Bitmap, "Demonstration of a flood filling algorithm.")]
    public class FloodFillDemo : DemoBase
    {
        ActualImage imageToFillOn;
        Point2D imageOffset = new Point2D(20, 60);

        public FloodFillDemo()
        {
            BackgroundColor = ColorRGBA.White;
            imageToFillOn = new ActualImage(400, 300, 32, new BlenderBGRA());
            var imageToFillGraphics = Graphics2D.CreateFromImage(imageToFillOn);
            imageToFillGraphics.Clear(ColorRGBA.White);
            imageToFillGraphics.DrawString("Click to fill", 20, 30);
            imageToFillGraphics.Circle(new Vector2(200, 150), 35, ColorRGBA.Black);
            imageToFillGraphics.Circle(new Vector2(200, 150), 30, ColorRGBA.Green);
            imageToFillGraphics.Rectangle(20, 50, 210, 280, ColorRGBA.Black);
            imageToFillGraphics.Rectangle(imageToFillOn.GetBounds(), ColorRGBA.Blue);

            Random rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                Ellipse elipse = new Ellipse(rand.Next(imageToFillOn.Width), rand.Next(imageToFillOn.Height), rand.Next(10, 60), rand.Next(10, 60));
                Stroke outline = new Stroke(elipse.MakeSinglePath());
                imageToFillGraphics.Render(outline, ColorRGBA.Black);
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

        public ColorRGBA BackgroundColor
        {
            get;
            set;
        }

        public override void Draw(Graphics2D graphics2D)
        {
            graphics2D.Render(imageToFillOn, imageOffset.x, imageOffset.y);

        }

        public override void MouseDown(int mx, int my, bool isRightButton)
        {
            int x = mx - imageOffset.x;
            int y = my - imageOffset.y;

            FloodFill filler = new FloodFill(ColorRGBA.Red);
            filler.Fill(imageToFillOn, x, y);
        }
    }


}
