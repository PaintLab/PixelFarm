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
    public class HeightTree<T> : IDisposable
    {
        // TODO: Add support for line word wrap to the text editor - with the height tree this is possible.
        RedBlackTree<HeightNode<T>> _tree = new RedBlackTree<HeightNode<T>>();
        public event EventHandler<HeightChangedEventArgs> LineUpdateFrom;

        readonly IMultiLineDocument _multiLineDoc;

        public double TotalHeight => _tree.Root._totalHeight;

        public int VisibleLineCount => _tree.Root._totalVisibleCount;

        public HeightTree(IMultiLineDocument editor)
        {
            _multiLineDoc = editor;

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
                    var lineNumber = _multiLineDoc.OffsetToLineNumber(change.NewOffset);
                    OnLineUpdateFrom(lineNumber - 1);
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
            lock (_tree)
            {
                try
                {
                    HeightNode<T> node = GetNodeByLine(line);
                    if (node == null)
                        return;
                    if (node._count == 1)
                    {
                        _tree.Remove(node);
                        return;
                    }
                    node._count--;
                }
                finally
                {
                    OnLineUpdateFrom(line - 1);
                }
            }
        }



        protected virtual void OnLineUpdateFrom(int line)
        {
            if (_rebuild)
                return;
            if (LineUpdateFrom != null)
            {
                LineUpdateFrom.Invoke(this, new HeightChangedEventArgs(line));
            }
        }



        void InsertLine(int line)
        {
            lock (_tree)
            {
                var newLine = new HeightNode<T>()
                {
                    _count = 1,
                    _height = _multiLineDoc.LineHeight
                };

                try
                {
                    if (line == _tree.Root._totalCount + 1)
                    {
                        _tree.InsertAfter(_tree.Root.GetOuterRight(), newLine);
                        return;
                    }
                    var node = GetNodeByLine(line);
                    if (node == null)
                        return;
                    if (node._count == 1)
                    {
                        _tree.InsertBefore(node, newLine);
                        return;
                    }
                    node._count++;
                }
                finally
                {
                    newLine.UpdateAugmentedData();
                    OnLineUpdateFrom(line);
                }
            }
        }

        bool _rebuild;
        public void Rebuild()
        {
            lock (_tree)
            {
                if (_multiLineDoc.IsDisposed)
                    return;
                _rebuild = true;
                try
                {
                    //markers.Clear();
                    _tree.Count = 1;

                    //assume same line height
                    int h = _multiLineDoc.LineCount * _multiLineDoc.LineHeight;
                    _tree.Root = new HeightNode<T>()
                    {
                        _height = h,
                        _totalHeight = h,
                        _totalCount = _multiLineDoc.LineCount,
                        _totalVisibleCount = _multiLineDoc.LineCount,
                        _count = _multiLineDoc.LineCount
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
                    _rebuild = false;
                }
            }
        }

        public void SetLineHeight(int lineNumber, int height)
        {
            lock (_tree)
            {
                HeightNode<T> node = GetNodeByLine(lineNumber);
                if (node == null)
                    throw new Exception("No node for line number " + lineNumber + " found. (maxLine=" + _tree.Root._totalCount + ")");
                int nodeStartLine = node.GetLineNumber();
                int remainingLineCount;
                if (nodeStartLine == lineNumber)
                {
                    remainingLineCount = node._count - 1;
                    ChangeHeight(node, 1, height);
                    if (remainingLineCount > 0)
                    {
                        InsertAfter(node, new HeightNode<T>()
                        {
                            _count = remainingLineCount,
                            _height = _multiLineDoc.LineHeight * remainingLineCount,
                            _foldLevel = node._foldLevel
                        }
                        );
                    }
                }
                else
                {
                    int newLineCount = lineNumber - nodeStartLine;
                    remainingLineCount = node._count - newLineCount - 1;
                    if (newLineCount != node._count)
                    {
                        int newHeight = _multiLineDoc.LineHeight * newLineCount;
                        ChangeHeight(node, newLineCount, newHeight);
                    }

                    var newNode = new HeightNode<T>()
                    {
                        _count = 1,
                        _height = height,
                        _foldLevel = node._foldLevel
                    };
                    InsertAfter(node, newNode);

                    if (remainingLineCount > 0)
                    {
                        InsertAfter(newNode, new HeightNode<T>()
                        {
                            _count = remainingLineCount,
                            _height = _multiLineDoc.LineHeight * remainingLineCount,
                            _foldLevel = node._foldLevel
                        }
                        );
                    }
                }
            }
            OnLineUpdateFrom(lineNumber);
        }


        public class HeightChangedEventArgs : EventArgs
        {
            public readonly int Line;
            public HeightChangedEventArgs(int line)
            {
                Line = line;
            }
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

        readonly HashSet<FoldMarker> _markers = new HashSet<FoldMarker>();

        public FoldMarker Fold(int lineNumber, int count)
        {
            lock (_tree)
            {
                GetSingleLineNode(lineNumber);
                lineNumber++;

                for (int i = lineNumber; i < lineNumber + count; i++)
                {
                    var node = GetSingleLineNode(i);
                    node._foldLevel++;
                    node.UpdateAugmentedData();
                }
                var result = new FoldMarker(lineNumber, count);
                _markers.Add(result);
                OnLineUpdateFrom(lineNumber - 1);
                return result;
            }
        }

        public void Unfold(FoldMarker marker, int lineNumber, int count)
        {
            lock (_tree)
            {
                if (marker == null || !_markers.Contains(marker))
                    return;
                _markers.Remove(marker);

                GetSingleLineNode(lineNumber);
                lineNumber++;
                for (int i = lineNumber; i < lineNumber + count; i++)
                {
                    var node = GetSingleLineNode(i);
                    node._foldLevel--;
                    node.UpdateAugmentedData();
                }
                OnLineUpdateFrom(lineNumber - 1);
            }
        }

        public double LineNumberToY(int lineNumber)
        {
            int curLine = System.Math.Min(_tree.Root._totalCount, lineNumber);
            if (curLine <= 0)
                return 0;
            lock (_tree)
            {
                HeightNode<T> node = GetSingleLineNode(curLine);
                int ln = curLine - 1;
                while (ln > 0 && node != null && node._foldLevel > 0)
                {
                    node = GetSingleLineNode(ln--);
                }
                if (ln == 0 || node == null)
                    return 0;
                double result = node.Left != null ? node.Left._totalHeight : 0;

                while (node.parent != null)
                {
                    if (node == node.parent.right)
                    {
                        if (node.parent.left != null)
                        {
                            result += node.parent.left._totalHeight;
                        }
                        if (node.parent._foldLevel == 0)
                        {
                            result += node.parent._height;
                        }
                    }
                    node = node.parent;
                }
                return result;
            }
        }

        public int YToLineNumber(double y)
        {
            lock (_tree)
            {
                var node = GetNodeByY(y);
                if (node == null)
                    return y < 0 ? DocumentLocation.MIN_LINE + (int)(y / _multiLineDoc.LineHeight) : _tree.Root._totalCount + (int)((y - _tree.Root._totalHeight) / _multiLineDoc.LineHeight);
                int lineOffset = 0;
                if (node._foldLevel == 0)
                {
                    double delta = y - node.GetY();
                    lineOffset = (int)(node._count * delta / node._height);
                }
                return node.GetLineNumber() + lineOffset;
            }
        }

        void InsertAfter(HeightNode<T> node, HeightNode<T> newNode)
        {
            lock (_tree)
            {
                if (newNode._count <= 0)
                    throw new ArgumentOutOfRangeException("new node count <= 0.");
                _tree.InsertAfter(node, newNode);
                newNode.UpdateAugmentedData();
            }
        }

        void InsertBefore(HeightNode<T> node, HeightNode<T> newNode)
        {
            lock (_tree)
            {
                if (newNode._count <= 0)
                    throw new ArgumentOutOfRangeException("new node count <= 0.");
                _tree.InsertBefore(node, newNode);
                newNode.UpdateAugmentedData();
            }
        }

        void ChangeHeight(HeightNode<T> node, int newCount, int newHeight)
        {
            lock (_tree)
            {
                node._count = newCount;
                node._height = newHeight;
                node.UpdateAugmentedData();
            }
        }

        int GetValidLine(int logicalLine)
        {
            if (logicalLine < DocumentLocation.MIN_LINE)
                return DocumentLocation.MIN_LINE;
            if (logicalLine > _multiLineDoc.LineCount)
                return _multiLineDoc.LineCount;
            return logicalLine;
        }

        public int LogicalToVisualLine(int logicalLine)
        {
            lock (_tree)
            {
                if (logicalLine < DocumentLocation.MIN_LINE)
                    return DocumentLocation.MIN_LINE;
                if (logicalLine > _tree.Root._totalCount)
                    return _tree.Root._totalCount + logicalLine - _tree.Root._totalCount;
                int line = GetValidLine(logicalLine);
                var node = GetNodeByLine(line);
                if (node == null)
                    return _tree.Root._totalCount + logicalLine - _tree.Root._totalCount;
                int delta = logicalLine - node.GetLineNumber();
                return node.GetVisibleLineNumber() + delta;
            }
        }

        int GetValidVisualLine(int logicalLine)
        {
            if (logicalLine < DocumentLocation.MIN_LINE)
                return DocumentLocation.MIN_LINE;
            if (logicalLine > VisibleLineCount)
                return VisibleLineCount;
            return logicalLine;
        }

        public int VisualToLogicalLine(int visualLineNumber)
        {
            lock (_tree)
            {
                if (visualLineNumber < DocumentLocation.MIN_LINE)
                    return DocumentLocation.MIN_LINE;
                if (visualLineNumber > _tree.Root._totalVisibleCount)
                    return _tree.Root._totalCount + visualLineNumber - _tree.Root._totalVisibleCount;
                int line = GetValidVisualLine(visualLineNumber);
                var node = GetNodeByVisibleLine(line);
                if (node == null)
                    return _tree.Root._totalCount + visualLineNumber - _tree.Root._totalVisibleCount;
                int delta = visualLineNumber - node.GetVisibleLineNumber();
                return node.GetLineNumber() + delta;
            }
        }

        HeightNode<T> GetSingleLineNode(int lineNumber)
        {
            var node = GetNodeByLine(lineNumber);
            if (node == null || node._count == 1)
                return node;

            int nodeStartLine = node.GetLineNumber();
            int linesBefore = lineNumber - nodeStartLine;
            if (linesBefore > 0)
            {
                var splittedNode = new HeightNode<T>();
                splittedNode._count = linesBefore;
                splittedNode._height = linesBefore * _multiLineDoc.LineHeight;
                splittedNode._foldLevel = node._foldLevel;
                if (splittedNode._count > 0)
                    InsertBefore(node, splittedNode);

                node._count -= linesBefore;
                node._height -= splittedNode._height;
                node.UpdateAugmentedData();

                if (node._count == 1)
                    return node;
            }

            InsertAfter(node, new HeightNode<T>()
            {
                _count = node._count - 1,
                _height = (node._count - 1) * _multiLineDoc.LineHeight,
                _foldLevel = node._foldLevel
            });

            node._count = 1;
            node._height = _multiLineDoc.LineHeight;
            node.UpdateAugmentedData();
            return node;
        }


        public HeightNode<T> GetNodeByLine(int lineNumber)
        {
            lock (_tree)
            {
                var node = _tree.Root;
                int i = lineNumber - 1;
                while (true)
                {
                    if (node == null)
                        return null;
                    if (node.left != null && i < node.left._totalCount)
                    {
                        node = node.left;
                    }
                    else
                    {
                        if (node.left != null)
                            i -= node.left._totalCount;
                        i -= node._count;
                        if (i < 0)
                            return node;
                        node = node.right;
                    }
                }
            }
        }

        HeightNode<T> GetNodeByVisibleLine(int lineNumber)
        {
            var node = _tree.Root;
            int i = lineNumber - 1;
            while (true)
            {
                if (node == null)
                    return null;
                if (node.left != null && i < node.left._totalVisibleCount)
                {
                    node = node.left;
                }
                else
                {
                    if (node.left != null)
                        i -= node.left._totalVisibleCount;
                    if (node._foldLevel == 0)
                        i -= node._count;
                    if (i < 0)
                        return node;
                    node = node.right;
                }
            }
        }

        HeightNode<T> GetNodeByY(double y)
        {
            HeightNode<T> node = _tree.Root;
            double h = y;
            while (true)
            {
                if (node == null)
                    return null;
                if (node.left != null && h < node.left._totalHeight)
                {
                    node = node.left;
                }
                else
                {
                    if (node.left != null)
                        h -= node.left._totalHeight;

                    if (node._foldLevel == 0)
                    {
                        h -= node._height;
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
    }



    public class HeightNode<T> : IRedBlackTreeNode<HeightNode<T>>
    {
        internal int _totalHeight;
        internal int _height;

        internal int _totalVisibleCount;
        internal int _totalCount;
        internal int _count = 1;

        internal int _foldLevel;

        public HeightNode()
        {
        }
        public T Data { get; set; }
        public int GetLineNumber()
        {
            int lineNumber = left != null ? left._totalCount : 0;
            var node = this;
            while (node.parent != null)
            {
                if (node == node.parent.right)
                {
                    if (node.parent.left != null)
                        lineNumber += node.parent.left._totalCount;
                    lineNumber += node.parent._count;
                }

                node = node.parent;
            }
            return lineNumber + 1;
        }

        public int Height => _height;
        public int GetY()
        {
            int result = left != null ? left._totalHeight : 0;
            var node = this;
            while (node.parent != null)
            {
                if (node == node.parent.right)
                {
                    if (node.parent.left != null)
                        result += node.parent.left._totalHeight;
                    if (node.parent._foldLevel == 0)
                        result += node.parent._height;
                }
                node = node.parent;
            }
            return result;
        }
        public int GetVisibleLineNumber()
        {
            int lineNumber = left != null ? left._totalVisibleCount : 0;
            var node = this;
            while (node.parent != null)
            {
                if (node == node.parent.right)
                {
                    if (node.parent.left != null)
                        lineNumber += node.parent.left._totalVisibleCount;
                    if (node.parent._foldLevel == 0)
                        lineNumber += node.parent._count;
                }

                node = node.parent;
            }
            return lineNumber + 1;
        }

        public override string ToString()
        {
            return string.Format(GetLineNumber() + "[HeightNode: totalHeight={0}, height={1}, totalVisibleCount = {5}, totalCount={2}, count={3}, foldLevel={4}]", _totalHeight, _height, _totalCount, _count, _foldLevel, _totalVisibleCount);
        }

        #region IRedBlackTreeNode implementation
        public void UpdateAugmentedData()
        {
            int newHeight;
            int newCount = _count;
            int newvisibleCount;

            if (_foldLevel == 0)
            {
                newHeight = _height;
                newvisibleCount = _count;
            }
            else
            {
                newvisibleCount = 0;
                newHeight = 0;
            }

            if (left != null)
            {
                newHeight += left._totalHeight;
                newCount += left._totalCount;
                newvisibleCount += left._totalVisibleCount;
            }

            if (right != null)
            {
                newHeight += right._totalHeight;
                newCount += right._totalCount;
                newvisibleCount += right._totalVisibleCount;
            }

            if (newHeight != _totalHeight || newCount != _totalCount || newvisibleCount != _totalVisibleCount)
            {
                this._totalHeight = newHeight;
                this._totalCount = newCount;
                this._totalVisibleCount = newvisibleCount;

                if (Parent != null)
                    Parent.UpdateAugmentedData();
            }
        }


        internal HeightNode<T> parent;
        public HeightNode<T> Parent
        {
            get => parent;
            set => parent = value;
        }

        internal HeightNode<T> left;
        public HeightNode<T> Left
        {
            get => left;
            set => left = value;
        }

        internal HeightNode<T> right;
        public HeightNode<T> Right
        {
            get => right;
            set => right = value;
        }

        public RedBlackColor Color { get; set; }

        public int CompareTo(object another)
        {
            throw new NotSupportedException();
        }

        #endregion

    }
}
