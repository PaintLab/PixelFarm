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
            this.SetSize(width, _b_height);
        }

        public void SetHeight(int height)
        {
#if DEBUG
            //if (this.dbugBreak)
            //{

            //}
#endif
            this.SetSize(_b_width, height);
        }

        public void SetSize(int width, int height)
        {
#if DEBUG
            //if (this.dbugBreak)
            //{

            //}
#endif
            if (_parentLink == null)
            {
                //direct set size
                _b_width = width;
                _b_height = height;
            }
            else
            {
                if (_b_width != width ||
                    _b_height != height)
                {
                    Rectangle prevBounds = this.RectBounds;
                    _b_width = width;
                    _b_height = height;

                    //combine before and after rect 
                    //add to invalidate root invalidate queue  
                    if (!BlockGraphicUpdateBubble)
                    {
                        this.InvalidateParentGraphics(Rectangle.Union(prevBounds, this.RectBounds));
                    }
                }
            }
        }

        public void SetLocation(int left, int top)
        {
            if (_parentLink == null)
            {
                _b_left = left;
                _b_top = top;
            }
            else
            {

                if (_b_left != left || _b_top != top)
                {
                    //set location not affect its content size 
                    if (!NeedClipArea)
                    {


                        Rectangle prevBounds = new Rectangle(_b_left, _b_top, _b_width, _b_height);

                        int diff_x = left - _b_left;
                        int diff_y = top - _b_top;
                        Rectangle newBounds = prevBounds;
                        newBounds.Offset(diff_x, diff_y);
                        //----------------
                        _b_left = left;
                        _b_top = top;
                        //----------------   
                        //combine before and after rect  
                        //add to invalidate root invalidate queue

                        if (!BlockGraphicUpdateBubble)
                        {
                            GetFirstClipParentRenderElement(this)?.InvalidateGraphics();
                        }
                    }
                    else
                    {
                        Rectangle prevBounds = this.RectBounds;
                        //----------------
                        _b_left = left;
                        _b_top = top;
                        //----------------   
                        //combine before and after rect  
                        //add to invalidate root invalidate queue
                        if (!BlockGraphicUpdateBubble)
                        {
                            this.InvalidateParentGraphics(Rectangle.Union(prevBounds, this.RectBounds));
                        }
                    }
                }
            }
        }

        public void SetBounds(int left, int top, int width, int height)
        {
            if (_parentLink == null)
            {
                _b_left = left;
                _b_top = top;
                _b_width = width;
                _b_height = height;
            }
            else
            {
                if (_b_left != left ||
                   _b_top != top ||
                   _b_width != width ||
                   _b_height != height)
                {

                    Rectangle prevBounds = this.RectBounds;
                    //bound changed
                    _b_left = left;
                    _b_top = top;
                    _b_width = width;
                    _b_height = height;

                    if (!BlockGraphicUpdateBubble)
                    {
                        this.InvalidateParentGraphics(Rectangle.Union(prevBounds, this.RectBounds));
                    }
                }
            }
        }
    }
}