//BSD, 2014-present, WinterDev
//MattersHackers
//AGG 2.4

using PixelFarm.Drawing;
using PixelFarm.VectorMath;
using PixelFarm.CpuBlit.VertexProcessing;
using PaintLab.Svg;

namespace PixelFarm.CpuBlit
{

    public class SpriteShape
    {
        VgVisualElement _vgVisElem;
        byte _alpha;
        Vector2 _center;
        Q1RectD _boundingRect;
        CpuBlit.VertexProcessing.ICoordTransformer _currentTx;

        public SpriteShape(VgVisualElement vgVisElem)//, RootGraphic root, int w, int h)
                                                     //: base(root, w, h)
        {
            LoadFromSvg(vgVisElem);
        }
        public Q1RectD Bounds=>_boundingRect;           
        
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
                _currentTx = _currentTx.MultiplyWith(tx);
                //if (_currentTx is CpuBlit.VertexProcessing.Affine)
                //{
                //    _currentTx = ((CpuBlit.VertexProcessing.Affine)_currentTx) * tx;
                //}
                //else if (_currentTx is CpuBlit.VertexProcessing.Perspective)
                //{
                //    _currentTx = ((CpuBlit.VertexProcessing.Perspective)_currentTx) * tx;
                //}
                //else
                //{

                //}

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
        //public void ApplyTransform(CpuBlit.VertexProcessing.Perspective tx)
        //{
        //    if (_currentTx == null)
        //    {
        //        _currentTx = tx;
        //    }
        //    else
        //    {
        //        //ORDER is IMPORTANT
        //        _currentTx = _currentTx.MultiplyWith(tx);
        //        //if (_currentTx is CpuBlit.VertexProcessing.Affine)
        //        //{
        //        //    _currentTx = ((CpuBlit.VertexProcessing.Affine)_currentTx) * tx;
        //        //}
        //        //else if (_currentTx is CpuBlit.VertexProcessing.Perspective)
        //        //{
        //        //    _currentTx = ((CpuBlit.VertexProcessing.Perspective)_currentTx) * tx;
        //        //}
        //        //else
        //        //{

        //        //}

        //    }
        //}

        public Vector2 Center => _center;

        public VgVisualElement GetVgVisualElem() => _vgVisElem;

        public void ApplyNewAlpha(byte alphaValue0_255)
        {
            _alpha = alphaValue0_255;
        }
        public void Paint(Painter p)
        {
            using (Tools.More.BorrowVgPaintArgs(p, out var paintArgs))
            {
                paintArgs._currentTx = _currentTx;
                _vgVisElem.Paint(paintArgs);
            }
        }
        public void Paint(VgPaintArgs paintArgs)
        {
            _vgVisElem.Paint(paintArgs);
        }
        public void Paint(Painter p, Bilinear tx)
        {
            //in this version, I can't apply bilinear tx to current tx matrix

            using (Tools.More.BorrowVgPaintArgs(p, out var paintArgs))
            {
                paintArgs.PaintVisitHandler = (vxs, painterA) =>
                {
                    //use external painter handler
                    //draw only outline with its fill-color.
                    Drawing.Painter m_painter = painterA.P;
                    Drawing.Color prevFillColor = m_painter.FillColor;

                    m_painter.FillColor = m_painter.FillColor;

                    using (Tools.BorrowVxs(out var v1))
                    {
                        tx.TransformToVxs(vxs, v1);
                        m_painter.Fill(v1);
                    }
                    m_painter.FillColor = prevFillColor;
                };
                _vgVisElem.Paint(paintArgs);
            }


        }
        public void Paint(Painter p, ICoordTransformer tx)
        {
            //TODO: implement this...
            //use prefix command for render vx 
            //------
            using (Tools.More.BorrowVgPaintArgs(p, out var paintArgs))
            {
                paintArgs._currentTx = tx;
                paintArgs.PaintVisitHandler = (vxs, arg) =>
                {
                    //use external painter handler
                    //draw only outline with its fill-color.
                    Drawing.Painter m_painter = arg.P;
                    Drawing.Color prevFillColor = m_painter.FillColor;
                    m_painter.FillColor = m_painter.FillColor;
                    m_painter.Fill(vxs);
                    m_painter.FillColor = prevFillColor;
                };
                _vgVisElem.Paint(paintArgs);
            }


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

        public void LoadFromSvg(VgVisualElement vgVisElem)
        {
            _vgVisElem = vgVisElem;
            UpdateBounds();
            //find center 
            _center.x = (_boundingRect.Right - _boundingRect.Left) / 2.0;
            _center.y = (_boundingRect.Top - _boundingRect.Bottom) / 2.0;
        }
        public void UpdateBounds()
        {
            _vgVisElem.InvalidateBounds();
            _boundingRect = _vgVisElem.GetRectBounds();
        }
        public void HitTestOnSubPart(VgHitChain hitChain)
        {
            _vgVisElem.HitTest(hitChain);
        }

    }



    public static class VgHitChainPool
    {
        //
        //
        [System.ThreadStatic]
        static System.Collections.Generic.Stack<VgHitChain> s_hitChains = new System.Collections.Generic.Stack<VgHitChain>();

        public static void GetFreeHitTestChain(out VgHitChain hitTestArgs)
        {
            if (s_hitChains.Count > 0)
            {
                hitTestArgs = s_hitChains.Pop();
            }
            else
            {
                hitTestArgs = new VgHitChain();
            }
        }
        public static void ReleaseHitTestChain(ref VgHitChain hitTestArgs)
        {
            hitTestArgs.Clear();
            s_hitChains.Push(hitTestArgs);
            hitTestArgs = null;
        }
    }
}