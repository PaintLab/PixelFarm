//Apache2, 2014-present, WinterDev

using System;
using ImageTools;
using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("1.1 SingleButton")]
    public class Demo_SingleButton : App
    {
        protected override void OnStart(AppHost host)
        {
            var sampleButton = new LayoutFarm.CustomWidgets.Box(30, 30);
            host.AddChild(sampleButton);
            int count = 0;

            var boxspec = new CustomRenderBoxSpec();

            boxspec.BackColor = PixelFarm.Drawing.Color.Red;
            boxspec.BorderColor = PixelFarm.Drawing.Color.Black;

            sampleButton.BoxSpec = boxspec;

            sampleButton.MouseDown += (s, e2) =>
            {
                Console.WriteLine("click :" + (count++));
            };
        }
    }


}