//Apache2, 2014-present, WinterDev


using PixelFarm.Drawing;
using Typography.TextBreak;
using Typography.TextLayout;
using Typography.Text;
using System;

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

        public RequestFont ReqFont { get; set; }

        ResolvedFont _resolvedFont;
        public ResolvedFont GetResolvedFont()
        {
            if (_resolvedFont != null)
            {
                return _resolvedFont;
            }
            else if (ReqFont != null)
            {
                return _resolvedFont = GlobalTextService.TxClient.ResolveFont(ReqFont);
            }
            else
            {
                return null;
            }
        }
        //
        internal Size MeasureString(in PixelFarm.Drawing.TextBufferSpan textBufferSpan)
        {
            return GlobalTextService.TxClient.MeasureString(textBufferSpan, GetResolvedFont());
        }

        internal float MeasureBlankLineHeight() => GetResolvedFont().LineSpacingInPixels;


        public void BreakToLineSegments(in Typography.Text.TextBufferSpan textBufferSpan, WordVisitor wordVisitor)
            => GlobalTextService.TxClient.BreakToLineSegments(textBufferSpan, wordVisitor);

        public void CalculateUserCharGlyphAdvancePos(
            in Typography.Text.TextBufferSpan textBufferSpan,
            ref TextSpanMeasureResult measureResult)
        {
            GlobalTextService.TxClient.CalculateUserCharGlyphAdvancePos(
                    textBufferSpan,
                    ReqFont,
                    ref measureResult);
        }

        internal void CalculateUserCharGlyphAdvancePos(in Typography.Text.TextBufferSpan textBufferSpan,
            ILineSegmentList lineSegs,
            ref TextSpanMeasureResult measureResult)
        {
            GlobalTextService.TxClient.CalculateUserCharGlyphAdvancePos(
                textBufferSpan,
                lineSegs,
                ReqFont,
                ref measureResult);
        }

    }

}