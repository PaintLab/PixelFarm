//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    public delegate void UIMouseEventHandler(object sender, UIMouseEventArgs e);
    public delegate void UIKeyEventHandler(object sender, UIKeyEventArgs e);
    public delegate void UIKeyPressEventHandler(object sender, UIKeyEventArgs e);
    public class UIKeyEventArgs : UIEventArgs
    {
        int _keyData;
        char _c;
        public UIKeyEventArgs()
        {
        }
        public int KeyData
        {
            get => _keyData;
            set => _keyData = value;
        }
        public bool HasKeyData => true;
        public char KeyChar => _c;
        public void SetKeyChar(char c) => _c = c;
        //
        public bool IsControlCharacter => Char.IsControl(_c);
        public UIKeys KeyCode => (UIKeys)this.KeyData & UIKeys.KeyCode;
        public void SetEventInfo(int keydata, bool shift, bool alt, bool control)
        {
            _keyData = keydata;
            this.Shift = shift;
            this.Alt = alt;
            this.Ctrl = control;
        }
    }
    public abstract class UIEventArgs : EventArgs
    {
        int _x;
        int _y;
        public UIEventArgs()
        {
        }
        public virtual void Clear()
        {
            _x = _y = 0;
            this.ExactHitObject = this.SourceHitElement = this.CurrentContextElement = null;
            this.Shift = this.Alt = this.Ctrl = this.CancelBubbling = false;
        }
        /// <summary>
        /// exact hit object (include run)
        /// </summary>
        public object ExactHitObject { get; set; }

        /// <summary>
        /// first hit IEventListener
        /// </summary>
        public IUIEventListener SourceHitElement { get; set; }
        //TODO: review here, ensure set this value 

        IUIEventListener _currContext;
        public IUIEventListener CurrentContextElement
        {
            get => _currContext;
            set
            {
                _currContext = value;
            }
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
        public UIEventName UIEventName { get; set; }
    }


    public enum UIMouseButtons
    {
        Left,
        Right,
        Middle,
        None
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



    public class UIMouseEventArgs : UIEventArgs
    {
        public UIMouseEventArgs()
        {
        }
        public UIMouseButtons Button { get; private set; }
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

        public bool IsFirstMouseEnter { get; set; }

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
            this.IsFirstMouseEnter = false;
        }
        //
        public MouseCursorStyle MouseCursorStyle { get; set; }
        //
        public bool IsDragging { get; set; }
        //
        //-------------------------------------------------------------------
        public IUIEventListener DraggingElement { get; private set; }
        public void SetMouseCapture(IUIEventListener listener)
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
    public class UIGuestTalkEventArgs : UIEventArgs
    {
        public UIGuestTalkEventArgs()
        {
        }
        public object Sender { get; set; }
        public IUIEventListener SenderAsIEventListener => this.Sender as IUIEventListener;
        public object UserMsgContent { get; set; }
        public int UserMsgFlags { get; set; }
    }

}