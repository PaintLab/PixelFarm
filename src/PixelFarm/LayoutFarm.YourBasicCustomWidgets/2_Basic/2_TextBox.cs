//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{

    public class TextBox : AbstractRectUI
    {
        TextSurfaceEventListener _textSurfaceListener;
        TextEditRenderBox _textEditRenderElement;
        bool _multiline;
        TextSpanStyle _defaultSpanStyle;
        Color _backgroundColor = Color.White;
        string _userTextContent;

        public TextBox(int width, int height, bool multiline)
            : base(width, height)
        {
            _multiline = multiline;

        }
        public void ClearText() => _textEditRenderElement?.ClearAllChildren();

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                if (_textEditRenderElement != null) _textEditRenderElement.BackgroundColor = value;
            }
        }

        public TextSpanStyle DefaultSpanStyle
        {
            get => _defaultSpanStyle;
            set
            {
                _defaultSpanStyle = value;
                if (_textEditRenderElement != null) _textEditRenderElement.CurrentTextSpanStyle = value;
            }
        }
        public ContentTextSplitter TextSplitter
        {
            get;
            set;
        }

        //
        public Point CaretPosition => _textEditRenderElement.CurrentCaretPos;
        public int CurrentLineCharIndex => _textEditRenderElement.CurrentLineCharIndex;
        public int CurrentRunCharIndex => _textEditRenderElement.CurrentTextRunCharIndex;
        public int CurrentLineHeight => _textEditRenderElement.CurrentLineHeight;
        //
        public bool HasSomeText => _textEditRenderElement.HasSomeText;
        //
        /// <summary>
        /// write all lines into stbuilder
        /// </summary>
        /// <param name="stbuilder"></param>
        public void ReadAllTextContent(StringBuilder stBuilder)
        {
            _textEditRenderElement.CopyContentToStringBuilder(stBuilder);
        }

        public string Text
        {
            get
            {
                if (_textEditRenderElement != null)
                {
                    StringBuilder stBuilder = new StringBuilder();
                    ReadAllTextContent(stBuilder);
                    return stBuilder.ToString();
                }
                else
                {
                    return _userTextContent;
                }
            }
            set
            {
                if (_textEditRenderElement == null)
                {
                    _userTextContent = value;
                    return;
                }
                //---------------                 

                _textEditRenderElement.ClearAllChildren();
                //convert to runs
                if (value == null)
                {
                    return;
                }
                //---------------                 
                using (var reader = new System.IO.StringReader(value))
                {
                    string line = reader.ReadLine(); // line
                    int lineCount = 0;
                    while (line != null)
                    {
                        if (lineCount > 0)
                        {
                            _textEditRenderElement.SplitCurrentLineToNewLine();
                        }

                        //create textspan
                        //user can parse text line to smaller span
                        //eg. split by whitespace 

                        if (this.TextSplitter != null)
                        {
                            //parse with textsplitter 
                            //TODO: review here ***
                            //we should encapsulte the detail of this ?
                            //1.technique, 2. performance
                            //char[] buffer = value.ToCharArray();
                            char[] buffer = line.ToCharArray();
                            if (buffer.Length == 0)
                            {

                            }
                            foreach (Composers.TextSplitBound splitBound in TextSplitter.ParseWordContent(buffer, 0, buffer.Length))
                            {
                                int startIndex = splitBound.startIndex;
                                int length = splitBound.length;
                                char[] splitBuffer = new char[length];
                                Array.Copy(buffer, startIndex, splitBuffer, 0, length);

                                //TODO: review
                                //this just test ***  that text box can hold freeze text run
                                //var textspan = textEditRenderElement.CreateFreezeTextRun(splitBuffer);
                                //-----------------------------------
                                //but for general 

                                EditableRun textRun = new EditableTextRun(_textEditRenderElement.Root,
                                    splitBuffer,
                                    _textEditRenderElement.CurrentTextSpanStyle);
                                textRun.UpdateRunWidth();
                                _textEditRenderElement.AddTextRun(textRun);
                            }
                        }
                        else
                        {

                            var textRun = new EditableTextRun(_textEditRenderElement.Root,
                                line,
                                  _textEditRenderElement.CurrentTextSpanStyle);
                            textRun.UpdateRunWidth();
                            _textEditRenderElement.AddTextRun(textRun);
                        }

                        lineCount++;
                        line = reader.ReadLine();
                    }
                }
                this.InvalidateGraphics();
            }
        }
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


        //        
        public override int ViewportX => _textEditRenderElement.ViewportX;
        //
        public override int ViewportY => _textEditRenderElement.ViewportY;
        //
        public override int InnerHeight => (_textEditRenderElement != null) ? _textEditRenderElement.InnerContentSize.Height : base.InnerHeight;
        //
        protected override bool HasReadyRenderElement => _textEditRenderElement != null;
        //
        public override RenderElement CurrentPrimaryRenderElement => _textEditRenderElement;
        //
        public override void SetViewport(int x, int y, object reqBy)
        {
            _textEditRenderElement?.SetViewport(x, y);

        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_textEditRenderElement == null)
            {
                var tbox = new TextEditRenderBox(rootgfx, this.Width, this.Height, _multiline);
                tbox.SetLocation(this.Left, this.Top);
                tbox.HasSpecificWidthAndHeight = true;
                if (_defaultSpanStyle.IsEmpty())
                {
                    _defaultSpanStyle = new TextSpanStyle();
                    _defaultSpanStyle.FontColor = Color.Black;
                    _defaultSpanStyle.ReqFont = rootgfx.DefaultTextEditFontInfo;
                    tbox.CurrentTextSpanStyle = _defaultSpanStyle;
                }
                else
                {
                    tbox.CurrentTextSpanStyle = _defaultSpanStyle;
                }
                tbox.BackgroundColor = _backgroundColor;
                tbox.SetController(this);
                tbox.ViewportChanged += (s, e) => RaiseViewportChanged();
                tbox.ContentSizeChanged += (s, e) =>
                {
                    RaiseLayoutFinished();
                };


                if (_textSurfaceListener != null)
                {
                    tbox.TextSurfaceListener = _textSurfaceListener;
                }
                _textEditRenderElement = tbox;
                if (_userTextContent != null)
                {
                    this.Text = _userTextContent;
                    _userTextContent = null;//clear
                }
            }
            return _textEditRenderElement;
        }
        //----------------------------------------------------------------
        public bool IsMultilineTextBox => _multiline;

        public static TextEditRenderBox GetTextEditRenderBox(TextBox txtbox)
        {
            return txtbox._textEditRenderElement;
        }
        public static InternalTextLayerController GetInternalTextLayerController(TextBox txtbox)
        {
            return txtbox._textEditRenderElement.TextLayerController;
        }

        public void FindCurrentUnderlyingWord(out int startAt, out int len)
        {
            _textEditRenderElement.FindCurrentUnderlyingWord(out startAt, out len);
        }

        public TextSurfaceEventListener TextEventListener
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
        public EditableRun CurrentTextSpan => _textEditRenderElement.CurrentTextRun;

        public void ReplaceCurrentTextRunContent(int nBackspaces, string newstr)
        {
            _textEditRenderElement?.ReplaceCurrentTextRunContent(nBackspaces, newstr);
        }
        //public void ReplaceCurrentLineTextRuns(IEnumerable<EditableRun> textRuns)
        //{
        //    if (_textEditRenderElement != null)
        //    {
        //        _textEditRenderElement.ReplaceCurrentLineTextRuns(textRuns);
        //    }
        //}
        public void CopyCurrentLine(StringBuilder stbuilder)
        {
            _textEditRenderElement.CopyCurrentLine(stbuilder);
        }

        public void FormatCurrentSelection(TextSpanStyle spanStyle)
        {
            //TODO: reimplement text-model again
            _textEditRenderElement.TextLayerController.DoFormatSelection(spanStyle);

        }
        public void FormatCurrentSelection(TextSpanStyle spanStyle, FontStyle toggleFontStyle)
        {
            //TODO: reimplement text-model again
            _textEditRenderElement.TextLayerController.DoFormatSelection(spanStyle, toggleFontStyle);
        }
        //---------------------------------------------------------------- 
        protected override void OnMouseLeave(UIMouseEventArgs e)
        {
            e.MouseCursorStyle = MouseCursorStyle.Arrow;
        }
        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            _textEditRenderElement.HandleDoubleClick(e);
            e.CancelBubbling = true;
        }
        protected override void OnMouseWheel(UIMouseEventArgs e)
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
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            this.Focus();
            e.MouseCursorStyle = MouseCursorStyle.IBeam;
            e.CancelBubbling = true;
            e.CurrentContextElement = this;
            _textEditRenderElement.HandleMouseDown(e);
        }
        protected override void OnLostKeyboardFocus(UIFocusEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            _textEditRenderElement.Blur();
        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                _textEditRenderElement.HandleDrag(e);
                e.CancelBubbling = true;
                e.MouseCursorStyle = MouseCursorStyle.IBeam;
            }
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
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


        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "textbox");
            this.Describe(visitor);
            visitor.TextNode(this.Text);
            visitor.EndElement();
        }
    }


    public class MaskTextBox : AbstractRectUI
    {
        TextSurfaceEventListener _textSurfaceListener;
        TextEditRenderBox _textEditRenderElement;

        bool _multiline;
        TextSpanStyle _defaultSpanStyle;
        Color _backgroundColor = Color.White;


        List<char> _actualUserInputText = new List<char>();
        int _keydownCharIndex = 0;

        public MaskTextBox(int width, int height)
            : base(width, height)
        {
            //
            _multiline = false;
            _textSurfaceListener = new TextSurfaceEventListener();
        }
        public void ClearText()
        {
            _textEditRenderElement?.ClearAllChildren();
            _actualUserInputText.Clear();
        }
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                if (_textEditRenderElement != null)
                {
                    _textEditRenderElement.BackgroundColor = value;
                }
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
        public ContentTextSplitter TextSplitter
        {
            get;
            set;
        }
        //
        public int CurrentLineHeight => _textEditRenderElement.CurrentLineHeight;
        //
        public Point CaretPosition => _textEditRenderElement.CurrentCaretPos;
        //
        public int CurrentLineCharIndex => _textEditRenderElement.CurrentLineCharIndex;
        //
        public int CurrentRunCharIndex => _textEditRenderElement.CurrentTextRunCharIndex;
        //
        public bool HasSomeText => _actualUserInputText.Count > 0;

        public string Text
        {
            get
            {
                //TODO: review here!...
                StringBuilder stBuilder = new StringBuilder();
                stBuilder.Append(_actualUserInputText.ToArray());
                return stBuilder.ToString();
            }
        }
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

        protected override bool HasReadyRenderElement => _textEditRenderElement != null;
        //
        public override RenderElement CurrentPrimaryRenderElement => _textEditRenderElement;
        //
        public TextSurfaceEventListener TextSurfaceEventListener => _textSurfaceListener;
        //
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_textEditRenderElement == null)
            {
                var tbox = new TextEditRenderBox(rootgfx, this.Width, this.Height, _multiline);
                tbox.SetLocation(this.Left, this.Top);
                tbox.HasSpecificWidthAndHeight = true;
                if (_defaultSpanStyle.IsEmpty())
                {
                    _defaultSpanStyle = new TextSpanStyle();
                    _defaultSpanStyle.ReqFont = rootgfx.DefaultTextEditFontInfo;
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
                            VisualSelectionRangeSnapShot removedRange = e.SelectionSnapShot;
                            _actualUserInputText.RemoveRange(removedRange.startColumnNum, removedRange.endColumnNum - removedRange.startColumnNum);
                        }
                    }
                    else if (_keydownCharIndex == currentCharIndex)
                    {
                        //del
                        VisualSelectionRangeSnapShot removedRange = e.SelectionSnapShot;
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
        //----------------------------------------------------------------
        public bool IsMultilineTextBox => _multiline;
        //
        public void FindCurrentUnderlyingWord(out int startAt, out int len)
        {
            _textEditRenderElement.FindCurrentUnderlyingWord(out startAt, out len);
        }
        //---------------------------------------------------------------- 
        protected override void OnMouseLeave(UIMouseEventArgs e)
        {
            e.MouseCursorStyle = MouseCursorStyle.Arrow;
        }
        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            _textEditRenderElement.HandleDoubleClick(e);
            e.CancelBubbling = true;
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


        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            _keydownCharIndex = _textEditRenderElement.CurrentLineCharIndex;
            //
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
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            this.Focus();
            e.MouseCursorStyle = MouseCursorStyle.IBeam;
            e.CancelBubbling = true;
            e.CurrentContextElement = this;
            _textEditRenderElement.HandleMouseDown(e);
        }
        protected override void OnLostKeyboardFocus(UIFocusEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            _textEditRenderElement.Blur();
        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                _textEditRenderElement.HandleDrag(e);
                e.CancelBubbling = true;
                e.MouseCursorStyle = MouseCursorStyle.IBeam;
            }
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
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


        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "textbox_password");
            this.Describe(visitor);
            visitor.TextNode(this.Text);
            visitor.EndElement();
        }
    }
}