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
    public abstract class EditableRun
    {
        EditableTextLine _ownerTextLine;
        RootGraphic _gfx;
        LinkedListNode<EditableRun> _editableRunInternalLinkedNode;
        public EditableRun(RootGraphic gfx)
        {
            _gfx = gfx;
            Width = 10;
            Height = 10;
        }
        public RootGraphic Root => _gfx;

        //-----
        public bool HitTest(Rectangle r)
        {
            return Bounds.IntersectsWith(r);
        }
        public bool HitTest(int x, int y)
        {
            return Bounds.Contains(x, y);
        }
        public bool IsBlockElement { get; set; }
        public virtual void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea) { }
        public bool HasParent => _ownerTextLine != null;
        public Size Size => new Size(Width, Height);
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Right => X + Width;
        public int Bottom => Y + Height;
        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);
        public static void DirectSetSize(EditableRun run, int w, int h)
        {
            run.Width = w;
            run.Height = h;
        }
        public static void DirectSetLocation(EditableRun run, int x, int y)
        {
            run.X = x;
            run.Y = y;
        }
        public static void RemoveParentLink(EditableRun run)
        {
            run._editableRunInternalLinkedNode = null;
        }
        protected void SetSize2(int w, int h)
        {
            Width = w;
            Height = h;
        }
        bool _validCalSize;
        bool _validContentArr;
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
                RenderBoxBase ownerBox = _ownerTextLine.OwnerFlowLayer.Owner;
                Rectangle bounds = this.Bounds;
                bounds.OffsetY(_ownerTextLine.Top);
                bounds.Offset(ownerBox.X, ownerBox.Y);
                //TODO: review invalidate bubble
                ownerBox.InvalidateParentGraphics(bounds);
            }
        }
        //-----

        public abstract char GetChar(int index);
        internal abstract bool IsInsertable { get; }
        public abstract string GetText();
        public abstract int CharacterCount { get; }
        //--------------------
        //model
        public abstract EditableRunCharLocation GetCharacterFromPixelOffset(int pixelOffset);
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
        internal static CopyRun InnerRemove(EditableRun tt, int startIndex, int length, bool withFreeRun)
        {
            return tt.Remove(startIndex, length, withFreeRun);
        }
        internal static CopyRun InnerRemove(EditableRun tt, int startIndex, bool withFreeRun)
        {
            return tt.Remove(startIndex, tt.CharacterCount - (startIndex), withFreeRun);
        }
        internal static EditableRunCharLocation InnerGetCharacterFromPixelOffset(EditableRun tt, int pixelOffset)
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
        public EditableRun NextRun
        {
            get
            {
                if (this.LinkedNodeForEditableRun != null)
                {
                    if (LinkedNodeForEditableRun.Next != null)
                    {
                        return LinkedNodeForEditableRun.Next.Value;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// prev run
        /// </summary>
        public EditableRun PrevRun
        {
            get
            {
                if (this.LinkedNodeForEditableRun != null)
                {
                    if (LinkedNodeForEditableRun.Previous != null)
                    {
                        return LinkedNodeForEditableRun.Previous.Value;
                    }
                }
                return null;
            }
        }
        //
        internal EditableTextLine OwnerEditableLine => _ownerTextLine;
        //
        internal LinkedListNode<EditableRun> LinkedNodeForEditableRun => _editableRunInternalLinkedNode;
        //
        internal void SetInternalLinkedNode(LinkedListNode<EditableRun> linkedNode, EditableTextLine ownerTextLine)
        {
            _ownerTextLine = ownerTextLine;
            _editableRunInternalLinkedNode = linkedNode;
            //EditableRun.SetParentLink(this, ownerTextLine);
        }
        //----------------------------------------------------------------------
        public void TopDownReCalculateContentSize()
        {
            InnerTextRunTopDownReCalculateContentSize(this);
        }

        public static void InnerTextRunTopDownReCalculateContentSize(EditableRun ve)
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
        //presentation of this run
        public abstract TextSpanStyle SpanStyle { get; }
        public abstract void SetStyle(TextSpanStyle spanStyle);
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