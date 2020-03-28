//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{
    public enum UIEventName : byte
    {
        Unknown,
        Click,
        DblClick,
        MouseDown,
        MouseMove,
        MouseUp,
        MouseHover,

        KeyDown,
        KeyUp,
        KeyPress,
        ProcessDialogKey,
        //
        MouseLostFocus,
        Wheel,
    }
    public enum UIKeyEventName : byte
    {
        KeyDown,
        KeyUp,
        KeyPress,
        ProcessDialogKey
    }
    public enum UIMouseEventName : byte
    {
        Click,
        DoubleClick,
        MouseDown,
        MouseMove,
        MouseUp,
        MouseEnter,
        MouseLeave,
        MouseHover,
        MouseWheel
    }
    public enum UIDragEventName : byte
    {
        DragStart,
        DragStop,
        Dragging
    }
    public enum UIFocusEventName : byte
    {
        Focus,
        LossingFocus
    }
}