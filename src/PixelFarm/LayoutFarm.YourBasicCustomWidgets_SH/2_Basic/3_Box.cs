//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{


    public enum ContentStretch
    {
        None,
        Horizontal,
        Vertical,
        Both,
    }


    public sealed class Box : AbstractBox, IContainerUI
    {
        UIList<UIElement> _uiList;
        public Box(int w, int h)
            : base(w, h)
        {

        }
        public object Tag { get; set; }
        public override void NotifyContentUpdate(UIElement childContent)
        {
            //set propersize

            //if (childContent is ImageBox)
            //{
            //    ImageBox imgBox = (ImageBox)childContent;
            //    this.SetSize(imgBox.Width, imgBox.Height); 
            //}

            this.InvalidateLayout();
            //this.ParentUI?.NotifyContentUpdate(this);
            this.ParentUI?.InvalidateLayout();
        }

        //----------------------------------------------------
        static UIElement[] s_empty = new UIElement[0];
        public IEnumerable<UIElement> GetChildIter()
        {
            if (_uiList != null)
            {
                return _uiList.GetIter();
            }
            return s_empty;
        }

        public int ChildCount => (_uiList != null) ? _uiList.Count : 0;

        public void Insert(int index, UIElement ui)
        {
            _needContentLayout = true;
            _uiList.Insert(this, index, ui);

            //LinkedListNode<UIElement> insertAt = _uiList.GetUIElementLinkedListNode(index);

            //if (this.HasReadyRenderElement)
            //{
            //    _primElement.InsertBefore(
            //            insertAt.Value.GetPrimaryRenderElement(_primElement.Root),
            //            ui.GetPrimaryRenderElement(_primElement.Root));

            //    if (_supportViewport)
            //    {
            //        this.InvalidateLayout();
            //    }
            //}

            //if (ui.NeedContentLayout)
            //{
            //    ui.InvalidateLayout();
            //}
        }

        public void AddAfter(UIElement afterUI, UIElement ui)
        {
            _uiList.AddAfter(this, afterUI, ui);
            //_uiList.AddAfter(afterUI, ui); 
            //_needContentLayout = true;
            ////insert new child after existing one 
            //if (this.HasReadyRenderElement)
            //{
            //    _primElement.InsertAfter(
            //        afterUI.GetPrimaryRenderElement(_primElement.Root),
            //        ui.GetPrimaryRenderElement(_primElement.Root));

            //    if (_supportViewport)
            //    {
            //        this.InvalidateLayout();
            //    }
            //}

            //if (ui.NeedContentLayout)
            //{
            //    ui.InvalidateLayout();
            //}
        }
        public void AddBefore(UIElement beforeUI, UIElement ui)
        {
            _needContentLayout = true;
            _uiList.AddBefore(this, beforeUI, ui);

            //if (this.HasReadyRenderElement)
            //{
            //    _primElement.InsertBefore(
            //        beforeUI.GetPrimaryRenderElement(_primElement.Root),
            //        ui.GetPrimaryRenderElement(_primElement.Root));

            //    if (_supportViewport)
            //    {
            //        this.InvalidateLayout();
            //    }
            //}

            //if (ui.NeedContentLayout)
            //{
            //    ui.InvalidateLayout();
            //}
        }
        public void AddFirst(UIElement ui)
        {
            if (_uiList == null)
            {
                _uiList = new UIList<UIElement>();
            }

            //_needContentLayout = true;
            _uiList.AddFirst(this, ui);

            //if (this.HasReadyRenderElement)
            //{
            //    _primElement.AddFirst(
            //        ui.GetPrimaryRenderElement(_primElement.Root));

            //    if (_supportViewport)
            //    {
            //        this.InvalidateLayout();
            //    }
            //}

            //if (ui.NeedContentLayout)
            //{
            //    ui.InvalidateLayout();
            //}
        }

        public void Add(UIElement ui)
        {
            if (_uiList == null)
            {
                _uiList = new UIList<UIElement>();
            }

            _needContentLayout = true;
            _uiList.Add(this, ui);


        }
        public void AddLast(UIElement ui) => Add(ui);
        public void RemoveChild(UIElement ui)
        {
            _uiList.Remove(this, ui);


            //_needContentLayout = true;
            //_uiList.RemoveUI(ui);
            //if (this.HasReadyRenderElement)
            //{
            //    if (_supportViewport)
            //    {
            //        this.InvalidateLayout();
            //    }
            //    _primElement.RemoveChild(ui.CurrentPrimaryRenderElement);
            //}
        }
        public void ClearChildren()
        {
            _needContentLayout = true;
            if (_uiList != null)
            {
                _uiList.Clear(this);
            }
            //if (!_uiList.IsNull)
            //{
            //    _uiList.Clear();
            //}
            //if (this.HasReadyRenderElement)
            //{
            //    _primElement.ClearAllChildren();
            //    if (Visible)
            //    {
            //        if (_supportViewport)
            //        {
            //            this.InvalidateLayout();
            //        }
            //    }
            //}
        }

        public bool BringChildToFront(UIElement ui, int steps)
        {
            //TODO: more step
            //support steps=1
            //now we need to find this node in the list

            var currentNode = _uiList.GetNodeLocator(ui);

            if (currentNode.HasNext())
            {
                UIElement next = currentNode.MoveNext();
                RemoveChild(ui);
                AddAfter(next, ui);
                return true;
            }
            return false;
        }

        public bool BringChildToFrontMost(UIElement ui)
        {
            RemoveChild(ui);
            AddLast(ui);
            return true;
        }

        public bool SendChildToBack(UIElement ui, int steps)
        {
            var currentNode = _uiList.GetNodeLocator(ui);

            if (currentNode.HasPrev())
            {
                UIElement prev = currentNode.MovePrev();
                RemoveChild(ui);
                AddBefore(prev, ui);
                return true;
            }
            return false;

        }

        public bool SendChildToBackMost(UIElement ui)
        {
            RemoveChild(ui);
            AddFirst(ui);
            return true;
        }

        public override BoxContentLayoutKind ContentLayoutKind
        {
            get => base.ContentLayoutKind;
            set
            {
                base.ContentLayoutKind = value; //invalidate layout after change this
                if (_uiList != null && _uiList.Count > 0)
                {
                    this.InvalidateLayout();
                }
            }
        }

        protected override void OnAcceptVisitor(UIVisitor visitor)
        {
            if (_uiList != null)
            {
                _uiList.AcceptVisitor(visitor);
            }
        }

        protected override IUICollection<UIElement> GetDefaultChildrenIter() => _uiList;
        protected override void BuildChildrenRenderElement(RenderElement parent)
        {
            GlobalRootGraphic.BlockGraphicsUpdate();
            parent.HasSpecificHeight = this.HasSpecificHeight;
            parent.HasSpecificWidth = this.HasSpecificWidth;
            parent.SetController(this);
            parent.SetVisible(this.Visible);
            parent.SetLocation(this.Left, this.Top);
            parent.HasSpecificWidthAndHeight = true; //?
            parent.SetViewport(this.ViewportLeft, this.ViewportTop);

            if (ChildCount > 0)
            {

                foreach (UIElement ui in GetChildIter())
                {
                    parent.AddChild(ui);
                }
            }

            GlobalRootGraphic.ReleaseGraphicsUpdate();
            parent.InvalidateGraphics();
        }
        public override void UpdateLayout()
        {
            base.UpdateLayout();
            foreach (var chlid in GetChildIter())
            {
                if (chlid != null)
                {
                    chlid.UpdateLayout();
                }
            }
        }

    }


}