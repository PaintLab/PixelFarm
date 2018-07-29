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
using PixelFarm.Drawing;
namespace PixelFarm.CpuBlit.VertexProcessing
{



    class StrokeGenerator
    {

        StrokeMath m_stroker;
        VertexStore _tmp_inputVxs;

        VertexStore m_out_vertices;
        double m_shorten;
        bool m_closed;

        public StrokeGenerator()
        {
            m_stroker = new StrokeMath();
            m_out_vertices = new VertexStore();
            _tmp_inputVxs = new VertexStore();
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
            _tmp_inputVxs.Clear();
            m_closed = false;
            _latest_moveX = _latest_moveY = 0;
        }
        public void Close()
        {
            m_closed = true;
            _tmp_inputVxs.AddMoveTo(_latest_moveX, _latest_moveY);
        }

        double _latest_moveX = 0;
        double _latest_moveY = 0;
        double _latestX = 0;
        double _latestY = 0;

        public void AddVertex(double x, double y, VertexCmd cmd)
        {
            //TODO: review 
            switch (cmd)
            {
                case VertexCmd.MoveTo:
                    _tmp_inputVxs.AddMoveTo(_latest_moveX = x, _latest_moveY = y);
                    break;
                case VertexCmd.Close:
                case VertexCmd.CloseAndEndFigure:

                    m_closed = true;
                    if(_latest_moveX != _latestX && _latest_moveX != _latestY)
                    {
                        _tmp_inputVxs.AddMoveTo(_latest_moveX, _latest_moveY);
                    }                    

                    break;
                default:
                    _tmp_inputVxs.AddLineTo(x, y);
                    break;
            }
            _latestX = x;
            _latestY = y;

        }
        public void WriteTo(VertexStore outputVxs)
        {
            GenStroke(outputVxs);
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
            int m_src_vertex = 0;

            //ready
            if (_tmp_inputVxs.Count < 2 + (m_closed ? 1 : 0))
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

                GetFirst2(_tmp_inputVxs, out Vertex2d v0, out Vertex2d v1);
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


                GetFirst2(_tmp_inputVxs, out Vertex2d v0, out Vertex2d v1);
                GetLast2(_tmp_inputVxs, out Vertex2d v_beforeLast, out Vertex2d v_last);

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
            while (m_src_vertex < _tmp_inputVxs.Count - 1)
            {

                GetTripleVertices(_tmp_inputVxs,
                     m_src_vertex,
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

                    GetLast2(_tmp_inputVxs, out Vertex2d beforeLast, out Vertex2d last);
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

                    GetFirst2(_tmp_inputVxs, out Vertex2d v0, out Vertex2d v1);
                    GetLast2(_tmp_inputVxs, out Vertex2d v_beforeLast, out Vertex2d v_last);

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


                GetTripleVertices(
                   _tmp_inputVxs,
                   m_src_vertex,
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
        static void GetTripleVertices(VertexStore vxs, int idx, out Vertex2d prev, out Vertex2d cur, out Vertex2d next)
        {
            //we want 3 vertices
            int len = vxs.Count;
            if (idx > 0 && idx + 2 <= len)
            {
                prev = GetVertex2d(vxs, idx - 1);
                cur = GetVertex2d(vxs, idx);
                next = GetVertex2d(vxs, idx + 1);
            }
            else
            {
                prev = cur = next = new Vertex2d();
            }
        }
        static Vertex2d GetVertex2d(VertexStore vxs, int index)
        {
            vxs.GetVertex(index, out double x0, out double y0);
            return new Vertex2d(x0, y0);
        }
        static void GetFirst2(VertexStore vxs, out Vertex2d first, out Vertex2d second)
        {
            first = GetVertex2d(vxs, 0);
            second = GetVertex2d(vxs, 1);
        }
        static void GetLast2(VertexStore vxs, out Vertex2d beforeLast, out Vertex2d last)
        {
            int len = vxs.Count;
            beforeLast = GetVertex2d(vxs, len - 2);
            last = GetVertex2d(vxs, len - 1);
        }
    }

}