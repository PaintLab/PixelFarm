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


        //----------------------------------- 
        public object Tag { get; set; }
        //----------------------------------- 
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
        public virtual bool NeedContentLayout
        {
            get { return false; }
        }

        //-------------------------------------------------------
        internal bool IsInLayoutQueue { get; set; }

        //-------------------------------------------------------
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