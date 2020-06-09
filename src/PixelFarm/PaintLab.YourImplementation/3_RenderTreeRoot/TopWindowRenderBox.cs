//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    public class TopWindowRenderBox : RenderBoxBase
    {
        RootGraphic _rootGfx;
        public TopWindowRenderBox(RootGraphic rootGfx, int width, int height)
            : base(width, height)
        {
            _rootGfx = rootGfx;
            this.IsTopWindow = true;
        }

        protected override RootGraphic Root => _rootGfx; //***

        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
            //TODO: implement FillRect() with no blending ... , or FastClear() 
            if (!WaitForStartRenderElement)
            {
                //just clear with white?
                d.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
                d.TextBackgroundColorHint = Color.White;
            }
            base.RenderClientContent(d, updateArea);
        }
    }
}