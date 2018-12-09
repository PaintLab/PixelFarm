//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing; 
namespace LayoutFarm
{
    [DemoNote("3.5 Demo_CompartmentBox2")]
    class Demo_CompartmentBox2 : App
    {
        LayoutFarm.CustomWidgets.NinespaceBox _ninespaceBox;
        protected override void OnStart(AppHost host)
        {
            //--------------------------------
            {
                //background element
                var bgbox = new LayoutFarm.CustomWidgets.Box(800, 600);
                bgbox.BackColor = Color.White;
                bgbox.SetLocation(0, 0);
                SetupBackgroundProperties(bgbox);
                host.AddChild(bgbox);
            }
            //--------------------------------
            //ninespace compartment
            _ninespaceBox = new LayoutFarm.CustomWidgets.NinespaceBox(800, 600);
            _ninespaceBox.ShowGrippers = true;
            host.AddChild(_ninespaceBox);
            _ninespaceBox.SetSize(800, 600);
        }
        void SetupBackgroundProperties(LayoutFarm.CustomWidgets.Box backgroundBox)
        {
            ////if click on background
            //backgroundBox.MouseDown += (s, e) =>
            //{
            //    controllerBox1.TargetBox = null;//release target box
            //    controllerBox1.Visible = false;
            //};

        }
    }
}