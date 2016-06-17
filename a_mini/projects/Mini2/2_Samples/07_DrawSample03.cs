using PixelFarm.DrawingGL;
namespace Mini2
{
    [Info(OrderCode = "07")]
    [Info("Drawing")]
    public class DrawSample07 : DemoBase
    {
        public override void Load()
        {
            //draw 1

            FormTestWinGLControl form = new FormTestWinGLControl();
            CanvasGL2d canvas = new CanvasGL2d(this.Width, this.Height);
            GLBitmap hwBmp = null;
            form.SetGLPaintHandler((o, s) =>
            {
                canvas.Clear(PixelFarm.Drawing.Color.White);
                canvas.StrokeColor = PixelFarm.Drawing.Color.DeepPink;
                var polygonCoords = new float[]{
                        5,300,
                        40,300,
                        50,340};
                canvas.FillPolygon(PixelFarm.Drawing.Color.DeepPink, polygonCoords, 3);
            });
            form.Show();
        }
    }
}