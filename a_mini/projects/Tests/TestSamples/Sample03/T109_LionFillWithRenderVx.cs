//MIT, 2014-2016,WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Agg;
namespace OpenTkEssTest
{
    [Info(OrderCode = "109")]
    [Info("T109_LionFillWithRenderVx")]
    public class T109_LionFillWithRenderVx : DemoBase
    {
        GLRenderSurface _glsf;
        SpriteShape lionShape;
        VertexStore lionVxs;
        GLPainter painter;
        List<RenderVx> lionRenderVxList = new List<RenderVx>();
        int tmpDrawVersion = 0;
        MultiPartTessResult multipartTessResult;

        protected override void OnGLSurfaceReady(GLRenderSurface glsf, GLPainter painter)
        {
            this._glsf = glsf;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            int max = Math.Max(this.Width, this.Height);

            lionShape = new SpriteShape();
            lionShape.ParseLion();
            //flip this lion vertically before use with openGL
            PixelFarm.Agg.Transform.Affine aff = PixelFarm.Agg.Transform.Affine.NewMatix(
                 PixelFarm.Agg.Transform.AffinePlan.Scale(1, -1),
                 PixelFarm.Agg.Transform.AffinePlan.Translate(0, 600));
            lionVxs = new VertexStore();
            aff.TransformToVxs(lionShape.Path.Vxs, lionVxs);
            //convert lion vxs to renderVx

            ////-------------
            ////version 1:
            //int j = lionShape.NumPaths;
            //int[] pathList = lionShape.PathIndexList;
            //for (int i = 0; i < j; ++i)
            //{
            //    lionRenderVxList.Add(painter.CreateRenderVx(new VertexStoreSnap(lionVxs, pathList[i])));
            //}
            ////------------- 
            //version 2:
            {
                tmpDrawVersion = 2;
                MultiPartPolygon mutiPartPolygon = new MultiPartPolygon();
                int j = lionShape.NumPaths;
                int[] pathList = lionShape.PathIndexList;
                Color[] colors = lionShape.Colors;
                for (int i = 0; i < j; ++i)
                {
                    //from lionvxs extract each part                      
                    //fetch data and add to multipart polygon
                    //if (i != 4) continue;
                    //if (i > 1)
                    //{
                    //    break;
                    //}
                    mutiPartPolygon.AddVertexSnap(new VertexStoreSnap(lionVxs, pathList[i]));
                }
                //then create single render vx
                this.multipartTessResult = painter.CreateMultiPartTessResult(mutiPartPolygon);
                //create render vx for the multipart test result                 
            }

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
            if (tmpDrawVersion == 2)
            {

                Color[] colors = lionShape.Colors;
                if (multipartTessResult != null)
                {
                    int j = multipartTessResult.PartCount;
                    for (int i = 0; i < j; ++i)
                    {
                        _glsf.FillRenderVx(colors[i], multipartTessResult, i);
                    }
                }

            }
            else
            {
                int j = lionRenderVxList.Count;
                Color[] colors = lionShape.Colors;
                for (int i = 0; i < j; ++i)
                {
                    _glsf.FillRenderVx(colors[i], lionRenderVxList[i]);
                }
            }
            //-------------------------------
            SwapBuffers();
        }
    }
}

