//Apache2, 2014-2018, WinterDev

using System.Collections.Generic;
namespace LayoutFarm.UI
{
    static class UISystem
    {
        static Queue<UIElement> s_layoutQueue = new Queue<UIElement>();
        static UISystem()
        {
            LayoutFarm.EventQueueSystem.CentralEventQueue.RegisterEventQueue(ClearLayoutQueue);
        }
        internal static void AddToLayoutQueue(UIElement ui)
        {
            if (ui.IsInLayoutQueue) return;
            s_layoutQueue.Enqueue(ui);
            ui.IsInLayoutQueue = true;
        }
        public static void ClearLayoutQueue()
        {
            int count = s_layoutQueue.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                UIElement ui = s_layoutQueue.Dequeue();
                ui.IsInLayoutQueue = false;
                UIElement.InvokeContentLayout(ui);

            }
        }
    }

    public abstract partial class UIElement : IUIEventListener
    {

#if DEBUG
        public bool dbugBreakMe;
#endif
        bool _hide;

        //bounds
        float _left;
        float _top;
        float _right;
        float _bottom;


        //~

        public UIElement()
        {
        }
        public abstract RenderElement GetPrimaryRenderElement(RootGraphic rootgfx);
        public abstract RenderElement CurrentPrimaryRenderElement
        {
            get;
        }
        protected abstract bool HasReadyRenderElement
        {
            get;
        }
        public abstract void InvalidateGraphics();


        System.WeakReference _tag;
        /// <summary>
        /// general purpose element
        /// </summary>
        public object Tag
        {
            get { return (_tag != null && _tag.IsAlive) ? _tag.Target : null; }
            set
            {
                _tag = (value != null) ? new System.WeakReference(value) : null;
            }
        }
        //----------------------------------- 

        public virtual void Focus()
        {
            //make this keyboard focusable
            if (this.HasReadyRenderElement)
            {
                //focus
                this.CurrentPrimaryRenderElement.Root.SetCurrentKeyboardFocus(this.CurrentPrimaryRenderElement);
            }
        }
        public virtual void Blur()
        {
            if (this.HasReadyRenderElement)
            {
                //focus
                this.CurrentPrimaryRenderElement.Root.SetCurrentKeyboardFocus(null);
            }
        }


        System.WeakReference _parent;
        public UIElement ParentUI
        {
            get { return (_parent != null && _parent.IsAlive) ? (UIElement)_parent.Target : null; }
            set
            {
                _parent = (value != null) ? new System.WeakReference(value) : null;
            }
        }
        public virtual bool Visible
        {
            get { return !this._hide; }
            set
            {
                this._hide = !value;
                if (this.HasReadyRenderElement)
                {
                    this.CurrentPrimaryRenderElement.SetVisible(value);
                }
            }
        }
        public PixelFarm.Drawing.Point GetGlobalLocation()
        {
            if (this.CurrentPrimaryRenderElement != null)
            {
                return this.CurrentPrimaryRenderElement.GetGlobalLocation();
            }
            return new PixelFarm.Drawing.Point((int)_left, (int)_top);
        }
        public void GetElementBounds(
           out float left,
           out float top,
           out float right,
           out float bottom)
        {
            left = _left;
            top = _top;
            right = _right;
            bottom = _bottom;
        }
        protected void SetElementBoundsWH(float width, float height)
        {
            _right = _left + width;
            _bottom = _top + height;
        }
        protected void SetElementBoundsLTWH(float left, float top, float width, float height)
        {
            //change 'TransparentBounds' => not effect visual presentation
            _left = left;
            _top = top;
            _right = left + width;
            _bottom = top + height;
        }
        protected void SetElementBounds(float left, float top, float right, float bottom)
        {   //change 'TransparentBounds' => not effect visual presentation
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }
        protected void SetElementBoundsLT(float left, float top)
        {

            _bottom = top + (_bottom - _top);
            _right = left + (_right - _left);
            _left = left;
            _top = top;
        }
        protected float BoundWidth { get { return _right - _left; } }
        protected float BoundHeight { get { return _bottom - _top; } }
        protected float BoundTop { get { return _top; } }
        protected float BoundLeft { get { return _left; } }

        //-------------------------------------------------------
        //layout ...
        public virtual bool NeedContentLayout
        {
            get { return false; }
        }
        internal bool IsInLayoutQueue { get; set; }

        //-------------------------------------------------------
        //events ...
        public bool TransparentAllMouseEvents
        {
            get;
            set;
        }
        public bool AutoStopMouseEventPropagation
        {
            get;
            set;
        }
        protected virtual void OnShown()
        {
        }
        protected virtual void OnHide()
        {
        }
        protected virtual void OnLostKeyboardFocus(UIFocusEventArgs e)
        {
        }
        protected virtual void OnLostMouseFocus(UIMouseEventArgs e)
        {
        }
        protected virtual void OnGotKeyboardFocus(UIFocusEventArgs e)
        {
        }
        protected virtual void OnDoubleClick(UIMouseEventArgs e)
        {
        }
        //-------------------------------------------------------
        protected virtual void OnMouseDown(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseMove(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseUp(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseEnter(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseLeave(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseWheel(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseHover(UIMouseEventArgs e)
        {
        }

        //------------------------------------------------------------
        protected virtual void OnKeyDown(UIKeyEventArgs e)
        {
        }
        protected virtual void OnKeyUp(UIKeyEventArgs e)
        {
        }
        protected virtual void OnKeyPress(UIKeyEventArgs e)
        {
        }
        protected virtual bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            return false;
        }
        //------------------------------------------------------------
        public void InvalidateLayout()
        {
            //add to layout queue
            UISystem.AddToLayoutQueue(this);
        }
        internal static void InvokeContentLayout(UIElement ui)
        {
            ui.OnContentLayout();
        }

        protected virtual void OnContentLayout()
        {
        }
        protected virtual void OnContentUpdate()
        {
        }
        protected virtual void OnInterComponentMsg(object sender, int msgcode, string msg)
        {
        }

        protected virtual void OnElementChanged()
        {
        }


        //
        public abstract void Walk(UIVisitor visitor);
        protected virtual void OnGuestTalk(UIGuestTalkEventArgs e)
        {
        }

#if DEBUG
        object dbugTagObject;
        public object dbugTag
        {
            get
            {
                return this.dbugTagObject;
            }
            set
            {
                this.dbugTagObject = value;
            }
        }
#endif
    }
}