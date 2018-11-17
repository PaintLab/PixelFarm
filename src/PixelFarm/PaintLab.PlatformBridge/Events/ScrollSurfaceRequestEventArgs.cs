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
        UIScrollEventType _eventType;
        int _oldValue;
        int _newValue;
        UIScrollOrientation _orientation;
        public UIScrollEventArgs(UIScrollEventType eventType, int oldValue, int newValue, UIScrollOrientation orientation)
        {
            this._eventType = eventType;
            this._oldValue = oldValue;
            this._newValue = newValue;
            this._orientation = orientation;
        }
        public UIScrollEventArgs(UIScrollEventType eventType, int oldValue, int newValue)
        {
            this._eventType = eventType;
            this._oldValue = oldValue;
            this._newValue = newValue;
        }
        public int NewValue
        {
            get
            {
                return this._newValue;
            }
        }
        public int OldValue
        {
            get
            {
                return this._oldValue;
            }
        }
        public UIScrollEventType Type
        {
            get
            {
                return this._eventType;
            }
        }
    }

    public class ScrollSurfaceRequestEventArgs : EventArgs
    {
        bool need_it = false;
        public ScrollSurfaceRequestEventArgs(bool need)
        {
            need_it = need;
        }
        public bool Need
        {
            get
            {
                return need_it;
            }
        }
    }
}