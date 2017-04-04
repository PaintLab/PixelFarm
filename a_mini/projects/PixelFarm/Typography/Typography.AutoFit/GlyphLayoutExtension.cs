//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using Typography.OpenFont;
namespace Typography.Rendering
{

    public static class GlyphFitOutlineExtensions
    {
        /// <summary>
        /// read fitting output
        /// </summary>
        /// <param name="tx">glyph translator</param>
        public static void ReadOutput(this GlyphFitOutline glyphOutline, IGlyphTranslator tx, float pxScale)
        {
            //TODO: review here
            if (glyphOutline == null) return;
            //
            //-----------------------------------------------------------            
            //create fit contour
            //this version use only Agg's vertical hint only ****
            //(ONLY vertical fitting , NOT apply horizontal fit)
            //-----------------------------------------------------------     
            //create outline
            //then create     
            List<GlyphContour> contours = glyphOutline.Contours;
            int j = contours.Count;
            for (int i = 0; i < j; ++i)
            {
                //new contour
                contours[i].ClearAllAdjustValues();
            }

#if DEBUG
            s_dbugAffectedPoints.Clear();
            s_dbugAff2.Clear();
#endif
            List<List<Point2d>> genPointList = new List<List<Point2d>>();
            for (int i = 0; i < j; ++i)
            {
                //new contour
                List<Point2d> genPoints = new List<Point2d>();
                CreateFitShape(genPoints, contours[i], pxScale, false, true, false);
                genPointList.Add(genPoints);
            }

            //-------------
            tx.BeginRead(j);
            for (int i = 0; i < j; ++i)
            {
                CreateFitShape3(tx, genPointList[i], contours[i]);
            }
            tx.EndRead();
            //-------------
        }
        const int GRID_SIZE = 1;
        const float GRID_SIZE_25 = 1f / 4f;
        const float GRID_SIZE_50 = 2f / 4f;
        const float GRID_SIZE_75 = 3f / 4f;

        const float GRID_SIZE_33 = 1f / 3f;
        const float GRID_SIZE_66 = 2f / 3f;


        static float RoundToNearestY(GlyphPoint2D p, float org, bool useHalfPixel)
        {
            float floo_int = (int)org;//floor 
            float remaining = org - floo_int;
            if (useHalfPixel)
            {
                if (remaining > GRID_SIZE_66)
                {
                    return (floo_int + 1f);
                }
                else if (remaining > (GRID_SIZE_33))
                {
                    return (floo_int + 0.5f);
                }
                else
                {
                    return floo_int;
                }
            }
            else
            {
                if (remaining > GRID_SIZE_50)
                {
                    return (floo_int + 1f);
                }
                else
                {
                    //we we move this point down
                    //the upper part point may affect the other(lower side)
                    //1.horizontal edge

                    EdgeLine h_edge = p.horizontalEdge;
                    EdgeLine matching_anotherSide = h_edge.GetMatchingOutsideEdge();
                    if (matching_anotherSide != null)
                    {
                        Poly2Tri.TriangulationPoint a_p = matching_anotherSide.p;
                        Poly2Tri.TriangulationPoint a_q = matching_anotherSide.q;
                        if (a_p != null && a_p.userData is GlyphPoint2D)
                        {
                            GlyphPoint2D a_glyph_p = (GlyphPoint2D)a_p.userData;
                            a_glyph_p.AdjustedY = -remaining;
#if DEBUG
                            if (!s_dbugAff2.ContainsKey(a_glyph_p))
                            {
                                s_dbugAff2.Add(a_glyph_p, true);
                                s_dbugAffectedPoints.Add(a_glyph_p);
                            }

#endif
                        }
                        if (a_q != null && a_q.userData is GlyphPoint2D)
                        {
                            GlyphPoint2D a_glyph_q = (GlyphPoint2D)a_q.userData;
                            a_glyph_q.AdjustedY = -remaining;
#if DEBUG
                            if (!s_dbugAff2.ContainsKey(a_glyph_q))
                            {
                                s_dbugAff2.Add(a_glyph_q, true);
                                s_dbugAffectedPoints.Add(a_glyph_q);
                            }

#endif
                        }
                    }

                    return floo_int;
                }
            }
        }
        static float RoundToNearestX(float org)
        {
            float actual1 = org;
            float integer1 = (int)(actual1);//lower
            float floatModulo = actual1 - integer1;

            if (floatModulo >= (GRID_SIZE_50))
            {
                return (integer1 + 1);
            }
            else
            {
                return integer1;
            }
        }
        struct Point2d
        {
            public float x;
            public float y;
            public Point2d(float x, float y)
            {

                this.x = x;
                this.y = y;
            }
#if DEBUG
            public override string ToString()
            {
                return "(" + x + "," + y + ")";
            }
#endif
        }
#if DEBUG
        public static List<GlyphPoint2D> s_dbugAffectedPoints = new List<GlyphPoint2D>();
        public static Dictionary<GlyphPoint2D, bool> s_dbugAff2 = new Dictionary<GlyphPoint2D, bool>();

#endif

        static void CreateFitShape3(IGlyphTranslator tx,
            List<Point2d> genPoints,
            GlyphContour contour)
        {

            int j = genPoints.Count;
            //merge 0 = start
            //double prev_px = 0;
            //double prev_py = 0; 
            float first_px = 0;
            float first_py = 0;

            //---------------
            //1st round for value adjustment
            //---------------

            //find adjust y
            List<GlyphPoint2D> flattenPoints = contour.flattenPoints;
            //---------------
            if (j != flattenPoints.Count)
            {
                throw new NotSupportedException();
            }
            //---------------
            for (int i = 0; i < j; ++i)
            {
                GlyphPoint2D glyphPoint = flattenPoints[i];
                Point2d p = genPoints[i];

                if (glyphPoint.AdjustedY != 0)
                {
                    if (i == 0)
                    {
                        //first point
                        tx.MoveTo(first_px = p.x, first_py = (float)(p.y + glyphPoint.AdjustedY));
                    }
                    else
                    {
                        tx.LineTo(p.x, (float)(p.y + glyphPoint.AdjustedY));
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        //first point
                        tx.MoveTo(first_px = p.x, first_py = p.y);
                    }
                    else
                    {
                        tx.LineTo(p.x, p.y);
                    }
                }
            }
            //close
            //tx.LineTo(first_px, first_py);
            tx.CloseContour();
        }
        static void CreateFitShape(
            List<Point2d> genPoints,
            GlyphContour contour,
            float pixelScale,
            bool x_axis,
            bool y_axis,
            bool useHalfPixel)
        {
            List<GlyphPoint2D> flattenPoints = contour.flattenPoints;

            int j = flattenPoints.Count;
            //merge 0 = start
            //double prev_px = 0;
            //double prev_py = 0;
            double p_x = 0;
            double p_y = 0;
            double first_px = 0;
            double first_py = 0;

            //---------------
            //1st round for value adjustment
            //---------------

            //find adjust y

            {
                GlyphPoint2D p = flattenPoints[0];
                p_x = p.x * pixelScale;
                p_y = p.y * pixelScale;

                if (y_axis && p.isPartOfHorizontalEdge && p.isUpperSide) //TODO: review here
                {
                    //vertical fitting, fit p_y to grid
                    //adjust if p is not part of curve
                    switch (p.kind)
                    {
                        case PointKind.LineStart:
                        case PointKind.LineStop:
                            p_y = RoundToNearestY(p, (float)p_y, useHalfPixel);
                            break;
                    }

                }
                if (x_axis && p.IsPartOfVerticalEdge && p.IsLeftSide)
                {
                    //horizontal fitting, fix p_x to grid
                    float new_x = RoundToNearestX((float)p_x);
                    p_x = new_x;
                    //adjust right-side vertical edge
                    EdgeLine rightside = p.GetMatchingVerticalEdge();
                }
                //tx.MoveTo((float)p_x, (float)p_y);
                genPoints.Add(new Point2d((float)p_x, (float)p_y));
                //-------------
                first_px = p_x;
                first_py = p_y;
            }

            for (int i = 1; i < j; ++i)
            {
                //all merge point is polygon point
                GlyphPoint2D p = flattenPoints[i];
                p_x = p.x * pixelScale;
                p_y = p.y * pixelScale;


                if (y_axis && p.isPartOfHorizontalEdge && p.isUpperSide)  //TODO: review here
                {
                    //vertical fitting, fit p_y to grid
                    p_y = RoundToNearestY(p, (float)p_y, useHalfPixel);
                }

                if (x_axis && p.IsPartOfVerticalEdge && p.IsLeftSide)
                {
                    //horizontal fitting, fix p_x to grid
                    float new_x = RoundToNearestX((float)p_x);
                    p_x = new_x;
                }

                genPoints.Add(new Point2d((float)p_x, (float)p_y));
            }
        }
    }

}