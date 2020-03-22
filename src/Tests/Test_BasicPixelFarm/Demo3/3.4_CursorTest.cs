//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("3.4_CursorTest")]
    class Demo_CursorTest : App
    {
        protected override void OnStart(AppHost host)
        {
            var sampleButton = new LayoutFarm.CustomWidgets.Box(30, 30);
            host.AddChild(sampleButton);
            int count = 0;

            sampleButton.MouseDown += new EventHandler<UIMouseEventArgs>((s, e2) =>
            {
                //click to create custom cursor
                Console.WriteLine("click :" + (count++));
            });
        }
    }
}