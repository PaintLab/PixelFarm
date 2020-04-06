//MIT, 2014-present,WinterDev

using System;
using PixelFarm.Drawing;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;
namespace OpenTkEssTest
{
    [Info(OrderCode = "108.1")]
    [Info("T1081_LionFillBmpToTexture", AvailableOn = AvailableOn.GLES)]
    public class T1081_LionFillBmpToTexture : DemoBase
    {
        //***
        //software-based bitmap cache
        //this example:
        //we render the lion with Agg (software-based)
        //then copy pixel buffer to gl texture
        //and render the texture the bg surface 
        //***

        //---------------------------

        MemBitmap _memBmp;

        AggPainter _aggPainter;
        //---------------------------
        GLPainterCore _pcx;
        SpriteShape _lionShape;
        GLPainter _painter;
        GLBitmap _glBmp;

        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.Core;
            _painter = painter;

        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            PaintLab.Svg.VgVisualDoc vgVisualDoc = PaintLab.Svg.VgVisualDocHelper.CreateVgVisualDocFromFile("Samples/lion.svg");
            _lionShape = new SpriteShape(vgVisualDoc.VgRootElem);

            var lionBounds = _lionShape.Bounds;
            //-------------
            _memBmp = new MemBitmap((int)lionBounds.Width, (int)lionBounds.Height);
            _aggPainter = AggPainter.Create(_memBmp);


            DrawLion(_aggPainter, _lionShape);
            //convert affImage to texture 
            _glBmp = DemoHelper.LoadTexture(_memBmp);
        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        static void DrawLion(Painter p, SpriteShape shape)
        {
            shape.Paint(p);

            //int j = shape.NumPaths;
            //int[] pathList = shape.PathIndexList;
            //Color[] colors = shape.Colors;
            //for (int i = 0; i < j; ++i)
            //{
            //    p.FillColor = colors[i];
            //    p.Fill(new VertexStoreSnap(myvxs, pathList[i]));
            //}
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();
            //-------------------------------
            _pcx.DrawImage(_glBmp, 0, 600);
            SwapBuffers();
        }
    }
}

