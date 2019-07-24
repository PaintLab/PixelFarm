//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.TextEditing
{

    public class RunStyle
    {
        ITextService _txt_services;
        public RunStyle(ITextService textService)
        {
            _txt_services = textService;
        }
        //
        public byte ContentHAlign;
        //
        public RequestFont ReqFont { get; set; }
        public Color FontColor { get; set; }
        //
        internal Size MeasureString(ref TextBufferSpan textBufferSpan)
        {
            return _txt_services.MeasureString(ref textBufferSpan, ReqFont);
        }
        internal float MeasureBlankLineHeight()
        {
            return _txt_services.MeasureBlankLineHeight(ReqFont);
        }
        internal bool SupportsWordBreak => _txt_services.SupportsWordBreak;
        internal ILineSegmentList BreakToLineSegments(ref TextBufferSpan textBufferSpan)
        {
            return _txt_services.BreakToLineSegments(ref textBufferSpan);
        }

        internal void CalculateUserCharGlyphAdvancePos(ref TextBufferSpan textBufferSpan,
            int[] outputXAdvances,
            out int outputW,
            out int outputLineH)
        {
            _txt_services.CalculateUserCharGlyphAdvancePos(
              ref textBufferSpan,
                ReqFont,
                outputXAdvances,
                out outputW,
                out outputLineH);
        }

        internal void CalculateUserCharGlyphAdvancePos(ref TextBufferSpan textBufferSpan,
            ILineSegmentList lineSegs,
            int[] outputXAdvances,
            out int outputW,
            out int outputLineH)
        {
            _txt_services.CalculateUserCharGlyphAdvancePos(
              ref textBufferSpan,
                lineSegs,
                ReqFont,
                outputXAdvances,
                out outputW,
                out outputLineH);
        }

    }

}