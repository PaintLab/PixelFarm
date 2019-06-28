//Modified from ...
// 
// SegmentTree.cs
//  
// Author:
//       mkrueger <mkrueger@novell.com>
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

//from namespace MonoDevelop.Ide.Editor
namespace PixelFarm.TreeCollection
{

    /// <summary>
    /// Provides immutable empty list instances.
    /// </summary>
    static class Empty<T>
    {
        public static readonly T[] Array = new T[0];
    }

    /// <summary>
    /// A segment tree contains overlapping segments and get all segments overlapping a segment.
    /// It's implemented as a augmented interval tree
    /// described in Cormen et al. (2001, Section 14.3: Interval trees, pp. 311–317).
    /// </summary>
    public class SegmentTree<T> : ITextSegmentTree, ICollection<T> where T : TreeSegment
    {
        readonly RedBlackTree<TreeSegment> _tree = new RedBlackTree<TreeSegment>();

        public SegmentTree()
        {

        }
        public int Count => _tree.Count;

        public IEnumerator<T> GetEnumerator()
        {
            var root = _tree.Root;
            if (root == null)
                yield break;
            var node = root.GetOuterLeft();
            while (node != null)
            {
                yield return (T)node;
                node = node.GetNextNode();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            foreach (T tt in this)
            {
                if (tt.Equals(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Debug.Assert(array != null);
            Debug.Assert(0 <= arrayIndex && arrayIndex < array.Length);
            int i = arrayIndex;
            foreach (T value in this)
                array[i++] = value;
        }

        bool ICollection<T>.IsReadOnly => false;


        public void Add(T item)
        {
            InternalAdd(item);
        }

        public bool Remove(T item)
        {
            return InternalRemove(item);
        }

        public void Clear()
        {
            _tree.Clear();
        }

        public IEnumerable<T> GetSegmentsAt(int offset)
        {
            return GetSegmentsOverlapping(offset, 0);
        }

        public IEnumerable<T> GetSegmentsOverlapping(ISegment segment)
        {
            if (segment.Offset < 0)
                return Empty<T>.Array;
            return GetSegmentsOverlapping(segment.Offset, segment.Length);
        }

        public IEnumerable<T> GetSegmentsOverlapping(int offset, int length)
        {
            if (_tree.Root == null)
                yield break;
            var intervalStack = new Interval(null, _tree.Root, offset, offset + length);
            while (intervalStack != null)
            {
                var interval = intervalStack;
                intervalStack = intervalStack.tail;
                if (interval.end < 0)
                    continue;

                var node = interval.node;
                int nodeStart = interval.start - node.DistanceToPrevNode;
                int nodeEnd = interval.end - node.DistanceToPrevNode;
                var leftNode = node.Left;
                if (leftNode != null)
                {
                    nodeStart -= leftNode.TotalLength;
                    nodeEnd -= leftNode.TotalLength;
                }

                if (node.DistanceToMaxEnd < nodeStart)
                    continue;

                if (leftNode != null)
                    intervalStack = new Interval(intervalStack, leftNode, interval.start, interval.end);

                if (nodeEnd < 0)
                    continue;

                if (nodeStart <= node.Length)
                    yield return (T)node;

                var rightNode = node.Right;
                if (rightNode != null)
                    intervalStack = new Interval(intervalStack, rightNode, nodeStart, nodeEnd);
            }
        }

        //public void InstallListener(ITextDocument doc)
        //{
        //    if (ownerDocument != null)
        //        throw new InvalidOperationException("Segment tree already installed");
        //    ownerDocument = doc;
        //    doc.TextChanged += UpdateOnTextReplace;
        //}

        //public void RemoveListener()
        //{
        //    if (ownerDocument == null)
        //        throw new InvalidOperationException("Segment tree is not installed");
        //    ownerDocument.TextChanged -= UpdateOnTextReplace;
        //    ownerDocument = null;
        //}

        //internal void UpdateOnTextReplace(object sender, TextChangeEventArgs e)
        //{
        //    for (int i = 0; i < e.TextChanges.Count; ++i)
        //    {
        //        var change = e.TextChanges[i];
        //        if (change.RemovalLength == 0)
        //        {
        //            var length = change.InsertionLength;
        //            foreach (var segment in GetSegmentsAt(change.Offset))
        //            //.Where(s => s.Offset < change.Offset && change.Offset < s.EndOffset))
        //            {
        //                if (segment.Offset < change.Offset && change.Offset < segment.EndOffset)
        //                {
        //                    segment.Length += length;
        //                    segment.UpdateAugmentedData();
        //                }
        //            }
        //            var node = SearchFirstSegmentWithStartAfter(change.Offset + 1);
        //            if (node != null)
        //            {
        //                node.DistanceToPrevNode += length;
        //                node.UpdateAugmentedData();
        //            }
        //            continue;
        //        }
        //        int delta = change.ChangeDelta;
        //        foreach (var segment in new List<T>(GetSegmentsOverlapping(change.Offset, change.RemovalLength)))
        //        {
        //            if (segment.Offset < change.Offset)
        //            {
        //                if (segment.EndOffset >= change.Offset + change.RemovalLength)
        //                {
        //                    segment.Length += delta;
        //                }
        //                else
        //                {
        //                    segment.Length = change.Offset - segment.Offset;
        //                }
        //                segment.UpdateAugmentedData();
        //                continue;
        //            }
        //            int remainingLength = segment.EndOffset - (change.Offset + change.RemovalLength);
        //            InternalRemove(segment);
        //            if (remainingLength > 0)
        //            {
        //                segment.Offset = change.Offset + change.RemovalLength;
        //                segment.Length = remainingLength;
        //                InternalAdd(segment);
        //            }
        //        }
        //        var next = SearchFirstSegmentWithStartAfter(change.Offset + 1);

        //        if (next != null)
        //        {
        //            next.DistanceToPrevNode += delta;
        //            next.UpdateAugmentedData();
        //        }
        //    }
        //}

        void InternalAdd(TreeSegment node)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            if (node._segmentTree != null)
                throw new InvalidOperationException("Node already attached.");

            node._segmentTree = this;


            int insertionOffset = node.Offset;
            node.DistanceToMaxEnd = node.Length;

            if (_tree.Root == null)
            {
                _tree.Count = 1;
                _tree.Root = (T)node;
                node.TotalLength = node.DistanceToPrevNode;
                return;
            }

            if (insertionOffset < _tree.Root.TotalLength)
            {
                var n = SearchNode(ref insertionOffset);
                node.TotalLength = node.DistanceToPrevNode = insertionOffset;
                n.DistanceToPrevNode -= insertionOffset;
                _tree.InsertBefore(n, node);
                return;
            }

            node.DistanceToPrevNode = node.TotalLength = insertionOffset - _tree.Root.TotalLength;
            _tree.InsertRight(_tree.Root.GetOuterRight(), node);
        }

        bool InternalRemove(TreeSegment node)
        {
            if (node._segmentTree == null)
                return false;
            if (node._segmentTree != this)
                throw new InvalidOperationException("Tried to remove tree segment from wrong tree.");
            var calculatedOffset = node.Offset;
            var next = node.GetNextNode();
            if (next != null)
                next.DistanceToPrevNode += node.DistanceToPrevNode;
            _tree.Remove(node);
            if (next != null)
                next.UpdateAugmentedData();
            node._segmentTree = null;
            node.Parent = node.Left = node.Right = null;
            node.DistanceToPrevNode = calculatedOffset;
            return true;
        }

        TreeSegment SearchFirstSegmentWithStartAfter(int startOffset)
        {
            if (_tree.Root == null)
                return null;
            if (startOffset <= 0)
                return _tree.Root.GetOuterLeft();
            var result = SearchNode(ref startOffset);
            while (startOffset == 0)
            {
                var pre = result == null ? _tree.Root.GetOuterRight() : result.GetPrevNode();
                if (pre == null)
                    return null;
                startOffset += pre.DistanceToPrevNode;
                result = pre;
            }
            return result;
        }

        TreeSegment SearchNode(ref int offset)
        {
            TreeSegment n = _tree.Root;
            while (true)
            {
                if (n.Left != null)
                {
                    if (offset < n.Left.TotalLength)
                    {
                        n = n.Left;
                        continue;
                    }
                    offset -= n.Left.TotalLength;
                }
                if (offset < n.DistanceToPrevNode)
                    return n;
                offset -= n.DistanceToPrevNode;
                if (n.Right == null)
                    return null;
                n = n.Right;
            }
        }

        #region TextSegmentTree implementation

        void ITextSegmentTree.Add(TreeSegment segment)
        {
            InternalAdd(segment);
        }

        bool ITextSegmentTree.Remove(TreeSegment segment)
        {
            return InternalRemove(segment);
        }

        #endregion



        class Interval
        {
            internal Interval tail;
            internal TreeSegment node;
            internal int start, end;

            public Interval(Interval tail, TreeSegment node, int start, int end)
            {
                this.tail = tail;
                this.node = node;
                this.start = start;
                this.end = end;
            }

            public override string ToString()
            {
                return string.Format("[Interval: start={0},end={1}]", start, end);
            }
        }
    }

    interface ITextSegmentTree
    {
        void Add(TreeSegment segment);
        bool Remove(TreeSegment segment);
    }

    public class TreeSegment : ISegment, IRedBlackTreeNode<TreeSegment>
    {

        public TreeSegment(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }

        public TreeSegment(ISegment segment) : this(segment.Offset, segment.Length)
        {
        }
        public int Offset
        {
            get
            {
                if (_segmentTree == null)
                    return DistanceToPrevNode;

                var curNode = this;
                int offset = curNode.DistanceToPrevNode;
                if (curNode.Left != null)
                    offset += curNode.Left.TotalLength;
                while (curNode.Parent != null)
                {
                    if (curNode == curNode.Parent.Right)
                    {
                        if (curNode.Parent.Left != null)
                            offset += curNode.Parent.Left.TotalLength;
                        offset += curNode.Parent.DistanceToPrevNode;
                    }
                    curNode = curNode.Parent;
                }
                return offset;
            }
            set
            {
                if (_segmentTree != null)
                    _segmentTree.Remove(this);
                DistanceToPrevNode = value;
                if (_segmentTree != null)
                    _segmentTree.Add(this);
            }
        }

        public int Length { get; private set; }

        public int EndOffset => Offset + Length;


        //Internal API

        internal ITextSegmentTree _segmentTree;
        public TreeSegment Parent { get; set; }
        public TreeSegment Left { get; set; }
        public TreeSegment Right { get; set; }
        public RedBlackColor Color { get; set; }
        public int CompareTo(object another)
        {
            throw new NotSupportedException();
        }
        public int CompareTo(TreeSegment another)
        {
            throw new NotSupportedException();
        }

        // TotalLength = DistanceToPrevNode + Left.DistanceToPrevNode + Right.DistanceToPrevNode
        internal int TotalLength;

        internal int DistanceToPrevNode;

        // DistanceToMaxEnd = Max (Length, left.DistanceToMaxEnd + Max (left.Offset, right.Offset) - Offset)
        internal int DistanceToMaxEnd;

        public void UpdateAugmentedData()
        {
            int totalLength = DistanceToPrevNode;
            int distanceToMaxEnd = Length;

            TreeSegment left = Left;
            if (left != null)
            {
                totalLength += left.TotalLength;
                int leftdistance = left.DistanceToMaxEnd - DistanceToPrevNode;
                var leftRight = left.Right;
                if (leftRight != null)
                    leftdistance -= leftRight.TotalLength;
                if (leftdistance > distanceToMaxEnd)
                    distanceToMaxEnd = leftdistance;
            }

            TreeSegment right = Right;
            if (right != null)
            {
                totalLength += right.TotalLength;
                int rightdistance = right.DistanceToMaxEnd + right.DistanceToPrevNode;
                TreeSegment rightLeft = right.Left;
                if (rightLeft != null)
                    rightdistance += rightLeft.TotalLength;
                if (rightdistance > distanceToMaxEnd)
                    distanceToMaxEnd = rightdistance;
            }

            if (TotalLength != totalLength || DistanceToMaxEnd != distanceToMaxEnd)
            {
                TotalLength = totalLength;
                DistanceToMaxEnd = distanceToMaxEnd;
                Parent?.UpdateAugmentedData();
            }
        }
    }
}