//Apache2, 2014-2017, WinterDev

using System;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class ImageBox : EaseBox
    {
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
        protected override void OnContentUpdate()
        {
            if (imageBinder.State == ImageBinderState.Loaded)
            {
                this.SetDesiredSize(this.imageBinder.ImageWidth, this.imageBinder.ImageHeight);
                this.SetSize(this.imageBinder.ImageWidth, this.imageBinder.ImageHeight);
                this.ParentUI.InvalidateLayout();
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
}
