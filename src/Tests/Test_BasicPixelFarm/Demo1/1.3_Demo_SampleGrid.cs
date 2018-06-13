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
                viewport.AddChild(gridView);
                gridView.MouseDown += (s1, e1) =>
                {
                };
            }
            //grid1
            {
                var gridView = new LayoutFarm.CustomWidgets.GridView(100, 100);
                gridView.SetLocation(200, 50);
                gridView.BuildGrid(2, 4, CellSizeStyle.UniformCell);
                viewport.AddChild(gridView);
                var simpleButton = new LayoutFarm.CustomWidgets.Box(20, 20);
                simpleButton.BackColor = KnownColors.FromKnownColor(KnownColor.OliveDrab);
                gridView.SetCellContent(simpleButton, 1, 1);
                gridView.MouseDown += (s1, e1) =>
                {


                };

                simpleButton.MouseDown += (s1, e1) =>
                {
                    var box = (LayoutFarm.CustomWidgets.Box)s1;
                    box.BackColor = KnownColors.FromKnownColor(KnownColor.OrangeRed);

                };
            }
            ////-----
            //grid2
            {
                var gridView = new LayoutFarm.CustomWidgets.GridView(200, 100);
                gridView.SetLocation(350, 50);
                gridView.BuildGrid(10, 8, CellSizeStyle.UniformCell);
                viewport.AddChild(gridView);
            }

            ////-----
            //grid3
            {
                var gridView = new LayoutFarm.CustomWidgets.GridView(200, 100);
                gridView.SetLocation(50, 250);
                gridView.BuildGrid(10, 8, CellSizeStyle.UniformCell);
                viewport.AddChild(gridView);
            }


            ////-----
            //grid4
            //{
            //    var gridView = new LayoutFarm.CustomWidgets.GridView(800, 400);
            //    gridView.SetLocation(10, 10);
            //    gridView.HasSpecificHeight = true;
            //    gridView.HasSpecificWidth = true;
            //    gridView.NeedClipArea = true;
            //    gridView.BuildGrid(4, 4, CellSizeStyle.UniformCell);



            //    var gridBox = new LayoutFarm.CustomWidgets.GridBox(400, 200);
            //    gridBox.SetLocation(300, 250);
            //    gridBox.SetGridView(gridView);
            //    viewport.AddContent(gridBox);
            //    gridBox.PerformContentLayout();
            //}
            ////-----
            //grid5
            {

                var gridView = new LayoutFarm.CustomWidgets.GridView(200, 100);
                gridView.SetLocation(50, 500);
                gridView.BuildGrid(10, 8, CellSizeStyle.UniformCell);
                viewport.AddChild(gridView);
            }

            {

                //grid 6
                var gridView = new LayoutFarm.CustomWidgets.GridView(200, 100);
                //gridView.HasSpecificHeight = true; //if not set ,scroll bar will not show scroll button
                //gridView.HasSpecificWidth = true;//if not set ,scroll bar will not show scroll button

                gridView.SetLocation(300, 500);
                gridView.NeedClipArea = true;
                gridView.BuildGrid(100, 4, 5, 20);
                viewport.AddChild(gridView);

                //manual sc-bar
                var vscbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 100);
                {
                    //add vrcbar for grid view                       
                    vscbar.SetLocation(gridView.Right + 10, gridView.Top);
                    vscbar.MinValue = 0;
                    vscbar.MaxValue = gridView.Height;
                    vscbar.SmallChange = 20;
                    viewport.AddChild(vscbar);

                    //add relation between viewpanel and scroll bar 
                    var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(vscbar.SliderBox, gridView);
                }
                //
                var hscbar = new LayoutFarm.CustomWidgets.ScrollBar(200, 15);
                {
                    //horizontal scrollbar
                    hscbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                    hscbar.SetLocation(gridView.Left, gridView.Bottom + 10);
                    hscbar.MinValue = 0;
                    hscbar.MaxValue = gridView.Width;
                    hscbar.SmallChange = 2;
                    viewport.AddChild(hscbar);
                    //add relation between viewpanel and scroll bar 
                    var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(hscbar.SliderBox, gridView);
                }

                //perform content layout again***
                gridView.PerformContentLayout();
            }
        }
    }
}