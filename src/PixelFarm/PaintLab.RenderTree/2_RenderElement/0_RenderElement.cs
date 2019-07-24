//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{

    public abstract partial class RenderElement : IRenderElement
    {

        //------
        //TODO: check if we can remove the _rootGfx here or not ***
        //check if all rendering shound occure in a single thread?
        //------

        RootGraphic _rootGfx;
        IParentLink _parentLink;
        object _controller;
        int _propFlags;
        bool _needClipArea;

        public RenderElement(RootGraphic rootGfx, int width, int height)
        {
            _b_width = width;
            _b_height = height;
            _rootGfx = rootGfx;
            _needClipArea = true;
#if DEBUG
            dbug_totalObjectId++;
            dbug_obj_id = dbug_totalObjectId;
#endif
        }

#if DEBUG
        /// <summary>
        /// on hardware-rendering backing, the system will try to provide a software rendering surface for this element
        /// </summary>
        public bool dbugPreferSoftwareRenderer { get; set; }
#endif
        // 
        public abstract void ResetRootGraphics(RootGraphic rootgfx);
        //
        protected static void DirectSetRootGraphics(RenderElement r, RootGraphic rootgfx)
        {
            r._rootGfx = rootgfx;
        }
        //
        public bool NeedClipArea
        {
            get => _needClipArea;
            set => _needClipArea = value;
        }
        //
        public RootGraphic Root => _rootGfx;
        //
        public RenderElement GetTopWindowRenderBox()
        {
            if (_parentLink == null) { return null; }
            return _rootGfx.TopWindowRenderBox as RenderElement;
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
            _controller = controller;
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
        public bool HasParent => _parentLink != null;
        public virtual void ClearAllChildren()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(nameof(ClearAllChildren) + " no IMPL");
#endif

        }
        public virtual void AddFirst(RenderElement renderE)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(nameof(AddChild) + " no IMPL");
#endif
        }
        public virtual void AddChild(RenderElement renderE)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(nameof(AddChild) + " no IMPL");
#endif
        }
        public virtual void InsertAfter(RenderElement afterElem, RenderElement renderE)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(nameof(InsertAfter) + " no IMPL");
#endif
        }
        public virtual void InsertBefore(RenderElement beforeElem, RenderElement renderE)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(nameof(InsertBefore) + " no IMPL");
#endif
        }
        public virtual void RemoveChild(RenderElement renderE)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(nameof(RemoveChild) + " no IMPL");
#endif
        }
        public virtual void RemoveSelf()
        {
            RenderElement parentLinkRenderE = ParentRenderElement;
            if (parentLinkRenderE != null)
            {
                parentLinkRenderE.RemoveChild(this);
            }
            else if (_parentLink == null)
            {
                _parentLink = null;
            }

        }
        protected bool HasParentLink => _parentLink != null;

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
            get => (_propFlags & RenderElementConst.MAY_HAS_CHILD) != 0;
            protected set
            {
                _propFlags = value ?
                      _propFlags | RenderElementConst.MAY_HAS_CHILD :
                      _propFlags & ~RenderElementConst.MAY_HAS_CHILD;
            }
        }
        public bool MayHasViewport
        {
            get => (_propFlags & RenderElementConst.MAY_HAS_VIEWPORT) != 0;
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
        //
        public bool Visible => ((_propFlags & RenderElementConst.HIDDEN) == 0);
        //
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
                    if (this.NeedClipArea)
                    {
                        this.InvalidateParentGraphics(this.RectBounds);
                    }
                    else
                    {
                        RenderElement firstClipRenderElemParent = GetFirstClipParentRenderElement(this);
                        if (firstClipRenderElemParent != null)
                        {
                            firstClipRenderElemParent.InvalidateGraphics();
                        }
                    }
                }
            }
        }

        static RenderElement GetFirstClipParentRenderElement(RenderElement re)
        {
            RenderElement p = re.ParentRenderElement;
            if (p != null)
            {
                while (!p.NeedClipArea)
                {
                    RenderElement parent = p.ParentRenderElement;
                    if (parent == null)
                    {
                        return p;
                    }
                    p = parent;
                }
                return p;
            }
            return re;
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
                return (_propFlags & RenderElementConst.IS_TOP_RENDERBOX) != 0;
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
                return (_propFlags & RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE) != 0;
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
        //
        public bool VisibleAndHasParent => ((_propFlags & RenderElementConst.HIDDEN) == 0) && (_parentLink != null);
        //
        //==============================================================
        //hit test
        public virtual bool HasCustomHitTest => false;
        protected virtual bool CustomHitTest(HitChain hitChain) => false;

        public bool HitTestCore(HitChain hitChain)
        {
#if DEBUG
            if (hitChain.dbugHitPhase == dbugHitChainPhase.MouseDown)
            {

            }
#endif

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
                        -_b_left + this.ViewportLeft,
                        -_b_top + this.ViewportTop);
                }
                else
                {
                    hitChain.OffsetTestPoint(-_b_left, -_b_top);
                }

                bool customHit = false;
                bool customHitResult = false;
                if (HasCustomHitTest)
                {
                    customHit = true;
                    customHitResult = CustomHitTest(hitChain);
                }
                else
                {
                    hitChain.AddHitObject(this);
                    if (this.MayHasChild)
                    {
                        this.ChildrenHitTestCore(hitChain);
                    }
                }


                if (this.MayHasViewport)
                {
                    hitChain.OffsetTestPoint(
                            _b_left - this.ViewportLeft,
                            _b_top - this.ViewportTop);
                }
                else
                {
                    hitChain.OffsetTestPoint(_b_left, _b_top);
                }

                if (customHit) return customHitResult;

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
                if (_needClipArea)
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
                        -_b_left + this.ViewportLeft,
                        -_b_top + this.ViewportTop);
                }
                else
                {
                    hitChain.OffsetTestPoint(-_b_left, -_b_top);
                }
                bool customHit = false;
                bool customHitResult = false;

                if (HasCustomHitTest)
                {
                    customHit = true;
                    customHitResult = CustomHitTest(hitChain);
                }
                else
                {
                    if (this.MayHasChild)
                    {
                        this.ChildrenHitTestCore(hitChain);
                    }
                }




                if (this.MayHasViewport)
                {
                    hitChain.OffsetTestPoint(
                            _b_left - this.ViewportLeft,
                            _b_top - this.ViewportTop);
                }
                else
                {
                    hitChain.OffsetTestPoint(_b_left, _b_top);
                }

                if (customHit) return customHitResult;

                return this.TransparentForAllEvents ?
                    false :                         //by-pass this element and go to next underlying sibling
                    hitChain.Count > preTestCount;

            }
        }

        //==============================================================
        //render...
        public abstract void CustomDrawToThisCanvas(DrawBoard d, Rectangle updateArea);
        public void DrawToThisCanvas(DrawBoard d, Rectangle updateArea)
        {
            //TODO: rename Canvas to Drawboard ?

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
                if (d.PushClipAreaRect(_b_width, _b_height, ref updateArea))
                {
#if DEBUG
                    if (dbugVRoot.dbug_RecordDrawingChain)
                    {
                        dbugVRoot.dbug_AddDrawElement(this, d);
                    }
#endif
                    //------------------------------------------ 
                    this.CustomDrawToThisCanvas(d, updateArea);
                    //------------------------------------------
                    _propFlags |= RenderElementConst.IS_GRAPHIC_VALID;
#if DEBUG
                    debug_RecordPostDrawInfo(d);
#endif
                }

                d.PopClipAreaRect();
            }
            else
            {
#if DEBUG
                if (dbugVRoot.dbug_RecordDrawingChain)
                {
                    dbugVRoot.dbug_AddDrawElement(this, d);
                }
#endif
                //------------------------------------------ 
                this.CustomDrawToThisCanvas(d, updateArea);
                //------------------------------------------
                _propFlags |= RenderElementConst.IS_GRAPHIC_VALID;
#if DEBUG
                debug_RecordPostDrawInfo(d);
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