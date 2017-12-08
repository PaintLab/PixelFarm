//Apache2, 2014-2017, WinterDev

using PaintLab;
namespace LayoutFarm
{
    [DemoNote("1.1 SingleButton")]
    class Demo_SingleButton : DemoBase2
    {
        protected override void OnStartDemo(IViewport viewport)
        {
            IUIRootElement root = viewport.Root;
            IUIBoxElement sampleButton = (IUIBoxElement)root.CreateElement2(BasicUIElementKind.SimpleBox);

            root.AddContent(sampleButton);
            sampleButton.SetLocation(20, 20);
            int count = 0;
            //sampleButton.MouseDown += new EventHandler<UIMouseEventArgs>((s, e2) =>
            //{
            //    Console.WriteLine("click :" + (count++));
            //});
        }
    }
}