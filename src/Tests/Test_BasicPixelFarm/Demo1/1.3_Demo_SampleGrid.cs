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
                var gridView = new LayoutFarm.CustomWidgets.GridView(100, 100);
                gridView.SetLocation(50, 50);
                gridView.BuildGrid(2, 4, CellSizeStyle.UniformCell);
                viewport.AddContent(gridView);
                gridView.MouseDown += (s1, e1) =>
                {
                };
            }
            //grid1
            {
                var gridView = new LayoutFarm.CustomWidgets.GridView(100, 100);
                gridView.SetLocation(200, 50);
                gridView.BuildGrid(2, 4, CellSizeStyle.UniformCell);
                viewport.AddContent(gridView);
                var simpleButton = new LayoutFarm.CustomWidgets.SimpleBox(20, 20);
                simpleButton.BackColor = KnownColors.FromKnownColor(KnownColor.OliveDrab);
                gridView.AddUI(simpleButton, 1, 1);
                gridView.MouseDown += (s1, e1) =>
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
                var gridView = new LayoutFarm.CustomWidgets.GridView(200, 100);
                gridView.SetLocation(350, 50);
                gridView.BuildGrid(10, 8, CellSizeStyle.UniformCell);
                viewport.AddContent(gridView);
            }

            ////-----
            //grid3
            {
                var gridView = new LayoutFarm.CustomWidgets.GridView(200, 100);
                gridView.SetLocation(50, 250);
                gridView.BuildGrid(10, 8, CellSizeStyle.UniformCell);
                viewport.AddContent(gridView);
            }


            ////-----
            //grid4
            {
                var gridBox = new LayoutFarm.CustomWidgets.GridBox(400,200);
                gridBox.SetLocation(300, 250); 
                viewport.AddContent(gridBox);
                gridBox.PerformContentLayout();
            }
        }

    }
}