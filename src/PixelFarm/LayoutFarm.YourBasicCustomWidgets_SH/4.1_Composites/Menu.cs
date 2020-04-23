//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
using System;
using System.Collections.Generic;
namespace LayoutFarm.CustomWidgets
{
    public class MenuItem : AbstractBox
    {
        HingeRelation _hingeRelation;
        List<MenuItem> _childItems;
        Box _floatPart;

        UIList<UIElement> _landPart;
        public MenuItem(int width, int height)
            : base(width, height)
        {
            _hingeRelation = new HingeRelation();
            _hingeRelation.LandPart = this;
            _landPart = new UIList<UIElement>();
        }
        public void AddLandPart(UIElement ui)
        {
            //add to its
            _landPart.Add(this, ui);

            if (_primElement != null)
            {
                _primElement.AddChild(ui);
            }
        }
        public Box FloatPart
        {
            get => _floatPart;
            set
            {
                _floatPart = value;
                _hingeRelation.FloatPart = value;
            }
        }
        public bool IsOpened => _hingeRelation.IsOpen;
        public void Open() => _hingeRelation.OpenHinge();
        public void Close() => _hingeRelation.CloseHinge();
        public void MaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintenceOpenState = true;
                this.ParentMenuItem.MaintenanceParentOpenState();
            }
        }
        public void UnmaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintenceOpenState = false;
                this.ParentMenuItem.MaintenanceParentOpenState();
            }
        }
        public bool MaintenceOpenState { get; private set; }
        public void CloseRecursiveUp()
        {
            this.Close();
            if (this.ParentMenuItem != null &&
               !this.ParentMenuItem.MaintenceOpenState)
            {
                this.ParentMenuItem.CloseRecursiveUp();
            }
        }
        public MenuItem ParentMenuItem { get; private set; }
        public HingeFloatPartStyle FloatPartStyle
        {
            get => _hingeRelation.FloatPartStyle;
            set => _hingeRelation.FloatPartStyle = value;
        }
        public void AddSubMenuItem(MenuItem childItem)
        {
            if (_childItems == null)
            {
                _childItems = new List<MenuItem>();
            }
            _childItems.Add(childItem);
            FloatPart.Add(childItem);
            childItem.ParentMenuItem = this;
        }
    }

}