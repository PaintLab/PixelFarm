//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;

namespace LayoutFarm.UI
{


    public interface IUICollectionNodeLocator<T>
        where T : UIElement
    {
        bool HasNext();
        bool HasPrev();
        T MoveNext();
        T MovePrev();
    }


    partial class UIElement
    {
        struct CollectionHelper
        {
            public static void CheckUI<T>(UIElement parent, T ui)
                where T : UIElement
            {
                if (ui._parent != null)
                {
                    throw new Exception("has some parent");
                }
                if (parent == ui)
                {
                    throw new Exception("same object");
                }
            }
            public static void UpdateLayout<T>(UIElement parent, T ui) where T : UIElement
            {
                if (ui.NeedContentLayout)
                {
                    if (!ui.IsInLayoutQueue)
                    {
                        ui.InvalidateLayout();
                    }
                }
                parent.InvalidateLayout();
            }
        }

        protected interface IUICollection1<T> where T : UIElement
        {
            IEnumerable<T> GetIter();
        }

        struct UICollectionNodeLocator1<T> : IUICollectionNodeLocator<T>
            where T : UIElement
        {
            internal LinkedListNode<T> _node;
            LinkedListNode<T> _curNode;
            internal UICollectionNodeLocator1(LinkedListNode<T> node)
            {
                _node = node;
                _curNode = node;
            }
            public bool HasNext()
            {
                return _curNode.Next != null;
            }
            public bool HasPrev()
            {
                return _curNode.Previous != null;
            }
            public T MoveNext()
            {
                _curNode = _node.Next;
                return _curNode.Value;
            }
            public T MovePrev()
            {
                _curNode = _node.Previous;
                return _curNode.Value;
            }
        }
        protected class UILinkedList<T> : IUICollection1<T>
            where T : UIElement
        {
            readonly LinkedList<T> _linkedList = new LinkedList<T>();
            public UILinkedList()
            {
            }
            public void Add(UIElement parent, T ui)
            {
                //ui must not have parent before!
                CollectionHelper.CheckUI(parent, ui);
                _linkedList.AddLast(ui);
                ui.ParentUI = parent;
                //---
                //presentation 
                parent._needContentLayout = true;
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parentRenderE.AddChild(ui.GetPrimaryRenderElement(parentRenderE.Root));
                    parent?.InvalidateLayout();
                }
                //---

            }
            public void AddFirst(UIElement parent, T ui)
            {
                //ui must not have parent before!
                CollectionHelper.CheckUI(parent, ui);
                _linkedList.AddFirst(ui);
                ui.ParentUI = parent;

                //---
                //presentation 
                parent._needContentLayout = true;
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parentRenderE.AddFirst(ui.GetPrimaryRenderElement(parentRenderE.Root));
                    parent?.InvalidateLayout();
                }
            }
            public void AddAfter(UIElement parent, T afterUI, T ui)
            {
                //ui must not have parent before!
                CollectionHelper.CheckUI(parent, ui);
                LinkedListNode<UIElement> linkedNode = afterUI._collectionLinkNode;
                ui._collectionLinkNode = (LinkedListNode<UIElement>)(object)_linkedList.AddAfter((LinkedListNode<T>)(object)linkedNode, ui);
                ui.ParentUI = parent;

                //---
                //presentation 
                parent._needContentLayout = true;
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parentRenderE.InsertAfter(
                        afterUI.GetPrimaryRenderElement(parentRenderE.Root),
                        ui.GetPrimaryRenderElement(parentRenderE.Root));
                    parent?.InvalidateLayout();
                }

            }
            public void AddBefore(UIElement parent, T beforeUI, T ui)
            {
                CollectionHelper.CheckUI(parent, ui);
                LinkedListNode<UIElement> linkedNode = beforeUI._collectionLinkNode;
                ui._collectionLinkNode = (LinkedListNode<UIElement>)(object)_linkedList.AddBefore((LinkedListNode<T>)(object)linkedNode, ui);
                ui.ParentUI = parent;


                //---
                //presentation 
                parent._needContentLayout = true;
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parentRenderE.InsertBefore(
                        beforeUI.GetPrimaryRenderElement(parentRenderE.Root),
                        ui.GetPrimaryRenderElement(parentRenderE.Root));
                    parent?.InvalidateLayout();
                }
            }
            public int Count => _linkedList.Count;
            public void Clear(UIElement parent)
            {
                LinkedListNode<T> node = _linkedList.First;
                while (node != null)
                {
                    UIElement ui = node.Value;
                    node = node.Next;

                    ui.ParentUI = null;
                    UIElement.UnsafeRemoveLinkedNode(ui);
                }
                _linkedList.Clear();

                //--------
                //need to remove presentation 
                parent._needContentLayout = true;
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;

                parentRenderE?.ClearAllChildren();  
            }

            public void Remove(UIElement parent, T ui)
            {
                //--------
                //need to remove presentation 
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parentRenderE.RemoveChild(ui.GetPrimaryRenderElement(parentRenderE.Root));
                }

                //--------
                //need to remove presentation 

                //we must ensure linked node is valid
                parent._needContentLayout = true;
                LinkedListNode<UIElement> linkedNode = ui._collectionLinkNode;
                _linkedList.Remove((LinkedListNode<T>)(object)linkedNode);

                parent?.InvalidateLayout(); 
            }
            public IEnumerable<T> GetIter()
            {
                var node = _linkedList.First;
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Next;
                }
            }

            public void AcceptVisitor(UIVisitor visitor)
            {
                var node = _linkedList.First;
                while (node != null && !visitor.StopWalking)
                {
                    UIElement.AcceptVisitor(node.Value, visitor);
                    node = node.Next;
                }
            }
            public IUICollectionNodeLocator<T> GetNodeLocator(T ui)
            {
                return new UICollectionNodeLocator1<T>((LinkedListNode<T>)(object)ui._collectionLinkNode);
            }

        }
        //
        protected class UIList<T> : IUICollection1<T>
            where T : UIElement
        {
            readonly List<T> _list = new List<T>();
            public UIList()
            {
            }
            public void Add(UIElement parent, T ui)
            {
                CollectionHelper.CheckUI(parent, ui);
                _list.Add(ui);
                ui._parent = parent;

                //---
                //presentation
                parent._needContentLayout = true;
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parentRenderE.AddChild(ui.GetPrimaryRenderElement(parentRenderE.Root));
                    CollectionHelper.UpdateLayout(parent, ui);
                }
            }
            public void AddFirst(UIElement parent, T ui)
            {
                CollectionHelper.CheckUI(parent, ui);
                _list.Insert(0, ui);
                ui._parent = parent;

                //---
                //presentation
                parent._needContentLayout = true;
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parentRenderE.AddFirst(ui.GetPrimaryRenderElement(parentRenderE.Root));
                    CollectionHelper.UpdateLayout(parent, ui);
                }
            }
            public void AddAfter(UIElement parent, T afterUI, T ui)
            {
                CollectionHelper.CheckUI(parent, ui);
                //find afterUI
                int index = _list.IndexOf(afterUI);
                if (index == -1)
                {
                    throw new NotSupportedException();//***
                }
                if (index == _list.Count - 1)
                {
                    //the last one
                    _list.Add(ui);
                }
                else
                {
                    _list.Insert(index + 1, ui);
                }
                ui.ParentUI = parent;

                //---
                //presentation
                parent._needContentLayout = true;
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parentRenderE.InsertAfter(
                        afterUI.GetPrimaryRenderElement(parentRenderE.Root),
                        ui.GetPrimaryRenderElement(parentRenderE.Root));

                    CollectionHelper.UpdateLayout(parent, ui);
                }
            }
            public void AddBefore(UIElement parent, T beforeUI, T ui)
            {
                CollectionHelper.CheckUI(parent, ui);
                //find afterUI
                int index = _list.IndexOf(beforeUI);
                if (index == -1)
                {
                    throw new NotSupportedException();//***
                }
                _list.Insert(index, ui);
                ui.ParentUI = parent;

                //---
                //presentation
                parent._needContentLayout = true;
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parentRenderE.InsertBefore(
                        beforeUI.GetPrimaryRenderElement(parentRenderE.Root),
                        ui.GetPrimaryRenderElement(parentRenderE.Root));
                    CollectionHelper.UpdateLayout(parent, ui);
                }
            }
            public void Remove(UIElement parent, T ui)
            {
                parent._needContentLayout = true;
                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parentRenderE.RemoveChild(ui.GetPrimaryRenderElement(parentRenderE.Root));

                }

                _list.Remove(ui);//***
                //---
                //presentation
                if (parentRenderE != null)
                {
                    CollectionHelper.UpdateLayout(parent, ui);
                }
            }
            public int Count => _list.Count;
            public void Clear(UIElement parent)
            {
                //clear all child
                //and remove parent linkage
                for (int i = _list.Count - 1; i >= 0; --i)
                {
                    UIElement ui = _list[i];
                    ui.ParentUI = null;
                    UIElement.UnsafeRemoveLinkedNode(ui);
                }
                _list.Clear();

                parent.InvalidateLayout();

            }
            public void AcceptVisitor(UIVisitor visitor)
            {
                int j = _list.Count;
                for (int i = 0; i < j && !visitor.StopWalking; ++i)
                {
                    UIElement.AcceptVisitor(_list[i], visitor);
                }
            }
            public IEnumerable<T> GetIter()
            {
                int j = _list.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return _list[i];
                }
            }
            public IUICollectionNodeLocator<T> GetNodeLocator(T ui)
            {
                return new UICollectionNodeLocator2<T>(_list, _list.IndexOf(ui));
            }

            //special for list
            public void Insert(UIElement parent, int index, T ui)
            {
                CollectionHelper.CheckUI(parent, ui);
                _list.Insert(index, ui);
                ui.ParentUI = parent;

                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    CollectionHelper.UpdateLayout(parent, ui);
                }
            }
            public void RemoveAt(UIElement parent, int index)
            {
                T existing = _list[index];
                _list.RemoveAt(index);
                existing.ParentUI = null;//clear link

                RenderElement parentRenderE = parent.CurrentPrimaryRenderElement;
                if (parentRenderE != null)
                {
                    parent.InvalidateLayout();
                }

            }
            public T this[int index] => _list[index];
        }
        public struct UICollectionNodeLocator2<T> : IUICollectionNodeLocator<T>
            where T : UIElement
        {
            readonly List<T> _list;
            int _index;
            internal UICollectionNodeLocator2(List<T> list, int index)
            {
                _list = list;
                _index = index;
            }
            public bool HasNext() => _index < _list.Count - 1;
            public bool HasPrev() => _index > 0;
            public T MoveNext()
            {
                _index++;
                return _list[_index];
            }
            public T MovePrev()
            {
                _index--;
                return _list[_index];
            }
        }

    }


}