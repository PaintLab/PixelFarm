//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    partial class RenderElement
    {
        public bool InvalidateGraphics()
        {
            //RELATIVE to this ***
            propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            if ((uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return false;
            }


            Rectangle rect = new Rectangle(0, 0, b_width, b_height);
            RootInvalidateGraphicArea(this, ref rect);
            return true;//TODO: review this 
        }
        public void InvalidateParentGraphics()
        {
            //RELATIVE to its parent
            this.InvalidateParentGraphics(this.RectBounds);
        }
        public void InvalidateParentGraphics(Rectangle totalBounds)
        {
            //RELATIVE to its parent***

            propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            RenderElement parent = this.ParentRenderElement; //start at parent ****
            //--------------------------------------- 
            if (parent != null)
            {
                this.rootGfx.InvalidateGraphicArea(parent, ref totalBounds, true);//RELATIVE to its parent***
            }
        }

        static void RootInvalidateGraphicArea(RenderElement re, ref Rectangle rect)
        {
            //RELATIVE to re ***
            //1.
            re.propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            //2.  
            re.rootGfx.InvalidateGraphicArea(re, ref rect);
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

        public static bool IsInTopDownReArrangePhase
        {
            //TODO: review this again !
            get
            {
                return true;
            }
            set
            {
            }
        }


        internal bool BlockGraphicUpdateBubble
        {
            get
            {
#if DEBUG
                return (uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0;
#else
                return (uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0;
#endif
            }
        }
    }
}