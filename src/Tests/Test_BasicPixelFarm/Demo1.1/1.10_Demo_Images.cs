//Apache2, 2014-2017, WinterDev

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
                viewport.AddContent(imgBox);
            }

        }
    }
}