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
        GLPainterContext _glsx;
        SpriteShape _lionShape;

        GLPainter _painter;
        protected override void OnGLSurfaceReady(GLPainterContext glsx, GLPainter painter)
        {
            _glsx = glsx;
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
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.ClearColorBuffer();
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

