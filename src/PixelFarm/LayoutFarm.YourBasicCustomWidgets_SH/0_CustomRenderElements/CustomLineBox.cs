//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;


namespace LayoutFarm.CustomWidgets
{
    struct MayBeEmptyTempContext<T> : IDisposable
    {
        internal readonly T _tool;
        internal MayBeEmptyTempContext(out T tool)
        {
            MayBeEmptyTempContext<T>.GetFreeItem(out _tool);
            tool = _tool;
        }
        public void Dispose()
        {
            if (_tool != null)
            {
                MayBeEmptyTempContext<T>.Release(_tool);
            }
        }

        public static readonly MayBeEmptyTempContext<T> Empty = new MayBeEmptyTempContext<T>();


        public delegate T CreateNewItemDelegate();
        public delegate void ReleaseItemDelegate(T item);


        [System.ThreadStatic]
        static Stack<T> s_pool;
        [System.ThreadStatic]
        static CreateNewItemDelegate s_newHandler;
        [System.ThreadStatic]
        static ReleaseItemDelegate s_releaseCleanUp;

        public static MayBeEmptyTempContext<T> Borrow(out T freeItem)
        {
            return new MayBeEmptyTempContext<T>(out freeItem);
        }

        public static void SetNewHandler(CreateNewItemDelegate newHandler, ReleaseItemDelegate releaseCleanUp = null)
        {
            //set new instance here, must set this first***
            if (s_pool == null)
            {
                s_pool = new Stack<T>();
            }
            s_newHandler = newHandler;
            s_releaseCleanUp = releaseCleanUp;
        }
        internal static void GetFreeItem(out T freeItem)
        {
            if (s_pool.Count > 0)
            {
                freeItem = s_pool.Pop();
            }
            else
            {
                freeItem = s_newHandler();
            }
        }
        internal static void Release(T item)
        {
            s_releaseCleanUp?.Invoke(item);
            s_pool.Push(item);
            //... 
        }
        public static bool IsInit()
        {
            return s_pool != null;
        }
    }

    static class LayoutTools
    {
        public static MayBeEmptyTempContext<LinkedList<T>> BorrowLinkedList<T>(out LinkedList<T> linkedlist)
        {
            if (!MayBeEmptyTempContext<LinkedList<T>>.IsInit())
            {
                MayBeEmptyTempContext<LinkedList<T>>.SetNewHandler(
                    () => new LinkedList<T>(),
                    list => list.Clear());
            }
            return MayBeEmptyTempContext<LinkedList<T>>.Borrow(out linkedlist);
        }
        public static MayBeEmptyTempContext<List<T>> BorrowList<T>(out List<T> linkedlist)
        {
            if (!MayBeEmptyTempContext<List<T>>.IsInit())
            {
                MayBeEmptyTempContext<List<T>>.SetNewHandler(
                    () => new List<T>(),
                    list => list.Clear());
            }
            return MayBeEmptyTempContext<List<T>>.Borrow(out linkedlist);
        }
        public static MayBeEmptyTempContext<LineBox> BorrowLineBox(out LineBox linebox)
        {
            if (!MayBeEmptyTempContext<LineBox>.IsInit())
            {
                MayBeEmptyTempContext<LineBox>.SetNewHandler(
                    () => new LineBox(),
                    line => line.Clear());
            }
            return MayBeEmptyTempContext<LineBox>.Borrow(out linebox);
        }
    }
    struct LineBoxesContext : IDisposable
    {
        RenderElement _owner;
        MayBeEmptyTempContext<List<LineBox>> _context;
        List<LineBox> _lineboxes;

        List<MayBeEmptyTempContext<LineBox>> _sharedLineBoxContexts;
        MayBeEmptyTempContext<List<MayBeEmptyTempContext<LineBox>>> _sharedLineBoxContextListContext;

        public LineBoxesContext(CustomRenderBox owner)
        {
            //set owner element if we want to preserver linebox ***
            _owner = owner;
            if (_owner == null)
            {
                //don't preserve linebox
                _context = LayoutTools.BorrowList(out _lineboxes);
                _sharedLineBoxContextListContext = LayoutTools.BorrowList(out _sharedLineBoxContexts);
            }
            else
            {
                //preserver context
                _context = MayBeEmptyTempContext<List<LineBox>>.Empty;
                _lineboxes = owner.Lines;
                if (_lineboxes == null)
                {
                    _lineboxes = owner.Lines = new List<LineBox>();
                }
                else
                {
                    _lineboxes.Clear();
                }

                _sharedLineBoxContexts = null;
                _sharedLineBoxContextListContext = MayBeEmptyTempContext<List<MayBeEmptyTempContext<LineBox>>>.Empty;

            }
        }
        public void Dispose()
        {
            //release if we use pool
            _context.Dispose();
            if (_sharedLineBoxContexts != null)
            {
                //release all lineboxes
                int j = _sharedLineBoxContexts.Count;
                for (int i = 0; i < j; ++i)
                {
                    _sharedLineBoxContexts[i].Dispose();
                }
                //
                _sharedLineBoxContexts.Clear();
                _sharedLineBoxContexts = null;
                //
                _sharedLineBoxContextListContext.Dispose();
            }
        }

        public LineBox AddNewLineBox()
        {
            if (_owner != null)
            {
                LineBox newline = new LineBox();
                newline.ParentRenderElement = _owner;//***
                _lineboxes.Add(newline);
                return newline;
            }
            else
            {
                //we can use it from pool
                //and we will release this later
                _sharedLineBoxContexts.Add(LayoutTools.BorrowLineBox(out LineBox sharedLinebox));
                _lineboxes.Add(sharedLinebox);

                return sharedLinebox;
            }
        }
    }

    class LineBox : IParentLink
    {
        LinkedList<RenderElement> _linkList = new LinkedList<RenderElement>();

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId;
#endif
        public LineBox()
        {
#if DEBUG
            dbugId = dbugTotalId++;
#endif

        }
        public int LineTop { get; set; }
        public int LineHeight { get; set; }
        public int LineBottom => LineTop + LineHeight;
        public RenderElement ParentRenderElement { get; internal set; }

        public void AdjustLocation(ref int px, ref int py)
        {
            //TODO: line left ?
            py += LineTop;
        }
#if DEBUG
        public string dbugGetLinkInfo()
        {
            return "linkbox";
        }
#endif
        public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            return null;
        }

        public bool IsIntersect(int minY, int maxY) => !((maxY < LineTop) || (minY > (LineTop + LineHeight)));

        //TODO: need to clear all parent link? 
        public void Clear() => _linkList.Clear();

        public void Add(RenderElement renderE)
        {
            _linkList.AddLast(renderE);
            if (ParentRenderElement != null)
            {
                RenderElement.SetParentLink(renderE, this);
            }
        }

        public int Count => _linkList.Count;

        public bool HitTestCore(HitChain hitChain)
        {

            if (LineTop <= hitChain.TestPointY &&
               (LineTop + LineHeight) > hitChain.TestPointY)
            {
                LinkedListNode<RenderElement> node = _linkList.First;
                hitChain.OffsetTestPoint(0, -LineTop);
                bool found = false;
                while (node != null)
                {
                    if (node.Value.HitTestCore(hitChain))
                    {
                        found = true;
                        break;
                    }
                    node = node.Next;
                }
                hitChain.OffsetTestPoint(0, LineTop);
                return found;
            }
            return false;
        }
        public void Render(DrawBoard d, UpdateArea updateArea)
        {
            LinkedListNode<RenderElement> renderNode = _linkList.First;
            Rectangle backup = updateArea.CurrentRect;
            int enter_canvas_x = d.OriginX;
            int enter_canvas_y = d.OriginY;

            while (renderNode != null)
            {
                //---------------------------
                //TODO: review here again
                RenderElement renderE = renderNode.Value;
                if (renderE.IntersectsWith(updateArea))
                {
                    int x = renderE.X;
                    int y = renderE.Y;

                    d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
                    updateArea.Offset(-x, -y);
                    RenderElement.Render(renderE, d, updateArea);
                    updateArea.Offset(x, y);
                }

                renderNode = renderNode.Next;
            }
            updateArea.CurrentRect = backup;//restore  
            d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);//restore
        }

        public void AdjustHorizontalAlignment(RectUIAlignment alignment)
        {
            //line, and spread data inside this line


        }
        public void AdjustVerticalAlignment(VerticalAlignment vertAlignment)
        {
            LinkedListNode<RenderElement> node = _linkList.First;

            switch (vertAlignment)
            {
                case VerticalAlignment.Bottom:
                    while (node != null)
                    {
                        RenderElement r = node.Value;
                        if (r is CustomRenderBox box)
                        {
                            int diff = LineHeight - r.Height;
                            if (diff > 0)
                            {
                                //change location
                                r.SetLocation(r.Left, box.ContentTop + diff);
                            }
                            else
                            {
                                //
                            }
                        }
                        else
                        {
                            r.SetLocation(r.Left, 0); //***
                        }

                        node = node.Next;//**
                    }
                    break;
                case VerticalAlignment.Middle:
                    {
                        //middle height of this line                        

                        while (node != null)
                        {
                            RenderElement r = node.Value;
                            if (r is CustomRenderBox box)
                            {
                                int diff = LineHeight - r.Height;
                                if (diff > 0)
                                {
                                    //change location
                                    r.SetLocation(r.Left, box.ContentTop + (diff / 2));
                                }
                                else
                                {
                                    //
                                }
                            }
                            else
                            {
                                r.SetLocation(r.Left, 0); //***
                            }

                            node = node.Next;//**
                        }
                    }
                    break;
                case VerticalAlignment.Top:
                    while (node != null)
                    {
                        RenderElement r = node.Value;
                        if (r is CustomRenderBox box)
                        {
                            r.SetLocation(r.Left, box.ContentTop);
                        }
                        else
                        {
                            r.SetLocation(r.Left, 0); //***
                        }

                        node = node.Next;//**
                    }
                    break;
                case VerticalAlignment.UserSpecific://TODO
                    break;
            }
        }
    }


}
