//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.TextEditing;
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


        internal TextBoxBase(int width, int height)
            : base(width, height)
        {
        }

        public int LineCount => _textEditRenderElement.LineCount;

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
        public ContentTextSplitter TextSplitter
        {
            get;
            set;
        }

        public bool IsMultilineTextBox => _multiline;
        //
        public int CurrentLineHeight => _textEditRenderElement.CurrentLineHeight;
        //
        public Point CaretPosition => _textEditRenderElement.CurrentCaretPos;
        //
        public int CurrentLineCharIndex => _textEditRenderElement.CurrentLineCharIndex;
        //
        public int CurrentRunCharIndex => _textEditRenderElement.CurrentTextRunCharIndex;
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

        public Size InnerBackgroundSize => (_textEditRenderElement != null) ? _textEditRenderElement.InnerBackgroundSize : new Size(this.Width, this.Height);
        public override int InnerWidth => (_textEditRenderElement != null) ? _textEditRenderElement.InnerContentSize.Width : base.InnerWidth;
        public override int InnerHeight => (_textEditRenderElement != null) ? _textEditRenderElement.InnerContentSize.Height : base.InnerHeight;

        public abstract string Text { get; set; }
        public abstract void SetText(IEnumerable<string> lines);
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
        public TextEditing.Commands.DocumentCommandListener DocCmdListener
        {
            get => _textEditRenderElement.DocCommandListener;
            set => _textEditRenderElement.DocCommandListener = value;
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

        internal bool IsSharedTextBox { get; set; }
        internal bool IsInTextBoxPool { get; set; }
    }




    public sealed class TextBox : TextBoxBase
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
        public void CopyContentTo(StringBuilder stBuilder)
        {
            _textEditRenderElement.CopyContentToStringBuilder(stBuilder);
        }
#if DEBUG
        public override void SetLocation(int left, int top)
        {
            base.SetLocation(left, top);
        }
#endif

        RunStyle _runStyle;
        RunStyle GetDefaultRunStyle()
        {
            if (_runStyle == null)
            {
                return _runStyle = new RunStyle(_textEditRenderElement.Root.TextServices)
                {
                    FontColor = DefaultSpanStyle.FontColor,
                    ReqFont = DefaultSpanStyle.ReqFont,
                    ContentHAlign = DefaultSpanStyle.ContentHAlign,
                };
            }
            else
            {
                return _runStyle;
            }
        }


        public override void SetText(IEnumerable<string> lines)
        {
            _doc = PlainTextDocumentHelper.CreatePlainTextDocument(lines);
            ReloadDocument();
            //if (_textEditRenderElement == null)
            //{
            //    _userTextContent2 = new List<string>();//copy content to here
            //    foreach (string line in lines)
            //    {
            //        //TEMP FIX, 
            //        //TODO: review here...
            //        string line1 = line.Replace("\t", "    ");
            //        _userTextContent2.Add(line1);
            //    }
            //    return;
            //}
            ////---------------                 
            //_textEditRenderElement.ClearAllChildren();
            ////convert to runs
            //if (lines == null)
            //{
            //    _userTextContent = null;
            //    _userTextContent2 = null;
            //    return;
            //}
            ////---------------     
            //int lineCount = 0;
            //foreach (string line in lines)
            //{
            //    if (lineCount > 0)
            //    {
            //        _textEditRenderElement.SplitCurrentLineToNewLine();
            //    }

            //    //create textspan
            //    //user can parse text line to smaller span
            //    //eg. split by whitespace 
            //    if (line.Length > 0)
            //    {
            //        if (this.TextSplitter != null)
            //        {
            //            //parse with textsplitter 
            //            //TODO: review here ***
            //            //we should encapsulte the detail of this ?
            //            //1.technique, 2. performance
            //            //char[] buffer = value.ToCharArray();
            //            char[] buffer = line.ToCharArray();

            //            RunStyle runStyle = GetDefaultRunStyle();

            //            foreach (Composers.TextSplitBounds splitBounds in TextSplitter.ParseWordContent(buffer, 0, buffer.Length))
            //            {
            //                int startIndex = splitBounds.startIndex;
            //                int length = splitBounds.length;

            //                //copy? to new textrun? word-by-word?
            //                char[] splitBuffer = new char[length];

            //                Array.Copy(buffer, startIndex, splitBuffer, 0, length);
            //                //TODO: review
            //                //this just test ***  that text box can hold freeze text run
            //                //var textspan = textEditRenderElement.CreateFreezeTextRun(splitBuffer);
            //                //-----------------------------------
            //                //but for general  

            //                var textRun = new TextRun(runStyle, splitBuffer);
            //                textRun.UpdateRunWidth();
            //                _textEditRenderElement.AddTextRun(textRun);
            //            }
            //        }
            //        else
            //        {

            //            RunStyle runStyle = GetDefaultRunStyle();
            //            //replace 1 tab with 4 blank spaces?
            //            string line1 = line.Replace("\t", "    ");
            //            var textRun = new TextRun(runStyle, line1);
            //            textRun.UpdateRunWidth();
            //            _textEditRenderElement.AddTextRun(textRun);
            //        }
            //    }
            //    lineCount++;
            //}

            //this.InvalidateGraphics();
        }
        public override string Text
        {
            get
            {
                if (_textEditRenderElement != null)
                {
                    //TODO, use string builder pool
                    StringBuilder stBuilder = new StringBuilder();
                    CopyContentTo(stBuilder);
                    return stBuilder.ToString();
                }
                else
                {
                    //TODO, use string builder pool
                    StringBuilder stBuilder = new StringBuilder();
                    _doc.CopyAllText(stBuilder);
                    return stBuilder.ToString();
                }
            }
            set
            {
                if (_textEditRenderElement == null)
                {
                    _doc = PlainTextDocumentHelper.CreatePlainTextDocument(value);
                    return;
                }
                //---------------                 
                if (value == null)
                {
                    _doc = new PlainTextDocument();
                    return;
                }
                _doc = PlainTextDocumentHelper.CreatePlainTextDocument(value);
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
            int lineCount = 0;

            RunStyle runstyle = GetDefaultRunStyle();
            foreach (PlainTextLine line in _doc.GetLineIter())
            {
                if (lineCount > 0)
                {
                    _textEditRenderElement.SplitCurrentLineToNewLine();
                }
                //we create an unparse text run***
                _textEditRenderElement.AddTextLine(line);
            }

            //using (var reader = new System.IO.StringReader(value))
            //{
            //    string line = reader.ReadLine(); // line
            //    int lineCount = 0;
            //    while (line != null)
            //    {
            //        if (lineCount > 0)
            //        {
            //            _textEditRenderElement.SplitCurrentLineToNewLine();
            //        }

            //        //create textspan
            //        //user can parse text line to smaller span
            //        //eg. split by whitespace 
            //        if (line.Length > 0)
            //        {

            //            if (this.TextSplitter != null)
            //            {
            //                //parse with textsplitter 
            //                //TODO: review here ***
            //                //we should encapsulte the detail of this ?
            //                //1.technique, 2. performance
            //                //char[] buffer = value.ToCharArray();
            //                char[] buffer = line.ToCharArray();
            //                RunStyle runstyle = GetDefaultRunStyle();
            //                //
            //                foreach (Composers.TextSplitBounds splitBound in TextSplitter.ParseWordContent(buffer, 0, buffer.Length))
            //                {
            //                    int startIndex = splitBound.startIndex;
            //                    int length = splitBound.length;
            //                    char[] splitBuffer = new char[length];
            //                    Array.Copy(buffer, startIndex, splitBuffer, 0, length);

            //                    //TODO: review
            //                    //this just test ***  that text box can hold freeze text run
            //                    //var textspan = textEditRenderElement.CreateFreezeTextRun(splitBuffer);
            //                    //-----------------------------------
            //                    //but for general 

            //                    var textRun = new TextRun(runstyle, splitBuffer);
            //                    textRun.UpdateRunWidth();
            //                    _textEditRenderElement.AddTextRun(textRun);
            //                }
            //            }
            //            else
            //            {
            //                RunStyle runstyle = GetDefaultRunStyle();
            //                var textRun = new TextRun(runstyle, line);
            //                textRun.UpdateRunWidth();
            //                _textEditRenderElement.AddTextRun(textRun);
            //            }
            //        }

            //        lineCount++;
            //        line = reader.ReadLine();
            //    }
            //}
            this.InvalidateGraphics();
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_textEditRenderElement == null)
            {
                var tbox = new TextEditRenderBox(rootgfx, this.Width, this.Height, _multiline, _isEditable);
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
                tbox.ViewportChanged += (s, e) =>
                {
                    RaiseViewportChanged();
                };

                tbox.ContentSizeChanged += (s, e) =>
                {
                    if (Height < tbox.InnerContentSize.Height)
                    {
                        this.SetHeight(tbox.InnerContentSize.Height + 2);
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

        public static TextFlowEditSession GetEditSession(TextBox txtbox) => TextEditRenderBox.GetCurrentEditSession(txtbox._textEditRenderElement);

        public Run CurrentTextSpan => _textEditRenderElement.CurrentTextRun;

        public void ReplaceCurrentTextRunContent(int nBackspaces, string newstr)
        {
            _textEditRenderElement?.ReplaceCurrentTextRunContent(nBackspaces, newstr);
        }

        public void CopyCurrentLine(StringBuilder stbuilder)
        {
            _textEditRenderElement.CopyCurrentLine(stbuilder);
        }

        //public void FormatCurrentSelection(TextSpanStyle spanStyle)
        //{
        //    //TODO: reimplement text-model again
        //    _textEditRenderElement.TextLayerController.DoFormatSelection(spanStyle);

        //}
        //public void FormatCurrentSelection(TextSpanStyle spanStyle, FontStyle toggleFontStyle)
        //{
        //    //TODO: reimplement text-model again
        //    _textEditRenderElement.TextLayerController.DoFormatSelection(spanStyle, toggleFontStyle);
        //}


    }


    public sealed class MaskTextBox : TextBoxBase
    {
        List<char> _actualUserInputText = new List<char>();
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
        public override void SetText(IEnumerable<string> lines)
        {

            //not support in this version
            //TODO: review here
#if DEBUG
            System.Diagnostics.Debug.WriteLine("maskTextBox_setText:");
#endif

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
    }




}