//MIT, 2016-2017, WinterDev

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
        float _x0, _y0;
        float _moveX, _moveY;
        int _coordCount = 0;
        public void Clear()
        {
            _x0 = _y0 = 0;
            _coordCount = 0;
            expandCoords.Clear();
        }
        public void MoveTo(float x0, float y0)
        {
            _moveX = _x0 = x0;
            _moveY = _y0 = y0;
            _coordCount = 2;
        }
        public void LineTo(float x1, float y1)
        {
            CreateSmoothLineSegment(expandCoords, _x0, _y0, _x0 = x1, _y0 = y1);
            _coordCount += 2;
        }
        public void CloseContour()
        {
            CreateSmoothLineSegment(expandCoords, _x0, _y0, _x0 = _moveX, _y0 = _moveY);
            //not add new coord
        }

        public float[] BuildSmoothBorder(out int borderTriangleStripCount)
        {
            //build smooth border from existing 
            borderTriangleStripCount = _coordCount * 2;
            //
            float[] result = expandCoords.ToArray();
            expandCoords.Clear();
            //
            return result;
        }
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
        static void CreateSmoothLineSegment(List<float> coords, float x1, float y1, float x2, float y2)
        {

            //create with no line join
            //TODO: implement line join ***
            //we can calculate rad on server-side, so=> reduce num of vertex

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
        List<float> _tempCoords = new List<float>();
        internal List<int> contourEndPoints = new List<int>();

        public MultiPartPolygon()
        {

        }

        public void AddVertexSnap(PixelFarm.Agg.VertexStoreSnap vxsSnap)
        {
            //begin new snap vxs
            _tempCoords.Clear();
            var iter = vxsSnap.GetVertexSnapIter();
            double x, y;
            PixelFarm.Agg.VertexCmd cmd;

            //int index = 0;
            while ((cmd = iter.GetNextVertex(out x, out y)) != Agg.VertexCmd.NoMore)
            {
                if (cmd == Agg.VertexCmd.Close || cmd == Agg.VertexCmd.CloseAndEndFigure)
                {
                    //temp fix1
                    //some vertex snap may has more than 1 part
                    //expandCoordsList.Add(_tempCoords.ToArray());
                    //_tempCoords.Clear();
                    //contourEndPoints.Add(index);                    
                    contourEndPoints.Add(_tempCoords.Count - 1);
                }
                //add command to
                _tempCoords.Add((float)x);
                _tempCoords.Add((float)y);
                //
                //index++;
            }

            if (_tempCoords.Count > 0)
            {
                expandCoordsList.Add(_tempCoords.ToArray());
            }
            _tempCoords.Clear();
        }

    }

    class MultiPartTessResult
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
        List<SmoothBorderSet> smoothBorders = new List<SmoothBorderSet>();
        VertexBufferObject _vbo_smoothBorder;


        public MultiPartTessResult()
        {
        }
        public int BeginPart()
        {
            if (_allArrayIndexList.Count > 0)
            {

            }
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
        public void AddSmoothBorders(float[] smoothBorderArr, int vertexStripCount)
        {
            smoothBorders.Add(new SmoothBorderSet(smoothBorderArr, vertexStripCount));
        }
        public List<SmoothBorderSet> GetAllSmoothBorderSet()
        {
            return this.smoothBorders;
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
                currentFirstComponentStartAt += borderSet.vertexStripCount;
                expandedBorderCoords.AddRange(borderSet.smoothBorderArr);
            }
            _vbo_smoothBorder.CreateBuffers(expandedBorderCoords.ToArray(), null, partRanges);
        }
        public VertexBufferObject GetBorderVBO()
        {
            InitMultiPartBorderVBOIfNeed();
            return _vbo_smoothBorder;
        }

    }
    struct SmoothBorderSet
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
    class GLRenderVxFormattedString : PixelFarm.Drawing.RenderVxFormattedString
    {
        public GLRenderVxFormattedString(string str)
        {
            this.OriginalString = str;
        }
    }
}