//-----------------------------------------------------------------
//Apache2, 2014-2018, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
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
    public class RectBoxController
    {
        UIControllerBox _boxLeftTop = new UIControllerBox(20, 20);
        UIControllerBox _boxLeftBottom = new UIControllerBox(20, 20);
        UIControllerBox _boxRightTop = new UIControllerBox(20, 20);
        UIControllerBox _boxRightBottom = new UIControllerBox(20, 20);
        UIControllerBox controllerBox1 = new UIControllerBox(40, 40);
        List<UIControllerBox> _controls = new List<UIControllerBox>();

        public RectBoxController()
        {

        }



        public void UpdateControllerBoxes(LayoutFarm.UI.UIBox box)
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

        public IEnumerable<UIBox> GetControllerIter()
        {
            foreach (var box in _controls)
            {
                yield return box;
            }
        }
        public void Init()
        {
            //------------
            controllerBox1 = new UIControllerBox(40, 40);
            {
                Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
                controllerBox1.BackColor = new Color(100, c.R, c.G, c.B);
                controllerBox1.SetLocation(200, 200);
                //controllerBox1.dbugTag = 3;
                controllerBox1.Visible = false;
                SetupControllerBoxProperties(controllerBox1);
                //viewport.AddContent(controllerBox1);
                _controls.Add(controllerBox1);
            }
            //------------

            _boxLeftTop = new UIControllerBox(20, 20);
            SetupCornerBoxController(_boxLeftTop);
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
            SetupCornerBoxController(_boxLeftBottom);
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
            SetupCornerBoxController(_boxRightTop);
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
            SetupCornerBoxController(_boxRightBottom);
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
        }

        public UIBox ControllerBoxMain
        {
            get { return controllerBox1; }
        }





        void SetupControllerBoxProperties(UIControllerBox controllerBox)
        {
            //for controller box  
            controllerBox.MouseDrag += (s, e) =>
            {
                Point pos = controllerBox.Position;
                int newX = pos.X + e.XDiff;
                int newY = pos.Y + e.YDiff;

                //also move controller box?
                int j = _controls.Count;
                for (int i = 0; i < j; ++i)
                {
                    UIControllerBox corner = _controls[i];
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
        void SetupCornerBoxController(UIControllerBox box)
        {
            Color c = KnownColors.FromKnownColor(KnownColor.Orange);
            box.BackColor = new Color(100, c.R, c.G, c.B);
            box.SetLocation(200, 200);
            //controllerBox1.dbugTag = 3;
            box.Visible = false;
            SetupControllerBoxProperties2(box);
            //viewport.AddContent(box);
            //
            _controls.Add(box);
        }
    }
}