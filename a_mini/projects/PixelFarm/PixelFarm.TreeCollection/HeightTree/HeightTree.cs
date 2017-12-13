// 
// HeightTree.cs
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
using System;
using System.Collections.Generic;


namespace PixelFarm.TreeCollection
{
    public class HashSet<T>
    {
        //for .NET 2.0
        Dictionary<T, bool> _dic = new Dictionary<T, bool>();
        public void Add(T data)
        {
            _dic[data] = true;
        }
        public bool Remove(T data)
        {
            return _dic.Remove(data);
        }
        public bool Contains(T data)
        {
            return _dic.ContainsKey(data);
        }
    }


    public interface IMultiLineDocument
    {
        /// <summary>
        /// general line height
        /// </summary>
        int LineHeight { get; }
        int LineCount { get; }
        event EventHandler<TextChangeEventArgs> TextChanged;
        event EventHandler FoldTreeUpdated;
        DocumentLocation OffsetToLocation(int offset);
        int OffsetToLineNumber(int offset);
        bool IsDisposed { get; }
        //List<TextLineMarker> extendingTextMarkers = new List<TextLineMarker>();
        //public IEnumerable<DocumentLine> LinesWithExtendingTextMarkers
        //{
        //    get
        //    {
        //        foreach (var marker in extendingTextMarkers)
        //        {
        //            var line = marker.LineSegment;
        //            if (line != null)
        //                yield return line;
        //        }
        //    }
        //}

        //public int OffsetToLineNumber(int offset)
        //{
        //    var snapshot = this.currentSnapshot;

        //    if (offset < 0 || offset > snapshot.Length)
        //        return 0;
        //    return snapshot.GetLineFromPosition(offset).LineNumber + 1;
        //}

        //public DocumentLocation OffsetToLocation(int offset)
        //{
        //    IDocumentLine line = this.GetLineByOffset(offset);
        //    if (line == null)
        //        return DocumentLocation.Empty;

        //    var col = System.Math.Max(1, System.Math.Min(line.LengthIncludingDelimiter, offset - line.Offset) + 1);
        //    return new DocumentLocation(line.LineNumber, col);
        //}

        //public int LocationToOffset(int line, int column)
        //{
        //    if (line > this.LineCount || line < DocumentLocation.MinLine)
        //        return -1;
        //    DocumentLine documentLine = GetLine(line);
        //    return System.Math.Min(Length, documentLine.Offset + System.Math.Max(0, System.Math.Min(documentLine.Length, column - 1)));
        //}
        //public int LocationToOffset(DocumentLocation location)
        //{
        //    return LocationToOffset(location.Line, location.Column);
        //}

        //public DocumentLocation OffsetToLocation(int offset)
        //{
        //    IDocumentLine line = this.GetLineByOffset(offset);
        //    if (line == null)
        //        return DocumentLocation.Empty;

        //    var col = System.Math.Max(1, System.Math.Min(line.LengthIncludingDelimiter, offset - line.Offset) + 1);
        //    return new DocumentLocation(line.LineNumber, col);
        //}
    }


    /// <summary>
    /// The height tree stores the heights of lines and provides a performant conversion between y and lineNumber.
    /// It takes care of message bubble heights and the height of folded sections.
    /// </summary>
    public class HeightTree : IDisposable
    {
        // TODO: Add support for line word wrap to the text editor - with the height tree this is possible.
        internal RedBlackTree<HeightNode> tree = new RedBlackTree<HeightNode>();
        readonly IMultiLineDocument _multiLineDoc;

        public double TotalHeight
        {
            get
            {
                return tree.Root.totalHeight;
            }
        }

        public int VisibleLineCount
        {
            get
            {
                return tree.Root.totalVisibleCount;
            }
        }

        public HeightTree(IMultiLineDocument editor)
        {
            this._multiLineDoc = editor;
            editor.TextChanged += Document_TextChanged;
            editor.FoldTreeUpdated += HandleFoldTreeUpdated;
            //this.editor.Document.TextChanged += Document_TextChanged; ;
            //this.editor.Document.FoldTreeUpdated += HandleFoldTreeUpdated;
        }


        void Document_TextChanged(object sender, TextChangeEventArgs e)
        {
            var oldHeight = TotalHeight;
            Rebuild();
            if ((int)oldHeight != (int)TotalHeight)
            {
                for (int i = 0; i < e.TextChanges.Count; ++i)
                {
                    var change = e.TextChanges[i];
                    var lineNumber = this._multiLineDoc.OffsetToLineNumber(change.NewOffset);
                    OnLineUpdateFrom(new HeightChangedEventArgs(lineNumber - 1));
                }
            }
        }


        public void Dispose()
        {
            _multiLineDoc.TextChanged -= Document_TextChanged;
            _multiLineDoc.FoldTreeUpdated -= HandleFoldTreeUpdated;
            //this.editor.Document.TextChanged -= Document_TextChanged; ;
            //this.editor.Document.FoldTreeUpdated -= HandleFoldTreeUpdated;
        }

        void HandleFoldTreeUpdated(object sender, EventArgs e)
        {
            //TODO: review here 
            //we ...
            Rebuild();
            //Application.Invoke((o, args) =>
            //{
            //  Rebuild();
            //});
        }

        void RemoveLine(int line)
        {
            lock (tree)
            {
                try
                {
                    var node = GetNodeByLine(line);
                    if (node == null)
                        return;
                    if (node.count == 1)
                    {
                        tree.Remove(node);
                        return;
                    }
                    node.count--;
                }
                finally
                {
                    OnLineUpdateFrom(new HeightChangedEventArgs(line - 1));
                }
            }
        }

        public event EventHandler<HeightChangedEventArgs> LineUpdateFrom;

        protected virtual void OnLineUpdateFrom(HeightChangedEventArgs e)
        {
            if (rebuild)
                return;
            var handler = this.LineUpdateFrom;
            if (handler != null)
                handler(this, e);
        }

        public class HeightChangedEventArgs : EventArgs
        {
            public int Line { get; set; }

            public HeightChangedEventArgs(int line)
            {
                Line = line;
            }
        }

        void InsertLine(int line)
        {
            lock (tree)
            {
                var newLine = new HeightNode()
                {
                    count = 1,
                    height = _multiLineDoc.LineHeight
                };

                try
                {
                    if (line == tree.Root.totalCount + 1)
                    {
                        tree.InsertAfter(tree.Root.GetOuterRight(), newLine);
                        return;
                    }
                    var node = GetNodeByLine(line);
                    if (node == null)
                        return;
                    if (node.count == 1)
                    {
                        tree.InsertBefore(node, newLine);
                        return;
                    }
                    node.count++;
                }
                finally
                {
                    newLine.UpdateAugmentedData();
                    OnLineUpdateFrom(new HeightChangedEventArgs(line));
                }
            }
        }

        bool rebuild;
        public void Rebuild()
        {
            lock (tree)
            {
                if (_multiLineDoc.IsDisposed)
                    return;
                rebuild = true;
                try
                {
                    //markers.Clear();
                    tree.Count = 1;
                    double h = _multiLineDoc.LineCount * _multiLineDoc.LineHeight;
                    tree.Root = new HeightNode()
                    {
                        height = h,
                        totalHeight = h,
                        totalCount = _multiLineDoc.LineCount,
                        totalVisibleCount = _multiLineDoc.LineCount,
                        count = _multiLineDoc.LineCount
                    };

                    //TODO: review
                    //foreach (var extendedTextMarkerLine in editor.Document.LinesWithExtendingTextMarkers)
                    //{
                    //    int lineNumber = extendedTextMarkerLine.LineNumber;
                    //    double height = editor.GetLineHeight(extendedTextMarkerLine);
                    //    SetLineHeight(lineNumber, height);
                    //}

                    //foreach (var segment in editor.Document.FoldedSegments.ToArray())
                    //{
                    //    int start = editor.OffsetToLineNumber(segment.Offset);
                    //    int end = editor.OffsetToLineNumber(segment.EndOffset);
                    //    segment.Marker = Fold(start, end - start);
                    //}
                }
                finally
                {
                    rebuild = false;
                }
            }
        }

        public void SetLineHeight(int lineNumber, double height)
        {
            lock (tree)
            {
                HeightNode node = GetNodeByLine(lineNumber);
                if (node == null)
                    throw new Exception("No node for line number " + lineNumber + " found. (maxLine=" + tree.Root.totalCount + ")");
                int nodeStartLine = node.GetLineNumber();
                int remainingLineCount;
                if (nodeStartLine == lineNumber)
                {
                    remainingLineCount = node.count - 1;
                    ChangeHeight(node, 1, height);
                    if (remainingLineCount > 0)
                    {
                        InsertAfter(node, new HeightNode()
                        {
                            count = remainingLineCount,
                            height = _multiLineDoc.LineHeight * remainingLineCount,
                            foldLevel = node.foldLevel
                        }
                        );
                    }
                }
                else
                {
                    int newLineCount = lineNumber - nodeStartLine;
                    remainingLineCount = node.count - newLineCount - 1;
                    if (newLineCount != node.count)
                    {
                        double newHeight = _multiLineDoc.LineHeight * newLineCount;
                        ChangeHeight(node, newLineCount, newHeight);
                    }

                    var newNode = new HeightNode()
                    {
                        count = 1,
                        height = height,
                        foldLevel = node.foldLevel
                    };
                    InsertAfter(node, newNode);

                    if (remainingLineCount > 0)
                    {
                        InsertAfter(newNode, new HeightNode()
                        {
                            count = remainingLineCount,
                            height = _multiLineDoc.LineHeight * remainingLineCount,
                            foldLevel = node.foldLevel
                        }
                        );
                    }
                }
            }
            OnLineUpdateFrom(new HeightChangedEventArgs(lineNumber));
        }

        public class FoldMarker
        {
            public readonly int Line;
            public readonly int Count;

            public FoldMarker(int line, int count)
            {
                this.Line = line;
                this.Count = count;
            }
        }

        readonly HashSet<FoldMarker> markers = new HashSet<FoldMarker>();

        public FoldMarker Fold(int lineNumber, int count)
        {
            lock (tree)
            {
                GetSingleLineNode(lineNumber);
                lineNumber++;

                for (int i = lineNumber; i < lineNumber + count; i++)
                {
                    var node = GetSingleLineNode(i);
                    node.foldLevel++;
                    node.UpdateAugmentedData();
                }
                var result = new FoldMarker(lineNumber, count);
                markers.Add(result);
                OnLineUpdateFrom(new HeightChangedEventArgs(lineNumber - 1));
                return result;
            }
        }

        public void Unfold(FoldMarker marker, int lineNumber, int count)
        {
            lock (tree)
            {
                if (marker == null || !markers.Contains(marker))
                    return;
                markers.Remove(marker);

                GetSingleLineNode(lineNumber);
                lineNumber++;
                for (int i = lineNumber; i < lineNumber + count; i++)
                {
                    var node = GetSingleLineNode(i);
                    node.foldLevel--;
                    node.UpdateAugmentedData();
                }
                OnLineUpdateFrom(new HeightChangedEventArgs(lineNumber - 1));
            }
        }

        public double LineNumberToY(int lineNumber)
        {
            int curLine = System.Math.Min(tree.Root.totalCount, lineNumber);
            if (curLine <= 0)
                return 0;
            lock (tree)
            {
                var node = GetSingleLineNode(curLine);
                int ln = curLine - 1;
                while (ln > 0 && node != null && node.foldLevel > 0)
                {
                    node = GetSingleLineNode(ln--);
                }
                if (ln == 0 || node == null)
                    return 0;
                double result = node.Left != null ? ((HeightNode)node.Left).totalHeight : 0;

                while (node.parent != null)
                {
                    if (node == node.parent.right)
                    {
                        if (node.parent.left != null)
                        {
                            result += node.parent.left.totalHeight;
                        }
                        if (node.parent.foldLevel == 0)
                        {
                            result += node.parent.height;
                        }
                    }
                    node = node.parent;
                }
                return result;
            }
        }

        public int YToLineNumber(double y)
        {
            lock (tree)
            {
                var node = GetNodeByY(y);
                if (node == null)
                    return y < 0 ? DocumentLocation.MinLine + (int)(y / _multiLineDoc.LineHeight) : tree.Root.totalCount + (int)((y - tree.Root.totalHeight) / _multiLineDoc.LineHeight);
                int lineOffset = 0;
                if (node.foldLevel == 0)
                {
                    double delta = y - node.GetY();
                    lineOffset = (int)(node.count * delta / node.height);
                }
                return node.GetLineNumber() + lineOffset;
            }
        }

        void InsertAfter(HeightNode node, HeightNode newNode)
        {
            lock (tree)
            {
                if (newNode.count <= 0)
                    throw new ArgumentOutOfRangeException("new node count <= 0.");
                tree.InsertAfter(node, newNode);
                newNode.UpdateAugmentedData();
            }
        }

        void InsertBefore(HeightNode node, HeightNode newNode)
        {
            lock (tree)
            {
                if (newNode.count <= 0)
                    throw new ArgumentOutOfRangeException("new node count <= 0.");
                tree.InsertBefore(node, newNode);
                newNode.UpdateAugmentedData();
            }
        }

        void ChangeHeight(HeightNode node, int newCount, double newHeight)
        {
            lock (tree)
            {
                node.count = newCount;
                node.height = newHeight;
                node.UpdateAugmentedData();
            }
        }

        int GetValidLine(int logicalLine)
        {
            if (logicalLine < DocumentLocation.MinLine)
                return DocumentLocation.MinLine;
            if (logicalLine > _multiLineDoc.LineCount)
                return _multiLineDoc.LineCount;
            return logicalLine;
        }

        public int LogicalToVisualLine(int logicalLine)
        {
            lock (tree)
            {
                if (logicalLine < DocumentLocation.MinLine)
                    return DocumentLocation.MinLine;
                if (logicalLine > tree.Root.totalCount)
                    return tree.Root.totalCount + logicalLine - tree.Root.totalCount;
                int line = GetValidLine(logicalLine);
                var node = GetNodeByLine(line);
                if (node == null)
                    return tree.Root.totalCount + logicalLine - tree.Root.totalCount;
                int delta = logicalLine - node.GetLineNumber();
                return node.GetVisibleLineNumber() + delta;
            }
        }

        int GetValidVisualLine(int logicalLine)
        {
            if (logicalLine < DocumentLocation.MinLine)
                return DocumentLocation.MinLine;
            if (logicalLine > VisibleLineCount)
                return VisibleLineCount;
            return logicalLine;
        }

        public int VisualToLogicalLine(int visualLineNumber)
        {
            lock (tree)
            {
                if (visualLineNumber < DocumentLocation.MinLine)
                    return DocumentLocation.MinLine;
                if (visualLineNumber > tree.Root.totalVisibleCount)
                    return tree.Root.totalCount + visualLineNumber - tree.Root.totalVisibleCount;
                int line = GetValidVisualLine(visualLineNumber);
                var node = GetNodeByVisibleLine(line);
                if (node == null)
                    return tree.Root.totalCount + visualLineNumber - tree.Root.totalVisibleCount;
                int delta = visualLineNumber - node.GetVisibleLineNumber();
                return node.GetLineNumber() + delta;
            }
        }

        HeightNode GetSingleLineNode(int lineNumber)
        {
            var node = GetNodeByLine(lineNumber);
            if (node == null || node.count == 1)
                return node;

            int nodeStartLine = node.GetLineNumber();
            int linesBefore = lineNumber - nodeStartLine;
            if (linesBefore > 0)
            {
                var splittedNode = new HeightNode();
                splittedNode.count = linesBefore;
                splittedNode.height = linesBefore * _multiLineDoc.LineHeight;
                splittedNode.foldLevel = node.foldLevel;
                if (splittedNode.count > 0)
                    InsertBefore(node, splittedNode);

                node.count -= linesBefore;
                node.height -= splittedNode.height;
                node.UpdateAugmentedData();

                if (node.count == 1)
                    return node;
            }

            InsertAfter(node, new HeightNode()
            {
                count = node.count - 1,
                height = (node.count - 1) * _multiLineDoc.LineHeight,
                foldLevel = node.foldLevel
            });

            node.count = 1;
            node.height = _multiLineDoc.LineHeight;
            node.UpdateAugmentedData();
            return node;
        }


        public HeightNode GetNodeByLine(int lineNumber)
        {
            lock (tree)
            {
                var node = tree.Root;
                int i = lineNumber - 1;
                while (true)
                {
                    if (node == null)
                        return null;
                    if (node.left != null && i < node.left.totalCount)
                    {
                        node = node.left;
                    }
                    else
                    {
                        if (node.left != null)
                            i -= node.left.totalCount;
                        i -= node.count;
                        if (i < 0)
                            return node;
                        node = node.right;
                    }
                }
            }
        }

        HeightNode GetNodeByVisibleLine(int lineNumber)
        {
            var node = tree.Root;
            int i = lineNumber - 1;
            while (true)
            {
                if (node == null)
                    return null;
                if (node.left != null && i < node.left.totalVisibleCount)
                {
                    node = node.left;
                }
                else
                {
                    if (node.left != null)
                        i -= node.left.totalVisibleCount;
                    if (node.foldLevel == 0)
                        i -= node.count;
                    if (i < 0)
                        return node;
                    node = node.right;
                }
            }
        }

        HeightNode GetNodeByY(double y)
        {
            var node = tree.Root;
            double h = y;
            while (true)
            {
                if (node == null)
                    return null;
                if (node.left != null && h < node.left.totalHeight)
                {
                    node = node.left;
                }
                else
                {
                    if (node.left != null)
                        h -= node.left.totalHeight;

                    if (node.foldLevel == 0)
                    {
                        h -= node.height;
                        if (h < 0)
                        {
                            return node;
                        }
                    }
                    /*
					} else {
						double deltaH = 0;
						var n = node.GetNextNode ();
						while (n != null && n.foldLevel > 0) {
							deltaH += n.height;
							n = n.GetNextNode ();
						}
					
						if (h - deltaH < 0) {
							return node;
						}
					}*/
                    node = node.right;
                }
            }
        }

        public class HeightNode : IRedBlackTreeNode
        {
            public double totalHeight;
            public double height;

            public int totalVisibleCount;
            public int totalCount;
            public int count = 1;

            public int foldLevel;

            public int GetLineNumber()
            {
                int lineNumber = left != null ? left.totalCount : 0;
                var node = this;
                while (node.parent != null)
                {
                    if (node == node.parent.right)
                    {
                        if (node.parent.left != null)
                            lineNumber += node.parent.left.totalCount;
                        lineNumber += node.parent.count;
                    }

                    node = node.parent;
                }
                return lineNumber + 1;
            }

            public int GetVisibleLineNumber()
            {
                int lineNumber = left != null ? left.totalVisibleCount : 0;
                var node = this;
                while (node.parent != null)
                {
                    if (node == node.parent.right)
                    {
                        if (node.parent.left != null)
                            lineNumber += node.parent.left.totalVisibleCount;
                        if (node.parent.foldLevel == 0)
                            lineNumber += node.parent.count;
                    }

                    node = node.parent;
                }
                return lineNumber + 1;
            }

            public double GetY()
            {
                double result = left != null ? left.totalHeight : 0;
                var node = this;
                while (node.parent != null)
                {
                    if (node == node.parent.right)
                    {
                        if (node.parent.left != null)
                            result += node.parent.left.totalHeight;
                        if (node.parent.foldLevel == 0)
                            result += node.parent.height;
                    }
                    node = node.parent;
                }
                return result;
            }

            public override string ToString()
            {
                return string.Format(GetLineNumber() + "[HeightNode: totalHeight={0}, height={1}, totalVisibleCount = {5}, totalCount={2}, count={3}, foldLevel={4}]", totalHeight, height, totalCount, count, foldLevel, totalVisibleCount);
            }

            #region IRedBlackTreeNode implementation
            public void UpdateAugmentedData()
            {
                double newHeight;
                int newCount = count;
                int newvisibleCount;

                if (foldLevel == 0)
                {
                    newHeight = height;
                    newvisibleCount = count;
                }
                else
                {
                    newvisibleCount = 0;
                    newHeight = 0;
                }

                if (left != null)
                {
                    newHeight += left.totalHeight;
                    newCount += left.totalCount;
                    newvisibleCount += left.totalVisibleCount;
                }

                if (right != null)
                {
                    newHeight += right.totalHeight;
                    newCount += right.totalCount;
                    newvisibleCount += right.totalVisibleCount;
                }

                if (newHeight != totalHeight || newCount != totalCount || newvisibleCount != totalVisibleCount)
                {
                    this.totalHeight = newHeight;
                    this.totalCount = newCount;
                    this.totalVisibleCount = newvisibleCount;

                    if (Parent != null)
                        Parent.UpdateAugmentedData();
                }
            }
            public HeightNode parent;
            public IRedBlackTreeNode Parent
            {
                get
                {
                    return parent;
                }
                set
                {
                    parent = (HeightNode)value;
                }
            }

            public HeightNode left;
            public IRedBlackTreeNode Left
            {
                get
                {
                    return left;
                }
                set
                {
                    left = (HeightNode)value;
                }
            }

            public HeightNode right;
            public IRedBlackTreeNode Right
            {
                get
                {
                    return right;
                }
                set
                {
                    right = (HeightNode)value;
                }
            }

            public RedBlackColor Color
            {
                get;
                set;
            }
            #endregion

        }

    }
}
