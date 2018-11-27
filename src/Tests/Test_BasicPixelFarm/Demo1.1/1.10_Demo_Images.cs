//Apache2, 2014-present, WinterDev
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("1.10 MultipleImages")]
    class Demo_MultipleImages : App
    {
        protected override void OnStart(AppHost host)
        {

            ImageBinder imgBinder = host.LoadImageAndBind("../Data/imgs/favorites32.png");
            for (int i = 0; i < 100; ++i)
            {
                //share 1 img binder with multiple img boxes
                var imgBox = new CustomWidgets.ImageBox(imgBinder.Width, imgBinder.Height);
                imgBox.ImageBinder = imgBinder;
                imgBox.SetLocation(i * 10, i * 10);
                host.AddChild(imgBox);
            }
        }
    }

    [DemoNote("1.10_1 MultipleLabels")]
    class Demo_MultipleLabels : App
    {
        protected override void OnStart(AppHost host)
        {
            for (int i = 0; i < 10; ++i)
            {
                Label label = new Label(17, 50);
                label.SetLocation(i * 20, i * 20);
                label.Color = PixelFarm.Drawing.Color.Black;
                label.Text = "ABCDEFGHIJKLMNOPQRSTUVWXZYZ0123456789";
                host.AddChild(label);
            }
        }
    }
}