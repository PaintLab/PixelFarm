//Apache2, 2014-2018, WinterDev

using System;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("1.1 SingleButton")]
    class Demo_SingleButton : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            var sampleButton = new LayoutFarm.CustomWidgets.Box(30, 30);
            viewport.AddChild(sampleButton);
            int count = 0;
            sampleButton.MouseDown += new EventHandler<UIMouseEventArgs>((s, e2) =>
            {
                Console.WriteLine("click :" + (count++));
            });
        }
    }
}