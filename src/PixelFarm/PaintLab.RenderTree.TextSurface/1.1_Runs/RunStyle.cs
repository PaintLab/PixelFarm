//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using Typography.TextBreak; 

namespace LayoutFarm.TextEditing
{
   

    public class RunStyle
    {
        public RunStyle()
        {

        }
        //
        public byte ContentHAlign;
        //
        public RequestFont ReqFont { get; set; }
        public Color FontColor { get; set; }
        //
        internal Size MeasureString(in TextBufferSpan textBufferSpan)
        {
            return GlobalRootGraphic2.TextService.MeasureString(textBufferSpan, ReqFont);
        }
        internal float MeasureBlankLineHeight()
        {
            return GlobalRootGraphic2.TextService.MeasureBlankLineHeight(ReqFont);
        }
        internal bool SupportsWordBreak => GlobalRootGraphic2.TextService.SupportsWordBreak;
        internal ILineSegmentList BreakToLineSegments(in TextBufferSpan textBufferSpan)
        {
            return GlobalRootGraphic2.TextService.BreakToLineSegments(textBufferSpan);
        }

        internal void CalculateUserCharGlyphAdvancePos(
            in TextBufferSpan textBufferSpan,
            ref TextSpanMeasureResult measureResult)
        {
            GlobalRootGraphic2.TextService.CalculateUserCharGlyphAdvancePos(
                    textBufferSpan,
                    ReqFont,
                    ref measureResult);
        }

        internal void CalculateUserCharGlyphAdvancePos(in TextBufferSpan textBufferSpan,
            ILineSegmentList lineSegs,
            ref TextSpanMeasureResult measureResult)
        {
            GlobalRootGraphic2.TextService.CalculateUserCharGlyphAdvancePos(
                textBufferSpan,
                lineSegs,
                ReqFont,
                ref measureResult);
        }

    }

}