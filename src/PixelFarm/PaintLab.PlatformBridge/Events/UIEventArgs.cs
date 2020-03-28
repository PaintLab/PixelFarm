//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{

    public class UIKeyEventArgs : UIEventArgs
    {
        uint _keyData;
        char _c;
        public UIKeyEventArgs()
        {
        }
        public uint KeyData
        {
            get => _keyData;
            set => _keyData = value;
        }
        public bool HasKeyData => true;
        public char KeyChar => _c;
        public void SetKeyChar(char c) => _c = c;
        //
        public override void Clear()
        {
            base.Clear();
            _c = '\0';
            _keyData = 0;
        }
        public bool IsControlCharacter => Char.IsControl(_c);
        public UIKeys KeyCode => (UIKeys)this.KeyData & UIKeys.KeyCode;
        public void SetEventInfo(uint keydata, bool shift, bool alt, bool control)
        {
            _keyData = keydata;
            this.Shift = shift;
            this.Alt = alt;
            this.Ctrl = control;
        }
        public void SetEventInfo(bool shift, bool alt, bool control)
        {
            this.Shift = shift;
            this.Alt = alt;
            this.Ctrl = control;
        }
    }

    public abstract class UIEventArgs : EventArgs
    {
        int _x;
        int _y;
        IUIEventListener _currContext;

        public UIEventArgs()
        {
        }
        public virtual void Clear()
        {
            _x = _y = 0;

            this.ExactHitObject = this.SourceHitElement = this.CurrentContextElement = null;
            this.Shift = this.Alt = this.Ctrl = this.CancelBubbling = false;
            MouseCursorStyle = MouseCursorStyle.Default;
            CustomMouseCursor = null;
        }
        public MouseCursorStyle MouseCursorStyle { get; set; }
        /// <summary>
        /// request for custom mouse cursor
        /// </summary>
        public Cursor CustomMouseCursor { get; set; }

        /// <summary>
        /// exact hit object (include run)
        /// </summary>
        public object ExactHitObject { get; set; }

        /// <summary>
        /// first hit IEventListener
        /// </summary>
        public IUIEventListener SourceHitElement { get; set; }
        //TODO: review here, ensure set this value 


        public IUIEventListener CurrentContextElement
        {
            get => _currContext;
            set => _currContext = value;
        }
        //TODO: review here, ensure set this value  
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public bool Ctrl { get; set; }
        public void SetLocation(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public int X => _x;
        public int Y => _y;

        public bool IsCanceled { get; private set; }
        public void StopPropagation()
        {
            this.IsCanceled = true;
        }
        public bool CancelBubbling
        {
            get => this.IsCanceled;
            set => this.IsCanceled = value;
        }

        //TODO: review this
        public UIEventName UIEventName { get; set; }
    }

    public enum FocusEventType
    {
        PreviewLostFocus,
        PreviewFocus,
        Focus,
        LostFocus
    }
    public class UIFocusEventArgs : UIEventArgs
    {

        public UIFocusEventArgs()
        {
            FocusEventType = FocusEventType.PreviewLostFocus;
        }
        public FocusEventType FocusEventType { get; set; }
        public object ToBeFocusElement { get; set; }

        public object ToBeLostFocusElement { get; set; }
        public override void Clear()
        {
            ToBeFocusElement = null;
            ToBeLostFocusElement = null;
            FocusEventType = FocusEventType.PreviewFocus;
            base.Clear();
        }
    }

    public class UIPaintEventArgs
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public class UIMousePressEventArgs : EventArgs
    {
        public UIMouseButtons Button { get; set; }
        public IUIEventListener CurrentContextElement { get; set; }
    }
    public class UIMouseHoverEventArgs : EventArgs
    {
        public IUIEventListener CurrentContextElement { get; set; }
    }

    public class UIMouseEventArgs : UIEventArgs
    {
        public UIMouseEventArgs()
        {
        }
        public UIMouseButtons Button { get; set; }
        public int Delta { get; private set; }
        public int Clicks { get; private set; }
        public int GlobalX { get; private set; }
        public int GlobalY { get; private set; }
        public int XDiff { get; private set; }
        public int YDiff { get; private set; }

        public void SetDiff(int xdiff, int ydiff)
        {
            this.XDiff = xdiff;
            this.YDiff = ydiff;
        }
        public void SetEventInfo(int x, int y, UIMouseButtons button, int clicks, int delta)
        {
            this.GlobalX = x;
            this.GlobalY = y;
            this.SetLocation(x, y);
            Button = button;
            Clicks = clicks;
            Delta = delta;
        }



        public override void Clear()
        {
            base.Clear();
            this.Button = UIMouseButtons.Left;
            this.Clicks =
                  this.XDiff =
                  this.YDiff =
                  this.GlobalX =
                  this.GlobalY =
                  this.CapturedMouseX =
                  this.CapturedMouseY = 0;
            this.MouseCursorStyle = UI.MouseCursorStyle.Default;
            this.IsDragging = false;
            this.DraggingElement = null;

            CurrentMousePressMonitor = null;
        }

        public bool IsDragging { get; set; }
        //-------------------------------------------------------------------
        public IUIEventListener DraggingElement { get; private set; }
        public void SetMouseCaptureElement(IUIEventListener listener)
        {
            this.DraggingElement = listener;
        }
        //-------------------------------------------------------------------

        public IUIEventListener CurrentMouseActive { get; set; }
        public IUIEventListener PreviousMouseDown { get; set; }
        public bool IsAlsoDoubleClick { get; set; }
        public int CapturedMouseX { get; set; }
        public int CapturedMouseY { get; set; }
        public int DiffCapturedX => this.X - this.CapturedMouseX;
        public int DiffCapturedY => this.Y - this.CapturedMouseY;
        public IUIEventListener CurrentMousePressMonitor { get; set; }
        public void StartMonitorMousePress(IUIEventListener listener)
        {
            CurrentMousePressMonitor = listener;
        }
    }

    /// <summary>
    /// primary mouse input
    /// </summary>
    public class PrimaryMouseEventArgs : EventArgs
    {
        public int Left { get; private set; }
        public int Top { get; private set; }
        public UIMouseButtons Button { get; private set; }
        public int Clicks { get; private set; }
        public int Delta { get; private set; }
        public PrimaryMouseEventArgs() { }
        public void SetMouseDownEventInfo(int x, int y, UIMouseButtons button, int clicks)
        {
            UIEventName = UIEventName.KeyDown;
            this.Left = x;
            this.Top = y;
            Button = button;
            Clicks = clicks;
            Delta = 0;
        }
        public void SetMouseMoveEventInfo(int x, int y)
        {
            UIEventName = UIEventName.MouseMove;
            this.Left = x;
            this.Top = y;
            Button = UIMouseButtons.None;
            Clicks = 0;
            Delta = 0;
        }
        public void SetMouseUpEventInfo(int x, int y, UIMouseButtons button)
        {
            UIEventName = UIEventName.MouseUp;
            this.Left = x;
            this.Top = y;
            Button = button;
            Clicks = 0;
            Delta = 0;
        }
        public void SetMouseWheelEventInfo(int x, int y, int delta)
        {
            UIEventName = UIEventName.Wheel;
            this.Left = x;
            this.Top = y;
            Button = UIMouseButtons.None;
            Clicks = 0;
            Delta = delta;

        }

        public UIEventName UIEventName { get; private set; }
    }




    public abstract class Cursor
    {
    }

    public class CursorRequest
    {
        public CursorRequest(string url, int width = 0)
        {
            Url = url;
            Width = 0;
        }
        public string Url { get; private set; }
        public int Width { get; private set; }
    }

    public enum MouseCursorStyle
    {
        Default,
        Arrow, //arrow (default)
        Hidden,//hidden cursor
        Pointer, //hand cursor
        IBeam,
        Move,
        EastWest,
        NorthSouth,
        CustomStyle,
    }
    public enum AffectedElementSideFlags
    {
        None = 0,
        Left = 1,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3
    }
    public class UIGuestMsgEventArgs : UIEventArgs
    {
        public UIGuestMsgEventArgs()
        {
        }

        public object Sender { get; set; }
        public IUIEventListener SenderAsIEventListener => this.Sender as IUIEventListener;
        public object UserMsgContent { get; set; }
        public int UserMsgFlags { get; set; }
    }

}