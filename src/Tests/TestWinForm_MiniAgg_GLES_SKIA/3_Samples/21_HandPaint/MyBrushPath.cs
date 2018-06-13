//BSD, 2014-present, WinterDev 
//adapt from Paper.js

using System;
using System.Collections.Generic;
using PixelFarm.Agg.Transform;
using PixelFarm.VectorMath;
using burningmime.curves; //for curve fit
using PixelFarm.Drawing;

namespace PixelFarm.Agg.Samples
{
    class MyBrushPath
    {


        bool validBoundingRect;
        VertexStore vxs;
        internal List<Vector2> contPoints = new List<Vector2>();
        RectD boundingRect = new RectD();
        VertexSource.CurveFlattener cflat = new VertexSource.CurveFlattener();
        bool isValidSmooth = false;
        public MyBrushPath()
        {
            this.StrokeColor = Drawing.Color.Transparent;
        }
        public void AddPointAtLast(int x, int y)
        {
            contPoints.Add(new Vector2(x, y));
            isValidSmooth = false;
        }
        public void AddPointAtFirst(int x, int y)
        {
            contPoints.Insert(0, new Vector2(x, y));
            isValidSmooth = false;
        }
        public VertexStore Vxs
        {
            get
            {
                return vxs;
            }
        }
        public void SetVxs(VertexStore vxs)
        {
            this.vxs = vxs;
            validBoundingRect = false;
        }
        public Vector2 GetStartPoint()
        {
            if (contPoints != null)
            {
                if (contPoints.Count > 0)
                {
                    return contPoints[0];
                }
                else
                {
                    return new Vector2();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public Vector2 GetEndPoint()
        {
            if (contPoints != null)
            {
                if (contPoints.Count > 0)
                {
                    return contPoints[contPoints.Count - 1];
                }
                else
                {
                    return new Vector2();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public Drawing.Color FillColor
        {
            get;
            set;
        }
        public RectD BoundingRect
        {
            get { return this.boundingRect; }
        }

        public void MoveBy(int xdiff, int ydiff)
        {
            //apply translation  
            //TODO: review here again, not to use new VertexStore()
            this.vxs = vxs.TranslateToNewVxs(xdiff, ydiff, new VertexStore());
            boundingRect.Offset(xdiff, ydiff);
        }
        public Drawing.Color StrokeColor
        {
            get;
            set;
        }
        public SmoothBrushMode BrushMode
        {
            get;
            set;
        }



        Stroke _stroke1 = new Stroke(1);

        public void MakeRegularPath(float strokeW)
        {

            if (this.isValidSmooth)
            {
                return;
            }
            this.isValidSmooth = true;
            //--------

            if (contPoints.Count == 0)
            {
                return;
            }

            //SimplifyPaths();
            _stroke1.Width = strokeW*2;
            _stroke1.LineCap = LineCap.Round;
            _stroke1.LineJoin = LineJoin.Round;

            var tmpVxs = new VertexStore();
            int j = contPoints.Count;
            for (int i = 0; i < j; ++i)
            {
                //TODO: review here
                //
                Vector2 v = contPoints[i];
                if (i == 0)
                {
                    tmpVxs.AddMoveTo(v.x, v.y);
                }
                else
                {
                    tmpVxs.AddLineTo(v.x, v.y);
                }
            }
            ////

            VertexStore v2 = new VertexStore();
            _stroke1.MakeVxs(tmpVxs, v2);



            vxs = v2;

            //release vxs to pool
        }

        void SimplifyPaths()
        {

            //return;
            //--------
            //lets smooth it 
            //string str1 = dbugDumpPointsToString(contPoints);
            //string str2 = dbugDumpPointsToString2(contPoints); 
            //var data2 = CurvePreprocess.RdpReduce(contPoints, 2);
            List<Vector2> data2 = contPoints;
            CubicBezier[] cubicBzs = CurveFit.Fit(data2, 8);
            vxs = new VertexStore();
            int j = cubicBzs.Length;
            //1. 
            if (j > 1)
            {
                //1st
                CubicBezier bz0 = cubicBzs[0];
                vxs.AddMoveTo(bz0.p0.x, bz0.p0.y);
                vxs.AddLineTo(bz0.p0.x, bz0.p0.y);
                if (!bz0.HasSomeNanComponent)
                {
                    vxs.AddCurve4To(
                        bz0.p1.x, bz0.p1.y,
                        bz0.p2.x, bz0.p2.y,
                        bz0.p3.x, bz0.p3.y);
                }
                else
                {
                    vxs.AddLineTo(bz0.p3.x, bz0.p3.y);
                }


                //-------------------------------
                for (int i = 1; i < j; ++i) //start at 1
                {
                    CubicBezier bz = cubicBzs[i];
                    if (!bz.HasSomeNanComponent)
                    {
                        vxs.AddCurve4To(
                            bz.p1.x, bz.p1.y,
                            bz.p2.x, bz.p2.y,
                            bz.p3.x, bz.p3.y);
                    }
                    else
                    {
                        vxs.AddLineTo(bz0.p3.x, bz0.p3.y);
                    }

                }
                //-------------------------------
                //close
                //TODO: we not need this AddLineTo()
                vxs.AddLineTo(bz0.p0.x, bz0.p0.y);
                vxs.AddCloseFigure();
            }
            else if (j == 1)
            {
                CubicBezier bz0 = cubicBzs[0];
                vxs.AddMoveTo(bz0.p0.x, bz0.p0.y);

                if (!bz0.HasSomeNanComponent)
                {
                    vxs.AddCurve4To(
                        bz0.p1.x, bz0.p1.y,
                        bz0.p2.x, bz0.p2.y,
                        bz0.p3.x, bz0.p3.y);
                }
                else
                {
                    vxs.AddLineTo(bz0.p3.x, bz0.p3.y);
                }



            }
            else
            {
                // = 0
            }

            //TODO: review here
            VertexStore v2 = new VertexStore();
            cflat.MakeVxs(vxs, v2);
            vxs = v2;
        }
        public void MakeSmoothPath()
        {
            if (this.isValidSmooth)
            {
                return;
            }
            this.isValidSmooth = true;
            //--------
            if (contPoints.Count == 0)
            {
                return;
            }
            SimplifyPaths();


        }
        public void Close()
        {
            this.vxs = new VertexStore();
            int j = contPoints.Count;
            if (j > 0)
            {
                var p = contPoints[0];
                vxs.AddMoveTo(p.x, p.y);
                for (int i = 1; i < j; ++i)
                {
                    p = contPoints[i];
                    vxs.AddLineTo(p.x, p.y);
                }
                vxs.AddCloseFigure();
            }
        }
#if DEBUG
        static string dbugDumpPointsToString(List<Vector2> points)
        {
            System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
            int j = points.Count;
            stbuilder.Append("new VECTOR[]{");
            for (int i = 0; i < j; ++i)
            {
                var pp = points[i];
                stbuilder.Append("new VECTOR(" + pp.x.ToString() + "," + pp.y.ToString() + ")");
                if (i < j - 1)
                {
                    stbuilder.Append(',');
                }
            }
            stbuilder.Append("}");
            return stbuilder.ToString();
        }
        static string dbugDumpPointsToString2(List<Vector2> points)
        {
            System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
            int j = points.Count;
            stbuilder.Append("new Point[]{");
            for (int i = 0; i < j; ++i)
            {
                var pp = points[i];
                stbuilder.Append("new Point(" + pp.x.ToString() + "," + pp.y.ToString() + ")");
                if (i < j - 1)
                {
                    stbuilder.Append(',');
                }
            }
            stbuilder.Append("}");
            return stbuilder.ToString();
        }
#endif
        public void InvalidateBoundingRect()
        {
            validBoundingRect = false;
        }
        public bool HitTest(int x, int y)
        {
            //check if point in polygon
            if (!validBoundingRect)
            {
                PixelFarm.Agg.BoundingRect.GetBoundingRect(new VertexStoreSnap(vxs), ref boundingRect);
                validBoundingRect = true;
            }
            if (this.boundingRect.Contains(x, y))
            {
                //fine tune
                //hit test ***
                return VertexHitTester.IsPointInVxs(this.vxs, x, y);
            }
            return false;
        }
    }
}