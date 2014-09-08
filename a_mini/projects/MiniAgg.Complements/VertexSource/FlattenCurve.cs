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
//
// classes conv_curve
//
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace MatterHackers.Agg.VertexSource
{
    //---------------------------------------------------------------conv_curve
    // Curve converter class. Any path storage can have Bezier curves defined 
    // by their control points. There're two types of curves supported: curve3 
    // and curve4. Curve3 is a conic Bezier curve with 2 endpoints and 1 control
    // point. Curve4 has 2 control points (4 points in total) and can be used
    // to interpolate more complicated curves. Curve4, unlike curve3 can be used 
    // to approximate arcs, both circular and elliptical. Curves are approximated 
    // with straight lines and one of the approaches is just to store the whole 
    // sequence of vertices that approximate our curve. It takes additional 
    // memory, and at the same time the consecutive vertices can be calculated 
    // on demand. 
    //
    // Initially, path storages are not suppose to keep all the vertices of the
    // curves (although, nothing prevents us from doing so). Instead, path_storage
    // keeps only vertices, needed to calculate a curve on demand. Those vertices
    // are marked with special commands. So, if the path_storage contains curves 
    // (which are not real curves yet), and we render this storage directly, 
    // all we will see is only 2 or 3 straight line segments (for curve3 and 
    // curve4 respectively). If we need to see real curves drawn we need to 
    // include this class into the conversion pipeline. 
    //
    // Class conv_curve recognizes commands path_cmd_curve3 and path_cmd_curve4 
    // and converts these vertices into a move_to/line_to sequence. 
    //-----------------------------------------------------------------------
    public class FlattenCurves
    {

        readonly Curve3 m_curve3 = new Curve3();
        readonly Curve4 m_curve4 = new Curve4();
        readonly VertexSnap vertextSource;

        public FlattenCurves(VertexSnap spath)
        {
            this.vertextSource = spath;
        }
        public FlattenCurves(VertexStorage vxs)
        {
            this.vertextSource = new VertexSnap(vxs);
        }
        public double ApproximationScale
        {
            get
            {
                return m_curve4.ApproximationScale;
            }

            set
            {
                m_curve3.approximation_scale(value);
                m_curve4.ApproximationScale = value;
            }
        }


        public Curves.CurveApproximationMethod ApproximationMethod
        {
            set
            {
                m_curve3.approximation_method(value);
                m_curve4.ApproximationMethod = value;
            }

            get
            {

                return m_curve4.ApproximationMethod;
            }
        }

        public double AngleTolerance
        {
            set
            {
                m_curve3.angle_tolerance(value);
                m_curve4.AngleTolerance = value;
            }

            get
            {
                return m_curve4.AngleTolerance;
            }
        }

        public double CuspLimit
        {
            set
            {
                m_curve3.cusp_limit(value);
                m_curve4.CuspLimit = value;
            }

            get
            {
                return m_curve4.CuspLimit;

            }
        }


        public VertexStorage MakeVxs()
        {

            List<VertexData> list = new List<VertexData>();
            foreach (var v in this.GetVertexIter())
            {
                list.Add(v);
            }
            return new VertexStorage(list);

        }

        IEnumerable<VertexData> GetVertexIter()
        {
            this.RewindZero();
            VertexData lastPosition = new VertexData();

            IEnumerator<VertexData> vertexDataEnumerator = vertextSource.GetVertexIter().GetEnumerator();
            while (vertexDataEnumerator.MoveNext())
            {
                VertexData vertexData = vertexDataEnumerator.Current;
                switch (vertexData.command)
                {
                    case ShapePath.FlagsAndCommand.CommandCurve3:
                        {
                            vertexDataEnumerator.MoveNext();
                            VertexData vertexDataEnd = vertexDataEnumerator.Current;
                            m_curve3.Init(lastPosition.x, lastPosition.y, vertexData.x, vertexData.y, vertexDataEnd.x, vertexDataEnd.y);
                            IEnumerator<VertexData> curveIterator = m_curve3.GetVertexIter().GetEnumerator();
                            curveIterator.MoveNext(); // First call returns path_cmd_move_to
                            do
                            {
                                curveIterator.MoveNext();
                                if (ShapePath.IsStop(curveIterator.Current.command))
                                {
                                    break;
                                }
                                vertexData = new VertexData(ShapePath.FlagsAndCommand.CommandLineTo, curveIterator.Current.position);
                                yield return vertexData;
                                lastPosition = vertexData;
                            } while (!ShapePath.IsStop(curveIterator.Current.command));
                        }
                        break;

                    case ShapePath.FlagsAndCommand.CommandCurve4:
                        {
                            vertexDataEnumerator.MoveNext();
                            VertexData vertexDataControl = vertexDataEnumerator.Current;
                            vertexDataEnumerator.MoveNext();
                            VertexData vertexDataEnd = vertexDataEnumerator.Current;
                            m_curve4.Init(lastPosition.x, lastPosition.y, vertexData.x, vertexData.y, vertexDataControl.x, vertexDataControl.y, vertexDataEnd.x, vertexDataEnd.y);
                            IEnumerator<VertexData> curveIterator = m_curve4.GetVertexIter().GetEnumerator();
                            curveIterator.MoveNext(); // First call returns path_cmd_move_to

                            while (!ShapePath.IsStop(vertexData.command))
                            {
                                curveIterator.MoveNext();

                                if (ShapePath.IsStop(curveIterator.Current.command))
                                {
                                    break;
                                }
                                vertexData = new VertexData(ShapePath.FlagsAndCommand.CommandLineTo, curveIterator.Current.position);
                                yield return vertexData;
                                lastPosition = vertexData;
                            }
                        }
                        break;

                    default:
                        yield return vertexData;
                        lastPosition = vertexData;
                        break;
                }
            }
        }

        void RewindZero()
        {
            vertextSource.RewindZero();
            m_curve3.Reset();
            m_curve4.Reset();
        }

    }
}