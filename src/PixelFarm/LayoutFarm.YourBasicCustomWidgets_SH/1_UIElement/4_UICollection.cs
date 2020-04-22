//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.UI
{
    public struct UICollection
    {
        readonly LinkedList<UIElement> _uiList;
        readonly UIElement _owner;
        public UICollection(UIElement owner)
        {
            _owner = owner;
            _uiList = new LinkedList<UIElement>();
        }

        public bool IsNull => _owner == null;
        public int Count => _uiList == null ? 0 : _uiList.Count;
        public IEnumerable<UIElement> GetIter()
        {
            var node = _uiList.First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        public void AddAfter(UIElement afterUI, UIElement ui)
        {
            if (ui._collectionLinkNode != null)
            {
                throw new Exception("has some parent");
            }
            ui._collectionLinkNode = _uiList.AddAfter(afterUI._collectionLinkNode, ui);
            ui.ParentUI = _owner;
        }
        public void AddBefore(UIElement beforeUI, UIElement ui)
        {
            if (ui._collectionLinkNode != null)
            {
                throw new Exception("has some parent");
            }
            ui._collectionLinkNode = _uiList.AddBefore(beforeUI._collectionLinkNode, ui);
            ui.ParentUI = _owner;
        }
        public void AddFirst(UIElement ui)
        {
            if (ui._collectionLinkNode != null)
            {
                throw new Exception("has some parent");
            }
            ui._collectionLinkNode = _uiList.AddFirst(ui);
            ui.ParentUI = _owner;
        }
        /// <summary>
        /// add last
        /// </summary>
        /// <param name="ui"></param>
        public void AddUI(UIElement ui)
        {
#if DEBUG
            if (_owner == ui)
                throw new Exception("cyclic!");

            if (ui._collectionLinkNode != null ||
                ui.ParentUI != null)
            {
                throw new Exception("has some parent");
            }
#endif

            ui._collectionLinkNode = _uiList.AddLast(ui);
            ui.ParentUI = _owner;
        }
        public bool RemoveUI(UIElement ui)
        {
            //remove specific ui 
            if (ui._collectionLinkNode == null)
            {
                return false;
            }
            _uiList.Remove(ui._collectionLinkNode);
            ui._collectionLinkNode = null;
            ui.ParentUI = null; //
            return true;
        }

        public LinkedListNode<UIElement> GetUIElementLinkedListNode(int index)
        {
            //since we use linked-list,
            //get element at index need to search 
            //TODO: review this again, consider more proper collection

            if (index <= _uiList.Count / 2)
            {
                //1st half
                LinkedListNode<UIElement> node = _uiList.First;
                int i = 0;
                while (i < index)
                {
                    node = node.Next;//next node 
                }
                return node;
            }
            else
            {
                LinkedListNode<UIElement> node = _uiList.Last;
                int i = _uiList.Count - 1;
                while (i > index)
                {
                    node = node.Previous;
                }
                return node;
            }
        }
        public void Clear()
        {
            //clear all parent relation
            LinkedListNode<UIElement> node = _uiList.First;
            while (node != null)
            {
                UIElement ui = node.Value;
                node = node.Next;

                ui.ParentUI = null;
                UIElement.UnsafeRemoveLinkedNode(ui);
            }
            _uiList.Clear();
        }

        public static void AcceptVisitor(UICollection collecton, UIVisitor visitor)
        {
            var node = collecton._uiList.First;
            while (node != null && !visitor.StopWalking)
            {
                UIElement.AcceptVisitor(node.Value, visitor);
                node = node.Next;
            }
        }
    }
}