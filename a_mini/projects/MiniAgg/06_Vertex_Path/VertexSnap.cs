//BSD 2014, WinterDev
using System.Collections.Generic;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg
{
    //----------------------------------------
    public struct VertexSnapIter
    {
        int currentIterIndex;
        VertexStorage vxs;
        internal VertexSnapIter(VertexSnap vsnap)
        { 
            this.vxs = vsnap.GetInternalVxs();
            this.currentIterIndex = vsnap.StartAt;
        }
        public ShapePath.FlagsAndCommand GetNextVertex(out double x, out double y)
        {
            return vxs.GetVertex(currentIterIndex++, out x, out y);   
        }        
    }
    
    public struct VertexSnap
    {
        int startAt;
        VertexStorage vxs;
      
        public VertexSnap(VertexStorage vxs)
        {
            this.vxs = vxs;
            this.startAt = 0; 
        }
        public VertexSnap(VertexStorage vxs, int startAt)
        {
            this.vxs = vxs;
            this.startAt = startAt; 
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
        public VertexSnapIter GetVertexSnapIter()
        {
            return new VertexSnapIter(this);
        }
        
        
       

    }
}