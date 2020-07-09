//Apache2, 2014-present, WinterDev


using PixelFarm.Drawing;
using Typography.TextBreak;
using Typography.TextServices;
namespace LayoutFarm.TextEditing
{
    public class RunStyle
    {
        public RunStyle()
        {

        }
        //
        public byte ContentHAlign;
        public Color FontColor { get; set; }

        RequestFont _reqFont;
        ResolvedFont _resolvedFont;
        public RequestFont ReqFont
        {
            get => _reqFont;
            set
            {
                _reqFont = value;
                _resolvedFont = null;
            }
        }
        public ResolvedFont ResolvedFont
        {
            get
            {
                if (_resolvedFont != null)
                {
                    return _resolvedFont;
                }
                else if (_reqFont != null)
                {
                    return _resolvedFont = GlobalTextService.TextService2.ResolveFont(_reqFont);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _resolvedFont = value;
                _reqFont = null;
            }
        }
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