//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using Typography.TextBreak;

namespace LayoutFarm
{
    public interface ITextService2
    {

        float MeasureWhitespace(RequestFont f);
        float MeasureBlankLineHeight(RequestFont f);
        //
        bool SupportsWordBreak { get; }

        ILineSegmentList BreakToLineSegments(in TextBufferSpan textBufferSpan);
        //
        Size MeasureString(in TextBufferSpan textBufferSpan, RequestFont font);

        void MeasureString(in TextBufferSpan textBufferSpan, RequestFont font, int maxWidth, out int charFit, out int charFitWidth);

        void CalculateUserCharGlyphAdvancePos(in TextBufferSpan textBufferSpan,
                RequestFont font,
                ref TextSpanMeasureResult result);

        void CalculateUserCharGlyphAdvancePos(in TextBufferSpan textBufferSpan, ILineSegmentList lineSegs,
               RequestFont font,
              ref TextSpanMeasureResult result);
    }


    public static class GlobalRootGraphic2
    {

        static ITextService2 _textServices;
        public static ITextService2 TextService
        {
            get => _textServices;
            set
            {
#if DEBUG
                if (_textServices != null)
                {

                }
#endif
                _textServices = value;

            }
        }
    }
}
