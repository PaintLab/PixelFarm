//Apache2, 2014-2018, WinterDev
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("3.2 DemoControllerBox")]
    class Demo_ControllerBoxs : DemoBase
    {
        UIControllerBox controllerBox1;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            var box1 = new LayoutFarm.CustomWidgets.SimpleBox(50, 50);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            //box1.dbugTag = 1;
            SetupActiveBoxProperties(box1);
            viewport.AddContent(box1);
            var box2 = new LayoutFarm.CustomWidgets.SimpleBox(30, 30);
            box2.SetLocation(50, 50);
            //box2.dbugTag = 2;
            SetupActiveBoxProperties(box2);
            viewport.AddContent(box2);
            controllerBox1 = new UIControllerBox(40, 40);
            Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
            controllerBox1.BackColor = new Color(100, c.R, c.G, c.B);
            controllerBox1.SetLocation(200, 200);
            //controllerBox1.dbugTag = 3;
            controllerBox1.Visible = false;
            SetupControllerBoxProperties(controllerBox1);
            viewport.AddContent(controllerBox1);
        }

        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.EaseBox box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //--------------------------------------------
                //move controller here 
                controllerBox1.SetBounds(box.Left - 5, box.Top - 5,
                                         box.Width + 10, box.Height + 10);
                controllerBox1.Visible = true;
                controllerBox1.TargetBox = box;
                e.SetMouseCapture(controllerBox1);
            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;
                controllerBox1.Visible = false;
                controllerBox1.TargetBox = null;
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

        //-----------------------------------------------------------------
        class UIControllerBox : LayoutFarm.CustomWidgets.EaseBox
        {
            public UIControllerBox(int w, int h)
                : base(w, h)
            {
            }
            public LayoutFarm.UI.UIBox TargetBox
            {
                get;
                set;
            }
            public override void Walk(UIVisitor visitor)
            {
                visitor.BeginElement(this, "ctrlbox");
                this.Describe(visitor);
                visitor.EndElement();
            }
        }
    }



    [DemoNote("3.2.1 DemoControllerBox")]
    class Demo_ControllerBoxs3_1 : DemoBase
    {
        UIControllerBox controllerBox1;


        UIControllerBox _boxLeftTop;
        UIControllerBox _boxLeftBottom;
        UIControllerBox _boxRightTop;
        UIControllerBox _boxRightBottom;

        List<UIControllerBox> _cornerControllers = new List<UIControllerBox>();
        protected override void OnStartDemo(SampleViewport viewport)
        {
            var box1 = new LayoutFarm.CustomWidgets.SimpleBox(50, 50);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            //box1.dbugTag = 1;
            SetupActiveBoxProperties(box1);
            viewport.AddContent(box1);
            var box2 = new LayoutFarm.CustomWidgets.SimpleBox(30, 30);
            box2.SetLocation(50, 50);
            //box2.dbugTag = 2;
            SetupActiveBoxProperties(box2);
            viewport.AddContent(box2);

            //------------
            controllerBox1 = new UIControllerBox(40, 40);
            {
                Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
                controllerBox1.BackColor = new Color(100, c.R, c.G, c.B);
                controllerBox1.SetLocation(200, 200);
                //controllerBox1.dbugTag = 3;
                controllerBox1.Visible = false;
                SetupControllerBoxProperties(controllerBox1);
                viewport.AddContent(controllerBox1);
            }
            //------------

            _boxLeftTop = new UIControllerBox(20, 20);
            SetupCornerBoxController(viewport, _boxLeftTop);
            _boxLeftTop.MouseDrag += (s1, e1) =>
            {
                //move other boxes ...
                UIBox target1 = _boxLeftTop.TargetBox;
                //update target
                target1.SetBounds(_boxLeftTop.Right,
                                  _boxLeftTop.Bottom,
                                  _boxRightTop.Left - _boxLeftTop.Right,
                                  _boxRightBottom.Top - _boxLeftTop.Bottom);
                //update other controller
                UpdateControllerBoxes(target1);

            };
            //------------
            _boxLeftBottom = new UIControllerBox(20, 20);
            SetupCornerBoxController(viewport, _boxLeftBottom);
            _boxLeftBottom.MouseDrag += (s1, e1) =>
            {
                UIBox target1 = _boxLeftBottom.TargetBox;
                //update target
                target1.SetBounds(_boxLeftBottom.Right,
                                  _boxLeftTop.Bottom,
                                  _boxRightTop.Left - _boxLeftBottom.Right,
                                  _boxLeftBottom.Top - _boxLeftTop.Bottom);
                //update other controller
                UpdateControllerBoxes(target1);
            };
            //------------ 

            _boxRightTop = new UIControllerBox(20, 20);
            SetupCornerBoxController(viewport, _boxRightTop);
            _boxRightTop.MouseDrag += (s1, e1) =>
            {
                UIBox target1 = _boxRightTop.TargetBox;
                //update target
                target1.SetBounds(_boxLeftTop.Right,
                                  _boxRightTop.Bottom,
                                  _boxRightTop.Left - _boxLeftTop.Right,
                                  _boxRightBottom.Top - _boxRightTop.Bottom);
                //update other controller
                UpdateControllerBoxes(target1);
            };


            //------------ 
            _boxRightBottom = new UIControllerBox(20, 20);
            SetupCornerBoxController(viewport, _boxRightBottom);
            _boxRightBottom.MouseDrag += (s1, e1) =>
            {
                UIBox target1 = _boxRightBottom.TargetBox;
                //update target
                target1.SetBounds(_boxLeftTop.Right,
                                  _boxLeftTop.Bottom,
                                  _boxRightBottom.Left - _boxLeftTop.Right,
                                  _boxRightBottom.Top - _boxLeftTop.Bottom);
                //update other controller
                UpdateControllerBoxes(target1);
            };

            //------------
        }
        void UpdateControllerBoxes(LayoutFarm.UI.UIBox box)
        {

            //move controller here 
            controllerBox1.SetBounds(box.Left - 5, box.Top - 5,
                                     box.Width + 10, box.Height + 10);
            controllerBox1.Visible = true;
            controllerBox1.TargetBox = box;

            {
                //left-top
                UIControllerBox ctrlBox = _boxLeftTop;
                ctrlBox.SetBounds(box.Left - 5, box.Top - 5, 5, 5);
                ctrlBox.TargetBox = box;
                ctrlBox.Visible = true;
            }
            {
                //right-top
                UIControllerBox ctrlBox = _boxRightTop;
                ctrlBox.SetBounds(box.Left + box.Width, box.Top - 5, 5, 5);
                ctrlBox.TargetBox = box;
                ctrlBox.Visible = true;
            }
            {
                //left-bottom
                UIControllerBox ctrlBox = _boxLeftBottom;
                ctrlBox.SetBounds(box.Left - 5, box.Top + box.Height, 5, 5);
                ctrlBox.TargetBox = box;
                ctrlBox.Visible = true;
            }
            {
                //right-bottom
                UIControllerBox ctrlBox = _boxRightBottom;
                ctrlBox.SetBounds(box.Left + box.Width, box.Top + box.Height, 5, 5);
                ctrlBox.TargetBox = box;
                ctrlBox.Visible = true;
            }
        }
        void SetupCornerBoxController(SampleViewport viewport, UIControllerBox box)
        {
            Color c = KnownColors.FromKnownColor(KnownColor.Orange);
            box.BackColor = new Color(100, c.R, c.G, c.B);
            box.SetLocation(200, 200);
            //controllerBox1.dbugTag = 3;
            box.Visible = false;
            SetupControllerBoxProperties2(box);
            viewport.AddContent(box);
            //
            _cornerControllers.Add(box);
        }
        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.EaseBox box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //--------------------------------------------
                e.SetMouseCapture(controllerBox1);

              
                //--------------------------------------------
                //move corner controllers
                UpdateControllerBoxes(box);
            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;
                controllerBox1.Visible = false;
                controllerBox1.TargetBox = null;
            };
        }


        void SetupControllerBoxProperties(UIControllerBox controllerBox)
        {
            //for controller box  
            controllerBox.MouseDrag += (s, e) =>
            {
                Point pos = controllerBox.Position;
                int newX = pos.X + e.XDiff;
                int newY = pos.Y + e.YDiff;
                controllerBox.SetLocation(newX, newY);


                //also move controller box?
                int j = _cornerControllers.Count;
                for (int i = 0; i < j; ++i)
                {
                    UIControllerBox corner = _cornerControllers[i];
                    Point p2 = corner.Position;
                    int newX2 = p2.X + e.XDiff;
                    int newY2 = p2.Y + e.YDiff;
                    corner.SetLocation(newX2, newY2);
                }
                var targetBox = controllerBox.TargetBox;
                if (targetBox != null)
                {
                    //move target box too
                    targetBox.SetLocation(newX + 5, newY + 5);
                }

                e.CancelBubbling = true;
            };
        }
        static void SetupControllerBoxProperties2(UIControllerBox cornerBox)
        {
            //for controller box  
            cornerBox.MouseDrag += (s, e) =>
            {
                Point pos = cornerBox.Position;
                int newX = pos.X + e.XDiff;
                int newY = pos.Y + e.YDiff;
                cornerBox.SetLocation(newX, newY);
                //var targetBox = cornerBox.TargetBox;
                //if (targetBox != null)
                //{
                //    //move target box too
                //    targetBox.SetLocation(newX + 5, newY + 5);
                //}
                e.CancelBubbling = true;
            };
        }
        //-----------------------------------------------------------------
        class UIControllerBox : LayoutFarm.CustomWidgets.EaseBox
        {
            public UIControllerBox(int w, int h)
                : base(w, h)
            {
            }
            public LayoutFarm.UI.UIBox TargetBox
            {
                get;
                set;
            }
            public override void Walk(UIVisitor visitor)
            {
                visitor.BeginElement(this, "ctrlbox");
                this.Describe(visitor);
                visitor.EndElement();
            }
        }
    }
}



