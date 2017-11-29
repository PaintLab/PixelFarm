// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)
//MIT, 2017, WinterDev

using ColorBlender;
using ColorBlender.Algorithms;

namespace LayoutFarm
{
    [DemoNote("1.13 ColorBlenderExample")]
    class DemoColorBlender : DemoBase
    {
        ColorMatch colorMatch;
        protected override void OnStartDemo(SampleViewport viewport)
        {

            
            //-------------------------------------
            {
                //horizontal scrollbar
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(200, 15);
                scbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                scbar.SetLocation(80, 10);
                scbar.MinValue = 0;
                scbar.MaxValue = 255*10;
                scbar.SmallChange = 1;
                viewport.AddContent(scbar);
            }
            {
                //horizontal scrollbar
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(200, 15);
                scbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                scbar.SetLocation(80, 40);
                scbar.MinValue = 0;
                scbar.MaxValue = 255 * 10;
                scbar.SmallChange = 1;
                viewport.AddContent(scbar);
            }

            {
                //horizontal scrollbar
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(200, 15);
                scbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                scbar.SetLocation(80, 80);
                scbar.MinValue = 0;
                scbar.MaxValue = 255 * 10;
                scbar.SmallChange = 1;
                viewport.AddContent(scbar);
            }
            //-------------------------------------



            colorMatch = new ColorMatch(213, 46, 49);
            RGB rgbValue = colorMatch.CurrentRGB;

            int x = 300;
            int y = 20;

            var cmd_R = new LayoutFarm.CustomWidgets.SimpleBox(30, 30);
            cmd_R.BackColor = new PixelFarm.Drawing.Color((byte)rgbValue.R, 0, 0);
            cmd_R.SetLocation(x, y);

            var cmd_G = new LayoutFarm.CustomWidgets.SimpleBox(30, 30);
            cmd_G.BackColor = new PixelFarm.Drawing.Color(0, (byte)rgbValue.G, 0);
            cmd_G.SetLocation(x + 30, y + 0);

            //
            var cmd_B = new LayoutFarm.CustomWidgets.SimpleBox(30, 30);
            cmd_B.BackColor = new PixelFarm.Drawing.Color(0, 0, (byte)rgbValue.B);
            cmd_B.SetLocation(x + 60, y + 0);

            var cmd_RGB = new LayoutFarm.CustomWidgets.SimpleBox(30, 30);
            cmd_RGB.BackColor = new PixelFarm.Drawing.Color((byte)rgbValue.R,
                (byte)rgbValue.G, (byte)rgbValue.B);
            cmd_RGB.SetLocation(x + 90, y + 0);



            viewport.AddContent(cmd_R);
            viewport.AddContent(cmd_G);
            viewport.AddContent(cmd_B);
            viewport.AddContent(cmd_RGB);
        }

    }
}