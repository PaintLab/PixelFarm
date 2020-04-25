//Apache2, 2014-present, WinterDev

using LayoutFarm.RenderBoxes;
using PixelFarm.Drawing;
namespace LayoutFarm
{
    public class TopWindowRenderBox : RenderBoxBase
    {
        public TopWindowRenderBox(RootGraphic rootGfx, int width, int height)
            : base(rootGfx, width, height)
        {
            this.IsTopWindow = true;
            this.HasSpecificWidthAndHeight = true;
        }

        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
            //TODO: implement FillRect() with no blending ... , or FastClear() 
            if (!WaitForStartRenderElement)
            {
                //just clear with white?
                d.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
                d.SetLatestFillAsTextBackgroundColorHint();
            }
            base.RenderClientContent(d, updateArea);
        }
    }
}