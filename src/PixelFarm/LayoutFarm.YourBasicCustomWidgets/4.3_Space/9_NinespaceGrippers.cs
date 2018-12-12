//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    public class NinespaceGrippers
    {
        NinespaceController _ninespaceController;
        bool _showGrippers;
        public NinespaceGrippers(NinespaceController ninespaceController)
        {
            _ninespaceController = ninespaceController;
        }
        public AbstractRectUI LeftGripper { get; set; }
        public AbstractRectUI TopGripper { get; set; }
        public AbstractRectUI RightGripper { get; set; }
        public AbstractRectUI BottomGripper { get; set; }
        public bool ShowGrippers
        {
            get => _showGrippers;
            set
            {
                _showGrippers = value;
                //set visible, invisible to all grippers
                if (LeftGripper != null) LeftGripper.Visible = value;
                if (TopGripper != null) TopGripper.Visible = value;
                if (RightGripper != null) RightGripper.Visible = value;
                if (BottomGripper != null) BottomGripper.Visible = value;
            }
        }

        internal void UpdateGripperPositions()
        {
            switch (_ninespaceController.SpaceConcept)
            {
                default:
                    throw new NotSupportedException();
                case SpaceConcept.NineSpaceFree:
                    return;
                case SpaceConcept.NineSpace:
                case SpaceConcept.FiveSpace:
                    {
                        if (this.LeftGripper != null)
                        {
                            //align on center
                            this.LeftGripper.SetLocation(
                               _ninespaceController.LeftSpacePart.Width - (this.LeftGripper.Width / 2),
                              _ninespaceController.Owner.Height / 2);
                        }
                        if (this.RightGripper != null)
                        {
                            this.RightGripper.SetLocation(
                                _ninespaceController.RightSpacePart.X - (this.RightGripper.Width / 2),
                                _ninespaceController.Owner.Height / 2);
                        }

                        if (this.TopGripper != null)
                        {
                            this.TopGripper.SetLocation(
                                _ninespaceController.TopSpacePart.X + (_ninespaceController.TopSpacePart.Width / 2) - (this.TopGripper.Width / 2),
                                _ninespaceController.TopSpacePart.Bottom - (this.TopGripper.Height / 2));
                        }

                        if (this.BottomGripper != null)
                        {
                            this.BottomGripper.SetLocation(
                               _ninespaceController.BottomSpacePart.X + (_ninespaceController.BottomSpacePart.Width / 2) - (this.TopGripper.Width / 2),
                                _ninespaceController.BottomSpacePart.Y - (this.BottomGripper.Height / 2));
                        }
                    }
                    break;
                case SpaceConcept.FourSpace:
                    {
                    }
                    break;
                case SpaceConcept.ThreeSpaceHorizontal:
                    {
                    }
                    break;
                case SpaceConcept.ThreeSpaceVertical:
                    {
                    }
                    break;
                //------------------------------------
                case SpaceConcept.TwoSpaceHorizontal:
                    {
                    }
                    break;
                case SpaceConcept.TwoSpaceVertical:
                    {
                    }
                    break;
            }
        }
        internal void UpdateNinespaces()
        {
            switch (_ninespaceController.SpaceConcept)
            {
                default:
                    throw new NotSupportedException();
                case SpaceConcept.NineSpaceFree:
                    return;
                case SpaceConcept.NineSpace:
                case SpaceConcept.FiveSpace:
                    {
                        if (this.LeftGripper != null)
                        {
                            //align on center
                            _ninespaceController.SetLeftSpaceWidth(this.LeftGripper.Left + (this.LeftGripper.Width / 2));
                        }
                        if (this.RightGripper != null)
                        {
                            _ninespaceController.SetRightSpaceWidth(
                                 (_ninespaceController.Owner.Width - this.RightGripper.Left) - (this.RightGripper.Width / 2));
                        }
                        if (this.TopGripper != null)
                        {
                            _ninespaceController.SetTopSpaceHeight(this.TopGripper.Top + this.TopGripper.Height / 2);
                        }
                        if (this.BottomGripper != null)
                        {
                            _ninespaceController.SetBottomSpaceHeight(
                               (_ninespaceController.Owner.Height - this.BottomGripper.Top) - this.BottomGripper.Height / 2);
                        }
                    }
                    break;
                case SpaceConcept.FourSpace:
                    {
                    }
                    break;
                case SpaceConcept.ThreeSpaceHorizontal:
                    {
                    }
                    break;
                case SpaceConcept.ThreeSpaceVertical:
                    {
                    }
                    break;
                //------------------------------------
                case SpaceConcept.TwoSpaceHorizontal:
                    {
                    }
                    break;
                case SpaceConcept.TwoSpaceVertical:
                    {
                    }
                    break;
            }
        }
    }
}