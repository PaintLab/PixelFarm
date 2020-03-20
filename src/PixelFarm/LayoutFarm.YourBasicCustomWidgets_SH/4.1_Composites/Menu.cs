//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class MenuItem : Box
    {
        HingeRelation _hingeRelation;
        List<MenuItem> _childItems;
        public MenuItem(int width, int height)
            : base(width, height)
        {
            _hingeRelation = new HingeRelation();
            _hingeRelation.LandPart = this;
        }

        public AbstractRectUI FloatPart
        {
            get => _hingeRelation.FloatPart;
            set => _hingeRelation.FloatPart = value;
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