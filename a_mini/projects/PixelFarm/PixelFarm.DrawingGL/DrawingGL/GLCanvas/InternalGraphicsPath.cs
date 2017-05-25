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
        public int begin;
        public int vertexCount;
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


    class MultiPartTessResult
    {
        List<float> _allCoords = new List<float>();
        List<ushort> _allArrayIndexList = new List<ushort>();
        List<PartRange> _partIndexList = new List<PartRange>();

        int _currentPartBeginAt = 0;


        public int BeginPart()
        {
            return _currentPartBeginAt = _partIndexList.Count;
        }
        public void EndPart()
        {
            //end current part
            int count = _allArrayIndexList.Count - _currentPartBeginAt;
            //
            PartRange p = new PartRange();
            p.begin = _currentPartBeginAt;
            p.vertexCount = count;
            //
            _partIndexList.Add(p);
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

        VertexBufferObject _vbo;
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
        public List<ushort> getAllArrayIndexList() { return _allArrayIndexList; }

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