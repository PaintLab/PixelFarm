//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class ImageBox : AbstractBox
    {
        ImageStrechKind _imgStretch = ImageStrechKind.None;
        CustomImageRenderBox imgRenderBox;
        ImageBinder imageBinder;
        EventHandler imgChangedSubscribe;

        public ImageBox(int width, int height)
            : base(width, height)
        {
            imgChangedSubscribe = (s, e) => OnContentUpdate();
        }
        public ImageBinder ImageBinder
        {
            get { return this.imageBinder; }
            set
            {
                if (imageBinder != null)
                {
                    //remove prev sub
                    imageBinder.ImageChanged -= imgChangedSubscribe;
                }

                this.imageBinder = value;

                if (this.imgRenderBox != null)
                {
                    this.imgRenderBox.ImageBinder = value;
                    this.InvalidateGraphics();
                }

                if (value != null)
                {
                    //subscribe img changed?
                    value.ImageChanged += imgChangedSubscribe;
                }
            }
        }

        protected override bool HasReadyRenderElement
        {
            get { return imgRenderBox != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (imgRenderBox == null)
            {
                var renderBox = new CustomImageRenderBox(rootgfx, this.Width, this.Height);
                renderBox.SetLocation(this.Left, this.Top);
                renderBox.ImageBinder = imageBinder;
                renderBox.SetController(this);
                renderBox.BackColor = this.BackColor;
                SetPrimaryRenderElement(renderBox);
                this.imgRenderBox = renderBox;
            }
            return this.imgRenderBox;
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.imgRenderBox; }
        }
        public override void SetSize(int width, int height)
        {
            SetElementBoundsWH(width, height);
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize(width, height);
            }
        }
        protected override void SetInnerContentSize(int w, int h)
        {
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize(w, h);
            }
        }
        protected override void OnContentUpdate()
        {
            if (imageBinder.State == ImageBinderState.Loaded)
            {
                //auto scale image

                if (this.HasSpecificSize)
                {
                    switch (_imgStretch)
                    {
                        default: throw new NotSupportedException();
                        case ImageStrechKind.None:
                            this.SetInnerContentSize(this.imageBinder.ImageWidth, this.imageBinder.ImageHeight);
                            break;
                        case ImageStrechKind.FitWidth:
                            float widthScale = this.Width / (float)imageBinder.ImageWidth;
                            this.SetInnerContentSize(
                                (int)(this.imageBinder.ImageWidth * widthScale),
                                (int)(this.imageBinder.ImageHeight * widthScale));
                            break;
                        case ImageStrechKind.FitHeight:
                            //fit img height 
                            //calculate scale ...
                            float heightScale = this.Height / (float)imageBinder.ImageHeight;
                            this.SetInnerContentSize(
                                (int)(this.imageBinder.ImageWidth * heightScale),
                                (int)(this.imageBinder.ImageHeight * heightScale));
                            break;

                    }
                }
                else if (this.HasSpecificWidth)
                {
                    float widthScale = this.Width / (float)imageBinder.ImageWidth;
                    this.SetInnerContentSize(
                        (int)(this.imageBinder.ImageWidth * widthScale),
                        (int)(this.imageBinder.ImageHeight * widthScale));

                    //2. viewport size
                    this.SetSize(this.Width, this.InnerHeight);
                }
                else if (this.HasSpecificHeight)
                {
                    float heightScale = this.Height / (float)imageBinder.ImageHeight;
                    this.SetInnerContentSize(
                        (int)(this.imageBinder.ImageWidth * heightScale),
                        (int)(this.imageBinder.ImageHeight * heightScale));

                    this.SetSize(this.InnerWidth, this.Height);
                }
                else
                {
                    //free scale
                    this.SetInnerContentSize(this.imageBinder.ImageWidth, this.imageBinder.ImageHeight);
                    this.SetSize(this.imageBinder.ImageWidth, this.imageBinder.ImageHeight);

                }
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
            if (imageBinder.State == ImageBinderState.Loaded)
            {
                this.SetSize(this.imageBinder.ImageWidth, this.imageBinder.ImageHeight);
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
