//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    partial class RenderElement
    {
        public void SetWidth(int width)
        {
#if DEBUG
            //if (this.dbugBreak)
            //{

            //}
#endif
            this.SetSize(width, this.b_height);
        }
        public void SetHeight(int height)
        {
#if DEBUG
            //if (this.dbugBreak)
            //{

            //}
#endif
            this.SetSize(this.b_width, height);
        }
        public void SetSize(int width, int height)
        {
#if DEBUG
            //if (this.dbugBreak)
            //{

            //}
#endif
            if (parentLink == null)
            {
                //direct set size
                this.b_width = width;
                this.b_height = height;
            }
            else
            {
                Rectangle prevBounds = this.RectBounds;
                this.b_width = width;
                this.b_height = height;
                //combine before and after rect 
                //add to invalidate root invalidate queue  
                this.InvalidateParentGraphics(Rectangle.Union(prevBounds, this.RectBounds));

            }
        }

        public void SetLocation(int left, int top)
        {
            if (parentLink == null)
            {
                this.b_left = left;
                this.b_top = top;
            }
            else
            {
                //set location not affect its content size 

                Rectangle prevBounds = this.RectBounds;
                //----------------

                this.b_left = left;
                this.b_top = top;
                //----------------   
                //combine before and after rect  
                //add to invalidate root invalidate queue
                this.InvalidateParentGraphics(Rectangle.Union(prevBounds, this.RectBounds));

            }
        }

        public void SetBounds(int left, int top, int width, int height)
        {

            if (parentLink == null)
            {
                this.b_left = left;
                this.b_top = top;
                this.b_width = width;
                this.b_height = height;
            }
            else
            {
                Rectangle prevBounds = this.RectBounds;
                this.b_left = left;
                this.b_top = top;
                this.b_width = width;
                this.b_height = height;
                this.InvalidateParentGraphics(Rectangle.Union(prevBounds, this.RectBounds));

            }
        }
    }
}