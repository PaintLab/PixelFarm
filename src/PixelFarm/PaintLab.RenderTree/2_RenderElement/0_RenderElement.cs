//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{

    public abstract partial class RenderElement : IRenderElement
    {
        RootGraphic _rootGfx;
        IParentLink _parentLink;
        object _controller;
        int _propFlags;
        bool _needClipArea;

        public RenderElement(RootGraphic rootGfx, int width, int height)
        {
            this._b_width = width;
            this._b_height = height;
            this._rootGfx = rootGfx;
#if DEBUG
            dbug_totalObjectId++;
            dbug_obj_id = dbug_totalObjectId;
            //this.dbug_SetFixedElementCode(this.GetType().Name);
#endif
        }

        public abstract void ResetRootGraphics(RootGraphic rootgfx);
        protected static void DirectSetRootGraphics(RenderElement r, RootGraphic rootgfx)
        {
            r._rootGfx = rootgfx;
        }
        public bool NeedClipArea
        {
            get { return _needClipArea; }
            set
            {
                _needClipArea = value;
            }
        }
        public RootGraphic Root
        {
            get { return this._rootGfx; }
        }

        public RenderElement GetTopWindowRenderBox()
        {
            if (_parentLink == null) { return null; }
            return this._rootGfx.TopWindowRenderBox as RenderElement;
        }

        //==============================================================
        //controller-listener
        public object GetController()
        {
            //TODO: move to extension method ***
            return _controller;
        }
        public void SetController(object controller)
        {
            //TODO: move to extension method ***
            this._controller = controller;
        }
        public bool TransparentForAllEvents
        {
            get
            {
                return (_propFlags & RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS) != 0;
            }
            set
            {
                _propFlags = value ?
                       _propFlags | RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS :
                       _propFlags & ~RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS;
            }
        }

        //==============================================================
        //parent/child ...
        public bool HasParent
        {
            get
            {
                return this._parentLink != null;
            }
        }
        public virtual void ClearAllChildren()
        {
        }
        public virtual void AddChild(RenderElement renderE)
        {
        }
        public virtual void RemoveChild(RenderElement renderE)
        {
        }

        protected bool HasParentLink
        {
            get { return this._parentLink != null; }
        }
        public RenderElement ParentRenderElement
        {
            get
            {
                if (_parentLink == null)
                {
                    return null;
                }
                return _parentLink.ParentRenderElement;
            }
        }

        public static void RemoveParentLink(RenderElement childElement)
        {
            childElement._parentLink = null;
        }
        public static void SetParentLink(RenderElement childElement, IParentLink parentLink)
        {
            childElement._parentLink = parentLink;
#if DEBUG
            if (childElement.ParentRenderElement == childElement)
            {
                //error!
                throw new NotSupportedException();
            }
#endif
        }
        public bool MayHasChild
        {
            get { return (_propFlags & RenderElementConst.MAY_HAS_CHILD) != 0; }
            protected set
            {
                _propFlags = value ?
                      _propFlags | RenderElementConst.MAY_HAS_CHILD :
                      _propFlags & ~RenderElementConst.MAY_HAS_CHILD;
            }
        }
        public bool MayHasViewport
        {
            get { return (_propFlags & RenderElementConst.MAY_HAS_VIEWPORT) != 0; }
            protected set
            {
                _propFlags = value ?
                      _propFlags | RenderElementConst.MAY_HAS_VIEWPORT :
                      _propFlags & ~RenderElementConst.MAY_HAS_VIEWPORT;
            }
        }
        public virtual RenderElement FindUnderlyingSiblingAtPoint(Point point)
        {
            return null;
        }

        public virtual void ChildrenHitTestCore(HitChain hitChain)
        {
        }
        //==============================================================
        public bool Visible
        {
            get
            {
                return ((_propFlags & RenderElementConst.HIDDEN) == 0);
            }
        }
        public void SetVisible(bool value)
        {
            //check if visible change? 
            if (this.Visible != value)
            {
                _propFlags = value ?
                    _propFlags & ~RenderElementConst.HIDDEN :
                    _propFlags | RenderElementConst.HIDDEN;
                if (_parentLink != null)
                {
                    this.InvalidateParentGraphics(this.RectBounds);
                }
            }
        }

        public bool IsBlockElement
        {
            get
            {
                return ((_propFlags & RenderElementConst.IS_BLOCK_ELEMENT) == RenderElementConst.IS_BLOCK_ELEMENT);
            }
            set
            {
                _propFlags = value ?
                     _propFlags | RenderElementConst.IS_BLOCK_ELEMENT :
                     _propFlags & ~RenderElementConst.IS_BLOCK_ELEMENT;
            }
        }

        public bool IsTopWindow
        {
            get
            {
                return (this._propFlags & RenderElementConst.IS_TOP_RENDERBOX) != 0;
            }
            set
            {
                _propFlags = value ?
                      _propFlags | RenderElementConst.IS_TOP_RENDERBOX :
                      _propFlags & ~RenderElementConst.IS_TOP_RENDERBOX;
            }
        }

        internal bool HasDoubleScrollableSurface
        {
            get
            {
                return (this._propFlags & RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE) != 0;
            }
            set
            {
                _propFlags = value ?
                      _propFlags | RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE :
                      _propFlags & ~RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE;
            }
        }

        internal bool HasSolidBackground
        {
            get
            {
                return (_propFlags & RenderElementConst.HAS_TRANSPARENT_BG) != 0;
            }
            set
            {
                _propFlags = value ?
                       _propFlags | RenderElementConst.HAS_TRANSPARENT_BG :
                       _propFlags & ~RenderElementConst.HAS_TRANSPARENT_BG;
            }
        }

        public bool VisibleAndHasParent
        {
            get { return ((this._propFlags & RenderElementConst.HIDDEN) == 0) && (this._parentLink != null); }
        }

        //==============================================================
        //hit test

        public bool HitTestCore(HitChain hitChain)
        {


            if ((_propFlags & RenderElementConst.HIDDEN) != 0)
            {
                return false;
            }


            int testX;
            int testY;
            hitChain.GetTestPoint(out testX, out testY);

            if ((testY >= _b_top && testY <= (_b_top + _b_height)
            && (testX >= _b_left && testX <= (_b_left + _b_width))))
            {
                if (this.MayHasViewport)
                {
                    hitChain.OffsetTestPoint(
                        -_b_left + this.ViewportX,
                        -_b_top + this.ViewportY);
                }
                else
                {
                    hitChain.OffsetTestPoint(-_b_left, -_b_top);
                }

                hitChain.AddHitObject(this);
                if (this.MayHasChild)
                {
                    this.ChildrenHitTestCore(hitChain);
                }

                if (this.MayHasViewport)
                {
                    hitChain.OffsetTestPoint(
                            _b_left - this.ViewportX,
                            _b_top - this.ViewportY);
                }
                else
                {
                    hitChain.OffsetTestPoint(_b_left, _b_top);
                }

                if ((_propFlags & RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS) != 0 &&
                    hitChain.TopMostElement == this)
                {
                    hitChain.RemoveCurrentHit();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                //not visual hit on this object..
                if (this._needClipArea)
                {
                    return false;
                }

                //---
                //if this RenderElement not need clip area
                //we should test on its child

                int preTestCount = hitChain.Count;

                if (this.MayHasViewport)
                {
                    hitChain.OffsetTestPoint(
                        -_b_left + this.ViewportX,
                        -_b_top + this.ViewportY);
                }
                else
                {
                    hitChain.OffsetTestPoint(-_b_left, -_b_top);
                }


                if (this.MayHasChild)
                {
                    this.ChildrenHitTestCore(hitChain);
                }

                if (this.MayHasViewport)
                {
                    hitChain.OffsetTestPoint(
                            _b_left - this.ViewportX,
                            _b_top - this.ViewportY);
                }
                else
                {
                    hitChain.OffsetTestPoint(_b_left, _b_top);
                }

                return hitChain.Count > preTestCount;
            }
        }

        //==============================================================
        //render...
        public abstract void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea);
        public void DrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {
            if ((_propFlags & RenderElementConst.HIDDEN) == RenderElementConst.HIDDEN)
            {
                return;
            }
#if DEBUG
            dbugVRoot.dbug_drawLevel++;
#endif

            if (_needClipArea)
            {
                //some elem may need clip for its child
                //some may not need
                if (canvas.PushClipAreaRect(_b_width, _b_height, ref updateArea))
                {
#if DEBUG
                    if (dbugVRoot.dbug_RecordDrawingChain)
                    {
                        dbugVRoot.dbug_AddDrawElement(this, canvas);
                    }
#endif
                    //------------------------------------------ 
                    this.CustomDrawToThisCanvas(canvas, updateArea);
                    //------------------------------------------
                    _propFlags |= RenderElementConst.IS_GRAPHIC_VALID;
#if DEBUG
                    debug_RecordPostDrawInfo(canvas);
#endif
                }

                canvas.PopClipAreaRect();
            }
            else
            {
#if DEBUG
                if (dbugVRoot.dbug_RecordDrawingChain)
                {
                    dbugVRoot.dbug_AddDrawElement(this, canvas);
                }
#endif
                //------------------------------------------ 
                this.CustomDrawToThisCanvas(canvas, updateArea);
                //------------------------------------------
                _propFlags |= RenderElementConst.IS_GRAPHIC_VALID;
#if DEBUG
                debug_RecordPostDrawInfo(canvas);
#endif

            }
#if DEBUG
            dbugVRoot.dbug_drawLevel--;
#endif
        }

        //==============================================================
        //set location and size , not bubble***

        public static void DirectSetSize(RenderElement visualElement, int width, int height)
        {
            visualElement._b_width = width;
            visualElement._b_height = height;
        }
        public static void DirectSetLocation(RenderElement visualElement, int x, int y)
        {
            visualElement._b_left = x;
            visualElement._b_top = y;
        }
    }
}