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
using System.Collections.Generic;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg 
{
    public struct VertexData
    {
        public ShapePath.FlagsAndCommand command;
        public Vector2 position;
        public VertexData(ShapePath.FlagsAndCommand command, Vector2 position)
        {
            this.command = command;
            this.position = position;
        }
        public VertexData(ShapePath.FlagsAndCommand command, double x, double y)
        {
            this.command = command;
            this.position = new Vector2(x, y);
        }
        public bool IsMoveTo
        {
            get { return command == ShapePath.FlagsAndCommand.CommandMoveTo; }
        }

        public bool IsLineTo
        {
            get { return command == ShapePath.FlagsAndCommand.CommandLineTo; }
        }
    }

    public interface IVertexSource
    {
        IEnumerable<VertexData> GetVertexIter();
        void RewindZero();
        ShapePath.FlagsAndCommand GetNextVertex(out double x, out double y);

        bool IsDynamicVertexGen { get; }


        VertexStorage MakeVxs();
        SinglePath MakeSinglePath();
    }

    public interface IVertexProducer
    {
        VertexStorage MakeVxs();
        SinglePath MakeSinglePath();
    }

}
