//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using Typography.Text;

namespace LayoutFarm.TextFlow
{

    /// <summary>
    /// any run, text, image etc
    /// </summary>
    public abstract partial class Run
    {
        readonly RunStyle _runStyle;
        TextLineBox _ownerTextLine;

        LinkedListNode<Run> _linkNode;

        int _left;
        int _top;
        int _width;
        int _height;

        internal Run(RunStyle runStyle)
        {
            _runStyle = runStyle;
            _width = _height = 10;//default
        }

        protected RequestFont GetFont() => _runStyle.ReqFont;

        protected Size MeasureString(in Typography.Text.TextBufferSpan textBufferSpan) => _runStyle.MeasureString(textBufferSpan);

        public RunStyle RunStyle => _runStyle;

        public bool HitTest(UpdateArea r) => Bounds.IntersectsWith(r.CurrentRect);

        public bool HitTest(int x, int y) => Bounds.Contains(x, y);

        public bool IsBlockElement { get; set; }

        public abstract void Draw(DrawBoard d, UpdateArea updateArea);

        public bool HasParent => _ownerTextLine != null;

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

        public abstract int GetChar(int index);

        public abstract void WriteTo(Typography.Text.TextCopyBuffer output);
        //public abstract void WriteTo(Typography.Text.TextCopyBuffer output, int start, int len);
        //public abstract void WriteTo(Typography.Text.TextCopyBuffer output, int start);
        //--------------------
        //model
        public abstract CharLocation GetCharacterFromPixelOffset(int pixelOffset);
        /// <summary>
        /// get run width from start (left**) to charOffset
        /// </summary>
        /// <param name="charOffset"></param>
        /// <returns></returns>
        public abstract int GetRunWidth(int charOffset);
        ///// <summary>
        ///// get run with from charOffset to 
        ///// </summary>
        ///// <param name="charOffset"></param>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //public abstract int GetRunWidth(int startAtCharOffset, int count);

        internal static CharLocation InnerGetCharacterFromPixelOffset(Run tt, int pixelOffset)
        {
            return tt.GetCharacterFromPixelOffset(pixelOffset);
        }
        public abstract void UpdateRunWidth();

        //public abstract TextSegment LeftCopy(int index);
        //public abstract TextSegment Copy(int startIndex, int length);
        //public abstract TextSegment Copy(int startIndex);

        ////------------------------------
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
        protected internal void InvalidateOwnerLineCharCount() => _ownerTextLine?.InvalidateCharCount();

    }

    public static class RunExtensions
    {
        [ThreadStatic]
        static Typography.Text.TextCopyBufferUtf32 s_copyBuffer;
        public static string GetUpperString(this Run textRun)
        {
            //TODO: review here
            if (s_copyBuffer != null)
            {
                s_copyBuffer = new TextCopyBufferUtf32();
            }
            else
            {
                s_copyBuffer.Clear();
            }
            textRun.WriteTo(s_copyBuffer);
            return s_copyBuffer.ToString().ToUpper();
        }
    }
}