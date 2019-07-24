//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("3.3 Demo_ControllerBoxs2")]
    class Demo_ControllerBoxs2 : App
    {
        UIControllerBox _controllerBox1;
        protected override void OnStart(AppHost host)
        {
            //--------------------------------

            var bgbox = new LayoutFarm.CustomWidgets.Box(800, 600);
            bgbox.BackColor = Color.White;
            bgbox.SetLocation(0, 0);
            SetupBackgroundProperties(bgbox);
            host.AddChild(bgbox);
            //--------------------------------

            var box1 = new LayoutFarm.CustomWidgets.Box(150, 150);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            //box1.dbugTag = 1;
            SetupActiveBoxProperties(box1);
            host.AddChild(box1);
            //--------------------------------

            var box2 = new LayoutFarm.CustomWidgets.Box(60, 60);
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
        void SetupBackgroundProperties(LayoutFarm.CustomWidgets.Box backgroundBox)
        {
            //if click on background
            backgroundBox.MouseDown += (s, e) =>
            {
                _controllerBox1.TargetBox = null;//release target box
                _controllerBox1.Visible = false;
            };
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
                _controllerBox1.TargetBox = box;
                _controllerBox1.SetLocation(box.Left - 5, box.Top - 5);
                _controllerBox1.SetSize(box.Width + 10, box.Height + 10);
                _controllerBox1.Visible = true;
                //--------------------------------------------
                e.CancelBubbling = true;
                //change mouse capture to another control
                e.SetMouseCaptureElement(_controllerBox1);
            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;
                //hide controller
                _controllerBox1.Visible = false;
                _controllerBox1.TargetBox = null;
                e.CancelBubbling = true;
            };
        }

        static void MoveWithSnapToGrid(UIControllerBox controllerBox, int dx, int dy)
        {
            //sample move with snap to grid
            Point pos = controllerBox.Position;
            int newX = pos.X + dx;
            int newY = pos.Y + dy;
            int gridSize = 5;
            float halfGrid = (float)gridSize / 2f;
            int nearestX = (int)((newX + halfGrid) / gridSize) * gridSize;
            int nearestY = (int)((newY + halfGrid) / gridSize) * gridSize;
            controllerBox.SetLocation(nearestX, nearestY);
            var targetBox = controllerBox.TargetBox;
            if (targetBox != null)
            {
                //move target box too
                targetBox.SetLocation(nearestX + gridSize, nearestY + gridSize);
            }
        }
        static void SetupControllerBoxProperties(UIControllerBox controllerBox)
        {
            //for controller box 
            controllerBox.MouseDrag += (s, e) =>
            {
                MoveWithSnapToGrid(controllerBox, e.DiffCapturedX, e.DiffCapturedY);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
            };
        }

        //-----------------------------------------------------------------
        class UIControllerBox : LayoutFarm.CustomWidgets.AbstractBox
        {
            LayoutFarm.CustomWidgets.GridView gridBox;
            public UIControllerBox(int w, int h)
                : base(w, h)
            {
            }
            public LayoutFarm.UI.AbstractRectUI TargetBox
            {
                get;
                set;
            }
            //get primary render element
            public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
            {
                if (!this.HasReadyRenderElement)
                {
                    gridBox = new LayoutFarm.CustomWidgets.GridView(30, 30);
                    gridBox.SetLocation(5, 5);
                    gridBox.BuildGrid(3, 3, CellSizeStyle.UniformCell);
                    var myRenderElement = base.GetPrimaryRenderElement(rootgfx) as LayoutFarm.CustomWidgets.CustomRenderBox;
                    myRenderElement.AddChild(gridBox);
                }
                return base.GetPrimaryRenderElement(rootgfx);
            }

            public override void SetSize(int width, int height)
            {
                base.SetSize(width, height);
                if (gridBox != null)
                {
                    //adjust grid size
                    gridBox.SetSize(width - 10, height - 10);
                }
            }
          
        }
    }
}