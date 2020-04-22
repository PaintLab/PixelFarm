//Apache2, 2014-present, WinterDev


using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class ComboBox : AbstractBox
    {
        HingeRelation _hingeRel = new HingeRelation();
        UICollection _children;
        public ComboBox(int width, int height)
            : base(width, height)
        {
            _hingeRel.LandPart = this;
            _children = new UICollection(this);
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

        public void AddLandPartContent(UIElement ui)
        {
            _children.AddUI(ui);
            if (_primElement != null)
            {
                _primElement.AddChild(ui);
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!HasReadyRenderElement)
            {
                RenderElement renderE = base.GetPrimaryRenderElement(rootgfx);
                foreach (UIElement ui in _children.GetIter())
                {
                    _primElement.AddChild(ui);
                }
                //???

                return renderE;
            }
            return base.GetPrimaryRenderElement(rootgfx);
        }
    }

}