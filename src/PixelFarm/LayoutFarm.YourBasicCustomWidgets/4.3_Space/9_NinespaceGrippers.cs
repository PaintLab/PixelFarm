//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    public class NinespaceGrippers
    {
        NinespaceController _ninespaceController;
        public NinespaceGrippers(NinespaceController ninespaceController)
        {
            this._ninespaceController = ninespaceController;
        }
        public AbstractRectUI LeftGripper
        {
            get;
            set;
        }
        public AbstractRectUI TopGripper
        {
            get;
            set;
        }

        public AbstractRectUI RightGripper
        {
            get;
            set;
        }
        public AbstractRectUI BottomGripper
        {
            get;
            set;
        }


        public void UpdateGripperPositions()
        {
            switch (this._ninespaceController.SpaceConcept)
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
                                this._ninespaceController.LeftSpace.Width - (this.LeftGripper.Width / 2),
                                this._ninespaceController.Owner.Height / 2);
                        }
                        if (this.RightGripper != null)
                        {
                            this.RightGripper.SetLocation(
                                this._ninespaceController.RightSpace.X - (this.RightGripper.Width / 2),
                                this._ninespaceController.Owner.Height / 2);
                        }

                        if (this.TopGripper != null)
                        {
                            this.TopGripper.SetLocation(
                               this._ninespaceController.TopSpace.X + (this._ninespaceController.TopSpace.Width / 2) - (this.TopGripper.Width / 2),
                               this._ninespaceController.TopSpace.Bottom - (this.TopGripper.Height / 2));
                        }

                        if (this.BottomGripper != null)
                        {
                            this.BottomGripper.SetLocation(
                               this._ninespaceController.BottomSpace.X + (this._ninespaceController.BottomSpace.Width / 2) - (this.TopGripper.Width / 2),
                               this._ninespaceController.BottomSpace.Y - (this.BottomGripper.Height / 2));
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
        public void UpdateNinespaces()
        {
            switch (this._ninespaceController.SpaceConcept)
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
                            this._ninespaceController.SetLeftSpaceWidth(this.LeftGripper.Left + (this.LeftGripper.Width / 2));
                        }
                        if (this.RightGripper != null)
                        {
                            this._ninespaceController.SetRightSpaceWidth(
                                (this._ninespaceController.Owner.Width - this.RightGripper.Left) - (this.RightGripper.Width / 2));
                        }
                        if (this.TopGripper != null)
                        {
                            this._ninespaceController.SetTopSpaceHeight(this.TopGripper.Top + this.TopGripper.Height / 2);
                        }
                        if (this.BottomGripper != null)
                        {
                            this._ninespaceController.SetBottomSpaceHeight(
                              (this._ninespaceController.Owner.Height - this.BottomGripper.Top) - this.BottomGripper.Height / 2);
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