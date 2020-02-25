//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    public enum InvalidateReason
    {
        Empty,
        ViewportChanged,
        UpdateLocalArea,
        InvalidateParentArea,
    }

    public class InvalidateGraphicsArgs
    {
        internal InvalidateGraphicsArgs() { }
        public InvalidateReason Reason { get; private set; }
        public bool PassSrcElement { get; private set; }
        public int LeftDiff { get; private set; }
        public int TopDiff { get; private set; }
        internal Rectangle Rect;
        internal Rectangle GlobalRect;
        public RenderElement SrcRenderElement { get; private set; }
        public void Reset()
        {
            LeftDiff = TopDiff = 0;
            GlobalRect = Rect = Rectangle.Empty;
            SrcRenderElement = null;
            Reason = InvalidateReason.Empty;
            PassSrcElement = false;
        }
        /// <summary>
        /// set info about this invalidate args
        /// </summary>
        /// <param name="srcElem"></param>
        /// <param name="leftDiff"></param>
        /// <param name="topDiff"></param>
        public void Reason_ChangeViewport(RenderElement srcElem, int leftDiff, int topDiff)
        {
            SrcRenderElement = srcElem;
            LeftDiff = leftDiff;
            TopDiff = topDiff;
            Reason = InvalidateReason.ViewportChanged;
        }
        public void Reason_UpdateLocalArea(RenderElement srcElem, Rectangle localBounds)
        {
            SrcRenderElement = srcElem;
            Rect = localBounds;
            Reason = InvalidateReason.UpdateLocalArea;
        }
        public void Reason_InvalidateParent(RenderElement srcElem, Rectangle localBounds)
        {
            SrcRenderElement = srcElem;
            Rect = localBounds;
            PassSrcElement = true;
            Reason = InvalidateReason.InvalidateParentArea;
        }
    }

    partial class RenderElement
    {
        internal void InvalidateGraphics(InvalidateGraphicsArgs args)
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

                //RELATIVE to this***
                //1.
                _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
                //2.  
                _rootGfx.BubbleUpInvalidateGraphicArea(args);
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
                InvalidateGraphicLocalArea(this, rect);
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
                    InvalidateGraphicsArgs arg = _rootGfx.GetInvalidateGfxArgs();
                    arg.Reason_InvalidateParent(parent, totalBounds);

                    _rootGfx.BubbleUpInvalidateGraphicArea(arg);//RELATIVE to its parent***
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

        public static void InvalidateGraphicLocalArea(RenderElement re, Rectangle localArea)
        {
            //RELATIVE to re ***

            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }

            re._propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            InvalidateGraphicsArgs inv = re._rootGfx.GetInvalidateGfxArgs();
            inv.Reason_UpdateLocalArea(re, localArea);
            re._rootGfx.BubbleUpInvalidateGraphicArea(inv);
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