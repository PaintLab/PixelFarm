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
        protected override PlainLayer CreateDefaultLayer() => new PlainLayer(this);
        protected override void RenderClientContent(DrawBoard canvas, Rectangle updateArea)
        {
            //TODO: implement FillRect() with no blending ... , or FastClear()
            if (!GlobalRootGraphic.WaitForFirstRenderElement)
            {
                canvas.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
            }
            this.DrawDefaultLayer(canvas, ref updateArea);
        }
    }
}