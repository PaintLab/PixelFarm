//BSD 2014, WinterDev
using System.Collections.Generic;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg.VertexSource
{
    //----------------------------------------
    public struct SinglePath : IVertexSource
    {
        int startAt;
        VertexStorage currentVertex;
        int currentIterIndex;
        public SinglePath(IVertexSource vertSource, int startAt)
        {
            this.currentVertex = null;
            this.startAt = startAt;
            this.currentIterIndex = startAt;
        }

        public SinglePath(VertexStorage currentVertex, int startAt)
        {
            this.currentVertex = currentVertex;
            this.startAt = startAt;
            this.currentIterIndex = startAt;
        }
         
        public void RewindZero()
        {
            this.currentIterIndex = startAt;
        }
        public void Rewind(int pathId)
        {
        }
        public ShapePath.FlagsAndCommand GetNextVertex(out double x, out double y)
        {
            var cmd = currentVertex.GetVertex(currentIterIndex, out x, out y);
            currentIterIndex++;
            return cmd;
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