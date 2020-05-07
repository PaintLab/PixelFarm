//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public enum ContentStretch : byte
    {
        None,
        Horizontal,
        Vertical,
        Both,
    }


    public sealed class Box : AbstractBox, IContainerUI, ISimpleContainerUI
    {
        UIList<UIElement> _uiList;
        public Box(int w, int h)
            : base(w, h)
        {

        }

        public override void NotifyContentUpdate(UIElement childContent)
        {
            //set propersize

            this.InvalidateLayout();
            this.ParentUI?.InvalidateLayout();
        }
        protected override IUICollection<UIElement> GetDefaultChildrenIter() => _uiList;


        public IEnumerable<UIElement> GetChildIter() => (_uiList != null) ? _uiList.GetIter() : UIElement.EmptyArr;

        public int ChildCount => (_uiList != null) ? _uiList.Count : 0;

        public void Insert(int index, UIElement ui) => _uiList.Insert(this, index, ui);

        public void AddAfter(UIElement afterUI, UIElement ui) => _uiList.AddAfter(this, afterUI, ui);

        public void AddBefore(UIElement beforeUI, UIElement ui) => _uiList.AddBefore(this, beforeUI, ui);

        public void AddFirst(UIElement ui)
        {
            if (_uiList == null)
            {
                _uiList = new UIList<UIElement>();
            }
            _uiList.AddFirst(this, ui);
        }


        public void Add(UIElement ui)
        {
            if (_uiList == null)
            {
                _uiList = new UIList<UIElement>();
            }
            _uiList.Add(this, ui);
        }
        public void AddContent(UIElement ui) => Add(ui);
        public void AddLast(UIElement ui) => Add(ui);
        public void RemoveChild(UIElement ui) => _uiList.Remove(this, ui);
        public void ClearChildren() => _uiList?.Clear(this);

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

        protected override void OnAcceptVisitor(UIVisitor visitor) => _uiList?.AcceptVisitor(visitor);

       
         
    }
}