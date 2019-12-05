//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    public enum InvalidateReason
    {

    }
    public class InvalidateGraphicsArgs
    {
        public int LeftDiff;
        public int TopDiff;
        public Rectangle Rect;
    }
    partial class RenderElement
    {
        public void InvalidateGraphics(InvalidateGraphicsArgs args)
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
                Rectangle rect = new Rectangle(0, 0, _b_width, _b_height);
                args.Rect = rect;
                RootInvalidateGraphicArea(this, args);
            }
            else
            {

            }
        }
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
                Rectangle rect = new Rectangle(0, 0, _b_width, _b_height);
                RootInvalidateGraphicArea(this, ref rect);
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
                    _rootGfx.InvalidateGraphicArea(parent, ref totalBounds, true);//RELATIVE to its parent***
                }
                else
                {

                }
            }
        }
        internal static bool RequestInvalidateGraphicsNoti(RenderElement re)
        {
            return (re._uiLayoutFlags & RenderElementConst.LY_REQ_INVALIDATE_RECT_EVENT) != 0;
        }
        internal static void InvokeInvalidateGraphicsNoti(RenderElement re, bool fromMe, Rectangle totalBounds)
        {
            re.OnInvalidateGraphicsNoti(fromMe, ref totalBounds);
        }
        static void RootInvalidateGraphicArea(RenderElement re, ref Rectangle rect)
        {
            //RELATIVE to re ***
            //1.
            re._propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            //2.  
            re._rootGfx.InvalidateGraphicArea(re, ref rect);
        }
        static void RootInvalidateGraphicArea(RenderElement re, InvalidateGraphicsArgs args)
        {
            //RELATIVE to re ***
            //1.
            re._propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            //2.  
           
            re._rootGfx.InvalidateGraphicArea(re, args);
        }
        public static void InvalidateGraphicLocalArea(RenderElement re, Rectangle localArea)
        {
            //RELATIVE to re ***

            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }
            RootInvalidateGraphicArea(re, ref localArea);
        }

        //TODO: review this again
        protected bool ForceReArrange
        {
            get { return true; }
            set { }
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
    }
}