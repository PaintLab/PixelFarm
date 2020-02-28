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
        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
            //TODO: implement FillRect() with no blending ... , or FastClear() 
            if (WaitForStartRenderElement)
            {
                //special mode***
                if (!this.IsBubbleGfxUpdateTracked)
                {
                    //in this mode if this elem is not tracked
                    //then return
                    return;
                }

                //if this is tracked=> it may not be the first render element 
                //(eg. it may be a background elem, it is tracked.)

                if (UnlockForStartRenderElement(this))
                {
                    //unlocked at this element
                    //so we fill background-content
                    d.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
                }
                //because it is tracked-> so child/descendant  of this node should be a start node
                this.DrawDefaultLayer(d, updateArea);
            }
            else
            {
                //not wait => normal render...
                d.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
                this.DrawDefaultLayer(d, updateArea);
            }

        }
    }
}