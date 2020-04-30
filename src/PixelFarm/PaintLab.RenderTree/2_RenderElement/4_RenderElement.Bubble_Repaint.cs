//Apache2, 2014-present, WinterDev

using System.Collections.Generic;

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
            if ((_uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }

            if (!GlobalRootGraphic.SuspendGraphicsUpdate)
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
        /// invalidate specific area 
        /// </summary>
        /// <param name="rect"></param>
        public void InvalidateGraphics(Rectangle rect)
        {
            _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            if ((_uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }

            if (!GlobalRootGraphic.SuspendGraphicsUpdate)
            {
                BubbleInvalidater.InvalidateGraphicLocalArea(this, rect);
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
            if ((_uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }

            if (!GlobalRootGraphic.SuspendGraphicsUpdate)
            {
                BubbleInvalidater.InvalidateGraphicLocalArea(this, new Rectangle(0, 0, _b_width, _b_height));
                //InvalidateGraphicLocalArea(this, new Rectangle(0, 0, _b_width, _b_height));
            }
            else
            {

            }
        }

        public void InvalidateParentGraphics()
        {
            //RELATIVE to its parent
            this.InvalidateParentGraphics(this.RectBounds);
        }

        protected virtual void OnInvalidateGraphicsNoti(bool fromMe, ref Rectangle totalBounds) { }

        public void InvalidateParentGraphics(Rectangle totalBounds)
        {
            //RELATIVE to its parent***

            _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            RenderElement parent = this.ParentRenderElement; //start at parent ****

            //--------------------------------------- 
            if ((_uiLayoutFlags & RenderElementConst.LY_REQ_INVALIDATE_RECT_EVENT) != 0)
            {
                OnInvalidateGraphicsNoti(true, ref totalBounds);
            }
            //
            if (parent != null)
            {
                if (!GlobalRootGraphic.SuspendGraphicsUpdate)
                {

                    InvalidateGfxArgs arg = BubbleInvalidater.GetInvalidateGfxArgs();
                    arg.SetReason_UpdateLocalArea(parent, totalBounds);
                    BubbleInvalidater.InternalBubbleUpInvalidateGraphicArea(arg);//RELATIVE to its parent***

                }
                else
                {

                }
            }
        }

        protected void InvalidateGfxLocalArea(Rectangle localArea)
        {
            BubbleInvalidater.InvalidateGraphicLocalArea(this, localArea);
        }
        internal static bool RequestInvalidateGraphicsNoti(RenderElement re)
        {
            return (re._uiLayoutFlags & RenderElementConst.LY_REQ_INVALIDATE_RECT_EVENT) != 0;
        }
        internal static void InvokeInvalidateGraphicsNoti(RenderElement re, bool fromMe, Rectangle totalBounds)
        {
            re.OnInvalidateGraphicsNoti(fromMe, ref totalBounds);
        }



        public void SuspendGraphicsUpdate()
        {
            _uiLayoutFlags |= RenderElementConst.LY_SUSPEND_GRAPHIC;
        }
        public void ResumeGraphicsUpdate()
        {
            _uiLayoutFlags &= ~RenderElementConst.LY_SUSPEND_GRAPHIC;
        }
        internal bool BlockGraphicUpdateBubble
        {
            get
            {
#if DEBUG
                return (_uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0;
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