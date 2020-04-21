//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    [DemoNote("1.7 ListView")]
    public class Demo_ListView : App
    {
        protected override void OnStart(AppHost host)
        {
            var listview = new LayoutFarm.CustomWidgets.ListBox(300, 400);
            listview.SetLocation(10, 10);
            listview.BackColor = KnownColors.FromKnownColor(KnownColor.LightGray);
            //add list view to viewport
            host.AddChild(listview);
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