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
        public Rectangle RectBounds
        {
            get
            {
                return new Rectangle(_b_left, _b_top, _b_width, _b_height);
            }
        }
        public Size Size
        {
            get
            {
                return new Size(_b_width, _b_height);
            }
        }
        public int X
        {
            get
            {
                return _b_left;
            }
        }
        public int Y
        {
            get
            {
                return _b_top;
            }
        }
        public int Right
        {
            get
            {
                return _b_left + _b_width;
            }
        }
        public int Bottom
        {
            get
            {
                return _b_top + _b_height;
            }
        }
        public Point Location
        {
            get
            {
                return new Point(_b_left, _b_top);
            }
        }
        public int Width
        {
            get
            {

                return _b_width;
            }
        }
        public int Height
        {
            get
            {
                return _b_height;
            }
        }


        //-----------------------------------------------
        public virtual int ViewportY
        {
            get
            {
                return 0;
            }
        }
        public virtual int ViewportX
        {
            get
            {
                return 0;
            }
        }
        public int ViewportBottom
        {
            get
            {
                return this.ViewportY + this.Height;
            }
        }
        public int ViewportRight
        {
            get
            {
                return this.ViewportX + this.Width;
            }
        }

        public virtual void SetViewport(int viewportX, int viewportY)
        {
            //do nothing
        }
        public virtual Size InnerContentSize
        {
            get { return this.Size; }
        }
        //-----------------------------------------------
        public Point GetGlobalLocation()
        {
            return GetGlobalLocationStatic(this);
        }
        static Point GetGlobalLocationStatic(RenderElement re)
        {
            RenderElement parentVisualElement = re.ParentRenderElement;
            if (parentVisualElement != null)
            {
                Point parentGlobalLocation = GetGlobalLocationStatic(parentVisualElement);
                re._parentLink.AdjustLocation(ref parentGlobalLocation);
                if (parentVisualElement.MayHasViewport)
                {
                    return new Point(
                        re._b_left + parentGlobalLocation.X - parentVisualElement.ViewportX,
                        re._b_top + parentGlobalLocation.Y - parentVisualElement.ViewportY);
                }
                else
                {
                    return new Point(re._b_left + parentGlobalLocation.X, re._b_top + parentGlobalLocation.Y);
                }
            }
            else
            {
                return re.Location;
            }
        }
        //----------------------------------------------- 
        public bool HasSpecificWidth
        {
            get
            {
                return ((_uiLayoutFlags & RenderElementConst.LY_HAS_SPC_WIDTH) == RenderElementConst.LY_HAS_SPC_WIDTH);
            }
            set
            {
                _uiLayoutFlags = value ?
                   _uiLayoutFlags | RenderElementConst.LY_HAS_SPC_WIDTH :
                   _uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_WIDTH;
            }
        }
        public bool HasSpecificHeight
        {
            get
            {
                return ((_uiLayoutFlags & RenderElementConst.LY_HAS_SPC_HEIGHT) == RenderElementConst.LY_HAS_SPC_HEIGHT);
            }
            set
            {
                _uiLayoutFlags = value ?
                    _uiLayoutFlags | RenderElementConst.LY_HAS_SPC_HEIGHT :
                    _uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_HEIGHT;
            }
        }

        public bool HasSpecificWidthAndHeight
        {
            get
            {
                return ((_uiLayoutFlags & RenderElementConst.LY_HAS_SPC_SIZE) != 0);
            }
            set
            {
                _uiLayoutFlags = value ?
                    _uiLayoutFlags | RenderElementConst.LY_HAS_SPC_SIZE :
                    _uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_SIZE;
            }
        }


        public bool Contains(Point testPoint)
        {
            return ((_propFlags & RenderElementConst.HIDDEN) != 0) ?
                        false :
                        ContainPoint(testPoint.X, testPoint.Y);
        }
        public bool ContainPoint(int x, int y)
        {
            return ((x >= _b_left && x < Right) && (y >= _b_top && y < Bottom));
        }
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
        public bool IntersectsWith(ref Rectangle r)
        {
            int left = this._b_left;
            if (((left <= r.Left) && (this.Right > r.Left)) ||
                ((left >= r.Left) && (left < r.Right)))
            {
                int top = this._b_top;
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
        public bool IntersectOnHorizontalWith(ref Rectangle r)
        {
            int left = this._b_left;
            return (((left <= r.Left) && (this.Right > r.Left)) ||
                     ((left >= r.Left) && (left < r.Right)));
        }
    }
}