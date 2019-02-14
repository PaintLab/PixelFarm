//MIT, 2016, Viktor Chlumsky, Multi-channel signed distance field generator, from https://github.com/Chlumsky/msdfgen
//MIT, 2017-present, WinterDev (C# port)
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace ExtMsdfGen
{
    class MyCustomPixelBlender : PixelFarm.CpuBlit.PixelProcessing.CustomPixelBlender
    {
        const int WHITE = (255 << 24) | (255 << 16) | (255 << 8) | 255;
        const int BLACK = (255 << 24);
        const int GREEN = (255 << 24) | (255 << 8);
        const int RED = (255 << 24) | (255 << 16);


        struct OverlapPart
        {
            readonly int _edgeA;
            readonly int _edgeB;
            readonly AreaKind _areaKindA;
            readonly AreaKind _areaKindB;
            public OverlapPart(int edgeA, AreaKind areaKindA, int edgeB, AreaKind areaKindB)
            {
                if (edgeB < edgeA)
                {
                    //swap
                    _edgeA = edgeB;
                    _edgeB = edgeA;
                    _areaKindA = areaKindB;
                    _areaKindB = areaKindA;
                }
                else
                {
                    _edgeA = edgeA;
                    _edgeB = edgeB;
                    _areaKindA = areaKindA;
                    _areaKindB = areaKindB;
                }

            }
#if DEBUG
            public override string ToString()
            {
                return _edgeA + "," + _edgeB;
            }

#endif
        }

        Dictionary<OverlapPart, int> _overlapParts = new Dictionary<OverlapPart, int>();
        internal List<List<int>> _overlapList = new List<List<int>>();


        public MyCustomPixelBlender()
        {
        }
        public void ClearOverlapList()
        {
            _overlapParts.Clear();
            _overlapList.Clear();
        }

        public int RegisterOverlapOuter(int corner1, int corner2, AreaKind areaKind)
        {

            OverlapPart overlapPart = new OverlapPart(corner1, areaKind, corner2, areaKind);
            if (!_overlapParts.TryGetValue(overlapPart, out int found))
            {
                int newPartNo = _overlapList.Count;
                _overlapParts.Add(overlapPart, newPartNo);
                //
                List<int> cornerList = new List<int>();
                _overlapList.Add(cornerList);
                cornerList.Add(corner1);
                cornerList.Add(corner2);
            }
            return found;
        }
        protected override unsafe void BlendPixel32Internal(int* dstPtr, Color srcColor)
        {
            CustomBlendPixel32(dstPtr, srcColor);
        }
        protected override unsafe void BlendPixel32Internal(int* dstPtr, Color srcColor, int coverageValue)
        {
            CustomBlendPixel32(dstPtr, srcColor);
        }
        unsafe void CustomBlendPixel32(int* dstPtr, Color srcColor)
        {
            int existingColor = *dstPtr;
            if (existingColor != WHITE && existingColor != BLACK)
            {
                int existing_R = (existingColor >> CO.R_SHIFT) & 0xFF;
                int existing_G = (existingColor >> CO.G_SHIFT) & 0xFF;
                int existing_B = (existingColor >> CO.B_SHIFT) & 0xFF;
                //overlap pixel found!
                //
                int existingCorner = (existing_R - 50) / 2;
                //check if exisingEdge is already overlap or not?
                int newCorner = (srcColor.R - 50) / 2;
                int newCorner_G = srcColor.G;


                //create new overlap list 
                if (existing_R == 255 && existing_G == 0 && existing_B == 0)
                {
                    //all red 
                    //then skip
                    return;
                }
                if (existingCorner == newCorner) return;
                // 
                if (srcColor.G == 75 || srcColor.G == 70)
                {
                    //new color is overlap color 
                    if (existing_G == 75 || existing_G == 70)
                    {
                        List<int> registerList = _overlapList[newCorner];
                        _overlapList[existingCorner].AddRange(registerList);
                    }
                    else
                    {
                        List<int> registerList = _overlapList[newCorner];
                        registerList.Add(existingCorner);
                        *dstPtr = EdgeBmpLut.EncodeToColor(newCorner, (existing_G == 0) ? AreaKind.OverlapInside : AreaKind.OverlapOutside).ToARGB();
                    }
                }
                else
                {
                    if (existing_G == 75 || existing_G == 70)
                    {
                        _overlapList[existingCorner].Add(newCorner);
                    }
                    else
                    {
                        ////create new overlap part
                        OverlapPart overlapPart = new OverlapPart(
                            existingCorner, (existing_G == 0) ? AreaKind.OverlapInside : AreaKind.OverlapOutside,
                            newCorner, (newCorner_G == 0) ? AreaKind.OverlapInside : AreaKind.OverlapOutside);

                        if (!_overlapParts.TryGetValue(overlapPart, out int found))
                        {
                            int newPartNo = _overlapList.Count;
                            _overlapParts.Add(overlapPart, newPartNo);
                            //
                            List<int> cornerList = new List<int>();
                            _overlapList.Add(cornerList);
                            cornerList.Add(existingCorner);
                            cornerList.Add(newCorner);
                            //set new color
                            *dstPtr = EdgeBmpLut.EncodeToColor(newPartNo, (existing_G == 0) ? AreaKind.OverlapInside : AreaKind.OverlapOutside).ToARGB();
                        }
                        else
                        {
                            //set new color
                            *dstPtr = EdgeBmpLut.EncodeToColor(found, (existing_G == 0) ? AreaKind.OverlapInside : AreaKind.OverlapOutside).ToARGB();
                        }
                    }
                }


            }
            else
            {
                *dstPtr = srcColor.ToARGB();
            }
        }
    }

    /// <summary>
    /// edge bitmap lookup table
    /// </summary>
    public class EdgeBmpLut
    {
        int _w;
        int _h;
        int[] _buffer;
        List<ContourCorner> _corners;
        List<EdgeSegment> _flattenEdges;
        List<EdgeSegment[]> _overlappedEdgeList;
        internal EdgeBmpLut(List<ContourCorner> corners, List<EdgeSegment> flattenEdges, List<int> segOfNextContours, List<int> cornerOfNextContours)
        {
            //move first to last 
            int startAt = 0;
            for (int i = 0; i < segOfNextContours.Count; ++i)
            {
                int nextStartAt = segOfNextContours[i];
                //
                EdgeSegment firstSegment = flattenEdges[startAt];

                flattenEdges.RemoveAt(startAt);
                if (i == segOfNextContours.Count - 1)
                {
                    flattenEdges.Add(firstSegment);
                }
                else
                {
                    flattenEdges.Insert(nextStartAt - 1, firstSegment);
                }
                startAt = nextStartAt;
            }

            _corners = corners;
            _flattenEdges = flattenEdges;
            EdgeOfNextContours = segOfNextContours;
            CornerOfNextContours = cornerOfNextContours;

            ConnectExtendedPoints(corners, cornerOfNextContours); //after arrange 
        }
        internal void SetOverlappedList(List<int[]> overlappedList)
        {
            int m = overlappedList.Count;
            _overlappedEdgeList = new List<EdgeSegment[]>(m);
            for (int i = 0; i < m; ++i)
            {
                int[] arr1 = overlappedList[i];
                EdgeSegment[] corners = new EdgeSegment[arr1.Length];//overlapping corner region
                for (int a = 0; a < arr1.Length; ++a)
                {
                    corners[a] = _corners[arr1[a]].CenterSegment;
                }
                _overlappedEdgeList.Add(corners);
            }

        }
        static void ConnectExtendedPoints(List<ContourCorner> corners, List<int> cornerOfNextContours)
        {
            //test 2 if each edge has unique color 
            int startAt = 0;
            for (int i = 0; i < cornerOfNextContours.Count; ++i)
            {
                int nextStartAt = cornerOfNextContours[i];
                for (int n = startAt + 1; n < nextStartAt; ++n)
                {
                    ContourCorner.ConnectToEachOther(corners[n - 1], corners[n]);
                }
                //--------------
                {
                    //the last one 
                    ContourCorner.ConnectToEachOther(corners[nextStartAt - 1], corners[startAt]);
                }
                //---------
                startAt = nextStartAt;//***
            }
        }
        //
        public List<int> EdgeOfNextContours { get; private set; }
        public List<int> CornerOfNextContours { get; private set; }

        //
        public void SetBmpBuffer(int w, int h, int[] buffer)
        {
            _w = w;
            _h = h;
            _buffer = buffer;
        }
        public List<ContourCorner> Corners => _corners;

        public int GetPixel(int x, int y) => _buffer[y * _w + x];

        const int WHITE = (255 << 24) | (255 << 16) | (255 << 8) | 255;

        public EdgeStructure GetEdgeStructure(int x, int y)
        {
            //decode 
            int pixel = _buffer[y * _w + x];
            if (pixel == 0)
            {
                return EdgeStructure.Empty;
            }
            else if (pixel == WHITE)
            {
                return EdgeStructure.Empty;
            }
            else
            {
                //G
                int g = (pixel >> PixelFarm.Drawing.CO.G_SHIFT) & 0xFF;
                //find index
                int r = pixel & 0xFF;


                int index = (r - 50) / 2; //encode, decode the color see below....

                if (g == 50)
                {
                    //outside

                    return new EdgeStructure(_corners[index].CenterSegment, AreaKind.Outside);
                }
                else if (g == 25)
                {
                    return new EdgeStructure(_corners[index].CenterSegment, AreaKind.OuterGap);
                }
                else if (g == 70)
                {
                    //return EdgeStructure.Empty;//debug
                    return new EdgeStructure(_overlappedEdgeList[index], AreaKind.OverlapInside);
                }
                else if (g == 75)
                {
                    //this is overlap rgn
                    //return EdgeStructure.Empty;//debug
                    return new EdgeStructure(_overlappedEdgeList[index], AreaKind.OverlapOutside);
                }
                else
                {
                    //inside
                    return new EdgeStructure(_corners[index].CenterSegment, AreaKind.Inside);
                }
            }
        }

        public static PixelFarm.Drawing.Color EncodeToColor(int cornerNo, AreaKind areaKind)
        {
            switch (areaKind)
            {
                default: throw new NotSupportedException();
                case AreaKind.Inside:
                    {
                        int color = (cornerNo * 2) + 50;
                        if (color > 254)
                        {

                        }
                        return new PixelFarm.Drawing.Color((byte)color, 0, (byte)color);
                    }
                case AreaKind.OuterGap:
                    {
                        int color = (cornerNo * 2) + 50;
                        if (color > 254)
                        {

                        }
                        return new PixelFarm.Drawing.Color((byte)color, 25, (byte)color);
                    }
                case AreaKind.Outside:
                    {
                        int color = (cornerNo * 2) + 50;
                        if (color > 254)
                        {

                        }
                        return new PixelFarm.Drawing.Color((byte)color, 50, (byte)color);

                    }
                case AreaKind.OverlapInside:
                    {
                        int color = (cornerNo * 2) + 50;
                        if (color > 254)
                        {

                        }
                        return new PixelFarm.Drawing.Color((byte)color, 70, (byte)color);
                    }
                case AreaKind.OverlapOutside:
                    {
                        int color = (cornerNo * 2) + 50;
                        if (color > 254)
                        {

                        }
                        return new PixelFarm.Drawing.Color((byte)color, 75, (byte)color);
                    }
            }

        }
    }

}