﻿//Apache2, 2014-present, WinterDev


using PixelFarm.Drawing;
using Typography.TextBreak;
using Typography.TextLayout;
using Typography.Text;

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
                    return _resolvedFont = GlobalTextService.TxtClient.ResolveFont(_reqFont);
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
        internal Size MeasureString(in PixelFarm.Drawing.TextBufferSpan textBufferSpan)
        {
            return GlobalTextService.TxtClient.MeasureString(textBufferSpan, ReqFont);
        }

        internal float MeasureBlankLineHeight()
        {
            return GlobalTextService.TxtClient.MeasureBlankLineHeight(ReqFont);
        }

       

        public void BreakToLineSegments(in Typography.Text.TextBufferSpan textBufferSpan, WordVisitor wordVisitor) 
            => GlobalTextService.TxtClient.BreakToLineSegments(textBufferSpan, wordVisitor);

        public void CalculateUserCharGlyphAdvancePos(
            in Typography.Text.TextBufferSpan textBufferSpan,
            ref TextSpanMeasureResult measureResult)
        {
            GlobalTextService.TxtClient.CalculateUserCharGlyphAdvancePos(
                    textBufferSpan,
                    ReqFont,
                    ref measureResult);
        }

        internal void CalculateUserCharGlyphAdvancePos(in Typography.Text.TextBufferSpan textBufferSpan,
            ILineSegmentList lineSegs,
            ref TextSpanMeasureResult measureResult)
        {
            GlobalTextService.TxtClient.CalculateUserCharGlyphAdvancePos(
                textBufferSpan,
                lineSegs,
                ReqFont,
                ref measureResult);
        }

    }

}