//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomImageRenderBox : CustomRenderBox
    {
        //
        public CustomImageRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {  
            this.BackColor = KnownColors.LightGray;
        }
        public override void ClearAllChildren()
        {
        }
        public ImageBinder ImageBinder { get; set; }

        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
            //this render element dose not have child node, so
            //if WaitForStartRenderElement == true,
            //then we skip rendering its content
            //else if this renderElement has more child, we need to walk down)

            if (WaitForStartRenderElement) { return; }

            if (ImageBinder == null) { return; }

            //----------------------------------
            switch (ImageBinder.State)
            {
                case BinderState.Loaded:
                    {
                        d.FillRectangle(this.BackColor, 0, 0, this.Width, this.Height);
                        d.DrawImage(ImageBinder,
                            new RectangleF(
                            ContentLeft, ContentTop,
                            ContentWidth,
                            ContentHeight));
                    }
                    break;
                case BinderState.Unload:
                    {
                        //wait next round ...
                        if (ImageBinder.HasLazyFunc)
                        {
                            ImageBinder.LazyLoadImage();
                        }
                        else if (ImageBinder is AtlasImageBinder)
                        {
                            //resolve this and draw
                            d.DrawImage(ImageBinder,
                               new RectangleF(
                               ContentLeft, ContentTop,
                               ContentWidth,
                               ContentHeight));
                        }
                    }
                    break;
            }

#if DEBUG
            //canvasPage.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));
#endif
        }
    }
}