//Apache2, 2014-present, WinterDev
using System;
using System.Collections.Generic;

using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using PaintLab.Svg;

using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
using LayoutFarm.CustomWidgets;

namespace LayoutFarm
{
    [DemoNote("4.3. Demo_Perspective")]
    public class Demo_Perspective : App
    {
        VgVisualElement _vgVisElem;
        MyTestSprite _mySprite;
        QuadWidgetControl _quadControl;
        Box _background;
        RectangleF _lionBounds;
        Box _cmdBiliear;
        public Demo_Perspective()
        {
        }
        protected override void OnStart(AppHost host)
        {
            _background = new Box(800, 600);
            _background.BackColor = Color.White;
            host.AddChild(_background);




            //---------------------------
            _vgVisElem = VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\lion.svg").VgRootElem;
            _mySprite = new MyTestSprite(_vgVisElem);
            //var evListener = new GeneralEventListener(); 
            //evListener.MouseDrag += (s, e) =>
            //{
            //    if (e.Ctrl)
            //    {
            //        //TODO: 
            //        //classic Agg's move and rotate                         

            //    }
            //    else
            //    {   //just move
            //        _mySprite.SetLocation(_mySprite.Left + e.XDiff, _mySprite.Top + e.YDiff);
            //    }
            //};
            //_mySprite.AttachExternalEventListener(evListener);


            var rectBounds = _vgVisElem.GetRectBounds();
            _quadControl = new QuadWidgetControl();
            _quadControl.ShapeUpdated += _quadControl_ShapeUpdated;
            //
            _quadControl.SetupCorners(
                new Quad2f(
                  (float)rectBounds.Left,
                  (float)rectBounds.Bottom,
                  (float)rectBounds.Width,
                  (float)rectBounds.Height));

            _lionBounds = new RectangleF((float)rectBounds.Left,
                  (float)rectBounds.Bottom,
                  (float)rectBounds.Width,
                  (float)rectBounds.Height);

            host.AddChild(_mySprite);
            host.AddChild(_quadControl);


            //--------------------
            _cmdBiliear = new Box(30, 30);
            _cmdBiliear.SetLocation(400, 20);
            _cmdBiliear.BackColor = Color.Yellow;
            _cmdBiliear.MouseDown += (s, e) =>
            {
                if (_useBilinear)
                {
                    _cmdBiliear.BackColor = Color.Yellow;
                    _useBilinear = false;
                }
                else
                {
                    _cmdBiliear.BackColor = Color.Red;
                    _useBilinear = true;
                }

            };
            host.AddChild(_cmdBiliear);
        }
        bool _useBilinear;

        double[] _quadCorners = new double[8];
        private void _quadControl_ShapeUpdated(QuadWidgetControl sender, EventArgs arg)
        {
            //update shape of sprite

            //transform from original lionBounds to quadPolygon 
            Quad2f quadCorners = _quadControl.GetQuadCorners();
            _quadCorners[0] = quadCorners.left_top_x;
            _quadCorners[1] = quadCorners.left_top_y;
            _quadCorners[2] = quadCorners.right_top_x;
            _quadCorners[3] = quadCorners.right_top_y;
            _quadCorners[4] = quadCorners.right_bottom_x;
            _quadCorners[5] = quadCorners.right_bottom_y;
            _quadCorners[6] = quadCorners.left_bottom_x;
            _quadCorners[7] = quadCorners.left_bottom_y;


            //this is bilinear transformation
            if (_useBilinear)
            {
                Bilinear txBilinear = Bilinear.RectToQuad(
                    _lionBounds.Left,
                    _lionBounds.Top,
                    _lionBounds.Right,
                    _lionBounds.Bottom,
                   _quadCorners);

                if (txBilinear.IsValid)
                {
                    SpriteShape spriteShape = _mySprite.GetSpriteShape();
                    spriteShape.ResetTransform();
                    spriteShape.ApplyTransform(txBilinear);
                }
            }
            else
            {
                Perspective perspective = new Perspective(
                     _lionBounds.Left,
                    _lionBounds.Top,
                    _lionBounds.Right,
                    _lionBounds.Bottom,
                    _quadCorners);
                if (perspective.IsValid)
                {
                    SpriteShape spriteShape = _mySprite.GetSpriteShape();
                    spriteShape.ResetTransform();
                    spriteShape.ApplyTransform(perspective);
                }
            }
        }
    }


    class QuadWidgetRenderElement : RenderElement
    {
        List<CustomRenderBox> _controlPoints = new List<CustomRenderBox>();
        RectangleF _approxRectBounds;
        public QuadWidgetRenderElement() : base(300, 300)
        {
            _approxRectBounds = new RectangleF(0, 0, 300, 300);
            this.NeedClipArea = false; //special for quad control
        }
        public void AddChild(RenderElement renderE)
        {

        }
        public void AddCustomControlBox(CustomRenderBox ctrlBox)
        {
            _controlPoints.Add(ctrlBox);

            //important: don't forget to set parent link
            //this will route update graphics msg from client to parent
            CustomRenderBox.SetParentLink(ctrlBox, this);


        }
        public void ClearControlBoxes()
        {
            _controlPoints.Clear();
            InvalidateGraphics();
        }


        public override bool HasCustomHitTest => true; //set to true, CustomHitTest() will be called
        protected override bool CustomHitTest(HitChain hitChain)
        {
            if (_approxRectBounds.Contains(hitChain.TestPoint))
            {
                //in rgn
                //check hit test on each child
                for (int i = _controlPoints.Count - 1; i >= 0; --i)
                {
                    CustomRenderBox ctrlPoint = _controlPoints[i];
                    if (ctrlPoint.HitTestCore(hitChain))
                    {
                        hitChain.AddHitObject(ctrlPoint);
                        return true;
                    }
                }
            }
            return false;
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            base.ChildrenHitTestCore(hitChain);
        }
        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {

            //1. save canvas origin, (don't forget to restore this)
            int enter_canvas_x = d.OriginX;
            int enter_canvas_y = d.OriginY;

            //d.FillRectangle(Color.FromArgb(100, Color.Yellow),
            //    _approxRectBounds.Left,
            //    _approxRectBounds.Top, 
            //    _approxRectBounds.Width,
            //    _approxRectBounds.Height);

            foreach (CustomRenderBox renderE in _controlPoints)
            {
                int x = renderE.X;
                int y = renderE.Y;

                //2. set canvas origin
                d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
                //3. offset update area (see opposite direction)
                updateArea.Offset(-x, -y);
                //4.
                RenderElement.Render(renderE, d, updateArea);
                //5. restore only update area back
                updateArea.Offset(x, y);
            }
            //restore enter canvas x, y
            d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);

            ////lets draw lines that link  control points
            int j = _controlPoints.Count;

            float strokeW = d.StrokeWidth;
            Color strokeColor = d.StrokeColor;
            d.StrokeWidth = 2;
            d.StrokeColor = Color.Black;


            for (int i = 1; i < j; ++i)
            {
                CustomRenderBox b_0 = _controlPoints[i - 1];
                CustomRenderBox b_1 = _controlPoints[i];
                d.DrawLine(b_0.X, b_0.Y, b_1.X, b_1.Y);
            }
            {
                CustomRenderBox b_0 = _controlPoints[j - 1];
                CustomRenderBox b_1 = _controlPoints[0];
                d.DrawLine(b_0.X, b_0.Y, b_1.X, b_1.Y);
            }
            //and the last one
            d.StrokeColor = strokeColor;
            d.StrokeWidth = strokeW;
        }

        public void UpdateRectBounds()
        {
            //find max bounds of this control
            var rectBoundsAcc = new RectBoundsAccum();
            rectBoundsAcc.Init();
            int j = _controlPoints.Count;
            for (int i = 0; i < j; ++i)
            {
                CustomRenderBox p = _controlPoints[i];
                rectBoundsAcc.Update(p.X, p.Y);
            }
            //
            _approxRectBounds = rectBoundsAcc.ToRectF();
            _approxRectBounds.Width += 10;
            _approxRectBounds.Height += 10;
        }
    }

    class QuadWidgetControl : UIElement
    {
        QuadWidgetRenderElement _quadRenderE;
        List<PointF> _corners = new List<PointF>();
        List<Box> _cornerBoxes = new List<Box>();
        bool _cornersAreNotUpdated = false;

        public event UIEventHandler<QuadWidgetControl, EventArgs> ShapeUpdated;

        public QuadWidgetControl()
        {

        }
        public void SetupCorners(Quad2f quad2f)
        {
            //set bounds
            //create child control point for 
            _corners.Clear();
            _corners.Add(new PointF(quad2f.left_top_x, quad2f.left_top_y));
            _corners.Add(new PointF(quad2f.right_top_x, quad2f.right_top_y));
            _corners.Add(new PointF(quad2f.right_bottom_x, quad2f.right_bottom_y));
            _corners.Add(new PointF(quad2f.left_bottom_x, quad2f.left_bottom_y));
            //from the point we create a rect child 
            _cornerBoxes.Clear();


            int j = _corners.Count;
            for (int i = 0; i < j; ++i)
            {
                Box cornerBox = new Box(10, 10);
                PointF p = _corners[i];
                cornerBox.SetLocation((int)p.X, (int)p.Y);
                cornerBox.MouseDrag += CornerBox_MouseDrag;
                cornerBox.NeedClipArea = false; //special for control box

                _cornerBoxes.Add(cornerBox);
            }
        }
        public Quad2f GetQuadCorners()
        {
            if (_cornersAreNotUpdated)
            {
                //update
                _corners.Clear();
                foreach (Box b in _cornerBoxes)
                {
                    _corners.Add(new PointF(b.Left, b.Top));
                }
            }

            Box b0 = _cornerBoxes[0];
            Box b1 = _cornerBoxes[1];
            Box b2 = _cornerBoxes[2];
            Box b3 = _cornerBoxes[3];
            var q = new PixelFarm.CpuBlit.VertexProcessing.Quad2f();
            q.left_top_x = b0.Left; q.left_top_y = b0.Top;
            q.right_top_x = b1.Left; q.right_top_y = b1.Top;
            q.right_bottom_x = b2.Left; q.right_bottom_y = b2.Top;
            q.left_bottom_x = b3.Left; q.left_bottom_y = b3.Top;
            return q;
        }
        private void CornerBox_MouseDrag(object sender, UIMouseMoveEventArgs e)
        {
            //move only this box
            if (sender is Box box)
            {
                box.SetLocation(box.Left + e.XDiff, box.Top + e.YDiff);
                _quadRenderE.UpdateRectBounds();
                _cornersAreNotUpdated = true;
            }
            ShapeUpdated(this, EventArgs.Empty);
        }

        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            base.OnMouseDown(e);
        }
        public override void InvalidateGraphics()
        {
            if (this.HasReadyRenderElement)
            {
                //invalidate 'bubble' rect 
                //is (0,0,w,h) start invalidate from current primary render element
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
        }
        protected override bool HasReadyRenderElement => _quadRenderE != null;
        public override RenderElement CurrentPrimaryRenderElement => _quadRenderE;
        public override RenderElement GetPrimaryRenderElement()
        {
            if (_quadRenderE == null)
            {
                _quadRenderE = new QuadWidgetRenderElement();
                int j = _cornerBoxes.Count;
                for (int i = 0; i < j; ++i)
                {
                    Box b = _cornerBoxes[i];
                    _quadRenderE.AddCustomControlBox((CustomRenderBox)b.GetPrimaryRenderElement());
                }

                _quadRenderE.SetController(this);
                _quadRenderE.UpdateRectBounds();
            }
            return _quadRenderE;
        }
    }

}