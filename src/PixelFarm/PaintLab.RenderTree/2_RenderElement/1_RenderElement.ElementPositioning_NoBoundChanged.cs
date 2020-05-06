//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    partial class RenderElement
    {
        //----------------------
        //rectangle boundary area 
        //for fast approximation
        int _b_top;
        int _b_left;
        int _b_width;
        int _b_height;


        int _uiLayoutFlags;
        //------------------------ 

        /// <summary>
        /// rectangle bounds relative to its parent element
        /// </summary>
        public Rectangle RectBounds => new Rectangle(_b_left, _b_top, _b_width, _b_height);
        //
        public Size Size => new Size(_b_width, _b_height);
        //
        public int X => _b_left;
        public int Y => _b_top;
        public int Left => _b_left;
        public int Top => _b_top;
        //
        public int Right => _b_left + _b_width;
        public int Bottom => _b_top + _b_height;
        //
        public Point Location => new Point(_b_left, _b_top);
        //
        public int Width => _b_width;
        public int Height => _b_height;

        //-----------------------------------------------
        public virtual int ViewportLeft => 0;
        public virtual int ViewportTop => 0;
        //
        public int ViewportBottom => this.ViewportTop + this.Height;
        public int ViewportRight => this.ViewportLeft + this.Width;

        //
        public virtual void SetViewport(int viewportLeft, int viewportTop)
        {
        }

        public virtual Size InnerContentSize => this.Size;
        public virtual Rectangle InnerContentBounds
        {
            get
            {
                Size s = InnerContentSize;
                return new Rectangle(_b_left, _b_top, s.Width, s.Height);
            }
        }
        //-----------------------------------------------

        public Point GetGlobalLocation() => GetGlobalLocationStatic(this);

        static Point GetGlobalLocationStatic(RenderElement re)
        {
            RenderElement parentVisualElement = re.ParentRenderElement;
            if (parentVisualElement != null)
            {
                Point parentGlobalLocation = GetGlobalLocationStatic(parentVisualElement);

                int p_x = parentGlobalLocation.X;
                int p_y = parentGlobalLocation.Y;

                re._parentLink.AdjustLocation(ref p_x, ref p_y);

                if (parentVisualElement.MayHasViewport)
                {
                    return new Point(
                        re._b_left + p_x - parentVisualElement.ViewportLeft,
                        re._b_top + p_y - parentVisualElement.ViewportTop);
                }
                else
                {
                    return new Point(re._b_left + p_x, re._b_top + p_y);
                }
            }
            else
            {
                return re.Location;
            }
        }
        //----------------------------------------------- 
        //public bool HasSpecificWidth
        //{
        //    get => ((_uiLayoutFlags & RenderElementConst.LY_HAS_SPC_WIDTH) == RenderElementConst.LY_HAS_SPC_WIDTH);

        //    set => _uiLayoutFlags = value ?
        //           _uiLayoutFlags | RenderElementConst.LY_HAS_SPC_WIDTH :
        //           _uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_WIDTH;
        //}
        //public bool HasSpecificHeight
        //{
        //    get => ((_uiLayoutFlags & RenderElementConst.LY_HAS_SPC_HEIGHT) == RenderElementConst.LY_HAS_SPC_HEIGHT);

        //    set => _uiLayoutFlags = value ?
        //           _uiLayoutFlags | RenderElementConst.LY_HAS_SPC_HEIGHT :
        //           _uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_HEIGHT;
        //}

        //public bool HasSpecificWidthAndHeight
        //{
        //    get => ((_uiLayoutFlags & RenderElementConst.LY_HAS_SPC_SIZE) != 0);

        //    set => _uiLayoutFlags = value ?
        //            _uiLayoutFlags | RenderElementConst.LY_HAS_SPC_SIZE :
        //            _uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_SIZE;

        //}
        protected bool NeedInvalidateRectEvent
        {
            get => ((_uiLayoutFlags & RenderElementConst.LY_REQ_INVALIDATE_RECT_EVENT) != 0);

            set => _uiLayoutFlags = value ?
                    _uiLayoutFlags | RenderElementConst.LY_REQ_INVALIDATE_RECT_EVENT :
                    _uiLayoutFlags & ~RenderElementConst.LY_REQ_INVALIDATE_RECT_EVENT;

        }

        public bool Contains(Point testPoint) => ((_propFlags & RenderElementConst.HIDDEN) != 0) ?
                                                    false :
                                                    ContainPoint(testPoint.X, testPoint.Y);

        public bool ContainPoint(int x, int y) => ((x >= _b_left && x < Right) && (y >= _b_top && y < Bottom));

        public bool ContainRect(Rectangle r)
        {
            return r.Left >= _b_left &&
                    r.Top >= _b_top &&
                    r.Right <= _b_left + _b_width &&
                    r.Bottom <= _b_top + _b_height;
        }
        public bool ContainRect(int x, int y, int width, int height)
        {
            return x >= _b_left &&
                    y >= _b_top &&
                    x + width <= _b_left + _b_width &&
                    y + height <= _b_top + _b_height;
        }
        /// <summary>
        /// no rect change
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool IntersectsWith(Rectangle r)
        {
            int left = _b_left;
            if (((left <= r.Left) && (this.Right > r.Left)) ||
                ((left >= r.Left) && (left < r.Right)))
            {
                int top = _b_top;
                return (((top <= r.Top) && (this.Bottom > r.Top)) ||
                          ((top >= r.Top) && (top < r.Bottom)));
            }
            return false;
        }
        public bool IntersectsWith(UpdateArea r)
        {
            int left = _b_left;
            if (((left <= r.Left) && (this.Right > r.Left)) ||
                ((left >= r.Left) && (left < r.Right)))
            {
                int top = _b_top;
                return (((top <= r.Top) && (this.Bottom > r.Top)) ||
                          ((top >= r.Top) && (top < r.Bottom)));
            }
            return false;
        }
        /// <summary>
        /// no rect change
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool IntersectOnHorizontalWith(Rectangle r)
        {
            int left = _b_left;
            return (((left <= r.Left) && (this.Right > r.Left)) ||
                     ((left >= r.Left) && (left < r.Right)));
        }
    }
}