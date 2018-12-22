//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;
using PaintLab.Svg;
namespace OpenTkEssTest
{
    [Info(OrderCode = "108")]
    [Info("T108_LionFill", SupportedOn = AvailableOn.GLES)]
    public class T108_LionFill : DemoBase
    {
        GLPainterContext _pcx;
        SpriteShape _lionShape;

        GLPainter _painter;
        protected override void OnGLPainterReady(GLPainterContext pcx, GLPainter painter)
        {
            _pcx = pcx;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            string sampleFile = "Samples/lion.svg";
            //string sampleFile = "Samples/tiger_whiskers.svg";
            //string sampleFile = "Samples/tiger002.svg";
            //string sampleFile = "Samples/tiger_wrinkles.svg";

            VgVisualElement vgVisElem = VgVisualDocHelper.CreateVgVisualDocFromFile(sampleFile).VgRootElem;
            _lionShape = new SpriteShape(vgVisElem);
            //flip this lion vertically before use with openGL
            PixelFarm.CpuBlit.VertexProcessing.Affine aff = PixelFarm.CpuBlit.VertexProcessing.Affine.NewMatix(
                 PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Scale(1, -1),
                 PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(0, 600));
            _lionShape.ApplyTransform(aff);
        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();
            //-------------------------------

            _lionShape.Paint(_painter);

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
            SwapBuffers();
        }
    }
}

