//Apache2, 2014-2018, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("1.3 Grid")]
    class Demo_Grid : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            //grid0
            {
                var gridBox = new LayoutFarm.CustomWidgets.GridBox(100, 100);
                gridBox.SetLocation(50, 50);
                gridBox.BuildGrid(2, 4, CellSizeStyle.UniformCell);
                viewport.AddContent(gridBox);
                gridBox.MouseDown += (s1, e1) =>
                {
                };
            }
            //grid1
            {
                var gridBox = new LayoutFarm.CustomWidgets.GridBox(100, 100);
                gridBox.SetLocation(200, 50);
                gridBox.BuildGrid(2, 4, CellSizeStyle.UniformCell);
                viewport.AddContent(gridBox);
                var simpleButton = new LayoutFarm.CustomWidgets.SimpleBox(20, 20);
                simpleButton.BackColor = KnownColors.FromKnownColor(KnownColor.OliveDrab);
                gridBox.AddUI(simpleButton, 1, 1);
                gridBox.MouseDown += (s1, e1) =>
                {


                };

                simpleButton.MouseDown += (s1, e1) =>
                {
                    var box = (LayoutFarm.CustomWidgets.SimpleBox)s1;
                    box.BackColor = KnownColors.FromKnownColor(KnownColor.OrangeRed);

                };
            }
            ////-----
            //grid2
            {
                var gridBox = new LayoutFarm.CustomWidgets.GridBox(200, 100);
                gridBox.SetLocation(350, 50);
                gridBox.BuildGrid(10, 8, CellSizeStyle.UniformCell);
                viewport.AddContent(gridBox);
            }
        }

    }
}