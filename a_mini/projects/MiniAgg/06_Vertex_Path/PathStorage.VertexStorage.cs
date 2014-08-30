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
    partial class PathStorage
    {
        class VertexStorage
        {
            int m_num_vertices;
            int m_allocated_vertices;

            //easy to transfer back to unmanaged part!
            double[] m_coord_xy;
            ShapePath.FlagsAndCommand[] m_CommandAndFlags;

            public VertexStorage()
            {
            }
            public void free_all()
            {
                m_coord_xy = null;
                m_CommandAndFlags = null;
                m_num_vertices = 0;
            }

            public int Count
            {
                get { return m_num_vertices; }
            }
            public void remove_all()
            {
                m_num_vertices = 0;
            }

            public void AddVertex(double x, double y, ShapePath.FlagsAndCommand CommandAndFlags)
            {
                allocate_if_required(m_num_vertices);

                m_coord_xy[m_num_vertices << 1] = x;
                m_coord_xy[(m_num_vertices << 1) + 1] = y;
                m_CommandAndFlags[m_num_vertices] = CommandAndFlags;

                m_num_vertices++;
            }

            public void modify_vertex(int index, double x, double y)
            {
                m_coord_xy[index << 1] = x;
                m_coord_xy[(index << 1) + 1] = y;
            }

            public void modify_vertex(int index, double x, double y, ShapePath.FlagsAndCommand CommandAndFlags)
            {
                //m_coord_x[index] = x;
                //m_coord_y[index] = y;
                m_coord_xy[index << 1] = x;
                m_coord_xy[(index << 1) + 1] = y;

                m_CommandAndFlags[index] = CommandAndFlags;
            }

            public void modify_command(int index, ShapePath.FlagsAndCommand CommandAndFlags)
            {
                m_CommandAndFlags[index] = CommandAndFlags;
            }

            public void swap_vertices(int v1, int v2)
            {

                double x_tmp, y_tmp;
                x_tmp = m_coord_xy[v1 << 1];
                y_tmp = m_coord_xy[(v1 << 1) + 1];

                m_coord_xy[v1 << 1] = m_coord_xy[v2 << 1];//x
                m_coord_xy[(v1 << 1) + 1] = m_coord_xy[(v2 << 1) + 1];//y

                m_coord_xy[v2 << 1] = x_tmp;
                m_coord_xy[(v2 << 1) + 1] = y_tmp;


                ShapePath.FlagsAndCommand cmd = m_CommandAndFlags[v1];
                m_CommandAndFlags[v1] = m_CommandAndFlags[v2];
                m_CommandAndFlags[v2] = cmd;
            }

            public ShapePath.FlagsAndCommand last_command()
            {
                if (m_num_vertices != 0)
                {
                    return command(m_num_vertices - 1);
                }

                return ShapePath.FlagsAndCommand.CommandStop;
            }

            public ShapePath.FlagsAndCommand last_vertex(out double x, out double y)
            {
                if (m_num_vertices != 0)
                {
                    return vertex((int)(m_num_vertices - 1), out x, out y);
                }

                x = new double();
                y = new double();
                return ShapePath.FlagsAndCommand.CommandStop;
            }

            public ShapePath.FlagsAndCommand prev_vertex(out double x, out double y)
            {
                if (m_num_vertices > 1)
                {
                    return vertex((int)(m_num_vertices - 2), out x, out y);
                }
                x = new double();
                y = new double();
                return ShapePath.FlagsAndCommand.CommandStop;
            }
            public double last_x()
            {
                if (m_num_vertices > 0)
                {
                    int index = (int)(m_num_vertices - 1);
                    return m_coord_xy[index << 1];
                }
                return new double();
            }
            public double last_y()
            {
                if (m_num_vertices > 0)
                {
                    int index = (int)(m_num_vertices - 1);
                    return m_coord_xy[(index << 1) + 1];
                }
                return new double();
            }

            public int total_vertices()
            {
                return m_num_vertices;
            }

            public ShapePath.FlagsAndCommand vertex(int index, out double x, out double y)
            {
                var i = index << 1;
                x = m_coord_xy[i];
                y = m_coord_xy[i + 1];
                return m_CommandAndFlags[index];
            }

            public ShapePath.FlagsAndCommand command(int index)
            {
                return m_CommandAndFlags[index];
            }

            void allocate_if_required(int indexToAdd)
            {
                if (indexToAdd < m_allocated_vertices)
                {
                    return;
                }

                while (indexToAdd >= m_allocated_vertices)
                {
                    int newSize = m_allocated_vertices + 256;

                    double[] new_xy = new double[newSize << 1];
                    ShapePath.FlagsAndCommand[] newCmd = new ShapePath.FlagsAndCommand[newSize];
                    if (m_coord_xy != null)
                    {
                        //copy old buffer to new buffer 
                        int actualLen = m_num_vertices << 1;
                        for (int i = actualLen - 1; i >= 0; )
                        {
                            new_xy[i] = m_coord_xy[i];
                            i--;
                            new_xy[i] = m_coord_xy[i];
                            i--;
                        }
                        for (int i = m_num_vertices - 1; i >= 0; --i)
                        {
                            newCmd[i] = m_CommandAndFlags[i];
                        }
                    }
                    m_coord_xy = new_xy;
                    m_CommandAndFlags = newCmd;

                    m_allocated_vertices = newSize;
                }
            }


            //----------------------------------------------------------
            internal static void UnsafeDirectSetData(
                VertexStorage vstore,
                int m_allocated_vertices,
                int m_num_vertices,
                double[] m_coord_xy,
                ShapePath.FlagsAndCommand[] m_CommandAndFlags)
            {
                vstore.m_num_vertices = m_num_vertices;
                vstore.m_allocated_vertices = m_allocated_vertices;
                vstore.m_coord_xy = m_coord_xy;
                vstore.m_CommandAndFlags = m_CommandAndFlags;
            }
            internal static void UnsafeDirectGetData(
                VertexStorage vstore,
                out int m_allocated_vertices,
                out int m_num_vertices,
                out double[] m_coord_xy,
                out ShapePath.FlagsAndCommand[] m_CommandAndFlags)
            {

                m_num_vertices = vstore.m_num_vertices;
                m_allocated_vertices = vstore.m_allocated_vertices;
                m_coord_xy = vstore.m_coord_xy;
                m_CommandAndFlags = vstore.m_CommandAndFlags;
            }

            //----------------------------------------------------------
        }
    }
}