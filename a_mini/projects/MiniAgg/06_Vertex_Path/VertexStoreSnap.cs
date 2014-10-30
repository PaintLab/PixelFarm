
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
        internal VertexSnapIter(VertexStoreSnap vsnap)
        { 
            
            this.vxs = vsnap.GetInternalVxs();
            this.currentIterIndex = vsnap.StartAt;
            if (vxs == null)
            {
            }
        }
        public ShapePath.FlagsAndCommand GetNextVertex(out double x, out double y)
        {
            return vxs.GetVertex(currentIterIndex++, out x, out y);   
        }        
    }
    
    public struct VertexStoreSnap
    {
        int startAt;
        VertexStorage vxs;
      
        public VertexStoreSnap(VertexStorage vxs)
        {
            this.vxs = vxs;
            this.startAt = 0; 
        }
        public VertexStoreSnap(VertexStorage vxs, int startAt)
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