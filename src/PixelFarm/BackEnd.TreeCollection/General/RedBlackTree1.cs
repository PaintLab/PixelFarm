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

    sealed class RedBlackTree1<T> where T : TreeSegment
    {
        const bool RED = true;
        const bool BLACK = false;

        public T Root { get; set; }

        public void InsertBefore(TreeSegment node, TreeSegment newNode)
        {
            if (node.Left == null)
            {
                InsertLeft(node, newNode);
            }
            else
            {
                InsertRight(node.Left.OuterRight, newNode);
            }
        }

        public void InsertLeft(TreeSegment parentNode, TreeSegment newNode)
        {
            parentNode.Left = newNode;
            newNode.Parent = parentNode;
            newNode.Color = RED;
            parentNode.UpdateAugmentedData();
            FixTreeOnInsert(newNode);
            Count++;
        }

        public void InsertRight(TreeSegment parentNode, TreeSegment newNode)
        {
            parentNode.Right = newNode;
            newNode.Parent = parentNode;
            newNode.Color = RED;
            parentNode.UpdateAugmentedData();
            FixTreeOnInsert(newNode);
            Count++;
        }

        void FixTreeOnInsert(TreeSegment node)
        {
            var parent = node.Parent;
            if (parent == null)
            {
                node.Color = BLACK;
                return;
            }

            if (parent.Color == BLACK)
                return;
            var uncle = node.Uncle;
            TreeSegment grandParent = parent.Parent;

            if (uncle != null && uncle.Color == RED)
            {
                parent.Color = BLACK;
                uncle.Color = BLACK;
                grandParent.Color = RED;
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

            parent.Color = BLACK;
            grandParent.Color = RED;
            if (node == parent.Left && parent == grandParent.Left)
            {
                RotateRight(grandParent);
            }
            else
            {
                RotateLeft(grandParent);
            }
        }

        void RotateLeft(TreeSegment node)
        {
            TreeSegment right = node.Right;
            Replace(node, right);
            node.Right = right.Left;
            if (node.Right != null)
                node.Right.Parent = node;
            right.Left = node;
            node.Parent = right;
            node.UpdateAugmentedData();
            node.Parent.UpdateAugmentedData();
        }

        void RotateRight(TreeSegment node)
        {
            TreeSegment left = node.Left;
            Replace(node, left);
            node.Left = left.Right;
            if (node.Left != null)
                node.Left.Parent = node;
            left.Right = node;
            node.Parent = left;
            node.UpdateAugmentedData();
            node.Parent.UpdateAugmentedData();
        }

        void Replace(TreeSegment oldNode, TreeSegment newNode)
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

        public void Remove(TreeSegment node)
        {
            if (node.Left != null && node.Right != null)
            {
                var outerLeft = node.Right.OuterLeft;
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
            InternalRemove(node);
        }

        void InternalRemove(TreeSegment node)
        {
            if (node.Left != null && node.Right != null)
            {
                var outerLeft = node.Right.OuterLeft;
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
            TreeSegment child = node.Left ?? node.Right;

            Replace(node, child);

            if (node.Color == BLACK && child != null)
            {
                if (child.Color == RED)
                {
                    child.Color = BLACK;
                }
                else
                {
                    DeleteOneChild(child);
                }
            }
        }

        static bool GetColorSafe(TreeSegment node)
        {
            return node != null ? node.Color : BLACK;
        }

        void DeleteOneChild(TreeSegment node)
        {
            // case 1
            if (node == null || node.Parent == null)
                return;

            var parent = node.Parent;
            var sibling = node.Sibling;
            if (sibling == null)
                return;

            // case 2
            if (sibling.Color == RED)
            {
                parent.Color = RED;
                sibling.Color = BLACK;
                if (node == parent.Left)
                {
                    RotateLeft(parent);
                }
                else
                {
                    RotateRight(parent);
                }
                sibling = node.Sibling;
                if (sibling == null)
                    return;
            }

            // case 3
            if (parent.Color == BLACK && sibling.Color == BLACK && GetColorSafe(sibling.Left) == BLACK && GetColorSafe(sibling.Right) == BLACK)
            {
                sibling.Color = RED;
                DeleteOneChild(parent);
                return;
            }

            // case 4
            if (parent.Color == RED && sibling.Color == BLACK && GetColorSafe(sibling.Left) == BLACK && GetColorSafe(sibling.Right) == BLACK)
            {
                sibling.Color = RED;
                parent.Color = BLACK;
                return;
            }

            // case 5
            if (node == parent.Left && sibling.Color == BLACK && GetColorSafe(sibling.Left) == RED && GetColorSafe(sibling.Right) == BLACK)
            {
                sibling.Color = RED;
                if (sibling.Left != null)
                    sibling.Left.Color = BLACK;
                RotateRight(sibling);
            }
            else if (node == parent.Right && sibling.Color == BLACK && GetColorSafe(sibling.Right) == RED && GetColorSafe(sibling.Left) == BLACK)
            {
                sibling.Color = RED;
                if (sibling.Right != null)
                    sibling.Right.Color = BLACK;
                RotateLeft(sibling);
            }

            // case 6
            sibling = node.Sibling;
            if (sibling == null)
                return;
            sibling.Color = parent.Color;
            parent.Color = BLACK;
            if (node == parent.Left)
            {
                if (sibling.Right != null)
                    sibling.Right.Color = BLACK;
                RotateLeft(parent);
            }
            else
            {
                if (sibling.Left != null)
                    sibling.Left.Color = BLACK;
                RotateRight(parent);
            }
        }

        public int Count { get; set; }

        public void Clear()
        {
            Root = null;
            Count = 0;
        }

        static string GetIndent(int level)
        {
            return new String('\t', level);
        }

        static void AppendNode(StringBuilder builder, TreeSegment node, int indent)
        {
            builder.Append(GetIndent(indent)).Append("Node (").Append((node.Color == RED ? "r" : "b")).Append("):").AppendLine(node.ToString());
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