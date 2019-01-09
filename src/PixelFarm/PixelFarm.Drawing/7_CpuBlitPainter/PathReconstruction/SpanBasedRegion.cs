//MIT, 2019-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace PixelFarm.PathReconstruction
{
    /// <summary>
    /// region that is created from path reconstruction
    /// </summary>
    public class SpanBasedRegion : CpuBlitRegion
    {
        public SpanBasedRegion() { }
        /// <summary>
        /// (must be) sorted hSpans, from reconstruction
        /// </summary>
        public HSpan[] HSpans { get; set; }
        /// <summary>
        /// reconstructed outline
        /// </summary>
        public SpanBasedRegionOutline Outline { get; set; }

    }
}