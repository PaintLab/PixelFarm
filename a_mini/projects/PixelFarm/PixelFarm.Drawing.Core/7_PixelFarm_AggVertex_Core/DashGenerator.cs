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
            int _currentMarkNo;

            double _currentSegLen;
            DashState _state;
            double _latest_X, _latest_Y;
            //-------------------------------
            internal VertexStore _output;

            public void AddSegmentMark(double markSegmentLen)
            {
                _segmentMarks.Add(new LineSegmentMark(markSegmentLen));
            }
            public void Clear()
            {

                _segmentMarks.Clear();
                _currentMarkNo = 0;
            }
            //-----------------------------------------------------
            public void MoveTo(double x0, double y0)
            {
                switch (_state)
                {
                    default: throw new NotSupportedException();
                    case DashState.Init:
                        _latest_X = x0;
                        _latest_Y = y0; 

                        StepToNextMarkerSegment(); 

                        break;
                    case DashState.PolyLine:
                        //stop current dash
                        //and reset to line start

                        break;
                }
            }
            void StepToNextMarkerSegment()
            {
                _currentMarker = _segmentMarks[_currentMarkNo];
                _currentSegLen = _currentMarker.len;
                if (_currentMarkNo + 1 < _segmentMarks.Count)
                {
                    _currentMarkNo++;
                }
                else
                {
                    _currentMarkNo = 0;
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
                            //find line segment length 
                            double newlineLen = AggMath.calc_distance(_latest_X, _latest_Y, x1, y1);
                            //check current gen state
                            //find angle
                            double angle = Math.Atan2(y1 - _latest_Y, x1 - _latest_X);
                            double cos = Math.Cos(angle);
                            double sin = Math.Sin(angle);
                            while (newlineLen > _currentSegLen)
                            {
                                //we can create a new segment
                                double new_x = _latest_X + (_currentSegLen * cos);
                                double new_y = _latest_Y + (_currentSegLen * sin);
                                newlineLen -= _currentSegLen;
                                //each segment has its own line production procedure
                                //eg. 
                                if ((_currentMarkNo % 2) == 0)
                                {
                                    _output.AddMoveTo(_latest_X, _latest_Y);
                                    _output.AddLineTo(new_x, new_y);
                                }
                                else
                                {

                                }
                                //--------------------
                                StepToNextMarkerSegment();
                                //--------------------
                                _latest_Y = new_y;
                                _latest_X = new_x;
                            }
                        }
                        break;
                }
            }

        }



    }

}