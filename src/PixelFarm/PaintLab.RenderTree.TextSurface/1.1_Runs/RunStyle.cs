//Apache2, 2014-present, WinterDev


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
            return GlobalTextService.TextService.MeasureString(textBufferSpan, ReqFont);
        }
        internal float MeasureBlankLineHeight()
        {
            return GlobalTextService.TextService.MeasureBlankLineHeight(ReqFont);
        }

        public bool SupportsWordBreak => GlobalTextService.AdvanceTextService.SupportsWordBreak;

        public void BreakToLineSegments(in TextBufferSpan textBufferSpan, WordVisitor wordVisitor) => GlobalTextService.AdvanceTextService.BreakToLineSegments(textBufferSpan, wordVisitor);

        public void CalculateUserCharGlyphAdvancePos(
            in TextBufferSpan textBufferSpan,
            ref TextSpanMeasureResult measureResult)
        {
            GlobalTextService.AdvanceTextService.CalculateUserCharGlyphAdvancePos(
                    textBufferSpan,
                    ReqFont,
                    ref measureResult);
        }

        internal void CalculateUserCharGlyphAdvancePos(in TextBufferSpan textBufferSpan,
            ILineSegmentList lineSegs,
            ref TextSpanMeasureResult measureResult)
        {
            GlobalTextService.AdvanceTextService.CalculateUserCharGlyphAdvancePos(
                textBufferSpan,
                lineSegs,
                ReqFont,
                ref measureResult);
        }

    }

}