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

   
}