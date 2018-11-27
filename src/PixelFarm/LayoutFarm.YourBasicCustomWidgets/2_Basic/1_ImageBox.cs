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
        public override void SetInnerContentSize(int w, int h)
        {
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize(w, h);
            }
        }
        protected override void OnContentUpdate()
        {
            if (imageBinder.State == BinderState.Loaded)
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
            if (imageBinder.State == BinderState.Loaded)
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
                        this.SetInnerContentSize(this.imageBinder.Width, this.imageBinder.Height);
                        break;
                    case ImageStrechKind.FitWidth:
                        float widthScale = this.Width / (float)imageBinder.Width;
                        this.SetInnerContentSize(
                            (int)(this.imageBinder.Width * widthScale),
                            (int)(this.imageBinder.Height * widthScale));
                        break;
                    case ImageStrechKind.FitHeight:
                        //fit img height 
                        //calculate scale ...
                        float heightScale = this.Height / (float)imageBinder.Height;
                        this.SetInnerContentSize(
                            (int)(this.imageBinder.Width * heightScale),
                            (int)(this.imageBinder.Height * heightScale));
                        break;

                }
            }
            else if (this.HasSpecificWidth)
            {
                float widthScale = this.Width / (float)imageBinder.Width;

                int innerW = (int)(this.imageBinder.Width * widthScale);
                int innerH = (int)(this.imageBinder.Height * widthScale);

                //2. viewport size
                this.SetSize(this.Width, innerH);

                this.SetInnerContentSize(
                   (int)innerW,
                   (int)innerH);
#if DEBUG
                imgRenderBox.dbugBreak = true;
#endif
            }
            else if (this.HasSpecificHeight)
            {
                float heightScale = this.Height / (float)imageBinder.Height;

                int innerW = (int)(this.imageBinder.Width * heightScale);
                int innerH = (int)(this.imageBinder.Height * heightScale);


                this.SetSize(innerW, this.Height);


                this.SetInnerContentSize(
                   (int)innerW,
                   (int)innerH);

            }
            else
            {
                //free scale

                this.SetSize(this.imageBinder.Width, this.imageBinder.Height);
                this.SetInnerContentSize(this.imageBinder.Width, this.imageBinder.Height);
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
