//MIT, 2016-present, WinterDev
using System;
using System.Collections.Generic;
using Pencil.Gaming;

namespace PixelFarm.Forms
{
    public static class Application
    {
        public static void EnableVisualStyles() { }
        public static void SetCompatibleTextRenderingDefault(bool value) { }
        public static event EventHandler Idle;
        public static void Run(Form form) { }
        public static void Run(ApplicationContext appContext) { }
    }

    public class Timer
    {
        public void Dispose() { }
        public bool Enabled { get; set; }
        public int Interval { get; set; }
        public event EventHandler Tick;
    }
    public class FormClosedEventArgs : EventArgs { }
    public class PreviewKeyEventArgs : EventArgs { }
    public class ApplicationContext
    {
        Form _mainForm;
        public ApplicationContext() { }
        public ApplicationContext(Form mainForm)
        {
            this._mainForm = mainForm;
        }

    }
    public class ControlCollection
    {
        Control _owner;
        List<Control> _children = new List<Control>();
        internal ControlCollection(Control owner)
        {
            _owner = owner;
        }
        public void Add(Control c)
        {
            if (_owner == c)
            {
                throw new NotSupportedException();
            }
            //
            _children.Add(c);

        }
        public bool Remove(Control c)
        {
            return _children.Remove(c);
        }
        public void Clear()
        {
            _children.Clear();
        }


        public int Count => _children.Count;
        public Control GetControl(int index) => _children[index];
    }
    public class Form : Control
    {
        public Form()
        {
            CreateNativeWindowHandle();
        }
        public void Hide()
        {
        }
        void CreateNativeWindowHandle()
        {

        }
        public void Invoke(Delegate ac)
        {
        }
        public virtual void Close()
        {
        }
        public event EventHandler<FormClosingEventArgs> FormClosing;
        public event EventHandler<FormClosedEventArgs> FormClosed;
    }





    public class Control : IDisposable
    {
        int _width;
        int _height;
        int _left;
        int _top;

        IntPtr _nativeHandle;
        ControlCollection _controls;


        public Control()
        {

        }
        public void SetBounds(int left, int top, int width, int height)
        {
            _left = left;
            _top = top;
            _width = width;
            _height = height;
        }
        internal static void SetNativeHandle(Control c, IntPtr nativeHandle)
        {
            c._nativeHandle = nativeHandle;
            c.OnHandleCreated(EventArgs.Empty);
        }

        protected bool DesignMode { get; set; }
        protected virtual void OnHandleCreated(EventArgs e)
        {
        }
        public virtual void Show()
        {
        }
        protected virtual void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
        }
        protected virtual void OnSizeChanged(EventArgs e)
        {
        }


        public ControlCollection Controls => (_controls ?? (_controls = new ControlCollection(this)));



        internal Control FindHitControl(int x, int y, out int hitOnX, out int hitOnY)
        {
            hitOnX = x;
            hitOnY = y;
            if (x >= _left && x < (_left + _width))
            {
                if (y >= _top && y < (_top + _height))
                {
                    //(x,y) are in this control 
                    if (_controls != null)
                    {
                        for (int i = _controls.Count - 1; i >= 0; --i)
                        {
                            Control child = _controls.GetControl(i);
                            int newX = x - child._left;
                            int newY = y - child._top;
                            Control hitOnSubChild = child.FindHitControl(newX, newY, out hitOnX, out hitOnY);
                            if (hitOnSubChild != null)
                            {
                                return hitOnSubChild;
                            }
                        }
                    }

                    return this;
                }
            }
            return null;
        }

        public void Focus()
        {
            //TODO: implement this
        }
        public virtual int Width
        {
            get => _width;
            set
            {
                _width = value;
                //TODO: implement this
            }
        }

        public virtual int Height
        {
            get => _height;
            set
            {
                _height = value;
                //TODO: implement this
            }
        }
        public int Left => _left;
        public int Top => _top;

        public bool IsHandleCreated { get; set; }
        public virtual IntPtr Handle => _nativeHandle;

        public void Dispose() { }
        public void SetSize(int w, int h)
        {
            _width = w;
            _height = h;

        }
        public bool Visible { get; set; }
        public virtual string Text { get; set; }

        public virtual Control TopLevelControl
        {
            get;
            set;
        }
        public Control Parent { get; set; }

        protected virtual void OnLoad(EventArgs e)
        {
        }

        public static Control CreateFromNativeWindowHwnd(IntPtr hwnd)
        {
            //preserve this
            Control newControl = new Control();
            Control.SetNativeHandle(newControl, hwnd);
            return newControl;
        }
        public static Form CreateFromNativeWindowHwnd2(IntPtr hwnd)
        {
            //preserve this
            Form newControl = new Form();
            Control.SetNativeHandle(newControl, hwnd);
            return newControl;
        }
        protected virtual void OnPaint(PaintEventArgs e)
        {
        }
        protected virtual void OnMouseDown(MouseButton btn, int x, int y)
        {

        }

        protected virtual void OnMouseMove(double x, double y)
        {

        }
        protected virtual void OnMouseUp(MouseButton btn, double x, double y)
        {

        }
        internal static void InvokeMouseButton(Control control,
            MouseButton btn,
            KeyActionKind action,
            int x, int y)
        {
            
            //TODO: implement detail methods  
            Control subControl = control.FindHitControl(x, y, out int hitOnX, out int hitOnY);
            if (subControl == null) return;//?

            switch (action)
            {
                default: throw new NotSupportedException();
                case KeyActionKind.Press:
                    subControl.OnMouseDown(btn, hitOnX, hitOnY);
                    break;
                case KeyActionKind.Release:
                    subControl.OnMouseUp(btn, hitOnX, hitOnY);
                    break;
                case KeyActionKind.Repeat:
                    //????
                    break;
            }
        }
        internal static void InvokeCursorPos(Control control, double x, double y)
        {
            control.OnMouseMove(x, y);
        }
        internal static void InvokeKeyPress(Control control, char c)
        {
            //TODO: implement detail methods
            control.OnKeyPress(c);
        }
        internal static void InvokeKey(Control control, Key key, int scanCode, KeyActionKind keyAction, KeyModifiers mods)
        {
            switch (keyAction)
            {
                default: throw new NotSupportedException();
                case KeyActionKind.Press:
                    control.OnKeyDown(key, scanCode, mods);
                    break;
                case KeyActionKind.Repeat:
                    control.OnKeyRepeat(key, scanCode, mods);
                    break;
                case KeyActionKind.Release:
                    control.OnKeyUp(key, scanCode, mods);
                    break;
            }
        }
        protected virtual void OnKeyUp(Key key, int scanCode, KeyModifiers mods)
        {
        }
        protected virtual void OnKeyDown(Key key, int scanCode, KeyModifiers mods)
        {
        }
        protected virtual void OnKeyRepeat(Key key, int scanCode, KeyModifiers mods)
        {
        }
        protected virtual void OnKeyPress(char c)
        {
        }
        public virtual void OnCursorLeave()
        {

        }
        protected virtual void OnIconify(bool iconify)
        {

        }
        protected virtual void OnFocus()
        {
        }
        protected virtual void OnLostFocus()
        {
        }
        protected virtual void OnClosing(ref bool cancel)
        {

        }
        protected virtual void OnCursorEnter()
        {

        }
        internal static void SetFocusState(Control f, bool focus)
        {
            if (focus)
            {
                f.OnFocus();
            }
            else
            {
                f.OnLostFocus();
            }
        }



        internal static void InvokeOnScroll(Control control, double xoffset, double yoffset)
        {
            //TODO: implement detail methods
        }
        internal static void SetIconifyState(Control control, bool iconify)
        {
            control.OnIconify(iconify);
        }
        internal static void InvokeOnWindowMove(Control control, int x, int y)
        {
            //window moved
            //on pos changed
            //TODO: implement detail methods
        }

        internal static void InvokeOnSizeChanged(Control control, int w, int h)
        {
            //on pos changed
            //TODO: implement detail methods
            control.OnSizeChanged(EventArgs.Empty);
        }
        internal static void InvokeOnRefresh(Control control)
        {
            //TODO: implement detail methods
        }
        internal static void InvokeOnClosing(Control control, ref bool cancel)
        {
            control.OnClosing(ref cancel);
        }
        internal static void SetCursorEnterState(Control control, bool enter)
        {
            if (enter)
            {
                control.OnCursorEnter();
            }
            else
            {
                control.OnCursorLeave();
            }
        }


    }
    public class FormClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }

    public class PreviewKeyDownEventArgs : EventArgs { }
}