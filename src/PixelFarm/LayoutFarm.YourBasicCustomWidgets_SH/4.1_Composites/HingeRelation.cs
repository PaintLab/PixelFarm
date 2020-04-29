//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
using PixelFarm.Drawing;

namespace LayoutFarm.CustomWidgets
{
    public enum HingeFloatPartStyle
    {
        Popup,
        Embeded
    }
    public class HingeRelation
    {
        bool _isOpen;
        RenderElement _floatPartRenderElement;

        public HingeRelation()
        {

        }

        public AbstractRectUI LandPart { get; set; }
        public AbstractRectUI FloatPart { get; set; }
        public bool IsOpen => _isOpen;
        public void ToggleOpenOrClose()
        {
            if (_isOpen)
            {
                CloseHinge();
            }
            else
            {
                OpenHinge();
            }
        }
        public void OpenHinge()
        {
            if (_isOpen) return;
            _isOpen = true;
            //-----------------------------------
            if (LandPart == null) return;
            if (FloatPart == null) return;//may be null

            switch (FloatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {

                        RenderElement renderE = LandPart.CurrentPrimaryRenderElement;
                        if (renderE != null)
                        {
                            IContainerRenderElement topRenderBox = renderE.GetTopWindowRenderBox();
                            if (topRenderBox != null)
                            {
                                Point globalLocation = LandPart.GetGlobalLocation();
                                FloatPart.SetLocation(globalLocation.X, globalLocation.Y + LandPart.Height);
                                _floatPartRenderElement = FloatPart.GetPrimaryRenderElement(topRenderBox.Root);
                                topRenderBox.AddChild(_floatPartRenderElement);
                            }
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
            if (LandPart == null) return;
            if (FloatPart == null) return;
            switch (FloatPartStyle)
            {
                default:
                    {
                    }
                    break;
                case HingeFloatPartStyle.Popup:
                    {
                        if (_floatPartRenderElement != null)
                        {
                            var topRenderBox = _floatPartRenderElement.GetTopWindowRenderBox();
                            if (topRenderBox != null)
                            {
                                topRenderBox.RemoveChild(_floatPartRenderElement);
                            }
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }

        public HingeFloatPartStyle FloatPartStyle { get; set; }
    }
}