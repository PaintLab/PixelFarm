//-----------------------------------------------------------------
//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
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
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "ctrlbox");
            this.Describe(visitor);
            visitor.EndElement();
        }
        public int Index { get; set; }
        public MoveDirection MoveDirection { get; set; }
    }
    enum MoveDirection
    {
        Both,
        XAxis,
        YAxis,

    }


    public class RectBoxController : UIElement
    {

        //TODO: review how to configure the controller box ...

        //-------------
        //corners
        UIControllerBox _boxLeftTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _boxLeftBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _boxRightTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _boxRightBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        //-------------

        //mid 
        UIControllerBox _midLeftSide = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _midRightSide = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _midTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _midBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow };

        //-------------

        UIControllerBox _centralBox = new UIControllerBox(40, 40);
        List<UIControllerBox> _controls = new List<UIControllerBox>();

        Box _groundBox;
        bool _hasPrimRenderE;
        public RectBoxController()
        {
            _groundBox = new Box(10, 10);
            _groundBox.BackColor = Color.Transparent;//*** 
            _groundBox.NeedClipArea = false;
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
            return _groundBox.GetPrimaryRenderElement(rootgfx);
        }
        public override void Walk(UIVisitor visitor)
        {

        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return _groundBox.CurrentPrimaryRenderElement; }
        }
        //-------------

        public void SetPosition(int x, int y)
        {
            //TODO: review here again***
            //temp fix for invalidate area of overlap children
            _groundBox.InvalidateOuterGraphics();
            foreach (var ctrl in _controls)
            {
                ctrl.InvalidateOuterGraphics();
            }
            _groundBox.SetLocation(x, y);
        }

        public override void Focus()
        {
            _centralBox.AcceptKeyboardFocus = true;
            _centralBox.Focus();
        }
        public void UpdateControllerBoxes(LayoutFarm.UI.AbstractRectUI targetBox)
        {

            //move controller here 
            _centralBox.SetLocationAndSize(
                            targetBox.Left - 5, targetBox.Top - 5,
                            targetBox.Width + 10, targetBox.Height + 10);
            _centralBox.Visible = true;
            _centralBox.TargetBox = targetBox;
            //------------
            //corners 
            //------------
            {
                //left-top
                UIControllerBox ctrlBox = _boxLeftTop;
                ctrlBox.SetLocationAndSize(targetBox.Left - 5, targetBox.Top - 5, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                //right-top
                UIControllerBox ctrlBox = _boxRightTop;
                ctrlBox.SetLocationAndSize(targetBox.Left + targetBox.Width, targetBox.Top - 5, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                //left-bottom
                UIControllerBox ctrlBox = _boxLeftBottom;
                ctrlBox.SetLocationAndSize(targetBox.Left - 5, targetBox.Top + targetBox.Height, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                //right-bottom
                UIControllerBox ctrlBox = _boxRightBottom;
                ctrlBox.SetLocationAndSize(targetBox.Left + targetBox.Width, targetBox.Top + targetBox.Height, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }


            //------------
            //mid-edges
            //------------
            {
                //left 
                UIControllerBox ctrlBox = _midLeftSide;
                ctrlBox.SetLocationAndSize(targetBox.Left - 5, (targetBox.Top - 5) + (targetBox.Height / 2), 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                //right 
                UIControllerBox ctrlBox = _midRightSide;
                ctrlBox.SetLocationAndSize(targetBox.Left + targetBox.Width, targetBox.Top - 5 + (targetBox.Height / 2), 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                //top
                UIControllerBox ctrlBox = _midTop;
                ctrlBox.SetLocationAndSize(targetBox.Left - 5 + (targetBox.Width / 2), targetBox.Top - 5, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                // bottom
                UIControllerBox ctrlBox = _midBottom;
                ctrlBox.SetLocationAndSize(targetBox.Left + (targetBox.Width / 2), targetBox.Bottom + 5, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }


        }
        void InitCornerControlBoxes()
        {

            _boxLeftTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow, Visible = false };
            SetupCorner_Controller(_boxLeftTop);
            _boxLeftTop.MouseDrag += (s1, e1) =>
            {
                //move other boxes ...
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                //update target
                if (target1 != null)
                {
                    target1.SetLocationAndSize(
                                        _boxLeftTop.Right,
                                       _boxLeftTop.Bottom,
                                          _boxRightTop.Left - _boxLeftTop.Right,
                                          _boxRightBottom.Top - _boxLeftTop.Bottom);
                    //update other controller
                    UpdateControllerBoxes(target1);
                }

            };
            _groundBox.AddChild(_boxLeftTop);
            //------------
            _boxLeftBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow, Visible = false };
            SetupCorner_Controller(_boxLeftBottom);
            _boxLeftBottom.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxLeftBottom.TargetBox;
                //update target
                if (target1 != null)
                {
                    target1.SetLocationAndSize(_boxLeftBottom.Right,
                                    _boxLeftTop.Bottom,
                                    _boxRightTop.Left - _boxLeftBottom.Right,
                                    _boxLeftBottom.Top - _boxLeftTop.Bottom);
                    //update other controller
                    UpdateControllerBoxes(target1);
                }

            };
            _groundBox.AddChild(_boxLeftBottom);
            //------------ 

            _boxRightTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow, Visible = false };
            SetupCorner_Controller(_boxRightTop);
            _boxRightTop.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxRightTop.TargetBox;
                //update target
                if (target1 != null)
                {
                    target1.SetLocationAndSize(
                                          _boxLeftTop.Right,
                                          _boxRightTop.Bottom,
                                          _boxRightTop.Left - _boxLeftTop.Right,
                                          _boxRightBottom.Top - _boxRightTop.Bottom);
                    //update other controller
                    UpdateControllerBoxes(target1);
                }
            };
            _groundBox.AddChild(_boxRightTop);

            //------------ 
            _boxRightBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow, Visible = false };
            SetupCorner_Controller(_boxRightBottom);
            _boxRightBottom.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxRightBottom.TargetBox;
                //update target
                if (target1 != null)
                {
                    target1.SetLocationAndSize(_boxLeftTop.Right,
                                      _boxLeftTop.Bottom,
                                      _boxRightBottom.Left - _boxLeftTop.Right,
                                      _boxRightBottom.Top - _boxLeftTop.Bottom);
                    //update other controller
                    UpdateControllerBoxes(target1);
                }

            };
            _groundBox.AddChild(_boxRightBottom);
            //------------ 

        }
        void SetupEdge_Controller(UIControllerBox box)
        {
            Color c = KnownColors.FromKnownColor(KnownColor.Blue);
            box.BackColor = c;// new Color(200, c.R, c.G, c.B);
            box.SetLocation(200, 200);
            box.Visible = true;
            switch (box.MoveDirection)
            {
                case MoveDirection.XAxis:
                    {
                        box.MouseDrag += (s, e) =>
                        {
                            Point pos = box.Position;
                            box.SetLocation(pos.X + e.XDiff, pos.Y);
                            //var targetBox = cornerBox.TargetBox;
                            //if (targetBox != null)
                            //{
                            //    //move target box too
                            //    targetBox.SetLocation(newX + 5, newY + 5);
                            //}

                            e.CancelBubbling = true;
                        };
                    }
                    break;
                case MoveDirection.YAxis:
                    {
                        box.MouseDrag += (s, e) =>
                        {
                            Point pos = box.Position;
                            box.SetLocation(pos.X, pos.Y + e.YDiff);
                            //var targetBox = cornerBox.TargetBox;
                            //if (targetBox != null)
                            //{
                            //    //move target box too
                            //    targetBox.SetLocation(newX + 5, newY + 5);
                            //}

                            e.CancelBubbling = true;
                        };
                    }
                    break;
            }

            _controls.Add(box);
        }
        void InitEdgeControlBoxes()
        {
            _midLeftSide = new UIControllerBox(20, 20) { MoveDirection = MoveDirection.XAxis, BackColor = Color.Blue, Visible = false };
            SetupEdge_Controller(_midLeftSide);
            _midLeftSide.MouseDrag += (s1, e1) =>
            {
                //move other boxes ...
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                //update target
                if (target1 != null)
                {
                    //change left side x and width
                    int xdiff = _midLeftSide.Right - target1.Left;
                    target1.SetLocationAndSize(
                                          target1.Left + xdiff,
                                          target1.Top,
                                          target1.Width - xdiff,
                                          target1.Height);
                    //update other controller
                    UpdateControllerBoxes(target1);
                }

            };
            _groundBox.AddChild(_midLeftSide);
            //------------
            _midRightSide = new UIControllerBox(20, 20) { MoveDirection = MoveDirection.XAxis, BackColor = Color.Blue, Visible = false };
            SetupEdge_Controller(_midRightSide);
            _midRightSide.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                //change left side x and width
                int xdiff = _midRightSide.Left - target1.Right;
                target1.SetLocationAndSize(
                                        target1.Left,
                                        target1.Top,
                                        target1.Width + xdiff,
                                        target1.Height);
                //update other controller
                UpdateControllerBoxes(target1);

            };
            _groundBox.AddChild(_midRightSide);
            //------------ 

            _midTop = new UIControllerBox(20, 20) { MoveDirection = MoveDirection.YAxis, BackColor = Color.Blue, Visible = false };
            SetupEdge_Controller(_midTop);
            _midTop.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                int ydiff = target1.Top - _midTop.Bottom;
                target1.SetLocationAndSize(
                                        target1.Left,
                                        target1.Top - ydiff,
                                        target1.Width,
                                        target1.Height + ydiff);
                //update other controller
                UpdateControllerBoxes(target1);
            };
            _groundBox.AddChild(_midTop);

            //------------ 
            _midBottom = new UIControllerBox(20, 20) { MoveDirection = MoveDirection.YAxis, BackColor = Color.Blue, Visible = false };
            SetupEdge_Controller(_midBottom);
            _midBottom.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                int ydiff = _midBottom.Top - target1.Bottom;

                target1.SetLocationAndSize(
                                        target1.Left,
                                        target1.Top,
                                        target1.Width,
                                        target1.Height + ydiff);
                //update other controller
                UpdateControllerBoxes(target1);
            };
            _groundBox.AddChild(_midBottom);
            //------------ 
        }
        public void Init()
        {
            //------------
            _centralBox = new UIControllerBox(40, 40);
            {
                Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
                _centralBox.BackColor = new Color(100, c.R, c.G, c.B);
                _centralBox.SetLocation(200, 200);
                //controllerBox1.dbugTag = 3;
                _centralBox.Visible = false;
                SetupControllerBoxProperties(_centralBox);
                //viewport.AddChild(controllerBox1);
                _controls.Add(_centralBox);
            }
            _groundBox.AddChild(_centralBox);
            //------------
            InitCornerControlBoxes();
            InitEdgeControlBoxes();
            //------------
        }

        public AbstractBox ControllerBoxMain
        {
            get { return _centralBox; }
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

        void SetupCorner_Controller(UIControllerBox box)
        {
            Color c = KnownColors.FromKnownColor(KnownColor.Orange);
            box.BackColor = c;// new Color(200, c.R, c.G, c.B);
            box.SetLocation(200, 200);
            box.Visible = true;
            box.MouseDrag += (s, e) =>
            {
                Point pos = box.Position;
                int newX = pos.X + e.XDiff;
                int newY = pos.Y + e.YDiff;
                box.SetLocation(newX, newY);
                //var targetBox = cornerBox.TargetBox;
                //if (targetBox != null)
                //{
                //    //move target box too
                //    targetBox.SetLocation(newX + 5, newY + 5);
                //}

                e.CancelBubbling = true;
            };
            _controls.Add(box);
        }
    }


    public class PolygonController : UIElement
    {
        Box _simpleBox;
        bool _hasPrimRenderE;
        List<PointF> _points = new List<PointF>();
        List<UIControllerBox> _controls = new List<UIControllerBox>();
        public PolygonController()
        {

            _simpleBox = new Box(10, 10);
            _simpleBox.TransparentAllMouseEvents = true;
            _simpleBox.NeedClipArea = false;
            //_simpleBox.BackColor = Color.Transparent;//*** 
#if DEBUG
            _simpleBox.BackColor = Color.Blue;//***  
#endif
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

        IUIEventListener _ui;
        public void SetTargetUISprite(IUIEventListener ui)
        {
            _ui = ui;
        }
        PixelFarm.Drawing.VertexStore _svgPath;
        public void UpdateControlPoints(PixelFarm.Drawing.VertexStore svgPath, float offsetX = 0, float offsetY = 0)
        {
            //1. we remove existing point from root

            _svgPath = svgPath;
            VertexStore vxs = svgPath;

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

                switch (vxs.GetVertex(i, out double x, out double y))
                {
                    case PixelFarm.CpuBlit.VertexCmd.MoveTo:
                        {

                            var ctrlPoint = new UIControllerBox(8, 8);
                            ctrlPoint.Index = i;
                            ctrlPoint.SetLocation((int)(x + offsetX), (int)(y + offsetY));
                            SetupCornerBoxController(ctrlPoint);
                            _controls.Add(ctrlPoint);
                            _simpleBox.AddChild(ctrlPoint);
                        }
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.LineTo:
                        {
                            var ctrlPoint = new UIControllerBox(8, 8);
                            ctrlPoint.Index = i;
                            ctrlPoint.SetLocation((int)(x + offsetX), (int)(y + offsetY));
                            SetupCornerBoxController(ctrlPoint);
                            _controls.Add(ctrlPoint);
                            _simpleBox.AddChild(ctrlPoint);
                        }
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.Close:
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
            //viewport.AddChild(box);
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

                //---------------------------------
                _simpleBox.InvalidateOuterGraphics();
                foreach (var ctrl in _controls)
                {
                    ctrl.InvalidateOuterGraphics();
                }

                //then update the vxs shape

                _svgPath.ReplaceVertex(cornerBox.Index, newX, newY);

                //PixelFarm.CpuBlit.SvgPart.SetResolvedObject(_svgPath, null);//clear

                //***
                _ui?.HandleElementUpdate();

            };
        }
    }

}