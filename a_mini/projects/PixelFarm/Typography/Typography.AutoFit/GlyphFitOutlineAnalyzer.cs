﻿//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;

namespace Typography.Rendering
{
    //This is PixelFarm's AutoFit
    //NOT FREE TYPE AUTO FIT***
    public partial class GlyphFitOutlineAnalyzer
    {
        GlyphPartFlattener analyzer = new GlyphPartFlattener();
        GlyphTranslatorToContour glyphToCountor = new GlyphTranslatorToContour();
        public GlyphFitOutlineAnalyzer()
        {

        }
#if DEBUG
        public GlyphFitOutline dbugAnalyze(GlyphContour testContour, ushort[] glyphContours)
        {


            //master outline analysis 
            List<GlyphContour> contours = new List<GlyphContour>() { testContour };
            int j = contours.Count;
            analyzer.NSteps = 2;
            for (int i = 0; i < j; ++i)
            {
                contours[i].Analyze(analyzer);
            }

            if (j > 0)
            {
                return CreateFitOutline(contours);
            }
            else
            {
                return null;
            }
        }
#endif
        static GlyphFitOutline CreateFitOutline(List<GlyphContour> contours)
        {

            int cntCount = contours.Count;
            GlyphContour cnt = contours[0];
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
            //2. tri angulaet 
            Poly2Tri.P2T.Triangulate(mainPolygon); //that poly is triangulated 
            //3. create fit outline
            GlyphFitOutline glyphFitOutline = new GlyphFitOutline(mainPolygon, contours);
            //4. analyze
            glyphFitOutline.Analyze();
            //------------------------------------------

            List<GlyphTriangle> triAngles = glyphFitOutline.dbugGetTriangles();
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

        struct TmpPoint
        {
            public readonly double x;
            public readonly double y;
            public TmpPoint(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
#if DEBUG
            public override string ToString()
            {
                return x + "," + y;
            }
#endif
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
            Dictionary<TmpPoint, bool> tmpPoints = new Dictionary<TmpPoint, bool>();
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
                    if (!tmpPoints.ContainsKey(tmp_point))
                    {
                        //ensure no duplicated point
                        tmpPoints.Add(tmp_point, true);
                        if (p.triangulationPoint != null)
                        {

                        }
                        points.Add(p.triangulationPoint = new Poly2Tri.TriangulationPoint(x, y) { userData = p });
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                    prevX = x;
                    prevY = y;
                }
            }

            Poly2Tri.Polygon polygon = new Poly2Tri.Polygon(points.ToArray());
            return polygon;

        }
        static void AssignPointEdgeInvolvement(EdgeLine edge)
        {
            if (!edge.IsOutside)
            {
                return;
            }

            switch (edge.SlopKind)
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
    }
}
namespace Typography.Rendering
{
    using Typography.OpenFont;
    partial class GlyphFitOutlineAnalyzer
    {
        public GlyphFitOutline Analyze(GlyphPointF[] glyphPoints, ushort[] glyphContours)
        {

            glyphToCountor.Read(glyphPoints, glyphContours);
            //master outline analysis 
            List<GlyphContour> contours = glyphToCountor.GetContours(); //analyzed contour             
            int j = contours.Count;
            analyzer.NSteps = 2;
            for (int i = 0; i < j; ++i)
            {
                contours[i].Analyze(analyzer);
            }

            if (j > 0)
            {
                return CreateFitOutline(contours);
            }
            else
            {
                return null;
            }
        }
    }
}