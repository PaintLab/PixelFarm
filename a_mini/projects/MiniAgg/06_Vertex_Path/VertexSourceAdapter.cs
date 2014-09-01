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

    //------------------------------------------------------conv_adaptor_vcgen
    public abstract class VertexSourceAdapter : IVertexSource
    {
        //null pattern
        struct NullMarkers : IMarkers
        {
            public void RemoveAll() { }
            public void AddVertex(double x, double y, ShapePath.FlagsAndCommand unknown) { }
        }

        readonly IVertexSource vtxsrc;

        IGenerator generator;
        IMarkers markers;
        Status m_status;
        ShapePath.FlagsAndCommand m_last_cmd;
        double m_start_x;
        double m_start_y;
        enum Status
        {
            Initial,
            Accumulate,
            Generate
        }

        public bool IsDynamicVertexGen { get { return this.vtxsrc.IsDynamicVertexGen; } }



        public VertexSourceAdapter(IVertexSource vertexSource, IGenerator generator)
        {
            markers = new NullMarkers();

            this.vtxsrc = vertexSource;
            this.generator = generator;
            m_status = Status.Initial;
        }
         

        protected IGenerator GetGenerator() { return generator; }

        

        public IEnumerable<VertexData> GetVertexIter()
        {
            this.RewindZero();
            ShapePath.FlagsAndCommand command = ShapePath.FlagsAndCommand.CommandStop;
            do
            {
                double x;
                double y;
                command = GetNextVertex(out x, out y);

                yield return new VertexData(command, new Vector2(x, y));

            } while (command != ShapePath.FlagsAndCommand.CommandStop);
        }

        public void Rewind(int path_id)
        {
            vtxsrc.Rewind(path_id);
            m_status = Status.Initial;
        }
        public void RewindZero()
        {
            vtxsrc.RewindZero();
            m_status = Status.Initial;
        }
        public ShapePath.FlagsAndCommand GetNextVertex(out double x, out double y)
        {
            x = 0;
            y = 0;
            ShapePath.FlagsAndCommand command = ShapePath.FlagsAndCommand.CommandStop;
            bool done = false;
            while (!done)
            {
                switch (m_status)
                {
                    case Status.Initial:
                        markers.RemoveAll();
                        m_last_cmd = vtxsrc.GetNextVertex(out m_start_x, out m_start_y);
                        m_status = Status.Accumulate;
                        goto case Status.Accumulate;

                    case Status.Accumulate:
                        if (ShapePath.IsStop(m_last_cmd))
                        {
                            return ShapePath.FlagsAndCommand.CommandStop;
                        }

                        generator.RemoveAll();
                        generator.AddVertex(m_start_x, m_start_y, ShapePath.FlagsAndCommand.CommandMoveTo);
                        markers.AddVertex(m_start_x, m_start_y, ShapePath.FlagsAndCommand.CommandMoveTo);

                        for (; ; )
                        {
                            command = vtxsrc.GetNextVertex(out x, out y);
                            //DebugFile.Print("x=" + x.ToString() + " y=" + y.ToString() + "\n");
                            if (ShapePath.IsVertextCommand(command))
                            {
                                m_last_cmd = command;
                                if (ShapePath.IsMoveTo(command))
                                {
                                    m_start_x = x;
                                    m_start_y = y;
                                    break;
                                }
                                generator.AddVertex(x, y, command);
                                markers.AddVertex(x, y, ShapePath.FlagsAndCommand.CommandLineTo);
                            }
                            else
                            {
                                if (ShapePath.IsStop(command))
                                {
                                    m_last_cmd = ShapePath.FlagsAndCommand.CommandStop;
                                    break;
                                }
                                if (ShapePath.IsEndPoly(command))
                                {
                                    generator.AddVertex(x, y, command);
                                    break;
                                }
                            }
                        }
                        generator.RewindZero();
                        m_status = Status.Generate;
                        goto case Status.Generate;

                    case Status.Generate:

                        command = generator.GetNextVertex(ref x, ref y);
                        //DebugFile.Print("x=" + x.ToString() + " y=" + y.ToString() + "\n");
                        if (ShapePath.IsStop(command))
                        {
                            m_status = Status.Accumulate;
                            break;
                        }
                        done = true;
                        break;
                }
            }
            return command;
        }
    }
}