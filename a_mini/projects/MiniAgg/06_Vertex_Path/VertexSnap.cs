//BSD 2014, WinterDev
using System.Collections.Generic;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg
{
    //----------------------------------------
    public struct VertexSnap
    {
        int startAt;      
        int currentIterIndex;

        VertexStorage vxs;
        public VertexSnap(VertexStorage vxs)
        {
            this.vxs = vxs;
            this.startAt = 0;
            this.currentIterIndex = startAt;
        }
        public VertexSnap(VertexStorage vxs, int startAt)
        {
            this.vxs = vxs;
            this.startAt = startAt;
            this.currentIterIndex = startAt;
        }
        public void RewindZero()
        {
            this.currentIterIndex = startAt;
        }
        public ShapePath.FlagsAndCommand GetNextVertex(out double x, out double y)
        {
            var cmd = vxs.GetVertex(currentIterIndex, out x, out y);
            currentIterIndex++;
            return cmd;
        }

        public VertexStorage GetInternalVxs()
        {
            return this.vxs;
        }
        public int StartAt
        {
            get { return this.startAt; }
        }
        public bool VxsHasMoreThanOnePart
        {
            get { return this.vxs.HasMoreThanOnePart; }
        }
        public IEnumerable<VertexData> GetVertexIter()
        {
            int j = vxs.Count;
            currentIterIndex = 0;
            for (int i = 0; i < j; ++i)
            {
                currentIterIndex++;
                double x, y;
                ShapePath.FlagsAndCommand cmd;
                cmd = vxs.GetVertex(i, out x, out y);
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


    }
}