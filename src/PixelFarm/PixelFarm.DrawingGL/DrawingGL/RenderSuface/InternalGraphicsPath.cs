//MIT, 2016-present, WinterDev

using System.Collections.Generic;


namespace PixelFarm.DrawingGL
{

    enum TessTriangleTechnique
    {

        DrawArray = 1,
        DrawElement = 2
    }



    class MultiFigures
    {
        List<Figure> _figures = new List<Figure>();
        //
        float[] _areaTess;
        ushort[] _areaTessIndexList;
        float[] _smoothBorderTess; //smooth border result 
        int _tessAreaVertexCount;

        public MultiFigures()
        {
        }
        public TessTriangleTechnique TessTriangleTech { get; private set; }
        public int FigureCount => _figures.Count;
        public Figure this[int index] => _figures[index];


        public void AddFigure(Figure figure)
        {
            _figures.Add(figure);
        }


        public float[] GetAreaTess(TessTool tess, TessTriangleTechnique tech)
        {

#if DEBUG
            if (this.TessTriangleTech == 0)
            {

            }
#endif

            if (TessTriangleTech != tech)
            {
                //re tess again
                this.TessTriangleTech = tech;
                //***
                using (Borrow(out ReusableCoordList resuableCoordList))
                {
                    List<float> coordXYs = resuableCoordList._coordXYs;
                    List<int> contourEndPoints = resuableCoordList._contourEndPoints;

                    int j = _figures.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        Figure figure = _figures[i];
                        coordXYs.AddRange(figure.coordXYs);
                        contourEndPoints.Add(coordXYs.Count - 1);
                    }


                    if (this.TessTriangleTech == TessTriangleTechnique.DrawArray)
                    {

                        return (_areaTess = tess.TessAsTriVertexArray(
                            coordXYs.ToArray(),
                            contourEndPoints.ToArray(),
                            out _tessAreaVertexCount));
                    }
                    else
                    {
                        _areaTessIndexList = tess.TessAsTriIndexArray(
                            coordXYs.ToArray(),
                            contourEndPoints.ToArray(),
                            out _areaTess,
                            out _tessAreaVertexCount);
                        return _areaTess;
                    }
                }
            }
            else
            {
                //if equal
                return _areaTess;
            }

        }

        public ushort[] GetAreaIndexList() => _areaTessIndexList; //after call GetAreaTess()
        public int TessAreaVertexCount => _tessAreaVertexCount; //after call GetAreaTess()
        //------------
        int _borderTriangleStripCount;//for smoothborder
        public float[] GetSmoothBorders(SmoothBorderBuilder smoothBorderBuilder)
        {
            //return existing result if not null
            //or create a newone if the old result is not exist

            return _smoothBorderTess ??
                    (_smoothBorderTess =
                    smoothBorderBuilder.BuildSmoothBorders(_figures, IsClosedFigure, out _borderTriangleStripCount));
        }
        public int BorderTriangleStripCount => _borderTriangleStripCount;
        public bool IsClosedFigure { get; set; }

        public void Reset()
        {
            _figures.Clear();
            _areaTess = null;
            _areaTessIndexList = null;
            _smoothBorderTess = null;
            _tessAreaVertexCount = 0;

        }

        class ReusableCoordList
        {
            public List<float> _coordXYs = new List<float>();
            public List<int> _contourEndPoints = new List<int>();

            public void Reset()
            {
                _coordXYs.Clear();
                _contourEndPoints.Clear();
            }
        }

        static PixelFarm.CpuBlit.VertexProcessing.TempContext<ReusableCoordList> Borrow(out ReusableCoordList coordList)
        {
            if (!PixelFarm.CpuBlit.VertexProcessing.Temp<ReusableCoordList>.IsInit())
            {
                PixelFarm.CpuBlit.VertexProcessing.Temp<ReusableCoordList>.SetNewHandler(
                    () => new ReusableCoordList(),
                    s => s.Reset());
            }
            return PixelFarm.CpuBlit.VertexProcessing.Temp<ReusableCoordList>.Borrow(out coordList);
        }

#if DEBUG
        public override string ToString()
        {
            System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
            if (_areaTess != null)
            {
                stbuilder.Append("A");
            }
            if (_smoothBorderTess != null)
            {
                stbuilder.Append("B");
            }
            return stbuilder.ToString();
        }
#endif 
    }
    class Figure
    {
        //TODO: review here again*** 
        public readonly float[] coordXYs; //this is user provide coord
                                          //---------
                                          //system tess ...
        float[] _areaTess;
        float[] _smoothBorderTess; //smooth border result
        int _borderTriangleStripCount;//for smoothborder
        int _tessAreaVertexCount;

        //---------
        ushort[] _indexListArray;//for VBO


        public Figure(float[] coordXYs)
        {
            this.coordXYs = coordXYs;
        }
        public TessTriangleTechnique TessTriangleTech { get; private set; }
        public bool IsClosedFigure { get; set; }
        public int BorderTriangleStripCount => _borderTriangleStripCount;
        public int TessAreaVertexCount => _tessAreaVertexCount;

        public float[] GetSmoothBorders(SmoothBorderBuilder smoothBorderBuilder)
        {
            //return existing result if not null
            //or create a newone if the old result is not exist

            return _smoothBorderTess ??
                    (_smoothBorderTess =
                    smoothBorderBuilder.BuildSmoothBorders(coordXYs, IsClosedFigure, out _borderTriangleStripCount));
        }
        public float[] GetAreaTess(TessTool tess, TessTriangleTechnique tech)
        {

#if DEBUG
            if (this.TessTriangleTech == 0)
            {

            }
#endif

            if (TessTriangleTech != tech)
            {
                //re tess again
                this.TessTriangleTech = tech;
                //***
                if (this.TessTriangleTech == TessTriangleTechnique.DrawArray)
                {

                    return _areaTess ??
                      (_areaTess = tess.TessAsTriVertexArray(coordXYs, null, out _tessAreaVertexCount));
                }
                else
                {
                    _indexListArray = tess.TessAsTriIndexArray(coordXYs,
                        null,
                        out _areaTess,
                        out _tessAreaVertexCount);
                    return _areaTess;
                }

            }
            else
            {
                //if equal
                return _areaTess;
            }
        }
    }

    class SmoothBorderBuilder
    {
        List<float> _expandCoords = new List<float>();
        public float[] BuildSmoothBorders(float[] coordXYs, bool isClosedFigure, out int borderTriangleStripCount)
        {
            _expandCoords.Clear();
            BuildSmoothBorders(coordXYs, isClosedFigure, _expandCoords, out borderTriangleStripCount);
            //
            float[] result = _expandCoords.ToArray();
            _expandCoords.Clear();
            //
            return result;
        }
        public float[] BuildSmoothBorders(List<Figure> figures, bool isClosedFigure, out int borderTriangleStripCount)
        {
            int j = figures.Count;
            _expandCoords.Clear();

            int total_b_triangleStripCount = 0;
            for (int i = 0; i < j; ++i)
            {
                Figure fig = figures[i];
                if (i == 0)
                {
                    //first
                    BuildSmoothBorders(fig.coordXYs, fig.IsClosedFigure, _expandCoords, out int b_triangleStripCount);
                    total_b_triangleStripCount += b_triangleStripCount;
                }
                else
                {
                    int latestCoordCount = _expandCoords.Count;
                    //add degenerative triangle
                    //for GLES30 consider=>GL_PRIMITIVE_RESTART_FIXED_INDEX
                    //see https://developer.apple.com/library/archive/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html


                    //To merge two triangle strips, duplicate the last vertex of the first strip and the first vertex of the second strip,
                    //as shown in Figure 8-6. When this strip is submitted to OpenGL ES, 
                    //triangles DEE, EEF, EFF, and FFG are considered degenerate and not processed or rasterized.

                    float prev_x = _expandCoords[latestCoordCount - 4];
                    float prev_y = _expandCoords[latestCoordCount - 3];
                    float prev_z = _expandCoords[latestCoordCount - 2];
                    float prev_w = _expandCoords[latestCoordCount - 1];

                    _expandCoords.Add(prev_x);
                    _expandCoords.Add(prev_y);
                    _expandCoords.Add(prev_z);
                    _expandCoords.Add(prev_w);

                    latestCoordCount += 4;//***

                    //since we use a single coord
                    //preserve space (4 coords) for degenerative triangles
                    //(see restore later)
                    _expandCoords.Add(0);
                    _expandCoords.Add(0);
                    _expandCoords.Add(0);
                    _expandCoords.Add(0);
                    //-----
                    BuildSmoothBorders(fig.coordXYs, fig.IsClosedFigure, _expandCoords, out int b_triangleStripCount);
                    //-----
                    //restore- 4 coord of latest expandCoords set
                    _expandCoords[latestCoordCount] = _expandCoords[latestCoordCount + 4];
                    _expandCoords[latestCoordCount + 1] = _expandCoords[latestCoordCount + 5];
                    _expandCoords[latestCoordCount + 2] = _expandCoords[latestCoordCount + 6];
                    _expandCoords[latestCoordCount + 3] = _expandCoords[latestCoordCount + 7];

                    total_b_triangleStripCount += b_triangleStripCount + 1; //*** +1 for a degenerative triangle
                }
            }

            borderTriangleStripCount = total_b_triangleStripCount;
            float[] result = _expandCoords.ToArray();
            _expandCoords.Clear();
            //
            return result;

        }


        //examples2
        //public float[] BuildSmoothBorders(float[] coordXYs, int segStartAt, int len, out int borderTriangleStripCount)
        //{
        //    _expandCoords.Clear();
        //    float[] coords = coordXYs;
        //    //from user input coords
        //    //expand it
        //    //TODO: review this again***

        //    int lim = (segStartAt + len);
        //    for (int i = segStartAt; i < lim;)
        //    {
        //        CreateSmoothLineSegment(_expandCoords,/*x0*/ coords[i], /*y0*/coords[i + 1],/*x1*/ coords[i + 2],/*y1*/ coords[i + 3]);
        //        i += 2;
        //    }
        //    //close coord
        //    int last = segStartAt + len;
        //    CreateSmoothLineSegment(_expandCoords, coords[last], coords[last + 1], coords[segStartAt], coords[segStartAt + 1]);

        //    borderTriangleStripCount = (len + 2) << 1; //(len + 2) *2
        //    //
        //    float[] result = _expandCoords.ToArray();
        //    _expandCoords.Clear();
        //    //
        //    return result;
        //}

        static void BuildSmoothBorders(float[] coordXYs, bool isClosedFigure, List<float> outputExpandCoords, out int borderTriangleStripCount)
        {
            float[] coords = coordXYs;
            int coordCount = coordXYs.Length;
            //from user input coords
            //expand it
            //TODO: review this again*** 
            if (isClosedFigure)
            {
                int lim = coordCount - 2;
                for (int i = 0; i < lim;)
                {
                    CreateSmoothLineSegment(outputExpandCoords, coords[i], coords[i + 1], coords[i + 2], coords[i + 3]);
                    i += 2;
                }
                //close coord
                CreateSmoothLineSegment(outputExpandCoords, coords[coordCount - 2], coords[coordCount - 1], coords[0], coords[1]);
                borderTriangleStripCount = coordCount << 1;// coordCount * 2;

            }
            else
            {

                int lim = coordCount - 2;
                for (int i = 0; i < lim;)
                {
                    CreateSmoothLineSegment(outputExpandCoords, coords[i], coords[i + 1], coords[i + 2], coords[i + 3]);
                    i += 2;
                }
                //TODO: review here
                //close coord-- TEMP fix***
                CreateSmoothLineSegment(outputExpandCoords, coords[coordCount - 2], coords[coordCount - 1], coords[coordCount - 2], coords[coordCount - 1]);

                borderTriangleStripCount = coordCount << 1;//coordCount * 2;
            }
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

 


    /// <summary>
    /// a wrapper of internal private class
    /// </summary>
    public class PathRenderVx : PixelFarm.Drawing.RenderVx
    {
        //since Figure is private=> we use this to expose to public 
        readonly Figure _figure;
        readonly MultiFigures _figures;
        internal PathRenderVx(MultiFigures figures)
        {
            _figure = null;
            _figures = figures;
        }
        internal PathRenderVx(Figure fig)
        {
            _figures = null;
            _figure = fig;
        }

        internal int FigCount => (_figure != null) ? 1 : _figures.FigureCount;

        internal Figure GetFig(int index)
        {
            if (index == 0)
            {
                return _figure ?? _figures[0];
            }
            else
            {
                return _figures[index];
            }
        }
        internal float[] GetAreaTess(TessTool tess)
        {
            return (_figure != null) ?
                        _figure.GetAreaTess(tess, TessTriangleTechnique.DrawArray) :
                        _figures.GetAreaTess(tess, TessTriangleTechnique.DrawArray);
        }

        //
        public int TessAreaVertexCount => (_figure != null) ?
                                           _figure.TessAreaVertexCount :
                                           _figures.TessAreaVertexCount;
        //
        //----------------------------------------------------
        //
        internal float[] GetSmoothBorders(SmoothBorderBuilder smoothBorderBuilder)
        {
            return (_figure != null) ?
                    _figure.GetSmoothBorders(smoothBorderBuilder) :
                    _figures.GetSmoothBorders(smoothBorderBuilder);
        }
        //
        //
        internal int BorderTriangleStripCount => (_figure != null) ?
                                                  _figure.BorderTriangleStripCount :
                                                  _figures.BorderTriangleStripCount;
        //
    }

    public class GLRenderVxFormattedString : PixelFarm.Drawing.RenderVxFormattedString
    {
        DrawingGL.VertexBufferObject _vbo;
        internal GLRenderVxFormattedString()
        {
        }
        public override string OriginalString => throw new System.NotImplementedException();//not used original string

        public float[] VertexCoords { get; set; }
        public ushort[] IndexArray { get; set; }
        public int IndexArrayCount { get; set; }

        public DrawingGL.VertexBufferObject GetVbo()
        {
            if (_vbo != null)
            {
                return _vbo;
            }

            _vbo = new VertexBufferObject();
            _vbo.CreateBuffers(this.VertexCoords, this.IndexArray);
            return _vbo;
        }
        public override void Dispose()
        {
            if (_vbo != null)
            {
                _vbo.Dispose();
                _vbo = null;
            }
            base.Dispose();
        }
    }
}