//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.UI;
using Typography.Text;

namespace LayoutFarm.TextFlow
{
    public class TextDomEventArgs : EventArgs
    {
        public bool UpdateJustCurrentLine;
        public readonly UIKeys Key;
        public readonly char c;
        public bool PreventDefault;
        public int delta;

        public TextDomEventArgs(char c)
        {
            this.c = c;
        }
        public TextDomEventArgs(UIKeys key)
        {
            this.Key = key;
        }
        public TextDomEventArgs(int delta)
        {
            this.delta = delta;
        }

        public TextDomEventArgs(bool updateJustCurrentLine)
        {
            this.UpdateJustCurrentLine = updateJustCurrentLine;
        }
        public TextDomEventArgs(bool updateJustCurrentLine, SelectionRangeSnapShot changedSnapShot)
        {
            this.UpdateJustCurrentLine = updateJustCurrentLine;
            this.SelectionSnapShot = changedSnapShot;
        }
        public SelectionRangeSnapShot SelectionSnapShot { get; private set; }
        public bool Shift { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }

        public UIKeyEventArgs OriginalKeyEventArgs { get; internal set; }
    }

    public class SplitToNewLineEventArgs : EventArgs
    {
        public int LineNumberBeforeSplit { get; set; }
        public int LineCharIndexBeforeSplit { get; set; }
    }
    public class NewStringAddedEventArgs : EventArgs
    {
        public char SingleChar { get; set; }
        public string Text { get; set; }
        public int InsertAtCharIndex { get; set; }
    }

    public sealed class TextSurfaceEventListener
    {
        TextEditRenderBox _targetTextSurface;
        char[] _previewKeyDownRegisterChars;
        public event EventHandler<TextDomEventArgs> PreviewArrowKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewEnterKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewDialogKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewMouseWheel;
        public event EventHandler<TextDomEventArgs> PreviewBackSpaceKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewRegisteredKeyPress;

        public event EventHandler<NewStringAddedEventArgs> CharacterAdded;
        public event EventHandler<TextDomEventArgs> CharacterRemoved;
        public event EventHandler<TextDomEventArgs> CharacterReplaced;
        public event EventHandler<TextDomEventArgs> ReplacedAll;
        public event EventHandler<TextDomEventArgs> ArrowKeyCaretPosChanged;
        public event EventHandler<TextDomEventArgs> KeyDown;
        public event EventHandler<TextDomEventArgs> KeyUp;
        public event EventHandler<UIKeyEventArgs> SpecialKeyInserted;
        public event EventHandler<SplitToNewLineEventArgs> SplitedNewLine;
        public TextSurfaceEventListener()
        {
        }

        public char[] PreviewKeydownRegisterChars
        {
            get => _previewKeyDownRegisterChars;
            set => _previewKeyDownRegisterChars = value;
        }
        //
        public TextEditRenderBox TextSurfaceElement => _targetTextSurface;
        //
        public void SetMonitoringTextSurface(TextEditRenderBox textSurfaceElement)
        {
            _targetTextSurface = textSurfaceElement;
        }

        internal static bool NotifyPreviewMouseWheel(TextSurfaceEventListener listener, UIMouseWheelEventArgs e)
        {
            if (listener.PreviewMouseWheel != null)
            {
                TextDomEventArgs e2 = new TextDomEventArgs(e.Delta);
                //TODO: add alt, control,shift
                listener.PreviewMouseWheel(listener, e2);
                return e2.PreventDefault;
            }
            return false;
        }

        internal static bool NotifyPreviewDialogKeyDown(TextSurfaceEventListener listener, UIKeyEventArgs keyEventArgs)
        {
            if (listener.PreviewDialogKeyDown != null)
            {
                TextDomEventArgs e = new TextDomEventArgs(keyEventArgs.KeyCode)
                {
                    Alt = keyEventArgs.Alt,
                    Control = keyEventArgs.Ctrl,
                    Shift = keyEventArgs.Shift
                };

                listener.PreviewDialogKeyDown(listener, e);
                return e.PreventDefault;
            }
            return false;
        }
        internal static bool NotifyPreviewEnter(TextSurfaceEventListener listener, UIKeyEventArgs keyEventArgs)
        {
            if (listener.PreviewEnterKeyDown != null)
            {
                TextDomEventArgs e = new TextDomEventArgs(keyEventArgs.KeyCode)
                {
                    Alt = keyEventArgs.Alt,
                    Control = keyEventArgs.Ctrl,
                    Shift = keyEventArgs.Shift
                };
                listener.PreviewEnterKeyDown(listener, e);
                return e.PreventDefault;
            }
            return false;
        }

        internal static bool NotifyPreviewBackSpace(TextSurfaceEventListener listener, UIKeyEventArgs keyEventArgs)
        {
            if (listener.PreviewBackSpaceKeyDown != null)
            {
                TextDomEventArgs e = new TextDomEventArgs(keyEventArgs.KeyCode)
                {
                    Alt = keyEventArgs.Alt,
                    Control = keyEventArgs.Ctrl,
                    Shift = keyEventArgs.Shift
                };
                listener.PreviewBackSpaceKeyDown(listener, e);
                return e.PreventDefault;
            }
            return false;
        }
        internal static bool NotifyPreviewArrow(TextSurfaceEventListener listener, UIKeyEventArgs keyEventArgs)
        {
            if (listener.PreviewArrowKeyDown != null)
            {
                TextDomEventArgs e = new TextDomEventArgs(keyEventArgs.KeyCode)
                {
                    Alt = keyEventArgs.Alt,
                    Control = keyEventArgs.Ctrl,
                    Shift = keyEventArgs.Shift
                };
                listener.PreviewArrowKeyDown(listener, e);
                return e.PreventDefault;
            }
            return false;
        }

        internal static bool NotifyPreviewKeyPress(TextSurfaceEventListener listener, UIKeyEventArgs keyEventArgs)
        {
            if (listener.IsRegisterPreviewKeyDownPress(keyEventArgs.KeyChar) &&
                listener.PreviewRegisteredKeyPress != null)
            {
                //TODO: review here use from pool?
                TextDomEventArgs e = new TextDomEventArgs(keyEventArgs.KeyChar)
                {
                    Alt = keyEventArgs.Alt,
                    Control = keyEventArgs.Ctrl,
                    Shift = keyEventArgs.Shift
                };
                //also set other keyboard info ?
                //eg. alt ctrl shift
                listener.PreviewRegisteredKeyPress(listener, e);
                return e.PreventDefault;
            }
            return false;
        }
        internal static void NotifyArrowKeyCaretPosChanged(TextSurfaceEventListener listener, UIKeys key)
        {

            listener.ArrowKeyCaretPosChanged?.Invoke(listener, new TextDomEventArgs(key));

        }
        bool IsRegisterPreviewKeyDownPress(char c)
        {
            if (_previewKeyDownRegisterChars != null)
            {
                for (int i = _previewKeyDownRegisterChars.Length - 1; i > -1; --i)
                {
                    if (_previewKeyDownRegisterChars[i] == c)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static void NotifyCharacterAdded(TextSurfaceEventListener listener, int insertCharIndex, char c)
        {
            if (listener.CharacterAdded != null)
            {
                listener.CharacterAdded(listener,
                    new NewStringAddedEventArgs() { SingleChar = c, InsertAtCharIndex = insertCharIndex });
            }
        }
        internal static void NotifyStringAdded(TextSurfaceEventListener listener, int insertCharIndex, string text)
        {
            if (listener.CharacterAdded != null)
            {
                listener.CharacterAdded(listener,
                    new NewStringAddedEventArgs() { Text = text, InsertAtCharIndex = insertCharIndex });
            }
        }
        internal static void NotifyCharacterReplaced(TextSurfaceEventListener listener, char c)
        {
            listener.CharacterReplaced?.Invoke(listener, new TextDomEventArgs(c));
        }
        internal static void NotifyCharactersRemoved(TextSurfaceEventListener listener, TextDomEventArgs e)
        {
            listener.CharacterRemoved?.Invoke(listener, e);
        }
        internal static void NotifyKeyDown(TextSurfaceEventListener listener, UIKeyEventArgs e)
        {
            listener.KeyDown?.Invoke(listener, new TextDomEventArgs(e.KeyCode) { Shift = e.Shift, Control = e.Ctrl, Alt = e.Alt, OriginalKeyEventArgs = e });
        }
        internal static void NotifyKeyUp(TextSurfaceEventListener listener, UIKeyEventArgs e)
        {
            listener.KeyUp?.Invoke(listener, new TextDomEventArgs(e.KeyCode) { Shift = e.Shift, Control = e.Ctrl, Alt = e.Alt, OriginalKeyEventArgs = e });
        }
        internal static void NofitySplitNewLine(TextSurfaceEventListener listener, SplitToNewLineEventArgs e)
        {
            listener.SplitedNewLine?.Invoke(listener, e);
        }

        internal static void NotifyKeyDownOnSingleLineText(TextSurfaceEventListener listener, UIKeyEventArgs e)
        {
            listener.KeyDown?.Invoke(listener, new TextDomEventArgs(e.KeyCode) { Shift = e.Shift, Control = e.Ctrl, Alt = e.Alt, OriginalKeyEventArgs = e });
        }
        internal static void NotifyReplaceAll(TextSurfaceEventListener listener, TextDomEventArgs e)
        {
            listener.ReplacedAll?.Invoke(listener, e);
        }

        internal static void NotifyFunctionKeyDown(TextSurfaceEventListener listener, UIKeys key)
        {
            //TODO:???
        }
    }

    public abstract class TextFlowEditSessionListener
    {
        int _currentLineNo;
        int _currentLineCharIndex;

        public TextFlowEditSessionListener()
        {

        }

        public int CurrentLineNewCharIndex => _currentLineCharIndex;
        public int CurrentLineNo => _currentLineNo;
        public bool HasSelection { get; private set; }
        //
        public int SelectionStartLineNo { get; private set; }
        public int SelectionStartLineCharIndex { get; private set; }
        public int SelectionEndLineNo { get; private set; }
        public int SelectionEndLineCharIndexNo { get; private set; }
        //

        public virtual void AddChar(int c)
        {

        }
        public virtual void AddText(TextCopyBuffer buffer)
        {

        }
        public virtual void SetCurrentPos(int lineNumber, int charIndex)
        {
            _currentLineNo = lineNumber;
            _currentLineCharIndex = charIndex;
        }
        public virtual void SetSelection(int startLineNo, int startLineCharIndex, int endLineNo, int endLineCharIndex)
        {
            //we need to ensure that the selection is normalized**
            SelectionStartLineNo = startLineNo;
            SelectionStartLineCharIndex = startLineCharIndex;
            SelectionEndLineNo = endLineNo;
            SelectionEndLineCharIndexNo = endLineCharIndex;
            HasSelection = true;
        }
        public virtual void CancelSelection()
        {
            //cancel prev selection
            HasSelection = false;
        }
        public virtual void SplitIntoNewLine()
        {

        }

        public virtual void DoDelete()
        {

        }
        public virtual void DoBackspace()
        {

        }
    }


}