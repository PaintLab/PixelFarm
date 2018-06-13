//Apache2, 2014-2018, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    [DemoNote("1.8 Hinge")]
    class Demo_Hinge : DemoBase
    {
        ImageBinder arrowBmp;
        SampleViewport viewport;

        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.viewport = viewport;
            var comboBox1 = CreateComboBox(20, 20);
            viewport.AddChild(comboBox1);
            var comboBox2 = CreateComboBox(50, 50);
            viewport.AddChild(comboBox2);
            //------------
            var menuItem = CreateMenuItem(50, 100);
            var menuItem2 = CreateMenuItem(5, 5);
            menuItem.AddSubMenuItem(menuItem2);
            viewport.AddChild(menuItem);
        }

        LayoutFarm.CustomWidgets.ComboBox CreateComboBox(int x, int y)
        {
            var comboBox = new CustomWidgets.ComboBox(400, 20);
            comboBox.SetLocation(x, y);
            //--------------------
            //1. create landing part
            var landPart = new LayoutFarm.CustomWidgets.Box(400, 20);
            landPart.BackColor = Color.Green;
            comboBox.LandPart = landPart;
            //--------------------------------------
            //add small px to land part
            //image
            //load bitmap with gdi+                
            if (arrowBmp == null)
            {
                arrowBmp = viewport.GetImageBinder2("../../Data/imgs/arrow_open.png");
            }
            LayoutFarm.CustomWidgets.ImageBox imgBox = new CustomWidgets.ImageBox(arrowBmp.Image.Width, arrowBmp.Image.Height);
            imgBox.ImageBinder = arrowBmp;
            //--------------------------------------
            //2. float part
            var floatPart = new LayoutFarm.CustomWidgets.Box(400, 100);
            floatPart.BackColor = Color.Blue;
            comboBox.FloatPart = floatPart;
            //--------------------------------------
            //if click on this image then
            imgBox.MouseDown += (s, e) =>
            {
                e.CancelBubbling = true;
                if (comboBox.IsOpen)
                {
                    comboBox.CloseHinge();
                }
                else
                {
                    comboBox.OpenHinge();
                }
            };
            imgBox.LostMouseFocus += (s, e) =>
            {
                if (comboBox.IsOpen)
                {
                    comboBox.CloseHinge();
                }
            };
            landPart.AddChild(imgBox);
            return comboBox;
        }

        LayoutFarm.CustomWidgets.MenuItem CreateMenuItem(int x, int y)
        {
            var mnuItem = new CustomWidgets.MenuItem(150, 20);
            mnuItem.SetLocation(x, y);
            //--------------------
            //1. create landing part
            var landPart = new LayoutFarm.CustomWidgets.Box(150, 20);
            landPart.BackColor = Color.OrangeRed;
            mnuItem.LandPart = landPart;
            //--------------------------------------
            //add small px to land part
            //image
            //load bitmap with gdi+        

            if (arrowBmp == null)
            {
                arrowBmp = this.viewport.GetImageBinder2("../../Data/imgs/arrow_open.png");
            }
            LayoutFarm.CustomWidgets.ImageBox imgBox = new CustomWidgets.ImageBox(arrowBmp.ImageWidth, arrowBmp.ImageHeight);
            imgBox.ImageBinder = arrowBmp;
            landPart.AddChild(imgBox);
            //--------------------------------------
            //if click on this image then
            imgBox.MouseDown += (s, e) =>
            {
                e.CancelBubbling = true;
                //1. maintenace parent menu***
                mnuItem.MaintenanceParentOpenState();
                //-----------------------------------------------
                if (mnuItem.IsOpened)
                {
                    mnuItem.Close();
                }
                else
                {
                    mnuItem.Open();
                }
            };
            imgBox.MouseUp += (s, e) =>
            {
                mnuItem.UnmaintenanceParentOpenState();
            };
            imgBox.LostMouseFocus += (s, e) =>
            {
                if (!mnuItem.MaintenceOpenState)
                {
                    mnuItem.CloseRecursiveUp();
                }
            };
            //--------------------------------------
            //2. float part
            var floatPart = new LayoutFarm.CustomWidgets.MenuBox(400, 100);
            floatPart.BackColor = Color.Gray;
            mnuItem.FloatPart = floatPart;
            return mnuItem;
        }


    }
}