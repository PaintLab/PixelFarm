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
        //check if all rendering should occur on a single thread?
        //------


        IParentLink _parentLink;
        object _controller;
        internal int _propFlags;

        public RenderElement(int width, int height)
        {
            _b_width = width;
            _b_height = height;
            NeedClipArea = true;
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

        public bool NeedClipArea { get; set; }
        //
        protected virtual RootGraphic Root => null;

        public RootGraphic GetRoot()
        {
            //recursive
            RootGraphic root = Root;//local root
            if (root != null) return root;
            return _parentLink?.ParentRenderElement?.GetRoot();//recursive
        }
        //
        public IContainerRenderElement GetTopWindowRenderBox()
        {
            if (_parentLink == null) { return null; }
            return GetRoot()?.TopWindowRenderBox as IContainerRenderElement;
        }

        //==============================================================
        //controller-listener
        public object GetController() => _controller;

        public void SetController(object controller)
        {
            _controller = controller;
        }

        public bool TransparentForMouseEvents
        {
            get => (_propFlags & RenderElementConst.TRANSPARENT_FOR_MOUSE_INPUT) != 0;

            set
            {
                _propFlags = value ?
                       _propFlags | RenderElementConst.TRANSPARENT_FOR_MOUSE_INPUT :
                       _propFlags & ~RenderElementConst.TRANSPARENT_FOR_MOUSE_INPUT;
            }
        }
        internal static void TrackBubbleUpdateLocalStatus(RenderElement renderE)
        {
            renderE._propFlags |= RenderElementConst.TRACKING_GFX;
        }
        internal static void ResetBubbleUpdateLocalStatus(RenderElement renderE)
        {

#if DEBUG
            if (RenderElement.IsBubbleGfxUpdateTrackedTip(renderE))
            {
                if (!dbugTrackingTipElems.ContainsKey(renderE))
                {
                    throw new NotSupportedException();
                }
                dbugTrackingTipElems.Remove(renderE);
            }

#endif

            renderE._propFlags &= ~(RenderElementConst.TRACKING_GFX | RenderElementConst.TRACKING_GFX_TIP | RenderElementConst.TRACKING_GFX_In_UPDATE_RGN_QUEUE);
            //renderE._propFlags &= ~(RenderElementConst.TRACKING_GFX);
            //renderE._propFlags &= ~(RenderElementConst.TRACKING_GFX_TIP);
        }

#if DEBUG
        internal static int dbugUpdateTrackingCount => dbugTrackingTipElems.Count;
        readonly static System.Collections.Generic.Dictionary<RenderElement, bool> dbugTrackingTipElems = new System.Collections.Generic.Dictionary<RenderElement, bool>();
#endif
        internal static void MarkAsGfxUpdateTip(RenderElement renderE)
        {
#if DEBUG
            if (dbugTrackingTipElems.ContainsKey(renderE))
            {
                //throw new NotSupportedException();
            }
            dbugTrackingTipElems[renderE] = true;
#endif

            renderE._propFlags |= RenderElementConst.TRACKING_GFX_TIP;
        }
        internal static void MarkAsInUpdateRgnQueue(RenderElement renderE)
        {
            renderE._propFlags |= RenderElementConst.TRACKING_GFX_In_UPDATE_RGN_QUEUE;
        }

        internal static bool IsBubbleGfxUpdateTracked(RenderElement re) => (re._propFlags & RenderElementConst.TRACKING_GFX) != 0;
        internal static bool IsBubbleGfxUpdateTrackedTip(RenderElement re) => (re._propFlags & RenderElementConst.TRACKING_GFX_TIP) != 0;
        internal static bool IsInUpdateRgnQueue(RenderElement re) => (re._propFlags & RenderElementConst.TRACKING_GFX_In_UPDATE_RGN_QUEUE) != 0;
        //==============================================================

        protected bool HasParentLink => _parentLink != null;

        public RenderElement ParentRenderElement
        {
            get
            {
                if (_parentLink == null)
                {
                    return null;
                }

#if DEBUG
                if (_parentLink.ParentRenderElement == this)
                {
                    throw new NotSupportedException();
                }
#endif

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

        /// <summary>
        /// this element needs pre-render evalution
        /// </summary>
        public bool NeedPreRenderEval
        {
            get => (_propFlags & RenderElementConst.NEED_PRE_RENDER_EVAL) != 0;
            protected set
            {
                _propFlags = value ?
                      _propFlags | RenderElementConst.NEED_PRE_RENDER_EVAL :
                      _propFlags & ~RenderElementConst.NEED_PRE_RENDER_EVAL;
            }
        }


        public virtual void ChildrenHitTestCore(HitChain hitChain)
        {
        }
        //==============================================================
        //
        public bool Visible => (_propFlags & RenderElementConst.HIDDEN) == 0;
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


        public bool IsTopWindow
        {
            get => (_propFlags & RenderElementConst.IS_TOP_RENDERBOX) != 0;

            protected set => _propFlags = value ?
                        _propFlags | RenderElementConst.IS_TOP_RENDERBOX :
                        _propFlags & ~RenderElementConst.IS_TOP_RENDERBOX;
        }


        internal bool HasDoubleScrollableSurface
        {
            get => (_propFlags & RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE) != 0;

            set => _propFlags = value ?
                      _propFlags | RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE :
                      _propFlags & ~RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE;
        }

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

            hitChain.GetTestPoint(out int testX, out int testY);

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

                if ((_propFlags & RenderElementConst.TRANSPARENT_FOR_MOUSE_INPUT) != 0 &&
                    hitChain.Exclude_TransparentMouse_Element &&
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
                if (NeedClipArea)
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

                return this.TransparentForMouseEvents ?
                    false :                         //by-pass this element and go to next underlying sibling
                    hitChain.Count > preTestCount;

            }
        }

        //==============================================================
        //RenderClientContent()...
        //if we set MayHasViewport = true, the root graphics will be offset the proper position
        //if we set MayHasViewport= false, we need to offset the root graphics manually. 
        protected abstract void RenderClientContent(DrawBoard d, UpdateArea updateArea);

        protected virtual void PreRenderEvaluation(DrawBoard d)
        {
            //need to set flags RenderElementConst.NEED_PRE_RENDER_EVAL to _propFlags 
        }
        public static void InvokePreRenderEvaluation(RenderElement r)
        {
            r.PreRenderEvaluation(null);
        }

        void IRenderElement.Render(DrawBoard d, UpdateArea updateArea) => Render(this, d, updateArea);

        public static void Render(RenderElement renderE, DrawBoard d, UpdateArea updateArea)
        {
            //TODO: rename Canvas to Drawboard ?
#if DEBUG
            if (renderE.dbugBreak)
            {

            }
#endif
            if ((renderE._propFlags & RenderElementConst.HIDDEN) == RenderElementConst.HIDDEN)
            {
                return;
            }

            if (WaitForStartRenderElement)
            {
                //special
                if (!RenderElement.IsBubbleGfxUpdateTracked(renderE))
                {
                    //special mode*** 
                    //in this mode if this elem is not tracked
                    //then return 

#if DEBUG
                    System.Diagnostics.Debug.WriteLine("skip_render:" + renderE.Width + "x" + renderE.Height);
#endif

                    return;
                }
                else
                {
                    UnlockForStartRenderElement(renderE);
                }
            }

#if DEBUG
            renderE.dbugVRoot.dbug_drawLevel++;
#endif
            if ((renderE._propFlags & RenderElementConst.NEED_PRE_RENDER_EVAL) == RenderElementConst.NEED_PRE_RENDER_EVAL)
            {
                //pre render evaluation before any clip
                //eg. content size may be invalid,
                renderE.PreRenderEvaluation(d);
            }

            if (renderE.NeedClipArea)
            {
                //some elem may need clip for its child
                //some may not need

                if (d.PushClipAreaRect(renderE._b_width, renderE._b_height, updateArea))
                {
                    //backup ***, new clip is applied to renderE's children node only, 
                    //it will be restored later, for other renderE's sibling
                    Rectangle prev_rect = updateArea.PreviousRect;
#if DEBUG
                    if (renderE.dbugVRoot.dbug_RecordDrawingChain)
                    {
                        renderE.dbugVRoot.dbug_AddDrawElement(renderE, d);
                    }
#endif

                    if ((renderE._propFlags & RenderElementConst.MAY_HAS_VIEWPORT) != 0)
                    {
                        int viewportLeft = renderE.ViewportLeft;
                        int viewportTop = renderE.ViewportTop;

                        if (viewportLeft == 0 && viewportTop == 0)
                        {
                            renderE.RenderClientContent(d, updateArea);
                        }
                        else
                        {
                            int enterCanvasX = d.OriginX;
                            int enterCanvasY = d.OriginY;

                            d.SetCanvasOrigin(enterCanvasX - viewportLeft, enterCanvasY - viewportTop);
                            updateArea.Offset(viewportLeft, viewportTop);

                            //---------------
                            renderE.RenderClientContent(d, updateArea);
                            //---------------
#if DEBUG
                            //for debug
                            // canvas.dbug_DrawCrossRect(Color.Red,updateArea);
#endif
                            d.SetCanvasOrigin(enterCanvasX, enterCanvasY); //restore 
                            updateArea.Offset(-viewportLeft, -viewportTop);

                        }
                    }
                    else
                    {
                        //------------------------------------------
                        renderE.RenderClientContent(d, updateArea);
                        //------------------------------------------
                    }


#if DEBUG
                    renderE.debug_RecordPostDrawInfo(d);
#endif
                    d.PopClipAreaRect();
                    updateArea.CurrentRect = prev_rect; //restore for other renderE sibling
                }
            }
            else
            {
#if DEBUG
                if (renderE.dbugVRoot.dbug_RecordDrawingChain)
                {
                    renderE.dbugVRoot.dbug_AddDrawElement(renderE, d);
                }
#endif
                //------------------------------------------ 

                if ((renderE._propFlags & RenderElementConst.MAY_HAS_VIEWPORT) != 0)
                {
                    int viewportLeft = renderE.ViewportLeft;
                    int viewportTop = renderE.ViewportTop;


                    if (viewportLeft == 0 && viewportTop == 0)
                    {
                        renderE.RenderClientContent(d, updateArea);
                    }
                    else
                    {
                        int enterCanvasX = d.OriginX;
                        int enterCanvasY = d.OriginY;

                        d.SetCanvasOrigin(enterCanvasX - viewportLeft, enterCanvasY - viewportTop);
                        updateArea.Offset(viewportLeft, viewportTop);

                        //---------------
                        renderE.RenderClientContent(d, updateArea);
                        //---------------
#if DEBUG
                        //for debug
                        // canvas.dbug_DrawCrossRect(Color.Red,updateArea);
#endif
                        d.SetCanvasOrigin(enterCanvasX, enterCanvasY); //restore 
                        updateArea.Offset(-viewportLeft, -viewportTop);

                    }
                }
                else
                {

                    renderE.RenderClientContent(d, updateArea);
                }
                //------------------------------------------

#if DEBUG
                renderE.debug_RecordPostDrawInfo(d);
#endif

            }
#if DEBUG
            renderE.dbugVRoot.dbug_drawLevel--;
#endif
        }


    }
}