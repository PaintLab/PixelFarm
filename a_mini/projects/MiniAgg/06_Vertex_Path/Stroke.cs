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
// conv_stroke
//
//----------------------------------------------------------------------------
using System.Collections.Generic;
namespace MatterHackers.Agg.VertexSource
{
    public sealed class Stroke// : VertexSourceAdapter
    {
        Status m_status;
        ShapePath.FlagsAndCommand m_last_cmd;
        double m_start_x;
        double m_start_y;


        StrokeGenerator strokeGen;
        IVertexSource vertexSource;
        public Stroke(IVertexSource vertexSource, double inWidth = 1)
        //: base(vertexSource, new StrokeGenerator())
        {
            this.vertexSource = vertexSource;
            this.strokeGen = new StrokeGenerator();
            this.Width = inWidth;
        }
        StrokeGenerator GetGenerator()
        {
            return this.strokeGen;
        }
        public LineCap LineCap
        {
            get { return this.GetGenerator().LineCap; }
            set { this.GetGenerator().LineCap = value; }
        }
        public LineJoin LineJoin
        {
            get { return this.GetGenerator().LineJoin; }
            set { this.GetGenerator().LineJoin = value; }
        }
        public InnerJoin InnerJoin
        {
            get { return this.GetGenerator().InnerJoin; }
            set { this.GetGenerator().InnerJoin = value; }
        }
        public double MiterLimit
        {
            get { return this.GetGenerator().MiterLimit; }
            set { this.GetGenerator().MiterLimit = value; }
        }
        public double InnerMiterLimit
        {
            get { return this.GetGenerator().InnerMiterLimit; }
            set { this.GetGenerator().InnerMiterLimit = value; }
        }
        public double Width
        {
            get { return this.GetGenerator().Width; }
            set { this.GetGenerator().Width = value; }
        }

        public void SetMiterLimitTheta(double t)
        {
            this.GetGenerator().SetMiterLimitTheta(t);
        }
        public double ApproximateScale
        {
            get { return this.GetGenerator().ApproximateScale; }
            set { this.GetGenerator().ApproximateScale = value; }
        }
        public double Shorten
        {
            get { return this.GetGenerator().Shorten; }
            set { this.GetGenerator().Shorten = value; }
        }
        public VertexStorage MakeVxs(VertexStorage vxs)
        {
            List<VertexData> list = new List<VertexData>();
            foreach (VertexData vx in this.GetVertexIter())
            {
                list.Add(vx);
            }
            return new VertexStorage(list);
        }
        ////---------------------------------------------------------
        //IEnumerable<VertexData> GetVertexIter()
        //{
        //    this.RewindZero();
        //    ShapePath.FlagsAndCommand command = ShapePath.FlagsAndCommand.CommandStop;
        //    do
        //    {
        //        double x;
        //        double y;
        //        command = GetNextVertex3(out x, out y);

        //        yield return new VertexData(command, x, y);

        //    } while (command != ShapePath.FlagsAndCommand.CommandStop);
        //}

        //void RewindZero()
        //{
        //    vertexSource.RewindZero();
        //    m_status = Status.Initial;
        //}
        //public ShapePath.FlagsAndCommand GetNextVertex3(out double x, out double y)
        //{
        //    x = 0;
        //    y = 0;
        //    ShapePath.FlagsAndCommand command = ShapePath.FlagsAndCommand.CommandStop;
        //    bool done = false;
        //    while (!done)
        //    {
        //        switch (m_status)
        //        {
        //            case Status.Initial:
        //                //markers.RemoveAll();
        //                m_last_cmd = vtxsrc.GetNextVertex(out m_start_x, out m_start_y);
        //                m_status = Status.Accumulate;
        //                goto case Status.Accumulate;

        //            case Status.Accumulate:
        //                if (ShapePath.IsStop(m_last_cmd))
        //                {
        //                    return ShapePath.FlagsAndCommand.CommandStop;
        //                }

        //                generator.RemoveAll();
        //                generator.AddVertex(m_start_x, m_start_y, ShapePath.FlagsAndCommand.CommandMoveTo);
        //                //markers.AddVertex(m_start_x, m_start_y, ShapePath.FlagsAndCommand.CommandMoveTo);

        //                bool runloop = true;
        //                while (runloop)
        //                {
        //                    command = vtxsrc.GetNextVertex(out x, out y);
        //                    switch (ShapePath.FlagsAndCommand.CommandsMask & command)
        //                    {
        //                        case ShapePath.FlagsAndCommand.CommandStop:
        //                            {
        //                                m_last_cmd = ShapePath.FlagsAndCommand.CommandStop;
        //                                runloop = false;
        //                            } break;
        //                        case ShapePath.FlagsAndCommand.CommandEndPoly:
        //                            {
        //                                generator.AddVertex(x, y, command);
        //                                runloop = false;
        //                            } break;
        //                        case ShapePath.FlagsAndCommand.CommandLineTo:
        //                        case ShapePath.FlagsAndCommand.CommandCurve3:
        //                        case ShapePath.FlagsAndCommand.CommandCurve4:
        //                            {
        //                                m_last_cmd = command;
        //                                generator.AddVertex(x, y, command);
        //                                //markers.AddVertex(x, y, ShapePath.FlagsAndCommand.CommandLineTo);

        //                            } break;
        //                        case ShapePath.FlagsAndCommand.CommandMoveTo:
        //                            {
        //                                m_last_cmd = command;
        //                                m_start_x = x;
        //                                m_start_y = y;
        //                                runloop = false;
        //                            } break;
        //                    }
        //                    //if (ShapePath.IsVertextCommand(command))
        //                    //{
        //                    //    m_last_cmd = command;
        //                    //    if (ShapePath.IsMoveTo(command))
        //                    //    {
        //                    //        m_start_x = x;
        //                    //        m_start_y = y;
        //                    //        break;
        //                    //    }
        //                    //    generator.AddVertex(x, y, command);
        //                    //    markers.AddVertex(x, y, ShapePath.FlagsAndCommand.CommandLineTo);
        //                    //}
        //                    //else
        //                    //{
        //                    //    if (ShapePath.IsStop(command))
        //                    //    {
        //                    //        m_last_cmd = ShapePath.FlagsAndCommand.CommandStop;
        //                    //        break;
        //                    //    }
        //                    //    if (ShapePath.IsEndPoly(command))
        //                    //    {
        //                    //        generator.AddVertex(x, y, command);
        //                    //        break;
        //                    //    }
        //                    //}
        //                }
        //                generator.RewindZero();
        //                m_status = Status.Generate;
        //                goto case Status.Generate;

        //            case Status.Generate:

        //                command = generator.GetNextVertex(ref x, ref y);
        //                //DebugFile.Print("x=" + x.ToString() + " y=" + y.ToString() + "\n");
        //                if (ShapePath.IsStop(command))
        //                {
        //                    m_status = Status.Accumulate;
        //                    break;
        //                }
        //                done = true;
        //                break;
        //        }
        //    }
        //    return command;
        //}
        ////---------------------------------------------------------
        //---------------------------------------------------------
        IEnumerable<VertexData> GetVertexIter()
        {
            this.RewindZero();
            ShapePath.FlagsAndCommand command = ShapePath.FlagsAndCommand.CommandStop;
            do
            {
                double x;
                double y;
                command = GetNextVertex3(out x, out y);

                yield return new VertexData(command, x, y);

            } while (command != ShapePath.FlagsAndCommand.CommandStop);
        }

        void RewindZero()
        {
            //vtxsrc.RewindZero();
            this.vertexSource.RewindZero();
            m_status = Status.Initial;
        }
        ShapePath.FlagsAndCommand GetNextVertex3(out double x, out double y)
        {
            var generator = this.GetGenerator();
            var vtxsrc = this.vertexSource;
            x = 0;
            y = 0;
            ShapePath.FlagsAndCommand command = ShapePath.FlagsAndCommand.CommandStop;
            bool done = false;
            while (!done)
            {
                switch (m_status)
                {
                    case Status.Initial:
                        //markers.RemoveAll();
                        m_last_cmd = this.vertexSource.GetNextVertex(out m_start_x, out m_start_y);
                        m_status = Status.Accumulate;
                        goto case Status.Accumulate;

                    case Status.Accumulate:
                        if (ShapePath.IsStop(m_last_cmd))
                        {
                            return ShapePath.FlagsAndCommand.CommandStop;
                        }

                        generator.RemoveAll();
                        generator.AddVertex(m_start_x, m_start_y, ShapePath.FlagsAndCommand.CommandMoveTo);
                        //markers.AddVertex(m_start_x, m_start_y, ShapePath.FlagsAndCommand.CommandMoveTo);

                        bool runloop = true;
                        while (runloop)
                        {
                            command = vtxsrc.GetNextVertex(out x, out y);
                            switch (ShapePath.FlagsAndCommand.CommandsMask & command)
                            {
                                case ShapePath.FlagsAndCommand.CommandStop:
                                    {
                                        m_last_cmd = ShapePath.FlagsAndCommand.CommandStop;
                                        runloop = false;
                                    } break;
                                case ShapePath.FlagsAndCommand.CommandEndPoly:
                                    {
                                        generator.AddVertex(x, y, command);
                                        runloop = false;
                                    } break;
                                case ShapePath.FlagsAndCommand.CommandLineTo:
                                case ShapePath.FlagsAndCommand.CommandCurve3:
                                case ShapePath.FlagsAndCommand.CommandCurve4:
                                    {
                                        m_last_cmd = command;
                                        generator.AddVertex(x, y, command);
                                        //markers.AddVertex(x, y, ShapePath.FlagsAndCommand.CommandLineTo);

                                    } break;
                                case ShapePath.FlagsAndCommand.CommandMoveTo:
                                    {
                                        m_last_cmd = command;
                                        m_start_x = x;
                                        m_start_y = y;
                                        runloop = false;
                                    } break;
                            }
                            //if (ShapePath.IsVertextCommand(command))
                            //{
                            //    m_last_cmd = command;
                            //    if (ShapePath.IsMoveTo(command))
                            //    {
                            //        m_start_x = x;
                            //        m_start_y = y;
                            //        break;
                            //    }
                            //    generator.AddVertex(x, y, command);
                            //    markers.AddVertex(x, y, ShapePath.FlagsAndCommand.CommandLineTo);
                            //}
                            //else
                            //{
                            //    if (ShapePath.IsStop(command))
                            //    {
                            //        m_last_cmd = ShapePath.FlagsAndCommand.CommandStop;
                            //        break;
                            //    }
                            //    if (ShapePath.IsEndPoly(command))
                            //    {
                            //        generator.AddVertex(x, y, command);
                            //        break;
                            //    }
                            //}
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
        //---------------------------------------------------------
        enum Status
        {
            Initial,
            Accumulate,
            Generate
        }
    }
}