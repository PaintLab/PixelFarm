//MIT, 2016-present, WinterDev
using System;
using System.Collections.Generic;
namespace PixelFarm.CpuBlit.VertexProcessing
{
    class ReusableCoordList
    {
        public List<float> _coordXYs = new List<float>();
        public List<int> _contourEndPoints = new List<int>();

        public void Reset()
        {
            _coordXYs.Clear();
            _contourEndPoints.Clear();
        }
        public static TempContext<ReusableCoordList> Borrow(out ReusableCoordList coordList)
        {
            if (!Temp<ReusableCoordList>.IsInit())
            {
                Temp<ReusableCoordList>.SetNewHandler(
                    () => new ReusableCoordList(),
                    s => s.Reset());
            }
            return PixelFarm.Temp<ReusableCoordList>.Borrow(out coordList);
        }
    }
    public enum TessTriangleTechnique
    {
        DrawArray = 1,
        DrawElement = 2
    }

    public class MultiFigures
    {
        Figure[] _figures;
        //
        float[] _areaTess;
        ushort[] _areaTessIndexList;
        float[] _smoothBorderTess; //smooth border result 
        int _tessAreaVertexCount;

        public MultiFigures(Figure[] figs)
        {
            _figures = figs;
        }

        public TessTriangleTechnique TessTriangleTech { get; private set; }
        public int FigureCount => _figures.Length;
        public Figure this[int index] => _figures[index];

        public float[] GetAreaTess(TessTool tess, Tesselate.Tesselator.WindingRuleType windingRuleType, TessTriangleTechnique tessTechnique)
        {

#if DEBUG
            if (this.TessTriangleTech == 0)
            {

            }
#endif

            if (TessTriangleTech != tessTechnique)
            {
                //re tess again
                this.TessTriangleTech = tessTechnique;
                //***
                using (ReusableCoordList.Borrow(out ReusableCoordList resuableCoordList))
                {
                    List<float> coordXYs = resuableCoordList._coordXYs;
                    List<int> contourEndPoints = resuableCoordList._contourEndPoints;


                    for (int i = 0; i < _figures.Length; ++i)
                    {
                        Figure figure = _figures[i];
                        coordXYs.AddRange(figure.coordXYs);
                        contourEndPoints.Add(coordXYs.Count - 1);
                    }


                    if (this.TessTriangleTech == TessTriangleTechnique.DrawArray)
                    {
                        tess.WindingRuleType = windingRuleType;
                        return (_areaTess = tess.TessAsTriVertexArray(
                            coordXYs.ToArray(),
                            contourEndPoints.ToArray(),
                            out _tessAreaVertexCount));
                    }
                    else
                    {
                        tess.WindingRuleType = windingRuleType;
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

        //public void Reset()
        //{
        //    _figures.Clear();
        //    _areaTess = null;
        //    _areaTessIndexList = null;
        //    _smoothBorderTess = null;
        //    _tessAreaVertexCount = 0; 
        //}

        public List<Poly2Tri.Polygon> GetTrianglulatedArea()
        {
            using (Poly2TriTool.Borrow(out Poly2TriTool poly2Tri))
            using (ReusableCoordList.Borrow(out ReusableCoordList reuseableList))
            {
                List<Poly2Tri.Polygon> output = new List<Poly2Tri.Polygon>();
                for (int i = 0; i < _figures.Length; ++i)
                {

                    Figure fig = _figures[i];
                    float[] figCoords = fig.coordXYs;

                    float prevX = float.MaxValue;
                    float prevY = float.MinValue;

                    int startAt = reuseableList._coordXYs.Count;
                    for (int n = 0; n < figCoords.Length;)
                    {
                        float x = figCoords[n];
                        float y = figCoords[n + 1];
                        reuseableList._coordXYs.Add(prevX = x);
                        reuseableList._coordXYs.Add(prevY = y);
                        n += 2;
                    }

                    {
                        if (reuseableList._coordXYs[startAt] == prevX && reuseableList._coordXYs[startAt + 1] == prevY)
                        {
                            reuseableList._coordXYs.RemoveAt(reuseableList._coordXYs.Count - 1);
                            reuseableList._coordXYs.RemoveAt(reuseableList._coordXYs.Count - 1);
                        }
                    }

                    reuseableList._contourEndPoints.Add(reuseableList._coordXYs.Count - 1);
                }

                poly2Tri.Triangulate(reuseableList._coordXYs.ToArray(), reuseableList._contourEndPoints.ToArray(), output);
                return Poly2TriPolygons = output;
            }
        }
        public List<Poly2Tri.Polygon> Poly2TriPolygons { get; private set; }



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

    public class Figure
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


        public float[] GetAreaTess(TessTool tess,
            Tesselate.Tesselator.WindingRuleType windingRuleType,
            TessTriangleTechnique tech)
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
                    tess.WindingRuleType = windingRuleType;
                    return _areaTess ??
                      (_areaTess = tess.TessAsTriVertexArray(coordXYs, null, out _tessAreaVertexCount));
                }
                else
                {
                    tess.WindingRuleType = windingRuleType;
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

        public ushort[] GetAreaIndexList() => _indexListArray; //after call GetAreaTess()

        public List<Poly2Tri.Polygon> GetTrianglulatedArea()
        {
            using (Poly2TriTool.Borrow(out Poly2TriTool poly2Tri))
            {
                List<Poly2Tri.Polygon> output = new List<Poly2Tri.Polygon>();
                poly2Tri.Triangulate(coordXYs, new int[] { coordXYs.Length }, output);
                return Poly2TriPolygons = output;
            }
        }
        public List<Poly2Tri.Polygon> Poly2TriPolygons { get; private set; }
#if DEBUG
        public override string ToString()
        {
            return "n=" + coordXYs.Length;
        }
#endif
    }

    class Poly2TriContour
    {
        public List<PixelFarm.Drawing.PointF> flattenPoints;
        bool _analyzedClockDirection;
        bool _isClockwise;
        public bool IsClockwise()
        {
            //after flatten
            if (_analyzedClockDirection)
            {
                return _isClockwise;
            }

            List<PixelFarm.Drawing.PointF> f_points = this.flattenPoints;
            if (f_points == null)
            {
                throw new NotSupportedException();
            }
            _analyzedClockDirection = true;


            //TODO: review here again***
            //---------------
            //http://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
            //check if hole or not
            //clockwise or counter-clockwise
            {
                //Some of the suggested methods will fail in the case of a non-convex polygon, such as a crescent. 
                //Here's a simple one that will work with non-convex polygons (it'll even work with a self-intersecting polygon like a figure-eight, telling you whether it's mostly clockwise).

                //Sum over the edges, (x2 − x1)(y2 + y1). 
                //If the result is positive the curve is clockwise,
                //if it's negative the curve is counter-clockwise. (The result is twice the enclosed area, with a +/- convention.)
                int j = flattenPoints.Count;
                double total = 0;
                for (int i = 1; i < j; ++i)
                {
                    PixelFarm.Drawing.PointF p0 = f_points[i - 1];
                    PixelFarm.Drawing.PointF p1 = f_points[i];
                    total += (p1.X - p0.X) * (p1.Y + p0.Y);

                }
                //the last one
                {
                    PixelFarm.Drawing.PointF p0 = f_points[j - 1];
                    PixelFarm.Drawing.PointF p1 = f_points[0];

                    total += (p1.X - p0.X) * (p1.Y + p0.Y);
                }
                _isClockwise = total >= 0;
            }
            return _isClockwise;
        }
    }

    public class Poly2TriTool
    {
        List<Poly2Tri.Polygon> _waitingHoles = new List<Poly2Tri.Polygon>();

        public void Triangulate(float[] polygon1, int[] contourEndIndices, List<Poly2Tri.Polygon> outputPolygons)
        {
            //create 
            List<Poly2TriContour> flattenContours = Poly2TriTool.CreateGlyphContours(polygon1, contourEndIndices);
            //--------------------------
            //TODO: review here, add hole or not  
            // more than 1 contours, no hole => eg.  i, j, ;,  etc
            // more than 1 contours, with hole => eg.  a,e ,   etc  

            //clockwise => not hole  
            _waitingHoles.Clear();

            int cntCount = flattenContours.Count;
            Poly2Tri.Polygon mainPolygon = null;
            //
            //this version if it is a hole=> we add it to main polygon
            //TODO: add to more proper polygon ***
            //eg i
            //-------------------------- 
            List<Poly2Tri.Polygon> otherPolygons = null;
            for (int n = 0; n < cntCount; ++n)
            {
                Poly2TriContour cnt = flattenContours[n];
                bool cntIsMainPolygon = cnt.IsClockwise();
                //if (yAxisFlipped)
                //{
                //    cntIsMainPolygon = !cntIsMainPolygon;
                //}

                if (cntIsMainPolygon)
                {
                    //main polygon
                    //not a hole
                    if (mainPolygon == null)
                    {
                        //if we don't have mainPolygon before
                        //this is main polygon
                        mainPolygon = CreatePolygon(cnt.flattenPoints);

                        if (_waitingHoles.Count > 0)
                        {
                            //flush all waiting holes to the main polygon
                            int j = _waitingHoles.Count;
                            for (int i = 0; i < j; ++i)
                            {
                                mainPolygon.AddHole(_waitingHoles[i]);
                            }
                            _waitingHoles.Clear();
                        }
                    }
                    else
                    {
                        //if we already have a main polygon
                        //then this is another sub polygon
                        //IsHole is correct after we Analyze() the glyph contour
                        Poly2Tri.Polygon subPolygon = CreatePolygon(cnt.flattenPoints);
                        if (otherPolygons == null)
                        {
                            otherPolygons = new List<Poly2Tri.Polygon>();
                        }
                        otherPolygons.Add(subPolygon);
                    }
                }
                else
                {
                    //this is a hole
                    Poly2Tri.Polygon subPolygon = CreatePolygon(cnt.flattenPoints);
                    if (mainPolygon == null)
                    {
                        //add to waiting polygon
                        _waitingHoles.Add(subPolygon);
                    }
                    else
                    {
                        //add to mainPolygon
                        mainPolygon.AddHole(subPolygon);
                    }
                }
            }
            if (_waitingHoles.Count > 0)
            {
                throw new NotSupportedException();
            }
            //------------------------------------------
            //2. tri angulate 
            Poly2Tri.P2T.Triangulate(mainPolygon); //that poly is triangulated 
            outputPolygons.Add(mainPolygon);

            if (otherPolygons != null)
            {
                outputPolygons.AddRange(otherPolygons);
                for (int i = otherPolygons.Count - 1; i >= 0; --i)
                {
                    Poly2Tri.P2T.Triangulate(otherPolygons[i]);
                }
            }
        }
        static Poly2Tri.Polygon CreatePolygon(List<PixelFarm.Drawing.PointF> flattenPoints)
        {
            List<Poly2Tri.TriangulationPoint> points = new List<Poly2Tri.TriangulationPoint>();

            //limitation: poly tri not accept duplicated points! *** 
            double prevX = 0;
            double prevY = 0;

            int j = flattenPoints.Count;
            //pass
            for (int i = 0; i < j; ++i)
            {
                Drawing.PointF p = flattenPoints[i];
                double x = p.X; //start from original X***
                double y = p.Y; //start from original Y***

                if (x == prevX && y == prevY)
                {
                    if (i > 0)
                    {
                        //skip duplicated point
                        continue;
                    }
                }
                else
                {
                    var triPoint = new Poly2Tri.TriangulationPoint(prevX = x, prevY = y) { userData = p };
                    points.Add(triPoint);
                }
            }
            return new Poly2Tri.Polygon(points.ToArray());
        }
        static List<Poly2TriContour> CreateGlyphContours(float[] polygon1, int[] contourEndIndices)
        {
            List<Poly2TriContour> contours = new List<Poly2TriContour>();
            int contourCount = contourEndIndices.Length;
            int index = 0;
            for (int c = 0; c < contourCount; ++c)
            {
                Poly2TriContour contour = new Poly2TriContour();
                List<PixelFarm.Drawing.PointF> list = new List<PixelFarm.Drawing.PointF>();
                contour.flattenPoints = list;

                int endAt = contourEndIndices[c];

                for (; index < endAt;)
                {
                    list.Add(new PixelFarm.Drawing.PointF(polygon1[index], polygon1[index + 1]));//the point is already flatten so=>false                     
                    index += 2;
                }

                //--
                //temp hack here!
                //ensure=> duplicated points,
                //most common => first point and last point
                PixelFarm.Drawing.PointF p0 = list[0];
                PixelFarm.Drawing.PointF lastPoint = list[list.Count - 1];
                if (p0.X == lastPoint.X && p0.Y == lastPoint.Y)
                {
                    list.RemoveAt(list.Count - 1);
                }
                //--
                contours.Add(contour);
            }
            return contours;
        }

        public Poly2TriTool()
        {

        }
        void Reset()
        {
            _waitingHoles.Clear();
        }
        public static TempContext<Poly2TriTool> Borrow(out Poly2TriTool poly2TriTool)
        {
            if (!Temp<Poly2TriTool>.IsInit())
            {
                Temp<Poly2TriTool>.SetNewHandler(
                    () => new Poly2TriTool(),
                    s => s.Reset());
            }
            return PixelFarm.Temp<Poly2TriTool>.Borrow(out poly2TriTool);
        }
    }


    public struct FigureContainer
    {
        public readonly Figure _figure;
        public readonly MultiFigures _multiFig;
        public FigureContainer(Figure fig)
        {
            _figure = fig;
            _multiFig = null;
        }
        public FigureContainer(MultiFigures multiFig)
        {
            _figure = null;
            _multiFig = multiFig;
        }
        public bool IsSingleFigure => _figure != null;
    }

    public class SmoothBorderBuilder
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
        public float[] BuildSmoothBorders(Figure[] figures, bool isClosedFigure, out int borderTriangleStripCount)
        {

            _expandCoords.Clear();

            int total_b_triangleStripCount = 0;
            for (int i = 0; i < figures.Length; ++i)
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

    public class FigureBuilder
    {
        //helper struct
        List<float> _xylist = new List<float>();
        List<Figure> _figs = new List<Figure>();
        public FigureBuilder()
        {
        }
        public FigureContainer Build(PixelFarm.Drawing.VertexStore vxs)
        {
            double prevX = 0;
            double prevY = 0;
            double prevMoveToX = 0;
            double prevMoveToY = 0;

            _xylist.Clear();
            _figs.Clear();
            //TODO: reivew here 
            //about how to reuse this list  
            //result...


            int index = 0;
            VertexCmd cmd;

            double x, y;
            while ((cmd = vxs.GetVertex(index++, out x, out y)) != VertexCmd.NoMore)
            {
                switch (cmd)
                {
                    case PixelFarm.CpuBlit.VertexCmd.MoveTo:

                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        _xylist.Add((float)x);
                        _xylist.Add((float)y);
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.LineTo:
                        _xylist.Add((float)x);
                        _xylist.Add((float)y);
                        prevX = x;
                        prevY = y;
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.Close:
                        {
                            //from current point 
                            _xylist.Add((float)prevMoveToX);
                            _xylist.Add((float)prevMoveToY);
                            prevX = prevMoveToX;
                            prevY = prevMoveToY;
                            //-----------
                            Figure newfig = new Figure(_xylist.ToArray());
                            newfig.IsClosedFigure = true;

                            _figs.Add(newfig);
                            //-----------
                            _xylist.Clear(); //clear temp list

                        }
                        break;
                    case VertexCmd.CloseAndEndFigure:
                        {
                            //from current point 
                            _xylist.Add((float)prevMoveToX);
                            _xylist.Add((float)prevMoveToY);
                            prevX = prevMoveToX;
                            prevY = prevMoveToY;
                            // 
                            Figure newfig = new Figure(_xylist.ToArray());
                            newfig.IsClosedFigure = true;
                            _figs.Add(newfig);
                            //-----------
                            _xylist.Clear();//clear temp list
                        }
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.NoMore:
                        goto EXIT_LOOP;
                    default:
                        throw new System.NotSupportedException();
                }
            }
        EXIT_LOOP:

            if (_figs.Count == 0)
            {
                Figure newfig = new Figure(_xylist.ToArray());
                newfig.IsClosedFigure = false;
                return new FigureContainer(newfig);
            }
            //
            if (_xylist.Count > 1)
            {
                _xylist.Add((float)prevMoveToX);
                _xylist.Add((float)prevMoveToY);
                prevX = prevMoveToX;
                prevY = prevMoveToY;
                //
                Figure newfig = new Figure(_xylist.ToArray());
                newfig.IsClosedFigure = true; //? 
                _figs.Add(newfig);
            }

            if (_figs.Count == 1)
            {
                Figure fig = _figs[0];
                _figs.Clear();
                return new FigureContainer(fig);
            }
            else
            {
                MultiFigures multiFig = new MultiFigures(_figs.ToArray());
                _figs.Clear();
                return new FigureContainer(multiFig);
            }
        }
    }
}