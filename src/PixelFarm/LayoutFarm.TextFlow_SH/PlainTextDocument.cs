//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Typography.Text;

namespace LayoutFarm.TextFlow
{
    public enum PlainTextLineEnd : byte
    {
        None,
        /// <summary>
        /// \r\n
        /// </summary>
        CRLF,
        /// <summary>
        /// \n
        /// </summary>
        LF,
    }

    class PlainTextLine
    {
        public PlainTextLine() { }
        public TextBufferSpan Content { get; set; }
        public PlainTextLineEnd EndWith { get; set; }
        /// <summary>
        /// write content to output
        /// </summary>
        /// <param name="output"></param>
        public void WriteTo(StringBuilder output)
        {
            PlainTextUtils.WriteTo(Content, output);
            //TODO: review endline
        }
        public int CharCount => Content.len;

        public override string ToString() => PlainTextUtils.ReadString(Content);

    }

    static class PlainTextUtils
    {
        [ThreadStatic]
        static Stack<StringBuilder> s_sb_pool;
        [ThreadStatic]
        static Stack<TextCopyBufferUtf32> s_copyBuffer_pool;

        [ThreadStatic]
        static bool s_init;

        static void Init()
        {
            s_init = true;
            s_sb_pool = new Stack<StringBuilder>();
            s_copyBuffer_pool = new Stack<TextCopyBufferUtf32>();
        }
        //
        static StringBuilder GetFreeStringBuilder()
        {
            if (!s_init) { Init(); }
            if (s_sb_pool.Count == 0)
            {
                return new StringBuilder();
            }
            else
            {
                return s_sb_pool.Pop();
            }
        }
        static void ReleaseStringBuilder(StringBuilder sb)
        {
            if (!s_init) { Init(); }
            sb.Length = 0;//reset
            s_sb_pool.Push(sb);
        }
        //
        static TextCopyBufferUtf32 GetFreeCopyBuffer()
        {
            if (!s_init) { Init(); }
            if (s_copyBuffer_pool.Count == 0)
            {
                return new TextCopyBufferUtf32();
            }
            else
            {
                return s_copyBuffer_pool.Pop();
            }
        }
        static void ReleaseCopyBuffer(TextCopyBufferUtf32 sb)
        {
            if (!s_init) { Init(); }
            sb.Clear();
            s_copyBuffer_pool.Push(sb);
        }

        //
        public static string ReadString(TextBufferSpan charSegment)
        {
            if (!s_init) { Init(); }

            TextCopyBufferUtf32 copyBuff = GetFreeCopyBuffer();
            charSegment.WriteTo(copyBuff);
            string result = ReadString(copyBuff);
            ReleaseCopyBuffer(copyBuff);
            return result;
        }
        public static string ReadString(TextCopyBufferUtf32 copyBuffer)
        {
            if (!s_init) { Init(); }

            StringBuilder sb = GetFreeStringBuilder();
            copyBuffer.CopyTo(sb);
            string result = sb.ToString();
            ReleaseStringBuilder(sb);
            return result;
        }
        public static string ReadCurrentLine(PlainTextEditSession block)
        {
            if (!s_init) { Init(); }

            StringBuilder sb = GetFreeStringBuilder();
            TextCopyBufferUtf32 copyBuff = GetFreeCopyBuffer();
            block.ReadCurrentLine(copyBuff);
            copyBuff.CopyTo(sb);
            string result = sb.ToString();
            ReleaseCopyBuffer(copyBuff);
            ReleaseStringBuilder(sb);
            return result;
        }

        public static void WriteTo(TextBufferSpan charSegment, StringBuilder sb)
        {
            if (!s_init) { Init(); }

            // 
            TextCopyBufferUtf32 copyBuff = GetFreeCopyBuffer();
            charSegment.WriteTo(copyBuff);
            copyBuff.CopyTo(sb);
            ReleaseCopyBuffer(copyBuff);
        }
    }




    class LineEditor
    {
        PlainTextEditSession _textBlock;//owner
        PlainTextLine _line;

        //TODO: review a proper data structure again,use tree of character? (eg RB tree?)
        //esp for a long line insertion, deletion

        readonly TextCopyBufferUtf32 _arr = new TextCopyBufferUtf32();
        bool _changed;
        bool _loaded;
        int _initContentLen;

        /// <summary>
        /// begin at 0 for new char index
        /// </summary>
        int _newCharIndex;//new character index 1:1 based on backend buffer (utf16 or utf32)
        internal LineEditor()
        {

        }
        internal void Bind(PlainTextEditSession textBlock)
        {
            _textBlock = textBlock;
        }
        public void Read(TextCopyBufferUtf32 output)
        {
            //read data from this line and write to output
            if (_loaded)
            {
                output.Append(_arr);
            }
            else
            {
                //not load then
                if (_line.Content.len > 0)
                {
                    _line.Content.WriteTo(output);
                }
            }

        }
        public void Read(TextCopyBufferUtf32 output, int index, int len)
        {
            if (_loaded)
            {
                output.Append(_arr, index, len);
            }
            else
            {
                _line.Content.WriteTo(output, index, len);
            }
        }
        public void Bind(PlainTextLine line)
        {
            if (_line == line)
            {
                return;//same line
            }
            //
            if (_line != null && _changed)
            {
                //update content back             
                _line.Content = _textBlock.CharSource.NewSegment(_arr);
            }
            //
            _line = line;
            //reset
            _newCharIndex = 0;
            _initContentLen = line.Content.len;
            _changed = false;
            _loaded = false;
            _arr.Clear();

            NewCharIndex = 0;
        }

        internal PlainTextLine CurrentLine => _line;

        /// <summary>
        /// load content of each line to edit mode
        /// </summary>
        void Load()
        {
            if (_loaded) { return; }

            //

            TextBufferSpan content = _line.Content;
            if (content.len > 0)
            {
                //copy content to temp buffer
                content.WriteTo(_arr);
            }
            _loaded = true;

        }
        /// <summary>
        /// character index for new insertion
        /// </summary>
        public int NewCharIndex
        {
            get => _newCharIndex;
            set
            {
                if (value <= ((!_loaded) ? _initContentLen : _arr.Length))
                {
                    _newCharIndex = value;
                }
                else if (value == _arr.Length + 1)
                {
                    //to the end of this
                    _newCharIndex = value;
                }
                else
                {
                    //not accept
                    //warn, and set default
                }
            }
        }

        public int GetChar(int index) => (index < _arr.Length) ? _arr.GetChar(index) : 0;
        public int GetCurrentChar() => (_newCharIndex < _arr.Length) ? _arr.GetChar(_newCharIndex) : 0;

        public bool CharIndexOnTheEnd => _newCharIndex == CharacterCount;

        public void AddText(TextCopyBuffer copyBuffer, int start, int len)
        {
            if (copyBuffer is TextCopyBufferUtf32 u32Buffer)
            {
                AddText(u32Buffer, start, len);
            }
            else
            {
                AddText((TextCopyBufferUtf16)copyBuffer, start, len);
            }
        }
        public void AddText(TextCopyBufferUtf32 copyBuffer, int start, int len)
        {
            if (!_loaded) { Load(); }//

            _arr.Insert(copyBuffer, _newCharIndex, start, len);
            _newCharIndex += len;
            _changed = true;
        }
        public void AddText(TextCopyBufferUtf16 copyBuffer, int start, int len)
        {
            //TODO: review here
            throw new NotSupportedException();
        }
        public void Split(TextCopyBufferUtf32 rightPart)
        {
            if (!_loaded) { Load(); }//

            int rightPartLen = _arr.Length - _newCharIndex;
            rightPart.Append(_arr, _newCharIndex, rightPartLen);
            _arr.Remove(_newCharIndex, rightPartLen);
            _changed = true;
        }

        public void DeleteRange(int start, int len)
        {
            if (!_loaded) { Load(); }

            if (len == 0) return;
            if (len < 0) { throw new NotSupportedException(); }

            _arr.Remove(start, len);
            _newCharIndex = start;
            _changed = true;
        }
        /// <summary>
        /// single backspace
        /// </summary>
        public void DoBackSpace()
        {
            if (!_loaded) { Load(); }

            if (_newCharIndex < 1) { return; }//early exit
            //
            _arr.Remove(_newCharIndex - 1, 1);
            _newCharIndex--;
            _changed = true;
        }
        /// <summary>
        /// add character to current index
        /// </summary>
        /// <param name="c"></param>
        public void AddChar(int c)
        {

            if (!_loaded) { Load(); }

            if (_arr.Length == _newCharIndex)
            {
                //the last one
                _arr.Append(c);
            }
            else
            {
                _arr.Insert(_newCharIndex, c);
            }
            _newCharIndex++;
            _changed = true;
        }
        public void Clear()
        {
            if (!_loaded) { Load(); }//

            _arr.Clear();
            _newCharIndex = 0;
            _changed = true;
        }
        /// <summary>
        /// character count
        /// </summary>
        public int CharacterCount => _loaded ? _arr.Length : _initContentLen;
    }

    public class PlainTextDocument
    {
        ForwardOnlyCharSource _charSource = new ForwardOnlyCharSource();
        readonly List<PlainTextLine> _lines = new List<PlainTextLine>();

        public PlainTextDocument(string text)
        {
            //extract text and copy to character source
            //TODO: review here again
            using (System.IO.StringReader reader = new System.IO.StringReader(text))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    //...        
                    _lines.Add(new PlainTextLine() { Content = _charSource.NewSegment(new TextBufferSpan(line.ToCharArray())) });
                    line = reader.ReadLine();
                }
            }
        }
        public PlainTextDocument(TextCopyBuffer copyBuffer)
        {
            //copy 
            if (copyBuffer.Length == 0)
            {
                _lines.Add(new PlainTextLine());
                return;
            }
            if (copyBuffer is TextCopyBufferUtf32 utf32)
            {
                copyBuffer.GetReader(out Typography.TextBreak.InputReader reader);
                while (reader.ReadLine(out int begin, out int len, out Typography.TextBreak.InputReader.LineEnd endWith))
                {
                    PlainTextLine p = new PlainTextLine() { Content = _charSource.NewSegment(utf32, begin, len) };
                    _lines.Add(p);
                }
            }
            else if (copyBuffer is TextCopyBufferUtf16 utf16)
            {
                copyBuffer.GetReader(out Typography.TextBreak.InputReader reader);
                while (reader.ReadLine(out int begin, out int len, out Typography.TextBreak.InputReader.LineEnd endWith))
                {
                    //...       
                    PlainTextLine p = new PlainTextLine() { Content = _charSource.NewSegment(utf16, begin, len) };
                    _lines.Add(p);
                }
            }

        }
        public int LineCount => _lines.Count;
        internal List<PlainTextLine> UnsafeInternalList => _lines;
        public ForwardOnlyCharSource CharSource => _charSource;

        public void WriteTo(StringBuilder output)
        {
            int j = _lines.Count;
            for (int i = 0; i < j; ++i)
            {
                if (i > 0)
                {
                    //TODO: review line-end ***
                    output.AppendLine();
                }
                _lines[i].WriteTo(output);
            }
        }

        public void WriteLineTo(int lineIndex, StringBuilder output)
        {
            _lines[lineIndex].WriteTo(output);
        }
    }


    public class PlainTextEditSession : ITextFlowEditSession
    {
        int _currentLineNo;
        PlainTextLine _currentLine;
        LineEditor _lineEditor = new LineEditor();
        TextCopyBufferUtf32 _copyBuffer = new TextCopyBufferUtf32();//**

        List<PlainTextLine> _lines;
        PlainTextDocument _doc;

        class PoolPrivateType { }

        static PlainTextEditSession()
        {
            ObjectPool<TextCopyBufferUtf32, PoolPrivateType>.SetDelegates(
                () => new TextCopyBufferUtf32(),
                buff => buff.Clear());
        }
        public PlainTextEditSession()
        {
            TempCopyBuffer = new TextCopyBufferUtf32();
        }
        public void LoadPlainText(PlainTextDocument doc)
        {

            _lineEditor = new LineEditor();
            _lineEditor.Bind(this);
            _doc = doc;
            _lines = doc.UnsafeInternalList;
            //move to 
            if (_lines.Count == 0)
            {
                //create default
                _lines.Add(new PlainTextLine());
                CurrentLineNumber = 0;
            }

            CurrentLineNumber = 0;
            NewCharIndex = 0;
        }
        public void Clear()
        {
            _lines.Clear();
            _lines.Add(new PlainTextLine()); //default
            CurrentLineNumber = 0;
        }
        public int GetLineCharCount(int lineNo)
        {
            if (lineNo < _lines.Count)
            {
                return _lines[lineNo].CharCount;
            }
            else
            {
                return -1;
            }
        }
        internal ForwardOnlyCharSource CharSource => _doc.CharSource;

        /// <summary>
        /// new character index of current line
        /// </summary>
        public int NewCharIndex
        {
            get => _lineEditor.NewCharIndex;
            set => _lineEditor.NewCharIndex = value;
        }
        public TextCopyBufferUtf32 TempCopyBuffer { get; set; }

        public bool IsOnTheEnd() => _lineEditor.CharIndexOnTheEnd;
        public void ReadCurrentLine(TextCopyBufferUtf32 output) => _lineEditor.Read(output);
        public void ReadCurrentLine(TextCopyBufferUtf32 output, int index, int len) => _lineEditor.Read(output, index, len);

        public int LineCount => _lines.Count;
        public int CurrentLineCharCount => _lineEditor.CharacterCount;
        public int CurrentLineNumber
        {
            get => _currentLineNo;
            set
            {
                if (value >= 0 && value < _lines.Count)
                {
                    _currentLineNo = value;

                    PlainTextLine line = _lines[value];
                    if (_currentLine != line)
                    {
                        //change 
                        _currentLine = line;
                        _lineEditor.Bind(line);
                    }
                }
            }
        }
        public void ReplaceCurrentLine(TextCopyBuffer copyBuffer)
        {
            _lineEditor.Clear();
            _lineEditor.AddText(copyBuffer, 0, copyBuffer.Length);
        }
        public int GetCharacter(int lineCharIndex) => _lineEditor.GetChar(lineCharIndex);
        public void AddChar(int c)
        {
            if (!_selection.isEmpty)
            {
                DeleteSelectedText();
            }

            if (_currentLine != null)
            {
                //1.  
                _lineEditor.AddChar(c);
            }
            else
            {
                //TODO: review this
                throw new NotSupportedException();
            }
        }

        public bool CanAcceptThisChar(int c) => CanCaretStopOnThisChar(c);

        public void AddText(string s)
        {
            _copyBuffer.Clear();
            _copyBuffer.Append(s);
            AddText(_copyBuffer);
        }
        public void AddText(char[] s)
        {
            _copyBuffer.Clear();
            _copyBuffer.Append(s);
            AddText(_copyBuffer);
        }
        public void AddText(TextCopyBuffer copy)
        {
            if (!_selection.isEmpty)
            {
                DeleteSelectedText();
            }

            //may by more than one line
            copy.GetReader(out Typography.TextBreak.InputReader reader);
            while (reader.ReadLine(out int begin, out int len, out Typography.TextBreak.InputReader.LineEnd lineEnd))
            {
                _lineEditor.AddText(copy, begin, len);

                if (lineEnd != Typography.TextBreak.InputReader.LineEnd.None)
                {
                    //insert new line
                    if (_currentLineNo == _lines.Count - 1)
                    {
                        //now we are in the last line
                        SplitIntoNewLine();//split and move to new line
                    }
                    else
                    {
                        _lines.Insert(CurrentLineNumber + 1, new PlainTextLine());
                        CurrentLineNumber++;//move to lower
                    }
                }
            }
        }


        public bool DoBackspace()
        {

            if (!_selection.isEmpty)
            {
                return DeleteSelectedText();
            }


            if (_lineEditor.NewCharIndex > 0)
            {
                //on current line
                _lineEditor.DoBackSpace();

                return true;
            }
            else if (CurrentLineNumber == 0)
            {
                //no more upper line
                return false;
            }

            //---------
            //we are on current line >0  and char_index =0;
            //we must bring the content from this line to upper line

            int charCount = _lineEditor.CharacterCount;
            if (charCount > 0)
            {
                //copy content of current line
                _copyBuffer.Clear();
                _lineEditor.Read(_copyBuffer);
            }

            CurrentLineNumber--;
            //move newchar index to end line                
            _lineEditor.NewCharIndex = _lineEditor.CharacterCount;
            if (charCount > 0)
            {
                //paste content from the lower line
                int pos = _lineEditor.NewCharIndex;
                _lineEditor.AddText(_copyBuffer, 0, _copyBuffer.Length);
                _lineEditor.NewCharIndex = pos;//move to latest pos
            }
            //and delete the lower line
            _lines.RemoveAt(CurrentLineNumber + 1);
            return true;
        }
        void InternalDoBackspace(int deleteCharCount)
        {
            for (int i = 0; i < deleteCharCount; ++i)
            {
                _lineEditor.DoBackSpace();
            }
        }
        bool DeleteSelectedText()
        {
            if (_selection.isEmpty) { return false; }

            //
            _selection.Normalize();

            int diff = _selection.endLineNo - _selection.startLineNo;

            if (diff == 0)
            {
                //on the sameline
                CurrentLineNumber = _selection.startLineNo;
                _lineEditor.DeleteRange(_selection.startLineNewCharIndex, _selection.endLineNewCharIndex - _selection.startLineNewCharIndex);
                _selection.Finish();
            }
            else if (diff == 1)
            {
                CurrentLineNumber = _selection.endLineNo;
                //delete end part
                _lineEditor.DeleteRange(0, _selection.endLineNewCharIndex);//**
                CurrentLineNumber = _selection.startLineNo;
                //delete begin part, from start to end of this line
                _lineEditor.DeleteRange(_selection.startLineNewCharIndex, _lineEditor.CharacterCount - _selection.startLineNewCharIndex);
                _selection.Finish();

                NewCharIndex = _lineEditor.CharacterCount;//move to end of this line
                DoDelete();//single delete to join lower line to upper
            }
            else
            {
                //delete in-between lines
                for (int i = 0; i < diff - 1; ++i)
                {
                    //diff-1 times
                    _lines.RemoveAt(_selection.startLineNo + 1);
                }

                CurrentLineNumber = _selection.endLineNo;
                //delete end part
                _lineEditor.DeleteRange(0, _selection.endLineNewCharIndex);//**
                CurrentLineNumber = _selection.startLineNo;
                //delete begin part, from start to end of this line
                _lineEditor.DeleteRange(_selection.startLineNewCharIndex, _lineEditor.CharacterCount - _selection.startLineNewCharIndex);
                _selection.Finish();

                NewCharIndex = _lineEditor.CharacterCount;//move to end of this line
                DoDelete();//single delete to join lower line to upper
            }
            return true;
        }
        public int PreviewSingleCharDelete()
        {
            //single deletion may affect more 1 char
            //this beh is different from do-backspace
            if (!_selection.isEmpty) { return -1; }//not evaluate
            if (_lineEditor.NewCharIndex < _lineEditor.CharacterCount)
            {
                int char_index = NewCharIndex;
                TryMoveCaretTo(char_index + 1);//try move to next charet stop 
                int char_index2 = NewCharIndex;
                NewCharIndex = char_index; //restore back
                return char_index2 - char_index;
            }
            else
            {
                return 0;//end of this line
            }
        }

        public void DoDelete()
        {
            //reset

            if (!_selection.isEmpty)
            {
                DeleteSelectedText();
                return;
            }

            if (_lineEditor.NewCharIndex < _lineEditor.CharacterCount)
            {
                int char_index = NewCharIndex;
                TryMoveCaretTo(char_index + 1);//try move to next charet stop
                int char_index2 = NewCharIndex;
                //delete  
                InternalDoBackspace(char_index2 - char_index);
            }
            else
            {

                //the bring to lower line to join with this line
                if (this.LineCount > 1 && _currentLineNo < this.LineCount - 1)
                {
                    //copy content from lower line to 
                    int pos = _lineEditor.NewCharIndex;
                    CurrentLineNumber++;//move to lower line
                    DoBackspace();
                    _lineEditor.NewCharIndex = pos;
                }
            }

        }

        /// <summary>
        /// do end on current line
        /// </summary>
        public void DoEnd()
        {
            _lineEditor.NewCharIndex = _lineEditor.CharacterCount;
        }

        /// <summary>
        /// do home on current line
        /// </summary>
        public void DoHome()
        {
            //do home on current line
            _lineEditor.NewCharIndex = 0;
        }

        /// <summary>
        /// split current line into newline
        /// </summary>
        public void SplitIntoNewLine()
        {
            if (!_selection.isEmpty)
            {
                DeleteSelectedText();
            }

            if (_lineEditor.CharIndexOnTheEnd)
            {
                //end of current line
                _lines.Insert(CurrentLineNumber + 1, new PlainTextLine());
                CurrentLineNumber++;//move to lower
            }
            else
            {
                //split current line into 2
                _copyBuffer.Clear();
                _lineEditor.Split(_copyBuffer);

                //insert newline
                _lines.Insert(CurrentLineNumber + 1, new PlainTextLine());
                CurrentLineNumber++;//move to lower                
                _lineEditor.AddText(_copyBuffer, 0, _copyBuffer.Length);
                _lineEditor.NewCharIndex = 0;//move to line start

            }
        }
        static TextCopyBufferUtf32 GetFreeTextCopyBuffer() => ObjectPool<TextCopyBufferUtf32, PoolPrivateType>.GetFreeInstance();
        static void RelaseTextCopyBuffer(TextCopyBufferUtf32 buff) => ObjectPool<TextCopyBufferUtf32, PoolPrivateType>.ReleaseInstance(buff);

        //------------------------------------------
        readonly PlainTextSelection _selection = new PlainTextSelection();

        /// <summary>
        /// start selection on current character index
        /// </summary>
        public void StartSelect() => _selection.StartSelect(_currentLineNo, _lineEditor.NewCharIndex);
        /// <summary>
        /// end selection
        /// </summary>
        public void EndSelect() => _selection.EndSelect(_currentLineNo, _lineEditor.NewCharIndex);
        public void CancelSelect() => _selection.CancelSelection();
        public bool HasSelection => !_selection.isEmpty;

        internal void GetSelection(out int startLineNo, out int startLineCharIndex, out int endLineNo, out int endLineCharIndex)
        {
            startLineNo = _selection.startLineNo;
            startLineCharIndex = _selection.startLineNewCharIndex;
            endLineNo = _selection.endLineNo;
            endLineCharIndex = _selection.endLineNewCharIndex;
        }
        static Func<char, bool> s_CaretCanStopOnThisChar;

        public static void SetCaretCanStopOnThisChar(Func<char, bool> caretCanStopOnThisCharDel)
        {
            s_CaretCanStopOnThisChar = caretCanStopOnThisCharDel;
        }
        internal static bool CanCaretStopOnThisChar(int c)
        {
            char upper = (char)(c >> 16);
            char lower = (char)c;

            if (char.IsHighSurrogate((char)upper))
            {
                return false;
            }

            UnicodeCategory unicodeCatg = char.GetUnicodeCategory(lower);
            switch (unicodeCatg)
            {

                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.Control:
                    break;
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    break;
                case UnicodeCategory.OtherLetter:

                    if (s_CaretCanStopOnThisChar != null)
                    {
                        return s_CaretCanStopOnThisChar(lower);
                    }
                    break;
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.EnclosingMark:
                    //recursive
                    return false;
                default:
                    break;
            }
            return true;


            //ref: https://docs.microsoft.com/en-us/dotnet/api/system.globalization.unicodecategory?view=netcore-3.1
            //ClosePunctuation 	21 	
            //Closing character of one of the paired punctuation marks, such as parentheses, square brackets, and braces. 
            //Signified by the Unicode designation "Pe" (punctuation, close). The value is 21.

            //ConnectorPunctuation 	18 	
            //Connector punctuation character that connects two characters. 
            //Signified by the Unicode designation "Pc" (punctuation, connector). The value is 18.

            //Control 	14 	
            //Control code character, with a Unicode value of U+007F or in the range U+0000 through U+001F or U+0080 through U+009F.
            //Signified by the Unicode designation "Cc" (other, control). The value is 14.

            //CurrencySymbol 	26 	
            //Currency symbol character. 
            //Signified by the Unicode designation "Sc" (symbol, currency). The value is 26.

            //DashPunctuation 	19 	
            //Dash or hyphen character. 
            //Signified by the Unicode designation "Pd" (punctuation, dash). The value is 19.

            //DecimalDigitNumber 	8 	
            //Decimal digit character, that is, a character in the range 0 through 9.
            //Signified by the Unicode designation "Nd" (number, decimal digit). The value is 8.

            //EnclosingMark 	7 	
            //Enclosing mark character, which is a nonspacing combining character that surrounds all previous characters up to and including a base character. 
            //Signified by the Unicode designation "Me" (mark, enclosing). The value is 7.

            //FinalQuotePunctuation 	23 	
            //Closing or final quotation mark character. 
            //Signified by the Unicode designation "Pf" (punctuation, final quote). The value is 23.

            //Format 	15 	
            //Format character that affects the layout of text or the operation of text processes, but is not normally rendered.
            //Signified by the Unicode designation "Cf" (other, format). The value is 15.

            //InitialQuotePunctuation 	22 	
            //Opening or initial quotation mark character. 
            //Signified by the Unicode designation "Pi" (punctuation, initial quote). The value is 22.

            //LetterNumber 	9 	
            //Number represented by a letter, instead of a decimal digit, for example, the Roman numeral for five, which is "V". 
            //The indicator is signified by the Unicode designation "Nl" (number, letter). The value is 9.

            //LineSeparator 	12 	
            //Character that is used to separate lines of text.
            //Signified by the Unicode designation "Zl" (separator, line). The value is 12.

            //LowercaseLetter 	1 	
            //Lowercase letter. 
            //Signified by the Unicode designation "Ll" (letter, lowercase). The value is 1.

            //MathSymbol 	25 	
            //Mathematical symbol character, such as "+" or "= ". 
            //Signified by the Unicode designation "Sm" (symbol, math). The value is 25.

            //ModifierLetter 	3 	
            //Modifier letter character, which is free-standing spacing character that indicates modifications of a preceding letter. 
            //Signified by the Unicode designation "Lm" (letter, modifier). The value is 3.

            //ModifierSymbol 	27 	
            //Modifier symbol character, which indicates modifications of surrounding characters. 
            //For example, the fraction slash indicates that the number to the left is the numerator and the number to the right is the denominator. 
            //The indicator is signified by the Unicode designation "Sk" (symbol, modifier). The value is 27.

            //NonSpacingMark 	5 	
            //Nonspacing character that indicates modifications of a base character. 
            //Signified by the Unicode designation "Mn" (mark, nonspacing). The value is 5.

            //OpenPunctuation 	20 	
            //Opening character of one of the paired punctuation marks, such as parentheses, square brackets, and braces.
            //Signified by the Unicode designation "Ps" (punctuation, open). The value is 20.

            //OtherLetter 	4 	
            //Letter that is not an uppercase letter, a lowercase letter, a titlecase letter, or a modifier letter. 
            //Signified by the Unicode designation "Lo" (letter, other). The value is 4.

            //OtherNotAssigned 	29 	
            //Character that is not assigned to any Unicode category. 
            //Signified by the Unicode designation "Cn" (other, not assigned). The value is 29.

            //OtherNumber 	10 	
            //Number that is neither a decimal digit nor a letter number, for example, the fraction 1/2. The indicator is signified by the Unicode designation "No" (number, other). The value is 10.

            //OtherPunctuation 	24 	
            //Punctuation character that is not a connector, a dash, open punctuation, close punctuation, an initial quote, or a final quote. 
            //Signified by the Unicode designation "Po" (punctuation, other). The value is 24.

            //OtherSymbol 	28 	
            //Symbol character that is not a mathematical symbol, a currency symbol or a modifier symbol. 
            //Signified by the Unicode designation "So" (symbol, other). The value is 28.

            //ParagraphSeparator 	13 	
            //Character used to separate paragraphs. 
            //Signified by the Unicode designation "Zp" (separator, paragraph). The value is 13.

            //PrivateUse 	17 	
            //Private-use character, with a Unicode value in the range U+E000 through U+F8FF. 
            //Signified by the Unicode designation "Co" (other, private use). The value is 17.

            //SpaceSeparator 	11 	
            //Space character, which has no glyph but is not a control or format character. 
            //Signified by the Unicode designation "Zs" (separator, space). The value is 11.

            //SpacingCombiningMark 	6 	
            //Spacing character that indicates modifications of a base character and affects the width of the glyph for that base character. 
            //Signified by the Unicode designation "Mc" (mark, spacing combining). The value is 6.

            //Surrogate 	16 	
            //High surrogate or a low surrogate character.
            //Surrogate code values are in the range U+D800 through U+DFFF. Signified by the Unicode designation "Cs" (other, surrogate). The value is 16.

            //TitlecaseLetter 	2 	
            //Titlecase letter.
            //Signified by the Unicode designation "Lt" (letter, titlecase). The value is 2.

            //UppercaseLetter 	0 	
            //Uppercase letter. 
            //Signified by the Unicode designation "Lu" (letter, uppercase). The value is 0.
        }

        public void TryMoveCaretTo(int charIndex, bool backward = false)
        {
            //on support on the same line only!** 
            _lineEditor.NewCharIndex = charIndex;
            //check if we can stop UI caret at this character index or not 

            if (backward)
            {
                //move caret backward
                int curChar = _lineEditor.GetCurrentChar();
                int tmp_index = charIndex;
                while (curChar != '\0' && tmp_index > 0 && !CanCaretStopOnThisChar(curChar))
                {
                    _lineEditor.NewCharIndex--;
                    curChar = _lineEditor.GetCurrentChar();
                    tmp_index--;
                }
            }
            else
            {
                int curChar = _lineEditor.GetCurrentChar();
                int tmp_index = charIndex;

                while (curChar != '\0' && !CanCaretStopOnThisChar(curChar) && !_lineEditor.CharIndexOnTheEnd)
                {
                    _lineEditor.NewCharIndex++; //move next
                    curChar = _lineEditor.GetCurrentChar();
                    tmp_index++;
                }
            }

        }

        class PlainTextSelection
        {
            public int startLineNo;
            public int startLineNewCharIndex;//start line new char-index
            public int endLineNo;
            public int endLineNewCharIndex;//on end line
            public bool isEmpty = true;

            public void StartSelect(int startLineNo, int charIndex)
            {
                this.isEmpty = false;
                this.startLineNo = endLineNo = startLineNo;
                this.startLineNewCharIndex = endLineNewCharIndex = charIndex;
            }
            public void EndSelect(int endLineNo, int charIndex)
            {
                this.endLineNo = endLineNo;
                this.endLineNewCharIndex = charIndex;
            }
            public void CancelSelection() => isEmpty = true;
            public void Finish() => isEmpty = true;
            /// <summary>
            /// normalized selection
            /// </summary>
            public void Normalize()
            {
                if (endLineNo < startLineNo)
                {
                    //swap
                    int temp = startLineNo;
                    startLineNo = endLineNo;
                    endLineNo = temp;
                    //
                    //force swap start and end char index
                    temp = endLineNewCharIndex;
                    startLineNewCharIndex = endLineNewCharIndex;
                    endLineNewCharIndex = temp;

                }
                else if (endLineNo == startLineNo)
                {
                    //on the sameline
                    if (endLineNewCharIndex < startLineNewCharIndex)
                    {
                        int temp = startLineNewCharIndex;
                        startLineNewCharIndex = endLineNewCharIndex;
                        endLineNewCharIndex = temp;
                    }
                }
            }
        }

        static char[] s_newline = new[] { '\r', '\n' };
        public void CopyAll(TextCopyBufferUtf32 output)
        {
            //all
            int j = _lines.Count;
            for (int i = 0; i < j; ++i)
            {
                if (i > 0)
                {
                    output.Append(s_newline);
                }
                if (i == _currentLineNo)
                {
                    _lineEditor.Read(output);
                }
                else
                {
                    _lines[i].Content.WriteTo(output);

                }
            }
        }

        public void CopyCurrentLine(TextCopyBufferUtf32 output)
        {
            _lineEditor.Read(output);
        }

        public void CopyLine(int lineNo, TextCopyBuffer output)
        {
            PlainTextLine line = _lines[lineNo];
            if (line == _lineEditor.CurrentLine)
            {
                _lineEditor.Read((TextCopyBufferUtf32)output);
            }
            else
            {
                //line with empty content ?
                if (!line.Content.IsEmpty)
                {
                    line.Content.WriteTo(output);
                }
            }
        }

      
        public void CopySelection(TextCopyBufferUtf32 output)
        {
            //only selection
            if (_selection.isEmpty) { return; }

            int currentLine = _currentLineNo;
            _selection.Normalize();
            if (_selection.startLineNo == _selection.endLineNo)
            {
                //on the sameline
                CurrentLineNumber = _selection.startLineNo;
                _lineEditor.Read(output, _selection.startLineNewCharIndex, _selection.endLineNewCharIndex - _selection.startLineNewCharIndex);
            }
            else
            {
                //more than 1 line
                CurrentLineNumber = _selection.startLineNo;
                _lineEditor.Read(output, _selection.startLineNewCharIndex, _lineEditor.CharacterCount - _selection.startLineNewCharIndex);
                //
                int endLineNo = _selection.endLineNo;
                if ((endLineNo - _selection.startLineNo - 1) > 0)
                {

                    for (int i = _selection.startLineNo + 1; i < endLineNo; ++i)
                    {
                        output.Append(s_newline);
                        _lines[i].Content.WriteTo(output);
                    }
                }

                CurrentLineNumber = endLineNo;
                output.Append(s_newline);
                _lineEditor.Read(output, 0, _selection.endLineNewCharIndex);
                CurrentLineNumber = currentLine;//gotback
            }

        }
    }


    public static class PlainTextEditSessionExtensions
    {
        public static void SetSelection(this PlainTextEditSession editSession, int startLineNo, int startLineCharIndex,
            int stopLineNo, int stopLineCharIndex)
        {
            editSession.CurrentLineNumber = startLineNo;
            editSession.NewCharIndex = startLineCharIndex;
            editSession.StartSelect();
            editSession.CurrentLineNumber = stopLineNo;
            editSession.NewCharIndex = stopLineCharIndex;
            editSession.EndSelect();
        }
    }


}
