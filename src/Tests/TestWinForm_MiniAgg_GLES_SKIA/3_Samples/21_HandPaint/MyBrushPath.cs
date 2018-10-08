//BSD, 2014-present, WinterDev 
//adapt from Paper.js

using System;
using System.Collections.Generic;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.VectorMath;
using burningmime.curves; //for curve fit
using PixelFarm.Drawing;

namespace PixelFarm.CpuBlit.Samples
{
    public enum SmoothBrushMode
    {
        SolidBrush,
        EraseBrush,
        CutBrush
    }
    public enum EditMode
    {
        Draw,
        Select
    }
    class MyBrushPath
    {

        bool _validBoundingRect;
        VertexStore _vxs;
        List<VertexStore> _subVxsList;
        List<Vector2> _contPoints = new List<Vector2>();
        RectD _boundingRect = new RectD();
        bool _cachedValid = false;
        const int SUBPATH_POINT_LIMIT = 100;


        public MyBrushPath()
        {
            this.StrokeColor = Drawing.Color.Transparent;
            _vxs = new VertexStore();

        }
        public void AddPointAtLast(int x, int y)
        {
            _contPoints.Add(new Vector2(x, y));
            _cachedValid = false;
        }
        public void AddPointAtFirst(int x, int y)
        {
            _contPoints.Insert(0, new Vector2(x, y));
            _cachedValid = false;
        }
        public void UpdateCachePainter(Painter cachePainter, float strokeW)
        {
            MakeRegularPath(strokeW, cachePainter);
        }
        public void FillPath(Painter p, float strokeW)
        {
            p.Fill(_vxs);
        }
        public Vector2 GetStartPoint()
        {
            if (_contPoints != null)
            {
                if (_contPoints.Count > 0)
                {
                    return _contPoints[0];
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
            if (_contPoints != null)
            {
                if (_contPoints.Count > 0)
                {
                    return _contPoints[_contPoints.Count - 1];
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
            get { return this._boundingRect; }
        }

        public void MoveBy(int xdiff, int ydiff)
        {
            //apply translation  
            //TODO: review here again, not to use new VertexStore()
            this._vxs = _vxs.TranslateToNewVxs(xdiff, ydiff, new VertexStore());
            _boundingRect.Offset(xdiff, ydiff);
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


        public void PaintLatest(Painter cachePainter)
        {
            int j = _contPoints.Count;
            using (VectorToolBox.Borrow(out Stroke stroke))
            using (VxsTemp.Borrow(out var v1, out var v2))
            {
                for (int i = 0; i < j; ++i)
                {
                    Vector2 v = _contPoints[i];
                    if (i == 0)
                    {
                        v1.AddMoveTo(v.x, v.y);
                    }
                    else
                    {
                        v1.AddLineTo(v.x, v.y);
                    }
                }

                stroke.MakeVxs(v1, v2);
                cachePainter.Fill(v2);
            }
        }
        public void MakeRegularPath(float strokeW, Painter cachePainter)
        {

            if (_cachedValid)
            {
                return;
            }
            _cachedValid = true;
            if (_contPoints.Count == 0)
            {
                _vxs = null;
                return;
            }

            using (VectorToolBox.Borrow(out Stroke stroke))
            using (VxsTemp.Borrow(out var v1, out var v2))
            {
                stroke.Width = strokeW;
                int j = _contPoints.Count;
                while (j > SUBPATH_POINT_LIMIT)
                {
                    //split the old one 
                    Vector2 v = new Vector2();
                    for (int i = 0; i < SUBPATH_POINT_LIMIT; ++i)
                    {
                        v = _contPoints[i];
                        if (i == 0)
                        {
                            v1.AddMoveTo(v.x, v.y);
                        }
                        else
                        {
                            v1.AddLineTo(v.x, v.y);
                        }
                    }

                    _contPoints.RemoveRange(0, SUBPATH_POINT_LIMIT);

                    stroke.MakeVxs(v1, v2);
                    //1.
                    // _vxs = v2.CreateTrim(); 
                    cachePainter.Fill(v2);

                    //ActualBitmap cacheBmp = CreateBmpCache(v2);
                    //
                    if (_subVxsList == null) { _subVxsList = new List<VertexStore>(); }
                    //if (_subBmpCacheList == null) { _subBmpCacheList = new List<ActualBitmap>(); } 
                    _subVxsList.Add(_vxs);
                    //_subBmpCacheList.Add(cacheBmp);
                    //
                    //
                    _contPoints.Add(v);
                    j = _contPoints.Count; //**

                    //Console.WriteLine(_subBmpCacheList.Count);
                }

                for (int i = 0; i < j; ++i)
                {
                    Vector2 v = _contPoints[i];
                    if (i == 0)
                    {
                        v1.AddMoveTo(v.x, v.y);
                    }
                    else
                    {
                        v1.AddLineTo(v.x, v.y);
                    }
                }
                _vxs.Clear();
                stroke.MakeVxs(v1, _vxs);
            }
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
            List<Vector2> data2 = _contPoints;
            CubicBezier[] cubicBzs = CurveFit.Fit(data2, 8);

            _vxs = new VertexStore();
            int j = cubicBzs.Length;
            //1. 
            if (j > 1)
            {
                //1st
                CubicBezier bz0 = cubicBzs[0];
                _vxs.AddMoveTo(bz0.p0.x, bz0.p0.y);
                _vxs.AddLineTo(bz0.p0.x, bz0.p0.y);
                if (!bz0.HasSomeNanComponent)
                {
                    _vxs.AddCurve4To(
                        bz0.p1.x, bz0.p1.y,
                        bz0.p2.x, bz0.p2.y,
                        bz0.p3.x, bz0.p3.y);
                }
                else
                {
                    _vxs.AddLineTo(bz0.p3.x, bz0.p3.y);
                }


                //-------------------------------
                for (int i = 1; i < j; ++i) //start at 1
                {
                    CubicBezier bz = cubicBzs[i];
                    if (!bz.HasSomeNanComponent)
                    {
                        _vxs.AddCurve4To(
                            bz.p1.x, bz.p1.y,
                            bz.p2.x, bz.p2.y,
                            bz.p3.x, bz.p3.y);
                    }
                    else
                    {
                        _vxs.AddLineTo(bz0.p3.x, bz0.p3.y);
                    }

                }
                //-------------------------------
                //close
                //TODO: we not need this AddLineTo()
                _vxs.AddLineTo(bz0.p0.x, bz0.p0.y);
                _vxs.AddCloseFigure();
            }
            else if (j == 1)
            {
                CubicBezier bz0 = cubicBzs[0];
                _vxs.AddMoveTo(bz0.p0.x, bz0.p0.y);

                if (!bz0.HasSomeNanComponent)
                {
                    _vxs.AddCurve4To(
                        bz0.p1.x, bz0.p1.y,
                        bz0.p2.x, bz0.p2.y,
                        bz0.p3.x, bz0.p3.y);
                }
                else
                {
                    _vxs.AddLineTo(bz0.p3.x, bz0.p3.y);
                }



            }
            else
            {
                // = 0
            }

            //TODO: review here
            using (VectorToolBox.Borrow(out CurveFlattener cflat))
            using (VxsTemp.Borrow(out var v1))
            {
                cflat.MakeVxs(_vxs, v1);
                _vxs = v1.CreateTrim();
            }
        }
        public void MakeSmoothPath()
        {
            if (this._cachedValid)
            {
                return;
            }
            this._cachedValid = true;
            //--------
            if (_contPoints.Count == 0)
            {
                return;
            }
            SimplifyPaths();


        }
        public void Close()
        {
            this._vxs = new VertexStore();
            int j = _contPoints.Count;
            if (j > 0)
            {
                Vector2 p = _contPoints[0];
                _vxs.AddMoveTo(p.x, p.y);
                for (int i = 1; i < j; ++i)
                {
                    p = _contPoints[i];
                    _vxs.AddLineTo(p.x, p.y);
                }
                _vxs.AddCloseFigure();
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
            _validBoundingRect = false;
        }
        public bool HitTest(int x, int y)
        {
            //check if point in polygon
            if (!_validBoundingRect)
            {
                PixelFarm.CpuBlit.VertexProcessing.BoundingRect.GetBoundingRect(_vxs, ref _boundingRect);
                _validBoundingRect = true;
            }
            if (this._boundingRect.Contains(x, y))
            {
                //fine tune
                //hit test ***
                return VertexHitTester.IsPointInVxs(this._vxs, x, y);
            }
            return false;
        }
    }
}