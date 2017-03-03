//MIT, 2014-2016,WinterDev

using System;
using PixelFarm.Drawing;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Drawing.Fonts;
using PixelFarm.Drawing.Text;

namespace OpenTkEssTest
{
    [Info(OrderCode = "402")]
    [Info("T402_BrushTest2")]
    public class T402_BrushTest2 : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        RenderVx glyph_vx;
        LinearGradientBrush linearGrBrush2;
        PixelFarm.Agg.VertexStoreSnap tempSnap1;
        //  PixelFarm.Drawing.Fonts.SvgFontStore svgFontStore = new PixelFarm.Drawing.Fonts.SvgFontStore();
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            //temp***
            int max = Math.Max(this.Width, this.Height);
            canvas2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(max, max);
            painter = new GLCanvasPainter(canvas2d, max, max);

            //----------------------
            var win32InstallFontProvider = new PixelFarm.Drawing.InstallFontsProviderWin32();
            InstalledFontCollection collection = new InstalledFontCollection();
            collection.LoadInstalledFont(win32InstallFontProvider.GetInstalledFontIter());
            InstalledFont tahomaFont = collection.GetFont("tahoma", InstalledFontStyle.Regular);
            FontFace tahomaFace = OpenFontLoader.LoadFont(tahomaFont.FontPath, ScriptLangs.Latin);
            ActualFont actualFont = tahomaFace.GetFontAtPointsSize(72);
            FontGlyph glyph = actualFont.GetGlyph('K');

            //var svgFont = svgFontStore.LoadFont("svg-LiberationSansFont", 300);
            ////PathWriter p01 = new PathWriter();
            ////p01.MoveTo(0, 0);
            ////p01.LineTo(50, 100);
            ////p01.LineTo(100, 0);
            //////-
            ////p01.MoveTo(220, 10);
            ////p01.LineTo(50, 75);
            ////p01.LineTo(25, 15);
            ////p01.CloseFigure();
            ////p01.Stop();
            ////m_pathVxs = p01.Vxs;

            //var m_pathVxs = svgFont.GetGlyph('K').originalVxs;// typeFaceForLargeA.GetGlyphForCharacter('a');
            ////m_pathVxs = MergeFontSubFigures(m_pathVxs);

            //Affine shape_mtx = Affine.NewMatix(AffinePlan.Translate(150, 100));
            //m_pathVxs = shape_mtx.TransformToVxs(m_pathVxs);
            //var curveFlattener = new CurveFlattener();
            //var m_pathVxs2 = curveFlattener.MakeVxs(m_pathVxs);

            glyph_vx = painter.CreateRenderVx(tempSnap1 = new PixelFarm.Agg.VertexStoreSnap(glyph.flattenVxs));

            linearGrBrush2 = new LinearGradientBrush(
               new PointF(0, 0), Color.Red,
               new PointF(100,100), Color.Black);
            //----------------------
        }
        //PixelFarm.Agg.VertexStore MergeFontSubFigures(PixelFarm.Agg.VertexStore vxs)
        //{
        //    //sometimes we need to merge sub-figure 
        //    //before send it to tess

        //    PixelFarm.Agg.VertexStore newOne = new PixelFarm.Agg.VertexStore();
        //    int count = vxs.Count;
        //    double latestMoveToX = 0, latestMoveToY = 0;
        //    for (int i = 0; i < count; ++i)
        //    {
        //        double x, y;
        //        var cmd = vxs.GetVertex(i, out x, out y);
        //        switch (cmd)
        //        {
        //            case PixelFarm.Agg.VertexCmd.CloseAndEndFigure:
        //                {
        //                    //close
        //                    newOne.AddVertex(
        //                        latestMoveToX,
        //                        latestMoveToY, PixelFarm.Agg.VertexCmd.LineTo);
        //                }
        //                break;
        //            case PixelFarm.Agg.VertexCmd.Stop:
        //                {
        //                    i = count + 1; //this will break loop to exit
        //                    break;
        //                }
        //            case PixelFarm.Agg.VertexCmd.MoveTo:
        //                {
        //                    newOne.AddVertex(
        //                           latestMoveToX = x,
        //                           latestMoveToY = y, cmd);
        //                }
        //                break;
        //            case PixelFarm.Agg.VertexCmd.EndFigure:
        //                {

        //                }
        //                break;
        //            default:
        //                {
        //                    newOne.AddVertex(
        //                           x,
        //                           y, cmd);
        //                }
        //                break;
        //        }
        //    }
        //    return newOne; 
        //}
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            painter.FillColor = PixelFarm.Drawing.Color.Black;
            //painter.FillRectLBWH(0, 0, 150, 150);
            //GLBitmap glBmp = LoadTexture("..\\logo-dark.jpg");
            //var textureBrush = new TextureBrush(new GLImage(glBmp));
            //painter.FillRenderVx(textureBrush, polygon1);
            ////------------------------------------------------------------------------- 

            //fill
            painter.FillColor = PixelFarm.Drawing.Color.Black;
            painter.FillRenderVx(linearGrBrush2, glyph_vx);
            //painter.FillRenderVx(glyph_vx);
            //-------------------------------------------------------------------------  


            SwapBuffers();
        }
    }
}

