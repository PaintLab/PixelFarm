//BSD, 2014-present, WinterDev 
//adapt from Paper.js

using System;
using System.Collections.Generic;

using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using PixelFarm.VectorMath;

using burningmime.curves; //for curve fit


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

        VertexStore _latestVxs;
        List<VertexStore> _subVxsPathList;
        List<Vector2> _contPoints = new List<Vector2>();
        //
        RectD _boundingRect = new RectD();
        bool _cachedValid = false;
        const int SUBPATH_POINT_LIMIT = 100;


        public MyBrushPath()
        {
            this.StrokeColor = Drawing.Color.Transparent;
            _latestVxs = new VertexStore();
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
            this._latestVxs = _latestVxs.TranslateToNewVxs(xdiff, ydiff, new VertexStore());
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


        public void SetVxs(VertexStore newVxs)
        {
            //clear existing vxs
            _contPoints.Clear();
            if (_subVxsPathList != null)
            {
                _subVxsPathList.Clear();
                _subVxsPathList = null;
            }
            //replace
            _latestVxs = newVxs;
        }
        public VertexStore GetMergedVxs()
        {
            //merge all vxs into a large one
            if (_subVxsPathList != null)
            {
                using (VxsTemp.Borrow(out var v1))
                {
                    int j = _subVxsPathList.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        v1.AppendVertexStore(_subVxsPathList[i]);
                    }
                    v1.AppendVertexStore(_latestVxs);
                    return v1.CreateTrim();//
                }
            }
            else
            {
                return _latestVxs;
            }
        }

        public int CacheCount => _subVxsPathList != null ? _subVxsPathList.Count : 0;

        public void PaintCache(Painter p)
        {
            if (_subVxsPathList != null)
            {
                int j = _subVxsPathList.Count;
                for (int i = 0; i < j; ++i)
                {
                    p.Fill(_subVxsPathList[i]);
                }
            }
        }
        public void PaintLatest(Painter p)
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
                p.Fill(v2);
            }
        }
        public void MakeRegularPath(float strokeW)
        {
            //convert points to vxs
            if (_cachedValid)
            {
                return;
            }
            _cachedValid = true;
            if (_contPoints.Count == 0)
            {
                _latestVxs = null;
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

                    if (_subVxsPathList == null) { _subVxsPathList = new List<VertexStore>(); }

                    _subVxsPathList.Add(v2.CreateTrim());

                    _contPoints.Add(v);
                    j = _contPoints.Count; //** 

                    v1.Clear();//reuse
                }
                //
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
                _latestVxs.Clear();
                stroke.MakeVxs(v1, _latestVxs);
            }
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

            _latestVxs = new VertexStore();
            int j = cubicBzs.Length;
            //1. 
            if (j > 1)
            {
                //1st
                CubicBezier bz0 = cubicBzs[0];
                _latestVxs.AddMoveTo(bz0.p0.x, bz0.p0.y);
                _latestVxs.AddLineTo(bz0.p0.x, bz0.p0.y);
                if (!bz0.HasSomeNanComponent)
                {
                    _latestVxs.AddCurve4To(
                        bz0.p1.x, bz0.p1.y,
                        bz0.p2.x, bz0.p2.y,
                        bz0.p3.x, bz0.p3.y);
                }
                else
                {
                    _latestVxs.AddLineTo(bz0.p3.x, bz0.p3.y);
                }


                //-------------------------------
                for (int i = 1; i < j; ++i) //start at 1
                {
                    CubicBezier bz = cubicBzs[i];
                    if (!bz.HasSomeNanComponent)
                    {
                        _latestVxs.AddCurve4To(
                            bz.p1.x, bz.p1.y,
                            bz.p2.x, bz.p2.y,
                            bz.p3.x, bz.p3.y);
                    }
                    else
                    {
                        _latestVxs.AddLineTo(bz0.p3.x, bz0.p3.y);
                    }

                }
                //-------------------------------
                //close
                //TODO: we not need this AddLineTo()
                _latestVxs.AddLineTo(bz0.p0.x, bz0.p0.y);
                _latestVxs.AddCloseFigure();
            }
            else if (j == 1)
            {
                CubicBezier bz0 = cubicBzs[0];
                _latestVxs.AddMoveTo(bz0.p0.x, bz0.p0.y);

                if (!bz0.HasSomeNanComponent)
                {
                    _latestVxs.AddCurve4To(
                        bz0.p1.x, bz0.p1.y,
                        bz0.p2.x, bz0.p2.y,
                        bz0.p3.x, bz0.p3.y);
                }
                else
                {
                    _latestVxs.AddLineTo(bz0.p3.x, bz0.p3.y);
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
                cflat.MakeVxs(_latestVxs, v1);
                _latestVxs = v1.CreateTrim();
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
            this._latestVxs = new VertexStore();
            int j = _contPoints.Count;
            if (j > 0)
            {
                Vector2 p = _contPoints[0];
                _latestVxs.AddMoveTo(p.x, p.y);
                for (int i = 1; i < j; ++i)
                {
                    p = _contPoints[i];
                    _latestVxs.AddLineTo(p.x, p.y);
                }
                _latestVxs.AddCloseFigure();
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
                PixelFarm.CpuBlit.VertexProcessing.BoundingRect.GetBoundingRect(_latestVxs, ref _boundingRect);
                _validBoundingRect = true;
            }
            if (this._boundingRect.Contains(x, y))
            {
                //fine tune
                //hit test ***
                return VertexHitTester.IsPointInVxs(this._latestVxs, x, y);
            }
            return false;
        }
    }
}