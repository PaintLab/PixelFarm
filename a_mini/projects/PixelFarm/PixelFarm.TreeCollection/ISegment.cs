//
// ISegment.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

//from namespace MonoDevelop.Core.Text
namespace PixelFarm.TreeCollection
{
    /// <summary>
    /// An (Offset,Length)-pair.
    /// </summary>
    public interface ISegment
    {
        /// <summary>
        /// Gets the start offset of the segment.
        /// </summary>
        int Offset { get; }

        /// <summary>
        /// Gets the length of the segment.
        /// </summary>
        /// <remarks>For line segments (IDocumentLine), the length does not include the line delimeter.</remarks>
        int Length { get; }

        /// <summary>
        /// Gets the end offset of the segment.
        /// </summary>
        /// <remarks>EndOffset = Offset + Length;</remarks>
        int EndOffset { get; }
    }
    /// <summary>
	/// Extension methods for <see cref="ISegment"/>.
	/// </summary>
	public static class ISegmentExtensions
    {
        /// <summary>
        /// Gets whether <paramref name="segment"/> fully contains the specified segment.
        /// </summary>
        /// <remarks>
        /// Use <c>segment.Contains(offset, 0)</c> to detect whether a segment (end inclusive) contains offset;
        /// use <c>segment.Contains(offset, 1)</c> to detect whether a segment (end exclusive) contains offset.
        /// </remarks>
        public static bool Contains(this ISegment segment, int offset, int length)
        {
            if (segment == null)
                throw new ArgumentNullException("segment");
            return segment.Offset <= offset && offset + length <= segment.EndOffset;
        }

        /// <summary>
        /// Gets whether <paramref name="segment"/> fully contains the specified segment.
        /// </summary>
        public static bool Contains(this ISegment segment, ISegment span)
        {
            if (segment == null)
                throw new ArgumentNullException("segment");
            if (span == null)
                throw new ArgumentNullException("span");
            return segment.Offset <= span.Offset && span.EndOffset <= segment.EndOffset;
        }

        /// <summary>
        /// Gets whether the offset is within the <paramref name="segment"/>.
        /// </summary>
        public static bool Contains(this ISegment segment, int offset)
        {
            if (segment == null)
                throw new ArgumentNullException("segment");
            return unchecked((uint)(offset - segment.Offset) < (uint)segment.Length);
        }

        /// <summary>
        /// Gets whether the offset is within the <paramref name="segment"/>.
        /// </summary>
        public static bool IsInside(this ISegment segment, int offset)
        {
            if (segment == null)
                throw new ArgumentNullException("segment");
            return unchecked((uint)(offset - segment.Offset) <= (uint)segment.Length);
        }

        /// <summary>
        /// Determines whether <paramref name="other"/> overlaps this span. Two spans are considered to overlap 
        /// if they have positions in common and neither is empty. Empty spans do not overlap with any 
        /// other span.
        /// </summary>
        public static bool OverlapsWith(this ISegment segment, ISegment other)
        {
            int overlapStart = Math.Max(segment.Offset, other.Offset);
            int overlapEnd = Math.Min(segment.EndOffset, other.EndOffset);
            return overlapStart < overlapEnd;
        }

        //public static ISegment AdjustSegment(this ISegment segment, TextChangeEventArgs args)
        //{
        //    if (segment == null)
        //        throw new ArgumentNullException("segment");
        //    var newStartOffset = args.GetNewOffset(segment.Offset);
        //    var newEndOffset = args.GetNewOffset(segment.EndOffset);
        //    return new TextSegment(newStartOffset, newEndOffset - newStartOffset);
        //}

        //public static IEnumerable<ISegment> AdjustSegments(this IEnumerable<ISegment> segments, TextChangeEventArgs args)
        //{
        //    if (segments == null)
        //        throw new ArgumentNullException("segments");
        //    foreach (var segment in segments)
        //    {
        //        yield return segment.AdjustSegment(args);
        //    }
        //}

        public static bool IsInvalid(this ISegment segment)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));
            return segment.Offset < 0;
        }
    }
}