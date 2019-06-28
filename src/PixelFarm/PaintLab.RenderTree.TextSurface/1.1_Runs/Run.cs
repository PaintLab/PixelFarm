//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.TextEditing
{

    /// <summary>
    /// any run, text, image etc
    /// </summary>
    public abstract class Run
    {
        bool _validCalSize;
        bool _validContentArr;
        TextLine _ownerTextLine;
        RunStyle _runStyle;
        LinkedListNode<Run> _linkNode;

        int _width;
        int _height;
        int _left;
        int _top;

        internal Run(TextLine ownerLine, RunStyle runStyle)
        {
            _runStyle = runStyle;
            _width = _height = 10;
        }

        protected RequestFont GetFont() => _runStyle.ReqFont;

        protected int MeasureLineHeightInt32()
        {
            return (int)Math.Round(_runStyle.MeasureBlankLineHeight());
        }
        protected float MeasureLineHeight()
        {
            return _runStyle.MeasureBlankLineHeight();
        }

        protected Size MeasureString(ref TextBufferSpan textBufferSpan) => _runStyle.MeasureString(ref textBufferSpan);

        protected bool SupportWordBreak => _runStyle.SupportsWordBreak;

        protected ILineSegmentList BreakToLineSegs(ref TextBufferSpan textBufferSpan)
        {
            return _runStyle.BreakToLineSegments(ref textBufferSpan);
        }

        protected void MeasureString2(ref TextBufferSpan textBufferSpan,
            ILineSegmentList lineSeg,
            int[] outputUsrCharAdvances,
            out int outputTotalW,
            out int outputLineHeight)
        {
            if (lineSeg != null)
            {
                ILineSegmentList seglist = _runStyle.BreakToLineSegments(ref textBufferSpan);
                _runStyle.CalculateUserCharGlyphAdvancePos(ref textBufferSpan, seglist,
                    outputUsrCharAdvances,
                    out outputTotalW,
                    out outputLineHeight);
            }
            else
            {
                _runStyle.CalculateUserCharGlyphAdvancePos(ref textBufferSpan,
                    outputUsrCharAdvances,
                    out outputTotalW,
                    out outputLineHeight);
            }
        }

        public RunStyle RunStyle => _runStyle;
        //
        public virtual void SetStyle(RunStyle runStyle)
        {
            _runStyle = runStyle;
        }
        public bool HitTest(Rectangle r)
        {
            return Bounds.IntersectsWith(r);
        }
        public bool HitTest(int x, int y)
        {
            return Bounds.Contains(x, y);
        }
        public bool IsBlockElement { get; set; }

        public abstract void Draw(DrawBoard canvas, Rectangle updateArea);

        public bool HasParent => _ownerTextLine != null;
        public Size Size => new Size(_width, _height);
        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public int Left { get => _left; set => _left = value; }
        public int Top { get => _top; set => _top = value; }
        public int Right => _left + _width;
        public int Bottom => _top + _height;
        //
        public Rectangle Bounds => new Rectangle(_left, _top, _width, _height);

        public static void DirectSetSize(Run run, int w, int h)
        {
            run._width = w;
            run._height = h;
        }
        public static void DirectSetLocation(Run run, int x, int y)
        {
            run._left = x;
            run._top = y;
        }
        public static void RemoveParentLink(Run run)
        {
            run._linkNode = null;
        }
        protected void SetSize2(int w, int h)
        {
            _width = w;
            _height = h;
        }
        public void MarkHasValidCalculateSize()
        {
            _validCalSize = true;
        }
        public void MarkValidContentArrangement()
        {
            _validContentArr = true;
        }
        protected void InvalidateGraphics()
        {
            if (_ownerTextLine != null)
            {
                _ownerTextLine.ClientRunInvalidateGraphics(this.Bounds);

            }
        }

        public abstract char GetChar(int index);
        internal abstract bool IsInsertable { get; }
        public abstract string GetText();
        public abstract int CharacterCount { get; }
        //--------------------
        //model
        public abstract CharLocation GetCharacterFromPixelOffset(int pixelOffset);
        /// <summary>
        /// get run width from start (left**) to charOffset
        /// </summary>
        /// <param name="charOffset"></param>
        /// <returns></returns>
        public abstract int GetRunWidth(int charOffset);

        ///////////////////////////////////////////////////////////////
        //edit funcs
        internal abstract void InsertAfter(int index, char c);
        internal abstract CopyRun Remove(int startIndex, int length, bool withFreeRun);
        internal static CopyRun InnerRemove(Run tt, int startIndex, int length, bool withFreeRun)
        {
            return tt.Remove(startIndex, length, withFreeRun);
        }
        internal static CopyRun InnerRemove(Run tt, int startIndex, bool withFreeRun)
        {
            return tt.Remove(startIndex, tt.CharacterCount - (startIndex), withFreeRun);
        }
        internal static CharLocation InnerGetCharacterFromPixelOffset(Run tt, int pixelOffset)
        {
            return tt.GetCharacterFromPixelOffset(pixelOffset);
        }
        public abstract void UpdateRunWidth();
        ///////////////////////////////////////////////////////////////  
        public abstract CopyRun CreateCopy();
        public abstract CopyRun LeftCopy(int index);
        public abstract CopyRun Copy(int startIndex, int length);
        public abstract CopyRun Copy(int startIndex);
        public abstract void CopyContentToStringBuilder(StringBuilder stBuilder);
        //------------------------------
        //owner, neighbor
        /// <summary>
        /// next run
        /// </summary>
        public Run NextRun
        {
            get
            {
                if (_linkNode != null)
                {
                    if (_linkNode.Next != null)
                    {
                        return _linkNode.Next.Value;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// prev run
        /// </summary>
        public Run PrevRun
        {
            get
            {
                if (_linkNode != null)
                {
                    if (_linkNode.Previous != null)
                    {
                        return _linkNode.Previous.Value;
                    }
                }
                return null;
            }
        }
        //
        internal TextLine OwnerLine => _ownerTextLine;
        //
        internal LinkedListNode<Run> LinkNode => _linkNode;
        //
        internal void SetInternalLinkNode(LinkedListNode<Run> linkNode)
        {
            _linkNode = linkNode;
        }
        //----------------------------------------------------------------------
        public void TopDownReCalculateContentSize()
        {
            InnerTextRunTopDownReCalculateContentSize(this);
        }

        public static void InnerTextRunTopDownReCalculateContentSize(Run ve)
        {
#if DEBUG
            //dbug_EnterTopDownReCalculateContent(ve);
#endif

            ve.UpdateRunWidth();
#if DEBUG
            //dbug_ExitTopDownReCalculateContent(ve);
#endif
        }
        //--------------------


#if DEBUG
        //public override string dbug_FullElementDescription()
        //{
        //    string user_elem_id = null;
        //    if (user_elem_id != null)
        //    {
        //        return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
        //            + " i" + dbug_obj_id + "a " + ((EditableRun)this).GetText() + ",(ID " + user_elem_id + ") " + dbug_GetLayoutInfo();
        //    }
        //    else
        //    {
        //        return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
        //         + " i" + dbug_obj_id + "a " + ((EditableRun)this).GetText() + " " + dbug_GetLayoutInfo();
        //    }
        //}
        //public override string ToString()
        //{
        //    return "[" + this.dbug_obj_id + "]" + GetText();
        //}
#endif
    }
}