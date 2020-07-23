//Apache2, 2014-present, WinterDev

using System;

namespace LayoutFarm.TextEditing
{
    public enum RunKind : byte
    {
        Text,
        Image,
        Solid
    }

    public class CopyRun
    {
        public RunKind RunKind { get; set; }
        public char[] RawContent { get; set; }

        public CopyRun(char[] rawContent)
        {
            RawContent = rawContent;
        }

        public int CharacterCount
        {
            get
            {
                switch (RunKind)
                {
                    case RunKind.Image:
                    case RunKind.Solid: return 1;
                    case RunKind.Text:
                        return RawContent.Length;
                    default: throw new NotSupportedException();
                }
            }
        }
    }


}