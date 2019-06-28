//MIT, 2019, WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using PixelFarm.TreeCollection;

namespace LayoutFarm.TextEditing
{
    public enum PlainTextLineEnd : byte
    {
        None,
        /// <summary>
        /// \r\n
        /// </summary>
        CRLF,
        /// <summary>
        /// \n
        /// </summary>
        LF,
    }

    public interface IRedNodeElem
    {
        int Compare(IRedNodeElem another);
    }

    public class PlainTextLineNode : IRedBlackTreeNode<PlainTextLineNode>
    {
        public PlainTextLineNode Parent { get; set; }
        public PlainTextLineNode Left { get; set; }
        public PlainTextLineNode Right { get; set; }
        public RedBlackColor Color { get; set; }

        public int CompareTo(object obj)
        {
            throw new System.NotImplementedException();
        }
        public int CompareTo(PlainTextLineNode another)
        {
            return this.LineNumber.CompareTo(another.LineNumber);
        }
        public void UpdateAugmentedData()
        {
        }

        PlainTextLine _textline;
        public PlainTextLineNode(PlainTextLine textline)
        {
            _textline = textline;
        }
        public PlainTextLine TextLine => _textline;
        public int LineNumber { get; internal set; }
#if DEBUG
        public override string ToString()
        {
            return _textline.ToString();
        }
#endif
    }

    /// <summary>
    /// immutable plain text line
    /// </summary>
    public class PlainTextLine
    {
        string _text;
        internal PlainTextLine()
        {
            _text = "";
        }
        internal PlainTextLine(string text)
        {
            _text = text;
        }
        public PlainTextLineEnd EndWith { get; set; }
        public PlainTextDocument OwnerDocument { get; set; }


        public string GetText() => _text;
        public void CopyText(StringBuilder stbuilder)
        {
            stbuilder.Append(_text);
        }
#if DEBUG
        public override string ToString()
        {
            return _text;
        }
#endif

        public PlainTextLine Clone()
        {
            return new PlainTextLine(_text);
        }
    }


    public class PlainTextDocument
    {

        RedBlackTree<PlainTextLineNode> _lines = new RedBlackTree<PlainTextLineNode>();
        public PlainTextDocument Clone()
        {
            PlainTextDocument newdoc = new PlainTextDocument();
            RedBlackTree<PlainTextLineNode> newlines = newdoc._lines;
            bool passFirstLine = false;
            PlainTextLineNode node = null;
            foreach (PlainTextLineNode line in _lines)
            {
                if (!passFirstLine)
                {
                    node = newdoc.AppendLine(line.TextLine.Clone());
                    passFirstLine = true;
                }
                else
                {
                    var plainTextLineNode = CreateLineNode(line.TextLine.Clone());
                    newlines.InsertAfter(node, plainTextLineNode);
                    node = plainTextLineNode;
                }
            }
            return newdoc;
        }

        /// <summary>
        /// append line
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        public PlainTextLineNode AppendLine(PlainTextLine line)
        {
            //append to last
            PlainTextLineNode lineNode = CreateLineNode(line);

            //add to last 
            //line num start at 0
            int lineCount = _lines.Count;
            lineNode.LineNumber = lineCount;
            _lines.AppendLast(lineNode);
            return lineNode;
        }

        static PlainTextLineNode CreateLineNode(PlainTextLine line)
        {
            return new PlainTextLineNode(line);
        }

        static PlainTextLineNode GetLine(PlainTextLineNode node, int lineNo)
        {
            //recursive


            if (node == null) return null;

            int node_lineNo = node.LineNumber;
            if (node_lineNo == lineNo)
            {
                //found
                return node;
            }
            else if (lineNo <= node_lineNo)
            {
                //get left side
                //recursive
                return GetLine(node.Left, lineNo);
            }
            else
            {
                //go rigt side
                //recursive
                return GetLine(node.Right, lineNo);
            }
        }

        public PlainTextLine GetLine(int lineNo) => GetLineNode(lineNo).TextLine;

        public PlainTextLineNode GetLineNode(int lineNo) => GetLine(_lines.Root, lineNo);

        public int LineCount => _lines.Count;

        public IEnumerable<PlainTextLine> GetLineIter()
        {
            foreach (var line in _lines)
            {
                yield return line.TextLine;
            }
        }

        public PlainTextLine AppendLine(string line)
        {
            //append to last
            PlainTextLine textline = new PlainTextLine(line);
            //add this
            AppendLine(textline);
            return textline;
        }

        public void Remove(int removeLineNo)
        {
            var removeLineNode = GetLineNode(removeLineNo);
            if (removeLineNode == null) return;
            //
            //update line number after remove

            var nextNode = removeLineNode.GetNextNode();
            _lines.Remove(removeLineNode);

            //change line number
            while (nextNode != null)
            {
                nextNode.LineNumber = removeLineNo;
                nextNode = nextNode.GetNextNode();
                removeLineNo++;
            }
        }
        public PlainTextLine Insert(int insertAtLineNo, string line)
        {
            //...
            var insertAtTextLineNode = GetLineNode(insertAtLineNo);
            if (insertAtTextLineNode == null)
            {
                //line no found
                return null;
            }

            //
            PlainTextLine textline = new PlainTextLine(line);
            PlainTextLineNode newnode = CreateLineNode(textline);
            // 
            _lines.InsertBefore(insertAtTextLineNode, newnode);
            //after insert then we update all line number
            newnode.LineNumber = insertAtLineNo; //set line
            //get next node
            PlainTextLineNode node = newnode.GetNextNode();
            while (node != null)
            {
                ++insertAtLineNo;
                node.LineNumber = insertAtLineNo;
                node = node.GetNextNode();
            }
            return textline;
        }
        public void CopyAllText(StringBuilder stbuilder)
        {
            bool passFirstLine = false;
            foreach (var line in _lines)
            {
                if (passFirstLine)
                {
                    stbuilder.AppendLine();
                }
                line.TextLine.CopyText(stbuilder);
                passFirstLine = true;
            }
        }
    }
}
