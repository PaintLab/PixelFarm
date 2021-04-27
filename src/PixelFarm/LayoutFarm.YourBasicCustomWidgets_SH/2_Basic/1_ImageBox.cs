//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.UI;
using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class ImageBox : AbstractBox
    {
        ImageStrechKind _imgStretch = ImageStrechKind.None;
        CustomImageRenderBox _imgRenderBox;
        ImageBinder _imageBinder;
        EventHandler _imgChangedSubscribe;
         

        public ImageBox() : this(16, 16)
        {
            //if user does not provide width and height,
            //we use default first, and set HasSpecificWidthAndHeight=false
            //(this feature  found in label, image box, and text-flow-label)
            HasSpecificWidthAndHeight = false;
        }

        protected override IUICollection<UIElement> GetDefaultChildrenIter() => null;

        public ImageBox(int width, int height)
            : base(width, height)
        {
            this.NeedClipArea = true;
            _imgChangedSubscribe = (s, e) => OnContentUpdate();
        }
        public ImageBinder ImageBinder
        {
            get => _imageBinder;
            set
            {

                if (_imageBinder != null)
                {
                    //remove prev sub
                    _imageBinder.ImageChanged -= _imgChangedSubscribe;
                }

                _imageBinder = value;

                if (_imgRenderBox != null)
                {
                    _imgRenderBox.ImageBinder = value;
                    this.InvalidateGraphics();
                }

                if (value != null)
                {
                    //subscribe img changed?
                    value.ImageChanged += _imgChangedSubscribe;
                }
            }
        }
         
        public override RenderElement GetPrimaryRenderElement()
        {
            if (_imgRenderBox == null)
            {
                var imgBox = new CustomImageRenderBox(this.Width, this.Height);
                SetCommonProperties(imgBox, this);
                imgBox.ImageBinder = _imageBinder; 
                SetPrimaryRenderElement(imgBox);
                _imgRenderBox = imgBox;
            }
            return _imgRenderBox;
        }
        public override RenderElement CurrentPrimaryRenderElement => _imgRenderBox;
        public override void SetSize(int width, int height)
        {
            SetElementBoundsWH(width, height);
            this.CurrentPrimaryRenderElement?.SetSize(width, height);
        }
        public override void SetInnerContentSize(int w, int h)
        {
            this.CurrentPrimaryRenderElement?.SetSize(w, h);
        }
        void OnContentUpdate()
        {
            if (_imageBinder != null && _imageBinder.State == BinderState.Loaded)
            {
                SetProperSize();
                this.ParentUI?.NotifyContentUpdate(this);
                this.ParentUI?.InvalidateLayout();

                this.InvalidateGraphics();
            }
        }
        public override void PerformContentLayout(LayoutUpdateArgs args)
        {
            if (_imageBinder != null && _imageBinder.State == BinderState.Loaded)
            {
                SetProperSize();
            }
        }

        int PaddingLeftRight => PaddingLeft + PaddingRight;
        int PaddingTopBottom => PaddingTop + PaddingBottom;

        int ContentAreaWidth => this.Width - PaddingLeftRight;
        int ContentAreaHeight => this.Height - PaddingTopBottom;

        void SetProperSize()
        {
            //auto scale image
            if (_imageBinder == null) return;

            if (this.HasSpecificWidthAndHeight)
            {
                switch (_imgStretch)
                {
                    default: throw new NotSupportedException();
                    case ImageStrechKind.None:
                        this.SetInnerContentSize(_imageBinder.Width + PaddingLeftRight, _imageBinder.Height + PaddingBottom + PaddingTop);
                        break;
                    case ImageStrechKind.FitWidth:
                        float widthScale = ContentAreaWidth / (float)_imageBinder.Width;
                        this.SetInnerContentSize(
                            (int)(_imageBinder.Width * widthScale) + PaddingLeftRight,
                            (int)(_imageBinder.Height * widthScale) + PaddingTopBottom);
                        break;
                    case ImageStrechKind.FitHeight:
                        //fit img height 
                        //calculate scale ...
                        float heightScale = ContentAreaHeight / (float)_imageBinder.Height;
                        this.SetInnerContentSize(
                            (int)(_imageBinder.Width * heightScale) + PaddingLeftRight,
                            (int)(_imageBinder.Height * heightScale) + PaddingTopBottom);
                        break;

                }
            }
            else if (this.HasSpecificWidth)
            {
                float widthScale = ContentAreaWidth / (float)_imageBinder.Width;

                int innerW = (int)(_imageBinder.Width * widthScale);
                int innerH = (int)(_imageBinder.Height * widthScale);

                //2. viewport size
                this.SetSize(this.Width, innerH);

                this.SetInnerContentSize(
                   (int)innerW,
                   (int)innerH);
#if DEBUG
                //_imgRenderBox.dbugBreak = true;
#endif
            }
            else if (this.HasSpecificHeight)
            {
                float heightScale = ContentAreaHeight / (float)_imageBinder.Height;

                int innerW = (int)(_imageBinder.Width * heightScale);
                int innerH = (int)(_imageBinder.Height * heightScale);


                this.SetSize(innerW, this.Height);


                this.SetInnerContentSize(
                   (int)innerW,
                   (int)innerH);
            }
            else
            {
                //free scale
                int newW = _imageBinder.Width + PaddingLeftRight;
                int newH = _imageBinder.Height + PaddingTopBottom;
                this.SetSize(newW, newH);
                this.SetInnerContentSize(newW, newH);
            }
        }
    }
    public enum ImageStrechKind
    {
        None,
        FitWidth,
        FitHeight
    }
}
