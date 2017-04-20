//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Typography.Rendering
{

    //this is PixelFarm version ***
    //render with MiniAgg 

    public class GlyphContourBuilder : OpenFont.IGlyphTranslator
    {

        List<GlyphContour> contours;
        float curX;
        float curY;
        float latestMoveToX;
        float latestMoveToY;
        GlyphContour currentCnt;
        GlyphPart _latestPart;
        public GlyphContourBuilder()
        {

        }
        public List<GlyphContour> GetContours()
        {
            return contours;
        }
        public void MoveTo(float x0, float y0)
        {
            this.latestMoveToX = this.curX = x0;
            this.latestMoveToY = this.curY = y0;
            _latestPart = null;
            //----------------------------

        }
        public void LineTo(float x1, float y1)
        {
            if (_latestPart != null)
            {
                currentCnt.AddPart(_latestPart = new GlyphLine(_latestPart, x1, y1));
            }
            else
            {
                currentCnt.AddPart(_latestPart = new GlyphLine(curX, curY, x1, y1));
            }
            this.curX = x1;
            this.curY = y1;
        }


        public void Curve3(float x1, float y1, float x2, float y2)
        {
            if (_latestPart != null)
            {
                currentCnt.AddPart(_latestPart = new GlyphCurve3(
                 _latestPart,
                  x1, y1,
                  x2, y2));
            }
            else
            {
                currentCnt.AddPart(new GlyphCurve3(
                    curX, curY,
                    x1, y1,
                    x2, y2));
            }

            this.curX = x2;
            this.curY = y2;
        }
        public void Curve4(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            if (_latestPart != null)
            {
                currentCnt.AddPart(_latestPart = new GlyphCurve4(
                   _latestPart,
                    x1, y1,
                    x2, y2,
                    x3, y3));
            }
            else
            {
                currentCnt.AddPart(_latestPart = new GlyphCurve4(
                   curX, curY,
                   x1, y1,
                   x2, y2,
                   x3, y3));
            }
            this.curX = x3;
            this.curY = y3;
        }

        public void CloseContour()
        {
            if (curX == latestMoveToX && curY == latestMoveToY)
            {
                //we not need to close 
            }
            else
            {
                if (_latestPart != null)
                {
                    currentCnt.AddPart(_latestPart = new GlyphLine(_latestPart, latestMoveToX, latestMoveToY));
                }
                else
                {
                    currentCnt.AddPart(_latestPart = new GlyphLine(curX, curY, latestMoveToX, latestMoveToY));
                }
            }

            this.curX = latestMoveToX;
            this.curY = latestMoveToY;

            if (currentCnt != null)
            {
                this.contours.Add(currentCnt);
                currentCnt = null;
            }
            //
            currentCnt = new GlyphContour();
        }
        public void BeginRead(int contourCount)
        {
            //reset all
            contours = new List<GlyphContour>();
            _latestPart = null;
            this.latestMoveToX = this.curX = this.latestMoveToY = this.curY = 0;
            //
            currentCnt = new GlyphContour();
            //new contour, but not add
        }
        public void EndRead()
        {

        }

    }

    public class GlyphContour
    {

        public List<GlyphPart> parts = new List<GlyphPart>();

        internal List<GlyphPoint> flattenPoints;
        internal List<GlyphEdge> edgeLines;

        bool analyzed;
        bool analyzedClockDirection;
        bool isClockwise;
        public GlyphContour()
        {
        }

        public void AddPart(GlyphPart part)
        {
            parts.Add(part);
        }

        internal void ClearAllAdjustValues()
        {
            for (int i = flattenPoints.Count - 1; i >= 0; --i)
            {
                flattenPoints[i].ClearAdjustValues();
            }
        }
        internal void Flatten(GlyphPartFlattener flattener)
        {
            //flatten once
            if (analyzed) return;
            //flatten each part ...
            //-------------------------------
            int j = parts.Count;
            //---------------
            List<GlyphPoint> prevResult = flattener.Result;
            List<GlyphPoint> tmpFlattenPoints = flattenPoints = flattener.Result = new List<GlyphPoint>();
            //start ...
            for (int i = 0; i < j; ++i)
            {
                //flatten each part
                parts[i].Flatten(flattener);
            }

            //assign number for all glyph point in this contour
            int pointCount = tmpFlattenPoints.Count;
            for (int i = 0; i < pointCount; ++i)
            {
                tmpFlattenPoints[i].GlyphPointNo = flattener.GetNewGlyphPointId();
            }

            flattener.Result = prevResult;
            analyzed = true;
        }
        public bool IsClosewise()
        {
            //after flatten
            if (analyzedClockDirection)
            {
                return isClockwise;
            }

            List<GlyphPoint> f_points = this.flattenPoints;
            if (f_points == null)
            {
                throw new NotSupportedException();
            }
            analyzedClockDirection = true;



            //---------------
            //http://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
            //check if hole or not
            //clockwise or counter-clockwise
            {
                //Some of the suggested methods will fail in the case of a non-convex polygon, such as a crescent. 
                //Here's a simple one that will work with non-convex polygons (it'll even work with a self-intersecting polygon like a figure-eight, telling you whether it's mostly clockwise).

                //Sum over the edges, (x2 − x1)(y2 + y1). 
                //If the result is positive the curve is clockwise,
                //if it's negative the curve is counter-clockwise. (The result is twice the enclosed area, with a +/- convention.)
                int j = flattenPoints.Count;
                double total = 0;
                for (int i = 1; i < j; ++i)
                {
                    GlyphPoint p0 = f_points[i - 1];
                    GlyphPoint p1 = f_points[i];

                    double x0 = p0.x;
                    double y0 = p0.y;
                    double x1 = p1.x;
                    double y1 = p1.y;

                    total += (x1 - x0) * (y1 + y0);
                    i += 2;
                }
                //the last one
                {
                    GlyphPoint p0 = f_points[j - 1];
                    GlyphPoint p1 = f_points[0];

                    double x0 = p0.x;
                    double y0 = p0.y;
                    double x1 = p1.x;
                    double y1 = p1.y;
                    total += (x1 - x0) * (y1 + y0);
                }
                isClockwise = total >= 0;
            }
            return isClockwise;
        }
        internal void CreateGlyphEdges()
        {
            int lim = flattenPoints.Count - 1;
            edgeLines = new List<GlyphEdge>();
            GlyphPoint p = null, q = null;
            EdgeLine edgeLine = null;
            for (int i = 0; i < lim; ++i)
            {
                p = flattenPoints[i];
                q = flattenPoints[i + 1];
                if ((edgeLine = FineCommonEdgeLine(p, q)) != null)
                {
                    edgeLines.Add(new GlyphEdge(edgeLine));
                }
            }
            //close   
            p = flattenPoints[lim];
            q = flattenPoints[0];
            if ((edgeLine = FineCommonEdgeLine(p, q)) != null)
            {
                edgeLines.Add(new GlyphEdge(edgeLine));
            }
        }
        static EdgeLine FineCommonEdgeLine(GlyphPoint p, GlyphPoint q)
        {
            if (p._edgeLine1 == q._edgeLine1 ||
                p._edgeLine1 == q._edgeLine2)
            {
                return p._edgeLine1;
            }
            else if (p._edgeLine2 == q._edgeLine1 ||
                     p._edgeLine2 == q._edgeLine2)
            {
                return p._edgeLine1;
            }
            else
            {

                return null;
            }
        }
    }

    public class GlyphEdge
    {
        internal readonly EdgeLine edgeLine;
        internal GlyphEdge(EdgeLine edgeLine)
        {
            this.edgeLine = edgeLine;
        }
    }

}