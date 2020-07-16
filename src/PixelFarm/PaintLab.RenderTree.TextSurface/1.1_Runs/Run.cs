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
        //bool _validCalSize;
        //bool _validContentArr;

        TextLineBox _ownerTextLine;
        RunStyle _runStyle;
        LinkedListNode<Run> _linkNode;

        int _left;
        int _top;
        int _width;
        int _height;


        internal Run(RunStyle runStyle)
        {
            _runStyle = runStyle;
            _width = _height = 10;
        }

        protected RequestFont GetFont() => _runStyle.ReqFont;

        protected int MeasureLineHeightInt32() => (int)Math.Round(_runStyle.MeasureBlankLineHeight());

        protected float MeasureLineHeight() => _runStyle.MeasureBlankLineHeight();

        protected Size MeasureString(in TextBufferSpan textBufferSpan) => _runStyle.MeasureString(textBufferSpan);

        public RunStyle RunStyle => _runStyle;
        //
        public virtual void SetStyle(RunStyle runStyle) => _runStyle = runStyle;

        public bool HitTest(Rectangle r) => Bounds.IntersectsWith(r);

        public bool HitTest(UpdateArea r) => Bounds.IntersectsWith(r.CurrentRect);

        public bool HitTest(int x, int y) => Bounds.Contains(x, y);

        public bool IsBlockElement { get; set; }

        public abstract void Draw(DrawBoard d, UpdateArea updateArea);

        public bool HasParent => _ownerTextLine != null;
        public Size Size => new Size(_width, _height);
        public int Width => _width;
        public int Height => _height;
        public int Left => _left;
        public int Top => _top;
        public int Right => _left + _width;
        public int Bottom => _top + _height;
        //
        public Rectangle Bounds => new Rectangle(_left, _top, _width, _height);

        internal static void DirectSetLocation(Run run, int x, int y)
        {
            run._left = x;
            run._top = y;
        }
        protected void SetSize(int w, int h)
        {
            _width = w;
            _height = h;
        }

        public static void RemoveParentLink(Run run)
        {
            run._ownerTextLine?.InvalidateCharCount();
            run._ownerTextLine = null;
            run._linkNode = null;
        }

        protected void InvalidateGraphics() => _ownerTextLine?.ClientRunInvalidateGraphics(this);

        internal abstract bool IsInsertable { get; }
        public abstract int CharacterCount { get; }
        public abstract char GetChar(int index);
        public abstract string GetText();
        public abstract void WriteTo(StringBuilder stbuilder);
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
        public Run NextRun => _linkNode?.Next?.Value;

        /// <summary>
        /// prev run
        /// </summary>
        public Run PrevRun => _linkNode?.Previous?.Value;

        internal TextLineBox OwnerLine => _ownerTextLine;

        internal LinkedListNode<Run> LinkNode => _linkNode;

        internal void SetLinkNode(LinkedListNode<Run> linkNode, TextLineBox owner)
        {
            _linkNode = linkNode;
            _ownerTextLine = owner;
            owner.InvalidateCharCount();
        }

        public void TopDownReCalculateContentSize() => this.UpdateRunWidth();
        protected internal void InvalidateOwnerLineCharCount() => _ownerTextLine?.InvalidateCharCount();
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