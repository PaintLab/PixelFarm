//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.UI
{
    public class UICollection
    {
        LinkedList<UIElement> _uiList = new LinkedList<UIElement>();
        UIElement _owner;
        public UICollection(UIElement owner)
        {
            _owner = owner;
        }

        public int Count => _uiList.Count;
        public IEnumerable<UIElement> GetIter()
        {
            var node = _uiList.First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        public void AddAfter(UIElement afterUI, UIElement newUI)
        {
            if (newUI._collectionLinkNode != null)
            {
                throw new Exception("has some parent");
            }
            newUI._collectionLinkNode = _uiList.AddAfter(afterUI._collectionLinkNode, newUI);
        }
        public void AddBefore(UIElement beforeUI, UIElement newUI)
        {
            if (newUI._collectionLinkNode != null)
            {
                throw new Exception("has some parent");
            }
            newUI._collectionLinkNode = _uiList.AddBefore(beforeUI._collectionLinkNode, newUI);
        }
        public void AddFirst(UIElement ui)
        {
            if (ui._collectionLinkNode != null)
            {
                throw new Exception("has some parent");
            }
            ui._collectionLinkNode = _uiList.AddFirst(ui);

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
#endif
            if (ui._collectionLinkNode != null)
            {
                throw new Exception("has some parent");
            }

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

            ui.ParentUI = null;
            return true;
        }
        public void Clear()
        {
            //clear all parent relation
            LinkedListNode<UIElement> node = _uiList.First;
            while (node != null)
            {
                node.Value.ParentUI = null;
                node = node.Next;
            }

            _uiList.Clear();
        }

    }
}