﻿//Apache2, 2014-present, WinterDev


using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class ComboBox : AbstractControlBox
    {
        HingeRelation _hingeRel = new HingeRelation();

        public ComboBox(int width, int height)
            : base(width, height)
        {
            _hingeRel.LandPart = this;
        }

        protected override void OnLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
            _hingeRel.CloseHinge();
            base.OnLostMouseFocus(e);
        }
        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            _hingeRel.ToggleOpenOrClose();
            base.OnMouseDown(e);
        }
        public AbstractRectUI FloatPart
        {
            get => _hingeRel.FloatPart;
            set => _hingeRel.FloatPart = value;
        }
        public bool IsOpen => _hingeRel.IsOpen;
        public void OpenHinge() => _hingeRel.OpenHinge();
        public void CloseHinge() => _hingeRel.CloseHinge();
        public HingeFloatPartStyle FloatPartStyle
        {
            get => _hingeRel.FloatPartStyle;
            set => _hingeRel.FloatPartStyle = value;
        } 
        public void AddLandPartContent(UIElement ui) => AddChild(ui); 
       
    }

}