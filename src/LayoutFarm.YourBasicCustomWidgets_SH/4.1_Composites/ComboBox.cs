//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class ComboBox : AbstractRectUI
    {
        CustomRenderBox _primElement;//background 
        Color _backColor = Color.LightGray;
        bool _isOpen;
        //1. land part
        AbstractRectUI _landPart;
        //2. float part   
        AbstractRectUI _floatPart;
        RenderElement _floatPartRenderElement;
        HingeFloatPartStyle _floatPartStyle;
        //
        public ComboBox(int width, int height)
            : base(width, height)
        {
        }
        //
        protected override bool HasReadyRenderElement => _primElement != null;
        //
        public override RenderElement CurrentPrimaryRenderElement => _primElement;
        //
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (HasReadyRenderElement)
                {
                    _primElement.BackColor = value;
                }
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                renderE.SetLocation(this.Left, this.Top);
                renderE.BackColor = _backColor;
                renderE.SetController(this);
                renderE.HasSpecificWidthAndHeight = true;
                //------------------------------------------------
                //create visual layer 
                if (_landPart != null)
                {
                    renderE.AddChild(_landPart);
                }
                if (_floatPart != null)
                {
                }
                //---------------------------------
                _primElement = renderE;
            }
            return _primElement;
        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {

            MouseDown?.Invoke(this, e);
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {

            MouseUp?.Invoke(this, e);
            base.OnMouseUp(e);
        }

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        //----------------------------------------------------  
        public AbstractRectUI LandPart
        {
            get => _landPart;
            set
            {
                _landPart = value;
                if (value != null)
                {
                    //if new value not null
                    //check existing land part
                    if (_landPart != null)
                    {
                        //remove existing landpart

                    }

                    if (_primElement != null)
                    {
                        //add 
                        _primElement.AddChild(value);
                    }
                }
                else
                {
                    if (_landPart != null)
                    {
                        //remove existing landpart

                    }
                }
            }
        }
        public AbstractRectUI FloatPart
        {
            get => _floatPart;
            set
            {
                _floatPart = value;
                if (value != null)
                {
                    //attach float part

                }
            }
        }
        //---------------------------------------------------- 
        public bool IsOpen => _isOpen;
        //---------------------------------------------------- 


        public void OpenHinge()
        {
            if (_isOpen) return;
            _isOpen = true;
            //-----------------------------------
            if (_primElement == null) return;
            if (_floatPart == null) return;
            switch (_floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        //add float part to top window layer
                        var topRenderBox = _primElement.GetTopWindowRenderBox();
                        if (topRenderBox != null)
                        {
                            Point globalLocation = _primElement.GetGlobalLocation();
                            _floatPart.SetLocation(globalLocation.X, globalLocation.Y + _primElement.Height);
                            _floatPartRenderElement = _floatPart.GetPrimaryRenderElement(_primElement.Root);
                            topRenderBox.AddChild(_floatPartRenderElement);
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }
        public void CloseHinge()
        {
            if (!_isOpen) return;
            _isOpen = false;
            if (_primElement == null) return;
            if (_floatPart == null) return;
            switch (_floatPartStyle)
            {
                default:
                    {
                    }
                    break;
                case HingeFloatPartStyle.Popup:
                    {
                        var topRenderBox = _primElement.GetTopWindowRenderBox();
                        if (topRenderBox != null)
                        {
                            topRenderBox.RemoveChild(_floatPartRenderElement);
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }

        public HingeFloatPartStyle FloatPartStyle
        {
            get => _floatPartStyle;
            set => _floatPartStyle = value;
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "combobox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}