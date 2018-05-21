//Apache2, 2014-2018, WinterDev

namespace LayoutFarm.UI
{
    public struct ToNotifySizeChangedEvent
    {
        public int xdiff;
        public int ydiff;
        public IEventListener ui;
        public AffectedElementSideFlags affectedSideFlags;
    }
}