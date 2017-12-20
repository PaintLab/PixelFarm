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
        GLRenderSurface _glsx;
        SpriteShape lionShape;
        VertexStore lionVxs;
        GLPainter painter;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
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
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.ClearColorBuffer();
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

