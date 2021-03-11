//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using Typography.Text;
namespace LayoutFarm.TextFlow
{
    partial class TextLineBox
    {

        public VisualPointInfo GetTextPointInfoFromCaretPoint(int caretX)
        {
            int accTextRunWidth = 0;
            int accTextRunCharCount = 0;
            Run lastestTextRun = null;
            foreach (Run t in _runs)
            {
                lastestTextRun = t;
                int thisTextRunWidth = t.Width;
                if (accTextRunWidth + thisTextRunWidth > caretX)
                {
                    CharLocation localPointInfo = t.GetCharacterFromPixelOffset(caretX - thisTextRunWidth);
                    var pointInfo = new EditableVisualPointInfo(this, accTextRunCharCount + localPointInfo.RunCharIndex);
                    pointInfo.SetAdditionVisualInfo(accTextRunCharCount, caretX, accTextRunWidth);
                    return pointInfo;
                }
                else
                {
                    accTextRunWidth += thisTextRunWidth;
                    accTextRunCharCount += t.CharacterCount;
                }
            }
            if (lastestTextRun != null)
            {
                return null;
            }
            else
            {

                EditableVisualPointInfo pInfo = new EditableVisualPointInfo(this, -1);
                pInfo.SetAdditionVisualInfo(accTextRunCharCount, caretX, accTextRunWidth);
                return pInfo;
            }
        }


        public EditableVisualPointInfo GetTextPointInfoFromCharIndex(int charIndex)
        {
            int limit = CharCount() - 1;
            if (charIndex > limit)
            {
                charIndex = limit;
            }


            int rCharOffset = 0;
            int rPixelOffset = 0;
            Run lastestRun = null;
            EditableVisualPointInfo textPointInfo = null;
            foreach (Run r in _runs)
            {
                lastestRun = r;
                int thisCharCount = lastestRun.CharacterCount;
                if (thisCharCount + rCharOffset > charIndex)
                {
                    int localCharOffset = charIndex - rCharOffset;
                    int pixelOffset = lastestRun.GetRunWidth(localCharOffset);
                    textPointInfo = new EditableVisualPointInfo(this, charIndex);
                    textPointInfo.SetAdditionVisualInfo(localCharOffset, rPixelOffset + pixelOffset, rPixelOffset);
                    return textPointInfo;
                }
                else
                {
                    rCharOffset += thisCharCount;
                    rPixelOffset += r.Width;
                }
            }


            textPointInfo = new EditableVisualPointInfo(this, charIndex);
            textPointInfo.SetAdditionVisualInfo(rCharOffset - lastestRun.CharacterCount, rPixelOffset, rPixelOffset - lastestRun.Width);
            return textPointInfo;
        }

    }
}