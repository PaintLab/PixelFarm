//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    [DemoNote("3.1 DemoVisible")]
    class Demo_Visible : App
    {
        protected override void OnStart(AppHost host)
        {
            var box1 = new LayoutFarm.CustomWidgets.Box(50, 50);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            host.AddChild(box1);
            //--------------------------------
            var box2 = new LayoutFarm.CustomWidgets.Box(30, 30);
            box2.SetLocation(50, 50);
            host.AddChild(box2);
            //1. mouse down         
            box1.MouseDown += (s, e) =>
            {
                box1.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                box2.Visible = false;
            };
            box1.MouseUp += (s, e) =>
            {
                box1.BackColor = Color.Red;
                box2.Visible = true;
            };
        }
    }
}