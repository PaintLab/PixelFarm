//BSD, 2014-2018, WinterDev
//MattersHackers
//AGG 2.4

using PixelFarm.Drawing;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;
using PaintLab.Svg;

using System.Text;

namespace PixelFarm.Agg
{


    //TODO: review here again***
    //move to SVG or renderVX

    public class SpriteShape
    {

        SvgRenderVx _svgRenderVx;
        PathWriter path = new PathWriter();
        Vector2 center;
        RectD boundingRect;

        public SpriteShape(SvgRenderVx svgRenderVx)
        {
            _svgRenderVx = svgRenderVx;
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
            int elemCount = _svgRenderVx.SvgVxCount;
            for (int i = 0; i < elemCount; ++i)
            {
                _svgRenderVx.ResetTransform();
            }
        }
        public void ApplyTransform(Agg.Transform.Affine tx)
        {
            int elemCount = _svgRenderVx.SvgVxCount;
            for (int i = 0; i < elemCount; ++i)
            {
                _svgRenderVx.SetInnerVx(i, SvgPart.TransformToNew(_svgRenderVx.GetInnerVx(i), tx));
            }
        }

        public void ApplyTransform(Agg.Transform.Bilinear tx)
        {
            int elemCount = _svgRenderVx.SvgVxCount;
            for (int i = 0; i < elemCount; ++i)
            {
                _svgRenderVx.SetInnerVx(i, SvgPart.TransformToNew(_svgRenderVx.GetInnerVx(i), tx));
            }
        }
        public Vector2 Center
        {
            get
            {
                return center;
            }
        }
        public SvgRenderVx GetRenderVx()
        {
            return _svgRenderVx;
        }

        public void ApplyNewAlpha(byte alphaValue0_255)
        {
            //Temp fix,            

            int elemCount = _svgRenderVx.SvgVxCount;
            for (int i = 0; i < elemCount; ++i)
            {
                SvgPart vx = _svgRenderVx.GetInnerVx(i);
                if (vx.HasFillColor)
                {
                    vx.FillColor = vx.FillColor.NewFromChangeAlpha(alphaValue0_255);
                }
                if (vx.HasStrokeColor)
                {
                    vx.StrokeColor = vx.StrokeColor.NewFromChangeAlpha(alphaValue0_255);
                }
            }
        }
        public void Paint(Painter p)
        {
            _svgRenderVx.Render(p);
        }

        public void Paint(Painter p, PixelFarm.Agg.Transform.Perspective tx)
        {
            _svgRenderVx.Render(p);
        }
        public void Paint(Painter p, PixelFarm.Agg.Transform.Affine tx)
        {

        }
        public void DrawOutline(Painter p)
        {
            //walk all parts and draw only outline 
            //not fill
            int renderVxCount = _svgRenderVx.SvgVxCount;
            for (int i = 0; i < renderVxCount; ++i)
            {

            }
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

        public void LoadFromSvg(SvgRenderVx svgRenderVx)
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
            int partCount = _svgRenderVx.SvgVxCount;
            RectD rectTotal = new RectD();
            for (int i = 0; i < partCount; ++i)
            {
                SvgPart vx = _svgRenderVx.GetInnerVx(i);
                if (vx.Kind != SvgRenderVxKind.Path)
                {
                    continue;
                }
                VertexStore innerVxs = vx.GetVxs();
                PixelFarm.Agg.BoundingRect.GetBoundingRect(new VertexStoreSnap(innerVxs), ref rectTotal);
            }
            this.boundingRect = rectTotal;
        }

        VertexStore _selectedVxs = null;
        public bool HitTestOnSubPart(float x, float y)
        {
            int partCount = _svgRenderVx.SvgVxCount;

            _selectedVxs = null;//reset
            for (int i = partCount - 1; i >= 0; --i)
            {
                //we do hittest top to bottom => (so => iter backward)

                SvgPart vx = _svgRenderVx.GetInnerVx(i);
                if (vx.Kind != SvgRenderVxKind.Path)
                {
                    continue;
                }
                VertexStore innerVxs = vx.GetVxs();
                //fine tune
                //hit test ***
                if (VertexHitTester.IsPointInVxs(innerVxs, x, y))
                {

                    if (_selectedVxs != null)
                    {
                        //de-selected this first
                    }


                    _selectedVxs = innerVxs;
                    vx.FillColor = Color.Black;
                    return true;
                }
            }
            return false;
        }

    }
}