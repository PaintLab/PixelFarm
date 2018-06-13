//Apache2, 2014-2018, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    [DemoNote("1.7 ListView")]
    class Demo_ListView : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            var listview = new LayoutFarm.CustomWidgets.ListView(300, 400);
            listview.SetLocation(10, 10);
            listview.BackColor = KnownColors.FromKnownColor(KnownColor.LightGray);
            //add list view to viewport
            viewport.AddChild(listview);
            //add 
            RequestFont listItemFont = new RequestFont("tahoma", 18);
            for (int i = 0; i < 10; ++i)
            {
                var listItem = new LayoutFarm.CustomWidgets.ListItem(400, 20);
                if ((i % 2) == 0)
                {
                    listItem.BackColor = KnownColors.FromKnownColor(KnownColor.OrangeRed);
                }
                else
                {
                    listItem.BackColor = KnownColors.FromKnownColor(KnownColor.Orange);
                }
                listItem.SetFont(listItemFont);
                listItem.Text = "A" + i;
                listview.AddItem(listItem);
            }
        }
    }
}