//MIT, 2019-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace PixelFarm.PathReconstruction
{
    public class SpanBasedRegion
    {
        public SpanBasedRegion() { }
        /// <summary>
        /// (must be) sorted hSpans
        /// </summary>
        public HSpan[] HSpans { get; set; }
    }
}