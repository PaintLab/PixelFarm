//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.TextEditing
{
    class TextMarkerLayer
    {
        List<VisualMarkerSelectionRange> _visualMarkers;
        public TextMarkerLayer(List<VisualMarkerSelectionRange> visualMarkers)
        {
            _visualMarkers = visualMarkers;
        }
        internal List<VisualMarkerSelectionRange> VisualMarkers => _visualMarkers;
        internal int VisualMarkerCount => (_visualMarkers == null) ? 0 : _visualMarkers.Count;
        public void AddMarker(VisualMarkerSelectionRange markerSelectionRange)
        {
            _visualMarkers.Add(markerSelectionRange);
        }
        public void Clear()
        {
            _visualMarkers.Clear();
        }
        public void Remove(VisualMarkerSelectionRange markerRange)
        {
            _visualMarkers.Remove(markerRange);
        }
    }




    sealed class TextFlowLayer
    {
        public event EventHandler Reflow; //TODO: review this field
        public event EventHandler ContentSizeChanged;//TODO: review this field


        //TODO: use linked-list or tree for lines
        List<TextLine> _lines = new List<TextLine>();
        
        ITextFlowLayerOwner _owner;

        /// <summary>
        /// default, current run style for new Run
        /// </summary>
        RunStyle _runStyle;

        public TextFlowLayer(ITextFlowLayerOwner owner,
            ITextService textService,
            RunStyle defaultSpanStyle)
        {
            _owner = owner;
            TextServices = textService;

            //start with single line per layer
            //and can be changed to multiline
            _runStyle = defaultSpanStyle;
            //add default lines
            _lines.Add(new TextLine(this));
        }

        public ITextService TextServices { get; set; }
        public RunStyle DefaultRunStyle => _runStyle;
        public void SetDefaultRunStyle(RunStyle runStyle)
        {
            _runStyle = runStyle;
        }
        internal void ClientLineBubbleupInvalidateArea(Rectangle clientInvalidatedArea)
        {
            _owner.ClientLayerBubbleUpInvalidateArea(clientInvalidatedArea);
        }

        public int DefaultLineHeight => _runStyle.ReqFont.LineSpacingInPixels;

        internal void NotifyContentSizeChanged() => ContentSizeChanged?.Invoke(this, EventArgs.Empty);

        public Run LatestHitRun { get; set; }

        public void SetUseDoubleCanvas(bool useWithWidth, bool useWithHeight)
        {
            //this.SetDoubleCanvas(useWithWidth, useWithHeight);
        }


        public bool FlowLayerHasMultiLines => _lines.Count > 1;

        internal IEnumerable<Run> GetDrawingIter(Run start, Run stop)
        {
            List<TextLine> lines = _lines;
            int j = lines.Count;
            for (int i = 0; i < j; ++i)
            {
                LinkedListNode<Run> curNode = lines[i].Last;
                bool enableIter = false;
                while (curNode != null)
                {
                    Run editableRun = curNode.Value;
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


        public void RunVisitor(RunVisitor visitor)
        {
            //similar to Draw...
            List<TextLine> lines = _lines;
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
                TextLine line = lines[i];
                int y = line.Top;

                if (visitor.StopOnNextLine)
                {
                    break; //break from for loop=> go to end
                }

                visitor.VisitNewLine(y); //*** 

                if (!visitor.SkipCurrentLineEditableRunIter)
                {
                    LinkedListNode<Run> curNode = line.First;
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
                        Run child = curNode.Value;

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

            List<TextLine> lines = _lines;
            int renderAreaTop = updateArea.Top;
            int renderAreaBottom = updateArea.Bottom;
            bool foundFirstLine = false;
            int j = lines.Count;
            for (int i = 0; i < j; ++i)
            {
                TextLine line = lines[i];
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
                LinkedListNode<Run> curNode = line.First;

                while (curNode != null)
                {
                    Run child = curNode.Value;
                    if (child.HitTest(updateArea))
                    {
                        int x = child.Left;
                        canvas.OffsetCanvasOriginX(x);
                        updateArea.OffsetX(-x);

                        child.Draw(canvas, updateArea);

                        canvas.OffsetCanvasOriginX(-x);
                        updateArea.OffsetX(x);
                    }
                    //if (child.IntersectOnHorizontalWith(ref updateArea))
                    //{
                    //    int x = child.X;
                    //    canvas.OffsetCanvasOriginX(x);
                    //    updateArea.OffsetX(-x);
                    //    child.DrawToThisCanvas(canvas, updateArea);
                    //    canvas.OffsetCanvasOriginX(-x);
                    //    updateArea.OffsetX(x);
                    //}
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

            List<TextLine> lines = _lines;
            int j = lines.Count;
            int testYPos = hitChain.TestPoint.Y;
            for (int i = 0; i < j; ++i)
            {
                TextLine line = lines[i];
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

        static Size ReCalculateContentSizeHorizontalFlow(TextFlowLayer layer)
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

            TextLine lastline = _lines[_lines.Count - 1];
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
                TextLine lastLine = GetTextLine(this.LineCount - 1);
                return (lastLine != null) ? lastLine.Top + lastLine.ActualLineHeight : DefaultLineHeight;
            }
        }


        internal TextLine GetTextLine(int lineId)
        {

            if (lineId < _lines.Count)
            {
                return _lines[lineId];
            }
            return null;
        }
        internal TextLine GetTextLineAtPos(int y)
        {
            List<TextLine> lines = _lines;
            if (lines != null)
            {
                int j = lines.Count;
                for (int i = 0; i < j; ++i)
                {
                    TextLine line = lines[i];
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
        void AppendLine(TextLine line)
        {
            List<TextLine> lines = _lines;
            int lineCount = lines.Count;
            TextLine lastLine = lines[lineCount - 1];
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

            List<TextLine> lines = _lines;
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
                TextLine line = lines[i];
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
                    TextLine newLine = null;
                    line.ValidateContentArrangement();
                    bool isFirstRunInThisLine = true;
                    foreach (Run currentRun in line.GetTextRunIter())
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
                            Run.DirectSetLocation(currentRun, curX, 0);
                            if (v_desired_height > maxHeightInRow)
                            {
                                maxHeightInRow = v_desired_height;
                            }
                            if (lastestIsBlock)
                            {
                                v_desired_width = ownerClientWidth;
                            }

                            Run.DirectSetSize(currentRun,
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
                                newLine = new TextLine(this);
                                newLine.AddLast(currentRun);
                                curY = curY_fromTop + maxHeightInRow;
                                curY_fromTop = curY;
                                maxHeightInRow = this.DefaultLineHeight;
                                Run nextR = currentRun.NextRun;
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
                                Run.DirectSetLocation(currentRun, curX, 0);
                                Run.DirectSetSize(currentRun,
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
        internal TextLine InsertNewLine(int insertAt)
        {
            TextLine newLine = new TextLine(this);
            this.InsertLine(insertAt, newLine);
            return newLine;
        }
        void InsertLine(int insertAt, TextLine textLine)
        {
            if (insertAt < 0)
            {
                throw new NotSupportedException();
            }



            List<TextLine> lines = _lines;
            int j = lines.Count;
            if (insertAt >= j)
            {
                AppendLine(textLine);
            }
            else
            {
                TextLine line = lines[insertAt];
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

                lines.Insert(insertAt, textLine);
            }

        }



        public void CopyContentToStringBuilder(StringBuilder stBuilder)
        {
            List<TextLine> lines = _lines;
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

        internal IEnumerable<Run> TextRunForward(Run startRun, Run stopRun)
        {
            TextLine currentLine = startRun.OwnerLine;
            TextLine stopLine = stopRun.OwnerLine;
            if (currentLine == stopLine)
            {
                foreach (Run r in currentLine.GetVisualElementForward(startRun, stopRun))
                {
                    yield return r;
                }
            }
            else
            {
                foreach (Run r in currentLine.GetVisualElementForward(startRun))
                {
                    yield return r;
                }
                currentLine = currentLine.Next;
                while (currentLine != null)
                {
                    if (currentLine == stopLine)
                    {
                        foreach (Run r in currentLine.GetTextRunIter())
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
                        foreach (Run r in currentLine.GetTextRunIter())
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
            List<TextLine> lines = _lines;
            for (int i = lines.Count - 1; i > -1; --i)
            {
                TextLine line = lines[i];
                line.RemoveOwnerFlowLayer();
                line.Clear();
            }
            lines.Clear();

            //auto add first line
            _lines.Add(new TextLine(this));
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
            List<TextLine> lines = _lines;
            if (lines.Count < 2)
            {
                return;
            }

            TextLine removedLine = lines[lineId];
            int cy = removedLine.Top;
            //
            lines.RemoveAt(lineId);
            removedLine.RemoveOwnerFlowLayer();

            int j = lines.Count;
            for (int i = lineId; i < j; ++i)
            {
                TextLine line = lines[i];
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
        public IEnumerable<Run> dbugGetDrawingIter2()
        {

            List<TextLine> lines = _lines;
            int j = lines.Count;
            for (int i = 0; i < j; ++i)
            {
                LinkedListNode<Run> curNode = lines[i].First;
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