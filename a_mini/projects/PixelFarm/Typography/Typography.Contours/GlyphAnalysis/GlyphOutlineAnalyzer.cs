//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;
using Typography.OpenFont;
namespace Typography.Contours
{
    //This is PixelFarm's AutoFit
    //NOT FREE TYPE AUTO FIT***

    public class GlyphOutlineAnalyzer
    {
        GlyphPartFlattener _glyphFlattener = new GlyphPartFlattener();
        GlyphContourBuilder _glyphToContour = new GlyphContourBuilder();
        public GlyphOutlineAnalyzer()
        {

        }

        /// <summary>
        /// calculate and create GlyphFitOutline
        /// </summary>
        /// <param name="glyphPoints"></param>
        /// <param name="glyphContours"></param>
        /// <returns></returns>
        public GlyphDynamicOutline CreateDynamicOutline(GlyphPointF[] glyphPoints, ushort[] glyphContours)
        {

            //1. convert original glyph point to contour
            _glyphToContour.Read(glyphPoints, glyphContours);
            //2. get result as list of contour
            List<GlyphContour> contours = _glyphToContour.GetContours();

            int cnt_count = contours.Count;
            //
            if (cnt_count > 0)
            {
                //3.before create dynamic contour we must flatten data inside the contour 
                _glyphFlattener.NSteps = 2;

                for (int i = 0; i < cnt_count; ++i)
                {
                    // (flatten each contour with the flattener)    
                    contours[i].Flatten(_glyphFlattener);
                }
                //4. after flatten, the we can create fit outline
                return CreateDynamicOutline(contours);
            }
            else
            {
                return GlyphDynamicOutline.CreateBlankDynamicOutline();
            }
        }

        /// <summary>
        /// create GlyphDynamicOutline from flatten contours
        /// </summary>
        /// <param name="flattenContours"></param>
        /// <returns></returns>
        static GlyphDynamicOutline CreateDynamicOutline(List<GlyphContour> flattenContours)
        {

            int cntCount = flattenContours.Count;
            GlyphContour cnt = flattenContours[0]; //first contour
            //--------------------------
            //TODO: review here, add hole or not 
            //first polygon
            Poly2Tri.Polygon mainPolygon = CreatePolygon(cnt.flattenPoints);//first contour        
            bool isClockWise = cnt.IsClosewise();
            //review is hole or not here
            //eg i
            //-------------------------- 
            for (int n = 1; n < cntCount; ++n)
            {
                cnt = flattenContours[n];
                //IsHole is correct after we Analyze() the glyph contour
                Poly2Tri.Polygon subPolygon = CreatePolygon(cnt.flattenPoints);
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

            //3. intermediate outline is used inside this lib 
            //and then convert intermediate outline to dynamic outline
            return new GlyphDynamicOutline(
                new GlyphIntermediateOutline(mainPolygon, flattenContours));
        }


        /// <summary>
        /// create polygon from GlyphContour
        /// </summary>
        /// <param name="cnt"></param>
        /// <returns></returns>
        static Poly2Tri.Polygon CreatePolygon(List<GlyphPoint> flattenPoints)
        {
            List<Poly2Tri.TriangulationPoint> points = new List<Poly2Tri.TriangulationPoint>();

            //limitation: poly tri not accept duplicated points! *** 
            double prevX = 0;
            double prevY = 0;

#if DEBUG
            //dbug check if all point is unique 
            dbugCheckAllGlyphsAreUnique(flattenPoints);
#endif


            int j = flattenPoints.Count;
            //pass
            for (int i = 0; i < j; ++i)
            {
                GlyphPoint p = flattenPoints[i];
                double x = p.OX; //start from original X***
                double y = p.OY; //start from original Y***

                if (x == prevX && y == prevY)
                {
                    if (i > 0)
                    {
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    var triPoint = new Poly2Tri.TriangulationPoint(prevX = x, prevY = y) { userData = p };
#if DEBUG
                    p.dbugTriangulationPoint = triPoint;
#endif
                    points.Add(triPoint);

                }
            }

            return new Poly2Tri.Polygon(points.ToArray());

        }
#if DEBUG
        struct dbugTmpPoint
        {
            public readonly double x;
            public readonly double y;
            public dbugTmpPoint(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
            public override string ToString()
            {
                return x + "," + y;
            }
        }
        static Dictionary<dbugTmpPoint, bool> s_debugTmpPoints = new Dictionary<dbugTmpPoint, bool>();
        static void dbugCheckAllGlyphsAreUnique(List<GlyphPoint> flattenPoints)
        {
            double prevX = 0;
            double prevY = 0;
            s_debugTmpPoints = new Dictionary<dbugTmpPoint, bool>();
            int lim = flattenPoints.Count - 1;
            for (int i = 0; i < lim; ++i)
            {
                GlyphPoint p = flattenPoints[i];
                double x = p.OX; //start from original X***
                double y = p.OY; //start from original Y***

                if (x == prevX && y == prevY)
                {
                    if (i > 0)
                    {
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    dbugTmpPoint tmp_point = new dbugTmpPoint(x, y);
                    if (!s_debugTmpPoints.ContainsKey(tmp_point))
                    {
                        //ensure no duplicated point
                        s_debugTmpPoints.Add(tmp_point, true);

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
