//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.TextEditing
{
    partial class EditableTextFlowLayer
    {
        public IEnumerable<RenderElement> GetRenderElementReverseIter()
        {
            List<EditableTextLine> lines = _lines;
            for (int i = lines.Count - 1; i >= 0; --i)
            {
                EditableTextLine ln = lines[i];
                LinkedListNode<EditableRun> veNode = ln.Last;
                while (veNode != null)
                {
                    yield return veNode.Value;
                    veNode = veNode.Previous;
                }
            }
        }
        public IEnumerable<RenderElement> GetRenderElementIter()
        {
            List<EditableTextLine> lines = _lines;
            int j = lines.Count;
            for (int i = 0; i < j; ++i)
            {
                EditableTextLine ln = lines[i];
                LinkedListNode<EditableRun> veNode = ln.First;
                while (veNode != null)
                {
                    yield return veNode.Value;
                    veNode = veNode.Next;
                }
            }
        }
        //public void AddTop(EditableRun visualElement)
        //{
        //    _lines[_lines.Count - 1].AddLast(visualElement);
        //}
        //public void AddBefore(EditableRun beforeVisualElement, EditableRun visualElement)
        //{
        //    EditableTextLine targetLine = beforeVisualElement.OwnerEditableLine;
        //    if (targetLine != null)
        //    {
        //        targetLine.AddBefore(beforeVisualElement, visualElement);
        //    }
        //    else
        //    {
        //        throw new NotSupportedException();
        //    }
        //}

        public void AddAfter(EditableRun afterVisualElement, EditableRun visualElement)
        {
            EditableTextLine targetLine = afterVisualElement.OwnerEditableLine;
            if (targetLine != null)
            {
                targetLine.AddAfter(afterVisualElement, visualElement);
            }
            else
            {
                throw new NotSupportedException();
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
    }
}