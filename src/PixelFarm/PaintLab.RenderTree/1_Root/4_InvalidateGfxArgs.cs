//Apache2, 2020-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    public enum InvalidateReason
    {
        Empty,
        ViewportChanged,
        UpdateLocalArea,
    }

    public class InvalidateGfxArgs
    {
        internal InvalidateGfxArgs() { }
        public InvalidateReason Reason { get; private set; }
        public bool PassSrcElement { get; private set; }
        public int LeftDiff { get; private set; }
        public int TopDiff { get; private set; }
        internal Rectangle Rect;
        internal Rectangle GlobalRect;

        internal RenderElement StartOn { get; set; }
        public RenderElement SrcRenderElement { get; private set; }
        public void Reset()
        {
            LeftDiff = TopDiff = 0;
            GlobalRect = Rect = Rectangle.Empty;
            SrcRenderElement = null;
            Reason = InvalidateReason.Empty;
            PassSrcElement = false;
            StartOn = null;
        }

        /// <summary>
        /// set info about this invalidate args
        /// </summary>
        /// <param name="srcElem"></param>
        /// <param name="leftDiff"></param>
        /// <param name="topDiff"></param>
        public void SetReason_ChangeViewport(RenderElement srcElem, int leftDiff, int topDiff)
        {
            SrcRenderElement = srcElem;
            Rect = new Rectangle(0, 0, srcElem.Width, srcElem.Height);
            LeftDiff = leftDiff;
            TopDiff = topDiff;
            Reason = InvalidateReason.ViewportChanged;
        }
        public void SetReason_UpdateLocalArea(RenderElement srcElem, Rectangle localBounds)
        {
            SrcRenderElement = srcElem;
            Rect = localBounds;
            Reason = InvalidateReason.UpdateLocalArea;
        }

        internal bool FromMainPool { get; set; }
#if DEBUG
        public bool dbugWaitingInPool { get; set; }
        public override string ToString() => Reason.ToString() + " " + SrcRenderElement.dbug_obj_id.ToString();
#endif
    }


}