//MIT, 2014-2017, WinterDev
//----------------------------------------------------------------------------
//some part from
// Anti-Grain Geometry - Version 2.4 
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
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
using PixelFarm.Agg.VertexSource;
namespace PixelFarm.Agg
{
    public class DashGenerator
    {
        LineMarker lineMarker = new LineMarker();
        public void AddDashMark(double len)
        {
            lineMarker.AddSegmentMark(len);
        }

        public void MakeVxs(VertexStore src, VertexStore output)
        {
            //
            //we do not flatten the curve 
            // 
            lineMarker._output = output;
            int count = src.Count;
            VertexCmd cmd;
            double x, y;
            for (int i = 0; i < count; ++i)
            {
                cmd = src.GetVertex(i, out x, out y);
                switch (cmd)
                {
                    case VertexCmd.MoveTo:
                        lineMarker.MoveTo(x, y);
                        break;
                    case VertexCmd.NoMore:
                        i = count + 1; //force end
                        break;
                    case VertexCmd.LineTo:
                        lineMarker.LineTo(x, y);
                        break;
                    case VertexCmd.P2c:
                    case VertexCmd.P3c:
                        throw new NotSupportedException();
                        break;
                    case VertexCmd.Close:
                    case VertexCmd.CloseAndEndFigure:
                        lineMarker.CloseFigure();
                        break;
                }
            }

        }
        enum DashState
        {
            Init,
            PolyLine,
        }

        class LineSegmentMark
        {
            public readonly double len;
            public LineSegmentMark(double len)
            {
                this.len = len;
            }
        }

        class LineMarker
        {

            List<LineSegmentMark> _segmentMarks = new List<LineSegmentMark>();
            LineSegmentMark _currentMarker;
            int _nextMarkNo;

            double _expectedSegmentLen;
            DashState _state;
            double _latest_X, _latest_Y;
            double _latest_moveto_X, _lastest_moveto_Y;

            //-------------------------------
            internal VertexStore _output;

            public void AddSegmentMark(double markSegmentLen)
            {
                _segmentMarks.Add(new LineSegmentMark(markSegmentLen));
            }
            public void Clear()
            {
                _total_accum_len = 0;
                _segmentMarks.Clear();
                _nextMarkNo = 0;
            }
            //-----------------------------------------------------
            void StepToNextMarkerSegment()
            {
                _currentMarker = _segmentMarks[_nextMarkNo];
                _expectedSegmentLen = _currentMarker.len;
                if (_nextMarkNo + 1 < _segmentMarks.Count)
                {
                    _nextMarkNo++;
                }
                else
                {
                    _nextMarkNo = 0;
                }
            }
            public void CloseFigure()
            {
                LineTo(_latest_moveto_X, _lastest_moveto_Y);
            }
            public void MoveTo(double x0, double y0)
            {
                switch (_state)
                {
                    default: throw new NotSupportedException();
                    case DashState.Init:
                        _latest_moveto_X = _latest_X = x0;
                        _lastest_moveto_Y = _latest_Y = y0;
                        StepToNextMarkerSegment();//start read
                        OnMoveTo();
                        break;
                    case DashState.PolyLine:
                        //stop current line 

                        break;
                }
            }
            public void LineTo(double x1, double y1)
            {
                switch (_state)
                {
                    default: throw new NotSupportedException();
                    case DashState.Init:

                        _state = DashState.PolyLine;
                        goto case DashState.PolyLine;
                    case DashState.PolyLine:
                        {

                            //clear prev segment len  
                            //find line segment length 
                            double new_remaining_len = AggMath.calc_distance(_latest_X, _latest_Y, x1, y1);


                            //check current gen state
                            //find angle
                            double angle = Math.Atan2(y1 - _latest_Y, x1 - _latest_X);
                            double sin = Math.Sin(angle);
                            double cos = Math.Cos(angle);
                            double new_x, new_y;

                            OnBeginLineSegment(sin, cos, ref new_remaining_len);

                            while (new_remaining_len >= _expectedSegmentLen)
                            {
                                //we can create a new segment
                                new_x = _latest_X + (_expectedSegmentLen * cos);
                                new_y = _latest_Y + (_expectedSegmentLen * sin);
                                new_remaining_len -= _expectedSegmentLen;
                                //each segment has its own line production procedure
                                //eg.  
                                OnSegment(new_x, new_y);
                                //--------------------
                                _latest_Y = new_y;
                                _latest_X = new_x;
                            }
                            //set on corner 
                            OnEndLineSegment(x1, y1, new_remaining_len);
                        }
                        break;
                }
            }
            protected virtual void OnBeginLineSegment(double sin, double cos, ref double new_remaining_len)
            {
                if (_total_accum_len > 0)
                {
                    //there is an incomplete len from prev step
                    //check if we can create a segment or not
                    if (_total_accum_len + new_remaining_len >= _expectedSegmentLen)
                    {
                        //***                        
                        //clear all previous collected points
                        int j = _tempPoints.Count;
                        double tmp_expectedLen = _expectedSegmentLen;
                        for (int i = 0; i < j;)
                        {
                            //p0-p1
                            TmpPoint p0 = _tempPoints[i];
                            TmpPoint p1 = _tempPoints[i + 1];

                            if (i == 0)
                            {
                                //move to
                                _output.AddMoveTo(p0.x, p0.y);
                            }
                            _output.AddLineTo(p1.x, p1.y);
                            double len = AggMath.calc_distance(p0.x, p0.y, p1.x, p1.y);
                            tmp_expectedLen -= len;
                            i += 2;
                            _latest_X = p1.x;
                            _latest_Y = p1.y;
                        }
                        _tempPoints.Clear();
                        //-----------------
                        //begin
                        if (tmp_expectedLen > 0)
                        {
                            //we can create a new segment
                            double new_x = _latest_X + (tmp_expectedLen * cos);
                            double new_y = _latest_Y + (tmp_expectedLen * sin);
                            new_remaining_len -= _expectedSegmentLen;
                            //each segment has its own line production procedure
                            //eg.  
                            _output.AddLineTo(this._latest_X = new_x, this._latest_Y = new_y);
                            StepToNextMarkerSegment();
                        }
                        //-----------------   
                        _total_accum_len = 0;
                    }
                    else
                    {

                    }
                }
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
            }


            List<TmpPoint> _tempPoints = new List<TmpPoint>();
            protected virtual void OnEndLineSegment(double x, double y, double remainingLen)
            {
                //remainingLen of current segment
                if (remainingLen > 0)
                {
                    //there are remaining segment that can be complete at this state
                    //so we just collect it
                    _total_accum_len += remainingLen;
                    _tempPoints.Add(new TmpPoint(_latest_X, _latest_Y));
                    _tempPoints.Add(new TmpPoint(x, y));


                }

            }
            protected virtual void OnMoveTo()
            {

            }

            double _total_accum_len;
            protected virtual void OnSegment(double new_x, double new_y)
            {
                //on complete segment ***
                //user can config
                //what todo on complete segment


                if ((_nextMarkNo % 2) == 1)
                {
                    _output.AddMoveTo(_latest_X, _latest_Y);
                    _output.AddLineTo(new_x, new_y);
                }
                else
                {

                }
                _total_accum_len = 0;
                StepToNextMarkerSegment();
            }
        }

    }

}