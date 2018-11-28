//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class HingeBox : AbstractRectUI
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
        public HingeBox(int width, int height)
            : base(width, height)
        {
        }

        protected override bool HasReadyRenderElement
        {
            get { return this._primElement != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this._primElement; }
        }
        public Color BackColor
        {
            get { return this._backColor; }
            set
            {
                this._backColor = value;
                if (HasReadyRenderElement)
                {
                    this._primElement.BackColor = value;
                }
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                this.SetLocation(this.Left, this.Top);
                renderE.BackColor = _backColor;
                renderE.SetController(this);
                renderE.HasSpecificWidthAndHeight = true;
                //------------------------------------------------
                //create visual layer                 

                if (this._landPart != null)
                {
                    renderE.AddChild(this._landPart);
                }
                if (this._floatPart != null)
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
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }

        //----------------------------------------------------

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        //----------------------------------------------------  
        public AbstractRectUI LandPart
        {
            get { return this._landPart; }
            set
            {
                this._landPart = value;
                if (value != null)
                {
                    //if new value not null
                    //check existing land part
                    if (this._landPart != null)
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
                    if (this._landPart != null)
                    {
                        //remove existing landpart

                    }
                }
            }
        }
        public AbstractRectUI FloatPart
        {
            get { return this._floatPart; }
            set
            {
                this._floatPart = value;
                if (value != null)
                {
                    //attach float part

                }
            }
        }
        //---------------------------------------------------- 
        public bool IsOpen
        {
            get { return this._isOpen; }
        }
        //---------------------------------------------------- 


        public void OpenHinge()
        {
            if (_isOpen) return;
            this._isOpen = true;
            //-----------------------------------
            if (this._primElement == null) return;
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
                            this._floatPartRenderElement = this._floatPart.GetPrimaryRenderElement(_primElement.Root);
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
            this._isOpen = false;
            if (this._primElement == null) return;
            if (_floatPart == null) return;
            switch (_floatPartStyle)
            {
                default:
                    {
                    }
                    break;
                case HingeFloatPartStyle.Popup:
                    {
                        if (_floatPartRenderElement != null)
                        {
                            //temp
                            var parentContainer = _floatPartRenderElement.ParentRenderElement as CustomRenderBox;
                            parentContainer.RemoveChild(_floatPartRenderElement);
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
            get { return this._floatPartStyle; }
            set
            {
                this._floatPartStyle = value;
            }
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "hingbox");
            visitor.EndElement();
        }
    }
    public enum HingeFloatPartStyle
    {
        Popup,
        Embeded
    }
}
