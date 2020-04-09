//Apache2, 2014-present, WinterDev
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("3.2 DemoControllerBox")]
    class Demo_ControllerBoxs : App
    {
        UIControllerBox _controllerBox1;
        protected override void OnStart(AppHost host)
        {
            var box1 = new LayoutFarm.CustomWidgets.Box(50, 50);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            //box1.dbugTag = 1;
            SetupActiveBoxProperties(box1);
            host.AddChild(box1);
            var box2 = new LayoutFarm.CustomWidgets.Box(30, 30);
            box2.SetLocation(50, 50);
            //box2.dbugTag = 2;
            SetupActiveBoxProperties(box2);
            host.AddChild(box2);
            _controllerBox1 = new UIControllerBox(40, 40);
            Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
            _controllerBox1.BackColor = new Color(100, c.R, c.G, c.B);
            _controllerBox1.SetLocation(200, 200);
            //controllerBox1.dbugTag = 3;
            _controllerBox1.Visible = false;
            SetupControllerBoxProperties(_controllerBox1);
            host.AddChild(_controllerBox1);
        }

        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.Box box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //--------------------------------------------
                //move controller here 
                _controllerBox1.SetLocationAndSize(
                    box.Left - 5, box.Top - 5,
                    box.Width + 10, box.Height + 10);
                _controllerBox1.Visible = true;
                _controllerBox1.TargetBox = box;
                e.SetMouseCapturedElement(_controllerBox1);
            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = KnownColors.LightGray;
                _controllerBox1.Visible = false;
                _controllerBox1.TargetBox = null;
            };
        }


        static void SetupControllerBoxProperties(UIControllerBox controllerBox)
        {
            //for controller box  
            controllerBox.MouseDrag += (s, e) =>
            {
                Point pos = controllerBox.Position;
                int newX = pos.X + e.XDiff;
                int newY = pos.Y + e.YDiff;
                controllerBox.SetLocation(newX, newY);
                var targetBox = controllerBox.TargetBox;
                if (targetBox != null)
                {
                    //move target box too
                    targetBox.SetLocation(newX + 5, newY + 5);
                }
                e.CancelBubbling = true;
            };
        }

        class UIControllerBox : LayoutFarm.CustomWidgets.AbstractBox
        {
            public UIControllerBox(int w, int h)
                : base(w, h)
            {
            }
            public LayoutFarm.UI.AbstractRectUI TargetBox
            {
                get;
                set;
            }
        }
    }


}



