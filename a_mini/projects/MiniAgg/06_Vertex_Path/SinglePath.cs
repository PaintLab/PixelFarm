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
            int j = currentVertex.Count;
            currentIterIndex = 0;
            for (int i = 0; i < j; ++i)
            {
                currentIterIndex++;
                double x, y;
                ShapePath.FlagsAndCommand cmd;
                cmd = currentVertex.GetVertex(i, out x, out y);
                if (cmd == ShapePath.FlagsAndCommand.CommandStop)
                {
                    yield return new VertexData(cmd, new Vector2(x, y));
                    break;
                }
                else
                {
                    yield return new VertexData(cmd, new Vector2(x, y));
                }
            }

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