//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.TextFlow;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
    public abstract class TextBoxBase : AbstractRectUI
    {
        protected TextSurfaceEventListener _textSurfaceListener;
        protected TextEditRenderBox _textEditRenderElement;
        protected bool _multiline;
        protected TextSpanStyle _defaultSpanStyle;
        protected Color _backgroundColor = Color.White;

        Color _selectionBgColor = Color.Yellow;//?
        Color _selectionTextColor = Color.Black;

        internal TextBoxBase(int width, int height)
            : base(width, height)
        {
        }

        public int LineCount => _textEditRenderElement.LineCount;

        public Color SelectionFontColor
        {
            get => _selectionBgColor;
            set
            {
                _selectionBgColor = value;
                if (_textEditRenderElement != null)
                {
                    _textEditRenderElement.SelectionTextColor = value;
                }
            }
        }
        public Color SelectionBackgroundColor
        {
            get => _selectionBgColor;
            set
            {
                _selectionBgColor = value;
                if (_textEditRenderElement != null)
                {
                    _textEditRenderElement.SelectionBackgroundColor = value;
                }
            }
        }
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                if (_textEditRenderElement != null) _textEditRenderElement.BackgroundColor = value;
            }
        }
        public override void SetFont(RequestFont font)
        {
            if (font == null) return;
            if (_textEditRenderElement == null)
            {
                _defaultSpanStyle.FontColor = Color.Black;
                _defaultSpanStyle.ReqFont = font;
            }
            else
            {
                _defaultSpanStyle.ReqFont = font;
                _defaultSpanStyle.FontColor = Color.Black;
                _textEditRenderElement.CurrentTextSpanStyle = _defaultSpanStyle;
            }
        }
        public TextSpanStyle DefaultSpanStyle
        {
            get => _defaultSpanStyle;
            set
            {
                _defaultSpanStyle = value;
                if (_textEditRenderElement != null)
                {
                    _textEditRenderElement.CurrentTextSpanStyle = value;
                }
            }
        }
        public ContentTextSplitter TextSplitter { get; set; }

        public bool IsMultilineTextBox => _multiline;
        //
        public int CurrentLineHeight => _textEditRenderElement.CurrentLineHeight;
        //
        public Point CaretPosition => _textEditRenderElement.CurrentCaretPos;
        //
        public int CurrentLineCharIndex => _textEditRenderElement.CurrentLineCharIndex;
        //
        
        public int CurrentLineNumber => _textEditRenderElement.CurrentLineNumber;

        public override void Focus()
        {
            //request keyboard focus
            base.Focus();
            _textEditRenderElement?.Focus();
        }
        public override void Blur()
        {
            base.Blur();
            _textEditRenderElement?.Blur();
        }
        public virtual bool HasSomeText => _textEditRenderElement.HasSomeText;
        public virtual void ClearText() => _textEditRenderElement?.ClearAllChildren();
        //
        public override RenderElement CurrentPrimaryRenderElement => _textEditRenderElement;


        //        
        public override int ViewportLeft => _textEditRenderElement.ViewportLeft;
        //
        public override int ViewportTop => _textEditRenderElement.ViewportTop;
        //

        public override void SetViewport(int x, int y, object reqBy) => _textEditRenderElement?.SetViewport(x, y);


        public override int InnerWidth => (_textEditRenderElement != null) ? _textEditRenderElement.InnerContentWidth : base.InnerWidth;
        public override int InnerHeight => (_textEditRenderElement != null) ? _textEditRenderElement.InnerContentHeight : base.InnerHeight;

        public abstract string Text { get; set; }

        //
        public void FindCurrentUnderlyingWord(out int startAt, out int len)
        {
            _textEditRenderElement.FindCurrentUnderlyingWord(out startAt, out len);
        }

        public virtual TextSurfaceEventListener TextEventListener
        {
            get => _textSurfaceListener;
            set
            {
                _textSurfaceListener = value;
                if (_textEditRenderElement != null)
                {
                    _textEditRenderElement.TextSurfaceListener = value;
                }
            }
        }
        //---------------------------------------------------------------- 
        protected override void OnMouseLeave(UIMouseLeaveEventArgs e)
        {
            e.MouseCursorStyle = MouseCursorStyle.Arrow;
        }
        protected override void OnDoubleClick(UIMouseEventArgs e)
        {

            _textEditRenderElement.HandleDoubleClick(e);
            e.CancelBubbling = true;
        }
        protected override void OnMouseWheel(UIMouseWheelEventArgs e)
        {
            //mouse wheel on  
            _textEditRenderElement.HandleMouseWheel(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyPress(UIKeyEventArgs e)
        {
            //eg. mask text
            //we collect actual key and send the mask to to the background  
            _textEditRenderElement.HandleKeyPress(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            _textEditRenderElement.HandleKeyDown(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            _textEditRenderElement.HandleKeyUp(e);
            e.CancelBubbling = true;
        }
        protected override bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            if (_textEditRenderElement.HandleProcessDialogKey(e))
            {
                e.CancelBubbling = true;
                return true;
            }
            return false;
        }
        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            this.Focus();
            e.MouseCursorStyle = MouseCursorStyle.IBeam;
            e.CancelBubbling = true;

            _textEditRenderElement.HandleMouseDown(e);
        }
        protected override void OnLostKeyboardFocus(UIFocusEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            _textEditRenderElement.Blur();
        }
        protected override void OnMouseMove(UIMouseMoveEventArgs e)
        {
            if (e.IsDragging)
            {
                _textEditRenderElement.HandleDrag(e);
                e.CancelBubbling = true;
                e.MouseCursorStyle = MouseCursorStyle.IBeam;
            }
        }
        protected override void OnMouseUp(UIMouseUpEventArgs e)
        {
            if (e.IsDragging)
            {
                _textEditRenderElement.HandleDragEnd(e);
            }
            else
            {
                _textEditRenderElement.HandleMouseUp(e);
            }
            e.MouseCursorStyle = MouseCursorStyle.Default;
            e.CancelBubbling = true;
        }

        internal bool IsSharedTextBox { get; set; }
        internal bool IsInTextBoxPool { get; set; }

        public void SelectAll() => _textEditRenderElement?.SelectAll();
    }

    public class TextBox : TextBoxBase
    {
        PlainTextDocument _doc;
        bool _isEditable;

        public TextBox(int width, int height, bool multiline, bool isEditable = true)
            : base(width, height)
        {
            _isEditable = isEditable;
            _multiline = multiline;
        }

        /// <summary>
        /// write all lines into stbuilder
        /// </summary>
        /// <param name="stbuilder"></param>
        public void CopyContentTo(Typography.Text.TextCopyBuffer output)
        {
            _textEditRenderElement.CopyContentToStringBuilder(output);
        }
        public void CopyCurrentLine(Typography.Text.TextCopyBuffer output)
        {
            //TODO
            _textEditRenderElement.CopyCurrentLine(output);
        }

        public VisualTextFlowEditSession GetEditSession() => TextEditRenderBox.GetEditSession(_textEditRenderElement);

#if DEBUG
        public override void SetLocation(int left, int top)
        {
            //for debug
            base.SetLocation(left, top);
        }
#endif

        public override string Text
        {
            get
            {
                if (_textEditRenderElement != null)
                {

                    using (new TextUtf32RangeCopyPoolContext<TextBox>(out Typography.Text.TextCopyBufferUtf32 output))
                    {
                        CopyContentTo(output);
                        return output.ToString();
                    }

                }
                else
                {
                    using (new StringBuilderPoolContext<TextBox>(out StringBuilder sb))
                    {
                        _doc.WriteTo(sb);
                        return sb.ToString();
                    }
                }
            }
            set
            {
                if (_textEditRenderElement == null)
                {
                    _doc = new PlainTextDocument(value);
                    return;
                }
                //---------------                 
                if (value == null)
                {
                    _doc = new PlainTextDocument("");
                    return;
                }
                _doc = new PlainTextDocument(value);
                ReloadDocument();
                //convert to runs
            }
        }
        void ReloadDocument()
        {
            if (_doc == null)
            {
                return;
            }

            _textEditRenderElement.ClearAllChildren();
            _textEditRenderElement.Reload(_doc);
            this.InvalidateGraphics();
        }
        public override RenderElement GetPrimaryRenderElement()
        {
            if (_textEditRenderElement == null)
            {
                var tbox = new TextEditRenderBox(this.Width, this.Height, _multiline, _isEditable);
                tbox.SetLocation(this.Left, this.Top);

                if (_defaultSpanStyle.IsEmpty())
                {
                    _defaultSpanStyle = new TextSpanStyle();
                    _defaultSpanStyle.FontColor = Color.Black;
                    _defaultSpanStyle.ReqFont = GlobalRootGraphic.CurrentRootGfx.DefaultTextEditFontInfo;
                    tbox.CurrentTextSpanStyle = _defaultSpanStyle;
                }
                else
                {
                    tbox.CurrentTextSpanStyle = _defaultSpanStyle;
                }

                tbox.BackgroundColor = _backgroundColor;
                tbox.SetController(this);
                tbox.ViewportChanged += (s, e) =>
                {
                    RaiseViewportChanged();
                };

                tbox.ContentSizeChanged += (s, e) =>
                {
                    if (Height < tbox.InnerContentHeight)
                    {
                        this.SetHeight(tbox.InnerContentHeight + 2);
                    }

                    RaiseLayoutFinished();
                };


                if (_textSurfaceListener != null)
                {
                    tbox.TextSurfaceListener = _textSurfaceListener;
                }

                _textEditRenderElement = tbox;

                ReloadDocument();
            }
            return _textEditRenderElement;
        }


        public static TextEditRenderBox GetTextEditRenderBox(TextBox txtbox)
        {
            return txtbox._textEditRenderElement;
        }


        public Run CurrentTextSpan => _textEditRenderElement.CurrentTextRun;

        public void ReplaceCurrentTextRunContent(int nBackspaces, string newstr)
        {
            _textEditRenderElement?.ReplaceCurrentTextRunContent(nBackspaces, newstr);
        }
    }


    public sealed class MaskTextBox : TextBoxBase
    {
        readonly List<char> _actualUserInputText = new List<char>();
        int _keydownCharIndex = 0;

        public MaskTextBox(int width, int height)
            : base(width, height)
        {
            //
            _multiline = false;
            _textSurfaceListener = new TextSurfaceEventListener();
        }
        public override TextSurfaceEventListener TextEventListener
        {
            //mask textbox has its own text box listener
            get => null;
            set { }
        }
        public override void ClearText()
        {
            base.ClearText();
            _actualUserInputText.Clear();
        }

        public override bool HasSomeText => _actualUserInputText.Count > 0;

        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            _keydownCharIndex = _textEditRenderElement.CurrentLineCharIndex;
            base.OnKeyDown(e);
        }
        protected override void OnKeyPress(UIKeyEventArgs e)
        {
            _keydownCharIndex = _textEditRenderElement.CurrentLineCharIndex;
            //eg. mask text
            //we collect actual key and send the mask to to the background 

            if (_keydownCharIndex == _actualUserInputText.Count)
            {
                _actualUserInputText.Add(e.KeyChar);
            }
            else
            {
                _actualUserInputText.Insert(_keydownCharIndex, e.KeyChar);
            }

            e.SetKeyChar('*');

            //
            _textEditRenderElement.HandleKeyPress(e);
            e.CancelBubbling = true;
        }

        public override string Text
        {
            get
            {
                //TODO: review here!...                
                StringBuilder stBuilder = new StringBuilder();
                stBuilder.Append(_actualUserInputText.ToArray());
                return stBuilder.ToString();
            }
            set
            {
                //can not set by code?

            }
        }

        public override RenderElement GetPrimaryRenderElement()
        {
            if (_textEditRenderElement == null)
            {
                var tbox = new TextEditRenderBox(this.Width, this.Height, _multiline);
                tbox.SetLocation(this.Left, this.Top);
                if (_defaultSpanStyle.IsEmpty())
                {
                    _defaultSpanStyle = new TextSpanStyle();
                    _defaultSpanStyle.FontColor = Color.Black;

                    _defaultSpanStyle.ReqFont = GlobalRootGraphic.CurrentRootGfx.DefaultTextEditFontInfo;
                    tbox.CurrentTextSpanStyle = _defaultSpanStyle;
                }
                else
                {
                    tbox.CurrentTextSpanStyle = _defaultSpanStyle;
                }
                tbox.BackgroundColor = _backgroundColor;
                tbox.SetController(this);

                //create 
                tbox.TextSurfaceListener = _textSurfaceListener;
                _textEditRenderElement = tbox;

                _textSurfaceListener.CharacterAdded += (s, e) =>
                {

                };
                _textSurfaceListener.CharacterRemoved += (s, e) =>
                {
                    //remove what?
                    int currentCharIndex = tbox.CurrentLineCharIndex;
                    if (_keydownCharIndex > currentCharIndex)
                    {
                        if (_keydownCharIndex - currentCharIndex == 1)
                        {
                            _actualUserInputText.RemoveAt(_keydownCharIndex - 1);
                        }
                        else
                        {
                            SelectionRangeSnapShot removedRange = e.SelectionSnapShot;
                            _actualUserInputText.RemoveRange(removedRange.startColumnNum, removedRange.endColumnNum - removedRange.startColumnNum);
                        }
                    }
                    else if (_keydownCharIndex == currentCharIndex)
                    {
                        //del
                        SelectionRangeSnapShot removedRange = e.SelectionSnapShot;
                        if (removedRange.endColumnNum == removedRange.startColumnNum)
                        {
                            _actualUserInputText.RemoveAt(_keydownCharIndex);
                        }
                        else
                        {
                            _actualUserInputText.RemoveRange(removedRange.startColumnNum, removedRange.endColumnNum - removedRange.startColumnNum);
                        }
                    }
                    else
                    {

                    }
                };
                _textSurfaceListener.CharacterReplaced += (s, e) =>
                {

                };
                _textSurfaceListener.ReplacedAll += (s, e) =>
                {

                };
            }
            return _textEditRenderElement;
        }
    }




}