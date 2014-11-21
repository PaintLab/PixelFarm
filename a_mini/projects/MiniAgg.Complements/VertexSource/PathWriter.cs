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
using System;
using System.Collections.Generic;
using PixelFarm.VectorMath;

namespace PixelFarm.Agg.VertexSource
{
    //---------------------------------------------------------------path_base
    // A container to store vertices with their flags. 
    // A path consists of a number of contours separated with "move_to" 
    // commands. The path storage can keep and maintain more than one
    // path. 
    // To navigate to the beginning of a particular path, use rewind(path_id);
    // Where path_id is what start_new_path() returns. So, when you call
    // start_new_path() you need to store its return value somewhere else
    // to navigate to the path afterwards.
    //
    // See also: vertex_source concept
    //------------------------------------------------------------------------ 

    public enum SvgPathCommand : byte
    {
        MoveTo,
        LineTo,
        HorizontalLineTo,
        VerticalLineTo,
        CurveTo,
        SmoothCurveTo,
        QuadraticBezierCurve,
        TSmoothQuadraticBezierCurveTo,
        Arc,
        ZClosePath
    }


    /// <summary>
    /// forward path writer
    /// </summary>
    public sealed class PathWriter
    {

        double lastMoveX;
        double lastMoveY;
        double lastX;
        double lastY;

        
        Vector2 curve4c2;
        Vector2 curve3c;
        SvgPathCommand latestSVGPathCmd;

        int figureCount = 0;
        List<int> figureList;
        VertexStore myvxs = new VertexStore();

        public int Count
        {
            get { return myvxs.Count; }
        }
        public void Clear()
        {
            myvxs.Clear();
            lastMoveX = lastMoveY = lastX = lastY = 0;

            curve3c = new Vector2(); 
            curve4c2 = new Vector2();
            latestSVGPathCmd = SvgPathCommand.MoveTo;

            figureCount = 0;

        }
        //--------------------------------------------------------------------
        public int StartFigure()
        {
            if (figureCount > 0)
            {
                //add sep command
                myvxs.AddVertex(0.0, 0.0, VertexCmd.Empty);

            }
            figureCount++;
            return myvxs.Count;
        }
        public void Stop()
        {
            myvxs.AddVertex(0, 0, VertexCmd.Empty);
        }
        //--------------------------------------------------------------------
        public void MoveTo(double x, double y)
        {
            this.latestSVGPathCmd = SvgPathCommand.MoveTo;
            myvxs.AddMoveTo(
                this.lastMoveX = this.lastX = x,
                this.lastMoveY = this.lastY = y);
        }
        public void MoveToRel(double x, double y)
        {
            this.latestSVGPathCmd = SvgPathCommand.MoveTo;
            myvxs.AddMoveTo(
                this.lastMoveX = (this.lastX += x),
                this.lastMoveY = (this.lastY += y));
        }
        public void LineTo(double x, double y)
        {
            this.latestSVGPathCmd = SvgPathCommand.LineTo;
            myvxs.AddLineTo(this.lastX = x, this.lastY = y);
        }
        public void LineToRel(double x, double y)
        {
            this.latestSVGPathCmd = SvgPathCommand.LineTo;
            myvxs.AddLineTo(
                this.lastX += x,
                this.lastY += y);
        }
        public void HorizontalLineTo(double x)
        {
            this.latestSVGPathCmd = SvgPathCommand.HorizontalLineTo;
            myvxs.AddLineTo(this.lastX = x, lastY);
        }
        public void HorizontalLineToRel(double x)
        {
            this.latestSVGPathCmd = SvgPathCommand.HorizontalLineTo;
            myvxs.AddLineTo(this.lastX += x, lastY);
        }
        public void VerticalLineTo(double y)
        {
            this.latestSVGPathCmd = SvgPathCommand.VerticalLineTo;
            myvxs.AddLineTo(lastX, this.lastY = y);
        }
        public void VerticalLineToRel(double y)
        {
            this.latestSVGPathCmd = SvgPathCommand.VerticalLineTo;
            myvxs.AddLineTo(lastX, this.lastY += y);
        }

        //-------------------------------------------------------------------
        static Vector2 CreateMirrorPoint(Vector2 mirrorPoint, Vector2 fixedPoint)
        {
            return new Vector2(
                fixedPoint.X - (mirrorPoint.X - fixedPoint.X),
                fixedPoint.Y - (mirrorPoint.Y - fixedPoint.Y));
        }
        //--------------------------------------------------------------------
        /// <summary>
        ///  Draws a quadratic Bezier curve from the current point to (x,y) using (xControl,yControl) as the control point.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Curve3(double cx, double cy, double x, double y)
        {
            this.latestSVGPathCmd = SvgPathCommand.QuadraticBezierCurve;
            this.curve3c = new Vector2(cx, cy);
            myvxs.AddVertexCurve3(cx, cy);
            myvxs.AddLineTo(this.lastX = x, this.lastY = y);
        }

        /// <summary>
        /// Draws a quadratic Bezier curve from the current point to (x,y) using (xControl,yControl) as the control point.
        /// </summary>
        /// <param name="xControl"></param>
        /// <param name="yControl"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Curve3Rel(double cx, double cy, double x, double y)
        {
            this.latestSVGPathCmd = SvgPathCommand.QuadraticBezierCurve;
            this.curve3c = new Vector2(this.lastX + cx, this.lastY + cy);

            myvxs.AddVertexCurve3(this.lastX + cx, this.lastY + cy);
            myvxs.AddLineTo(this.lastX += x, this.lastY += y);

        }

        /// <summary> 
        /// <para>Draws a quadratic Bezier curve from the current point to (x,y).</para>
        /// <para>The control point is assumed to be the reflection of the control point on the previous command relative to the current point.</para>
        /// <para>(If there is no previous command or if the previous command was not a curve, assume the control point is coincident with the current point.)</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SmoothCurve3(double x, double y)
        {
            switch (this.latestSVGPathCmd)
            {
                case SvgPathCommand.QuadraticBezierCurve:
                case SvgPathCommand.TSmoothQuadraticBezierCurveTo:
                    {
                        //curve3
                        var newC3 = CreateMirrorPoint(this.curve3c, new Vector2(this.lastX, this.lastY));
                        Curve3(newC3.X, newC3.Y, x, y);
                    } break;
                case SvgPathCommand.CurveTo:
                case SvgPathCommand.SmoothCurveTo:
                    {
                        //curve4
                        var newC3 = CreateMirrorPoint(this.curve4c2, new Vector2(this.lastX, this.lastY));
                        Curve3(newC3.X, newC3.Y, x, y);
                    } break;
                default:
                    {
                        Curve3(this.lastX, this.lastY, x, y);
                    } break;
            }
            this.latestSVGPathCmd = SvgPathCommand.TSmoothQuadraticBezierCurveTo; 
        }

        /// <summary>
        /// <para>Draws a quadratic Bezier curve from the current point to (x,y).</para>
        /// <para>The control point is assumed to be the reflection of the control point on the previous command relative to the current point.</para>
        /// <para>(If there is no previous command or if the previous command was not a curve, assume the control point is coincident with the current point.)</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SmoothCurve3Rel(double x, double y)
        {
            this.SmoothCurve3(this.lastX + x, this.lastY + y);
        }
        //-----------------------------------------------------------------------
        public void Curve4(double cx1, double cy1,
                                   double cx2, double xy2,
                                   double x, double y)
        {
            this.latestSVGPathCmd = SvgPathCommand.CurveTo;
            myvxs.AddVertexCurve4(cx1, cy1);
            myvxs.AddVertexCurve4(cx2, xy2);
            myvxs.AddLineTo(this.lastX = x, this.lastY = y);
        }

        public void Curve4Rel(double cx1, double cy1,
                              double cx2, double cy2,
                              double x, double y)
        {

            this.latestSVGPathCmd = SvgPathCommand.CurveTo;
            myvxs.AddVertexCurve4(this.lastX + cx1, this.lastY + cy1);
            myvxs.AddVertexCurve4(this.lastX + cx2, this.lastY + cy2);
            myvxs.AddLineTo(this.lastX += x, this.lastY += y);
        }

        //--------------------------------------------------------------------
        public void SmoothCurve4(double cx2, double cy2,
                       double x, double y)
        {

            switch (this.latestSVGPathCmd)
            {
                case SvgPathCommand.QuadraticBezierCurve:
                case SvgPathCommand.TSmoothQuadraticBezierCurveTo:
                    {
                        //curve3
                        var newC4p1 = CreateMirrorPoint(this.curve3c, new Vector2(this.lastX, this.lastY));
                        Curve4(newC4p1.X, newC4p1.Y, cx2, cy2, x, y);
                    } break;
                case SvgPathCommand.CurveTo:
                case SvgPathCommand.SmoothCurveTo:
                    {
                        //curve4
                        var newC4p1 = CreateMirrorPoint(this.curve4c2, new Vector2(this.lastX, this.lastY));
                        Curve4(newC4p1.X, newC4p1.Y, cx2, cy2, x, y);
                    } break;
                default:
                    {
                        Curve4(this.lastX, this.lastY, cx2, cy2, x, y);
                    } break;
            }
            this.latestSVGPathCmd = SvgPathCommand.SmoothCurveTo;
        }

        public void SmoothCurve4Rel(double cx2, double cy2,
                                    double x, double y)
        {

            SmoothCurve4(this.lastX + cx2, this.lastY + cy2, this.lastX + x, this.lastY + y);
        }

        //=======================================================================
        //TODO: implement arc to ***
        /*
        public void arc_to(double rx, double ry,
                               double angle,
                               bool large_arc_flag,
                               bool sweep_flag,
                               double x, double y)
    {
        if(m_vertices.total_vertices() && is_vertex(m_vertices.last_command()))
        {
            double epsilon = 1e-30;
            double x0 = 0.0;
            double y0 = 0.0;
            m_vertices.last_vertex(&x0, &y0);

            rx = fabs(rx);
            ry = fabs(ry);

            // Ensure radii are valid
            //-------------------------
            if(rx < epsilon || ry < epsilon) 
            {
                line_to(x, y);
                return;
            }

            if(calc_distance(x0, y0, x, y) < epsilon)
            {
                // If the endpoints (x, y) and (x0, y0) are identical, then this
                // is equivalent to omitting the elliptical arc segment entirely.
                return;
            }
            bezier_arc_svg a(x0, y0, rx, ry, angle, large_arc_flag, sweep_flag, x, y);
            if(a.radii_ok())
            {
                join_path(a);
            }
            else
            {
                line_to(x, y);
            }
        }
        else
        {
            move_to(x, y);
        }
    }

    public void arc_rel(double rx, double ry,
                                double angle,
                                bool large_arc_flag,
                                bool sweep_flag,
                                double dx, double dy)
    {
        rel_to_abs(&dx, &dy);
        arc_to(rx, ry, angle, large_arc_flag, sweep_flag, dx, dy);
    }
     */
        //=======================================================================


        public VertexStore Vxs
        {
            get { return this.myvxs; }
        }
        public VertexStoreSnap MakeVertexSnap()
        {
            return new VertexStoreSnap(this.myvxs);
        }

        VertexCmd GetLastVertex(out double x, out double y)
        {
            return myvxs.GetLastVertex(out x, out y);
        }


        // Flip all vertices horizontally or vertically, 
        // between x1 and x2, or between y1 and y2 respectively
        //--------------------------------------------------------------------


        public void ClosePolygonCCW()
        {
            if (VertexHelper.IsVertextCommand(myvxs.GetLastCommand()))
            {
                myvxs.AddVertex((int)EndVertexOrientation.CCW, 0, VertexCmd.EndAndCloseFigure);
            }
        }
        public void ClosePolygon()
        {
            if (VertexHelper.IsVertextCommand(myvxs.GetLastCommand()))
            {
                myvxs.AddVertex(0, 0, VertexCmd.EndAndCloseFigure);
            }
        }
        //// Concatenate path. The path is added as is.
        public void ConcatPath(VertexStoreSnap s)
        {
            double x, y;
            VertexCmd cmd_flags;
            var snapIter = s.GetVertexSnapIter();
            while ((cmd_flags = snapIter.GetNextVertex(out x, out y)) != VertexCmd.Empty)
            {
                myvxs.AddVertex(x, y, cmd_flags);
            }
        }

        //--------------------------------------------------------------------
        // Join path. The path is joined with the existing one, that is, 
        // it behaves as if the pen of a plotter was always down (drawing)
        //template<class VertexSource>  
        public void JoinPath(VertexStoreSnap s)
        {
            double x, y;
            var snapIter = s.GetVertexSnapIter();
            VertexCmd cmd = snapIter.GetNextVertex(out x, out y);
            if (cmd == VertexCmd.Empty)
            {
                return;
            }

            if (VertexHelper.IsVertextCommand(cmd))
            {
                double x0, y0;
                VertexCmd flags0 = GetLastVertex(out x0, out y0);

                if (VertexHelper.IsVertextCommand(flags0))
                {
                    if (AggMath.calc_distance(x, y, x0, y0) > AggMath.VERTEX_DISTANCE_EPSILON)
                    {
                        if (VertexHelper.IsMoveTo(cmd))
                        {
                            cmd = VertexCmd.LineTo;
                        }
                        myvxs.AddVertex(x, y, cmd);
                    }
                }
                else
                {
                    if (VertexHelper.IsEmpty(flags0))
                    {
                        cmd = VertexCmd.MoveTo;
                    }
                    else
                    {
                        if (VertexHelper.IsMoveTo(cmd))
                        {
                            cmd = VertexCmd.LineTo;
                        }
                    }
                    myvxs.AddVertex(x, y, cmd);
                }
            }

            while ((cmd = snapIter.GetNextVertex(out x, out y)) != VertexCmd.Empty)
            {
                myvxs.AddVertex(x, y, VertexHelper.IsMoveTo(cmd) ? VertexCmd.LineTo : cmd);
            }

        }


        public static void UnsafeDirectSetData(
            PathWriter pathStore,
            int m_allocated_vertices,
            int m_num_vertices,
            double[] m_coord_xy,
            VertexCmd[] m_CommandAndFlags)
        {

            VertexStore.UnsafeDirectSetData(
                pathStore.Vxs,
                m_allocated_vertices,
                m_num_vertices,
                m_coord_xy,
                m_CommandAndFlags);
        }
        public static void UnsafeDirectGetData(
            PathWriter pathStore,
            out int m_allocated_vertices,
            out int m_num_vertices,
            out double[] m_coord_xy,
            out VertexCmd[] m_CommandAndFlags)
        {
            VertexStore.UnsafeDirectGetData(
                pathStore.Vxs,
                out m_allocated_vertices,
                out m_num_vertices,
                out m_coord_xy,
                out m_CommandAndFlags);
        }

    }
}