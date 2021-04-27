﻿//
// System.Drawing.RectangleF.cs
//
// Author:
//   Mike Kestner (mkestner@speakeasy.net)
//
// Copyright (C) 2001 Mike Kestner
// Copyright (C) 2004, 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
namespace PixelFarm.Drawing
{
    public struct RectangleF
    {
        float _left, _top, _width, _height;
        /// <summary>
        ///	Empty Shared Field
        /// </summary>
        ///
        /// <remarks>
        ///	An uninitialized RectangleF Structure.
        /// </remarks>

        public static readonly RectangleF Empty;
        /// <summary>
        ///	FromLTRB Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a RectangleF structure from left, top, right,
        ///	and bottom coordinates.
        /// </remarks>

        public static RectangleF FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }

        /// <summary>
        ///	Inflate Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a new RectangleF by inflating an existing 
        ///	RectangleF by the specified coordinate values.
        /// </remarks>

        public static RectangleF Inflate(in RectangleF rect, float x, float y)
        {
            RectangleF ir = new RectangleF(rect._left, rect.Top, rect.Width, rect.Height);
            ir.Inflate(x, y);
            return ir;
        }

        /// <summary>
        ///	Inflate Method
        /// </summary>
        ///
        /// <remarks>
        ///	Inflates the RectangleF by a specified width and height.
        /// </remarks>

        public void Inflate(float dw, float dh)
        {
            Inflate(new SizeF(dw, dh));
        }

        /// <summary>
        ///	Inflate Method
        /// </summary>
        ///
        /// <remarks>
        ///	Inflates the RectangleF by a specified Size.
        /// </remarks>

        public void Inflate(SizeF size)
        {
            _left -= size.Width;
            _top -= size.Height;
            _width += size.Width * 2;
            _height += size.Height * 2;
        }

        /// <summary>
        ///	Intersect Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a new RectangleF by intersecting 2 existing 
        ///	RectangleFs. Returns null if there is no intersection.
        /// </remarks>

        public static RectangleF Intersect(in RectangleF a, in RectangleF b)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            if (!a.IntersectsWithInclusive(b))
                return Empty;
            return FromLTRB(
                Math.Max(a.Left, b.Left),
                Math.Max(a.Top, b.Top),
                Math.Min(a.Right, b.Right),
                Math.Min(a.Bottom, b.Bottom));
        }

        /// <summary>
        ///	Intersect Method
        /// </summary>
        ///
        /// <remarks>
        ///	Replaces the RectangleF with the intersection of itself
        ///	and another RectangleF.
        /// </remarks>

        public void Intersect(in RectangleF rect)
        {
            this = RectangleF.Intersect(this, rect);
        }

        /// <summary>
        ///	Union Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a new RectangleF from the union of 2 existing 
        ///	RectangleFs.
        /// </remarks>

        public static RectangleF Union(in RectangleF a, in RectangleF b)
        {
            return FromLTRB(Math.Min(a.Left, b.Left),
                     Math.Min(a.Top, b.Top),
                     Math.Max(a.Right, b.Right),
                     Math.Max(a.Bottom, b.Bottom));
        }

        /// <summary>
        ///	Equality Operator
        /// </summary>
        ///
        /// <remarks>
        ///	Compares two RectangleF objects. The return value is
        ///	based on the equivalence of the Location and Size 
        ///	properties of the two RectangleFs.
        /// </remarks>

        public static bool operator ==(in RectangleF left, in RectangleF right)
        {
            return (left.Left == right.Left) && (left.Top == right.Top) &&
                                (left.Width == right.Width) && (left.Height == right.Height);
        }

        /// <summary>
        ///	Inequality Operator
        /// </summary>
        ///
        /// <remarks>
        ///	Compares two RectangleF objects. The return value is
        ///	based on the equivalence of the Location and Size 
        ///	properties of the two RectangleFs.
        /// </remarks>

        public static bool operator !=(in RectangleF left, in RectangleF right)
        {
            return (left.Left != right.Left) || (left.Top != right.Top) ||
                                (left.Width != right.Width) || (left.Height != right.Height);
        }

        /// <summary>
        ///	Rectangle to RectangleF Conversion
        /// </summary>
        ///
        /// <remarks>
        ///	Converts a Rectangle object to a RectangleF.
        /// </remarks>

        public static implicit operator RectangleF(in Rectangle r)
        {
            return new RectangleF(r.X, r.Y, r.Width, r.Height);
        }


        // -----------------------
        // Public Constructors
        // -----------------------

        /// <summary>
        ///	RectangleF Constructor
        /// </summary>
        ///
        /// <remarks>
        ///	Creates a RectangleF from PointF and SizeF values.
        /// </remarks>

        public RectangleF(PointF location, SizeF size)
        {
            _left = location.X;
            _top = location.Y;
            _width = size.Width;
            _height = size.Height;
        }

        /// <summary>
        ///	RectangleF Constructor
        /// </summary>
        ///
        /// <remarks>
        ///	Creates a RectangleF from a specified x,y location and
        ///	width and height values.
        /// </remarks>

        public RectangleF(float x, float y, float width, float height)
        {
            _left = x;
            _top = y;
            _width = width;
            _height = height;
        }
        /// <summary>
        ///	Bottom Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Y coordinate of the bottom edge of the RectangleF.
        ///	Read only.
        /// </remarks> 

        public float Bottom => _top + Height;



        /// <summary>
        ///	Height Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Height of the RectangleF.
        /// </remarks>

        public float Height
        {
            get => _height;

            set => _height = value;

        }

        /// <summary>
        ///	IsEmpty Property
        /// </summary>
        ///
        /// <remarks>
        ///	Indicates if the width or height are zero. Read only.
        /// </remarks>
        //		

        public bool IsEmpty => (_width <= 0 || _height <= 0);

        /// <summary>
        ///	Left Property
        /// </summary>
        ///
        /// <remarks>
        ///	The X coordinate of the left edge of the RectangleF.
        ///	Read only.
        /// </remarks>


        public float Left => _left;
        

        /// <summary>
        ///	Location Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Location of the top-left corner of the RectangleF.
        /// </remarks>

        public PointF Location
        {
            get => new PointF(_left, _top);

            set
            {
                _left = value.X;
                _top = value.Y;
            }
        }

        /// <summary>
        ///	Right Property
        /// </summary>
        ///
        /// <remarks>
        ///	The X coordinate of the right edge of the RectangleF.
        ///	Read only.
        /// </remarks>
        public float Right => _left + Width;

        /// <summary>
        ///	Size Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Size of the RectangleF.
        /// </remarks>
        public SizeF Size
        {
            get => new SizeF(_width, _height);

            set
            {
                _width = value.Width;
                _height = value.Height;
            }
        }

        /// <summary>
        ///	Top Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Y coordinate of the top edge of the RectangleF.
        ///	Read only.
        /// </remarks>
        public float Top => _top;



        /// <summary>
        ///	Width Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Width of the RectangleF.
        /// </remarks>

        public float Width
        {
            get => _width;

            set => _width = value;

        }

        ///// <summary>
        /////	X Property
        ///// </summary>
        /////
        ///// <remarks>
        /////	The X coordinate of the RectangleF.
        ///// </remarks>

        //public float X
        //{
        //    get => _left;

        //    set => _left = value;

        //}

        ///// <summary>
        /////	Y Property
        ///// </summary>
        /////
        ///// <remarks>
        /////	The Y coordinate of the RectangleF.
        ///// </remarks>

        //public float Y
        //{
        //    get => _top;

        //    set => _top = value;

        //}

        /// <summary>
        ///	Contains Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if an x,y coordinate lies within this RectangleF.
        /// </remarks>

        public bool Contains(float x, float y)
        {
            return ((x >= Left) && (x < Right) &&
                (y >= Top) && (y < Bottom));
        }

        /// <summary>
        ///	Contains Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if a Point lies within this RectangleF.
        /// </remarks>

        public bool Contains(PointF pt)
        {
            return Contains(pt.X, pt.Y);
        }

        /// <summary>
        ///	Contains Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if a RectangleF lies entirely within this 
        ///	RectangleF.
        /// </remarks>

        public bool Contains(in RectangleF rect)
        {
            return _left <= rect._left && Right >= rect.Right && _top <= rect._top && Bottom >= rect.Bottom;
        }

        /// <summary>
        ///	Equals Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks equivalence of this RectangleF and an object.
        /// </remarks>

        public override bool Equals(object obj) => (obj is RectangleF rect) && rect == this;


        /// <summary>
        ///	GetHashCode Method
        /// </summary>
        ///
        /// <remarks>
        ///	Calculates a hashing value.
        /// </remarks> 
        public override int GetHashCode()
        {
            //TODO: review here !!!!
            //return (int)(_x + _y + _width + _height);
            return (int)(_height + _width) ^ (int)(_left + _top);
        }

        /// <summary>
        ///	IntersectsWith Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if a RectangleF intersects with this one.
        /// </remarks>

        public bool IntersectsWith(in RectangleF rect)
        {
            return !((Left >= rect.Right) || (Right <= rect.Left) ||
                (Top >= rect.Bottom) || (Bottom <= rect.Top));
        }

        private bool IntersectsWithInclusive(in RectangleF r)
        {
            return !((Left > r.Right) || (Right < r.Left) ||
                (Top > r.Bottom) || (Bottom < r.Top));
        }

        /// <summary>
        ///	Offset Method
        /// </summary>
        ///
        /// <remarks>
        ///	Moves the RectangleF a specified distance.
        /// </remarks>

        public void Offset(float dx, float dy)
        {
            _left += dx;
            _top += dy;
        }

        /// <summary>
        ///	Offset Method
        /// </summary>
        ///
        /// <remarks>
        ///	Moves the RectangleF a specified distance.
        /// </remarks>

        public void Offset(PointF pos)
        {
            Offset(pos.X, pos.Y);
        }
        public float X => _left;
        public float Y => _top;

        /// <summary>
        ///	ToString Method
        /// </summary>
        ///
        /// <remarks>
        ///	Formats the RectangleF in (x,y,w,h) notation.
        /// </remarks>

        public override string ToString()
        {
            return String.Format("{{X={0},Y={1},Width={2},Height={3}}}",
                         _left, _top, _width, _height);
        }

        public RectangleF CreateNormalizedRect(float totalW, float totalH) => new RectangleF(_left / totalW, _top / totalH, _width / totalW, _height / totalH);
    }
}
