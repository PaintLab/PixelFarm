//MIT, 2016-present, WinterDev

using System.Collections.Generic;
using ClipperLib;
using PixelFarm.Drawing;
namespace PixelFarm.CpuBlit.VertexProcessing
{
    public enum VxsClipperType : byte
    {
        InterSect = ClipType.ctIntersection,
        Union = ClipType.ctUnion,
        Difference = ClipType.ctDifference,
        Xor = ClipType.ctXor,
    }

    public class VxsClipper
    {

        List<IntPolygon> _aPolys = new List<IntPolygon>();
        List<IntPolygon> _bPolys = new List<IntPolygon>();
        List<IntPolygon> _intersectedPolys = new List<IntPolygon>();
        Clipper _clipper = new Clipper();



        public static void CombinePaths(
            VertexStore a,
            VertexStore b,
            VxsClipperType vxsClipType,
            bool separateIntoSmallSubPaths,
            List<VertexStore> results)
        {

            using (VectorToolBox.Borrow(out VxsClipper clipper))
            {
                clipper.CombinePathsInternal(a, b, vxsClipType, separateIntoSmallSubPaths, results);
            }
        }

        internal VxsClipper() { }
        internal void Reset()
        {
            _aPolys.Clear();
            _bPolys.Clear();
            _intersectedPolys.Clear();
            _clipper.Clear();

        }
        //
        void CombinePathsInternal(
           VertexStore a,
           VertexStore b,
           VxsClipperType vxsClipType,
           bool separateIntoSmallSubPaths,
           List<VertexStore> resultList)
        {

            //prepare instance
            //reset all used fields

            ClipType clipType = (ClipType)vxsClipType;
            CreatePolygons(a, _aPolys);
            CreatePolygons(b, _bPolys);

            _clipper.AddPaths(_aPolys, PolyType.ptSubject, true);
            _clipper.AddPaths(_bPolys, PolyType.ptClip, true);
            _clipper.Execute(clipType, _intersectedPolys);

            if (separateIntoSmallSubPaths)
            {
                foreach (List<IntPoint> polygon in _intersectedPolys)
                {
                    int j = polygon.Count;
                    if (j > 0)
                    {
                        //first one
                        IntPoint point = polygon[0];
                        using (VxsTemp.Borrow(out VertexStore v1))
                        using (VectorToolBox.Borrow(v1, out PathWriter pw))
                        {
                            pw.MoveTo(point.X / 1000.0, point.Y / 1000.0);
                            //next others ...
                            if (j > 1)
                            {
                                for (int i = 1; i < j; ++i)
                                {
                                    point = polygon[i];
                                    pw.LineTo(point.X / 1000.0, point.Y / 1000.0);
                                }
                            }

                            pw.CloseFigure();
                            resultList.Add(v1.CreateTrim()); //copy
                            pw.Clear();
                        }
                    }
                }
            }
            else
            {
                using (VxsTemp.Borrow(out VertexStore v1))
                using (VectorToolBox.Borrow(v1, out PathWriter pw))
                {
                    foreach (List<IntPoint> polygon in _intersectedPolys)
                    {
                        int j = polygon.Count;
                        if (j > 0)
                        {
                            //first one
                            IntPoint point = polygon[0];
                            pw.MoveTo(point.X / 1000.0, point.Y / 1000.0);
                            //next others ...
                            if (j > 1)
                            {
                                for (int i = 1; i < j; ++i)
                                {
                                    point = polygon[i];
                                    pw.LineTo(point.X / 1000.0, point.Y / 1000.0);
                                }
                            }
                            pw.CloseFigure();
                        }
                    }
                    pw.Stop();
                    resultList.Add(v1.CreateTrim());
                }
            }
        }


        static void CreatePolygons(VertexStore a, List<IntPolygon> allPolys)
        {

            IntPolygon currentPoly = null;
            VertexData last = new VertexData();
            VertexData first = new VertexData();
            bool addedFirst = false;
            double x, y;

            int index = 0;
            VertexCmd cmd;
            while ((cmd = a.GetVertex(index++, out x, out y)) != VertexCmd.NoMore)
            {
                if (cmd == VertexCmd.LineTo)
                {
                    if (currentPoly == null)
                    {
                        currentPoly = new IntPolygon();
                        allPolys.Add(currentPoly);
                    }
                    //
                    if (!addedFirst)
                    {
                        currentPoly.Add(new IntPoint((long)(last.x * 1000), (long)(last.y * 1000)));
                        addedFirst = true;
                        first = last;
                    }
                    currentPoly.Add(new IntPoint((long)(x * 1000), (long)(y * 1000)));
                    last = new VertexData(cmd, x, y);
                }
                else
                {
                    addedFirst = false;
                    currentPoly = new IntPolygon();
                    allPolys.Add(currentPoly);
                    if (cmd == VertexCmd.MoveTo)
                    {
                        last = new VertexData(cmd, x, y);
                    }
                    else
                    {
                        last = first;
                    }
                }
            }


        }
    }


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
