//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
{
    public interface IParentLink
    {
        RenderElement ParentRenderElement { get; }
        void AdjustLocation(ref int px,ref int py);
        RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point);
#if DEBUG
        string dbugGetLinkInfo();
#endif

    }
}