//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    class UINinespaceBox : LayoutFarm.CustomWidgets.AbstractBox
    {
        Box _boxLeftTop;
        Box _boxRightTop;
        Box _boxLeftBottom;
        Box _boxRightBottom;
        //-------------------------------------
        Box _boxLeft;
        Box _boxTop;
        Box _boxRight;
        Box _boxBottom;
        //-------------------------------------
        Box _centerBox;
        Box _gripperLeft;
        Box _gripperRight;
        Box _gripperTop;
        Box _gripperBottom;
        DockSpacesController _dockspaceController;
        NinespaceGrippers _ninespaceGrippers;
        public UINinespaceBox(int w, int h)
            : base(w, h)
        {
            SetupDockSpaces();
        }
        void SetupDockSpaces()
        {
            //1. controller
            _dockspaceController = new DockSpacesController(this, SpaceConcept.NineSpace);
            //2.  
            _dockspaceController.LeftTopSpace.Content = _boxLeftTop = CreateSpaceBox(SpaceName.LeftTop, Color.Red);
            _dockspaceController.RightTopSpace.Content = _boxRightTop = CreateSpaceBox(SpaceName.RightTop, Color.Red);
            _dockspaceController.LeftBottomSpace.Content = _boxLeftBottom = CreateSpaceBox(SpaceName.LeftBottom, Color.Red);
            _dockspaceController.RightBottomSpace.Content = _boxRightBottom = CreateSpaceBox(SpaceName.RightBottom, Color.Red);
            //3.
            _dockspaceController.LeftSpace.Content = _boxLeft = CreateSpaceBox(SpaceName.Left, Color.Blue);
            _dockspaceController.TopSpace.Content = _boxTop = CreateSpaceBox(SpaceName.Top, Color.Yellow);
            _dockspaceController.RightSpace.Content = _boxRight = CreateSpaceBox(SpaceName.Right, Color.Green);
            _dockspaceController.BottomSpace.Content = _boxBottom = CreateSpaceBox(SpaceName.Bottom, Color.Yellow);
            //--------------------------------
            //left and right space expansion
            _dockspaceController.LeftSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
            _dockspaceController.RightSpaceVerticalExpansion = VerticalBoxExpansion.TopBottom;
            _dockspaceController.SetRightSpaceWidth(200);
            _dockspaceController.SetLeftSpaceWidth(200);
            //------------------------------------------------------------------------------------
            _ninespaceGrippers = new NinespaceGrippers(_dockspaceController);
            _ninespaceGrippers.LeftGripper = _gripperLeft = CreateGripper(Color.Red, false);
            _ninespaceGrippers.RightGripper = _gripperRight = CreateGripper(Color.Red, false);
            _ninespaceGrippers.TopGripper = _gripperTop = CreateGripper(Color.Red, true);
            _ninespaceGrippers.BottomGripper = _gripperBottom = CreateGripper(Color.Red, true);
            _ninespaceGrippers.UpdateGripperPositions();
            //------------------------------------------------------------------------------------
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "ninebox");
            this.Describe(visitor);
            visitor.EndElement();
        }
        CustomWidgets.Box CreateGripper(PixelFarm.Drawing.Color bgcolor, bool isVertical)
        {
            int controllerBoxWH = 10;
            var gripperBox = new CustomWidgets.Box(controllerBoxWH, controllerBoxWH);
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
        static CustomWidgets.Box CreateSpaceBox(SpaceName name, Color bgcolor)
        {
            int controllerBoxWH = 10;
            CustomWidgets.Box spaceBox = new CustomWidgets.Box(controllerBoxWH, controllerBoxWH);
            spaceBox.BackColor = bgcolor;
            spaceBox.Tag = name;
            return spaceBox;
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!this.HasReadyRenderElement)
            {
                var renderE = base.GetPrimaryRenderElement(rootgfx);
                //------------------------------------------------------
                renderE.AddChild(_boxLeftTop);
                renderE.AddChild(_boxRightTop);
                renderE.AddChild(_boxLeftBottom);
                renderE.AddChild(_boxRightBottom);
                //------------------------------------------------------
                renderE.AddChild(_boxLeft);
                renderE.AddChild(_boxRight);
                renderE.AddChild(_boxTop);
                renderE.AddChild(_boxBottom);
                //grippers
                renderE.AddChild(_gripperLeft);
                renderE.AddChild(_gripperRight);
                renderE.AddChild(_gripperTop);
                renderE.AddChild(_gripperBottom);
                //------------------------------------------------------
            }
            return base.GetPrimaryRenderElement(rootgfx);
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            _dockspaceController.SetSize(width, height);
        }

        public Box LeftSpace => _boxLeft;
        public Box RightSpace => _boxRight;
        public Box TopSpace => _boxTop;
        public Box BottomSpace => _boxBottom;
    }
}