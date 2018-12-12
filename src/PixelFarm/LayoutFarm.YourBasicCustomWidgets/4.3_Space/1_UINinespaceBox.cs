//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{

    public class NinespaceBox : AbstractBox
    {





        DockSpacesController _dockspaceController;
        NinespaceGrippers _ninespaceGrippers;


        public NinespaceBox(int w, int h)
            : base(w, h)
        {
            SetupDockSpaces(SpaceConcept.NineSpace);
        }
        public NinespaceBox(int w, int h, SpaceConcept spaceConcept)
            : base(w, h)
        {
            SetupDockSpaces(spaceConcept);
        }

        public bool ShowGrippers
        {
            get => _ninespaceGrippers.ShowGrippers;
            set => _ninespaceGrippers.ShowGrippers = value;
        }
        static Box CreateSpaceBox(SpaceName name, Color bgcolor)
        {
            int controllerBoxWH = 10;
            Box spaceBox = new Box(controllerBoxWH, controllerBoxWH);
            spaceBox.BackColor = bgcolor;
            spaceBox.Tag = name;
            return spaceBox;
        }




        //default color for each space
        //user can specific this later ...
        static Color _leftTopColor = Color.White;
        static Color _rightTopColor = Color.White;
        static Color _leftBottomColor = Color.White;
        static Color _rightBottomColor = Color.White;
        static Color _leftColor = Color.White;
        static Color _topColor = Color.White;
        static Color _rightColor = Color.White;
        static Color _bottomColor = Color.White;
        static Color _centerColor = Color.White;
        static Color _gripperColor = Color.FromArgb(200, Color.Gray);

        void SetupDockSpaces(SpaceConcept spaceConcept)
        {

#if DEBUG
            _leftTopColor = Color.Red;
            _rightTopColor = Color.Red;
            _leftBottomColor = Color.Red;
            _rightBottomColor = Color.Red;
            //
            _leftColor = Color.Blue;
            _topColor = Color.Yellow;
            _rightColor = Color.Green;
            _bottomColor = Color.Yellow;
#endif


            //1. controller
            _dockspaceController = new DockSpacesController(this, spaceConcept);
            //2.  
            _dockspaceController.LeftTopSpacePart.Content = CreateSpaceBox(SpaceName.LeftTop, _leftTopColor);
            _dockspaceController.RightTopSpacePart.Content = CreateSpaceBox(SpaceName.RightTop, _rightTopColor);
            _dockspaceController.LeftBottomSpacePart.Content = CreateSpaceBox(SpaceName.LeftBottom, _leftBottomColor);
            _dockspaceController.RightBottomSpacePart.Content = CreateSpaceBox(SpaceName.RightBottom, _rightBottomColor);
            //3.
            _dockspaceController.LeftSpacePart.Content = CreateSpaceBox(SpaceName.Left, _leftColor);
            _dockspaceController.TopSpacePart.Content = CreateSpaceBox(SpaceName.Top, _topColor);
            _dockspaceController.RightSpacePart.Content = CreateSpaceBox(SpaceName.Right, _rightColor);
            _dockspaceController.BottomSpacePart.Content = CreateSpaceBox(SpaceName.Bottom, _bottomColor);
            _dockspaceController.CenterSpacePart.Content = CreateSpaceBox(SpaceName.Center, _centerColor);
            //--------------------------------
            //left and right space expansion
            //dockspaceController.LeftSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
            //dockspaceController.RightSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
            _dockspaceController.SetRightSpaceWidth(200);
            _dockspaceController.SetLeftSpaceWidth(200);
            //------------------------------------------------------------------------------------
            _ninespaceGrippers = new NinespaceGrippers(_dockspaceController);
            _ninespaceGrippers.LeftGripper = CreateGripper(_gripperColor, false);
            _ninespaceGrippers.RightGripper = CreateGripper(_gripperColor, false);
            _ninespaceGrippers.TopGripper = CreateGripper(_gripperColor, true);
            _ninespaceGrippers.BottomGripper = CreateGripper(_gripperColor, true);
            _ninespaceGrippers.UpdateGripperPositions();
            //------------------------------------------------------------------------------------
        }
        public void SetDockSpaceConcept(LayoutFarm.UI.SpaceConcept concept)
        {
        }
        AbstractBox CreateGripper(PixelFarm.Drawing.Color bgcolor, bool isVertical)
        {
            int controllerBoxWH = 10;
            var gripperBox = new Box(controllerBoxWH, controllerBoxWH);
            gripperBox.BackColor = bgcolor;
            //---------------------------------------------------------------------

            gripperBox.MouseDrag += (s, e) =>
            {
                Point pos = gripperBox.Position;
                if (isVertical)
                {
                    gripperBox.SetLocation(pos.X, pos.Y + e.YDiff);
                }
                else
                {
                    gripperBox.SetLocation(pos.X + e.XDiff, pos.Y);
                }

                _ninespaceGrippers.UpdateNinespaces();
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
            };
            gripperBox.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                e.CancelBubbling = true;
            };
            return gripperBox;
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!HasReadyRenderElement)
            {
                var renderE = base.GetPrimaryRenderElement(rootgfx);
                //------------------------------------------------------
                renderE.AddChild(CentralSpace);
                //------------------------------------------------------
                renderE.AddChild(LeftTopSpace);
                renderE.AddChild(RightTopSpace);
                renderE.AddChild(LeftBottomSpace);
                renderE.AddChild(RightBottomSpace);
                //------------------------------------------------------
                renderE.AddChild(LeftSpace);
                renderE.AddChild(RightSpace);
                renderE.AddChild(TopSpace);
                renderE.AddChild(BottomSpace);
                //------------------------------------------------------
                //grippers

                renderE.AddChild(_ninespaceGrippers.LeftGripper);
                renderE.AddChild(_ninespaceGrippers.RightGripper);
                renderE.AddChild(_ninespaceGrippers.TopGripper);
                renderE.AddChild(_ninespaceGrippers.BottomGripper);
                //------------------------------------------------------
                return renderE;
            }
            return base.GetPrimaryRenderElement(rootgfx);
        }
        public NinespaceGrippers Grippers => _ninespaceGrippers;
        public DockSpacesController DockSpancesController => _dockspaceController;
        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            _dockspaceController.SetSize(width, height);
        }
        public override void PerformContentLayout()
        {
            _dockspaceController.ArrangeAllSpaces();
        }
        //
        public Box LeftSpace => (Box)_dockspaceController.LeftSpacePart.Content;
        public Box RightSpace => (Box)_dockspaceController.RightSpacePart.Content;
        public Box TopSpace => (Box)_dockspaceController.TopSpacePart.Content;
        public Box BottomSpace => (Box)_dockspaceController.BottomSpacePart.Content;
        //
        public Box CentralSpace => (Box)_dockspaceController.CenterSpacePart.Content;
        //
        public Box LeftTopSpace => (Box)_dockspaceController.LeftTopSpacePart.Content;
        public Box RightTopSpace => (Box)_dockspaceController.RightTopSpacePart.Content;
        public Box LeftBottomSpace => (Box)_dockspaceController.LeftBottomSpacePart.Content;
        public Box RightBottomSpace => (Box)_dockspaceController.RightBottomSpacePart.Content;
        //

        public void SetLeftSpaceWidth(int w)
        {
            _dockspaceController.SetLeftSpaceWidth(w);
            _ninespaceGrippers.UpdateGripperPositions();
        }
        public void SetRightSpaceWidth(int w)
        {
            _dockspaceController.SetRightSpaceWidth(w);
            _ninespaceGrippers.UpdateGripperPositions();
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "ninebox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}