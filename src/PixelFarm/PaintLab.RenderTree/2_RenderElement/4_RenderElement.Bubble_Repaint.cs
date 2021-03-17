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
            if ((_propFlags & RenderElementConst.SUSPEND_GRAPHICS) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif

                //early exit 
                BubbleInvalidater.ReleaseInvalidateGfxArgs(args);
                return;
            }

            if (this.IsTopWindow)
            {
                //top window does not have parent
                BubbleInvalidater.InternalBubbleUpInvalidateGraphicArea(args);
            }
            else
            {
                RenderElement parent = this.ParentRenderElement;
                if (parent != null && !parent.BlockGraphicUpdateBubble)
                {
                    BubbleInvalidater.InternalBubbleUpInvalidateGraphicArea(args);
                }
            }
        }

        /// <summary>
        /// invalidate entire area
        /// </summary>
        public void InvalidateGraphics()
        {
            //RELATIVE to this ***
            //
            if ((_propFlags & RenderElementConst.SUSPEND_GRAPHICS) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }

            if (this.IsTopWindow)
            {
                //top window does not have parent
                BubbleInvalidater.InvalidateGraphicLocalArea(this, new Rectangle(0, 0, _b_width, _b_height));
            }
            else
            {
                RenderElement parent = this.ParentRenderElement;
                if (parent != null && !parent.BlockGraphicUpdateBubble)
                {
                    BubbleInvalidater.InvalidateGraphicLocalArea(this, new Rectangle(0, 0, _b_width, _b_height));
                }
            }

        }
        public void InvalidateGraphics(Rectangle localArea)
        {
#if DEBUG
            if (this.IsTopWindow)
            {
                //System.Diagnostics.Debugger.Break();
            }
#endif
            //
            if ((_propFlags & RenderElementConst.SUSPEND_GRAPHICS) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }
            if (this.IsTopWindow)
            {
                BubbleInvalidater.InvalidateGraphicLocalArea(this, localArea);
            }
            else
            {
                RenderElement parent = this.ParentRenderElement;
                if (parent != null && !parent.BlockGraphicUpdateBubble)
                {
                    BubbleInvalidater.InvalidateGraphicLocalArea(this, localArea);
                }
            }
        }

        protected virtual void OnInvalidateGraphicsNoti(bool fromMe, ref Rectangle totalBounds) { }

        //RELATIVE to its parent
        public void InvalidateParentGraphics() => this.InvalidateParentGraphics(this.RectBounds);

        public void InvalidateParentGraphics(Rectangle totalBounds)
        {
            //RELATIVE to its parent***
#if DEBUG
            if (this.IsTopWindow)
            {
                System.Diagnostics.Debugger.Break();
            }
#endif

            if ((_propFlags & RenderElementConst.REQ_INVALIDATE_RECT_EVENT) != 0)
            {
                OnInvalidateGraphicsNoti(true, ref totalBounds);
            }

            RenderElement parent = this.ParentRenderElement; //start at parent ****
            if (parent != null && !parent.BlockGraphicUpdateBubble)
            {
                parent.InvalidateGraphics(totalBounds);
            }
        }


        internal static bool RequestInvalidateGraphicsNoti(RenderElement re) => (re._propFlags & RenderElementConst.REQ_INVALIDATE_RECT_EVENT) != 0;

        internal static void InvokeInvalidateGraphicsNoti(RenderElement re, bool fromMe, Rectangle totalBounds) => re.OnInvalidateGraphicsNoti(fromMe, ref totalBounds);

        public void SuspendGraphicsUpdate() => _propFlags |= RenderElementConst.SUSPEND_GRAPHICS;

        public void ResumeGraphicsUpdate() => _propFlags &= ~RenderElementConst.SUSPEND_GRAPHICS;

        public bool BlockGraphicUpdateBubble => (_propFlags & RenderElementConst.SUSPEND_GRAPHICS) != 0;

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