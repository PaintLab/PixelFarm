//Apache2, 2014-2017, WinterDev

namespace LayoutFarm
{
    [DemoNote("1.12 MultipleImages")]
    class Demo_SmallAnimation : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            ImageBinder imgBinder = new ClientImageBinder();
            //force load image
            imgBinder.SetImage(LoadBitmap("../../Data/imgs/favorites32.png"));
            for (int i = 0; i < 100; ++i)
            {
                //share 1 img binder with multiple img boxes
                var imgBox = new CustomWidgets.ImageBox(
                    imgBinder.Image.Width,
                    imgBinder.Image.Height);

                imgBox.ImageBinder = imgBinder;
                imgBox.SetLocation(i * 32, 20);
                imgBox.MouseDown += (s, e) =>
                {
                    //test start animation  
                    int nsteps = 40;
                    UIPlatform.RegisterTimerTask(20, timer =>
                    {
                        imgBox.SetLocation(imgBox.Left, imgBox.Top + 10);
                        nsteps--;
                        if (nsteps <= 0)
                        {
                            timer.Remove();
                        }
                    });
                };
                viewport.AddContent(imgBox);
            }

        }

    }
}