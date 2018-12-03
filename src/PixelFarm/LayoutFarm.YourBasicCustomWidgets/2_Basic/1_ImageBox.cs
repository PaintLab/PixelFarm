//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class ImageBox : AbstractBox
    {
        ImageStrechKind _imgStretch = ImageStrechKind.None;
        CustomImageRenderBox _imgRenderBox;
        ImageBinder _imageBinder;
        EventHandler _imgChangedSubscribe;

        public ImageBox(int width, int height)
            : base(width, height)
        {
            _imgChangedSubscribe = (s, e) => OnContentUpdate();
        }
        public ImageBinder ImageBinder
        {
            get { return this._imageBinder; }
            set
            {
                if (_imageBinder != null)
                {
                    //remove prev sub
                    _imageBinder.ImageChanged -= _imgChangedSubscribe;
                }

                this._imageBinder = value;

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

        protected override bool HasReadyRenderElement => _imgRenderBox != null;
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_imgRenderBox == null)
            {
                var renderBox = new CustomImageRenderBox(rootgfx, this.Width, this.Height);
                renderBox.SetLocation(this.Left, this.Top);
                renderBox.ImageBinder = _imageBinder;
                renderBox.SetController(this);
                renderBox.BackColor = this.BackColor;
                renderBox.NeedClipArea = this.NeedClipArea;

                SetPrimaryRenderElement(renderBox);
                _imgRenderBox = renderBox;
            }
            return _imgRenderBox;
        }
        public override RenderElement CurrentPrimaryRenderElement => _imgRenderBox;
        public override void SetSize(int width, int height)
        {
            SetElementBoundsWH(width, height);
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize(width, height);
            }
        }
        public override void SetInnerContentSize(int w, int h)
        {
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize(w, h);
            }
        }
        protected override void OnContentUpdate()
        {
            if (_imageBinder.State == BinderState.Loaded)
            {

                SetProperSize();

                this.ParentUI?.NotifyContentUpdate(this);
                this.ParentUI?.InvalidateLayout();
            }
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "imgbox");
            this.Describe(visitor);
            visitor.EndElement();
        }
        public override void PerformContentLayout()
        {
            if (_imageBinder.State == BinderState.Loaded)
            {
                SetProperSize();
            }
        }
        void SetProperSize()
        {
            //auto scale image

            if (this.HasSpecificWidthAndHeight)
            {
                switch (_imgStretch)
                {
                    default: throw new NotSupportedException();
                    case ImageStrechKind.None:
                        this.SetInnerContentSize(_imageBinder.Width, _imageBinder.Height);
                        break;
                    case ImageStrechKind.FitWidth:
                        float widthScale = this.Width / (float)_imageBinder.Width;
                        this.SetInnerContentSize(
                            (int)(_imageBinder.Width * widthScale),
                            (int)(_imageBinder.Height * widthScale));
                        break;
                    case ImageStrechKind.FitHeight:
                        //fit img height 
                        //calculate scale ...
                        float heightScale = this.Height / (float)_imageBinder.Height;
                        this.SetInnerContentSize(
                            (int)(_imageBinder.Width * heightScale),
                            (int)(_imageBinder.Height * heightScale));
                        break;

                }
            }
            else if (this.HasSpecificWidth)
            {
                float widthScale = this.Width / (float)_imageBinder.Width;

                int innerW = (int)(_imageBinder.Width * widthScale);
                int innerH = (int)(_imageBinder.Height * widthScale);

                //2. viewport size
                this.SetSize(this.Width, innerH);

                this.SetInnerContentSize(
                   (int)innerW,
                   (int)innerH);
#if DEBUG
                _imgRenderBox.dbugBreak = true;
#endif
            }
            else if (this.HasSpecificHeight)
            {
                float heightScale = this.Height / (float)_imageBinder.Height;

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

                this.SetSize(_imageBinder.Width, _imageBinder.Height);
                this.SetInnerContentSize(_imageBinder.Width, _imageBinder.Height);
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
