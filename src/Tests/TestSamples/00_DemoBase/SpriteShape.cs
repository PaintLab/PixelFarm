//BSD, 2014-present, WinterDev
//MattersHackers
//AGG 2.4

using PixelFarm.Drawing;
using PixelFarm.VectorMath;
using PaintLab.Svg;
using LayoutFarm;
namespace PixelFarm.CpuBlit
{

    public class SpriteShape
    {
        VgRenderVx _svgRenderVx;
        byte _alpha;
        Vector2 _center;
        RectD _boundingRect;
        CpuBlit.VertexProcessing.Affine _currentTx;

        public SpriteShape(VgRenderVx svgRenderVx)//, RootGraphic root, int w, int h)
                                                  //: base(root, w, h)
        {
            LoadFromSvg(svgRenderVx);
        }
        public RectD Bounds
        {
            get
            {
                return _boundingRect;
            }
        }
        public void ResetTransform()
        {
            _currentTx = null;
        }
        public void ApplyTransform(CpuBlit.VertexProcessing.Affine tx)
        {
            //apply transform to all part
            if (_currentTx == null)
            {
                _currentTx = tx;
            }
            else
            {
                //ORDER is IMPORTANT
                _currentTx = _currentTx * tx;
            }
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
                return _center;
            }
        }
        public VgRenderVx GetRenderVx()
        {
            return _svgRenderVx;
        }

        public void ApplyNewAlpha(byte alphaValue0_255)
        {
            _alpha = alphaValue0_255;
        }
        public void Paint(Painter p)
        {
            VgPainterArgsPool.GetFreePainterArgs(p, out VgPaintArgs paintArgs);
            paintArgs._currentTx = _currentTx;
            _svgRenderVx._renderE.Paint(paintArgs);
            VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);
        }
        public void Paint(VgPaintArgs paintArgs)
        {
            _svgRenderVx._renderE.Paint(paintArgs);
        }
        public void Paint(Painter p, PixelFarm.CpuBlit.VertexProcessing.Perspective tx)
        {
            //TODO: implement this...
            //use prefix command for render vx
            //p.Render(_svgRenderVx);
            //_svgRenderVx.Render(p);
        }
        public void Paint(Painter p, PixelFarm.CpuBlit.VertexProcessing.Affine tx)
        {
            //TODO: implement this...
            //use prefix command for render vx 
            //------
            VgPainterArgsPool.GetFreePainterArgs(p, out VgPaintArgs paintArgs);
            paintArgs._currentTx = tx;
            paintArgs.ExternalVxsVisitHandler = (vxs, painterA) =>
            {
                //use external painter handler
                //draw only outline with its fill-color.
                Drawing.Painter m_painter = paintArgs.P;
                Drawing.Color prevFillColor = m_painter.FillColor;
                m_painter.FillColor = m_painter.FillColor;
                m_painter.Fill(vxs);
                m_painter.FillColor = prevFillColor;
            };
            _svgRenderVx._renderE.Paint(paintArgs);
            VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);

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
            _center.x = (_boundingRect.Right - _boundingRect.Left) / 2.0;
            _center.y = (_boundingRect.Top - _boundingRect.Bottom) / 2.0;
        }
        public void UpdateBounds()
        {
            _svgRenderVx.InvalidateBounds();
            this._boundingRect = _svgRenderVx.GetBounds();
        }
        public void HitTestOnSubPart(SvgHitChain hitChain)
        {
            _svgRenderVx._renderE.HitTest(hitChain);
        }

        //public override void ResetRootGraphics(RootGraphic rootgfx)
        //{
        //    DirectSetRootGraphics(this, rootgfx);
        //}

        //public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        //{
        //    throw new System.NotImplementedException();
        //}
    }



    public static class VgHitChainPool
    {
        //
        //
        [System.ThreadStatic]
        static System.Collections.Generic.Stack<SvgHitChain> s_hitChains = new System.Collections.Generic.Stack<SvgHitChain>();

        public static void GetFreeHitTestChain(out SvgHitChain hitTestArgs)
        {
            if (s_hitChains.Count > 0)
            {
                hitTestArgs = s_hitChains.Pop();
            }
            else
            {
                hitTestArgs = new SvgHitChain();
            }
        }
        public static void ReleaseHitTestChain(ref SvgHitChain hitTestArgs)
        {
            hitTestArgs.Clear();
            s_hitChains.Push(hitTestArgs);
            hitTestArgs = null;
        }
    }
}