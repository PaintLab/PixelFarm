//Apache2, 2014-present, WinterDev

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

#if DEBUG
            //if (UIElement.s_dbugBreakOnSetBounds)
            //{
            //    for (int i = count - 1; i >= 0; --i)
            //    {
            //        UIElement ui = s_layoutQueue.Dequeue();
            //        ui.IsInLayoutQueue = false;
            //        if (ui.IsInLayoutQueue)
            //        {
            //            //should not occur
            //            throw new System.NotSupportedException();
            //        }
            //    }
            //    return;
            //}
#endif


            for (int i = count - 1; i >= 0; --i)
            {
                UIElement ui = s_layoutQueue.Dequeue();
                ui.IsInLayoutQueue = false;
                UIElement.InvokeContentLayout(ui);
#if DEBUG
                if (ui.IsInLayoutQueue)
                {
                    //should not occur
                    throw new System.NotSupportedException();
                }
#endif
            }



        }
    }

    public abstract partial class UIElement : IUIEventListener
    {

#if DEBUG
        public bool dbugBreakMe;
        static int s_dbugTotalId;
        public readonly int dbugId = s_dbugTotalId++;

#endif
        bool _hide;

        //bounds
        float _left;
        float _top;
        float _right;
        float _bottom;
        //object _tag;
        UIElement _parent;
        internal LinkedListNode<UIElement> _collectionLinkNode;

        public UIElement()
        {
            //if (dbugId == 114)
            //{ 
            //}
        }
        public bool DisableAutoMouseCapture { get; set; }
        public abstract RenderElement GetPrimaryRenderElement(RootGraphic rootgfx);
        public abstract RenderElement CurrentPrimaryRenderElement { get; }
        protected virtual bool HasReadyRenderElement => CurrentPrimaryRenderElement != null;
        public abstract void InvalidateGraphics();

        public bool AcceptKeyboardFocus
        {
            get;
            set;
        }

        public virtual object Tag
        {
            get => null;
            set
            {
                throw new System.NotSupportedException("user must override this");
            }
        }

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
        public UIElement ParentUI
        {
            get => _parent;
            set
            {

                //if (value == null)
                //{

                //}

                _parent = value;
            }
        }

        public UIElement NextUIElement
        {
            get
            {
                if (_collectionLinkNode != null)
                {
                    LinkedListNode<UIElement> nextNode = _collectionLinkNode.Next;
                    return (nextNode != null) ? nextNode.Value : null;
                }
                return null;
            }
        }
        public UIElement PrevUIElement
        {
            get
            {
                if (_collectionLinkNode != null)
                {
                    LinkedListNode<UIElement> prevNode = _collectionLinkNode.Previous;
                    return (prevNode != null) ? prevNode.Value : null;
                }
                return null;
            }
        }
        //------------------------------
        public virtual void RemoveChild(UIElement ui)
        {
#if DEBUG
            throw new System.NotSupportedException("user must impl this");
#endif
        }
        public virtual void ClearChildren()
        {
#if DEBUG
            throw new System.NotSupportedException("user must impl this");
#endif
        }
        public virtual void RemoveSelf()
        {


            RenderElement currentRenderE = this.CurrentPrimaryRenderElement;
            if (currentRenderE != null &&
                currentRenderE.HasParent)
            {
                currentRenderE.RemoveSelf();
            }
            if (_parent != null)
            {
                _parent.RemoveChild(this);
            }
            this.InvalidateOuterGraphics();
#if DEBUG
            if (_collectionLinkNode != null || _parent != null)
            {
                throw new System.Exception("");
            }
#endif
        }

        public virtual void AddFirst(UIElement ui)
        {
#if DEBUG
            throw new System.Exception("empty!");
#endif

        }
        public virtual void AddAfter(UIElement afterUI, UIElement ui)
        {
#if DEBUG
            throw new System.Exception("empty!");
#endif
        }
        public virtual void AddBefore(UIElement beforeUI, UIElement ui)
        {
#if DEBUG
            throw new System.Exception("empty!");
#endif
        }
        public virtual void Add(UIElement ui)
        {
#if DEBUG
            throw new System.Exception("empty!");
#endif
        }
        public virtual void BringToTopMost()
        {
            if (_parent != null)
            {
                //after RemoveSelf_parent is set to null
                //so we backup it before RemoveSelf
                UIElement parentUI = _parent;
                parentUI.RemoveChild(this);
                parentUI.Add(this);
                this.InvalidateGraphics();
            }
        }
        public virtual void BringToTopOneStep()
        {
            if (_parent != null)
            {
                //find next element
                UIElement next = this.NextUIElement;
                if (next != null)
                {
                    UIElement parentUI = _parent;
                    parentUI.RemoveChild(this);
                    parentUI.AddAfter(next, this);
                    this.InvalidateGraphics();
                }
            }
        }
        public virtual void SendToBackMost()
        {
            if (_parent != null)
            {
                //after RemoveSelf_parent is set to null
                //so we backup it before RemoveSelf

                UIElement parentUI = _parent;
                parentUI.RemoveChild(this);
                parentUI.AddFirst(this);
                this.InvalidateGraphics();
            }
        }
        public virtual void SendOneStepToBack()
        {
            if (_parent != null)
            {
                //find next element
                UIElement prev = this.PrevUIElement;
                if (prev != null)
                {
                    UIElement parentUI = _parent;
                    parentUI.RemoveChild(this);
                    parentUI.AddBefore(prev, this);
                }
            }
        }

        //------------------------------
        public virtual void InvalidateOuterGraphics()
        {

        }
        public virtual bool Visible
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return CurrentPrimaryRenderElement.Visible;
                }
                else
                {
                    return !_hide;
                }
            }
            set
            {
                _hide = !value;
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
        public PixelFarm.Drawing.Point GetLocation()
        {
            return new PixelFarm.Drawing.Point((int)_left, (int)_top);
        }
        public virtual void GetViewport(out int left, out int top)
        {
            left = top = 0;
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
#if DEBUG
            //if (s_dbugBreakOnSetBounds)
            //{
            //    if (dbugBreakMe)
            //    {

            //    }
            //    else if (!dbugIsImgBox)
            //    {

            //    }
            //}
#endif
            _right = _left + width;
            _bottom = _top + height;
        }
        protected void SetElementBoundsLTWH(float left, float top, float width, float height)
        {
#if DEBUG
            //if (s_dbugBreakOnSetBounds)
            //{
            //    if (dbugBreakMe)
            //    {

            //    }
            //    else if (!dbugIsImgBox)
            //    {

            //    }
            //}
#endif
            //change 'TransparentBounds' => not effect visual presentation
            _left = left;
            _top = top;
            _right = left + width;
            _bottom = top + height;
        }
        protected void SetElementBounds(float left, float top, float right, float bottom)
        {
#if DEBUG
            //if (s_dbugBreakOnSetBounds)
            //{
            //    if (dbugBreakMe)
            //    {

            //    }
            //    else if (!dbugIsImgBox)
            //    {

            //    }
            //}
#endif
            //change 'TransparentBounds' => not effect visual presentation
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }
        protected void SetElementBoundsLT(float left, float top)
        {

#if DEBUG
            //if (s_dbugBreakOnSetBounds)
            //{
            //    if (dbugBreakMe)
            //    {

            //    }
            //    else if (!dbugIsImgBox)
            //    {

            //    }
            //}
#endif


            _bottom = top + (_bottom - _top);
            _right = left + (_right - _left);
            _left = left;
            _top = top;
        }

#if DEBUG
        public static bool s_dbugBreakOnSetBounds = false;
#endif
        //-------------------------------------------------------
        protected float BoundWidth => _right - _left;
        protected float BoundHeight => _bottom - _top;
        protected float BoundTop => _top;
        protected float BoundLeft => _left;
        //-------------------------------------------------------
        //layout ...
        public virtual bool NeedContentLayout => false;
        internal bool IsInLayoutQueue { get; set; }
        //-------------------------------------------------------
        //events ...
        bool _transparentAllMouseEvents; //TODO: review here
        public bool TransparentAllMouseEvents
        {
            get => _transparentAllMouseEvents;
            set
            {
                _transparentAllMouseEvents = value;
                if (this.HasReadyRenderElement)
                {
                    this.CurrentPrimaryRenderElement.TransparentForAllEvents = value;
                }
            }
        }
        //
        public bool AutoStopMouseEventPropagation { get; set; }
        //
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
        public virtual void NotifyContentUpdate(UIElement childContent)
        {
            //
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
        public abstract void Accept(UIVisitor visitor);
        protected virtual void OnGuestTalk(UIGuestTalkEventArgs e)
        {
        }
        public static void UnsafeRemoveLinkedNode(UIElement ui)
        {
            ui._collectionLinkNode = null;
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