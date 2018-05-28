//Apache2, 2014-2018, WinterDev

namespace LayoutFarm.UI
{
    public interface IBoxElement
    {
        void ChangeElementSize(int w, int h);
        int MinHeight { get; }
    }
    public interface IUIElement
    {
        bool AttachEventListener(IUIEventListener eventListener); 
    }
    public interface IUIRootElement
    {
        IUIEventListener CreateEventListener();
        IUIElement CreateElement(string elemName);
        void AddContent(IUIElement uiElement); 
    }
    /// <summary>
    /// can listen to some event
    /// </summary>
    public interface IUIEventListener
    {
        event UIEventHandler<IEventArgs> MouseDown;
        event UIEventHandler<IEventArgs> MouseUp;
        event UIEventHandler<IEventArgs> MouseMove;
        //
        event UIEventHandler<IEventArgs> KeyDown;
        event UIEventHandler<IEventArgs> KeyPress;
        event UIEventHandler<IEventArgs> KeyUp;
    }
    public interface IEventArgs
    {
        IEventName EventName { get; }
        int X { get; }
        int Y { get; }
    }
    public enum IEventName
    {
        Custom,

        MouseDown,
        MouseMove,
        MouseUp,
        //Focus
        //
        KeyDown,
        KeyPress,
        KeyUp

    }

    public delegate void UIEventHandler<T>(T e)
        where T : IEventArgs;

}