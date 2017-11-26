#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion



namespace PdfSharp.Drawing.Pdf
{
    class XBlankGraphicsPdfRenderer : IXGraphicsRenderer
    {
        public void AddTransform(XMatrix transform, XMatrixOrder matrixOrder)
        {

        }

        public void BeginContainer(IXGraphicsContainer container, XRect dstrect, XRect srcrect, XGraphicsUnit unit)
        {

        }

        public void Close()
        {

        }

        public void DrawArc(XPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {

        }

        public void DrawBezier(XPen pen, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {

        }

        public void DrawBeziers(XPen pen, XPoint[] points)
        {

        }

        public void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points, double tension, XFillMode fillmode)
        {

        }

        public void DrawCurve(XPen pen, XPoint[] points, double tension)
        {

        }

        public void DrawEllipse(XPen pen, XBrush brush, double x, double y, double width, double height)
        {

        }

        public void DrawImage(XImage image, double x, double y, double width, double height)
        {

        }

        public void DrawImage(XImage image, XRect destRect, XRect srcRect, XGraphicsUnit srcUnit)
        {

        }

        public void DrawLine(XPen pen, double x1, double y1, double x2, double y2)
        {

        }

        public void DrawLines(XPen pen, XPoint[] points)
        {

        }

        public void DrawPath(XPen pen, XBrush brush, XGraphicsPath path)
        {

        }

        public void DrawPie(XPen pen, XBrush brush, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {

        }

        public void DrawPolygon(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode)
        {

        }

        public void DrawRectangle(XPen pen, XBrush brush, double x, double y, double width, double height)
        {

        }

        public void DrawRectangles(XPen pen, XBrush brush, XRect[] rects)
        {

        }

        public void DrawRoundedRectangle(XPen pen, XBrush brush, double x, double y, double width, double height, double ellipseWidth, double ellipseHeight)
        {

        }

        public void DrawString(string s, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
        {

        }

        public void EndContainer(IXGraphicsContainer container)
        {

        }

        public void ResetClip()
        {

        }

        public void Restore(XGraphicsState state)
        {

        }

        public void Save(XGraphicsState state)
        {

        }

        public void SetClip(XGraphicsPath path, XCombineMode combineMode)
        {

        }

        public void WriteComment(string comment)
        {

        }
    }
}