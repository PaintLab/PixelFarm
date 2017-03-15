//BSD, 2014-2017, WinterDev
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

namespace PixelFarm.Agg
{
    class StrokeGeneratorX
    {
        StrokeMath m_stroker;
        VertexDistanceList _curCurtexDistanceList;
        Queue<VertexDistanceList> _vertextDistanceListQueue = new Queue<VertexDistanceList>();


        VertexStore m_out_vertices;
        double m_shorten;
        bool m_closed;
        StrokeMath.Status m_status;
        StrokeMath.Status m_prev_status;
        int m_src_vertex;
        int m_out_vertex;
        public StrokeGeneratorX()
        {
            m_stroker = new StrokeMath();
            m_out_vertices = new VertexStore();
            m_status = StrokeMath.Status.Init;
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
            _curCurtexDistanceList = null;
            if (_curCurtexDistanceList != null)
            {
                _curCurtexDistanceList.Clear();
                _curCurtexDistanceList = null;
            }

            _vertextDistanceListQueue.Clear();
            m_closed = false;
            m_status = StrokeMath.Status.Init;

        }
        public void AddVertex(double x, double y, VertexCmd cmd)
        {
            m_status = StrokeMath.Status.Init;
            switch (cmd)
            {
                case VertexCmd.MoveTo:
                    if (_curCurtexDistanceList == null)
                    {
                        _curCurtexDistanceList = new Agg.VertexDistanceList();
                    }
                    else
                    {
                        _vertextDistanceListQueue.Enqueue(_curCurtexDistanceList);
                        _curCurtexDistanceList = new Agg.VertexDistanceList();
                    }
                    _curCurtexDistanceList.AddVertex(new VertexDistance(x, y)); 
                    break;
                case VertexCmd.Close:
                case VertexCmd.CloseAndEndFigure:
                    m_closed = true;
                    break;
                default:
                    _curCurtexDistanceList.AddVertex(new VertexDistance(x, y));
                    break;
            }
        }

        public void WriteTo(VertexStore outputVxs)
        {
            this.Rewind();
            double x = 0, y = 0;
            for (;;)
            {
                VertexCmd cmd = GetNextVertex(ref x, ref y); 
                if (cmd == VertexCmd.NoMore)
                {
                    if (_vertextDistanceListQueue.Count > 0)
                    {
                        m_status = StrokeMath.Status.Ready;
                        m_src_vertex = 0;
                        m_out_vertex = 0;
                        //switch to first one
                        _curCurtexDistanceList = _vertextDistanceListQueue.Dequeue();
                        continue;
                    }
                    else
                    {
                        break;//exit from loop
                    }
                }
                outputVxs.AddVertex(x, y, cmd);
            }
        }
        void Rewind()
        {
            if (m_status == StrokeMath.Status.Init)
            {
                _curCurtexDistanceList.Close(m_closed);
                VertexHelper.ShortenPath(_curCurtexDistanceList, m_shorten, m_closed);
                if (_curCurtexDistanceList.Count < 3) { m_closed = false; }
            }
            m_status = StrokeMath.Status.Ready;
            m_src_vertex = 0;
            m_out_vertex = 0;
            if (_vertextDistanceListQueue.Count > 0)
            {
                _vertextDistanceListQueue.Enqueue(_curCurtexDistanceList);
                //switch to first one
                _curCurtexDistanceList = _vertextDistanceListQueue.Dequeue();
            }
        }

        VertexCmd GetNextVertex(ref double x, ref double y)
        {
            VertexCmd cmd = VertexCmd.LineTo;
            do
            {
                switch (m_status)
                {
                    case StrokeMath.Status.Init:
                        this.Rewind();
                        goto case StrokeMath.Status.Ready;
                    case StrokeMath.Status.Ready:

                        if (_curCurtexDistanceList.Count < 2 + (m_closed ? 1 : 0))
                        {
                            cmd = VertexCmd.NoMore;
                            break;
                        }
                        m_status = m_closed ? StrokeMath.Status.Outline1 : StrokeMath.Status.Cap1;
                        cmd = VertexCmd.MoveTo;
                        m_src_vertex = 0;
                        m_out_vertex = 0;
                        break;
                    case StrokeMath.Status.Cap1:
                        m_stroker.CreateCap(
                            m_out_vertices,
                            _curCurtexDistanceList[0],
                            _curCurtexDistanceList[1],
                            _curCurtexDistanceList[0].dist);
                        m_src_vertex = 1;
                        m_prev_status = StrokeMath.Status.Outline1;
                        m_status = StrokeMath.Status.OutVertices;
                        m_out_vertex = 0;
                        break;
                    case StrokeMath.Status.Cap2:
                        m_stroker.CreateCap(m_out_vertices,
                            _curCurtexDistanceList[_curCurtexDistanceList.Count - 1],
                            _curCurtexDistanceList[_curCurtexDistanceList.Count - 2],
                            _curCurtexDistanceList[_curCurtexDistanceList.Count - 2].dist);
                        m_prev_status = StrokeMath.Status.Outline2;
                        m_status = StrokeMath.Status.OutVertices;
                        m_out_vertex = 0;
                        break;
                    case StrokeMath.Status.Outline1:
                        {
                            if (m_closed)
                            {
                                if (m_src_vertex >= _curCurtexDistanceList.Count)
                                {
                                    m_prev_status = StrokeMath.Status.CloseFirst;
                                    m_status = StrokeMath.Status.EndPoly1;
                                    break;
                                }
                            }
                            else
                            {
                                if (m_src_vertex >= _curCurtexDistanceList.Count - 1)
                                {
                                    m_status = StrokeMath.Status.Cap2;
                                    break;
                                }
                            }

                            VertexDistance prev, cur, next;
                            _curCurtexDistanceList.GetTripleVertices(m_src_vertex,
                                out prev,
                                out cur,
                                out next);
                            //check if we should join or not ?

                            //don't join it
                            m_stroker.CreateJoin(m_out_vertices,
                           prev,
                           cur,
                           next,
                           prev.dist,
                           cur.dist);

                            ++m_src_vertex;
                            m_prev_status = m_status;
                            m_status = StrokeMath.Status.OutVertices;
                            m_out_vertex = 0;

                        }
                        break;
                    case StrokeMath.Status.CloseFirst:
                        m_status = StrokeMath.Status.Outline2;
                        cmd = VertexCmd.MoveTo;
                        goto case StrokeMath.Status.Outline2;
                    case StrokeMath.Status.Outline2:
                        {
                            if (m_src_vertex <= (!m_closed ? 1 : 0))
                            {
                                m_status = StrokeMath.Status.EndPoly2;
                                m_prev_status = StrokeMath.Status.Stop;
                                break;
                            }

                            --m_src_vertex;

                            VertexDistance prev, cur, next;
                            _curCurtexDistanceList.GetTripleVertices(m_src_vertex,
                                out prev,
                                out cur,
                                out next);

                            m_stroker.CreateJoin(m_out_vertices,
                              next,
                              cur,
                              prev,
                              cur.dist,
                              prev.dist);
                            m_prev_status = m_status;
                            m_status = StrokeMath.Status.OutVertices;
                            m_out_vertex = 0;

                        }
                        break;
                    case StrokeMath.Status.OutVertices:
                        if (m_out_vertex >= m_out_vertices.Count)
                        {
                            m_status = m_prev_status;
                        }
                        else
                        {
                            m_out_vertices.GetVertex(m_out_vertex++, out x, out y);
                            //Vector2 c = m_out_vertices[(int)m_out_vertex++];
                            //x = c.x;
                            //y = c.y;
                            return cmd;
                        }
                        break;
                    case StrokeMath.Status.EndPoly1:
                        m_status = m_prev_status;
                        x = (int)EndVertexOrientation.CCW;
                        return VertexCmd.Close;
                    case StrokeMath.Status.EndPoly2:
                        m_status = m_prev_status;
                        x = (int)EndVertexOrientation.CW;
                        return VertexCmd.Close;
                    case StrokeMath.Status.Stop:
                        cmd = VertexCmd.NoMore;
                        break;
                }

            } while (!VertexHelper.IsEmpty(cmd));
            return cmd;
        }
    }


    class StrokeGenerator
    {
        StrokeMath m_stroker;
        VxsDistanceList2 vxsList = new VxsDistanceList2();


        VertexStore m_out_vertices;
        double m_shorten;
        bool m_closed;
        StrokeMath.Status m_status;
        StrokeMath.Status m_prev_status;
        int m_src_vertex;
        int m_out_vertex;
        public StrokeGenerator()
        {
            m_stroker = new StrokeMath();
            m_out_vertices = new VertexStore();
            m_status = StrokeMath.Status.Init;
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
            vxsList.Clear();
            m_closed = false;
            m_status = StrokeMath.Status.Init;

        }
        public void AddVertex(double x, double y, VertexCmd cmd)
        {
            //TODO: review 
            m_status = StrokeMath.Status.Init;
            switch (cmd)
            {
                case VertexCmd.MoveTo:


                    vxsList.AddMoveTo(x, y);
                    break;
                case VertexCmd.Close:
                case VertexCmd.CloseAndEndFigure:
                    m_closed = true;
                    break;
                default:
                    vxsList.AddVertex(new VertexDistance(x, y));
                    break;
            }
        }

        public void WriteTo(VertexStore outputVxs)
        {

            this.Rewind();
            int currentRangeIndex = 0;
            double x = 0, y = 0;
            for (;;)
            {
                VertexCmd cmd = GetNextVertex(ref x, ref y);
                if (cmd == VertexCmd.NoMore)
                {
                    if (currentRangeIndex + 1 < vxsList.RangeCount)
                    {
                        //move to next range
                        vxsList.SetRangeIndex(currentRangeIndex + 1);
                        currentRangeIndex++;

                        m_status = StrokeMath.Status.Ready;
                        m_src_vertex = 0;
                        m_out_vertex = 0;
                        continue;
                    }
                    else
                    {
                        break;//exit from loop
                    }
                }
                outputVxs.AddVertex(x, y, cmd);
            }
        }
        void Rewind()
        {
            if (m_status == StrokeMath.Status.Init)
            {
                vxsList.Rewind();
                if (vxsList.CurrentRangeLen < 3)
                {
                    //force
                    m_closed = false;
                }
                //_curCurtexDistanceList.Close(m_closed);
                //VertexHelper.ShortenPath(_curCurtexDistanceList, m_shorten, m_closed);
                //if (_curCurtexDistanceList.Count < 3) { m_closed = false; }
            }
            m_status = StrokeMath.Status.Ready;
            m_src_vertex = 0;
            m_out_vertex = 0;
            //if (_vertextDistanceListQueue.Count > 0)
            //{
            //    _vertextDistanceListQueue.Enqueue(_curCurtexDistanceList);
            //    //switch to first one
            //    _curCurtexDistanceList = _vertextDistanceListQueue.Dequeue();
            //}
            vxsList.Rewind();
        }

        VertexCmd GetNextVertex(ref double x, ref double y)
        {
            VertexCmd cmd = VertexCmd.LineTo;
            do
            {
                switch (m_status)
                {
                    case StrokeMath.Status.Init:
                        this.Rewind();
                        goto case StrokeMath.Status.Ready;
                    case StrokeMath.Status.Ready:

                        if (vxsList.CurrentRangeLen < 2 + (m_closed ? 1 : 0))
                        {
                            cmd = VertexCmd.NoMore;
                            break;
                        }
                        m_status = m_closed ? StrokeMath.Status.Outline1 : StrokeMath.Status.Cap1;
                        cmd = VertexCmd.MoveTo;
                        m_src_vertex = 0;
                        m_out_vertex = 0;
                        break;
                    case StrokeMath.Status.Cap1:
                        {
                            VertexDistance v0, v1;
                            vxsList.GetFirst2(out v0, out v1);
                            m_stroker.CreateCap(
                                m_out_vertices,
                                v0,
                                v1,
                                v0.dist);
                            m_src_vertex = 1;
                            m_prev_status = StrokeMath.Status.Outline1;
                            m_status = StrokeMath.Status.OutVertices;
                            m_out_vertex = 0;
                        }
                        break;
                    case StrokeMath.Status.Cap2:
                        {
                            VertexDistance beforeLast, last;
                            vxsList.GetLast2(out beforeLast, out last);
                            m_stroker.CreateCap(m_out_vertices,
                                last,
                                beforeLast,
                                beforeLast.dist);
                            m_prev_status = StrokeMath.Status.Outline2;
                            m_status = StrokeMath.Status.OutVertices;
                            m_out_vertex = 0;
                        }
                        break;
                    case StrokeMath.Status.Outline1:
                        {
                            if (m_closed)
                            {
                                if (m_src_vertex >= vxsList.CurrentRangeLen)
                                {
                                    m_prev_status = StrokeMath.Status.CloseFirst;
                                    m_status = StrokeMath.Status.EndPoly1;
                                    break;
                                }
                            }
                            else
                            {
                                if (m_src_vertex >= vxsList.CurrentRangeLen - 1)
                                {
                                    m_status = StrokeMath.Status.Cap2;
                                    break;
                                }
                            }

                            VertexDistance prev, cur, next;
                            vxsList.GetTripleVertices(m_src_vertex,
                                out prev,
                                out cur,
                                out next);
                            //check if we should join or not ?

                            //don't join it
                            m_stroker.CreateJoin(m_out_vertices,
                           prev,
                           cur,
                           next,
                           prev.dist,
                           cur.dist);

                            ++m_src_vertex;
                            m_prev_status = m_status;
                            m_status = StrokeMath.Status.OutVertices;
                            m_out_vertex = 0;

                        }
                        break;
                    case StrokeMath.Status.CloseFirst:
                        m_status = StrokeMath.Status.Outline2;
                        cmd = VertexCmd.MoveTo;
                        goto case StrokeMath.Status.Outline2;
                    case StrokeMath.Status.Outline2:
                        {
                            if (m_src_vertex <= (!m_closed ? 1 : 0))
                            {
                                m_status = StrokeMath.Status.EndPoly2;
                                m_prev_status = StrokeMath.Status.Stop;
                                break;
                            }

                            --m_src_vertex;

                            VertexDistance prev, cur, next;
                            vxsList.GetTripleVertices(m_src_vertex,
                                out prev,
                                out cur,
                                out next);

                            m_stroker.CreateJoin(m_out_vertices,
                              next,
                              cur,
                              prev,
                              cur.dist,
                              prev.dist);
                            m_prev_status = m_status;
                            m_status = StrokeMath.Status.OutVertices;
                            m_out_vertex = 0;

                        }
                        break;
                    case StrokeMath.Status.OutVertices:
                        if (m_out_vertex >= m_out_vertices.Count)
                        {
                            m_status = m_prev_status;
                        }
                        else
                        {
                            m_out_vertices.GetVertex(m_out_vertex++, out x, out y);
                            //Vector2 c = m_out_vertices[(int)m_out_vertex++];
                            //x = c.x;
                            //y = c.y;
                            return cmd;
                        }
                        break;
                    case StrokeMath.Status.EndPoly1:
                        m_status = m_prev_status;
                        x = (int)EndVertexOrientation.CCW;
                        return VertexCmd.Close;
                    case StrokeMath.Status.EndPoly2:
                        m_status = m_prev_status;
                        x = (int)EndVertexOrientation.CW;
                        return VertexCmd.Close;
                    case StrokeMath.Status.Stop:
                        cmd = VertexCmd.NoMore;
                        break;
                }

            } while (!VertexHelper.IsEmpty(cmd));
            return cmd;
        }
    }

    class VxsDistanceList2
    {
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
        List<VertexDistance> _vxsList = new List<VertexDistance>();
        List<Range> _ranges = new List<Range>();
        Range _range;
        VertexDistance _latest = new VertexDistance();
        int _rangeIndex = 0;
        public void AddMoveTo(double x, double y)
        {
            //TODO: review here
            //1. stop current range
            if (_ranges.Count > 0)
            {
                _ranges[_ranges.Count - 1].SetEndAt(_vxsList.Count);
            }
            _ranges.Add(_range = new Range(_vxsList.Count));
            AddVertex(new Agg.VertexDistance(x, y));
        }

        public int RangeIndex { get { return this._rangeIndex; } }
        public void SetRangeIndex(int index)
        {
            this._rangeIndex = index;
            _range = _ranges[index];
        }
        public int RangeCount
        {
            get { return _ranges.Count; }
        }
        public int CurrentRangeLen
        {
            get { return _range.len; }
        }
        public void AddLineTo(double x, double y)
        {
            AddVertex(new Agg.VertexDistance(x, y));
        }
        public void AddVertex(VertexDistance val)
        {
            int count = _range.Count;
            //Ensure that the new one is not duplicate with the last one
            switch (count)
            {
                case 0:
                    _vxsList.Add(_latest = val);
                    _range.SetLen(count + 1);
                    break;
                default:
                    if (!_vxsList[count - 1].IsEqual(val))
                    {
                        _range.SetLen(count + 1);
                        _vxsList.Add(_latest = val);
                    }
                    break;
            }
        }
        public void Clear()
        {
            _ranges.Clear();
            _vxsList.Clear();
            _latest = new Agg.VertexDistance();
            _rangeIndex = 0;
        }
        public void Rewind()
        {
            _rangeIndex = 0;
            if (_ranges.Count > 0)
            {
                _range = _ranges[_rangeIndex];
            }
        }

        public void ReplaceLast(VertexDistance val)
        {
            _vxsList.RemoveAt(_vxsList.Count - 1);
            AddVertex(val);
        }
        public void GetTripleVertices(int idx, out VertexDistance prev, out VertexDistance cur, out VertexDistance next)
        {
            //we want 3 vertices
            if (idx > 1 && idx + 2 <= _range.Count)
            {
                prev = _vxsList[_range.beginAt + idx - 1];
                cur = _vxsList[_range.beginAt + idx];
                next = _vxsList[_range.beginAt + idx + 1];
                prev.UpdateLen(cur);
                cur.UpdateLen(next);
            }
            else
            {
                prev = cur = next = new VertexDistance();
            }
        }
        public void GetFirst2(out VertexDistance first, out VertexDistance second)
        {
            first = _vxsList[_range.beginAt];
            second = _vxsList[_range.beginAt + 1];
            first.UpdateLen(second);
        }
        public void GetLast2(out VertexDistance beforeLast, out VertexDistance last)
        {
            beforeLast = _vxsList[_range.beginAt + _range.len - 2];
            last = _vxsList[_range.beginAt + _range.len - 1];
            beforeLast.UpdateLen(last);
        }
    }


}