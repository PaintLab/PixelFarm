 //BSD 2014, WinterDev
using System.Collections.Generic;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg.VertexSource
{
    //----------------------------------------
    public class VertexSourceReader : IVertexSource
    {

        VertexStorage currentVertex;
        internal VertexSourceReader()
        {
        }
        public void RewindZero()
        {

        }
        public void Rewind(int pathId)
        {

        }

        public ShapePath.FlagsAndCommand GetNextVertex(out double x, out double y)
        {
            throw new System.NotSupportedException();
        }
        public IEnumerable<VertexData> GetVertexIter()
        {
            throw new System.NotSupportedException();
        }
        public bool IsDynamicVertexGen
        {
            get
            {
                return false;
            }
        }

    }
}