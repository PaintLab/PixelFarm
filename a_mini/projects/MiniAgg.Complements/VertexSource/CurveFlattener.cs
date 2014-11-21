//2014 BSD,WinterDev   
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

namespace PixelFarm.Agg.VertexSource
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

    public class CurveFlattener
    {
        
        //tools , curve producer
        readonly Curve3 m_curve3 = new Curve3();
        readonly Curve4 m_curve4 = new Curve4();

        public double ApproximationScale
        {
            get
            {
                return m_curve4.ApproximationScale;
            }
            set
            {
                m_curve3.ApproximationScale = value;
                m_curve4.ApproximationScale = value;
            }
        }
        public Curves.CurveApproximationMethod ApproximationMethod
        {
            get
            {
                return m_curve4.ApproximationMethod;
            }
            set
            {
                m_curve3.ApproximationMethod = value;
                m_curve4.ApproximationMethod = value;
            }
        }
        public double AngleTolerance
        {
            get
            {
                return m_curve4.AngleTolerance;
            }
            set
            {
                m_curve3.AngleTolerance = value;
                m_curve4.AngleTolerance = value;
            }
        }
        public double CuspLimit
        {
            get
            {
                return m_curve4.CuspLimit;
            }
            set
            {
                m_curve3.CuspLimit = value;
                m_curve4.CuspLimit = value;
            }
        } 
        public VertexStore MakeVxs(VertexStoreSnap vsnap)
        {

            VertexStore vxs = new VertexStore();
            VertexData lastVertextData = new VertexData();
            m_curve3.Reset();
            m_curve4.Reset();

            var snapIter = vsnap.GetVertexSnapIter();
            double x, y;
            VertexCmd cmd;
            do
            {
                //this vertex
                cmd = snapIter.GetNextVertex(out x, out y);
                VertexData vertexData = new VertexData(cmd, x, y);

                switch (cmd)
                {
                    case VertexCmd.Curve3:
                        {
                            //curve3  have one contol point
                            //current x,y is control point

                            //forward read next for endpoint 
                            double tmp_vx, tmp_vy;
                            cmd = snapIter.GetNextVertex(out tmp_vx, out tmp_vy);
                            VertexData endPoint = new VertexData(cmd, tmp_vx, tmp_vy);                             
                            //----------------------------------------------------- 
                            m_curve3.MakeLines(vxs, lastVertextData.x, lastVertextData.y, x, y, endPoint.x, endPoint.y); 
                            //-----------------------------------------------------
                            vertexData = endPoint;
                            lastVertextData = endPoint;

                        }
                        break;

                    case VertexCmd.Curve4:
                        {
                            //curve 4 has 2 control points
                            //current x,y is first control point
                            //then read next control point
                            //and end point

                            double tmp_vx, tmp_vy;
                            //second control point
                            cmd = snapIter.GetNextVertex(out tmp_vx, out tmp_vy);
                            VertexData secondCtrlPoint = new VertexData(cmd, tmp_vx, tmp_vy);

                            cmd = snapIter.GetNextVertex(out tmp_vx, out tmp_vy);
                            VertexData endPoint = new VertexData(cmd, tmp_vx, tmp_vy);

                            m_curve4.Init(lastVertextData.x, lastVertextData.y, x, y, secondCtrlPoint.x, secondCtrlPoint.y, endPoint.x, endPoint.y);
                            IEnumerator<VertexData> curveIterator = m_curve4.GetVertexIter().GetEnumerator();
                            curveIterator.MoveNext(); // First call returns path_cmd_move_to

                            while (!VertexHelper.IsEmpty(vertexData.command))
                            {
                                curveIterator.MoveNext();

                                if (VertexHelper.IsEmpty(curveIterator.Current.command))
                                {
                                    break;
                                }


                                var position = curveIterator.Current.position;

                                vertexData = new VertexData(VertexCmd.LineTo, position);                                 
                                vxs.AddLineTo(position.X, position.Y);
                                lastVertextData = vertexData;
                            }
                        }
                        break;
                    default:

                        vxs.AddVertex(vertexData.x, vertexData.y, vertexData.command);
                        lastVertextData = vertexData;
                        break;
                }
            } while (cmd != VertexCmd.Empty);
            return vxs;

        }
        
        //public VertexStore MakeVxs2(VertexStoreSnap vsnap)
        //{

        //    VertexStore vxs = new VertexStore();
        //    VertexData lastVertextData = new VertexData();
        //    m_curve3.Reset();
        //    m_curve4.Reset();

        //    var snapIter = vsnap.GetVertexSnapIter();
        //    double x, y;
        //    NxCmdAndFlags cmd;
        //    do
        //    {
        //        cmd = snapIter.GetNextVertex(out x, out y);
        //        VertexData vertexData = new VertexData(cmd, x, y);

        //        switch (cmd)
        //        {
        //            case NxCmdAndFlags.Curve3:
        //                {
        //                    double tmp_vx, tmp_vy;
        //                    cmd = snapIter.GetNextVertex(out tmp_vx, out tmp_vy);

        //                    VertexData vertexDataEnd = new VertexData(cmd, tmp_vx, tmp_vy);
        //                    m_curve3.Init(lastVertextData.x, lastVertextData.y, x, y, vertexDataEnd.x, vertexDataEnd.y);

        //                    IEnumerator<VertexData> curveIterator = m_curve3.GetVertexIter().GetEnumerator();
        //                    curveIterator.MoveNext(); // First call returns path_cmd_move_to

        //                    do
        //                    {
        //                        curveIterator.MoveNext();
        //                        VertexData currentVertextData = curveIterator.Current;
        //                        if (ShapePath.IsEmpty(currentVertextData.command))
        //                        {
        //                            break;
        //                        }

        //                        vertexData = new VertexData(
        //                           NxCmdAndFlags.LineTo,
        //                           currentVertextData.position);

        //                        vxs.AddVertex(vertexData);

        //                        lastVertextData = vertexData;

        //                    } while (!ShapePath.IsEmpty(curveIterator.Current.command));
        //                }
        //                break;

        //            case NxCmdAndFlags.Curve4:
        //                {
        //                    double tmp_vx, tmp_vy;

        //                    cmd = snapIter.GetNextVertex(out tmp_vx, out tmp_vy);
        //                    VertexData vertexDataControl = new VertexData(cmd, tmp_vx, tmp_vy);
        //                    cmd = snapIter.GetNextVertex(out tmp_vx, out tmp_vy);
        //                    VertexData vertexDataEnd = new VertexData(cmd, tmp_vx, tmp_vy);

        //                    m_curve4.Init(lastVertextData.x, lastVertextData.y, x, y, vertexDataControl.x, vertexDataControl.y, vertexDataEnd.x, vertexDataEnd.y);
        //                    IEnumerator<VertexData> curveIterator = m_curve4.GetVertexIter().GetEnumerator();
        //                    curveIterator.MoveNext(); // First call returns path_cmd_move_to

        //                    while (!ShapePath.IsEmpty(vertexData.command))
        //                    {
        //                        curveIterator.MoveNext();

        //                        if (ShapePath.IsEmpty(curveIterator.Current.command))
        //                        {
        //                            break;
        //                        }


        //                        var position = curveIterator.Current.position;

        //                        vertexData = new VertexData(NxCmdAndFlags.LineTo, position);
        //                        vxs.AddVertex(vertexData);

        //                        lastVertextData = vertexData;
        //                    }
        //                }
        //                break;
        //            default:

        //                vxs.AddVertex(vertexData);
        //                lastVertextData = vertexData;
        //                break;
        //        }
        //    } while (cmd != NxCmdAndFlags.Empty);
        //    return vxs;

        //}
        
        
        public VertexStore MakeVxs(VertexStore srcVxs)
        {
            return MakeVxs(new VertexStoreSnap(srcVxs));            
             
        }
    }
}