//MIT, 2016-present, WinterDev

using System.Collections.Generic;

namespace PixelFarm.DrawingGL
{
    class Figure
    {
        //TODO: review here again*** 
        public float[] coordXYs; //this is user provide coord
        //---------
        //system tess ...
        public float[] areaTess;
        float[] smoothBorderTess; //smooth border result
        int _borderTriangleStripCount;
        int _tessAreaVertexCount;
        //---------
        public ushort[] indexListArray;
        float[] tessXYCoords2;
        //---------
        VertexBufferObject _vboArea;
        //---------
        public Figure(float[] coordXYs)
        {
            this.coordXYs = coordXYs;
        }
        public int BorderTriangleStripCount { get { return _borderTriangleStripCount; } }
        public int TessAreaVertexCount { get { return _tessAreaVertexCount; } }

        public bool SupportVertexBuffer
        {
            get;
            set;
        }
        public float[] GetSmoothBorders(SmoothBorderBuilder smoothBorderBuilder)
        {
            if (smoothBorderTess == null)
            {
                return smoothBorderTess =
                    smoothBorderBuilder.BuildSmoothBorders(coordXYs, out _borderTriangleStripCount);
            }
            return smoothBorderTess;
        }

        public float[] GetAreaTess(TessTool tess)
        {
            if (areaTess == null)
            {
                //triangle list                
                return areaTess = tess.TessAsTriVertexArray(coordXYs, null, out this._tessAreaVertexCount);
            }
            return areaTess;
        }
        /// <summary>
        /// vertex buffer of the solid area part
        /// </summary>
        public VertexBufferObject GetAreaTessAsVBO(TessTool tess)
        {
            if (_vboArea == null)
            {
                //tess
                indexListArray = tess.TessAsTriIndexArray(coordXYs, null,
                    out tessXYCoords2, out this._tessAreaVertexCount);
                _vboArea = new VertexBufferObject();
                _vboArea.CreateBuffers(tessXYCoords2, indexListArray, null);
            }
            return _vboArea;
        }

    }

    public struct PartRange
    {
        public readonly int beginVertexAt;
        public readonly int beginElemIndexAt;
        public readonly int elemCount;
        public PartRange(int beginVertexAt, int beginElemIndexAt, int elemCount)
        {
            this.beginVertexAt = beginVertexAt;
            this.beginElemIndexAt = beginElemIndexAt;
            this.elemCount = elemCount;
        }
#if DEBUG
        public override string ToString()
        {
            return beginElemIndexAt + ":" + elemCount;
        }
#endif
    }
    public struct BorderPart
    {
        public int beginAtBorderSetIndex;
        public int count;
    }
    public struct VBOPart
    {
        public readonly VertexBufferObject vbo;
        public readonly PartRange partRange;
        public VBOPart(VertexBufferObject vbo, PartRange partRange)
        {
            this.vbo = vbo;
            this.partRange = partRange;
        }
        public override string ToString()
        {
            return partRange.ToString();
        }
    }



    class SmoothBorderBuilder
    {
        List<float> expandCoords = new List<float>();

        public float[] BuildSmoothBorders(float[] coordXYs, out int borderTriangleStripCount)
        {
            expandCoords.Clear();
            float[] coords = coordXYs;
            int coordCount = coordXYs.Length;
            //from user input coords
            //expand it
            //TODO: review this again***
            int lim = coordCount - 2;
            for (int i = 0; i < lim;)
            {
                CreateSmoothLineSegment(expandCoords, coords[i], coords[i + 1], coords[i + 2], coords[i + 3]);
                i += 2;
            }
            //close coord
            CreateSmoothLineSegment(expandCoords, coords[coordCount - 2], coords[coordCount - 1], coords[0], coords[1]);

            borderTriangleStripCount = coordCount * 2;
            //
            float[] result = expandCoords.ToArray();
            expandCoords.Clear();
            //
            return result;
        }
        public float[] BuildSmoothBorders(float[] coordXYs, int segStartAt, int len, out int borderTriangleStripCount)
        {
            expandCoords.Clear();
            float[] coords = coordXYs;
            //from user input coords
            //expand it
            //TODO: review this again***

            int lim = (segStartAt + len);
            for (int i = segStartAt; i < lim;)
            {
                CreateSmoothLineSegment(expandCoords,/*x0*/ coords[i], /*y0*/coords[i + 1],/*x1*/ coords[i + 2],/*y1*/ coords[i + 3]);
                i += 2;
            }
            //close coord
            int last = segStartAt + len;
            CreateSmoothLineSegment(expandCoords, coords[last], coords[last + 1], coords[segStartAt], coords[segStartAt + 1]);

            borderTriangleStripCount = (len + 2) * 2;
            //
            float[] result = expandCoords.ToArray();
            expandCoords.Clear();
            //
            return result;
        }
        static void CreateSmoothLineSegment(List<float> coords, float x1, float y1, float x2, float y2)
        {
            //create with no line join
            //TODO: implement line join ***
            //we can calculate rad on server-side (GPU), so=> reduce num of vertex

            //from https://blog.mapbox.com/drawing-antialiased-lines-with-opengl-8766f34192dc

            float rad1 = (float)System.Math.Atan2(
                   y2 - y1,  //dy
                   x2 - x1); //dx
            coords.Add(x1); coords.Add(y1); coords.Add(0); coords.Add(rad1); //1 vertex
            coords.Add(x1); coords.Add(y1); coords.Add(1); coords.Add(rad1); //1 vertex
            coords.Add(x2); coords.Add(y2); coords.Add(0); coords.Add(rad1); //1 vertex
            coords.Add(x2); coords.Add(y2); coords.Add(1); coords.Add(rad1); //1 vertex
        }
    }


    public class MultiPartPolygon
    {
        internal List<float[]> expandCoordsList = new List<float[]>();
        internal List<int[]> contourEndPoints = new List<int[]>();


        List<float> _tempCoords = new List<float>();
        List<int> _tempEndPoints = new List<int>();

        public MultiPartPolygon()
        {

        }
        public void AddVertexSnap(PixelFarm.Drawing.VertexStoreSnap vxsSnap)
        {
            //begin new snap vxs
            _tempCoords.Clear();
            _tempEndPoints.Clear();

            var iter = vxsSnap.GetVertexSnapIter();
            double x, y;
            PixelFarm.Agg.VertexCmd cmd;
            int totalXYCount = 0;
            int index = 0;
            float latestMoveToX = 0, latestMoveToY = 0;
            float latestX = 0, latestY = 0;
            while ((cmd = iter.GetNextVertex(out x, out y)) != Agg.VertexCmd.NoMore)
            {
                if (cmd == Agg.VertexCmd.Close || cmd == Agg.VertexCmd.CloseAndEndFigure)
                {
                    index = 0; //reset
                    //temp fix1
                    //some vertex snap may has more than 1 part 
                    _tempEndPoints.Add(totalXYCount - 1);
                    _tempCoords.Add(latestMoveToX);
                    _tempCoords.Add(latestMoveToY);
                    latestX = latestMoveToX = (float)x;
                    latestY = latestMoveToY = (float)y;
                }
                else
                {
                    _tempCoords.Add(latestX = (float)x);
                    _tempCoords.Add(latestY = (float)y);
                    if (index == 0)
                    {
                        latestMoveToX = latestX;
                        latestMoveToY = latestY;
                    }
                    index++;
                }
                totalXYCount += 2;

            }

            if (_tempCoords.Count > 0)
            {
                expandCoordsList.Add(_tempCoords.ToArray());
                contourEndPoints.Add(_tempEndPoints.ToArray());

            }


            _tempCoords.Clear();
            _tempEndPoints.Clear();
        }

    }

    public class MultiPartTessResult
    {
        //--------------------------------------------------
        //area 
        List<float> _allCoords = new List<float>();
        List<ushort> _allArrayIndexList = new List<ushort>();
        List<PartRange> _partIndexList = new List<PartRange>();

        int _currentPartBeginElementIndex = 0;
        int _currentPartFirstComponentStartAt = 0;
        VertexBufferObject _vbo;
        //--------------------------------------------------
        //border
        List<BorderPart> _borderParts = new List<BorderPart>();
        List<SmoothBorderSet> smoothBorders = new List<SmoothBorderSet>();
        VertexBufferObject _vbo_smoothBorder;
        PartRange[] _borderPartRanges;

        internal MultiPartTessResult()
        {
        }
        public int BeginPart()
        {

            _currentPartFirstComponentStartAt = _allCoords.Count;
            return _currentPartBeginElementIndex = _allArrayIndexList.Count;
        }
        public void EndPart()
        {
            _partIndexList.Add(
                new PartRange(
                    _currentPartFirstComponentStartAt,
                    _currentPartBeginElementIndex,
                    _allArrayIndexList.Count - _currentPartBeginElementIndex));
        }
        public void AddVertexIndexList(List<ushort> arr)
        {
            _allArrayIndexList.AddRange(arr);
        }
        public void Clear()
        {
            _allCoords.Clear();
        }
        public void AddTessCoord(float x, float y)
        {
            _allCoords.Add(x);
            _allCoords.Add(y);
        }
        public void AddTessCoords(float[] xy)
        {
            _allCoords.AddRange(xy);
        }
        public int PartCount
        {
            get { return _partIndexList.Count; }
        }


        void InitMultiPartVBOIfNeed()
        {
            if (_vbo != null) return;
            //
            _vbo = new VertexBufferObject();
            _vbo.CreateBuffers(_allCoords.ToArray(), _allArrayIndexList.ToArray(), _partIndexList.ToArray());
        }
        public VertexBufferObject GetVBO()
        {
            InitMultiPartVBOIfNeed();
            return _vbo;
        }
        public PartRange GetPartRange(int index)
        {
            return _partIndexList[index];
        }
        public List<float> GetAllCoords() { return _allCoords; }
        public List<ushort> GetAllArrayIndexList() { return _allArrayIndexList; }
        //--------------------------------------------------
        int _currentBorderPartBeginAt;
        public void BeginBorderPart()
        {
            //begin new border part
            _currentBorderPartBeginAt = smoothBorders.Count;
        }
        public void EndBorderPart()
        {
            //add to list
            BorderPart borderPart = new BorderPart();
            borderPart.beginAtBorderSetIndex = _currentBorderPartBeginAt;
            borderPart.count = smoothBorders.Count - _currentBorderPartBeginAt;
            _borderParts.Add(borderPart);
        }
        public void AddSmoothBorders(float[] smoothBorderArr, int vertexStripCount)
        {
            smoothBorders.Add(new SmoothBorderSet(smoothBorderArr, vertexStripCount));
        }
        public BorderPart GetBorderPartRange(int index)
        {
            return _borderParts[index];
        }
        public PartRange GetSmoothBorderPartRange(int index)
        {
            return _borderPartRanges[index];
        }

        void InitMultiPartBorderVBOIfNeed()
        {
            if (_vbo_smoothBorder != null) return;
            //
            _vbo_smoothBorder = new VertexBufferObject();
            List<SmoothBorderSet> borderSets = this.smoothBorders;
            int j = borderSets.Count;

            PartRange[] partRanges = new PartRange[j];
            int currentFirstComponentStartAt = 0;
            List<float> expandedBorderCoords = new List<float>();
            for (int i = 0; i < j; ++i)
            {
                SmoothBorderSet borderSet = borderSets[i];
                //create part range
                partRanges[i] = new PartRange(currentFirstComponentStartAt, 0, borderSet.vertexStripCount);
                expandedBorderCoords.AddRange(borderSet.smoothBorderArr);
                currentFirstComponentStartAt += borderSet.smoothBorderArr.Length;
            }
            _borderPartRanges = partRanges;
            _vbo_smoothBorder.CreateBuffers(expandedBorderCoords.ToArray(), null, partRanges);
        }
        public VertexBufferObject GetBorderVBO()
        {
            InitMultiPartBorderVBOIfNeed();
            return _vbo_smoothBorder;
        }
    }



    public struct SmoothBorderSet
    {
        public readonly float[] smoothBorderArr;
        public readonly int vertexStripCount;
        public SmoothBorderSet(float[] smoothBorderArr, int vertexStripCount)
        {
            this.smoothBorderArr = smoothBorderArr;
            this.vertexStripCount = vertexStripCount;
        }
    }

    /// <summary>
    /// a wrapper of internal private class
    /// </summary>
    public struct InternalGraphicsPath
    {
        //since Figure is private=> we use this to expose to public


        readonly Figure _figure;
        internal readonly MultiPartTessResult _mutltiPartTess;
        readonly List<Figure> figures;
        internal InternalGraphicsPath(List<Figure> figures)
        {
            _figure = null;
            _mutltiPartTess = null;
            this.figures = figures;
        }
        internal InternalGraphicsPath(Figure fig)
        {
            this.figures = null;
            this._mutltiPartTess = null;
            _figure = fig;
        }
        internal InternalGraphicsPath(MultiPartTessResult _mutltiPartTess)
        {
            this._figure = null;
            this.figures = null;
            this._mutltiPartTess = _mutltiPartTess;
        }

        internal int FigCount
        {
            get
            {
                if (_figure != null)
                {
                    return 1;
                }
                if (figures != null)
                {
                    return figures.Count;
                }
                return 0;
            }
        }
        internal Figure GetFig(int index)
        {
            if (index == 0)
            {
                return _figure ?? figures[0];
            }
            else
            {
                return figures[index];
            }
        }
    }



    class GLRenderVx : PixelFarm.Drawing.RenderVx
    {
        internal InternalGraphicsPath gxpth;
        internal MultiPartTessResult multipartTessResult;
        public GLRenderVx(InternalGraphicsPath gxpth)
        {
            this.gxpth = gxpth;
        }
        public GLRenderVx(MultiPartTessResult multipartTessResult)
        {
            this.multipartTessResult = multipartTessResult;
        }
    }
    public class GLRenderVxFormattedString : PixelFarm.Drawing.RenderVxFormattedString
    {
        char[] _charBuffer;
        DrawingGL.VertexBufferObject2 _vbo2;

        internal GLRenderVxFormattedString(char[] charBuffer)
        {
            this._charBuffer = charBuffer;
        }
        public override string OriginalString
        {
            get { return new string(_charBuffer); }
        }
        public float[] VertexCoords { get; set; }
        public ushort[] IndexArray { get; set; }
        public int VertexCount { get; set; }

        public DrawingGL.VertexBufferObject2 GetVbo()
        {
            if (_vbo2 != null)
            {
                return _vbo2;
            }

            _vbo2 = new VertexBufferObject2();
            _vbo2.CreateBuffers(this.VertexCoords, this.IndexArray);
            return _vbo2;
        }
        public override void Dispose()
        {
            //
            base.Dispose();
        }
    }
}