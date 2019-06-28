// RedBlackTree.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

//------------------------------
//MIT, 2019, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

//from namespace Mono.TextEditor.Utils
namespace PixelFarm.TreeCollection
{
    public enum RedBlackColor : byte
    {
        Black = 0,
        Red = 1
    }


    public interface IRedBlackTreeNode<T> : IComparable
    {
        T Parent { get; set; }
        T Left { get; set; }
        T Right { get; set; }
        int CompareTo(T another);
        RedBlackColor Color { get; set; }
        void UpdateAugmentedData();
    }

    public interface IRedBlackNodeCompare<T>
    {
        int GetCompareValue(T another);
    }

    public static class RedBlackTreeExtensionMethods
    {
        public static bool IsLeaf<T>(this IRedBlackTreeNode<T> node)
        {
            return node.Left == null && node.Right == null;
        }

        public static T GetSibling<T>(this T node) where T : class, IRedBlackTreeNode<T>
        {
            if (node.Parent == null)
                return null;
            return (T)(node == node.Parent.Left ? node.Parent.Right : node.Parent.Left);
        }

        public static T GetOuterLeft<T>(this T node) where T : class, IRedBlackTreeNode<T>
        {
            IRedBlackTreeNode<T> result = node;
            while (result.Left != null)
                result = result.Left;
            return (T)result;
        }

        public static T GetOuterRight<T>(this T node) where T : class, IRedBlackTreeNode<T>
        {
            IRedBlackTreeNode<T> result = node;
            while (result.Right != null)
            {
                result = result.Right;
            }
            return (T)result;
        }

        public static T GetGrandparent<T>(this T node) where T : class, IRedBlackTreeNode<T>
        {
            return (T)(node.Parent != null ? node.Parent.Parent : null);
        }

        public static T GetUncle<T>(this T node) where T : class, IRedBlackTreeNode<T>
        {
            IRedBlackTreeNode<T> grandparent = node.GetGrandparent();
            if (grandparent == null)
                return null;
            return (T)(node.Parent == grandparent.Left ? grandparent.Right : grandparent.Left);
        }

        public static T GetNextNode<T>(this T node) where T : class, IRedBlackTreeNode<T>
        {
            if (node.Right == null)
            {
                IRedBlackTreeNode<T> curNode = node;
                IRedBlackTreeNode<T> oldNode;
                do
                {
                    oldNode = curNode;
                    curNode = curNode.Parent;
                } while (curNode != null && curNode.Right == oldNode);
                return (T)curNode;
            }
            return (T)node.Right.GetOuterLeft();
        }

        public static T GetPrevNode<T>(this T node) where T : class, IRedBlackTreeNode<T>
        {
            if (node.Left == null)
            {
                IRedBlackTreeNode<T> curNode = node;
                IRedBlackTreeNode<T> oldNode;
                do
                {
                    oldNode = curNode;
                    curNode = curNode.Parent;
                } while (curNode != null && curNode.Left == oldNode);
                return (T)curNode;
            }
            return (T)node.Left.GetOuterRight();
        }

        public static void AppendLast<T>(this RedBlackTree<T> tree, T node) where T : class, IRedBlackTreeNode<T>
        {
            if (tree.Root == null)
            {
                tree.Root = node;
                tree.Count = 1;
            }
            else
            {
                T rightMost = tree.Root.GetOuterRight();
                tree.InsertAfter(rightMost, node);
            }
        }
    }

    public class RedBlackTree<T> : ICollection<T> where T : class, IRedBlackTreeNode<T>, IComparable
    {

        public RedBlackTree()
        {

        }
        public T Root { get; set; }

        bool ICollection<T>.Remove(T node)
        {
            Remove(node);
            return true;
        }

        public void Add(T node)
        {
            if (Root == null)
            {
                Count = 1;
                Root = node;
                FixTreeOnInsert(node);
                return;
            }

            T parent = Root;

            while (true)
            {
                //use Generic version CompareTo()
                if (parent.CompareTo(node) <= 0)
                {
                    if (parent.Left == null)
                    {
                        InsertLeft(parent, node);
                        break;
                    }
                    parent = parent.Left;
                }
                else
                {
                    if (parent.Right == null)
                    {
                        InsertRight(parent, node);
                        break;
                    }
                    parent = parent.Right;
                }
            }
        }

        public void InsertBefore(T node, T newNode)
        {
            if (node.Left == null)
            {
                InsertLeft(node, newNode);
            }
            else
            {
                InsertRight(node.Left.GetOuterRight(), newNode);
            }
        }

        public void InsertAfter(T node, T newNode)
        {
            if (node.Right == null)
            {
                InsertRight(node, newNode);
            }
            else
            {
                InsertLeft(node.Right.GetOuterLeft(), newNode);
            }
        }

        public void InsertLeft(T parentNode, T newNode)
        {
            parentNode.Left = newNode;
            newNode.Parent = parentNode;
            newNode.Color = RedBlackColor.Red;
            parentNode.UpdateAugmentedData();
            FixTreeOnInsert(newNode);
            Count++;
        }

        public void InsertRight(T parentNode, T newNode)
        {
            parentNode.Right = newNode;
            newNode.Parent = parentNode;
            newNode.Color = RedBlackColor.Red;
            parentNode.UpdateAugmentedData();
            FixTreeOnInsert(newNode);
            Count++;
        }

        void FixTreeOnInsert(T node)
        {
            T parent = node.Parent;
            if (parent == null)
            {
                node.Color = RedBlackColor.Black;
                return;
            }

            if (parent.Color == RedBlackColor.Black)
                return;

            T uncle = node.GetUncle();
            T grandParent = parent.Parent;

            if (uncle != null && uncle.Color == RedBlackColor.Red)
            {
                parent.Color = RedBlackColor.Black;
                uncle.Color = RedBlackColor.Black;
                grandParent.Color = RedBlackColor.Red;
                FixTreeOnInsert(grandParent);
                return;
            }

            if (node == parent.Right && parent == grandParent.Left)
            {
                RotateLeft(parent);
                node = node.Left;
            }
            else if (node == parent.Left && parent == grandParent.Right)
            {
                RotateRight(parent);
                node = node.Right;
            }

            parent = node.Parent;
            grandParent = parent.Parent;

            parent.Color = RedBlackColor.Black;
            grandParent.Color = RedBlackColor.Red;
            if (node == parent.Left && parent == grandParent.Left)
            {
                RotateRight(grandParent);
            }
            else
            {
                RotateLeft(grandParent);
            }
        }

        void RotateLeft(T node)
        {
            T right = node.Right;
            Replace(node, right);
            node.Right = right.Left;
            if (node.Right != null)
                node.Right.Parent = node;
            right.Left = node;
            node.Parent = right;
            node.UpdateAugmentedData();
            node.Parent.UpdateAugmentedData();
        }

        void RotateRight(T node)
        {
            T left = node.Left;
            Replace(node, left);
            node.Left = left.Right;
            if (node.Left != null)
                node.Left.Parent = node;
            left.Right = node;
            node.Parent = left;
            node.UpdateAugmentedData();
            node.Parent.UpdateAugmentedData();
        }

        void Replace(T oldNode, T newNode)
        {
            if (newNode != null)
                newNode.Parent = oldNode.Parent;
            if (oldNode.Parent == null)
            {
                Root = (T)newNode;
            }
            else
            {
                if (oldNode.Parent.Left == oldNode)
                    oldNode.Parent.Left = newNode;
                else
                    oldNode.Parent.Right = newNode;
                oldNode.Parent.UpdateAugmentedData();
            }
        }

        public void Remove(T node)
        {
            if (node.Left != null && node.Right != null)
            {
                T outerLeft = node.Right.GetOuterLeft();
                InternalRemove(outerLeft);
                Replace(node, outerLeft);

                outerLeft.Color = node.Color;
                outerLeft.Left = node.Left;
                if (outerLeft.Left != null)
                    outerLeft.Left.Parent = outerLeft;

                outerLeft.Right = node.Right;
                if (outerLeft.Right != null)
                    outerLeft.Right.Parent = outerLeft;
                outerLeft.UpdateAugmentedData();
                OnNodeRemoved(node);
                return;
            }
            InternalRemove(node);
            OnNodeRemoved(node);
        }

        void InternalRemove(T node)
        {
            if (node.Left != null && node.Right != null)
            {
                T outerLeft = node.Right.GetOuterLeft();
                InternalRemove(outerLeft);
                Replace(node, outerLeft);

                outerLeft.Color = node.Color;
                outerLeft.Left = node.Left;
                if (outerLeft.Left != null)
                    outerLeft.Left.Parent = outerLeft;

                outerLeft.Right = node.Right;
                if (outerLeft.Right != null)
                    outerLeft.Right.Parent = outerLeft;
                outerLeft.UpdateAugmentedData();
                return;
            }
            Count--;
            // node has only one child
            T child = node.Left ?? node.Right;

            Replace(node, child);

            if (node.Color == RedBlackColor.Black && child != null)
            {
                if (child.Color == RedBlackColor.Red)
                {
                    child.Color = RedBlackColor.Black;
                }
                else
                {
                    DeleteOneChild(child);
                }
            }
        }

        protected virtual void OnNodeRemoved(T removedNode)
        {
            if (this.NodeRemoved != null)
            {
                NodeRemoved(this, new RedBlackTreeNodeEventArgs(removedNode));
            }
        }

        public event EventHandler<RedBlackTreeNodeEventArgs> NodeRemoved;

        static RedBlackColor GetColorSafe(T node)
        {
            return node != null ? node.Color : RedBlackColor.Black;
        }

        void DeleteOneChild(T node)
        {
            // case 1
            if (node == null || node.Parent == null)
                return;

            var parent = node.Parent;
            var sibling = node.GetSibling();
            if (sibling == null)
                return;

            // case 2
            if (sibling.Color == RedBlackColor.Red)
            {
                parent.Color = RedBlackColor.Red;
                sibling.Color = RedBlackColor.Black;
                if (node == parent.Left)
                {
                    RotateLeft(parent);
                }
                else
                {
                    RotateRight(parent);
                }
                sibling = node.GetSibling();
                if (sibling == null)
                    return;
            }

            // case 3
            if (parent.Color == RedBlackColor.Black && sibling.Color == RedBlackColor.Black && GetColorSafe(sibling.Left) == RedBlackColor.Black && GetColorSafe(sibling.Right) == RedBlackColor.Black)
            {
                sibling.Color = RedBlackColor.Red;
                DeleteOneChild(parent);
                return;
            }

            // case 4
            if (parent.Color == RedBlackColor.Red && sibling.Color == RedBlackColor.Black && GetColorSafe(sibling.Left) == RedBlackColor.Black && GetColorSafe(sibling.Right) == RedBlackColor.Black)
            {
                sibling.Color = RedBlackColor.Red;
                parent.Color = RedBlackColor.Black;
                return;
            }

            // case 5
            if (node == parent.Left && sibling.Color == RedBlackColor.Black && GetColorSafe(sibling.Left) == RedBlackColor.Red && GetColorSafe(sibling.Right) == RedBlackColor.Black)
            {
                sibling.Color = RedBlackColor.Red;
                if (sibling.Left != null)
                    sibling.Left.Color = RedBlackColor.Black;
                RotateRight(sibling);
            }
            else if (node == parent.Right && sibling.Color == RedBlackColor.Black && GetColorSafe(sibling.Right) == RedBlackColor.Red && GetColorSafe(sibling.Left) == RedBlackColor.Black)
            {
                sibling.Color = RedBlackColor.Red;
                if (sibling.Right != null)
                    sibling.Right.Color = RedBlackColor.Black;
                RotateLeft(sibling);
            }

            // case 6
            sibling = node.GetSibling();
            if (sibling == null)
                return;
            sibling.Color = parent.Color;
            parent.Color = RedBlackColor.Black;
            if (node == parent.Left)
            {
                if (sibling.Right != null)
                    sibling.Right.Color = RedBlackColor.Black;
                RotateLeft(parent);
            }
            else
            {
                if (sibling.Left != null)
                    sibling.Left.Color = RedBlackColor.Black;
                RotateRight(parent);
            }
        }

        #region ICollection<T> implementation
        public int Count { get; internal set; }

        public void Clear()
        {
            Root = null;
            Count = 0;
        }

        public bool Contains(T item)
        {
            foreach (T ii in this)
            {
                if (ii.Equals(item))
                {
                    return true;
                }
            }
            return false;
            //return this.Any(i => item.Equals(i));
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (Root == null)
                yield break;
            var node = Root.GetOuterLeft();
            while (node != null)
            {
                yield return node;
                node = node.GetNextNode();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (Root == null)
                yield break;
            var node = Root.GetOuterLeft();
            while (node != null)
            {
                yield return node;
                node = node.GetNextNode();
            }
        }

        public bool IsReadOnly => false;

        public void CopyTo(T[] array, int arrayIndex)
        {
            Debug.Assert(array != null);
            Debug.Assert(0 <= arrayIndex && arrayIndex < array.Length);
            int i = arrayIndex;
            foreach (T value in this)
                array[i++] = value;
        }

        #endregion

        public class RedBlackTreeNodeEventArgs : EventArgs
        {
            public T Node { get; private set; }

            public RedBlackTreeNodeEventArgs(T node)
            {
                Node = node;
            }
        }

        static string GetIndent(int level)
        {
            return new String('\t', level);
        }

        static void AppendNode(StringBuilder builder, T node, int indent)
        {
            builder.Append(GetIndent(indent)).Append("Node (").Append((node.Color == RedBlackColor.Red ? "r" : "b")).Append("):").AppendLine(node.ToString());
            builder.Append(GetIndent(indent)).Append("Left: ");
            if (node.Left != null)
            {
                builder.Append(Environment.NewLine);
                AppendNode(builder, node.Left, indent + 1);
            }
            else
            {
                builder.Append("null");
            }

            builder.Append(Environment.NewLine);
            builder.Append(GetIndent(indent)).Append("Right: ");
            if (node.Right != null)
            {
                builder.Append(Environment.NewLine);
                AppendNode(builder, node.Right, indent + 1);
            }
            else
            {
                builder.Append("null");
            }
        }

        public override string ToString()
        {
            if (Root == null)
                return "<null>";
            var result = new StringBuilder();
            AppendNode(result, Root, 0);
            return result.ToString();
        }
    }
}
