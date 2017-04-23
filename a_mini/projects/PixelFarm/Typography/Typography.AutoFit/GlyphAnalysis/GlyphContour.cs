//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Typography.Rendering
{

    public class GlyphContour
    {

        public List<GlyphPart> parts = new List<GlyphPart>();

        internal List<GlyphPoint> flattenPoints; //original flatten points

        internal List<GlyphEdge> edges; //for dyanmic outline processing

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
            if (GlyphPoint.SameCoordAs(tmpFlattenPoints[pointCount - 1], tmpFlattenPoints[0]))
            {
                //check if the last point is the same value as the first 
                //if yes => remove the last one
                tmpFlattenPoints.RemoveAt(pointCount - 1);
                pointCount--;
            }


#if DEBUG
            for (int i = 0; i < pointCount; ++i)
            {
                tmpFlattenPoints[i].dbugGlyphPointNo = flattener.GetNewGlyphPointId();
            }
#endif
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

#if DEBUG
        internal void dbugCheckGlyphPoints()
        {
            //int j = flattenPoints.Count;
            //GlyphPoint p = null;
            //for (int i = 0; i < j; ++i)
            //{
            //    p = flattenPoints[i];
            //    if (p.E0 == null || p.E1 == null)
            //    {

            //    }
            //}

        }
#endif
        internal void CreateGlyphEdges()
        {
            int lim = flattenPoints.Count - 1;
            edges = new List<GlyphEdge>();
            GlyphPoint p = null, q = null;
            EdgeLine edgeLine = null;

            for (int i = 0; i < lim; ++i)
            {
                p = flattenPoints[i];
                q = flattenPoints[i + 1];
                if ((edgeLine = FineCommonEdgeLine(p, q)) != null)
                {
                    edges.Add(new GlyphEdge(p, q, edgeLine));
                }
                else
                {

                }
            }
            //close   
            p = flattenPoints[lim];
            q = flattenPoints[0];
            if ((edgeLine = FineCommonEdgeLine(p, q)) != null)
            {
                edges.Add(new GlyphEdge(p, q, edgeLine));
            }
            else
            {
                //not found
            }
            int j = edges.Count;
            for (int i = 0; i < j; ++i)
            {
                edges[i].FindPerpendicularBones();
            }
            //
        }
        internal void ApplyNewRelativeEdgeDistance(float relativeDistance)
        {
            if (edges == null) return;
            //
            int j = edges.Count;
            if (j < 1) return;

            //for (int i = 0; i < j; ++i)
            //{
            //    edges[i].ApplyNewEdgeDistance(relativeDistance);
            //}
            ////find new cutpoint between edges
            //create new cutting point between edges
            int lim = j - 1;
            for (int i = 0; i < lim; ++i)
            {
                //e0 and e1 share cutpoint
                GlyphEdge.FindCutPoint(edges[i], edges[i + 1]);
            }
            //last one 
            GlyphEdge.FindCutPoint(edges[lim], edges[0]);
        }
        static EdgeLine FineCommonEdgeLine(GlyphPoint p, GlyphPoint q)
        {
            if (p.E0 == q.E0 ||
                p.E0 == q.E1)
            {
                return p.E0;
            }
            else if (p.E1 == q.E0 ||
                     p.E1 == q.E1)
            {
                return p.E1;
            }
            else
            {

                return null;
            }
        }
    }

}