//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{

    public class UIKeyEventArgs : UIEventArgs
    {
        public UIKeyEventArgs()
        {
        }
        public bool HasKeyData => true;
        public uint KeyData { get; internal set; }
        public char KeyChar { get; internal set; }
        public void SetKeyChar(char c) => KeyChar = c;
        //
        protected override void OnClearData()
        {
            base.OnClearData();
            KeyChar = '\0';
            KeyData = 0;
        }
        public bool IsControlCharacter => Char.IsControl(KeyChar);
        public UIKeys KeyCode => (UIKeys)this.KeyData & UIKeys.KeyCode;


        internal UIEventName _eventName;
        public override UIEventName UIEventName => _eventName;
    }

    namespace ForImplementator
    {
        public static partial class UIEventArgsExtensions
        {
            public static void SetEventInfo(this UIKeyEventArgs e, UIEventName eventName)
            {
                e._eventName = eventName;
            }
            public static void SetEventInfo(this UIFocusEventArgs e, UIEventName eventName)
            {
                e._evName = eventName;
            }
            public static void SetEventInfo(this UIKeyEventArgs e, uint keydata, bool shift, bool alt, bool control, UIEventName eventName)
            {
                e.KeyData = keydata;
                e.Shift = shift;
                e.Alt = alt;
                e.Ctrl = control;
                e._eventName = eventName;
            }
            public static void SetEventInfo(this UIKeyEventArgs e, bool shift, bool alt, bool control, UIEventName eventName)
            {
                e.Shift = shift;
                e.Alt = alt;
                e.Ctrl = control;
                e._eventName = eventName;
            }
            public static void ResetAll(this UIEventArgs e) => UIEventArgs.Clear(e);

            public static void SetExactHitObject(this UIEventArgs e, object obj) => e.ExactHitObject = obj;
            public static void SetSourceHitObject(this UIEventArgs e, IUIEventListener obj) => e.SourceHitElement = obj;
            public static void SetCurrentContextElement(this UIEventArgs e, IUIEventListener obj) => e.CurrentContextElement = obj;
        }
    }



    public abstract class UIEventArgs : EventArgs
    {
        public UIEventArgs()
        {
        }
        protected virtual void OnClearData()
        {
            X = Y = 0;
            this.ExactHitObject = this.SourceHitElement = this.CurrentContextElement = null;
            this.Shift = this.Alt = this.Ctrl = this.CancelBubbling = false;
            MouseCursorStyle = MouseCursorStyle.Default;
            CustomMouseCursor = null;
        }
        internal static void Clear(UIEventArgs e)
        {
            e.OnClearData();
        }


        /// <summary>
        /// request for custom mouse cursor
        /// </summary>
        public MouseCursorStyle MouseCursorStyle { get; set; }
        /// <summary>
        /// request for custom mouse cursor
        /// </summary>
        public Cursor CustomMouseCursor { get; set; }

        /// <summary>
        /// exact hit object (include run)
        /// </summary>
        public object ExactHitObject { get; internal set; }

        /// <summary>
        /// first hit IEventListener
        /// </summary>
        public IUIEventListener SourceHitElement { get; internal set; }
        //TODO: review here, ensure set this value 


        public IUIEventListener CurrentContextElement { get; internal set; }
        //TODO: review here, ensure set this value  
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public bool Ctrl { get; set; }


        public int X { get; private set; }
        public int Y { get; private set; }

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

        public abstract UIEventName UIEventName { get; }
        public void SetLocation(int x, int y)
        {
            X = x;
            Y = y;
        }
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
        protected override void OnClearData()
        {
            ToBeFocusElement = null;
            ToBeLostFocusElement = null;
            FocusEventType = FocusEventType.PreviewFocus;
            base.OnClearData();
        }
        internal UIEventName _evName;
        public override UIEventName UIEventName => _evName;
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

    public class UIMouseLostFocusEventArgs : UIEventArgs
    {
        public UIMouseLostFocusEventArgs() { }
        public override UIEventName UIEventName => UIEventName.MouseLostFocus;
    }


    public class UIMouseLeaveEventArgs : UIEventArgs
    {
        public UIMouseLeaveEventArgs() { }
        public bool IsDragging { get; set; }
        public int XDiff { get; private set; }
        public int YDiff { get; private set; }

        public static void SetDiff(UIMouseLeaveEventArgs e, int xdiff, int ydiff)
        {
            e.XDiff = xdiff;
            e.YDiff = ydiff;
        }

        public override UIEventName UIEventName => UIEventName.MouseMove;
    }

    public class UIMouseDownEventArgs : UIMouseEventArgs
    {
        public UIMouseDownEventArgs() { }
        public int Clicks => _click;
        public UIMouseButtons Buttons => _buttons;
        public override UIEventName UIEventName => UIEventName.MouseDown;
    }
    public class UIMouseMoveEventArgs : UIMouseEventArgs
    {
        public UIMouseMoveEventArgs() { }
        public UIMouseButtons Buttons
        {
            get => _buttons;
            set => _buttons = value;
        }
        public bool IsDragging { get; set; }
        protected override void OnClearData()
        {
            base.OnClearData();
            IsDragging = false;
        }
        public override UIEventName UIEventName => UIEventName.MouseMove;
    }
    public class UIMouseUpEventArgs : UIMouseEventArgs
    {
        public UIMouseUpEventArgs() { }
        public bool IsAlsoDoubleClick { get; set; }
        public bool IsDragging { get; set; }
        public UIMouseButtons Buttons => _buttons;
        public override UIEventName UIEventName => UIEventName.MouseUp;
        protected override void OnClearData()
        {
            base.OnClearData();
            IsAlsoDoubleClick = false;
            IsDragging = false;
        }
    }
    public class UIMouseWheelEventArgs : UIMouseEventArgs
    {
        public UIMouseWheelEventArgs() { }
        public int Delta => _delta;
        public override UIEventName UIEventName => UIEventName.Wheel;
    }
    public abstract class UIMouseEventArgs : UIEventArgs
    {
        public UIMouseEventArgs()
        {
        }
        /// <summary>
        /// root-level left 
        /// </summary>
        public int GlobalX { get; private set; }
        /// <summary>
        /// root-level top
        /// </summary>
        public int GlobalY { get; private set; }
        /// <summary>
        /// x diff from previouse mouse pos
        /// </summary>
        public int XDiff { get; private set; }
        /// <summary>
        /// y diff from previous mouse pos
        /// </summary>
        public int YDiff { get; private set; }

        public static void SetDiff(UIMouseEventArgs e, int xdiff, int ydiff)
        {
            e.XDiff = xdiff;
            e.YDiff = ydiff;
        }
        public static void SetEventInfo(UIMouseEventArgs e, int x, int y, UIMouseButtons button, int clicks, int delta)
        {
            e.GlobalX = x;
            e.GlobalY = y;
            e.SetLocation(x, y);
            e._buttons = button;
            e._click = clicks;
            e._delta = delta;
        }

        internal int _delta;
        internal int _click;
        internal UIMouseButtons _buttons;

        protected override void OnClearData()
        {
            base.OnClearData();
            _buttons = UIMouseButtons.Left;
            _click =
                  this.XDiff =
                  this.YDiff =
                  this.GlobalX =
                  this.GlobalY =
                  this.CapturedMouseX =
                  this.CapturedMouseY = 0;

            this.MouseCursorStyle = UI.MouseCursorStyle.Default;
            CustomMouseCursor = null;

            this.CapturedElement = null;
        }


        //-----
        //TODO: review here
        public IUIEventListener CapturedElement { get; private set; }
        public void SetMouseCapturedElement(IUIEventListener listener)
        {
            this.CapturedElement = listener;
            CapturedMouseX = X;
            CapturedMouseY = Y;
        }

        public int CapturedMouseX { get; set; }
        public int CapturedMouseY { get; set; }
        public int DiffCapturedX => this.X - this.CapturedMouseX;
        public int DiffCapturedY => this.Y - this.CapturedMouseY;

    }

    /// <summary>
    /// primary mouse input from the platform
    /// </summary>
    public class PrimaryMouseEventArgs : EventArgs
    {
        //accept input from external platform
        //then we translate this primary-mouse-event-args
        //to various UIMouseEventArgs


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
        public override UIEventName UIEventName => UIEventName.Unknown;
    }

}