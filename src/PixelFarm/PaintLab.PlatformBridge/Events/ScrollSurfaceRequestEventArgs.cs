//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    public enum UIScrollEventType
    {
        SmallDecrement = 0,
        //
        SmallIncrement = 1,
        //
        LargeDecrement = 2,
        //
        LargeIncrement = 3,
        //
        ThumbPosition = 4,
        //
        ThumbTrack = 5,
        //
        First = 6,
        //
        Last = 7,
        //
        EndScroll = 8,
    }

    public enum UIScrollOrientation
    {
        HorizontalScroll = 0,
        //
        VerticalScroll = 1,
    }

    public class UIScrollEventArgs : EventArgs
    {

        UIScrollOrientation _orientation;
        public UIScrollEventArgs(UIScrollEventType eventType, int oldValue, int newValue, UIScrollOrientation orientation)
        {
            EventType = eventType;
            OldValue = oldValue;
            NewValue = newValue;
            _orientation = orientation;
        }
        public UIScrollEventArgs(UIScrollEventType eventType, int oldValue, int newValue)
        {
            EventType = eventType;
            OldValue = oldValue;
            NewValue = newValue;
        }
        //
        public int NewValue { get; private set; }
        //
        public int OldValue { get; private set; }
        public UIScrollEventType EventType { get; private set; }
    }

    public class ScrollSurfaceRequestEventArgs : EventArgs
    {

        public ScrollSurfaceRequestEventArgs(bool need)
        {
            Need = need;
        }
        public bool Need { get; private set; }
    }
}