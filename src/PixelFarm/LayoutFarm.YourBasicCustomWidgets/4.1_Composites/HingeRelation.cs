//Apache2, 2014-present, WinterDev


using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class HingeRelation
    {
        bool _isOpen;
        //1. land part
        AbstractRectUI _landPart;
        //2. float part   
        AbstractRectUI _floatPart;
        RenderElement _floatPartRenderElement;
        HingeFloatPartStyle _floatPartStyle;

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
                    //if (primElement != null)
                    //{
                    //    //add 
                    //    var visualPlainLayer = primElement.Layers.GetLayer(0) as VisualPlainLayer;
                    //    if (visualPlainLayer != null)
                    //    {
                    //        visualPlainLayer.AddChild(value.GetPrimaryRenderElement(primElement.Root));
                    //    } 
                    //} 
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
            if (_landPart == null) return;
            if (_floatPart == null) return;
            switch (_floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        //add float part to top window layer
                        //var topRenderBox = primElement.GetTopWindowRenderBox();
                        //if (topRenderBox != null)
                        //{
                        //    Point globalLocation = primElement.GetGlobalLocation();
                        //    floatPart.SetLocation(globalLocation.X, globalLocation.Y + primElement.Height);
                        //    this.floatPartRenderElement = this.floatPart.GetPrimaryRenderElement(primElement.Root);
                        //    topRenderBox.AddChild(floatPartRenderElement);
                        //}

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
            if (this._landPart == null) return;
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
    }
}