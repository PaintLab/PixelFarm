//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{


    partial class RenderElement
    {

        internal bool NoClipOrBgIsNotOpaque => !NeedClipArea || (_propFlags & RenderElementConst.TRACKING_BG_IS_NOT_OPAQUE) != 0;

        /// <summary>
        /// background is not 100% opaque
        /// </summary>
        protected bool BgIsNotOpaque
        {
            get => (_propFlags & RenderElementConst.TRACKING_BG_IS_NOT_OPAQUE) != 0;

            set => _propFlags = value ?
                 _propFlags | RenderElementConst.TRACKING_BG_IS_NOT_OPAQUE :
                 _propFlags & ~RenderElementConst.TRACKING_BG_IS_NOT_OPAQUE;
        }

        internal void InvalidateGraphics(InvalidateGfxArgs args)
        {
            //RELATIVE to this ***
            _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            if ((_propFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }

            if (!GlobalRootGraphic.s_SuspendGraphicsUpdate)
            {
                //RELATIVE to this***
                //1.
                _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
                //2.  
                BubbleInvalidater.InternalBubbleUpInvalidateGraphicArea(args);

            }
            else
            {

            }
        }

        /// <summary>
        /// invalidate entire area
        /// </summary>
        public void InvalidateGraphics()
        {
            //RELATIVE to this ***
            _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            if ((_propFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }

            RenderElement parent = this.ParentRenderElement;
            if (parent != null && !parent.BlockGraphicUpdateBubble && !GlobalRootGraphic.s_SuspendGraphicsUpdate)
            {
                BubbleInvalidater.InvalidateGraphicLocalArea(this, new Rectangle(0, 0, _b_width, _b_height));
            }
        }

        public void InvalidateGraphics(Rectangle rect)
        {
            _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            if ((_propFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }
            RenderElement parent = this.ParentRenderElement;
            if (parent != null && !parent.BlockGraphicUpdateBubble && !GlobalRootGraphic.s_SuspendGraphicsUpdate)
            {
                BubbleInvalidater.InvalidateGraphicLocalArea(this, rect);
            }
        }
        protected virtual void OnInvalidateGraphicsNoti(bool fromMe, ref Rectangle totalBounds) { }

        //RELATIVE to its parent
        public void InvalidateParentGraphics() => this.InvalidateParentGraphics(this.RectBounds);

        public void InvalidateParentGraphics(Rectangle totalBounds)
        {
            //RELATIVE to its parent***

            _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            if ((_propFlags & RenderElementConst.LY_REQ_INVALIDATE_RECT_EVENT) != 0)
            {
                OnInvalidateGraphicsNoti(true, ref totalBounds);
            }

            RenderElement parent = this.ParentRenderElement; //start at parent ****
            if (parent != null && !parent.BlockGraphicUpdateBubble && !GlobalRootGraphic.s_SuspendGraphicsUpdate)
            {
                InvalidateGfxArgs arg = BubbleInvalidater.GetInvalidateGfxArgs();
                arg.SetReason_UpdateLocalArea(parent, totalBounds);
                BubbleInvalidater.InternalBubbleUpInvalidateGraphicArea(arg);//RELATIVE to its parent***
            }
        }

        protected void InvalidateGfxLocalArea(Rectangle localArea)
        {
            if (!this.BlockGraphicUpdateBubble)
            {
                BubbleInvalidater.InvalidateGraphicLocalArea(this, localArea);
            }
        }
        internal static bool RequestInvalidateGraphicsNoti(RenderElement re) => (re._propFlags & RenderElementConst.LY_REQ_INVALIDATE_RECT_EVENT) != 0;

        internal static void InvokeInvalidateGraphicsNoti(RenderElement re, bool fromMe, Rectangle totalBounds) => re.OnInvalidateGraphicsNoti(fromMe, ref totalBounds);

        public void SuspendGraphicsUpdate() => _propFlags |= RenderElementConst.LY_SUSPEND_GRAPHIC;

        public void ResumeGraphicsUpdate() => _propFlags &= ~RenderElementConst.LY_SUSPEND_GRAPHIC;

        public bool BlockGraphicUpdateBubble
        {
            get
            {
#if DEBUG
                return (_propFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0;
#else
                return (_uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0;
#endif
            }
        }

        public static bool WaitForStartRenderElement { get; internal set; }
        static bool UnlockForStartRenderElement(RenderElement re)
        {
            if ((re._propFlags & RenderElementConst.TRACKING_GFX_TIP) != 0)
            {
                WaitForStartRenderElement = false;//unlock
                return true;
            }
            return false;
        }
    }
}