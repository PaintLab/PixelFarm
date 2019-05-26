//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomImageRenderBox : CustomRenderBox
    {

        ImageBinder _imageBinder;
        public CustomImageRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.BackColor = Color.LightGray;
        }
        public override void ClearAllChildren()
        {
        }
        public ImageBinder ImageBinder
        {
            get => _imageBinder;
            set => _imageBinder = value;
        }

        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            if (_imageBinder == null) { return; }

            //----------------------------------
            switch (_imageBinder.State)
            {
                case BinderState.Loaded:
                    {
                        canvas.FillRectangle(this.BackColor, 0, 0, this.Width, this.Height);
                        canvas.DrawImage(_imageBinder,
                            new RectangleF(
                            ContentLeft, ContentTop,
                            ContentWidth,
                            ContentHeight));
                    }
                    break;
                case BinderState.Unload:
                    {
                        //wait next round ...
                        if (_imageBinder.HasLazyFunc)
                        {
                            _imageBinder.LazyLoadImage();
                        }
                        else if(_imageBinder is AtlasImageBinder atlas)
                        {
                            //resolve this and draw
                            canvas.DrawImage(_imageBinder,
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