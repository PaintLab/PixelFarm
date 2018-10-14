//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.UI;
namespace LayoutFarm.Text
{
    public class TextDomEventArgs : EventArgs
    {
        public bool updateJustCurrentLine;
        public readonly UIKeys key;
        public readonly char c;
        public bool PreventDefault;
        public int delta;
        public TextDomEventArgs(char c)
        {
            this.c = c;
        }
        public TextDomEventArgs(UIKeys key)
        {
            this.key = key;
        }
        public TextDomEventArgs(int delta)
        {
            this.delta = delta;
        }

        public TextDomEventArgs(bool updateJustCurrentLine)
        {
            this.updateJustCurrentLine = updateJustCurrentLine;
        }
        public TextDomEventArgs(bool updateJustCurrentLine, VisualSelectionRangeSnapShot changedSnapShot)
        {
            this.updateJustCurrentLine = updateJustCurrentLine;
            this.SelectionSnapShot = changedSnapShot;
        }
        public VisualSelectionRangeSnapShot SelectionSnapShot { get; private set; }
        public bool Shift { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }
    }

    public sealed class TextSurfaceEventListener
    {
        TextEditRenderBox targetTextSurface;
        char[] previewKeyDownRegisterChars;
        public event EventHandler<TextDomEventArgs> PreviewArrowKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewEnterKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewDialogKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewMouseWheel;

        public event EventHandler<TextDomEventArgs> PreviewBackSpaceKeyDown;
        public event EventHandler<TextDomEventArgs> PreviewRegisteredKeyPress;
        public event EventHandler<TextDomEventArgs> CharacterAdded;
        public event EventHandler<TextDomEventArgs> CharacterRemoved;
        public event EventHandler<TextDomEventArgs> CharacterReplaced;
        public event EventHandler<TextDomEventArgs> ReplacedAll;
        public event EventHandler<TextDomEventArgs> ArrowKeyCaretPosChanged;
        public event EventHandler<TextDomEventArgs> KeyDown;
        public event EventHandler<UIKeyEventArgs> SpecialKeyInserted;
        public event EventHandler<UIKeyEventArgs> SplitedNewLine;
        public TextSurfaceEventListener()
        {
        }

        public char[] PreviewKeydownRegisterChars
        {
            get
            {
                return previewKeyDownRegisterChars;
            }
            set
            {
                previewKeyDownRegisterChars = value;
            }
        }
        public TextEditRenderBox TextSurfaceElement
        {
            get
            {
                return targetTextSurface;
            }
        }
        public void SetMonitoringTextSurface(TextEditRenderBox textSurfaceElement)
        {
            this.targetTextSurface = textSurfaceElement;
        }

        internal static bool NotifyPreviewMouseWheel(TextSurfaceEventListener listener, UIMouseEventArgs e)
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
            if (listener.ArrowKeyCaretPosChanged != null)
            {
                listener.ArrowKeyCaretPosChanged(listener, new TextDomEventArgs(key));
            }
        }
        bool IsRegisterPreviewKeyDownPress(char c)
        {
            if (previewKeyDownRegisterChars != null)
            {
                for (int i = previewKeyDownRegisterChars.Length - 1; i > -1; --i)
                {
                    if (previewKeyDownRegisterChars[i] == c)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static void NotifyCharacterAdded(TextSurfaceEventListener listener, char c)
        {
            if (listener.CharacterAdded != null)
            {
                listener.CharacterAdded(listener, new TextDomEventArgs(c));
            }
        }

        internal static void NotifyCharactersReplaced(TextSurfaceEventListener listener, char c)
        {
            if (listener.CharacterReplaced != null)
            {
                listener.CharacterReplaced(listener, new TextDomEventArgs(c));
            }
        }
        internal static void NotifyCharactersRemoved(TextSurfaceEventListener listener, TextDomEventArgs e)
        {
            if (listener.CharacterRemoved != null)
            {
                listener.CharacterRemoved(listener, e);
            }
        }
        internal static void NotifyKeyDown(TextSurfaceEventListener listener, UIKeyEventArgs e)
        {
            if (listener.KeyDown != null)
            {
                listener.KeyDown(listener, new TextDomEventArgs(e.KeyCode) { Shift = e.Shift, Control = e.Ctrl, Alt = e.Alt });
            }
        }
        internal static void NofitySplitNewLine(TextSurfaceEventListener listener, UIKeyEventArgs e)
        {
            if (listener.SplitedNewLine != null)
            {
                listener.SplitedNewLine(listener, e);
            }
        }
        internal static void NotifyReplaceAll(TextSurfaceEventListener listener, TextDomEventArgs e)
        {
            if (listener.ReplacedAll != null)
            {
                listener.ReplacedAll(listener, e);
            }

        }

        internal static void NotifyFunctionKeyDown(TextSurfaceEventListener listener, UIKeys key)
        {
        }
    }
}