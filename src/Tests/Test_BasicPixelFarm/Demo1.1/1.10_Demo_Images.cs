//Apache2, 2014-present, WinterDev
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("1.10 MultipleImages")]
    class Demo_MultipleImages : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {

            ImageBinder imgBinder = viewport.GetImageBinder2("../../Data/imgs/favorites32.png");
            for (int i = 0; i < 100; ++i)
            {
                //share 1 img binder with multiple img boxes
                var imgBox = new CustomWidgets.ImageBox(imgBinder.Image.Width, imgBinder.Image.Height);
                imgBox.ImageBinder = imgBinder;
                imgBox.SetLocation(i * 10, i * 10);
                viewport.AddChild(imgBox);
            }
        }
    }

    [DemoNote("1.10_1 MultipleLabels")]
    class Demo_MultipleLabels : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            for (int i = 0; i < 10; ++i)
            {
                Label label = new Label(17, 50);
                label.SetLocation(i * 20, i * 20);
                label.Color = PixelFarm.Drawing.Color.Black;
                label.Text = "ABCDEFGHIJKLMNOPQRSTUVWXZYZ0123456789";
                viewport.AddChild(label);
            }
        }
    }
}