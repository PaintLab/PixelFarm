//MIT, 2014-present, WinterDev

namespace PixelFarm.Drawing
{
    public abstract class CustomRenderSurface
    {
        public CustomRenderSurface()
        {
        }

        public abstract bool FullModeUpdate
        {
            get;
            set;
        }
        public abstract int Width
        {
            get;
        }
        public abstract int Height
        {
            get;
        }
        public abstract void ConfirmSizeChanged();
        public abstract void QuadPagesCalculateCanvas();
        public abstract Size OwnerInnerContentSize
        {
            get;
        }
        public abstract void DrawToThisPage(DrawBoard destPage, Rectangle updateArea);
    }
}