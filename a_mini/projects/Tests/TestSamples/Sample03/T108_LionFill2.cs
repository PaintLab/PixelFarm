//MIT, 2014-2016,WinterDev

using System;
using PixelFarm.Drawing;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Agg;

namespace OpenTkEssTest
{
    [Info(OrderCode = "108")]
    [Info("T108_LionFill")]
    public class T108_LionFill : DemoBase
    {
        GLRenderSurface _glsf;
        SpriteShape lionShape;
        VertexStore lionVxs;
        GLCanvasPainter painter;
        protected override void OnGLSurfaceReady(GLRenderSurface glsf, GLCanvasPainter painter)
        {
            this._glsf = glsf;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
           
            lionShape = new SpriteShape();
            lionShape.ParseLion();

            //flip this lion vertically before use with openGL
            PixelFarm.Agg.Transform.Affine aff = PixelFarm.Agg.Transform.Affine.NewMatix(
                 PixelFarm.Agg.Transform.AffinePlan.Scale(1, -1),
                 PixelFarm.Agg.Transform.AffinePlan.Translate(0, 600));
            lionVxs = new VertexStore();
            aff.TransformToVxs(lionShape.Path.Vxs, lionVxs);


        }
        protected override void DemoClosing()
        {
            _glsf.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsf.SmoothMode = SmoothMode.Smooth;
            _glsf.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsf.ClearColorBuffer();
            //-------------------------------

            int j = lionShape.NumPaths;
            int[] pathList = lionShape.PathIndexList;
            Color[] colors = lionShape.Colors;
            VertexStore myvxs = lionVxs;
            for (int i = 0; i < j; ++i)
            {
                painter.FillColor = colors[i];
                painter.Fill(new VertexStoreSnap(myvxs, pathList[i]));
            }
            //-------------------------------
            SwapBuffers();
        }
    }
}

