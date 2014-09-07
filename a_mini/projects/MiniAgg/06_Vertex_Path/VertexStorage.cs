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

namespace MatterHackers.Agg
{
    public class VertexStorage
    {
        int m_num_vertices;
        int m_allocated_vertices;
        double[] m_coord_xy;
        ShapePath.FlagsAndCommand[] m_CommandAndFlags;
       

        public VertexStorage()
        {
        }

        public VertexStorage(List<VertexData> vxlist)
        {
            int j = this.m_num_vertices = vxlist.Count;
            m_coord_xy = new double[(j << 1) + 2];
            m_CommandAndFlags = new ShapePath.FlagsAndCommand[j + 2];
            int n = 0;
            for (int i = 0; i < j; ++i)
            {
                VertexData vxdata = vxlist[i];
                m_coord_xy[n++] = vxdata.position.x;
                m_coord_xy[n++] = vxdata.position.y;
                m_CommandAndFlags[i] = vxdata.command;
            }
        }
        public VertexStorage(List<VertexData> vxlist, bool hasMoreThanOnePart)
            : this(vxlist)
        {
            this.HasMoreThanOnePart = hasMoreThanOnePart;
        }
        public bool HasMoreThanOnePart { get; set; }
        public void FreeAll()
        {
            m_coord_xy = null;
            m_CommandAndFlags = null;
            m_num_vertices = 0;
        }
        public int Count
        {
            get { return m_num_vertices; }
        }
        public void Clear()
        {
            m_num_vertices = 0;
        }
        public void AddVertex(double x, double y, ShapePath.FlagsAndCommand CommandAndFlags)
        {
            AllocIfRequired(m_num_vertices);

            m_coord_xy[m_num_vertices << 1] = x;
            m_coord_xy[(m_num_vertices << 1) + 1] = y;
            m_CommandAndFlags[m_num_vertices] = CommandAndFlags;

            m_num_vertices++;
        }
        public void ReplaceVertex(int index, double x, double y)
        {
            m_coord_xy[index << 1] = x;
            m_coord_xy[(index << 1) + 1] = y;
        }
        public void ReplaceVertex(int index, double x, double y, ShapePath.FlagsAndCommand CommandAndFlags)
        {

            m_coord_xy[index << 1] = x;
            m_coord_xy[(index << 1) + 1] = y;
            m_CommandAndFlags[index] = CommandAndFlags;
        }
        public void ReplaceCommand(int index, ShapePath.FlagsAndCommand CommandAndFlags)
        {
            m_CommandAndFlags[index] = CommandAndFlags;
        }
        public void SwapVertices(int v1, int v2)
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

        public ShapePath.FlagsAndCommand GetLastCommand()
        {
            if (m_num_vertices != 0)
            {
                return GetCommand(m_num_vertices - 1);
            }

            return ShapePath.FlagsAndCommand.CommandStop;
        }

        public ShapePath.FlagsAndCommand GetLastVertex(out double x, out double y)
        {
            if (m_num_vertices != 0)
            {
                return GetVertex((int)(m_num_vertices - 1), out x, out y);
            }

            x = 0;
            y = 0;
            return ShapePath.FlagsAndCommand.CommandStop;
        }

        public ShapePath.FlagsAndCommand GetPrevVertex(out double x, out double y)
        {

            if (m_num_vertices > 1)
            {
                return GetVertex((int)(m_num_vertices - 2), out x, out y);
            }
            x = 0;
            y = 0;
            return ShapePath.FlagsAndCommand.CommandStop;
        }
        public double GetLastX()
        {
            if (m_num_vertices > 0)
            {
                int index = (int)(m_num_vertices - 1);
                return m_coord_xy[index << 1];
            }
            return new double();
        }
        public double GetLastY()
        {
            if (m_num_vertices > 0)
            {
                int index = (int)(m_num_vertices - 1);
                return m_coord_xy[(index << 1) + 1];
            }
            return 0;
        }
        public ShapePath.FlagsAndCommand GetVertex(int index, out double x, out double y)
        {
            var i = index << 1;
            x = m_coord_xy[i];
            y = m_coord_xy[i + 1];
            return m_CommandAndFlags[index];
        }
        public void GetVertexXY(int index, out double x, out double y)
        {
            var i = index << 1;
            x = m_coord_xy[i];
            y = m_coord_xy[i + 1];
        }
        public ShapePath.FlagsAndCommand GetCommand(int index)
        {
            return m_CommandAndFlags[index];
        }

        void AllocIfRequired(int indexToAdd)
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