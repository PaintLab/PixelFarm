//MIT, 2014-2016,WinterDev

using System;
using PixelFarm.Drawing;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Agg;
namespace OpenTkEssTest
{
    [Info(OrderCode = "108.1")]
    [Info("T1081_LionFillBmpToTexture")]
    public class T1081_LionFillBmpToTexture : PrebuiltGLControlDemoBase
    {
        //***
        //software-based bitmap cache
        //this example:
        //we render the lion with Agg (software-based)
        //then copy pixel buffer to gl texture
        //and render the texture the bg surface 
        //***

        //---------------------------

        ActualImage aggImage;
        ImageGraphics2D imgGfx2d;
        AggCanvasPainter aggPainter;

        //---------------------------
        CanvasGL2d canvas2d;
        SpriteShape lionShape;
        VertexStore lionVxs;
        GLCanvasPainter painter;



        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            //-------------
            aggImage = new ActualImage(200, 100, PixelFarm.Agg.Image.PixelFormat.ARGB32);
            imgGfx2d = new ImageGraphics2D(aggImage, null);
            aggPainter = new AggCanvasPainter(imgGfx2d);
            //lion
            lionShape = new SpriteShape();
            lionShape.ParseLion();
            //flip this lion vertically before use with openGL
            PixelFarm.Agg.Transform.Affine aff = PixelFarm.Agg.Transform.Affine.NewMatix(
                 PixelFarm.Agg.Transform.AffinePlan.Scale(1, -1),
                 PixelFarm.Agg.Transform.AffinePlan.Translate(0, 600));
            lionVxs = aff.TransformToVxs(lionShape.Path.Vxs);
            DrawLion(aggPainter, lionShape, lionVxs);


            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            //-------------------------

            //-------------------------

            painter = new GLCanvasPainter(canvas2d, max, max);
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        static void DrawLion(CanvasPainter p, SpriteShape shape, VertexStore myvxs)
        {
            int j = shape.NumPaths;
            int[] pathList = shape.PathIndexList;
            Color[] colors = shape.Colors;
            for (int i = 0; i < j; ++i)
            {
                p.FillColor = colors[i];
                p.Fill(new VertexStoreSnap(myvxs, pathList[i]));
            }
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            //-------------------------------

            //int j = lionShape.NumPaths;
            //int[] pathList = lionShape.PathIndexList;
            //Color[] colors = lionShape.Colors;
            //VertexStore myvxs = lionVxs;
            //for (int i = 0; i < j; ++i)
            //{
            //    painter.FillColor = colors[i];
            //    painter.Fill(new VertexStoreSnap(myvxs, pathList[i]));
            //}
            //-------------------------------
            miniGLControl.SwapBuffers();
        }
    }
}

