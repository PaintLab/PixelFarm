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
    }



    public class RectBoxController : UIElement
    {
        UIControllerBox _boxLeftTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _boxLeftBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _boxRightTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _boxRightBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow };

        UIControllerBox _centrolBox = new UIControllerBox(40, 40);
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

        public override void Focus()
        {
            _centrolBox.AcceptKeyboardFocus = true;
            _centrolBox.Focus();
        }
        public void UpdateControllerBoxes(LayoutFarm.UI.AbstractRectUI targetBox)
        {

            //move controller here 
            _centrolBox.SetLocationAndSize(targetBox.Left - 5, targetBox.Top - 5,
                                     targetBox.Width + 10, targetBox.Height + 10);
            _centrolBox.Visible = true;
            _centrolBox.TargetBox = targetBox;
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
        }

        public void Init()
        {
            //------------
            _centrolBox = new UIControllerBox(40, 40);
            {
                Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
                _centrolBox.BackColor = new Color(100, c.R, c.G, c.B);
                _centrolBox.SetLocation(200, 200);
                //controllerBox1.dbugTag = 3;
                _centrolBox.Visible = false;
                SetupControllerBoxProperties(_centrolBox);
                //viewport.AddChild(controllerBox1);
                _controls.Add(_centrolBox);
            }
            _groundBox.AddChild(_centrolBox);
            //------------

            _boxLeftTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow, Visible = false };
            SetupCornerBoxController(_boxLeftTop);
            _boxLeftTop.MouseDrag += (s1, e1) =>
            {
                //move other boxes ...
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                //update target
                if (target1 != null)
                {
                    target1.SetLocationAndSize(_boxLeftTop.Right,
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
            SetupCornerBoxController(_boxLeftBottom);
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
            SetupCornerBoxController(_boxRightTop);
            _boxRightTop.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxRightTop.TargetBox;
                //update target
                if (target1 != null)
                {
                    target1.SetLocationAndSize(_boxLeftTop.Right,
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
            SetupCornerBoxController(_boxRightBottom);
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
        }

        public AbstractBox ControllerBoxMain
        {
            get { return _centrolBox; }
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
            box.BackColor = c;// new Color(200, c.R, c.G, c.B);
            box.SetLocation(200, 200);
            //controllerBox1.dbugTag = 3;
            box.Visible = true;
            SetupControllerBoxProperties2(box);
            //viewport.AddChild(box);
            //
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
        public void UpdateControlPoints(PixelFarm.Drawing.VertexStore svgPath)
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
                var cmd = vxs.GetVertex(i, out double x, out double y);
                switch (cmd)
                {
                    case PixelFarm.CpuBlit.VertexCmd.MoveTo:
                        {

                            var ctrlPoint = new UIControllerBox(8, 8);
                            ctrlPoint.Index = i;
                            ctrlPoint.SetLocation((int)x, (int)y);
                            SetupCornerBoxController(ctrlPoint);
                            _controls.Add(ctrlPoint);
                            _simpleBox.AddChild(ctrlPoint);
                        }
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.LineTo:
                        {
                            var ctrlPoint = new UIControllerBox(8, 8);
                            ctrlPoint.Index = i;
                            ctrlPoint.SetLocation((int)x, (int)y);
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

                _ui.HandleElementUpdate();

            };
        }
    }

}