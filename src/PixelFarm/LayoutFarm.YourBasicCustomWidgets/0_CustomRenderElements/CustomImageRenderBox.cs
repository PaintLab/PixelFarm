//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomImageRenderBox : CustomRenderBox
    {

        ImageBinder _imageBinder;
        int _paddingLeft, _paddingTop, _paddingRight, _paddingBottom;
        bool _hasSomePadding;
        bool _evalPadding;//evaluate padding again

        public CustomImageRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            _evalPadding = true;
            this.BackColor = Color.LightGray;
        }
        public int PaddingLeft
        {
            get => _paddingLeft;
            set
            {
                _paddingLeft = value;
                _evalPadding = true;
            }
        }
        public int PaddingTop
        {
            get => _paddingTop;
            set
            {
                _paddingTop = value;
                _evalPadding = true;
            }
        }
        public int PaddingBottom
        {
            get => _paddingBottom;
            set
            {
                _paddingBottom = value;
                _evalPadding = true;
            }
        }
        public int PaddingRight
        {
            get => _paddingRight;
            set
            {
                _paddingRight = value;
                _evalPadding = true;
            }
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
                        if (_evalPadding)
                        {
                            _hasSomePadding = _paddingLeft != 0 || _paddingRight != 0 || _paddingBottom != 0 || _paddingTop != 0;
                            _evalPadding = false;
                        }
                        if (_hasSomePadding)
                        {
                            canvas.FillRectangle(this.BackColor, 0, 0, this.Width, this.Height);
                            canvas.DrawImage(_imageBinder,
                                new RectangleF(
                                PaddingLeft, PaddingTop,
                                this.Width - (PaddingLeft + PaddingRight),
                                this.Height - (PaddingTop + PaddingBottom)));
                        }
                        else
                        {

                            canvas.DrawImage(_imageBinder,
                               new RectangleF(
                               0, 0,
                               this.Width,
                               this.Height));
                        }
                    }
                    break;
                case BinderState.Unload:
                    {
                        //wait next round ...
                        if (_imageBinder.HasLazyFunc)
                        {
                            _imageBinder.LazyLoadImage();
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