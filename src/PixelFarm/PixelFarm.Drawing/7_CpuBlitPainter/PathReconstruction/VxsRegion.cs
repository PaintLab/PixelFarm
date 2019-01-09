//MIT, 2019-present, WinterDev

using System.Collections.Generic;
using ClipperLib;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.CpuBlit.PathReconstruction
{
    public class VxsRegion : PixelFarm.Drawing.Region
    {
        bool _isSimpleRect;
        VertexStore _vxs;//vector path for the data
        List<VertexStore> _subVxsList;
        bool _evalRectBounds;

        /// <summary>
        /// create simple Rect region
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public VxsRegion(float left, float top, float width, float height)
        {
            _isSimpleRect = true;
            using (VxsTemp.Borrow(out VertexStore v1))
            using (VectorToolBox.Borrow(out SimpleRect rect))
            {
                rect.SetRectFromLTWH(left, top, width, height);
                rect.MakeVxs(v1);
                _vxs = v1.CreateTrim();
            }
        }
        /// <summary>
        /// create a region from vxs (may be simple rect vxs or complex vxs)
        /// </summary>
        /// <param name="vxs"></param>
        public VxsRegion(VertexStore vxs)
        {
            //COPY
            _vxs = vxs.CreateTrim();//we don't store outside data
        }
        private VxsRegion(List<VertexStore> subVxsList)
        {
            _subVxsList = subVxsList;
        }
        public override object InnerRegion => null;
        public override void Dispose()
        {
            if (_vxs != null)
            {
                _vxs = null;
            }
        }

        public VxsRegion NewXor(VxsRegion another)
        {
            List<VertexStore> subVxsList = new List<VertexStore>();
            VxsClipper.CombinePaths(this._vxs, another._vxs, VxsClipperType.Xor, true, subVxsList);
            if (subVxsList.Count > 1)
            {
                //?
                return new VxsRegion(subVxsList);
            }
            else if (subVxsList.Count == 1)
            {
                return new VxsRegion(subVxsList[0]);
            }
            else
            {
                //???
                throw new System.NotSupportedException();
            }
        }
        /// <summary>
        /// CREATE new region from this combine with another
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public VxsRegion NewUnion(VxsRegion another)
        {
            List<VertexStore> subVxsList = new List<VertexStore>();
            VxsClipper.CombinePaths(this._vxs, another._vxs, VxsClipperType.Union, true, subVxsList);
            if (subVxsList.Count > 1)
            {
                //?
                return new VxsRegion(subVxsList);
            }
            else if (subVxsList.Count == 1)
            {
                return new VxsRegion(subVxsList[0]);
            }
            else
            {
                //???
                throw new System.NotSupportedException();
            }
        }

    }


}