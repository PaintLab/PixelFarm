//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;

using System.IO;
using System.Windows.Forms;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;

using PixelFarm.Contours;
using Typography.OpenFont;
using PixelFarm.CpuBlit.VertexProcessing;

using Mini;
namespace PixelFarm
{
    public enum GlyphDemo_TessTech
    {
        SgiTess,
        Poly2TriDT,
    }

    [Info(OrderCode = "09")]
    [Info(DemoCategory.Vector)]
    public class GlyphDemo : DemoBase
    {
        Typeface _typeface;
        PixelFarm.Drawing.Fonts.GlyphTranslatorToVxs _tovxs;
        Typography.Contours.GlyphOutlineBuilder _glyphPathBuilder;
        TessTool _tessTool;
        public GlyphDemo()
        {
            SingleChar = "";

            string testFont = "c:\\Windows\\Fonts\\Tahoma.ttf";
            using (FileStream fs = new FileStream(testFont, FileMode.Open, FileAccess.Read))
            {
                OpenFontReader reader = new OpenFontReader();
                _typeface = reader.Read(fs);
            }

            _tovxs = new PixelFarm.Drawing.Fonts.GlyphTranslatorToVxs();
            _glyphPathBuilder = new Typography.Contours.GlyphOutlineBuilder(_typeface);
            //
            _tessTool = new TessTool();
        }

        [DemoConfig]
        public string SingleChar { get; set; }
        [DemoConfig]
        public GlyphDemo_TessTech Tess { get; set; }
        [DemoConfig]
        public bool ContourAnalysis { get; set; }

        public override void Draw(Painter p)
        {
            UpdateOutput(p);
        }



        void UpdateOutput(Painter painter)
        {
            string oneChar = SingleChar.Trim();
            if (string.IsNullOrEmpty(oneChar)) return;
            //
            char selectedChar = oneChar[0];

            DrawOutput(painter, _typeface, selectedChar);
        }

        void DrawOutput(Painter painter, Typeface typeface, char selectedChar)
        {
#if DEBUG
            painter.Clear(Color.White);

            //this is a demo.
            //
            float fontSizeInPts = 300;
            _glyphPathBuilder.BuildFromGlyphIndex(typeface.GetGlyphIndex(selectedChar), fontSizeInPts);

            var prevColor = painter.StrokeColor;
            painter.StrokeColor = Color.Black;
            using (VxsTemp.Borrow(out var v1))
            {
                _glyphPathBuilder.ReadShapes(_tovxs);
                _tovxs.WriteOutput(v1); //write content from GlyphTranslator to v1

                painter.Fill(v1, PixelFarm.Drawing.Color.Gray);
                _tovxs.Reset();

                //tess the vxs 

                FigureBuilder figBuilder = new FigureBuilder();
                FigureContainer container = figBuilder.Build(v1);
                TessTriangleTechnique tessTechnique = TessTriangleTechnique.DrawElement;

                if (container.IsSingleFigure)
                {
                    Figure figure = container._figure;
                    if (Tess == GlyphDemo_TessTech.SgiTess)
                    {
                        //coords of tess triangles 
                        switch (tessTechnique)
                        {
                            case TessTriangleTechnique.DrawArray:
                                {
                                    painter.dbugDrawTessTriangles(figure.GetAreaTess(_tessTool, _tessTool.WindingRuleType, TessTriangleTechnique.DrawArray));
                                }
                                break;
                            case TessTriangleTechnique.DrawElement:
                                {
                                    float[] tessArea = figure.GetAreaTess(_tessTool, _tessTool.WindingRuleType, TessTriangleTechnique.DrawElement);
                                    ushort[] index = figure.GetAreaIndexList();
                                    painter.dbugDrawTessTriangles(tessArea, index);
                                }
                                break;
                        }
                    }
                    else
                    {

                        if (ContourAnalysis)
                        {
                            ContourAnalyzer analyzer1 = new ContourAnalyzer();
                            IntermediateOutline outline = analyzer1.CreateIntermediateOutline(v1);

                            GlyphDebugContourVisualizer dbugVisualizer = new GlyphDebugContourVisualizer();
                            dbugVisualizer.SetPainter(painter);
                            dbugVisualizer.Scale = _typeface.CalculateScaleToPixelFromPointSize(fontSizeInPts);
                            dbugVisualizer.WalkCentroidLine(outline);
                        }
                        else
                        {
                            //Poly2Tri                        
                            List<Poly2Tri.Polygon> polygons = figure.GetTrianglulatedArea(false);
                            //draw polygon 
                            painter.StrokeColor = Color.Red;
                            painter.dbugDrawPoly2TriPolygon(polygons);
                        }
                    }

                }
                else
                {
                    MultiFigures multiFig = container._multiFig;
                    if (Tess == GlyphDemo_TessTech.SgiTess)
                    {
                        switch (tessTechnique)
                        {
                            case TessTriangleTechnique.DrawArray:
                                {
                                    painter.dbugDrawTessTriangles(multiFig.GetAreaTess(_tessTool, _tessTool.WindingRuleType, TessTriangleTechnique.DrawArray));
                                }
                                break;
                            case TessTriangleTechnique.DrawElement:
                                {
                                    float[] tessArea = multiFig.GetAreaTess(_tessTool, _tessTool.WindingRuleType, TessTriangleTechnique.DrawElement);
                                    ushort[] index = multiFig.GetAreaIndexList();
                                    painter.dbugDrawTessTriangles(tessArea, index);
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (ContourAnalysis)
                        {
                            ContourAnalyzer analyzer1 = new ContourAnalyzer();
                            IntermediateOutline outline = analyzer1.CreateIntermediateOutline(v1);

                            GlyphDebugContourVisualizer dbugVisualizer = new GlyphDebugContourVisualizer();
                            dbugVisualizer.SetPainter(painter);
                            dbugVisualizer.Scale = _typeface.CalculateScaleToPixelFromPointSize(fontSizeInPts);
                            dbugVisualizer.WalkCentroidLine(outline);
                        }
                        else
                        {
                            List<Poly2Tri.Polygon> polygons = multiFig.GetTrianglulatedArea(false);
                            painter.StrokeColor = Color.Red;
                            painter.dbugDrawPoly2TriPolygon(polygons);
                        }
                    }
                }

            }
            painter.StrokeColor = prevColor;
            //-------------



            //tess
            //if (rdoTessSGI.Checked)
            //{

            //    //SGI Tess Lib

            //    if (!_tessTool.TessPolygon(polygon1, _contourEnds))
            //    {
            //        return;
            //    }
            //    //1.
            //    List<ushort> indexList = _tessTool.TessIndexList;
            //    //2.
            //    List<TessVertex2d> tempVertexList = _tessTool.TempVertexList;
            //    //3.
            //    int vertexCount = indexList.Count;
            //    //-----------------------------    
            //    int orgVertexCount = polygon1.Length / 2;
            //    float[] vtx = new float[vertexCount * 2];//***
            //    int n = 0;

            //    for (int p = 0; p < vertexCount; ++p)
            //    {
            //        ushort index = indexList[p];
            //        if (index >= orgVertexCount)
            //        {
            //            //extra coord (newly created)
            //            TessVertex2d extraVertex = tempVertexList[index - orgVertexCount];
            //            vtx[n] = (float)extraVertex.x;
            //            vtx[n + 1] = (float)extraVertex.y;
            //        }
            //        else
            //        {
            //            //original corrd
            //            vtx[n] = (float)polygon1[index * 2];
            //            vtx[n + 1] = (float)polygon1[(index * 2) + 1];
            //        }
            //        n += 2;
            //    }
            //    //-----------------------------    
            //    //draw tess result
            //    int j = vtx.Length;
            //    for (int i = 0; i < j;)
            //    {
            //        var p0 = new PointF(vtx[i], vtx[i + 1]);
            //        var p1 = new PointF(vtx[i + 2], vtx[i + 3]);
            //        var p2 = new PointF(vtx[i + 4], vtx[i + 5]);

            //        _g.DrawLine(Pens.Red, p0, p1);
            //        _g.DrawLine(Pens.Red, p1, p2);
            //        _g.DrawLine(Pens.Red, p2, p0);

            //        i += 6;
            //    }
            //}
            //else
            //{

            //    List<Poly2Tri.Polygon> outputPolygons = new List<Poly2Tri.Polygon>();
            //    Poly2TriExampleHelper.Triangulate(polygon1, contourEndIndices, flipYAxis, outputPolygons);
            //    foreach (Poly2Tri.Polygon polygon in outputPolygons)
            //    {
            //        foreach (Poly2Tri.DelaunayTriangle tri in polygon.Triangles)
            //        {
            //            Poly2Tri.TriangulationPoint p0 = tri.P0;
            //            Poly2Tri.TriangulationPoint p1 = tri.P1;
            //            Poly2Tri.TriangulationPoint p2 = tri.P2;

            //            _g.DrawLine(Pens.Red, (float)p0.X, (float)p0.Y, (float)p1.X, (float)p1.Y);
            //            _g.DrawLine(Pens.Red, (float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y);
            //            _g.DrawLine(Pens.Red, (float)p2.X, (float)p2.Y, (float)p0.X, (float)p0.Y);
            //        }
            //    }
            //}
#endif
        }

    }
}