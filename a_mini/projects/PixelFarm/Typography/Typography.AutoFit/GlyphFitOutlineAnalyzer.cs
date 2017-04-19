//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;
using Typography.OpenFont;
namespace Typography.Rendering
{
    //This is PixelFarm's AutoFit
    //NOT FREE TYPE AUTO FIT***

    public class GlyphFitOutlineAnalyzer
    {
        GlyphPartFlattener _glyhFlattener = new GlyphPartFlattener();
        GlyphContourBuilder _glyphToCountor = new GlyphContourBuilder();
        public GlyphFitOutlineAnalyzer()
        {

        }

        /// <summary>
        /// calculate and create GlyphFitOutline
        /// </summary>
        /// <param name="glyphPoints"></param>
        /// <param name="glyphContours"></param>
        /// <returns></returns>
        public GlyphDynamicOutline CreateGlyphFitOutline(GlyphPointF[] glyphPoints, ushort[] glyphContours)
        {

            //1. convert original glyph point to contour
            _glyphToCountor.Read(glyphPoints, glyphContours);
            //2. get result as list of contour
            List<GlyphContour> contours = _glyphToCountor.GetContours();
            //
            //3. flatten each contour with the flattener
            int j = contours.Count;
            _glyhFlattener.NSteps = 2;
            for (int i = 0; i < j; ++i)
            {
                contours[i].Flatten(_glyhFlattener);
            }
            //4. after flatten, the we can create fit outline
            if (j > 0)
            {
                return CreateFitOutline(contours);
            }
            else
            {
                return null;
            }
        }


        static GlyphDynamicOutline CreateFitOutline(List<GlyphContour> contours)
        {

            int cntCount = contours.Count;
            GlyphContour cnt = contours[0]; //first contour
            //--------------------------
            //1. create polygon 
            Poly2Tri.Polygon mainPolygon = CreatePolygon(cnt);//first contour        
            bool isClockWise = cnt.IsClosewise();
            //review is hole or not here
            //eg i
            //-------------------------- 
            for (int n = 1; n < cntCount; ++n)
            {
                cnt = contours[n];
                //IsHole is correct after we Analyze() the glyph contour
                Poly2Tri.Polygon subPolygon = CreatePolygon(cnt);
                //TODO: review here 
                mainPolygon.AddHole(subPolygon);
                //if (cnt.IsClosewise())
                //{ 
                //}

                //if (cnt.IsClosewise())
                //{
                //    mainPolygon.AddHole(subPolygon);
                //}
                //else
                //{
                //    //TODO: review here
                //    //the is a complete separate part                   
                //    //eg i j has 2 part (dot over i and j etc)
                //}
            }
            //------------------------------------------
            //2. tri angulate 
            Poly2Tri.P2T.Triangulate(mainPolygon); //that poly is triangulated 
            //3. create fit outline
            GlyphDynamicOutline glyphFitOutline = new GlyphDynamicOutline(mainPolygon, contours);

            List<GlyphTriangle> triAngles = glyphFitOutline.GetTriangles();
            int triangleCount = triAngles.Count;
            for (int i = 0; i < triangleCount; ++i)
            {
                //---------------
                GlyphTriangle tri = triAngles[i];
                AssignPointEdgeInvolvement(tri.e0);
                AssignPointEdgeInvolvement(tri.e1);
                AssignPointEdgeInvolvement(tri.e2);
            }

            return glyphFitOutline;
        }




        /// <summary>
        /// create polygon from flatten curve outline point
        /// </summary>
        /// <param name="cnt"></param>
        /// <returns></returns>
        static Poly2Tri.Polygon CreatePolygon(GlyphContour cnt)
        {
            List<Poly2Tri.TriangulationPoint> points = new List<Poly2Tri.TriangulationPoint>();
            List<GlyphPoint2D> flattenPoints = cnt.flattenPoints;
            //limitation: poly tri not accept duplicated points! *** 
            double prevX = 0;
            double prevY = 0;

#if DEBUG
            //dbug check if all point is unique 
            dbugCheckAllGlyphsAreUnique(flattenPoints);
#endif

            //1st point

            //TODO: review here -> about last point
            //
            int lim = flattenPoints.Count - 1;
            //pass
            for (int i = 0; i < lim; ++i)
            {
                GlyphPoint2D p = flattenPoints[i];
                double x = p.x;
                double y = p.y;

                if (x == prevX && y == prevY)
                {
                    if (i > 0)
                    {
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    points.Add(p.triangulationPoint =
                        new Poly2Tri.TriangulationPoint(prevX = x, prevY = y) { userData = p });

                }
            }

            return new Poly2Tri.Polygon(points.ToArray());

        }
        static void AssignPointEdgeInvolvement(EdgeLine edge)
        {
            if (!edge.IsOutside)
            {
                return;
            }

            switch (edge.SlopeKind)
            {

                case LineSlopeKind.Horizontal:
                    {
                        //horiontal edge
                        //must check if this is upper horizontal 
                        //or lower horizontal 
                        //we know after do bone analysis

                        //------------
                        //both p and q of this edge is part of horizontal edge 
                        var p = edge.p.userData as GlyphPoint2D;
                        if (p != null)
                        {
                            //TODO: review here
                            p.isPartOfHorizontalEdge = true;
                            p.isUpperSide = edge.IsUpper;
                            p.horizontalEdge = edge;
                        }

                        var q = edge.q.userData as GlyphPoint2D;
                        if (q != null)
                        {
                            //TODO: review here
                            q.isPartOfHorizontalEdge = true;
                            q.horizontalEdge = edge;
                            q.isUpperSide = edge.IsUpper;
                        }
                    }
                    break;
                case LineSlopeKind.Vertical:
                    {
                        //both p and q of this edge is part of vertical edge 
                        var p = edge.p.userData as GlyphPoint2D;
                        if (p != null)
                        {
                            //TODO: review here 
                            p.AddVerticalEdge(edge);
                        }

                        var q = edge.q.userData as GlyphPoint2D;
                        if (q != null)
                        {   //TODO: review here

                            q.AddVerticalEdge(edge);
                        }
                    }
                    break;
            }

        }
        //============================

#if DEBUG
        struct TmpPoint
        {
            public readonly double x;
            public readonly double y;
            public TmpPoint(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
            public override string ToString()
            {
                return x + "," + y;
            }
        }
        static Dictionary<TmpPoint, bool> s_debugTmpPoints = new Dictionary<TmpPoint, bool>();
        static void dbugCheckAllGlyphsAreUnique(List<GlyphPoint2D> flattenPoints)
        {
            double prevX = 0;
            double prevY = 0;
            s_debugTmpPoints = new Dictionary<TmpPoint, bool>();
            int lim = flattenPoints.Count - 1;
            for (int i = 0; i < lim; ++i)
            {
                GlyphPoint2D p = flattenPoints[i];
                double x = p.x;
                double y = p.y;

                if (x == prevX && y == prevY)
                {
                    if (i > 0)
                    {
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    TmpPoint tmp_point = new TmpPoint(x, y);
                    if (!s_debugTmpPoints.ContainsKey(tmp_point))
                    {
                        //ensure no duplicated point
                        s_debugTmpPoints.Add(tmp_point, true);
                        if (p.triangulationPoint != null)
                        {
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                    prevX = x;
                    prevY = y;
                }
            }

        }
#endif







    }
}
