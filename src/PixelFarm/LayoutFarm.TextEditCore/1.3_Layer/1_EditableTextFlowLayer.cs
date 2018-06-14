//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;


namespace LayoutFarm.Text
{
    public interface HitChain
    {
        int TestPointX { get; }
        int TestPointY { get; }
    }
    public interface TextEditRenderBox
    {
        void NotifyTextContentSizeChanged();
        TextSurfaceEventListener TextSurfaceEventListener { get; }
        void SetTextSurfaceEventListner(TextSurfaceEventListener listener);
    }



    /// <summary>
    /// layer is a collection of line
    /// </summary>
    partial class EditableTextFlowLayer
    {

        protected int layerFlags;
        protected const int IS_LAYER_HIDDEN = 1 << (14 - 1);
        protected const int IS_GROUND_LAYER = 1 << (15 - 1);
        protected const int MAY_HAS_OTHER_OVERLAP_CHILD = 1 << (16 - 1);
        protected const int DOUBLE_BACKCANVAS_WIDTH = 1 << (18 - 1);
        protected const int DOUBLE_BACKCANVAS_HEIGHT = 1 << (19 - 1);
        protected const int CONTENT_DRAWING = 1 << (22 - 1);
        protected const int ARRANGEMENT_VALID = 1 << (23 - 1);
        protected const int HAS_CALCULATE_SIZE = 1 << (24 - 1);
        protected const int FLOWLAYER_HAS_MULTILINE = 1 << (25 - 1);


        object lineCollection;

        public EditableTextFlowLayer()
        {
            //start with single line per layer
            //and can be changed to multiline
            lineCollection = new EditableTextLine(this);
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public TextSpanStyle CurrentTextSpanStyle
        {
            get;
            set;
        }
        public bool FlowLayerHasMultiLines
        {
            get
            {
                return (layerFlags & FLOWLAYER_HAS_MULTILINE) != 0;
            }
            private set
            {
                if (value)
                {
                    layerFlags |= FLOWLAYER_HAS_MULTILINE;
                }
                else
                {
                    layerFlags &= ~FLOWLAYER_HAS_MULTILINE;
                }
            }
        }

        internal IEnumerable<EditableRun> GetDrawingIter(EditableRun start, EditableRun stop)
        {
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                int j = lines.Count;
                for (int i = 0; i < j; ++i)
                {
                    LinkedListNode<EditableRun> curNode = lines[i].Last;
                    while (curNode != null)
                    {
                        yield return curNode.Value;
                        curNode = curNode.Previous;
                    }
                }
            }
            else
            {
                EditableTextLine onlyLine = (EditableTextLine)lineCollection;
                LinkedListNode<EditableRun> curNode = onlyLine.Last;
                while (curNode != null)
                {
                    yield return curNode.Value;
                    curNode = curNode.Previous;
                }
            }
        }

        public int LineCount
        {
            get
            {
                if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
                {
                    return ((List<EditableTextLine>)lineCollection).Count;
                }
                else
                {
                    return 1;
                }
            }
        }

        public bool HitTestCore(HitChain hitChain)
        {
            if ((layerFlags & IS_LAYER_HIDDEN) == 0)
            {
                if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
                {
                    List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                    int j = lines.Count;
                    int testYPos = hitChain.TestPointY;
                    for (int i = 0; i < j; ++i)
                    {
                        EditableTextLine line = lines[i];
                        if (line.LineBottom < testYPos)
                        {
                            continue;
                        }
                        else if (line.HitTestCore(hitChain))
                        {
                            return true;
                        }
                        else if (line.LineTop > testYPos)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    EditableTextLine onlyLine = (EditableTextLine)lineCollection;
                    return onlyLine.HitTestCore(hitChain);
                }
            }
            return false;
        }

        static Size ReCalculateContentSizeHorizontalFlow(EditableTextFlowLayer layer)
        {
            if (layer.lineCollection == null)
            {
                return Size.Empty;
            }

            //only one line
            EditableTextLine line = (EditableTextLine)layer.lineCollection;
            LinkedListNode<EditableRun> c_node = line.First;
            //--------
            int curX = 0;

            int maxHeightInRow = 0;
            int maxWidth = 0;
            while (c_node != null)
            {
                EditableRun run = c_node.Value;
                int runHeight = run.Height;
                if (runHeight > maxHeightInRow)
                {
                    maxHeightInRow = runHeight;
                }
                curX += run.Width;
                if (curX > maxWidth)
                {
                    maxWidth = curX;
                }


                //next
                c_node = c_node.Next;
            }

            return new Size(maxWidth, maxHeightInRow);
        }

        public void TopDownReArrangeContent()
        {
            //vinv_IsInTopDownReArrangePhase = true;
            //#if DEBUG
            //            vinv_dbug_EnterLayerReArrangeContent(this);
            //#endif
            //            //this.BeginLayerLayoutUpdate();

            //            RenderBoxBase container = this.OwnerRenderElement as RenderBoxBase;
            //            if (container != null)
            //            {
            PerformHorizontalFlowArrange(0, this.Width, 0);
            //            }

            //            if (Reflow != null)
            //            {
            //                Reflow(this, EventArgs.Empty);
            //            }

            //            //this.EndLayerLayoutUpdate();
            //#if DEBUG
            //            vinv_dbug_ExitLayerReArrangeContent();
            //#endif
        }

        void vinv_dbug_ExitLayerReCalculateContent() { }
        void vinv_dbug_EnterLayerReCalculateContent(object o) { }
        public void TopDownReCalculateContentSize()
        {
#if DEBUG

            vinv_dbug_EnterLayerReCalculateContent(this);
#endif
            if (this.LineCount > 1)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)this.lineCollection;
                EditableTextLine lastline = lines[lines.Count - 1];
                SetPostCalculateLayerContentSize(lastline.ActualLineWidth, lastline.ActualLineHeight + lastline.LineTop);
            }
            else
            {
                //re-calculate content size 
                //of a single line
                SetPostCalculateLayerContentSize(ReCalculateContentSizeHorizontalFlow(this));
            }
#if DEBUG
            vinv_dbug_ExitLayerReCalculateContent();
#endif
        }

       
        void SetPostCalculateLayerContentSize(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        void SetPostCalculateLayerContentSize(Size s)
        {
            this.Width = s.Width;
            this.Height = s.Height;
        }
        internal EditableTextLine GetTextLine(int lineId)
        {
            List<EditableTextLine> lines = lineCollection as List<EditableTextLine>;
            if (lines != null)
            {
                if (lineId < lines.Count)
                {
                    return lines[lineId];
                }
            }
            else if (lineId == 0)
            {
                return (EditableTextLine)lineCollection;
            }

            return null;
        }



        internal EditableTextLine GetTextLineAtPos(int y)
        {
            if (lineCollection != null)
            {
                if (lineCollection is List<EditableTextLine>)
                {
                    List<EditableTextLine> lines = lineCollection as List<EditableTextLine>;
                    if (lines != null)
                    {
                        int j = lines.Count;
                        for (int i = 0; i < j; ++i)
                        {
                            EditableTextLine line = lines[i];
                            if (line.IntersectsWith(y))
                            {
                                return line;
                            }
                        }
                    }
                }
                else
                {
                    EditableTextLine line = (EditableTextLine)lineCollection;
                    if (line.IntersectsWith(y))
                    {
                        return line;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// append to last
        /// </summary>
        /// <param name="line"></param>
        void AppendLine(EditableTextLine line)
        {
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                int lineCount = lines.Count;
                EditableTextLine lastLine = lines[lineCount - 1];
                line.SetLineNumber(lineCount);
                line.SetTop(lastLine.Top + lastLine.ActualLineHeight);
                lines.Add(line);
            }
            else
            {
                EditableTextLine onlyLine = (EditableTextLine)lineCollection;
                List<EditableTextLine> newLineList = new List<EditableTextLine>();
                newLineList.Add(onlyLine);
                line.SetTop(onlyLine.ActualLineHeight);
                line.SetLineNumber(1);
                newLineList.Add(line);
                lineCollection = newLineList;
                FlowLayerHasMultiLines = true;
            }
        }
        void vinv_dbug_EndSetElementBound()
        {

        }
        void vinv_dbug_EndSetElementBound(object o)
        {

        }
        void vinv_dbug_BeginSetElementBound(object o) { }

        void PerformHorizontalFlowArrangeForMultilineText(
            int ownerClientLeft, int ownerClientWidth,
            int ownerClientTop)
        {
#if DEBUG
            long startTick = DateTime.Now.Ticks;
#endif

            List<EditableTextLine> lines = (List<EditableTextLine>)this.lineCollection;
            int ownerClientRight = ownerClientLeft + ownerClientWidth;
            int curX = 0;
            int curY = 0;
            bool lastestIsBlock = false;
            int maxWidth = 0;
            int curY_fromTop = ownerClientTop;
            int maxHeightInRow = EditableTextLine.DEFAULT_LINE_HEIGHT;
            int lineCount = lines.Count;
            for (int i = 0; i < lineCount; ++i)
            {
                EditableTextLine line = lines[i];
                curX = ownerClientLeft;
                lastestIsBlock = false;
                line.SetTop(curY_fromTop);
                if (!line.NeedArrange)
                {
                    maxHeightInRow = line.ActualLineHeight;
                    if (line.ActualLineWidth > maxWidth)
                    {
                        maxWidth = line.ActualLineWidth;
                    }
                }
                else
                {
                    maxHeightInRow = EditableTextLine.DEFAULT_LINE_HEIGHT;
                    EditableTextLine newLine = null;
                    line.ValidateContentArrangement();
                    bool isFirstRunInThisLine = true;
                    foreach (EditableRun currentRun in line.GetTextRunIter())
                    {
#if DEBUG
                        vinv_dbug_BeginSetElementBound(currentRun);
#endif
                        int v_desired_width = currentRun.Width;
                        int v_desired_height = currentRun.Height;
                        if (isFirstRunInThisLine)
                        {
                            lastestIsBlock = currentRun.IsInlineBlockElement;
                            if (v_desired_height > maxHeightInRow)
                            {
                                maxHeightInRow = v_desired_height;
                            }

                            currentRun.SetLocation(curX, 0);
                            if (v_desired_height > maxHeightInRow)
                            {
                                maxHeightInRow = v_desired_height;
                            }
                            if (lastestIsBlock)
                            {
                                v_desired_width = ownerClientWidth;
                            }

                            currentRun.SetSize(v_desired_width, v_desired_height);
                            currentRun.MarkValidContentArrangement();
                            curX += v_desired_width;
                            isFirstRunInThisLine = false;
                        }
                        else
                        {
                            if (lastestIsBlock || currentRun.IsInlineBlockElement ||
                            (curX + v_desired_width > ownerClientRight))
                            {
                                newLine = new EditableTextLine(this);
                                newLine.AddLast(currentRun);
                                curY = curY_fromTop + maxHeightInRow;
                                curY_fromTop = curY;
                                maxHeightInRow = EditableTextLine.DEFAULT_LINE_HEIGHT;
                                EditableRun nextR = currentRun.NextTextRun;
                                while (nextR != null)
                                {
                                    line.UnsafeRemoveVisualElement(nextR);
                                    newLine.AddLast(nextR);
                                    nextR = nextR.NextTextRun;
                                }
                                if (i + 1 == lineCount)
                                {
                                    lines.Add(newLine);
                                }
                                else
                                {
                                    lines.Insert(i + 1, newLine);
                                }
                                lineCount++;
                                break;
                            }
                            else
                            {
                                lastestIsBlock = currentRun.IsInlineBlockElement;
                                if (v_desired_height > maxHeightInRow)
                                {
                                    maxHeightInRow = v_desired_height;
                                } 
                                currentRun.SetLocation(curX, 0);
                                currentRun.SetSize(v_desired_width, v_desired_height); 

                                currentRun.MarkValidContentArrangement();
                                curX += v_desired_width;
                            }
                        }

#if DEBUG
                        vinv_dbug_EndSetElementBound(currentRun);
#endif

                    }
                    if (curX > maxWidth)
                    {
                        maxWidth = curX;
                    }
                }
                line.SetPostArrangeLineSize(maxWidth, maxHeightInRow);
                curY = curY_fromTop + maxHeightInRow;
                curY_fromTop = curY;
            }
            ValidateArrangement();
        }
        void ValidateArrangement()
        {

        }
        void PerformHorizontalFlowArrange(
            int ownerClientLeft,
            int ownerClientWidth,
            int ownerClientTop)
        {
            if (lineCollection == null)
            {
                return;
            }
            //
            //if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            //{
            //go multi line mode
            PerformHorizontalFlowArrangeForMultilineText(
                ownerClientLeft,
                ownerClientWidth,
                ownerClientTop);
            return;

        }


        internal EditableTextLine InsertNewLine(int insertAt)
        {
            EditableTextLine newLine = new EditableTextLine(this);
            this.InsertLine(insertAt, newLine);
            return newLine;
        }
        void InsertLine(int insertAt, EditableTextLine textLine)
        {
            if (insertAt < 0)
            {
                throw new NotSupportedException();
            }

            List<EditableTextLine> lines = lineCollection as List<EditableTextLine>;
            if (lines != null)
            {
                int j = lines.Count;
                if (insertAt >= j)
                {
                    AppendLine(textLine);
                }
                else
                {
                    EditableTextLine line = lines[insertAt];
                    int cy = line.Top;
                    textLine.SetTop(cy);
                    textLine.SetLineNumber(insertAt);
                    cy += line.ActualLineHeight;
                    for (int i = insertAt; i < j; i++)
                    {
                        line = lines[i];
                        line.SetTop(cy);
                        line.SetLineNumber(i + 1);
                        cy += line.ActualLineHeight;
                    }
                    textLine.editableFlowLayer = this;
                    lines.Insert(insertAt, textLine);
                }
            }
            else
            {
                lines = new List<EditableTextLine>();
                lines.Add((EditableTextLine)lineCollection);
                lineCollection = lines;
                FlowLayerHasMultiLines = true;
                int j = lines.Count;
                if (insertAt >= j)
                {
                    //append last
                    AppendLine(textLine);
                }
                else
                {
                    EditableTextLine line = lines[insertAt];
                    int cy = line.Top;
                    textLine.SetTop(cy);
                    textLine.SetLineNumber(insertAt);
                    cy += line.ActualLineHeight;
                    for (int i = insertAt; i < j; i++)
                    {
                        line = lines[i];
                        line.SetTop(cy);
                        line.SetLineNumber(i + 1);
                        cy += line.ActualLineHeight;
                    }
                    textLine.editableFlowLayer = this;
                    lines.Insert(insertAt, textLine);
                }
            }
        }



        public void CopyContentToStringBuilder(StringBuilder stBuilder)
        {
            List<EditableTextLine> lines = lineCollection as List<EditableTextLine>;
            if (lines != null)
            {
                int j = lines.Count;
                int n = j - 1;
                for (int i = 0; i < j; ++i)
                {
                    lines[i].CopyLineContent(stBuilder);
                    if (i < n)
                    {
                        stBuilder.Append('\n');
                    }
                }
            }
            else
            {
                ((EditableTextLine)lineCollection).CopyLineContent(stBuilder);
            }
        }

        internal IEnumerable<EditableRun> TextRunForward(EditableRun startRun, EditableRun stopRun)
        {
            EditableTextLine currentLine = startRun.OwnerEditableLine;
            EditableTextLine stopLine = stopRun.OwnerEditableLine;
            if (currentLine == stopLine)
            {
                foreach (EditableRun r in currentLine.GetVisualElementForward(startRun, stopRun))
                {
                    yield return r;
                }
            }
            else
            {
                foreach (EditableRun r in currentLine.GetVisualElementForward(startRun))
                {
                    yield return r;
                }
                currentLine = currentLine.Next;
                while (currentLine != null)
                {
                    if (currentLine == stopLine)
                    {
                        foreach (EditableRun r in currentLine.GetTextRunIter())
                        {
                            if (r == stopRun)
                            {
                                break;
                            }
                            else
                            {
                                yield return r;
                            }
                        }
                        break;
                    }
                    else
                    {
                        foreach (EditableRun r in currentLine.GetTextRunIter())
                        {
                            yield return r;
                        }
                        currentLine = currentLine.Next;
                    }
                }
            }
        }
        internal void Reload(IEnumerable<EditableRun> runs)
        {
            Clear();
            foreach (EditableRun run in runs)
            {
                AddTop(run);
            }
        }

#if DEBUG
        //void debug_RecordLineInfo(RenderBoxBase owner, EditableTextLine line)
        //{
        //    RootGraphic visualroot = this.dbugVRoot;
        //    if (visualroot.dbug_RecordDrawingChain)
        //    {
        //    }
        //}

        //public override void dbug_DumpElementProps(dbugLayoutMsgWriter writer)
        //{
        //    writer.Add(new dbugLayoutMsg(
        //        this, this.ToString()));
        //    writer.EnterNewLevel();
        //    foreach (EditableRun child in this.dbugGetDrawingIter2())
        //    {
        //        child.dbug_DumpVisualProps(writer);
        //    }
        //    writer.LeaveCurrentLevel();
        //}
        //public override string ToString()
        //{
        //    return "editable flow layer " + "(L" + dbug_layer_id + this.dbugLayerState + ") postcal:" +
        //        this.PostCalculateContentSize.ToString() + " of " + this.OwnerRenderElement.dbug_FullElementDescription();
        //}
        public IEnumerable<EditableRun> dbugGetDrawingIter2()
        {
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                int j = lines.Count;
                for (int i = 0; i < j; ++i)
                {
                    LinkedListNode<EditableRun> curNode = lines[i].First;
                    while (curNode != null)
                    {
                        yield return curNode.Value;
                        curNode = curNode.Next;
                    }
                }
            }
            else
            {
                EditableTextLine onlyLine = (EditableTextLine)lineCollection;
                LinkedListNode<EditableRun> curNode = onlyLine.First;
                while (curNode != null)
                {
                    yield return curNode.Value;
                    curNode = curNode.Next;
                }
            }
        }

#endif
    }
}