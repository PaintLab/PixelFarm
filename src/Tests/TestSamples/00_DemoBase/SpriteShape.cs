//BSD, 2014-present, WinterDev
//MattersHackers
//AGG 2.4

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.VectorMath;

namespace PixelFarm.CpuBlit
{


    //TODO: review here again***
    //move to SVG or renderVX

    public class SpriteShape
    {
        VgRenderVx _org;
        VgRenderVx _svgRenderVx;
        PathWriter path = new PathWriter();
        Vector2 center;
        RectD boundingRect;
        public SpriteShape(VgRenderVx svgRenderVx)
        {
            _svgRenderVx = svgRenderVx;
            //create a copy 
            _org = svgRenderVx.Clone();
        }

        public RectD Bounds
        {
            get
            {
                return boundingRect;
            }
        }
        public void ResetTransform()
        {
            _svgRenderVx = _org.Clone();
        }
        public void ApplyTransform(CpuBlit.VertexProcessing.Affine tx)
        {
            //int elemCount = _svgRenderVx.SvgVxCount;
            //for (int i = 0; i < elemCount; ++i)
            //{
            //    _svgRenderVx.SetInnerVx(i, SvgCmd.TransformToNew(_svgRenderVx.GetInnerVx(i), tx));
            //}
        }

        public void ApplyTransform(CpuBlit.VertexProcessing.Bilinear tx)
        {
            //int elemCount = _svgRenderVx.SvgVxCount;
            //for (int i = 0; i < elemCount; ++i)
            //{
            //    _svgRenderVx.SetInnerVx(i, SvgCmd.TransformToNew(_svgRenderVx.GetInnerVx(i), tx));
            //}
        }
        public Vector2 Center
        {
            get
            {
                return center;
            }
        }
        public VgRenderVx GetRenderVx()
        {
            return _svgRenderVx;
        }

        public void ApplyNewAlpha(byte alphaValue0_255)
        {
            //Temp fix,            
            throw new System.NotSupportedException();

            //int elemCount = _svgRenderVx.VgCmdCount;
            //for (int i = 0; i < elemCount; ++i)
            //{
            //    VgCmd vx = _svgRenderVx.GetVgCmd(i);
            //    switch (vx.Name)
            //    {
            //        case VgCommandName.FillColor:
            //            {
            //                VgCmdFillColor fillColor = (VgCmdFillColor)vx;
            //                fillColor.Color = fillColor.Color.NewFromChangeAlpha(alphaValue0_255);
            //            }
            //            break;
            //        case VgCommandName.StrokeColor:
            //            {
            //                VgCmdStrokeColor strokColor = (VgCmdStrokeColor)vx;
            //                strokColor.Color = strokColor.Color.NewFromChangeAlpha(alphaValue0_255);
            //            }
            //            break;
            //    }
            //}
        }
        public void Paint(Painter p)
        {
            p.Render(_svgRenderVx);

        }

        public void Paint(Painter p, PixelFarm.CpuBlit.VertexProcessing.Perspective tx)
        {
            //TODO: implement this...
            //use prefix command for render vx
            p.Render(_svgRenderVx);
            //_svgRenderVx.Render(p);
        }
        public void Paint(Painter p, PixelFarm.CpuBlit.VertexProcessing.Affine tx)
        {
            //TODO: implement this...
            //use prefix command for render vx
        }
        public void DrawOutline(Painter p)
        {
            //walk all parts and draw only outline 
            //not fill
            //int renderVxCount = _svgRenderVx.VgCmdCount;
            //for (int i = 0; i < renderVxCount; ++i)
            //{

            //}



            //int j = lionShape.NumPaths;
            //int[] pathList = lionShape.PathIndexList;
            //Drawing.Color[] colors = lionShape.Colors;

            //var vxs = GetFreeVxs();
            //var vxs2 = stroke1.MakeVxs(affTx.TransformToVxs(lionShape.Vxs, vxs), GetFreeVxs());
            //for (int i = 0; i < j; ++i)
            //{
            //    p.StrokeColor = colors[i];
            //    p.Draw(new PixelFarm.Drawing.VertexStoreSnap(vxs2, pathList[i]));

            //}
            ////not agg   
            //Release(ref vxs);
            //Release(ref vxs2);
            //return; //**

        }

        public void LoadFromSvg(VgRenderVx svgRenderVx)
        {
            _svgRenderVx = svgRenderVx;
            UpdateBounds();
            //find center 
            center.x = (boundingRect.Right - boundingRect.Left) / 2.0;
            center.y = (boundingRect.Top - boundingRect.Bottom) / 2.0;
        }

        public void UpdateBounds()
        {
            //find bound
            //TODO: review here
            int partCount = _svgRenderVx.VgCmdCount;
            RectD rectTotal = new RectD();
            for (int i = 0; i < partCount; ++i)
            {
                VgCmd vx = _svgRenderVx.GetVgCmd(i);
                if (vx.Name != VgCommandName.Path)
                {
                    continue;
                }
                VgCmdPath path = (VgCmdPath)vx;
                BoundingRect.GetBoundingRect(new VertexStoreSnap(path.Vxs), ref rectTotal);
            }
            this.boundingRect = rectTotal;
        }

        VertexStore _selectedVxs = null;
        public bool HitTestOnSubPart(float x, float y)
        {
            throw new System.NotSupportedException();

            int partCount = _svgRenderVx.VgCmdCount;

            _selectedVxs = null;//reset
            for (int i = partCount - 1; i >= 0; --i)
            {
                //we do hittest top to bottom => (so => iter backward)

                VgCmd vx = _svgRenderVx.GetVgCmd(i);
                if (vx.Name != VgCommandName.Path)
                {
                    continue;
                }
                VgCmdPath path = (VgCmdPath)vx;
                VertexStore innerVxs = path.Vxs;
                //fine tune
                //hit test ***
                if (VertexHitTester.IsPointInVxs(innerVxs, x, y))
                {

                    if (_selectedVxs != null)
                    {
                        //de-selected this first
                    }
                    _selectedVxs = innerVxs;
                    //vx.FillColor = Color.Black;
                    return true;
                }
            }
            return false;
        }

    }
}