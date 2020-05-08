//Apache2, 2014-present, WinterDev

namespace LayoutFarm
{
    static class RenderElementConst
    {
        public const int HIDDEN = 1 << (1 - 1);
        public const int TRACKING_BG_IS_NOT_OPAQUE = 1 << (2 - 1); //background is not opaque
        public const int TRACKING_GFX = 1 << (3 - 1);
        public const int TRACKING_GFX_TIP = 1 << (4 - 1);
        public const int TRACKING_GFX_In_UPDATE_RGN_QUEUE = 1 << (5 - 1);

        /// <summary>
        /// transparent for mouse down,move (include drag),up except scroll up-down
        /// </summary>
        public const int TRANSPARENT_FOR_MOUSE_INPUT = 1 << (6 - 1);
        /// <summary>
        /// transparent for keyboard input (keydown, keypress, keyup)
        /// </summary>
        public const int TRANSPARENT_FOR_KEYBOARD_INPUT = 1 << (7 - 1);
        /// <summary>
        /// transparent for scroll input
        /// </summary>
        public const int TRANSPARENT_FOR_SCORLL_INPUT = 1 << (8 - 1);

        public const int HAS_DOUBLE_SCROLL_SURFACE = 1 << (14 - 1); //TODO: review here

        public const int MAY_HAS_CHILD = 1 << (16 - 1);
        public const int MAY_HAS_VIEWPORT = 1 << (17 - 1);
        public const int NEED_PRE_RENDER_EVAL = 1 << (18 - 1);
        public const int IS_TOP_RENDERBOX = 1 << (19 - 1);

        public const int LY_SUSPEND_GRAPHIC = 1 << (20 - 1);
        public const int LY_REQ_INVALIDATE_RECT_EVENT = 1 << (21 - 1);

#if DEBUG
        public const int dbugFIRST_ARR_PASS = 1 << (24 - 1);//TODO: review this again,
#endif

    }
}
