//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.TextEditing
{
    public class EditableRunVisitor
    {
        public EditableRunVisitor()
        {

        }
        public Point CurrentCaretPos { get; set; }
        public bool StopOnNextLine { get; set; }
        public bool SkipCurrentLineEditableRunIter { get; set; }
        public Rectangle UpdateArea { get; set; }
        public bool UseUpdateArea { get; set; }
        public bool SkipMarkerLayer { get; set; }
        public bool SkipSelectionLayer { get; set; }

        public virtual void OnBeginTextLayer() { }
        public virtual void OnEndTextLayer() { }
        public virtual void VisitNewLine(int lineTop) { }
        public virtual void VisitEditableRun(EditableRun run) { }
        //
        //
        public virtual void OnBeginSelectionBG() { }
        public virtual void OnEndSelectionBG() { }

        public virtual void OnBeginMarkerLayer() { }
        public virtual void OnEndMarkerLayer() { }
        public virtual void VisitMarker(VisualMarkerSelectionRange markerRange) { }

        public virtual void VisitSelectionRange(VisualSelectionRange selRange) { }
    }

    class EditableTextFlowLayer
    {
        public event EventHandler Reflow; //TODO: review this field
        public event EventHandler ContentSizeChanged;

        List<EditableTextLine> _lines = new List<EditableTextLine>();
        RenderBoxBase _owner;

        public EditableTextFlowLayer(RenderBoxBase owner, TextSpanStyle defaultSpanStyle)
        {
            _owner = owner;
            RootGraphic = _owner.Root;
            //start with single line per layer
            //and can be changed to multiline
            DefaultSpanStyle = defaultSpanStyle;
            //add default lines
            _lines.Add(new EditableTextLine(this));
        }
        public RenderBoxBase Owner => _owner;
        public RootGraphic RootGraphic { get; set; }
        public int DefaultLineHeight => DefaultSpanStyle.ReqFont.LineSpacingInPixels;

        public TextSpanStyle DefaultSpanStyle { get; set; }

        internal void NotifyContentSizeChanged() => ContentSizeChanged?.Invoke(this, EventArgs.Empty);

        public EditableRun LatestHitRun { get; set; }

        public void SetUseDoubleCanvas(bool useWithWidth, bool useWithHeight)
        {
            //this.SetDoubleCanvas(useWithWidth, useWithHeight);
        }

        public bool FlowLayerHasMultiLines => _lines.Count > 1;


        internal IEnumerable<EditableRun> GetDrawingIter(EditableRun start, EditableRun stop)
        {
            List<EditableTextLine> lines = _lines;
            int j = lines.Count;
            for (int i = 0; i < j; ++i)
            {
                LinkedListNode<EditableRun> curNode = lines[i].Last;
                bool enableIter = false;
                while (curNode != null)
                {
                    EditableRun editableRun = curNode.Value;
                    if (editableRun == stop)
                    {
                        //found stop
                        enableIter = true;
                        yield return editableRun;

                        if (stop == start)
                        {
                            break;//break here
                        }
                        curNode = curNode.Previous;
                        continue;//get next
                    }
                    else if (editableRun == start)
                    {
                        //stop
                        yield return editableRun;
                        break;
                    }
                    //
                    //
                    if (enableIter)
                    {
                        yield return editableRun;
                    }
                    curNode = curNode.Previous;
                }
            }
        }

        public int LineCount => _lines.Count;


        public void RunVisitor(EditableRunVisitor visitor)
        {
            //similar to Draw...
            List<EditableTextLine> lines = _lines;
            int renderAreaTop;
            int renderAreaBottom;
            if (visitor.UseUpdateArea)
            {
                renderAreaTop = visitor.UpdateArea.Top;
                renderAreaBottom = visitor.UpdateArea.Bottom;
            }
            else
            {
                renderAreaTop = 0;
                renderAreaBottom = this.Bottom;
            }


            bool foundFirstLine = false;
            int j = lines.Count;
            for (int i = 0; i < j; ++i)
            {
                EditableTextLine line = lines[i];
                int y = line.Top;

                if (visitor.StopOnNextLine)
                {
                    break; //break from for loop=> go to end
                }

                visitor.VisitNewLine(y); //*** 

                if (!visitor.SkipCurrentLineEditableRunIter)
                {
                    LinkedListNode<EditableRun> curNode = line.First;
                    if (!foundFirstLine)
                    {
                        if (y + line.ActualLineHeight < renderAreaTop)
                        {
                            continue;
                        }
                        else
                        {
                            foundFirstLine = true;
                        }
                    }
                    else
                    {
                        if (y > renderAreaBottom)
                        {
                            break;
                        }
                    }
                    while (curNode != null)
                    {
                        EditableRun child = curNode.Value;

                        //iter entire line, not check horizontal line intersect
                        visitor.VisitEditableRun(child);

                        curNode = curNode.Next;
                    }
                }
            }
        }
        public void DrawChildContent(DrawBoard canvas, Rectangle updateArea)
        {
            //if ((_layerFlags & IS_LAYER_HIDDEN) != 0)
            //{
            //    return;
            //}

            //this.BeginDrawingChildContent();

            List<EditableTextLine> lines = _lines;
            int renderAreaTop = updateArea.Top;
            int renderAreaBottom = updateArea.Bottom;
            bool foundFirstLine = false;
            int j = lines.Count;
            for (int i = 0; i < j; ++i)
            {
                EditableTextLine line = lines[i];
                //#if DEBUG
                //                if (this.OwnerRenderElement is RenderBoxBase)
                //                {
                //                    debug_RecordLineInfo((RenderBoxBase)OwnerRenderElement, line);
                //                }

                //                //  canvas.DrawRectangle(Color.Gray, 0, line.LineTop, line.ActualLineWidth, line.ActualLineHeight);
                //                if (line.RunCount > 1)
                //                {

                //                }
                //#endif


                int y = line.Top;

                if (!foundFirstLine)
                {
                    if (y + line.ActualLineHeight < renderAreaTop)
                    {
                        continue;
                    }
                    else
                    {
                        foundFirstLine = true;
                    }
                }
                else
                {
                    if (y >= renderAreaBottom)
                    {
                        break;
                    }
                }

                updateArea.OffsetY(-y);
                canvas.OffsetCanvasOriginY(y);
                LinkedListNode<EditableRun> curNode = line.First;
                while (curNode != null)
                {
                    EditableRun child = curNode.Value;
                    if (child.IntersectOnHorizontalWith(ref updateArea))
                    {
                        int x = child.X;
                        canvas.OffsetCanvasOriginX(x);
                        updateArea.OffsetX(-x);
                        child.DrawToThisCanvas(canvas, updateArea);
                        canvas.OffsetCanvasOriginX(-x);
                        updateArea.OffsetX(x);
                    }
                    curNode = curNode.Next;
                }
                canvas.OffsetCanvasOriginY(-y);
                updateArea.OffsetY(y);
            }

            //this.FinishDrawingChildContent();
        }

        public bool HitTestCore(HitChain hitChain)
        {
#if DEBUG
            if (hitChain.dbugHitPhase == dbugHitChainPhase.MouseDown)
            {

            }
#endif

            List<EditableTextLine> lines = _lines;
            int j = lines.Count;
            int testYPos = hitChain.TestPoint.Y;
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
            return false;
        }

        static Size ReCalculateContentSizeHorizontalFlow(EditableTextFlowLayer layer)
        {
            throw new NotSupportedException();
            ////only one line
            //EditableTextLine line = (EditableTextLine)layer._lineCollection;
            //LinkedListNode<EditableRun> c_node = line.First;
            ////--------
            //int curX = 0;

            //int maxHeightInRow = 0;
            //int maxWidth = 0;
            //while (c_node != null)
            //{
            //    EditableRun run = c_node.Value;
            //    int runHeight = run.Height;
            //    if (runHeight > maxHeightInRow)
            //    {
            //        maxHeightInRow = runHeight;
            //    }
            //    curX += run.Width;
            //    if (curX > maxWidth)
            //    {
            //        maxWidth = curX;
            //    }


            //    //next
            //    c_node = c_node.Next;
            //}

            //return new Size(maxWidth, maxHeightInRow);
        }

        public void TopDownReArrangeContent(int containerWidth)
        {
            //vinv_IsInTopDownReArrangePhase = true;
#if DEBUG
            //vinv_dbug_EnterLayerReArrangeContent(this);
#endif
            //this.BeginLayerLayoutUpdate(); 

            PerformHorizontalFlowArrange(0, containerWidth, 0);
            //TODO: review reflow again!
            Reflow?.Invoke(this, EventArgs.Empty);
            //this.EndLayerLayoutUpdate();
#if DEBUG
            //vinv_dbug_ExitLayerReArrangeContent();
#endif
        }


        int _posCalContentW;
        int _posCalContentH;
        void SetPostCalculateLayerContentSize(int w, int h)
        {
            _posCalContentH = h;
            _posCalContentW = w;
        }
        public void TopDownReCalculateContentSize()
        {
#if DEBUG

            //vinv_dbug_EnterLayerReCalculateContent(this);
#endif

            EditableTextLine lastline = _lines[_lines.Count - 1];
            SetPostCalculateLayerContentSize(lastline.ActualLineWidth, lastline.ActualLineHeight + lastline.LineTop);

#if DEBUG
            //vinv_dbug_ExitLayerReCalculateContent();
#endif
        }

        public int Bottom
        {
            //get bottom
            get
            {
                EditableTextLine lastLine = GetTextLine(this.LineCount - 1);
                return (lastLine != null) ? lastLine.Top + lastLine.ActualLineHeight : DefaultLineHeight;
            }
        }


        internal EditableTextLine GetTextLine(int lineId)
        {

            if (lineId < _lines.Count)
            {
                return _lines[lineId];
            }
            return null;
        }
        internal EditableTextLine GetTextLineAtPos(int y)
        {
            List<EditableTextLine> lines = _lines;
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
            return null;
        }
        /// <summary>
        /// append to last
        /// </summary>
        /// <param name="line"></param>
        void AppendLine(EditableTextLine line)
        {
            List<EditableTextLine> lines = _lines;
            int lineCount = lines.Count;
            EditableTextLine lastLine = lines[lineCount - 1];
            line.SetLineNumber(lineCount);
            line.SetTop(lastLine.Top + lastLine.ActualLineHeight);
            lines.Add(line);

        }
        void PerformHorizontalFlowArrangeForMultilineText(
            int ownerClientLeft, int ownerClientWidth,
            int ownerClientTop)
        {
#if DEBUG
            long startTick = DateTime.Now.Ticks;
#endif

            List<EditableTextLine> lines = _lines;
            int ownerClientRight = ownerClientLeft + ownerClientWidth;
            int curX = 0;
            int curY = 0;
            bool lastestIsBlock = false;
            int maxWidth = 0;
            int curY_fromTop = ownerClientTop;
            int maxHeightInRow = this.DefaultLineHeight;
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
                    maxHeightInRow = this.DefaultLineHeight;
                    EditableTextLine newLine = null;
                    line.ValidateContentArrangement();
                    bool isFirstRunInThisLine = true;
                    foreach (EditableRun currentRun in line.GetTextRunIter())
                    {
#if DEBUG
                        //vinv_dbug_BeginSetElementBound(currentRun);
#endif
                        int v_desired_width = currentRun.Width;
                        int v_desired_height = currentRun.Height;
                        if (isFirstRunInThisLine)
                        {
                            lastestIsBlock = currentRun.IsBlockElement;
                            if (v_desired_height > maxHeightInRow)
                            {
                                maxHeightInRow = v_desired_height;
                            }
                            EditableRun.DirectSetLocation(currentRun, curX, 0);
                            if (v_desired_height > maxHeightInRow)
                            {
                                maxHeightInRow = v_desired_height;
                            }
                            if (lastestIsBlock)
                            {
                                v_desired_width = ownerClientWidth;
                            }

                            EditableRun.DirectSetSize(currentRun,
                                    v_desired_width, v_desired_height);
                            currentRun.MarkValidContentArrangement();
                            curX += v_desired_width;
                            isFirstRunInThisLine = false;
                        }
                        else
                        {
                            if (lastestIsBlock || currentRun.IsBlockElement ||
                            (curX + v_desired_width > ownerClientRight))
                            {
                                newLine = new EditableTextLine(this);
                                newLine.AddLast(currentRun);
                                curY = curY_fromTop + maxHeightInRow;
                                curY_fromTop = curY;
                                maxHeightInRow = this.DefaultLineHeight;
                                EditableRun nextR = currentRun.NextRun;
                                while (nextR != null)
                                {
                                    line.UnsafeRemoveVisualElement(nextR);
                                    newLine.AddLast(nextR);
                                    nextR = nextR.NextRun;
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
                                lastestIsBlock = currentRun.IsBlockElement;
                                if (v_desired_height > maxHeightInRow)
                                {
                                    maxHeightInRow = v_desired_height;
                                }
                                EditableRun.DirectSetLocation(currentRun, curX, 0);
                                EditableRun.DirectSetSize(currentRun,
                                       v_desired_width, v_desired_height);
                                currentRun.MarkValidContentArrangement();
                                curX += v_desired_width;
                            }
                        }

#if DEBUG
                        // vinv_dbug_EndSetElementBound(currentRun);
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
        void PerformHorizontalFlowArrange(
            int ownerClientLeft, int ownerClientWidth,
            int ownerClientTop)
        {

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

        bool _isArrangeValid;
        void ValidateArrangement()
        {
            _isArrangeValid = true;
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



            List<EditableTextLine> lines = _lines;
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
                textLine.EditableFlowLayer = this;
                lines.Insert(insertAt, textLine);
            }

        }



        public void CopyContentToStringBuilder(StringBuilder stBuilder)
        {
            List<EditableTextLine> lines = _lines;
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
        public void Clear()
        {
            List<EditableTextLine> lines = _lines;
            for (int i = lines.Count - 1; i > -1; --i)
            {
                EditableTextLine line = lines[i];
                line.EditableFlowLayer = null;
                line.Clear();
            }
            lines.Clear();

            //auto add first line
            _lines.Add(new EditableTextLine(this));
        }

        internal void Remove(int lineId)
        {
#if DEBUG
            if (lineId < 0)
            {
                throw new NotSupportedException();
            }
#endif
            //if ((_layerFlags & FLOWLAYER_HAS_MULTILINE) == 0)
            //{
            //    return;
            //}
            List<EditableTextLine> lines = _lines;
            if (lines.Count < 2)
            {
                return;
            }

            EditableTextLine removedLine = lines[lineId];
            int cy = removedLine.Top;
            //
            lines.RemoveAt(lineId);
            removedLine.EditableFlowLayer = null;

            int j = lines.Count;
            for (int i = lineId; i < j; ++i)
            {
                EditableTextLine line = lines[i];
                line.SetTop(cy);
                line.SetLineNumber(i);
                cy += line.ActualLineHeight;
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

            List<EditableTextLine> lines = _lines;
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

#endif
    }



}