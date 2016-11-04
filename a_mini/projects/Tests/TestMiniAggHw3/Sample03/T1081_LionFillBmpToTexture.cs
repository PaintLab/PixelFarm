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
        GLCanvasPainter painter;

        GLBitmap glBmp;

        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            lionShape = new SpriteShape();
            lionShape.ParseLion();
            RectD lionBounds = lionShape.Bounds;
            //-------------
            aggImage = new ActualImage((int)lionBounds.Width, (int)lionBounds.Height, PixelFarm.Agg.Imaging.PixelFormat.ARGB32);
            imgGfx2d = new ImageGraphics2D(aggImage);
            aggPainter = new AggCanvasPainter(imgGfx2d);

            DrawLion(aggPainter, lionShape, lionShape.Path.Vxs);
            //convert affImage to texture 
            glBmp = LoadTexture(aggImage);

            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
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
            canvas2d.DrawImage(glBmp, 0, 600);
            miniGLControl.SwapBuffers();
        }
    }
}

