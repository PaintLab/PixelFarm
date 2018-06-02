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
        public int Index { get; set; }
    }



    public class RectBoxController : UIElement
    {
        UIControllerBox _boxLeftTop = new UIControllerBox(20, 20);
        UIControllerBox _boxLeftBottom = new UIControllerBox(20, 20);
        UIControllerBox _boxRightTop = new UIControllerBox(20, 20);
        UIControllerBox _boxRightBottom = new UIControllerBox(20, 20);
        UIControllerBox controllerBox1 = new UIControllerBox(40, 40);
        List<UIControllerBox> _controls = new List<UIControllerBox>();

        SimpleBox _simpleBox;
        bool _hasPrimRenderE;
        public RectBoxController()
        {
            _simpleBox = new SimpleBox(10, 10);
            _simpleBox.BackColor = Color.Transparent;//*** 
        }
        //-------------
        public override void InvalidateGraphics()
        {
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
        }
        protected override bool HasReadyRenderElement
        {
            get { return _hasPrimRenderE; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            _hasPrimRenderE = true;
            return _simpleBox.GetPrimaryRenderElement(rootgfx);
        }
        public override void Walk(UIVisitor visitor)
        {

        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return _simpleBox.CurrentPrimaryRenderElement; }
        }
        //-------------
        public void Focus()
        {
            controllerBox1.AcceptKeyboardFocus = true;
            controllerBox1.Focus();
        }

        public void UpdateControllerBoxes(LayoutFarm.UI.UIBox box)
        {

            //move controller here 
            controllerBox1.SetLocationAndSize(box.Left - 5, box.Top - 5,
                                     box.Width + 10, box.Height + 10);
            controllerBox1.Visible = true;
            controllerBox1.TargetBox = box;


            {
                //left-top
                UIControllerBox ctrlBox = _boxLeftTop;
                ctrlBox.SetLocationAndSize(box.Left - 5, box.Top - 5, 5, 5);
                ctrlBox.TargetBox = box;
                ctrlBox.Visible = true;
            }
            {
                //right-top
                UIControllerBox ctrlBox = _boxRightTop;
                ctrlBox.SetLocationAndSize(box.Left + box.Width, box.Top - 5, 5, 5);
                ctrlBox.TargetBox = box;
                ctrlBox.Visible = true;
            }
            {
                //left-bottom
                UIControllerBox ctrlBox = _boxLeftBottom;
                ctrlBox.SetLocationAndSize(box.Left - 5, box.Top + box.Height, 5, 5);
                ctrlBox.TargetBox = box;
                ctrlBox.Visible = true;
            }
            {
                //right-bottom
                UIControllerBox ctrlBox = _boxRightBottom;
                ctrlBox.SetLocationAndSize(box.Left + box.Width, box.Top + box.Height, 5, 5);
                ctrlBox.TargetBox = box;
                ctrlBox.Visible = true;
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
            _simpleBox.AddChild(controllerBox1);
            //------------

            _boxLeftTop = new UIControllerBox(20, 20);
            SetupCornerBoxController(_boxLeftTop);
            _boxLeftTop.MouseDrag += (s1, e1) =>
            {
                //move other boxes ...
                UIBox target1 = _boxLeftTop.TargetBox;
                //update target
                target1.SetLocationAndSize(_boxLeftTop.Right,
                                      _boxLeftTop.Bottom,
                                      _boxRightTop.Left - _boxLeftTop.Right,
                                      _boxRightBottom.Top - _boxLeftTop.Bottom);
                //update other controller
                UpdateControllerBoxes(target1);

            };
            _simpleBox.AddChild(_boxLeftTop);
            //------------
            _boxLeftBottom = new UIControllerBox(20, 20);
            SetupCornerBoxController(_boxLeftBottom);
            _boxLeftBottom.MouseDrag += (s1, e1) =>
            {
                UIBox target1 = _boxLeftBottom.TargetBox;
                //update target
                target1.SetLocationAndSize(_boxLeftBottom.Right,
                                      _boxLeftTop.Bottom,
                                      _boxRightTop.Left - _boxLeftBottom.Right,
                                      _boxLeftBottom.Top - _boxLeftTop.Bottom);
                //update other controller
                UpdateControllerBoxes(target1);
            };
            _simpleBox.AddChild(_boxLeftBottom);
            //------------ 

            _boxRightTop = new UIControllerBox(20, 20);
            SetupCornerBoxController(_boxRightTop);
            _boxRightTop.MouseDrag += (s1, e1) =>
            {
                UIBox target1 = _boxRightTop.TargetBox;
                //update target
                target1.SetLocationAndSize(_boxLeftTop.Right,
                                      _boxRightTop.Bottom,
                                      _boxRightTop.Left - _boxLeftTop.Right,
                                      _boxRightBottom.Top - _boxRightTop.Bottom);
                //update other controller
                UpdateControllerBoxes(target1);
            };
            _simpleBox.AddChild(_boxRightTop);

            //------------ 
            _boxRightBottom = new UIControllerBox(20, 20);
            SetupCornerBoxController(_boxRightBottom);
            _boxRightBottom.MouseDrag += (s1, e1) =>
            {
                UIBox target1 = _boxRightBottom.TargetBox;
                //update target
                target1.SetLocationAndSize(_boxLeftTop.Right,
                                      _boxLeftTop.Bottom,
                                      _boxRightBottom.Left - _boxLeftTop.Right,
                                      _boxRightBottom.Top - _boxLeftTop.Bottom);
                //update other controller
                UpdateControllerBoxes(target1);
            };
            _simpleBox.AddChild(_boxRightBottom);
        }

        public EaseBox ControllerBoxMain
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


    public class PolygonController : UIElement
    {
        SimpleBox _simpleBox;
        bool _hasPrimRenderE;
        List<PointF> _points = new List<PointF>();
        List<UIControllerBox> _controls = new List<UIControllerBox>();

        VertexStore _vxs;


        public PolygonController()
        {

            _simpleBox = new SimpleBox(10, 10);
            _simpleBox.NeedClipArea = false;
            _simpleBox.BackColor = Color.Transparent;//*** 

        }
        //-------------
        public override void InvalidateGraphics()
        {
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
        }
        protected override bool HasReadyRenderElement
        {
            get { return _hasPrimRenderE; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            _hasPrimRenderE = true;
            return _simpleBox.GetPrimaryRenderElement(rootgfx);
        }
        public override void Walk(UIVisitor visitor)
        {

        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return _simpleBox.CurrentPrimaryRenderElement; }
        }

        public void SetPosition(int x, int y)
        {
            //TODO: review here again***
            //temp fix for invalidate area of overlap children
            _simpleBox.InvalidateOuterGraphics();
            foreach (var ctrl in _controls)
            {
                ctrl.InvalidateOuterGraphics();
            }
            _simpleBox.SetLocation(x, y);
        }
        public void UpdateControlPoints(VertexStore vxs)
        {
            //1. we remove existing point from root
            _vxs = vxs;
            int m = _controls.Count;
            for (int n = 0; n < m; ++n)
            {
                _controls[n].RemoveSelf();
            }
            _controls.Clear(); //***
            _points.Clear();

            //2. create new control points...

            int j = vxs.Count;
            for (int i = 0; i < j; ++i)
            {
                var cmd = vxs.GetVertex(i, out double x, out double y);
                switch (cmd)
                {
                    case PixelFarm.Agg.VertexCmd.MoveTo:
                        {

                            var ctrlPoint = new UIControllerBox(8, 8);
                            ctrlPoint.Index = i;
                            ctrlPoint.SetLocation((int)x, (int)y);
                            SetupCornerBoxController(ctrlPoint);
                            _controls.Add(ctrlPoint);
                            _simpleBox.AddChild(ctrlPoint);
                        }
                        break;
                    case PixelFarm.Agg.VertexCmd.LineTo:
                        {
                            var ctrlPoint = new UIControllerBox(8, 8);
                            ctrlPoint.Index = i;
                            ctrlPoint.SetLocation((int)x, (int)y);
                            SetupCornerBoxController(ctrlPoint);
                            _controls.Add(ctrlPoint);
                            _simpleBox.AddChild(ctrlPoint);
                        }
                        break;
                    case PixelFarm.Agg.VertexCmd.Close:
                        break;
                }
            }

        }
        void SetupCornerBoxController(UIControllerBox box)
        {
            Color c = KnownColors.FromKnownColor(KnownColor.Orange);
            box.BackColor = new Color(100, c.R, c.G, c.B);

            //controllerBox1.dbugTag = 3;
            box.Visible = true;
            SetupCornerProperties(box);
            //viewport.AddContent(box);
            //
            _controls.Add(box);
        }

        void SetupCornerProperties(UIControllerBox cornerBox)
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

                //then update the vxs shape
                _vxs.ReplaceVertex(cornerBox.Index, newX, newY);

            };
        }
    }

}