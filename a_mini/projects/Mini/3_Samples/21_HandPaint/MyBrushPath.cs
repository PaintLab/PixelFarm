//BSD 2014,2015 WinterDev 
//adapt from Paper.js

using System;
using System.Collections.Generic;
using PixelFarm.Agg.Transform;
using PixelFarm.VectorMath;
using burningmime.curves; //for curve fit
namespace PixelFarm.Agg.Samples
{
    class MyBrushPath
    {
        bool validBoundingRect;
        VertexStore vxs;
        internal List<Vector2> contPoints = new List<Vector2>();
        RectD boundingRect = new RectD();
        bool isValidSmooth = false;
        public MyBrushPath()
        {
            this.StrokeColor = ColorRGBA.Transparent;
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
        public VertexStore Vxs { get { return vxs; } }
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
        public ColorRGBA FillColor
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
            this.vxs = Affine.TranslateToVxs(vxs, xdiff, ydiff);
            boundingRect.Offset(xdiff, ydiff);
        }
        public ColorRGBA StrokeColor
        {
            get;
            set;
        }
        public SmoothBrushMode BrushMode
        {
            get;
            set;
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
            //return;
            //--------
            //lets smooth it 
            //string str1 = dbugDumpPointsToString(contPoints);
            //string str2 = dbugDumpPointsToString2(contPoints); 
            //var data2 = CurvePreprocess.RdpReduce(contPoints, 2);
            var data2 = contPoints;
            CubicBezier[] cubicBzs = CurveFit.Fit(data2, 8);
            //PathWriter pWriter = new PathWriter();
            //pWriter.StartFigure();

            //int j = cubicBzs.Length;
            //for (int i = 0; i < j; ++i)
            //{
            //    CubicBezier bz = cubicBzs[i];
            //    pWriter.MoveTo(bz.p0.x, bz.p0.y);
            //    pWriter.LineTo(bz.p0.x, bz.p0.y);

            //    pWriter.Curve4(bz.p1.x, bz.p1.y,
            //            bz.p2.x, bz.p2.y,
            //            bz.p3.x, bz.p3.y);
            //}
            //pWriter.CloseFigureCCW();  
            vxs = new VertexStore();
            int j = cubicBzs.Length;
            //1. 
            if (j > 0)
            {
                //1st
                CubicBezier bz0 = cubicBzs[0];
                vxs.AddMoveTo(bz0.p0.x, bz0.p0.y);
                vxs.AddLineTo(bz0.p0.x, bz0.p0.y);
                vxs.AddP3c(bz0.p1.x, bz0.p1.y);
                vxs.AddP3c(bz0.p2.x, bz0.p2.y);
                vxs.AddLineTo(bz0.p3.x, bz0.p3.y);
                //-------------------------------
                for (int i = 1; i < j; ++i) //start at 1
                {
                    CubicBezier bz = cubicBzs[i];
                    vxs.AddP3c(bz.p1.x, bz.p1.y);
                    vxs.AddP3c(bz.p2.x, bz.p2.y);
                    vxs.AddLineTo(bz.p3.x, bz.p3.y);
                }
                //-------------------------------
                //close
                vxs.AddLineTo(bz0.p0.x, bz0.p0.y);
            }
            vxs.AddCloseFigure();
            PixelFarm.Agg.VertexSource.CurveFlattener cflat = new PixelFarm.Agg.VertexSource.CurveFlattener();
            vxs = cflat.MakeVxs(vxs);
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
                PixelFarm.Agg.BoundingRect.GetBoundingRect(new Agg.VertexStoreSnap(vxs), ref boundingRect);
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