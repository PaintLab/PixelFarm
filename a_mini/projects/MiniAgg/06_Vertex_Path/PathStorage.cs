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
using MatterHackers.VectorMath;

namespace MatterHackers.Agg.VertexSource
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


    public sealed class PathStorage : IVertexDest
    {
        VertexStorage vertices;
        int iteratorIndex;
        public PathStorage()
        {
            vertices = new VertexStorage();
        }
        public PathStorage(PathStorage pathStorageToShareFrom)
        {
            //for share
            vertices = pathStorageToShareFrom.vertices;
        }
        internal VertexStorage Vsx
        {
            get
            {
                return this.vertices;
            }
        }
        public void AddVertex(double x, double y)
        {
            throw new System.NotImplementedException();
        }
        public void AddVertex(double x, double y, ShapePath.FlagsAndCommand flagsAndCommand)
        {
            vertices.AddVertex(x, y, flagsAndCommand);
        }

        public int Count
        {
            get { return vertices.Count; }
        }

        public void Clear()
        {
            vertices.Clear();
            iteratorIndex = 0;
        }

        public void ClearAll()
        {
            vertices.FreeAll();
            iteratorIndex = 0;
        }

        // Make path functions
        //--------------------------------------------------------------------
        public int StartNewPath()
        {
            if (!ShapePath.IsStop(vertices.GetLastCommand()))
            {
                vertices.AddVertex(0.0, 0.0, ShapePath.FlagsAndCommand.CommandStop);
            }
            return vertices.Count;
        }


        public void RelToAbs(ref double x, ref double y)
        {
            if (vertices.Count != 0)
            {
                double x2;
                double y2;
                if (ShapePath.IsVertextCommand(vertices.GetLastVertex(out x2, out y2)))
                {
                    x += x2;
                    y += y2;
                }
            }
        }

        public void MoveTo(double x, double y)
        {
            vertices.AddVertex(x, y, ShapePath.FlagsAndCommand.CommandMoveTo);
        }

        public void LineTo(double x, double y)
        {
            vertices.AddVertex(x, y, ShapePath.FlagsAndCommand.CommandLineTo);
        }

        public void HorizontalLineTo(double x)
        {
            vertices.AddVertex(x, GetLastY(), ShapePath.FlagsAndCommand.CommandLineTo);
        }

        public void VerticalLineTo(double y)
        {
            vertices.AddVertex(GetLastX(), y, ShapePath.FlagsAndCommand.CommandLineTo);
        }

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

        /// <summary>
        /// Draws a quadratic Bézier curve from the current point to (x,y) using (xControl,yControl) as the control point.
        /// </summary>
        /// <param name="xControl"></param>
        /// <param name="yControl"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Curve3(double xControl, double yControl, double x, double y)
        {
            vertices.AddVertex(xControl, yControl, ShapePath.FlagsAndCommand.CommandCurve3);
            vertices.AddVertex(x, y, ShapePath.FlagsAndCommand.CommandCurve3);
        }

        /// <summary>
        /// Draws a quadratic Bézier curve from the current point to (x,y) using (xControl,yControl) as the control point.
        /// </summary>
        /// <param name="xControl"></param>
        /// <param name="yControl"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Curve3Rel(double dx_ctrl, double dy_ctrl, double dx_to, double dy_to)
        {
            RelToAbs(ref dx_ctrl, ref dy_ctrl);
            RelToAbs(ref dx_to, ref dy_to);
            vertices.AddVertex(dx_ctrl, dy_ctrl, ShapePath.FlagsAndCommand.CommandCurve3);
            vertices.AddVertex(dx_to, dy_to, ShapePath.FlagsAndCommand.CommandCurve3);
        }

        /// <summary>
        /// <para>Draws a quadratic Bézier curve from the current point to (x,y).</para>
        /// <para>The control point is assumed to be the reflection of the control point on the previous command relative to the current point.</para>
        /// <para>(If there is no previous command or if the previous command was not a curve, assume the control point is coincident with the current point.)</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Curve3(double x, double y)
        {
            double x0;
            double y0;
            if (ShapePath.IsVertextCommand(vertices.GetLastVertex(out x0, out y0)))
            {
                double x_ctrl;
                double y_ctrl;
                ShapePath.FlagsAndCommand cmd = vertices.GetBeforeLastVetex(out x_ctrl, out y_ctrl);
                if (ShapePath.IsCurve(cmd))
                {
                    x_ctrl = x0 + x0 - x_ctrl;
                    y_ctrl = y0 + y0 - y_ctrl;
                }
                else
                {
                    x_ctrl = x0;
                    y_ctrl = y0;
                }
                Curve3(x_ctrl, y_ctrl, x, y);
            }
        }

        /// <summary>
        /// <para>Draws a quadratic Bézier curve from the current point to (x,y).</para>
        /// <para>The control point is assumed to be the reflection of the control point on the previous command relative to the current point.</para>
        /// <para>(If there is no previous command or if the previous command was not a curve, assume the control point is coincident with the current point.)</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Curve3Rel(double dx_to, double dy_to)
        {
            RelToAbs(ref dx_to, ref dy_to);
            Curve3(dx_to, dy_to);
        }

        public void Curve4(double x_ctrl1, double y_ctrl1,
                                   double x_ctrl2, double y_ctrl2,
                                   double x_to, double y_to)
        {
            vertices.AddVertex(x_ctrl1, y_ctrl1, ShapePath.FlagsAndCommand.CommandCurve4);
            vertices.AddVertex(x_ctrl2, y_ctrl2, ShapePath.FlagsAndCommand.CommandCurve4);
            vertices.AddVertex(x_to, y_to, ShapePath.FlagsAndCommand.CommandCurve4);
        }

        public void Curve4Rel(double dx_ctrl1, double dy_ctrl1,
                                       double dx_ctrl2, double dy_ctrl2,
                                       double dx_to, double dy_to)
        {
            RelToAbs(ref dx_ctrl1, ref dy_ctrl1);
            RelToAbs(ref dx_ctrl2, ref dy_ctrl2);
            RelToAbs(ref dx_to, ref dy_to);
            vertices.AddVertex(dx_ctrl1, dy_ctrl1, ShapePath.FlagsAndCommand.CommandCurve4);
            vertices.AddVertex(dx_ctrl2, dy_ctrl2, ShapePath.FlagsAndCommand.CommandCurve4);
            vertices.AddVertex(dx_to, dy_to, ShapePath.FlagsAndCommand.CommandCurve4);
        }
        public VertexStorage MakeVxs()
        {
            return this.vertices;
        }
        public VertexSnap MakeVertexSnap()
        {
            return new VertexSnap(this.vertices);
        }
        public void Curve4(double x_ctrl2, double y_ctrl2,
                       double x_to, double y_to)
        {
            double x0;
            double y0;
            if (ShapePath.IsVertextCommand(GetLastVertex(out x0, out y0)))
            {
                double x_ctrl1;
                double y_ctrl1;
                ShapePath.FlagsAndCommand cmd = GetBeforeLastVertex(out x_ctrl1, out y_ctrl1);
                if (ShapePath.IsCurve(cmd))
                {
                    x_ctrl1 = x0 + x0 - x_ctrl1;
                    y_ctrl1 = y0 + y0 - y_ctrl1;
                }
                else
                {
                    x_ctrl1 = x0;
                    y_ctrl1 = y0;
                }
                Curve4(x_ctrl1, y_ctrl1, x_ctrl2, y_ctrl2, x_to, y_to);
            }
        }

        public void Curve4Rel(double dx_ctrl2, double dy_ctrl2,
                                       double dx_to, double dy_to)
        {
            RelToAbs(ref dx_ctrl2, ref dy_ctrl2);
            RelToAbs(ref dx_to, ref dy_to);
            Curve4(dx_ctrl2, dy_ctrl2, dx_to, dy_to);
        }

        ShapePath.FlagsAndCommand GetLastVertex(out double x, out double y)
        {
            return vertices.GetLastVertex(out x, out y);
        }

        ShapePath.FlagsAndCommand GetBeforeLastVertex(out double x, out double y)
        {
            return vertices.GetBeforeLastVetex(out x, out y);
        }

        double GetLastX()
        {
            return vertices.GetLastX();
        }

        double GetLastY()
        {
            return vertices.GetLastY();
        }


        public IEnumerable<VertexData> GetVertexIter()
        {
            int count = vertices.Count;
            for (int i = 0; i < count; i++)
            {
                double x = 0;
                double y = 0;
                ShapePath.FlagsAndCommand command = vertices.GetVertex(i, out x, out y);
                yield return new VertexData(command, new Vector2(x, y));
            }
            yield return new VertexData(ShapePath.FlagsAndCommand.CommandStop, new Vector2(0, 0));
        }
        public void RewindZ()
        {
            iteratorIndex = 0;
        }
        public ShapePath.FlagsAndCommand GetNextVertex(out double x, out double y)
        {
            if (iteratorIndex >= vertices.Count)
            {
                x = 0;
                y = 0;
                return ShapePath.FlagsAndCommand.CommandStop;
            }
            return vertices.GetVertex(iteratorIndex++, out x, out y);
        }
        //----------------------------------------------------------------

        // Arrange the orientation of a polygon, all polygons in a path, 
        // or in all paths. After calling arrange_orientations() or 
        // arrange_orientations_all_paths(), all the polygons will have 
        // the same orientation, i.e. path_flags_cw or path_flags_ccw
        //--------------------------------------------------------------------
        int ArrangePolygonOrientation(int start, bool closewise)
        {
            //if (orientation == ShapePath.FlagsAndCommand.FlagNone) return start;

            // Skip all non-vertices at the beginning
            ShapePath.FlagsAndCommand orientFlags = closewise ? ShapePath.FlagsAndCommand.FlagCW : ShapePath.FlagsAndCommand.FlagCCW;

            int vcount = vertices.Count;
            while (start < vcount &&
                  !ShapePath.IsVertextCommand(vertices.GetCommand(start)))
            {
                ++start;
            }

            // Skip all insignificant move_to
            while (start + 1 < vcount &&
                  ShapePath.IsMoveTo(vertices.GetCommand(start)) &&
                  ShapePath.IsMoveTo(vertices.GetCommand(start + 1)))
            {
                ++start;
            }

            // Find the last vertex
            int end = start + 1;
            while (end < vcount &&
                  !ShapePath.IsNextPoly(vertices.GetCommand(end)))
            {
                ++end;
            }


            if (end - start > 2)
            {
                if (PerceivePolygonOrientation(start, end) != orientFlags)
                {
                    // Invert polygon, set orientation flag, and skip all end_poly
                    InvertPolygon(start, end);
                    ShapePath.FlagsAndCommand flags;
                    while (end < vertices.Count &&
                          ShapePath.IsEndPoly(flags = vertices.GetCommand(end)))
                    {
                        vertices.ReplaceCommand(end++, flags | orientFlags);// Path.set_orientation(cmd, orientation));
                    }
                }
            }
            return end;
        }

        int ArrangeOrientations(int start, bool closewise)
        {

            while (start < vertices.Count)
            {
                start = ArrangePolygonOrientation(start, closewise);
                if (ShapePath.IsStop(vertices.GetCommand(start)))
                {
                    ++start;
                    break;
                }
            }

            return start;
        }

        public void ArrangeOrientationsAll(bool closewise)
        {
            int start = 0;
            while (start < vertices.Count)
            {
                start = ArrangeOrientations(start, closewise);
            }
        }


        //public void ArrangeOrientationsAll(ShapePath.FlagsAndCommand orientation)
        //{
        //    if (orientation != ShapePath.FlagsAndCommand.FlagNone)
        //    {

        //    }
        //}

        // Flip all vertices horizontally or vertically, 
        // between x1 and x2, or between y1 and y2 respectively
        //--------------------------------------------------------------------
        public void FlipX(double x1, double x2)
        {
            int i;
            double x, y;
            int count = this.vertices.Count;
            for (i = 0; i < count; ++i)
            {
                ShapePath.FlagsAndCommand flags = vertices.GetVertex(i, out x, out y);
                if (ShapePath.IsVertextCommand(flags))
                {
                    vertices.ReplaceVertex(i, x2 - x + x1, y);
                }
            }
        }

        public void FlipY(double y1, double y2)
        {
            int i;
            double x, y;
            int count = this.vertices.Count;
            for (i = 0; i < count; ++i)
            {
                ShapePath.FlagsAndCommand flags = vertices.GetVertex(i, out x, out y);
                if (ShapePath.IsVertextCommand(flags))
                {
                    vertices.ReplaceVertex(i, x, y2 - y + y1);
                }
            }
        }
        public void ClosePolygon()
        {
            ClosePolygon(ShapePath.FlagsAndCommand.FlagNone);
        }
        public void ClosePolygonCCW()
        {
            ClosePolygon(ShapePath.FlagsAndCommand.FlagCCW);
        }
        void ClosePolygon(ShapePath.FlagsAndCommand flags)
        {
            var flags2 = flags | ShapePath.FlagsAndCommand.FlagClose;

            if (ShapePath.IsVertextCommand(vertices.GetLastCommand()))
            {
                vertices.AddVertex(0.0, 0.0, ShapePath.FlagsAndCommand.CommandEndPoly | flags2);
            }
        }

        //// Concatenate path. The path is added as is.


        public void ConcatPath(VertexSnap s)
        {
            double x, y;
            ShapePath.FlagsAndCommand cmd_flags;
            s.RewindZ();
            while ((cmd_flags = s.GetNextVertex(out x, out y)) != ShapePath.FlagsAndCommand.CommandStop)
            {
                vertices.AddVertex(x, y, cmd_flags);
            }
        }
        //--------------------------------------------------------------------
        // Join path. The path is joined with the existing one, that is, 
        // it behaves as if the pen of a plotter was always down (drawing)
        //template<class VertexSource>  
        public void JoinPath(VertexSnap s)
        {
            double x, y;
            s.RewindZ();
            ShapePath.FlagsAndCommand cmd = s.GetNextVertex(out x, out y);
            if (cmd == ShapePath.FlagsAndCommand.CommandStop)
            {
                return;
            }

            if (ShapePath.IsVertextCommand(cmd))
            {
                double x0, y0;
                ShapePath.FlagsAndCommand flags0 = GetLastVertex(out x0, out y0);

                if (ShapePath.IsVertextCommand(flags0))
                {
                    if (AggMath.calc_distance(x, y, x0, y0) > AggMath.VERTEX_DISTANCE_EPSILON)
                    {
                        if (ShapePath.IsMoveTo(cmd))
                        {
                            cmd = ShapePath.FlagsAndCommand.CommandLineTo;
                        }
                        vertices.AddVertex(x, y, cmd);
                    }
                }
                else
                {
                    if (ShapePath.IsStop(flags0))
                    {
                        cmd = ShapePath.FlagsAndCommand.CommandMoveTo;
                    }
                    else
                    {
                        if (ShapePath.IsMoveTo(cmd))
                        {
                            cmd = ShapePath.FlagsAndCommand.CommandLineTo;
                        }
                    }
                    vertices.AddVertex(x, y, cmd);
                }
            }

            while ((cmd = s.GetNextVertex(out x, out y)) != ShapePath.FlagsAndCommand.CommandStop)
            {
                vertices.AddVertex(x, y, ShapePath.IsMoveTo(cmd) ?
                                      ShapePath.FlagsAndCommand.CommandLineTo :
                                                cmd);
            }

        }

        /*
        // Concatenate polygon/polyline. 
        //--------------------------------------------------------------------
        void concat_poly(T* data, int num_points, bool closed)
        {
            poly_plain_adaptor<T> poly(data, num_points, closed);
            concat_path(poly);
        }

        // Join polygon/polyline continuously.
        //--------------------------------------------------------------------
        void join_poly(T* data, int num_points, bool closed)
        {
            poly_plain_adaptor<T> poly(data, num_points, closed);
            join_path(poly);
        }
         */
        //--------------------------------------------------------------------


        //public void Translate(double dx, double dy, int path_id)
        //{
        //    int num_ver = vertices.Count;
        //    for (; path_id < num_ver; path_id++)
        //    {
        //        double x, y;
        //        ShapePath.FlagsAndCommand flags = this.vertices.GetVertex(path_id, out x, out y);
        //        if (ShapePath.IsStop(flags)) break;
        //        if (ShapePath.IsVertextCommand(flags))
        //        {
        //            x += dx;
        //            y += dy;
        //            vertices.ReplaceVertex(path_id, x, y);
        //        }
        //    }
        //}

        public void TranslateAll(double dx, double dy)
        {
            int index;
            int num_ver = vertices.Count;
            for (index = 0; index < num_ver; index++)
            {
                double x, y;
                if (ShapePath.IsVertextCommand(vertices.GetVertex(index, out x, out y)))
                {
                    x += dx;
                    y += dy;
                    vertices.ReplaceVertex(index, x, y);
                }
            }
        }

        //--------------------------------------------------------------------


        //public void Transform(Transform.Affine trans, int path_id)
        //{
        //    int num_ver = vertices.Count;
        //    for (; path_id < num_ver; path_id++)
        //    {
        //        double x, y;
        //        ShapePath.FlagsAndCommand cmd = vertices.GetVertex(path_id, out x, out y);
        //        if (cmd == ShapePath.FlagsAndCommand.CommandStop)
        //        {
        //            break;
        //        }
        //        if (ShapePath.IsVertextCommand(cmd))
        //        {
        //            trans.Transform(ref x, ref y);
        //            vertices.ReplaceVertex(path_id, x, y);
        //        }
        //    }
        //}

        //--------------------------------------------------------------------
        public void TransformAll(Transform.Affine trans)
        {
            int index;
            int num_ver = vertices.Count;
            for (index = 0; index < num_ver; index++)
            {
                double x, y;
                if (ShapePath.IsVertextCommand(vertices.GetVertex(index, out x, out y)))
                {
                    trans.Transform(ref x, ref y);
                    vertices.ReplaceVertex(index, x, y);
                }
            }
        }

        public void InvertPolygon(int start)
        {
            // Skip all non-vertices at the beginning
            while (start < vertices.Count &&
                  !ShapePath.IsVertextCommand(vertices.GetCommand(start))) ++start;

            // Skip all insignificant move_to
            while (start + 1 < vertices.Count &&
                  ShapePath.IsMoveTo(vertices.GetCommand(start)) &&
                  ShapePath.IsMoveTo(vertices.GetCommand(start + 1))) ++start;

            // Find the last vertex
            int end = start + 1;
            while (end < vertices.Count &&
                  !ShapePath.IsNextPoly(vertices.GetCommand(end))) ++end;

            InvertPolygon(start, end);
        }

        ShapePath.FlagsAndCommand PerceivePolygonOrientation(int start, int end)
        {
            // Calculate signed area (double area to be exact)
            //---------------------
            int np = end - start;
            double area = 0.0;
            int i;
            for (i = 0; i < np; i++)
            {
                double x1, y1, x2, y2;
                vertices.GetVertexXY(start + i, out x1, out y1);
                vertices.GetVertexXY(start + (i + 1) % np, out x2, out y2);
                area += x1 * y2 - y1 * x2;
            }
            return (area < 0.0) ? ShapePath.FlagsAndCommand.FlagCW : ShapePath.FlagsAndCommand.FlagCCW;
        }

        void InvertPolygon(int start, int end)
        {
            int i;
            ShapePath.FlagsAndCommand tmp_PathAndFlags = vertices.GetCommand(start);

            --end; // Make "end" inclusive

            // Shift all commands to one position
            for (i = start; i < end; i++)
            {
                vertices.ReplaceCommand(i, vertices.GetCommand(i + 1));
            }

            // Assign starting command to the ending command
            vertices.ReplaceCommand(end, tmp_PathAndFlags);

            // Reverse the polygon
            while (end > start)
            {
                vertices.SwapVertices(start++, end--);
            }
        }


        //----------------------------------------------------------

        public static void UnsafeDirectSetData(
            PathStorage pathStore,
            int m_allocated_vertices,
            int m_num_vertices,
            double[] m_coord_xy,
            ShapePath.FlagsAndCommand[] m_CommandAndFlags)
        {

            VertexStorage.UnsafeDirectSetData(
                pathStore.vertices,
                m_allocated_vertices,
                m_num_vertices,
                m_coord_xy,
                m_CommandAndFlags);
        }
        public static void UnsafeDirectGetData(
            PathStorage pathStore,
            out int m_allocated_vertices,
            out int m_num_vertices,
            out double[] m_coord_xy,
            out ShapePath.FlagsAndCommand[] m_CommandAndFlags)
        {
            VertexStorage.UnsafeDirectGetData(
                pathStore.vertices,
                out m_allocated_vertices,
                out m_num_vertices,
                out m_coord_xy,
                out m_CommandAndFlags);
        }

    }
}