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
    public class T108_LionFill : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        SpriteShape lionShape;
        VertexStore lionVxs;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            lionShape = new SpriteShape();
            lionShape.ParseLion();
            //flip this lion vertically before use with openGL
            PixelFarm.Agg.Transform.Affine aff = PixelFarm.Agg.Transform.Affine.NewMatix(
                 PixelFarm.Agg.Transform.AffinePlan.Scale(1, -1),
                 PixelFarm.Agg.Transform.AffinePlan.Translate(0, 600));
            lionVxs = aff.TransformToVxs(lionShape.Path.Vxs);
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            int j = lionShape.NumPaths;
            int[] pathList = lionShape.PathIndexList;
            ColorRGBA[] colors = lionShape.Colors;
            //graphics2D.UseSubPixelRendering = true; 
            var myvxs = lionVxs;
            for (int i = 0; i < j; ++i)
            {
                canvas2d.FillVxsSnap(ToPixelFarmColor(colors[i]), new VertexStoreSnap(myvxs, pathList[i]));
            }
            //-------------------------------
            miniGLControl.SwapBuffers();
        }
        static PixelFarm.Drawing.Color ToPixelFarmColor(ColorRGBA c)
        {
            return PixelFarm.Drawing.Color.FromArgb(
                c.alpha,
                c.red,
                c.green, c.blue);
        }
    }
}

