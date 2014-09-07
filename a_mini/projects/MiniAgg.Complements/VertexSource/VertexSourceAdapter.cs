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
    public abstract class VertexSourceAdapter
    {
        ////null pattern 
        readonly IVertexSource vtxsrc;
        IVertextGenerator generator;
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
        public VertexSourceAdapter(
            IVertexSource vertexSource,
            IVertextGenerator generator)
        {
            this.vtxsrc = vertexSource;
            this.generator = generator;
            m_status = Status.Initial;
        }

        public bool IsDynamicVertexGen { get { return true; } }

        protected IVertextGenerator GetGenerator() { return generator; }


        public VertexStorage MakeVxs(VertexStorage vxs)        
        {
            throw new NotSupportedException();
            //List<VertexData> list = new List<VertexData>();
            //foreach (VertexData vx in this.GetVertexIter())
            //{
            //    list.Add(vx);
            //}
            //return new VertexStorage(list);
        }
       
    }
}