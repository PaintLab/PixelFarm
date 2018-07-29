//BSD, 2014-present, WinterDev
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using PixelFarm.VectorMath;
using PixelFarm.Drawing;
namespace PixelFarm.CpuBlit.VertexProcessing
{



    class StrokeGenerator
    {




        StrokeMath m_stroker;
        MultiPartsVertexList multipartVertexDistanceList = new MultiPartsVertexList();
        VertexStore m_out_vertices;
        double m_shorten;
        bool m_closed;
        int m_src_vertex;
        public StrokeGenerator()
        {
            m_stroker = new StrokeMath();
            m_out_vertices = new VertexStore();

            m_closed = false;
        }

        public LineCap LineCap
        {
            get { return this.m_stroker.LineCap; }
            set { this.m_stroker.LineCap = value; }
        }
        public LineJoin LineJoin
        {
            get { return this.m_stroker.LineJoin; }
            set { this.m_stroker.LineJoin = value; }
        }
        public InnerJoin InnerJoin
        {
            get { return this.m_stroker.InnerJoin; }
            set { this.m_stroker.InnerJoin = value; }
        }

        public double Width
        {
            get { return m_stroker.Width; }
            set { this.m_stroker.Width = value; }
        }
        public void SetMiterLimitTheta(double t) { m_stroker.SetMiterLimitTheta(t); }


        public double InnerMiterLimit
        {
            get { return this.m_stroker.InnerMiterLimit; }
            set { this.m_stroker.InnerMiterLimit = value; }
        }
        public double MiterLimit
        {
            get { return this.m_stroker.InnerMiterLimit; }
            set { this.m_stroker.InnerMiterLimit = value; }
        }
        public double ApproximateScale
        {
            get { return this.m_stroker.ApproximateScale; }
            set { this.m_stroker.ApproximateScale = value; }
        }
        public bool AutoDetectOrientation
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }
        public double Shorten
        {
            get { return this.m_shorten; }
            set { this.m_shorten = value; }
        }
        // Vertex Generator Interface
        public void Reset()
        {
            multipartVertexDistanceList.Clear();
            m_closed = false;
        }
        public void Close()
        {
            m_closed = true;
            multipartVertexDistanceList.Close();
        }
        public void AddVertex(double x, double y, VertexCmd cmd)
        {
            //TODO: review 

            switch (cmd)
            {
                case VertexCmd.MoveTo:
                    multipartVertexDistanceList.AddMoveTo(x, y);
                    break;
                case VertexCmd.Close:
                case VertexCmd.CloseAndEndFigure:
                    //  m_closed = true;
                    multipartVertexDistanceList.Close();
                    break;
                default:
                    multipartVertexDistanceList.AddVertex(new Vertex2d(x, y));
                    break;
            }
        }
        public void WriteTo(VertexStore outputVxs)
        {
            GenStroke(outputVxs);
        }

        void Rewind()
        {
            multipartVertexDistanceList.Rewind();
            if (multipartVertexDistanceList.CurrentRangeLen < 3)
            {
                //force
                m_closed = false;
            }
            m_src_vertex = 0;
            multipartVertexDistanceList.Rewind();
        }

        void AppendVertices(VertexStore dest, VertexStore src, int src_index = 0)
        {
            int j = src.Count;
            for (int i = src_index; i < j; ++i)
            {
                VertexCmd cmd = src.GetVertex(i, out double x, out double y);
                if (cmd != VertexCmd.NoMore)
                {
                    dest.AddVertex(x, y, cmd);
                }
            }
        }
        void GenStroke(VertexStore output)
        {
            //agg_vcgen_stroke.cpp 
            this.Rewind();//1.  
            //ready
            if (multipartVertexDistanceList.CurrentRangeLen < 2 + (m_closed ? 1 : 0))
            {

                return;
            }
            //
            //we start at cap1 
            //check if close polygon or not
            //if lines( not close) => then start with some kind of line cap
            //if closed polygon => start with outline


            double latest_moveX = 0;
            double latest_moveY = 0;

            if (!m_closed)
            {
                //cap1

                multipartVertexDistanceList.GetFirst2(out Vertex2d v0, out Vertex2d v1);
                m_stroker.CreateCap(
                    m_out_vertices,
                    v0,
                    v1,
                    v0.CalLen(v1));

                m_out_vertices.GetVertex(0, out latest_moveX, out latest_moveY);
                AppendVertices(output, m_out_vertices);
            }
            else
            {


                multipartVertexDistanceList.GetFirst2(out Vertex2d v0, out Vertex2d v1);
                multipartVertexDistanceList.GetLast2(out Vertex2d v_beforeLast, out Vertex2d v_last);

                if (v_last.x == v0.x && v_last.y == v0.y)
                {
                    v_last = v_beforeLast;
                }

                // v_last-> v0-> v1
                m_stroker.CreateJoin(m_out_vertices,
                    v_last,
                    v0,
                    v1,
                    v_last.CalLen(v0),
                    v0.CalLen(v1));
                m_out_vertices.GetVertex(0, out latest_moveX, out latest_moveY);
                output.AddMoveTo(latest_moveX, latest_moveY);
                //others 
                AppendVertices(output, m_out_vertices, 1);

            }
            //----------------
            m_src_vertex = 1;
            //----------------

            //line until end cap 
            while (m_src_vertex < multipartVertexDistanceList.CurrentRangeLen - 1)
            {

                multipartVertexDistanceList.GetTripleVertices(m_src_vertex,
                    out Vertex2d prev,
                    out Vertex2d cur,
                    out Vertex2d next);
                //check if we should join or not ?

                //don't join it
                m_stroker.CreateJoin(m_out_vertices,
                   prev,
                   cur,
                   next,
                   prev.CalLen(cur),
                   cur.CalLen(next));

                ++m_src_vertex;


                AppendVertices(output, m_out_vertices);
            }

            //do cap 2 => turn back
            {
                if (!m_closed)
                {

                    multipartVertexDistanceList.GetLast2(out Vertex2d beforeLast, out Vertex2d last);
                    m_stroker.CreateCap(m_out_vertices,
                        last, //**please note different direction (compare with above)
                        beforeLast,
                        beforeLast.CalLen(last));

                    AppendVertices(output, m_out_vertices);
                }
                else
                {

                    output.GetVertex(0, out latest_moveX, out latest_moveY);
                    output.AddLineTo(latest_moveX, latest_moveY);
                    output.AddCloseFigure();
                    //begin inner
                    //move to inner 

                    // v_last <- v0 <- v1

                    multipartVertexDistanceList.GetFirst2(out Vertex2d v0, out Vertex2d v1);
                    multipartVertexDistanceList.GetLast2(out Vertex2d v_beforeLast, out Vertex2d v_last);

                    if (v_last.x == v0.x && v_last.y == v0.y)
                    {
                        v_last = v_beforeLast;
                    }

                    //**please note different direction (compare with above)

                    m_stroker.CreateJoin(m_out_vertices,
                        v1,
                        v0,
                        v_last,
                        v1.CalLen(v0),
                        v0.CalLen(v_last));


                    m_out_vertices.GetVertex(0, out latest_moveX, out latest_moveY);
                    output.AddMoveTo(latest_moveX, latest_moveY);
                    //others 
                    AppendVertices(output, m_out_vertices, 1);

                }
            }
            //and turn back to begin
            --m_src_vertex;
            while (m_src_vertex > 0)
            {


                multipartVertexDistanceList.GetTripleVertices(m_src_vertex,
                    out Vertex2d prev,
                    out Vertex2d cur,
                    out Vertex2d next);

                m_stroker.CreateJoin(m_out_vertices,
                  next, //**please note different direction (compare with above)
                  cur,
                  prev,
                  cur.CalLen(next),
                  prev.CalLen(cur));

                --m_src_vertex;

                AppendVertices(output, m_out_vertices);
            }

            {
                if (!m_closed)
                {
                    output.GetVertex(0, out latest_moveX, out latest_moveY);

                }
                output.AddLineTo(latest_moveX, latest_moveY);
                output.AddCloseFigure();

            }
        }

    }

    class MultiPartsVertexList
    {
        //TODO make this struct
        class Range
        {
            public int beginAt;
            public int len;
            public Range(int beginAt)
            {
                this.beginAt = beginAt;
                this.len = 0;
            }
            public int Count
            {
                get { return len; }
            }
            public void SetLen(int len)
            {
                this.len = len;
            }
            public void SetEndAt(int endAt)
            {
                this.len = endAt - beginAt;
            }
        }


        Vertex2d _latestVertex = new Vertex2d();

        List<Vertex2d> _vertextDistanceList = new List<Vertex2d>();
        List<Range> _ranges = new List<Range>(); //prev ranges

        Range _latestRange; //current range (before each close)
        int _rangeIndex = 0;//point to reading index in to _ranges List

        double _latestMoveToX;
        double _latestMoveToY;

        public MultiPartsVertexList()
        {

        }
        public void AddVertex(Vertex2d val)
        {
            int count = _latestRange.Count;
            if (count == 0)
            {
                _vertextDistanceList.Add(_latestVertex = val);
                _latestRange.SetLen(count + 1);
            }
            else
            {
                //Ensure that the new one is not duplicate with the last one
                if (!_latestVertex.IsEqual(val))
                {
                    _latestRange.SetLen(count + 1);
                    _vertextDistanceList.Add(_latestVertex = val);
                }
            }

        }
        public void Close()
        {
            //close current range
            AddVertex(new Vertex2d(_latestMoveToX, _latestMoveToY));

        }
        public void AddMoveTo(double x, double y)
        {
            //TODO: review here
            //1. stop current range
            if (_ranges.Count > 0)
            {
                //update end of latest range
                _ranges[_ranges.Count - 1].SetEndAt(_vertextDistanceList.Count);
            }

            //start new range with x and y
            _ranges.Add(_latestRange = new Range(_vertextDistanceList.Count));
            AddVertex(new Vertex2d(x, y));
            _latestMoveToX = x;
            _latestMoveToY = y;
        }

        public int RangeIndex { get { return this._rangeIndex; } }
        public void SetRangeIndex(int index)
        {
            this._rangeIndex = index;
            _latestRange = _ranges[index];
        }
        public int RangeCount
        {
            get { return _ranges.Count; }
        }
        public int CurrentRangeLen
        {
            get
            {
                return (_latestRange == null) ? 0 : _latestRange.len;
            }
        }


        public void Clear()
        {
            _ranges.Clear();
            _vertextDistanceList.Clear();
            _latestVertex = new Vertex2d();
            _rangeIndex = 0;
            _latestRange = null;
        }
        public void Rewind()
        {
            _rangeIndex = 0;
            if (_ranges.Count > 0)
            {
                _latestRange = _ranges[_rangeIndex];
            }
        }
        public void GetTripleVertices(int idx, out Vertex2d prev, out Vertex2d cur, out Vertex2d next)
        {
            //we want 3 vertices
            if (idx > 0 && idx + 2 <= _latestRange.Count)
            {
                prev = _vertextDistanceList[_latestRange.beginAt + idx - 1];
                cur = _vertextDistanceList[_latestRange.beginAt + idx];
                next = _vertextDistanceList[_latestRange.beginAt + idx + 1];

            }
            else
            {
                prev = cur = next = new Vertex2d();
            }
        }
        public void GetFirst2(out Vertex2d first, out Vertex2d second)
        {
            first = _vertextDistanceList[_latestRange.beginAt];
            second = _vertextDistanceList[_latestRange.beginAt + 1];
        }
        public void GetLast2(out Vertex2d beforeLast, out Vertex2d last)
        {
            beforeLast = _vertextDistanceList[_latestRange.beginAt + _latestRange.len - 2];
            last = _vertextDistanceList[_latestRange.beginAt + _latestRange.len - 1];
        }

    }

}